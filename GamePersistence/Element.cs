using System;
using System.Collections.Generic;
using System.Windows.Markup;


namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt ein einzelnes Element.
    /// </summary>
    [ContentProperty( "Kollisionsregeln" )]
    public class Element
    {
        /// <summary>
        /// Die horizontale Position des Elementes.
        /// </summary>
        public int HorizontalePosition { get; set; }

        /// <summary>
        /// Die vertikale Position des Elementes.
        /// </summary>
        public int VertikalePosition { get; set; }

        /// <summary>
        /// Die Höhe des Elementes.
        /// </summary>
        public double Hoehe { get; set; }

        /// <summary>
        /// Die Breite des Elementes.
        /// </summary>
        public double Breite { get; set; }

        /// <summary>
        /// Der relative Pfad zur Datei mit der Melodie, die bei Kollision mit dem Element abgespielt wird.
        /// </summary>
        public string Melodie { get; set; }

        /// <summary>
        /// Optional ein eindeutiger Name für das Element
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gesetz, wenn das Element erst einmal nicht sichtbar sein soll.
        /// </summary>
        public bool AnfänglichUnsichtbar { get; set; }

        /// <summary>
        /// Alle Kollisionsregeln, die anzuwenden sind.
        /// </summary>
        public List<Kollisionsregel> Kollisionsregeln { get; private set; }

        /// <summary>
        /// Erstellt ein Element.
        /// </summary>
        public Element()
        {
            // Abschliessen
            Kollisionsregeln = new List<Kollisionsregel>();
        }

        /// <summary>
        /// Rekonstruiert ein Element aus einer älteren Konfiguration.
        /// </summary>
        /// <param name="alteDarstellung">Die ursprüngliche Konfiguration.</param>
        protected Element( V1.Element alteDarstellung )
        {
            // Abschliessen
            Kollisionsregeln = new List<Kollisionsregel>();

            // Alles kopieren
            Breite = alteDarstellung.Width;
            Hoehe = alteDarstellung.Height;
            Melodie = alteDarstellung.Melodie;
            VertikalePosition = alteDarstellung.Bottom;
            HorizontalePosition = alteDarstellung.Left;

            // Lebensenergie auswerten
            var lebensenergie = alteDarstellung.Lebensenergie;
            if (lebensenergie != 0)
            {
                // Regel anlegen
                var energieRegel = new Energieregel { Lebensenergie = lebensenergie };

                // Konfiguration der Regel abschliessen
                if (lebensenergie > 0)
                    energieRegel.ArtDerKollision = KollisionsArten.VomSpielerGetroffen;
                else if (alteDarstellung.ElementArt == V1.ElementArt.Beweglich)
                    energieRegel.ArtDerKollision = KollisionsArten.VomSpielerSeitlichGetroffen;
                else
                    energieRegel.ArtDerKollision = KollisionsArten.VomSpielerGetroffen;

                // Regel eintragen
                Kollisionsregeln.Add( energieRegel );
            }

            // Ausgang anmelden
            if (alteDarstellung.ElementArt == V1.ElementArt.Ausgang)
                Kollisionsregeln.Add( new Enderegel { Gewonnen = true, ArtDerKollision = KollisionsArten.VomSpielerGetroffen } );

            // Element ausblenden - das muss immer die letzte Regel sein
            if (alteDarstellung.ElementArt == V1.ElementArt.Beweglich)
                Kollisionsregeln.Add( new Verschwinderegel { ArtDerKollision = KollisionsArten.VomSpielerGetroffen } );
        }

        /// <summary>
        /// Rekonstruiert ein Element aus einer älteren Konfiguration.
        /// </summary>
        /// <param name="alteDarstellung">Die ursprüngliche Konfiguration.</param>
        /// <returns>Das gewünschte Element.</returns>
        public static Element Erzeugen( V1.Element alteDarstellung )
        {
            // Prüfen
            if (alteDarstellung == null)
                throw new ArgumentNullException( "alteDarstellung" );

            // Element mit Bild
            var elementMitBild = alteDarstellung as V1.BildElement;
            if (elementMitBild != null)
                return new ElementMitBildSequenz( elementMitBild );

            // Wandeln
            return new Element( alteDarstellung );
        }
    }
}
