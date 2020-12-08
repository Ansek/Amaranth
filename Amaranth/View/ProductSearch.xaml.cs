using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text.RegularExpressions;

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

        private void TextBoxDouble(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^\d{1,8}(\.\d{0,2})?$");
            string t = ((TextBox)sender).Text + e.Text;
            if (!regex.IsMatch(t))
                e.Handled = true;
        }
    }
}
