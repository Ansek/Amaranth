using System;
using System.Reflection;
using Word = Microsoft.Office.Interop.Word;

namespace Amaranth.Model
{
	public class Report
	{
		IDBAdapter _adapter;

		public Report(IDBAdapter adapter)
		{
			_adapter = adapter;
		}

		public void Print(Data.ProductRequest request)
		{
			object oMissing = Missing.Value;
			object oEndOfDoc = "\\endofdoc";

			Word._Application oWord;
			Word._Document oDoc;
			oWord = new Microsoft.Office.Interop.Word.Application();
			oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
			ref oMissing, ref oMissing);



			oWord.Visible = true;
		}
	}
}
