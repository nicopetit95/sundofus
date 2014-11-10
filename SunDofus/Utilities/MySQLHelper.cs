using MySql.Data.MySqlClient;
using SunDofus.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunDofus.Utilities
{
    class MySQLHelper
    {
        private MySqlConnection m_Connection;
        public MySqlTransaction Transaction { get; private set; }

        public MySQLHelper()
        {
            m_Connection = new MySqlConnection();

            m_Connection.ConnectionString = string.Format("server={0};uid={1};pwd='{2}';database={3}",
                    Program.Config.DBServer, Program.Config.DBUser,
                    Program.Config.DBPass, Program.Config.DBName);

            m_Connection.Open();

            Utilities.Loggers.Status.Write("Connected to the database");
        }

        public MySqlConnection Use()
        {
            if (m_Connection.State == System.Data.ConnectionState.Broken || m_Connection.State == System.Data.ConnectionState.Closed)
                m_Connection.Open();

            return m_Connection;
        }

        public void Close()
        {
            m_Connection.Close();
        }

        public void BeginTransaction()
        {
            Transaction = Use().BeginTransaction();
        }

        public bool Commit()
        {
            var result = false;

            try
            {
                Transaction.Commit();
                Transaction.Dispose();

                result = true;
            }
            catch
            {
                Transaction.Rollback();
                Transaction.Dispose();
            }
            finally
            {
                Transaction = null;
            }

            return result;
        }

        public bool RollBack()
        {
            var result = false;

            try
            {
                Transaction.Rollback();
                Transaction.Dispose();
                Transaction = null;

                result = true;
            }
            catch { }

            return result;
        }
    }
}
