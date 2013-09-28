using System;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Beschreibt, wie sich ein Element gerade bewegt.
    /// </summary>
    [Flags]
    public enum ElementBewegung
    {
        /// <summary>
        /// Das Element ruht
        /// </summary>
        Ruht = 0,

        /// <summary>
        /// Das Element bewegt sich nach unten.
        /// </summary>
        FälltNachUnten = 0x1,

        /// <summary>
        /// Das Element spring nach oben.
        /// </summary>
        SpringtNachOben = 0x2,

        /// <summary>
        /// Das Element bewegt sich nach links.
        /// </summary>
        LäuftNachLinks = 0x4,

        /// <summary>
        /// Das Element bewegt sich nach rechts.
        /// </summary>
        LäuftNachRechts = 0x8,
    }
}
