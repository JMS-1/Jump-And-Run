using System;
using System.Collections.Generic;
using System.Linq;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Beschreibt ein einzelnes Element auf dem Spielfeld.
    /// </summary>
    public abstract class GrundElement : Fläche
    {
        /// <summary>
        /// Legt fest, ob dieses Element der Schwerkraft folgt.
        /// </summary>
        protected abstract bool SchwerkraftNutzen { get; }

        /// <summary>
        /// Wird ausgelöst, nachdem sich das Element bewegt hat.
        /// </summary>
        public event Action<GrundElement> ElementHatSichBewegt;

        /// <summary>
        /// Alle Regeln zu diesem Element.
        /// </summary>
        private List<ElementRegel> m_regeln = new List<ElementRegel>();

        /// <summary>
        /// Das zugehörige Spielfeld.
        /// </summary>
        private Simulation m_spielfeld;

        /// <summary>
        /// Die Art, wie sich das Element bewegt.
        /// </summary>
        private ElementBewegung m_bewegung;

        /// <summary>
        /// Die Art, wie sich das Element bewegt.
        /// </summary>
        public ElementBewegung Bewegung
        {
            get
            {
                // Melden
                return m_bewegung;
            }
            set
            {
                // Es hat sich nichts verändert
                if (value == m_bewegung)
                    return;

                // Ändern
                m_bewegung = value;

                // An alle melden, die es interessiert
                ZustandWurdeVerändert();
            }
        }

        /// <summary>
        /// Wird ausgelöst, wenn sich der Zustan des Elementes verändert hat.
        /// </summary>
        public event Action<GrundElement> ZustandVerändert;

        /// <summary>
        /// Der Name dieses Elementes.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gesetzt, wenn das Element nicht angezeigt oder sonstwie berücksichtigt wird.
        /// </summary>
        private bool m_deaktiviert;

        /// <summary>
        /// Meldet, ob das Element deaktiviert wurde.
        /// </summary>
        public bool IstDeaktiviert
        {
            get
            {
                // Melden
                return m_deaktiviert;
            }
            internal set
            {
                // Es gibt keine Änderung
                if (value == m_deaktiviert)
                    return;

                // Ändern
                m_deaktiviert = value;

                // An alle melden, die es interessiert
                ZustandWurdeVerändert();
            }
        }

        /// <summary>
        /// Meldet eine Zustandsveränderung an alle Interessenten.
        /// </summary>
        protected virtual void ZustandWurdeVerändert()
        {
            // An alle melden, die es interessiert
            var interessenten = ZustandVerändert;
            if (interessenten != null)
                interessenten( this );
        }

        /// <summary>
        /// Wird aufgerufen, sobald dieses Element einem Spielfeld zugeordnet wurde.
        /// </summary>
        public event Action<GrundElement> SpielfeldZugeordnet;

        /// <summary>
        /// Wird aufgerufen, sobald ein Element Teil eines Spielfelds ist.
        /// </summary>
        protected virtual void SpielfeldWurdeZugeordnet()
        {
            // Erdanziehung ergänzen
            if (SchwerkraftNutzen)
                GeschwindigkeitsRegel.GeschwindigkeitErgänzen( Spielfeld.FallRegel );

            // Sonstige interessenten
            var interessenten = SpielfeldZugeordnet;
            if (interessenten != null)
                interessenten( this );
        }

        /// <summary>
        /// Meldet das zugehörige Spielfeld.
        /// </summary>
        public Simulation Spielfeld
        {
            get
            {
                // Melden
                return m_spielfeld;
            }
            internal set
            {
                // Das geht nur einmal
                if (!ReferenceEquals( m_spielfeld, null ))
                    throw new NotSupportedException( "Spielfeld" );

                // Merken
                m_spielfeld = value;

                // Abgeleitete Klassen informieren
                SpielfeldWurdeZugeordnet();
            }
        }

        /// <summary>
        /// Erstellt ein neues Element.
        /// </summary>
        /// <param name="position">Die Position des Elementes auf dem Spielfeld.</param>
        /// <param name="ausdehnung">Die relative Größe des Elementes.</param>
        /// <param name="initialDeaktiviert">Gesetzt, wenn das Element bei Beginn nicht berücksichtigt wird.</param>
        /// <param name="name">Der optionale Name dieses Elementes.</param>
        internal GrundElement( Position position, Ausdehnung ausdehnung, bool initialDeaktiviert, string name )
            : base( position, ausdehnung )
        {
            // Initialer Zustand
            IstDeaktiviert = initialDeaktiviert;
            Name = name;

            // Abschliessen
            RegelAnmelden( new GeschwindigkeitsRegel() );
            RegelAnmelden( new KollisionsRegel( this ) );
        }

        /// <summary>
        /// Ergänzt eine Regel.
        /// </summary>
        /// <param name="regel">Die neue Regel.</param>
        public void RegelAnmelden( ElementRegel regel )
        {
            // Prüfen
            if (regel == null)
                throw new ArgumentNullException( "regel" );

            // Merken
            m_regeln.Add( regel );
        }

        /// <summary>
        /// Ermittelt alle Regeln einer bestimmten Art.
        /// </summary>
        /// <typeparam name="TRegelArt">Die Art der Regel.</typeparam>
        /// <returns>Die gewünschte Liste.</returns>
        public IEnumerable<TRegelArt> FindRegeln<TRegelArt>() where TRegelArt : ElementRegel
        {
            // Durchreichen
            return m_regeln.OfType<TRegelArt>();
        }

        /// <summary>
        /// Meldet die einzige Geschwindigkeitsregel.
        /// </summary>
        public GeschwindigkeitsRegel GeschwindigkeitsRegel { get { return FindRegeln<GeschwindigkeitsRegel>().Single(); } }

        /// <summary>
        /// Meldet die einzige Kollisionsregel.
        /// </summary>
        public KollisionsRegel KollisionsRegel { get { return FindRegeln<KollisionsRegel>().Single(); } }

        /// <summary>
        /// Bewegt das verwaltete Element.
        /// </summary>
        /// <param name="abstandX">Die horizontale Differenz.</param>
        /// <param name="abstandY">Die vertikale Differenz.</param>
        public void Bewegen( GenaueZahl abstandX, GenaueZahl abstandY )
        {
            // Bewegung ausführen
            Bewegen( Position.SicherErzeugen( Position.HorizontalePosition + abstandX, Position.VertikalePosition + abstandY ) );
        }

        /// <summary>
        /// Bewegt das verwaltete Element.
        /// </summary>
        /// <param name="neuePosition">Die Zielposition.</param>
        public void Bewegen( Position neuePosition )
        {
            // Wir wissen nichts
            var bewegung = ElementBewegung.Ruht;

            // Horizontal prüfen
            switch (Math.Sign( neuePosition.HorizontalePosition.CompareTo( Position.HorizontalePosition ) ))
            {
                case +1: bewegung |= ElementBewegung.LäuftNachRechts; break;
                case -1: bewegung |= ElementBewegung.LäuftNachLinks; break;
            }

            // Vertikal prüfen
            switch (Math.Sign( neuePosition.VertikalePosition.CompareTo( Position.VertikalePosition ) ))
            {
                case +1: bewegung |= ElementBewegung.SpringtNachOben; break;
                case -1: bewegung |= ElementBewegung.FälltNachUnten; break;
            }

            // Änderung durchreichen
            Bewegung = bewegung;

            // Die Position wurde gar nicht verändert
            if (bewegung == ElementBewegung.Ruht)
                return;

            // Neue Position merken
            Verschieben( neuePosition );

            // Benachrichtigung auslösen
            BewegungWurdeAusgeführt();
        }

        /// <summary>
        /// Teilt allen Interessenten mit, dass eine Bewegung ausgeführt wurde.
        /// </summary>
        protected virtual void BewegungWurdeAusgeführt()
        {
            // Benachrichtigung auslösen
            var interessenten = ElementHatSichBewegt;
            if (interessenten != null)
                interessenten( this );
        }

        /// <summary>
        /// Teilt diesem Element mit, dass es sich relativ zur Spielfigur bewegen soll.
        /// </summary>
        /// <param name="horizontaleGeschwindigkeit">Die horizontale Geschwindigkeit der Bewegung relativ zur
        /// einfachen Geschwindigkeit der Spielfigur.</param>
        /// <param name="aufDenSpielerZubewegen">Gesetzt, wenn das Element sich auf die Spielfigur zubewegen soll.</param>
        public void BewegungRelativZumSpielerAktivieren( int horizontaleGeschwindigkeit, bool aufDenSpielerZubewegen )
        {
            // Nächste Anmeldung
            Action<Geschwindigkeit> neuBerechnen = null;

            // Konfigurieren
            neuBerechnen = g =>
            {
                // Geschwindigkeit ermitteln
                var geschwindigkeit = GenaueZahl.Null;
                var spielzeit = TimeSpan.Zero;
                var spielfeld = Spielfeld;
                if (spielfeld != null)
                {
                    // Wir können nun die Spielzeit auslesen
                    spielzeit = spielfeld.VerbrauchteZeit;

                    // Spieler suchen und Richtung ermitteln
                    var spieler = spielfeld.Elemente.OfType<Spieler>().FirstOrDefault();
                    if (spieler != null)
                        if (horizontaleGeschwindigkeit > 0)
                        {
                            // Geschwindigkeit setzen
                            var differenz = spieler.Position.HorizontalePosition.CompareTo( Position.HorizontalePosition );
                            if (differenz != 0)
                            {
                                // Geschwindigkeit ermitteln - ein positiver Wert ist eine Bewegung nach rechts
                                geschwindigkeit = spieler.EinfacheGeschwindigkeit * horizontaleGeschwindigkeit;

                                // Wenn wir uns dem Spieler nähern sollen und dieser links von uns ist müssen wir in die andere Richtung laufen - und umgekehrt
                                if (aufDenSpielerZubewegen == (differenz < 0))
                                    geschwindigkeit = -geschwindigkeit;
                            }
                        }
                }

                // Geschwindigkeitsregel erzeugen
                var spielerAnpeilen = TemporaereGeschwindigkeit.Erzeugen( geschwindigkeit, GenaueZahl.Null, spielzeit + TimeSpan.FromMilliseconds( 20 ) );

                // Überwachung einrichten
                spielerAnpeilen.Deaktiviert += neuBerechnen;

                // Anmelden
                GeschwindigkeitsRegel.GeschwindigkeitErgänzen( spielerAnpeilen );
            };

            // Erstmalig einrichten
            neuBerechnen( null );
        }
    }
}
