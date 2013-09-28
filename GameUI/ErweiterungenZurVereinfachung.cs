using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;


namespace JMS.JnRV2.Anzeige
{
    /// <summary>
    /// Einige Hilfsmethoden zum einfacheren Auslösen von Ereignissen.
    /// </summary>
    public static class ErweiterungenZurVereinfachung
    {
        /// <summary>
        /// Ermittelt aus einem Ausdruck den Namen einer Eigenschaft.
        /// </summary>
        /// <typeparam name="TArtDesWertes">Die Art des Wertes.</typeparam>
        /// <param name="ausdruck">Der auszuwertende Ausdruck.</param>
        /// <returns>Der Name der verwendeten Eigenschaft.</returns>
        public static string ErmitteleDenNamenEinerEigenschaft<TArtDesWertes>( Expression<Func<TArtDesWertes>> ausdruck )
        {
            // Durchreichen
            return ErmitteleDenNamenEinerEigenschaft( (LambdaExpression) ausdruck );
        }

        /// <summary>
        /// Ermittelt aus einem Ausdruck den Namen einer Eigenschaft.
        /// </summary>
        /// <typeparam name="TKlasse">Die zu untersuchende Objektklasse.</typeparam>
        /// <typeparam name="TArtDesWertes">Die Art des Wertes.</typeparam>
        /// <param name="ausdruck">Der auszuwertende Ausdruck.</param>
        /// <returns>Der Name der verwendeten Eigenschaft.</returns>
        public static string ErmitteleDenNamenEinerEigenschaft<TKlasse, TArtDesWertes>( Expression<Func<TKlasse, TArtDesWertes>> ausdruck )
        {
            // Durchreichen
            return ErmitteleDenNamenEinerEigenschaft( (LambdaExpression) ausdruck );
        }

        /// <summary>
        /// Ermittelt aus einem Ausdruck den Namen einer Eigenschaft.
        /// </summary>
        /// <param name="ausdruck">Der auszuwertende Ausdruck.</param>
        /// <returns>Der Name der verwendeten Eigenschaft.</returns>
        private static string ErmitteleDenNamenEinerEigenschaft( LambdaExpression ausdruck )
        {
            // Get nur in bestimmten Fällen
            var formel = (MemberExpression) ausdruck.Body;

            // Name auslesen
            return formel.Member.Name;
        }

        /// <summary>
        /// Löst ein Ereignis int der Oberflächenumgebung aus.
        /// </summary>
        /// <param name="ereignis">Das gewünschte Ereignis.</param>
        /// <param name="args">Die Parameter zum Ereignis.</param>
        public static void EreignisSicherAuslösen( this Delegate ereignis, params object[] args )
        {
            // Keine Interessenten
            if (ereignis == null)
                return;

            // Situation prüfen
            var dispatcher = Deployment.Current.Dispatcher;
            if (dispatcher.CheckAccess())
                ereignis.DynamicInvoke( args );
            else
                dispatcher.BeginInvoke( ereignis, args );
        }

        /// <summary>
        /// Meldet, dass sich eine Eigenschaft verändert hat.
        /// </summary>
        /// <param name="interessenten">Alle, die an Informationen über Änderungen an der Quelle interessiert sind.</param>
        /// <param name="quelle">Die Quelle, deren Eigenschaft verändert wurde.</param>
        /// <param name="nameDerEigenschaft">Der Name der veränderten Eigenschaft.</param>
        /// <exception cref="ArgumentNullException">Es wurde keine Quelle angegeben.</exception>
        public static void EigenschaftWurdeVerändert( this PropertyChangedEventHandler interessenten, INotifyPropertyChanged quelle, string nameDerEigenschaft )
        {
            // Prüfen
            if (quelle == null)
                throw new ArgumentNullException( "quelle" );

            // Weiterreichen
            interessenten.EreignisSicherAuslösen( quelle, new PropertyChangedEventArgs( nameDerEigenschaft ) );
        }

        /// <summary>
        /// Wird aufgerufen, um einen neuen Wert für eine Eigenschaft einzutragen.
        /// </summary>
        /// <typeparam name="TArtDerEigenschaft">Die Art der Eigenschaft.</typeparam>
        /// <param name="interessenten">Alle, die an Informationen über Änderungen an der Quelle interessiert sind.</param>
        /// <param name="quelle">Die Quelle, deren Eigenschaft verändert wird.</param>
        /// <param name="nameDerEigenschaft">Der Name der Eigenschaft.</param>
        /// <param name="alterWert">Der alte Wert der Eigenschaft.</param>
        /// <param name="neuerWert">Der neue Wert der Eigenschaft.</param>
        /// <returns>Gesetzt, wenn die Eigenschaft verändert wurde.</returns>
        public static bool EigenschaftVerändern<TArtDerEigenschaft>( this PropertyChangedEventHandler interessenten, INotifyPropertyChanged quelle, string nameDerEigenschaft, ref TArtDerEigenschaft alterWert, TArtDerEigenschaft neuerWert )
        {
            // Es hat sich nichts verändert
            if (Equals( alterWert, neuerWert ))
                return false;

            // Änderung durchführen
            alterWert = neuerWert;

            // Melden
            interessenten.EigenschaftWurdeVerändert( quelle, nameDerEigenschaft );

            // Wir haben was getan
            return true;
        }
    }
}
