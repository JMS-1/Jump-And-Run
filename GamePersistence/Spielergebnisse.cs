using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;


namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Alle Ergebnisse zu einem einzelnen Spiel.
    /// </summary>
    public class Spielergebnisse
    {
        /// <summary>
        /// Die Kennung des zugehörigen Spielfeldes.
        /// </summary>
        private readonly Guid m_kennung;

        /// <summary>
        /// Die Liste der besten Ergebnisse.
        /// </summary>
        public readonly List<Spielergebnis> BesteErgebnisse = new List<Spielergebnis>();

        /// <summary>
        /// Die Zahl der ausgeführten Spiele.
        /// </summary>
        public int Anzahl { get; set; }

        /// <summary>
        /// Die im Spielverlauf gesammelten Punkte.
        /// </summary>
        public ulong Punkte { get; set; }

        /// <summary>
        /// Die am Ende verbleibende Restenergie.
        /// </summary>
        public ulong Restenergie { get; set; }

        /// <summary>
        /// Das Gesamtergebnis des Spiels.
        /// </summary>
        public ulong GesamtErgebnis { get { return (uint) Math.Round( Punkte + Restenergie / 100m ); } }

        /// <summary>
        /// Erstellt neue Spielergebnisse.
        /// </summary>
        /// <param name="kennung">Die Kennung des zugehörigen Spielfeldes.</param>
        private Spielergebnisse( Guid kennung )
        {
            // Einfach nur merken
            m_kennung = kennung;
        }

        /// <summary>
        /// Lädt die Spielergebnisse aus der Ablage.
        /// </summary>
        private void Laden()
        {
            // Ablage der Spielergebnisse suchen
            AblageNutzen( true, datei =>
            {
                // Die Datei binär lesen
                using (var leser = new BinaryReader( datei ))
                    try
                    {
                        // Unsere Daten
                        Anzahl = leser.ReadInt32();
                        Punkte = leser.ReadUInt64();
                        Restenergie = leser.ReadUInt64();

                        // Alle Einzelergebnisse
                        for (var ergebnisse = leser.ReadInt32(); ergebnisse-- > 0; )
                            BesteErgebnisse.Add( Spielergebnis.Laden( leser ) );
                    }
                    catch (IOException)
                    {
                        // Fehler werden ignoriert
                    }
            } );
        }

        /// <summary>
        /// Erweitert die Spielergebnisse und aktualisiert die Ablage.
        /// </summary>
        /// <param name="punkte">Die gesammelten Punkte.</param>
        /// <param name="restEnergie">Die noch verbleibende Energie.</param>
        /// <returns>Das Gesamtergebnis - egal, ob dieses übernommen wurde oder nicht.</returns>
        public Spielergebnis NeuesErgebnis( uint punkte, uint restEnergie )
        {
            // Zählen
            Restenergie += restEnergie;
            Punkte += punkte;
            Anzahl += 1;

            // Neues Ergebnis anlegen
            var ergebnis = new Spielergebnis { Endzeitpunkt = DateTime.UtcNow, Punkte = punkte, Restenergie = restEnergie };
            var vergleich = ergebnis.Gesamtergebnis;

            // Einfügeposition ermitteln
            var neuerIndex = 0;

            // Beste Position ermitteln
            for (; neuerIndex < BesteErgebnisse.Count; neuerIndex++)
                if (vergleich >= BesteErgebnisse[neuerIndex].Gesamtergebnis)
                    break;

            // Einfügen
            BesteErgebnisse.Insert( neuerIndex, ergebnis );

            // Abschneiden
            if (BesteErgebnisse.Count > 5)
                BesteErgebnisse.RemoveRange( 5, BesteErgebnisse.Count - 5 );

            // Ablage aktualisieren
            AblageNutzen( false, datei =>
            {
                // Die Datei wird binär beschrieben
                using (var schreiber = new BinaryWriter( datei ))
                {
                    // Unsere Daten
                    schreiber.Write( Anzahl );
                    schreiber.Write( Punkte );
                    schreiber.Write( Restenergie );
                    schreiber.Write( BesteErgebnisse.Count );

                    // Alle Ergebnisse
                    BesteErgebnisse.ForEach( bestes => bestes.Speichern( schreiber ) );
                }
            } );

            // Melden
            return ergebnis;
        }

        /// <summary>
        /// Führt eine Operation auf der Ablage aus.
        /// </summary>
        /// <param name="lesen">Gesetzt, wenn die Ablage gelesen werden soll.</param>
        /// <param name="operation">Die gewünschte Operation.</param>
        private void AblageNutzen( bool lesen, Action<Stream> operation )
        {
            // Lokale Speicherung ist leider untersagt.
            if (!IsolatedStorageFile.IsEnabled)
                return;

            // An die lokale Ablage anbinden
            using (var ablage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Die Ablage kann nicht verwendet werden
                if (ablage == null)
                    return;

                // Dateiname berechnen
                var dateiName = string.Format( "Level-{0}", m_kennung.ToString( "N" ).ToUpper() );

                // Datei öffnen
                if (lesen)
                {
                    // Datei gibt es gar nicht
                    if (!ablage.FileExists( dateiName ))
                        return;

                    // Datei öffnen
                    using (var datei = ablage.OpenFile( dateiName, FileMode.Open ))
                        operation( datei );
                }
                else
                {
                    // Datei überschreiben
                    using (var datei = ablage.OpenFile( dateiName, FileMode.Create ))
                        operation( datei );
                }
            }
        }

        /// <summary>
        /// Ermittelt die Spielergebnisse.
        /// </summary>
        /// <param name="eindeutigerName">Der eindeutige Name der Ergebnisse.</param>
        /// <returns>Die gewünschten Spielergebnisse.</returns>
        public static Spielergebnisse Laden( Guid eindeutigerName )
        {
            // Die gewünschten Ergebnisse
            var ergebnisse = new Spielergebnisse( eindeutigerName );

            // Schauen wir mal, ob wir schon einmal etwas gespeichert haben
            ergebnisse.Laden();

            // Melden
            return ergebnisse;
        }
    }
}
