using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class UsersVM : INotifyPropertyChanged
    {
        bool _isSelect;
        public bool IsSelect
        {
            get => _isSelect;
            set { _isSelect = value; OnValueChanged(); }
        }

        User _user;
        public User User
        {
            get => _user;
            set { _user = value; OnValueChanged(); }
        }

        List<User> _listUsers;
        public List<User> ListUsers
        {
            get => _listUsers;
            set { _listUsers = value; OnValueChanged(); }
        }

        public UsersVM()
        {
            _isSelect = false;
            ListUsers = DataBaseSinglFacade.GetListUser();
        }

        public Command<User> SetUser
        {
            get => new Command<User>((u) =>
            {
                User = new User(u);
                IsSelect = true;
            }, (u) => u != null);
        }

        public Command Create
        {
            get => new Command(() =>
            {
                User = new User();
                IsSelect = false;
            });
        }

        public Command Add
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Insert(_user);
                ListUsers = DataBaseSinglFacade.GetListUser();
                User = null;
            }, () => _user != null && !_isSelect);
        }

        public Command Change
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Update(_user);
                ListUsers = DataBaseSinglFacade.GetListUser();
                User = null;
            }, () => _user != null && _isSelect);
        }

        public Command Delete
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Delete(_user);
                ListUsers = DataBaseSinglFacade.GetListUser();
                User = null;
            }, () => _user != null && _isSelect);
        }

        public Command ResetPassword
        {
            get => new Command(() => Auth.ResetPassword(_user.Login), () => _user != null && _isSelect);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
