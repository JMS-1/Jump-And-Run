using System;
using System.ComponentModel;
using System.Windows.Media;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Beschreibt die Position der Spielfigur auf dem Spielfeld.
    /// </summary>
    /// <typeparam name="TArtDesSpielElementes">Die Art des zugehörigen Elementes der Simulation.</typeparam>
    internal class FigurElement<TArtDesSpielElementes> : BildElementBasis<Figur, TArtDesSpielElementes>
    {
        /// <summary>
        /// Ein mit dem Element verbundenes Bild.
        /// </summary>
        protected override ImageSource AktuellesBild { get { return Quelle.Bild; } }

        /// <summary>
        /// Erstellt eine neue Präsentation für ein Element.
        /// </summary>
        /// <param name="horizontalePosition">Die initiale horizontale Position des Elementes.</param>
        /// <param name="vertikalePosition">Die initiale vertikale Position des Elementes.</param>
        /// <param name="figur">Die zugehörige Visualisierung der Spielfigur.</param>
        /// <param name="initialisierung">Eine Methode zur abschliessenden Initialisierung sobald die Simulation bereit ist.</param>
        public FigurElement( double horizontalePosition, double vertikalePosition, Figur figur, Func<TArtDesSpielElementes> initialisierung )
            : base( null, true, horizontalePosition, vertikalePosition, figur, 0, initialisierung )
        {
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich die Spielzeit verändert hat.
        /// </summary>
        /// <param name="interessenten">Alle, die an Veränderungen interessiert sind.</param>
        protected override void SpielzeitWurdeVerändert( PropertyChangedEventHandler interessenten )
        {
            // Durchreichen
            Quelle.SpielZeit = GespielteZeit;
        }

        /// <summary>
        /// Wird aufgerufen, sobald dieses Element zum ersten mal sichtbar wird.
        /// </summary>
        protected override void ErstmaligVerfügbar()
        {
            // Neu positionieren
            PositionVerändern( EchteHorizontalePosition - Breite / 2, EchteVertikalePosition - Hoehe / 2 );
        }

        /// <summary>
        /// Legt die Art der Bewegung fest.
        /// </summary>
        /// <param name="artDerBewegung">Die neue Art der Bewegung.</param>
        public void SetzeBewegungsAnzeige( ZustandDerFigur artDerBewegung )
        {
            // Durchreichen
            Quelle.AktuellerZustand = artDerBewegung;
        }
    }
}
