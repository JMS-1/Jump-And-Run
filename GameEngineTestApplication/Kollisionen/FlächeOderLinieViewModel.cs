using System.ComponentModel;


namespace JMS.JnRV2.Ablauf.Tests.Kollisionen
{
    /// <summary>
    /// Beschreibt ein Fläche auf dem Spielfeld.
    /// </summary>
    public class FlächeOderLinieViewModel : INotifyPropertyChanged, IKollisionsTestElement
    {
        /// <summary>
        /// Beschreibt die Fläche.
        /// </summary>
        public Fläche Fläche { get; private set; }

        /// <summary>
        /// Die absolute Breite des Spielfeldes.
        /// </summary>
        private decimal m_absoluteBreite;

        /// <summary>
        /// Die absolute Höhe des Spielfeldes.
        /// </summary>
        private decimal m_absoluteHoehe;

        /// <summary>
        /// Gesetzt, wenn das Hindernis im Weg steht.
        /// </summary>
        public bool m_getroffen;

        /// <summary>
        /// Meldet oder legt fest, ob dieses Hindernis bei einer Bewegung berührt würde.
        /// </summary>
        public bool Getroffen { get { return m_getroffen; } set { this.EigenschaftVerändern( PropertyChanged, "Farbe", ref m_getroffen, value ); } }

        /// <summary>
        /// Erstellt eine neue Beschreibung.
        /// </summary>
        /// <param name="fläche">Die zu verwaltende Fläche.</param>
        public FlächeOderLinieViewModel( Fläche fläche )
        {
            // Merken
            Fläche = fläche;
        }

        /// <summary>
        /// Legt die Ausdehnung des Spielfelds fest.
        /// </summary>
        /// <param name="breite">Die absolute Breite.</param>
        /// <param name="höhe">Die absolute Höhe.</param>
        public void SpielfeldAusdehnungFestlegen( decimal breite, decimal höhe )
        {
            // Merken
            m_absoluteBreite = breite;
            m_absoluteHoehe = höhe;

            // Änderungen melden
            PropertyChanged.EigenschaftVerändert( this, "Links" );
            PropertyChanged.EigenschaftVerändert( this, "Oben" );
            PropertyChanged.EigenschaftVerändert( this, "Breite" );
            PropertyChanged.EigenschaftVerändert( this, "Hoehe" );
        }

        /// <summary>
        /// Meldet die minimale horizontale Position.
        /// </summary>
        public decimal Links { get { return (decimal) (Fläche.Position.HorizontalePosition - Fläche.Ausdehnung.Breite / 2) * m_absoluteBreite; } }

        /// <summary>
        /// Meldet die minimale vertikale Position.
        /// </summary>
        public decimal Oben { get { return (decimal) (GenaueZahl.Eins - (Fläche.Position.VertikalePosition + Fläche.Ausdehnung.Höhe / 2)) * m_absoluteHoehe; } }

        /// <summary>
        /// Meldet die Breite.
        /// </summary>
        public decimal Breite { get { return (decimal) Fläche.Ausdehnung.Breite * m_absoluteBreite; } }

        /// <summary>
        /// Meldet die Höhe.
        /// </summary>
        public decimal Hoehe { get { return (decimal) Fläche.Ausdehnung.Höhe * m_absoluteHoehe; } }

        /// <summary>
        /// Meldet die Farbe.
        /// </summary>
        public string Farbe
        {
            get
            {
                // Schauen wir mal, ob wir im Weg stehen
                if (m_getroffen)
                    return "Yellow";
                else
                    return "LightGreen";
            }
        }

        /// <summary>
        /// Wird ausgelöst, wenn sich eine Eigenschaft verändert hat.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
