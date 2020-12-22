using System;
using System.Collections.ObjectModel;

namespace Amaranth.Model.Data
{
    /// <summary>
    /// Задает параметры для поиска товаров в БД.
    /// </summary>
    public class ProductRequest : BindableBase
    {
        /// <summary>
        /// Конструктор для параметров запроса товаров.
        /// </summary>
        public ProductRequest()
        {
            _checkTitle = true;
            Tags = new ObservableCollection<Tag>();
            _title = string.Empty;
            _fromDate = DateTime.Now;
            _fromDate = _fromDate.AddMonths(-1);
            _recordsCount = 1;
            _toDate = DateTime.Now;
        }

        bool _checkTitle;
        /// <summary>
        /// Задает флаг поиска по наименованию.
        /// </summary>
        public bool CheckTitle
        {
            get => _checkTitle;
            set => SetValue(ref _checkTitle, value);
        }

        string _title;
        /// <summary>
        /// Текст для поиска по наименованию.
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetValue(ref _title, value);
        }

        bool _checkRecordsCount;
        /// <summary>
        /// Задает флаг на ограничение количества найденных записей.
        /// </summary>
        public bool CheckRecordsCount
        {
            get => _checkRecordsCount;
            set => SetValue(ref _checkRecordsCount, value);
        }

        int _recordsCount;
        /// <summary>
        /// Устанавливает ограничение на количество найденных записей.
        /// </summary>
        public int RecordsCount
        {
            get => _recordsCount;
            set { if (value > 0) SetValue(ref _recordsCount, value); }
        }

        bool _checkTags;
        /// <summary>
        /// Задает флаг на проверку по тегам.
        /// </summary>
        public bool CheckTags
        {
            get => _checkTags;
            set => SetValue(ref _checkTags, value);
        }

        /// <summary>
        /// Список тегов.
        /// </summary>
        public ObservableCollection<Tag> Tags { get; }

        bool _checkPrice;
        /// <summary>
        /// Задает флаг на проверку вхождения в диапазон цен. 
        /// </summary>
        public bool CheckPrice
        {
            get => _checkPrice;
            set => SetValue(ref _checkPrice, value);
        }

        double _fromPrice;
        /// <summary>
        /// Минимальное значение цены.
        /// </summary>
        public double FromPrice
        {
            get => _fromPrice;
            set => SetValue(ref _fromPrice, value);
        }

        double  _toPrice;
        /// <summary>
        /// Максимальное значение цены.
        /// </summary>
        public double ToPrice
        {
            get => _toPrice;
            set => SetValue(ref _toPrice, value);
        }

        string _fromPriceText;
        /// <summary>
        /// Текстовое значение минимальной цены для проверки на форме.
        /// </summary>
        public string FromPriceText
        {
            get => _fromPriceText;
            set { SetValue(ref _fromPriceText, value); double.TryParse(value, out _fromPrice); OnValueChanged("FromPrice"); }
        }

        string _toPriceText;
        /// <summary>
        /// Текстовое значение минимальной цены для проверки на форме.
        /// </summary>
        public string ToPriceText
        {
            get => _toPriceText;
            set { SetValue(ref _toPriceText, value); double.TryParse(value, out _toPrice); OnValueChanged("ToPrice"); }
        }

        bool _checkCategory;
        /// <summary>
        /// Задает флаг на определение категории товара.
        /// </summary>
        public bool CheckCategory
        {
            get => _checkCategory;
            set => SetValue(ref _checkCategory, value);
        }

        int _category;
        /// <summary>
        /// Идентификатор категории товара.
        /// </summary>
        public int Category
        {
            get => _category;
            set { _category = value; OnValueChanged(); }
        }

        bool _checkDateComplited;
        /// <summary>
        /// Задает флаг на проверку вхождения в диапазон даты завершения заказа. 
        /// </summary>
        public bool CheckDateComplited
        {
            get => _checkDateComplited;
            set { _checkDateComplited = value; OnValueChanged(); }
        }

        DateTime _fromDate;
        /// <summary>
        /// Ранее значение даты диапазона.
        /// </summary>
        public DateTime FromDate
        {
            get => _fromDate;
            set => SetValue(ref _fromDate, value);
        }

        DateTime _toDate;
        /// <summary>
        /// Позднее значение даты диапазона.
        /// </summary>
        public DateTime ToDate
        {
            get => _toDate;
            set => SetValue(ref _toDate, value);
        }

        /// <summary>
        /// Копирует данные запроса.
        /// </summary>
        /// <param name="request">Объект запроса.</param>
        public void Copy(ProductRequest request)
        {
            _title = request._title;
            _category = request._category;
            _fromPrice = request._fromPrice;
            _toPrice = request._toPrice;
            _checkTitle = request._checkTitle;
            _checkTags = request._checkTags;
            _checkPrice = request._checkPrice;
            _checkCategory = request._checkCategory;
        }

        /// <summary>
        /// Формирует SQL-фрагмент для запроса к БД.
        /// </summary>
        /// <returns>SQL-фрагмент.</returns>
        public string GetCondition()
        {
            string condition = string.Empty;

            // Установка условий для поиска по тегам
            if (_checkTags && Tags.Count > 0)
            {
                var str = string.Empty;
                foreach (var t in Tags)
                {
                    if (str != string.Empty)
                        str += " OR ";
                    str += $"idTag = {t.Id}";

                }
                condition += $"(idProduct, {Tags.Count}) IN (SELECT idProduct, count(idTag) FROM product_tag WHERE {str} GROUP BY idProduct)";
            }

            // Установка условия поиска по наименованию
            if (_checkTitle && _title != string.Empty)
            {
                if (condition != string.Empty)
                    condition += " AND ";
                if (_checkDateComplited)
                    condition += $"Title = '{_title}'";       // Для отчётов требуется точный поиск
                else
                    condition += $"Title LIKE '%{_title.Replace("'", @"\'")}%'";    // Для поиска товаров лучше поиск на вхождение
            }

            // Установка условия поиска по цене
            if (_checkPrice)
            {
                if (condition != string.Empty)
                    condition += " AND ";
                condition += $"{_fromPrice} <= Price AND Price <= {_toPrice}";
            }

            // Установка условия поиска по категории
            if (_checkCategory)
            {
                if (condition != string.Empty)
                    condition += " AND ";
                condition += $"idCategory = {_category}";
            }

            // Установка условия по проверке даты завершения
            if (_checkDateComplited)
            {
                var cond = string.Empty;
                if (_checkRecordsCount) // Если требуется ограничить количество записей
                    cond = $"LIMIT 0, {_recordsCount}";

                if (condition != string.Empty)
                    condition += " AND ";

                // Установка запроса с ограничением даты
                condition = $"idProduct IN (" +
                    $"SELECT idProduct " +
                    $"FROM (SELECT idProduct, sum(Count) AS Count " +
                    $"FROM sale_view " +
                    $"WHERE {condition} Date " +
                    $"BETWEEN '{_fromDate:yyyy-MM-dd}' " +
                    $"AND '{_toDate:yyyy-MM-dd}' " +
                    $"GROUP BY idProduct " +
                    $"ORDER BY Count DESC " +
                    $"{cond}) AS t)";
            }

            return condition;
        }
    }
}
