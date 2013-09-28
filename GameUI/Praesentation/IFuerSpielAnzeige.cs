using System.ComponentModel;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Wird zu Anzeige eines Spiels verwendet.
    /// </summary>
    public interface IFuerSpielAnzeige : INotifyPropertyChanged
    {
        /// <summary>
        /// Die Breite des Sichtfensters.
        /// </summary>
        double Breite { get; }

        /// <summary>
        /// Die Höhe des Sichtfensters.
        /// </summary>
        double Hoehe { get; }

        /// <summary>
        /// Das aktuelle Spielfeld.
        /// </summary>
        IFuerSpielfeldAnzeige Spielfeld { get; }

        /// <summary>
        /// Meldet die Steuerelemente der Simulation.
        /// </summary>
        IFuerSpielSteuerung Steuerung { get; }

        /// <summary>
        /// Meldet die horizontale Position des Sichtfensters, so wie sie in der Verschiebung verwendet wird.
        /// </summary>
        double HorizontaleVerschiebung { get; }

        /// <summary>
        /// Meldet die vertikale Position des Sichtfensters, so wie sie in der Verschiebung verwendet wird.
        /// </summary>
        double VertikaleVerschiebung { get; }
    }

    /// <summary>
    /// Stellt die Namen der Eigenschaften der <see cref="IFuerSpielAnzeige"/> zur Verfügung.
    /// </summary>
    public static class FuerSpielAnzeige
    {
        /// <summary>
        /// Der Name der Eigenschaft mit der horizontalen Verschiebung des Sichtfensters.
        /// </summary>
        public static readonly string HorizontaleVerschiebung = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerSpielAnzeige i ) => i.HorizontaleVerschiebung );

        /// <summary>
        /// Der Name der Eigenschaft mit der vertikalen Verschiebung des Sichtfensters.
        /// </summary>
        public static readonly string VertikaleVerschiebung = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerSpielAnzeige i ) => i.VertikaleVerschiebung );
    }
}
