

namespace JMS.JnRV2.Ablage.V1
{
    /// <summary>
    /// Beschreibt ein Spiel.
    /// </summary>
    public class Spiel
    {
        /// <summary>
        /// Die Breite des Sichtfensters ins Spielfeld.
        /// </summary>
        public double SichtfensterBreite { get; set; }

        /// <summary>
        /// Die Höhe des Sichtfensters ins Spielfeld.
        /// </summary>
        public double SichtfensterHoehe { get; set; }

        /// <summary>
        /// Die bei 1 beginnende laufende Nummer des Spielfelds.
        /// </summary>
        public int LevelIndex { get; set; }
    }
}
