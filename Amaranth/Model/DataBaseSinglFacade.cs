using System;
using System.Data;
using System.Collections.Generic;
using Amaranth.Model.Data;

namespace Amaranth.Model
{
	/// <summary>
	/// Класс паттерна Facade. Упрощает доступ объектов системы к БД.
	/// </summary>
	public class DataBaseSinglFacade
	{
		/// <summary>
		/// Экземпляр данного класс для паттерна Singleton.
		/// </summary>
		static DataBaseSinglFacade _intance;

		/// <summary>
		/// Ссылка на адаптер, для доступа к БД.
		/// </summary>
		IDBAdapter _adapter;
		static IDBAdapter _adapterOld;		

		/// <summary>
		/// Скрытый конструтор согласно паттерну Singleton.
		/// </summary>
		private DataBaseSinglFacade()
		{
		}

		/// <summary>
		/// Инициализация объекта паттерна Singleton.
		/// </summary>
		/// <returns>Объект класса авторизации.</returns>
		public static DataBaseSinglFacade GetInstance()
		{
			return _intance ?? (_intance = new DataBaseSinglFacade());
		}

		/// <summary>
		/// Установка адаптера, для доступа к БД.
		/// </summary>
		/// <param name="adapter">Объект адаптера.</param>
		public void SetAdapter(IDBAdapter adapter)
		{
			_adapter = _adapterOld = adapter;
		}

		/// <summary>
		/// Флаг, который определеяе установлен ли адаптер
		/// </summary>
		public bool IsSetAdapter => _adapter != null;

		/*--- Методы для работы с товарами ---*/

		/// <summary>
		/// Для оповещения, что был изменен список товаров.
		/// </summary>
		public event Action ProductListChanged;

		/// <summary>
		/// Добавление данных о товаре.
		/// </summary>
		/// <param name="product">Добавляемый товар.</param>
		public void Insert(ProductInfo product)
        {
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Приведение к родительскому классу для записи основных полей
			var p = new Product(product);
			_adapter.Insert(p);
			product.Id = p.Id;				// Обновление параметра идентификатора
			_adapter.UpdateCollection(p);   // Обновление данных о тегах

			_adapter.Insert(product);		// Запись дополнительных полей
			ProductListChanged?.Invoke();	// Оповещение об изменении в списке товаров
		}

		/// <summary>
		/// Изменение данных о товаре.
		/// </summary>
		/// <param name="product">Изменяемый товар.</param>
		public void Update(ProductInfo product)
        {
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Приведение к родительскому классу для изменение основных полей
			var p = new Product(product);
			_adapter.Update(p);
			_adapter.UpdateCollection(p);   // Обновление данных о тегах

			_adapter.Update(product);		// Изменение дополнительных полей
			ProductListChanged?.Invoke();	// Оповещение об изменении в списке товаров
		}

		/// <summary>
		/// Удаление данных о товаре.
		/// </summary>
		/// <param name="product">Удаляемый товар.</param>
		public void Delete(ProductInfo product)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Приведение к родительскому классу для удаления основных полей
			var p = new Product(product);

			// Удаление всех ссылок на товар в таблице тегов
			_adapter.Delete("Product_Tag", "idProduct", p.Id);
			_adapter.Delete(p);
						
			_adapter.Delete(product);		// Удаление дополнительных полей
			ProductListChanged?.Invoke();	// Оповещение об изменении в списке товаров
		}

		/// <summary>
		/// Запрашивает список товаров.
		/// </summary>
		/// <param name="categories">Список категорий для создания объектов товара.</param>
		/// <param name="request">Запрос для выборки товаров.</param>
		/// <param name="count">Количество записей для загрузки.</param>
		/// <param name="pos">Смещение.</param>
		/// <returns>Список найденных товаров</returns>
		public List<Product> GetProductList(IEnumerable<Category> categories, ProductRequest request, int count, int pos)
        {
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Получение данных из расширенной таблицы (с полем зарезервированного количества товаров)
			var table = _adapter.LoadTable("product_view", request.GetCondition(), count, pos);

			var list = new List<Product>();
			// Разбор строк полученной таблицы
			for (int i = 0; i < table.Rows.Count; i++)
			{
				// Поиск соответсвующей категории
				var idCategory = Convert.ToInt32(table.Rows[i]["idCategory"]);
				Category category = null;
				foreach (var c in categories)
					if (c.Id == idCategory)
					{
						category = c;
						break;
					}

				// Создает объекта товара
				var product = new Product(category);
				FillData(product, table, i);		// Копирование значений из таблицы
				_adapter.FillCollection(product);   // Заполнение информацией о тегах
				foreach (var tag in product)
					_adapter.LoadData(tag);			// Получение заголовков тегов
				list.Add(product);
			}
			return list;
		}

