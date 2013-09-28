

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt das Einsammeln von Punkten.
    /// </summary>
    public class Punkteregel : Kollisionsregel
    {
        /// <summary>
        /// Die Differenz an Punkten, die übertragen werden soll.
        /// </summary>
        public int Punkte { get; set; }
    }
}
