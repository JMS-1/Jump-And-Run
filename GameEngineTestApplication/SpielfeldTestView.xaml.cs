using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace JMS.JnRV2.Ablauf.Tests
{
    /// <summary>
    /// Zeigt ein Spielfeld an.
    /// </summary>
    partial class SpielfeldTestView
    {
        /// <summary>
        /// Erstellt eine neue Anzeige.
        /// </summary>
        public SpielfeldTestView()
        {
            // Konfiguration (XAML) laden
            InitializeComponent();

            // Auf Veränderungen reagieren
            DataContextChanged += SpielfeldWurdeVerändert;
            SizeChanged += GrößeWurdeVerändert;
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich die Größe der Anzeige verändert hat.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Die Informationen zur Veränderung.</param>
        private void GrößeWurdeVerändert( object sender, SizeChangedEventArgs e )
        {
            // Anzeige aktualisieren
            AlleElementeNeuPositionieren();
        }

        /// <summary>
        /// Berechnet die Positionen aller Elemente neu.
        /// </summary>
        private void AlleElementeNeuPositionieren()
        {
            // Alle Elemente besuchen
            foreach (var element in Elemente)
                ElementNeuPositionieren( element );
        }

        /// <summary>
        /// Meldet alle Elemente, die auf diesem Spielfeld vorhanden sind.
        /// </summary>
        private IEnumerable<ElementTestViewModel> Elemente { get { return Children.OfType<ElementTestView>().Select( anzeige => anzeige.Element ); } }

        /// <summary>
        /// Berechent die Anzeige eines einzelnen Elements neu.
        /// </summary>
        /// <param name="element">Das zu bearbeitende Element.</param>
        private void ElementNeuPositionieren( ElementTestViewModel element )
        {
            // Informationen noch nicht verfügbar
            var width = ActualWidth;
            if (double.IsNaN( width ))
                return;
            var height = ActualHeight;
            if (double.IsNaN( height ))
                return;

            // Das Element kann sich selbst anzeigen
            element.AnzeigePositionNeuBerechnen( width, height );
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich das zugehörige Spielfeld verändert hat.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Wird ignoriert.</param>
        private void SpielfeldWurdeVerändert( object sender, DependencyPropertyChangedEventArgs e )
        {
            // Erst einmal aufräumen
            Children.Clear();

            // Neues Spielfeld ermitteln
            var spielfeld = DataContext as SpielfeldTestViewModel;
            if (spielfeld == null)
                return;

            // Elemente laden
            foreach (var element in spielfeld.Elemente)
                Children.Add( new ElementTestView { DataContext = element } );

            // Auf Änderungen reagieren
            foreach (var element in Elemente)
                element.ElementHatSichBewegt += ( s, a ) => ElementNeuPositionieren( element );

            // Erstmalige Anzeige
            AlleElementeNeuPositionieren();
        }
    }
}
