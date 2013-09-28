using JMS.JnRV2.Ablage;
using JMS.JnRV2.Anzeige.PraesentationsModelle;
using JMS.JnRV2.Anzeige.Verbinder;


namespace JMS.JnRV2.Start
{
    /// <summary>
    /// Die Informationen zur Anzeige einer Spielfigur mit einigen zusätzlichen Informationen daran.
    /// </summary>
    public class SpielfigurMitInformation
    {
        /// <summary>
        /// Die maximale Geschwindigkeit dieser Figur.
        /// </summary>
        public int Geschwindigkeit { get; private set; }

        /// <summary>
        /// Die Anzahl der Sprünge, die eine Figur in schneller Folge machen darf.
        /// </summary>
        public int Spruenge { get; private set; }

        /// <summary>
        /// Die Stärke eines einzelnen Sprungs der Figur.
        /// </summary>
        public int Sprungstaerke { get; private set; }

        /// <summary>
        /// Die visuelle Darstellung der Figur.
        /// </summary>
        public Figur Figur { get; private set; }

        /// <summary>
        /// Die zugehörige Konfiguration.
        /// </summary>
        public Spielfigur Konfiguration { get; private set; }

        /// <summary>
        /// Der Name der Spielfigur.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Erstellt eine neue Information.
        /// </summary>
        /// <param name="figur">Die Spielfigur.</param>
        public SpielfigurMitInformation( Spielfigur figur )
        {
            // Die Originalkonfiguration
            Konfiguration = figur;

            // Merkt sich die Spielfigur
            Figur = Konfiguration.ErzeugePräsentation();

            // Übernehmen
            Geschwindigkeit = figur.MaximaleGeschwindigkeit;
            Spruenge = figur.SpruengeNacheinander;
            Sprungstaerke = figur.SprungStaerke;
            Name = figur.Name;
        }
    }
}
