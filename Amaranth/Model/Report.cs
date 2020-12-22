using System;
using System.Collections.Generic;
using System.Data;
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
		/// Задает объект документа Word.
		/// </summary>
		Word.Document _doc;

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
			// Проверка на пустую строку имени
			if (request.CheckTitle && request.Title == string.Empty)
				throw new Exception("Не было задано наименование для товара");

			string condition = request.GetCondition();	// Формирование запроса информации о товарах
			var products = _adapter.LoadTable("sale_view", condition);	// Отправка запроса

			if (products.Rows.Count == 0)	// Если данные не были получены
				throw new Exception("Для данных параметров не было найдено ни одной записи");

			// Создание нового объекта документа
			var app = new Word.Application();
			_doc = app.Documents.Add();

			// Формирование текста заголовка
			string title;
			if (request.CheckTitle)
				title = $"Отчёт о продаже товара «{request.Title}»" + Convert.ToChar(11) +
					$"за период от {request.FromDate:dd MMM. yyyy} до {request.ToDate:dd MMM. yyyy}";
			else
				title = $"Отчёт о наиболее продаваемых товаров" + Convert.ToChar(11) +
					$"за период от {request.FromDate:dd MMM. yyyy} до {request.ToDate:dd MMM. yyyy}";
			
			var parag = _doc.Paragraphs.Add();	// Новый абзац заголовка
			parag.Range.Font.Bold = 1;			// Выделение заголовка
			parag.Range.Font.Size = 16;			// Размер текста заголовка
			parag.Range.Text = title;			// Установка текста заголовка
			// Установка заголовка по середине
			parag.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
			// Обнуление отступа текста
			parag.Range.ParagraphFormat.FirstLineIndent = 0;

			parag.Range.InsertParagraphAfter();	// Новая строка
			// Продолжение текста с левого краю
			parag.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
			parag.Range.Font.Size = 14;			// Размер основного текста
			parag.Range.InsertParagraphAfter();	// Новая строка

			// Установка сведений о поиске
			if (request.CheckRecordsCount)
				TextFormat("Количество записей для отбора", $"{request.RecordsCount}");

			// Установка сведений о	категории
			if (request.CheckCategory)
				TextFormat("Выбранная категория", $"{request.Category}");

			// Установка сведений о	цене
			if (request.CheckPrice)
				TextFormat("Выбранный диапазон стоимости", $"от {request.FromPrice} до {request.ToPrice} рублей");

			// Установка сведений о	тегах
			if (request.CheckTags && request.Tags.Count > 0)
			{
				string tags = string.Empty;
				foreach (var t in request.Tags)
                {
					if (tags != string.Empty)
						tags += ", ";
					tags += t.Title;
				}
				TextFormat("Выбранные теги", tags);
			}

			// Установка соответвующей таблицы
			if (request.CheckRecordsCount)
				PrintMany(products);
			else
				PrintOne(products);

			app.Visible = true;		// Отображение сгенерированного документа
			_doc = null;
		}

		/// <summary>
		/// Печать отчёта по одному товару.
		/// </summary>
		/// <param name="response">Результат запроса, по которому генерится отчёт.</param>
		private void PrintOne(DataTable response)
        {
			var parag = _doc.Paragraphs.Add();  // Новый абзац для таблицы
			// Создание таблицы
			var table = _doc.Tables.Add(parag.Range, response.Rows.Count + 1, 3);
			// Установка типа линии
			table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
			// Установка вертикального положения текста
			table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
			table.Range.Font.Size = 12;     // Размер текста
			table.Range.Font.Bold = 0;		// Сброс выделения по умолчанию
			// Сброс отступов
			table.Range.ParagraphFormat.CharacterUnitFirstLineIndent = 0;
			table.Range.ParagraphFormat.FirstLineIndent = 0;
			// Установка нулевого интервала между строками
			table.Range.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle;

			// Установка ширины колонок
			table.Columns[1].Width = 120;
			table.Columns[2].Width = 100;
			table.Columns[2].Width = 100;
			// Установка заголовков
			table.Cell(1, 1).Range.Text = "Дата продажи";
			table.Cell(1, 2).Range.Text = "Количество проданных товаров";
			table.Cell(1, 3).Range.Text = "Стоимость";

			table.Rows[1].Range.Bold = 1;	// Выделение заголовков
			// Центрирование заголовков
			table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

			int i = 2;
			double TotalCost = 0;
            // Преобразование для сортировки по датам
            var dv = new DataView(response)
            {
                Sort = "Date"
            };
            // Вывод столбцов
            foreach (DataRow r in dv.ToTable().Rows)
			{
				double tc = Convert.ToDouble(r["Cost"]);
				TotalCost += tc;    // Расчет итоговой стоимости
				// Установка значений
				if (r["Date"] is DateTime)
					table.Cell(i, 1).Range.Text = $"{Convert.ToDateTime(r["Date"]):dd MMMM yyyy}";
				table.Cell(i, 2).Range.Text = r["Count"] + " шт.";
				table.Cell(i, 3).Range.Text = $"{tc:0.00} руб.";
				// Установка положения
				table.Cell(i, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
				table.Cell(i, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
				table.Cell(i, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
				i++;
			}
			parag.Range.InsertParagraphAfter(); // Новая строка
			parag.Range.Font.Size = 14;         // Возвращение размера

			// Вывод итоговой стоимости
			TextFormat("Итоговая стоимость", $"{TotalCost:0.00} руб.");
		}

		/// <summary>
		/// Печать отчёта по нескольким товарам.
		/// </summary>
		/// <param name="response">Результат запроса, по которому генерится отчёт.</param>
		private void PrintMany(DataTable response)
		{
			// Подсчет уникальных товаров
			var dict = new Dictionary<string, (int, double)>();
			foreach (DataRow r in response.Rows)
			{
				// Получение значений
				string title = r["Title"].ToString();
				int count = Convert.ToInt32(r["Count"]);
				double cost = Convert.ToDouble(r["Cost"]);
				if (dict.ContainsKey(title))
				{
					// Перересчет количества и стоимости
					count += dict[title].Item1;
					cost += dict[title].Item2;
					dict[title] = (count, cost);
				}
				else
					dict.Add(title, (count, cost));
			}

			var parag = _doc.Paragraphs.Add();  // Новый абзац для таблицы
												// Создание таблицы
			var table = _doc.Tables.Add(parag.Range, dict.Count + 1, 4);
			// Установка типа линии
			table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
			// Установка вертикального положения текста
			table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
			table.Range.Font.Size = 12;     // Размер текста
			table.Range.Font.Bold = 0;      // Сброс выделения по умолчанию
											// Сброс отступов
			table.Range.ParagraphFormat.CharacterUnitFirstLineIndent = 0;
			table.Range.ParagraphFormat.FirstLineIndent = 0;
			// Установка нулевого интервала между строками
			table.Range.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle;

			// Установка ширины колонок
			table.Columns[1].Width = 20;
			table.Columns[2].Width = 120;
			table.Columns[3].Width = 100;
			table.Columns[4].Width = 100;
			// Установка заголовков
			table.Cell(1, 1).Range.Text = "№";
			table.Cell(1, 2).Range.Text = "Наименование";
			table.Cell(1, 3).Range.Text = "Количество проданных товаров";
			table.Cell(1, 4).Range.Text = "Стоимость";

			table.Rows[1].Range.Bold = 1;   // Выделение заголовков
											// Центрирование заголовков
			table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

			int i = 2;
			double TotalCost = 0;
			// Вывод столбцов
			foreach (var r in dict)
			{
				double tc = Convert.ToDouble(r.Value.Item2); // Стоимость
				TotalCost += tc;    // Расчет итоговой стоимости
				// Установка значений
				table.Cell(i, 1).Range.Text = $"{i - 1}";
				table.Cell(i, 2).Range.Text = r.Key;
				table.Cell(i, 3).Range.Text = $"{r.Value.Item1:0.00} шт."; // Количество
				table.Cell(i, 4).Range.Text = $"{tc:0.00} руб.";
				// Установка положения
				table.Cell(i, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
				table.Cell(i, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
				table.Cell(i, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
				table.Cell(i, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
				i++;
			}
			parag.Range.InsertParagraphAfter(); // Новая строка
			parag.Range.Font.Size = 14;         // Возвращение размера

			// Вывод итоговой стоимости
			TextFormat("Итоговая стоимость", $"{TotalCost:0.00} руб.");
		}

		/// <summary>
		/// Устанавливает текст в формате "заголовок: значение"
		/// </summary>
		/// <param name="title">Заголовок.</param>
		/// <param name="value">Значение.</param>
		private void TextFormat(string title, string value)
        {
			var parag = _doc.Paragraphs.Add();  // Новый абзац
			parag.Range.Text = $"{title}: {value}";
			parag.Range.Font.Bold = 1;			// Выделение заголовочной части
			var range = parag.Range;            // Установка абзаца для выделения
			range.SetRange(range.End - value.Length - 1, range.End);
			range.Font.Bold = 0;                // Сброс для основной части
			parag.Range.InsertParagraphAfter(); // Новая строка
		}
	}
}
