using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для формы заказа.
    /// </summary>
    public partial class OrderPage : UserControl
    {
        public OrderPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.OrderVM();
        }

        // Проверка на ввод целочисленных значений
        private void TextBoxNumber(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            int i;
            var t = ((TextBox)sender).Text + e.Text;
            if (!int.TryParse(t, out i))
                e.Handled = true;
        }
    }
}
