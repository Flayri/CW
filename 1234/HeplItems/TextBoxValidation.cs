using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;


namespace _1234
{
    class TextBoxValidation
    {
        static readonly SolidColorBrush colorError = new SolidColorBrush(Color.FromRgb(255, 197, 197));
        public static bool TextChanged(TextBox tb)
        {

            if (tb.Name == "textBoxInventItemsName")
            {
                if (tb.Text.Length < 2)
                {
                    tb.Background = colorError;
                    return false;
                }

                tb.Background = Brushes.White;
            }

            if (tb.Name == "textBoxItemsAmmount")
            {
                if (!Regex.IsMatch(tb.Text, @"^[0-9]+$") & tb.Text.Length >= 1)
                {
                    tb.Background = colorError;
                    return false;
                }

                tb.Background = Brushes.White;
            }

            if (tb.Name == "textBoxInventItemsPrice" & tb.Text.Length >= 1)
            {
                if (!Regex.IsMatch(tb.Text, @"^[0-9]+$"))
                {
                    tb.Background = colorError;
                    return false;
                }

                tb.Background = Brushes.White;
            }

            if (tb.Name == "textBoxItemsBuyDate" & tb.Text.Length >= 1)
            {

                if (!Regex.IsMatch(tb.Text, @"^(0[1-9]|[12][0-9]|3[01])[.](0[1-9]|1[012])[.](19|20)\d\d$"))
                {
                    tb.Background = colorError;
                    return false;
                }

                //Дата не должна быть > сегодняшней
                DateTime dateFromTB = Convert.ToDateTime(tb.Text);
                if (dateFromTB > DateTime.Now)
                {
                    tb.Background = colorError;
                    return false;
                }

                tb.Background = Brushes.White;
            }

            if (tb.Name == "textBoxItemsLifeTime" & tb.Text.Length >= 1)
            {
                if (!Regex.IsMatch(tb.Text, @"^[0-9]+$"))
                {
                    tb.Background = colorError;
                    return false;
                }

                bool lifeTimeIsParsed = int.TryParse(tb.Text, out int _lifeTime);
                if (lifeTimeIsParsed)
                {
                    if (_lifeTime >= 12000)
                    {
                        tb.Background = colorError;
                        return false;
                    }

                }

                tb.Background = Brushes.White;
            }

            //СОТРУДНИКИ
            if (tb.Name == "textBoxEmployeeName")
            {
                if (!Regex.IsMatch(tb.Text, @"^[A-ЯЁ][а-яё]+\s[A-ЯЁ][а-яё]+$"))
                {
                    tb.Background = colorError;
                    return false;
                }

                tb.Background = Brushes.White;
            }

            if (tb.Name == "textBoxEmployeePhone" & tb.Text.Length >= 1)
            {
                if (!Regex.IsMatch(tb.Text, @"^\+375 \((17|29|33|44)\) [0-9]{3}-[0-9]{2}-[0-9]{2}$"))
                {
                    tb.Background = colorError;
                    return false;
                }

                tb.Background = Brushes.White;
            }

            if (tb.Name == "textBoxEmployeeEmail" & tb.Text.Length >= 1)
            {
                if (!Regex.IsMatch(tb.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    tb.Background = colorError;
                    return false;

                }

                tb.Background = Brushes.White;
            }

            if (tb.Name == "textBoxEmployeePhone" || tb.Name == "textBoxEmployeeEmail" && tb.Text == "")
            {
                tb.Background = Brushes.White;
            }

           
            if (tb.Name == "textBoxDepartmentEmail" & tb.Text.Length >= 1)
            {
                if (!Regex.IsMatch(tb.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    tb.Background = colorError;
                    return false;
                }

                tb.Background = Brushes.White;
            }

            if (tb.Name == "textBoxDepartmentName")
            {
                if (!Regex.IsMatch(tb.Text, @"^(\w+\s)*\w+$"))
                {
                    tb.Background = colorError;
                    return false;
                }

                tb.Background = Brushes.White;
            }

            //ФОРМА РЕГИСТРАЦИИ 
            if (tb.Name == "YourNameRegisterTextBox" & tb.Text.Length >= 1)
            {
                //if (!Regex.IsMatch(tb.Text, @"^[а-яё]+$"))
                //{
                //    BorderForNameRegisterTextBox.Background = colorError;
                //    return false;
                //}

                //tbBorderForNameRegisterTextBoxBackground = Brushes.White;
            }

            //КАТЕГОРИЯ
            if (tb.Name == "textBoxCategoryName" & tb.Text.Length >= 1)
            {
                if (!Regex.IsMatch(tb.Text, @"^(\w+\s)*\w+$"))
                {
                    tb.Background = colorError;
                    return false;
                }

                tb.Background = Brushes.White;
            }

            if (tb.Name == "textBoxCategoryName" & tb.Text.Length == 0)
            {
                tb.Background = colorError;
                return false;
            }


            if (tb.Name == "textBoxCategoryDescription" & tb.Text.Length >= 1)
            {
                //if (!Regex.IsMatch(tb.Text, @"^(\w+\s+[,])*\w+$"))
                //{
                //    tb.Background = colorError;
                //    return false;
                //}

                if (tb.Text == "")
                    tb.Background = Brushes.White;

                tb.Background = Brushes.White;
            }

            //Фильтр
            if (tb.Name == "textBoxItemsDateOffSorting" & tb.Text.Length >= 1)
            {
                if (!Regex.IsMatch(tb.Text, @"^(0[1-9]|[12][0-9]|3[01])[.](0[1-9]|1[012])[.](19|20)\d\d$"))
                {
                    tb.Background = colorError;
                    return false;
                }



                tb.Background = Brushes.White;
            }

            if (tb.Name == "textBoxItemsDateOffSorting" && tb.Text == "")
            {
                tb.Background = Brushes.White;
            }

            return true;
        }

    }
}
