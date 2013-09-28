using System;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Beschreibt die Geschwindigkeit eines Elementes.
    /// </summary>
    public class Geschwindigkeit
    {
        /// <summary>
        /// Die horizontale Geschwindigkeit.
        /// </summary>
        private readonly GenaueZahl m_horizontaleGeschwindigkeit;

        /// <summary>
        /// Die vertikale Geschwindigkeit.
        /// </summary>
        private readonly GenaueZahl m_vertikaleGeschwindigkeit;

        /// <summary>
        /// Erstellt eine neue Geschwindigkeitsinformation.
        /// </summary>
        /// <param name="horizontaleGeschwindigkeit">Die horizontale Geschwindigkeit, wobei <i>1</i> bedeutet, dass das gesamte Spielfeld in einer Sekunden abgelaufen wird.</param>
        /// <param name="vertikaleGeschwindigkeit">Die vertikale Geschwindigkeit, wobei <i>1</i> bedeutet, dass das gesamte Spielfeld in einer Sekunden abgelaufen wird.</param>
        protected Geschwindigkeit( GenaueZahl horizontaleGeschwindigkeit, GenaueZahl vertikaleGeschwindigkeit )
        {
            // Merken
            m_horizontaleGeschwindigkeit = horizontaleGeschwindigkeit;
            m_vertikaleGeschwindigkeit = vertikaleGeschwindigkeit;
        }

        /// <summary>
        /// Erstellt eine neue Geschwindigkeitsinformation.
        /// </summary>
        /// <param name="horizontaleGeschwindigkeit">Die horizontale Geschwindigkeit, wobei <i>1</i> bedeutet, dass das gesamte Spielfeld in einer Sekunden abgelaufen wird.</param>
        /// <param name="vertikaleGeschwindigkeit">Die vertikale Geschwindigkeit, wobei <i>1</i> bedeutet, dass das gesamte Spielfeld in einer Sekunden abgelaufen wird.</param>
        /// <returns>Die gewünschte Beschreibung der Geschwindigkeit.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Mindestens ein Parameter ist ungültig.</exception>
        public static Geschwindigkeit Erzeugen( GenaueZahl horizontaleGeschwindigkeit, GenaueZahl vertikaleGeschwindigkeit )
        {
            // Anlegen
            return new Geschwindigkeit( horizontaleGeschwindigkeit, vertikaleGeschwindigkeit );
        }

        /// <summary>
        /// Die horizontale Geschwindigkeit.
        /// </summary>
        public GenaueZahl HorizontaleGeschwindigkeit { get { return m_horizontaleGeschwindigkeit; } }

        /// <summary>
        /// Die vertikale Geschwindigkeit.
        /// </summary>
        public GenaueZahl VertikaleGeschwindigkeit { get { return m_vertikaleGeschwindigkeit; } }

        /// <summary>
        /// Meldet, bis wann die Geschwindigkeit angewendet werden soll.
        /// </summary>
        public virtual TimeSpan GültigBis { get { return TimeSpan.MaxValue; } }

        /// <summary>
        /// Wird aufgerufen, wenn die Geschwindigkeit deaktiviert wird.
        /// </summary>
        internal virtual void Deaktivieren() { }

        /// <summary>
        /// Meldet einen Anzeigetext zu Testzwecken.
        /// </summary>
        /// <returns>Der gewünschte Anzeigetext.</returns>
        public override string ToString()
        {
            // Zusammenbauen
            return string.Format( "({0}, {1}) für immer", m_horizontaleGeschwindigkeit, m_vertikaleGeschwindigkeit );
        }
    }
}
