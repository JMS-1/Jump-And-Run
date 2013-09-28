using System;
using System.ComponentModel;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Anzeige.PraesentationsModelle
{
    /// <summary>
    /// Beschreibt die Position eines Bildes auf dem Spielfeld.
    /// </summary>
    /// <typeparam name="TArtDesBildes">Die konkrete Art der Implementierung der Bildquelle.</typeparam>
    /// <typeparam name="TArtDesSpielElementes">Die Art des zugehörigen Elementes der Simulation.</typeparam>
    internal abstract class BildElementBasis<TArtDesBildes, TArtDesSpielElementes> : Element<TArtDesSpielElementes> where TArtDesBildes : IFuerBildAnzeige
    {
        /// <summary>
        /// Alle Bilder dieses Elementes.
        /// </summary>
        protected TArtDesBildes Quelle { get; private set; }

        /// <summary>
        /// Erstellt eine neue Präsentation für ein Element.
        /// </summary>
        /// <param name="name">Der Name des Elementes.</param>
        /// <param name="initialSichtbar">Gesetzt, wenn das Element beim Starten sichtbar sein soll.</param>
        /// <param name="horizontalePosition">Die initiale horizontale Position des Elementes.</param>
        /// <param name="vertikalePosition">Die initiale vertikale Position des Elementes.</param>
        /// <param name="quelle">Die zugehörige Visualisierung.</param>
        /// <param name="ebene">Die Ebene, auf der das Element angezeigt werden soll.</param>
        /// <param name="initialisierung">Eine Methode zur abschliessenden Initialisierung sobald die Simulation bereit ist.</param>
        public BildElementBasis( string name, bool initialSichtbar, double horizontalePosition, double vertikalePosition, TArtDesBildes quelle, int ebene, Func<TArtDesSpielElementes> initialisierung )
            : base( name, initialSichtbar, horizontalePosition, vertikalePosition, ebene, initialisierung )
        {
            // Prüfen
            if (quelle == null)
                throw new ArgumentNullException( "figur" );

            // Merken
            Quelle = quelle;

            // Überwachung aktivieren
            quelle.PropertyChanged += QuelleWurdeVerändert;

            // Initiale Werte setzen
            Breite = quelle.Breite;
            Hoehe = quelle.Hoehe;

            // Verfügbarkeit prüfen
            VerfügbarkeitSetzen();
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich an dem Bild etwas verändert hat.
        /// </summary>
        /// <param name="sender">Wird ignoriert.</param>
        /// <param name="e">Informationen zur Veränderung.</param>
        private void QuelleWurdeVerändert( object sender, PropertyChangedEventArgs e )
        {
            // Schauen wir, was sich verändert hat und dann übernehmen wir diese Änderung geeignet
            if (FuerBildAnzeige.Bild.Equals( e.PropertyName ))
                EigenschaftWurdeVerändert( FuerBildAnzeige.Bild );
            else if (FuerBildAnzeige.Breite.Equals( e.PropertyName ))
                Breite = Quelle.Breite;
            else if (FuerBildAnzeige.Höhe.Equals( e.PropertyName ))
                Hoehe = Quelle.Hoehe;

            // Verfügbarkeit prüfen
            VerfügbarkeitSetzen();
        }
    }
}
