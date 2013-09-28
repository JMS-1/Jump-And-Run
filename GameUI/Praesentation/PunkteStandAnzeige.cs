using System.Windows.Controls;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Zeigt den aktuellen Punktestand an.
    /// </summary>
    public class PunkteStandAnzeige : Control
    {
        /// <summary>
        /// Erzeugt eine neue Anzeige des Punktestandes.
        /// </summary>
        public PunkteStandAnzeige()
        {
            // Konfiguration (XAML) laden
            DefaultStyleKey = typeof( PunkteStandAnzeige );
        }
    }
}
