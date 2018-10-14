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
using LiteDB;

namespace EvickaWPF
{
    /// <summary>
    /// Interakční logika pro BandsAdmin.xaml
    /// </summary>
    public partial class BandsAdmin : Page
    {
        public BandsAdmin()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Naplneni bandlistu datama z DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bandListInitialize(object sender, EventArgs e)
        {

            try
            {
                using (var db = new LiteDatabase(@"EvaDB.db"))
                {
                    var bands = db.GetCollection<Band>("Bands");
                    var queryBands = bands.FindAll();
                    var bandList = queryBands.OrderBy(x => x.name).ToList();
                    bandListView.ItemsSource = bandList;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Tlacitko nova kapela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newBand(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new NewBand());
            
        }

        /// <summary>
        /// Tlacitko edit a detail kapely. Posila mi vybranou kapelu do dalsi stranky. Nutnost Try catch na nevybranou kapelu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editBandButton_Click(object sender, RoutedEventArgs e)
        {
            object selectedRow = bandListView.SelectedItem;
            Band band = (Band)selectedRow;

            if(band == null)
            {
                MessageBox.Show("Nevybral jsi kapelu na detial");
                this.NavigationService.Navigate(new BandsAdmin());
            }
            else
            {
                this.NavigationService.Navigate(new BandDetail(band));
            }

        }

        /// <summary>
        /// Mazani kapely, nice to have potvrzeni pred smazanim.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteBandButton_Click(object sender, RoutedEventArgs e)
        {
            object selectedRow = bandListView.SelectedItem;
            Band band = (Band)selectedRow;
            if(band != null)
            {
                try
                {
                    using (var db = new LiteDatabase(@"EvaDB.db"))
                    {
                        var bands = db.GetCollection<Band>("Bands");
                        bands.Delete(band._id);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Nevybral jsi kapelu ke smazani");
            }

            this.NavigationService.Navigate(new BandsAdmin());
        }

        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
    
}
