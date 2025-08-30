using System.Windows;
using Amaranth.View;
using Amaranth.Model.Data;

namespace Amaranth.Service
{
    /// <summary>
    /// Класс для вызова диалоговых окон в системе.
    /// </summary>
    class DialogueService
    {
        /// <summary>
        /// Вызов окна ввода данных для MySQL.
        /// </summary>
        public static ConnectionData ShowConnectionWindow()
        {
            var view = new ConnectionWindow();
            view.ShowDialog();
            return view.VM.Data;
        }

        /// <summary>
        /// Вызов окна для авторизации пользователя.
        /// </summary>
        public static void ShowLoginWindow()
        {
            var view = new LoginWindow();
            view.ShowDialog();
        }

        /// <summary>
        /// Вывод сообщения об ошибке.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        public static void ShowError(string message)
        {
            // Перевод стандартных ошибок.
            if (message == "The connection is already open.")
                message = "Соединение с базой данных не было корректно закрыто. Пожалуйста, перезапустите приложение.";
            else if (message.Contains("You have an error in your SQL syntax;"))
                message = "Был совершен некорректный запрос к базе данных.";
            else if (message.Contains("a foreign key constraint fails"))
                message = "Не удается выполнить изменение с данной записью, так как она связана с другой.";
            MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Вывод предупреждающего сообщения.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        public static bool ShowWarning(string message)
        {
            return MessageBox.Show(message, "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Вывод информационного сообщения.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        public static void ShowInformation(string message)
        {
            MessageBox.Show(message, "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
