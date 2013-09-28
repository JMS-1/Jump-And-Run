

namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Beschreibt ein einzelnes Element auf dem Spielfeld.
    /// </summary>
    public sealed class Element : GrundElement
    {
        /// <summary>
        /// Legt fest, ob dieses Element der Schwerkraft folgt.
        /// </summary>
        protected override bool SchwerkraftNutzen { get { return Fällt; } }

        /// <summary>
        /// Legt fest, ob dieses Element der Schwerkraft folgt.
        /// </summary>
        public bool Fällt { private get; set; }

        /// <summary>
        /// Erstellt ein neues Element.
        /// </summary>
        /// <param name="position">Die Position des Elementes auf dem Spielfeld.</param>
        /// <param name="ausdehnung">Die relative Größe des Elementes.</param>
        /// <param name="initialDeaktiviert">Gesetzt, wenn das Element bei Beginn nicht berücksichtigt wird.</param>
        /// <param name="name">Der optionale Name dieses Elementes.</param>
        public Element( Position position, Ausdehnung ausdehnung, bool initialDeaktiviert, string name )
            : base( position, ausdehnung, initialDeaktiviert, name )
        {
        }
    }
}
