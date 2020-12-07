using System.Windows;

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
            var mv = new ViewModel.LoginMV();
            DataContext = mv;
            mv.ClickOk += () => DialogResult = true;
        }
    }
}
