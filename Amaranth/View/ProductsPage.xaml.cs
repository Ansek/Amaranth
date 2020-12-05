using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : UserControl
    {
        public ProductsPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.ProductsVM();
        }
    }
}
