using System.Collections.Generic;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы пользователей.
    /// </summary>
    class UsersVM : BindableBase
    {
        /// <summary>
        /// Для доступа к функциям БД.
        /// </summary>
        readonly DataBaseSinglFacade _db;

        /// <summary>
        /// Конструктор посредника для формы пользователей.
        /// </summary>
        public UsersVM()
        {
            _db = DataBaseSinglFacade.GetInstance(); // Получение экземпляра Singleton
            // Привязка к событию изменения списка пользователей
            _db.UserListChanged += () =>
            {
                ListUsers = _db.GetUserList();  // Обновление списка
                User = null;                    // Сброс текущей записи
            };
            // Устанока параметров по умолчанию
            _isSelect = false;
            ListUsers = _db.GetUserList();
        }

        bool _isSelect;
        /// <summary>
        /// Флаг, что продукт был получен путем выбора из списка.
        /// </summary>
        public bool IsSelect
        {
            get => _isSelect;
            set => SetValue(ref _isSelect, value);
        }

        User _user;
        /// <summary>
        /// Текущий пользователь.
        /// </summary>
        public User User
        {
            get => _user;
            set => SetValue(ref _user, value);
        }

        List<User> _listUsers;
        /// <summary>
        /// Список пользователей.
        /// </summary>
        public List<User> ListUsers
        {
            get => _listUsers;
            set => SetValue(ref _listUsers, value);
        }

        /// <summary>
        /// Установка значений выбранного пользователя.
        /// </summary>
        public Command<User> SetUser
        {
            get => new Command<User>((u) =>
            {
                User = new User(u);
                IsSelect = true;
            }, (u) => u != null);
        }

        /// <summary>
        /// Создание объекта нового пользователя.
        /// </summary>
        public Command Create
        {
            get => new Command(() =>
            {
                User = new User();
                IsSelect = false;
            });
        }

        /// <summary>
        /// Добавление информации о пользователе в БД.
        /// </summary>
        public Command Add
        {
            get => new Command(() =>
            {
                _db.Insert(_user);
            }, () => _user != null && !_isSelect);
        }

        /// <summary>
        /// Изменение информации о пользователе в БД.
        /// </summary>
        public Command Change
        {
            get => new Command(() =>
            {
                _db.Update(_user);
            }, () => _user != null && _isSelect);
        }

        /// <summary>
        /// Удаление информации о пользователе из БД.
        /// </summary>
        public Command Delete
        {
            get => new Command(() =>
            {
                if (DialogueService.ShowWarning("Вы действительно хотите удалить информацию о данном пользователе?"))
                    _db.Delete(_user);
            }, () => _user != null && _isSelect);
        }

        /// <summary>
        /// Сброс пароля для выбранного пользователя.
        /// </summary>
        public Command ResetPassword
        {
            get => new Command(() =>
            {
                Auth.ResetPassword(_user.Login);
                DialogueService.ShowInformation($"Пароль для пользователя {_user.Login} был сброшен.");
            }, () => _user != null && _isSelect);
        }
    }
}
