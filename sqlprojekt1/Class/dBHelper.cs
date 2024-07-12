using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace sqlprojekt1.Class
{
    internal class dBHelper
    {
        // Declartion internal variables
        private SQLiteConnection m_connection = null;
        private string m_connectionString = "";
        private SQLiteDataAdapter m_dataAdapter = null;
        private DataSet m_dataSet = null;
        private string m_fieldNameID = "";
        // The DataSet is filled with the methode LoadDataSet
        public DataSet DataSet
        {
            get { return m_dataSet; }
        }
        // Constructor -> ConnectionString is required
        public dBHelper(string connectionString)
        {
            m_connectionString = connectionString;
        }
        // Load the DataSet
        public bool Load(string commandText, string fieldNameID)
        {
            // Save the variables
            m_fieldNameID = fieldNameID;
            try
            {
                // Open de connectie
                m_connection = new SQLiteConnection(m_connectionString);
                m_connection.Open();
                // Make a DataAdapter
                m_dataAdapter = new SQLiteDataAdapter(commandText,
               m_connection);
                // Link a eventhandler to the RowUpdated-event of the
                DataAdapter
                //m_dataAdapter.RowUpdated += new SqlRowUpdatedEventHandler
                (m_dataAdapter_RowUpdated);
                m_dataAdapter.RowUpdated += m_dataAdapter_RowUpdated;
                m_dataSet = new DataSet();
                // For a save --> create Commands
                if (!string.IsNullOrEmpty(fieldNameID))
                {
                    SQLiteCommandBuilder commandBuilder =
                   new SQLiteCommandBuilder(m_dataAdapter);
                    m_dataAdapter.InsertCommand =
                   commandBuilder.GetInsertCommand();
                    m_dataAdapter.DeleteCommand =
                   commandBuilder.GetDeleteCommand();
                    m_dataAdapter.UpdateCommand =
                   commandBuilder.GetUpdateCommand();
                }
                // Fill the DataSet
                m_dataAdapter.Fill(m_dataSet);
                // We're here, OK!
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Always close
                m_connection.Close();
            }
        }
        // Load the DataSet
        public bool Load(string commandText)
        {
            return Load(commandText, "");
        }
        // Save the DataSet
        public bool Save()
        {
            // Save is only posible if ID is known
            if (m_fieldNameID.Trim().Length == 0)
            {
                return false;
            }
            try
            {
                // Open the connection
                m_connection.Open();
                // Save the DataRow. This triggers an event OnRowUpdated
                m_dataAdapter.Update(m_dataSet);
                // We here, OK!
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Close
                m_connection.Close();
            }
        }
        // Save is only posible if ID is known
        void m_dataAdapter_RowUpdated(object sender,
        System.Data.Common.RowUpdatedEventArgs
       e)
        {
            // The (just receaved?) ID is only interesting with a new record
            if (e.StatementType == StatementType.Insert)
            {
                // Determin the just receaved ID
                SQLiteCommand command = new SQLiteCommand
                ("SELECT last_insert_rowid() AS ID",
               m_connection);

                // Get the new ID and Save in the according field
                object newID = command.ExecuteScalar();
                // BIf errors then no ID --> thus testing required
                if (newID == System.DBNull.Value == false)
                {
                    // Put the ID in the DataRow
                    e.Row[m_fieldNameID] = Convert.ToInt32(nieuweID);
                }
            }
        }
    }
}
