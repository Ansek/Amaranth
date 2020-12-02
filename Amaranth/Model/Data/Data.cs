using System.Collections;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
	public class Data : IEnumerable<TData>
	{
		Dictionary<string, object> _data;

		public int RecordId { get; set; }
		public string IdName { get; set; }
		public string TableName { get; set; }

		public Data()
        {
			_data = new Dictionary<string, object>();
        }

		public void Add(string name)
		{
			_data.Add(name, null);
		}

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
			var list = new List<KeyValuePair<string, object>>(_data);
			foreach (var data in list)
				yield return new TData
				{
					Name = data.Key,
					Value = data.Value,
					Type = data.Value?.GetType()
				};
		}

		IEnumerator IEnumerable.GetEnumerator()
        {
			return GetEnumerator();
		}

		public object this[string name]
        {
			get => _data[name];
			set => _data[name] = value;
		}
	}
}
