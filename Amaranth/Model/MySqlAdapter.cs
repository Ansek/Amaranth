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

            // Обновление параметра идентификатора
            if (data.IdColumn is int)
            {
                int id = GetMaxValue(data.Table, data.IdColumnName);
                data.SetData(data.IdColumnName, id);
            }                
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
        /// Удаление записей из БД по параметрам.
        /// </summary>
        /// <param name="table">Таблица, из которой будут удалены записи.</param>
        /// <param name="column">Столбец, по которому будет проводиться проверка.</param>
        /// <param name="value">Значение, по которому будуь удалены записи.</param>
        public void Delete(string table, string column, object value)
        {
            var cmd = new MySqlCommand("", _connect);

            // Формирование запроса на удаление
            string idCmd;
            int i = 0;
            var paramKey = GetParamater(value, ref i, out idCmd); // Формирование параметра ключа
            cmd.Parameters.Add(paramKey);               // Добавление параметра ключа в команду
            cmd.CommandText = $"DELETE FROM {table} WHERE {column} = {idCmd};";

            // Выполнение запроса
            ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Загрузка данных из БД.
        /// </summary>
        /// <param name="table">Имя таблицы.</param>
        /// <param name="condition">Условие выборки.</param>
        /// <param name="count">Количество записей.</param>
        /// <param name="pos">Смещение.</param>
        /// <returns>Таблица с результатами запроса.</returns>
        public DataTable LoadTable(string table, string condition = null, int count = 0, int pos = 0)
        {
            // Составление скрипта запроса
            if (condition != null && condition != string.Empty)
                condition = $"WHERE {condition}";
            var limit = (count > 0) ? $"LIMIT {pos}, {count}" : "";
            var sql = $"SELECT * FROM {table} {condition} {limit};";

            //Выполнение запроса
            return LoadTable(table, sql);
        }

        /// <summary>
        /// Загружает данные для данной записи.
        /// </summary>
        /// <param name="data">Объект записи.</param>
        public void LoadData(IData data)
        {
            // Формирование запроса
            var sql = $"SELECT * FROM {data.Table} WHERE {data.IdColumnName} = {data.IdColumn};";
            //Выполнение запроса
            var table = LoadTable(data.Table, sql);
            // Заполнение
            if (table.Rows.Count > 0)
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    var column = table.Columns[j].ColumnName;   // Получение имени столбца
                    var value = table.Rows[0][j];               // Получение значения по этому столбцу
                    data.SetData(column, value);                // Запись данных в объект заполнения
                }
        }

        /// <summary>
        /// Обновляет сведения для элементах коллекции внешней таблицы.
        /// </summary>
        /// <param name="collection">Объект коллекции.</param>
        public void UpdateCollection(IDataCollection collection)
        {
            var cmd = new MySqlCommand("", _connect);
            int i = 0;
            // Перебор элементов коллекции
            foreach (var c in collection.GetDataCollection())
            {
                // Если требуется добавить элемент
                if (c.IdItem > 0 && c.IsAdd)
                {
                    // Формирование запроса на добавление
                    string idCmd, columns, values;
                    columns = values = string.Empty;
                    foreach (var d in c.GetData())
                    {
                        if (columns != string.Empty) // Разделение данных по запятым
                        {
                            columns += ",";
                            values += ",";
                        }
                        var param = GetParamater(d.Item2, ref i, out idCmd); // Формирование параметра
                        cmd.Parameters.Add(param);  // Добавление параметра в команду
                        columns += d.Item1;         // Формирование списка столбцов
                        values += idCmd;            // Формирование списка значений
                    }
                    string idCmd1, idCmd2;
                    var paramKey1 = GetParamater(collection.IdColumn, ref i, out idCmd1); // Формирование параметра первого ключа
                    cmd.Parameters.Add(paramKey1);                              // Добавление параметра первого ключа в команду
                    var paramKey2 = GetParamater(c.IdItem, ref i, out idCmd2);  // Формирование параметра второго ключа
                    cmd.Parameters.Add(paramKey2);                              // Добавление параметра второго ключа в команду
                    if (columns != string.Empty)
                    {
                        columns += ",";
                        values += ",";
                    }
                    columns += $"{ collection.IdColumnName},{ collection.IdItemName}";  // Добавление ключевых параметров
                    values += $"{idCmd1},{idCmd2}";
                    cmd.CommandText += $"INSERT INTO {collection.CollectionTable} ({columns}) VALUES ({values});\n";
                }
                // Если требуется удалить элемент коллекции
                if (c.IdItem > 0 && c.IsDelete)
                {
                    // Формирование запроса на удаление
                    string idCmd1, idCmd2;
                    var paramKey1 = GetParamater(collection.IdColumn, ref i, out idCmd1); // Формирование параметра первого ключа
                    cmd.Parameters.Add(paramKey1);                              // Добавление параметра первого ключа в команду
                    var paramKey2 = GetParamater(c.IdItem, ref i, out idCmd2);  // Формирование параметра второго ключа
                    cmd.Parameters.Add(paramKey2);                              // Добавление параметра второго ключа в команду
                    cmd.CommandText += $"DELETE FROM {collection.CollectionTable} WHERE {collection.IdColumnName} = {idCmd1} AND {collection.IdItemName} = {idCmd2};\n";
                }    
            }
            // Выполнение списка составленных запросов
            if (cmd.CommandText != string.Empty)
                ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Заполняет коллекцию данными из внешней таблицы.
        /// </summary>
        /// <param name="collection">Объект коллекции.</param>
        public void FillCollection(IDataCollection collection)
        {
            var sql = $"SELECT * FROM {collection.CollectionTable} WHERE {collection.IdColumnName} = {collection.IdColumn};";
            var table = LoadTable(collection.CollectionTable, sql);

            // Перебор значений заданной строки
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var item = collection.CreateItem(); // Получение нового элемента коллекции для заполнения
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    var column = table.Columns[j].ColumnName;   // Получение имени столбца
                    var value = table.Rows[i][j];               // Получение значения по этому столбцу
                    item.SetData(column, value);                // Запись данных в объект заполнения
                }
            }
        }

        /// <summary>
        /// Создание таблицы в БД.
        /// </summary>
        /// <param name="table">Имя таблицы.</param>
        /// <param name="columnIdName">Имя ключевого поля.</param>
        /// <param name="columns">Список столбцов таблицы.</param>
        public void CreateTable(string table, string columnIdName, List<string> columns)
        {
            // Формирвание полей для столбцов
            string field = string.Empty;
            foreach (var column in columns)
                field += $", {column} TEXT NULL";

            // Формирование запроса на создание таблицы
            string sql = $"CREATE TABLE {table} ({columnIdName} INT NOT NULL{field}, PRIMARY KEY ({columnIdName}));";
            var cmd = new MySqlCommand(sql, _connect);

            //Выполнение запроса
            ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Добавление столбца в таблицу.
        /// </summary>
        /// <param name="table">Имя таблицы.</param>
        /// <param name="column">Имя столбца.</param>
        public void AddColumn(string table, string column)
        {
            // Формирование запроса на добавление столбца
            string sql = $"ALTER TABLE {table} ADD {column} TEXT NULL;";
            var cmd = new MySqlCommand(sql, _connect);

            //Выполнение запроса
            ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Удаление столбца из таблицы.
        /// </summary>
        /// <param name="table">Имя таблицы.</param>
        /// <param name="column">Имя столбца.</param>
        public void DeleteColumn(string table, string column)
        {
            // Формирование запроса на удаление столбца
            string sql = $"ALTER TABLE {table} DROP COLUMN {column};";
            var cmd = new MySqlCommand(sql, _connect);

            //Выполнение запроса
            ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Удаление таблицы из БД.
        /// </summary>
        /// <param name="table">Имя таблицы.</param>
        public void DeleteTable(string table)
        {
            // Формирование запроса на удаление таблицы
            string sql = $"DROP TABLE {table};";
            var cmd = new MySqlCommand(sql, _connect);

            //Выполнение запроса
            ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Получение максимального значения в таблице.
        /// </summary>
        /// <param name="table">Имя таблицы.</param>
        /// <param name="column">Имя столбца.</param>
        public int GetMaxValue(string table, string column, string condition = null)
        {
            // Составление скрипта запроса
            if (condition != null)
                condition = $"WHERE {condition}";
            string sql = $"SELECT max({column}) FROM {table} {condition};";
            var cmd = new MySqlCommand(sql, _connect);

            //Выполнение запроса
            return ExecuteScalar<int>(cmd);
        }

        /// <summary>
        /// Загружает все данные заданного столбца.
        /// </summary>
        /// <param name="table">Имя таблицы.</param>
        /// <param name="column">Имя столбца.</param>
        /// <returns>Список значений столбца.</returns>
        public List<string> GetColumn(string table, string column)
        {
            // Составление скрипта запроса
            string sql = $"SELECT {column} FROM {table} ORDER BY {column};";
            // Получение данных
            var colTable = LoadTable(table, sql);
            var list = new List<string>();
            // Перебор значений заданной строки
            for (int i = 0; i < colTable.Rows.Count; i++)
                list.Add(colTable.Rows[i][0].ToString());
            return list;
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
        /// Загружает результат запроса в таблицу.
        /// </summary>
        /// <param name="table">Имя таблицы.</param>
        /// <param name="sql">Скрипт запроса.</param>
        /// <returns>Таблица с результатами запроса.</returns>
        DataTable LoadTable(string table, string sql)
        {
            var ds = new DataSet();
            // Настройка параметров
            var cmd = new MySqlCommand(sql, _connect);
            var msadapter = new MySqlDataAdapter(cmd);
            // Загрузка таблицы
            msadapter.Fill(ds, table);
            return ds.Tables[table];
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

        public void CreateTableOld(string name, List<string> columns)
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

        public void DeleteTableOld(string name)
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

        public int Insert(data data)
        {
            throw new NotImplementedException();
        }

        public void Update(data data)
        {
            throw new NotImplementedException();
        }

        public void Delete(data data)
        {
            throw new NotImplementedException();
        }
    }
}
