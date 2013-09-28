

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt, wie ein Element sich bei einer Kollision verhalten soll.
    /// </summary>
    public abstract class Kollisionsregel
    {
        /// <summary>
        /// Die Art der Kollision.
        /// </summary>
        public KollisionsArten ArtDerKollision { get; set; }
    }
}
