using System;


namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt eine Spielfigur.
    /// </summary>
    public class Spielfigur
    {
        /// <summary>
        /// Der Name der Spielfigur.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Die Bilder für eine ruhende Spielfigur.
        /// </summary>
        public BildSequenz BilderRuhend { get; private set; }

        /// <summary>
        /// Die Bilder für die Spielfigur, wenn sie sich nach links bewegt.
        /// </summary>
        public BildSequenz BilderNachLinks { get; private set; }

        /// <summary>
        /// Die Bilder für die Spielfigur, wenn sie sich nach rechts bewegt.
        /// </summary>
        public BildSequenz BilderNachRechts { get; private set; }

        /// <summary>
        /// Die Bilder für die Spielfigur, wenn sie fliegt.
        /// </summary>
        public BildSequenz BilderImSprung { get; private set; }

        /// <summary>
        /// Gibt an, wie schnell die Bildsequenzen abgespielt werden sollen.
        /// </summary>
        public int BilderProSekunde { get; set; }

        /// <summary>
        /// Ein Wert von <i>100</i> bedeutet, dass eine Normspielfigur mit einer Höhe von
        /// 60 Pixel 150 Pixel in die Höhe springen kann. Bei größer Höhe verändert sich
        /// diese Distanz proportional. Es dauert 0.1 Sekunden, bis der Sprung seine höchste
        /// Höhe erreicht hat, danach fällt die Spielfigur in 0.3 Sekunden zurück auf das
        /// ursprüngliche Niveau.
        /// </summary>
        public int SprungStaerke { get; set; }

        /// <summary>
        /// Gibt an, wie viele Sprünge in direkter Folge ausgeführt werden können.
        /// </summary>
        public int SpruengeNacheinander { get; set; }

        /// <summary>
        /// Ist dieser Wert <i>1</i> so bedeutet dies, dass sich eine Normspielfigur mit 
        /// einer Breite von 40 Pixel pro Sekunde um 68 Pixel bewegt - bei größerer Breite 
        /// entsprechende schneller, i.e. bei 60 Pixel Breite 102 Pixel pro Sekunde und so 
        /// weiter. Die tatsächliche Geschwindigkeit kann ein ganzes Vielfaches dieser 
        /// Grundgeschwindigkeit sein, wobei der Faktor maximal den in dieser Eigenschaft 
        /// festgehaltenen Wert annehmen kann.
        /// <seealso cref="Skalierungswerte.EinfacheHorizontaleGeschwindigkeit"/>
        /// </summary>
        public int MaximaleGeschwindigkeit { get; set; }

        /// <summary>
        /// Erstellt eine neue Spielfigur.
        /// </summary>
        public Spielfigur()
        {
            // Alles aufsetzen
            BilderNachRechts = new BildSequenz();
            BilderNachLinks = new BildSequenz();
            BilderImSprung = new BildSequenz();
            BilderRuhend = new BildSequenz();
            BilderProSekunde = 1;
            Name = "Spieler";
        }

        /// <summary>
        /// Erstellt eine neue Spielfigur aus einer alten Konfiguration.
        /// </summary>
        /// <param name="alteDarstellung">Die ursprüngliche Darstellung der Spielfigur.</param>
        public Spielfigur( V1.Figur alteDarstellung )
        {
            // Prüfen
            if (alteDarstellung == null)
                throw new ArgumentNullException( "alteDarstellung" );

            // Alles übernehmen
            MaximaleGeschwindigkeit = alteDarstellung.MaximaleGeschwindigkeit;
            SpruengeNacheinander = alteDarstellung.SpruengeNacheinander;
            BilderProSekunde = alteDarstellung.BilderProSekunde;
            BilderNachRechts = alteDarstellung.BilderNachRechts;
            BilderNachLinks = alteDarstellung.BilderNachLinks;
            BilderImSprung = alteDarstellung.BilderImSprung;
            SprungStaerke = alteDarstellung.SprungStaerke;
            BilderRuhend = alteDarstellung.BilderRuhend;
            Name = alteDarstellung.Name;
        }
    }
}
