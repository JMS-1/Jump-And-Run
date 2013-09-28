using System;
using System.Collections.Generic;


namespace JMS.JnRV2.Ablauf.Kollisionen
{
    /// <summary>
    /// Wird von einer Analyseeinheit für Kollisionen angeboten.
    /// </summary>
    public interface IKollisionsAnalyse
    {
        /// <summary>
        /// Ermittelt den Bewegungspfad.
        /// </summary>
        /// <param name="kollisionsMeldung">Wird ausgelöst, wenn eine Kollision stattfindet und kann
        /// den gesamten Ablauf beendet,</param>
        /// <returns>Die Liste der einzelnen Bewegungsschritte.</returns>
        IEnumerable<Ausdehnung> PfadErmitteln( Func<Fläche, bool> kollisionsMeldung = null );
    }
}
