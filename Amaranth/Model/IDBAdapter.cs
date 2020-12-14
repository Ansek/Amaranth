using Amaranth.Model.Data;
using System.Collections.Generic;
using data = Amaranth.Model.Data.Data;

namespace Amaranth.Model
{
	/// <summary>
	/// Предоставляет доступ к управлющему классу БД.
	/// </summary>
    public interface IDBAdapter
    {
		/// <summary>
		/// Добавляет запись в БД.
		/// </summary>
		/// <param name="data">Добавляемое значение.</param>
		void Insert(IData data);

		/// <summary>
		/// Изменяет запись в БД.
		/// </summary>
		/// <param name="data">Изменяемое значение.</param>
		void Update(IData data);

		/// <summary>
		/// Удаляет запись из БД.
		/// </summary>
		/// <param name="data">Удаляемое значение.</param>
		void Delete(IData data);

		// ----------------
		int Insert(data data);

		void Update(data data);

		void Delete(data data);

		void Load(ref data data);

		List<data> LoadList(string table);

		List<data> LoadList(string table, int pos, int count, string condition);

		data GetUser(string login, string password);

		List<data> GetQuery(string table, string condition);

		List<string> GetColumn(string column, string table);

		bool IsTableExists(string name);

		bool IsColumnExists(string name, string table);

		int GetRecordsCount(string table);

		int GetRecordsCount(string table, string condition);

		void CreateTable(string name, List<string> columns);

		void DeleteTable(string name);

		void AddColumn(string name, string table);

		void DeleteColumn(string name, string table);

		int GetMaxValue(string name, string table);

		int GetMaxValue(string name, string table, string condition);
	}
}
