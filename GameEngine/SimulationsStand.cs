

namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Der Stand der Simulation.
    /// </summary>
    public enum SimulationsStand
    {
        /// <summary>
        /// Das Spiel wurde angehalten, kann aber fortgesetzt werden.
        /// </summary>
        Angehalten,

        /// <summary>
        /// Das Spiel läuft gerade.
        /// </summary>
        Läuft,

        /// <summary>
        /// Das Spiel wurde verloren und kann nicht fortgesetzt werden.
        /// </summary>
        Verloren,

        /// <summary>
        /// Das Spiel wurde gewonnen und kann nicht fortgesetzt werden.
        /// </summary>
        Gewonnen,
    }
}
