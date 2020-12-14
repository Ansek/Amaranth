using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для формы категорий.
    /// </summary>
    public partial class СategoriesPage : UserControl
    {
        public СategoriesPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.СategoriesVM();
        }

        //Проверка на ввод строк не больше 32 символов
        private void TextBoxlLen32(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string t = ((TextBox)sender).Text + e.Text;
            if (t.Length > 32)
                e.Handled = true;
        }

        //Проверка на ввод строк не больше 64 символов
        private void TextBoxlLen64(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string t = ((TextBox)sender).Text + e.Text;
            if (t.Length > 64)
                e.Handled = true;
        }
    }
}
