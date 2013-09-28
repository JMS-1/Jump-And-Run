

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt ein Element auf dem Spielfeld, das mit einem eigenen Bild versehen ist.
    /// </summary>
    public class ElementMitBildSequenz : Element
    {
        /// <summary>
        /// Alle Bilder dieses Elementes.
        /// </summary>
        public BildSequenz Bilder { get; private set; }

        /// <summary>
        /// Die Anzahl der Bilder, die pro Sekunde dargestellt werden sollen.
        /// </summary>
        public int BilderProSekunde { get; set; }

        /// <summary>
        /// Beschreibt, wie sich dieses Element auf dem Spielfeld bewegt.
        /// </summary>
        public ElementBewegung Animation { get; set; }

        /// <summary>
        /// Meldet oder setzt die Darstellungsebene des Elementes.
        /// </summary>
        public int Ebene { get; set; }

        /// <summary>
        /// Gesetzt, wenn die Schwerkraft auf das Element wirkt.
        /// </summary>
        public bool Faellt { get; set; }

        /// <summary>
        /// Erstellt ein neues Element mit Bild.
        /// </summary>
        public ElementMitBildSequenz()
        {
            // Fertigstellen
            Bilder = new BildSequenz();
            Ebene = -1;
        }

        /// <summary>
        /// Erstellt ein neues Element mit Bild aus einer alten Konfiguration.
        /// </summary>
        /// <param name="alteDarstellung">Die ursprüngliche Konfiguration.</param>
        internal ElementMitBildSequenz( V1.BildElement alteDarstellung )
            : base( alteDarstellung )
        {
            // Alles übernehmen
            Animation = ElementBewegung.Erzeugen( alteDarstellung.Bewegung );
            BilderProSekunde = alteDarstellung.BilderProSekunde;
            Bilder = alteDarstellung.Bilder;
            Ebene = alteDarstellung.Ebene;

            // Punkte auswerten
            var punkte = alteDarstellung.Wert;
            if (punkte != 0)
                Kollisionsregeln.Insert( 0, new Punkteregel { ArtDerKollision = KollisionsArten.VomSpielerGetroffen, Punkte = punkte } );
        }
    }
}
