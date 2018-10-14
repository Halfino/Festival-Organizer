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
    /// Interakční logika pro BandDetail.xaml
    /// </summary>
    public partial class BandDetail : Page
    {
        Band bandDetail;
        public BandDetail()
        {
            InitializeComponent();
        }

        public BandDetail(object band):this()
        {
            bandDetail = (Band) band;
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new LiteDatabase(@"EvaDB.db"))
                {
                    var bands = db.GetCollection<Band>("Bands");
                    var contacts = db.GetCollection<BandContact>("BandContacts");
                    var queryContacts = contacts.Find(x=>x.bandId == bandDetail._id);
                    

                    contactListView.ItemsSource = queryContacts;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new BandsAdmin());
        }

        private void detailLoaded(object sender, RoutedEventArgs e)
        {
            detailHeader.Content = "Detail kapely " + bandDetail.name;
        }

        private void saveBandButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                    string personalBandNote = new TextRange(personalNote.Document.ContentStart, personalNote.Document.ContentEnd).Text;
                    bandDetail.name = "tak to zkusime";
                    bandDetail.personalNote = personalBandNote;
                    bandDetail.saveBandToDb(bandDetail);
                    this.NavigationService.Navigate(new BandDetail(bandDetail));                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.NavigationService.Navigate(new BandsAdmin());
            }
        }

        private void noteLoaded(object sender, RoutedEventArgs e)
        {
            personalNote.Document.Blocks.Clear();
            personalNote.Document.Blocks.Add(new Paragraph(new Run(bandDetail.personalNote)));
        }
    }
}
