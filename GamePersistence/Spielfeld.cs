using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;


namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Die Konfiguration eines Spielfelds.
    /// </summary>
    [ContentProperty( "Elemente" )]
    public class Spielfeld
    {
        /// <summary>
        /// Der eindeutige Name des Spielfelds zur Ablage der Ergebnisse.
        /// </summary>
        public string Kennung { get; set; }

        /// <summary>
        /// Das Hintergrundbild.
        /// </summary>
        public string Bild { get; set; }

        /// <summary>
        /// Alle Elemente auf diesem Spielfeld.
        /// </summary>
        public List<Element> Elemente { get; private set; }

        /// <summary>
        /// Die anfängliche Position der Spielfigur.
        /// </summary>
        public Point InitialePosition { get; set; }

        /// <summary>
        /// Die anfängliche Lebensenergie.
        /// </summary>
        public int InitialeLebensenergie { get; set; }

        /// <summary>
        /// Das Überladungsbild für den Fall, dass das Spiel verloren wurde.
        /// </summary>
        public string BildVerloren { get; set; }

        /// <summary>
        /// Das Überladungsbild für den Fall, dass das Spiel gewonnen wurde.
        /// </summary>
        public string BildGewonnen { get; set; }

        /// <summary>
        /// Die maximale Sprungstärke für dieses Spiel.
        /// </summary>
        public int MaximaleSprungStaerke { get; set; }

        /// <summary>
        /// Die maximale Geschwindigkeit für dieses Spiel.
        /// </summary>
        public int MaximaleGeschwindigkeit { get; set; }

        /// <summary>
        /// Der Name des Spielfelds.
        /// </summary>
        public string Beschreibung { get; set; }

        /// <summary>
        /// Alle Spielergebnisse.
        /// </summary>
        private Spielergebnisse m_ergebnisse;

        /// <summary>
        /// Alle Spielergebnisse.
        /// </summary>
        public Spielergebnisse Ergebnisse
        {
            get
            {
                // Einmalig erzeugen
                if (m_ergebnisse == null)
                    m_ergebnisse = Spielergebnisse.Laden( new Guid( Kennung ) );

                // Melden
                return m_ergebnisse;
            }
        }

        /// <summary>
        /// Erzeugt ein neues Spielfeld.
        /// </summary>
        public Spielfeld()
        {
            // Initialisierung beenden
            Kennung = Guid.NewGuid().ToString().ToLower();
            Elemente = new List<Element>();
            InitialeLebensenergie = 10000;
        }

        /// <summary>
        /// Erstellt ein neues Spielfeld aus einer alten Konfiguration.
        /// </summary>
        /// <param name="alteDarstellung">Die alte Konfiguration.</param>
        public Spielfeld( V1.Spielfeld alteDarstellung )
        {
            // Prüfen
            if (alteDarstellung == null)
                throw new ArgumentNullException( "alteDarstellung" );

            // Alles übernehmen
            MaximaleGeschwindigkeit = alteDarstellung.MaximaleGeschwindigkeit;
            MaximaleSprungStaerke = alteDarstellung.MaximaleSprungStaerke;
            InitialeLebensenergie = alteDarstellung.InitialeLebensenergie;
            InitialePosition = alteDarstellung.InitialePosition;
            BildVerloren = alteDarstellung.BildVerloren;
            BildGewonnen = alteDarstellung.BildGewonnen;
            Beschreibung = alteDarstellung.Name;
            Kennung = alteDarstellung.Kennung;
            Bild = alteDarstellung.Bild;

            // Elemente wandeln
            Elemente = alteDarstellung.Elemente.Select( Element.Erzeugen ).ToList();
        }
    }
}
