using System.Windows.Controls;
using System.Windows.Data;


namespace JMS.JnRV2.Ablauf.Tests
{
    /// <summary>
    /// Zeigt ein einzelnes Element auf dem Spielfeld an.
    /// </summary>
    public class ElementTestView : Control
    {
        /// <summary>
        /// Erzeugt eine neue Anzeige.
        /// </summary>
        public ElementTestView()
        {
            // XAML Konfiguration laden
            DefaultStyleKey = typeof( ElementTestView );

            // Dynamische Konfiguration festlegen
            BindingOperations.SetBinding( this, Canvas.LeftProperty, new Binding( "X" ) );
            BindingOperations.SetBinding( this, Canvas.TopProperty, new Binding( "Y" ) );
        }

        /// <summary>
        /// Meldet das zugehörige Element.
        /// </summary>
        public ElementTestViewModel Element { get { return (ElementTestViewModel) DataContext; } }
    }
}
