using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;


namespace _1234
{
    /// <summary>
    /// Логика взаимодействия для FilterPage.xaml
    /// </summary>
    public partial class FilterPage : Page
    {
        SqlDataAdapter adapter;
        DataTable dt;
        WarehouseEntities ctx;

        public FilterPage()
        {
            InitializeComponent();
            DataContext = this;
            FillDataGrid("LifeTimeTable");
            FillComboBox(comboBoxItrmsResponsibleHumanSorting);
            FillComboBox(comboBoxItemsCategorySorting);
        }

        public void FillDataGrid(string fillForWhatDG)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    string connectionStr = ctx.Database.Connection.ConnectionString;
                   
                    {
                        string sqlQuery = @"SELECT * FROM ItemsView";
                        adapter = new SqlDataAdapter(sqlQuery, connectionStr);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        LifeTimeDatagrid.ItemsSource = dt.DefaultView;
                        
                    }
                }
            }

            catch (Exception e)
            {
                MessageBox.Show($"Информация об ошибке: {e.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //заполнение ComboBox
        public void FillComboBox(ComboBox cb)
        {
            using (ctx = new WarehouseEntities())
            {
                string connectionStr = ctx.Database.Connection.ConnectionString;

                //«Ответственное лицо»
                if ( cb == comboBoxItrmsResponsibleHumanSorting)
                {
                    string sqlQuery = @"SELECT Name FROM Employees";
                    adapter = new SqlDataAdapter(sqlQuery, connectionStr);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    cb.ItemsSource = dt.DefaultView;
                    cb.DisplayMemberPath = dt.Columns["Name"].ToString();
                }

             

                //«Категория»
                if ( cb == comboBoxItemsCategorySorting)
                {
                    string sqlQuery = @"SELECT Name FROM Categories";
                    adapter = new SqlDataAdapter(sqlQuery, connectionStr);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    cb.ItemsSource = dt.DefaultView;
                    cb.DisplayMemberPath = dt.Columns["Name"].ToString();
                }
            }
        }

        private void ButtonLifeTimeFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    string connectionStr = ctx.Database.Connection.ConnectionString;
                    string dateQuery;
                    string s1 = "";
                    string s2 = "";
                    string s3 = "";

                    if (textBoxItemsDateOffSorting.Text == "")
                        dateQuery = "";
                    else
                        dateQuery = $"AND convert(datetime, [Дата списания], 103) <= convert(datetime, '{textBoxItemsDateOffSorting.Text}', 103)";

                    if (comboBoxItrmsResponsibleHumanSorting.Text == "")
                        s1 = "OR  [Ответственный] IS NULL";




                    if (comboBoxItemsCategorySorting.Text == "")
                        s3 = " OR  [Категория] IS NULL";


                    adapter = new SqlDataAdapter($"SELECT * FROM ItemsView WHERE ([Ответственный] LIKE @Worker {s1}) AND ([Категория] LIKE @Category {s3}) {dateQuery}", connectionStr);

                    if (comboBoxItrmsResponsibleHumanSorting.Text == "")
                        adapter.SelectCommand.Parameters.AddWithValue("@Worker", "%");

                    else
                        adapter.SelectCommand.Parameters.AddWithValue("@Worker", comboBoxItrmsResponsibleHumanSorting.Text);



                    if (comboBoxItemsCategorySorting.Text == "")
                        adapter.SelectCommand.Parameters.AddWithValue("@Category", "%");

                    else
                        adapter.SelectCommand.Parameters.AddWithValue("@Category", comboBoxItemsCategorySorting.Text);


                    dt = new DataTable();
                    adapter.Fill(dt);


                    if (LifeTimeDatagrid != null)
                    {
                        //для плавного появления
                        LifeTimeDatagrid.Visibility = Visibility.Hidden;
                        LifeTimeDatagrid.Visibility = Visibility.Visible;

                        LifeTimeDatagrid.ItemsSource = dt.DefaultView;



                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show($"Информация об ошибке: {ee.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        
        private void ButtonResetCBResponsibleWorkerFilter_Click(object sender, RoutedEventArgs e)
        {
            comboBoxItrmsResponsibleHumanSorting.SelectedIndex = -1;
        }

       
        //очистка combobox Категория по клику на кнопку [очистить]
        private void ButtonResetCBCategoryFilter_Click(object sender, RoutedEventArgs e)
        {
            comboBoxItemsCategorySorting.SelectedIndex = -1;
        }


        private void ButtonMakeFilterReport_Click(object sender, RoutedEventArgs e)
        {
            MakeReport(LifeTimeDatagrid);
        }

        private void FilterValidation_TextChangedEvent(object sender, TextChangedEventArgs e)
        {

            TextBox tb = sender as TextBox;
            TextBoxValidation.TextChanged(tb);
            if (TextBoxValidation.TextChanged(textBoxItemsDateOffSorting))
                ButtonSort.IsEnabled = true; //отключение кнопки обновить
            else
                ButtonSort.IsEnabled = false;

        }

        private void ButtonFillTextBoxFilterForNext6Month_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            string date = now.AddMonths(6).ToString("dd.MM.yyyy");

            textBoxItemsDateOffSorting.Text = date;
        }

        #region Excel
        public void MakeReport(DataGrid WhatDGRepot)
        {
            if (MessageBox.Show("Формирование отчета может занять некоторое время. Продолжить?", "Формирование отчета", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                bool isHeader = true;

                Excel.Application ExcelApp = new Excel.Application();
                Excel.Workbook ExcelBook = ExcelApp.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet ExcelWorkSheet = (Excel.Worksheet)ExcelBook.Sheets[1];


                int i = 2;
                int j = 2;

                foreach (DataRow row in (WhatDGRepot.ItemsSource as DataView).Table.Rows)
                {
                    foreach (DataColumn column in (WhatDGRepot.ItemsSource as DataView).Table.Columns)
                    {
                        if (isHeader)
                        {
                            ExcelWorkSheet.Cells[i - 1, j].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            ExcelWorkSheet.Cells[i - 1, j].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(111, 126, 149));
                            ExcelWorkSheet.Cells[i - 1, j].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                            ExcelWorkSheet.Cells[i - 1, j] = column.ColumnName;


                        }

                        if (column.DataType == typeof(string))
                        {
                            if (i % 2 == 0)
                                ExcelWorkSheet.Cells[i, j].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(251, 252, 254));
                            else
                                ExcelWorkSheet.Cells[i, j].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255, 255, 255));

                            ExcelWorkSheet.Cells[i, j] = row.Field<string>(column);
                        }

                        if (column.DataType == typeof(int))
                        {
                            if (i % 2 == 0)
                                ExcelWorkSheet.Cells[i, j].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(251, 252, 254));
                            else
                                ExcelWorkSheet.Cells[i, j].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255, 255, 255));

                            ExcelWorkSheet.Cells[i, j] = row.Field<int?>(column);
                        }
                        j++;
                    }

                    i++;
                    j = 2;
                    isHeader = false;
                }

                ExcelWorkSheet.Columns.AutoFit();
                ExcelWorkSheet.Rows.RowHeight = 20;
                ExcelWorkSheet.Rows.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;

                ExcelApp.Visible = true;
                ExcelApp.UserControl = true;
            }
        }
        #endregion
    }
}
