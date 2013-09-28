using System;
using System.Collections.Generic;
using System.Linq;


namespace JMS.JnRV2.Ablauf.Kollisionen
{
    /// <summary>
    /// Hilfsklasse zur Durchführung der Kollisionsanalyse.
    /// </summary>
    internal class KollisionsAnalyse : IKollisionsAnalyse
    {
        /// <summary>
        /// Die Liste aller Hindernisse.
        /// </summary>
        private readonly List<Hindernis> m_hindernisse;

        /// <summary>
        /// Die gesamte Bewegung.
        /// </summary>
        public Ausdehnung VolleStrecke { get; private set; }

        /// <summary>
        /// Der Rest der Strecke, der noch zu durchwandern ist.
        /// </summary>
        public Ausdehnung PlanungsBereich { get; private set; }

        /// <summary>
        /// Beschreibt die geplante Bewegung.
        /// </summary>
        public ElementInBewegung Bewegung { get; private set; }

        /// <summary>
        /// Erstellt eine neue Analyseeinheit.
        /// </summary>
        /// <param name="element">Die Definition der gewünschten Bewegung.</param>
        /// <param name="hindernisse">Alle Hindernisse auf dem Spielfeld, die mit dem Element tatächlich kollidieren.</param>
        internal KollisionsAnalyse( ElementInBewegung element, IEnumerable<Fläche> hindernisse )
        {
            // Merken
            Bewegung = element;

            // Über alles
            VolleStrecke = Ausdehnung.Erzeugen( element.HorizontaleVerschiebung.Abs(), element.VertikaleVerschiebung.Abs() );

            // Liste aller Hindernisse erzeugen
            m_hindernisse = hindernisse.Select( hindernis => new Hindernis( this, hindernis ) ).ToList();
        }

