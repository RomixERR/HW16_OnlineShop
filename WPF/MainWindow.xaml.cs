using OnlineShop.WPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnlineShop
{
    public partial class MainWindow : Window
    {
        public Repository rep;
        public MainWindow()
        {
            InitializeComponent();

            rep = new Repository(@"E:\C#\SKILLBOXC#\HW16\OnlineShop\DataBases");

            dataGridCustomers.ItemsSource = rep.CustomersTable.DefaultView;
            //dataGridCustomers.CellEditEnding += rep.CustomersCellEditEnding;


            dataGridOrders.ItemsSource = rep.OrdersTable.DefaultView;
            dataGridOrders.CellEditEnding += rep.OrdersCellEditEnding; ///


            btEmailFilter.Click += (s, e) => {rep.SetFindForEmailAccessSelectCommand(GetSelectedString(dataGridCustomers.SelectedItem,"Email")); };
            btNoFilter.Click += (s, e) => { rep.SetTablesDefault(); };
            //btSave.Click += rep.BtSave_Click;
            
        }

        private string GetSelectedString(object selectedItem, string findStringToCell)
        {
            if (selectedItem == null) return null;
            if (selectedItem.GetType() != typeof(DataRowView)) return null;
            DataRowView drw = selectedItem as DataRowView;
            return drw[findStringToCell].ToString();
        }

        private void CustomersEdit_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dataGridCustomers.SelectedItem;
            EditCustomerRowWindow window = new EditCustomerRowWindow(dataRowView);
            if (window.ShowDialog() == true) rep.CustomersCellEditEnding(null,null);
        }

        private void CustomersAdd_Click(object sender, RoutedEventArgs e)
        {
            //rep.dataRowView = (DataRowView)dataGridCustomers.Items[dataGridCustomers.Items.Count-1];
            DataRow r = rep.CustomersTable.NewRow();
            
            EditCustomerRowWindow window = new EditCustomerRowWindow(r);
            if (window.ShowDialog() == true)
            {
                rep.CustomersTable.Rows.Add(r);
                rep.CustomersCellEditEnding(null, null);
            }
        }

        private void CustomersDelete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dataGridCustomers.SelectedItem;
            dataRowView.Delete();
            rep.CustomersCellEditEnding(null, null);
        }
    }


    //===========Repository===========
    public class Repository
    {
        //for customersDB (localDB)
        private SqlConnection cConnection;
        private SqlDataAdapter cAdapter;
        public DataTable CustomersTable;

        //for OrdersDB (Access)
        private OleDbConnection oConnection;
        private OleDbDataAdapter oAdapter;
        public DataTable OrdersTable;

        public string AccessConnectionString { get; private set; }
        //public string AccessSelectString { get; private set; }
        //public string AccessUpdateString { get; private set; }
        //public OleDbCommand AccessSelectCommand { get; private set; }
        //public OleDbCommand AccessUpdateCommand { get; private set; }

        public string LocalDBConnectionString { get; private set; }
        //public string LocalDBSelectString { get; private set; }
        //public string LocalDBUpdateString { get; private set; }
        //public SqlCommand LocalDBSelectCommand { get; private set; }
        //public SqlCommand LocalDBUpdateCommand { get; private set; }

        //public DataRowView dataRowView;


        public Repository(string pathToDB)
        {

            AccessConnectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                     $@"Data Source={pathToDB}\OrdersDatabase.accdb;" +
                                     $@"Jet OLEDB:Database Password=123;";
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

            //DateTime burn1 = DateTime.Parse("23.01.1978");
            //DateTime burn2 = DateTime.Parse("25.11.1983");
            //TimeSpan ts = burn2 - burn1;
            //Debug.WriteLine($"================================================ Years: {ts.TotalDays/365}, Days: {ts.TotalDays % 365}");


            SetTablesDefault();

        }

        private void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            Debug.WriteLine($"=>{sender.GetType().Name}; CurrentState = {e.CurrentState}");
        }

        public void OrdersCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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


        public void CustomersCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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

        //public void BtSave_Click(object sender, RoutedEventArgs e)
        //{
        //    oAdapter.Update(OrdersTable);
        //}

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
            LocalDBUpdateCommand.Parameters.Add("@PhoneNumber", SqlDbType.Int, 0, "PhoneNumber");
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
            LocalDBInsertCommand.Parameters.Add("@PhoneNumber", SqlDbType.Int, 0, "PhoneNumber");
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
 









