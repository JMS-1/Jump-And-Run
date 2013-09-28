

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Bei einer Kollision erscheint ein anderes Element.
    /// </summary>
    public class Erscheineregel : Kollisionsregel
    {
        /// <summary>
        /// Der Name des Elementes, das erscheinen soll.
        /// </summary>
        public string Name { get; set; }
    }
}
