using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Zeigt ein Spielfeld mit allen Spielelementen an.
    /// </summary>
    [TemplatePart( Name = ErwarteteElemente.Elemente, Type = typeof( Canvas ) )]
    public class SpielfeldAnzeige : ContentControl
    {
        /// <summary>
        /// Alle in der Vorlage erwarteten Oberflächenelemente.
        /// </summary>
        public static class ErwarteteElemente
        {
            /// <summary>
            /// Der Name der Pinwand für die Elemente.
            /// </summary>
            public const string Elemente = "elemente";
        }

        /// <summary>
        /// Erstellt eine neue Anzeige.
        /// </summary>
        public SpielfeldAnzeige()
        {
            // Konfiguration (XAML) laden
            DefaultStyleKey = typeof( SpielfeldAnzeige );
        }

        /// <summary>
        /// Wird aufgrufen, wenn das Overflächenelement vorbereitet wird.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Basisklasse zuerst
            base.OnApplyTemplate();

            // Schauen wir mal, ob das ein Spielfeld ist
            var spielfeld = DataContext as IFuerSpielfeldAnzeige;
            if (spielfeld == null)
                return;

            // Elementanzeige ermitteln
            var elemente = (Canvas) GetTemplateChild( ErwarteteElemente.Elemente );
            var angezeigteElemente = elemente.Children;

            // Statische Elemente retten
            var hintergrund = elemente.Children.First();

            // Alle Elemente von der Anzeige löschen
            angezeigteElemente.Clear();

            // Hintergrund setzen
            angezeigteElemente.Add( hintergrund );

            // Alle Elemente des Spielfeldes bearbeiten
            foreach (var element in spielfeld.Elemente)
            {
                // Anzeige erzeugen
                var elementAnzeige = new BildAnzeige { DataContext = element };

                // Position binden
                elementAnzeige.SetBinding( UIElement.VisibilityProperty, new Binding( FuerElementAnzeige.Sichtbarkeit ) { Source = element } );
                elementAnzeige.SetBinding( Canvas.LeftProperty, new Binding( FuerElementAnzeige.HorizontalePosition ) { Source = element } );
                elementAnzeige.SetBinding( Canvas.TopProperty, new Binding( FuerElementAnzeige.VertikalePosition ) { Source = element } );
                elementAnzeige.SetBinding( Canvas.ZIndexProperty, new Binding( FuerElementAnzeige.Ebene ) { Source = element } );

                // Zur Anzeige bringen
                angezeigteElemente.Add( elementAnzeige );
            }
        }
    }
}
