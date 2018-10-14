using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using System.Configuration;

namespace EvickaWPF
{
    public abstract class LiteDbConnection
    {
     
        public static string getDbName()
        {
            string dbName = @"" + ConfigurationSettings.AppSettings["DATABASE"];
            return dbName;
        }



    }
}
