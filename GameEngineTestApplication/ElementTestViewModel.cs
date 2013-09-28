using System;
using System.ComponentModel;
using System.Windows;


namespace JMS.JnRV2.Ablauf.Tests
{
    /// <summary>
    /// Präsentiert ein einzelnes Element.
    /// </summary>
    public class ElementTestViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Das zugehörige Spielfeld.
        /// </summary>
        public SpielfeldTestViewModel Spielfeld { get; private set; }

        /// <summary>
        /// Das verwaltete Element.
        /// </summary>
        protected GrundElement Element { get; private set; }

        /// <summary>
        /// Die horizontale Position des Elementes.
        /// </summary>
        private double m_x;

        /// <summary>
        /// Liest oder setzt die horizontale Position des Elementes.
        /// </summary>
        public double X { get { return m_x; } set { this.EigenschaftVerändern( PropertyChanged, "X", ref m_x, value ); } }

        /// <summary>
        /// Die vertikale Position des Elementes.
        /// </summary>
        private double m_y;

        /// <summary>
        /// Liest oder setzt die vertikale Position des Elementes.
        /// </summary>
        public double Y { get { return m_y; } set { this.EigenschaftVerändern( PropertyChanged, "Y", ref m_y, value ); } }

        /// <summary>
        /// Die vertikale Höhe des Elementes.
        /// </summary>
        private double m_hoehe;

        /// <summary>
        /// Liest oder setzt die Höhe des Elementes.
        /// </summary>
        public double Hoehe { get { return m_hoehe; } set { this.EigenschaftVerändern( PropertyChanged, "Hoehe", ref m_hoehe, value ); } }

        /// <summary>
        /// Die vertikale Breite des Elementes.
        /// </summary>
        private double m_breite;

        /// <summary>
        /// Liest oder setzt die Breite des Elementes.
        /// </summary>
        public double Breite { get { return m_breite; } set { this.EigenschaftVerändern( PropertyChanged, "Breite", ref m_breite, value ); } }

        /// <summary>
        /// Die horizontale Position des Zentrums des Elementes.
        /// </summary>
        private double m_xZentrum;

        /// <summary>
        /// Liest oder setzt die horizontale Position des Zentrums des Elementes.
        /// </summary>
        public double XZentrum { get { return m_xZentrum; } set { this.EigenschaftVerändern( PropertyChanged, "XZentrum", ref m_xZentrum, value ); } }

        /// <summary>
        /// Die vertikale Position des Zentrums des Elementes.
        /// </summary>
        private double m_yZentrum;

        /// <summary>
        /// Liest oder setzt die vertikale Position des Zentrums des Elementes.
        /// </summary>
        public double YZentrum { get { return m_yZentrum; } set { this.EigenschaftVerändern( PropertyChanged, "YZentrum", ref m_yZentrum, value ); } }

        /// <summary>
        /// Wird ausgelöst, wenn sich das Element bewegt hat.
        /// </summary>
        public event EventHandler<EventArgs> ElementHatSichBewegt;

        /// <summary>
        /// Erstellt eine neue Präsentation.
        /// </summary>
        /// <param name="element">Das zu verwaltende Element.</param>
        /// <param name="spielfeld">Die Präsentation des zugehörigen Spielfeldes.</param>
        /// <exception cref="ArgumentNullException">Es wurden nicht alle Parameter angegeben.</exception>
        public static ElementTestViewModel Erzeugen( GrundElement element, SpielfeldTestViewModel spielfeld )
        {
            // Prüfen
            if (spielfeld == null)
                throw new ArgumentNullException( "spielfeld" );
            if (element == null)
                throw new ArgumentNullException( "element" );

            // Je nach Art ausführen
            var spieler = element as Spieler;
            if (ReferenceEquals( spieler, null ))
                return new ElementTestViewModel( element, spielfeld );
            else
                return new SpielerTestViewModel( spieler, spielfeld );
        }

        /// <summary>
        /// Erstellt eine neue Präsentation.
        /// </summary>
        /// <param name="element">Das zu verwaltende Element.</param>
        /// <param name="spielfeld">Die Präsentation des zugehörigen Spielfeldes.</param>
        protected ElementTestViewModel( GrundElement element, SpielfeldTestViewModel spielfeld )
        {
            // Merken
            Spielfeld = spielfeld;
            Element = element;

            // Auf Änderungen reagieren
            Element.ZustandVerändert += e => EigenschaftVerändert( "Sichtbarkeit" );
            Element.ElementHatSichBewegt += BewegungBeendet;
        }

        /// <summary>
        /// Meldet, dass sich eine Eigenschaft verändert hat.
        /// </summary>
        /// <param name="eigenschaft">Der Name der Eigenschaft.</param>
        protected void EigenschaftVerändert( string eigenschaft )
        {
            // Melden
            PropertyChanged.EigenschaftVerändert( this, eigenschaft );
        }

        /// <summary>
        /// Meldet die Sichtbarkeit des Elementes.
        /// </summary>
        public Visibility Sichtbarkeit
        {
            get
            {
                // Zustand auswerten
                if (Element.IstDeaktiviert)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }

        /// <summary>
        /// Wird ausgelöst, wenn sich das Element bewegt hat.
        /// </summary>
        /// <param name="element">Wird ignoriert.</param>
        private void BewegungBeendet( GrundElement element )
        {
            // Weiter reichen
            ElementHatSichBewegt.EreignisAuslösen( this, EventArgs.Empty );
        }

        /// <summary>
        /// Berechnet die Dimensionierung des Elementes neu.
        /// </summary>
        /// <param name="breiteDesSpielfelds">Die neue Breite des Spielfelds.</param>
        /// <param name="höheDesSpielfeld">Die beue Höhe des Spielfelds.</param>
        public void AnzeigePositionNeuBerechnen( double breiteDesSpielfelds, double höheDesSpielfeld )
        {
            // Relative Position und Größe auslesen
            var ausdehnung = Element.Ausdehnung;
            var position = Element.Position;

            // Aktuelle Ausdehnung ermitteln
            var breite = (double) (ausdehnung.Breite * (decimal) breiteDesSpielfelds);
            var höhe = (double) (ausdehnung.Höhe * (decimal) höheDesSpielfeld);
            var halbeBreite = breite / 2;
            var halbeHöhe = höhe / 2;

            // Neu Position berechnen
            var y = (double) ((GenaueZahl.Eins - position.VertikalePosition) * (decimal) höheDesSpielfeld);
            var x = (double) (position.HorizontalePosition * (decimal) breiteDesSpielfelds);

            // Den Rest in die Bildschirmanzeige umsetzen
            X = x - halbeBreite;
            Y = y - halbeHöhe;
            XZentrum = halbeBreite;
            YZentrum = halbeHöhe;
            Breite = breite;
            Hoehe = höhe;
        }

        /// <summary>
        /// Bewegt das verwaltete Element.
        /// </summary>
        /// <param name="abstandX">Die horizontale Differenz.</param>
        /// <param name="abstandY">Die vertikale Differenz.</param>
        public void Bewegen( double abstandX, double abstandY )
        {
            // Durchreichen
            Element.Bewegen( (GenaueZahl) abstandX, (GenaueZahl) abstandY );
        }

        /// <summary>
        /// Wird ausgelöst, wenn sich eine Eigenschaft verändert hat.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
