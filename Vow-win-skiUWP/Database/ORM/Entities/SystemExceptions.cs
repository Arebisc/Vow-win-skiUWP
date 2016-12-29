using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Vow_win_skiUWP.Database.ORM.Entities
{
    public class SystemExceptions : Entity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public long Date { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public SystemExceptions() { }

        public SystemExceptions(long date, string message, string stackTrace)
            :this()
        {
            this.Date = date;
            this.Message = message;
            this.StackTrace = stackTrace;
        }

        public SystemExceptions(int id, long date, string message, string stackTrace)
            :this(date, message, stackTrace)
        {
            this.Id = id;
        }

        public override string ToString()
        {
            return (Id + " " + new DateTime(Date) + " " + Message + " " + StackTrace);
        }
    }
}
