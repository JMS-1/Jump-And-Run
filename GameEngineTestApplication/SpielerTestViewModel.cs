using System;
using System.Windows.Input;


namespace JMS.JnRV2.Ablauf.Tests
{
    /// <summary>
    /// Beschreibt die Spielfigur.
    /// </summary>
    public class SpielerTestViewModel : ElementTestViewModel
    {
        /// <summary>
        /// Lässt die Spielfigur schneller nach links laufen.
        /// </summary>
        public ICommand NachLinksBefehl { get; private set; }

        /// <summary>
        /// Lässt die Spielfigur schneller nach rechts laufen.
        /// </summary>
        public ICommand NachRechtsBefehl { get; private set; }

        /// <summary>
        /// Hält die horizontale Bewegung der Spielfigur an.
        /// </summary>
        public ICommand AnhaltenBefehl { get; private set; }

        /// <summary>
        /// Führt einen Sprung aus.
        /// </summary>
        public ICommand SpringenBefehl { get; private set; }

        /// <summary>
        /// Erzeugt eine neue Beschreibung einer Spielfigur.
        /// </summary>
        /// <param name="spieler">Die Spielfigur.</param>
        /// <param name="spielfeld">Das gesamte Spielfeld.</param>
        public SpielerTestViewModel( Spieler spieler, SpielfeldTestViewModel spielfeld )
            : base( spieler, spielfeld )
        {
            // Befehle anlegen
            NachRechtsBefehl = Werkzeuge.WandeleZuBefehl( Spieler.SchnellerNachRechtsOderLangsamerNachLinks );
            NachLinksBefehl = Werkzeuge.WandeleZuBefehl( Spieler.SchnellerNachLinksOderLangsamerNachRechts );
            AnhaltenBefehl = Werkzeuge.WandeleZuBefehl( Spieler.Anhalten );
            SpringenBefehl = Werkzeuge.WandeleZuBefehl( Spieler.Springen );

            // Auf Änderungen reagieren
            spieler.PunkteVerändert += s => EigenschaftVerändert( "Punkte" );
            spieler.LebenskraftVerändert += s => EigenschaftVerändert( "Energie" );
        }

        /// <summary>
        /// Meldet die bisher gesammelten Punkte.
        /// </summary>
        public uint Punkte { get { return Spieler.Punkte; } }

        /// <summary>
        /// Meldet die aktuelle Lebensenergie.
        /// </summary>
        public int Energie { get { return Spieler.Lebenskraft; } }

        /// <summary>
        /// Meldet die Spielfigur.
        /// </summary>
        private Spieler Spieler { get { return (Spieler) Element; } }
    }
}
