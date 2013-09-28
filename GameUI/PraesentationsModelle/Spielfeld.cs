using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Präsentiert die Konfiguration eines Spielfeldes.
    /// </summary>
    public class Spielfeld : IFuerSpielfeldAnzeige
    {
        #region Schnittstelle IFuerSpielfeldAnzeige

        /// <summary>
        /// Das Hintergrundbild des Spielfelds.
        /// </summary>
        public IFuerBildAnzeige Hintergrund { get { return m_hintergrund; } }

        /// <summary>
        /// Meldet alle Elemente auf diesem Spielfeld.
        /// </summary>
        public IEnumerable<IFuerElementAnzeige> Elemente { get { return m_elemente.AsReadOnly(); } }

        /// <summary>
        /// Meldet, ob das Spielfeld schon sichtbar ist.
        /// </summary>
        public Visibility Sichtbarkeit { get { return IstVerfügbar ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// Wird bei Veränderungen aufgerufen.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Der Name der Eigenschaft zur Verfügbarkeit des Spielfeldes.
        /// </summary>
        internal static readonly string _IstVerfügbar = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( Spielfeld i ) => i.IstVerfügbar );

        /// <summary>
        /// Das Hintergrundbild des Spielfelds.
        /// </summary>
        private readonly EinzelBild m_hintergrund;

        /// <summary>
        /// Alle Elemente auf diesem Spielfeld.
        /// </summary>
        private readonly List<Element> m_elemente;

        /// <summary>
        /// Prüft, ob das Spielfeld vollständig geladen wurde.
        /// </summary>
        internal bool IstVerfügbar
        {
            get
            {
                // Hintergrund noch nicht geladen
                if (!m_hintergrund.IstVerfügbar)
                    return false;

                // Alle Elemente auf dem Spielfeld
                return m_elemente.All( element => element.IstVerfügbar );
            }
        }

        /// <summary>
        /// Meldet oder setzt die bisher verstrichene Spielzeit.
        /// </summary>
        internal TimeSpan GespielteZeit
        {
            set
            {
                // An alle Elemente durchreichen - wir glauben einfach einmal, dass neue Werte auch tatsächlich Veränderungen bedeuten
                m_elemente.ForEach( element => element.GespielteZeit = value );
            }
        }

        /// <summary>
        /// Das Bild, das bei einer Niederlage angezeigt wird.
        /// </summary>
        internal IFuerBildAnzeige BildVerloren { get; private set; }

        /// <summary>
        /// Das Bild, das bei einem Sieg angezeigt wird.
        /// </summary>
        internal IFuerBildAnzeige BildGewonnen { get; private set; }

        /// <summary>
        /// Die Verwaltung der Spielergebnisse.
        /// </summary>
        internal Ergebnisse Ergebnisse { get; private set; }

        /// <summary>
        /// Erstellt eine neue Präsentation.
        /// </summary>
        /// <param name="hintergrundBild">Der relative Pfad zu dem Hintergrundbild.</param>
        /// <param name="verloren">Das Bild, das angezeigt wird, wenn das Spiel verloren wurde.</param>
        /// <param name="gewonnen">Das Bild, das angezeigt wird, wenn das Spiel gewonnen wurde.</param>
        /// <param name="figur">Optional die Spielfigur selbst.</param>
        /// <param name="elemente">Alle Elemente dieses Spielfelds - mit Ausnahme der Spielfigur selbst.</param>
        /// <param name="ergebnisse">Die Verwaltung der Spielergebnisse.</param>
        internal Spielfeld( string hintergrundBild, string verloren, string gewonnen, Element figur, IEnumerable<Element> elemente, Ergebnisse ergebnisse )
        {
            // Prüfen
            if (string.IsNullOrEmpty( hintergrundBild ))
                throw new ArgumentNullException( "hintergrundBild" );
            if (ergebnisse == null)
                throw new ArgumentNullException( "ergebnisse" );

            // Bilder setzen
            if (!string.IsNullOrEmpty( verloren ))
                BildVerloren = new EinzelBild( verloren );
            if (!string.IsNullOrEmpty( gewonnen ))
                BildGewonnen = new EinzelBild( gewonnen );

            // Auslesen
            m_hintergrund = new EinzelBild( hintergrundBild );
            Ergebnisse = ergebnisse;

            // Auf Änderungen reagieren
            m_hintergrund.PropertyChanged += ( s, a ) => ElementeAktualisieren();
            m_hintergrund.PropertyChanged += ( s, a ) => VerfügbarkeitNeuPrüfen( EinzelBild._IstVerfügbar, a.PropertyName );

            // Alle Elemente abbilden
            m_elemente = elemente.ToList();

            // Spielfigur ergänzen, sofern bekannt - immer als letztes, damit diese immer sichtbar bleibt
            m_elemente.Add( figur );

            // Alle Elemente überwachen
            m_elemente.ForEach( element => element.PropertyChanged += ( s, a ) => VerfügbarkeitNeuPrüfen( Element._IstVerfügbar, a.PropertyName ) );

            // Erstmalig aufsetzen
            ElementeAktualisieren();
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich etwas verändert hat.
        /// </summary>
        /// <param name="zielEigenschaft">Der Name der Verfügbarkeitseigenschaft.</param>
        /// <param name="geänderteEigenschaft">Der Name der tatsächlich verwendeten Eigenschaft.</param>
        private void VerfügbarkeitNeuPrüfen( string zielEigenschaft, string geänderteEigenschaft )
        {
            // Prüfung durchführen
            if (!zielEigenschaft.Equals( geänderteEigenschaft ))
                return;

            // Potentielle Änderung melden
            PropertyChanged.EigenschaftWurdeVerändert( this, _IstVerfügbar );
            PropertyChanged.EigenschaftWurdeVerändert( this, FuerSpielfeldAnzeige.Sichtbarkeit );
        }

        /// <summary>
        /// Teilt allen Elementen ein verändertes Bild mit.
        /// </summary>
        private void ElementeAktualisieren()
        {
            // Bild auslesen
            var bild = m_hintergrund;
            var breite = bild.Breite;
            if (double.IsNaN( breite ))
                return;
            var höhe = bild.Hoehe;
            if (double.IsNaN( höhe ))
                return;

            // Weiterleiten
            m_elemente.ForEach( element => element.SpielfeldWurdeVerändert( breite, höhe ) );
        }
    }
}
