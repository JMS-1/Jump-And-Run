using System;
using System.Linq;
using JMS.JnRV2.Ablage;
using JMS.JnRV2.Ablauf;


namespace JMS.JnRV2.Anzeige.Verbinder
{
    partial class SimulationsVerbinder
    {
        /// <summary>
        /// Die unterstützten Arten von Kollisionen.
        /// </summary>
        private static readonly Func<Kollisionsregel, GrundElement, bool>[] s_KollisionsArten =
        {
            (regel, element) => RegelAnwenden( regel as Punkteregel, element ),
            (regel, element) => RegelAnwenden( regel as Energieregel, element ),
            (regel, element) => RegelAnwenden( regel as Enderegel, element ),
            (regel, element) => RegelAnwenden( regel as Verschieberegel, element ),
            (regel, element) => RegelAnwenden( regel as Verschwinderegel, element ),
            (regel, element) => RegelAnwenden( regel as Erscheineregel, element ),
        };

        /// <summary>
        /// Erzeugt eine Kollisionsregel in der Simulation.
        /// </summary>
        /// <param name="element">Das betroffene Element auf dem Spielfeld.</param>
        /// <param name="artDerKollision">Die konfigurierte Art der Kollision.</param>
        /// <param name="aktion">Die zugehörige Reaktion.</param>
        private static void RegelAnmelden( this GrundElement element, KollisionsArten artDerKollision, KollisionsRegel.KollisionsAktion aktion )
        {
            // Regel anmelden - je nach Art des Elementes kann die Wirkung auf die Lebensenergie durch darauf springen egalisiert werden.
            var regeln = element.KollisionsRegel;
            switch (artDerKollision)
            {
                case KollisionsArten.VomSpielerSeitlichGetroffen: regeln.RegelAnmelden( KollisionsRegel.WennVomSpielerGetroffenAberNichtVonOben, aktion ); break;
                case KollisionsArten.GetroffenAberNichtVomSpieler: regeln.RegelAnmelden( KollisionsRegel.WennGetroffenAberNichtVomSpieler, aktion ); break;
                case KollisionsArten.SpielerIstGetroffen: regeln.RegelAnmelden( KollisionsRegel.WennAufSpielerGetroffen, aktion ); break;
                case KollisionsArten.VomSpielerGetroffen: regeln.RegelAnmelden( KollisionsRegel.WennVomSpielerGetroffen, aktion ); break;
                case KollisionsArten.Getroffen: regeln.RegelAnmelden( KollisionsRegel.WennGetroffen, aktion ); break;
                default: throw new NotImplementedException( artDerKollision.ToString() );
            }
        }

        /// <summary>
        /// Erstellt eine Regel aus der Konfiguration.
        /// </summary>
        /// <param name="regel">Eine beliebige Regel.</param>
        /// <param name="element">Das zugehörige Element.</param>
        /// <returns>Gesetzt, wenn die Regel ausgewertet werden konnte.</returns>
        private static bool RegelAnwenden( Energieregel regel, GrundElement element )
        {
            // Prüfen
            if (regel == null)
                return false;

            // Lebensenergie auslesen und Regel erzeugen
            var lebensenergie = regel.Lebensenergie;
            if (lebensenergie != 0)
                element.RegelAnmelden( regel.ArtDerKollision, KollisionsRegel.LebenskraftDesSpielersÄndern( lebensenergie ) );

            // Fertig
            return true;
        }

        /// <summary>
        /// Erstellt eine Regel aus der Konfiguration.
        /// </summary>
        /// <param name="regel">Eine beliebige Regel.</param>
        /// <param name="element">Das zugehörige Element.</param>
        /// <returns>Gesetzt, wenn die Regel ausgewertet werden konnte.</returns>
        private static bool RegelAnwenden( Verschieberegel regel, GrundElement element )
        {
            // Prüfen
            if (regel == null)
                return false;

            // Anwenden
            element.RegelAnmelden( regel.ArtDerKollision, KollisionsRegel.HindernisVerschieben() );

            // Fertig
            return true;
        }

