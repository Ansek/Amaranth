using System;
using System.Data;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using data = Amaranth.Model.Data.Data;
using Amaranth.Model.Data;

namespace Amaranth.Model
{
    /// <summary>
    /// Предоставляет функции для работы с СУБД MySql
    /// </summary>
	public class MySqlAdapter : IDBAdapter
	{
        /// <summary>
        /// Хранит настройки соедения
        /// </summary>
		MySqlConnection _connect;

        public MySqlAdapter()
        {
            // Установка параметров соединия с БД
            string param = "datasource = localhost; port = 3306; username = root; database = amaranth; password = 123";
            _connect = new MySqlConnection(param);
        }

        /// <summary>
        /// Добавляет запись в БД.
        /// </summary>
        /// <param name="data">Добавляемое значение.</param>
        public void Insert(IData data)
        {
            var cmd = new MySqlCommand("", _connect);

            // Формирование запроса на добавление
            string idCmd, columns, values;
            columns = values = string.Empty;
            int i = 0;
            foreach (var d in data.GetData())
            {
                if (i != 0) // Разделение данных по запятым
                {
                    columns += ",";
                    values += ",";
                }
                var param = GetParamater(d.Item2, ref i, out idCmd); // Формирование параметра
                cmd.Parameters.Add(param);  // Добавление параметра в команду
                columns += d.Item1;         // Формирование списка столбцов
                values += idCmd;            // Формирование списка значений
            }
            cmd.CommandText = $"INSERT INTO {data.Table} ({columns}) VALUES ({values});";

            // Выполнение запроса
            ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Изменяет запись в БД.
        /// </summary>
        /// <param name="data">Изменяемое значение.</param>
        public void Update(IData data)
        {
            var cmd = new MySqlCommand("", _connect);

            // Формирование запроса на изменение
            string idCmd;
            string parameters = string.Empty;
            int i = 0;
            foreach (var d in data.GetData())
            {
                if (i != 0 )// Разделение данных по запятым
                    parameters += ",";
                var param = GetParamater(d.Item2, ref i, out idCmd); // Формирование параметра
                cmd.Parameters.Add(param);              // Добавление параметра в команду
                parameters += $"{d.Item1} = {idCmd}";   // Указание изменяемых значений
            }
            var paramKey = GetParamater(data.IdColumn, ref i, out idCmd); // Формирование параметра ключа
            cmd.Parameters.Add(paramKey);               // Добавление параметра ключа в команду
            cmd.CommandText = $"UPDATE {data.Table} SET {parameters} WHERE {data.IdColumnName} = {idCmd};";

            // Выполнение запроса
            ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Удаляет запись из БД.
        /// </summary>
        /// <param name="data">Удаляемое значение.</param>
        public void Delete(IData data)
        {
            var cmd = new MySqlCommand("", _connect);

            // Формирование запроса на удаление
            string idCmd;
            int i = 0;
            var paramKey = GetParamater(data.IdColumn, ref i, out idCmd); // Формирование параметра ключа
            cmd.Parameters.Add(paramKey);               // Добавление параметра ключа в команду
            cmd.CommandText = $"DELETE FROM {data.Table} WHERE {data.IdColumnName} = {idCmd};";

            // Выполнение запроса
            ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Выполнение запроса без получения ответа.
        /// </summary>
        /// <param name="cmd">Объект команды для выполнения запроса.</param>
        void ExecuteNonQuery(MySqlCommand cmd)
        {
            //Выполнение запроса
            _connect.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                _connect.Close();
            }
        }

        /// <summary>
        /// Выполнение запроса с получением единственного значения.
        /// </summary>
        /// <param name="cmd">Объект команды для выполнения запроса.</param>
        /// <returns>Возвращает единственный результат.</returns>
        T ExecuteScalar<T>(MySqlCommand cmd)
        {
            object o;
            //Выполнение запроса
            _connect.Open();
            try
            {
                o = cmd.ExecuteScalar();
            }
            finally
            {
                _connect.Close();
            }
            // Попытка преобразования к данному типу
            return (o is T) ? (T)o : default(T);
        }

        /// <summary>
        /// Выполнение запроса с получением нескольких значений.
        /// </summary>
        /// <param name="cmd">Объект команды для выполнения запроса.</param>
        /// <returns>Возвращает объект для чтения результатов.</returns>
        MySqlDataReader ExecuteReader(MySqlCommand cmd)
        {
            MySqlDataReader r;
            //Выполнение запроса
            _connect.Open();
            try
            {
                r = cmd.ExecuteReader();
            }
            finally
            {
                _connect.Close();
            }
            return r;
        }

        /// <summary>
        /// Упаковка значения в формат параметра команды.
        /// </summary>
        /// <param name="value">Получаемое значение.</param>
        /// <param name="i">Номер параметра. Увеличивается на единицу.</param>
        /// <param name="name">Возвращаемое имя параметра.</param>
        /// <returns>Возвращает параметр с полученным значением.</returns>
        MySqlParameter GetParamater(object value, ref int i, out string name)
        {
            name = $"@{i++}"; // Формирование имени параметра

            // Определение типа параметра
            var type = MySqlDbType.Text;
            if (value is int)
                type = MySqlDbType.Int32;
            else if (value is string)
                type = MySqlDbType.VarChar;
            else if (value is double)
                type = MySqlDbType.Decimal;
            else if (value is bool)
                type = MySqlDbType.Bit;
            else if (value is DateTime)
                type = MySqlDbType.DateTime;

            // Упаковка полученных данных
            return new MySqlParameter(name, type) { Value = value };
        }

        // ------------------------------------------------------------------------


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
                //var type = GetEquivalent(d.Type);
                var type = MySqlDbType.Int32;
                cmd.Parameters.Add(idCmd, type);
                cmd.Parameters[idCmd].Value = d.Value;
            }

            try
            {
                cmd.CommandText = $"INSERT INTO {data.TableName} ({columns}) VALUES ({values});";
                int l = cmd.ExecuteNonQuery();
                if (l > 0 && data.TableName != "user" && data.TableName != "tag")
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
                //var type = GetEquivalent(d.Type);
                var type = MySqlDbType.Int32;
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
            string columns = string.Empty;
            var cmd = new MySqlCommand();
            cmd.Connection = _connect;

            foreach (var d in data)
            {
                if (columns != string.Empty)
                    columns += ",";
                columns += $"{d.Name}";
            }

            if (columns == string.Empty)
                return;

            _connect.Open();
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
            string sql = $"SELECT FirstName, LastName, IsAdministrator FROM user WHERE login = '{login}' AND password = '{password}';";

            data data = null;
            var cmd = new MySqlCommand(sql, _connect);

            try
            {
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    data = new data();
                    data.Add("FirstName", reader.GetValue(0));
                    data.Add("LastName", reader.GetValue(1));
                    data.Add("IsAdministrator", reader.GetValue(2));
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

        public List<data> LoadList(string table, int pos, int count, string condition)
        {
            if (condition != string.Empty)
                condition = $"WHERE {condition}";
            string sql = $"SELECT * FROM {table} {condition} LIMIT {pos}, {count};";
            return GetList(table, sql);
        }

        public List<data> GetQuery(string table, string condition)
        {
            string sql = $"SELECT * FROM {table} WHERE {condition};";
            return GetList(table, sql);
        }

        public List<string> GetColumn(string column, string table)
        {
            string sql = $"SELECT {column} FROM {table} ORDER BY {column};";
            var data = GetList(table, sql);
            var list = new List<string>();
            foreach (var d in data)
                list.Add(d[column].ToString());
            return list;
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
