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
		}

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
			int i = 0;
			// Проверка заголовка на уникальность
			foreach (var desc in _list)
            {
				if (desc.Title == title)
                {
					// Если есть, но помечена как удаленная
					if (desc.IsDelete)
                    {
						desc.IsDelete = false;
						OnCollectionChanged();  // Оповещение формы об изменении
						return;
					}
					throw new Exception("Имя '" + title + "' уже задано внутри Category");
				}
				// Определение максимального значения индекса
				if (i < desc.Id)
					i = desc.Id;
			}
			// Если идентификатор не задан
			if (id == -1)
            {
				// Добавления новой записи
				_list.Add(new Description()
				{
					Id = i + 1,
					Title = title,
					IsAdd = true
				});
			}				
			else
            {
				// Добавления старой записи
				_list.Add(new Description()
				{
					Id = id,
					Title = title
				});
			}

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
					if (_list[i].IsAdd)				// Если параметр добавлен недавно
						_list.RemoveAt(i);
					else
						_list[i].IsDelete = true;	// Отметка пункта как удаляемого
					OnCollectionChanged();			// Оповещение формы об изменении
					break;
				}
		}

		/// <summary>
		/// Имя таблицы для хранения значений пунктов описания.
		/// </summary>
		public string DescriptionTable => $"CategoryDescriptions{_id}";

		/// <summary>
		/// Получение списка новых столбцов таблицы для хранения значений пунктов описания.
		/// </summary>
		/// <returns>Список добавляемых столбцов.</returns>
		public List<string> GetAddColumn()
        {
			var list = new List<string>();
			// Поиск столбцов
			foreach (var d in _list)
				if (d.IsAdd)
					list.Add($"Desc{d.Id}");
			return list;
		}

		/// <summary>
		/// Получение списка столбцов таблицы для хранения значений пунктов описания, которые следует удалить.
		/// </summary>
		/// <returns>Список удаляемых столбцов.</returns>
		public List<string> GetDeleteColumn()
		{
			var list = new List<string>();
			// Поиск столбцов
			foreach (var d in _list)
				if (d.IsDelete)
					list.Add($"Desc{d.Id}");
			return list;
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

		public string CollectionTable => $"Description";

        public string IdItemName => "idDescription";

        /// <summary>
        /// Получение данных об элементе коллекции.
        /// </summary>
        /// <returns>Возвращает интерфейс на элемент.</returns>
        public IEnumerable<ICollectionItem> GetDataCollection()
		{
			for (int i = 0; i < _list.Count; i++)
				yield return _list[i];
		}

		/// <summary>
		/// Создает новый объект коллекции и возвращает интерфейс для заполнения.
		/// </summary>
		/// <returns>Объект для заполнения.</returns>
		public ICollectionItem CreateItem()
		{
			var desc = new Description();
			_list.Add(desc);
			return desc;
		}

		/*--- Скрывающий метод для BindableBaseCollection ---*/

		/// <summary>
		/// Возвращает шаблон для описания данной категории.
		/// </summary>
		/// <returns>Перечислитель для классов описания</returns>
		public override IEnumerator<Description> GetEnumerator()
		{
			foreach (var desc in _list)
				if (!desc.IsDelete)
					yield return new Description(desc);
		}
    }
}
