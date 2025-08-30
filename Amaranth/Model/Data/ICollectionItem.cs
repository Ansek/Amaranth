using System.Collections.Generic;

namespace Amaranth.Model.Data
{
    /// <summary>
    /// Элемент коллекции для обновления. Для использования в интерфейсе IDataCollection.
    /// </summary>
    public interface ICollectionItem
    {
        /// <summary>
        /// Второе значение первичного ключа (составной ключ).
        /// </summary>
        int IdItem { get; }

        /// <summary>
        /// Флаг для отметки добавляемого значения.
        /// </summary>
        bool IsAdd { get; set; }

        /// <summary>
        /// Флаг для отметки удаляемого значения.
        /// </summary>
        bool IsDelete { get; set; }

        /// <summary>
        /// Получение данных об имени столбцах и их содержимом.
        /// </summary>
        /// <returns>Возвращает кортеж из имени столбца и его значения.</returns>
        IEnumerable<(string, object)> GetData();

        /// <summary>
        /// Заполнение данных по указанным столбцам.
        /// </summary>
        /// <param name="column">Имя столбца.</param>
        /// <param name="value">Значение столбца.</param>
        void SetData(string column, object value);
    }
}
