using System;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
    /// <summary>
    /// Хранит данные о заказе.
    /// </summary>
    public class Order : BindableBaseCollection<Product>, IData, IDataCollection
    {
        /// <summary>
        /// Конструктор для объекта заказа.
        /// </summary>
        public Order()
        {
            _id = -1;
            _finalPrice = 0;
            _completionDate = null;
        }

        int _id;
        /// <summary>
        /// Идентификатор заказа.
        /// </summary>
        public int Id
        {
            get => _id;
            set => SetValue(ref _id, value);
        }

        DateTime? _creationDate;
        /// <summary>
        /// Дата оформления заказа.
        /// </summary>
        public DateTime? CreationDate
        {
            get => _creationDate;
            set => SetValue(ref _creationDate, value);
        }

        DateTime? _completionDate;
        /// <summary>
        /// Дата завершения заказа.
        /// </summary>
        public DateTime? CompletionDate
        {
            get => _completionDate;
            set => SetValue(ref _completionDate, value);
        }

        double _finalPrice;
        /// <summary>
        /// Для вывода итоговой стоимости заказа.
        /// </summary>
        public double FinalPrice
        {
            get => _finalPrice;
            set => SetValue(ref _finalPrice, value);
        }

        /// <summary>
        /// Добавления товара в список заказа.
        /// </summary>
        /// <param name="product">Добавляемый товар.</param>
        public void Add(Product product)
        {
            // Проверка, что товара нет в списке
            foreach (var p in _list)
                if (p.Id == product.Id)
                    return;

            product.IsAdd = true;
            _list.Add(product);
            // Перерасчет итоговой суммы
            FinalPrice += product.CountProduct * product.Price;

            OnCollectionChanged();	// Оповещение формы об изменении
        }

        /// <summary>
        /// Удаление товара из списка заказа.
        /// </summary>
        /// <param name="product">Удаляемый товар.</param>
        public void Delete(Product product)
        {
            _list.Remove(product); 
            // Перерасчет итоговой суммы
            FinalPrice -= product.CountProduct * product.Price;

            OnCollectionChanged();	// Оповещение формы об изменении
        }

        /// <summary>
        /// Изменение количества продуктов в списке.
        /// </summary>
        /// <param name="id">Идентификатор продукта.</param>
        /// <param name="count">Новое значение количества.</param>
        public void ChangeCount(int id, int count)
        {
            // Поиск продукта с указанным идентификатором
            for (int i = 0; i < _list.Count; i++)
                if (_list[i].Id == id)
                {
                    // Перерасчет итоговой суммы
                    FinalPrice += (count - _list[i].CountProduct) * _list[i].Price;
                    // Обновление количества
                    _list[i].CountProduct = count;
                    break;
                }
        }

        /*--- Свойства и методы для интерфейса IData ---*/

        /// <summary>
        /// Значений первичного ключа.
        /// </summary>
        public object IdColumn => _id;

        /// <summary>
        /// Имя столбца значения первичного ключа.
        /// </summary>
        public string IdColumnName => "idOrder";

        /// <summary>
        /// Имя таблицы.
        /// </summary>
        public string Table => "`Order`";

        /// <summary>
        /// Получение данных об имени столбцах и их содержимом.
        /// </summary>
        /// <returns>Возвращает кортеж из имени столбца и его значения.</returns>
        public IEnumerable<(string, object)> GetData()
        {
            yield return ("CreationDate", _creationDate);
            yield return ("CompletionDate", _completionDate);
        }

        /// <summary>
        /// Заполнение данных по указанным столбцам.
        /// </summary>
        /// <param name="column">Имя столбца.</param>
        /// <param name="value">Значение столбца.</param>
        public void SetData(string column, object value)
        {
            switch (column)
            {
                case "idOrder":
                    Id = Convert.ToInt32(value);
                    break;
                case "CreationDate":
                    CreationDate = value as DateTime?;
                    break;
                case "CompletionDate":
                    CompletionDate = value as DateTime?;
                    break;
            }
        }

        /*--- Свойства и методы для интерфейса IDataCollection ---*/

        public string CollectionTable => $"Order_Product";

        public string IdItemName => "idProduct";

        /// <summary>
        /// Получение данных об элементе коллекции.
        /// </summary>
        /// <returns>Возвращает интерфейс на элемент.</returns>
        public IEnumerable<ICollectionItem> GetDataCollection()
        {
            for (int i = 0; i < _list.Count; i++)
                yield return _list[i];
        }

        /// <summary>
        /// Создает новый объект коллекции и возвращает интерфейс для заполнения.
        /// </summary>
        /// <returns>Объект для заполнения.</returns>
        public ICollectionItem CreateItem()
        {
            var prod = new Product(new Category());
            _list.Add(prod);
            return prod;
        }
    }
}
