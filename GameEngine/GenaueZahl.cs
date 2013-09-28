using System;
using System.Collections.Generic;
using System.Globalization;


namespace JMS.JnRV2.Ablauf
{
    /// <summary>
    /// Repräsentiert eine Festkommazahl mit einer wohldefinierten Genauigkeit von
    /// exakt dreizehn Stellen nach dem Komma. Diese Zahlen werden im Allgemeinen 
    /// innerhalb der Simulation im relativen Koordinatensystem verwendet.
    /// </summary>
    public struct GenaueZahl : IComparable<GenaueZahl>
    {
        /// <summary>
        /// Der Ursprung des Koordinatensystems.
        /// </summary>
        public static GenaueZahl Null = new GenaueZahl( 0m );

        /// <summary>
        /// Das Ende des Koordinatensystems.
        /// </summary>
        public static GenaueZahl Eins = new GenaueZahl( 10000000000000m );

        /// <summary>
        /// Die gewünschte Koordinate skaliert mit <see cref="Eins"/>.
        /// </summary>
        private readonly decimal m_wert;

        /// <summary>
        /// Legt eine neue Zahl an.
        /// </summary>
        /// <param name="wert">Der Wert in voller Darstellung.</param>
        private GenaueZahl( decimal wert )
        {
            // Merken
            m_wert = decimal.Truncate( wert );
        }

        /// <summary>
        /// Meldet einen Anzeigewert für die Korrdinate.
        /// </summary>
        /// <returns>Der gewünschte Anzeigewert.</returns>
        public override string ToString()
        {
            // Melden
            return ((decimal) this).ToString( "G", CultureInfo.InvariantCulture );
        }

        /// <summary>
        /// Stellt sicher, dass sich die Zahl zwischen 0 und 1 befindet.
        /// </summary>
        /// <returns>Die Zahl oder <see cref="Null"/> oder <see cref="Eins"/>.</returns>
        public GenaueZahl ZwischenNullUndEins()
        {
            // Prüfen
            if (m_wert < Null.m_wert)
                return Null;
            else if (m_wert > Eins.m_wert)
                return Eins;
            else
                return this;
        }

        /// <summary>
        /// Wandelt eine Koordinate in eine Fließkommazahl.
        /// </summary>
        /// <param name="koordinate">Die Koordinate.</param>
        /// <returns>Die zugehörige Fließkommazahl.</returns>
        public static explicit operator double( GenaueZahl koordinate )
        {
            // Andere Wandlung nutzen
            return (double) (decimal) koordinate;
        }

        /// <summary>
        /// Wandelt eine Koordinate in eine Festkommazahl.
        /// </summary>
        /// <param name="koordinate">Die Koordinate.</param>
        /// <returns>Die zugehörige Festkommazahl.</returns>
        public static explicit operator decimal( GenaueZahl koordinate )
        {
            // Relativ zum Bezugspunkt bereichnen
            return koordinate.m_wert / Eins.m_wert;
        }

        /// <summary>
        /// Wandelt eine Festkomma in eine Koordinate.
        /// </summary>
        /// <param name="zahl">Die gewünschte Zahl.</param>
        /// <returns>Die zugehörige Koordinate.</returns>
        public static explicit operator GenaueZahl( decimal zahl )
        {
            // Andere Wandlung nutzen
            return new GenaueZahl( checked( zahl * Eins.m_wert ) );
        }

        /// <summary>
        /// Wandelt eine Fließkommazahl in eine Koordinate.
        /// </summary>
        /// <param name="zahl">Die gewünschte Zahl.</param>
        /// <returns>Die zugehörige Koordinate.</returns>
        public static explicit operator GenaueZahl( double zahl )
        {
            // Andere Wandlung nutzen
            return (GenaueZahl) checked( (decimal) zahl );
        }

        /// <summary>
        /// Vergleicht eine Koordinate mit einem beliebigen Objekt.
        /// </summary>
        /// <param name="einObjekt">Irgendein Objekt.</param>
        /// <returns>Das Ergebnis des Vergleichs.</returns>
        public override bool Equals( object einObjekt )
        {
            // Typ prüfen
            var andereKoordinate = einObjekt as GenaueZahl?;
            if (!andereKoordinate.HasValue)
                return false;

            // Vergleich nutzen
            return CompareTo( andereKoordinate.Value ) == 0;
        }

        /// <summary>
        /// Meldet ein Kürzel für die Koordinate.
        /// </summary>
        /// <returns>Das gewünschte Kürzel.</returns>
        public override int GetHashCode()
        {
            // Melden
            return m_wert.GetHashCode();
        }

        /// <summary>
        /// Negiert eine Koordinate.
        /// </summary>
        /// <param name="koordinate">Eine Koordinate.</param>
        /// <returns>Das Ergebnis der Berechnung.</returns>
        public static GenaueZahl operator -( GenaueZahl koordinate )
        {
            // Durchreichen
            return new GenaueZahl( -koordinate.m_wert );
        }

        /// <summary>
        /// Vergleicht zwei Koordinaten.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Gesetzt, wenn die erste Koordinate kleiner als die zweite Koordinate ist.</returns>
        public static bool operator <( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Durchreichen
            return ersteKoordinate.CompareTo( zweiteKoordinate ) < 0;
        }

        /// <summary>
        /// Vergleicht zwei Koordinaten.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Gesetzt, wenn die erste Koordinate größer als die zweite Koordinate ist.</returns>
        public static bool operator >( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Durchreichen
            return ersteKoordinate.CompareTo( zweiteKoordinate ) > 0;
        }

