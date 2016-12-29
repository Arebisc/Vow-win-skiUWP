using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Vow_win_skiUWP.Log;

namespace Vow_win_skiUWP.Database
{
    public class ConnectionHolder
    {
        private static readonly object SyncRoot = new object();
        private static volatile SQLiteConnection _connectionHolder;

        private const string DbName = "Vow-Win-Ski.db";
        private static readonly string DbPath = Path.Combine(Windows.Storage.ApplicationData.Current.RoamingFolder.Path, DbName);

        private ConnectionHolder() { }

        public static SQLiteConnection GetConnectionHolder
        {
            get
            {
                if (!ConnectionExists())
                {
                    lock (SyncRoot)
                    {
                        if (!ConnectionExists())
                            InitDatabaseConnection();
                    }
                }
                return _connectionHolder;
            }
        }

        private static bool ConnectionExists()
        {
            if (_connectionHolder != null)
                return true;
            return false;
        }

        private static void InitDatabaseConnection()
        {
            try
            {
                _connectionHolder = new SQLiteConnection(DbPath);
            }
            catch (Exception e)
            {
                Reporter.Report(e);
            }
        }

        public static void CloseConnection()
        {
            if (ConnectionExists())
            {
                _connectionHolder.Close();
                _connectionHolder.Dispose();
            }
        }
    }
}
