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
            rep = new Repository(this, @"E:\C#\SKILLBOXC#\HW16\OnlineShop\DataBases");
            
        }
    }

    //===========Repository===========
    public class Repository
    {
        //for customersDB (localDB)
        private SqlConnection cConnection;
        private SqlDataAdapter cAdapter;
        private DataTable CustomersTable;

        //for OrdersDB (Access)
        private OleDbConnection oConnection;
        private OleDbDataAdapter oAdapter;
        private DataTable OrdersTable;

        public string AccessConnectionString { get; private set; }
        public string AccessSelectString { get; private set; }
        public string AccessUpdateString { get; private set; }
        public OleDbCommand AccessSelectCommand { get; private set; }
        public OleDbCommand AccessUpdateCommand { get; private set; }

        public string localDBConnectionString { get; private set; }
        public string LocalDBSelectString { get; private set; }
        public string LocalDBUpdateString { get; private set; }

        private MainWindow window;


        public Repository(MainWindow window, string pathToDB)
        {
                this.window = window;
                AccessConnectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                         $@"Data Source={pathToDB}\OrdersDatabase.accdb;" +
                                         $@"Jet OLEDB:Database Password=123;";
                localDBConnectionString =$@"Data Source=(localdb)\MSSQLLocalDB;" +
                                         $@"AttachDbFilename={pathToDB}\CustomersDatabase.mdf;" +
                                         $@"Integrated Security=True";

            cConnection = new SqlConnection(localDBConnectionString);
            cAdapter = new SqlDataAdapter();
            CustomersTable = new DataTable("Customers");
            oConnection = new OleDbConnection(AccessConnectionString);
            oAdapter = new OleDbDataAdapter();
            OrdersTable = new DataTable("Orders");
            oConnection.StateChange += OConnection_StateChange;

            SetTablesDefault();
            FirstRunFillTables();

            //Debug.WriteLine( OrdersTable.Rows.Count);
            window.dataGridOrders.ItemsSource = OrdersTable.DefaultView;
            window.dataGridOrders.CellEditEnding += DataGridOrders_CellEditEnding;
            window.button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            SetFindForEmailAccessSelectCommand("111@mail.ru");
            FirstRunFillTables();
            //oAdapter.Update(OrdersTable);
        }

        private void OConnection_StateChange(object sender, StateChangeEventArgs e)
        {
            Debug.WriteLine(e.CurrentState);
        }

        private void DataGridOrders_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            oAdapter.Update(OrdersTable);
        }

        private void FirstRunFillTables()
        {
            oConnection.Open();
            OrdersTable.Clear();
            oAdapter.Fill(OrdersTable);
            oConnection.Close();
        }

        public void SetTablesDefault()
        {
            SetDefaultAccessSelectCommand();
            SetDefaultAccessUpdateCommand();
        }

        public void SetDefaultAccessSelectCommand()
        {
            AccessSelectString = $@"SELECT * FROM Orders;";
            AccessSelectCommand = new OleDbCommand(AccessSelectString, oConnection);
            oAdapter.SelectCommand = AccessSelectCommand;
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

        public void SetFindForEmailAccessSelectCommand(string email)
        {
            AccessSelectString = $"SELECT * FROM Orders " +
                                 $"WHERE Email = @Email;";
            AccessSelectCommand = new OleDbCommand(AccessSelectString, oConnection);
            AccessSelectCommand.Parameters.AddWithValue("@Email", email);
            oAdapter.SelectCommand = AccessSelectCommand;            
        }



    }
}
 









