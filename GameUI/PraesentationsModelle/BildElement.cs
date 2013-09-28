using System;
using System.ComponentModel;
using System.Windows.Media;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Beschreibt ein einzelnes Element mit Bild auf dem Spielfeld.
    /// </summary>
    /// <typeparam name="TArtDesSpielElementes">Die Art des zugehörigen Elementes der Simulation.</typeparam>
    internal class BildElement<TArtDesSpielElementes> : BildElementBasis<BildFolge, TArtDesSpielElementes>
    {
        /// <summary>
        /// Das aktuell zu verwendende Bild.
        /// </summary>
        private int m_aktuellerIndex;

        /// <summary>
        /// Ein mit dem Element verbundenes Bild.
        /// </summary>
        protected override ImageSource AktuellesBild { get { return Quelle[m_aktuellerIndex % Quelle.AnzahlDerBilder]; } }

        /// <summary>
        /// Legt fest, wie schnell die Bildsequenz abgespielt werden soll.
        /// </summary>
        private readonly int m_bilderProSekunde;

        /// <summary>
        /// Erstellt eine neue Präsentation für ein Element.
        /// </summary>
        /// <param name="name">Der Name des Elementes.</param>
        /// <param name="initialSichtbar">Gesetzt, wenn das Element beim Starten sichtbar sein soll.</param>
        /// <param name="horizontalePosition">Die initiale horizontale Position des Elementes.</param>
        /// <param name="vertikalePosition">Die initiale vertikale Position des Elementes.</param>
        /// <param name="bilder">Die Bilder zu diesem Element.</param>
        /// <param name="bilderProSekunde">Die Anzahl der Bildwechsel pro Sekunde.</param>
        /// <param name="ebene">Die Ebene, auf der das Element angezeigt werden soll.</param>
        /// <param name="initialisierung">Eine Methode zur abschliessenden Initialisierung sobald die Simulation bereit ist.</param>
        public BildElement( string name, bool initialSichtbar, double horizontalePosition, double vertikalePosition, BildFolge bilder, int bilderProSekunde, int ebene, Func<TArtDesSpielElementes> initialisierung )
            : base( name, initialSichtbar, horizontalePosition, vertikalePosition, bilder, ebene, initialisierung )
        {
            // Prüfen
            if (bilderProSekunde < 0)
                throw new ArgumentNullException( "bilderProSekunde" );

            // Merken
            m_bilderProSekunde = bilderProSekunde;
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich die Spielzeit verändert hat.
        /// </summary>
        /// <param name="interessenten">Alle, die an Veränderungen interessiert sind.</param>
        protected override void SpielzeitWurdeVerändert( PropertyChangedEventHandler interessenten )
        {
            // Weitersetzen
            if (m_bilderProSekunde > 0)
                interessenten.EigenschaftVerändern( this, FuerBildAnzeige.Bild, ref m_aktuellerIndex, (int) Math.Round( GespielteZeit.TotalSeconds * m_bilderProSekunde ) );
        }
    }
}
