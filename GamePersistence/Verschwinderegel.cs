

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Bei einer Kollision wird das Element vom Spielfeld entfernt.
    /// </summary>
    public class Verschwinderegel : Kollisionsregel
    {
        /// <summary>
        /// Legt fest, welches Element verschwinden soll.
        /// </summary>
        public WasSollVerschwinden WasSollVerschwinden { get; set; }
    }
}
