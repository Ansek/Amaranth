using System;
using System.Collections.Generic;
using Amaranth.Model.Data;
using data = Amaranth.Model.Data.Data;

namespace Amaranth.Model
{
	public class DataBaseSinglFacade
	{
		static DataBaseSinglFacade _intance;
		static IDBAdapter _adapter;

		private DataBaseSinglFacade()
		{
		}

		public static DataBaseSinglFacade GetInstance()
		{
			return _intance ?? (_intance = new DataBaseSinglFacade());
		}

		public static void SetAdapter(IDBAdapter adapter)
		{
			_adapter = adapter;
		}

		public static void Insert(ProductInfo product)
		{
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
			data.Add("IsAdministrator", user.IsAdministrator);
			data.TableName = "user";
			_adapter.Insert(data);
		}

		public static void Update(ProductInfo product)
		{
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
		}

		public static List<Product> GetListProduct(int pos, int count)
		{
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
	}
}
