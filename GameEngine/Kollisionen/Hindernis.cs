

namespace JMS.JnRV2.Ablauf.Kollisionen
{
    /// <summary>
    /// Beschreibt ein einzelnes Hindernis.
    /// </summary>
    internal class Hindernis
    {
        /// <summary>
        /// Die zugehörige Analyseinstanz.
        /// </summary>
        private readonly KollisionsAnalyse m_analyse;

        /// <summary>
        /// Das originale Element.
        /// </summary>
        public Fläche Element { get; private set; }

        /// <summary>
        /// Der erweiterte Bereich, den dieses Element abdeckt.
        /// </summary>
        public Bereich Bereich { get; private set; }

        /// <summary>
        /// Erzeugt eine neue Beschreibung.
        /// </summary>
        /// <param name="analyse">Die zugehörige Analyseinstanz.</param>
        /// <param name="element">Das zugehörige Hindernis.</param>
        public Hindernis( KollisionsAnalyse analyse, Fläche element )
        {
            // Merken
            m_analyse = analyse;
            Element = element;

            // Die Ausdehung des sich bewegenden Elementes ermitteln
            var bewegung = analyse.Bewegung;
            var ausgangsBereich = bewegung.Fläche.Bereich;

            // Die Ausdehung des Hindernisses ermitteln
            var elementBereich = element.Bereich;

            // Element im Planungsbereich positionieren - bei Bedarf wird alles in den positiven Quadranten gespiegelt, wir nehmen den Abstand zum Ursprung
            var xRelativ = bewegung.VonLinksNachRechts ? (elementBereich.KleinsteHorizontalePosition - ausgangsBereich.GrößteHorizontalePosition) : (ausgangsBereich.KleinsteHorizontalePosition - elementBereich.GrößteHorizontalePosition);
            var yRelativ = bewegung.VonUntenNachOben ? (elementBereich.KleinsteVertikalePosition - ausgangsBereich.GrößteVertikalePosition) : (ausgangsBereich.KleinsteVertikalePosition - elementBereich.GrößteVertikalePosition);

            // Gesamte Ausdehnung ermitteln, so wie sie in der Planung benötigt wird - zusammengesetzt aus den Ausdehnungen beider beteiligter Elemente
            var breite = elementBereich.Breite + ausgangsBereich.Breite;
            var höhe = elementBereich.Höhe + ausgangsBereich.Höhe;

            // Anfänglichen Bereich berechnen - dieser wird mit jedem Schritt angepasst
            Bereich = Bereich.Erzeugen( xRelativ, yRelativ, breite, höhe );
        }

        /// <summary>
        /// Verschiebt dieses Element nach einem Analyseschritt.
        /// </summary>
        /// <param name="schritt">Die Schrittweite.</param>
        public void BereichGemäßSchrittweiteVerschieben( Ausdehnung schritt )
        {
            // Bereich neu berechnen - der Ursprung verschiebt sich nach rechts oben, i.e. unsere relativen Koordinaten nach links unten
            Bereich = 
                Bereich.Erzeugen
                    ( 
                        Bereich.KleinsteHorizontalePosition - schritt.Breite, 
                        Bereich.KleinsteVertikalePosition - schritt.Höhe, 
                        Bereich.Breite, 
                        Bereich.Höhe 
                    );
        }

        /// <summary>
        /// Meldet die Schrittweite bis zur Kollision der Elemente.
        /// </summary>
        public Ausdehnung? SchrittweiteBisZurKollision { get; private set; }

        /// <summary>
        /// Gesetzt, wenn die Bewegung an der Seite des Hindernisses kollidiert.
        /// </summary>
        public bool KollisionGegenDieSeite { get; private set; }

        /// <summary>
        /// Gesetzt, wenn die vertikale Achse im Inneren des Elementes liegt.
        /// </summary>
        public bool ÜberlagertDieVertikaleAchse { get { return (Bereich.KleinsteHorizontalePosition < GenaueZahl.Null) && (Bereich.GrößteHorizontalePosition > GenaueZahl.Null); } }

        /// <summary>
        /// Gesetzt, wenn die horizontale Achse im Inneren des Elementes liegt.
        /// </summary>
        public bool ÜberlagertDieHorizontaleAchse { get { return (Bereich.KleinsteVertikalePosition < GenaueZahl.Null) && (Bereich.GrößteVertikalePosition > GenaueZahl.Null); } }

