using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Wird für die Anzeige eines Spielfeldes angeboten.
    /// </summary>
    public interface IFuerSpielfeldAnzeige : INotifyPropertyChanged
    {
        /// <summary>
        /// Das Hintergrundbild des Spielfelds.
        /// </summary>
        IFuerBildAnzeige Hintergrund { get; }

        /// <summary>
        /// Meldet, ob das Spielfeld schon sichtbar ist.
        /// </summary>
        Visibility Sichtbarkeit { get; }

        /// <summary>
        /// Alle Elemente auf diesem Spielfeld.
        /// </summary>
        IEnumerable<IFuerElementAnzeige> Elemente { get; }
    }

    /// <summary>
    /// Stellt die Namen der Eigenschaften der <see cref="IFuerSpielfeldAnzeige"/> Schnittstelle zur Verfügung.
    /// </summary>
    public static class FuerSpielfeldAnzeige
    {
        /// <summary>
        /// Der Name der Eigenschaft mit der Sichtbarkeit des Spielfelds.
        /// </summary>
        public static readonly string Sichtbarkeit = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerSpielfeldAnzeige i ) => i.Sichtbarkeit );
    }
}
