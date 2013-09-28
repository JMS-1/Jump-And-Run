using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JMS.JnRV2.Ablauf.Kollisionen;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Wird von Elementen angeboten, die sich selbstständig bewegen.
    /// </summary>
    public class GeschwindigkeitsRegel : ElementRegel
    {
        /// <summary>
        /// Beschreibt die Bewegungsinformation eines Elementes.
        /// </summary>
        private class BewegungsInformation
        {
            /// <summary>
            /// Die aktuelle Geschwindigkeit des Elementes.
            /// </summary>
            public readonly Geschwindigkeit Geschwindigkeit;

            /// <summary>
            /// Das zugehörige Element.
            /// </summary>
            public readonly GrundElement Element;

            /// <summary>
            /// Die Anzahl der noch möglichen Elementarschritte.
            /// </summary>
            private int m_verbleibendeSchritte;

            /// <summary>
            /// Der pro Elementarschritt zurückgelegte horizontale Weg.
            /// </summary>
            private readonly GenaueZahl m_xSchritt;

            /// <summary>
            /// Der pro Elementarschritt zurückgelegte vertikale Weg.
            /// </summary>
            private readonly GenaueZahl m_ySchritt;

            /// <summary>
            /// Erzeugt eine neue Beschreibung.
            /// </summary>
            /// <param name="element">Das zugehörige Element.</param>
            /// <param name="spielZeit">Die bisher verstrichene Spielzeit.</param>
            /// <param name="sekunden">Die Anzahl der Sekunden seit der letzten Berechnung.</param>
            public BewegungsInformation( GrundElement element, TimeSpan spielZeit, decimal sekunden )
            {
                // Nicht nutzbar
                if (element.IstDeaktiviert)
                    return;

                // Merken
                m_verbleibendeSchritte = (int) Math.Round( 1000m * sekunden );
                Element = element;

                // Geschwindigkeit auslesen
                var geschwindigkeit = element.GeschwindigkeitsRegel.AktuelleGeschwindigkeitBerechnen( spielZeit );
                if (geschwindigkeit == null)
                    return;

                // Anfangsposition auslesen
                var aktuellePosition = element.Position;

                // Endposition berechnen
                var xEnde = (aktuellePosition.HorizontalePosition + geschwindigkeit.HorizontaleGeschwindigkeit * sekunden).ZwischenNullUndEins();
                var yEnde = (aktuellePosition.VertikalePosition + geschwindigkeit.VertikaleGeschwindigkeit * sekunden).ZwischenNullUndEins();

                // Ziel und Geschwindigkeit merken
                Geschwindigkeit = geschwindigkeit;

                // Schrittweiten berechnen
                m_xSchritt = (xEnde - aktuellePosition.HorizontalePosition) / m_verbleibendeSchritte;
                m_ySchritt = (yEnde - aktuellePosition.VertikalePosition) / m_verbleibendeSchritte;

                // Es witd sich nichts verändern
                if (m_xSchritt == GenaueZahl.Null)
                    if (m_ySchritt == GenaueZahl.Null)
                        return;

                // Dem Element mitteilen, dass es nun losgeht
                element.Bewegen( aktuellePosition );
            }

            /// <summary>
            /// Versucht einen Elementarschritt auszuführen.
            /// </summary>
            /// <param name="elemente">Alle anderen Elemente, gegen die eine Prüfung stattfindet.</param>
            /// <returns>Gesetzt, wenn eine Bewegung ausgeführt wurde.</returns>
            public bool ElementarBewegungAusführen( IEnumerable<GrundElement> elemente )
            {
                // Uns gibt es nicht mehr
                if (Element.IstDeaktiviert)
                    return false;

                // Wir sind schon fertig
                if (m_verbleibendeSchritte < 1)
                    return false;

                // Einen weniger
                m_verbleibendeSchritte -= 1;

                // Bewegung ermitteln
                var aktuellePosition = Element.Position;
                var ausdehnung = Element.Ausdehnung;
                var aktuelleFläche = Fläche.Erzeugen( aktuellePosition, ausdehnung );
                var bewegung = ElementInBewegung.Erzeugen( aktuelleFläche, m_xSchritt, m_ySchritt );
                var analyse = bewegung.BeginneAnalyse( elemente.Where( element => !ReferenceEquals( element, Element ) && !element.IstDeaktiviert ) );
                var schritte = analyse.PfadErmitteln( hindernis => ((GrundElement) hindernis).KollisionsRegel.KollisionAuswerten( Element ) ).ToArray();

                // Gesamte Bewegung
                var breite = schritte.Select( schritt => schritt.Breite ).Sum();
                var höhe = schritte.Select( schritt => schritt.Höhe ).Sum();
                if (breite == GenaueZahl.Null)
                    if (höhe == GenaueZahl.Null)
                        return false;

                // Grenzen berechnen
                var halbeBreite = ausdehnung.Breite / 2;
                var halbeHöhe = ausdehnung.Höhe / 2;

                // Neuen Mittelpunkt des Elementes berechnen
                var horizontalesEnde = aktuellePosition.HorizontalePosition + (bewegung.VonLinksNachRechts ? breite : -breite);
                var vertikalesEnde = aktuellePosition.VertikalePosition + (bewegung.VonUntenNachOben ? höhe : -höhe);

                // Sicherstellen, dass wir den Spielbereich nicht verlassen
                horizontalesEnde = GenaueZahlHelfer.Max( halbeBreite, GenaueZahlHelfer.Min( GenaueZahl.Eins - halbeBreite, horizontalesEnde ) );
                vertikalesEnde = GenaueZahlHelfer.Max( halbeHöhe, GenaueZahlHelfer.Min( GenaueZahl.Eins - halbeHöhe, vertikalesEnde ) );

                // Zielposition annehmen
                Element.Bewegen( Position.Erzeugen( horizontalesEnde, vertikalesEnde ) );

                // Wir haben uns bewegt
                return true;
            }
        }

        /// <summary>
        /// Alle Geschwindigkeiten.
        /// </summary>
        private volatile List<Geschwindigkeit> m_liste = new List<Geschwindigkeit>();

        /// <summary>
        /// Erzeugt eine neue Regel.
        /// </summary>
        internal GeschwindigkeitsRegel()
        {
        }

        /// <summary>
        /// Ergänzt eine neue Geschwindigkeit.
        /// </summary>
        /// <param name="geschwindigkeit">Die neue Geschwindigkeit.</param>
        public void GeschwindigkeitErgänzen( Geschwindigkeit geschwindigkeit )
        {
            // Muss parallel ausführbar sein
            for (; ; )
            {
                // Alte Liste
                var liste = m_liste;

                // Neue Liste
                var neu = new List<Geschwindigkeit>( liste ) { geschwindigkeit };

                // Sicher ersetzen
#pragma warning disable 0420
                if (ReferenceEquals( Interlocked.CompareExchange( ref m_liste, neu, liste ), liste ))
#pragma warning restore 0420
                    break;
            }
        }

        /// <summary>
        /// Entfernt eine einzelne Geschwindigkeitsregel.
        /// </summary>
        /// <param name="geschwindigkeit">Die zu entfernende Geschwindigkeitsregel.</param>
        public void GeschwindigkeitEntfernen( Geschwindigkeit geschwindigkeit )
        {
            // Muss parallel ausführbar sein
            for (; ; )
            {
                // Alte Liste
                var liste = m_liste;

                // Neue Liste ohne den angegebenen Eintrag
                var neu = new List<Geschwindigkeit>( liste.Where( bekannteGeschwindigkeit => !ReferenceEquals( bekannteGeschwindigkeit, geschwindigkeit ) ) );

                // Sicher ersetzen
                if (neu.Count < liste.Count)
#pragma warning disable 0420
                    if (ReferenceEquals( Interlocked.CompareExchange( ref m_liste, neu, liste ), liste ))
#pragma warning restore 0420
                        break;
            }
        }

        /// <summary>
        /// Ermittelt die aktuelle Geschwindigkeit.
        /// </summary>
        /// <param name="spielZeit">Die aktuelle Spielzeit.</param>
        /// <returns>Die aktuelle Geschwindigkeit.</returns>
        public Geschwindigkeit AktuelleGeschwindigkeitBerechnen( TimeSpan spielZeit )
        {
            // Muss parallel ausführbar sein
            for (; ; )
            {
                // Alte Losze
                var liste = m_liste;

                // Neue Liste
                var neu = new List<Geschwindigkeit>( liste );

                // Alle nicht mehr gültigen Geschwindigkeiten
                var abgelaufen = new HashSet<Geschwindigkeit>( neu.Where( geschwindigkeit => geschwindigkeit.GültigBis <= spielZeit ) );
                if (abgelaufen.Count > 0)
                {
                    // Bereinigen 
                    neu.RemoveAll( abgelaufen.Contains );

                    // Ersetzen
#pragma warning disable 0420
                    if (!ReferenceEquals( Interlocked.CompareExchange( ref m_liste, neu, liste ), liste ))
#pragma warning restore 0420
                        continue;

                    // Melden
                    foreach (var geschwindigkeit in abgelaufen)
                        geschwindigkeit.Deaktivieren();
                }

                // Eventuell veränderte Liste laden
                liste = m_liste;
                if (liste.Count < 1)
                    return null;

                // Ersten Eintrag nehmen
                var kombinierteGeschwindigkeit = liste[0];
                var xGeschwindigkeit = kombinierteGeschwindigkeit.HorizontaleGeschwindigkeit;
                var yGeschwindigkeit = kombinierteGeschwindigkeit.VertikaleGeschwindigkeit;

                // Alle Geschwindigkeiten anwenden
                foreach (var geschwindigkeit in liste.Skip( 1 ))
                {
                    // Einfach zusammen rechnen
                    xGeschwindigkeit += geschwindigkeit.HorizontaleGeschwindigkeit;
                    yGeschwindigkeit += geschwindigkeit.VertikaleGeschwindigkeit;
                }

                // Melden
                return Geschwindigkeit.Erzeugen( xGeschwindigkeit, yGeschwindigkeit );
            }
        }

        /// <summary>
        /// Bewegt alle Elemente.
        /// </summary>
        /// <param name="spielfeld">Das aktuelle Spielfeld.</param>
        /// <param name="spielZeit">Die bisherige Spielzeit.</param>
        /// <param name="verpassteZeit">Die seit dem letzten Aufruf verpasste Spielzeit.</param>
        internal static void Ausführen( Simulation spielfeld, TimeSpan spielZeit, TimeSpan verpassteZeit )
        {
            // Verpasste Zeit umrechnen
            var verpassteSekunden = (decimal) verpassteZeit.TotalSeconds;

            // Alle Elemente in Bewegung
            var elemente =
                spielfeld
                    .Elemente
                    .Select( element => new BewegungsInformation( element, spielZeit, verpassteSekunden ) )
                    .Where( info => info.Geschwindigkeit != null )
                    .ToArray();


            // Elementarbewegungen ausführen solange es nötig ist
            for (bool inBewegung = true; inBewegung; )
            {
                // Alle Elemente untersuchen
                inBewegung = elemente.Count(
                    element =>
                    {
                        // Bewegt sich nicht
                        if (!element.ElementarBewegungAusführen( spielfeld.Elemente ))
                            return false;

                        // Merken
                        inBewegung = true;

                        // Wir haben uns bewegt
                        return true;
                    } ) > 0;
            }
        }
    }
}
