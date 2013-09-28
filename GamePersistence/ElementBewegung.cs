using System;
using System.Collections.Generic;
using System.Linq;


namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt, wie sich ein Element bewegen soll.
    /// </summary>
    public abstract class ElementBewegung
    {
        /// <summary>
        /// Beschreibt einen einzelnen Schritt der Bewegung.
        /// </summary>
        public struct Bewegungselement
        {
            /// <summary>
            /// Die horizontale Distanz, die überwunden werden soll.
            /// </summary>
            private double m_horizontaleDistanz;

            /// <summary>
            /// Die horizontale Distanz, die überwunden werden soll.
            /// </summary>
            public double HorizontaleDistanz { get { return m_horizontaleDistanz; } }

            /// <summary>
            /// Die vertikale Distanz, die überwunden werden soll.
            /// </summary>
            private double m_vertikaleDistanz;

            /// <summary>
            /// Die vertikale Distanz, die überwunden werden soll.
            /// </summary>
            public double VertikaleDistanz { get { return m_vertikaleDistanz; } }

            /// <summary>
            /// Die Zeit, die für die Überwindung der Streckt angesetzt werden soll.
            /// </summary>
            private TimeSpan m_dauer;

            /// <summary>
            /// Die Zeit, die für die Überwindung der Streckt angesetzt werden soll.
            /// </summary>
            public TimeSpan Dauer { get { return m_dauer; } }

            /// <summary>
            /// Erstellt einen neuen Schritt.
            /// </summary>
            /// <param name="horizontaleDistanz">Die zu überwindenden horizontale Distanz.</param>
            /// <param name="vertikaleDistanz">Die zu überwindenden vertikale Distanz.</param>
            /// <param name="dauer">Die gewünschte Dauer der Bewegung.</param>
            public Bewegungselement( double horizontaleDistanz, double vertikaleDistanz, TimeSpan dauer )
            {
                // Alles merken
                m_horizontaleDistanz = horizontaleDistanz;
                m_vertikaleDistanz = vertikaleDistanz;
                m_dauer = dauer;
            }
        }

        /// <summary>
        /// Erstellt eine Bewegung aus einer älteren Konfiguration.
        /// </summary>
        /// <param name="alteDarstellung">Die ursprüngliche Konfiguration.</param>
        /// <returns>Die neue Repräsentation der Bewegung des Elementes.</returns>
        public static ElementBewegung Erzeugen( V1.Bewegung alteDarstellung )
        {
            // Das Element steht ruhig da
            if (alteDarstellung == null)
                return null;

            // Einfaches hoch und runter
            var hüpfen = alteDarstellung as V1.Huepfen;
            if (hüpfen != null)
                return new VertikaleBewegung( hüpfen );

            // Im Moment nicht unterstützt
            return null;
        }

        /// <summary>
        /// Meldet die einzelnen Schritte der Bewegung.
        /// </summary>
        public virtual IEnumerable<Bewegungselement> Schritte { get { return Enumerable.Empty<Bewegungselement>(); } }
    }
}
