using System.Windows.Controls;

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
    }
}
