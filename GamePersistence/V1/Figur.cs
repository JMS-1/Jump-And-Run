

namespace JMS.JnRV2.Ablage.V1
{
    /// <summary>
    /// Die Konfiguration der Spielfigur.
    /// </summary>
    public class Figur
    {
        /// <summary>
        /// Der Name der Spielfigur.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gibt an, wie weit ein einzelner Sprung in die Höhe geht.
        /// </summary>
        public int SprungStaerke { get; set; }

        /// <summary>
        /// Gibt an, wie viele Sprünge in direkter Folge ausgeführt werden können.
        /// </summary>
        public int SpruengeNacheinander { get; set; }

        /// <summary>
        /// Gibt an, wie schnell die Bildsequenzen abgespielt werden sollen.
        /// </summary>
        public int BilderProSekunde { get; set; }

        /// <summary>
        /// Legt die maximale Geschwindigkeit der Spielfigur fest.
        /// </summary>
        public int MaximaleGeschwindigkeit { get; set; }

        /// <summary>
        /// Die Bilder für eine ruhende Spielfigur.
        /// </summary>
        public BilderFeld BilderRuhend { get; private set; }

        /// <summary>
        /// Die Bilder für die Spielfigur, wenn sie sich nach links bewegt.
        /// </summary>
        public BilderFeld BilderNachLinks { get; private set; }

        /// <summary>
        /// Die Bilder für die Spielfigur, wenn sie sich nach rechts bewegt.
        /// </summary>
        public BilderFeld BilderNachRechts { get; private set; }

        /// <summary>
        /// Die Bilder für die Spielfigur, wenn sie fliegt.
        /// </summary>
        public BilderFeld BilderImSprung { get; private set; }

        /// <summary>
        /// Erstellt eine neue Spielfigur.
        /// </summary>
        public Figur()
        {
            // Aufsetzen
            BilderRuhend = new BilderFeld();
            BilderImSprung = new BilderFeld();
            BilderNachLinks = new BilderFeld();
            BilderNachRechts = new BilderFeld();
        }
    }
}
