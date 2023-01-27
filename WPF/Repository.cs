using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OnlineShop
{
    public class Repository
    {
        //for customersDB (localDB)
        public string AccessConnectionString { get; private set; }
        private SqlConnection cConnection;
        private SqlDataAdapter cAdapter;
        public DataTable CustomersTable;

        //for OrdersDB (Access)
        public string LocalDBConnectionString { get; private set; }
        private OleDbConnection oConnection;
        private OleDbDataAdapter oAdapter;
        public DataTable OrdersTable;

        public Repository(string pathToDB, string password)
        {

            AccessConnectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                     $@"Data Source={pathToDB}\OrdersDatabase.accdb;" +
                                     $@"Jet OLEDB:Database Password={password};";
            LocalDBConnectionString = $@"Data Source=(localdb)\MSSQLLocalDB;" +
                                     $@"AttachDbFilename={pathToDB}\CustomersDatabase.mdf;" +
                                     $@"Integrated Security=True";

            cConnection = new SqlConnection(LocalDBConnectionString);
            cAdapter = new SqlDataAdapter();
            CustomersTable = new DataTable("Customers");
            oConnection = new OleDbConnection(AccessConnectionString);
            oAdapter = new OleDbDataAdapter();
            OrdersTable = new DataTable("Orders");
            oConnection.StateChange += Connection_StateChange;
            cConnection.StateChange += Connection_StateChange;

            SetTablesDefault();
        }

        private void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            Debug.WriteLine($"=>{sender.GetType().Name}; CurrentState = {e.CurrentState}");
        }

        public void OrdersAdapterUpdate()
        {
            try
            {
                oAdapter.Update(OrdersTable);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR OrdersCellEditEnding: {ex.Message}");
            }
        }

        public void CustomersAdapterUpdate()
        {
            try
            {
                cAdapter.Update(CustomersTable);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR CustomersCellEditEnding: {ex.Message}");
            }
        }

        private async Task DatabaseQuery()
        {
            var oCTask = oConnection.OpenAsync();
            OrdersTable.Clear();           

            var cCTask = cConnection.OpenAsync();
            CustomersTable.Clear();

            await Task.WhenAll(oCTask, cCTask);

            oAdapter.Fill(OrdersTable);
            oConnection.Close();
            cAdapter.Fill(CustomersTable);
            cConnection.Close();
        }

        public void SetTablesDefault()
        {
            SetDefaultAccessSelectCommand();
            SetDefaultAccessUpdateCommand();
            SetAccessInsertCommand();
            SetAccessDeleteCommand();
            SetDefaultLocalDBSelectCommand();
            SetDefaultLocalDBUpdateCommand();
            SetLocalDBInsertCommand();
            SetLocalDBDeleteCommand();

            DatabaseQuery();
        }

        public void SetDefaultAccessSelectCommand()
        {
            string AccessSelectString = $@"SELECT * FROM Orders;";
            OleDbCommand AccessSelectCommand = new OleDbCommand(AccessSelectString, oConnection);
            oAdapter.SelectCommand = AccessSelectCommand;
        }

        public void SetDefaultLocalDBSelectCommand()
        {
            string LocalDBSelectString = $@"SELECT * FROM Customers;";
            SqlCommand LocalDBSelectCommand = new SqlCommand(LocalDBSelectString, cConnection);
            cAdapter.SelectCommand = LocalDBSelectCommand;
        }

        public void SetDefaultAccessUpdateCommand()
        {
            string AccessUpdateString = $"UPDATE Orders SET " +
                                $" " +
                                $"Email = @Email, " +
                                $"ProductCode = @ProductCode, " +
                                $"NameOfProduct = @NameOfProduct " +
                                $"WHERE ID = @FindID;";
            OleDbCommand AccessUpdateCommand = new OleDbCommand(AccessUpdateString, oConnection);
            AccessUpdateCommand.Parameters.Add("@Email", OleDbType.WChar, 0, "Email");
            AccessUpdateCommand.Parameters.Add("@ProductCode", OleDbType.Integer, 0, "ProductCode");
            AccessUpdateCommand.Parameters.Add("@NameOfProduct", OleDbType.WChar, 0, "NameOfProduct");
            AccessUpdateCommand.Parameters.Add("@FindID", OleDbType.Integer, 0, "ID").SourceVersion = DataRowVersion.Original;
            oAdapter.UpdateCommand = AccessUpdateCommand;
        }

        public void SetDefaultLocalDBUpdateCommand()
        {
            string LocalDBUpdateString = $"UPDATE Customers SET " +
                                $" " +
                                $"LastName = @LastName, " +
                                $"FirstName = @FirstName, " +
                                $"MiddleName = @MiddleName, " +
                                $"PhoneNumber = @PhoneNumber, " +
                                $"Email = @Email " +
                                $"WHERE ID = @FindID;";
            SqlCommand LocalDBUpdateCommand = new SqlCommand(LocalDBUpdateString, cConnection);
            LocalDBUpdateCommand.Parameters.Add("@LastName", SqlDbType.NVarChar, 0, "LastName");
            LocalDBUpdateCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar, 0, "FirstName");
            LocalDBUpdateCommand.Parameters.Add("@MiddleName", SqlDbType.NVarChar, 0, "MiddleName");
            LocalDBUpdateCommand.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 0, "PhoneNumber");
            LocalDBUpdateCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 0, "Email");
            LocalDBUpdateCommand.Parameters.Add("@FindID", SqlDbType.Int, 0, "ID").SourceVersion = DataRowVersion.Original;
            cAdapter.UpdateCommand = LocalDBUpdateCommand;
        }

        public void SetAccessInsertCommand()
        {
            string AccessInsertCommandString = $"INSERT INTO Orders " +
                                $"(Email,ProductCode,NameOfProduct) VALUES " +
                                $"(@Email, " +
                                $"@ProductCode, " +
                                $"@NameOfProduct); " +
                                //$"SET @ID = @@IDENTITY;";
                                $"";
            OleDbCommand AccessInsertCommand = new OleDbCommand(AccessInsertCommandString, oConnection);
            AccessInsertCommand.Parameters.Add("@Email", OleDbType.WChar, 0, "Email");
            AccessInsertCommand.Parameters.Add("@ProductCode", OleDbType.Integer, 0, "ProductCode");
            AccessInsertCommand.Parameters.Add("@NameOfProduct", OleDbType.WChar, 0, "NameOfProduct");
            //AccessInsertCommand.Parameters.Add("@ID", OleDbType.Integer, 0, "ID").Direction = ParameterDirection.Output;
            oAdapter.InsertCommand = AccessInsertCommand;
        }

        public void SetLocalDBInsertCommand()
        {
            string LocalDBInsertString = $"INSERT INTO Customers " +
                                $"(LastName, FirstName, MiddleName, PhoneNumber, Email) VALUES " +
                                $"(@LastName, " +
                                $"@FirstName, " +
                                $"@MiddleName, " +
                                $"@PhoneNumber, " +
                                $"@Email); " +
                                $"SET @ID = @@IDENTITY;";
            SqlCommand LocalDBInsertCommand = new SqlCommand(LocalDBInsertString, cConnection);
            LocalDBInsertCommand.Parameters.Add("@LastName", SqlDbType.NVarChar, 0, "LastName");
            LocalDBInsertCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar, 0, "FirstName");
            LocalDBInsertCommand.Parameters.Add("@MiddleName", SqlDbType.NVarChar, 0, "MiddleName");
            LocalDBInsertCommand.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 0, "PhoneNumber");
            LocalDBInsertCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 0, "Email");
            LocalDBInsertCommand.Parameters.Add("@ID", SqlDbType.Int, 0, "ID").Direction = ParameterDirection.Output;
            cAdapter.InsertCommand = LocalDBInsertCommand;
        }

        public void SetFindForEmailAccessSelectCommand(string email)
        {
            if (string.IsNullOrEmpty(email)) return;
            string AccessSelectString = $"SELECT * FROM Orders " +
                                 $"WHERE Email = @Email;";
            OleDbCommand AccessSelectCommand = new OleDbCommand(AccessSelectString, oConnection);
            AccessSelectCommand.Parameters.AddWithValue("@Email", email);
            oAdapter.SelectCommand = AccessSelectCommand;
            DatabaseQuery();
        }

        public void SetAccessDeleteCommand()
        {
            string AccessDeleteString = $@"DELETE FROM Orders WHERE ID = @ID;";
            OleDbCommand AccessDeleteCommand = new OleDbCommand(AccessDeleteString, oConnection);
            AccessDeleteCommand.Parameters.Add("@ID", OleDbType.Integer, 0, "ID");
            oAdapter.DeleteCommand = AccessDeleteCommand;
        }

        public void SetLocalDBDeleteCommand()
        {
            string LocalDBDeleteString = $@"DELETE FROM Customers WHERE ID = @ID;";
            SqlCommand LocalDBDeleteCommand = new SqlCommand(LocalDBDeleteString, cConnection);
            LocalDBDeleteCommand.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
            cAdapter.DeleteCommand = LocalDBDeleteCommand;
        }
    }
}