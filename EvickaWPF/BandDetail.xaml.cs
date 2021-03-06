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
                using (var db = new LiteDatabase(LiteDbConnection.getDbName()))
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
                string description = new TextRange(bandDescription.Document.ContentStart, bandDescription.Document.ContentEnd).Text;
                bandDetail.name = bandName.Text;
                bandDetail.city = bandCity.Text;
                bandDetail.banzone = bandBandzone.Text;
                bandDetail.description = description;
                bandDetail.facebook = bandFacebook.Text;
                bandDetail.style = bandStyle.Text;
                bandDetail.website = bandWeb.Text;
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

        private void detailPageLoaded(object sender, RoutedEventArgs e)
        {
            detailHeader.Content = "Detail kapely " + bandDetail.name;
            bandName.Text = bandDetail.name;
            bandCity.Text = bandDetail.city;
            bandStyle.Text = bandDetail.style;
            bandDescription.Document.Blocks.Clear();
            bandDescription.Document.Blocks.Add(new Paragraph(new Run(bandDetail.description)));
            bandFacebook.Text = (bandDetail.facebook != null) ? bandDetail.facebook : "Kapela nemá Facebook" ;
            bandBandzone.Text = (bandDetail.banzone != null) ? bandDetail.banzone : "Kapela nemá Bandzone";
            bandWeb.Text = (bandDetail.website != null) ? bandDetail.website : "Kapela nemá webovou stránku";
        }

        private void deleteContactButtonClick(object sender, RoutedEventArgs e)
        {
            BandContact contactToDelete = (BandContact) contactListView.SelectedItem;
            if (contactToDelete != null)
            {
                try
                {
                    using (var db = new LiteDatabase(LiteDbConnection.getDbName()))
                    {
                        var contacts = db.GetCollection<BandContact>("BandContacts");
                        MessageBoxResult myResult;
                        myResult = MessageBox.Show("Opravdu chcete smazat kontakt " + contactToDelete.fName +" " + 
                            contactToDelete.lName + " ?", "Delete Confirmation", MessageBoxButton.OKCancel);
                        if (myResult == MessageBoxResult.OK)
                        {
                            contacts.Delete(contactToDelete._id);
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
                MessageBox.Show("Není vybrán kontakt!");
            }

            this.NavigationService.Navigate(new BandDetail(bandDetail));

        }

        private void addContactButtonClick(object sender, RoutedEventArgs e)
        {
            int bandId = bandDetail._id;
            this.NavigationService.Navigate(new AddContact(bandId));
            
        }
    }
}
