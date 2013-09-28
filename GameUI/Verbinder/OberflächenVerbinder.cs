using System;
using System.Linq;
using JMS.JnRV2.Anzeige.PraesentationsModelle;


namespace JMS.JnRV2.Anzeige.Verbinder
{
    /// <summary>
    /// Einige Hilfsmethoden, mit denen aus persistierten Strukturen Datenmodelle zur Anzeige in der
    /// Oberfläche erstellt werden.
    /// </summary>
    public static class OberflächenVerbinder
    {
        /// <summary>
        /// Erstellt das Modell einer Spielfigur.
        /// </summary>
        /// <param name="figur">Eine Spielfigur.</param>
        /// <returns>Das zugehörige Modell.</returns>
        /// <exception cref="ArgumentNullException">Es wurde keine Spielfigur angegeben.</exception>
        public static Figur ErzeugePräsentation( this Ablage.Spielfigur figur )
        {
            // Prüfen
            if (figur == null)
                throw new ArgumentNullException( "figur" );

            // Anlegen
            return
                new Figur
                (
                    figur.Name,
                    figur.BilderProSekunde,
                    figur.BilderRuhend.ErzeugePräsentation(),
                    figur.BilderNachRechts.ErzeugePräsentation(),
                    figur.BilderNachLinks.ErzeugePräsentation(),
                    figur.BilderImSprung.ErzeugePräsentation()
                );
        }

        /// <summary>
        /// Erstellt zu einer Liste von Bildern die zugehörige Sequenz.
        /// </summary>
        /// <param name="bilder">Eine Liste von Bildern.</param>
        /// <returns>Die angeforderte Sequenz.</returns>
        /// <exception cref="ArgumentNullException">Es wurde keine Liste angegeben.</exception>
        public static BildFolge ErzeugePräsentation( this Ablage.BildSequenz bilder )
        {
            // Prüfen
            if (bilder == null)
                throw new ArgumentNullException( "bilder" );

            // Erzeugen
            return
                new BildFolge
                (
                    bilder.Select( bild => bild.Quelle ).ToArray()
                );
        }

        /// <summary>
        /// Erstellt die Präsentation eines Spielfeldes.
        /// </summary>
        /// <param name="spielfeld">Die Konfiguration des Spielfelds.</param>
        /// <param name="spielfigur">Optional eine Spielfigur, die auf dem Spielfeld platziert werden soll.</param>
        /// <returns>Die zugehörige Repräsentation des Spielfelds.</returns>
        /// <exception cref="ArgumentNullException">Es wurde kein Spielfeld angegeben.</exception>
        public static Spielfeld ErzeugePräsentation( this Ablage.Spielfeld spielfeld, Element spielfigur )
        {
            // Prüfen
            if (spielfeld == null)
                throw new ArgumentNullException( "spielfeld" );

            // Erzeugen
            return
                new Spielfeld
                (
                    spielfeld.Bild,
                    spielfeld.BildVerloren,
                    spielfeld.BildGewonnen,
                    spielfigur,
                    spielfeld.Elemente.Select( ErzeugePräsentation ),
                    new Ergebnisse( spielfeld.Ergebnisse )
                );
        }

        /// <summary>
        /// Erstellt die Repräsentation eines Spiels.
        /// </summary>
        /// <param name="spielKonfiguration">Die Konfiguration des Spiels.</param>
        /// <param name="spielfigur">Eine bereits vorbereitete Spielfigur.</param>
        /// <returns>Die angeforderte Repräsentation.</returns>
        public static Spiel ErzeugePräsentation( this Ablage.Spiel spielKonfiguration, Ablage.Spielfigur spielfigur )
        {
            // Prüfen
            if (spielKonfiguration == null)
                return null;
            if (spielfigur == null)
                throw new ArgumentNullException( "spielfigur" );

            // Das Spielfeld ist fest mit dem Spiel verbunden
            var spielfeld = spielKonfiguration.Spielfeld;

            // Präsentation der Spielfigur auf dem Spielfeld
            var figur =
                new FigurElement<IVerbinderErzeuger>
                (
                    spielfeld.InitialePosition.X,
                    spielfeld.InitialePosition.Y,
                    spielfigur.ErzeugePräsentation(),
                    () => new SpielerErzeuger<IVerbinderErzeuger>( spielfigur, spielfeld )
                );

            // Erzeugen            
            var spiel =
                Spiel.Erzeugen
                (
                    spielKonfiguration.SichtfensterBreite,
                    spielKonfiguration.SichtfensterHoehe,
                    spielfeld.ErzeugePräsentation( figur ),
                    figur,
                    SimulationsVerbinder.SpielumgebungAufsetzen
                );

            // Melden
            return spiel;
        }

        /// <summary>
        /// Erstellt eine neue Präsentation für ein Element.
        /// </summary>
        /// <param name="element">Das zu verwendende Element.</param>
        /// <returns>Eine neue Präsentation.</returns>
        public static Element ErzeugePräsentation( Ablage.Element element )
        {
            // Prüfen
            if (element == null)
                throw new ArgumentNullException( "element" );

            // Element mit Bild
            var elementMitBild = element as Ablage.ElementMitBildSequenz;
            if (elementMitBild != null)
                return
                    new BildElement<IVerbinderErzeuger>
                    (
                        elementMitBild.Name,
                        !elementMitBild.AnfänglichUnsichtbar,
                        elementMitBild.HorizontalePosition,
                        elementMitBild.VertikalePosition,
                        elementMitBild.Bilder.ErzeugePräsentation(),
                        elementMitBild.BilderProSekunde,
                        elementMitBild.Ebene,
                        () => new BildElementErzeuger( elementMitBild )
                    );

            // Anlegen
            return
                new Element<IVerbinderErzeuger>
                (
                    element.Name,
                    !element.AnfänglichUnsichtbar,
                    element.HorizontalePosition,
                    element.VertikalePosition,
                    element.Breite,
                    element.Hoehe,
                    -1,
                    () => new ElementErzeuger( element )
                );
        }

    }
}
