using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для формы генерации отчётов.
    /// </summary>
    public partial class ReportsPage : UserControl
    {
        ///
        public ReportsPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.ReportsVM();
        }

        // Проверка на ввод целочисленных значений
        private void TextBoxNumber(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            int i;
            var t = ((TextBox)sender).Text + e.Text;
            if (!int.TryParse(t, out i))
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
