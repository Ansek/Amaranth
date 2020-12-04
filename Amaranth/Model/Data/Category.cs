using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Amaranth.Model.Data
{
	public class Category : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable<Description>
	{
		int _id;
		string _title;
		List<Description> _description;

		public Category()
        {
			_id = -1;
			_description = new List<Description>();
			DeletedIds = new List<int>();
		}

		public Category(Category category)
        {
			_id = category._id;
			_title = category._title;
			_description = new List<Description>(category._description);
			DeletedIds = new List<int>();
		}

		public List<int> DeletedIds { get; }

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

		public int Count => _description.Count;

		public void AddDescription(string title, int id = -1)
		{
			foreach (var desc in _description)
				if (desc.Title == title)
					throw new Exception("Имя '" + title + "' уже задано внутри Category");

			_description.Add(new Description()
			{
				Id = id,
				Title = title
			});

			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void RemoveDescription(Description description)
		{
			for (int i = 0; i < _description.Count; i++)
				if (_description[i].Id == description.Id && _description[i].Title == description.Title)
                {
					DeletedIds.Add(_description[i].Id);
					_description.RemoveAt(i);
					CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
					break;
				}
		}

		public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

        public IEnumerator<Description> GetEnumerator()
        {
			foreach (var desc in _description)
				yield return new Description(desc);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
