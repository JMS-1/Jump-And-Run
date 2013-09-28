using System;
using System.Windows;
using System.Windows.Input;


namespace JMS.JnRV2.Ablauf.Tests
{
    /// <summary>
    /// Verwaltet den aktuellen Spielzustand und erlaubt ein Wechsel zwischen den Zuständen.
    /// </summary>
    public class SpielZustandTestViewModel : ICommand
    {
        /// <summary>
        /// Das zugehörige Spielfeld.
        /// </summary>
        private readonly Simulation m_spielfeld;

        /// <summary>
        /// Gesetzt, wenn es sich hier der Befehl zum Starten der Spielsimulation handelt.
        /// </summary>
        private readonly bool m_starter;

        /// <summary>
        /// Erstellt einen Befehl zum Starten der Spielsimulation.
        /// </summary>
        /// <param name="spielfeld">Das zugehörige Spielfeld.</param>
        /// <returns>Der gewünschte Befehl.</returns>
        public static ICommand ErzeugeStartBefehl( Simulation spielfeld ) { return new SpielZustandTestViewModel( spielfeld, true ); }

        /// <summary>
        /// Erstellt einen Befehl zum Beenden der Spielsimulation.
        /// </summary>
        /// <param name="spielfeld">Das zugehörige Spielfeld.</param>
        /// <returns>Der gewünschte Befehl.</returns>
        public static ICommand ErzeugeBeendenBefehl( Simulation spielfeld ) { return new SpielZustandTestViewModel( spielfeld, false ); }

        /// <summary>
        /// Erzeugt eine neue Präsentation.
        /// </summary>
        /// <param name="spielfeld">Das zugehörige Spielfeld.</param>
        /// <param name="spielStarten">Gesetzt, wenn dieser Befehl das Spiel starten soll.</param>
        private SpielZustandTestViewModel( Simulation spielfeld, bool spielStarten )
        {
            // Merken
            m_starter = spielStarten;
            m_spielfeld = spielfeld;

            // Anmelden
            m_spielfeld.SpielZustandHatSichVeraendert += NeuerSpielZustand;
        }

        /// <summary>
        /// Wird immer aufgerufen, wenn sich der Zustand des Spiels an sich verändert hat.
        /// </summary>
        /// <param name="spielfeld">Wird ignoriert.</param>
        private void NeuerSpielZustand( Simulation spielfeld )
        {
            // Durchreihen
            CanExecuteChanged.EreignisAuslösen( this, EventArgs.Empty );
        }

        /// <summary>
        /// Prüft, ob die zugehörige Änderung ausgeführt werden kann.
        /// </summary>
        /// <param name="parameter">Wird ignoriert.</param>
        /// <returns>Gesetzt, wenn der zugehörige Befehl ausgeführt werden kann.</returns>
        public bool CanExecute( object parameter )
        {
            // Einfach nur vergleichen
            if (m_starter)
                return m_spielfeld.Status == SimulationsStand.Angehalten;
            else
                return m_spielfeld.Status == SimulationsStand.Läuft;
        }

        /// <summary>
        /// Wird ausgelöst, wenn sich der Spielzustand verändert hat.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Führt die Änderung am Spielzustand aus.
        /// </summary>
        /// <param name="parameter">Wird ignoriert.</param>
        public void Execute( object parameter )
        {
            // Gewünschte Änderung auslösen
            if (m_starter)
                m_spielfeld.StartenOderFortsetzen();
            else
                m_spielfeld.UnterbrechenOderBeenden( SimulationsStand.Angehalten );
        }
    }
}
