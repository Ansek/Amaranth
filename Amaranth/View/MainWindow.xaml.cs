using System.Windows;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для формы основного окна.
    /// </summary>
    public partial class MainWindow : Window
    {
        ///
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.MainWindowVM();
        }
    }
}
