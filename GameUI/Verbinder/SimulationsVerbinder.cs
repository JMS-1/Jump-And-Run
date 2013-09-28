using System;
using System.Windows;
using JMS.JnRV2.Ablauf;


namespace JMS.JnRV2.Anzeige.Verbinder
{
    /// <summary>
    /// Hilfsmethoden zur Konfiguration der Simulation.
    /// </summary>
    internal static partial class SimulationsVerbinder
    {
        /// <summary>
        /// Ermittelt einen ganze Zahl aus einer Festkommazahl.
        /// </summary>
        /// <param name="koordinate">Eine Bildschirmkoordinate, eventuell mit Nachkommastellen.</param>
        /// <returns>Die Koordinate als ganze Zahl.</returns>
        private static double InPixelWandeln( decimal koordinate )
        {
            // Das ist nicht sehr schwer
            return (double) Math.Round( koordinate, 0 );
        }

        /// <summary>
        /// Wird aufgerufen, um die Spielumgebung aufzusetzen.
        /// </summary>
        /// <param name="spiel">Ein Spiel, das nun vollständig verfügbar ist. Insbesondere sind die Dimensionen
        /// aller Elemente auf dem Spielfeld bekannt.</param>
        public static void SpielumgebungAufsetzen( this PraesentationsModelle.Spiel spiel )
        {
            // Sichtbaren Bereich ermitteln
            var spielfeld = spiel.Spielfeld;
            var hintergrund = spielfeld.Hintergrund;
            var höheSpielfeld = (decimal) hintergrund.Hoehe;
            var links = (decimal) (spiel.Breite / 2);
            var unten = (decimal) (spiel.Hoehe / 2);
            var breite = (decimal) (hintergrund.Breite - spiel.Breite);
            var höhe = höheSpielfeld - (decimal) spiel.Hoehe;

            // Spielfeld anlegen
            var simulation = new Simulation();

            // Ausdehung der Spielfigur berücksichtigen
            var spielfigur = spiel.Spielfigur;
            if (spielfigur != null)
            {
                // Auslesen
                var breiteDerFigur = (decimal) spielfigur.Breite;
                var höheDerFigur = (decimal) spielfigur.Hoehe;

                // Korrigieren
                links -= breiteDerFigur / 2;
                unten -= höheDerFigur / 2;
                breite += breiteDerFigur;
                höhe += höheDerFigur;

                // Fallgeschwindigkeit schätzen
                simulation.FallGeschwindigkeit = (GenaueZahl) (Ablage.Skalierungswerte.Fallgeschwindigkeit * höheDerFigur / höheSpielfeld);
            }

            // Methode zum zurückrechnen    
            Func<Bereich, Rect> absoluteKoordinaten = bereich =>
                new Rect
                (
                    InPixelWandeln( (decimal) bereich.KleinsteHorizontalePosition * breite + links ),
                    InPixelWandeln( (decimal) bereich.KleinsteVertikalePosition * höhe + unten ),
                    InPixelWandeln( (decimal) bereich.Breite * breite ),
                    InPixelWandeln( (decimal) bereich.Höhe * höhe )
                );

            // Alle Elemente dort ergänzen
            foreach (PraesentationsModelle.Element<IVerbinderErzeuger> element in spielfeld.Elemente)
            {
                // Das Element mit dem Spiel verbinden
                element.Spiel = spiel;

                // Elemente oberhalb der Spielfigur werden von der Simulation nicht erfasst
                if (element.Ebene > 0)
                    continue;

                // Koordinaten blind umrechnen 
                var relativUnten = (höheSpielfeld - (decimal) (element.VertikalePosition + element.Hoehe) - unten) / höhe;
                var relativLinks = ((decimal) element.HorizontalePosition - links) / breite;
                var relativeBreite = (decimal) element.Breite / breite;
                var relativeHöhe = (decimal) element.Hoehe / höhe;

                // Horizontal beschneiden
                if (relativLinks > 1)
                    continue;
                else if (relativLinks < 0)
                {
                    // Später anfangen und etwas schmaler
                    relativeBreite += relativLinks;
                    relativLinks = 0;
                }

                // Vertikal beschneiden
                if (relativUnten > 1)
                    continue;
                else if (relativUnten < 0)
                {
                    // Später anfangen und etwas niedriger
                    relativeHöhe += relativUnten;
                    relativUnten = 0;
                }

                // Breite beschneiden
                if (relativeBreite <= 0)
                    continue;
                else if ((relativLinks + relativeBreite) > 1)
                    relativeBreite = 1 - relativLinks;

                // Höhe beschneiden
                if (relativeHöhe <= 0)
                    continue;
                else if ((relativUnten + relativeHöhe) > 1)
                    relativeHöhe = 1 - relativUnten;

                // Ausdehung der Fläche zum Element
                var position = Position.Erzeugen( (GenaueZahl) (relativLinks + relativeBreite / 2), (GenaueZahl) (relativUnten + relativeHöhe / 2) );
                var ausdehung = Ausdehnung.Erzeugen( (GenaueZahl) (relativeBreite), (GenaueZahl) (relativeHöhe) );

                // Der Algorithmus zur Erzeugung des Elementes
                var initialisierung = element.InitialisiereVerbinder();

                // Element erzeugen
                var verbinder = initialisierung.Erzeuge( position, ausdehung, breite, höhe, absoluteKoordinaten, element );

                // Element im Spielfeld anmelden
                simulation.ElementHinzufügen( verbinder );
            }

            // Zeitgeber anbinden
            simulation.Zeitgeber += ( s, zeit ) => spiel.GespielteZeit = zeit;

            // Simulation anbinden
            ((PraesentationsModelle.Steuerung) spiel.Steuerung).SimulationSetzen( simulation );
        }
    }
}
