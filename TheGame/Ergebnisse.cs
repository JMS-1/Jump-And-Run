using System.Linq;


namespace JMS.JnRV2.Start
{
    /// <summary>
    /// Bietet die Spielergebnisse zur Anzeige an.
    /// </summary>
    public class Ergebnisse
    {
        /// <summary>
        /// Stellt ein einzelnes Ergebnis dar.
        /// </summary>
        public class Einzelergebnis
        {
            /// <summary>
            /// Der Zeitpunkt, zudem das Spiel beendet wurde.
            /// </summary>
            public string Datum { get; private set; }

            /// <summary>
            /// Meldet das Gesamtergebnis des Tests.
            /// </summary>
            public uint Gesamtergebnis { get; private set; }

            /// <summary>
            /// Meldet die gesammelten Punkte.
            /// </summary>
            public uint Punkte { get; private set; }

            /// <summary>
            /// Meldet die Restenergie des Tests.
            /// </summary>
            public uint Restenergie { get; private set; }

            /// <summary>
            /// Erstellt ein neues Einzelergebnis.
            /// </summary>
            /// <param name="ergebnis">Die Informationen aus der Ablage.</param>
            private Einzelergebnis( Ablage.Spielergebnis ergebnis )
            {
                // Übernehmen
                Datum = ergebnis.Endzeitpunkt.ToLocalTime().ToString( "dd.MM HH:mm" );
                Gesamtergebnis = ergebnis.Gesamtergebnis;
                Restenergie = ergebnis.Restenergie;
                Punkte = ergebnis.Punkte;
            }

            /// <summary>
            /// Erstellt ein neues Einzelergebnis.
            /// </summary>
            /// <param name="ergebnis">Die Informationen aus der Ablage.</param>
            /// <returns>Die Präsentation des Ergebnisses.</returns>
            public static Einzelergebnis Erzeuge( Ablage.Spielergebnis ergebnis )
            {
                // Anlegen
                return new Einzelergebnis( ergebnis );
            }
        }

        /// <summary>
        /// Die in der Ablage verwalteten Ergebnisse.
        /// </summary>
        private readonly Ablage.Spielergebnisse m_ergebnisse;

        /// <summary>
        /// Die Anzahl der ausgeführten Spiele.
        /// </summary>
        public int Spiele { get { return m_ergebnisse.Anzahl; } }

        /// <summary>
        /// Die besten Durchläufe.
        /// </summary>
        public Einzelergebnis[] BestenListe { get { return m_ergebnisse.BesteErgebnisse.Select( Einzelergebnis.Erzeuge ).ToArray(); } }

        /// <summary>
        /// Meldet das beste Ergebnis.
        /// </summary>
        public uint BestesErgebnis
        {
            get
            {
                // Schauen wir mal, ob wir schon einmal gespielt haben
                var ergebnis = m_ergebnisse.BesteErgebnisse.FirstOrDefault();
                if (ergebnis == null)
                    return 0;
                else
                    return ergebnis.Gesamtergebnis;
            }
        }
        /// <summary>
        /// Erstellt eine neue Präsentation.
        /// </summary>
        /// <param name="ergebnisse">Die Verwaltung der Spielergebnisse.</param>
        public Ergebnisse( Ablage.Spielergebnisse ergebnisse )
        {
            // Merken
            m_ergebnisse = ergebnisse;
        }
    }
}
