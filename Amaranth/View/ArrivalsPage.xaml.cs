using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для ArrivalsPage.xaml
    /// </summary>
    public partial class ArrivalsPage : UserControl
    {
        public ArrivalsPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.ArrivalsVM();
        }

        private void TextBoxNumber(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            int i;
            var t = ((TextBox)sender).Text + e.Text;
            if (!int.TryParse(t, out i))
                e.Handled = true;
        }
    }
}
