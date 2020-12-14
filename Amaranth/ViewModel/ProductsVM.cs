using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы товаров.
    /// </summary>
    class ProductsVM : BindableBase
    {
        /// <summary>
        /// Для доступа к функциям БД.
        /// </summary>
        readonly DataBaseSinglFacade _db;

        /// <summary>
        /// Флаг, что продукт был получен путем выбора из списка.
        /// </summary>
        bool _isSelect;

        /// <summary>
        /// Список загруженных тегов.
        /// </summary>
        List<string> _oldTags;

        /// <summary>
        /// Конструктор посредника для формы товаров.
        /// </summary>
        public ProductsVM()
        {
            _db = DataBaseSinglFacade.GetInstance(); // Получение экземпляра Singleton
            // Устанока параметров по умолчанию
            _isSelect = false;
            ListTags = new ObservableCollection<string>();
        }

        ProductInfo _product;
        /// <summary>
        /// Выбранный товар.
        /// </summary>
        public ProductInfo Product
        {
            get => _product;
            set => SetValue(ref _product, value);
        }

        string _tagField;
        /// <summary>
        /// Для хранения поля ввода имени тега.
        /// </summary>
        public string TagField
        {
            get => _tagField;
            set => SetValue(ref _tagField, value);
        }

        /// <summary>
        /// Создание объекта нового товара.
        /// </summary>
        public Command<Category> Create
        {
            get => new Command<Category>((c) =>
            {
                Product = new ProductInfo(c);
                ListTags.Clear();
                _oldTags = null;
                _isSelect = false;
                TagField = "";
            }, (c) => c != null);
        }

        /// <summary>
        /// Установка значения выбранного товара.
        /// </summary>
        public Command<Product> SetProduct
        {
            get => new Command<Product>((p) =>
            {
                Product = DataBaseSinglFacade.LoadInfo(p);
                _oldTags = DataBaseSinglFacade.LoadTags(p.Id);
                ListTags.Clear();
                foreach (var t in _oldTags)
                    ListTags.Add(t);
                _isSelect = true;
                TagField = "";
            }, (p) => p != null);
        }

        /// <summary>
        /// Добавление информации о товаре в БД.
        /// </summary>
        public Command Add
        {
            get => new Command(() =>
            {
                _db.Insert(_product);
                //int id = 0; // TO DO 
                //DataBaseSinglFacade.AddTags(id, new List<string>(ListTags));
                Product = null;
            }, () => _product != null && !_isSelect);
        }

        /// <summary>
        /// Изменение информации о товаре в БД.
        /// </summary>
        public Command Change
        {
            get => new Command(() =>
            {
                _db.Update(_product);
                var list = new List<string>();
                foreach (var t in ListTags)
                    if (!_oldTags.Contains(t))
                        list.Add(t);
                DataBaseSinglFacade.AddTags(_product.Id, list);
                list.Clear();
                foreach (var t in _oldTags)
                    if (!ListTags.Contains(t))
                        list.Add(t);
                DataBaseSinglFacade.DeleteTags(_product.Id, list);
                Product = null;
            }, () => _product != null && _isSelect);
        }

        /// <summary>
        /// Удаление информации о товаре из БД.
        /// </summary>
        public Command Delete
        {
            get => new Command(() =>
            {
                if (DialogueService.ShowWarning("Вы действительно хотите удалить информацию о данном товаре?"))
                {
                    _db.Delete(_product);
                    var list = new List<string>();
                    Product = null;
                }
            }, () => _product != null && _isSelect);
        }

        /// <summary>
        /// Добавление тега.
        /// </summary>
        public Command AddTag
        {
            get => new Command(() =>
            {
                if (!ListTags.Contains(TagField))
                    ListTags.Add(TagField);
                TagField = "";
            }, () => TagField != "");
        }

        /// <summary>
        /// Удаление тега.
        /// </summary>
        public Command<string> RemoveTag
        {
            get => new Command<string>((t) =>
            {
                ListTags.Remove(t);
            }, (t) => t != "");
        }

        /// <summary>
        /// Активный список тегов
        /// </summary>
        public ObservableCollection<string> ListTags { get; }
        /// <summary>
        /// Список категорий.
        /// </summary>
        public ObservableCollection<Category> Categories => DataBaseSinglFacade.Categories;
        /// <summary>
        /// Список тегов
        /// </summary>
        public ObservableCollection<string> Tags => DataBaseSinglFacade.Tags;
    }
}