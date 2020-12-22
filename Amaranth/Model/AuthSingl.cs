using System;
using System.ComponentModel;
using Amaranth.Model.Data;

namespace Amaranth.Model
{
	/// <summary>
	/// Класс для работы с авторизацией пользователя.
	/// </summary>
	public class AuthSingl : INotifyPropertyChanged
	{
		/// <summary>
		/// Экземпляр данного класс для паттерна Singleton.
		/// </summary>
		static AuthSingl _intance;

		/// <summary>
		/// Ссылка на адаптер, для доступа к БД.
		/// </summary>
		IDBAdapter _adapter;

        //static string _salt = BCrypt.Net.BCrypt.GenerateSalt(); - команда для генерации соли
        /// <summary>
        /// Соль для алгоритма хеширования.
        /// </summary>
        readonly string _salt = "$2a$11$V3BYXXhVVRwTfYT5bbGyFu";

		/// <summary>
		/// Скрытый конструктор согласно паттерну Singleton.
		/// </summary>
		private AuthSingl()
		{
		}

		/// <summary>
		/// Пользователь, прошедший авторизацию в системе.
		/// </summary>
		public User CurrentUser { get; set; }

		/// <summary>
		/// Инициализация объекта паттерна Singleton.
		/// </summary>
		/// <returns>Объект класса авторизации.</returns>
		public static AuthSingl GetInstance()
		{
			return _intance ?? (_intance = new AuthSingl()); // Проверка, что не равно null
		}

		/// <summary>
		/// Установка адаптера, для доступа к БД.
		/// </summary>
		/// <param name="adapter">Объект адаптера.</param>
		public void SetAdapter(IDBAdapter adapter)
        {
			_adapter = adapter;
        }

		/// <summary>
		/// Авторизация пользователя в системы.
		/// </summary>
		/// <param name="login">Логин пользователя.</param>
		/// <param name="password">Пароль пользователя.</param>
		public void SignIn(string login, string password)
		{
			if (_adapter != null)
			{
				// Шифрование пароля
				string hash = BCrypt.Net.BCrypt.HashPassword(password, _salt);
                // Попытка получить объект
                var user = new User
                {
                    Login = $"'{login}'"
                };
                _adapter.LoadData(user, $"password = '{hash}'");
				if (user.LastName != null && user.FirstName != null)
				{
					// Запись данных пользователя
					CurrentUser = user;
					_intance.OnUserChanged(); // Оповещение для формы
				}
				else
					throw new Exception("Пароль или логин не подходят");
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
		}

		/// <summary>
		/// Авторизация пользователя в системе с установкой пароля.
		/// </summary>
		/// <param name="login">Логин пользователя.</param>
		/// <param name="password">Новый пароль пользователя.</param>
		public void SignInFirst(string login, string password)
		{
			if (_adapter != null)
			{
				// Шифрование пароля
				string hash = BCrypt.Net.BCrypt.HashPassword(password, _salt);
                // Попытка получить объект
                var user = new User
                {
                    Login = $"'{login}'"
                };
                _adapter.LoadData(user, "password = ''");
				if (user.LastName != null && user.FirstName != null)
				{
					// Запись данных пользователя
					CurrentUser = user;
					_adapter.Update("User", "Login", login, "Password", hash);
					_intance.OnUserChanged(); // Оповещение для формы
				}
				else
					throw new Exception("Пользователь на найден или уже ему установлен пароль");
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
		}

		/// <summary>
		/// Сброс информации о текущем пользователе.
		/// </summary>
		public void SignOut()
		{
			CurrentUser = null;
			_intance.OnUserChanged(); // Оповещение для формы
		}

		/// <summary>
		/// Сброс пароля для указанного пользователя.
		/// </summary>
		/// <param name="login">Логин пользователя.</param>
		public void ResetPassword(string login)
        {
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			_adapter.Update("User", "Login", login, "Password", "");
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event Action UserChanged;
		/// <summary>
		/// Оповещение о смене пользователя.
		/// </summary>
		void OnUserChanged()
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentUser"));
			UserChanged?.Invoke();
		}
	}
}
