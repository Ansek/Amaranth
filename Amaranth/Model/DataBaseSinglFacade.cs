using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amaranth.Model.Data;
using data = Amaranth.Model.Data.Data;

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
			if (_categories == null)
				GetListCategory();
			_productTitles = new ObservableCollection<string>(_adapter.GetColumn("Title", "product"));
			_tags = new ObservableCollection<string>(_adapter.GetColumn("Name", "tag"));
		}

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
			var p = product as Product;
			_adapter.Insert(p);

			// Запись дополнительных полей
			_adapter.Insert(product);
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
			var p = product as Product;
			_adapter.Update(p);

			// Изменение дополнительных полей
			_adapter.Update(product);
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
			var p = product as Product;
			_adapter.Delete(p);

			// Удаление дополнительных полей
			_adapter.Delete(product);
		}

		/// <summary>
		/// Запрашивает список товаров.
		/// </summary>
		/// <param name="categories">Список категорий для создания объектов товара.</param>
		/// <param name="request">Запрос для выборки товаров.</param>
		/// <param name="count">Количество записей для загрузки.</param>
		/// <param name="pos">Смещение.</param>
		/// <returns>Список найденных товаров</returns>
		public List<Product> GetListProduct(IEnumerable<Category> categories, ProductRequest request, int count, int pos)
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
				FillData(product, table, i); // Копирование значений из таблицы
				list.Add(product);
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

			// Запись данных категории
			_adapter.Insert(category);
		}

		/// <summary>
		/// Изменение данных о категории.
		/// </summary>
		/// <param name="category">Изменяемая категория.</param>
		public void Update(Category category)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");
						
			_adapter.Update(category);      // Изменение данных категории

			CategoryListChanged?.Invoke();  // Оповещение об изменении
		}

		/// <summary>
		/// Удаление данных о категории.
		/// </summary>
		/// <param name="category">Удаляемая категория.</param>
		public void Delete(Category category)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса DataBaseSinglFacade");
						
			_adapter.Delete(category);		// Удаление данных категории

			CategoryListChanged?.Invoke();  // Оповещение об изменении
		}

		/// <summary>
		/// Запрашивает список категорий.
		/// </summary>
		/// <returns>Список найденных категорий.</returns>
		public List<Category> GetListCategory()
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
				FillData(category, table, i); // Копирование значений из таблицы
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

			UserListChanged?.Invoke();  // Оповещение об изменении
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

			UserListChanged?.Invoke();	// Оповещение об изменении
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

			UserListChanged?.Invoke();	// Оповещение об изменении
		}

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
				var column = table.Columns[i].ColumnName;	// Получение имени столбца
				var value = table.Rows[iRow][i];			// Получение значения по этому столбцу
				data.SetData(column, value);				// Запись данных в объект заполнения
			}
		}

		/// <summary>
		/// Запрашивает список категорий.
		/// </summary>
		/// <returns>Список найденных категорий.</returns>
		public List<User> GetListUser()
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

		//--------------------------------------------------------


		static ObservableCollection<Category> _categories;
		static ObservableCollection<string> _productTitles;
		static ObservableCollection<string> _tags;


		public static event Action ProductChanged;


		public static ObservableCollection<Category> Categories => _categories;

		public static ObservableCollection<string> ProductTitles => _productTitles;

		public static ObservableCollection<string> Tags => _tags;



		public static int InsertOld(ProductInfo product)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("Title", product.Title);
			data.Add("Price", product.Price);
			data.Add("Count", product.Count);
			data.Add("Prescription", product.Prescription);
			data.Add("idCategory", product.Category.Id);
			data.TableName = "product";
			data.IdName = "idProduct";
			int id = _adapterOld.Insert(data);

			data.Clear();
			data.TableName = $"CategoryDescriptions{product.Category.Id}";
			data.IdName = "id";
			data.Add("id", id);
			foreach (var d in product)
				data.Add($"desc{d.Id}", d.Value);

			_adapterOld.Insert(data);
			ProductChanged?.Invoke();
			return id;
		}

		public static void InsertOld(Category category)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("Title", category.Title);
			data.IdName = "idCategory";
			data.TableName = "category";
			int id = _adapterOld.Insert(data);
			int i = 1;
			var columns = new List<string>();

			foreach (var desc in category)
				if (desc.Id == -1)
				{
					data.Clear();
					columns.Add($"desc{i}");
					data.Add("idDescription", i++);
					data.Add("idCategory", id);
					data.Add("Title", desc.Title);
					data.TableName = "description";
					_adapterOld.Insert(data);
				}

			string table = $"CategoryDescriptions{id}";
			_adapterOld.CreateTable(table, columns);
		}

		public static void InsertOld(User user)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("Login", user.Login);
			data.Add("FirstName", user.FirstName);
			data.Add("LastName", user.LastName);
			data.Add("Password", "");
			data.Add("IsAdministrator", user.IsAdministrator);
			data.TableName = "user";
			_adapterOld.Insert(data);
		}

		public static void UpdateOld(ProductInfo product, bool onlyCount = false)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("Count", product.Count);
			if (!onlyCount)
			{
				data.Add("Title", product.Title);
				data.Add("Price", product.Price);
				data.Add("Prescription", product.Prescription);
				data.Add("idCategory", product.Category.Id);
			}
			data.TableName = "product";
			data.IdName = "idProduct";
			data.RecordId = product.Id;
			_adapterOld.Update(data);

			data.Clear();
			data.TableName = $"CategoryDescriptions{product.Category.Id}";
			data.IdName = "id";
			foreach (var d in product)
				data.Add($"desc{d.Id}", d.Value);

			_adapterOld.Update(data);
			ProductChanged?.Invoke();
		}

		public static void UpdateOld(Category category)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("Title", category.Title);
			data.TableName = "category";
			data.IdName = "idCategory";
			data.RecordId = category.Id;
			_adapterOld.Update(data);

			int i = _adapterOld.GetMaxValue("idDescription", "description", $"idCategory = {category.Id}") + 1;
			foreach (var desc in category)
				if (desc.Id == -1)
				{
					data.Clear();
					_adapterOld.AddColumn($"desc{i}", $"CategoryDescriptions{category.Id}");
					data.Add("idDescription", i++);
					data.Add("idCategory", category.Id);
					data.Add("Title", desc.Title);
					data.TableName = "description";
					_adapterOld.Insert(data);
				}

			foreach (var j in category.DeletedIds)
            {
				data.Clear();
				data.TableName = "description";
				data.IdName = "idDescription";
				data.RecordId = $"{j} AND idCategory = {category.Id}";
				_adapterOld.Delete(data);
				_adapterOld.DeleteColumn($"desc{j}", $"CategoryDescriptions{category.Id}");
			}
		}

		public static void UpdateOld(User user)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("FirstName", user.FirstName);
			data.Add("LastName", user.LastName);
			data.Add("IsAdministrator", user.IsAdministrator);
			data.TableName = "user";
			data.IdName = "Login";
			data.RecordId = $"'{user.Login}'";
			_adapterOld.Update(data);
		}

		public static void DeleteOld(ProductInfo product)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.IdName = "idProduct";
			data.RecordId = product.Id;

			data.TableName = "product_tag";
			_adapterOld.Delete(data);

			data.TableName = "product";
			_adapterOld.Delete(data);

			data.TableName = $"CategoryDescriptions{product.Category.Id}";
			data.IdName = "id";
			_adapterOld.Delete(data);
			ProductChanged?.Invoke();
		}

		public static void DeleteOld(Category category)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.TableName = "description";
			data.IdName = "idCategory";
			data.RecordId = category.Id;
			_adapterOld.Delete(data);

			data.TableName = "category";
			_adapterOld.Delete(data);

			_adapterOld.DeleteTable($"CategoryDescriptions{category.Id}");
		}

		public static void DeleteOld(User user)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.TableName = "user";
			data.IdName = "Login";
			data.RecordId = $"'{user.Login}'";
			_adapterOld.Delete(data);
		}

		public static ProductInfo LoadInfo(Product product)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.TableName = $"CategoryDescriptions{product.Category.Id}";
			data.IdName = "id";
			data.RecordId = product.Id;
			foreach (var d in product.Category)
				data.Add($"desc{d.Id}");

			_adapterOld.Load(ref data);

			var values = new List<string>();
			foreach (var d in data)
				if (d.Value == null)
					values.Add(string.Empty);
				else
					values.Add(d.Value.ToString());

			return new ProductInfo(product, values);
		}

		public static string GetCondition(ProductRequest request)
        {
			string condition = string.Empty;

			if (request.CheckTags && request.Tags.Count > 0)
			{
				var str = string.Empty;
				foreach (var t in request.Tags)
				{
					if (str != string.Empty)
						str += " OR ";
					str += $"Tag = '{t}'";

				}
				condition += $"(idProduct, {request.Tags.Count}) IN (SELECT idProduct, count(Tag) FROM product_tag WHERE {str} GROUP BY idProduct)";
			}

			if (request.CheckTitle && request.Title != null)
			{
				var r = request.Title.Replace("'", @"\'");
				if (condition != string.Empty)
					condition += $" AND Title LIKE '%{r}%'";
				else
					condition += $" Title LIKE '%{r}%'";
			}

			if (request.CheckPrice)
			{
				if (condition != string.Empty)
					condition += $" AND {request.FromPrice} <= Price AND Price <= {request.ToPrice}";
				else
					condition += $" {request.FromPrice} <= Price AND Price <= {request.ToPrice}";
			}

			if (request.CheckDateComplited)
			{
				var select = $"SELECT op.idProduct FROM `order` o, order_product op " +
					$"WHERE o.idOrder = op.idOrder AND DATE(o.CompletionDate)" +
					$" BETWEEN '{request.FromDate.ToString("yyyy-MM-dd")}'" +
					$" AND '{request.ToDate.ToString("yyyy-MM-dd")}' GROUP BY op.idProduct";
				if (condition != string.Empty)
					condition += $" AND idProduct IN ({select})";
				else
					condition += $"idProduct IN ({select})";
			}

			if (request.CheckCategory)
			{
				if (condition != string.Empty)
					condition += $" AND idCategory = {request.Category}";
				else
					condition += $" idCategory = {request.Category}";
			}

			return condition;
		}

		public static List<Product> GetListProductOld(int pos, int count, ProductRequest request)
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var condition = GetCondition(request);
			var products = _adapterOld.LoadList("product_view", pos, count, condition);
			if (products.Count > 0)
			{
				var list = new List<Product>();
				foreach (var p in products)
				{
					Category category = null;
					foreach (var c in _categories)
						if (c.Id == Convert.ToInt32(p["idCategory"]))
                        {
							category = c;
							break;
						}

					var product = new Product(category)
					{
						Id = Convert.ToInt32(p["idProduct"]),
						Title = Convert.ToString(p["Title"]),
						Price = Convert.ToDouble(p["Price"]),
						Count = Convert.ToInt32(p["Count"]),
						Prescription = Convert.ToBoolean(p["Prescription"])
					};

					if (p["Reserve"] != DBNull.Value)
						product.Reserve = Convert.ToInt32(p["Reserve"]);

					list.Add(product);
				}
				return list;
			}
			return null;
		}

		public static List<Category> GetListCategoryOld()
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var categories = _adapterOld.LoadList("category");
			if (categories.Count > 0)
			{
				var list = new List<Category>();
				foreach (var c in categories)
				{
					var category = new Category()
					{
						Id = Convert.ToInt32(c["idCategory"]),
						Title = Convert.ToString(c["Title"])
					};
					var desc = _adapterOld.GetQuery("description", $"idCategory = {category.Id}");
					foreach (var d in desc)
                    {
						int id = Convert.ToInt32(d["idDescription"]);
						var title = Convert.ToString(d["Title"]);
						category.AddDescription(title, id);
					}
					list.Add(category);
				}
				_categories = new ObservableCollection<Category>(list);
				return list;
			}
			return null;
		}

		public static List<User> GetListUserOld()
		{
			if (_adapterOld == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var users = _adapterOld.LoadList("user");
			if (users.Count > 0)
            {
				var list = new List<User>();
				foreach (var u in users)
				{
					list.Add(new User()
					{
						FirstName = Convert.ToString(u["FirstName"]),
						LastName = Convert.ToString(u["LastName"]),
						Login = Convert.ToString(u["Login"]),
						IsAdministrator = Convert.ToBoolean(u["IsAdministrator"])
					});
				}
				return list;
			}
			return null;
		}

		public static List<Order> GetListOrder(int pos, int count, bool onlyActive = false, bool onlyCompleted = false)
        {
			var condition = string.Empty;
			if (onlyActive && !onlyCompleted)
				condition = "CompletionDate IS NULL";
			else if(!onlyActive && onlyCompleted)
				condition = "CompletionDate IS NOT NULL";

			var orders = _adapterOld.LoadList("`order`", pos, count, condition);
			if (orders.Count > 0)
			{
				var list = new List<Order>();
				foreach (var o in orders)
				{
					var order = new Order()
					{
						Id = Convert.ToInt32(o["idOrder"]),
						CreationDate = Convert.ToDateTime(o["CreationDate"])
					};

					if (o["CompletionDate"] != DBNull.Value)
						order.CompletionDate = Convert.ToDateTime(o["CompletionDate"]);

					list.Add(order);
				}
				return list;
			}
			return null;
		}

		public static Order GetOrder(int id)
        {
			Order order = null;
			var data = new data();
			data.Add("CreationDate");
			data.Add("CompletionDate");
			data.TableName = "`order`";
			data.IdName = "idOrder";
			data.RecordId = id;

			_adapterOld.Load(ref data);
			if (data["CreationDate"] != null)
            {
				order = new Order()
				{
					Id = id,
					CreationDate = Convert.ToDateTime(data["CreationDate"])
				};
				if (data["CompletionDate"] != DBNull.Value)
					order.CompletionDate = Convert.ToDateTime(data["CompletionDate"]);

				var products = _adapterOld.GetQuery("Product p, Order_Product op", $"p.IdProduct = op.IdProduct AND op.idOrder = {id}");
				foreach (var p in products)
				{
					Category category = null;
					foreach (var c in _categories)
						if (c.Id == Convert.ToInt32(p["idCategory"]))
						{
							category = c;
							break;
						}

					double price = Convert.ToDouble((p["Price1"] == DBNull.Value) ? p["Price"] : p["Price1"]);

					order.Add(new Product(category)
					{
						Id = Convert.ToInt32(p["idProduct"]),
						Title = Convert.ToString(p["Title"]),
						Price = price,
						Count = Convert.ToInt32(p["Count1"]),
						Prescription = Convert.ToBoolean(p["Prescription"])
					});
				}
			}
			return order;
		}

		public static void CompleteOrder(Order order)
        {
			var data = new data();
			data.TableName = "`order`";
			data.IdName = "idOrder";
			var date = DateTime.Now;

			if (order.CreationDate == null)
            {
				data.Add("CreationDate", date);
				data.Add("CompletionDate", date);
				int id = _adapterOld.Insert(data);

				data.TableName = "order_product";
				foreach (var p in order)
                {
					data.Clear();
					data.Add("idOrder", id);
					data.Add("idProduct", p.Id);
					data.Add("Count", p.Count);
					data.Add("Price", p.Price);
					_adapterOld.Insert(data);
				}
			}
			else
            {
				data.RecordId = order.Id;
				data.Add("CompletionDate", date);
				_adapterOld.Update(data);

				data.TableName = "order_product";
				foreach (var p in order)
				{
					data.RecordId = $"{order.Id} AND idProduct = {p.Id}";
					data.Clear();
					data.Add("Count", p.Count);
					data.Add("Price", p.Price);
					_adapterOld.Update(data);
				}
			}
			ProductChanged?.Invoke();
		}

		public static void CancelOrder(Order order)
		{
			var data = new data();
			data.TableName = "order_product";
			data.IdName = "idOrder";
			data.RecordId = order.Id;
			_adapterOld.Delete(data);

			data.TableName = "`order`";
			_adapterOld.Delete(data);
			ProductChanged?.Invoke();
		}

		public static void AddTags(int idProduct, List<string> tags)
        {
			var data = new data();
			data.TableName = "product_tag";
			data.IdName = "idProduct";

			foreach (var tag in tags)
            {
				if (_adapterOld.GetRecordsCount("tag", $"Name = '{tag}'") == 0)
				{
					var temp = new data();
					temp.TableName = "tag";
					temp.Add("Name", tag);
					_adapterOld.Insert(temp);
					Tags.Add(tag);
				}

				data.Clear();
				data.Add("idProduct", idProduct);
				data.Add("Tag", tag);
				_adapterOld.Insert(data);
			}
		}

		public static void DeleteTags(int idProduct, List<string> tags)
		{
			var data = new data();
			data.TableName = "product_tag";
			data.IdName = "idProduct";

			foreach (var tag in tags)
            {
				data.RecordId = $"{idProduct} AND Tag = '{tag}'";
				_adapterOld.Delete(data);
			}
		}

		public static List<string> LoadTags(int idProduct)
		{
			var data = _adapterOld.GetQuery("product_tag", $"idProduct = {idProduct}");
			var list = new List<string>();

			foreach (var d in data)
				list.Add(d["Tag"].ToString());

			return list;
		}

		public static int GetMaxCountProduct(int idProduct)
        {
			var data = _adapterOld.GetQuery("product_view", $"idProduct = {idProduct}");
			if (data != null)
            {
				int reserve = Convert.ToInt32(data[0]["Reserve"]);
				int count = Convert.ToInt32(data[0]["Count"]);
				return count - reserve;
			}
			return 0;
		}

		public static void SubMaxCountProduct(int idProduct, int count)
        {
			var data = _adapterOld.GetQuery("product", $"idProduct = {idProduct}");
			if (data != null)
			{
				int newCount = Convert.ToInt32(data[0]["Count"]) - count;
				data[0].Clear();
				data[0].Add("Count", newCount);
				data[0].TableName = "product";
				data[0].IdName = "idProduct";
				data[0].RecordId = idProduct;
				_adapterOld.Update(data[0]);
				ProductChanged?.Invoke();
			}
		}
	}
}
