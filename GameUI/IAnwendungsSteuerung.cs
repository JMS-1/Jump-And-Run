

namespace JMS.JnRV2.Anzeige
{
    /// <summary>
    /// Erlaubt es, die Anwendung neu zu starten.
    /// </summary>
    public interface IAnwendungsSteuerung
    {
        /// <summary>
        /// Führt einen Neustart aus
        /// </summary>
        void AllesVonVorne();
    }
}
