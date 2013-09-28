using System;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Eine Position auf dem Spielfeld. Der Ursprung links unten hat die Position
    /// <i>(0, 0)</i> die rechte obere Ecke ist <i>(1, 1)</i>.
    /// </summary>
    public struct Position
    {
        /// <summary>
        /// Die horizontale Position.
        /// </summary>
        private readonly GenaueZahl m_horizontalePosition;

        /// <summary>
        /// Die vertikale Position.
        /// </summary>
        private readonly GenaueZahl m_vertikalePosition;

        /// <summary>
        /// Erstellt eine neue Position auf dem Spielfeld.
        /// </summary>
        /// <param name="horizontalePosition">Die horizonatale Koordinate.</param>
        /// <param name="vertikalePosition">Die vertikale Koordinate.</param>
        private Position( GenaueZahl horizontalePosition, GenaueZahl vertikalePosition )
        {
            // Merken
            m_horizontalePosition = horizontalePosition;
            m_vertikalePosition = vertikalePosition;
        }

        /// <summary>
        /// Erstellt eine neue Position.
        /// </summary>
        /// <param name="horizontalePosition">Die horizontale Position.</param>
        /// <param name="vertikalePosition">Die vertikale Position.</param>
        /// <returns>Die gewünschte neue Position.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Die Position ist ungültig.</exception>
        public static Position Erzeugen( GenaueZahl horizontalePosition, GenaueZahl vertikalePosition )
        {
            // Prüfen
            if (horizontalePosition < GenaueZahl.Null)
                throw new ArgumentOutOfRangeException( "horizontalePosition" );
            if (vertikalePosition < GenaueZahl.Null)
                throw new ArgumentOutOfRangeException( "vertikalePosition" );
            if (horizontalePosition > GenaueZahl.Eins)
                throw new ArgumentOutOfRangeException( "horizontalePosition" );
            if (vertikalePosition > GenaueZahl.Eins)
                throw new ArgumentOutOfRangeException( "vertikalePosition" );

            // Anlegen
            return new Position( horizontalePosition, vertikalePosition );
        }

        /// <summary>
        /// Erstellt eine neue Position.
        /// </summary>
        /// <param name="horizontalePosition">Die horizontale Position.</param>
        /// <param name="vertikalePosition">Die vertikale Position.</param>
        /// <returns>Die gewünschte neue Position.</returns>
        public static Position SicherErzeugen( GenaueZahl horizontalePosition, GenaueZahl vertikalePosition )
        {
            // Anlegen
            return Erzeugen( horizontalePosition.ZwischenNullUndEins(), vertikalePosition.ZwischenNullUndEins() );
        }

        /// <summary>
        /// Meldet die horizontale Position.
        /// </summary>
        public GenaueZahl HorizontalePosition { get { return m_horizontalePosition; } }

        /// <summary>
        /// Meldet die vertikale Position.
        /// </summary>
        public GenaueZahl VertikalePosition { get { return m_vertikalePosition; } }

        /// <summary>
        /// Meldet einen Anzeigetext zu Testzwecken.
        /// </summary>
        /// <returns>Der gewünschte Anzeigetext.</returns>
        public override string ToString()
        {
            // Zusammenbauen
            return string.Format( "({0}, {1})", m_horizontalePosition, m_vertikalePosition );
        }
    }
}
