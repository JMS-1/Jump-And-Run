using System;
using System.Windows;
using JMS.JnRV2.Ablauf;


namespace JMS.JnRV2.Anzeige.Verbinder
{
    /// <summary>
    /// Über diese Schnittstelle werden die Verbinder erzeugt.
    /// </summary>
    /// <typeparam name="TArtDesElementes">Die Art des Elementes auf dem Spielfeld.</typeparam>
    internal abstract class ElementErzeuger<TArtDesElementes> : IVerbinderErzeuger where TArtDesElementes : Ablage.Element
    {
        /// <summary>
        /// Die zugehörige Basiskonfiguration aus der Ablage.
        /// </summary>
        protected TArtDesElementes Element { get; private set; }

        /// <summary>
        /// Erstellt einen neuen Erzeuger.
        /// </summary>
        /// <param name="element">Die Basiskonfiguration aus der Ablage.</param>
        protected ElementErzeuger( TArtDesElementes element )
        {
            // Merken
            Element = element;
        }

        /// <summary>
        /// Erlaubt es abgeleiteten Klassen, zusätzliche Kollisionsregeln vor die eigenen Regeln einzufügen.
        /// </summary>
        /// <param name="element">Das Element auf dem Spielfeld.</param>
        /// <param name="breite">Die gesamte Breite der Simulation.</param>
        /// <param name="höhe">Die gesamte Höhe der Simulation.</param>
        protected virtual void ZusätzlicheKollisionsregelnAnlegen( GrundElement element, decimal breite, decimal höhe )
        {
        }

        /// <summary>
        /// Erzeugt eine neue Verbindung.
        /// </summary>
        /// <param name="position">Die Position des Elementes in der Simulation.</param>
        /// <param name="ausdehnung">Die Ausdehung des Elementes in der Simulation.</param>
        /// <param name="breite">Die gesamte Breite der Simulation.</param>
        /// <param name="höhe">Die gesamte Höhe der Simulation.</param>
        /// <param name="umrechner">Methode zum Ermitteln der absoluten Koordinaten.</param>
        /// <param name="element">Das Element, auf das sich diese Verbindung bezieht.</param>
        /// <returns>Einen neuen Verbinder.</returns>
        public virtual GrundElement Erzeuge( Position position, Ausdehnung ausdehnung, decimal breite, decimal höhe, Func<Bereich, Rect> umrechner, PraesentationsModelle.Element element )
        {
            // Anlegen
            var fläche = new Element( position, ausdehnung, !element.IstSichtbar, element.Name );

            // Bei einer Kollision immer als erstes die Musik starten
            var melodie = Element.Melodie;
            if (!string.IsNullOrEmpty( melodie ))
                fläche.KollisionsRegel.RegelAnmelden( KollisionsRegel.WennVomSpielerGetroffen, ( fest, beweglich ) =>
                    {
                        // Melodie vermerken
                        ((PraesentationsModelle.Steuerung) element.Spiel.Steuerung).MelodieAbspielen( melodie );

                        // Weiter machen
                        return true;
                    } );

            // Erst einmal die Kollisionsregeln der abgeleiteten Klasse
            ZusätzlicheKollisionsregelnAnlegen( fläche, breite, höhe );

            // Nun die eigenen Regeln
            Element.Kollisionsregeln.ForEach( regel => regel.AnwendenAuf( fläche ) );

            // Auf Bewegung reagieren
            fläche.ElementHatSichBewegt += s =>
                {
                    // Absolute Position berechnen
                    var bereichAbsolut = umrechner( fläche.Bereich );

                    // Übertragen
                    element.PositionVerändern( bereichAbsolut.Left, bereichAbsolut.Top );
                };

            // Auf Zustände reagieren
            fläche.ZustandVerändert += s => element.IstSichtbar = !fläche.IstDeaktiviert;

            // Melden
            return fläche;
        }
    }

    /// <summary>
    /// Über diese Schnittstelle werden die Verbinder erzeugt.
    /// </summary>
    internal class ElementErzeuger : ElementErzeuger<Ablage.Element>
    {
        /// <summary>
        /// Erstellt einen neuen Erzeuger.
        /// </summary>
        /// <param name="element">Die Basiskonfiguration aus der Ablage.</param>
        public ElementErzeuger( Ablage.Element element )
            : base( element )
        {
        }
    }

    /// <summary>
    /// Über diese Schnittstelle werden die Verbinder erzeugt.
    /// </summary>
    internal class BildElementErzeuger : ElementErzeuger<Ablage.ElementMitBildSequenz>
    {
        /// <summary>
        /// Erstellt einen neuen Erzeuger.
        /// </summary>
        /// <param name="element">Die Basiskonfiguration aus der Ablage.</param>
        public BildElementErzeuger( Ablage.ElementMitBildSequenz element )
            : base( element )
        {
        }

        /// <summary>
        /// Erlaubt es abgeleiteten Klassen, zusätzliche Kollisionsregeln vor die eigenen Regeln einzufügen.
        /// </summary>
        /// <param name="element">Das Element auf dem Spielfeld.</param>
        /// <param name="breite">Die gesamte Breite der Simulation.</param>
        /// <param name="höhe">Die gesamte Höhe der Simulation.</param>
        protected override void ZusätzlicheKollisionsregelnAnlegen( GrundElement element, decimal breite, decimal höhe )
        {
            // Die eigenen Regeln anwenden
            Element.Animation.Aktivieren( element, breite, höhe );
        }

        /// <summary>
        /// Erzeugt eine neue Verbindung.
        /// </summary>
        /// <param name="position">Die Position des Elementes in der Simulation.</param>
        /// <param name="ausdehnung">Die Ausdehung des Elementes in der Simulation.</param>
        /// <param name="breite">Die gesamte Breite der Simulation.</param>
        /// <param name="höhe">Die gesamte Höhe der Simulation.</param>
        /// <param name="umrechner">Methode zum Ermitteln der absoluten Koordinaten.</param>
        /// <param name="element">Das Element, auf das sich diese Verbindung bezieht.</param>
        /// <returns>Einen neuen Verbinder.</returns>
        public override GrundElement Erzeuge( Position position, Ausdehnung ausdehnung, decimal breite, decimal höhe, Func<Bereich, Rect> umrechner, PraesentationsModelle.Element element )
        {
            // Erzeugen
            var fläche = (Ablauf.Element) base.Erzeuge( position, ausdehnung, breite, höhe, umrechner, element );

            // Abschliessend konfigurieren
            fläche.Fällt = Element.Faellt;

            // Melden
            return fläche;
        }
    }
}
