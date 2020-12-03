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
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
