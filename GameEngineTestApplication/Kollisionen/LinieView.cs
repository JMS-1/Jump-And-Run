using System.Windows.Controls;
using System.Windows.Data;


namespace JMS.JnRV2.Ablauf.Tests.Kollisionen
{
    /// <summary>
    /// Zeigt eine einzelne Linie an.
    /// </summary>
    public class LinieView : Control
    {
        /// <summary>
        /// Erstellt eine neue Anzeige.
        /// </summary>
        public LinieView()
        {
            // Konfiguration (XAML) laden
            DefaultStyleKey = typeof( LinieView );
        }

        /// <summary>
        /// Legt eine neue Anzeige an.
        /// </summary>
        /// <param name="fläche">Die zugehörige Fläche.</param>
        /// <returns>Das gewünschte Hindernis.</returns>
        public static LinieView Erzeugen( FlächeOderLinieViewModel fläche )
        {
            // Anlegen
            var linie = new LinieView { DataContext = fläche };

            // Für die Positionierung auf dem Spielfeld vorbereiten
            BindingOperations.SetBinding( linie, Canvas.LeftProperty, new Binding( "Links" ) { Source = fläche } );
            BindingOperations.SetBinding( linie, Canvas.TopProperty, new Binding( "Oben" ) { Source = fläche } );

            // Melden
            return linie;
        }
    }
}
