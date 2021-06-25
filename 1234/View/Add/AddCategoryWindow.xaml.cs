using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace _1234
{
    /// <summary>
    /// Логика взаимодействия для AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {
        WarehouseEntities ctx;
        SQLCategoriesRepository SQLCategoriesRepository;
        //Singleton
        private static AddCategoryWindow _instance;
        public static AddCategoryWindow GetInstance()
        {
            if (_instance == null)
                _instance = new AddCategoryWindow();

            return _instance;
        }
        public static void ClearInstance()
        {
            if (_instance != null)
                _instance = null;
        }

        public AddCategoryWindow()
        {
            InitializeComponent();
            SQLCategoriesRepository = new SQLCategoriesRepository();
        }

        //Валидация
        private void Validation_TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                TextBoxValidation.TextChanged(tb);
                if (TextBoxValidation.TextChanged(textBoxCategoryName) && TextBoxValidation.TextChanged(textBoxCategoryDescription))
                    ButtonAdd.IsEnabled = true;
                else
                    ButtonAdd.IsEnabled = false;
            }
            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        //Кнопка «Добавить»
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    Categories category = new Categories
                    {
                        Name = textBoxCategoryName.Text.ToString(),
                        Description = textBoxCategoryDescription.Text
                    };

                    Categories isAlreadyExist = ctx.Categories.Where(x => x.Name == category.Name).FirstOrDefault();

                    if (isAlreadyExist != null)
                    {
                        MessageBox.Show($"Не удалось добавить! Категория «{category.Name}» уже существует", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }


                    else
                    {
                        SQLCategoriesRepository.CreateElement(category);
                        SQLCategoriesRepository.SaveElement();

                        this.Close();
                    }

                }

            }

            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        //Отключение кнопки «Добавить» при запуске окна
        private void DisableAddBtn_WindowLoadedEvent(object sender, RoutedEventArgs e)
        {
            ButtonAdd.IsEnabled = false;
        }

        //Кнопка «Отмена» и «Закрыть»
        private void ButtonCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }


    public class SQLCategoriesRepository : IRepository<Categories>
    {
        private WarehouseEntities db;
        public SQLCategoriesRepository()
        {
            this.db = new WarehouseEntities();
        }

        public IEnumerable<Categories> GetElementsList()
        {
            return db.Categories;
        }

        public Categories GetElement(int id)
        {
            return db.Categories.Find(id);
        }

        public void CreateElement(Categories categories)
        {
            db.Categories.Add(categories);
        }

        public void UpdateElement(Categories categories)
        {
            db.Entry(categories).State = EntityState.Modified;
        }

        public void DeleteElement(Categories categories)
        {
            Categories cat = db.Categories.Find(categories);
            if (cat != null)
                db.Categories.Remove(cat);
        }

        public void SaveElement()
        {
            db.SaveChanges();
        }

    }
    }
