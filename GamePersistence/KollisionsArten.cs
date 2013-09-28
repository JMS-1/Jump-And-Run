

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Bezeichnet die einzelnen Arten einer Kollision.
    /// </summary>
    public enum KollisionsArten
    {
        /// <summary>
        /// Es ist keine besondere Art ausgewählt.
        /// </summary>
        Unbekannt,

        /// <summary>
        /// Der Spieler berührt ein Element durch seine Bewegung.
        /// </summary>
        VomSpielerGetroffen,

        /// <summary>
        /// Der Spieler berührt ein Element im Verlaufe einer rein horizontalen Bewegung.
        /// </summary>
        VomSpielerSeitlichGetroffen,

        /// <summary>
        /// Der Spieler berührt ein Element im Verlaufe einer rein vertikalen Bewegung.
        /// </summary>
        VomSpielerVertikalGetroffen,

        /// <summary>
        /// Ein Element wird von einem anderen Element oder dem Spieler getroffen.
        /// </summary>
        Getroffen,

        /// <summary>
        /// Ein Element wird von einem anderen Element getroffen.
        /// </summary>
        GetroffenAberNichtVomSpieler,

        /// <summary>
        /// Ein Hindernis ist gegen den Spieler geknallt.
        /// </summary>
        SpielerIstGetroffen,
    }
}
