using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Markup;


namespace JMS.JnRV2.Ablage
{
    /// <summary>
    /// Hilfsklasse zum Nachladen von Dateien.
    /// </summary>
    public static class DateiLader
    {
        /// <summary>
        /// Das Namensschema für die Dateien, in denen die SPielfelder beschrieben sind.
        /// </summary>
        private const string FormatFürDateinamenVonSpielfeldern = "Spielfelder/Level{0:00}";

        /// <summary>
        /// Wird gesetzt, sobald die allgemeine Konfiguration geladen wurde.
        /// </summary>
        private static bool m_konfigurationGeladen;

        /// <summary>
        /// Ermittelt den Inhalt einer XAML Datei.
        /// </summary>
        /// <typeparam name="TNeuerDatentyp">Die Art des Objektes.</typeparam>
        /// <typeparam name="TAlterDatentyp">Die Art des Objektes in einer älteren Spielversion.</typeparam>
        /// <param name="relativerPfad">Der Pfad der Datei relativ zur Anwendung.</param>
        /// <param name="objectVerfügbar">Wird aufgerufen, sobald das Objekt geladen wurde.</param>
        private static void ObjektAusXamlDatei<TNeuerDatentyp, TAlterDatentyp>( string relativerPfad, Action<TNeuerDatentyp> objectVerfügbar )
            where TAlterDatentyp : class
            where TNeuerDatentyp : class
        {
            // Prüfen
            if (string.IsNullOrEmpty( relativerPfad ))
                throw new ArgumentNullException( "relativerPfad" );
            if (objectVerfügbar == null)
                throw new ArgumentNullException( "objectVerfügbar" );

            // Rekostruieren
            TextDatei( relativerPfad + ".xaml", inhalt =>
                {
                    // Sichere Umgebung
                    try
                    {
                        // Schon falsch
                        if (inhalt == null)
                        {
                            // Das müssen wir auf der primären Umgebung machen
                            Deployment.Current.Dispatcher.BeginInvoke( objectVerfügbar, default( TAlterDatentyp ) );

                            // Aufhören
                            return;
                        }

                        // Namensräume wandeln
                        var aktuellerInhalt =
                            inhalt
                                .Replace( "clr-namespace:Jump_And_Run_First_Try.ViewModels;assembly=JumpAndRunFirstTry", "clr-namespace:JMS.JnRV2.Ablage.V1;assembly=GamePersistence" )
                                .Replace( "clr-namespace:Jump_And_Run_First_Try.Manipulators;assembly=JumpAndRunFirstTry", "clr-namespace:JMS.JnRV2.Ablage.V1;assembly=GamePersistence" );

                        // Das müssen wir auf der primären Umgebung machen
                        Deployment.Current.Dispatcher.BeginInvoke( () =>
                            {
                                // Noch einmal in sicher
                                try
                                {
                                    // Machen wir mal ein Objekt
                                    var objekt = XamlReader.Load( aktuellerInhalt );

                                    // Das müssen wir wandeln
                                    var objektInAlterDarstellung = objekt as TAlterDatentyp;
                                    if (objektInAlterDarstellung != null)
                                        if (typeof( TAlterDatentyp ) != typeof( TNeuerDatentyp ))
                                            objekt = Activator.CreateInstance( typeof( TNeuerDatentyp ), objekt );

                                    // Meldung auslösen
                                    objectVerfügbar( (TNeuerDatentyp) objekt );
                                }
                                catch (Exception)
                                {
                                    // Ups, das war wohl nichts
                                    objectVerfügbar( default( TNeuerDatentyp ) );
                                }
                            } );
                    }
                    catch (Exception)
                    {
                        // Das müssen wir auf der primären Umgebung machen
                        Deployment.Current.Dispatcher.BeginInvoke( objectVerfügbar, default( TNeuerDatentyp ) );
                    }
                } );
        }

        /// <summary>
        /// Ermittelt den Inhalt einer Textdatei.
        /// </summary>
        /// <param name="relativerPfad">Der Pfad der Datei relativ zur Anwendung.</param>
        /// <param name="inhaltVerfügbar">Wird aufgerufen, sobald der Inhalt der Datei verfügbar ist.</param>
        private static void TextDatei( string relativerPfad, Action<string> inhaltVerfügbar )
        {
            // Prüfen
            if (string.IsNullOrEmpty( relativerPfad ))
                throw new ArgumentNullException( "relativerPfad" );
            if (inhaltVerfügbar == null)
                throw new ArgumentNullException( "inhaltVerfügbar" );

            // Alle Fehler abfangen
            try
            {
                // Ladevorgang vorbereiten
                var uri = new Uri( Application.Current.Host.Source, string.Format( "../{0}", relativerPfad ) );
                var request = (HttpWebRequest) HttpWebRequest.Create( uri );

                // Ladevorgang auslösen
                request.BeginGetResponse( responseInfo =>
                    {
                        // Alle Fehler abfangen
                        try
                        {
                            // Ergebnis auslesen
                            var originalRequestForResponse = (HttpWebRequest) responseInfo.AsyncState;
                            var response = (HttpWebResponse) originalRequestForResponse.EndGetResponse( responseInfo );

                            // Fehler prüfen
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                // Leider müssen wir prüfen, ob wir auch die Datei bekommen haben, die wir angefordert haben (Apache Web Server)
                                var responseUri = response.ResponseUri;
                                var requestedFile = uri.GetComponents( UriComponents.Path, UriFormat.UriEscaped );
                                var gotFile = responseUri.GetComponents( UriComponents.Path, UriFormat.UriEscaped );
                                if (string.Equals( requestedFile, gotFile ))
                                {
                                    // Vorkonfiguration aus Datei laden
                                    string inhalt;

                                    // Data auslesen
                                    using (var content = response.GetResponseStream())
                                    using (var reader = new StreamReader( content ))
                                        inhalt = reader.ReadToEnd();

                                    // Beenden
                                    inhaltVerfügbar( inhalt );

                                    // Fertig
                                    return;
                                }
                            }

                            // Vergessen wir lieber einmal
                            inhaltVerfügbar( null );
                        }
                        catch
                        {
                            // Abschliessen
                            inhaltVerfügbar( null );
                        }
                    }, request );
            }
            catch
            {
                // Abschliessen
                inhaltVerfügbar( null );
            }
        }

