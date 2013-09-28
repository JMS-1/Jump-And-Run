using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JMS.JnRV2.Ablage;
using JMS.JnRV2.Anzeige.PraesentationsModelle;
using JMS.JnRV2.Anzeige.Verbinder;


namespace JMS.JnRV2.Ablauf.Tests.Abläufe
{
    /// <summary>
    /// Eine Testumgebung für ein Probespiel.
    /// </summary>
    partial class ProbeSpielen
    {
        /// <summary>
        /// Alle Spielfiguren, die wir gefunden haben.
        /// </summary>
        private Ablage.Spielfigur[] m_spielfiguren;

        /// <summary>
        /// Erstellt eine neue Testumgebung.
        /// </summary>
        public ProbeSpielen()
        {
            // Konfiguration (XAML) laden
            InitializeComponent();

            // Wir müssen auch aufräumen
            Unloaded += ( s, a ) => AufräumenBeimBeenden();

            // Direkt schon einmal die Figuren laden
            DateiLader.SpielfigurenLaden( figuren =>
                {
                    // Nichts gefunden
                    if (figuren == null)
                        return;

                    // Merken
                    m_spielfiguren = figuren;

                    // Wandeln und zuweisen
                    m_figuren.ItemsSource = figuren.Select( OberflächenVerbinder.ErzeugePräsentation ).ToArray();
                } );
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Anzeige geschlossen wird.
        /// </summary>
        private void AufräumenBeimBeenden()
        {
            // Wir verwenden den Befehl, den uns die Simulation anbietet
            var anhalten = m_anhalten.Command;
            if (anhalten != null)
                anhalten.Execute( null );
        }

        /// <summary>
        /// Versucht, das angegebene Spiel zu Laden.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Wird ignoriert.</param>
        private void SpielLaden( object sender, RoutedEventArgs e )
        {
            // Es gibt keinen Namen
            var nameDesSpiels = m_nameDesSpiels.Text;
            if (string.IsNullOrEmpty( nameDesSpiels ))
                return;

            // Es gibt keine Figur
            var figurIndex = m_figuren.SelectedIndex;
            if (figurIndex < 0)
                return;

            // Wandeln
            var schaltfläche = (Button) sender;

            // Das geht nur ein einziges Mal
            schaltfläche.IsEnabled = false;

            // Im Hintergrund laden
            DateiLader.SpielLaden( nameDesSpiels, null, spiel => DataContext = spiel.ErzeugePräsentation( m_spielfiguren[figurIndex] ) );
        }
    }
}
