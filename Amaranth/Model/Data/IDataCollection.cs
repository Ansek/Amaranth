using System.Collections.Generic;

namespace Amaranth.Model.Data
{
    /// <summary>
    /// Интерфейс контейнера для обновлении коллекции.
    /// </summary>
    public interface IDataCollection
    {
        /// <summary>
        /// Первое значение первичного ключа (составной ключ).
        /// </summary>
        object IdColumn { get; }

        /// <summary>
        /// Имя столбца первого значения первичного ключа (составной ключ).
        /// </summary>
        string IdColumnName { get; }

        /// <summary>
        /// Имя столбца второго значения первичного ключа (составной ключ).
        /// </summary>
        string IdItemName { get; }

        /// <summary>
        /// Имя таблицы для хранения коллекции.
        /// </summary>
        string CollectionTable { get; }

        /// <summary>
        /// Получение данных об элементе коллекции.
        /// </summary>
        /// <returns>Возвращает интерфейс на элемент.</returns>
        IEnumerable<ICollectionItem> GetDataCollection();

        /// <summary>
        /// Создает новый объект коллекции и возвращает интерфейс для заполнения.
        /// </summary>
        /// <returns>Объект для заполнения.</returns>
        ICollectionItem CreateItem();
    }
}
