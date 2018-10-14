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
#pragma warning disable CS0618 // Type or member is obsolete
            string dbName = @"" + ConfigurationSettings.AppSettings["DATABASE"];
#pragma warning restore CS0618 // Type or member is obsolete
            return dbName;
        }



    }
}
