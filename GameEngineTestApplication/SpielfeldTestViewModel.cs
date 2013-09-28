using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace JMS.JnRV2.Ablauf.Tests
{
    /// <summary>
    /// Präsentiert ein Spielfeld.
    /// </summary>
    public class SpielfeldTestViewModel
    {
        /// <summary>
        /// Das verwaltete Spielfeld.
        /// </summary>
        private readonly Simulation m_spielfeld = new Simulation();

        /// <summary>
        /// Die Liste aller Elemente
        /// </summary>
        private readonly List<ElementTestViewModel> m_elemente = new List<ElementTestViewModel>();

        /// <summary>
        /// Der Befehl zum Starten (oder Fortsetzen) der Spielsimulation.
        /// </summary>
        public ICommand StartBefehl { get; private set; }

        /// <summary>
        /// Der Befehl zum Beenden (oder Unterbrechen) der Spielsimulation.
        /// </summary>
        public ICommand BeendenBefehl { get; private set; }

        /// <summary>
        /// Die aktuelle Spielfigur.
        /// </summary>
        public SpielerTestViewModel Spieler { get; private set; }

        /// <summary>
        /// Erstellt eine neue Präsentation.
        /// </summary>
        public SpielfeldTestViewModel()
        {
            // Befehle anlegen
            BeendenBefehl = SpielZustandTestViewModel.ErzeugeBeendenBefehl( m_spielfeld );
            StartBefehl = SpielZustandTestViewModel.ErzeugeStartBefehl( m_spielfeld );
        }

        /// <summary>
        /// Meldet die Liste aller Elemente.
        /// </summary>
        public IEnumerable<ElementTestViewModel> Elemente { get { return m_elemente.AsReadOnly(); } }

        /// <summary>
        /// Ergänzt ein neues Element.
        /// </summary>
        /// <param name="element">Das gewünschte Element.</param>
        /// <returns>Die Präsentation des Elementes.</returns>
        public ElementTestViewModel Add( GrundElement element )
        {
            // Prüfen
            var viewModel = ElementTestViewModel.Erzeugen( element, this );

            // Schauen wir mal, ob das die Spielfigur ist
            var spieler = viewModel as SpielerTestViewModel;
            if (!ReferenceEquals( spieler, null ))
                if (ReferenceEquals( Spieler, null ))
                    Spieler = spieler;
                else
                    throw new InvalidOperationException( "Spieler" );

            // Durchreichen
            m_spielfeld.ElementHinzufügen( element );

            // Merken
            m_elemente.Add( viewModel );

            // Melden
            return viewModel;
        }
    }
}
