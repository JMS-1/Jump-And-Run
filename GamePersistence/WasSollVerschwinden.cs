

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt beim einem Treffer, welches Element verschwinden soll.
    /// </summary>
    public enum WasSollVerschwinden
    {
        /// <summary>
        /// Das Element, das angestossen wurde.
        /// </summary>
        Fest,

        /// <summary>
        /// Das Element, das sich bewegt hat.
        /// </summary>
        Beweglich,

        /// <summary>
        /// Das Element, für das die Regel definiert wurde.
        /// </summary>
        Selbst,
    }
}
