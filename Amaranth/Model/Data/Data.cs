using System.Collections;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
	public class Data : IEnumerable<TData>
	{
		Dictionary<string, object> _data;

		public int RecordId { get; set; }
		public string TableName { get; set; }

		public void Add(string name, object value)
		{
			_data.Add(name, value);
		}

        public void Remove(string name)
		{
			_data.Remove(name);
		}

		public IEnumerator<TData> GetEnumerator()
		{
			foreach (var data in _data)
				yield return new TData
				{
					Name = data.Key,
					Value = data.Key,
					Type = data.Key.GetType()
				};
		}

		IEnumerator IEnumerable.GetEnumerator()
        {
			return GetEnumerator();
		}
    }
}
