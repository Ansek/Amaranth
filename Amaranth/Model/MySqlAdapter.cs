using System;
using System.Data;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
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
                var param = GetParamater(d.Item2, ref i, out idCmd);    // Формирование параметра
                cmd.Parameters.Add(param);                              // Добавление параметра в команду
                parameters += $"{d.Item1} = {idCmd}";                   // Указание изменяемых значений
            }
            var paramKey = GetParamater(data.IdColumn, ref i, out idCmd);   // Формирование параметра ключа
            cmd.Parameters.Add(paramKey);                               // Добавление параметра ключа в команду
            cmd.CommandText = $"UPDATE {data.Table} SET {parameters} WHERE {data.IdColumnName} = {idCmd};";

            // Выполнение запроса
            ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Обновляет поле таблицы в БД.
        /// </summary>
        /// <param name="table">Таблица, в которой будет обновляено данное.</param>
        /// <param name="idColumn">Столбец идентификатора.</param>
        /// <param name="idValue">Значение идентификатора.</param>
        /// <param name="column">Столбец, в котором будет производиться изменение.</param>
        /// <param name="value">Значение, которое будет записываться.</param>
        public void Update(string table, string idColumn, object idValue, string column, object value)
        {
            var cmd = new MySqlCommand("", _connect);
            int i = 0;
            string idCmd, valueCmd;
            var paramKey = GetParamater(idValue, ref i, out idCmd);         // Формирование параметра ключа
            cmd.Parameters.Add(paramKey);                                   // Добавление параметра ключа в команду
            var paramValue = GetParamater(value, ref i, out valueCmd);    // Формирование параметра значения
            cmd.Parameters.Add(paramValue);                                 // Добавление параметра значения в команду

            cmd.CommandText = $"UPDATE {table} SET {column} = {valueCmd} WHERE {idColumn} = {idCmd};";

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
		/// <param name="condition">Дополнительное условие для выборки.</param>
        public void LoadData(IData data, string condition = null)
        {
            // Составление скрипта запроса
            if (condition != null && condition != string.Empty)
                condition = $"AND {condition}";
            var sql = $"SELECT * FROM {data.Table} WHERE {data.IdColumnName} = {data.IdColumn} {condition};";
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
            return ExecuteScalar(cmd);
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
        /// Получение значения ячейки по заданному условию.
        /// </summary>
        /// <param name="table">Имя таблицы.</param>
        /// <param name="column">Имя столбца.</param>
        /// <param name="condition">Условие поиска.</param>
        /// <returns>Значение ячейки.</returns>
        public int GetNumber(string table, string column, string condition)
        {
            // Составление скрипта запроса
            string sql = $"SELECT {column} FROM {table} WHERE {condition};";
            var cmd = new MySqlCommand(sql, _connect);
            // Получение значения
            return ExecuteScalar(cmd);
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
        int ExecuteScalar(MySqlCommand cmd)
        {
            int i = 0;
            //Выполнение запроса
            _connect.Open();
            try
            {
                object o = cmd.ExecuteScalar();
                // Попытка преобразования 
                if (o != DBNull.Value)
                    i = Convert.ToInt32(o);
            }
            finally
            {
                _connect.Close();
            }
            return i;
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
    }
}
