using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Amaranth.Model.Data
{
	public class Description : INotifyPropertyChanged
	{
		string _title, _value;

		public int Id { get; set; }

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

		public Description()
        {
			Id = -1;
        }

		public Description(Description description)
		{
			Id = description.Id;
			_title = description._title;
			_value = description._value;
		}

		public event Action<string> ValueChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnValueChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