        /// <summary>
        /// Vergleicht zwei Koordinaten.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Gesetzt, wenn die erste Koordinate kleiner oder gleich der zweiten Koordinate ist.</returns>
        public static bool operator <=( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Durchreichen
            return ersteKoordinate.CompareTo( zweiteKoordinate ) <= 0;
        }

        /// <summary>
        /// Vergleicht zwei Koordinaten.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Gesetzt, wenn die erste Koordinate größer oder gleich der zweiten Koordinate ist.</returns>
        public static bool operator >=( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Durchreichen
            return ersteKoordinate.CompareTo( zweiteKoordinate ) >= 0;
        }

        /// <summary>
        /// Vergleicht zwei Koordinaten.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Gesetzt, wenn die erste Koordinate gleich der zweiten Koordinate ist.</returns>
        public static bool operator ==( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Durchreichen
            return ersteKoordinate.CompareTo( zweiteKoordinate ) == 0;
        }

        /// <summary>
        /// Vergleicht zwei Koordinaten.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Gesetzt, wenn die erste Koordinate nicht gleich der zweiten Koordinate ist.</returns>
        public static bool operator !=( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Durchreichen
            return ersteKoordinate.CompareTo( zweiteKoordinate ) != 0;
        }

        /// <summary>
        /// Addiert zwei Koordinaten.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Das Ergebnis der Berechnung.</returns>
        public static GenaueZahl operator +( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Durchreichen
            return new GenaueZahl( checked( ersteKoordinate.m_wert + zweiteKoordinate.m_wert ) );
        }

        /// <summary>
        /// Subtrahiert zwei Koordinaten.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Das Ergebnis der Berechnung.</returns>
        public static GenaueZahl operator -( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Durchreichen
            return new GenaueZahl( checked( ersteKoordinate.m_wert - zweiteKoordinate.m_wert ) );
        }

        /// <summary>
        /// Dividiert eine Koordinaten durch eine Zahl.
        /// </summary>
        /// <param name="koordinate">Eine Koordinate.</param>
        /// <param name="zahl">Eine Zahl.</param>
        /// <returns>Das Ergebnis der Berechnung.</returns>
        public static GenaueZahl operator /( GenaueZahl koordinate, decimal zahl )
        {
            // Ausführen
            return new GenaueZahl( checked( koordinate.m_wert / zahl ) );
        }

        /// <summary>
        /// Dividiert eine Koordinaten durch eine andere.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Das Ergebnis der Berechnung.</returns>
        public static GenaueZahl operator /( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Durchreichen
            return ersteKoordinate / (decimal) zweiteKoordinate;
        }

        /// <summary>
        /// Multipliziert eine Koordinaten mit einer Zahl.
        /// </summary>
        /// <param name="koordinate">Eine Koordinate.</param>
        /// <param name="zahl">Eine Zahl.</param>
        /// <returns>Das Ergebnis der Berechnung.</returns>
        public static GenaueZahl operator *( GenaueZahl koordinate, decimal zahl )
        {
            // Durchreichen
            return new GenaueZahl( checked( koordinate.m_wert * zahl ) );
        }

        /// <summary>
        /// Multipliziert eine Koordinaten mit einer andere.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Das Ergebnis der Berechnung.</returns>
        public static GenaueZahl operator *( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Durchreichen
            return ersteKoordinate * (decimal) zweiteKoordinate;
        }

        /// <summary>
        /// Meldet den Absolutwert der Koordinate.
        /// </summary>
        /// <returns>Das Ergebnis der Berechnung.</returns>
        public GenaueZahl Abs()
        {
            // Erzeugen
            if (m_wert < 0)
                return -this;
            else
                return this;
        }

        /// <summary>
        /// Vergleicht diese Koordinate mit einer anderen.
        /// </summary>
        /// <param name="andereKoordinate">Eine andere Koordinate.</param>
        /// <returns>Der Unterschied zwischen den Koordinaten,</returns>
        public int CompareTo( GenaueZahl andereKoordinate )
        {
            // Durchreichen
            return m_wert.CompareTo( andereKoordinate.m_wert );
        }
    }

    /// <summary>
    /// Einige Methoden zur einfacheren Nutzung der <see cref="GenaueZahl"/>.
    /// </summary>
    public static class GenaueZahlHelfer
    {
        /// <summary>
        /// Ermittelt die größere Koordinate.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Die größere der beiden Koordinaten.</returns>
        internal static GenaueZahl Max( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Vergleichen
            if (ersteKoordinate < zweiteKoordinate)
                return zweiteKoordinate;
            else
                return ersteKoordinate;
        }

        /// <summary>
        /// Ermittelt die kleinere Koordinate.
        /// </summary>
        /// <param name="ersteKoordinate">Eine Koordinate.</param>
        /// <param name="zweiteKoordinate">Eine andere Koordinate.</param>
        /// <returns>Die kleinere der beiden Koordinaten.</returns>
        internal static GenaueZahl Min( GenaueZahl ersteKoordinate, GenaueZahl zweiteKoordinate )
        {
            // Vergleichen
            if (ersteKoordinate < zweiteKoordinate)
                return ersteKoordinate;
            else
                return zweiteKoordinate;
        }

        /// <summary>
        /// Addiert eine Liste von Koordinaten.
        /// </summary>
        /// <param name="zahlen">Die gewünschte Liste.</param>
        /// <returns>Das Ergebnis der Berechnung.</returns>
        public static GenaueZahl Sum( this IEnumerable<GenaueZahl> zahlen )
        {
            // Prüfen
            if (zahlen == null)
                throw new NullReferenceException( "zahlen" );

            // Summe
            var summe = GenaueZahl.Null;

            // Summe berechnen
            foreach (var zahl in zahlen)
                summe = summe + zahl;

            // Melden
            return summe;
        }
    }
}
