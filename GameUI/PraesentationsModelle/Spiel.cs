using System;
using System.ComponentModel;
using System.Windows;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Die Konfiguration eines Spiels.
    /// </summary>
    public class Spiel : IFuerSpielAnzeige
    {
        #region Schnittstelle IFuerSpielAnzeige

        /// <summary>
        /// Die Breite des Sichtfensters.
        /// </summary>
        public double Breite { get; private set; }

        /// <summary>
        /// Die Höhe des Sichtfensters.
        /// </summary>
        public double Hoehe { get; private set; }

        /// <summary>
        /// Meldet die horizontale Position des Sichtfensters, so wie sie in der Verschiebung verwendet wird.
        /// </summary>
        public double HorizontaleVerschiebung { get { return -(HorizontalePosition - Breite / 2); } }

        /// <summary>
        /// Meldet die vertikale Position des Sichtfensters, so wie sie in der Verschiebung verwendet wird.
        /// </summary>
        public double VertikaleVerschiebung
        {
            get
            {
                // Wir brauchen die Höhe des Hintergrundbildes
                var bild = m_Spielfeld.Hintergrund;
                if (bild == null)
                    return 0;
                var höhe = bild.Hoehe;
                if (double.IsNaN( höhe ))
                    return 0;

                // Immer als relative Angabe
                return -(höhe - VertikalePosition - Hoehe / 2);
            }
        }

        /// <summary>
        /// Das aktuelle Spielfeld.
        /// </summary>
        public IFuerSpielfeldAnzeige Spielfeld { get { return m_Spielfeld; } }

        /// <summary>
        /// Die Steuerung der Simulation.
        /// </summary>
        public IFuerSpielSteuerung Steuerung { get; private set; }

        /// <summary>
        /// Wird bei Veränderungen aufgerufen.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Die horizontale Position des Sichtfensters.
        /// </summary>
        private double HorizontalePosition
        {
            get
            {
                // Da müssen wir die Spielfigur fragen
                var figur = Spielfigur;
                var breite = figur.Breite;
                if (double.IsNaN( breite ))
                    return 0;

                // Melden
                return figur.EchteHorizontalePosition + breite / 2;
            }
        }

        /// <summary>
        /// Die vertikale Position des Sichtfensters.
        /// </summary>
        private double VertikalePosition
        {
            get
            {
                // Da müssen wir die Spielfigur fragen
                var figur = Spielfigur;
                var höhe = figur.Hoehe;
                if (double.IsNaN( höhe ))
                    return 0;

                // Melden
                return figur.EchteVertikalePosition + höhe / 2;
            }
        }

        /// <summary>
        /// Das aktuelle Spielfeld.
        /// </summary>
        private readonly Spielfeld m_Spielfeld;

        /// <summary>
        /// Wird aufgerufen, wenn sich Eckdaten des Spielfelds verändert haben.
        /// </summary>
        /// <param name="sender">Das Spielfeld.</param>
        /// <param name="e">Informationen zur Änderung.</param>
        private void HintergrundBildWurdeVerändert( object sender, PropertyChangedEventArgs e )
        {
            // Uns interessieren nur bestimmte Information
            if (FuerBildAnzeige.Breite.Equals( e.PropertyName ))
                PropertyChanged.EigenschaftWurdeVerändert( this, FuerSpielAnzeige.HorizontaleVerschiebung );
            else if (FuerBildAnzeige.Höhe.Equals( e.PropertyName ))
                PropertyChanged.EigenschaftWurdeVerändert( this, FuerSpielAnzeige.VertikaleVerschiebung );
        }

        /// <summary>
        /// Die aktuelle Spielzeit.
        /// </summary>
        private TimeSpan m_spielZeit;

        /// <summary>
        /// Meldet oder setzt die bisher verstrichene Spielzeit.
        /// </summary>
        internal TimeSpan GespielteZeit
        {
            set
            {
                // Keine Änderung
                if (value == m_spielZeit)
                    return;

                // Merken
                m_spielZeit = value;

                // Durchreichen
                m_Spielfeld.GespielteZeit = value;
            }
        }

        /// <summary>
        /// Wird aufgerufen, um die Spielumgebung aufzusetzen.
        /// </summary>
        private readonly Action<Spiel> m_umgebungAufsetzen;

        /// <summary>
        /// Die aktuelle Spielfigur.
        /// </summary>
        internal Element Spielfigur { get; private set; }

        /// <summary>
        /// Erstellt eine neue Konfiguration.
        /// </summary>
        /// <param name="breite">Die Breite des Sichtfensters.</param>
        /// <param name="höhe">Die Höhe des Sichtfensters.</param>
        /// <param name="spielfeld">Das zugehörige Spielfeld.</param>
        /// <param name="spielfigur">Optional die zu verwendende Spielfigur.</param>
        /// <param name="umgebungAufsetzen">Wird aufgerufen, um die Spielumgebung aufzusetzen.</param>
        internal static Spiel Erzeugen( double breite, double höhe, Spielfeld spielfeld, Element spielfigur, Action<Spiel> umgebungAufsetzen )
        {
            // Prüfen
            if (breite <= 0)
                throw new ArgumentOutOfRangeException( "breite" );
            if (höhe <= 0)
                throw new ArgumentOutOfRangeException( "höhe" );
            if (spielfeld == null)
                throw new ArgumentNullException( "spielfeld" );
            if (umgebungAufsetzen == null)
                throw new ArgumentNullException( "umgebungAufsetzen" );

            // Durchreichen
            return new Spiel( breite, höhe, spielfeld, spielfigur, umgebungAufsetzen );
        }

        /// <summary>
        /// Erstellt eine neue Konfiguration.
        /// </summary>
        /// <param name="breite">Die Breite des Sichtfensters.</param>
        /// <param name="höhe">Die Höhe des Sichtfensters.</param>
        /// <param name="spielfeld">Das zugehörige Spielfeld.</param>
        /// <param name="spielfigur">Optional die zu verwendende Spielfigur.</param>
        /// <param name="umgebungAufsetzen">Wird aufgerufen, um die Spielumgebung aufzusetzen.</param>
        private Spiel( double breite, double höhe, Spielfeld spielfeld, Element spielfigur, Action<Spiel> umgebungAufsetzen )
        {
            // Initialisierung abschliessen
            Steuerung = new Steuerung( spielfeld );

            // Merken
            m_umgebungAufsetzen = umgebungAufsetzen;

            // Spielfeld und -figur als erstes übernehmen
            Spielfigur = spielfigur;
            m_Spielfeld = spielfeld;

            // Änderungen überwachen
            m_Spielfeld.Hintergrund.PropertyChanged += HintergrundBildWurdeVerändert;
            m_Spielfeld.PropertyChanged += SpielfeldWurdeVerändert;
            Spielfigur.PropertyChanged += SpielfigurZentrieren;

            // Übernehmen
            Breite = breite;
            Hoehe = höhe;

            // Schauen wir mal, ob wie schon loslegen können
            AufSpielumgebungPrüfen();
        }

        /// <summary>
        /// Setzt die Spielfigur in die Mitte der Anzeige.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Informationen zur Veränderung.</param>
        private void SpielfigurZentrieren( object sender, PropertyChangedEventArgs e )
        {
            // Was uns so interessiert
            var geänderteEigenschaft = e.PropertyName;
            if (!Element._EchteHorizontalePosition.Equals( geänderteEigenschaft ))
                if (!Element._EchteVertikalePosition.Equals( geänderteEigenschaft ))
                    if (!FuerBildAnzeige.Breite.Equals( geänderteEigenschaft ))
                        if (!FuerBildAnzeige.Höhe.Equals( geänderteEigenschaft ))
                            return;

            // Wir brauchen dazu alle Informationen der Spielfigur
            var figur = Spielfigur;
            if (!figur.IstVerfügbar)
                return;
            if (double.IsNaN( figur.Breite ))
                return;
            if (double.IsNaN( figur.Hoehe ))
                return;

            // Der große Rundumschlag
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerSpielAnzeige.HorizontaleVerschiebung );
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerSpielAnzeige.VertikaleVerschiebung );
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich das Spielfeld verändert hat.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Informationen zur Veränderung.</param>
        private void SpielfeldWurdeVerändert( object sender, PropertyChangedEventArgs e )
        {
            // Schauen wir einmal nach, ob es uns interessiert
            if (PraesentationsModelle.Spielfeld._IstVerfügbar.Equals( e.PropertyName ))
                AufSpielumgebungPrüfen();
        }

        /// <summary>
        /// Startet das Aufsetzen im Hintergrund, sobald das Spielfeld verfügbar ist.
        /// </summary>
        private void AufSpielumgebungPrüfen()
        {
            // Ups, noch ist es nicht soweit
            if (!m_Spielfeld.IstVerfügbar)
                return;

            // Das müssen wir nur ein einziges Mal machen
            m_Spielfeld.PropertyChanged -= SpielfeldWurdeVerändert;

            // Demnächst einmal aufrufen
            Deployment.Current.Dispatcher.BeginInvoke( m_umgebungAufsetzen, this );
        }
    }
}
