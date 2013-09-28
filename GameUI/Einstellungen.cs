using System.Windows;


namespace JMS.JnRV2.Anzeige
{
    /// <summary>
    /// Einige statischen Einstellungen.
    /// </summary>
    public sealed class Einstellungen
    {
        /// <summary>
        /// Bereitet die Nutzung dieser Klasse vor.
        /// </summary>
        static Einstellungen()
        {
            // Standardinitialisierung
            TestUmrahmung = new Thickness( 0 );
        }

        /// <summary>
        /// Erzeugt neue Einstellungen.
        /// </summary>
        public Einstellungen()
        {
        }

        /// <summary>
        /// Wird verwendet, um im Testmodus Rahmen von Elementen anzuzeigen, damit
        /// deren Dimensionen sichtbar sind.
        /// </summary>
        public static Thickness TestUmrahmung { get; set; }
    }
}
