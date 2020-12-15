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
			if (request.CheckTitle && request.Title == string.Empty)
				throw new Exception("Не было задано наименование для товара");

			int pos = 0;
			int count = 1;
			if (request.CheckRecordsCount)
				count = request.RecordsCount;
			string condition = request.GetCondition();

			var products = _adapter.LoadList("product_view", pos, count, condition);

			if (products.Count == 0)
				throw new Exception("Для данных параметров не было найдено ни одной записи");

			object oMissing = Missing.Value;
			object oEndOfDoc = "\\endofdoc";

			Word._Application oWord;
			Word._Document oDoc;
			oWord = new Microsoft.Office.Interop.Word.Application();
			oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
			ref oMissing, ref oMissing);

			Word.Paragraph oPara1;
			oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);
			oPara1.Range.Text = "Отчет о продажах";
			oPara1.Range.Font.Bold = 1;
			oPara1.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
			oPara1.CharacterUnitFirstLineIndent = 0;
			oPara1.FirstLineIndent = 0;
			oPara1.Format.SpaceAfter = 24;
			oPara1.Range.InsertParagraphAfter();

			if (request.CheckTitle)
			{
                Word.Paragraph oPara2;
                object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                oPara2 = oDoc.Content.Paragraphs.Add(ref oRng);
                oPara2.Range.Text = $"Данные для товара «{products[0]["Title"]}» \n";
				oPara2.Range.InsertParagraphAfter();

				string s = string.Empty;
				s += "Идентификатор - " + products[0]["idProduct"].ToString() + '\n';
				s += "Заголовок - " + products[0]["Title"].ToString() + '\n';
				s += "Цена - " + products[0]["Price"].ToString() + '\n';
				s += "Колицество - " + products[0]["Count"].ToString() + '\n';
				s += "По рецепту - " + products[0]["Prescription"].ToString() + '\n';
				s += "Номер категории - " + products[0]["idCategory"].ToString() + '\n';

				Word.Paragraph oPara3;
				oPara3 = oDoc.Content.Paragraphs.Add(ref oRng);
				oPara3.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
				oPara3.Format.SpaceAfter = 2;
				oPara3.Range.Font.Bold = 0;
				oPara3.Range.Text = s;
				oPara3.Range.InsertParagraphAfter();
			}
			else
			{
				Word.Paragraph oPara2;
				object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				oPara2 = oDoc.Content.Paragraphs.Add(ref oRng);
				oPara2.Range.Text = $"Данные для {products.Count} товаров";
				oPara2.Range.InsertParagraphAfter();

				string s = string.Empty;
				foreach (var p in products)
                {
					s += "Идентификатор - " + p["idProduct"].ToString() + '\n';
					s += "Заголовок - " + p["Title"].ToString() + '\n';
					s += "Цена - " + p["Price"].ToString() + '\n';
					s += "Колицество - " + p["Count"].ToString() + '\n';
					s += "По рецепту - " + p["Prescription"].ToString() + '\n';
					s += "Номер категории - " + p["idCategory"].ToString() + "\n\n\n";
				}

				Word.Paragraph oPara3;
				oPara3 = oDoc.Content.Paragraphs.Add(ref oRng);
				oPara3.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
				oPara3.Format.SpaceAfter = 2;
				oPara3.Range.Font.Bold = 0;
				oPara3.Range.Text = s;
				oPara3.Range.InsertParagraphAfter();
			}
			oWord.Visible = true;
		}
	}
}
