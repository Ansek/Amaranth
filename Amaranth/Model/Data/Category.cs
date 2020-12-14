using System;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
	/// <summary>
	/// Категория, определяет дополнительные поля для товара.
	/// </summary>
	public class Category : BindableBaseCollection<Description>, IData, IDataCollection
	{
		/// <summary>
		/// Конструктор для объекта категории.
		/// </summary>
		public Category()
		{
			_id = -1;
			DeletedIds = new List<int>();
		}

		/// <summary>
		/// Копирующий конструктор объекта для категории.
		/// </summary>
		/// <param name="category">Объект копирования.</param>
		public Category(Category category)
		{
			_id = category._id;
			_title = category._title;
			_list = new List<Description>(category._list); // Поле из BindableBaseCollection
			DeletedIds = new List<int>();
		}

		/// <summary>
		/// Список для хранения идентификатор пунктов описания, которые требуется удалить.
		/// </summary>
		public List<int> DeletedIds { get; }

		int _id;
		/// <summary>
		/// Идентификатор категории.
		/// </summary>
		public int Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}

		string _title;
		/// <summary>
		/// Заголовок категории.
		/// </summary>
		public string Title
		{
			get => _title;
			set => SetValue(ref _title, value);
		}

		/// <summary>
		/// Добавление пункта описания.
		/// </summary>
		/// <param name="title">Заголовок описания.</param>
		/// <param name="id">Идентификатор описания</param>
		public void AddDescription(string title, int id = -1)
		{
			// Проверка заголовка на уникальность
			foreach (var desc in _list)
				if (desc.Title == title)
					throw new Exception("Имя '" + title + "' уже задано внутри Category");

			// Добавления 
			_list.Add(new Description()
			{
				Id = id,
				Title = title
			});

			OnCollectionChanged();	// Оповещение формы об изменении
		}

		/// <summary>
		/// Удаление пункта описания из категории.
		/// </summary>
		/// <param name="description">Удаляемый пункт</param>
		public void RemoveDescription(Description description)
		{
			// Поиск удаляемого пункта
			for (int i = 0; i < _list.Count; i++)
				if (_list[i].Id == description.Id && _list[i].Title == description.Title)
				{
					DeletedIds.Add(_list[i].Id);	// Отметка пункта как удаляемого
					_list.RemoveAt(i);				// Удаление из основного списка
					OnCollectionChanged();			// Оповещение формы об изменении
					break;
				}
		}

		/*--- Свойства и методы для интерфейса IData ---*/

		/// <summary>
		/// Значений первичного ключа.
		/// </summary>
		public object IdColumn => _id;

		/// <summary>
		/// Имя столбца значения первичного ключа.
		/// </summary>
		public string IdColumnName => "idCategory";

		/// <summary>
		/// Имя таблицы.
		/// </summary>
		public string Table => "Category";

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
			if (column == "idCategory")
				Id = Convert.ToInt32(value);
			else if (column == "Title")
				Title = value as string;
		}

		/*--- Свойства и методы для интерфейса IDataCollection ---*/

		public string CollectionTable => $"CategoryDescriptions{_id}";

		/// <summary>
		/// Получение данных об элементе коллекции.
		/// </summary>
		/// <returns>Возвращает интерфейс на элемент.</returns>
		public IEnumerable<IData> GetDataCollection()
		{
			foreach (var el in _list)
				yield return el;
		}

		/// <summary>
		/// Передает данные для заполнения коллекции.
		/// </summary>
		/// <param name="data">Возвращает интерфейс на элемент.</param>
		public void SetDataCollection(IEnumerable<IData> data)
		{
			_list.Clear(); // Очистка от старых данных
			foreach (var el in data)
            {
				var dest = new Description();
				//dest.SetData(el.GetData()); // Копирование полученных данных
				_list.Add(dest);
			}
		}

		/*--- Скрывающий метод для BindableBaseCollection ---*/

		/// <summary>
		/// Возвращает шаблон для описания данной категории.
		/// </summary>
		/// <returns>Перечислитель для классов описания</returns>
		public new IEnumerator<Description> GetEnumerator()
		{
			foreach (var desc in _list)
				yield return new Description(desc);
		}
    }
}
