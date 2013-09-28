using System;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Beschreibt die Geschwindigkeit eines Elementes.
    /// </summary>
    public class TemporaereGeschwindigkeit : Geschwindigkeit
    {
        /// <summary>
        /// Der Zeitpunkt, bis zu dem die Geschwindigkeit angewendet werden soll.
        /// </summary>
        private TimeSpan m_anwendenBis;

        /// <summary>
        /// Wird aufgerufen, wenn die Geschwindigkeit nicht mehr weiter verwendet wird.
        /// </summary>
        public event Action<Geschwindigkeit> Deaktiviert;

        /// <summary>
        /// Erstellt eine neue Geschwindigkeitsinformation.
        /// </summary>
        /// <param name="xGeschwidigkeit">Die horizontale Geschwindigkeit, wobei <i>1</i> bedeutet, dass das gesamte Spielfeld in einer Sekunden abgelaufen wird.</param>
        /// <param name="yGeschwindigkeit">Die vertikale Geschwindigkeit, wobei <i>1</i> bedeutet, dass das gesamte Spielfeld in einer Sekunden abgelaufen wird.</param>
        /// <param name="gueltigBis">Der Zeitpunkt, bis zu dem die Geschwindigkeit auf das zugehörige Element angewendet werden soll.</param>
        protected TemporaereGeschwindigkeit( GenaueZahl xGeschwidigkeit, GenaueZahl yGeschwindigkeit, TimeSpan gueltigBis )
            : base( xGeschwidigkeit, yGeschwindigkeit )
        {
            // Merken
            m_anwendenBis = gueltigBis;
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Geschwindigkeit deaktiviert wird.
        /// </summary>
        internal override void Deaktivieren()
        {
            // Weiterleiten
            var interessenten = Deaktiviert;
            if (interessenten != null)
                interessenten( this );
        }

        /// <summary>
        /// Erstellt eine neue Geschwindigkeitsinformation.
        /// </summary>
        /// <param name="xGeschwidigkeit">Die horizontale Geschwindigkeit, wobei <i>1</i> bedeutet, dass das gesamte Spielfeld in einer Sekunden abgelaufen wird.</param>
        /// <param name="yGeschwindigkeit">Die vertikale Geschwindigkeit, wobei <i>1</i> bedeutet, dass das gesamte Spielfeld in einer Sekunden abgelaufen wird.</param>
        /// <param name="gueltigBis">Der Zeitpunkt, bis zu dem die Geschwindigkeit auf das zugehörige Element angewendet werden soll.</param>
        /// <returns>Die gewünschte Beschreibung der Geschwindigkeit.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Mindestens ein Parameter ist ungültig.</exception>
        public static TemporaereGeschwindigkeit Erzeugen( GenaueZahl xGeschwidigkeit, GenaueZahl yGeschwindigkeit, TimeSpan gueltigBis )
        {
            // Prüfen
            if (gueltigBis.Ticks < 0)
                throw new ArgumentOutOfRangeException( "gueltigBis" );

            // Anlegen
            return new TemporaereGeschwindigkeit( xGeschwidigkeit, yGeschwindigkeit, gueltigBis );
        }

        /// <summary>
        /// Meldet, bis wann die Geschwindigkeit angewendet werden soll.
        /// </summary>
        public override TimeSpan GültigBis { get { return m_anwendenBis; } }

        /// <summary>
        /// Meldet einen Anzeigetext zu Testzwecken.
        /// </summary>
        /// <returns>Der gewünschte Anzeigetext.</returns>
        public override string ToString()
        {
            // Zusammenbauen
            return string.Format( "({0}, {1}) bis {2}", HorizontaleGeschwindigkeit, VertikaleGeschwindigkeit, m_anwendenBis );
        }
    }
}