        /// <summary>
        /// Ermittelt den Bewegungspfad.
        /// </summary>
        /// <param name="kollisionsMeldung">Wird ausgelöst, wenn eine Kollision stattfindet und kann
        /// den gesamten Ablauf beendet,</param>
        /// <returns>Die Liste der einzelnen Bewegungsschritte.</returns>
        public IEnumerable<Ausdehnung> PfadErmitteln( Func<Fläche, bool> kollisionsMeldung = null )
        {
            // Das geht gar nicht
            if (m_hindernisse.Any( hindernis => hindernis.Element.Bereich.ÜberschneidetSichMit( Bewegung.Fläche.Bereich ) ))
                yield break;

            // Was noch übrig ist
            var restBreite = VolleStrecke.Breite;
            var restHöhe = VolleStrecke.Höhe;

            // So lange es geht
            for (PlanungsBereich = VolleStrecke; ; )
            {
                // Suchen wir einmal nach dem ersten Hindernis auf dem Weg
                var erstesHindernis = ErstesHindernisFinden();
                if (erstesHindernis == null)
                {
                    // Freie Fahrt für freie Bürger :-)
                    yield return PlanungsBereich;
                    yield break;
                }

                // Ersten Schritt melden
                var ausdehnung = erstesHindernis.SchrittweiteBisZurKollision.Value;
                if (ausdehnung.Breite != GenaueZahl.Null)
                    yield return ausdehnung;
                else if (ausdehnung.Höhe != GenaueZahl.Null)
                    yield return ausdehnung;

                // Element melden und auf Wunsch die weitere Analyse beenden
                if (kollisionsMeldung != null)
                {
                    // Alle Kollisionen am gleichen Punkt
                    var ähnlicheKollisionen =
                        m_hindernisse
                            .Where( hindernis =>
                                {
                                    // Kollision bestimmen
                                    var schritt = hindernis.SchrittweiteBisZurKollision;
                                    if (!schritt.HasValue)
                                        return false;

                                    // Horizontal
                                    if (schritt.Value.Breite != ausdehnung.Breite)
                                        return false;

                                    // Vertikal
                                    return (schritt.Value.Höhe == ausdehnung.Höhe);

                                } )
                            .Select( hindernis => hindernis.Element )
                            .ToArray();

                    // Alle prüfen und gegebenenfalls aufhören
                    if (ähnlicheKollisionen.Count( kollisionsMeldung ) != ähnlicheKollisionen.Length)
                        yield break;
                }

                // Wird sind am Anschlag
                if (ausdehnung.Breite > PlanungsBereich.Breite)
                    yield break;
                if (ausdehnung.Höhe > PlanungsBereich.Höhe)
                    yield break;

                // Art der Kollision betrachten
                if (erstesHindernis.KollisionGegenDieSeite)
                {
                    // Schauen wir mal, ob wir uns noch etwas frei bewegen können
                    var vertikalesEndeDerBewegung = GenaueZahlHelfer.Min( ErstesVertikalesHindernisFinden( erstesHindernis ), restHöhe );
                    if (vertikalesEndeDerBewegung > ausdehnung.Höhe)
                        yield return Ausdehnung.Erzeugen( GenaueZahl.Null, vertikalesEndeDerBewegung - ausdehnung.Höhe );

                    // Bewegungsraum anpassen
                    ausdehnung = Ausdehnung.Erzeugen( ausdehnung.Breite, vertikalesEndeDerBewegung );
                }
                else
                {
                    // Schauen wir mal, ob wir uns noch etwas frei bewegen können
                    var horizontalesEndeDerBewegung = GenaueZahlHelfer.Min( ErstesHorizontalesHindernisFinden( erstesHindernis ), restBreite );
                    if (horizontalesEndeDerBewegung > ausdehnung.Breite)
                        yield return Ausdehnung.Erzeugen( horizontalesEndeDerBewegung - ausdehnung.Breite, GenaueZahl.Null );

                    // Bewegungsraum anpassen
                    ausdehnung = Ausdehnung.Erzeugen( horizontalesEndeDerBewegung, ausdehnung.Höhe );
                }

                // Ups, wir bewegen uns gar nich
                if (ausdehnung.Breite == GenaueZahl.Null)
                    if (ausdehnung.Höhe == GenaueZahl.Null)
                        yield break;

                // Neue Breite berechnen               
                restBreite = restBreite - ausdehnung.Breite;
                restHöhe = restHöhe - ausdehnung.Höhe;

                // Alles abgearbeitet
                if (restBreite < GenaueZahl.Null)
                    yield break;
                if (restHöhe < GenaueZahl.Null)
                    yield break;
                if (restBreite == GenaueZahl.Null)
                    if (restHöhe == GenaueZahl.Null)
                        yield break;

                // Alle Elemente anpassen
                m_hindernisse.ForEach( hindernis => hindernis.BereichGemäßSchrittweiteVerschieben( ausdehnung ) );

                // Art der Bewegung
                if ((VolleStrecke.Breite == GenaueZahl.Null) || (restBreite == GenaueZahl.Null))
                    PlanungsBereich = Ausdehnung.Erzeugen( GenaueZahl.Null, restHöhe );
                else if ((VolleStrecke.Höhe == GenaueZahl.Null) || (restHöhe == GenaueZahl.Null))
                    PlanungsBereich = Ausdehnung.Erzeugen( restBreite, GenaueZahl.Null );
                else
                {
                    // Aktuelles Verhältnis berechenen
                    var aktuellesSeitenverhältnis = restBreite / restHöhe;
                    var erlaubtesSeitenverhältnis = VolleStrecke.Breite / VolleStrecke.Höhe;

                    // Je nach Situation verfahren
                    if (aktuellesSeitenverhältnis < erlaubtesSeitenverhältnis)
                        PlanungsBereich = Ausdehnung.Erzeugen( restBreite, restBreite / erlaubtesSeitenverhältnis );
                    else
                        PlanungsBereich = Ausdehnung.Erzeugen( restHöhe * erlaubtesSeitenverhältnis, restHöhe );
                }
            }
        }

        /// <summary>
        /// Nachdem ein Hindernis an der Unterseite getroffen wurde kann die Bewegung horizontal
        /// maximal bis zum Ende des Hindernisses fortgesetzt werden.
        /// </summary>
        /// <param name="kollision">Dieses Hindernis haben wir von unten getroffen.</param>
        /// <returns>Die horizontale Bewegung, die wir noch ausführen können.</returns>
        private GenaueZahl ErstesHorizontalesHindernisFinden( Hindernis kollision )
        {
            // Die horizontale Achse am Auftreffpunkt
            var schritt = kollision.SchrittweiteBisZurKollision.Value;
            var achse = Bereich.Erzeugen( schritt.Breite, schritt.Höhe, VolleStrecke.Breite, GenaueZahl.Null );

            // Mit Ausnahme des bereits bekannten Hindernisses alle Hindernisse, die diese Achse schneiden
            var hindernisseAufDerAchse =
                m_hindernisse
                    .Where( hindernis => !ReferenceEquals( hindernis, kollision ) )
                    .Where( hindernis => hindernis.Bereich.ÜberschneidetSichMit( achse ) );

            // Geplant ist es, an dem Hindernis vorbei zu ziehen
            var horizontalesEndeDerBewegung = kollision.Bereich.GrößteHorizontalePosition;

            // Wenn nichts im Weg steht machen wir das so
            var erstesHindernisAufDerAchse = BestesHindernisFinden( hindernisseAufDerAchse, hindernis => hindernis.Bereich.KleinsteHorizontalePosition );
            if (erstesHindernisAufDerAchse == null)
                return horizontalesEndeDerBewegung;

            // Liegt das Hindernis näher als das geplante Ende der Bewegung, so müssen wir früher aufhören
            var horizontalesEnde = erstesHindernisAufDerAchse.Bereich.KleinsteHorizontalePosition;
            if (horizontalesEnde < horizontalesEndeDerBewegung)
                return horizontalesEnde;

            // Wir können den vollen Weg wie geplant nutzen
            return horizontalesEndeDerBewegung;
        }

