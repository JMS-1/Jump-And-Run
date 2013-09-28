using System;
using System.IO;


namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Beschreibt ein einzelnes Spielergebnis.
    /// </summary>
    public class Spielergebnis
    {
        /// <summary>
        /// Der Zeitpunkt, an dem das Spiel gewonnen wurde.
        /// </summary>
        public DateTime Endzeitpunkt { get; set; }

        /// <summary>
        /// Die im Spielverlauf gesammelten Punkte.
        /// </summary>
        public uint Punkte { get; set; }

        /// <summary>
        /// Die am Ende verbleibende Restenergie.
        /// </summary>
        public uint Restenergie { get; set; }

        /// <summary>
        /// Das Gesamtergebnis des Spiels.
        /// </summary>
        public uint Gesamtergebnis { get { return (uint) Math.Round( Punkte + Restenergie / 100m ); } }

        /// <summary>
        /// Speichert das Spielergebnis in der Ablage.
        /// </summary>
        /// <param name="datei">Die zu verwendende Ablage.</param>
        internal void Speichern( BinaryWriter datei )
        {
            // Alle Daten
            datei.Write( Endzeitpunkt.Ticks );
            datei.Write( Punkte );
            datei.Write( Restenergie );
        }

        /// <summary>
        /// Rekonstruiert ein Spielergebnis aus der Ablage.
        /// </summary>
        /// <param name="datei">Die zu verwendende Ablage.</param>
        /// <returns>Das rekonstruierte Ergebnis.</returns>
        internal static Spielergebnis Laden( BinaryReader datei )
        {
            // Ergebnis anlegen
            var ergebnis = new Spielergebnis();

            // Auslesen
            ergebnis.Endzeitpunkt = new DateTime( datei.ReadInt64() );
            ergebnis.Punkte = datei.ReadUInt32();
            ergebnis.Restenergie = datei.ReadUInt32();

            // Melden
            return ergebnis;
        }
    }
}
