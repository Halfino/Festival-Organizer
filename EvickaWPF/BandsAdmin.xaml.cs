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
using iText.Layout;
using iText.Layout.Element;
using Table = iText.Layout.Element.Table;
using iText.Layout.Properties;
using iText.IO.Util;
using System.Windows.Markup;

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
                        var bands = db.GetCollection<Band>("Bands");
                        MessageBoxResult myResult;
                        myResult = MessageBox.Show("Opravdu chcete smazat kapelu " + band.name + " ?", "Delete Confirmation", MessageBoxButton.OKCancel);
                        if (myResult == MessageBoxResult.OK)
                        {
                            bands.Delete(band._id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
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
            //prepare PDF document
            PdfWriter writer = new PdfWriter("testPDFs/testPDF.pdf");
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4.Rotate());
            document.SetMargins(20, 20, 20, 20);
            PdfFont font = PdfFontFactory.CreateFont(FontConstants.HELVETICA);
            PdfFont bold = PdfFontFactory.CreateFont(FontConstants.HELVETICA_BOLD);

            //set number of table columns and their width relatiuve to each other (that works weird, changing values has no affection)
            Table table = new Table(new float[] { 1, 1, 1, 1, 1, 1, 1, 1 });
            //table width related to page
            table.SetWidth(UnitValue.CreatePercentValue(100));

            string line;
            List<string> lines = new List<string>();
            using (var db = new LiteDatabase(LiteDbConnection.getDbName()))
            {
                // prepare data to strings for PDF creating
                var bands = db.GetCollection<Band>("Bands");
                var queryBands = bands.FindAll();
                var sortedBands = queryBands.OrderBy(x => x.name);
                foreach (var band in sortedBands)
                {
                    Band bandToProcess = (Band)band;
                    line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", band.name, band.city, band.style, band.facebook, band.banzone, band.website, band.members, band.description);
                    lines.Add(line);
                }
            }
            string headerLine = "Jméno, Mesto, Žánr, Facebook, Bandzone, Website, Složení, Popis";
            //process headerLine
            process(table, headerLine, bold, true);
            //process data rows into table
            foreach (String tableLine in lines)
            {
                process(table, tableLine, font, false);
            }

            document.Add(table);
            document.Close();
        }

        /// <summary>
        /// Process table row for PDF export
        /// </summary>
        /// <param name="table"></param>
        /// <param name="line"></param>
        /// <param name="font"></param>
        /// <param name="isHeader"></param>
        private void process(Table table, String line, PdfFont font, Boolean isHeader)
        {
            StringTokenizer tokenizer = new StringTokenizer(line, ",");

            while (tokenizer.HasMoreTokens())
            {
                if (isHeader)
                {
                    table.AddHeaderCell(new Cell().Add(new iText.Layout.Element.Paragraph(tokenizer.NextToken()).SetFont(font)));
                }
                else
                {
                    table.AddCell(new Cell().Add(new iText.Layout.Element.Paragraph(tokenizer.NextToken()).SetFont(font)));
                }

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
            //prepare PDF document
            PdfWriter writer = new PdfWriter("testPDFs/VyhledaneKapely.pdf");
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4.Rotate());
            document.SetMargins(20, 20, 20, 20);
            PdfFont font = PdfFontFactory.CreateFont(FontConstants.HELVETICA);
            PdfFont bold = PdfFontFactory.CreateFont(FontConstants.HELVETICA_BOLD);

            //set number of table columns and their width relatiuve to each other (that works weird, changing values has no affection)
            Table table = new Table(new float[] { 1, 1, 1, 1, 1, 1, 1, 1 });
            //table width related to page
            table.SetWidth(UnitValue.CreatePercentValue(100));

            string line;
            List<string> lines = new List<string>();
            using (var db = new LiteDatabase(LiteDbConnection.getDbName()))
            {
                // prepare data to strings for PDF creating
                //var bands = db.GetCollection<Band>("Bands");
                //var queryBands = bands.FindAll();
                var queryBands = bandListView.ItemsSource;
                //var sortedBands = queryBands.OrderBy(x => x.name);
                foreach (var band in queryBands)
                {
                    Band bandToProcess = (Band)band;
                    line = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                        bandToProcess.name,
                        bandToProcess.city,
                        bandToProcess.style,
                        bandToProcess.facebook,
                        bandToProcess.banzone,
                        bandToProcess.website,
                        bandToProcess.members,
                        bandToProcess.description);
                    lines.Add(line);
                }
            }
            string headerLine = "Jméno, Mesto, Žánr, Facebook, Bandzone, Website, Složení, Popis";
            //process headerLine
            process(table, headerLine, bold, true);
            //process data rows into table
            foreach (String tableLine in lines)
            {
                process(table, tableLine, font, false);
            }

            document.Add(table);
            document.Close();
        }
    }

    }
