using System;
using System.Windows;
using System.Windows.Controls;

namespace Amaranth.View
{
    /// <summary>
    /// Логика взаимодействия для формы авторизации
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        ///
        public ConnectionWindow()
        {
            InitializeComponent();
            VM = new ViewModel.ConnectionVM();
            DataContext = VM;
            VM.ClickOk += () => DialogResult = true;
        }

        /// <summary>
        /// Класс-посредник с данными.
        /// </summary>
        public ViewModel.ConnectionVM VM { get; } 

        //Проверка на ввод строк не больше 32 символов
        private void TextBoxlLen32(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string t = string.Empty;
            if (sender is TextBox) // Определени элемента управления
                t = ((TextBox)sender).Text + e.Text;
            else
                t = ((PasswordBox)sender).Password + e.Text;
            if (t.Length > 32)
                e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!VM.IsOk)
                Environment.Exit(0);
        }
    }
}
