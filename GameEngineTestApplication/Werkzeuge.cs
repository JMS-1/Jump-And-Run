using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;


namespace JMS.JnRV2.Ablauf.Tests
{
    /// <summary>
    /// Eine Sammlung von Hilfsmethoden.
    /// </summary>
    public static class Werkzeuge
    {
        /// <summary>
        /// Verwaltet einen einfachen Befehl
        /// </summary>
        /// <typeparam name="TParameter">Die Art des verwendeten Parameters.</typeparam>
        private class Aktion<TParameter> : ICommand
        {
            /// <summary>
            /// Die mit dem Befehl verbundene Methode.
            /// </summary>
            private readonly Action<TParameter> m_methode;

            /// <summary>
            /// Erstellt einen neuen Befehl.
            /// </summary>
            /// <param name="methode">Die auszuführende Methode.</param>
            public Aktion( Action<TParameter> methode )
            {
                // Merken
                m_methode = methode;
            }

            /// <summary>
            /// Meldet, ob der Befehl ausführbar ist.
            /// </summary>
            /// <param name="parameter">Wird ignoriert.</param>
            /// <returns>Gesetzt, wenn der Befehl ausführbar ist.</returns>
            public bool CanExecute( object parameter )
            {
                // Immer 
                return true;
            }

            /// <summary>
            /// Wird ausgelöst, wenn die Ausführbarkeit des Befehls verändert wurde.
            /// </summary>
#pragma warning disable 0067
            public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

            /// <summary>
            /// Führt den Befehl aus.
            /// </summary>
            /// <param name="parameter">Parameter zur Methode.</param>
            public void Execute( object parameter )
            {
                // Immer
                var methode = m_methode;
                if (methode != null)
                    methode( (TParameter) parameter );
            }
        }

        /// <summary>
        /// Löst ein Ereignis im korrekten Kontext aus.
        /// </summary>
        /// <param name="methode">Die gewünschte Methode.</param>
        /// <param name="parameter">Die Parameter zur Methode.</param>
        public static void EreignisAuslösen( this Delegate methode, params object[] parameter )
        {
            // Nichts zu tun
            if (methode == null)
                return;

            // Verteiler nutzen
            var dispatcher = Deployment.Current.Dispatcher;
            if (dispatcher.CheckAccess())
                methode.DynamicInvoke( parameter );
            else
                dispatcher.BeginInvoke( methode, parameter );
        }

        /// <summary>
        /// Meldet, dass sich eine Eigenschaft verändert hat.
        /// </summary>
        /// <param name="interessenten">Alle Interessenten an Veränderungen von Eigenschaften.</param>
        /// <param name="quelle">Das Objekt, dessen Eigenschaft verändert wurde.</param>
        /// <param name="name">Der Name der Eigenschaft.</param>
        public static void EigenschaftVerändert( this PropertyChangedEventHandler interessenten, INotifyPropertyChanged quelle, string name = null )
        {
            // Weiter reichen
            interessenten.EreignisAuslösen( quelle, new PropertyChangedEventArgs( name ) );
        }

        /// <summary>
        /// Ändert eine Eigenschaft.
        /// </summary>
        /// <typeparam name="TArt">Die Art der Eigenschaft.</typeparam>
        /// <param name="quelle">Das Objekt, dessen Eigenschaft verändert wurde.</param>
        /// <param name="interessenten">Alle Interessenten an Veränderungen von Eigenschaften.</param>
        /// <param name="name">Der Name der Eigenschaft.</param>
        /// <param name="aktuellerWert">Der aktuelle Wert der Eigenschaft.</param>
        /// <param name="neuerWert">Der gewünschte neue Wert der Eigenschaft.</param>
        /// <returns>Gesetzt, wenn sich der Wert verändert hat.</returns>
        public static bool EigenschaftVerändern<TArt>( this INotifyPropertyChanged quelle, PropertyChangedEventHandler interessenten, string name, ref TArt aktuellerWert, TArt neuerWert )
        {
            // Es hat sich nichts verändert
            if (Equals( neuerWert, aktuellerWert ))
                return false;

            // Ändern
            aktuellerWert = neuerWert;

            // Melden
            interessenten.EigenschaftVerändert( quelle, name );

            // Es hat sich etwas verändert
            return true;
        }

        /// <summary>
        /// Erstellt einen Befhel zu einer Methode.
        /// </summary>
        /// <param name="methode">Die Methode zum Befehl.</param>
        /// <returns>Die Repräsentation des Befehls als Methode.</returns>
        public static ICommand WandeleZuBefehl( Action methode )
        {
            // Durchreichen
            return WandeleZuBefehl<object>( p => methode() );
        }

        /// <summary>
        /// Erstellt einen Befhel zu einer Methode.
        /// </summary>
        /// <typeparam name="TParameter">Die Art des Parameters zur Methode.</typeparam>
        /// <param name="methode">Die Methode zum Befehl.</param>
        /// <returns>Die Repräsentation des Befehls als Methode.</returns>
        public static ICommand WandeleZuBefehl<TParameter>( Action<TParameter> methode )
        {
            // Durchreichen
            return new Aktion<TParameter>( methode );
        }
    }
}
