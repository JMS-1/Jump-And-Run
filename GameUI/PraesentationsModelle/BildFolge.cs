using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Verwaltet eine Liste von Bildern.
    /// </summary>
    public class BildFolge : IFuerBildAnzeige
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
        public ImageSource Bild { get { return this[0]; } }

        /// <summary>
        /// Wird ausgelöst, wenn sich irgendetwas verändert hat.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Die Breite des Bildes.
        /// </summary>
        private double m_breite = double.NaN;

        /// <summary>
        /// Die Höhe des Bildes.
        /// </summary>
        private double m_höhe = double.NaN;

        /// <summary>
        /// Die Ablage der Bilder.
        /// </summary>
        private readonly string[] m_pfade;

        /// <summary>
        /// Alle Bilder.
        /// </summary>
        private readonly BitmapImage[] m_bilder;

        /// <summary>
        /// Meldet die Anzahl der Bilder.
        /// </summary>
        internal int AnzahlDerBilder { get { return m_bilder.Length; } }

        /// <summary>
        /// Meldet ein bestimmtes Bild.
        /// </summary>
        /// <param name="index">Die laufende Nummer des Bildes.</param>
        /// <exception cref="ArgumentOutOfRangeException">Das gewünschte Bild existiert nicht.</exception>
        internal ImageSource this[int index] { get { return BildAnfordern( index ); } }

        /// <summary>
        /// Meldet ein bestimmtes Bild.
        /// </summary>
        /// <param name="index">Die laufende Nummer des Bildes.</param>
        /// <exception cref="ArgumentOutOfRangeException">Das gewünschte Bild existiert nicht.</exception>
        private ImageSource BildAnfordern( int index )
        {
            // Prüfen
            if (index < 0)
                throw new ArgumentOutOfRangeException( "index" );
            if (index >= m_bilder.Length)
                throw new ArgumentOutOfRangeException( "index" );

            // Vielleicht haben wir das schon
            var bild = m_bilder[index];
            if (bild != null)
                return bild;

            // Anlegen
            bild = new BitmapImage { CreateOptions = BitmapCreateOptions.None };

            // Vermerken damit wir es nur einmal anlegen
            m_bilder[index] = bild;

            // Wir überwachen immer nur das allererste Bild
            if (index == 0)
                bild.ImageOpened += EinBildWurdeGeladen;

            // Und Speicherort zuordnen
            bild.UriSource = new Uri( m_pfade[index], UriKind.Relative );

            // Neues Bild melden
            return bild;
        }

        /// <summary>
        /// Erzeugt eine neue Verwaltung.
        /// </summary>
        /// <param name="relativePfadeZuDenBildern">Die Ablage der Bilder.</param>
        internal BildFolge( params string[] relativePfadeZuDenBildern )
        {
            // Merken
            m_pfade = relativePfadeZuDenBildern ?? new string[0];

            // Prüfen
            if (m_pfade.Length < 1)
                throw new ArgumentOutOfRangeException( "relativePfadeZuDenBildern" );
            if (m_pfade.Any( pfad => string.IsNullOrEmpty( pfad ) ))
                throw new ArgumentNullException( "relativePfadeZuDenBildern" );

            // Bilderspeicher vorbereiten
            m_bilder = new BitmapImage[m_pfade.Length];

            // Erstes Bild anfordern - dieses legt die Größe für alle anderen Bilder fest
            BildAnfordern( 0 );
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Bild geladen wurde.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Wird ignoriert.</param>
        private void EinBildWurdeGeladen( object sender, RoutedEventArgs e )
        {
            // Bild ermitteln
            var bild = (BitmapImage) sender;

            // Werte überschreiben
            Breite = bild.PixelWidth;
            Hoehe = bild.PixelHeight;
        }
    }
}
