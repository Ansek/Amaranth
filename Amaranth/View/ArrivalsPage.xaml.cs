using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для формы поступления товаров.
    /// </summary>
    public partial class ArrivalsPage : UserControl
    {
        public ArrivalsPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.ArrivalsVM();
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