        /// <summary>
        /// Prüft, ob die Bewegung seitlich auf das Hindernis trifft.
        /// </summary>
        /// <returns>Gesetzt, wenn ein seitliches Auftreffen erkannt wurde.</returns>
        private bool PrüfeAufSeitlichesAuftreffen()
        {
            // Wir befinden uns vollständig auf der falschen Seite
            if (Bereich.KleinsteHorizontalePosition <= GenaueZahl.Null)
                return false;

            // Wird liegen horizontal völlig ausserhalb der Bewegung
            if (Bereich.KleinsteHorizontalePosition >= m_analyse.PlanungsBereich.Breite)
                return false;

            // Schauen wir einmal, ob es sich um eine rein horizontale Bewegung handelt
            if (m_analyse.PlanungsBereich.Höhe == GenaueZahl.Null)
            {
                // Dieses Hindernis schneidet die horizontale Achse nicht
                if (!ÜberlagertDieHorizontaleAchse)
                    return false;

                // Horizontale Bewegung bis zur Kante des Hindernisses
                SchrittweiteBisZurKollision = Ausdehnung.Erzeugen( Bereich.KleinsteHorizontalePosition, GenaueZahl.Null );
                KollisionGegenDieSeite = true;

                // Fertig
                return true;
            }

            // Dreisatz zum Ermitteln der Höhe der Bewegungslinie
            var yReferenz = Bereich.KleinsteHorizontalePosition * m_analyse.VolleStrecke.Höhe / m_analyse.VolleStrecke.Breite;
            var yRelativ = yReferenz - Bereich.KleinsteVertikalePosition;

            // Wir sind zu hoch
            if (yRelativ < GenaueZahl.Null)
                return false;

            // Wir sind zu niedrig
            if (yRelativ >= Bereich.Höhe)
                return false;

            // Die Bewegung trifft uns an der Seite
            SchrittweiteBisZurKollision = Ausdehnung.Erzeugen( Bereich.KleinsteHorizontalePosition, yReferenz );
            KollisionGegenDieSeite = true;

            // Fertig
            return true;
        }

        /// <summary>
        /// Prüft, ob die Bewegung von unten gegen das Hindernis trifft.
        /// </summary>
        /// <returns>Gesetzt, wenn ein Treffen von unten erfolgt.</returns>
        private bool PrüfeAufAuftreffenVonUnten()
        {
            // Wir liegen völlig unterhalb der Bewegung
            if (Bereich.KleinsteVertikalePosition <= GenaueZahl.Null)
                return false;

            // Wir liegen völlig oberhalb der Bewegung
            if (Bereich.KleinsteVertikalePosition >= m_analyse.PlanungsBereich.Höhe)
                return false;

            // Die Bewegung ist rein vertikal.
            if (m_analyse.PlanungsBereich.Breite == GenaueZahl.Null)
            {
                // Das Hindernis überdeckt die vertikale Achse nicht
                if (!ÜberlagertDieVertikaleAchse)
                    return false;

                // Die Bewegung trifft uns sofort
                SchrittweiteBisZurKollision = Ausdehnung.Erzeugen( GenaueZahl.Null, Bereich.KleinsteVertikalePosition );
                KollisionGegenDieSeite = false;

                // Fertig
                return true;
            }

            // Dreisatz zum Ermitteln der horizontalen Position auf der Bewegungslinie
            var xReferenz = Bereich.KleinsteVertikalePosition * m_analyse.VolleStrecke.Breite / m_analyse.VolleStrecke.Höhe;
            var xRelativ = xReferenz - Bereich.KleinsteHorizontalePosition;

            // Wir sind zu weit weg
            if (xRelativ < GenaueZahl.Null)
                return false;

            // Wir sind zu schmal
            if (xRelativ >= Bereich.Breite)
                return false;

            // Die Bewegung trifft uns von unten
            SchrittweiteBisZurKollision = Ausdehnung.Erzeugen( xReferenz, Bereich.KleinsteVertikalePosition );
            KollisionGegenDieSeite = false;

            // Fertig
            return true;
        }

        /// <summary>
        /// Gesetzt, wenn das Hindernis direkt auf der horizontalen Achse liegt und den Ursprung der Bewegung überlagert.
        /// </summary>
        private bool LiegtAufDerHorizontalenAchse { get { return (Bereich.KleinsteVertikalePosition == GenaueZahl.Null) && ÜberlagertDieVertikaleAchse; } }

        /// <summary>
        /// Gesetzt, wenn das Hindernis direkt an der vertikalen Achse anliegt und den Ursprung der Bewegung überlagert.
        /// </summary>
        private bool LehntAnDerVertikalenAchse { get { return (Bereich.KleinsteHorizontalePosition == GenaueZahl.Null) && ÜberlagertDieHorizontaleAchse; } }

        /// <summary>
        /// Berechnet den Schnittpunkt mit der geplanten Bewegung.
        /// </summary>
        /// <returns>Der Abstand bis zur Kollision.</returns>
        public bool AufKollisionPrüfen()
        {
            // Zurücksetzen
            SchrittweiteBisZurKollision = null;

            // Schauen wir mal, ob die horizontale Richtung relevant ist
            if (PrüfeAufSeitlichesAuftreffen())
                return true;

            // Schauen wir mal, ob die vertikale Richtung relevant ist
            if (PrüfeAufAuftreffenVonUnten())
                return true;

            // Wir blockieren die vertikale Achse
            if (!LiegtAufDerHorizontalenAchse)
                if (!LehntAnDerVertikalenAchse)
                    return false;

            // Fall die Bewegung rein vertikal ist haben wir vielleicht noch etwas Spielraum ansonsten ist das Ende der Fahnenstange erreicht
            if (m_analyse.PlanungsBereich.Breite == GenaueZahl.Null)
            {
                // Von unten
                SchrittweiteBisZurKollision = Ausdehnung.Erzeugen( GenaueZahl.Null, Bereich.KleinsteVertikalePosition );
                KollisionGegenDieSeite = false;
            }
            else
            {
                // Da geht nichts mehr
                SchrittweiteBisZurKollision = Ausdehnung.Erzeugen( GenaueZahl.Null, GenaueZahl.Null );
                KollisionGegenDieSeite = LehntAnDerVertikalenAchse;
            }

            // Fertig
            return true;
        }
    }
}
