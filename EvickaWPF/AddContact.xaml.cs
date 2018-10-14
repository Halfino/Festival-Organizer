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
    /// Interakční logika pro AddContact.xaml
    /// </summary>
    public partial class AddContact : Page
    {
        int contactBandId;
        Band band;

        public AddContact()
        {
            InitializeComponent();
        }

        public AddContact(int bandId)
        {
            contactBandId = bandId;
            try
            {
                using (var db = new LiteDatabase(LiteDbConnection.getDbName()))
                {
                    var bands = db.GetCollection<Band>("Bands");
                    band = bands.FindById(contactBandId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            InitializeComponent();
        }

        private void detailLoaded(object sender, RoutedEventArgs e)
        {
            detailHeader.Content = "Nový kontakt ke kapele " + band.name;
        }

        private void newContact(object sender, RoutedEventArgs e)
        {
            BandContact newContact = new BandContact();
            newContact.fName = contactName.Text;
            newContact.lName = ContactSurname.Text;
            newContact.function = contactFunction.Text;
            newContact.phone = contactPhone.Text;
            newContact.email = contactEmail.Text;
            newContact.bandId = contactBandId;

            newContact.saveContactToDb(newContact);
            this.NavigationService.Navigate(new BandDetail(band));
        }

        private void back(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new BandDetail(band));
        }

        private void focusOn(object sender, RoutedEventArgs e)
        {
            contactName.Text = "";
        }

        private void focusOnSurname(object sender, RoutedEventArgs e)
        {
            ContactSurname.Text = "";
        }

        private void focusOnFunction(object sender, RoutedEventArgs e)
        {
            contactFunction.Text = "";
        }

        private void focusOnPhone(object sender, RoutedEventArgs e)
        {
            contactPhone.Text = "";
        }
    }
}
