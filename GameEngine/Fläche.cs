

namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Beschreibt eine Fläche auf dem Spielfeld.
    /// </summary>
    public class Fläche
    {
        /// <summary>
        /// Die aktuelle Position auf dem Spielfeld.
        /// </summary>
        private Position m_position;

        /// <summary>
        /// Meldet die aktuelle Position auf dem Spielfeld.
        /// </summary>
        public Position Position { get { return m_position; } }

        /// <summary>
        /// Meldet die Ausdehung.
        /// </summary>
        public Ausdehnung Ausdehnung { get; private set; }

        /// <summary>
        /// Der Bereich, den die Fläche belegt.
        /// </summary>
        public Bereich Bereich { get; private set; }

        /// <summary>
        /// Erstellt eine neue Fläche.
        /// </summary>
        /// <param name="position">Die Position auf dem Spielfeld.</param>
        /// <param name="ausdehnung">Die relative Größe.</param>
        protected Fläche( Position position, Ausdehnung ausdehnung )
        {
            // Merken
            Ausdehnung = ausdehnung;
            m_position = position;

            // Bereich festlegen
            Bereich = Bereich.Erzeugen( m_position, Ausdehnung );
        }

        /// <summary>
        /// Erstellt ein neues Element.
        /// </summary>
        /// <param name="position">Die Position auf dem Spielfeld.</param>
        /// <param name="ausdehnung">Die relative Größe.</param>
        /// <returns>Die neue Fläche.</returns>
        public static Fläche Erzeugen( Position position, Ausdehnung ausdehnung )
        {
            // Durchreichen
            return new Fläche( position, ausdehnung );
        }

        /// <summary>
        /// Verschiebt die Fläche auf dem Spielfeld.
        /// </summary>
        /// <param name="neuePosition">Die neue Position der Fläche.</param>
        protected void Verschieben( Position neuePosition )
        {
            // Neue Position merken
            m_position = neuePosition;

            // Bereich aktualisieren
            Bereich = Bereich.Erzeugen( neuePosition, Ausdehnung );
        }
    }
}
