using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Übernimmt die Steuerung der Simulation.
    /// </summary>
    public class Steuerung : IFuerSpielSteuerung, IFuerPunkteStandAnzeige
    {
        #region Schnittstelle IFuerPunkteStandAnzeige

        /// <summary>
        /// Meldet den aktuellen Punktestand.
        /// </summary>
        public uint? Punktestand
        {
            get
            {
                // Da müssen wir die Spielfigur fragen
                var spieler = m_spieler;
                if (spieler == null)
                    return null;
                else
                    return spieler.Punkte;
            }
        }

        /// <summary>
        /// Meldet die aktuelle Lebensenergie der Spielfigur.
        /// </summary>
        public int? Lebensenergie
        {
            get
            {
                // Da müssen wir die Spielfigur fragen
                var spieler = m_spieler;
                if (spieler == null)
                    return null;
                else
                    return spieler.Lebenskraft;
            }
        }

        /// <summary>
        /// Die Farbe, in der die Lebensenergie dargestellt werden soll.
        /// </summary>
        public string LebensenergieFarbe
        {
            get
            {
                // Das geht nur über die Lebensenergie
                var energie = Lebensenergie.GetValueOrDefault( 0 );
                if (energie > 1000)
                    return Colors.Green.ToString();
                else if (energie > 500)
                    return Colors.Yellow.ToString();
                else
                    return Colors.Red.ToString();
            }
        }

        #endregion

        #region Schnittstelle IFuerSpielSteuerung

        /// <summary>
        /// Meldet den Befehl zum Starten der Simulation.
        /// </summary>
        public ICommand Starten { get { return m_starten; } }

        /// <summary>
        /// Meldet den Befehl zum Anhalten der Simulation.
        /// </summary>
        public ICommand Anhalten { get { return m_anhalten; } }

        /// <summary>
        /// Meldet den Befehl, mit dem die Spielfigur schneller nach links bewegt wird.
        /// </summary>
        public ICommand SchnellerNachLinks { get { return m_nachLinks; } }

        /// <summary>
        /// Meldet den Befehl, mit dem die Spielfigur schneller nach rechts bewegt wird.
        /// </summary>
        public ICommand SchnellerNachRechts { get { return m_nachRechts; } }

        /// <summary>
        /// Meldet den Befehl, mit dem die Spielfigur angehalten wird.
        /// </summary>
        public ICommand Stillgestanden { get { return m_stillstehen; } }

        /// <summary>
        /// Meldet den Befehl, mit dem die Spielfigur zum Sprung veranlasst wird.
        /// </summary>
        public ICommand Springen { get { return m_springen; } }

        /// <summary>
        /// Gesetzt, sobald die Simulation zugeordnet wurde.
        /// </summary>
        public bool SimulationIstVerfügbar { get { return (m_simulation != null); } }

        /// <summary>
        /// Meldet die Sichtbarkeit der Steuerung.
        /// </summary>
        public Visibility Sichtbarkeit { get { return SimulationIstVerfügbar ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// Meldet, ob das Schlussbild sichtbar ist.
        /// </summary>
        public Visibility SichtbarkeitSchlussbild
        {
            get
            {
                // Sicher nicht, solange gar nichts aktiv ist
                var simulation = m_simulation;
                if (simulation == null)
                    return Visibility.Collapsed;

                // Je nach Zustand
                var zustand = simulation.Status;
                if (zustand != Ablauf.SimulationsStand.Gewonnen)
                    if (zustand != Ablauf.SimulationsStand.Verloren)
                        return Visibility.Collapsed;

                // Ja, das sollten wir uns anschauen
                return Visibility.Visible;
            }
        }

        /// <summary>
        /// Meldet das Abscshlussbild, sofern das Spiel beendet wurde.
        /// </summary>
        public IFuerBildAnzeige Schlussbild
        {
            get
            {
                // Sicher nicht, solange gar nichts aktiv ist
                var simulation = m_simulation;
                if (simulation == null)
                    return null;

                // Je nach Zustand
                var zustand = simulation.Status;
                if (zustand == Ablauf.SimulationsStand.Gewonnen)
                    return m_spielfeld.BildGewonnen;
                else if (zustand == Ablauf.SimulationsStand.Verloren)
                    return m_spielfeld.BildVerloren;
                else
                    return null;
            }
        }

        /// <summary>
        /// Meldet die Spielzeit in Sekunden.
        /// </summary>
        public int SpielzeitInSekunden { get { return m_spielzeitInSekunden; } private set { PropertyChanged.EigenschaftVerändern( this, FuerSpielSteuerung.SpielzeitInSekunden, ref m_spielzeitInSekunden, value ); } }

        /// <summary>
        /// Die aktuellen Ergebnisse.
        /// </summary>
        public IFuerErgebnisAnzeige Ergebnis { get { return m_spielfeld.Ergebnisse; } }

        /// <summary>
        /// Wird aufgerufen, sobald eine neue Melodie abgespielt werden kann.
        /// </summary>
        /// <param name="musikStarten">Methode zum Starten einer neuen Melodie.</param>
        public void MusikKannAbgespieltWerden( Action<string> musikStarten )
        {
            // Durchreichen
            ((Action<string, Action<string>>) MusikStarten).EreignisSicherAuslösen( null, musikStarten );
        }

        /// <summary>
        /// Wird ausgelöst, wenn sich etwas verändert hat.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Stellt einen Befehl für die Oberfläche zur Verfügung.
        /// </summary>
        private class Steuerbefehl : ICommand
        {
            /// <summary>
            /// Prüft, ob der Befehl ausführbar ist.
            /// </summary>
            private readonly Func<bool> m_kannAusgeführtWerden;

            /// <summary>
            /// Die Aktion, die hinter dem Befehl steht.
            /// </summary>
            private readonly Action m_aktion;

            /// <summary>
            /// Erstellt eine neue Präsentation.
            /// </summary>
            /// <param name="aktion">Die Aktion hinter dem Befehl.</param>
            /// <param name="kannAusgeführtWerden">Die Methode, mit der die Ausführbarkeit des Befehls geprüft werden kann.</param>
            public Steuerbefehl( Action aktion, Func<bool> kannAusgeführtWerden )
            {
                // Merken
                m_kannAusgeführtWerden = kannAusgeführtWerden;
                m_aktion = aktion;
            }

            /// <summary>
            /// Fordert alle Interessenten auf, den Ausführungszustand neu zu prüfen.
            /// </summary>
            public void NeuPrüfenLassen()
            {
                // Weiterleiten
                CanExecuteChanged.EreignisSicherAuslösen( this, EventArgs.Empty );
            }

            /// <summary>
            /// Meldet, ob eine Ausführung möglich ist.
            /// </summary>
            /// <param name="parameter">Wird ignoriert.</param>
            /// <returns>Gesetzt, wenn eine Ausführung möglich ist.</returns>
            public bool CanExecute( object parameter )
            {
                // Nachfragen
                return m_kannAusgeführtWerden();
            }

            /// <summary>
            /// Wird ausgelöst, wenn erneut auf die Ausführbarkeit geprüft werden soll.
            /// </summary>
            public event EventHandler CanExecuteChanged;

            /// <summary>
            /// Führt die zugeordnete Aktion aus.
            /// </summary>
            /// <param name="parameter">Wird ignoriert.</param>
            public void Execute( object parameter )
            {
                // Prüfen und los
                if (m_kannAusgeführtWerden())
                    m_aktion();
            }
        }

        /// <summary>
        /// Der Befehl zum Starten der Simulation.
        /// </summary>
        private readonly Steuerbefehl m_starten;

        /// <summary>
        /// Der Befehl zum Anhalten der Simulation.
        /// </summary>
        private readonly Steuerbefehl m_anhalten;

        /// <summary>
        /// Der Befehl um die Spielfigur schneller nach links zu bewegen.
        /// </summary>
        private readonly Steuerbefehl m_nachLinks;

        /// <summary>
        /// Der Befehl um die Spielfigur schneller nach rechts zu bewegen.
        /// </summary>
        private readonly Steuerbefehl m_nachRechts;

        /// <summary>
        /// Der Befehl um die Spielfigur anzuhalten.
        /// </summary>
        private readonly Steuerbefehl m_stillstehen;

        /// <summary>
        /// Der Befehl um die Spielfigur springen zu lassen.
        /// </summary>
        private readonly Steuerbefehl m_springen;

        /// <summary>
        /// Das zugehörige Spielfeld.
        /// </summary>
        private readonly Spielfeld m_spielfeld;

        /// <summary>
        /// Die zugehörige Simulation.
        /// </summary>
        private Ablauf.Simulation m_simulation;

        /// <summary>
        /// Die aktuelle Spielfigur.
        /// </summary>
        private Ablauf.Spieler m_spieler;

        /// <summary>
        /// Gesetzt, wenn das Ergebnis des Spiels vermerkt wurde.
        /// </summary>
        private bool m_ergebnisWurdeGemeldet;

        /// <summary>
        /// Die bisher verstrichene Spielzeit in Sekunden.
        /// </summary>
        private int m_spielzeitInSekunden;

        /// <summary>
        /// Erstellt eine neue Steuerung.
        /// </summary>
        internal Steuerung( Spielfeld spielfeld )
        {
            // Merken
            m_spielfeld = spielfeld;

            // Befehle anlegen
            m_starten = new Steuerbefehl( StartenOderFortsetzen, () => SimulationIstVerfügbar && (m_simulation.Status == Ablauf.SimulationsStand.Angehalten) );
            m_anhalten = new Steuerbefehl( AnhaltenOderBeenden, () => SimulationIstVerfügbar && (m_simulation.Status == Ablauf.SimulationsStand.Läuft) );
            m_nachRechts = new Steuerbefehl( () => m_spieler.SchnellerNachRechtsOderLangsamerNachLinks(), SpielLäuftUndSpielfigurKannBewegtWerden );
            m_nachLinks = new Steuerbefehl( () => m_spieler.SchnellerNachLinksOderLangsamerNachRechts(), SpielLäuftUndSpielfigurKannBewegtWerden );
            m_stillstehen = new Steuerbefehl( () => m_spieler.Anhalten(), SpielLäuftUndSpielfigurKannBewegtWerden );
            m_springen = new Steuerbefehl( () => m_spieler.Springen(), SpielLäuftUndSpielfigurKannBewegtWerden );

            // Verbindungen mit untergeordneten Modellen herstellen
            m_spielfeld.Ergebnisse.SichtbarkeitAuslesen = BerechneSichtbarkeitDesErgebnisses;
        }

        /// <summary>
        /// Wird aufgerufen, wenn der Punktstand verändert wurde.
        /// </summary>
        internal void PunktestandAktualisieren()
        {
            // Melden
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerPunkteStandAnzeige.Punktestand );
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Lebensenergie verändert wurde.
        /// </summary>
        internal void LebensengergieAktualisieren()
        {
            // Melden
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerPunkteStandAnzeige.Lebensenergie );
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerPunkteStandAnzeige.LebensenergieFarbe );
        }

        /// <summary>
        /// Prüft, ob die Simulation aktiv ist.
        /// </summary>
        /// <returns>Gesetzt, wenn die Simulation gesetzt ist.</returns>
        private bool SpielLäuftUndSpielfigurKannBewegtWerden()
        {
            // Das ist einfach
            return SimulationIstVerfügbar && (m_simulation.Status == Ablauf.SimulationsStand.Läuft) && (m_spieler != null);
        }

        /// <summary>
        /// Fordert eine erneute Prüfung aller Befehle an.
        /// </summary>
        private void AlleBefehleNeuPrüfen()
        {
            // Alle einzeln durchgehen
            m_starten.NeuPrüfenLassen();
            m_anhalten.NeuPrüfenLassen();
            m_springen.NeuPrüfenLassen();
            m_nachLinks.NeuPrüfenLassen();
            m_nachRechts.NeuPrüfenLassen();
            m_stillstehen.NeuPrüfenLassen();

            // Und wir nehmen auch noch das Schlussbild dazu
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerSpielSteuerung.Schlussbild );
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerSpielSteuerung.SichtbarkeitSchlussbild );

            // Durchreichen
            m_spielfeld.Ergebnisse.SichtbarkeitWurdeVerändert();

            // Geht nur, wenn die Simulation schon bereit ist
            var simulation = m_simulation;
            if (simulation == null)
                return;
            if (simulation.Status != Ablauf.SimulationsStand.Gewonnen)
                return;

            // Und alle Werte
            var punkte = Punktestand;
            if (!punkte.HasValue)
                return;
            var energie = Lebensenergie;
            if (!energie.HasValue)
                return;
            if (energie.Value < 1)
                return;

            // Haben wir das bereits gemacht?
            if (m_ergebnisWurdeGemeldet)
                return;

            // Nie wieder melden
            m_ergebnisWurdeGemeldet = true;

            // Merken
            m_spielfeld.Ergebnisse.NeuesErgebnis( punkte.Value, checked( (uint) energie.Value ) );
        }

        /// <summary>
        /// Startet die Simulation - oder setzt sie nach einer Pause fort.
        /// </summary>
        private void StartenOderFortsetzen()
        {
            // Ausführen
            m_simulation.StartenOderFortsetzen();
        }

        /// <summary>
        /// Hält die Simulation an.
        /// </summary>
        private void AnhaltenOderBeenden()
        {
            // Ausführen
            m_simulation.UnterbrechenOderBeenden( Ablauf.SimulationsStand.Angehalten );
        }

        /// <summary>
        /// Lege die Simulation fest.
        /// </summary>
        /// <param name="simulation">Die zu verwendende Simulation.</param>
        internal void SimulationSetzen( Ablauf.Simulation simulation )
        {
            // Prüfen
            if (simulation == null)
                throw new ArgumentNullException( "simulation" );

            // Das dürfen wir nur ein einziges Mal machen
            if (m_simulation != null)
                throw new NotSupportedException( "SimulationSetzen" );

            // Merken
            m_spieler = simulation.Elemente.OfType<Ablauf.Spieler>().SingleOrDefault();
            m_simulation = simulation;

            // Änderungen überwachen
            m_simulation.Zeitgeber += ( s, spielzeit ) => SpielzeitInSekunden = (int) spielzeit.TotalSeconds;
            m_simulation.SpielZustandHatSichVeraendert += s => AlleBefehleNeuPrüfen();

            // Melden
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerPunkteStandAnzeige.LebensenergieFarbe );
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerSpielSteuerung.SimulationIstVerfügbar );
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerPunkteStandAnzeige.Lebensenergie );
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerPunkteStandAnzeige.Punktestand );
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerSpielSteuerung.Sichtbarkeit );
            AlleBefehleNeuPrüfen();
        }

        /// <summary>
        /// Meldet, ob das Ergebnis sichtbar ist.
        /// </summary>
        private Visibility BerechneSichtbarkeitDesErgebnisses()
        {
            // Sicher nicht, solange gar nichts aktiv ist
            var simulation = m_simulation;
            if (simulation == null)
                return Visibility.Collapsed;

            // Je nach Zustand
            var zustand = simulation.Status;
            if (zustand != Ablauf.SimulationsStand.Gewonnen)
                return Visibility.Collapsed;

            // Ja, das sollten wir uns anschauen
            return Visibility.Visible;
        }

        #region Melodie abspielen

        /// <summary>
        /// Die Methoden zum Abspielen einer Melodie.
        /// </summary>
        private Action<string> m_musikStarten;

        /// <summary>
        /// Die nächste abzuspielende Melodie.
        /// </summary>
        private string m_nächstesMusikStück;

        /// <summary>
        /// Prüft, ob eine neue Melodie abgespielt werden soll.
        /// </summary>
        /// <param name="dateiPfad">Der Pfad zur Datei.</param>
        /// <param name="musikStarten">Methode zum Starten einer neuen Melodie.</param>
        private void MusikStarten( string dateiPfad, Action<string> musikStarten )
        {
            // Werte übernehmen
            if (!string.IsNullOrEmpty( dateiPfad ))
                m_nächstesMusikStück = dateiPfad;
            if (musikStarten != null)
                m_musikStarten = musikStarten;

            // Nichts zu tun
            dateiPfad = m_nächstesMusikStück;
            if (string.IsNullOrEmpty( dateiPfad ))
                return;
            musikStarten = m_musikStarten;
            if (musikStarten == null)
                return;

            // Zurück setzen
            m_nächstesMusikStück = null;
            m_musikStarten = null;

            // Aktivieren
            musikStarten( dateiPfad );
        }

        /// <summary>
        /// Versucht eine Melodie abzuspielen.
        /// </summary>
        /// <param name="dateiPfad">Der Pfad zur Datei.</param>
        internal void MelodieAbspielen( string dateiPfad )
        {
            // Durchreichen
            ((Action<string, Action<string>>) MusikStarten).EreignisSicherAuslösen( dateiPfad, null );
        }

        #endregion
    }
}
