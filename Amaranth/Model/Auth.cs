using System;
using System.ComponentModel;
using Amaranth.Model.Data;
using data = Amaranth.Model.Data.Data;

namespace Amaranth.Model
{
	public class Auth : INotifyPropertyChanged
	{
		static Auth _intance;
		static IDBAdapter _adapter;
		//string _salt = BCrypt.Net.BCrypt.GenerateSalt();
		static string _salt = "$2a$11$V3BYXXhVVRwTfYT5bbGyFu";

		private Auth()
		{
		}

		public static User CurrentUser { get; set; }

		public static Auth GetInstance()
		{
			return _intance ?? (_intance = new Auth());
		}

		public static void SetAdapter(IDBAdapter adapter)
        {
			_adapter = adapter;
        }

		public static void SignIn(string login, string password)
		{
			if (_adapter != null)
			{
				string hash = null;
				// TO DO
				throw new NotImplementedException();
				_intance.OnUserChanged();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
		}

		public static void SignInFirst(string login, string password)
		{
			if (_adapter != null)
			{
				string hash = BCrypt.Net.BCrypt.HashPassword(password, _salt);
				// TO DO
				throw new NotImplementedException();
				_intance.OnUserChanged();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
		}

		public static void SignOut()
		{
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
				_intance.OnUserChanged();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
		}

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

		void OnUserChanged()
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentUser"));
		}
	}
}