        /// <summary>
        /// Lädt eine Liste von Elementen.
        /// </summary>
        /// <typeparam name="TAlterDatentyp">Die ursprünglice Konfiguration.</typeparam>
        /// <typeparam name="TNeuerDatentyp">Die aktuelle Konfiguration.</typeparam>
        /// <param name="formatFürDateinamen">Das Format für die Dateinamen.</param>
        /// <param name="allesGeladen">Wird aufgerufen, wenn der Ladevorgang abgeschlossen wurde.</param>
        private static void ListeLaden<TNeuerDatentyp, TAlterDatentyp>( string formatFürDateinamen, Action<TNeuerDatentyp[]> allesGeladen )
            where TAlterDatentyp : class
            where TNeuerDatentyp : class
        {
            // Prüfen
            if (allesGeladen == null)
                throw new ArgumentNullException( "allesGeladen" );

            // Die Liste aller Figuren
            var liste = new List<TNeuerDatentyp>();

            // Ein einzelner Ladeschritt
            Action laden = null;

            // Algorithmus definieren
            laden = () => ObjektAusXamlDatei<TNeuerDatentyp, TAlterDatentyp>( string.Format( formatFürDateinamen, liste.Count + 1 ),
                geladen =>
                {
                    // Schauen wir mal, ob wir eine erhalten haben
                    if (geladen == null)
                    {
                        // Benachrichtigung auslösen
                        allesGeladen( liste.ToArray() );
                    }
                    else
                    {
                        // Merken
                        liste.Add( geladen );

                        // Ab mit der nächsten
                        laden();
                    }
                } );

            // Und los geht es
            laden();
        }

        /// <summary>
        /// Lädt alle Spielfiguren.
        /// </summary>
        /// <param name="alleFigurenGeladen">Wird aufgerufen, sobald alle Spielfiguren geladen sind.</param>
        public static void SpielfigurenLaden( Action<Spielfigur[]> alleFigurenGeladen )
        {
            // Durchreichen
            ListeLaden<Spielfigur, V1.Figur>( "Spielfiguren/Spieler{0:00}", alleFigurenGeladen );
        }

        /// <summary>
        /// Lädt alle Spielfelder.
        /// </summary>
        /// <param name="alleFelderGeladen">Wird aufgerufen, sobald alle Spielfelder bereit stehen.</param>
        public static void SpielfelderLaden( Action<Spielfeld[]> alleFelderGeladen )
        {
            // Durchreichen
            KonfigurationLaden( () => ListeLaden<Spielfeld, V1.Spielfeld>( FormatFürDateinamenVonSpielfeldern, alleFelderGeladen ) );
        }

        /// <summary>
        /// Lädt ein einzelnes Spiel und das damit verbundene Spielfeld.
        /// </summary>
        /// <param name="nameDesSpiels">Der Name des Spiels.</param>
        /// <param name="spielGeladen">Wird aufgerufen, sobald das Spiel geladen wurde.</param>
        public static void SpielLaden( string nameDesSpiels, Action<Spiel> spielGeladen )
        {
            // Prüfen
            if (string.IsNullOrEmpty( nameDesSpiels ))
                throw new ArgumentNullException( "nameDesSpiels" );
            if (spielGeladen == null)
                throw new ArgumentNullException( "spielGeladen" );

            // Das passiert im Hintergrund
            ObjektAusXamlDatei<Spiel, V1.Spiel>( string.Format( "Spiele/{0}", nameDesSpiels ), spielGeladen );
        }

        /// <summary>
        /// Lädt die übergreifende Konfiguration.
        /// </summary>
        /// <param name="konfigurationWurdeGeladen">Wird aufgerufen, wenn die Konfiguration bereit steht.</param>
        private static void KonfigurationLaden( Action konfigurationWurdeGeladen )
        {
            // Direkt oder mit Verzögerung
            if (m_konfigurationGeladen)
            {
                // Sofort aufrufen
                konfigurationWurdeGeladen();
            }
            else
            {
                // Erst einmal die Konfiguration laden
                ObjektAusXamlDatei<ResourceDictionary, ResourceDictionary>( "Spielfelder/Common", konfiguration =>
                {
                    // Einmischen             
                    if (m_konfigurationGeladen = (konfiguration != null))
                        Application.Current.Resources.MergedDictionaries.Add( konfiguration );

                    // Endlich können wir loslegen
                    konfigurationWurdeGeladen();
                } );
            }
        }
    }
}
