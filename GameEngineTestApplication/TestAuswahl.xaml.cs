using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using JMS.JnRV2.Ablauf.Tests.Abläufe;
using JMS.JnRV2.Ablauf.Tests.Altlasten;
using JMS.JnRV2.Ablauf.Tests.Kollisionen;


namespace JMS.JnRV2.Ablauf.Tests
{
    /// <summary>
    /// Mit Hilfe dieser Klasse wird ein Test ausgewählt.
    /// </summary>
    partial class TestAuswahl
    {
        /// <summary>
        /// Die möglichen Auswahlalternativen.
        /// </summary>
        public static TestBeschreibung[] _Tests = 
            {
                new TestBeschreibung { AuswahlText = "(kein Test)" },
                new TestBeschreibung<ProbeSpielen> { AuswahlText = "Spielen" },
                new TestBeschreibung<AltesFormatAnzeigen> { AuswahlText = "Alte Spiele" },
                new TestBeschreibung<VollständigerTest>  { AuswahlText = "Abläufe" },
                new TestBeschreibung<KollisionsPrüfung> { AuswahlText = "Kollisionen" },
            };

        /// <summary>
        /// Beschreibt einen einzelnen Test.
        /// </summary>
        public class TestBeschreibung
        {
            /// <summary>
            /// Der Name, unter dem der Text erscheint.
            /// </summary>
            public string AuswahlText { get; set; }

            /// <summary>
            /// Erstellte einen neuen Test.
            /// </summary>
            /// <returns>Der gewünschte Test.</returns>
            public virtual UIElement ErzeugeTest() { return null; }
        }

        /// <summary>
        /// Beschreibt einen einzelnen Test.
        /// </summary>
        /// <typeparam name="TArtDesTests">Die Art des Tests.</typeparam>
        public class TestBeschreibung<TArtDesTests> : TestBeschreibung where TArtDesTests : UIElement, new()
        {
            /// <summary>
            /// Erstellte einen neuen Test.
            /// </summary>
            /// <returns>Der gewünschte Test.</returns>
            public override UIElement ErzeugeTest() { return new TArtDesTests(); }
        }

        /// <summary>
        /// Meldet den aktuellen Test.
        /// </summary>
        public UIElement AktuellerTest
        {
            get { return (UIElement) GetValue( AktuellerTestProperty ); }
            private set { SetValue( AktuellerTestProperty, value ); }
        }

        /// <summary>
        /// Diese Eigenschaft beschreibt den aktuellen Test.
        /// </summary>
        public static readonly DependencyProperty AktuellerTestProperty =
            DependencyProperty.Register
            (
                "AktuellerTest",
                typeof( UIElement ),
                typeof( TestAuswahl ),
                new PropertyMetadata( null )
            );

        /// <summary>
        /// Alle Testbeschreibungen, die zur Auswahl angeboten werden sollen.
        /// </summary>
        public TestBeschreibung[] TestBeschreibungen { get; private set; }

        /// <summary>
        /// Erstellt eine neue Auswahl.
        /// </summary>
        public TestAuswahl()
        {
            // Fertig stellen
            TestBeschreibungen = _Tests;

            // Konfiguration (XAML) laden
            InitializeComponent();           
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein neuer Test geladen werden soll.
        /// </summary>
        /// <param name="sender">Das veränderte Element..</param>
        /// <param name="e">Beschreibt die Veränderung der Auswahl.</param>
        private void NeuenTestLaden( object sender, SelectionChangedEventArgs e )
        {
            // Neue Auswahl
            var liste = (Selector) sender;
            var auswahl = liste.SelectedItem as TestBeschreibung;

            // Aktivieren
            AktuellerTest = (auswahl == null) ? null : auswahl.ErzeugeTest();
        }
    }
}
