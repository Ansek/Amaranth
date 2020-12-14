using System.Collections;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
    /// <summary>
    /// Расширяет информацию о продукте.
    /// </summary>
	public class ProductInfo : Product, IData, IEnumerable<Description>
	{
        /// <summary>
        /// Для хранения значения внешнего параметра.
        /// </summary>
        List<string> _values;

        /// <summary>
        /// Конструктор для объекта информации о продуктах. 
        /// </summary>
        /// <param name="category">Категория, которая определяет дополнительные поля.</param>
        public ProductInfo(Category category) : base(category)
        {
            _values = new List<string>();
            // Резервирование места для дополнительных значений
            for (int i = 0; i < category.Count; i++)
                _values.Add(string.Empty);
        }

        /// <summary>
        /// Копирующий констуктор для объекта информации о продуктах.
        /// </summary>
        /// <param name="product">Родительский объект данного продукта.</param>
        /// <param name="values">Набор дополнительных значений.</param>
        public ProductInfo(Product product, List<string> values) : base(product.Category)
        {
            _id = product.Id;
            _title = product.Title;
            _price = product.Price;
            _count = product.Count;
            _prescription = product.Prescription;
            _values = values;
        }

        /// <summary>
        /// Возвращает перечислитель, для вывода дополнительных значений.
        /// </summary>
        /// <returns>Возвращает набор объектов описания.</returns>
        public IEnumerator<Description> GetEnumerator()
        {
            int i = 0;
            // Получение шаблона описания из категории
            foreach (var desc in _category)
            {
                desc.Index = i;             // Сохранение параметра индекса для связи с _values 
                desc.Value = _values[i];    // Передача i-го значения 
                // Установка события для оповощения об изменении значения 
                desc.ValueChanged += (s, j) => { _values[j] = s; };
                yield return desc;
                i++;
            }
        }

        /// <summary>
        /// Возвращает перечислитель, выполняющий перебор элементов по коллекции.
        /// </summary>
        /// <returns>Перечислитель, который можно использовать для итерации по коллекции.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /*--- Свойства и методы для интерфейса IData ---*/

        /// <summary>
        /// Значений первичного ключа.
        /// </summary>
        public new object IdColumn => _id;

        /// <summary>
        /// Имя столбца значения первичного ключа.
        /// </summary>
        public new string IdColumnName => "idProduct";

        /// <summary>
        /// Имя таблицы.
        /// </summary>
        public new string Table => $"CategoryDescriptions{_category.Id}";

        /// <summary>
        /// Получение данных об имени столбцах и их содержимом.
        /// </summary>
        /// <returns>Возвращает кортеж из имени столбца и его значения.</returns>
        public new IEnumerable<(string, object)> GetData()
        {
            for (int i = 0; i < _values.Count; i++)
                 yield return ($"Desc{i}", _values[i]);
        }

        /// <summary>
        /// Заполнение данных по указанным столбцам.
        /// </summary>
        /// <param name="data">Кортеж из имени столбца и его значения.</param>
        public new void SetData(IEnumerable<(string, object)> data)
        {
            int i = 0;
            foreach (var d in data)
            {
                if (d.Item1 == $"Desc{i}")
                    _values[i] = d.Item2 as string;
                i++;
            }
        }
    }
}
