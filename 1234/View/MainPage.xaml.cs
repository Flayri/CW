using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Threading;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System;
using System.Windows;
using System.Linq;

namespace _1234
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {

        SqlDataAdapter adapter;
        DataTable dt;
        WarehouseEntities ctx;
        DispatcherTimer timer;
        DateTime time;

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            FillComboBox(comboBoxItrmsResponsibleHuman);
            FillComboBox(comboBoxItemsCategories);
            FillDataGrid("ItemsTable");
        }

        public void FillDataGrid(string fillForWhatDG)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    string connectionStr = ctx.Database.Connection.ConnectionString;
                    if (fillForWhatDG == "ItemsTable")
                    {
                        string sqlQuery = @"SELECT * FROM ItemsView";
                        adapter = new SqlDataAdapter(sqlQuery, connectionStr);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        MainDataGrid.ItemsSource = dt.DefaultView;
                        //MainDataGrid.Columns[0].Width = 20;
                        //MainDataGrid.Columns[1].Width = 185;
                        //MainDataGrid.Columns[2].Width = 65;
                        //MainDataGrid.Columns[4].Width = 95;
                        //MainDataGrid.Columns[5].Width = 55;
                        //MainDataGrid.Columns[6].Width = 125;
                       

                    }

                    
                }
            }

            catch (Exception e)
            {
                MessageBox.Show($"Информация об ошибке: {e.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        public void FillComboBox(ComboBox cb)
        {
            using (ctx = new WarehouseEntities())
            {
                string connectionStr = ctx.Database.Connection.ConnectionString;

                //«Ответственное лицо»
                if (cb == comboBoxItrmsResponsibleHuman )
                {
                    string sqlQuery = @"SELECT Name FROM Employees";
                    adapter = new SqlDataAdapter(sqlQuery, connectionStr);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    cb.ItemsSource = dt.DefaultView;
                    cb.DisplayMemberPath = dt.Columns["Name"].ToString();
                }

               

                //«Категория»
                if (cb == comboBoxItemsCategories )
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

        private void TextBoxAllowOnlyDigits_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

        private void TextBoxChangedSearchItems_Event(object sender, TextChangedEventArgs e)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    string connectionStr = ctx.Database.Connection.ConnectionString;

                    adapter = new SqlDataAdapter("SELECT * FROM ItemsView WHERE Наименование LIKE @search OR [№] LIKE @search OR [Ответственный] LIKE @search ", connectionStr);
                    adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + textBoxItemsSearch.Text + "%");
                    dt = new DataTable();
                    adapter.Fill(dt);

                    if (MainDataGrid != null)
                    {
                        MainDataGrid.ItemsSource = dt.DefaultView;
                        //MainDataGrid.Columns[0].Width = 20;
                        //MainDataGrid.Columns[1].Width = 195;
                        //MainDataGrid.Columns[2].Width = 65;
                        //MainDataGrid.Columns[4].Width = 95;
                        //MainDataGrid.Columns[5].Width = 55;
                        //MainDataGrid.Columns[6].Width = 125;
                        //MainDataGrid.Columns[7].Width = 145;
                    }


                }
            }
            catch (Exception ee)
            {
                MessageBox.Show($"Информация об ошибке: {ee.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public void UpdateAndSaveChanges(string WhatSave)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {

                    //Логика обновления ВСЁ ИМУЩЕСТВО
                    if (WhatSave == "GoodsInfo")
                    {


                        if (MainDataGrid.SelectedCells.Count == 9)
                        {
                            DataRowView rowView = MainDataGrid.SelectedItem as DataRowView;
                            //id выделенной строки
                            int selectedItem_ID = Convert.ToInt32(rowView.Row[0]);

                            Items updatedtems = new Items();
                            //поиск обновляемого элемента в контексте по ID (Primary key)
                            updatedtems = ctx.Items.Where(x => x.ID_item == selectedItem_ID).FirstOrDefault();

                            //Название
                            updatedtems.ItemName = textBoxInventItemsName.Text.ToString();

                            //Кол-во
                            bool ammountIsParsed = int.TryParse(textBoxItemsAmmount.Text, out int _ammount);
                            if (!ammountIsParsed)
                                updatedtems.Ammount = null;
                            else
                                updatedtems.Ammount = _ammount;

                            //Цена
                            bool priceIsParsed = int.TryParse(textBoxInventItemsPrice.Text, out int _price);
                            if (!priceIsParsed)
                                updatedtems.Price = null;
                            else
                                updatedtems.Price = _price;

                            //Дата покупки
                            updatedtems.BuyDate = textBoxItemsBuyDate.Text;

                            //Срок службы (число)
                            bool lifeTimeIsParsed = int.TryParse(textBoxItemsLifeTime.Text, out int _lifeTime);
                            if (!lifeTimeIsParsed)
                                updatedtems.LifeTime = null;
                            else
                                updatedtems.LifeTime = _lifeTime;

                            //Категория
                            if (comboBoxItemsCategories.Text != "")
                            {
                                Categories category = ctx.Categories.Where(x => x.Name == comboBoxItemsCategories.Text).FirstOrDefault();
                                if (category != null)
                                    updatedtems.FK_Category = category.ID;
                            }

                            else
                                updatedtems.FK_Category = null;

                            //Обновление срока службы (дата списания)
                            if (textBoxItemsBuyDate.Text != "")
                            {
                                if (textBoxItemsLifeTime.Text != "")
                                {
                                    DateTime buyDateFromTextBox = Convert.ToDateTime(textBoxItemsBuyDate.Text);
                                    updatedtems.OffDate = buyDateFromTextBox.AddMonths(_lifeTime).ToString("dd.MM.yyyy");
                                }
                            }

                            else
                            {
                                updatedtems.OffDate = null;

                            }

                            //Ответственный сотрудник
                            if (comboBoxItrmsResponsibleHuman.Text != "")
                            {
                                Employees employee = ctx.Employees.Where(x => x.Name == comboBoxItrmsResponsibleHuman.Text).FirstOrDefault();
                                if (employee != null)
                                    updatedtems.FK_ResponsibleEmployee = employee.ID_Employee;
                            }

                            else
                            { updatedtems.FK_ResponsibleEmployee = null; }

                            ctx.Entry(updatedtems).State = EntityState.Modified;
                            ctx.SaveChanges();

                            FillDataGrid("ItemsTable");


                        }
                    }

                }              
                
            }
            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void ButtonUpdateGoodsInfo_Click(object sender, RoutedEventArgs e)
        {
            UpdateAndSaveChanges("GoodsInfo");
            //MessageBox.Show($"Данные успешно обновлены", "Информация", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        //клик по кнопке «УДАЛИТЬ ЗАПИСЬ»
        private void ButtonDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            using (ctx = new WarehouseEntities())
            {
                if (MainDataGrid.SelectedCells.Count == 10)
                {
                    DataRowView rowView = MainDataGrid.SelectedItem as DataRowView;
                    //id выделенной строки
                    int selectedItem_ID = Convert.ToInt32(rowView.Row[0]);
                    Items updatedtems = new Items();
                    //поиск обновляемого элемента в контексте по ID (Primary key)
                    updatedtems = ctx.Items.Where(x => x.ID_item == selectedItem_ID).FirstOrDefault();
                    ctx.Items.Remove(updatedtems);

                    ctx.Entry(updatedtems).State = EntityState.Deleted;
                    ctx.SaveChanges();

                    FillDataGrid("ItemsTable");
                }
            }
        }


        //клик по кнопке «ДОБАВИТЬ ЗАПИСЬ»
        private void ButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddItemWindow addItemWindow = AddItemWindow.GetInstance();

                if (addItemWindow != null)
                {
                    addItemWindow.Show();
                    addItemWindow.Topmost = true;

                    addItemWindow.Closed += (object s, EventArgs EvArgs) =>
                    {
                        FillDataGrid("ItemsTable");
                        AddItemWindow.ClearInstance();
                    };
                }
            }

            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoodsInfoValidation_TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            if (MainDataGrid.SelectedCells.Count == 9)
            {
                TextBox tb = sender as TextBox;
                TextBoxValidation.TextChanged(tb);
                if (TextBoxValidation.TextChanged(textBoxInventItemsName) && TextBoxValidation.TextChanged(textBoxItemsAmmount) && TextBoxValidation.TextChanged(textBoxInventItemsPrice) && TextBoxValidation.TextChanged(textBoxItemsBuyDate) && TextBoxValidation.TextChanged(textBoxItemsLifeTime))
                    ButtonUpdateGoodsInfo.IsEnabled = true; //отключение кнопки обновить
                else
                    ButtonUpdateGoodsInfo.IsEnabled = false;
            }
        }
        private void ButtonMakeItemsReport_Click(object sender, RoutedEventArgs e)
        {
            MakeReport(MainDataGrid);
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


       

        //очистка по клику на кнопку [очистить] combobox «Ответственный сотрудник»
        private void ButtonResetCBResponsibleWorker_Click(object sender, RoutedEventArgs e)
        {
            comboBoxItrmsResponsibleHuman.SelectedIndex = -1;
        }

       

        //очистка по клику на кнопку [очистить] combobox «Категория»
        private void ButtonResetCBCategory_Click(object sender, RoutedEventArgs e)
        {
            comboBoxItemsCategories.SelectedIndex = -1;
        }
    }
}
