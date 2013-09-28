using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;


namespace JMS.JnRV2.Ablage.V1
{
    /// <summary>
    /// Die Konfiguration eines Spielfelds.
    /// </summary>
    [ContentProperty( "Elemente" )]
    public class Spielfeld
    {
        /// <summary>
        /// Der eindeutige Name des Spielfelds zur ablage der Ergebnisse.
        /// </summary>
        public string Kennung { get; set; }

        /// <summary>
        /// Der Name des Spielfelds.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Das Hintergrundbild.
        /// </summary>
        public string Bild { get; set; }

        /// <summary>
        /// Das Überladungsbild für den Fall, dass das Spiel verloren wurde.
        /// </summary>
        public string BildVerloren { get; set; }

        /// <summary>
        /// Das Überladungsbild für den Fall, dass das Spiel gewonnen wurde.
        /// </summary>
        public string BildGewonnen { get; set; }

        /// <summary>
        /// Die anfängliche Position der Spielfigur.
        /// </summary>
        public Point InitialePosition { get; set; }

        /// <summary>
        /// Die anfängliche Lebensenergie.
        /// </summary>
        public int InitialeLebensenergie { get; set; }

        /// <summary>
        /// Die maximale Sprungstärke für dieses Spiel.
        /// </summary>
        public int MaximaleSprungStaerke { get; set; }

        /// <summary>
        /// Die maximale Geschwindigkeit für dieses Spiel.
        /// </summary>
        public int MaximaleGeschwindigkeit { get; set; }

        /// <summary>
        /// Alle Elemente auf diesem Spielfeld.
        /// </summary>
        public List<Element> Elemente { get; private set; }

        /// <summary>
        /// Erstellt ein neues Spielfeld.
        /// </summary>
        public Spielfeld()
        {
            // Initialisierung abschliessen
            Elemente = new List<Element>();
            InitialeLebensenergie = 10000;
        }
    }
}
