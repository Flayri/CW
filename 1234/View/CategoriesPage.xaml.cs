using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;


namespace _1234
{
    /// <summary>
    /// Логика взаимодействия для CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        SqlDataAdapter adapter;
        DataTable dt;
        WarehouseEntities ctx;

        public CategoriesPage()
        {
            InitializeComponent();
            DataContext = this;
            FillDataGrid("CategoriesTable");
        }

        public void FillDataGrid(string fillForWhatDG)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    string connectionStr = ctx.Database.Connection.ConnectionString;


                    if (fillForWhatDG == "CategoriesTable")
                    {
                        string sqlQuery = @"SELECT * FROM CategoriesView";
                        adapter = new SqlDataAdapter(sqlQuery, connectionStr);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        CategoryesDataGrid.ItemsSource = dt.DefaultView;
                        //CategoryesDataGrid.Columns[0].Visibility = Visibility.Hidden;
                        //CategoryesDataGrid.Columns[1].Width = 125;
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


                    //Логика обновления КАТЕГОРИИ 
                    if (WhatSave == "Categories")
                    {
                        if (CategoryesDataGrid.SelectedCells.Count == 3)
                        {
                            DataRowView rowView = CategoryesDataGrid.SelectedItem as DataRowView;
                            //id выделенной строки
                            int selectedItem_ID = Convert.ToInt32(rowView.Row[0]);

                            Categories category = new Categories();

                            //поиск обновляемого элемента в контексте по ID (Primary key)
                            category = ctx.Categories.Where(x => x.ID == selectedItem_ID).FirstOrDefault();
                            category.Name = textBoxCategoryName.Text.ToString();
                            category.Description = textBoxCategoryDescription.Text.ToString();


                            ctx.Entry(category).State = EntityState.Modified;
                            ctx.SaveChanges();

                            FillDataGrid("CategoriesTable");

                        }
                    }

                }
            }
            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }



        private void ButtonAddCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddCategoryWindow addCategoryWindow = AddCategoryWindow.GetInstance();

                if (addCategoryWindow != null)
                {
                    addCategoryWindow.Show();
                    addCategoryWindow.Topmost = true;

                    addCategoryWindow.Closed += (object s, EventArgs EvArgs) =>
                    {
                        FillDataGrid("CategoriesTable");
                        AddCategoryWindow.ClearInstance();
                    };
                }

            }
            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void ButtonUpdateCategoryInfo_Click(object sender, RoutedEventArgs e)
        {
            UpdateAndSaveChanges("Categories");
        }



        private void ButtonDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    if (CategoryesDataGrid.SelectedCells.Count == 3)
                    {
                        DataRowView rowView = CategoryesDataGrid.SelectedItem as DataRowView;

                        //id выделенной строки (Primary key)
                        int selectedCategory_ID = Convert.ToInt32(rowView.Row[0]);

                        //сотрудник которого надо удалить
                        Categories del_category = new Categories();

                        //поиск удаляемого сотрудника в контексте по ID (Primary key)
                        del_category = ctx.Categories.Where(x => x.ID == selectedCategory_ID).FirstOrDefault();

                        if (del_category != null)
                        {
                            //есть ли вещи связанные в с этим сотрудником в таблице InventItems (FK)?
                            var removeThisItems = ctx.Items.Where(x => x.FK_Category == selectedCategory_ID);

                            //есть вещи
                            if (removeThisItems.Count() >= 1)
                            {
                                MessageBoxResult result = MessageBox.Show($"В отделе сейчас находится имущество. Удалить отдел и сбросить критерий «Категория» у имущества находящегося в данной категории?", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                                if (result == MessageBoxResult.OK)
                                {
                                    foreach (var item in removeThisItems)
                                    {
                                        if (item.FK_Category == selectedCategory_ID)
                                            item.FK_Category = null;
                                    }

                                    ctx.Categories.Remove(del_category);
                                    ctx.Entry(del_category).State = EntityState.Deleted;
                                    ctx.SaveChanges();
                                    FillDataGrid("CategoriesTable");
                                }
                            }

                            //нету вещей
                            else if (removeThisItems.Count() < 1)
                            {
                                //удаление сотрудника
                                ctx.Categories.Remove(del_category);
                                ctx.Entry(del_category).State = EntityState.Deleted;
                                ctx.SaveChanges();
                                //обновление DataGrid для просмотра изменений
                                FillDataGrid("CategoriesTable");
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

        private void CategoryValidation_TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            if (CategoryesDataGrid.SelectedCells.Count == 3)
            {
                TextBox tb = sender as TextBox;
                TextBoxValidation.TextChanged(tb);
                if (TextBoxValidation.TextChanged(textBoxCategoryName) && TextBoxValidation.TextChanged(textBoxCategoryDescription))
                    ButtonUpdateCategoryInfo.IsEnabled = true; //отключение кнопки обновить
                else
                    ButtonUpdateCategoryInfo.IsEnabled = false;
            }
        }

    }
}
