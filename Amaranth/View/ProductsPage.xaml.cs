using System.Windows.Controls;
using System.Text.RegularExpressions;

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

        private void TextBoxlLen64(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string t = ((TextBox) sender).Text + e.Text;
            if (t.Length > 64)
                e.Handled = true;
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
