using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Database.ORM;

namespace Vow_win_skiUWP.Log
{
    class Reporter
    {
        public static void Report(Exception e)
        {
            Console.WriteLine(e.Message + " " + e.StackTrace);
        }

        public static void ReportLogToDatabase(Database.ORM.Entities.Log log)
        {
            Queries.InsertEntity(log);
        }

        public static void ReportLogToConsole(Database.ORM.Entities.Log log)
        {
            Console.WriteLine(log.ToString());
        }

        public static void ReportExceptionToDatabase(Database.ORM.Entities.SystemExceptions ex)
        {
            Queries.InsertEntity(ex);
        }

        public static void ReportExceptionToConsole(Database.ORM.Entities.SystemExceptions ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
