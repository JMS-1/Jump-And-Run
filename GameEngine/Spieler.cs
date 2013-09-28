using System;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Beschreibt die Spielfigur.
    /// </summary>
    public sealed class Spieler : GrundElement
    {
        /// <summary>
        /// Die normale Geschwindigkeit der Spielfigur, in der Voreinstellung werden 25 Sekunden benötigt
        /// um das gesamte Spielfeld zu durchqueren.
        /// </summary>
        private GenaueZahl m_einfacheGeschwindigkeit;

        /// <summary>
        /// Die normale Geschwindigkeit der Spielfigur, in der Voreinstellung werden 25 Sekunden benötigt
        /// um das gesamte Spielfeld zu durchqueren.
        /// </summary>
        public GenaueZahl EinfacheGeschwindigkeit
        {
            get
            {
                // Melden
                return m_einfacheGeschwindigkeit;
            }
            set
            {
                // Merken
                m_einfacheGeschwindigkeit = value;
            }
        }

        /// <summary>
        /// Gibt an, wie schnell der Spieler werden kann. In der Voreinstellung kann die einfache
        /// Geschwindigkeit verfünfacht werden.
        /// </summary>
        private int m_maximalerGeschwindigkeitsFaktor;

        /// <summary>
        /// Gibt an, wie schnell der Spieler werden kann. In der Voreinstellung kann die einfache
        /// Geschwindigkeit verfünfacht werden.
        /// </summary>
        public int MaximalerGeschwindigkeitsFaktor
        {
            get
            {
                // Melden
                return m_maximalerGeschwindigkeitsFaktor;
            }
            set
            {
                // Prüfen
                if (m_maximalerGeschwindigkeitsFaktor < 0)
                    throw new ArgumentOutOfRangeException( "MaximalerGeschwindigkeitsFaktor" );

                // Merken
                m_maximalerGeschwindigkeitsFaktor = value;

                // Abschneiden
                if (Math.Abs( m_aktuellerFaktor ) > m_maximalerGeschwindigkeitsFaktor)
                    m_aktuellerFaktor = Math.Sign( m_aktuellerFaktor ) * m_maximalerGeschwindigkeitsFaktor;
            }
        }

        /// <summary>
        /// Der aktuelle Geschwindigkeitsfaktor.
        /// </summary>
        private int m_aktuellerFaktor;

        /// <summary>
        /// Die aktuelle Geschwindigkeit.
        /// </summary>
        public Geschwindigkeit AktuelleGeschwindigkeit { get; private set; }

        /// <summary>
        /// Die maximal erlaubte Sprungstärke.
        /// </summary>
        private int m_maximalerSprung;

        /// <summary>
        /// Meldet oder legt fest, wieviele Sprünge in Folge maximal ausgeführt werden dürfen.
        /// </summary>
        public int MaximalerSprung
        {
            get
            {
                // Melden
                return m_maximalerSprung;
            }
            set
            {
                // Prüfen
                if (value < 0)
                    throw new ArgumentOutOfRangeException( "MaximalerSprung" );

                // Merken
                m_maximalerSprung = value;
            }
        }

        /// <summary>
        /// Die aktuelle Anzahl von aufeinanderfolgenden Sprüngen.
        /// </summary>
        private int m_aktuellerSprung;

        /// <summary>
        /// Die aktuellen Sprungstärke.
        /// </summary>
        private GenaueZahl m_sprungStärke;

        /// <summary>
        /// Meldet oder ändert die Stärke des Sprungs.
        /// </summary>
        public GenaueZahl SprungStärke
        {
            get
            {
                // Melden
                return m_sprungStärke;
            }
            set
            {
                // Prüfen
                if (value < GenaueZahl.Null)
                    throw new ArgumentOutOfRangeException( "SprungStärke" );

                // Ändern
                m_sprungStärke = value;
            }
        }

        /// <summary>
        /// Die Dauer eines Sprungs.
        /// </summary>
        private TimeSpan m_sprungDauer;

        /// <summary>
        /// Meldet ändert die Dauer des Sprungs.
        /// </summary>
        public TimeSpan SprungDauer
        {
            get
            {
                // Melden
                return m_sprungDauer;
            }
            set
            {
                // Prüfen
                if (value.Ticks <= 0)
                    throw new ArgumentOutOfRangeException( "SprungDauer" );

                // Merken
                m_sprungDauer = value;
            }
        }

        /// <summary>
        /// Legt fest, wann der nächste Sprung erlaubt ist.
        /// </summary>
        private TimeSpan m_nächsterErlaubterSprung;

        /// <summary>
        /// Bisher gesammelte Punkte.
        /// </summary>
        private uint m_punkte;

        /// <summary>
        /// Meldet die bisher gesammelten Punkte.
        /// </summary>
        public uint Punkte { get { return m_punkte; } }

        /// <summary>
        /// Wird ausgelöst, wenn sich die Anzahl der Punkte verändert hat.
        /// </summary>
        public event Action<Spieler> PunkteVerändert;

        /// <summary>
        /// Verändert die Anzahl der Punkte.
        /// </summary>
        /// <param name="punkteDifferenz">Die Differenz in Punkten - diese kann auch negativ sein.</param>
        public void PunkteSammeln( int punkteDifferenz )
        {
            // Neue Anzahl
            var punkte = checked( (uint) Math.Max( 0, (int) m_punkte + punkteDifferenz ) );
            if (punkte == m_punkte)
                return;

            // Merken
            m_punkte = punkte;

            // Schauen wir mal, ob das jemanden interessiert
            PunkteWurdenVerändert();
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich der Punktestand verändert hat.
        /// </summary>
        private void PunkteWurdenVerändert()
        {
            // Interessenten informieren
            var interessenten = PunkteVerändert;
            if (interessenten != null)
                interessenten( this );
        }

        /// <summary>
        /// Aktuelle Lebensenergie.
        /// </summary>
        private int m_energie;

        /// <summary>
        /// Die Schwächung durch die verstreichende Zeit.
        /// </summary>
        private int m_schwächung;

        /// <summary>
        /// Meldet die aktuelle Lebensenergie.
        /// </summary>
        public int Lebenskraft { get { return Math.Max( 0, m_energie - m_schwächung ); } }

        /// <summary>
        /// Wird ausgelöst, wenn sich die Lebensenergie verändert hat.
        /// </summary>
        public event Action<Spieler> LebenskraftVerändert;

        /// <summary>
        /// Verändert die Lebensenergie.
        /// </summary>
        /// <param name="energieDifferenz">Die Differenz der Energie.</param>
        public void LebenskraftÄndern( int energieDifferenz )
        {
            // Neue Anzahl
            var energie = m_energie + energieDifferenz;
            if (energie == m_energie)
                return;

            // Merken
            m_energie = energie;

            // Schauen wir mal, ob das jemanden interessiert
            LebenskraftWurdeVerändert();
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich die Lebenskraft verändert hat.
        /// </summary>
        private void LebenskraftWurdeVerändert()
        {
            // An alle interessenten weiterleiten
            var interessenten = LebenskraftVerändert;
            if (interessenten != null)
                interessenten( this );

            // Wir sind leider verstorben
            if (m_energie < m_schwächung)
                Spielfeld.UnterbrechenOderBeenden( SimulationsStand.Verloren );
        }

        /// <summary>
        /// Legt fest, ob dieses Element der Schwerkraft folgt.
        /// </summary>
        protected override bool SchwerkraftNutzen { get { return true; } }

        /// <summary>
        /// Erzeugt eine neue Spielfigur.
        /// </summary>
        /// <param name="position">Die Position des Elementes auf dem Spielfeld.</param>
        /// <param name="ausdehnung">Die relative Größe des Elementes.</param>
        public Spieler( Position position, Ausdehnung ausdehnung )
            : base( position, ausdehnung, false, null )
        {
            // Initialisierung beenden
            AktuelleGeschwindigkeit = Geschwindigkeit.Erzeugen( GenaueZahl.Null, GenaueZahl.Null );
            m_sprungDauer = TimeSpan.FromMilliseconds( 100 );
            m_einfacheGeschwindigkeit = GenaueZahl.Eins / 25;
            m_maximalerGeschwindigkeitsFaktor = 5;
            m_sprungStärke = GenaueZahl.Eins / 2;
            m_maximalerSprung = 3;
            m_aktuellerFaktor = 0;
        }

        /// <summary>
        /// Bewegt die Spielfigur eine Einheit schneller nach rechts.
        /// </summary>
        public void SchnellerNachRechtsOderLangsamerNachLinks()
        {
            // Hilfsfunktion verwenden
            FaktorÄndern( +1 );
        }

        /// <summary>
        /// Bewegt die Spielfigur eine Einheit schneller nach links.
        /// </summary>
        public void SchnellerNachLinksOderLangsamerNachRechts()
        {
            // Hilfsfunktion verwenden
            FaktorÄndern( -1 );
        }

        /// <summary>
        /// Stoppt die Spielfigur.
        /// </summary>
        public void Anhalten()
        {
            // HilfsFunktion verwenden
            FaktorÜbernehmen( 0 );
        }

        /// <summary>
        /// Führt einen Sprung aus.
        /// </summary>
        public void Springen()
        {
            // Dürfen wir überhaupt
            if (m_sprungStärke <= GenaueZahl.Null)
                return;
            if (m_maximalerSprung < 1)
                return;

            // Geht nur, wenn wir auch fallen
            var fallGeschwindigkeit = Spielfeld.FallGeschwindigkeit;
            if (fallGeschwindigkeit <= GenaueZahl.Null)
                return;

            // Nur, wenn das Spiel aktiv ist
            if (Spielfeld.Status != SimulationsStand.Läuft)
                return;

            // Wenn wir noch in der Sprungsequenz sind, müssen wir die Grenzwerte beachten
            var spielZeit = Spielfeld.VerbrauchteZeit;
            if (spielZeit < m_nächsterErlaubterSprung)
                if (m_aktuellerSprung >= m_maximalerSprung)
                    return;

            // Es beginnt eine neue Sprungsequenz
            if (spielZeit >= m_nächsterErlaubterSprung)
            {
                // Alles auf Anfang
                m_nächsterErlaubterSprung = spielZeit;
                m_aktuellerSprung = 0;
            }

            // Sprung zählen
            m_aktuellerSprung++;

            // Wie weit bring uns der Sprung
            var distanz = m_sprungStärke * (decimal) m_sprungDauer.TotalSeconds;

            // Wie lange brauchen wir, um das durch Fallen aufzuheben
            var fallDauer = distanz / fallGeschwindigkeit;

            // So lange können wir nicht neu springen
            m_nächsterErlaubterSprung += TimeSpan.FromSeconds( (double) fallDauer );

            // Gewschwindigkeit anlegen
            GeschwindigkeitsRegel.GeschwindigkeitErgänzen( TemporaereGeschwindigkeit.Erzeugen( GenaueZahl.Null, m_sprungStärke, spielZeit + m_sprungDauer ) );
        }

        /// <summary>
        /// Bewegt die Spielfigur eine Einheit schneller.
        /// </summary>
        /// <param name="richtung">Gibt die Richtung der Änderung an.</param>
        private void FaktorÄndern( int richtung )
        {
            // Nur wenn die Spielfigur in Bewegung ist
            if (m_aktuellerFaktor == 0)
                FaktorÜbernehmen( richtung );
            else
                FaktorÜbernehmen( m_aktuellerFaktor + richtung );
        }

        /// <summary>
        /// Verändert die aktuelle Geschwindigkeit der Spielfigur.
        /// </summary>
        /// <param name="neuerFaktor">Der neue Geschwindigkeitsfaktor.</param>
        private void FaktorÜbernehmen( int neuerFaktor )
        {
            // So schnell geht es gar nicht
            if (Math.Abs( neuerFaktor ) > m_maximalerGeschwindigkeitsFaktor)
                return;

            // Faktor aktualisieren
            m_aktuellerFaktor = neuerFaktor;

            // Verwaltung der Geschwindigkeit
            var regeln = GeschwindigkeitsRegel;

            // Alte Geschwindigkeit nicht mehr verwenden
            regeln.GeschwindigkeitEntfernen( AktuelleGeschwindigkeit );

            // Neue Geschwindigkeit ermitteln
            AktuelleGeschwindigkeit = Geschwindigkeit.Erzeugen( m_einfacheGeschwindigkeit * m_aktuellerFaktor, GenaueZahl.Null );

            // Und ab jetzt verwenden
            regeln.GeschwindigkeitErgänzen( AktuelleGeschwindigkeit );
        }

        /// <summary>
        /// Wird aufgerufen, sobald ein Element Teil eines Spielfelds ist.
        /// </summary>
        protected override void SpielfeldWurdeZugeordnet()
        {
            // Basisarbeiten ausführen
            base.SpielfeldWurdeZugeordnet();

            // Wir müssen die Spielzeit überwachen
            Spielfeld.Zeitgeber += ZeitHatSichVerändert;

            // Eigene Geschwindigkeit ergänzen
            GeschwindigkeitsRegel.GeschwindigkeitErgänzen( AktuelleGeschwindigkeit );
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich die Zeit verändert hat.
        /// </summary>
        /// <param name="spielfeld">Die Simulation, in der wir leben.</param>
        /// <param name="neueZeit">Die ab nun gültige Spielzeit.</param>
        private void ZeitHatSichVerändert( Simulation spielfeld, TimeSpan neueZeit )
        {
            // Neuen Abzug berechnen
            var schwächung = (int) Math.Round( neueZeit.TotalSeconds * 62.5 );
            if (schwächung == m_schwächung)
                return;

            // Übernehmen
            m_schwächung = schwächung;

            // Melden
            LebenskraftWurdeVerändert();
        }
    }
}
