using System;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Beschreibt die im Spielverlauf unveränderliche Ausdehnung eines Elements
    /// in relativen Angaben zwischen <i>0.0</i> und <i>1.0</i>.
    /// </summary>
    public struct Ausdehnung
    {
        /// <summary>
        /// Die Breite.
        /// </summary>
        private readonly GenaueZahl m_breite;

        /// <summary>
        /// Die Höhe.
        /// </summary>
        private readonly GenaueZahl m_hoehe;

        /// <summary>
        /// Erstellt eine neue Ausdehung.
        /// </summary>
        /// <param name="breite">Die Breite der Fläche.</param>
        /// <param name="höhe">Die Höhe der Fläche.</param>
        private Ausdehnung( GenaueZahl breite, GenaueZahl höhe )
        {
            // Merken
            m_breite = breite;
            m_hoehe = höhe;
        }

        /// <summary>
        /// Erstellt eine neue Ausdehnung.
        /// </summary>
        /// <param name="breite">Die relative Breite.</param>
        /// <param name="höhe">Die relative Höhe.</param>
        /// <returns>Die gewünschte neue Ausdehnung.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Die angegebene Ausdehung ist unzulässig.</exception>
        public static Ausdehnung Erzeugen( GenaueZahl breite, GenaueZahl höhe )
        {
            // Prüfen
            if (breite < GenaueZahl.Null)
                throw new ArgumentOutOfRangeException( "breite" );
            if (höhe < GenaueZahl.Null)
                throw new ArgumentOutOfRangeException( "höhe" );
            if (breite > GenaueZahl.Eins)
                throw new ArgumentOutOfRangeException( "breite" );
            if (höhe > GenaueZahl.Eins)
                throw new ArgumentOutOfRangeException( "höhe" );

            // Merken
            return new Ausdehnung( breite, höhe );
        }

        /// <summary>
        /// Meldet die Breite.
        /// </summary>
        public GenaueZahl Breite { get { return m_breite; } }

        /// <summary>
        /// Meldet die Höhe.
        /// </summary>
        public GenaueZahl Höhe { get { return m_hoehe; } }

        /// <summary>
        /// Meldet einen Anzeigetext zu Testzwecken.
        /// </summary>
        /// <returns>Der gewünschte Anzeigetext.</returns>
        public override string ToString()
        {
            // Zusammenbauen
            return string.Format( "({0}, {1})", m_breite, m_hoehe );
        }
    }
}
