using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Data.Entity;

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _1234
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SqlDataAdapter adapter;
        DataTable dt;
        WarehouseEntities ctx;
        DispatcherTimer timer;
        DateTime time;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void MainWindowInitialize(object sender, EventArgs e)
        {
          

            await Task.Run(() =>
            {
                try
                {
                    using (ctx = new WarehouseEntities())
                    {
                        ctx.Items.Load();
                        ctx.Categories.Load();
                        ctx.Employees.Load();
                    }
                }

                catch (Exception exteption)
                {
                    MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
    }
}
