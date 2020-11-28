using System;
using System.ComponentModel;
using Amaranth.Model.Data;

namespace Amaranth.Model
{
	public class Auth : INotifyPropertyChanged
	{
		static Auth _intance;
		static IDBAdapter _adapter;

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

		public event PropertyChangedEventHandler PropertyChanged;

		void OnUserChanged()
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentUser"));
		}
	}
}
