using System;
using System.Windows.Controls;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы авторизации.
    /// </summary>
    class LoginVM : BindableBase
    {
        /// <summary>
        /// Оповещает, что нажата кнопка ОК.
        /// </summary>
        public event Action ClickOk;

        string _login;
        /// <summary>
        /// Логин пользователя.
        /// </summary>
        public string Login
        {
            get => _login;
            set => SetValue(ref _login, value);
        }

        bool _isFirst;
        /// <summary>
        /// Флаг, что будет проводиться установка пароля.
        /// </summary>
        public bool IsFirst
        {
            get => _isFirst;
            set => SetValue(ref _isFirst, value);
        }

        /// <summary>
        /// Нажатие кнопки OK.
        /// </summary>
        public Command<PasswordBox> Ok
        {
            get => new Command<PasswordBox>((pass) =>
            {
                var auth = AuthSingl.GetInstance();
                if (_isFirst)
                    auth.SignInFirst(_login, pass.Password);    // Вход с установкой пароля
                else
                    auth.SignIn(_login, pass.Password);         // Стандартная проверка
                ClickOk?.Invoke();  // Оповещение, что нажата кнопка Ok
            }, (pass) => _login != string.Empty && pass.Password != string.Empty);
        }
    }
}
