using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EvickaWPF
{
    /// <summary>
    /// Interakční logika pro mainPage.xaml
    /// </summary>
    public partial class mainPage : Page
    {
        public mainPage()
        {
            InitializeComponent();
            String whatIsDone = "Hotovo: vytvoreni kapely, Smazani kapely, editace kapely, potvrzeni ke smazani." + 
                Environment.NewLine + "Potreba udelat: Pridani, editace a mazani vice kontaktu na kapele. Zatim se da pouze pridat jeden kontakt bez moznosti editace";
            MessageBox.Show(whatIsDone, "TODO");
        }

        /// <summary>
        /// Tlacitko na kapely
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bandListButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new BandsAdmin());
        }
    }
}
