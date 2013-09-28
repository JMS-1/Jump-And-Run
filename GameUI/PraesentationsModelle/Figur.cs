using System;
using System.ComponentModel;
using System.Windows.Media;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Repräsentiert eine Spielfigur mit den verschiedenen Bewegungszuständen.
    /// </summary>
    public class Figur : IFuerBildAnzeige
    {
        #region Schnittstelle IFuerBildAnzeige

        /// <summary>
        /// Die Breite des Bildes.
        /// </summary>
        public double Breite { get { return m_bilder[(int) ZustandDerFigur.Ruhend].Breite; } }

        /// <summary>
        /// Die Höhe des Bildes.
        /// </summary>
        public double Hoehe { get { return m_bilder[(int) ZustandDerFigur.Ruhend].Hoehe; } }

        /// <summary>
        /// Meldet das aktuelle Bild.
        /// </summary>
        public ImageSource Bild
        {
            get
            {
                // Bild im aktuellen Zustand ermitteln
                var relativesBild = Math.Max( 0, m_aktuellesBild );
                var sequenz = m_bilder[(int) m_aktuellerZustand];

                // Bild anzeigen
                return sequenz[relativesBild % sequenz.AnzahlDerBilder];
            }
        }

        /// <summary>
        /// Wird ausgelöst, wenn sich irgendetwas verändert hat.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Für jeden möglichen Zustand wird die Bildsequenz gespeichert.
        /// </summary>
        private readonly BildFolge[] m_bilder = new BildFolge[(int) ZustandDerFigur.Anzahl];

        /// <summary>
        /// Die Anzahl der Bilder pro Sekunde.
        /// </summary>
        private readonly int m_bilderProSekunde;

        /// <summary>
        /// Die laufende Nummer des aktuellen Bildes.
        /// </summary>
        private int m_aktuellesBild;

        /// <summary>
        /// Meldet die aktuelle Spielzeit.
        /// </summary>
        internal TimeSpan SpielZeit
        {
            set
            {
                // Wir bewegen uns gar nicht
                if (m_bilderProSekunde <= 0)
                    return;

                // Neues Bild berechnen
                var bildIndex = (int) Math.Round( value.TotalSeconds * m_bilderProSekunde );
                if (bildIndex <= m_aktuellesBild)
                    return;

                // Index setzen
                m_aktuellesBild = bildIndex;

                // Wir haben nun ein neues Bild
                PropertyChanged.EigenschaftWurdeVerändert( this, FuerBildAnzeige.Bild );
            }
        }

        /// <summary>
        /// Der aktuelle Zustand der Spielfigur.
        /// </summary>
        private ZustandDerFigur m_aktuellerZustand = ZustandDerFigur.Ruhend;

        /// <summary>
        /// Meldet oder setzt den aktuellen Zustand der Spielfigur.
        /// </summary>
        internal ZustandDerFigur AktuellerZustand
        {
            set
            {
                // Es hat sich nichts verändert
                if (value == m_aktuellerZustand)
                    return;

                // Änderung durchführen
                m_aktuellerZustand = value;

                // Änderung melden
                PropertyChanged.EigenschaftWurdeVerändert( this, FuerBildAnzeige.Bild );
            }
        }

        /// <summary>
        /// Der Name der Spielfigur.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Erstellt eine neue Repräsentation.
        /// </summary>
        /// <param name="name">Der Name der Spielfigur.</param>
        /// <param name="bilderProSekunde">Die Anzahl der pro Sekunde abgespielten Bilder.</param>
        /// <param name="ruhend">Alle Bilder der Spielfigur in Ruhe.</param>
        /// <param name="nachRechts">Alle Bilder der Spielfigur, wenn diese nach rechts läuft.</param>
        /// <param name="nachLinks">Alle Bilder der Spielfigur, wenn diese nach links läuft.</param>
        /// <param name="imSprung">Alle Bilder der Spielfigur in der Luft.</param>
        /// <exception cref="ArgumentNullException">Mindestens eine Bildsequenz wurde nicht angegeben.</exception>
        internal Figur( string name, int bilderProSekunde, BildFolge ruhend, BildFolge nachRechts, BildFolge nachLinks, BildFolge imSprung )
        {
            // Prüfen
            if (bilderProSekunde < 0)
                throw new ArgumentOutOfRangeException( "bilderProSekunde" );
            if (ruhend == null)
                throw new ArgumentNullException( "ruhend" );
            if (nachRechts == null)
                throw new ArgumentNullException( "nachRechts" );
            if (nachLinks == null)
                throw new ArgumentNullException( "nachLinks" );
            if (imSprung == null)
                throw new ArgumentNullException( "imSprung" );

            // Merken
            Name = string.IsNullOrEmpty( name ) ? "Spieler" : name;
            m_bilder[(int) ZustandDerFigur.NachRechts] = nachRechts;
            m_bilder[(int) ZustandDerFigur.NachLinks] = nachLinks;
            m_bilder[(int) ZustandDerFigur.InDerLuft] = imSprung;
            m_bilder[(int) ZustandDerFigur.Ruhend] = ruhend;
            m_bilderProSekunde = bilderProSekunde;

            // Auf Änderungen überwachen
            ruhend.PropertyChanged += ReferenzBildWurdeVerändert;
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich die Daten des Referenzbilds verändert haben.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Information zur Veränderung.</param>
        private void ReferenzBildWurdeVerändert( object sender, PropertyChangedEventArgs e )
        {
            // Wir überwachen nur Breite und Höhe
            if (!FuerBildAnzeige.Breite.Equals( e.PropertyName ))
                if (!FuerBildAnzeige.Höhe.Equals( e.PropertyName ))
                    return;

            // Durchreichen
            PropertyChanged.EigenschaftWurdeVerändert( this, e.PropertyName );
        }
    }
}
