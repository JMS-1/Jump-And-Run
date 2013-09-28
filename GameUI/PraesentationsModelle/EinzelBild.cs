using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Lädt ein Bild dynamisch und setzt dann die entsprechenden Dimensionen.
    /// </summary>
    public class EinzelBild : IFuerBildAnzeige
    {
        #region Schnittstelle IFuerBildAnzeige

        /// <summary>
        /// Die Breite des Bildes.
        /// </summary>
        public double Breite { get { return m_breite; } private set { PropertyChanged.EigenschaftVerändern( this, FuerBildAnzeige.Breite, ref m_breite, value ); } }

        /// <summary>
        /// Die Höhe des Bildes.
        /// </summary>
        public double Hoehe { get { return m_höhe; } private set { PropertyChanged.EigenschaftVerändern( this, FuerBildAnzeige.Höhe, ref m_höhe, value ); } }

        /// <summary>
        /// Meldet das aktuelle Bild.
        /// </summary>
        public ImageSource Bild { get { return m_image; } }

        /// <summary>
        /// Wird ausgelöst, wenn sich irgendetwas verändert hat.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Der Name der Eigenschaft zur Verfügbarkeit des zugehörigen Bildes.
        /// </summary>
        internal static readonly string _IstVerfügbar = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( EinzelBild i ) => i.IstVerfügbar );

        /// <summary>
        /// Die Breite des Bildes.
        /// </summary>
        private double m_breite = double.NaN;

        /// <summary>
        /// Die Höhe des Bildes.
        /// </summary>
        private double m_höhe = double.NaN;

        /// <summary>
        /// Gesetzt, sobald das zugehörige Bild geladen wurde.
        /// </summary>
        private bool m_verfügbar;

        /// <summary>
        /// Meldet, ob das zugehörige Bild geladen wurden.
        /// </summary>
        internal bool IstVerfügbar { get { return m_verfügbar; } private set { PropertyChanged.EigenschaftVerändern( this, _IstVerfügbar, ref m_verfügbar, value ); } }

        /// <summary>
        /// Das aktuelle Bild.
        /// </summary>
        private readonly BitmapImage m_image;

        /// <summary>
        /// Erzeugt eine neue Verwaltung.
        /// </summary>
        /// <param name="relativerPfadZumBild">Die Ablage des Bildes.</param>
        internal EinzelBild( string relativerPfadZumBild )
        {
            // Prüfen
            if (string.IsNullOrEmpty( relativerPfadZumBild ))
                throw new ArgumentNullException( "relativerPfadZumBild" );

            // Aktuelles Bild einmalig erzeugen
            m_image = new BitmapImage { CreateOptions = BitmapCreateOptions.None };

            // Auf Änderungen reagieren
            m_image.ImageOpened += BildWurdeGeladen;

            // Pfad setzen
            m_image.UriSource = new System.Uri( relativerPfadZumBild, UriKind.Relative );
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Bild geladen wurde.
        /// </summary>
        /// <param name="sender">Die Quelle des Ladevorgangs.</param>
        /// <param name="e">Wird ignoriert.</param>
        private void BildWurdeGeladen( object sender, RoutedEventArgs e )
        {
            // Werte überschreiben
            Breite = m_image.PixelWidth;
            Hoehe = m_image.PixelHeight;

            // Wir haben nun ein Bild
            IstVerfügbar = true;
        }
    }
}
