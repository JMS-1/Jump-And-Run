using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Dieses Anzeigeelement visualisiert ein Objekt, das die Schnittstelle
    /// <see cref="IFuerBildAnzeige"/> anbietet.
    /// </summary>
    public class BildAnzeige : Control
    {
        /// <summary>
        /// Meldet oder legt fest, wie das enthaltene Bild zu skalieren ist.
        /// </summary>
        public Stretch BildSkalierung
        {
            get { return (Stretch) GetValue( BildSkalierungProperty ); }
            set { SetValue( BildSkalierungProperty, value ); }
        }

        /// <summary>
        /// Die Eigenschaft mit dem Skalierungsverhalten des Bildes.
        /// </summary>
        public static readonly DependencyProperty BildSkalierungProperty =
            DependencyProperty.Register
                (
                    "BildSkalierung",
                    typeof( Stretch ),
                    typeof( BildAnzeige ),
                    new PropertyMetadata( Stretch.None )
                );

        /// <summary>
        /// Meldet die Umrahmung der Anzeige - üblicherweise genutzt für Testzwecke.
        /// </summary>
        public Thickness BildUmrahmung
        {
            get { return (Thickness) GetValue( BildUmrahmungProperty ); }
            set { SetValue( BildUmrahmungProperty, value ); }
        }

        /// <summary>
        /// Die Eigenschaft mit der Umrahmung der Anzeige.
        /// </summary>
        public static readonly DependencyProperty BildUmrahmungProperty =
            DependencyProperty.Register
                (
                    "BildUmrahmung",
                    typeof( Thickness ),
                    typeof( BildAnzeige ),
                    new PropertyMetadata( Einstellungen.TestUmrahmung )
                );

        /// <summary>
        /// Erzeugt ein neues Element.
        /// </summary>
        public BildAnzeige()
        {
            // Konfiguration (XAML) laden
            DefaultStyleKey = typeof( BildAnzeige );
        }
    }

    /// <summary>
    /// Dieses Anzeigeelement visualisiert ein Objekt, das die Schnittstelle
    /// <see cref="IFuerBildAnzeige"/> anbietet.
    /// </summary>
    public class BildAnzeigeMitNeustartOption : BildAnzeige
    {
        /// <summary>
        /// Erzeugt eine neue Anzeige.
        /// </summary>
        public BildAnzeigeMitNeustartOption()
        {
            // Auf Befehl hin alles von vorne starten
            MouseLeftButtonUp += ( sender, args ) =>
            {
                // Mal schauen, ob wir einen Neustart erlauben
                var neustart = Application.Current.RootVisual as IAnwendungsSteuerung;
                if (neustart != null)
                    neustart.AllesVonVorne();
            };
        }
    }
}
