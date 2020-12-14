using System;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
	/// <summary>
	/// Определяет пункт описания в категории
	/// </summary>
	public class Description : BindableBase, IData
	{
		/// <summary>
		/// Конструктор для объекта описания.
		/// </summary>
		public Description()
		{
			Id = -1;
		}

		/// <summary>
		/// Копирующий конструктор для объекта описания.
		/// </summary>
		/// <param name="description">Объект копирования.</param>
		public Description(Description description)
		{
			Id = description.Id;
			_title = description._title;
			_value = description._value;
			ValueChanged = description.ValueChanged;
		}

		public event Action<string, int> ValueChanged;

		/// <summary>
		/// Идентификатор описания категории.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Индекс для определения связи со значением.
		/// </summary>
		public int Index { get; set; }

		string _title;
		/// <summary>
		/// Заголовок пункт описания.
		/// </summary>
		public string Title
		{
			get => _title;
			set => SetValue(ref _title, value);
		}

		string _value;
		/// <summary>
		/// Значение пункта описания.
		/// </summary>
		public string Value
		{
			get => _value;
			set { SetValue(ref _value, value); ValueChanged?.Invoke(value, Index); }
		}

		/*--- Свойства и методы для интерфейса IData ---*/

		/// <summary>
		/// Значений первичного ключа.
		/// </summary>
		public object IdColumn => Id;

		/// <summary>
		/// Имя столбца значения первичного ключа.
		/// </summary>
		public string IdColumnName => "idDescription";

		/// <summary>
		/// Имя таблицы.
		/// </summary>
		public string Table => "Description";

		/// <summary>
		/// Получение данных об имени столбцах и их содержимом.
		/// </summary>
		/// <returns>Возвращает кортеж из имени столбца и его значения.</returns>
		public IEnumerable<(string, object)> GetData()
		{
			yield return ("Title", _title);
		}

		/// <summary>
		/// Заполнение данных по указанным столбцам.
		/// </summary>
		/// <param name="column">Имя столбца.</param>
		/// <param name="value">Значение столбца.</param>
		public void SetData(string column, object value)
		{
			if (column == "idDescription")
				Id = Convert.ToInt32(value);
			else if (column == "Title")
				Title = value as string;
		}
	}
}
