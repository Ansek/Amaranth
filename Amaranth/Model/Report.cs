using System;
using System.Data;
using System.Reflection;
using Word = Microsoft.Office.Interop.Word;

namespace Amaranth.Model
{
	/// <summary>
	/// Класс для генерации отчёта.
	/// </summary>
	public class Report
	{
		/// <summary>
		/// Ссылка на адаптер, для доступа к БД.
		/// </summary>
		readonly IDBAdapter _adapter;

		/// <summary>
		/// Конструктор класса генерации отчёта. 
		/// </summary>
		/// <param name="adapter">Установка адаптера, для доступа к БД.</param>
		public Report(IDBAdapter adapter)
		{
			_adapter = adapter;
		}

		/// <summary>
		/// Запуск генерации отчёта.
		/// </summary>
		/// <param name="request">Запрос, по которому надо получить данные для отчёта.</param>
		public void Print(Data.ProductRequest request)
		{
			if (request.CheckTitle && request.Title == string.Empty)
				throw new Exception("Не было задано наименование для товара");

			string condition = request.GetCondition();

			var products = _adapter.LoadTable("sale_view", condition);

			if (products.Rows.Count == 0)
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
               // oPara2.Range.Text = $"Данные для товара «{products[0]["Title"]}» \n";
				oPara2.Range.InsertParagraphAfter();
				/*
								string s = string.Empty;
								s += "Идентификатор - " + products[0]["idProduct"].ToString() + '\n';
								s += "Заголовок - " + products[0]["Title"].ToString() + '\n';
								s += "Цена - " + products[0]["Price"].ToString() + '\n';
								s += "Колицество - " + products[0]["Count"].ToString() + '\n';
								s += "По рецепту - " + products[0]["Prescription"].ToString() + '\n';
								s += "Номер категории - " + products[0]["idCategory"].ToString() + '\n';*/
				string s = string.Empty;
				foreach (DataRow r in products.Rows)
                {
					foreach (var e in r.ItemArray)
						s += e.ToString() + '\n';
					s += "------------------\n";
				}

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
				//oPara2.Range.Text = $"Данные для {products.Count} товаров";
				oPara2.Range.InsertParagraphAfter();
				
	
				string s = string.Empty;
				foreach (DataRow r in products.Rows)
				{
					foreach (var e in r.ItemArray)
						s += e.ToString() + '\n';
					s += "------------------\n";
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

		/// <summary>
		/// Печать отчёта по одному товару.
		/// </summary>
		/// <param name="response">Результат запроса, по которому генерится отчёт.</param>
		private void PrintOne(DataTable response)
        {

        }

		/// <summary>
		/// Печать отчёта по нескольким товарам.
		/// </summary>
		/// <param name="response">Результат запроса, по которому генерится отчёт.</param>
		private void PrintMany(DataTable response)
		{

		}
	}
}
