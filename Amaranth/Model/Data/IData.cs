using System.Collections.Generic;

namespace Amaranth.Model.Data
{
    /// <summary>
    /// Интерфейс для предоставления доступа к БД через адаптер.
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// Значений первичного ключа.
        /// </summary>
        object IdColumn { get; }

        /// <summary>
        /// Имя столбца значения первичного ключа.
        /// </summary>
        string IdColumnName { get; }

        /// <summary>
        /// Имя таблицы.
        /// </summary>
        string Table { get; }

        /// <summary>
        /// Получение данных об имени столбцах и их содержимом.
        /// </summary>
        /// <returns>Возвращает кортеж из имени столбца и его значения.</returns>
        IEnumerable<(string, object)> GetData();

        /// <summary>
        /// Заполнение данных по указанным столбцам.
        /// </summary>
        /// <param name="data">Кортеж из имени столбца и его значения.</param>
        void SetData(IEnumerable<(string, object)> data);
    }
}
