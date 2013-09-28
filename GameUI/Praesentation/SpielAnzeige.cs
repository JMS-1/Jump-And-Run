using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Zeigt ein Spiel an.
    /// </summary>
    [TemplatePart( Name = ErwarteteElemente.Schlussbild, Type = typeof( FrameworkElement ) )]
    [TemplatePart( Name = ErwarteteElemente.Spielfeld, Type = typeof( UIElement ) )]
    [TemplatePart( Name = ErwarteteElemente.Musik, Type = typeof( MediaElement ) )]
    [TemplatePart( Name = ErwarteteElemente.Fokus, Type = typeof( UIElement ) )]
    [TemplatePart( Name = ErwarteteElemente.Schummeln, Type = typeof( Image ) )]
    public class SpielAnzeige : Control
    {
        /// <summary>
        /// Alle in der Vorlage erwarteten Oberflächenelemente.
        /// </summary>
        public static class ErwarteteElemente
        {
            /// <summary>
            /// Der Name des Elementes, das den Eingabefokus erhalten soll.
            /// </summary>
            public const string Fokus = "fokus";

            /// <summary>
            /// Der Name des Multimediaelementes zum Abspielen von Geräuschen.
            /// </summary>
            public const string Musik = "musik";

            /// <summary>
            /// Der Name des Bildes zum Schummeln - ein Blick auf das gesamte Spielfeld.
            /// </summary>
            public const string Schummeln = "cheat";

            /// <summary>
            /// Der Name des Spielfeldelementes.
            /// </summary>
            public const string Spielfeld = "spielfeld";

            /// <summary>
            /// Der Name für das Element, in dem das Schlussbild angezeigt wird.
            /// </summary>
            public const string Schlussbild = "schlussbild";

            /// <summary>
            /// Der Name einer optionalen Animation des Schlussbildes.
            /// </summary>
            public const string SchlussbildAnimation = "SchlussbildAnimation";
        }

        /// <summary>
        /// Zeigt an, dass wir uns um die Eingabe kümmern.
        /// </summary>
        private UIElement m_fokus;

        /// <summary>
        /// Hier spielt die Musik.
        /// </summary>
        private MediaElement m_musik;

        /// <summary>
        /// Erlaubt einen Blick auf das gesamte Spielfeld.
        /// </summary>
        private Image m_cheat;

        /// <summary>
        /// Das aktuelle Spielfeld.
        /// </summary>
        private UIElement m_spielfeld;

        /// <summary>
        /// Erstellt eine neue Anzeige.
        /// </summary>
        public SpielAnzeige()
        {
            // Konfiguration (XAML) laden
            DefaultStyleKey = typeof( SpielAnzeige );

            // Verzögerte Initialisierung
            DataContextChanged += ( s, a ) => MusikBereit();
        }

        /// <summary>
        /// Wird aufgerufen, sobald die Vorlage geladen wurde.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Basisklasse aufrufen
            base.OnApplyTemplate();

            // Elemente suchen
            m_spielfeld = (UIElement) GetTemplateChild( ErwarteteElemente.Spielfeld );
            m_musik = (MediaElement) GetTemplateChild( ErwarteteElemente.Musik );
            m_fokus = (UIElement) GetTemplateChild( ErwarteteElemente.Fokus );
            m_cheat = (Image) GetTemplateChild( ErwarteteElemente.Schummeln );

            // Immer reagieren, wenn eine Melodie abgespielt wurde
            m_musik.MediaEnded += ( s, a ) => MusikBereit();

            // An das Schlussbild binden
            var schlussbild = (FrameworkElement) GetTemplateChild( ErwarteteElemente.Schlussbild );
            if (schlussbild != null)
            {
                // Schauen wir mal, ob es eine Animation für das Schlussbild gibt
                var animation = schlussbild.Resources[ErwarteteElemente.SchlussbildAnimation] as Storyboard;
                if (animation != null)
                    animation.Begin();
            }

            // Eingabe anfordern
            Focus();
        }

        /// <summary>
        /// Beginnt mit dem Abspielen einer Melodie.
        /// </summary>
        /// <param name="dateiPfad">Der relative Pfad zur Datei mit der Melodie.</param>
        private void MusikStarten( string dateiPfad )
        {
            // Einfach übernehmen
            m_musik.Source = new Uri( string.Format( "../{0}", dateiPfad ), UriKind.Relative );
        }

        /// <summary>
        /// Wird aufgerufen, sobald eine neue Melodie gestartet werden könnte.
        /// </summary>
        private void MusikBereit()
        {
            // An die Steuerung melden
            Steuerung.MusikKannAbgespieltWerden( MusikStarten );
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Spielanzeige den Eingabefokus erhalten hat.
        /// </summary>
        /// <param name="e">Wird ignoriert.</param>
        protected override void OnGotFocus( RoutedEventArgs e )
        {
            // Durchreichen
            base.OnGotFocus( e );

            // Visualisieren
            if (m_fokus != null)
                m_fokus.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Spielanzeige den Eingabefokus verloren hat.
        /// </summary>
        /// <param name="e">Wird ignoriert.</param>
        protected override void OnLostFocus( RoutedEventArgs e )
        {
            // Durchreichen
            base.OnGotFocus( e );

            // Visualisieren
            if (m_fokus != null)
                m_fokus.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Wird aufgerufen, sobald eine Tasteneingabe erfolgt ist.
        /// </summary>
        /// <param name="e">Die beobachtete Taste.</param>
        protected override void OnKeyDown( KeyEventArgs e )
        {
            // Erst mal die Basis
            base.OnKeyDown( e );

            // Ups, schon erledigt
            if (e.Handled)
                return;

            // Ein kleiner Blick auf das Spielfeld
            if (e.Key == Key.F8)
            {
                // Anzeigen oder entfernen

                if (m_cheat.Source == null)
                    m_cheat.Source = new WriteableBitmap( m_spielfeld, new MatrixTransform() );
                else
                    m_cheat.Source = null;

                // Haben wir gemacht
                e.Handled = true;

                // Das war es
                return;
            }

            // Vermutlich brauchen wir die Steuerung
            var steuerung = Steuerung;

            // Der Befehl, den wir auslösen sollen
            ICommand befehl;

            // Auswerten
            switch (e.Key)
            {
                case Key.F2: befehl = steuerung.Starten.CanExecute( null ) ? steuerung.Starten : steuerung.Anhalten; break;
                case Key.Right: befehl = steuerung.SchnellerNachRechts; break;
                case Key.Left: befehl = steuerung.SchnellerNachLinks; break;
                case Key.Enter: befehl = steuerung.Stillgestanden; break;
                case Key.Space: befehl = steuerung.Springen; break;
                default: return;
            }

            // Prüfen
            e.Handled = befehl.CanExecute( null );

            // Durchreichen
            if (e.Handled)
                befehl.Execute( null );
        }

        /// <summary>
        /// Ermittelt die aktuelle Simulation.
        /// </summary>
        private IFuerSpielAnzeige Spiel { get { return (IFuerSpielAnzeige) DataContext; } }

        /// <summary>
        /// Ermittelt die aktuelle Steuerung der Simulation.
        /// </summary>
        private IFuerSpielSteuerung Steuerung { get { return Spiel.Steuerung; } }
    }
}
