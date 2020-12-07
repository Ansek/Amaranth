using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для ProductSearch.xaml
    /// </summary>
    public partial class ProductSearch : UserControl
    {
        public static readonly DependencyProperty SetProductProperty;

        static ProductSearch()
        {
            SetProductProperty = DependencyProperty.Register("SetProduct", typeof(ICommand), typeof(ProductSearch));
        }

        public ProductSearch()
        {
            InitializeComponent();            
            DataContext = new ViewModel.ProductSearchVM();
        }

        public ICommand SetProduct
        {
            get => (ICommand)GetValue(SetProductProperty);
            set { SetValue(SetProductProperty, value); }
        }
    }
}
