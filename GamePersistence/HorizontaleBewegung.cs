using System;
using System.Collections.Generic;


namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt eine rein horizontale Bewegung eines Elementes.
    /// </summary>
    public class HorizontaleBewegung : ElementBewegung
    {
        /// <summary>
        /// Meldet oder legt fest, ob die Beweungung von links nach rechts erfolgt.
        /// </summary>
        public bool VonLinksNachRechts { get; set; }

        /// <summary>
        /// Meldet oder setzt die Geschwindigkeit der Bewegung.
        /// </summary>
        public double Geschwindigkeit { get; set; }

        /// <summary>
        /// Erstellt eine neue Beschreibung.
        /// </summary>
        public HorizontaleBewegung()
        {
        }

        /// <summary>
        /// Meldet die einzelnen Schritte der Bewegung.
        /// </summary>
        public override IEnumerable<Bewegungselement> Schritte
        {
            get
            {
                // Umrechnen
                var dauer = TimeSpan.FromSeconds( 1.0 / Geschwindigkeit );

                // Einfach nur in eine Richtung
                yield return new Bewegungselement( VonLinksNachRechts ? 100 : -100, 0, dauer );
            }
        }
    }
}
