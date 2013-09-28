using System.Windows;
using System.Xml.Serialization;


namespace JMS.JnRV2.Ablage.V1
{
    /// <summary>
    /// Beschreibt ein beliebiges Element auf dem Spielfeld.
    /// </summary>
    public class Element : FrameworkElement
    {
        /// <summary>
        /// Meldet oder setzt die Art des Elements.
        /// </summary>
        public ElementArt ElementArt
        {
            get { return (ElementArt) GetValue( ElementArtProperty ); }
            set { SetValue( ElementArtProperty, value ); }
        }

        /// <summary>
        /// Die Eigenschaft mit der Art des Elements
        /// </summary>
        public static readonly DependencyProperty ElementArtProperty =
            DependencyProperty.Register
                (
                    "ElementArt",
                    typeof( ElementArt ),
                    typeof( Element ),
                    new PropertyMetadata( null )
                );

        /// <summary>
        /// Der relative Pfad zur Datei mit der Melodie, die bei Kollision mit dem Element abgespielt wird.
        /// </summary>
        public string Melodie
        {
            get { return (string) GetValue( MelodieProperty ); }
            set { SetValue( MelodieProperty, value ); }
        }

        /// <summary>
        /// Die Eigenschaft mit dem relativen Pfad der Kollisionsmelodie.
        /// </summary>
        public static readonly DependencyProperty MelodieProperty =
            DependencyProperty.Register
                (
                    "Melodie",
                    typeof( string ),
                    typeof( Element ),
                    new PropertyMetadata( null )
                );

        /// <summary>
        /// Die Differenz der Lebensenergie, die bei Kollision mit diesem Element angerechnet wird.
        /// </summary>
        public int Lebensenergie
        {
            get { return (int) GetValue( LebensenergieProperty ); }
            set { SetValue( LebensenergieProperty, value ); }
        }

        /// <summary>
        /// Die Eigenschaft mit der Differenz der Lebensenergie.
        /// </summary>
        public static readonly DependencyProperty LebensenergieProperty =
            DependencyProperty.Register
                (
                    "Lebensenergie",
                    typeof( int ),
                    typeof( Element ),
                    new PropertyMetadata( 0 )
                );

        /// <summary>
        /// Die konkrete Art und damit Konfiguration des Elementes.
        /// </summary>
        private string m_typ;

        /// <summary>
        /// Die konkrete Art und damit Konfiguration des Elementes.
        /// </summary>
        public string Typ
        {
            get
            {
                // Melden
                return m_typ;
            }
            set
            {
                // Ändern
                m_typ = value;

                // Zugehörige Konfiguration ermitteln und anwenden
                var konfiguration = (Style) Application.Current.Resources[m_typ];
                if (konfiguration != null)
                    Style = konfiguration;
            }
        }

        /// <summary>
        /// Die horizontale Position des Elementes.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Die vertikale Position des Elementes.
        /// </summary>
        public int Bottom { get; set; }
    }
}
