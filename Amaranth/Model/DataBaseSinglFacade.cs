using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amaranth.Model.Data;
using data = Amaranth.Model.Data.Data;

namespace Amaranth.Model
{
	public class DataBaseSinglFacade
	{
		static DataBaseSinglFacade _intance;
		static IDBAdapter _adapter;
		static ObservableCollection<Category> _categories;
		static ObservableCollection<string> _productTitles;
		static ObservableCollection<string> _tags;

		private DataBaseSinglFacade()
		{
		}

		public static event Action ProductChanged;

		public static ObservableCollection<Category> Categories => _categories;

		public static ObservableCollection<string> ProductTitles => _productTitles;

		public static ObservableCollection<string> Tags => _tags;

		public static DataBaseSinglFacade GetInstance()
		{
			return _intance ?? (_intance = new DataBaseSinglFacade());
		}

		public static void SetAdapter(IDBAdapter adapter)
		{
			_adapter = adapter;
			if (_categories == null)
				GetListCategory();
			_productTitles = new ObservableCollection<string>(_adapter.GetColumn("Title", "product"));
			_tags = new ObservableCollection<string>(_adapter.GetColumn("Name", "tag"));
		}

		public static int Insert(ProductInfo product)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("Title", product.Title);
			data.Add("Price", product.Price);
			data.Add("Count", product.Count);
			data.Add("Prescription", product.Prescription);
			data.Add("idCategory", product.Category.Id);
			data.TableName = "product";
			data.IdName = "idProduct";
			int id = _adapter.Insert(data);

			data.Clear();
			data.TableName = $"CategoryDescriptions{product.Category.Id}";
			data.IdName = "id";
			data.Add("id", id);
			foreach (var d in product)
				data.Add($"desc{d.Id}", d.Value);

			_adapter.Insert(data);
			ProductChanged?.Invoke();
			return id;
		}

