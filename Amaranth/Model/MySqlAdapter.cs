using System;
using System.Data;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using data = Amaranth.Model.Data.Data;

namespace Amaranth.Model
{
	public class MySqlAdapter : IDBAdapter
	{
		MySqlConnection _connect;

        public MySqlAdapter()
        {
            string param = "datasource = localhost; port = 3306; username = root; database = amaranth; password = 123";
            _connect = new MySqlConnection(param);
        }

        public int Insert(data data)
        {
            _connect.Open();
            int id = -1;
            string idCmd;
            string columns = string.Empty;
            string values = string.Empty;
            var cmd = new MySqlCommand();
            cmd.Connection = _connect;

            int i = 0;
            foreach (var d in data)
            {
                if (i != 0)
                {
                    columns += ",";
                    values += ",";
                }
                idCmd = $"@{i++}";
                columns += d.Name;
                values += idCmd;
                var type = GetEquivalent(d.Type);
                cmd.Parameters.Add(idCmd, type);
                cmd.Parameters[idCmd].Value = d.Value;
            }

            try
            {
                cmd.CommandText = $"INSERT INTO {data.TableName} ({columns}) VALUES ({values});";
                int l = cmd.ExecuteNonQuery();
                if (l > 0 && data.TableName != "user")
                {
                    cmd.CommandText = $"SELECT max({data.IdName}) FROM {data.TableName};";
                    id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            finally
            {
                _connect.Close();
            }
            return id;
        }

        public void Update(data data)
        {
            _connect.Open();
            string idCmd;
            string parameters = string.Empty;
            var cmd = new MySqlCommand();
            cmd.Connection = _connect;

            int i = 0;
            foreach (var d in data)
            {
                if (i != 0)
                    parameters += ",";
                idCmd = $"@{i++}";
                parameters += $"{d.Name} = {idCmd}";
                var type = GetEquivalent(d.Type);
                cmd.Parameters.Add(idCmd, type);
                cmd.Parameters[idCmd].Value = d.Value;
            }

            cmd.CommandText = $"UPDATE {data.TableName} SET {parameters} WHERE {data.IdName} = {data.RecordId};";
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                _connect.Close();
            }
        }

        public void Delete(data data)
        {
            _connect.Open();
            string sql = $"DELETE FROM {data.TableName} WHERE {data.IdName} = {data.RecordId};";
            var cmd = new MySqlCommand(sql, _connect);
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                _connect.Close();
            }
        }

        public void Load(ref data data)
        {
            _connect.Open();
            string columns = string.Empty;
            var cmd = new MySqlCommand();
            cmd.Connection = _connect;

            foreach (var d in data)
            {
                if (columns != string.Empty)
                    columns += ",";
                columns += $"{d.Name}";
            }

            cmd.CommandText = $"SELECT {columns} FROM {data.TableName} WHERE {data.IdName} = {data.RecordId};";
            try
            {
                var reader = cmd.ExecuteReader();

                int i = 0;
                while (reader.Read())
                {
                    foreach (var d in data)
                        data[d.Name] = reader.GetValue(i++);
                }
            }
            finally
            {
                _connect.Close();
            }
        }

        public data GetUser(string login, string password)
        {
            _connect.Open();
            string sql = $"SELECT firstname, lastname FROM user WHERE login = '{login}' AND password = '{password}';";

            data data = null;
            var cmd = new MySqlCommand(sql, _connect);

            try
            {
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    data = new data();
                    data.Add("firstname", reader.GetValue(0));
                    data.Add("lastname", reader.GetValue(1));
                }
            }
            finally
            {
                _connect.Close();
            }
            return data;
        }

        public List<data> LoadList(string table)
        {
            string sql = $"SELECT * FROM {table};";
            return GetList(table, sql);
        }

        public List<data> LoadList(string table, int pos, int count)
        {
            string sql = $"SELECT * FROM {table} LIMIT {pos}, {count};";
            return GetList(table, sql);
        }

        public List<data> GetQuery(string table, string condition)
        {
            string sql = $"SELECT * FROM {table} WHERE {condition};";
            return GetList(table, sql);
        }

        public bool IsTableExists(string name)
        {
            string sql = $"SHOW TABLES FROM amaranth LIKE '{name}';";
            return GetScalar(sql) != null;
        }

        public bool IsColumnExists(string name, string table)
        {
            string sql = $"SHOW COLUMNS FROM amaranth.{table} LIKE '{name}';";
            return GetScalar(sql) != null;
        }

        public int GetRecordsCount(string table)
        {
            string sql = $"SELECT count(*) FROM {table};";
            return Convert.ToInt32(GetScalar(sql));
        }

        public int GetRecordsCount(string table, string condition)
        {
            string sql = $"SELECT count(*) FROM {table} WHERE {condition};";
            return Convert.ToInt32(GetScalar(sql));
        }

        public void CreateTable(string name, List<string> columns)
        {
            string field = string.Empty;
            foreach (var column in columns)
                field += $", {column} TEXT NULL";

            _connect.Open();
            string sql = $"CREATE TABLE {name} (id INT NOT NULL{field}, PRIMARY KEY (id));";
            var cmd = new MySqlCommand(sql, _connect);
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                _connect.Close();
            }
        }

        public void DeleteTable(string name)
        {
            _connect.Open();
            string sql = $"DROP TABLE {name};";
            var cmd = new MySqlCommand(sql, _connect);
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                _connect.Close();
            }
        }

        public void AddColumn(string name, string table)
        {
            _connect.Open();
            string sql = $"ALTER TABLE {table} ADD {name} TEXT NULL;";
            var cmd = new MySqlCommand(sql, _connect);
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                _connect.Close();
            }
        }

        public void DeleteColumn(string name, string table)
        {
            _connect.Open();
            string sql = $"ALTER TABLE {table} DROP COLUMN {name};";
            var cmd = new MySqlCommand(sql, _connect);
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                _connect.Close();
            }
        }

        public int GetMaxValue(string name, string table)
        {
            string sql = $"SELECT max({name}) FROM {table};";
            return Convert.ToInt32(GetScalar(sql));
        }

        public int GetMaxValue(string name, string table, string condition)
        {
            string sql = $"SELECT max({name}) FROM {table} WHERE {condition};";
            return Convert.ToInt32(GetScalar(sql));
        }

        MySqlDbType GetEquivalent(Type type)
        {
            MySqlDbType t = MySqlDbType.Text;
            if (type == typeof(int))
                t = MySqlDbType.Int32;
            else if (type == typeof(string))
                t = MySqlDbType.VarChar;
            else if (type == typeof(double))
                t = MySqlDbType.Decimal;
            else if (type == typeof(bool))
                t = MySqlDbType.Bit;
            return t;
        }

        List<data> GetList(string table, string sql)
        {
            var ds = new DataSet();
            var cmd = new MySqlCommand(sql, _connect);
            var msadapter = new MySqlDataAdapter(cmd);
            msadapter.Fill(ds, table);

            List<data> list = new List<data>();
            foreach (DataRow row in ds.Tables[table].Rows)
            {
                var data = new data();
                for (int i = 0; i < ds.Tables[table].Columns.Count; i++)
                {
                    var col = ds.Tables[table].Columns[i].ColumnName;
                    data.Add(col, row[i]);
                }
                list.Add(data);
            }

            return list;
        }

        object GetScalar(string sql)
        {
            _connect.Open();
            var cmd = new MySqlCommand(sql, _connect);
            var o = cmd.ExecuteScalar();
            _connect.Close();
            return o;
        }
    }
}
