using System;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Beschreibt den Bereich, den ein Element abdeckt.
    /// </summary>
    public struct Bereich
    {
        /// <summary>
        /// Die linke Seite.
        /// </summary>
        private readonly GenaueZahl m_kleinsteHorizontalePosition;

        /// <summary>
        /// Meldet die linke Seite.
        /// </summary>
        public GenaueZahl KleinsteHorizontalePosition { get { return m_kleinsteHorizontalePosition; } }

        /// <summary>
        /// Die rechte Seite.
        /// </summary>
        private readonly GenaueZahl m_größteHorizontalePosition;

        /// <summary>
        /// Meldet die rechte Seite.
        /// </summary>
        public GenaueZahl GrößteHorizontalePosition { get { return m_größteHorizontalePosition; } }

        /// <summary>
        /// Das untere Ende.
        /// </summary>
        private readonly GenaueZahl m_kleinsteVertikalePosition;

        /// <summary>
        /// Meldet das untere Ende.
        /// </summary>
        public GenaueZahl KleinsteVertikalePosition { get { return m_kleinsteVertikalePosition; } }

        /// <summary>
        /// Meldet das obere Ende.
        /// </summary>
        public GenaueZahl GrößteVertikalePosition { get { return m_größteVertikalePosition; } }

        /// <summary>
        /// Das obere Ende.
        /// </summary>
        private readonly GenaueZahl m_größteVertikalePosition;

        /// <summary>
        /// Meldet die Breite des Bereichs.
        /// </summary>
        public GenaueZahl Breite { get { return m_größteHorizontalePosition - m_kleinsteHorizontalePosition; } }

        /// <summary>
        /// Meldet die Höhe des Bereichs.
        /// </summary>
        public GenaueZahl Höhe { get { return m_größteVertikalePosition - m_kleinsteVertikalePosition; } }

        /// <summary>
        /// Erstellt einen neuen Bereich.
        /// </summary>
        /// <param name="horizontalePosition">Der horizontale Anfang des Bereichs.</param>
        /// <param name="vertikalePosition">Der vertikale Anfang des Bereichs.</param>
        /// <param name="breite">Die Breite des Bereichs.</param>
        /// <param name="höhe">Die Höhe des Bereichs.</param>
        private Bereich( GenaueZahl horizontalePosition, GenaueZahl vertikalePosition, GenaueZahl breite, GenaueZahl höhe )
        {
            // Merken
            m_größteHorizontalePosition = horizontalePosition + breite;
            m_größteVertikalePosition = vertikalePosition + höhe;
            m_kleinsteHorizontalePosition = horizontalePosition;
            m_kleinsteVertikalePosition = vertikalePosition;
        }

        /// <summary>
        /// Erstellt einen neuen Bereich für ein Element.
        /// </summary>
        /// <param name="position">Die Koordinaten des Elementzentrums.</param>
        /// <param name="ausdehnung">Die Ausdehung des Elementes.</param>
        /// <returns>Der gewünschte Bereich.</returns>
        public static Bereich Erzeugen( Position position, Ausdehnung ausdehnung )
        {
            // Erzeugen
            return
                Erzeugen
                    (
                        position.HorizontalePosition - ausdehnung.Breite / 2,
                        position.VertikalePosition - ausdehnung.Höhe / 2,
                        ausdehnung.Breite,
                        ausdehnung.Höhe
                    );
        }

        /// <summary>
        /// Erstellt einen neuen Bereich.
        /// </summary>
        /// <param name="horizontalePosition">Der horizontale Anfang des Bereichs.</param>
        /// <param name="vertikalePosition">Der vertikale Anfang des Bereichs.</param>
        /// <param name="breite">Die Breite des Bereichs.</param>
        /// <param name="höhe">Die Höhe des Bereichs.</param>
        /// <returns>Der gewünschte Bereich.</returns>
        public static Bereich Erzeugen( GenaueZahl horizontalePosition, GenaueZahl vertikalePosition, GenaueZahl breite, GenaueZahl höhe )
        {
            // Erzeugen
            if (breite < GenaueZahl.Null)
                return Erzeugen( horizontalePosition + breite, vertikalePosition, -breite, höhe );
            else if (höhe < GenaueZahl.Null)
                return Erzeugen( horizontalePosition, vertikalePosition + höhe, breite, -höhe );
            else
                return new Bereich( horizontalePosition, vertikalePosition, breite, höhe );
        }

        /// <summary>
        /// Gesetzt, wenn zwei Bereiche sich überschneiden.
        /// </summary>
        /// <param name="andererBereich">Irgendein anderer Bereich.</param>
        /// <returns>Gesetzt, wenn eine Überschneidung vorliegt.</returns>
        public bool ÜberschneidetSichMit( Bereich andererBereich )
        {
            // Horizontal prüfen
            if (m_größteHorizontalePosition <= andererBereich.m_kleinsteHorizontalePosition)
                return false;
            if (andererBereich.m_größteHorizontalePosition <= m_kleinsteHorizontalePosition)
                return false;

            // Vertikal prüfen
            if (m_größteVertikalePosition <= andererBereich.m_kleinsteVertikalePosition)
                return false;
            if (andererBereich.m_größteVertikalePosition <= m_kleinsteVertikalePosition)
                return false;

            // Überschneidung gefunden
            return true;
        }

        /// <summary>
        /// Meldet einen Anzeigetext zu Testzwecken.
        /// </summary>
        /// <returns>Der gewünschte Anzeigetext.</returns>
        public override string ToString()
        {
            // Zusammenbauen
            return string.Format( "({0}..{1}, {2}..{3})", m_kleinsteHorizontalePosition, m_größteHorizontalePosition, m_kleinsteVertikalePosition, m_größteVertikalePosition );
        }
    }
}