		/// <summary>
		/// Загрузка дополнительной информации о товаре.
		/// </summary>
		/// <param name="product">Исходный объект товара.</param>
		/// <returns>Дополненный объект товара.</returns>
		public ProductInfo LoadInfo(Product product)
        {
			var info = new ProductInfo(product); // Создание нового контейнера
			// Получение значений полей описания
			var table = _adapter.LoadTable(info.Table, $"idProduct = {info.IdColumn}");

			// Разбор строк полученной таблицы
			if (table.Rows.Count > 0)
				FillData(info, table, 0);

			return info;
		}

		/// <summary>
		/// Возвращает заголовки товаров для поиска.
		/// </summary>
		/// <returns>Список заголовков.</returns>
		public IEnumerable<string> GetProductTitles()
        {
			return _adapter.GetColumn("Product", "Title");
		}

		/*--- Методы для работы с тегами ---*/

		/// <summary>
		/// Для оповещения, что был изменен список товаров.
		/// </summary>
		public event Action TagListChanged;

		/// <summary>
		/// Добавление списка тегов в БД.
		/// </summary>
		/// <param name="tags">Список тегов.</param>
		public void AddTags(List<Tag> tags)
        {
			// Добавление тегов по отдельности
			foreach (var tag in tags)
				_adapter.Insert(tag);

			if (tags.Count > 0)
				TagListChanged?.Invoke(); // Оповещение об изменении в списке тегов
		}

		/// <summary>
		/// Запрашивает список тегов.
		/// </summary>
		/// <returns>Список найденных категорий.</returns>
		public List<Tag> GetTagList()
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Получение данных из таблицы категорий
			var table = _adapter.LoadTable("tag");

