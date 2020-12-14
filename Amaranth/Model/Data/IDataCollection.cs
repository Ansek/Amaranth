using System.Collections.Generic;

namespace Amaranth.Model.Data
{
    interface IDataCollection
    {
        /// <summary>
        /// Имя таблицы для хранения коллекции.
        /// </summary>
        string CollectionTable { get; }

        /// <summary>
        /// Получение данных об элементе коллекции.
        /// </summary>
        /// <returns>Возвращает интерфейс на элемент.</returns>
        IEnumerable<IData> GetDataCollection();

        /// <summary>
        /// Передает данные для заполнения коллекции.
        /// </summary>
        /// <param name="data">Возвращает интерфейс на элемент.</param>
        void SetDataCollection(IEnumerable<IData> data);
    }
}
