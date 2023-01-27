using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OnlineShop.WPF
{
    /// <summary>
    /// Логика взаимодействия для EditOrderRowWindow.xaml
    /// </summary>
    public partial class EditOrderRowWindow : Window
    {
        private DataRow dataRow;
        private bool flag;
        private EditOrderRowWindow()
        {
            InitializeComponent();
        }

        public EditOrderRowWindow(DataRowView dataRowView) : this()
        {
            DataContext = dataRowView;
            flag = false;
        }

        public EditOrderRowWindow(DataRow dataRow) : this()
        {
            this.dataRow = dataRow;
            flag = true;
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                try
                {
                    dataRow["Email"] = tbEmail.Text;
                    dataRow["ProductCode"] = tbProductCode.Text;
                    dataRow["NameOfProduct"] = tbNameOfProduct.Text;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ERROR btOk_Click in EditOrderRowWindow {ex.Message}");
                    this.DialogResult = false;
                    return;
                }

            }

            this.DialogResult = true;
        }
    }
}
