using System;
using System.Collections.Generic;


namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt eine rein vertikale Bewegung eines Elementes.
    /// </summary>
    public class VertikaleBewegung : ElementBewegung
    {
        /// <summary>
        /// Meldet oder setzt die Höhe des Sprungs beim Hüpfen.
        /// </summary>
        public double Sprunghöhe { get; set; }

        /// <summary>
        /// Meldet oder setzt die Dauer des Sprungs beim Hüpfen in Sekunden.
        /// </summary>
        public double Sprungzeit { get; set; }

        /// <summary>
        /// Erstellt eine neue Beschreibung.
        /// </summary>
        public VertikaleBewegung()
        {
        }

        /// <summary>
        /// Erstellt eine neue Beschreibung aus einer älteren Konfiguration.
        /// </summary>
        /// <param name="alteDarstellung">Die ursprüngliche Konfiguration.</param>
        internal VertikaleBewegung( V1.Huepfen alteDarstellung )
        {
            // Alles übernehmen
            Sprunghöhe = alteDarstellung.SprungHoehe;
            Sprungzeit = alteDarstellung.SprungZeit;
        }

        /// <summary>
        /// Meldet die einzelnen Schritte der Bewegung.
        /// </summary>
        public override IEnumerable<Bewegungselement> Schritte
        {
            get
            {
                // Umrechnen
                var sprungzeit = TimeSpan.FromSeconds( Sprungzeit );

                // Hoch und dann wieder runter
                yield return new Bewegungselement( 0, Sprunghöhe, sprungzeit );
                yield return new Bewegungselement( 0, -Sprunghöhe, sprungzeit );
            }
        }
    }
}
