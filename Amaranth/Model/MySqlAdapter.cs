using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using data = Amaranth.Model.Data.Data;

namespace Amaranth.Model
{
	public class MySqlAdapter : IDBAdapter
	{
		MySqlConnection _connect;

        public void Delete(data data)
        {
            throw new NotImplementedException();
        }

        public List<data> GetQuery(string table, string condition)
        {
            throw new NotImplementedException();
        }

        public data GetUser(string login, string password)
        {
            throw new NotImplementedException();
        }

        public int Insert(data data)
        {
            throw new NotImplementedException();
        }

        public bool IsColumnExists(string name, string table)
        {
            throw new NotImplementedException();
        }

        public bool IsTableExists(string name)
        {
            throw new NotImplementedException();
        }

        public void Load(ref data data)
        {
            throw new NotImplementedException();
        }

        public List<data> LoadList(string table)
        {
            throw new NotImplementedException();
        }

        public List<data> LoadList(string table, int pos, int count)
        {
            throw new NotImplementedException();
        }

        public void Update(data data)
        {
            throw new NotImplementedException();
        }

        private void RunCommand(string sql)
		{
			throw new NotImplementedException();
		}
	}
}
