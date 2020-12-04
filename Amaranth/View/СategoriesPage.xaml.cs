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
    }
}
