using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace _1234
{
    /// <summary>
    /// Логика взаимодействия для StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        SqlDataAdapter adapter;
        DataTable dt;
        WarehouseEntities ctx;
        DispatcherTimer timer;
        DateTime time;

        public StatisticsPage()
        {
            InitializeComponent();
            FillStartScreendDashbroad();
        }
       

       
        public void FillStartScreendDashbroad()
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    TextBlockCountOfItems.Text = ctx.Items.Count().ToString();
                    TextBlockSumOfAllItems.Text = ctx.Items.Select(x => x.Price).Sum().ToString() + "$";

                    //Group BY распределение по категориям
                    var groupByDepartments_InventItems = ctx.Items.GroupBy(x => x.Categories)
                                                                         .Select(group => new { Name = group.Key, Count = group.Count() });
                    //Group BY распределение по ответсвтенным лицам
                    var groupByResponsibleHuman_InventItems = ctx.Items.GroupBy(x => x.Employees)
                                                                             .Select(group => new { Name = group.Key, Count = group.Count() });
                    DepartmentsPieChart.Series.Clear();
                    ResponsibleWorkersPieChart.Series.Clear();

                    //скрытие LiveCharts если они пустые
                    if (groupByDepartments_InventItems.Count() <= 1)
                    {
                        DepPieChartTB.Visibility = Visibility.Hidden;
                        DepBorder.Visibility = Visibility.Hidden;
                        DepartmentsPieChart.Visibility = Visibility.Hidden;
                    }

                    else
                    {
                        DepPieChartTB.Visibility = Visibility.Visible;
                        DepBorder.Visibility = Visibility.Visible;
                        DepartmentsPieChart.Visibility = Visibility.Visible;
                    }

                    if (groupByResponsibleHuman_InventItems.Count() <= 1)
                    {
                        EmpPieChartTB.Visibility = Visibility.Hidden;
                        EmpBorder.Visibility = Visibility.Hidden;
                        ResponsibleWorkersPieChart.Visibility = Visibility.Hidden;
                    }

                    else
                    {
                        EmpPieChartTB.Visibility = Visibility.Visible;
                        EmpBorder.Visibility = Visibility.Visible;
                        ResponsibleWorkersPieChart.Visibility = Visibility.Visible;
                    }
                    //labelDate.Content = time.ToLongDateString();
                    //заполнение LiveCharts
                    foreach (var oneGroup in groupByDepartments_InventItems)
                    {
                        if (oneGroup.Name != null)
                            DepartmentsPieChart.Series.Add(new PieSeries { Title = $"{oneGroup.Name.Name}", Values = new ChartValues<int> { oneGroup.Count } });
                    }

                    foreach (var oneGroup in groupByResponsibleHuman_InventItems)
                    {
                        if (oneGroup.Name != null)
                            ResponsibleWorkersPieChart.Series.Add(new PieSeries { Title = $"{oneGroup.Name.Name}", Values = new ChartValues<int> { oneGroup.Count } });
                    }
                }
            }

            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
