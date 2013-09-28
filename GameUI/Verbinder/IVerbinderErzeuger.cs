using System;
using System.Windows;
using JMS.JnRV2.Ablauf;


namespace JMS.JnRV2.Anzeige.Verbinder
{
    /// <summary>
    /// Über diese Schnittstelle werden die Verbinder erzeugt.
    /// </summary>
    internal interface IVerbinderErzeuger
    {
        /// <summary>
        /// Erzeugt eine neue Verbindung.
        /// </summary>
        /// <param name="position">Die Position des Elementes in der Simulation.</param>
        /// <param name="ausdehnung">Die Ausdehung des Elementes in der Simulation.</param>
        /// <param name="breite">Die gesamte Breite der Simulation.</param>
        /// <param name="höhe">Die gesamte Höhe der Simulation.</param>
        /// <param name="umrechner">Methode zum Ermitteln der absoluten Koordinaten.</param>
        /// <param name="element">Das Element, auf das sich diese Verbindung bezieht.</param>
        /// <returns>Einen neuen Verbinder.</returns>
        GrundElement Erzeuge( Position position, Ausdehnung ausdehnung, decimal breite, decimal höhe, Func<Bereich, Rect> umrechner, PraesentationsModelle.Element element );
    }
}
