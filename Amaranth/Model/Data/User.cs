using System;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
	/// <summary>
	/// Хранит информацию о пользователе.
	/// </summary>
	public class User : BindableBase, IData
	{
		/// <summary>
		/// Конструктор для объекта пользователя.
		/// </summary>
		public User()
        {
        }

		/// <summary>
		/// Копирующий конструктор для объекта описания.
		/// </summary>
		/// <param name="user">Объект копирования.</param>
		public User(User user)
        {
			_login = user._login;
			_firstName = user._firstName;
			_lastName = user._lastName;
			_isAdministrator = user._isAdministrator;
		}

		string _login;
		/// <summary>
		/// Значение логина пользователя.
		/// </summary>
		public string Login
		{
			get => _login;
			set => SetValue(ref _login, value);
		}

		string _firstName;
		/// <summary>
		/// Имя пользователя.
		/// </summary>
		public string FirstName
		{
			get => _firstName;
			set => SetValue(ref _firstName, value);
		}

        string _lastName;
		/// <summary>
		/// Фамилия пользователя.
		/// </summary>
		public string LastName
		{
			get => _lastName;
			set => SetValue(ref _lastName, value);
		}

        bool _isAdministrator;
		/// <summary>
		/// Флаг, является ли данный пользователь администратором.
		/// </summary>
		public bool IsAdministrator
		{
			get => _isAdministrator;
			set => SetValue(ref _isAdministrator, value);
		}

		/*--- Свойства и методы для интерфейса IData ---*/

		/// <summary>
		/// Значений первичного ключа.
		/// </summary>
		public object IdColumn => _login;

		/// <summary>
		/// Имя столбца значения первичного ключа.
		/// </summary>
		public string IdColumnName => "Login";

		/// <summary>
		/// Имя таблицы.
		/// </summary>
		public string Table => "User";

		/// <summary>
		/// Получение данных об имени столбцах и их содержимом.
		/// </summary>
		/// <returns>Возвращает кортеж из имени столбца и его значения.</returns>
		public IEnumerable<(string, object)> GetData()
		{
			yield return ("Login", _login);
			yield return ("FirstName", _firstName);
			yield return ("LastName", _lastName);
			yield return ("IsAdministrator", _isAdministrator);
		}

		/// <summary>
		/// Заполнение данных по указанным столбцам.
		/// </summary>
		/// <param name="column">Имя столбца.</param>
		/// <param name="value">Значение столбца.</param>
		public void SetData(string column, object value)
		{
			switch (column)
			{
				case "Login":
					Login = value as string;
					break;
				case "FirstName":
					FirstName = value as string;
					break;
				case "LastName":
					LastName = value as string;
					break;
				case "IsAdministrator":
					IsAdministrator = Convert.ToBoolean(value);
					break;
			}
		}
	}
}
