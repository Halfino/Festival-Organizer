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
    /// Interakční logika pro NewBand.xaml
    /// </summary>
    public partial class NewBand : Page
    {
        public NewBand()
        {
            InitializeComponent();
        }

        private void newBand(object sender, RoutedEventArgs e)
        {
            Band band = new Band();
            band.name = newBandName.Text;
            band.city = newBandCity.Text;
            band.style = newBandStyle.Text;
            band.description = new TextRange(bandDescription.Document.ContentStart, bandDescription.Document.ContentEnd).Text;
            band.facebook = facebook.Text;
            band.banzone = bandzone.Text;
            band.website = website.Text;
            band.personalNote = new TextRange(personalNote.Document.ContentStart, personalNote.Document.ContentEnd).Text;
            band.members = new TextRange(bandMembers.Document.ContentStart, bandMembers.Document.ContentEnd).Text;

            band.saveBandToDb(band);

            if (contactName != null)
            {
                BandContact contact = new BandContact();
                contact.fName = contactName.Text;
                contact.lName = ContactSurname.Text;
                contact.function = contactFunction.Text;
                contact.phone = contactPhone.Text;
                contact.email = contactEmail.Text;
                contact.bandId = band._id;

                contact.saveContactToDb(contact);
            }

            this.NavigationService.Navigate(new BandsAdmin());
        }

        private void back(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new mainPage());
        }
    }
}
