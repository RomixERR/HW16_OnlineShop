using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Data;
using System.Diagnostics;

namespace OnlineShop.WPF
{
    /// <summary>
    /// Логика взаимодействия для EditRowWindow.xaml
    /// </summary>
    public partial class EditCustomerRowWindow : Window
    {
        private DataRow dataRow;
        private bool flag;
        private EditCustomerRowWindow()
        {
            InitializeComponent();
        }

        public EditCustomerRowWindow(DataRowView dataRowView) : this()
        {
            
            DataContext = dataRowView;
            flag = false;
        }

        public EditCustomerRowWindow(DataRow dataRow) : this()
        {
            
            this.dataRow = dataRow;
            flag = true;
            //DataContext = dataRow.Table;
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                try
                {
                    dataRow["LastName"] = tbLastName.Text;
                    dataRow["FirstName"] = tbFirstName.Text;
                    dataRow["MiddleName"] = tbMiddleName.Text;
                    dataRow["PhoneNumber"] = tbPhoneNumber.Text;
                    dataRow["Email"] = tbEmail.Text;
                } catch (Exception ex)
                {
                    Debug.WriteLine($"ERROR btOk_Click in EditCustomerRowWindow {ex.Message}");
                    this.DialogResult = false;
                    return;
                }
                
            }              


            this.DialogResult = true;
        }
    }
}
