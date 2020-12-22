using System;
using System.Windows.Input;

namespace Amaranth.Service
{
    /// <summary>
    /// Команда для выполнения действий на форме.
    /// </summary>
    public class Command : ICommand
    {
        /// <summary>
        /// Отслеживает изменения с целью проверки доступности команды.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        readonly Action _execute;
        readonly Func<bool> _canExecute;
        /// <summary>
        /// Конструктор для объекта команды.
        /// </summary>
        /// <param name="execute">Действие на выполнение.</param>
        /// <param name="canExecute">Проверка доступности команды.</param>
        public Command(Action execute, Func<bool> canExecute = null)
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
            return _canExecute == null || _canExecute();
        }

        /// <summary>
        /// Выполнение задачи данной команды.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        public void Execute(object parameter)
        {
            try
            {
                _execute();
            }
            catch (Exception ex)
            {
                DialogueService.ShowError(ex.Message);
            }
        }
    }
}
