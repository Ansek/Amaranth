using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для формы пользователей.
    /// </summary>
    public partial class UsersPage : UserControl
    {
        public UsersPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.UsersVM();
        }

        //Проверка на ввод строк не больше 32 символов
        private void TextBoxlLen32(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string t = ((TextBox)sender).Text + e.Text;
            if (t.Length > 32)
                e.Handled = true;
        }

        //Проверка на ввод строк не больше 64 символов с кириллицей
        private void TextBoxlLen32Rus(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[а-яА-Я]+$");
            string t = ((TextBox)sender).Text + e.Text;
            if (t.Length > 32 || !regex.IsMatch(t))
                e.Handled = true;
        }

        //Проверка на ввод строк не больше 64 символов с кириллицей
        private void TextBoxlLen64Rus(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[а-яА-Я-]+$");
            string t = ((TextBox)sender).Text + e.Text;
            if (t.Length > 64 || !regex.IsMatch(t))
                e.Handled = true;
        }
    }
}
