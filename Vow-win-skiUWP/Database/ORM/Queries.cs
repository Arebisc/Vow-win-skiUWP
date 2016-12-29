using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Vow_win_skiUWP.Database.ORM.Entities;
using Vow_win_skiUWP.Log;

namespace Vow_win_skiUWP.Database.ORM
{
    public class Queries
    {
        public static void InsertEntity(Entity entity)
        {
            try
            {
                ConnectionHolder.GetConnectionHolder.Insert(entity);
            }
            catch (SQLiteException e)
            {
                Reporter.Report(e);
            }
        }

        public static TableQuery<Entities.Log> GetLogs()
        {
            try
            {
                return ConnectionHolder.GetConnectionHolder.Table<Entities.Log>();
            }
            catch (SQLiteException e)
            {
                Reporter.Report(e);
            }
            return default(TableQuery<Entities.Log>);
        }

        public static TableQuery<Entities.SystemExceptions> GetApplicationExceptions()
        {
            try
            {
                return ConnectionHolder.GetConnectionHolder.Table<Entities.SystemExceptions>();
            }
            catch (SQLiteException e)
            {
                Reporter.Report(e);
            }
            return default(TableQuery<Entities.SystemExceptions>);
        }

        public static void InitDatabaseTables()
        {
            try
            {
                ConnectionHolder.GetConnectionHolder.CreateTable<Entities.Log>();
                ConnectionHolder.GetConnectionHolder.CreateTable<SystemExceptions>();
            }
            catch (SQLiteException e)
            {
                Reporter.Report(e);
            }
        }
    }
}
