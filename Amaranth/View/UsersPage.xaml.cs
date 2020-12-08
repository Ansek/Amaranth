using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : UserControl
    {
        public UsersPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.UsersVM();
        }

        private void TextBoxlLen32(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string t = ((TextBox)sender).Text + e.Text;
            if (t.Length > 32)
                e.Handled = true;
        }

        private void TextBoxlLen32Rus(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[а-яА-Я]+$");
            string t = ((TextBox)sender).Text + e.Text;
            if (t.Length > 32 || !regex.IsMatch(t))
                e.Handled = true;
        }

        private void TextBoxlLen64Rus(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[а-яА-Я-]+$");
            string t = ((TextBox)sender).Text + e.Text;
            if (t.Length > 64 || !regex.IsMatch(t))
                e.Handled = true;
        }
    }
}
