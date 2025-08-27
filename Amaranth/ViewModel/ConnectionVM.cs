using System;
using System.Windows.Controls;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для окна настройки соединения.
    /// </summary>
    public class ConnectionVM : BindableBase
    {
        /// <summary>
        /// Данные о полях соединения к БД.
        /// </summary>
        public ConnectionData Data { get; }

        ///
        public ConnectionVM()
        {
            Data = new ConnectionData();
        }

        /// <summary>
        /// Для оповоещения, что окно закрылось по нажатию OK. 
        /// </summary>
        public bool IsOk { get; private set; } = false;

        /// <summary>
        /// Оповещает, что нажата кнопка ОК.
        /// </summary>
        public event Action ClickOk;

        /// <summary>
        /// Нажатие кнопки OK.
        /// </summary>
        public Command<PasswordBox> Ok
        {
            get => new Command<PasswordBox>((pass) =>
            {
                Data.Password = pass.Password;
                IsOk = true;
                ClickOk?.Invoke();  // Закрытие окна
            }, (pass) => !(string.IsNullOrWhiteSpace(Data.Server) || string.IsNullOrWhiteSpace(Data.DBName) ||
                string.IsNullOrWhiteSpace(Data.UserName) || string.IsNullOrWhiteSpace(pass.Password)));
        }

        /// <summary>
        /// Нажатие кнопки Выйти.
        /// </summary>
        public Command Exit => new Command(() => Environment.Exit(0));
    }
}
