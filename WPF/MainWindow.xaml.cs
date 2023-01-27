using OnlineShop.WPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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


        private void btFakeUsers_Click(object sender, RoutedEventArgs e)
        {
            DataRow dataRow;
            int amountGetUsers = 5;
            FakeUsersLite.FakeUser fakeUser = new FakeUsersLite.FakeUser(FakeUsersLite.FakeUser.Egender.Male);
            for (int i = 0; i < amountGetUsers; i++)
            {
                dataRow = rep.CustomersTable.NewRow();
                dataRow["LastName"] = fakeUser.GetLName();
                dataRow["FirstName"] = fakeUser.GetFName();
                dataRow["MiddleName"] = fakeUser.GetMName();
                dataRow["PhoneNumber"] = fakeUser.GetPhone();
                dataRow["Email"] = fakeUser.GetEmail();
                rep.CustomersTable.Rows.Add(dataRow);
            }           
            rep.CustomersAdapterUpdate();           
        }

        private void btFakeOrders_Click(object sender, RoutedEventArgs e)
        {
            int countOfAllRows = rep.CustomersTable.Rows.Count;
            int amountGetUsers = 5;
            int maxProductCodeForGen = 1000;
            List<string> Emails = new List<string>();
            Random r = new Random();
            DataRow dataRow;
            FakeUsersLite.FakeUser fakeUser = new FakeUsersLite.FakeUser(FakeUsersLite.FakeUser.Egender.Thing);

            for (int i = 0; i < amountGetUsers; i++)
            {
                DataRow row = rep.CustomersTable.Rows[r.Next(0, countOfAllRows)];
                dataRow = rep.OrdersTable.NewRow();
                dataRow["Email"] = (string)row["Email"];
                dataRow["ProductCode"] = r.Next(0, maxProductCodeForGen);
                dataRow["NameOfProduct"] = fakeUser.GetFName();
                rep.OrdersTable.Rows.Add(dataRow);
            }

            rep.OrdersAdapterUpdate();
        }
    }
}
 









