using System.ComponentModel;
using JMS.JnRV2.Anzeige.Praesentation;


namespace JMS.JnRV2.Start
{
    /// <summary>
    /// Dieses Fenster steuert die Auswahl des Spiels.
    /// </summary>
    partial class SpielAuswahl
    {
        /// <summary>
        /// Erzeugt ein neues Auswahlfenster.
        /// </summary>
        public SpielAuswahl()
        {
            // Konfiguration (XAML) laden
            InitializeComponent();

            // Änderungen anmelden
            var auswahl = DataContext as AuswahlInformationen;
            if (auswahl != null)
                auswahl.PropertyChanged += InformationWurdeVerändert;
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich etwas an der Auswahl verändert hat.
        /// </summary>
        /// <param name="sender">Die aktuelle Information.</param>
        /// <param name="e">Beschreibt die Veränderung.</param>
        private void InformationWurdeVerändert( object sender, PropertyChangedEventArgs e )
        {
            // Mal schauen, ob uns das interessiert
            if (!AuswahlInformationen._AktuellesSpiel.Equals( e.PropertyName ))
                return;

            // Oberfläche anpassen
            m_inhalt.Content = new SpielAnzeige { DataContext = ((AuswahlInformationen) sender).AktuellesSpiel };
        }
    }
}
