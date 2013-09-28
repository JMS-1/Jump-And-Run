using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Browser;


namespace JMS.JnRV2.Ablauf.Tests
{
    /// <summary>
    /// Repräsentiert die Testanwenundung als Ganzes.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Erstellt eine neue Anwendung.
        /// </summary>
        public App()
        {
            // Ereignisse anmelden
            UnhandledException += Application_UnhandledException;
            Startup += ( s, e ) => RootVisual = new TestAuswahl();

            // Konfiguration (XAML) laden
            InitializeComponent();
        }

        /// <summary>
        /// Bearbeitet unerwartete Fehler.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Beschreibt den beobachteten Fehler.</param>
        private void Application_UnhandledException( object sender, ApplicationUnhandledExceptionEventArgs e )
        {
            // Wenn wir gerade im Testmodus sind, brauchen wird eigentlich nichts zu tun
            if (Debugger.IsAttached)
                return;

            // Fehler als bearbeitet markieren
            e.Handled = true;

            // Fehler anzeigen
            Deployment.Current.Dispatcher.BeginInvoke( () => ReportErrorToDOM( e ) );
        }

        /// <summary>
        /// Zeigt einen Fehler an.
        /// </summary>
        /// <param name="e">Informationen zum Fehler.</param>
        private void ReportErrorToDOM( ApplicationUnhandledExceptionEventArgs e )
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
