using System.Windows.Controls;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Stellt das Ergebnis nach dem Gewinn eines Levels dar.
    /// </summary>
    public class ErgebnisAnzeige : Control
    {
        /// <summary>
        /// Erstellt eine neue Anzeige.
        /// </summary>
        public ErgebnisAnzeige()
        {
            // Konfiguration (XAML) laden
            DefaultStyleKey = typeof( ErgebnisAnzeige );
        }
    }
}
