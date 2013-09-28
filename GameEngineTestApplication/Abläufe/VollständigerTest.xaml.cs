using System;
using JMS.JnRV2.Ablauf.Animationen;
using JMS.JnRV2.Ablauf.Tests;


namespace JMS.JnRV2.Ablauf.Tests.Abläufe
{
    /// <summary>
    /// Repräsentiert die Startseite der Testanwendung.
    /// </summary>
    partial class VollständigerTest
    {
        /// <summary>
        /// Erzeugt eine neue Startseite.
        /// </summary>
        public VollständigerTest()
        {
            // Konfiguration (XAML) laden
            InitializeComponent();

            // Spielfeld erzeugen
            var feld = new SpielfeldTestViewModel();

            // Bewegliche Platte
            var platte = new Element( Position.Erzeugen( Ablauf.GenaueZahl.Eins * 0.7m, Ablauf.GenaueZahl.Eins * 0.8m ), Ausdehnung.Erzeugen( Ablauf.GenaueZahl.Eins * 0.2m, Ablauf.GenaueZahl.Eins * 0.01m ) );

            // Geschwindigkeit festlegen
            platte.GeschwindigkeitsRegel.GeschwindigkeitErgänzen( Geschwindigkeit.Erzeugen( Ablauf.GenaueZahl.Eins * -0.02m, Ablauf.GenaueZahl.Null ) );

            // Mauer
            var mauer = new Element( Position.Erzeugen( Ablauf.GenaueZahl.Eins * 0.3m, Ablauf.GenaueZahl.Eins * 0.755m ), Ausdehnung.Erzeugen( Ablauf.GenaueZahl.Eins * 0.01m, Ablauf.GenaueZahl.Eins * 0.1m ) );

            // Regel festlegen
            mauer.KollisionsRegel.RegelAnmelden( KollisionsRegel.WennVomSpielerGetroffen, KollisionsRegel.PunkteAnDenSpielerÜbergeben( 1000 ) );
            mauer.KollisionsRegel.RegelAnmelden( KollisionsRegel.WennVomSpielerGetroffen, KollisionsRegel.LebenskraftDesSpielersÄndern( -250 ) );
            mauer.KollisionsRegel.RegelAnmelden( KollisionsRegel.WennVomSpielerGetroffen, KollisionsRegel.ElementDeaktivieren );

            // Bewegliches Element
            var hüpfer = new Element( Position.Erzeugen( Ablauf.GenaueZahl.Eins * 0.35m, Ablauf.GenaueZahl.Eins * 0.78m ), Ausdehnung.Erzeugen( Ablauf.GenaueZahl.Eins * 0.01m, Ablauf.GenaueZahl.Eins * 0.01m ) );

            // Regel anwenden
            FreieBewegung.Aktivieren( hüpfer,
                TemporaereGeschwindigkeit.Erzeugen( Ablauf.GenaueZahl.Null, Ablauf.GenaueZahl.Eins * -0.08m, TimeSpan.FromSeconds( 1 ) ),
                TemporaereGeschwindigkeit.Erzeugen( Ablauf.GenaueZahl.Eins * 0.05m, Ablauf.GenaueZahl.Null, TimeSpan.FromSeconds( 1 ) ),
                TemporaereGeschwindigkeit.Erzeugen( Ablauf.GenaueZahl.Null, Ablauf.GenaueZahl.Eins * 0.08m, TimeSpan.FromSeconds( 1 ) ),
                TemporaereGeschwindigkeit.Erzeugen( Ablauf.GenaueZahl.Eins * -0.05m, Ablauf.GenaueZahl.Null, TimeSpan.FromSeconds( 1 ) ) );

            // Spielfeld vorbereiten
            feld.Add( platte );
            feld.Add( new Element( Position.Erzeugen( Ablauf.GenaueZahl.Eins * 0.4m, Ablauf.GenaueZahl.Eins * 0.7m ), Ausdehnung.Erzeugen( Ablauf.GenaueZahl.Eins * 0.3m, Ablauf.GenaueZahl.Eins * 0.01m ) ) );
            feld.Add( mauer );
            feld.Add( hüpfer );
            feld.Add( new Element( Position.Erzeugen( Ablauf.GenaueZahl.Eins * 0.2m, Ablauf.GenaueZahl.Eins * 0.6m ), Ausdehnung.Erzeugen( Ablauf.GenaueZahl.Eins * 0.1m, Ablauf.GenaueZahl.Eins * 0.01m ) ) );
            feld.Add( new Element( Position.Erzeugen( Ablauf.GenaueZahl.Eins * 0.1m, Ablauf.GenaueZahl.Eins * 0.2m ), Ausdehnung.Erzeugen( Ablauf.GenaueZahl.Eins * 0.1m, Ablauf.GenaueZahl.Eins * 0.01m ) ) );
            feld.Add( new Element( Position.Erzeugen( Ablauf.GenaueZahl.Eins * 0.5m, Ablauf.GenaueZahl.Eins * 0.1m ), Ausdehnung.Erzeugen( Ablauf.GenaueZahl.Eins * 1.0m, Ablauf.GenaueZahl.Eins * 0.01m ) ) );

            // Spieler erzeugen
            var spieler = new Spieler( Position.Erzeugen( Ablauf.GenaueZahl.Eins * 0.8m, Ablauf.GenaueZahl.Eins * 0.9m ), Ausdehnung.Erzeugen( Ablauf.GenaueZahl.Eins * 0.01m, Ablauf.GenaueZahl.Eins * 0.01m ) );

            // Limit setzen
            spieler.LebenskraftÄndern( 10000 );

            // Spieler ergänzen
            feld.Add( spieler);

            // Spielfeld verwenden
            DataContext = feld;

            // Korrekt beenden
            Unloaded += ( s, a ) => feld.BeendenBefehl.Execute( null );
        }
    }
}
