using System.ComponentModel;
using System.Windows.Media;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Diese Schnittstelle wird von einer <see cref="BildAnzeige"/> von dem
    /// zugeordneten Objekt erwartet.
    /// </summary>
    public interface IFuerBildAnzeige : INotifyPropertyChanged
    {
        /// <summary>
        /// Die Breite des Bildes.
        /// </summary>
        double Breite { get; }

        /// <summary>
        /// Die Höhe des Bildes.
        /// </summary>
        double Hoehe { get; }

        /// <summary>
        /// Meldet das aktuelle Bild.
        /// </summary>
        ImageSource Bild { get; }
    }

    /// <summary>
    /// Stellt die Namen der <see cref="IFuerBildAnzeige"/> Eigenschaften refakturierungssicher bereit.
    /// </summary>
    public static class FuerBildAnzeige
    {
        /// <summary>
        /// Der Name der Eigenschaft mit der Breite des Elementes.
        /// </summary>
        public static readonly string Breite = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerBildAnzeige i ) => i.Breite );

        /// <summary>
        /// Der Name der Eigenschaft mit der Höhe des Elementes.
        /// </summary>
        public static readonly string Höhe = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerBildAnzeige i ) => i.Hoehe );

        /// <summary>
        /// Der Name der Eigenschaft mit dem Bild.
        /// </summary>
        public static readonly string Bild = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerBildAnzeige i ) => i.Bild );
    }
}
