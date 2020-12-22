using System.Windows.Controls;
using System.Collections.ObjectModel;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.View;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы основного окна.
    /// </summary>
    class MainWindowVM : BindableBase
    {
        /// <summary>
        /// Для доступа к функциям БД.
        /// </summary>
        readonly DataBaseSinglFacade _db;

        /// <summary>
        /// Для доступа к функциям авторизации.
        /// </summary>
        readonly AuthSingl _auth;

        /// <summary>
        /// Конструктор посредника для формы основного окна.
        /// </summary>
        public MainWindowVM()
        {
            var mySql = new MySqlAdapter();             // Инициализация класса адаптера для СУБД MySql
            _auth = AuthSingl.GetInstance();            // Получение экземпляра Singleton
            _auth.SetAdapter(mySql);                    // Установка адаптера MySql
            _db = DataBaseSinglFacade.GetInstance();    // Получение экземпляра Singleton        
            _db.SetAdapter(mySql);                      // Установка адаптера MySql

            // Привязка к событию изменения списка категорий
            _db.CategoryListChanged += () =>
            {
                UpdateCategories();
            };
            UpdateCategories();
            // Привязка к событию изменения списка тегов
            _db.TagListChanged += () =>
            {
                UpdateTags();
            };
            UpdateTags();
            // Привязка к событию изменения списка товаров
            _db.ProductListChanged += () =>
            {
                UpdateProductTitles();
            };
            UpdateProductTitles();
            // Установка списка страниц
            Pages = new ObservableCollection<UserControl>();
            // Привязка к событию изменения текущего пользователя
            _auth.UserChanged += () =>
            {
                OnValueChanged("CurrentUser");  // Обновление информации на форме
                Pages.Clear();
                if (_auth.CurrentUser != null)
                    if (_auth.CurrentUser.IsAdministrator)  // Страницы для администратора
                    {
                        Pages.Add(new ProductsPage());
                        Pages.Add(new ReportsPage());
                        Pages.Add(new UsersPage());
                        Pages.Add(new СategoriesPage());
                        Pages.Add(new ArrivalsPage());
                        Pages.Add(new ListOrdersPage());
                    }
                    else                                    // Страницы для фармацепта
                    {
                        Pages.Add(new OrderPage());
                        Pages.Add(new ArrivalsPage());
                        Pages.Add(new ListOrdersPage());
                    }
            };
            // Привязка к события запроса на открытие страницы
            ProductSearchVM.OpenProduct += (product) =>
            {
                var page = new ShowProductPage(product);    // Создание новой страницы для товара
                Pages.Add(page);        // Добавление в список вкладок
                CurrentPage = page;     // Отображение страницы
            };
        }

        /// <summary>
        /// Конструктор для инициализации статических свойств.
        /// </summary>
        static MainWindowVM()
        {
            Categories = new ObservableCollection<Category>();
            Tags = new ObservableCollection<Tag>();
            ProductTitles = new ObservableCollection<string>();
        }

        /// <summary>
        /// Для получения доступа к списку категорий.
        /// </summary>
        public static ObservableCollection<Category> Categories;

        /// <summary>
        /// Для получения доступа к списку тегов.
        /// </summary>
        public static ObservableCollection<Tag> Tags;

        /// <summary>
        /// Для получения доступа к заголовкам продуктов.
        /// </summary>
        public static ObservableCollection<string> ProductTitles;

        /// <summary>
        /// Задает список для вкладок на форме.
        /// </summary>
        public ObservableCollection<UserControl> Pages { get; set; }

        UserControl _page;
        /// <summary>
        /// Для отображения текущей страницы.
        /// </summary>
        public UserControl CurrentPage
        {
            get => _page;
            set { _page = value; OnValueChanged(); }
        }

        /// <summary>
        /// Для отображения информации о текущем пользователе.
        /// </summary>
        public User CurrentUser => _auth.CurrentUser;

        public Command<UserControl> CloseInfo
        {
            get => new Command<UserControl>((p) =>
            {
                Pages.Remove(p);
            });
        }

        /// <summary>
        /// Вызов окна авторизации пользователя.
        /// </summary>
        public Command SignIn
        {
            get => new Command(() => { DialogueService.ShowLoginWindow(); });
        }

        /// <summary>
        /// Сброс информации о текущем пользователе.
        /// </summary>
        public Command SignOut
        {
            get => new Command(() => _auth.SignOut());
        }

        /// <summary>
        /// Обновление списка категорий.
        /// </summary>
        void UpdateCategories()
        {
            Categories.Clear();                         // Очистка
            foreach (var c in _db.GetCategoryList())    // Загрузка
                Categories.Add(c);                      // Добавление
        }

        /// <summary>
        /// Обновление списка тегов.
        /// </summary>
        void UpdateTags()
        {
            Tags.Clear();                           // Очистка
            foreach (var c in _db.GetTagList())     // Загрузка
                Tags.Add(c);                        // Добавление
        }

        /// <summary>
        /// Обновление списка заголовков продуктов.
        /// </summary>
        void UpdateProductTitles()
        {
            ProductTitles.Clear();                      // Очистка
            foreach (var t in _db.GetProductTitles())   // Загрузка
                ProductTitles.Add(t);                   // Добавление
        }
    }
}
