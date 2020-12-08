using System.Windows;
using Amaranth.View;

namespace Amaranth.Service
{
    class DialogueService
    {
        public static void ShowLoginWindow()
        {
            var view = new LoginWindow();
            view.ShowDialog();
        }

        public static void ShowError(string message)
        {
            if (message == "The connection is already open.")
                message = "Соединение с базой данных не было корректно закрыто. Пожалуйста, перезапустите приложение.";
            else if (message.Contains("You have an error in your SQL syntax;"))
                message = "Был совершен некорректный запрос к базе данных.";
            else if (message.Contains("a foreign key constraint fails"))
                message = "Не удается выполнить изменение с данной записью, так как она связана с другой.";
            MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool ShowWarning(string message)
        {
            return MessageBox.Show(message, "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        public static void ShowInformation(string message)
        {
            MessageBox.Show(message, "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
