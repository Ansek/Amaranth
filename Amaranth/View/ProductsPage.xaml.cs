using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для формы товаров.
    /// </summary>
    public partial class ProductsPage : UserControl
    {
        public ProductsPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.ProductsVM();
        }

        //Проверка на ввод строк не больше 64 символов
        private void TextBoxlLen64(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string t = ((TextBox) sender).Text + e.Text;
            if (t.Length > 64)
                e.Handled = true;
        }

        // Проверка на ввод вещественного вещественных значений
        private void TextBoxDouble(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^\d{1,8}(\.\d{0,2})?$");
            string t = ((TextBox)sender).Text + e.Text;
            if (!regex.IsMatch(t))
                e.Handled = true;
        }
    }
}
