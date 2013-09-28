

namespace JMS.JnRV2.Ablage.V1
{
    /// <summary>
    /// Beschreibt die Hüpfbewegung eines Elementes.
    /// </summary>
    public class Huepfen : Bewegung
    {
        /// <summary>
        /// Meldet oder setzt die Höhe des Sprungs beim Hüpfen.
        /// </summary>
        public double SprungHoehe { get; set; }

        /// <summary>
        /// Meldet oder setzt die Dauer des Sprungs beim Hüpfen.
        /// </summary>
        public double SprungZeit { get; set; }
    }
}
