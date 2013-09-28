using JMS.JnRV2.Ablage;


namespace JMS.JnRV2.Start
{
    /// <summary>
    /// Liefert die Informationen zu einem Spielfeld.
    /// </summary>
    public class SpielfeldMitInformation
    {
        /// <summary>
        /// Eine Kurzbeschreibung des Spielfelds.
        /// </summary>
        public string Beschreibung { get { return Konfiguration.Beschreibung; } }

        /// <summary>
        /// Die ursprüngliche Konfiguration des Spielfelds.
        /// </summary>
        public Spielfeld Konfiguration { get; private set; }

        /// <summary>
        /// Alle Spielergebnisse.
        /// </summary>
        public Ergebnisse Ergebnisse { get; private set; }

        /// <summary>
        /// Erzeugt einen neuen Informationssatz.
        /// </summary>
        /// <param name="spielfeld">Das zugehörige Spielfeld.</param>
        public SpielfeldMitInformation( Spielfeld spielfeld )
        {
            // Merken
            Konfiguration = spielfeld;

            // Ergebnisse bereit stellen
            Ergebnisse = new Ergebnisse( spielfeld.Ergebnisse );
        }
    }
}
