using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Amaranth.Model.Data
{
	public class Category : INotifyPropertyChanged, IEnumerable<Description>
	{
		int _id;
		string _title;
		List<string> _names;
		List<string> _titles;

		public Category()
        {
			_id = -1;
        }

		public int Id
		{
			get => _id;
			set { _id = value; OnValueChanged(); } 
		}

		public string Title
		{
			get => _title;
			set { _title = value; OnValueChanged(); }
		}

        public void AddDescription(string name, string title)
		{
			if (_names.Contains(name))
				throw new Exception("Имя " + name + "уже задано внутри Category");
			else
            {
				_names.Add(name);
				_titles.Add(name);
			}
		}

		public void RemoveDescription(string name)
		{
			int i = _names.IndexOf(name);
			if (_names.Contains(name))
            {
				_names.RemoveAt(i);
				_titles.RemoveAt(i);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnValueChanged([CallerMemberName] string name = "")
        {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

        public IEnumerator<Description> GetEnumerator()
        {
			for (int i = 0; i < _names.Count; i++)
				yield return new Description()
				{
					Name = _names[i],
					Title = _titles[i]
				};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
