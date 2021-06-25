using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.Entity;

namespace _1234
{
    /// <summary>
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {

        SqlDataAdapter adapter;
        DataTable dt;
        WarehouseEntities ctx;

        public EmployeesPage()
        {
            InitializeComponent();
            DataContext = this;
            FillDataGrid("EmployeesTable");
        }
        public void FillDataGrid(string fillForWhatDG)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    string connectionStr = ctx.Database.Connection.ConnectionString;
                   
                    

                    if (fillForWhatDG == "EmployeesTable")
                    {
                        string sqlQuery = @"SELECT * FROM EmployeeView";
                        adapter = new SqlDataAdapter(sqlQuery, connectionStr);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        EmployeesDataGrid.ItemsSource = dt.DefaultView;
                        //скрытие колонки c Primary KEY, которая нужна для обновления данных
                        //EmployeesDataGrid.Columns[0].Visibility = Visibility.Hidden;
                    }

                    
                }
            }

            catch (Exception e)
            {
                MessageBox.Show($"Информация об ошибке: {e.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void UpdateAndSaveChanges(string WhatSave)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {                                

                    //Логика обновления СОТРУДНИКА
                    if (WhatSave == "Employees")
                    {
                        if (EmployeesDataGrid.SelectedCells.Count == 4)
                        {
                            DataRowView rowView = EmployeesDataGrid.SelectedItem as DataRowView;
                            //id выделенной строки
                            int selectedItem_ID = Convert.ToInt32(rowView.Row[0]);

                            Employees updatedEmployee = new Employees();

                            //поиск обновляемого элемента в контексте по ID (Primary key)
                            updatedEmployee = ctx.Employees.Where(x => x.ID_Employee == selectedItem_ID).FirstOrDefault();
                            updatedEmployee.Name = textBoxEmployeeName.Text.ToString();
                            updatedEmployee.Phone = textBoxEmployeePhone.Text.ToString();
                            updatedEmployee.Email = textBoxEmployeeEmail.Text.ToString();

                            ctx.Entry(updatedEmployee).State = EntityState.Modified;
                            ctx.SaveChanges();

                            FillDataGrid("EmployeesTable");

                        }
                    }

                  

                }
            }
            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBoxChangedSearchEmployees_Event(object sender, TextChangedEventArgs e)
        {
            if (EmployeesDataGrid != null && EmployeesDataGrid.Visibility == Visibility)
            {
                using (ctx = new WarehouseEntities())
                {
                    string connectionStr = ctx.Database.Connection.ConnectionString;

                    adapter = new SqlDataAdapter("SELECT * FROM EmployeeView WHERE [Сотрудник] LIKE @search OR [Телефон] LIKE @search OR [Email] LIKE @search", connectionStr);
                    adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + textBoxEmployeesSearch.Text + "%");
                    dt = new DataTable();
                    adapter.Fill(dt);

                    EmployeesDataGrid.ItemsSource = dt.DefaultView;
                    EmployeesDataGrid.Columns[0].Visibility = Visibility.Hidden;
                }
            }
        }
        private void ButtonUpdateEmployeesInfo_Click(object sender, RoutedEventArgs e)
        {
            UpdateAndSaveChanges("Employees");
        }

        //клик по кнопке «УДАЛИТЬ СОТРУДНИКА»
        private void ButtonDeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    if (EmployeesDataGrid.SelectedCells.Count == 4)
                    {
                        DataRowView rowView = EmployeesDataGrid.SelectedItem as DataRowView;

                        //id выделенной строки (Primary key)
                        int selectedEmployee_ID = Convert.ToInt32(rowView.Row[0]);

                        //сотрудник которого надо удалить
                        Employees deletedEmployee = new Employees();

                        //поиск удаляемого сотрудника в контексте по ID (Primary key)
                        deletedEmployee = ctx.Employees.Where(x => x.ID_Employee == selectedEmployee_ID).FirstOrDefault();

                        if (deletedEmployee != null)
                        {
                            //есть ли вещи связанные в с этим сотрудником в таблице InventItems (FK)?
                            var removeThisItems = ctx.Items.Where(x => x.FK_ResponsibleEmployee == selectedEmployee_ID);

                            //есть вещи
                            if (removeThisItems.Count() >= 1)
                            {
                                MessageBoxResult result = MessageBox.Show($"У сотрудника сейчас находится имущество. Удалить сотрудника и сбросить критерий «Ответсвтенный сотрудник» у имущества находящегося у {deletedEmployee.Name}?", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                                if (result == MessageBoxResult.OK)
                                {
                                    foreach (var item in removeThisItems)
                                    {
                                        if (item.FK_ResponsibleEmployee == selectedEmployee_ID)
                                            item.FK_ResponsibleEmployee = null;
                                    }

                                    ctx.Employees.Remove(deletedEmployee);
                                    ctx.Entry(deletedEmployee).State = EntityState.Deleted;
                                    ctx.SaveChanges();
                                    FillDataGrid("EmployeesTable");
                                }
                            }

                            //нету вещей
                            else if (removeThisItems.Count() < 1)
                            {
                                //удаление сотрудника
                                ctx.Employees.Remove(deletedEmployee);
                                ctx.Entry(deletedEmployee).State = EntityState.Deleted;
                                ctx.SaveChanges();
                                //обновление DataGrid для просмотра изменений
                                FillDataGrid("EmployeesTable");
                            }


                        }
                    }
                }
            }

            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddEmployeeWindow addEmployeeWindow = AddEmployeeWindow.GetInstance();

                if (addEmployeeWindow != null)
                {
                    addEmployeeWindow.Show();
                    addEmployeeWindow.Topmost = true;

                    addEmployeeWindow.Closed += (object s, EventArgs EvArgs) =>
                    {
                        FillDataGrid("EmployeesTable");
                        AddEmployeeWindow.ClearInstance();
                    };
                }

            }
            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmployeesInfoValidation_TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedCells.Count == 4)
            {
                TextBox tb = sender as TextBox;
                TextBoxValidation.TextChanged(tb);
                if (TextBoxValidation.TextChanged(textBoxEmployeeName) && TextBoxValidation.TextChanged(textBoxEmployeePhone) && TextBoxValidation.TextChanged(textBoxEmployeeEmail))
                    buttonUpdateEmployeesInfo.IsEnabled = true; //отключение кнопки обновить
                else
                    buttonUpdateEmployeesInfo.IsEnabled = false;
            }
        }


        //клик по кнопке «СФОРМИРОВАТЬ ОТЧЁТ»
        private void ButtonMakeEmployeesReport_Click(object sender, RoutedEventArgs e)
        {
            MakeReport(EmployeesDataGrid);
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
