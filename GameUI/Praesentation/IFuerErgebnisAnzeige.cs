using System.ComponentModel;
using System.Windows;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Verwaltet die Spielergebnisse zu einem Level.
    /// </summary>
    public interface IFuerErgebnisAnzeige : INotifyPropertyChanged
    {
        /// <summary>
        /// Die eingesammelten Punkte.
        /// </summary>
        uint Punkte { get; }

        /// <summary>
        /// Die verbleibende Energie.
        /// </summary>
        uint Restenergie { get; }

        /// <summary>
        /// Das Gesamtergebnis.
        /// </summary>
        uint Gesamtergebnis { get; }

        /// <summary>
        /// Meldet, ob das Ergebnis sichtbar ist.
        /// </summary>
        Visibility Sichtbarkeit { get; }
    }

    /// <summary>
    /// Die Namen der Eigenschaften der <see cref="IFuerErgebnisAnzeige"/> Schnittstelle.
    /// </summary>
    public static class FuerErgebnisAnzeige
    {
        /// <summary>
        /// Der Name der Eigenschaft mit den Punkten.
        /// </summary>
        public static readonly string Punkte = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerErgebnisAnzeige i ) => i.Punkte );

        /// <summary>
        /// Der Name der Eigenschaft mit der Restenergie.
        /// </summary>
        public static readonly string Restenergie = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerErgebnisAnzeige i ) => i.Restenergie );

        /// <summary>
        /// Der Name der Eigenschaft mit der Sichtbarkeit.
        /// </summary>
        public static readonly string Sichtbarkeit = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerErgebnisAnzeige i ) => i.Sichtbarkeit );

        /// <summary>
        /// Der Name der Eigenschaft mit dem Gesamtergebnis.
        /// </summary>
        public static readonly string Gesamtergebnis = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerErgebnisAnzeige i ) => i.Gesamtergebnis );
    }
}
