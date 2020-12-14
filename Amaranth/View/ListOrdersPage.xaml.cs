using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для формы списка заказов.
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
