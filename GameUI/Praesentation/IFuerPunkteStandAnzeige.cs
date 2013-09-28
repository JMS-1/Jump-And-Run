using System.ComponentModel;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Beschreibt die Daten für die Anzeige des Punktestandes
    /// </summary>
    public interface IFuerPunkteStandAnzeige : INotifyPropertyChanged
    {
        /// <summary>
        /// Meldet den aktuellen Punktestand.
        /// </summary>
        uint? Punktestand { get; }

        /// <summary>
        /// Meldet die aktuelle Lebensenergie der Spielfigur.
        /// </summary>
        int? Lebensenergie { get; }

        /// <summary>
        /// Die Farbe, in der die Lebensenergie dargestellt werden soll.
        /// </summary>
        string LebensenergieFarbe { get; }
    }

    /// <summary>
    /// Bietet die Namen der Eigenschaft des Punktestandes an.
    /// </summary>
    public static class FuerPunkteStandAnzeige
    {
        /// <summary>
        /// Der Name der Eigenschaft mit dem Punktestand.
        /// </summary>
        public static readonly string Punktestand = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerPunkteStandAnzeige i ) => i.Punktestand );

        /// <summary>
        /// Der Name der Eigenschaft mit der Farbe der Restenergie.
        /// </summary>
        public static readonly string LebensenergieFarbe = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerPunkteStandAnzeige i ) => i.LebensenergieFarbe );

        /// <summary>
        /// Der Name der Eigenschaft mit der Restenergie.
        /// </summary>
        public static readonly string Lebensenergie = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerPunkteStandAnzeige i ) => i.Lebensenergie );
    }
}