		public static void Insert(Category category)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("Title", category.Title);
			data.IdName = "idCategory";
			data.TableName = "category";
			int id = _adapter.Insert(data);
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
					_adapter.Insert(data);
				}

			string table = $"CategoryDescriptions{id}";
			_adapter.CreateTable(table, columns);
		}

		public static void Insert(User user)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("Login", user.Login);
			data.Add("FirstName", user.FirstName);
			data.Add("LastName", user.LastName);
			data.Add("Password", "");
			data.Add("IsAdministrator", user.IsAdministrator);
			data.TableName = "user";
			_adapter.Insert(data);
		}

		public static void Update(ProductInfo product, bool onlyCount = false)
		{
			if (_adapter == null)
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
			_adapter.Update(data);

			data.Clear();
			data.TableName = $"CategoryDescriptions{product.Category.Id}";
			data.IdName = "id";
			foreach (var d in product)
				data.Add($"desc{d.Id}", d.Value);

			_adapter.Update(data);
			ProductChanged?.Invoke();
		}

		public static void Update(Category category)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("Title", category.Title);
			data.TableName = "category";
			data.IdName = "idCategory";
			data.RecordId = category.Id;
			_adapter.Update(data);

			int i = _adapter.GetMaxValue("idDescription", "description", $"idCategory = {category.Id}") + 1;
			foreach (var desc in category)
				if (desc.Id == -1)
				{
					data.Clear();
					_adapter.AddColumn($"desc{i}", $"CategoryDescriptions{category.Id}");
					data.Add("idDescription", i++);
					data.Add("idCategory", category.Id);
					data.Add("Title", desc.Title);
					data.TableName = "description";
					_adapter.Insert(data);
				}

			foreach (var j in category.DeletedIds)
            {
				data.Clear();
				data.TableName = "description";
				data.IdName = "idDescription";
				data.RecordId = $"{j} AND idCategory = {category.Id}";
				_adapter.Delete(data);
				_adapter.DeleteColumn($"desc{j}", $"CategoryDescriptions{category.Id}");
			}
		}

		public static void Update(User user)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.Add("FirstName", user.FirstName);
			data.Add("LastName", user.LastName);
			data.Add("IsAdministrator", user.IsAdministrator);
			data.TableName = "user";
			data.IdName = "Login";
			data.RecordId = $"'{user.Login}'";
			_adapter.Update(data);
		}

		public static void Delete(ProductInfo product)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.IdName = "idProduct";
			data.RecordId = product.Id;

			data.TableName = "product_tag";
			_adapter.Delete(data);

			data.TableName = "product";
			_adapter.Delete(data);

			data.TableName = $"CategoryDescriptions{product.Category.Id}";
			data.IdName = "id";
			_adapter.Delete(data);
			ProductChanged?.Invoke();
		}

		public static void Delete(Category category)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.TableName = "description";
			data.IdName = "idCategory";
			data.RecordId = category.Id;
			_adapter.Delete(data);

			data.TableName = "category";
			_adapter.Delete(data);

			_adapter.DeleteTable($"CategoryDescriptions{category.Id}");
		}

		public static void Delete(User user)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.TableName = "user";
			data.IdName = "Login";
			data.RecordId = $"'{user.Login}'";
			_adapter.Delete(data);
		}

		public static ProductInfo LoadInfo(Product product)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var data = new data();
			data.TableName = $"CategoryDescriptions{product.Category.Id}";
			data.IdName = "id";
			data.RecordId = product.Id;
			foreach (var d in product.Category)
				data.Add($"desc{d.Id}");

			_adapter.Load(ref data);

			var values = new List<string>();
			foreach (var d in data)
				if (d.Value == null)
					values.Add(string.Empty);
				else
					values.Add(d.Value.ToString());

			return new ProductInfo(product, values);
		}

		static string GetCondition(ProductRequest request)
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

			if (request.CheckCategory)
			{
				if (condition != string.Empty)
					condition += $" AND idCategory = {request.Category}";
				else
					condition += $" idCategory = {request.Category}";
			}

			return condition;
		}

		public static List<Product> GetListProduct(int pos, int count, ProductRequest request)
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var condition = GetCondition(request);
			var products = _adapter.LoadList("product", pos, count, condition);
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

					list.Add(new Product(category)
					{
						Id = Convert.ToInt32(p["idProduct"]),
						Title = Convert.ToString(p["Title"]),
						Price = Convert.ToDouble(p["Price"]),
						Count = Convert.ToInt32(p["Count"]),
						Prescription = Convert.ToBoolean(p["Prescription"])
					});
				}
				return list;
			}
			return null;
		}

		public static List<Category> GetListCategory()
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var categories = _adapter.LoadList("category");
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
					var desc = _adapter.GetQuery("description", $"idCategory = {category.Id}");
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

		public static List<User> GetListUser()
		{
			if (_adapter == null)
				throw new Exception("Не задан адаптер для класса Auth");

			var users = _adapter.LoadList("user");
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

			var orders = _adapter.LoadList("`order`", pos, count, condition);
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

			_adapter.Load(ref data);
			if (data["CreationDate"] != null)
            {
				order = new Order()
				{
					Id = id,
					CreationDate = Convert.ToDateTime(data["CreationDate"])
				};
				if (data["CompletionDate"] != DBNull.Value)
					order.CompletionDate = Convert.ToDateTime(data["CompletionDate"]);

				var products = _adapter.GetQuery("Product p, Order_Product op", $"p.IdProduct = op.IdProduct AND op.idOrder = {id}");
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
				int id = _adapter.Insert(data);

				data.TableName = "order_product";
				foreach (var p in order)
                {
					data.Clear();
					data.Add("idOrder", id);
					data.Add("idProduct", p.Id);
					data.Add("Count", p.Count);
					data.Add("Price", p.Price);
					_adapter.Insert(data);
				}
			}
			else
            {
				data.RecordId = order.Id;
				data.Add("CompletionDate", date);
				_adapter.Update(data);

				data.TableName = "order_product";
				foreach (var p in order)
				{
					data.RecordId = $"{order.Id} AND idProduct = {p.Id}";
					data.Clear();
					data.Add("Count", p.Count);
					data.Add("Price", p.Price);
					_adapter.Update(data);
				}
			}
		}

		public static void CancelOrder(Order order)
		{
			var data = new data();
			data.TableName = "order_product";
			data.IdName = "idOrder";
			data.RecordId = order.Id;
			_adapter.Delete(data);

			data.TableName = "`order`";
			_adapter.Delete(data);
		}

		public static void AddTags(int idProduct, List<string> tags)
        {
			var data = new data();
			data.TableName = "product_tag";
			data.IdName = "idProduct";

			foreach (var tag in tags)
            {
				if (_adapter.GetRecordsCount("tag", $"Name = '{tag}'") == 0)
				{
					var temp = new data();
					temp.TableName = "tag";
					temp.Add("Name", tag);
					_adapter.Insert(temp);
					Tags.Add(tag);
				}

				data.Clear();
				data.Add("idProduct", idProduct);
				data.Add("Tag", tag);
				_adapter.Insert(data);
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
				_adapter.Delete(data);
			}
		}

		public static List<string> LoadTags(int idProduct)
		{
			var data = _adapter.GetQuery("product_tag", $"idProduct = {idProduct}");
			var list = new List<string>();

			foreach (var d in data)
				list.Add(d["Tag"].ToString());

			return list;
		}
	}
}
