using OnlineShop.WPF;
using System.Data;
using System.Windows;

namespace OnlineShop
{
    public partial class MainWindow : Window
    {
        public Repository rep;
        public MainWindow()
        {
            InitializeComponent();
            rep = new Repository(@"E:\C#\SKILLBOXC#\HW16\OnlineShop\DataBases","123");
            dataGridCustomers.ItemsSource = rep.CustomersTable.DefaultView;
            dataGridOrders.ItemsSource = rep.OrdersTable.DefaultView;
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

        #region MENU

        private void CustomersEdit_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dataGridCustomers.SelectedItem;
            if (dataRowView == null) return;
            EditCustomerRowWindow window = new EditCustomerRowWindow(dataRowView);
            if (window.ShowDialog() == true) rep.CustomersAdapterUpdate();
        }
        private void CustomersAdd_Click(object sender, RoutedEventArgs e)
        {
            //rep.dataRowView = (DataRowView)dataGridCustomers.Items[dataGridCustomers.Items.Count-1];
            DataRow r = rep.CustomersTable.NewRow();
            
            EditCustomerRowWindow window = new EditCustomerRowWindow(r);
            if (window.ShowDialog() == true)
            {
                rep.CustomersTable.Rows.Add(r);
                rep.CustomersAdapterUpdate();
            }
        }
        private void CustomersDelete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dataGridCustomers.SelectedItem;
            dataRowView.Delete();
            rep.CustomersAdapterUpdate();
        }

        private void OrdersEdit_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dataGridOrders.SelectedItem;
            if (dataRowView == null) return;
            EditOrderRowWindow window = new EditOrderRowWindow(dataRowView);
            if (window.ShowDialog() == true) rep.OrdersAdapterUpdate();
        }

        private void OrdersAdd_Click(object sender, RoutedEventArgs e)
        {
            DataRow r = rep.OrdersTable.NewRow();

            EditOrderRowWindow window = new EditOrderRowWindow(r);
            if (window.ShowDialog() == true)
            {
                rep.OrdersTable.Rows.Add(r);
                rep.OrdersAdapterUpdate();
            }
        }

        private void OrdersDelete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)dataGridOrders.SelectedItem;
            dataRowView.Delete();
            rep.OrdersAdapterUpdate();
        }
        #endregion
    }
}
 









