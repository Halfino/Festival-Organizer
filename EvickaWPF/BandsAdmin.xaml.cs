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
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using LiteDB;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using iText.Layout.Element;
using Table = iText.Layout.Element.Table;
using iText.Layout.Properties;
using iText.IO.Util;
using System.Windows.Markup;
using System.Xml.Linq;
using Organizer;
using Microsoft.Win32;

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
                using (var db = new LiteDatabase(LiteDbConnection.getDbName()))
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

            if (band == null)
            {
                MessageBox.Show("Není vybrána kapela!");
                this.NavigationService.Navigate(new BandsAdmin());
            }
            else
            {
                this.NavigationService.Navigate(new BandDetail(band));
            }

        }

        /// <summary>
        /// Mazani kapely.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteBandButton_Click(object sender, RoutedEventArgs e)
        {
            object selectedRow = bandListView.SelectedItem;
            Band band = (Band)selectedRow;
            if (band != null)
            {
                try
                {
                    using (var db = new LiteDatabase(LiteDbConnection.getDbName()))
                    {
                        MessageBoxResult myResult;
                        myResult = MessageBox.Show("Opravdu chcete smazat kapelu " + band.name + " ?", "Delete Confirmation", MessageBoxButton.OKCancel);
                        if (myResult == MessageBoxResult.OK)
                        {
                            var bands = db.GetCollection<Band>("Bands");
                            var contacts = db.GetCollection<BandContact>("BandContacts");
                            List<BandContact> contactsQuery = contacts.FindAll().ToList();
                            List<BandContact> contactsToDelete = contactsQuery.FindAll(delegate (BandContact bk)
                            {
                                return bk.bandId == band._id;
                            });
                            foreach (BandContact contact in contactsToDelete)
                            {
                                contacts.Delete(contact._id);
                            }
                            bands.Delete(band._id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Není vybrána kapela!");
            }

            this.NavigationService.Navigate(new BandsAdmin());
        }

        private void exitButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Export data tables to PDF.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportPDF(object sender, RoutedEventArgs e)
        {

            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "PDF(*.pdf)|*.pdf";
                dialog.ShowDialog();
                string path = dialog.FileName;



            List<Band> bands;
            ArrayList bandsToPdf = new ArrayList();

            using (var db = new LiteDatabase(LiteDbConnection.getDbName()))
            {
                var dbBands = db.GetCollection<Band>("Bands");
                bands = dbBands.FindAll().ToList();

                foreach (Band bandToProcess in bands)
                {
                    bandsToPdf.Add(bandToProcess);
                }

                PdfExport.processBandsToPdf(bandsToPdf, path, 12);
            }
            }
            catch(Exception ex)
            {
                this.NavigationService.Navigate(new BandsAdmin());          
            }

        }



        private void textChanged(object sender, TextChangedEventArgs e)
        {
            string searching = searchBox.Text.ToUpper();
            using (var db = new LiteDatabase(LiteDbConnection.getDbName()))
            {
                var bands = db.GetCollection<Band>("Bands");
                var queryBands = bands.FindAll();
                if (searching != null)
                {
                    List<Band> findBands = new List<Band>();
                    foreach (Band theBand in queryBands)
                    {
                        if (theBand.name.ToUpper().Contains(searching))
                        {
                            findBands.Add(theBand);
                        }
                    }
                    var bandList = findBands.OrderBy(x => x.name);
                    bandListView.ItemsSource = bandList;
                }

                else
                {
                    var allBandList = queryBands.OrderBy(x => x.name).ToList();
                    bandListView.ItemsSource = allBandList;
                }

            }
        }

        private void pdfExportSearchedClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PDF(*.pdf)|*.pdf";
            dialog.ShowDialog();
            string path = dialog.FileName;

            ArrayList bandsToPdf = new ArrayList();
            var queryBands = bandListView.ItemsSource;
            foreach (var band in queryBands)
            {
                Band bandToProcess = (Band)band;
                bandsToPdf.Add(band);
            }
            try
            {
                PdfExport.processBandsToPdf(bandsToPdf, path, 12);
            }
            catch
            {
                this.NavigationService.Navigate(new BandsAdmin());
            }
           
        }
    }
}
