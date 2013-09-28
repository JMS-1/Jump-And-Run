

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Das Spiel ist zu Ende, wenn der Spieler mit diesem Element kollidiert.
    /// </summary>
    public class Enderegel : Kollisionsregel
    {
        /// <summary>
        /// Gesetzt, wenn das Spiel dann gewonnen wurde - typisch für einen Ausgang.
        /// </summary>
        public bool Gewonnen { get; set; }
    }
}
