﻿using System;
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
            band.city = newBandCity.Text.Any() ? newBandCity.Text : "Nevyplneno";
            band.description = new TextRange(bandDescription.Document.ContentStart, bandDescription.Document.ContentEnd).Text;
            band.facebook = facebook.Text.Contains("facebook".ToUpper()) ? facebook.Text : "Kapela nemá Facebook" ;
            band.banzone = bandzone.Text.Contains("banzone".ToUpper()) ? bandzone.Text : "Kapela nemá Bandzone";
            band.website = website.Text.Contains("www.") ? website.Text : "Kapela nemá webovou stránku";
            band.style = newBandStyle.Text.Any() ? newBandStyle.Text : "Nedefinován styl!";
            band.personalNote = new TextRange(personalNote.Document.ContentStart, personalNote.Document.ContentEnd).Text;

            if (band.checkIfBandExists(band))
            {
                MessageBox.Show("Kapela s tímto názvem již existuje!");
            }
            else
            {
                band.saveBandToDb(band);
                if (contactName.Text.Any() && contactName.Text != "Jmeno")
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
        }

        private void back(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new BandsAdmin());
        }

        private void focusOnContactName(object sender, RoutedEventArgs e)
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

        private void focusOn(object sender, RoutedEventArgs e)
        {

        }
    }
}
