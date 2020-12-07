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
    }
}
