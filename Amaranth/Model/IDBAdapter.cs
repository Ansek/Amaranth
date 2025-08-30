using System.Data;
using System.Collections.Generic;
using Amaranth.Model.Data;

namespace Amaranth.Model
{
	/// <summary>
	/// Предоставляет доступ к управляющему классу БД.
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
		/// Обновляет поле таблицы в БД.
		/// </summary>
		/// <param name="table">Таблица, в которой будет обновлено данное.</param>
		/// <param name="idColumn">Столбец идентификатора.</param>
		/// <param name="idValue">Значение идентификатора.</param>
		/// <param name="column">Столбец, в котором будет производиться изменение.</param>
		/// <param name="value">Значение, которое будет записываться.</param>
		void Update(string table, string idColumn, object idValue, string column, object value);

		/// <summary>
		/// Удаляет запись из БД.
		/// </summary>
		/// <param name="data">Удаляемое значение.</param>
		void Delete(IData data);

		/// <summary>
		/// Удаление записей из БД по параметрам.
		/// </summary>
		/// <param name="table">Таблица, из которой будут удалены записи.</param>
		/// <param name="column">Столбец, по которому будет проводиться проверка.</param>
		/// <param name="value">Значение, по которому будут удалены записи.</param>
		void Delete(string table, string column, object value);

		/// <summary>
		/// Загрузка данных из БД.
		/// </summary>
		/// <param name="table">Имя таблицы.</param>
		/// <param name="condition">Условие выборки.</param>
		/// <param name="count">Количество записей.</param>
		/// <param name="pos">Смещение.</param>
		/// <returns>Таблица с результатами запроса.</returns>
		DataTable LoadTable(string table, string condition = null, int count = 0, int pos = 0);

		/// <summary>
		/// Загружает данные для данной записи по идентификатору.
		/// </summary>
		/// <param name="data">Объект записи.</param>
		/// <param name="condition">Дополнительное условие для выборки.</param>
		void LoadData(IData data, string condition = null);

		/// <summary>
		/// Обновляет сведения для элементов коллекции внешней таблицы.
		/// </summary>
		/// <param name="collection">Объект коллекции.</param>
		void UpdateCollection(IDataCollection collection);

		/// <summary>
		/// Заполняет коллекцию данными из внешней таблицы.
		/// </summary>
		/// <param name="collection">Объект коллекции.</param>
		void FillCollection(IDataCollection collection);

		/// <summary>
		/// Создание таблицы в БД.
		/// </summary>
		/// <param name="table">Имя таблицы.</param>
		/// <param name="columnIdName">Имя ключевого поля.</param>
		/// <param name="columns">Список столбцов таблицы.</param>
		void CreateTable(string table, string columnIdName, List<string> columns);

		/// <summary>
		/// Добавление столбца в таблицу.
		/// </summary>
		/// <param name="table">Имя таблицы.</param>
		/// <param name="column">Имя столбца.</param>
		void AddColumn(string table, string column);

		/// <summary>
		/// Удаление столбца из таблицы.
		/// </summary>
		/// <param name="table">Имя таблицы.</param>
		/// <param name="column">Имя столбца.</param>
		void DeleteColumn(string table, string column);

		/// <summary>
		/// Удаление таблицы из БД.
		/// </summary>
		/// <param name="table">Имя таблицы.</param>
		void DeleteTable(string table);

		/// <summary>
		/// Загружает все данные заданного столбца.
		/// </summary>
		/// <param name="table">Имя таблицы.</param>
		/// <param name="column">Имя столбца.</param>
		/// <returns>Список значений столбца.</returns>
		List<string> GetColumn(string table, string column);

		/// <summary>
		/// Получение значения ячейки по заданному условию.
		/// </summary>
		/// <param name="table">Имя таблицы.</param>
		/// <param name="column">Имя столбца.</param>
		/// <param name="condition">Условие поиска.</param>
		/// <returns>Значение ячейки.</returns>
		int GetNumber(string table, string column, string condition);
	}
}
