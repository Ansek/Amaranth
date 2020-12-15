using System.Collections.Generic;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы категорий.
    /// </summary>
    class СategoriesVM : BindableBase
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
        /// Конструктор посредника для формы категорий.
        /// </summary>
        public СategoriesVM()
        {
            _db = DataBaseSinglFacade.GetInstance(); // Получение экземпляра Singleton
            _db.CategoryListChanged += () =>
            {
                ListСategories = _db.GetCategoryList(); // Обновление списка
                Category = null;                        // Сброс текущей записи
            };
            // Установка параметров по умолчанию
            _isSelect = false;
            _descriptionTitle = string.Empty;
            ListСategories = _db.GetCategoryList();
        }

        Category _category;
        /// <summary>
        /// Выбранная категория.
        /// </summary>
        public Category Category
        {
            get => _category;
            set => SetValue(ref _category, value);
        }

        List<Category> _listСategories;
        /// <summary>
        /// Список категорий.
        /// </summary>
        public List<Category> ListСategories
        {
            get => _listСategories;
            set => SetValue(ref _listСategories, value);
        }

        string _descriptionTitle;
        /// <summary>
        /// Для хранения значения поля названия описания.
        /// </summary>
        public string DescriptionTitle
        {
            get => _descriptionTitle;
            set => SetValue(ref _descriptionTitle, value);
        }

        /// <summary>
        /// Установка значений выбранной категории.
        /// </summary>
        public Command<Category> SetCategory
        {
            get => new Command<Category>((u) =>
            {
                Category = new Category(u);
                _isSelect = true;
            }, (u) => u != null);
        }

        /// <summary>
        /// Создание объекта нового категории.
        /// </summary>
        public Command Create
        {
            get => new Command(() =>
            {
                Category = new Category();
                _isSelect = false;
            });
        }

        /// <summary>
        /// Добавление информации о категории в БД.
        /// </summary>
        public Command Add
        {
            get => new Command(() =>
            {
                _db.Insert(_category);
            }, () => _category != null && !_isSelect);
        }

        /// <summary>
        /// Изменение информации о категории в БД.
        /// </summary>
        public Command Change
        {
            get => new Command(() =>
            {
                _db.Update(_category);
            }, () => _category != null && _isSelect);
        }

        /// <summary>
        /// Удаление информации о категории из БД.
        /// </summary>
        public Command Delete
        {
            get => new Command(() =>
            {
                if (DialogueService.ShowWarning("Вы действительно хотите удалить информацию о данной категории?"))
                    _db.Delete(_category);
            }, () => _category != null && _isSelect);
        }

        /// <summary>
        /// Добавление пункта описания.
        /// </summary>
        public Command AddDescription
        {
            get => new Command(() =>
            {
                _category.AddDescription(_descriptionTitle);
                DescriptionTitle = "";
            }, () => _category != null && _descriptionTitle != string.Empty);
        }

        /// <summary>
        /// Удаление пункта описания.
        /// </summary>
        public Command<Description> RemoveDescription
        {
            get => new Command<Description>((desc) =>
            {
                _category.RemoveDescription(desc);
            }, (desc) => _category != null);
        }
    }
}
