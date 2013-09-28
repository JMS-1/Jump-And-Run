

namespace JMS.JnRV2.Ablage.V1
{
    /// <summary>
    /// Die Art eines Elementes.
    /// </summary>
    public enum ElementArt
    {
        /// <summary>
        /// Eine Falle, die bei Berührung nicht verschwindet.
        /// </summary>
        EwigeFalle,

        /// <summary>
        /// Steht einfach nur so im Weg herum.
        /// </summary>
        Sperre,

        /// <summary>
        /// Das Element kann sich bewegen.
        /// </summary>
        Beweglich,

        /// <summary>
        /// Die Berügrung mit diesem Element beendet das Spiel und führt zum Gewinn.
        /// </summary>
        Ausgang,
    }
}
