using System;
using System.Collections.Generic;
using System.Linq;


namespace JMS.JnRV2.Ablauf.Kollisionen
{
    /// <summary>
    /// Beschreibt ein Element, dass sich in Bewegung befindet.
    /// </summary>
    public class ElementInBewegung
    {
        /// <summary>
        /// Die ursprüngliche Position des Elementes.
        /// </summary>
        public Fläche Fläche { get; private set; }

        /// <summary>
        /// Der Umfang der horizontalen Bewegung.
        /// </summary>
        public GenaueZahl HorizontaleVerschiebung { get; private set; }

        /// <summary>
        /// Gesetzt, wenn die Bewegung nicht nach Rechts erfolgt.
        /// </summary>
        public bool VonLinksNachRechts { get { return HorizontaleVerschiebung >= GenaueZahl.Null; } }

        /// <summary>
        /// Der Umfang der vertikalen Bewegung.
        /// </summary>
        public GenaueZahl VertikaleVerschiebung { get; private set; }

        /// <summary>
        /// Gesetzt, wenn die Bewegung nicht nach Unten erfolgt.
        /// </summary>
        public bool VonUntenNachOben { get { return VertikaleVerschiebung >= GenaueZahl.Null; } }

        /// <summary>
        /// Erzeugt eine neue Beschreibung.
        /// </summary>
        /// <param name="fläche">Die tatsächliche Position des Elementes.</param>
        /// <param name="horizontaleVerschiebung">Die Verschiebung in horizontale Richtung.</param>
        /// <param name="vertikaleVerschiebung">Die Verschiebung in vertikale Richtung.</param>
        private ElementInBewegung( Fläche fläche, GenaueZahl horizontaleVerschiebung, GenaueZahl vertikaleVerschiebung )
        {
            // Alles merken
            HorizontaleVerschiebung = horizontaleVerschiebung;
            VertikaleVerschiebung = vertikaleVerschiebung;
            Fläche = fläche;

            // Breite und Höhe des Bewegungspfads ermitteln
            var breite = Fläche.Bereich.Breite + HorizontaleVerschiebung.Abs();
            var höhe = Fläche.Bereich.Höhe + VertikaleVerschiebung.Abs();

            // Ursprung bestimmen
            var links = Fläche.Bereich.KleinsteHorizontalePosition;
            var unten = Fläche.Bereich.KleinsteVertikalePosition;

            // Je nach Orientierung korrigieren
            if (HorizontaleVerschiebung < GenaueZahl.Null)
                links += HorizontaleVerschiebung;
            if (VertikaleVerschiebung < GenaueZahl.Null)
                unten += VertikaleVerschiebung;

            // Gesamtumfang ermitteln
            GesamterBereich = Bereich.Erzeugen( links, unten, breite, höhe );
        }

        /// <summary>
        /// Erzeugt eine neue Beschreibung.
        /// </summary>
        /// <param name="fläche">Die tatsächliche Position des Elementes.</param>
        /// <param name="horizontaleVerschiebung">Die Verschiebung in horizontale Richtung.</param>
        /// <param name="vertikaleVerschiebung">Die Verschiebung in vertikale Richtung.</param>
        /// <returns>Die gewünschte Beschreibung der Bewegung.</returns>
        public static ElementInBewegung Erzeugen( Fläche fläche, GenaueZahl horizontaleVerschiebung, GenaueZahl vertikaleVerschiebung )
        {
            // Forward
            return new ElementInBewegung( fläche, horizontaleVerschiebung, vertikaleVerschiebung );
        }

        /// <summary>
        /// Ermittelt alle Elemente, mit denen dieses Element während einer Bewegung kollidiert.
        /// </summary>
        /// <param name="elemente">Eine Liste von Elementen.</param>
        /// <returns>Die Elemente, mit denen wir kollidieren werden.</returns>
        public IEnumerable<Fläche> KollisionenErmitteln( IEnumerable<Fläche> elemente )
        {
            // Prüfen
            if (elemente == null)
                throw new ArgumentNullException( "elemente" );

            // Alle Elemente absuchen
            return elemente.Where( element => GesamterBereich.ÜberschneidetSichMit( element.Bereich ) );
        }

        /// <summary>
        /// Ermittelt den gesamten Bereich der Bewegung.
        /// </summary>
        public Bereich GesamterBereich { get; private set; }

        /// <summary>
        /// Erzeugt eine neue Kollisionsanalyse.
        /// </summary>
        /// <param name="elemente">Die Liste aller Elemente auf dem Spielfeld.</param>
        /// <returns>Die neue Analyseeinheit.</returns>
        public IKollisionsAnalyse BeginneAnalyse( IEnumerable<Fläche> elemente )
        {
            // Durchreichen
            return new KollisionsAnalyse( this, KollisionenErmitteln( elemente ) );
        }
    }
}
