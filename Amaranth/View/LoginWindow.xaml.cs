using System.Windows;
using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var mv = new ViewModel.LoginVM();
            DataContext = mv;
            mv.ClickOk += () => DialogResult = true;
        }

        private void TextBoxlLen32(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string t = string.Empty;
            if (sender.GetType() == typeof(TextBox))
                t = ((TextBox)sender).Text + e.Text;
            else
                t = ((PasswordBox)sender).Password + e.Text;
            if (t.Length > 32)
                e.Handled = true;
        }
    }
}
