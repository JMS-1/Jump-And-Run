using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JMS.JnRV2.Ablage;
using JMS.JnRV2.Anzeige;
using JMS.JnRV2.Anzeige.Praesentation;
using JMS.JnRV2.Anzeige.Verbinder;


namespace JMS.JnRV2.Start
{
    /// <summary>
    /// Stellt alle notwendigen Informationen zur Auswahl der Spielumgebung zur Verfügung.
    /// </summary>
    public class AuswahlInformationen : INotifyPropertyChanged
    {
        /// <summary>
        /// Die Einstellung mit dem Namen der Spielfigur.
        /// </summary>
        private const string EinstellungsnameFigur = "figur";

        /// <summary>
        /// Die Einstellung mit dem Namen des Spielfelds.
        /// </summary>
        private const string EinstellungsnameLevel = "level";

        /// <summary>
        /// Der Name des Spiels - wird von aussen gesetzt.
        /// </summary>
        public static string NameDesSpiels;

        /// <summary>
        /// Repräsentiert einen Befehl.
        /// </summary>
        private class AuswahlBefehl : ICommand
        {
            /// <summary>
            /// Die zugehörige Information.
            /// </summary>
            private readonly AuswahlInformationen m_information;

            /// <summary>
            /// Gesetzt, wenn einmal gestartet wurde.
            /// </summary>
            private bool m_schonAusgeführt;

            /// <summary>
            /// Erstellt einen neuen Befehl.
            /// </summary>
            /// <param name="information">Die zugehörige Auswahl.</param>
            public AuswahlBefehl( AuswahlInformationen information )
            {
                // Merken
                m_information = information;

                // Bei allen Änderungen den eigenen Zustand neu berechnen lassen
                information.PropertyChanged += ( s, a ) => CanExecuteChanged.EreignisSicherAuslösen( this, EventArgs.Empty );
            }

            /// <summary>
            /// Prüft, ob eine Ausführung möglich ist.
            /// </summary>
            /// <param name="parameter">Wird ignoriert.</param>
            /// <returns>Gesetzt, wenn eine Ausführung möglich ist.</returns>
            public bool CanExecute( object parameter )
            {
                // Fragen wir mal die Auswahl
                if (m_information.SpielKonfiguration == null)
                    return false;
                if (m_information.AktuelleSpielfigur == null)
                    return false;
                if (m_information.AktuellesSpielfeld == null)
                    return false;

                // Fragen wir uns selbst
                return !m_schonAusgeführt;
            }

            /// <summary>
            /// Wird ausgelöst, wenn sich der Ausführungsstatus verändert hat.
            /// </summary>
            public event System.EventHandler CanExecuteChanged;

            /// <summary>
            /// Löst den Befehl aus.
            /// </summary>
            /// <param name="parameter">Wird ignoriert.</param>
            public void Execute( object parameter )
            {
                // Geht das denn?
                if (!CanExecute( parameter ))
                    return;

                // Nur ein einziges Mal, bitte
                m_schonAusgeführt = true;

                // Aktion ausführen
                m_information.SimulationErzeugen();
            }
        }

        /// <summary>
        /// Der Name der Eigenschaft mit den Spielfiguren.
        /// </summary>
        private static readonly string _Figuren = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( AuswahlInformationen i ) => i.Figuren );

