

namespace JMS.JnRV2.Anzeige
{
    /// <summary>
    /// Beschreibt die einzelnen Zustände der Spielfigur.
    /// </summary>
    public enum ZustandDerFigur
    {
        /// <summary>
        /// Die Spielfigur ruht.
        /// </summary>
        Ruhend = 0,

        /// <summary>
        /// Die Spielfigur bewegt sich nach rechts.
        /// </summary>
        NachRechts = 1,

        /// <summary>
        /// Die Spielfigur bewegt sich nach links.
        /// </summary>
        NachLinks = 2,

        /// <summary>
        /// Die Spielfigur befindet sich in der Luft.
        /// </summary>
        InDerLuft = 3,

        /// <summary>
        /// Die Anzahl der möglichen Zustände.
        /// </summary>
        Anzahl = 4,
    }
}
