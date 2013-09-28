

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt eine Bewegung relativ zur Spielfigur.
    /// </summary>
    public class BewegungRelativZurSpielfigur : ElementBewegung
    {
        /// <summary>
        /// Gesetzt, wenn sich das Element auf die Spielfigur zubewegen soll.
        /// </summary>
        public bool Angriff { get; set; }

        /// <summary>
        /// Die Geschwindigkeit, mit der sich das Element horizontal bewegen soll.
        /// </summary>
        public int HorizontaleGeschwindigkeit { get; set; }

        /// <summary>
        /// Erstellt eine neue Beschreibung.
        /// </summary>
        public BewegungRelativZurSpielfigur()
        {
        }
    }
}
