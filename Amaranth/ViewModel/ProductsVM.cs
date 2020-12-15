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
        /// Содержит список новых тегов.
        /// </summary>
        List<Tag> _newTags;

        /// <summary>
        /// Конструктор посредника для формы товаров.
        /// </summary>
        public ProductsVM()
        {
            _db = DataBaseSinglFacade.GetInstance(); // Получение экземпляра Singleton
            // Установка параметров по умолчанию
            _isSelect = false;
            _newTags = new List<Tag>();
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

        Tag _currentTag;
        /// <summary>
        /// Для хранения текущего выбранного тега.
        /// </summary>
        public Tag CurrentTag
        {
            get => _currentTag;
            set => SetValue(ref _currentTag, value);
        }

        /// <summary>
        /// Создание объекта нового товара.
        /// </summary>
        public Command<Category> Create
        {
            get => new Command<Category>((c) =>
            {
                Product = new ProductInfo(c);
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
                if (_db.IsSetAdapter)
                    Product = _db.LoadInfo(p);
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
                _db.AddTags(_newTags); // Обновление списка тегов перед добавление товара
                _newTags.Clear();
                _db.Insert(_product);
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
                _db.AddTags(_newTags); // Обновление списка тегов перед изменением товара
                _db.Update(_product);
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
                if (CurrentTag == null) // Проверка, что задан новый тег
                {
                    CurrentTag = new Tag() { Title = TagField };
                    _newTags.Add(CurrentTag);
                }
                Product.AddTag(CurrentTag);
                TagField = "";
            }, () => TagField != "");
        }

        /// <summary>
        /// Удаление тега.
        /// </summary>
        public Command<Tag> RemoveTag
        {
            get => new Command<Tag>((tag) =>
            {
                if (_newTags.Contains(tag)) // Проверка, что тег добавлен недавно
                    _newTags.Remove(tag);
                Product.RemoveTag(tag);
            }, (tag) => tag != null);
        }
    }
}