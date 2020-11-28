using System;
using System.Collections.Generic;
using Amaranth.Model.Data;

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
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
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
			if (_adapter != null)
			{
				// TO DO
				throw new NotImplementedException();
			}
			else
				throw new Exception("Не задан адаптер для класса Auth");
		}
	}
}
