using System.Windows;


namespace JMS.JnRV2.Ablage.V1
{
    /// <summary>
    /// Beschreibt ein sichtbares Element auf dem Spielfeld.
    /// </summary>
    public class BildElement : Element
    {
        /// <summary>
        /// Meldet oder setzt den Wert des Elements.
        /// </summary>
        public int Wert
        {
            get { return (int) GetValue( WertProperty ); }
            set { SetValue( WertProperty, value ); }
        }

        /// <summary>
        /// Die Eigenschaft mit dem Wert des Elements.
        /// </summary>
        public static readonly DependencyProperty WertProperty =
            DependencyProperty.Register
                (
                    "Wert",
                    typeof( int ),
                    typeof( BildElement ),
                    new PropertyMetadata( 0 )
                );

        /// <summary>
        /// Alle Bilder zu diesem Element.
        /// </summary>
        public BilderFeld Bilder
        {
            get { return (BilderFeld) GetValue( BilderProperty ); }
            set { SetValue( BilderProperty, value ); }
        }

        /// <summary>
        /// Die Eigenschaft mit den Bildern zu diesem Element.
        /// </summary>
        public static readonly DependencyProperty BilderProperty =
            DependencyProperty.Register
                (
                    "Bilder",
                    typeof( BilderFeld ),
                    typeof( BildElement ),
                    new PropertyMetadata( new BilderFeld() )
                );

        /// <summary>
        /// Legt fest, wie viele Bilder pro Sekunde angezeigt werden sollen.
        /// </summary>
        public int BilderProSekunde
        {
            get { return (int) GetValue( BilderProSekundeProperty ); }
            set { SetValue( BilderProSekundeProperty, value ); }
        }

        /// <summary>
        /// Die Eigenschaft mit der Anzahl der Bilder pro Sekunde.
        /// </summary>
        public static readonly DependencyProperty BilderProSekundeProperty =
            DependencyProperty.Register
                (
                    "BilderProSekunde",
                    typeof( int ),
                    typeof( BildElement ),
                    new PropertyMetadata( 0 )
                );


        /// <summary>
        /// Meldet oder setzt die Art der Bewegung.
        /// </summary>
        public Bewegung Bewegung
        {
            get { return (Bewegung) GetValue( BewegungProperty ); }
            set { SetValue( BewegungProperty, value ); }
        }

        /// <summary>
        /// Die Eigenschaft mit der Art der Bewegung.
        /// </summary>
        public static readonly DependencyProperty BewegungProperty =
            DependencyProperty.Register
                (
                    "Bewegung",
                    typeof( Bewegung ),
                    typeof( BildElement ),
                    new PropertyMetadata( null )
                );

        /// <summary>
        /// Meldet oder setzt die Darstellungsebene des Elementes.
        /// </summary>
        public int Ebene
        {
            get { return (int) GetValue( EbeneProperty ); }
            set { SetValue( EbeneProperty, value ); }
        }

        /// <summary>
        /// Die Eigenschaft mit der Darstellungsebene des Elementes.
        /// </summary>
        public static readonly DependencyProperty EbeneProperty =
            DependencyProperty.Register
                (
                    "Ebene",
                    typeof( int ),
                    typeof( BildElement ),
                    new PropertyMetadata( -1 )
                );
    }
}
