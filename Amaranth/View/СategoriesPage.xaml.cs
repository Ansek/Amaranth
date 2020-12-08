using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для СategoriesPage.xaml
    /// </summary>
    public partial class СategoriesPage : UserControl
    {
        public СategoriesPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.СategoriesVM();
        }

        private void TextBoxlLen32(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string t = ((TextBox)sender).Text + e.Text;
            if (t.Length > 32)
                e.Handled = true;
        }

        private void TextBoxlLen64(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string t = ((TextBox)sender).Text + e.Text;
            if (t.Length > 64)
                e.Handled = true;
        }
    }
}
