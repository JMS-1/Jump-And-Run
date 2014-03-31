using System;


namespace JMS.JnRV2.Ablage
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

        /// <summary>
        /// Ein Ausdruck zur Vorauswahl von Spielfeldern.
        /// </summary>
        public string SpielfeldKategorien { get; set; }

        /// <summary>
        /// Das zugehörige Spielfeld.
        /// </summary>
        public Spielfeld Spielfeld { get; internal set; }

        /// <summary>
        /// Erzeugt ein neues Spiel.
        /// </summary>
        public Spiel()
        {
        }

        /// <summary>
        /// Erzeugt ein neues Spiel aus einer alten Konfiguration.
        /// </summary>
        /// <param name="alteDarstellung">Die urspräungliche Darstellung der Konfiguration.</param>
        public Spiel( V1.Spiel alteDarstellung )
        {
            // Alles übernehmen
            SichtfensterBreite = alteDarstellung.SichtfensterBreite;
            SichtfensterHoehe = alteDarstellung.SichtfensterHoehe;
            LevelIndex = alteDarstellung.LevelIndex;
        }
    }
}
