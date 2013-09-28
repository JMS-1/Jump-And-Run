using System;
using System.Linq;
using JMS.JnRV2.Ablage;
using JMS.JnRV2.Ablauf;


namespace JMS.JnRV2.Anzeige.Verbinder
{
    partial class SimulationsVerbinder
    {
        /// <summary>
        /// Alle bekannten Bewegungsarten dür ein Element auf dem Spielfeld.
        /// </summary>
        private static readonly Func<Ablage.ElementBewegung, GrundElement, decimal, decimal, bool>[] s_bewegungsArten =
        {
            (bewegung, element, breite, höhe) => Aktivieren(bewegung as VertikaleBewegung, element, breite, höhe),
            (bewegung, element, breite, höhe) => Aktivieren(bewegung as HorizontaleBewegung, element, breite, höhe),
            (bewegung, element, breite, höhe) => Aktivieren(bewegung as BewegungRelativZurSpielfigur, element, breite, höhe),
        };

        /// <summary>
        /// Prüft, ob das Element eine Bewegung ausführen soll.
        /// </summary>
        /// <param name="bewegung">Die Konfiguration der Bewegung.</param>
        /// <param name="element">Das Element auf dem Spielfeld.</param>
        /// <param name="breite">Die gesamte Breite der Simulation.</param>
        /// <param name="höhe">Die gesamte Höhe der Simulation.</param>
        private static void StandardAktivierung( this Ablage.ElementBewegung bewegung, GrundElement element, decimal breite, decimal höhe )
        {
            // Regeln anwendern
            var schritte = bewegung.Schritte.ToArray();
            if (schritte.Length < 1)
                return;

            // In die Präsentationssprache wandeln
            var geschwindigkeiten = schritte.Select( schritt =>
                TemporaereGeschwindigkeit.Erzeugen
                (
                    (GenaueZahl) (schritt.HorizontaleDistanz / schritt.Dauer.TotalSeconds) / breite,
                    (GenaueZahl) (schritt.VertikaleDistanz / schritt.Dauer.TotalSeconds) / höhe,
                    schritt.Dauer
                ) );

            // Anmelden
            FreieBewegung.Aktivieren( element, geschwindigkeiten.ToArray() );
        }

        /// <summary>
        /// Versucht, eine Bewegungsregel anzuwenden.
        /// </summary>
        /// <param name="bewegung">Die zu prüfende Bewegung.</param>
        /// <param name="element">Die Fläche auf dem Spielfeld.</param>
        /// <param name="breite">Die Breite des Spielfelds.</param>
        /// <param name="höhe">Die Höhe des Spielfelds.</param>
        /// <returns>Gesetzt, wenn eine Aktivierung stattgefunden hat.</returns>
        private static bool Aktivieren( VertikaleBewegung bewegung, GrundElement element, decimal breite, decimal höhe )
        {
            // Sind wir nicht
            if (bewegung == null)
                return false;

            // Durchreichen
            bewegung.StandardAktivierung( element, breite, höhe );

            // Fertig
            return true;
        }

        /// <summary>
        /// Versucht, eine Bewegungsregel anzuwenden.
        /// </summary>
        /// <param name="bewegung">Die zu prüfende Bewegung.</param>
        /// <param name="element">Die Fläche auf dem Spielfeld.</param>
        /// <param name="breite">Die Breite des Spielfelds.</param>
        /// <param name="höhe">Die Höhe des Spielfelds.</param>
        /// <returns>Gesetzt, wenn eine Aktivierung stattgefunden hat.</returns>
        private static bool Aktivieren( HorizontaleBewegung bewegung, GrundElement element, decimal breite, decimal höhe )
        {
            // Sind wir nicht
            if (bewegung == null)
                return false;

            // Durchreichen
            bewegung.StandardAktivierung( element, breite, höhe );

            // Fertig
            return true;
        }

        /// <summary>
        /// Versucht, eine Bewegungsregel anzuwenden.
        /// </summary>
        /// <param name="bewegung">Die zu prüfende Bewegung.</param>
        /// <param name="element">Die Fläche auf dem Spielfeld.</param>
        /// <param name="breite">Die Breite des Spielfelds.</param>
        /// <param name="höhe">Die Höhe des Spielfelds.</param>
        /// <returns>Gesetzt, wenn eine Aktivierung stattgefunden hat.</returns>
        private static bool Aktivieren( BewegungRelativZurSpielfigur bewegung, GrundElement element, decimal breite, decimal höhe )
        {
            // Sind wir nicht
            if (bewegung == null)
                return false;

            // Anmelden
            element.BewegungRelativZumSpielerAktivieren( bewegung.HorizontaleGeschwindigkeit, bewegung.Angriff );

            // Fertig
            return true;
        }

        /// <summary>
        /// Prüft, ob das Element eine Bewegung ausführen soll.
        /// </summary>
        /// <param name="bewegung">Die Konfiguration der Bewegung.</param>
        /// <param name="element">Das Element auf dem Spielfeld.</param>
        /// <param name="breite">Die gesamte Breite der Simulation.</param>
        /// <param name="höhe">Die gesamte Höhe der Simulation.</param>
        public static void Aktivieren( this Ablage.ElementBewegung bewegung, GrundElement element, decimal breite, decimal höhe )
        {
            // Keine Bewegung 
            if (bewegung == null)
                return;

            // Alle unterstützten Bewegungen prüfen
            if (!s_bewegungsArten.Any( art => art( bewegung, element, breite, höhe ) ))
                throw new NotSupportedException( bewegung.GetType().Name );
        }
    }
}
