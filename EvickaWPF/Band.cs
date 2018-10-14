using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using System.Configuration;

namespace EvickaWPF
{
    class Band
    {
        public int _id { get; set; }
        public string name { get; set;}
        public string city { get; set; }
        public string style { get; set; }
        public string description { get; set; }
        public string members { get; set; }
        public string facebook { get; set; }
        public string banzone { get; set; }
        public string website { get; set; }
        public string personalNote { get; set; }



        public Band()
        {
        }

        public void saveBandToDb(Band band)
        {
            try
            {
                using (var db = new LiteDatabase(LiteDbConnection.getDbName()))
                {
                    var bands = db.GetCollection<Band>("Bands");
                    bands.Upsert(band);
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
