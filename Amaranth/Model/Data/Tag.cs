using System;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
    /// <summary>
    /// Для хранения параметра тега.
    /// </summary>
    public class Tag : BindableBase, IData, ICollectionItem
    {
        /// <summary>
        /// Конструктор для объекта тега.
        /// </summary>
        public Tag()
        {
            Id = -1;
        }

        /// <summary>
        /// Идентификатор описания категории.
        /// </summary>
        public int Id { get; set; }

        string _title;
        /// <summary>
        /// Заголовок тега.
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetValue(ref _title, value);
        }

		/*--- Свойства и методы для интерфейса IData ---*/

		/// <summary>
		/// Значений первичного ключа.
		/// </summary>
		public object IdColumn => Id;

		/// <summary>
		/// Имя столбца значения первичного ключа.
		/// </summary>
		public string IdColumnName => "idTag";

		/// <summary>
		/// Имя таблицы.
		/// </summary>
		public string Table => "Tag";

		/// <summary>
		/// Получение данных об имени столбцах и их содержимом.
		/// </summary>
		/// <returns>Возвращает кортеж из имени столбца и его значения.</returns>
		public IEnumerable<(string, object)> GetData()
		{
			// Если не идет операция по добавлению во внешнюю таблицу
			if (!IsAdd)
				yield return ("Title", _title);
		}

		/// <summary>
		/// Заполнение данных по указанным столбцам.
		/// </summary>
		/// <param name="column">Имя столбца.</param>
		/// <param name="value">Значение столбца.</param>
		public void SetData(string column, object value)
		{
			if (column == "idTag")
				Id = Convert.ToInt32(value);
			else if (column == "Title")
				Title = value as string;
		}

		/*--- Свойства и методы для интерфейса ICollectionItem ---*/

		/// <summary>
		/// Второе значение первичного ключа (составной ключ).
		/// </summary>
		public int IdItem => Id;

		/// <summary>
		/// Флаг для отметки добавляемого значения.
		/// </summary>
		public bool IsAdd { get; set; }

		/// <summary>
		/// Флаг для отметки удаляемого значения
		/// </summary>
		public bool IsDelete { get; set; }
	}
}
