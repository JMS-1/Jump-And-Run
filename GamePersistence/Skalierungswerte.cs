

namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Hier sind alle Werte eingetragen, die zur Umrechnung von Werten in der Konfiguration
    /// benötigt werden.
    /// </summary>
    public static class Skalierungswerte
    {
        /// <summary>
        /// Mit diesem Wert werden horizontale Geschwindigkeiten umgerechnet. Hat ein Spielelement
        /// (vor allem die Spielfigur) eine Geschwindigkeit von <i>1</i>, so wird die Breite
        /// des Elementes mit diesem Faktor multipliziert, um die horizontale Geschwindigkeit
        /// in Pixel pro Sekunde zu ermitteln.
        /// </summary>
        /// <remarks>
        /// Eine Normfigur mit einer Breite von 40 Pixeln würde sich bei einer Geschwindigkeit
        /// von <i>1</i> um <i>68</i> Pixel pro Sekunde bewegen.
        /// </remarks>
        public const decimal EinfacheHorizontaleGeschwindigkeit = 1.7m;

        /// <summary>
        /// Mit diesem Wert wird die Sprungstärke umgerechnet. Hat ein Spielelement (vor
        /// allem die Spielfigur) eine Sprungstärke von <i>1</i>, so wir die Höhe des
        /// Elementes mit diesem Faktor sowie der <see cref="SprungDauer"/> multipliziert um 
        /// die Höhe zu ermitteln, die mit einem einfachen Sprung erreicht werden kann.
        /// </summary>
        /// <remarks>
        /// Eine Normfigur mit einer Höhe von 60 Pixeln würde bei einer Sprungstärke
        /// von <i>100</i> und einer Sprungdauer von <i>0.1</i> eine Sprunghöhe von 
        /// <i>150</i> Pixeln erreichen.</remarks>
        public const decimal EinfacheSprungstärke = 0.25m * SprungDauer;

        /// <summary>
        /// Die Sekunden, die für einen Sprung in die Höhe verwendet wird. Bereits mit Beginn
        /// des Sprungs beginnt im Allgemeinen der Fall zurück zum Boden.
        /// </summary>
        public const decimal SprungDauer = 0.1m;

        /// <summary>
        /// Die Pixel pro Sekunde, die ein jedes Spielelement nach unten fällt, sofern
        /// für das Element die Erdanziehung berücksichtigt werden soll (wie etwa bei
        /// der Spielfigur).
        /// </summary>
        /// <remarks>
        /// Wird eine Normfigur mit einer Höhe von <i>60</i> Pixeln verwendet, so
        /// beträgt die Fallgeschwindigkeit <i>600</i> Pixel pro Sekunde.
        /// </remarks>
        public const decimal Fallgeschwindigkeit = 10m;
    }
}
