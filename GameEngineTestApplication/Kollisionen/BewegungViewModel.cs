using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JMS.JnRV2.Ablauf.Kollisionen;


namespace JMS.JnRV2.Ablauf.Tests.Kollisionen
{
    /// <summary>
    /// Beschreibt ein Element in Bewegung.
    /// </summary>
    public class BewegungViewModel : INotifyPropertyChanged, IKollisionsTestElement
    {
        /// <summary>
        /// Beschreibt die aktuelle Bewegung.
        /// </summary>
        private ElementInBewegung m_bewegung;

        /// <summary>
        /// Die absolute Breite des Spielfeldes.
        /// </summary>
        private decimal m_absoluteBreite;

        /// <summary>
        /// Die absolute Höhe des Spielfeldes.
        /// </summary>
        private decimal m_absoluteHoehe;

        /// <summary>
        /// Berechnet die Kollisionen aller Element auf dem Spielfeld mit diesem Bewegungsprofil.
        /// </summary>
        public ICommand KollisionenBerechnen { get; private set; }

        /// <summary>
        /// Findet den Weg durch die Hindernisse.
        /// </summary>
        public ICommand WegFinden { get; private set; }

        /// <summary>
        /// Erstellt eine neue Beschreibung.
        /// </summary>
        public BewegungViewModel()
        {
            // Befehle anlegen
            KollisionenBerechnen = Werkzeuge.WandeleZuBefehl( ( IList<UIElement> liste ) => ErmitteleKollisionen( liste ) );
            WegFinden = Werkzeuge.WandeleZuBefehl( ( IList<UIElement> liste ) => FindDenBestenWeg( liste ) );

            // Testumgebung erstellen
            m_bewegung =
                ElementInBewegung.Erzeugen
                (
                    Fläche.Erzeugen(
                        Position.Erzeugen( Ablauf.GenaueZahl.Eins * 0.3m, Ablauf.GenaueZahl.Eins * 0.2m ),
                        Ausdehnung.Erzeugen( Ablauf.GenaueZahl.Eins * 0.12m, Ablauf.GenaueZahl.Eins * 0.06m ) ),
                    Ablauf.GenaueZahl.Eins * 0.2m,
                    Ablauf.GenaueZahl.Eins * 0.3m
                );

            // Selbstüberwachung
            PropertyChanged += EinstellungenWurdenVerändert;
        }

        /// <summary>
        /// Findet den besten Weg durch eine Reihe von Hindernissen.
        /// </summary>
        /// <param name="liste">Alle Hindernisse.</param>
        private void FindDenBestenWeg( IList<UIElement> liste )
        {
            // Erst einmal alle Hindernisse finden
            var hindernisse = ErmitteleKollisionen( liste );

            // Kollisionen ermitteln
            var kollisionen = m_bewegung.BeginneAnalyse( hindernisse );
            var pfad = kollisionen.PfadErmitteln().ToArray();
            if (pfad.Length < 1)
                return;

            // Da geht es los
            var absoluterUrsprung = m_bewegung.Fläche.Position;

            // Alle Schritte
            foreach (var schritt in pfad)
            {
                // Das Anzeigeelement erwartet die Angabe des Zentrum anstelle der rechten unteren Ecke
                var startFürAnzeige = Position.Erzeugen( absoluterUrsprung.HorizontalePosition + schritt.Breite / 2, absoluterUrsprung.VertikalePosition + schritt.Höhe / 2 );

                // Linie erstellen
                var linie = new FlächeOderLinieViewModel( Fläche.Erzeugen( startFürAnzeige, schritt ) );

                // Positionieren
                linie.SpielfeldAusdehnungFestlegen( m_absoluteBreite, m_absoluteHoehe );

                // Und anzeigen
                liste.Add( LinieView.Erzeugen( linie ) );

                // Bezugspunkt verschieben
                absoluterUrsprung = Position.Erzeugen( absoluterUrsprung.HorizontalePosition + schritt.Breite, absoluterUrsprung.VertikalePosition + schritt.Höhe );
            }
        }

