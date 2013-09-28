using System.Windows.Controls;
using System.Windows.Data;


namespace JMS.JnRV2.Ablauf.Tests.Kollisionen
{
    /// <summary>
    /// Zeigt ein Hindernis in der Testumgebung an.
    /// </summary>
    public class HindernisView : Control
    {
        /// <summary>
        /// Erzeugt eine neue Anzeige.
        /// </summary>
        public HindernisView()
        {
            // Konfiguation (XAML) laden
            DefaultStyleKey = typeof( HindernisView );
        }

        /// <summary>
        /// Legt eine neue Anzeige an.
        /// </summary>
        /// <param name="x">Die horizontale Position des Hindernisses.</param>
        /// <param name="y">Die vertikale Position des Hindernisses.</param>
        /// <param name="breite">Die Breite des Hindernisses.</param>
        /// <param name="höhe">Die Höhe des Hindernisses.</param>
        /// <returns>Das gewünschte Hindernis.</returns>
        public static HindernisView Erzeugen( decimal x, decimal y, decimal breite, decimal höhe )
        {
            // Daten erzeugen
            var fläche = new FlächeOderLinieViewModel( Fläche.Erzeugen( Position.Erzeugen( GenaueZahl.Eins * x, GenaueZahl.Eins * y ), Ausdehnung.Erzeugen( GenaueZahl.Eins * breite, GenaueZahl.Eins * höhe ) ) );

            // Anlegen
            var hindernis = new HindernisView { DataContext = fläche };

            // Für die Positionierung auf dem Spielfeld vorbereiten
            BindingOperations.SetBinding( hindernis, Canvas.LeftProperty, new Binding( "Links" ) { Source = fläche } );
            BindingOperations.SetBinding( hindernis, Canvas.TopProperty, new Binding( "Oben" ) { Source = fläche } );

            // Melden
            return hindernis;
        }
    }
}
