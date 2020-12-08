using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class LoginVM : INotifyPropertyChanged
    {
        string _login;
        public string Login
        {
            get => _login;
            set { _login = value; OnValueChanged(); }
        }

        bool _isFirst;
        public bool IsFirst
        {
            get => _isFirst;
            set { _isFirst = value; OnValueChanged(); }
        }

        public event Action ClickOk;

        public Command<PasswordBox> Ok
        {
            get => new Command<PasswordBox>((pass) =>
            {
                if (_isFirst)
                    Auth.SignInFirst(_login, pass.Password);
                else
                    Auth.SignIn(_login, pass.Password);
                ClickOk?.Invoke();
            }, (pass) => _login != string.Empty && pass.Password != string.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
