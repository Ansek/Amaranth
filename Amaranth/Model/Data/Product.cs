using System;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
	/// <summary>
	/// Определяет товар.
	/// </summary>
	public class Product : BindableBase, IData
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
			_priceText = product._priceText;
			_price = product._price;
			_prescription = product._prescription;
			_category = product._category;
		}

		protected int _id;
		/// <summary>
		/// Идентификатор товара.
		/// </summary>
		public int Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}

		protected string _title;
		/// <summary>
		/// Заголовок товара.
		/// </summary>
		public string Title
		{
			get => _title;
			set => SetValue(ref _title, value);
		}

		protected double _price;
		/// <summary>
		/// Цена товара.
		/// </summary>
		public double Price
		{
			get => _price;
			set { SetValue(ref _price, value); OnValueChanged("PriceText"); }
		}

		protected string _priceText;
		/// <summary>
		/// Значение цены для контроля ввода.
		/// </summary>
		public string PriceText
		{
			get => _priceText;
			set { SetValue(ref _priceText, value); double.TryParse(value.Replace('.', ','), out _price); OnValueChanged("Price"); }
		}

		protected int _count;
		/// <summary>
		/// Для хранения значения количества товаров.
		/// </summary>
		public int Count
		{
			get => _count;
			set => SetValue(ref _count, value);
		}

		protected int _reserve;
		/// <summary>
		/// Количество зарезервированных товаров.
		/// </summary>
		public int Reserve
		{
			get => _reserve;
			set => SetValue(ref _reserve, value);
		}

		protected bool _prescription;
		/// <summary>
		/// Флаг, выдается ли товар по рецепту.
		/// </summary>
		public bool Prescription
		{
			get => _prescription;
			set => SetValue(ref _prescription, value);
		}

		protected Category _category;
		/// <summary>
		/// Категория товара.
		/// </summary>
		public Category Category => _category;

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
		public IEnumerable<(string, object)> GetData()
		{
			yield return ("Title", _title);
			yield return ("Price", _price);
			yield return ("Count", _count);
			yield return ("Prescription", _prescription);
			yield return ("idCategory", _price);
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
					Price = Convert.ToDouble(value);
					break;
				case "Count":
					Count = Convert.ToInt32(value);
					break;
				case "Prescription":
					Prescription = Convert.ToBoolean(value);
					break;
				case "Reserve":
					Reserve = (value != DBNull.Value) ? Convert.ToInt32(value) : 0;
					break;
			}					
		}

	}
}