        /// <summary>
        /// Ermittelt, wie weit wir uns vertikal entlang einer Seite des getroffenen Hindernisses 
        /// bewegen können.
        /// </summary>
        /// <param name="kollision">Das aktuelle Hindernis, das wir an der Seite getroffen haben.</param>
        /// <returns>Der vertikale Abstand, den wir uns noch bewegen dürfen.</returns>
        private GenaueZahl ErstesVertikalesHindernisFinden( Hindernis kollision )
        {
            // Die vertikale Achse am Ende der aktuellen Bewegung
            var schritt = kollision.SchrittweiteBisZurKollision.Value;
            var achse = Bereich.Erzeugen( schritt.Breite, schritt.Höhe, GenaueZahl.Null, VolleStrecke.Höhe );

            // Mit Ausnahme des bereits bekannten Hindernisses alle Hindernisse, die diese Achse schneiden
            var hindernisseAufDerAchse =
                m_hindernisse
                    .Where( hindernis => !ReferenceEquals( hindernis, kollision ) )
                    .Where( hindernis => hindernis.Bereich.ÜberschneidetSichMit( achse ) );

            // Wir wollen uns maximal bis zum Ende des Hindernisses bewegen
            var vertialesEndeDesHindernisses = kollision.Bereich.GrößteVertikalePosition;

            // Wenn nichts im Weg steht, verwenden wir diesen Abstand
            var erstesHindernisAufDerAchse = BestesHindernisFinden( hindernisseAufDerAchse, hindernis => hindernis.Bereich.KleinsteVertikalePosition );
            if (erstesHindernisAufDerAchse == null)
                return vertialesEndeDesHindernisses;

            // Wenn etwas im Weg steht und wir eher darauf treffen, so wird der freie Weg entsprechend reduziert
            var vertikalesEnde = erstesHindernisAufDerAchse.Bereich.KleinsteVertikalePosition;
            if (vertikalesEnde < vertialesEndeDesHindernisses)
                return vertikalesEnde;

            // Ansonsten bleibt alles beim Alten
            return vertialesEndeDesHindernisses;
        }

        /// <summary>
        /// Ermittelt das erste Hindernis zwischen Start und Endpunkt der Bewegung.
        /// </summary>
        /// <returns>Das erste Hindernis auf dem Weg.</returns>
        private Hindernis ErstesHindernisFinden()
        {
            // Schauen wir einmal, ob es Kollisionen gibt
            var alleKollisionen = m_hindernisse.Where( hindernis => hindernis.AufKollisionPrüfen() ).ToArray();
            if (alleKollisionen.Length < 1)
                return null;

            // Die erste horizontale Position ermitteln, an der eine Kollision stattfindet - genau genommen handelt es sich um einen Kandidaten
            var ersteHorizontaleKollision = BestesHindernisFinden( alleKollisionen, hindernis => hindernis.SchrittweiteBisZurKollision.Value.Breite ).SchrittweiteBisZurKollision.Value.Breite;

            // Und jetzt von allen mit gleicher horizontaler Koordinate den ersten vertikalen Treffer
            return BestesHindernisFinden( alleKollisionen.Where( hindernis => hindernis.SchrittweiteBisZurKollision.Value.Breite == ersteHorizontaleKollision ), hindernis => hindernis.SchrittweiteBisZurKollision.Value.Höhe );
        }

        /// <summary>
        /// Ermittelt das am besten passende Hindernis aus einer Liste.
        /// </summary>
        /// <param name="hindernisse">Eine Liste von Hindernissen.</param>
        /// <param name="eigenschaft">Liest eine Eigenschaft aus..</param>
        /// <returns>Das am besten passende Hindernis</returns>
        private static Hindernis BestesHindernisFinden( IEnumerable<Hindernis> hindernisse, Func<Hindernis, GenaueZahl> eigenschaft )
        {
            // Noch nichts gefunden
            GenaueZahl bestWert = GenaueZahl.Null;
            Hindernis bestes = null;

            // Alle absuchen
            foreach (var hindernis in hindernisse)
            {
                // Wert auslesen
                var wert = eigenschaft( hindernis );

                // Können wir nicht brauchen
                if (bestes != null)
                    if (wert >= bestWert)
                        continue;

                // Übernehmen
                bestes = hindernis;
                bestWert = wert;
            }

            // Melden
            return bestes;
        }
    }
}
