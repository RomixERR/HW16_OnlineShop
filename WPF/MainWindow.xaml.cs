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
//using System.Windows.Data;
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

            dataGridOrders.ItemsSource = rep.OrdersTable.DefaultView;
            dataGridOrders.CellEditEnding += rep.OrdersCellEditEnding;
            dataGridCustomers.ItemsSource = rep.CustomersTable.DefaultView;
            dataGridCustomers.CellEditEnding += rep.CustomersCellEditEnding;
            //button1.Click += (s, e) => { rep.SetFindForEmailAccessSelectCommand("111@mail.ru"); };
            btEmailFilter.Click += (s, e) => {rep.SetFindForEmailAccessSelectCommand(GetSelectedString(dataGridCustomers.SelectedItem,"Email")); };
            btNoFilter.Click += (s, e) => { rep.SetTablesDefault(); };
        }

        private string GetSelectedString(object selectedItem, string findStringToCell)
        {
            if (selectedItem == null) return null;
            if (selectedItem.GetType() != typeof(DataRowView)) return null;
            DataRowView drw = selectedItem as DataRowView;
            return drw[findStringToCell].ToString();
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
        public string AccessSelectString { get; private set; }
        public string AccessUpdateString { get; private set; }
        public OleDbCommand AccessSelectCommand { get; private set; }
        public OleDbCommand AccessUpdateCommand { get; private set; }

        public string LocalDBConnectionString { get; private set; }
        public string LocalDBSelectString { get; private set; }
        public string LocalDBUpdateString { get; private set; }
        public SqlCommand LocalDBSelectCommand { get; private set; }
        public SqlCommand LocalDBUpdateCommand { get; private set; }

        private MainWindow window;


        public Repository(string pathToDB)
        {
                //this.window = window;
                AccessConnectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                         $@"Data Source={pathToDB}\OrdersDatabase.accdb;" +
                                         $@"Jet OLEDB:Database Password=123;";
                LocalDBConnectionString =$@"Data Source=(localdb)\MSSQLLocalDB;" +
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

        public void OrdersCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            oAdapter.Update(OrdersTable);
        }

        public void CustomersCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            cAdapter.Update(CustomersTable);
        }

        private void DatabaseQuery()
        {
            oConnection.Open();
            OrdersTable.Clear();
            oAdapter.Fill(OrdersTable);
            oConnection.Close();

            cConnection.Open();
            CustomersTable.Clear();
            cAdapter.Fill(CustomersTable);
            cConnection.Close();
        }

        public void SetTablesDefault()
        {
            SetDefaultAccessSelectCommand();
            SetDefaultAccessUpdateCommand();
            SetDefaultLocalDBSelectCommand();
            SetDefaultLocalDBUpdateCommand();
            DatabaseQuery();
        }

        public void SetDefaultAccessSelectCommand()
        {
            AccessSelectString = $@"SELECT * FROM Orders;";
            AccessSelectCommand = new OleDbCommand(AccessSelectString, oConnection);
            oAdapter.SelectCommand = AccessSelectCommand;
        }

        public void SetDefaultLocalDBSelectCommand()
        {
            LocalDBSelectString = $@"SELECT * FROM Customers;";
            LocalDBSelectCommand = new SqlCommand(LocalDBSelectString, cConnection);
            cAdapter.SelectCommand = LocalDBSelectCommand;
        }

        public void SetDefaultAccessUpdateCommand()
        {
            AccessUpdateString =$"UPDATE Orders SET " +
                                $" " +
                                $"Email = @Email, " +
                                $"ProductCode = @ProductCode, " +
                                $"NameOfProduct = @NameOfProduct " +
                                $"WHERE ID = @FindID;";
            AccessUpdateCommand = new OleDbCommand(AccessUpdateString, oConnection);
            AccessUpdateCommand.Parameters.Add("@Email", OleDbType.WChar, 0, "Email");
            AccessUpdateCommand.Parameters.Add("@ProductCode", OleDbType.Integer, 0, "ProductCode");
            AccessUpdateCommand.Parameters.Add("@NameOfProduct", OleDbType.WChar, 0, "NameOfProduct");
            AccessUpdateCommand.Parameters.Add("@FindID", OleDbType.Integer, 0, "ID");
            oAdapter.UpdateCommand = AccessUpdateCommand;
        }

        public void SetDefaultLocalDBUpdateCommand()
        {
            LocalDBUpdateString = $"UPDATE Customers SET " +
                                $" " +
                                $"LastName = @LastName, " +
                                $"FirstName = @FirstName, " +
                                $"MiddleName = @MiddleName, " +
                                $"PhoneNumber = @PhoneNumber, " +
                                $"Email = @Email " +
                                $"WHERE ID = @FindID;";
            LocalDBUpdateCommand = new SqlCommand(LocalDBUpdateString, cConnection);
            LocalDBUpdateCommand.Parameters.Add("@LastName",SqlDbType.NVarChar, 0, "LastName");
            LocalDBUpdateCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar, 0, "FirstName");
            LocalDBUpdateCommand.Parameters.Add("@MiddleName", SqlDbType.NVarChar, 0, "MiddleName");
            LocalDBUpdateCommand.Parameters.Add("@PhoneNumber", SqlDbType.Int , 0, "PhoneNumber");
            LocalDBUpdateCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 0, "Email");
            LocalDBUpdateCommand.Parameters.Add("@FindID", SqlDbType.Int, 0, "ID");
            cAdapter.UpdateCommand = LocalDBUpdateCommand;
        }

        public void SetFindForEmailAccessSelectCommand(string email)
        {
            if (string.IsNullOrEmpty(email)) return;
            AccessSelectString = $"SELECT * FROM Orders " +
                                 $"WHERE Email = @Email;";
            AccessSelectCommand = new OleDbCommand(AccessSelectString, oConnection);
            AccessSelectCommand.Parameters.AddWithValue("@Email", email);
            oAdapter.SelectCommand = AccessSelectCommand;
            DatabaseQuery();
        }



    }
}
 









