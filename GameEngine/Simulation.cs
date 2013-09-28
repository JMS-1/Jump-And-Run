using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Bezeichnet eine einzelne Spielebene.
    /// </summary>
    public class Simulation
    {
        /// <summary>
        /// Die Elemente auf dem Spielfeld.
        /// </summary>
        private readonly List<GrundElement> m_elemente = new List<GrundElement>();

        /// <summary>
        /// Meldet, ob die Spielsimulation aktiv ist.
        /// </summary>
        private volatile SimulationsStand m_stand = SimulationsStand.Angehalten;

        /// <summary>
        /// Meldet, ob die Spielsimulation aktiv ist.
        /// </summary>
        public SimulationsStand Status { get { return m_stand; } }

        /// <summary>
        /// Wird ausgelöst, wenn sich das Spiel zwischen der Simulation und dem 
        /// Wartezustand wechselt.
        /// </summary>
        public event Action<Simulation> SpielZustandHatSichVeraendert;

        /// <summary>
        /// Die gesamte Laufzeit der Simulation bis zum vorherigen Beenden.
        /// </summary>
        private TimeSpan m_zeitBasis;

        /// <summary>
        /// Der Zeitpunkt, an dem das Spiel das letzte Mal gestartet wurde.
        /// </summary>
        private DateTime m_letzerStart;

        /// <summary>
        /// Wird ständig aufgerufen, um die Spielsimulation auszuführen.
        /// </summary>
        private Timer m_ticker;

        /// <summary>
        /// Der Zeitpunkt, zu dem letztmalig ein Simulationsschritt ausgeführt wurde.
        /// </summary>
        private TimeSpan m_letzteAusfuehrung;

        /// <summary>
        /// Die aktuelle Fallgeschwindigkeit in diesem Level.
        /// </summary>
        public Geschwindigkeit FallRegel { get; private set; }

        /// <summary>
        /// Liest oder setzt die aktuelle Fallgeschwindigkeits des Levels. Die Voreinstellung
        /// ist <i>0.1</i>, i.e. im ein Fall über die gesamte Höhe benötigt 10 Sekunden.
        /// </summary>
        public GenaueZahl FallGeschwindigkeit
        {
            get
            {
                // Melden
                return -FallRegel.VertikaleGeschwindigkeit;
            }
            set
            {
                // Ändern
                FallRegel = Geschwindigkeit.Erzeugen( GenaueZahl.Null, -value );
            }
        }

        /// <summary>
        /// Erzeugt ein neues Spielfeld.
        /// </summary>
        public Simulation()
        {
            // Voreinstellungen setzen
            FallGeschwindigkeit = GenaueZahl.Eins / 10;
        }

        /// <summary>
        /// Meldet eine Auflistung aller Elemente.
        /// </summary>
        public IEnumerable<GrundElement> Elemente { get { return m_elemente.AsReadOnly(); } }

        /// <summary>
        /// Wird ständig aufgerufen, um die Simulation durchzuführen.
        /// </summary>
        /// <param name="info">Wird ignoriert.</param>
        private void SimulationAusführen( object info )
        {
            // Nicht aktiv
            if (m_stand != SimulationsStand.Läuft)
                return;

            // Zu jedem Zeitpunkt darf die Simulation nur einmal laufen
            if (!Monitor.TryEnter( info ))
                return;

            // Saubere Freigabe
            try
            {
                // Aktuelle Spielzeit auslesen
                var zeitAbsolut = VerbrauchteZeit;
                var alteZeitAbsolut = m_letzteAusfuehrung;
                var zeitRelativ = zeitAbsolut - alteZeitAbsolut;
                if (zeitRelativ.Ticks <= 0)
                    return;

                // Spielzeit merken
                m_letzteAusfuehrung = zeitAbsolut;

                // Sonderregeln ausführen
                GeschwindigkeitsRegel.Ausführen( this, zeitAbsolut, zeitRelativ );

                // Schauen wir einmal, ob das jemanden interessiert
                var interessenten = Zeitgeber;
                if (interessenten != null)
                    interessenten( this, zeitAbsolut );
            }
            finally
            {
                // Der nächste, bitte
                Monitor.Exit( info );
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich der Ablauf verändert hat.
        /// </summary>
        private void SpielZustandVerändert()
        {
            // Mal schauen, ob das jemanden interessiert
            var interessenten = SpielZustandHatSichVeraendert;
            if (interessenten != null)
                interessenten( this );
        }

        /// <summary>
        /// Ergänzt ein einzelnes Element.
        /// </summary>
        /// <param name="element">Ein Element, das auf diesem Spielfeld eingesetzt werden soll.</param>
        /// <exception cref="ArgumentNullException">Es wurde kein Element angegeben.</exception>
        public void ElementHinzufügen( GrundElement element )
        {
            // Prüfen
            if (element == null)
                throw new ArgumentNullException( "element" );

            // Verbinden
            element.Spielfeld = this;

            // Merken
            m_elemente.Add( element );
        }

        /// <summary>
        /// Wird bei Veränderung der Zeitbasis aufgerufen.
        /// </summary>
        public event Action<Simulation, TimeSpan> Zeitgeber;

        /// <summary>
        /// Die bisher in der Simulation verbrachte Spielzeit.
        /// </summary>
        public TimeSpan VerbrauchteZeit
        {
            get
            {
                // Aufsummierte Zeit
                var gesamt = m_zeitBasis;

                // Wenn das Spiel läfut, kommt da noch die aktuelle Zeit dazu
                if (m_stand == SimulationsStand.Läuft)
                    gesamt += DateTime.UtcNow - m_letzerStart;

                // Melden
                return gesamt;
            }
        }

        /// <summary>
        /// Beginnt mit der Spielsimulation.
        /// </summary>
        public void StartenOderFortsetzen()
        {
            // Nur, wenn wir nicht aktiv sind
            if (m_stand != SimulationsStand.Angehalten)
                return;

            // Startzeit dieses Laufs
            m_letzerStart = DateTime.UtcNow;

            // Zustand ändern
            m_stand = SimulationsStand.Läuft;

            // Zeitgeber starten
            using (m_ticker)
                m_ticker = new Timer( SimulationAusführen, new object(), TimeSpan.Zero, TimeSpan.FromMilliseconds( 10 ) );

            // Veränderung melden
            SpielZustandVerändert();
        }

        /// <summary>
        /// Beendet die Spielsimulation.
        /// </summary>
        /// <param name="ergebnis">Das Ergebnis des Spiels.</param>
        public void UnterbrechenOderBeenden( SimulationsStand ergebnis )
        {
            // Prüfen
            if (ergebnis == SimulationsStand.Läuft)
                throw new ArgumentException( "ergebnis" );

            // Nur, wenn wir  aktiv sind
            if (m_stand != SimulationsStand.Läuft)
                return;

            // Zeitgeber stoppen
            using (m_ticker)
                m_ticker = null;

            // Gesamtzeit ermitteln
            m_zeitBasis = VerbrauchteZeit;

            // Zustand ändern
            m_stand = ergebnis;

            // Veränderung melden
            SpielZustandVerändert();
        }
    }
}
