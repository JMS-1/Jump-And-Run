using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;


namespace JMS.JnRV2.Ablage.V1
{
    /// <summary>
    /// Beschreibt die freie Bewegung eines Elementes.
    /// </summary>
    [ContentProperty( "Stuetzpunkte" )]
    public class PfadFolgen : Bewegung
    {
        /// <summary>
        /// Die Zeit für einen Durchlauf des Pfads.
        /// </summary>
        public double VolleZeit { get; set; }

        /// <summary>
        /// Die Eckpunkte der Bewegung.
        /// </summary>
        public List<Point> Stuetzpunkte { get; private set; }

        /// <summary>
        /// Erzeugt eine neue Beschreibung.
        /// </summary>
        public PfadFolgen()
        {
            // Initialisierung beenden
            Stuetzpunkte = new List<Point>();
        }
    }
}
