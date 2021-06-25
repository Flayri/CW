using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace _1234
{
    /// <summary>
    /// Логика взаимодействия для AddEmployeeWindow.xaml
    /// </summary>
    public partial class AddEmployeeWindow : Window
    {
        WarehouseEntities ctx;

        //Singleton
        private static AddEmployeeWindow _instance;
        public static AddEmployeeWindow GetInstance()
        {
            if (_instance == null)
                _instance = new AddEmployeeWindow();

            return _instance;
        }
        public static void ClearInstance()
        {
            if (_instance != null)
                _instance = null;
        }

        public AddEmployeeWindow()
        {
            InitializeComponent();
        }

        //Валидация
        private void Validation_TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                TextBoxValidation.TextChanged(tb);
                if (TextBoxValidation.TextChanged(textBoxEmployeeName) && TextBoxValidation.TextChanged(textBoxEmployeePhone) && TextBoxValidation.TextChanged(textBoxEmployeeEmail))
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
                    Employees employee = new Employees();

                    //Название
                    employee.Name = textBoxEmployeeName.Text.ToString();
                    employee.Phone = textBoxEmployeePhone.Text.ToString();
                    employee.Email = textBoxEmployeeEmail.Text.ToString();

                    Employees isAlreadyExist = ctx.Employees.Where(x => x.Name == employee.Name).FirstOrDefault();

                    if (isAlreadyExist != null)
                    {
                        MessageBox.Show($"Не удалось добавить! Сотрудник {employee.Name} уже в базе", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    else
                    {
                        ctx.Entry(employee).State = EntityState.Added;
                        ctx.SaveChanges();
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

}
