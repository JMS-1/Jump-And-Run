using System;
using System.Windows;
using JMS.JnRV2.Ablauf;


namespace JMS.JnRV2.Anzeige.Verbinder
{
    /// <summary>
    /// Über diese Schnittstelle werden die Verbinder erzeugt.
    /// </summary>
    /// <typeparam name="TArtDesSpielElementes">Die Art des zugehörigen Spielelementes.</typeparam>
    internal class SpielerErzeuger<TArtDesSpielElementes> : IVerbinderErzeuger
    {
        /// <summary>
        /// Die initiale Lebensenergie der Spielfigur.
        /// </summary>
        private readonly int m_lebensenergie;

        /// <summary>
        /// Die maximale Anzahl aufeinanderfolgender Sprünge.
        /// </summary>
        private readonly int m_maximaleSprünge;

        /// <summary>
        /// Die maximale Geschwindigkeit der Spielfigur.
        /// </summary>
        private readonly int m_maximaleGeschwindigkeit;

        /// <summary>
        /// Die maximale Sprungstärke der Spielfigur.
        /// </summary>
        private readonly decimal m_maximaleSprungStärke;

        /// <summary>
        /// Erstellt einen neuen Verbinder.
        /// </summary>
        /// <param name="figur">Die Konfiguration der Spielfigur.</param>
        /// <param name="spielfeld">Das Spielfeld, auf dem die Spielfigur lebt.</param>
        public SpielerErzeuger( Ablage.Spielfigur figur, Ablage.Spielfeld spielfeld )
        {
            // Merken
            m_maximaleGeschwindigkeit = Math.Min( figur.MaximaleGeschwindigkeit, spielfeld.MaximaleGeschwindigkeit );
            m_maximaleSprungStärke = Math.Min( figur.SprungStaerke, spielfeld.MaximaleSprungStaerke );
            m_lebensenergie = spielfeld.InitialeLebensenergie;
            m_maximaleSprünge = figur.SpruengeNacheinander;
        }

        /// <summary>
        /// Erzeugt eine neue Verbindung.
        /// </summary>
        /// <param name="position">Die Position des Elementes in der Simulation.</param>
        /// <param name="ausdehnung">Die Ausdehung des Elementes in der Simulation.</param>
        /// <param name="breite">Die gesamte Breite der Simulation.</param>
        /// <param name="höhe">Die gesamte Höhe der Simulation.</param>
        /// <param name="umrechner">Methode zum Ermitteln der absoluten Koordinaten.</param>
        /// <param name="element">Das Element, auf das sich diese Verbindung bezieht.</param>
        /// <returns>Einen neuen Verbinder.</returns>
        public GrundElement Erzeuge( Position position, Ausdehnung ausdehnung, decimal breite, decimal höhe, Func<Bereich, Rect> umrechner, PraesentationsModelle.Element element )
        {
            // Wandeln
            var figurElement = (PraesentationsModelle.FigurElement<TArtDesSpielElementes>) element;
            var spiel = figurElement.Spiel;
            var steuerung = (PraesentationsModelle.Steuerung) spiel.Steuerung;

            // Anlegen
            var spieler = new Spieler( position, ausdehnung );

            // Grundkonfiguration setzen
            spieler.SprungStärke = (GenaueZahl) (Ablage.Skalierungswerte.EinfacheSprungstärke / Ablage.Skalierungswerte.SprungDauer * m_maximaleSprungStärke * (decimal) figurElement.Hoehe / höhe);
            spieler.EinfacheGeschwindigkeit = (GenaueZahl) (Ablage.Skalierungswerte.EinfacheHorizontaleGeschwindigkeit * (decimal) figurElement.Breite / breite);
            spieler.SprungDauer = TimeSpan.FromSeconds( (double) Ablage.Skalierungswerte.SprungDauer );
            spieler.MaximalerGeschwindigkeitsFaktor = m_maximaleGeschwindigkeit;
            spieler.MaximalerSprung = m_maximaleSprünge;

            // Basisdaten vorbereiten
            spieler.LebenskraftÄndern( m_lebensenergie );

            // Bewegungen überwachen
            spieler.ElementHatSichBewegt += s =>
                {
                    // Absolute Position berechnen
                    var bereichAbsolut = umrechner( spieler.Bereich );

                    // Übertragen
                    element.PositionVerändern( bereichAbsolut.Left, bereichAbsolut.Top );
                };

            // Zustand überwachen
            spieler.ZustandVerändert += s =>
                {
                    // Weiter geben
                    var bewegung = spieler.Bewegung;
                    if (bewegung == ElementBewegung.Ruht)
                        figurElement.SetzeBewegungsAnzeige( ZustandDerFigur.Ruhend );
                    else if ((bewegung & ElementBewegung.FälltNachUnten) == ElementBewegung.FälltNachUnten)
                        figurElement.SetzeBewegungsAnzeige( ZustandDerFigur.InDerLuft );
                    else if ((bewegung & ElementBewegung.SpringtNachOben) == ElementBewegung.SpringtNachOben)
                        figurElement.SetzeBewegungsAnzeige( ZustandDerFigur.InDerLuft );
                    else if ((bewegung & ElementBewegung.LäuftNachLinks) == ElementBewegung.LäuftNachLinks)
                        figurElement.SetzeBewegungsAnzeige( ZustandDerFigur.NachLinks );
                    else if ((bewegung & ElementBewegung.LäuftNachRechts) == ElementBewegung.LäuftNachRechts)
                        figurElement.SetzeBewegungsAnzeige( ZustandDerFigur.NachRechts );
                    else
                        figurElement.SetzeBewegungsAnzeige( ZustandDerFigur.Ruhend );
                };

            // Punkte überwachen
            spieler.PunkteVerändert += s => steuerung.PunktestandAktualisieren();

            // Lebensenergie überwachen
            spieler.LebenskraftVerändert += s => steuerung.LebensengergieAktualisieren();

            // Melden
            return spieler;
        }
    }
}
