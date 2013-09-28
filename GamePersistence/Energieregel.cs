

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt einen Energieübertrag durch Berührung.
    /// </summary>
    public class Energieregel : Kollisionsregel
    {
        /// <summary>
        /// Die Differenz an Lebensenergie, die durch diese Regel übertragen wird.
        /// </summary>
        public int Lebensenergie { get; set; }
    }
}
