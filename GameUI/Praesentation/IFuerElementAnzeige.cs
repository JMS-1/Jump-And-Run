using System.Windows;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Beschreibt die Präsentationssicht auf ein Element des Spielfelds.
    /// </summary>
    public interface IFuerElementAnzeige : IFuerBildAnzeige
    {
        /// <summary>
        /// Meldet die horizontale Position des Elementes.
        /// </summary>
        double HorizontalePosition { get; }

        /// <summary>
        /// Meldet die vertikale Position des Elementes.
        /// </summary>
        double VertikalePosition { get; }

        /// <summary>
        /// Meldet, ob das Element überhaupt sichtbar ist.
        /// </summary>
        Visibility Sichtbarkeit { get; }

        /// <summary>
        /// Legt die Ebene fest, in der das Element angezeigt wird.
        /// </summary>
        int Ebene { get; }
    }

    /// <summary>
    /// Stellt die Namen der Eigenschaft der <see cref="IFuerElementAnzeige"/> zur Verfügung.
    /// </summary>
    public static class FuerElementAnzeige
    {
        /// <summary>
        /// Der Name der Eigenschaft mit der horizontalen Position eines Elementes.
        /// </summary>
        public static readonly string HorizontalePosition = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerElementAnzeige i ) => i.HorizontalePosition );

        /// <summary>
        /// Der Name der Eigenschaft mit der vertikalen Position eines Elementes.
        /// </summary>
        public static readonly string VertikalePosition = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerElementAnzeige i ) => i.VertikalePosition );

        /// <summary>
        /// Die Eigenschaft mit der Anzeigeebene des Elementes.
        /// </summary>
        public static readonly string Ebene = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerElementAnzeige i ) => i.Ebene );

        /// <summary>
        /// Die Eigenschaft mit der Sichtbarkeit des Elementes.
        /// </summary>
        public static readonly string Sichtbarkeit = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerElementAnzeige i ) => i.Sichtbarkeit );
    }
}
