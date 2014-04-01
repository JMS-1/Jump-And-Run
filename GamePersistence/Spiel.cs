using System;
using System.Collections.Generic;
using System.Linq;


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
        /// Ein Ausdruck zur Vorauswahl von Spielfeldern.
        /// </summary>
        public string SpielfeldKategorien { get; set; }

        /// <summary>
        /// Alle möglichen Arten von Spielfeldern.
        /// </summary>
        private HashSet<string> m_erlaubteKategorien;

        /// <summary>
        /// Das zugehörige Spielfeld.
        /// </summary>
        public Spielfeld Spielfeld { get; set; }

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
        }

        /// <summary>
        /// Prüft, ob ein Spielfeld verwendet werden kann.
        /// </summary>
        /// <param name="spielfeld">Ein Spielfeld.</param>
        /// <returns>Gesetzt, wenn es sich um ein gültiges Spielfeld handelt.</returns>
        public bool SpielfeldIstGültig( Spielfeld spielfeld )
        {
            // Geht gar nicht
            if (spielfeld == null)
                return false;

            // Art des Spielfelds und Filter ermitteln
            var kategorie = spielfeld.Kategorie;
            var filter = SpielfeldKategorien;

            // Wenn kein Filter eingesetzt wird sind nur Spielfelder ohne Art erlaubt
            if (string.IsNullOrWhiteSpace( filter ))
                return string.IsNullOrWhiteSpace( kategorie );

            // Gibt es allerdings einen Filter so werden nur Spielfelder mit Art berücksichtigt
            else if (string.IsNullOrWhiteSpace( kategorie ))
                return false;

            // Einmal der langsame Weg
            if (m_erlaubteKategorien == null)
                m_erlaubteKategorien = new HashSet<string>( filter.Split( ',' ).Select( k => k.Trim() ) );

            // Auswerten auf die schnelle Art
            return m_erlaubteKategorien.Contains( kategorie );
        }
    }
}
