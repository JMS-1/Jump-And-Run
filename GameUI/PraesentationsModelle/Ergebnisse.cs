using System;
using System.ComponentModel;
using System.Windows;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Verwaltet die Spielergebnisse zu einem Level.
    /// </summary>
    public class Ergebnisse : IFuerErgebnisAnzeige
    {
        #region Schnittstelle IFuerErgebnisAnzeige

        /// <summary>
        /// Die eingesammelten Punkte.
        /// </summary>
        public uint Punkte { get; private set; }

        /// <summary>
        /// Die verbleibende Energie.
        /// </summary>
        public uint Restenergie { get; private set; }

        /// <summary>
        /// Das Gesamtergebnis.
        /// </summary>
        public uint Gesamtergebnis { get; private set; }

        /// <summary>
        /// Meldet, ob das Ergebnis sichtbar ist.
        /// </summary>
        public Visibility Sichtbarkeit
        {
            get
            {
                // Methode ermitteln uns ausführen
                var auslesen = SichtbarkeitAuslesen;
                if (auslesen == null)
                    return Visibility.Collapsed;
                else
                    return auslesen();
            }
        }

        /// <summary>
        /// Wird ausgelöst, wenn sich etwas verändert hat.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Die tatsächliche Verwaltung der Ergebnisse.
        /// </summary>
        private readonly Ablage.Spielergebnisse m_ergebnisse;

        /// <summary>
        /// Die Methode zum Auslesen der Sichtbarkeit.
        /// </summary>
        internal Func<Visibility> SichtbarkeitAuslesen { private get; set; }

        /// <summary>
        /// Erstellt eine neue Verwaltung.
        /// </summary>
        /// <param name="ergebnisse">Die direkte Verwaltung der Ablagestrukturen.</param>
        internal Ergebnisse( Ablage.Spielergebnisse ergebnisse )
        {
            // Prüfen
            if (ergebnisse == null)
                throw new ArgumentNullException( "ergebnisse" );

            // Merken
            m_ergebnisse = ergebnisse;
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Sichtbarkeit verändert wurde.
        /// </summary>
        internal void SichtbarkeitWurdeVerändert()
        {
            // Weiterreichen
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerErgebnisAnzeige.Sichtbarkeit );
        }

        /// <summary>
        /// Erweitert die Spielergebnisse und aktualisiert die Ablage.
        /// </summary>
        /// <param name="punkte">Die gesammelten Punkte.</param>
        /// <param name="restEnergie">Die noch verbleibende Energie.</param>
        internal void NeuesErgebnis( uint punkte, uint restEnergie )
        {
            // Durchreichen
            var ergebnis = m_ergebnisse.NeuesErgebnis( punkte, restEnergie );

            // Übernehmen
            Gesamtergebnis = ergebnis.Gesamtergebnis;
            Restenergie = ergebnis.Restenergie;
            Punkte = ergebnis.Punkte;

            // Einfach alles als verändert melden
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerErgebnisAnzeige.Punkte );
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerErgebnisAnzeige.Restenergie );
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerErgebnisAnzeige.Gesamtergebnis );
        }
    }
}
