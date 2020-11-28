using System;

namespace Amaranth.Model
{
	public class Report
	{
		IDBAdapter _adapter;

		public Report(IDBAdapter adapter)
		{
			_adapter = adapter;
		}

		public void Print(string condition)
		{
			throw new NotImplementedException();
		}
	}
}
