using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;


namespace JMS.JnRV2.Anzeige.Praesentation
{
    /// <summary>
    /// Beschreibt die Steuerungselemente der Simulation.
    /// </summary>
    public interface IFuerSpielSteuerung : INotifyPropertyChanged
    {
        /// <summary>
        /// Meldet den Befehl zum Starten der Simulation.
        /// </summary>
        ICommand Starten { get; }

        /// <summary>
        /// Meldet den Befehl zum Anhalten der Simulation.
        /// </summary>
        ICommand Anhalten { get; }

        /// <summary>
        /// Meldet den Befehl, mit dem die Spielfigur schneller nach links bewegt wird.
        /// </summary>
        ICommand SchnellerNachLinks { get; }

        /// <summary>
        /// Meldet den Befehl, mit dem die Spielfigur schneller nach rechts bewegt wird.
        /// </summary>
        ICommand SchnellerNachRechts { get; }

        /// <summary>
        /// Meldet den Befehl, mit dem die Spielfigur angehalten wird.
        /// </summary>
        ICommand Stillgestanden { get; }

        /// <summary>
        /// Meldet den Befehl, mit dem die Spielfigur zum Sprung veranlasst wird.
        /// </summary>
        ICommand Springen { get; }

        /// <summary>
        /// Gesetzt, sobald die Simulation zugeordnet wurde.
        /// </summary>
        bool SimulationIstVerfügbar { get; }

        /// <summary>
        /// Meldet die Sichtbarkeit der Steuerung.
        /// </summary>
        Visibility Sichtbarkeit { get; }

        /// <summary>
        /// Meldet, ob das Schlussbild sichtbar ist.
        /// </summary>
        Visibility SichtbarkeitSchlussbild { get; }

        /// <summary>
        /// Meldet das Abscshlussbild, sofern das Spiel beendet wurde.
        /// </summary>
        IFuerBildAnzeige Schlussbild { get; }

        /// <summary>
        /// Meldet die Spielzeit in Sekunden.
        /// </summary>
        int SpielzeitInSekunden { get; }

        /// <summary>
        /// Wird aufgerufen, sobald eine neue Melodie abgespielt werden kann.
        /// </summary>
        /// <param name="melodieStarten">Methode zum Starten einer neuen Melodie.</param>
        void MusikKannAbgespieltWerden( Action<string> melodieStarten );

        /// <summary>
        /// Die aktuellen Ergebnisse.
        /// </summary>
        IFuerErgebnisAnzeige Ergebnis { get; }
    }

    /// <summary>
    /// Die Namen aller Eigenschaften der <see cref="IFuerSpielSteuerung"/> Schnittstelle.
    /// </summary>
    public static class FuerSpielSteuerung
    {
        /// <summary>
        /// Der Name der Eigenschaft mit der Verfügbarkeit der Simulation.
        /// </summary>
        public static readonly string SimulationIstVerfügbar = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerSpielSteuerung i ) => i.SimulationIstVerfügbar );

        /// <summary>
        /// Der Name der Eigenschaft mit der Sichtbarkeit der Steuerelemente.
        /// </summary>
        public static readonly string Sichtbarkeit = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerSpielSteuerung i ) => i.Sichtbarkeit );

        /// <summary>
        /// Der Name der Eigenschaft mit dem Schlussbildes.
        /// </summary>
        public static readonly string Schlussbild = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerSpielSteuerung i ) => i.Schlussbild );

        /// <summary>
        /// Der Name der Eigenschaft mit der Sichtbarkeit des Schlussbildes.
        /// </summary>
        public static readonly string SichtbarkeitSchlussbild = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerSpielSteuerung i ) => i.SichtbarkeitSchlussbild );

        /// <summary>
        /// Der Name der Eigenschaft mit der bisher verstrichenen Spielzeit.
        /// </summary>
        public static readonly string SpielzeitInSekunden = ErweiterungenZurVereinfachung.ErmitteleDenNamenEinerEigenschaft( ( IFuerSpielSteuerung i ) => i.SpielzeitInSekunden );
    }
}
