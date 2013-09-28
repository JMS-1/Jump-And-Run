using System;
using System.Collections.Generic;
using System.Linq;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Beschreibt, wie ein Element auf Kollisionen reagieren soll.
    /// </summary>
    public class KollisionsRegel : ElementRegel
    {
        /// <summary>
        /// Signatur einer Methode zur Prüfung einer Bedingung.
        /// </summary>
        /// <param name="getroffenesElement">Das Element, das von einem anderen getroffen wurde.</param>
        /// <param name="elementInBewegung">Das Element, dass durch seine Bewegung mit einem anderen kollidiert ist.</param>
        /// <returns>Gesetzt, wenn die Kollisionen eine Bearbeitung erfolgt.</returns>
        public delegate bool BedingungsRegel( GrundElement getroffenesElement, GrundElement elementInBewegung );

        /// <summary>
        /// Signatur eine Methode, die bei einer Kollision auszuführen ist.
        /// </summary>
        /// <param name="getroffenesElement">Das Element, das von einem anderen getroffen wurde.</param>
        /// <param name="elementInBewegung">Das Element, dass durch seine Bewegung mit einem anderen kollidiert ist.</param>
        /// <returns>Gesetzt, wenn weitere Aktionen ausgewertet werden sollen. Ansonsten werden nicht nur die Folgeregeln
        /// ignoriert, sondern auch die aktuelle Bewegung abgebrochen.</returns>
        public delegate bool KollisionsAktion( GrundElement getroffenesElement, GrundElement elementInBewegung );

        /// <summary>
        /// Prüft, ob die sich bewegende Spielfigur gegen ein Hindernis getroffen ist.
        /// </summary>
        public static readonly BedingungsRegel WennVomSpielerGetroffen = ( fest, beweglich ) => beweglich is Spieler;

        /// <summary>
        /// Prüft, ob irgendetwas gegen ein Hindernis trifft.
        /// </summary>
        public static readonly BedingungsRegel WennGetroffen = ( fest, beweglich ) => true;

        /// <summary>
        /// Prüft, ob ein sich bewegendes Element gegen ein Hindernis getroffen ist.
        /// </summary>
        public static readonly BedingungsRegel WennGetroffenAberNichtVomSpieler = ( fest, beweglich ) => !(beweglich is Spieler);

        /// <summary>
        /// Prüft, ob ein Hindernis gegen den Spiel triff.
        /// </summary>
        public static readonly BedingungsRegel WennAufSpielerGetroffen = ( fest, beweglich ) => fest is Spieler;

        /// <summary>
        /// Prüft, ob die sich bewegende Spielfigur gegen ein Hindernis getroffen ist und dabei nicht darauf gesprungen ist.
        /// </summary>
        public static readonly BedingungsRegel WennVomSpielerGetroffenAberNichtVonOben =
            ( fest, beweglich ) =>
            {
                // Ist es überhaupt die Spielfigur
                var spieler = beweglich as Spieler;
                if (spieler == null)
                    return false;

                // Zählt nur, wenn wir nicht plump dagegen laufen
                var bewegung = spieler.Bewegung;
                if ((bewegung & ElementBewegung.FälltNachUnten) == ElementBewegung.FälltNachUnten)
                    return false;
                else if ((bewegung & ElementBewegung.SpringtNachOben) == ElementBewegung.SpringtNachOben)
                    return false;
                else
                    return true;
            };

        /// <summary>
        /// Deaktiviert das getroffene Element und bricht dann die weitere Bearbeitung ab.
        /// </summary>
        public static readonly KollisionsAktion ElementDeaktivieren = ( fest, beweglich ) => { fest.IstDeaktiviert = true; return false; };

        /// <summary>
        /// Deaktiviert das getroffene Element und bricht dann die weitere Bearbeitung ab.
        /// </summary>
        public static readonly KollisionsAktion BeweglichesElementDeaktivieren = ( fest, beweglich ) => { beweglich.IstDeaktiviert = true; return false; };

        /// <summary>
        /// Wird aufgerufen, wenn das Spiel gewonnen wurde.
        /// </summary>
        public static readonly KollisionsAktion SpielGewonnen = ( fest, beweglich ) => { fest.Spielfeld.UnterbrechenOderBeenden( SimulationsStand.Gewonnen ); return false; };

        /// <summary>
        /// Wird aufgerufen, wenn das Spiel verloren wurde.
        /// </summary>
        public static readonly KollisionsAktion SpielVerloren = ( fest, beweglich ) => { fest.Spielfeld.UnterbrechenOderBeenden( SimulationsStand.Verloren ); return false; };

        /// <summary>
        /// Meldet eine Methode, die bei einer Kollision einmalig einen Punkteübertrag vornimmt.
        /// </summary>
        /// <param name="punkteDifferenz">Die gewünschte Differenz der Punkte.</param>
        /// <returns>Die angeforderte Methode.</returns>
        public static KollisionsAktion PunkteAnDenSpielerÜbergeben( int punkteDifferenz )
        {
            // Methode anlegen
            return ( fest, beweglich ) =>
                {
                    // Nichts mehr zu tun
                    if (punkteDifferenz == 0)
                        return true;

                    // Übergabe vornehmen
                    var spieler = (beweglich as Spieler) ?? (fest as Spieler);
                    if (spieler != null)
                        spieler.PunkteSammeln( punkteDifferenz );

                    // Nur einmal
                    punkteDifferenz = 0;

                    // Weitere Aktionen auswerten
                    return true;
                };
        }

        /// <summary>
        /// Deaktiviert ein Element.
        /// </summary>
        /// <param name="element">Das gewünschte Element.</param>
        /// <returns>Die Aktion, die das Element verschwinden läßt.</returns>
        public static KollisionsAktion BestimmtesElementDeaktivieren( GrundElement element )
        {
            // Methode anlegen
            return ( fest, beweglich ) => { element.IstDeaktiviert = true; return true; };
        }

        /// <summary>
        /// Aktiviert ein anderes Element.
        /// </summary>
        /// <param name="elementName">Der Name des zu aktivierenden Elementes.</param>
        public static KollisionsAktion ElementAktivieren( string elementName )
        {
            // Methode anlegen
            return ( fest, beweglich ) =>
            {
                // Suchen wir unser Element einmal
                var zielElement = fest.Spielfeld.Elemente.FirstOrDefault( test => string.Equals( test.Name, elementName ) );
                if (zielElement != null)
                    zielElement.IstDeaktiviert = false;

                // Weiter machen
                return true;
            };
        }

        /// <summary>
        /// Meldet eine Methode, die bei einer Kollision einmalig eine Änderung der Lebensenergie vornimmt.
        /// </summary>
        /// <param name="energieDifferenz">Die gewünschte Differenz der Energie.</param>
        /// <returns>Die angeforderte Methode.</returns>
        public static KollisionsAktion LebenskraftDesSpielersÄndern( int energieDifferenz )
        {
            // Methode anlegen
            return ( fest, beweglich ) =>
            {
                // Nichts mehr zu tun
                if (energieDifferenz == 0)
                    return true;

                // Übergabe vornehmen
                var spieler = (beweglich as Spieler) ?? (fest as Spieler);
                if (spieler != null)
                    spieler.LebenskraftÄndern( energieDifferenz );

                // Nur einmal
                energieDifferenz = 0;

                // Weitere Aktionen auswerten
                return true;
            };
        }

        /// <summary>
        /// Verschiebt ein getroffenes Element.
        /// </summary>
        /// <returns>Die gewünschte Regel.</returns>
        public static KollisionsAktion HindernisVerschieben()
        {
            // Die letzte aktive Regel
            var geschwindigkeit = default( TemporaereGeschwindigkeit );

            // Aktion erstellen
            return ( fest, beweglich ) =>
                {
                    // Nicht, wenn die Regel schon aktiv ist
                    if (geschwindigkeit != null)
                        return true;

                    // Geschwindigkeit auslesen
                    var jetzt = beweglich.Spielfeld.VerbrauchteZeit;
                    var impuls = beweglich.GeschwindigkeitsRegel.AktuelleGeschwindigkeitBerechnen( jetzt );

                    // Übertragen
                    geschwindigkeit = TemporaereGeschwindigkeit.Erzeugen( impuls.HorizontaleGeschwindigkeit, GenaueZahl.Null, jetzt + TimeSpan.FromMilliseconds( 20 ) );

                    // Auch wieder abmelden
                    geschwindigkeit.Deaktiviert += g => geschwindigkeit = null;

                    // Anwenden
                    fest.GeschwindigkeitsRegel.GeschwindigkeitErgänzen( geschwindigkeit );

                    // Weitere Regeln anwenden
                    return true;
                };
        }

        /// <summary>
        /// Beschreibt eine einzelne Reaktion auf eine Kollision.
        /// </summary>
        private struct WennDann
        {
            /// <summary>
            /// Die Bedingung, die zur Ausführung der Reaktion erfüllt sein muss.
            /// </summary>
            public BedingungsRegel Bedingung;

            /// <summary>
            /// Die Reaktion auf dem Element.
            /// </summary>
            public KollisionsAktion Reaktion;
        }

        /// <summary>
        /// Alle Regeln, die angewendet werden sollen.
        /// </summary>
        private readonly List<WennDann> m_regeln = new List<WennDann>();

        /// <summary>
        /// Das zugehörige Element.
        /// </summary>
        private readonly GrundElement m_element;

        /// <summary>
        /// Erstellt ein neues Regelwerk.
        /// </summary>
        /// <param name="zugehörigesElement">Das zugehörige Element.</param>
        internal KollisionsRegel( GrundElement zugehörigesElement )
        {
            // Merken
            m_element = zugehörigesElement;
        }

        /// <summary>
        /// Führt alle Aktionen zu einer Kollision aus.
        /// </summary>
        /// <param name="elementInBewegung">Das Element, dessen Bewegung für die Kollision verantwortlich ist.</param>
        /// <returns>Gesetzt, wenn weitere Aktionen und Bewegungen ausgeführt werden dürfen.</returns>
        internal bool KollisionAuswerten( GrundElement elementInBewegung )
        {
            // Prüfen
            if (elementInBewegung == null)
                throw new ArgumentNullException( "elementInBewegung" );

            // Regeln des anderen Elementes mit berücksichtigen
            var weitereRegeln = elementInBewegung.KollisionsRegel;

            // Alle Regeln
            WennDann[] regeln;
            lock (m_regeln)
                lock (weitereRegeln.m_regeln)
                    regeln = m_regeln.Concat( weitereRegeln.m_regeln ).ToArray();

            // Auswerten
            for (int i = 0; i < regeln.Length; i++)
                if (regeln[i].Bedingung( m_element, elementInBewegung ))
                    if (!regeln[i].Reaktion( m_element, elementInBewegung ))
                        return false;

            // Weiter machen
            return true;
        }

        /// <summary>
        /// Beschreibt das Verhalten bei einer Kollision.
        /// </summary>
        /// <param name="bedingung">Die Bedingung, die erfüllt sein muss, damit irgendetwas passiert.</param>
        /// <param name="reaktion">Die Aktion, die mit Erfüllen der Bedingung ausgeführt werden soll.</param>
        public void RegelAnmelden( BedingungsRegel bedingung, KollisionsAktion reaktion )
        {
            // Prüfen
            if (bedingung == null)
                throw new ArgumentNullException( "bedingung" );
            if (reaktion == null)
                throw new ArgumentNullException( "reaktion" );

            // Sicher merken
            lock (m_regeln)
                m_regeln.Add( new WennDann { Bedingung = bedingung, Reaktion = reaktion } );
        }
    }
}
