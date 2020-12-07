using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для ShowProductPage.xaml
    /// </summary>
    public partial class ShowProductPage : UserControl
    {
        public ShowProductPage(Model.Data.Product product)
        {
            InitializeComponent();
            DataContext = new ViewModel.ShowProductVM(product);
        }
    }
}
