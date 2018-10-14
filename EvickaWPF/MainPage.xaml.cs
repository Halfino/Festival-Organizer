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
            String whatIsDone = "Hotovo: vytvoreni kapely, Smazani kapely, editace kapely, potvrzeni ke smazani, pridani a mazani kontaktu, vyhledavani kapel, PDF export vsech kapel." + 
                Environment.NewLine + "Potreba udelat: Editace kontaktu? festivaly, ukoly, predelat PDF export na lepsi format";
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