			var list = new List<Tag>();
			// Разбор строк полученной таблицы
			for (int i = 0; i < table.Rows.Count; i++)
			{
				// Создает объекта категории
				var tag = new Tag();
				FillData(tag, table, i);       // Копирование значений из таблицы
				list.Add(tag);
			}
			return list;
		}

		/*--- Методы для работы с категориями ---*/

		/// <summary>
		/// Для оповещения, что был изменен список категорий.
		/// </summary>
		public event Action CategoryListChanged;

		/// <summary>
		/// Добавление данных о категории.
		/// </summary>
		/// <param name="category">Добавляемая категория.</param>
		public void Insert(Category category)
        {
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			_adapter.Insert(category);				// Запись данных категории
			_adapter.UpdateCollection(category);    // Обновление данных о пунктах описания товаров
			// Добавление связанной таблицы описания
			_adapter.CreateTable(category.DescriptionTable, "idProduct", category.GetAddColumn());
			CategoryListChanged?.Invoke();			// Оповещение об изменении в списке категорий
		}

		/// <summary>
		/// Изменение данных о категории.
		/// </summary>
		/// <param name="category">Изменяемая категория.</param>
		public void Update(Category category)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");
						
			_adapter.Update(category);				// Изменение данных категории
			_adapter.UpdateCollection(category);    // Обновление данных о пунктах описания товаров

			// Добавление новых столбцов при наличии
			foreach (var column in category.GetAddColumn())
				_adapter.AddColumn(category.DescriptionTable, column);
			// Удаление новых столбцов при наличии
			foreach (var column in category.GetDeleteColumn())
				_adapter.DeleteColumn(category.DescriptionTable, column);

			CategoryListChanged?.Invoke();			// Оповещение об изменении в списке категорий
		}

		/// <summary>
		/// Удаление данных о категории.
		/// </summary>
		/// <param name="category">Удаляемая категория.</param>
		public void Delete(Category category)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Удаление всех ссылок на категорию в таблице пунктов описаний
			_adapter.Delete("Description", "idCategory", category.Id);
			_adapter.Delete(category);          // Удаление данных категории
			// Удаление связанной таблицы описания
			_adapter.DeleteTable(category.DescriptionTable);    
			CategoryListChanged?.Invoke();		// Оповещение об изменении в списке категорий
		}

		/// <summary>
		/// Запрашивает список категорий.
		/// </summary>
		/// <returns>Список найденных категорий.</returns>
		public List<Category> GetCategoryList()
        {
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Получение данных из таблицы категорий
			var table = _adapter.LoadTable("category");

			var list = new List<Category>();
			// Разбор строк полученной таблицы
			for (int i = 0; i < table.Rows.Count; i++)
			{
				// Создает объекта категории
				var category = new Category();
				FillData(category, table, i);		// Копирование значений из таблицы
				_adapter.FillCollection(category);	// Заполнение информацией о пунктах описания товара
				list.Add(category);
			}
			return list;
		}

		/*--- Методы для работы с пользователями ---*/

		/// <summary>
		/// Для оповещения, что был изменен список пользователей.
		/// </summary>
		public event Action UserListChanged;

		/// <summary>
		/// Добавление данных о категории.
		/// </summary>
		/// <param name="category">Добавляемая категория.</param>
		public void Insert(User user)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");
						
			_adapter.Insert(user);		// Запись данных пользователя
			UserListChanged?.Invoke();  // Оповещение об изменении в списке пользователей
		}

		/// <summary>
		/// Изменение данных о категории.
		/// </summary>
		/// <param name="category">Изменяемая категория.</param>
		public void Update(User user)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");
						
			_adapter.Update(user);		// Изменение данных пользователя
			UserListChanged?.Invoke();  // Оповещение об изменении в списке пользователей
		}

		/// <summary>
		/// Удаление данных о категории.
		/// </summary>
		/// <param name="category">Удаляемая категория.</param>
		public void Delete(User user)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			_adapter.Delete(user);		// Удаление данных пользователя
			UserListChanged?.Invoke();  // Оповещение об изменении в списке пользователей
		}

		/// <summary>
		/// Запрашивает список категорий.
		/// </summary>
		/// <returns>Список найденных категорий.</returns>
		public List<User> GetUserList()
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Получение данных из таблицы пользователей
			var table = _adapter.LoadTable("user");

			var list = new List<User>();
			// Разбор строк полученной таблицы
			for (int i = 0; i < table.Rows.Count; i++)
			{
				// Создает объекта категории
				var user = new User();
				FillData(user, table, i); // Копирование значений из таблицы
				list.Add(user);
			}
			return list;
		}

		/*--- Методы для работы с заказами ---*/

		/// <summary>
		/// Получает сведение о заказе по его номеру.
		/// </summary>
		/// <param name="id">Номер заказа.</param>
		/// <returns>Объект заказа.</returns>
		public Order GetOrder(int id)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Получение данных о заказе
			var table = _adapter.LoadTable("`order`", $"idOrder = {id}");

			// Создает объекта заказа
			Order order = null;
			if (table.Rows.Count > 0)
			{
				order = new Order();
				FillData(order, table, 0);			// Копирование значений из таблицы
				_adapter.FillCollection(order);		// Заполнение информацией о продуктах
				foreach (var product in order)
				{
					product.NotUpdateCost = true;   // Запрет на изменение стоимости, чтобы сохранить предущее значение
					_adapter.LoadData(product);     // Получение информации о продуктах
				}
				order.RecalculationConst();			// Перерасчет итоговой суммы
			}
			return order;
		}

		/// <summary>
		/// Завершает указанный заказ.
		/// </summary>
		/// <param name="order">Объект заказа.</param>
		public void CompleteOrder(Order order)
        {
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			var date = DateTime.Now;
			// Если заказ не был создан заранее
			if (order.CreationDate == null)
            {
				//Установка дат создания и завершения заказа
				order.CreationDate = date;
				order.CompletionDate = date;
				// Запись в таблицах
				_adapter.Insert(order);
				_adapter.UpdateCollection(order);
			}
			else
            {
				// Установка даты завершения заказа
				order.CompletionDate = date;
				// Запись в таблицах
				_adapter.Update(order);
				_adapter.UpdateCollection(order);
			}
		}

		/// <summary>
		/// Отменяет указанный заказ.
		/// </summary>
		/// <param name="order">Объект заказа.</param>
		public void CancelOrder(Order order)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Удаление всех ссылок на заказ в списке товаров
			_adapter.Delete("Order_Product", "idOrder", order.Id);
			_adapter.Delete(order); // Удаление данных заказа
		}

		/// <summary>
		/// Загружает список со сведениями о товарах.
		/// </summary>
		/// <param name="count">Количество записей.</param>
		/// <param name="pos">Смещение поиска.</param>
		/// <param name="onlyActive">Флаг, показать активные заказы.</param>
		/// <param name="onlyCompleted">Флаг, показать заверщенные заказы.</param>
		/// <returns></returns>
		public List<Order> GetListOrder(int count, int pos, bool onlyActive = false, bool onlyCompleted = false)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Формирование строки условия
			var condition = string.Empty;
			if (onlyActive && !onlyCompleted)
				condition = "CompletionDate IS NULL";
			else if (!onlyActive && onlyCompleted)
				condition = "CompletionDate IS NOT NULL";

			// Получение данных из таблицы заказов
			var table = _adapter.LoadTable("`order`", condition, count, pos);

			var list = new List<Order>();
			// Разбор строк полученной таблицы
			for (int i = 0; i < table.Rows.Count; i++)
			{
				// Создает объекта заказа
				var order = new Order();
				FillData(order, table, i);			// Копирование значений из таблицы
				_adapter.FillCollection(order);     // Заполнение информацией о продуктах
				foreach (var product in order)
					_adapter.LoadData(product);		// Получение информации о продуктах
				list.Add(order);
			}
			return list;
		}

		/// <summary>
		/// Получает максимально допустимое количество для покупки товаров.
		/// </summary>
		/// <param name="idProduct">Идентификатор товара.</param>
		/// <returns>Допустимое количество товаров.</returns>
		public int GetMaxCountProduct(int idProduct)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			int res = 0;
			// Получение требуемых параметров для расчета
			int reserve = _adapter.GetNumber("Product_View", "Reserve", $"idProduct = {idProduct}");
			int count = _adapter.GetNumber("Product_View", "Count", $"idProduct = {idProduct}");
			// Расчет значений
			return count - reserve;
		}

		/// <summary>
		/// Вычитает данную стоимость из стоимости в БД.
		/// </summary>
		/// <param name="product">Объект товара с вычитающим количеством.</param>
		public void SubMaxCountProduct(Product product)
        {
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");

			// Получение исходной стоимости
			int? count = _adapter.GetNumber("Product_View", "Count", $"idProduct = {product.Id}");
			//Если значения получены
			if (count != null)
            {
				// Перерасчет 
				product.ProductCount = (int)count - product.ProductCount;
				// Обновление данных
				product.SaveOnlyCount = true;
				_adapter.Update(product);
			}
		}

		/*--- Вспомогательные методы ---*/

		/// <summary>
		/// Заполнение данными через объект таблицы.
		/// </summary>
		/// <param name="data">Объект для заполнения.</param>
		/// <param name="table">Объект таблицы.</param>
		/// <param name="iRow">Идентификатор строки</param>
		void FillData(IData data, DataTable table, int iRow)
		{
			// Перебор значений заданной строки
			for (int i = 0; i < table.Columns.Count; i++)
			{
				var column = table.Columns[i].ColumnName;   // Получение имени столбца
				var value = table.Rows[iRow][i];            // Получение значения по этому столбцу
				data.SetData(column, value);                // Запись данных в объект заполнения
			}
		}
	}
}
