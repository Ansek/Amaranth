using System;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
	/// <summary>
	/// Определяет товар.
	/// </summary>
	public class Product : BindableBaseCollection<Tag>, IData, IDataCollection, ICollectionItem
	{
		/// <summary>
		/// Конструктор для объекта товара.
		/// </summary>
		/// <param name="category">Категория, которая определяет дополнительные поля.</param>
		public Product(Category category)
		{
			_id = -1;
			_category = category;
			_prescription = false;
		}

		/// <summary>
		/// Копирующий конструктор для объекта товара.
		/// </summary>
		/// <param name="product">Объект копирования.</param>
		public Product(Product product)
		{
			_id = product._id;
			_count = product._count;
			_reserve = product._reserve;
			_title = product._title;
			_price = product._price;
			_priceText = product._priceText;
			_prescription = product._prescription;
			_category = product._category;
			_list = product._list;
		}

        ///
        protected int _id;
		/// <summary>
		/// Идентификатор товара.
		/// </summary>
		public int Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}

        ///
        protected string _title;
		/// <summary>
		/// Заголовок товара.
		/// </summary>
		public string Title
		{
			get => _title;
			set => SetValue(ref _title, value);
		}

        ///
        protected double _price;
		/// <summary>
		/// Цена товара.
		/// </summary>
		public double Price
		{
			get => _price;
			set { SetValue(ref _price, value); PriceText = _price.ToString(); }
		}

        ///
        protected string _priceText;
		/// <summary>
		/// Значение цены для контроля ввода.
		/// </summary>
		public string PriceText
		{
			get => _priceText;
			set { SetValue(ref _priceText, value); double.TryParse(value.Replace('.', ','), out _price); OnValueChanged("Price"); }
		}

        ///
        protected int _count;
		/// <summary>
		/// Для хранения значения количества товаров.
		/// </summary>
		public int ProductCount
		{
			get => _count;
			set => SetValue(ref _count, value);
		}

        ///
        protected int _reserve;
		/// <summary>
		/// Количество зарезервированных товаров.
		/// </summary>
		public int Reserve
		{
			get => _reserve;
			set => SetValue(ref _reserve, value);
		}

        ///
        protected bool _prescription;
		/// <summary>
		/// Флаг, выдается ли товар по рецепту.
		/// </summary>
		public bool Prescription
		{
			get => _prescription;
			set => SetValue(ref _prescription, value);
		}

        ///
        protected Category _category;
		/// <summary>
		/// Категория товара.
		/// </summary>
		public Category Category => _category;

		/// <summary>
		/// Добавление тега.
		/// </summary>
		/// <param name="tag">Добавляемый тег.</param>
		public void AddTag(Tag tag)
		{
			// Проверка заголовка на уникальность
			foreach (var desc in _list)
			{
				if (desc.Title == tag.Title)
				{
					// Если есть, но помечена как удаленная
					if (desc.IsDelete)
					{
						desc.IsDelete = false;
						OnCollectionChanged();  // Оповещение формы об изменении
						return;
					}
					return; // Дубликат не добавляется
				}
			}
			tag.IsAdd = true; // Установка флага на добавление

			// Добавление тега
			_list.Add(tag);

			OnCollectionChanged();  // Оповещение формы об изменении
		}

		/// <summary>
		/// Удаление тега.
		/// </summary>
		/// <param name="tag">Удаляемый тег.</param>
		public void RemoveTag(Tag tag)
		{
			// Поиск удаляемого пункта
			for (int i = 0; i < _list.Count; i++)
				if (_list[i].Id == tag.Id && _list[i].Title == tag.Title)
				{
					if (_list[i].IsAdd)             // Если параметр добавлен недавно
						_list.RemoveAt(i);
					else
						_list[i].IsDelete = true;   // Отметка пункта как удаляемого
					OnCollectionChanged();          // Оповещение формы об изменении
					break;
				}
		}

		/// <summary>
		/// Флаг, устанавливающий запись только значения количества.
		/// </summary>
		public bool SaveOnlyCount { get; set; }

		/// <summary>
		/// Флаг, устанавливающий запрет на изменение стоимости товаров, при загрузке.
		/// </summary>
		public bool NotUpdateCost { get; set; }

		/*--- Свойства и методы для интерфейса IData ---*/

		/// <summary>
		/// Значений первичного ключа.
		/// </summary>
		public object IdColumn => _id;

		/// <summary>
		/// Имя столбца значения первичного ключа.
		/// </summary>
		public string IdColumnName => "idProduct";

		/// <summary>
		/// Имя таблицы.
		/// </summary>
		public string Table => "Product";

		/// <summary>
		/// Получение данных об имени столбцах и их содержимом.
		/// </summary>
		/// <returns>Возвращает кортеж из имени столбца и его значения.</returns>
		IEnumerable<(string, object)> IData.GetData()
		{
			if (!SaveOnlyCount)
            {
				yield return ("Title", _title);
				yield return ("Price", _price);				
				yield return ("Prescription", _prescription);
				yield return ("idCategory", _category.Id);
			}
			yield return ("Count", _count);
		}

		/// <summary>
		/// Получение данных об имени столбцах и их содержимом.
		/// </summary>
		/// <returns>Возвращает кортеж из имени столбца и его значения.</returns>
		IEnumerable<(string, object)> ICollectionItem.GetData()
        {
			yield return ("Price", _price);
			yield return ("Count", _count);
		}

		/// <summary>
		/// Заполнение данных по указанным столбцам.
		/// </summary>
		/// <param name="column">Имя столбца.</param>
		/// <param name="value">Значение столбца.</param>
		public void SetData(string column, object value)
		{
			switch (column)
            {
				case "idProduct":
					Id = Convert.ToInt32(value);
					break;
				case "Title":
					Title = value as string;
					break;
				case "Price":
					if (!NotUpdateCost || Price == 0)
						Price = (value != DBNull.Value) ? Convert.ToDouble(value) : 0;
					break;
				case "Count":
					if (!NotUpdateCost)
						ProductCount = Convert.ToInt32(value);
					break;
				case "Prescription":
					Prescription = Convert.ToBoolean(value);
					break;
				case "Reserve":
					Reserve = (value != DBNull.Value) ? Convert.ToInt32(value) : 0;
					break;
			}					
		}

        /*--- Свойства и методы для интерфейса IDataCollection ---*/

        ///
        public string CollectionTable => "Product_Tag";
        ///
        public string IdItemName => "idTag";

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
			var tag = new Tag();
			_list.Add(tag);
			return tag;
		}

		/*--- Свойства для интерфейса ICollectionItem ---*/

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
