using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace EvickaWPF
{
    class BandContact
    {
        public int _id { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string function { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int bandId { get; set; }

        public BandContact() { }

        public void saveContactToDb(BandContact contact)
        {
            try
            {
                using (var db = new LiteDatabase(@"EvaDB.db"))
                {
                    var contacts = db.GetCollection<BandContact>("BandContacts");
                    contacts.Insert(contact);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

    }
}
