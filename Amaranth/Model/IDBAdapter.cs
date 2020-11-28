using System.Collections.Generic;
using data = Amaranth.Model.Data.Data;

namespace Amaranth.Model
{
    public interface IDBAdapter
    {
		int Insert(data data);

		void Update(data data);

		void Delete(data data);

		void Load(ref data data);

		List<data> LoadList(string table);

		List<data> LoadList(string table, int pos, int count);

		data GetUser(string login, string password);

		List<data> GetQuery(string table, string condition);

		bool IsTableExists(string name);

		bool IsColumnExists(string name, string table);
	}
}
