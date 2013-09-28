

namespace JMS.JnRV2.Ablauf.Tests.Kollisionen
{
    /// <summary>
    /// Diese Schnittstelle wird von allen Elementen angeboten, die im Rahmen der Kollisionstests verwendet werden.
    /// </summary>
    public interface IKollisionsTestElement
    {
        /// <summary>
        /// Meldet die Ausdehung des Spielfelds.
        /// </summary>
        /// <param name="breite">Die absolute Breite des Spielfelds.</param>
        /// <param name="höhe">Die relative Breite des Spielfelds.</param>
        void SpielfeldAusdehnungFestlegen( decimal breite, decimal höhe );
    }
}
