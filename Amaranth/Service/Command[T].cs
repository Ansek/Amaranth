using System;
using System.Windows.Input;

namespace Amaranth.Service
{
    /// <summary>
    /// Команда для выполнения действий на форме.
    /// </summary>
    public class Command<T> : ICommand
    {
        /// <summary>
        /// Отслеживает изменения с целью проверки доступности команды.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        readonly Action<T> _execute;
        readonly Func<T, bool> _canExecute;
        /// <summary>
        /// Конструктор для объекта команды.
        /// </summary>
        /// <param name="execute">Действие на выполнение.</param>
        /// <param name="canExecute">Проверка доступности команды.</param>
        public Command(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Проверяет, может ли быть вызвана данная команда.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        /// <returns>Флаг доступности команды.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || parameter != null && _canExecute((T)parameter);
        }

        /// <summary>
        /// Выполнение задачи данной команды.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        public void Execute(object parameter)
        {
            try
            {
                _execute((T)parameter);
            }
            catch (Exception ex)
            {
                DialogueService.ShowError(ex.Message);
            }
        }
    }
}
