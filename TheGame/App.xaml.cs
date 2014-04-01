using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Media;
using JMS.JnRV2.Anzeige;


namespace JMS.JnRV2.Start
{
    /// <summary>
    /// Repräsentiert die Anwendung als Ganzes.
    /// </summary>
    partial class App
    {
        /// <summary>
        /// Der Name der Arbeitsumgebung.
        /// </summary>
        const string AuswahlName = "auswahl";

        /// <summary>
        /// Die visuelle Umgebung des Spielfelds.
        /// </summary>
        private class VisuelleUmgebung : ContentControl, IAnwendungsSteuerung
        {
            /// <summary>
            /// Die Parameter der Anwendung.
            /// </summary>
            private StartupEventArgs m_parameter;

            /// <summary>
            /// Führt einen Neustart der Anwendung aus.
            /// </summary>
            public void AllesVonVorne()
            {
                // Aufräumen
                Application.Current.Resources.Remove( AuswahlName );

                // Und los
                Starten( m_parameter );
            }

            /// <summary>
            /// Führt einen Neustart der Anwendung aus.
            /// </summary>
            /// <param name="e">Die Startparameter.</param>
            public void Starten( StartupEventArgs e )
            {
                // Das merken wir uns erst einmal
                m_parameter = e;

                // Statische Konfiguration auswerten
                AuswahlInformationen.NameDesSpiels = e.InitParams["Spiel"];

                // Die Arbeitsumgebung anlegen
                Application.Current.Resources.Add( AuswahlName, new AuswahlInformationen() );

                // Dynamische Konfiguration auswerten
                string testRahmenAnzeigen;
                if (e.InitParams.TryGetValue( "TestRahmen", out testRahmenAnzeigen ))
                {
                    // Wandeln und setzen
                    bool anzeigen;
                    if (bool.TryParse( testRahmenAnzeigen, out anzeigen ))
                        if (anzeigen)
                            Einstellungen.TestUmrahmung = new Thickness( 1 );
                }

                // Konfigurieren
                Content = new SpielAuswahl();
            }
        }

        /// <summary>
        /// Die visuelle Umgebung des Spielfelds.
        /// </summary>
        private readonly VisuelleUmgebung m_umgebung;

        /// <summary>
        /// Erstellt eine neue Anwendung.
        /// </summary>
        public App()
        {
            // Ereignisse überwachen
            Startup += AnwendungStarten;
            UnhandledException += UnerwarteteAusnahmeBearbeiten;

            // Konfiguration (XAML) laden
            InitializeComponent();

            // Visuelle Umgebung erzeugen
            m_umgebung =
                new VisuelleUmgebung
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = new SolidColorBrush( Colors.Red ),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Visibility = Visibility.Visible,
                };
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Anwendung zum Start bereit ist.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Beschreibt die Konfiguration der Anwendung.</param>
        private void AnwendungStarten( object sender, StartupEventArgs e )
        {
            // Vorbereiten
            m_umgebung.Starten( e );

            // Und verwenden
            RootVisual = m_umgebung;
        }

        /// <summary>
        /// Bearbeitet unerwartete Fehler.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Beschreibt den beobachteten Fehler.</param>
        private void UnerwarteteAusnahmeBearbeiten( object sender, ApplicationUnhandledExceptionEventArgs e )
        {
            // Wenn wir gerade im Testmodus sind, brauchen wird eigentlich nichts zu tun
            if (Debugger.IsAttached)
                return;

            // Fehler als bearbeitet markieren
            e.Handled = true;

            // Fehler anzeigen
            Deployment.Current.Dispatcher.BeginInvoke( () => AusnahmeInHTMLAnzeigen( e ) );
        }

        /// <summary>
        /// Zeigt einen Fehler an.
        /// </summary>
        /// <param name="e">Informationen zum Fehler.</param>
        private void AusnahmeInHTMLAnzeigen( ApplicationUnhandledExceptionEventArgs e )
        {
            // Besser, es schlägt nicht noch einmal fehl!
            try
            {
                // Nachricht erstellen
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;

                // Zur Anzeige vorbereiten
                errorMsg = errorMsg.Replace( '"', '\'' ).Replace( "\r\n", @"\n" );

                // Anzeigen
                HtmlPage.Window.Eval( "throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");" );
            }
            catch (Exception)
            {
                // Tja, da können wir nichts mehr machen
            }
        }
    }
}
