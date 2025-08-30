using System.Collections.Generic;

namespace Amaranth.Model.Data
{
    /// <summary>
    /// Расширяет информацию о продукте.
    /// </summary>
	public class ProductInfo : Product, IData
    {
        /// <summary>
        /// Для хранения значения внешнего параметра.
        /// </summary>
        readonly List<string> _values;

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
        /// Копирующий конструктор для объекта информации о продуктах.
        /// </summary>
        /// <param name="product">Родительский объект данного продукта.</param>
        public ProductInfo(Product product) : base(product)
        {
            _values = new List<string>();
            // Резервирование места для дополнительных значений
            for (int i = 0; i < _category.Count; i++)
                _values.Add(string.Empty);
        }

        /// <summary>
        /// Возвращает перечислитель, для вывода дополнительных значений.
        /// </summary>
        public IEnumerable<Description> Description
        {
            get
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
        }

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
        public IEnumerable<(string, object)> GetData()
        {
            yield return ($"idProduct", Id);
            foreach (var desc in Description)
                yield return ($"Desc{desc.Id}", _values[desc.Index]);                 
        }

        /// <summary>
        /// Заполнение данных по указанным столбцам.
        /// </summary>
        /// <param name="column">Имя столбца.</param>
        /// <param name="value">Значение столбца.</param>
        public new void SetData(string column, object value)
        {
            foreach (var desc in Description)
                if (column == $"Desc{desc.Id}")
                {
                    _values[desc.Index] = value as string;
                    break;
                }
        }
    }
}
