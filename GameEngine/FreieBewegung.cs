using System;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Sorgt dafür, dass ein Element sich frei bewegt
    /// </summary>
    public static class FreieBewegung
    {
        /// <summary>
        /// Aktiviert eine freie Bewegung auf einem Element.
        /// </summary>
        /// <param name="element">Das Element.</param>
        /// <param name="geschwindigkeiten">Die einzelnen Geschwindigkeiten mit einem relativen Bezugspunkt auf den Anfang des Spiels.</param>
        public static void Aktivieren( GrundElement element, params TemporaereGeschwindigkeit[] geschwindigkeiten )
        {
            // Prüfen
            if (element == null)
                throw new ArgumentNullException( "element" );

            // Nichts zu tun
            if (geschwindigkeiten == null)
                return;
            if (geschwindigkeiten.Length < 1)
                return;

            // Aktuellen index setzen
            var index = 0;

            // Aktivierungsregel
            Action<Geschwindigkeit> nächsteGeschwindigkeit = null;

            // Regel definieren
            nächsteGeschwindigkeit =
                beendeteGeschwindigkeit =>
                {
                    // Geschwindigkeit auslesen
                    var geschwindigkeit = geschwindigkeiten[index++];

                    // Auf den nächsten Schritt vorbereiten
                    index %= geschwindigkeiten.Length;

                    // Realative Zeitangabe in eine absolute umrechnen
                    var absoluteGeschwindigkeit =
                        TemporaereGeschwindigkeit.Erzeugen
                            (
                                geschwindigkeit.HorizontaleGeschwindigkeit,
                                geschwindigkeit.VertikaleGeschwindigkeit,
                                element.Spielfeld.VerbrauchteZeit + geschwindigkeit.GültigBis
                            );

                    // Überwachung auf Ende anmelden
                    absoluteGeschwindigkeit.Deaktiviert += nächsteGeschwindigkeit;

                    // Aktivieren
                    element.GeschwindigkeitsRegel.GeschwindigkeitErgänzen( absoluteGeschwindigkeit );
                };

            // Falls wir schon im Spiel sind können wir direkt loslegen, ansonsten müssen wir etwas warten
            if (element.Spielfeld != null)
                nächsteGeschwindigkeit( null );
            else
                element.SpielfeldZugeordnet += e => nächsteGeschwindigkeit( null );
        }
    }
}
