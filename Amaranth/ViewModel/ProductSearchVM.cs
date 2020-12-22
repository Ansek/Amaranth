using System;
using System.Collections.Generic;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы поиска товара.
    /// </summary>
    class ProductSearchVM : BindableBase
    {
        /// <summary>
        /// Для доступа к функциям БД.
        /// </summary>
        readonly DataBaseSinglFacade _db;

        /// <summary>
        /// Задает максимальное количество отображаемых заказов в таблице.
        /// </summary>
        int _countAll;

        /// <summary>
        /// Для хранения информации о ранее выполненном запросе. Для страниц перехода.
        /// </summary>
        readonly ProductRequest _oldRequest;

        /// <summary>
        /// Оповещает о намерении открыть информацию о продукте.
        /// </summary>
        public static event Action<Product> OpenProduct;

        /// <summary>
        /// Конструктор посредника для формы поиска товара.
        /// </summary>
        public ProductSearchVM()
        {
            _db = DataBaseSinglFacade.GetInstance(); // Получение экземпляра Singleton
            // Привязка к событию изменения списка товаров
            _db.ProductListChanged += () =>
            {
                var categories = MainWindowVM.Categories;
                ListProducts = _db.GetProductList(categories, _oldRequest, _countAll, Position);  // Обновление списка
            };
            // Установка параметров по умолчанию
            _countAll = 10;
            _currentNumber = 1;
            _maxNumber = 1;
            _request = new ProductRequest();
            _oldRequest = new ProductRequest();
            // Загрузка и отображение списка товаров
            ListProducts = _db.GetProductList(MainWindowVM.Categories, _oldRequest, _countAll, Position);
        }

        List<Product> _listProducts;
        /// <summary>
        /// Содержит список найденных товаров.
        /// </summary>
        public List<Product> ListProducts
        {
            get => _listProducts;
            set => SetValue(ref _listProducts, value);
        }

        ProductRequest _request;
        /// <summary>
        /// Объект с параметрами запроса.
        /// </summary>
        public ProductRequest Request
        {
            get => _request;
            set => SetValue(ref _request, value);
        }

        int _currentNumber;
        /// <summary>
        /// Текущий номер страницы.
        /// </summary>
        public int CurrentNumber
        {
            get => _currentNumber;
            set => SetValue(ref _currentNumber, value);
        }

        int _maxNumber;
        /// <summary>
        /// Максимальный номер страницы.
        /// </summary>
        public int MaxNumber
        {
            get => _maxNumber;
            set => SetValue(ref _maxNumber, value);
        }

        /// <summary>
        /// Определяет на какой позиции надо извлечь данные.
        /// </summary>
        public int Position => _countAll * (_currentNumber - 1);

        string _tagField;
        /// <summary>
        /// Для хранения поля ввода имени тега.
        /// </summary>
        public string TagField
        {
            get => _tagField;
            set => SetValue(ref _tagField, value);
        }

        Command<Product> _setProduct;
        /// <summary>
        /// Установка значения выбранного товара.
        /// </summary>
        public Command<Product> SetProduct
        {
            get => _setProduct;
            set => SetValue(ref _setProduct, value);
        }

        /// <summary>
        /// Запуск поиска товаров.
        /// </summary>
        public Command Find
        {
            get => new Command(() =>
            {
                var categories = MainWindowVM.Categories;
                ListProducts = _db.GetProductList(categories, _request, _countAll, Position);  // Обновление списка
                _oldRequest.Copy(_request); // Копирование запроса (в случае перехода на другую страницу)
            });
        }

        /// <summary>
        /// Добавление тега для поиска.
        /// </summary>
        public Command<Tag> AddTag
        {
            get => new Command<Tag>((tag) =>
            {
                if (!Request.Tags.Contains(tag)) // Сохранение только уникальных тегов
                    Request.Tags.Add(tag);
                TagField = "";
            }, (tag) => tag != null);
        }

        /// <summary>
        /// Удаление тега.
        /// </summary>
        public Command<Tag> RemoveTag
        {
            get => new Command<Tag>((tag) =>
            {
                Request.Tags.Remove(tag);
            }, (tag) => tag != null);
        }

        /// <summary>
        /// Устанавливает максимальное количество отображаемых заказов в таблице.
        /// </summary>
        public Command<int> SetCountAll
        {
            get => new Command<int>((count) =>
            {
                _countAll = count;
                var categories = MainWindowVM.Categories;
                ListProducts = _db.GetProductList(categories, _oldRequest, _countAll, Position);  // Обновление списка
            });
        }

        /// <summary>
        /// Переход на станицу назад.
        /// </summary>
        public Command GoPrevious
        {
            get => new Command(() =>
            {
                CurrentNumber--;
                var categories = MainWindowVM.Categories;
                ListProducts = _db.GetProductList(categories, _oldRequest, _countAll, Position);  // Обновление списка
            }, () => _currentNumber != 1);
        }

        /// <summary>
        /// Переход на станицу вперед.
        /// </summary>
        public Command GoNext
        {
            get => new Command(() =>
            {
                CurrentNumber++;
                var categories = MainWindowVM.Categories;
                ListProducts = _db.GetProductList(categories, _oldRequest, _countAll, Position);  // Обновление списка
            }, () => _currentNumber != _maxNumber);
        }

        /// <summary>
        /// Открывает страницу информации о товаре.
        /// </summary>
        public Command<Product> OpenInfo
        {
            get => new Command<Product>((i) => OpenProduct?.Invoke(i)); 
        }
    }
}