        /// <summary>
        /// Wird aufgerufen, um alle Kollisionen zu ermitteln.
        /// </summary>
        /// <param name="liste">Alle Elemente auf dem Spielfeld.</param>
        /// <returns>Alle Elemente, die mit diesem Bewegungspfad kollidieren.</returns>
        private List<Fläche> ErmitteleKollisionen( IList<UIElement> liste )
        {
            // Alle Linien entfernen
            for (int i = liste.Count; i-- > 0; )
                if (liste[i] is LinieView)
                    liste.RemoveAt( i );

            // Alle Hindernisse ermitteln
            var hindernisse = liste.OfType<FrameworkElement>().Select( view => view.DataContext ).OfType<FlächeOderLinieViewModel>().ToList();

            // Alle Kollisionen ermitteln
            var kollisionen = m_bewegung.KollisionenErmitteln( hindernisse.Select( fläche => fläche.Fläche ) ).ToList();
            var getroffeneFlächen = new HashSet<Fläche>( kollisionen );

            // Markierung ändern
            hindernisse.ForEach( hindernis => hindernis.Getroffen = getroffeneFlächen.Contains( hindernis.Fläche ) );

            // Melden
            return kollisionen;
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

            // Alles aktualisieren
            InTextdarstellungUmrechnen();

            // Zusätzliche Änderungen melden
            PropertyChanged.EigenschaftVerändert( this, "Links" );
            PropertyChanged.EigenschaftVerändert( this, "Oben" );
            PropertyChanged.EigenschaftVerändert( this, "Breite" );
            PropertyChanged.EigenschaftVerändert( this, "Hoehe" );
            PropertyChanged.EigenschaftVerändert( this, "BreiteMitBewegung" );
            PropertyChanged.EigenschaftVerändert( this, "HoeheMitBewegung" );
            PropertyChanged.EigenschaftVerändert( this, "XVerschiebung" );
            PropertyChanged.EigenschaftVerändert( this, "YVerschiebung" );
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich irgend etwas verändert hat.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Wird ignoriert.</param>
        private void EinstellungenWurdenVerändert( object sender, PropertyChangedEventArgs e )
        {
            // Get gar nicht
            var breite = m_absoluteBreite;
            if (breite <= 0)
                return;
            var höhe = m_absoluteHoehe;
            if (höhe <= 0)
                return;

            // Kleinste horizontale Position
            decimal links;
            if (!decimal.TryParse( m_links, NumberStyles.Any, CultureInfo.InvariantCulture, out links ))
                return;
            if (links < 0)
                return;

            // Kleinste vertikale Position
            decimal oben;
            if (!decimal.TryParse( m_oben, NumberStyles.Any, CultureInfo.InvariantCulture, out oben ))
                return;
            if (oben < 0)
                return;

            // Horizontale Verschiebung
            decimal xVerschiebung;
            if (!decimal.TryParse( m_xVerschiebung, NumberStyles.Any, CultureInfo.InvariantCulture, out xVerschiebung ))
                return;
            if (xVerschiebung < 0)
                return;

            // Vertikale Verschiebung
            decimal yVerschiebung;
            if (!decimal.TryParse( m_yVerschiebung, NumberStyles.Any, CultureInfo.InvariantCulture, out yVerschiebung ))
                return;
            if (yVerschiebung < 0)
                return;

            // Umrechnen
            xVerschiebung /= m_absoluteBreite;
            yVerschiebung /= m_absoluteHoehe;

            // Rückrechnen
            var neuePosition =
                Position.SicherErzeugen
                (
                   (GenaueZahl) ZahlRunden( links / m_absoluteBreite + (decimal) (m_bewegung.Fläche.Ausdehnung.Breite / 2) ),
                   (GenaueZahl) ZahlRunden( 1m - oben / m_absoluteHoehe - yVerschiebung - (decimal) (m_bewegung.Fläche.Ausdehnung.Höhe / 2) )
                );

            // Es hat sich gar nichts verändert
            if (m_bewegung.Fläche.Position.HorizontalePosition == neuePosition.HorizontalePosition)
                if (m_bewegung.Fläche.Position.VertikalePosition == neuePosition.VertikalePosition)
                    if (m_bewegung.HorizontaleVerschiebung == (GenaueZahl) xVerschiebung)
                        if (m_bewegung.VertikaleVerschiebung == (GenaueZahl) yVerschiebung)
                            return;

            // Neue Bewegung anlegen
            m_bewegung = ElementInBewegung.Erzeugen( Fläche.Erzeugen( neuePosition, m_bewegung.Fläche.Ausdehnung ), (GenaueZahl) xVerschiebung, (GenaueZahl) yVerschiebung );

            // Melden
            PropertyChanged.EigenschaftVerändert( this, "Links" );
            PropertyChanged.EigenschaftVerändert( this, "Oben" );
            PropertyChanged.EigenschaftVerändert( this, "Breite" );
            PropertyChanged.EigenschaftVerändert( this, "Hoehe" );
            PropertyChanged.EigenschaftVerändert( this, "XVerschiebung" );
            PropertyChanged.EigenschaftVerändert( this, "YVerschiebung" );
            PropertyChanged.EigenschaftVerändert( this, "BreiteMitBewegung" );
            PropertyChanged.EigenschaftVerändert( this, "HoeheMitBewegung" );

            // Texte neu berechnen
            InTextdarstellungUmrechnen();
        }

        /// <summary>
        /// Rechnet die relativen Dimensionen in die aktuellen Dimensionen um.
        /// </summary>
        private void InTextdarstellungUmrechnen()
        {
            // Darstellung in Textform berechnen
            m_xVerschiebung = XVerschiebung.ToString( CultureInfo.InvariantCulture );
            m_yVerschiebung = YVerschiebung.ToString( CultureInfo.InvariantCulture );
            m_links = Links.ToString( CultureInfo.InvariantCulture );
            m_oben = Oben.ToString( CultureInfo.InvariantCulture );

            // Melden
            PropertyChanged.EigenschaftVerändert( this, "LinksAlsText" );
            PropertyChanged.EigenschaftVerändert( this, "ObenAlsText" );
            PropertyChanged.EigenschaftVerändert( this, "XVerschiebungAlsText" );
            PropertyChanged.EigenschaftVerändert( this, "YVerschiebungAlsText" );
        }

        /// <summary>
        /// Meldet die minimale horizontale Position.
        /// </summary>
        public decimal Links { get { return ZahlRunden( (decimal) (m_bewegung.Fläche.Position.HorizontalePosition - m_bewegung.Fläche.Ausdehnung.Breite / 2) * m_absoluteBreite ); } }

        /// <summary>
        /// Meldet die minimale vertikale Position.
        /// </summary>
        public decimal Oben { get { return ZahlRunden( (decimal) (GenaueZahl.Eins - (m_bewegung.Fläche.Position.VertikalePosition + m_bewegung.VertikaleVerschiebung + m_bewegung.Fläche.Ausdehnung.Höhe / 2)) * m_absoluteHoehe ); } }

        /// <summary>
        /// Meldet die gesamte Breite.
        /// </summary>
        public decimal BreiteMitBewegung { get { return ZahlRunden( (decimal) (m_bewegung.Fläche.Ausdehnung.Breite + m_bewegung.HorizontaleVerschiebung) * m_absoluteBreite ); } }

        /// <summary>
        /// Meldet die gesamte Höhe.
        /// </summary>
        public decimal HoeheMitBewegung { get { return ZahlRunden( (decimal) (m_bewegung.Fläche.Ausdehnung.Höhe + m_bewegung.VertikaleVerschiebung) * m_absoluteHoehe ); } }

        /// <summary>
        /// Meldet die Breite.
        /// </summary>
        public decimal Breite { get { return ZahlRunden( (decimal) m_bewegung.Fläche.Ausdehnung.Breite * m_absoluteBreite ); } }

        /// <summary>
        /// Meldet die Höhe.
        /// </summary>
        public decimal Hoehe { get { return ZahlRunden( (decimal) m_bewegung.Fläche.Ausdehnung.Höhe * m_absoluteHoehe ); } }

        /// <summary>
        /// Meldet die horizonale Bewegung.
        /// </summary>
        public decimal XVerschiebung { get { return ZahlRunden( (decimal) m_bewegung.HorizontaleVerschiebung * m_absoluteBreite ); } }

        /// <summary>
        /// Meldet die vertikale Bewegung.
        /// </summary>
        public decimal YVerschiebung { get { return ZahlRunden( (decimal) m_bewegung.VertikaleVerschiebung * m_absoluteHoehe ); } }

        /// <summary>
        /// Rundet eine Zahl auf eine vernünftige Genauigkeit.
        /// </summary>
        /// <param name="zahl">Irgendeine Zahl.</param>
        /// <returns>Eine Zahl mit der geeigneten Genauigkeit.</returns>
        private static decimal ZahlRunden( decimal zahl )
        {
            // Maximale Genauigkeit festlegen
            return Math.Round( zahl, 4 );
        }

        /// <summary>
        /// Die minimale horizonale Position als Zeichenkette.
        /// </summary>
        private string m_links;

        /// <summary>
        /// Meldet die minimale horizontale Position.
        /// </summary>
        public string LinksAlsText { get { return m_links; } set { this.EigenschaftVerändern( PropertyChanged, "LinksAlsText", ref m_links, value ); } }

        /// <summary>
        /// Die minimale vertikale Position als Zeichenkette.
        /// </summary>
        private string m_oben;

        /// <summary>
        /// Meldet die minimale vertikale Position.
        /// </summary>
        public string ObenAlsText { get { return m_oben; } set { this.EigenschaftVerändern( PropertyChanged, "ObenAlsText", ref m_oben, value ); } }

        /// <summary>
        /// Die horizontale Verschiebung als Text.
        /// </summary>
        private string m_xVerschiebung;

        /// <summary>
        /// Meldet die horizontale Verschiebung.
        /// </summary>
        public string XVerschiebungAlsText { get { return m_xVerschiebung; } set { this.EigenschaftVerändern( PropertyChanged, "XVerschiebungAlsText", ref m_xVerschiebung, value ); } }

        /// <summary>
        /// Die vertikale Verschiebung als Text.
        /// </summary>
        private string m_yVerschiebung;

        /// <summary>
        /// Meldet die vertikale Verschiebung.
        /// </summary>
        public string YVerschiebungAlsText { get { return m_yVerschiebung; } set { this.EigenschaftVerändern( PropertyChanged, "YVerschiebungAlsText", ref m_yVerschiebung, value ); } }

        /// <summary>
        /// Wird ausgelöst, wenn sich eine Eigenschaft verändert hat.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
