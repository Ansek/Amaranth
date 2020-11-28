using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Amaranth.Model.Data
{
	public class User : INotifyPropertyChanged
	{
		string _login, _firstName, _lastName;
		bool _isAdministrator;

		public string Login
		{
			get => _login;
			set { _login = value; OnValueChanged(); }
		}

		public string FirstName
		{
			get => _firstName;
			set { _firstName = value; OnValueChanged(); }
		}

		public string LastName
		{
			get => _lastName;
			set { _lastName = value; OnValueChanged(); }
		}

		public bool IsAdministrator
		{
			get => _isAdministrator;
			set { _isAdministrator = value; OnValueChanged(); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnValueChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