        /// <summary>
        /// Erstellt eine Regel aus der Konfiguration.
        /// </summary>
        /// <param name="regel">Eine beliebige Regel.</param>
        /// <param name="element">Das zugehörige Element.</param>
        /// <returns>Gesetzt, wenn die Regel ausgewertet werden konnte.</returns>
        private static bool RegelAnwenden( Punkteregel regel, GrundElement element )
        {
            // Prüfen
            if (regel == null)
                return false;

            // Punkte verteilen
            var punkte = regel.Punkte;
            if (punkte != 0)
                element.RegelAnmelden( regel.ArtDerKollision, KollisionsRegel.PunkteAnDenSpielerÜbergeben( punkte ) );

            // Fertig
            return true;
        }

        /// <summary>
        /// Erzeugt eine Kollisionsregeln, die das Spiel beendet.
        /// </summary>
        /// <param name="regel">Die zu untersuchende Regel.</param>
        /// <param name="element">Das betroffene Element auf dem Spielfeld.</param>
        /// <returns>Gesetzt, wenn die Regel angewendet wurde.</returns>
        private static bool RegelAnwenden( Enderegel regel, GrundElement element )
        {
            // Prüfen
            if (regel == null)
                return false;

            // Erzeugen und anwenden
            element.RegelAnmelden( regel.ArtDerKollision, regel.Gewonnen ? KollisionsRegel.SpielGewonnen : KollisionsRegel.SpielVerloren );

            // Fertig
            return true;
        }

        /// <summary>
        /// Erzeugt eine Regel, die ein Element bei Kollision ausblendet.
        /// </summary>
        /// <param name="regel">Die zu untersuchende Regel.</param>
        /// <param name="element">Das betroffene Element auf dem Spielfeld.</param>
        /// <returns>Gesetzt, wenn die Regel angewendet wurde.</returns>
        private static bool RegelAnwenden( Verschwinderegel regel, GrundElement element )
        {
            // Prüfen
            if (regel == null)
                return false;

            // Erzeugen und anwenden
            switch (regel.WasSollVerschwinden)
            {
                case WasSollVerschwinden.Fest: element.RegelAnmelden( regel.ArtDerKollision, KollisionsRegel.ElementDeaktivieren ); break;
                case WasSollVerschwinden.Beweglich: element.RegelAnmelden( regel.ArtDerKollision, KollisionsRegel.BeweglichesElementDeaktivieren ); break;
                case WasSollVerschwinden.Selbst: element.RegelAnmelden( regel.ArtDerKollision, KollisionsRegel.BestimmtesElementDeaktivieren( element ) ); break;
                default: throw new NotSupportedException( regel.WasSollVerschwinden.ToString() );
            }

            // Fertig
            return true;
        }

        /// <summary>
        /// Erzeugt eine Regel, die ein Element bei Kollision aktiviert.
        /// </summary>
        /// <param name="regel">Die zu untersuchende Regel.</param>
        /// <param name="element">Das betroffene Element auf dem Spielfeld.</param>
        /// <returns>Gesetzt, wenn die Regel angewendet wurde.</returns>
        private static bool RegelAnwenden( Erscheineregel regel, GrundElement element )
        {
            // Prüfen
            if (regel == null)
                return false;

            // Erzeugen und anwenden
            element.RegelAnmelden( regel.ArtDerKollision, KollisionsRegel.ElementAktivieren( regel.Name ) );

            // Fertig
            return true;
        }

        /// <summary>
        /// Erstellt eine Regel aus der Konfiguration.
        /// </summary>
        /// <param name="regel">Eine beliebige Regel.</param>
        /// <param name="element">Das zugehörige Element.</param>
        public static void AnwendenAuf( this Kollisionsregel regel, GrundElement element )
        {
            // Prüfen
            if (regel == null)
                throw new NullReferenceException( "regel" );
            if (element == null)
                throw new ArgumentNullException( "element" );

            // Alle bekannten Regelarten prüfen
            if (!s_KollisionsArten.Any( art => art( regel, element ) ))
                throw new NotSupportedException( regel.GetType().Name );
        }
    }
}
