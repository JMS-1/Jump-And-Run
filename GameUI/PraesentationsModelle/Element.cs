using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Beschreibt ein einzelnes Element auf dem Spielfeld.
    /// </summary>
    public abstract class Element : IFuerElementAnzeige
    {
        #region Schnittstelle IFuerElementAnzeige

        /// <summary>
        /// Meldet die Breite des Elementes.
        /// </summary>
        public double Breite
        {
            get
            {
                // Melden
                return m_breite;
            }
            protected set
            {
                // Ändern
                if (PropertyChanged.EigenschaftVerändern( this, FuerBildAnzeige.Breite, ref m_breite, value ))
                    EigenschaftWurdeVerändert( FuerElementAnzeige.HorizontalePosition );
            }
        }

        /// <summary>
        /// Meldet die Höhe des Elementes.
        /// </summary>
        public double Hoehe
        {
            get
            {
                // Melden
                return m_hoehe;
            }
            protected set
            {
                // Ändern
                if (PropertyChanged.EigenschaftVerändern( this, FuerBildAnzeige.Höhe, ref m_hoehe, value ))
                    EigenschaftWurdeVerändert( FuerElementAnzeige.VertikalePosition );
            }
        }

        /// <summary>
        /// Ein mit dem Element verbundenes Bild.
        /// </summary>
        public ImageSource Bild { get { return AktuellesBild; } }

        /// <summary>
        /// Meldet die Sichtbarkeit des Elementes.
        /// </summary>
        public Visibility Sichtbarkeit { get { return m_sichtbar ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// Meldet die horizontale Position des Elementes.
        /// </summary>
        public double HorizontalePosition { get { return EchteHorizontalePosition; } }

        /// <summary>
        /// Meldet die vertikale Position des Elementes.
        /// </summary>
        public double VertikalePosition
        {
            get
            {
                // Schauen wir mal, ob wir alles haben
                var spielfeldHöhe = m_spielfeldHöhe;
                if (double.IsNaN( spielfeldHöhe ))
                    return 0;
                var eigeneHöhe = Hoehe;
                if (double.IsNaN( eigeneHöhe ))
                    return 0;

                // Umrechnen in das Silverlight Koordinatensystem, bei dem die vertikale Zählung oben beginnt
                return spielfeldHöhe - (EchteVertikalePosition + eigeneHöhe);
            }
        }

        /// <summary>
        /// Legt die Ebene fest, in der das Element angezeigt wird.
        /// </summary>
        public int Ebene { get; private set; }

        /// <summary>
        /// Wird ausgelöst, wenn sich irgend etwas verändert hat.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Der Name der Eigenschaft mit der echten horizontalen Position des Elementes.
        /// </summary>
        internal static readonly string _EchteHorizontalePosition = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( Element i ) => i.EchteHorizontalePosition );

        /// <summary>
        /// Der Name der Eigenschaft mit der echten vertikalen Position des Elementes.
        /// </summary>
        internal static readonly string _EchteVertikalePosition = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( Element i ) => i.EchteVertikalePosition );

        /// <summary>
        /// Der Name der Eigenschaft zur Verfügbarkeit des zugehörigen Bildes.
        /// </summary>
        internal static readonly string _IstVerfügbar = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( Element i ) => i.IstVerfügbar );

        /// <summary>
        /// Das zugehörige Spiel.
        /// </summary>
        public Spiel Spiel { get; internal set; }

        /// <summary>
        /// Die Breite des Elementes.
        /// </summary>
        private double m_breite = double.NaN;

        /// <summary>
        /// Die Höhe des Elementes.
        /// </summary>
        private double m_hoehe = double.NaN;

        /// <summary>
        /// Der Name des Elementes.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// Gesetzt, wenn das Element sichtbar ist. In dieser ersten Version sind alle Elemente initial sichtbar.
        /// </summary>
        private bool m_sichtbar;

        /// <summary>
        /// Meldet oder legt fest, ob das Element sichtbar ist.
        /// </summary>
        internal bool IstSichtbar
        {
            get { return m_sichtbar; }
            set
            {
                // Nichts verändert
                if (value == m_sichtbar)
                    return;

                // Merken
                m_sichtbar = value;

                // Melden
                PropertyChanged.EigenschaftWurdeVerändert( this, FuerElementAnzeige.Sichtbarkeit );
            }
        }

        /// <summary>
        /// Verändert die Position des Elementes.
        /// </summary>
        /// <param name="horizontalePosition">Die neue horizontale Position.</param>
        /// <param name="vertikalePosition">Die neue vertikale Position.</param>
        internal void PositionVerändern( double horizontalePosition, double vertikalePosition )
        {
            // Durchreichen
            EchteHorizontalePosition = horizontalePosition;
            EchteVertikalePosition = vertikalePosition;
        }

        /// <summary>
        /// Meldet, dass eine Eigenschaft verändert wurde.
        /// </summary>
        /// <param name="nameDerEigenschaft">Der Name der Eigenschaft.</param>
        protected void EigenschaftWurdeVerändert( string nameDerEigenschaft )
        {
            // Weiterleiten
            PropertyChanged.EigenschaftWurdeVerändert( this, nameDerEigenschaft );
        }

        /// <summary>
        /// Die horizontale Position des Elementes.
        /// </summary>
        private double m_echteHorizontalePosition;

        /// <summary>
        /// Die horizontale Position des Elementes.
        /// </summary>
        public double EchteHorizontalePosition
        {
            get
            {
                // Melden
                return m_echteHorizontalePosition;
            }
            set
            {
                // Ändern
                if (PropertyChanged.EigenschaftVerändern( this, _EchteHorizontalePosition, ref m_echteHorizontalePosition, value ))
                    PropertyChanged.EigenschaftWurdeVerändert( this, FuerElementAnzeige.HorizontalePosition );
            }
        }

        /// <summary>
        /// Die vertikale Position des Elementes.
        /// </summary>
        private double m_echteVertikalePosition;

        /// <summary>
        /// Die vertikale Position des Elementes.
        /// </summary>
        public double EchteVertikalePosition
        {
            get
            {
                // Melden
                return m_echteVertikalePosition;
            }
            set
            {
                // Ändern
                if (PropertyChanged.EigenschaftVerändern( this, _EchteVertikalePosition, ref m_echteVertikalePosition, value ))
                    PropertyChanged.EigenschaftWurdeVerändert( this, FuerElementAnzeige.VertikalePosition );
            }
        }

        /// <summary>
        /// Die Breite des Spielfelds.
        /// </summary>
        private double m_spielfeldBreite = double.NaN;

        /// <summary>
        /// Die Höhe des Spielfelds.
        /// </summary>
        private double m_spielfeldHöhe = double.NaN;

        /// <summary>
        /// Ein mit dem Element verbundenes Bild.
        /// </summary>
        protected virtual ImageSource AktuellesBild { get { return null; } }

        /// <summary>
        /// Die aktuelle Spielzeit.
        /// </summary>
        private TimeSpan m_spielZeit;

        /// <summary>
        /// Meldet oder setzt die bisher verstrichene Spielzeit.
        /// </summary>
        internal TimeSpan GespielteZeit
        {
            get
            {
                // Melden
                return m_spielZeit;
            }
            set
            {
                // Blind übernehmen
                m_spielZeit = value;

                // Melden
                SpielzeitWurdeVerändert( PropertyChanged );
            }
        }

        /// <summary>
        /// Gesetzt, sobald das zugehörige Bild geladen wurde.
        /// </summary>
        private bool m_verfügbar = false;

        /// <summary>
        /// Meldet, ob das zugehörige Bild geladen wurden.
        /// </summary>
        internal bool IstVerfügbar { get { return m_verfügbar; } }

        /// <summary>
        /// Wird aufgerufen, wenn sich die Spielzeit verändert hat.
        /// </summary>
        /// <param name="interessenten">Alle, die an Veränderungen interessiert sind.</param>
        protected virtual void SpielzeitWurdeVerändert( PropertyChangedEventHandler interessenten )
        {
        }

        /// <summary>
        /// Erstellt eine neue Präsentation für ein Element.
        /// </summary>
        /// <param name="name">Der Name des Elementes.</param>
        /// <param name="initialSichtbar">Gesetzt, wenn das Element beim Starten sichtbar sein soll.</param>
        /// <param name="horizontalePosition">Die initiale horizontale Position des Elementes.</param>
        /// <param name="vertikalePosition">Die initiale vertikale Position des Elementes.</param>
        /// <param name="ebene">Die Ebene, auf der das Element angezeigt werden soll.</param>
        internal protected Element( string name, bool initialSichtbar, double horizontalePosition, double vertikalePosition, int ebene )
        {
            // Übernehmen
            m_echteHorizontalePosition = horizontalePosition;
            m_echteVertikalePosition = vertikalePosition;
            m_sichtbar = initialSichtbar;
            Ebene = ebene;
            Name = name;
        }

        /// <summary>
        /// Erstellt eine neue Präsentation für ein Element.
        /// </summary>
        /// <param name="name">Der Name des Elementes.</param>
        /// <param name="initialSichtbar">Gesetzt, wenn das Element beim Starten sichtbar sein soll.</param>
        /// <param name="horizontalePosition">Die initiale horizontale Position des Elementes.</param>
        /// <param name="vertikalePosition">Die initiale vertikale Position des Elementes.</param>
        /// <param name="breite">Die anfängliche Breite des Elementes.</param>
        /// <param name="höhe">Die anfängliche Höhe des Elementes.</param>
        /// <param name="ebene">Die Ebene, auf der das Element angezeigt werden soll.</param>
        internal Element( string name, bool initialSichtbar, double horizontalePosition, double vertikalePosition, double breite, double höhe, int ebene )
            : this( name, initialSichtbar, horizontalePosition, vertikalePosition, ebene )
        {
            // Übernehmen
            m_verfügbar = true;
            Breite = breite;
            Hoehe = höhe;
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich das Bild verändert hat.
        /// </summary>
        /// <param name="neueBreite">Die neue Breite.</param>
        /// <param name="neueHöhe">Die heue Höhe.</param>
        internal void SpielfeldWurdeVerändert( double neueBreite, double neueHöhe )
        {
            // Durchreichen
            PropertyChanged.EigenschaftVerändern( this, FuerElementAnzeige.HorizontalePosition, ref m_spielfeldBreite, neueBreite );
            PropertyChanged.EigenschaftVerändern( this, FuerElementAnzeige.VertikalePosition, ref m_spielfeldHöhe, neueHöhe );
        }

        /// <summary>
        /// Wird aufgerufen, sobald dieses Element zum ersten mal sichtbar wird.
        /// </summary>
        protected virtual void ErstmaligVerfügbar()
        {
        }

        /// <summary>
        /// Sieht nach, ob dieses Element nun vollständig verfügbar ist.
        /// </summary>
        protected void VerfügbarkeitSetzen()
        {
            // Das machen wir nur einmal
            if (m_verfügbar)
                return;

            // Dazu müssen wir unsere Dimensionen kennen
            m_verfügbar = !double.IsNaN( Breite ) && !double.IsNaN( Hoehe );

            // Immer noch nicht
            if (!m_verfügbar)
                return;

            // Wird sind erstmalig sichtbar geworden
            ErstmaligVerfügbar();

            // Melden
            PropertyChanged.EigenschaftWurdeVerändert( this, _IstVerfügbar );
        }
    }

    /// <summary>
    /// Beschreibt ein einzelnes Element auf dem Spielfeld.
    /// </summary>
    /// <typeparam name="TArtDesSpielElementes">Die Art des zugehörigen Elementes der Simulation.</typeparam>
    internal class Element<TArtDesSpielElementes> : Element
    {
        /// <summary>
        /// Wird verwendet, um die Konfiguration abzuschliessen, sobald die Simulation angelegt ist.
        /// </summary>
        internal readonly Func<TArtDesSpielElementes> InitialisiereVerbinder;

        /// <summary>
        /// Erstellt eine neue Präsentation für ein Element.
        /// </summary>
        /// <param name="name">Der Name des Elementes.</param>
        /// <param name="initialSichtbar">Gesetzt, wenn das Element beim Starten sichtbar sein soll.</param>
        /// <param name="horizontalePosition">Die initiale horizontale Position des Elementes.</param>
        /// <param name="vertikalePosition">Die initiale vertikale Position des Elementes.</param>
        /// <param name="ebene">Die Ebene, auf der das Element angezeigt werden soll.</param>
        /// <param name="initialisierung">Eine Methode zur abschliessenden Initialisierung sobald die Simulation bereit ist.</param>
        protected Element( string name, bool initialSichtbar, double horizontalePosition, double vertikalePosition, int ebene, Func<TArtDesSpielElementes> initialisierung )
            : base( name, initialSichtbar, horizontalePosition, vertikalePosition, ebene )
        {
            // Prüfen
            if (initialisierung == null)
                throw new ArgumentNullException( "initialisierung" );

            // Übernehmen
            InitialisiereVerbinder = initialisierung;
        }

        /// <summary>
        /// Erstellt eine neue Präsentation für ein Element.
        /// </summary>
        /// <param name="name">Der Name des Elementes.</param>
        /// <param name="initialSichtbar">Gesetzt, wenn das Element beim Starten sichtbar sein soll.</param>
        /// <param name="horizontalePosition">Die initiale horizontale Position des Elementes.</param>
        /// <param name="vertikalePosition">Die initiale vertikale Position des Elementes.</param>
        /// <param name="breite">Die anfängliche Breite des Elementes.</param>
        /// <param name="höhe">Die anfängliche Höhe des Elementes.</param>
        /// <param name="ebene">Die Ebene, auf der das Element angezeigt werden soll.</param>
        /// <param name="initialisierung">Eine Methode zur abschliessenden Initialisierung sobald die Simulation bereit ist.</param>
        public Element( string name, bool initialSichtbar, double horizontalePosition, double vertikalePosition, double breite, double höhe, int ebene, Func<TArtDesSpielElementes> initialisierung )
            : this( name, initialSichtbar, horizontalePosition, vertikalePosition, ebene, initialisierung )
        {
            // Übernehmen
            Breite = breite;
            Hoehe = höhe;

            // Fertig stellen
            VerfügbarkeitSetzen();
        }
    }
}
