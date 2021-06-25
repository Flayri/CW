using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace _1234
{
    /// <summary>
    /// Логика взаимодействия для AddItemWindow.xaml
    /// </summary>
    public partial class AddItemWindow : Window
    {
        WarehouseEntities ctx;
        SqlDataAdapter adapter;
        DataTable dt;
        string connectionStr;

        //Singleton
        private static AddItemWindow _instance;
        public static AddItemWindow GetInstance()
        {
            if (_instance == null) 
                _instance = new AddItemWindow();

            return _instance;
        }
        public static void ClearInstance()
        {
            if (_instance != null)
                _instance = null;
        }

        public AddItemWindow()
        {
            InitializeComponent();
            FillComboBox(comboBoxItrmsResponsibleHuman); 
            FillComboBox(comboBoxItemsCategory);
        }

        //Валидация
        private void Validation_TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                TextBoxValidation.TextChanged(tb);
                if (TextBoxValidation.TextChanged(textBoxInventItemsName) && TextBoxValidation.TextChanged(textBoxItemsAmmount) && TextBoxValidation.TextChanged(textBoxInventItemsPrice) && TextBoxValidation.TextChanged(textBoxItemsBuyDate) && TextBoxValidation.TextChanged(textBoxItemsLifeTime))
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
                    Items AddedItem = new Items();

                    //Название
                    AddedItem.ItemName = textBoxInventItemsName.Text.ToString();

                    //Кол-во
                    bool ammountIsParsed = int.TryParse(textBoxItemsAmmount.Text, out int _ammount);
                    if (!ammountIsParsed)
                        AddedItem.Ammount = null;
                    else
                        AddedItem.Ammount = _ammount;

                    //Цена
                    bool priceIsParsed = int.TryParse(textBoxInventItemsPrice.Text, out int _price);
                    if (!priceIsParsed)
                        AddedItem.Price = null;
                    else
                        AddedItem.Price = _price;

                    //Срок эксплуатации (число)
                    bool lifeTimeIsParsed = int.TryParse(textBoxItemsLifeTime.Text, out int _lifeTime);
                    if (!lifeTimeIsParsed)
                        AddedItem.LifeTime = null;
                    else
                        AddedItem.LifeTime = _lifeTime;

                   

                    //Обновление срока службы (дата списания)
                    if (textBoxItemsBuyDate.Text != "")
                    {
                        if (textBoxItemsLifeTime.Text != "")
                        {
                            DateTime buyDateFromTextBox = Convert.ToDateTime(textBoxItemsBuyDate.Text);
                            AddedItem.OffDate = buyDateFromTextBox.AddMonths(_lifeTime).ToString("dd.MM.yyyy");
                        }
                    }

                    //Дата покупки
                    AddedItem.BuyDate = textBoxItemsBuyDate.Text;

                    //Ответственный сотрудник
                    if (comboBoxItrmsResponsibleHuman.Text != "")
                    {
                        Employees employee = ctx.Employees.Where(x => x.Name == comboBoxItrmsResponsibleHuman.Text).FirstOrDefault();
                        if (employee != null)
                            AddedItem.FK_ResponsibleEmployee = employee.ID_Employee;
                    }

                   

                    //Категория
                    if (comboBoxItemsCategory.Text != "")
                    {
                        Categories category = ctx.Categories.Where(x => x.Name == comboBoxItemsCategory.Text).FirstOrDefault();
                        if (category != null)
                            AddedItem.FK_Category = category.ID;
                    }

                    ctx.Entry(AddedItem).State = EntityState.Added;
                    ctx.SaveChanges();

                }

                this.Close();
            }

            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        //Кнопка «Отмена» и «Закрыть»
        private void ButtonCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Заполнение combobox инфой из БД
        private void FillComboBox(ComboBox cb)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    connectionStr = ctx.Database.Connection.ConnectionString;
                    if (cb == comboBoxItrmsResponsibleHuman)
                    {
                        string sqlQuery = @"SELECT Name FROM Employees";
                        adapter = new SqlDataAdapter(sqlQuery, connectionStr);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        cb.ItemsSource = dt.DefaultView;
                        cb.DisplayMemberPath = dt.Columns["Name"].ToString();
                    }

                    

                    if(cb == comboBoxItemsCategory)
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

            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        //Отключение кнопки «Добавить» при запуске
        private void DisableAddBtn_WindowLoadedEvent(object sender, RoutedEventArgs e)
        {
            ButtonAdd.IsEnabled = false;
        }

        //Разрешить ввод только цифр
        private void TextBoxAllowOnlyDigits_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

    }
}
