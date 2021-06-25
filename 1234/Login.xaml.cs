using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _1234
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        SqlDataAdapter adapter;
        DataTable dt;
        WarehouseEntities ctx;
        DispatcherTimer timer;
        DateTime time;
        public Login()
        {
            InitializeComponent();
        }


        private void LoginValidation_TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Подсветка некорректных полей
                TextBox tb = sender as TextBox;
                CanUserPressLoginButton(tb);

                if (btnLogin != null)
                {
                    if (CanUserPressLoginButton(LoginEmailTextBox))
                        btnLogin.IsEnabled = true;

                    else
                        btnLogin.IsEnabled = false;
                }
            }

            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private bool CanUserPressLoginButton(TextBox tb)
        {
            SolidColorBrush colorError = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 80, 80));
            SolidColorBrush colorCorrect = new SolidColorBrush(System.Windows.Media.Color.FromRgb(60, 179, 113));
            SolidColorBrush whiteColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));


            if (tb.Name == "LoginEmailTextBox" & BorderLoginAccountEmail != null)
            {
                if (tb.Text == "Email")
                {
                    BorderLoginAccountEmail.Background = whiteColor;
                    return false;
                }

                if (!Regex.IsMatch(tb.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    BorderLoginAccountEmail.Background = colorError;
                    return false;
                }

                BorderLoginAccountEmail.Background = colorCorrect;
            }

            return true;
        }
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush colorError = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 80, 80));
            SolidColorBrush colorCorrect = new SolidColorBrush(System.Windows.Media.Color.FromRgb(60, 179, 113));
            SolidColorBrush whiteColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

            try
            {
                using (ctx = new WarehouseEntities())
                {


                    //пользователь с ввёденными в TextBox данными
                    Users tryUserIndenify = new Users()
                    {
                        Email = LoginEmailTextBox.Text.ToString().Trim(),
                        Password = Security.Encrypt(LoginPasswordBox.Password.ToString().Trim())
                    };

                    //Есть ли такой пользоватеь в бд?
                    Users user = ctx.Users
                        .Where(x => x.Password == tryUserIndenify.Password && x.Email == tryUserIndenify.Email).FirstOrDefault();



                    if (user != null)
                    {

                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();

                        this.Close();
                        //изменение размера окна
                        //this.Height = 590;
                        //this.Width = 1500;
                        //this.MinHeight = 520;
                        //this.MinWidth = 1400;

                        //ResizeMode = ResizeMode.CanResize;
                        //WindowStyle = WindowStyle.SingleBorderWindow;
                        //CenterWindowOnScreen();
                        //HideAllUIElements();
                        //MakeElementsVisible("DashbroadGrid");

                        //timer = new DispatcherTimer();
                        //timer.Tick += new EventHandler(TimerTick);
                        //timer.Interval = new TimeSpan(0, 0, 0, 1);
                        //time = DateTime.Now;
                        //timer.Start();
                        // labelDate.Content = time.ToLongDateString();

                    }

                    else
                        MessageBox.Show($"Не верный email или пароль.", "Не удалось войти", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }

            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        //private void TimerTick(object sender, EventArgs e)
        //{
        //    labelTime.Content = DateTime.Now.Subtract(time).ToString(@"hh\:mm\:ss");
        //}

        private void RegisterValidation_TextChangedEvent(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Подсветка некорректных полей
                TextBox tb = sender as TextBox;
                CanUserPressRegButton(tb);

                if (ButtonRegisterAccount != null)
                {
                    if (CanUserPressRegButton(YourNameRegisterTextBox) && CanUserPressRegButton(EmailRegisterTextBox) && CanUserPressRegButton(PasswordRegisterTextBox))
                        ButtonRegisterAccount.IsEnabled = true;

                    else
                        ButtonRegisterAccount.IsEnabled = false;
                }
            }

            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool CanUserPressRegButton(TextBox tb)
        {
            SolidColorBrush colorError = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 80, 80));
            SolidColorBrush colorCorrect = new SolidColorBrush(System.Windows.Media.Color.FromRgb(60, 179, 113));
            SolidColorBrush whiteColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

            if (tb.Name == "YourNameRegisterTextBox" & BorderForNameRegisterTextBox != null)
            {
                if (!Regex.IsMatch(tb.Text, @"^[A-ЯЁ][а-яё]+$"))
                {
                    BorderForNameRegisterTextBox.Background = colorError;
                    return false;
                }

                if (tb.Text == "Ваше Имя")
                {
                    BorderForNameRegisterTextBox.Background = whiteColor;
                    return false;
                }

                BorderForNameRegisterTextBox.Background = colorCorrect;
            }

            if (tb.Name == "EmailRegisterTextBox" & BorderForEmailRegisterTextBox != null)
            {
                if (tb.Text == "Email")
                {
                    BorderForEmailRegisterTextBox.Background = whiteColor;
                    return false;
                }

                if (!Regex.IsMatch(tb.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    BorderForEmailRegisterTextBox.Background = colorError;
                    return false;
                }

                BorderForEmailRegisterTextBox.Background = colorCorrect;
            }

            if (tb.Name == "PasswordRegisterTextBox" & BorderForPasswordRegisterTextBox != null)
            {
                if (tb.Text == "Пароль")
                {
                    BorderForPasswordRegisterTextBox.Background = whiteColor;
                    return false;
                }

                if (!Regex.IsMatch(tb.Text, @"^[a-zA-Z0-9]+$"))
                {
                    BorderForPasswordRegisterTextBox.Background = colorError;
                    return false;
                }

                if (tb.Text.Length <= 4)
                {
                    BorderForPasswordRegisterTextBox.Background = colorError;
                    return false;
                }

                BorderForPasswordRegisterTextBox.Background = colorCorrect;
            }

            return true;
        }
        private void ButtonRegisterAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ctx = new WarehouseEntities())
                {
                    //создание пользователя с полями из TextBox
                    Users user = new Users()
                    {
                        Name = YourNameRegisterTextBox.Text.ToString().Trim(),
                        Email = EmailRegisterTextBox.Text.ToString().Trim(),
                        Password = Security.Encrypt(PasswordRegisterTextBox.Text.ToString().Trim())
                    };

                    ctx.Users.Add(user);
                    ctx.Entry(user).State = EntityState.Added;
                    ctx.SaveChanges();

                    MessageBox.Show($"Пользователь зарегистрирован. Теперь вы можете войти в аккаунт.", "Успешно добавлен", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }

            catch (Exception exteption)
            {
                MessageBox.Show($"Информация об ошибке: {exteption.Message}", "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
            private void ButtonIalreayHaveAnAccount_Click(object sender, RoutedEventArgs e)
            {
                RegisterGrid.Visibility = Visibility.Hidden;
                StartScreenLoginGrid.Visibility = Visibility.Visible;
            }

        private void RegisterYourNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= RegisterYourNameTextBox_GotFocus;
        }
        private void RegisterEmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= RegisterEmailTextBox_GotFocus;
        }
        private void RegisterPasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= RegisterPasswordTextBox_GotFocus;
        }

        private void LoginEmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= LoginEmailTextBox_GotFocus;
        }
        //сброс начальногр значения в PasswordBox
        private void LoginPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = (PasswordBox)sender;
            pb.Password = string.Empty;
            pb.GotFocus -= LoginPasswordBox_GotFocus;
        }

        private void ButtonIalreadyHaveNotAccount_Click(object sender, RoutedEventArgs e)
        {
            StartScreenLoginGrid.Visibility = Visibility.Collapsed;
            RegisterGrid.Visibility = Visibility.Visible;
        }
    }
}
