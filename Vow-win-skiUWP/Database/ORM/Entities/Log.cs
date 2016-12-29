using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Vow_win_skiUWP.Database.ORM.Entities
{
    public class Log : Entity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public long Date { get; set; }
        public string Message { get; set; }

        public Log() { }

        public Log(long date, string message)
            :this()
        {
            this.Date = date;
            this.Message = message;
        }

        public Log(int id, long date, string message)
            :this(date, message)
        {
            this.Id = Id;
        }

        public override string ToString()
        {
            return (Id + " " + new DateTime(Date) + " " + Message);
        }
    }
}
