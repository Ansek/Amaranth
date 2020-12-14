using System;
using System.ComponentModel;
using Amaranth.Model.Data;
using data = Amaranth.Model.Data.Data;

namespace Amaranth.Model
{
	/// <summary>
	/// Класс для работы с авторизацией пользователя.
	/// </summary>
	public class Auth : INotifyPropertyChanged
	{
		/// <summary>
		/// Экземпляр данного класс для паттерна Singleton.
		/// </summary>
		static Auth _intance;

		/// <summary>
		/// Ссылка на адаптер, для доступа к БД.
		/// </summary>
		static IDBAdapter _adapter;

		//static string _salt = BCrypt.Net.BCrypt.GenerateSalt(); - команда для генерации соли
		/// <summary>
		/// Соль для алгоритма хеширования.
		/// </summary>
		static string _salt = "$2a$11$V3BYXXhVVRwTfYT5bbGyFu";

		/// <summary>
		/// Скрытый конструтор согласно паттерну Singleton.
		/// </summary>
		private Auth()
		{
		}

		/// <summary>
		/// Пользователь прошедший авторизацицию в системе.
		/// </summary>
		public static User CurrentUser { get; set; }

		/// <summary>
		/// Инициализация объекта паттерна Singleton.
		/// </summary>
		/// <returns>Объект класса авторизации.</returns>
		public static Auth GetInstance()
		{
			return _intance ?? (_intance = new Auth()); // Проверка, что не равно null
		}

		/// <summary>
		/// Установка адаптера, для доступа к БД.
		/// </summary>
		/// <param name="adapter">Объект адаптера.</param>
		public static void SetAdapter(IDBAdapter adapter)
        {
			_adapter = adapter;
        }

		/// <summary>
		/// Авторизация пользователя в системы.
		/// </summary>
		/// <param name="login">Логин пользователя.</param>
		/// <param name="password">Пароль пользователя.</param>
		public static void SignIn(string login, string password)
		{
			if (_adapter != null)
			{
				// Шифрование пароля
				string hash = BCrypt.Net.BCrypt.HashPassword(password, _salt);
				// Попытка получить объект
				var data = _adapter.GetUser(login, hash);
				if (data != null)
				{
					// Получение данных
					CurrentUser = new User()
					{
						FirstName = Convert.ToString(data["FirstName"]),
						LastName = Convert.ToString(data["LastName"]),
						IsAdministrator = Convert.ToBoolean(data["IsAdministrator"])
					};
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
		public static void SignInFirst(string login, string password)
		{
			if (_adapter != null)
			{
				// Шифрование пароля
				string hash = BCrypt.Net.BCrypt.HashPassword(password, _salt);
				// Попытка получить объект
				var data = _adapter.GetUser(login, "");
				if (data != null)
                {
					// Получение данных
					CurrentUser = new User()
					{
						FirstName = Convert.ToString(data["FirstName"]),
						LastName = Convert.ToString(data["LastName"]),
						IsAdministrator = Convert.ToBoolean(data["IsAdministrator"])
					};

					// Обновление данных в БД (установка пароля)
					data.TableName = "user";
					data.IdName = "Login";
					data.RecordId = $"'{login}'";
					data.Clear();
					data.Add("Password", hash);
					_adapter.Update(data);
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
		public static void SignOut()
		{
			CurrentUser = null;
			_intance.OnUserChanged(); // Оповещение для формы
		}

		/// <summary>
		/// Сброс пароля для указанного пользователя.
		/// </summary>
		/// <param name="login">Логин пользователя.</param>
		public static void ResetPassword(string login)
        {
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("Password", "");
			data.TableName = "user";
			data.IdName = "Login";
			data.RecordId = $"'{login}'";
			_adapter.Update(data);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public static event Action UserChanged;
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
