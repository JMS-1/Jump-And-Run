using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using JMS.JnRV2.Ablage;
using JMS.JnRV2.Anzeige;
using JMS.JnRV2.Anzeige.Verbinder;


namespace JMS.JnRV2.Ablauf.Tests.Altlasten
{
    /// <summary>
    /// Lädt die Konfigurationsdateien der ersten Spielversion.
    /// </summary>
    partial class AltesFormatAnzeigen
    {
        /// <summary>
        /// Alle bekannten Spielfiguren.
        /// </summary>
        private Ablage.Spielfigur[] m_spielfiguren = { };

        /// <summary>
        /// Die gesamte Konfiguration des Spiels.
        /// </summary>
        private Ablage.Spiel m_spiel;

        /// <summary>
        /// Wird periodisch aktiviert.
        /// </summary>
        private Timer m_timer;

        /// <summary>
        /// Erstellt eine neue Anzeige.
        /// </summary>
        public AltesFormatAnzeigen()
        {
            // Konfiguration (XAML) laden
            InitializeComponent();

            // Abschliessen
            m_zustaende.ItemsSource = new[] { ZustandDerFigur.Ruhend, ZustandDerFigur.NachRechts, ZustandDerFigur.NachLinks, ZustandDerFigur.InDerLuft };

            // Sauber beenden
            Unloaded += ( s, a ) => VorDemBeendenAufräumen();

            // Ladevorgang aufsetzen
            DateiLader.SpielfigurenLaden(
                figuren =>
                {
                    // Merken 
                    m_spielfiguren = figuren;

                    // Nun das Spiel selbst
                    DateiLader.SpielLaden( "DasSpiel", null,
                        spiel =>
                        {
                            // Merken
                            m_spiel = spiel;

                            // Fertig
                            InitialisierungAbgeschlossen();
                        } );
                } );
        }

        /// <summary>
        /// Beendet alle aktiven Abläufe.
        /// </summary>
        private void VorDemBeendenAufräumen()
        {
            // Spiel ermitteln
            var spiel = (Anzeige.PraesentationsModelle.Spiel) sichtfenster.DataContext;
            if (spiel != null)
                spiel.Steuerung.Anhalten.Execute( null );

            // Unseren eigenen Zeitgeber
            using (m_timer)
                m_timer = null;
        }

        /// <summary>
        /// Startet die Simulation, falls möglich.
        /// </summary>
        /// <param name="sender">Der Steuerbefehl für den Start.</param>
        /// <param name="args">Wird ignoriert.</param>
        private void StarteSimulationWennMöglich( object sender, EventArgs args )
        {
            // Befehl wandeln
            var befehl = (ICommand) sender;

            // Noch nicht bereit
            if (!befehl.CanExecute( null ))
                return;

            // Abmelden
            befehl.CanExecuteChanged -= StarteSimulationWennMöglich;

            // Und loslegen
            befehl.Execute( null );
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Initialisierung abgeschlossen wurde.
        /// </summary>
        private void InitialisierungAbgeschlossen()
        {
            // Spiel erzeugen
            var aktuellesSpiel = m_spiel.ErzeugePräsentation( m_spielfiguren[0] );

            // Anmelden
            aktuellesSpiel.Steuerung.Starten.CanExecuteChanged += StarteSimulationWennMöglich;

            // Präsentationen ermitteln
            var figuren = m_spielfiguren.Select( OberflächenVerbinder.ErzeugePräsentation ).ToList();
            var spielStart = DateTime.UtcNow;

            // Animation aktivieren
            using (m_timer)
                m_timer = new Timer( zustand =>
                {
                    // Spielzeit ermitteln
                    var vergangeneZeit = DateTime.UtcNow - spielStart;

                    // Zeit an die Figuren weitergeben
                    figuren.ForEach( quelle => quelle.SpielZeit = vergangeneZeit );
                }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds( 10 ) );

            // Listen laden
            m_figuren.ItemsSource = figuren;

            // Spiel als letztes laden
            sichtfenster.DataContext = aktuellesSpiel;

            // Und los
            StarteSimulationWennMöglich( aktuellesSpiel.Steuerung.Starten, EventArgs.Empty );
        }
    }
}