        /// <summary>
        /// Der Name der Eigenschaft mit der aktuell ausgewählen Spielfigur.
        /// </summary>
        private static readonly string _AktuelleSpielfigur = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( AuswahlInformationen i ) => i.AktuelleSpielfigur );

        /// <summary>
        /// Der Name der Eigenschaft mit der Liste der Spielfelder.
        /// </summary>
        private static readonly string _Spielfelder = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( AuswahlInformationen i ) => i.Spielfelder );

        /// <summary>
        /// Der Name der Eigenschaft mit dem aktuellen Spielfeld.
        /// </summary>
        private static readonly string _AktuellesSpielfeld = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( AuswahlInformationen i ) => i.AktuellesSpielfeld );

        /// <summary>
        /// Die Konfiguration des Spielumfelds.
        /// </summary>
        private static readonly string _SpielKonfiguration = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( AuswahlInformationen i ) => i.SpielKonfiguration );

        /// <summary>
        /// Die Eigenschaft mit dem aktuellen Spiel.
        /// </summary>
        public static readonly string _AktuellesSpiel = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( AuswahlInformationen i ) => i.AktuellesSpiel );

        /// <summary>
        /// Der Name der Eigenschaft mit der Sichtbarkeit des Ladebalkens - oder analoger Mechanismen.
        /// </summary>
        private static readonly string _SichtbarkeitLadebalken = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( AuswahlInformationen i ) => i.SichtbarkeitLadebalken );

        /// <summary>
        /// Die Liste aller Spielfiguren.
        /// </summary>
        public SpielfigurMitInformation[] Figuren { get; private set; }

        /// <summary>
        /// Die aktuell ausgewählte Spielfigur.
        /// </summary>
        private SpielfigurMitInformation m_aktuelleSpielfigur;

        /// <summary>
        /// Die aktuell ausgewählte Spielfigur.
        /// </summary>
        public SpielfigurMitInformation AktuelleSpielfigur
        {
            get
            {
                // Report
                return m_aktuelleSpielfigur;
            }
            set
            {
                // Schauen wir mal, ob sich etwas verändert
                if (!PropertyChanged.EigenschaftVerändern( this, _AktuelleSpielfigur, ref m_aktuelleSpielfigur, value ))
                    return;

                // Speichern vorbereiten
                var einstellungen = IsolatedStorageSettings.ApplicationSettings;
                var name = (value == null) ? null : value.Name;

                // Merken
                einstellungen[EinstellungsnameFigur] = name;

                // Und für später fixieren
                einstellungen.Save();
            }
        }

        /// <summary>
        /// Alle Spielfelder.
        /// </summary>
        private SpielfeldMitInformation[] m_spielfelder;

        /// <summary>
        /// Alle Spielfelder.
        /// </summary>
        public SpielfeldMitInformation[] Spielfelder { get; private set; }

        /// <summary>
        /// Das aktuelle Spielfeld.
        /// </summary>
        private SpielfeldMitInformation m_aktuellesSpielfeld;

        /// <summary>
        /// Meldet oder ändert das aktuelle Spielfeld.
        /// </summary>
        public SpielfeldMitInformation AktuellesSpielfeld
        {
            get
            {
                // Report
                return m_aktuellesSpielfeld;
            }
            set
            {
                // Update
                if (!PropertyChanged.EigenschaftVerändern( this, _AktuellesSpielfeld, ref m_aktuellesSpielfeld, value ))
                    return;

                // Speichern vorbereiten
                var einstellungen = IsolatedStorageSettings.ApplicationSettings;
                var kennung = (value == null) ? null : value.Konfiguration.Kennung;

                // Merken
                einstellungen[EinstellungsnameLevel] = kennung;

                // Und für später fixieren
                einstellungen.Save();
            }
        }

        /// <summary>
        /// Der Befehl zum Starten des Spiels.
        /// </summary>
        public ICommand Starten { get; private set; }

        /// <summary>
        /// Die Präsentation für das aktuell laufende Spiel.
        /// </summary>
        public Anzeige.PraesentationsModelle.Spiel AktuellesSpiel { get; private set; }

        /// <summary>
        /// Die Konfiuguration des vorgegebenen Spiels.
        /// </summary>
        private Spiel m_spiel;

        /// <summary>
        /// Die Konfiuguration des vorgegebenen Spiels.
        /// </summary>
        public Spiel SpielKonfiguration
        {
            get
            {
                // Report
                return m_spiel;
            }
            set
            {
                // Update
                PropertyChanged.EigenschaftVerändern( this, _SpielKonfiguration, ref m_spiel, value );
            }
        }

        /// <summary>
        /// Erzeugt einen neuen Satz von Informationen.
        /// </summary>
        public AuswahlInformationen()
        {
            // Initialisierung beenden
            SichtbarkeitLadebalken = Visibility.Collapsed;
            Spielfelder = new SpielfeldMitInformation[0];
            Figuren = new SpielfigurMitInformation[0];
            Starten = new AuswahlBefehl( this );

            // Alles aynchron laden
            DateiLader.SpielLaden( NameDesSpiels, SpielWurdeGeladen );
            DateiLader.SpielfigurenLaden( SpielfigurenWurdenGeladen );
            DateiLader.SpielfelderLaden( SpielfelderWurdenGeladen );
        }

        /// <summary>
        /// Wird aufgerufen, sobald alle Spielfiguren geladen sind.
        /// </summary>
        /// <param name="figuren">Alle bekannten Spielfiguren.</param>
        private void SpielfigurenWurdenGeladen( Spielfigur[] figuren )
        {
            // Wandeln
            if (figuren != null)
                Figuren = figuren.Select( figur => new SpielfigurMitInformation( figur ) ).ToArray();

            // Blind melden
            PropertyChanged.EigenschaftWurdeVerändert( this, _Figuren );

            // Aktuelle Einstellungen laden
            string name;
            if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue( EinstellungsnameFigur, out name ))
                AktuelleSpielfigur = null;
            else
                AktuelleSpielfigur = Figuren.FirstOrDefault( figur => StringComparer.Ordinal.Equals( name, figur.Name ) );
        }

        /// <summary>
        /// Wird aufgerufen, sobald alle Spielfelder geladen sind.
        /// </summary>
        /// <param name="spielfelder">Alle bekannten Spielfelder.</param>
        private void SpielfelderWurdenGeladen( Spielfeld[] spielfelder )
        {
            // Wandeln
            if (spielfelder != null)
                m_spielfelder = spielfelder.Select( spielfeld => new SpielfeldMitInformation( spielfeld ) ).ToArray();

            // Auswahlliste vorbereiten
            SpielFelderAuswahlVorbereiten();
        }

        /// <summary>
        /// Bereitet die Auswahlliste der Spielfelder vor.
        /// </summary>
        private void SpielFelderAuswahlVorbereiten()
        {
            // Geht noch nicht
            if (m_spielfelder == null)
                return;
            if (SpielKonfiguration == null)
                return;

            // Übernehmen
            Spielfelder = m_spielfelder.Where( spielfeld => SpielKonfiguration.SpielfeldIstGültig( spielfeld.Konfiguration ) ).ToArray();

            // Blind melden
            PropertyChanged.EigenschaftWurdeVerändert( this, _Spielfelder );

            // Aktuelle Einstellungen laden
            string kennung;
            if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue( EinstellungsnameLevel, out kennung ))
                AktuellesSpielfeld = Spielfelder.FirstOrDefault();
            else
                AktuellesSpielfeld = Spielfelder.FirstOrDefault( feld => StringComparer.Ordinal.Equals( kennung, feld.Konfiguration.Kennung ) );
        }

        /// <summary>
        /// Gesetzt, wenn ein Ladevorgang aktiv ist.
        /// </summary>
        public Visibility SichtbarkeitLadebalken { get; private set; }

        /// <summary>
        /// Wird aufgerufen, wenn die Simulation des Spiel angelegt werden soll.
        /// </summary>
        private void SimulationErzeugen()
        {
            // Spielfeld ermitteln
            var spielfeld = AktuellesSpielfeld;
            if (spielfeld == null)
                return;

            // Spielfigur ermitteln
            var spielfigur = AktuelleSpielfigur;
            if (spielfigur == null)
                return;

            // Ladevorgang läuft nun
            SichtbarkeitLadebalken = Visibility.Visible;

            // Informieren
            PropertyChanged.EigenschaftWurdeVerändert( this, _SichtbarkeitLadebalken );

            // Ausgewähltes Spielfeld aktivieren
            SpielKonfiguration.Spielfeld = spielfeld.Konfiguration;

            // Präsentation des Spiels erzeugen
            AktuellesSpiel = SpielKonfiguration.ErzeugePräsentation( spielfigur.Konfiguration );

            // Jetzt müssen wir nur noch auf die Simulation warten
            AktuellesSpiel.Steuerung.PropertyChanged += SimulationWurdeVerändert;

            // Einmal testen
            BereitschaftDerSimulationPrüfen();
        }

        /// <summary>
        /// Wird aufgerufen, sobald das Spiel verfügbar ist.
        /// </summary>
        /// <param name="spiel">Das neu geladene Spiel.</param>
        private void SpielWurdeGeladen( Spiel spiel )
        {
            // Merken
            if (spiel != null)
                SpielKonfiguration = spiel;

            // Auswahlliste vorbereiten
            SpielFelderAuswahlVorbereiten();
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich an der Simulation irgendetwas veräandert hat.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Beschreibt die Änderung.</param>
        private void SimulationWurdeVerändert( object sender, PropertyChangedEventArgs e )
        {
            // Interessiert uns nicht
            if (!FuerSpielSteuerung.SimulationIstVerfügbar.Equals( e.PropertyName ))
                return;

            // Durchreichen
            BereitschaftDerSimulationPrüfen();
        }

        /// <summary>
        /// Prüft, ob das Spiel nun begonnen werden kann.
        /// </summary>
        private void BereitschaftDerSimulationPrüfen()
        {
            // Nö, noch nicht
            var steuerung = AktuellesSpiel.Steuerung;
            if (!steuerung.SimulationIstVerfügbar)
                return;

            // Weitere Änderungen interessieren und hier nicht mehr
            steuerung.PropertyChanged -= SimulationWurdeVerändert;

            // Wartemeldung ausblenden
            SichtbarkeitLadebalken = Visibility.Collapsed;

            // Änderung melden
            PropertyChanged.EigenschaftWurdeVerändert( this, _SichtbarkeitLadebalken );

            // Abschliessend melden, dass nun ein Spiel verfügbar ist
            PropertyChanged.EigenschaftWurdeVerändert( this, _AktuellesSpiel );
        }

        /// <summary>
        /// Wird ausgelöst, wenn sich etwas verändert hat.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
