using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Amaranth.Model.Data
{
	public class Description : INotifyPropertyChanged
	{
		string _name, _title, _value;

		public string Name
		{
			get => _name;
			set { _name = value; OnValueChanged(); }
		}

		public string Title
		{
			get => _title;
			set { _title = value; OnValueChanged(); }
		}

		public string Value
		{
			get => _value;
			set { _value = value; OnValueChanged(); }
		}

		public event Action<string> ValueChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnValueChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
