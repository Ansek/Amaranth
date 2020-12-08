using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для ListOrdersPage.xaml
    /// </summary>
    public partial class ListOrdersPage : UserControl
    {
        public ListOrdersPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.ListOrdersVM();
        }
    }
}
