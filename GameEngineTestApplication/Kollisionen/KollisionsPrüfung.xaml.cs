using System.Windows;


namespace JMS.JnRV2.Ablauf.Tests.Kollisionen
{
    /// <summary>
    /// Überprüft das korrekte Arbeiten der Kollisionsalgorithmen.
    /// </summary>
    partial class KollisionsPrüfung
    {
        /// <summary>
        /// Die Beschreibung der Bewegung.
        /// </summary>
        public BewegungViewModel Bewegung { get; private set; }

        /// <summary>
        /// Erstellt ein neues Fenster.
        /// </summary>
        public KollisionsPrüfung()
        {
            // Fertigstellen
            Bewegung = new BewegungViewModel();

            // Konfiguration (XAML) laden
            InitializeComponent();

            // Hindernisse anlegen
            container.Children.Add( HindernisView.Erzeugen( 0.3m, 0.5m, 0.06m, 0.06m ) );
            container.Children.Add( HindernisView.Erzeugen( 0.35m, 0.35m, 0.06m, 0.06m ) );
            container.Children.Add( HindernisView.Erzeugen( 0.4m, 0.17m, 0.06m, 0.06m ) );
            container.Children.Add( HindernisView.Erzeugen( 0.6m, 0.8m, 0.1m, 0.02m ) );
            container.Children.Add( HindernisView.Erzeugen( 0.68m, 0.7m, 0.1m, 0.2m ) );

            // Abwarten
            container.Loaded += ElementeStehenBereit;
        }

        /// <summary>
        /// Wird aufgerufen, wenn alle Elemente auf dem Spielfeld bereit stehen.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Wird ignoriert.</param>
        private void ElementeStehenBereit( object sender, RoutedEventArgs e )
        {
            // Verbinden
            foreach (FrameworkElement elementView in container.Children)
            {
                // An die Daten binden
                var element = ((IKollisionsTestElement) elementView.DataContext);

                // Anmelden
                container.SizeChanged += ( s, a ) => element.SpielfeldAusdehnungFestlegen( (decimal) container.ActualWidth, (decimal) container.ActualHeight );

                // Initial positionieren
                element.SpielfeldAusdehnungFestlegen( (decimal) container.ActualWidth, (decimal) container.ActualHeight );
            }
        }
    }
}
