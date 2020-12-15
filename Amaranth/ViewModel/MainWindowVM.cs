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
        /// Констуктор посредника для формы основного окна.
        /// </summary>
        public MainWindowVM()
        {
            MySqlAdapter mySql = new MySqlAdapter();
            Auth.GetInstance();
            Auth.SetAdapter(mySql);
            _db = DataBaseSinglFacade.GetInstance();
            _db.SetAdapter(mySql);

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

            Pages = new ObservableCollection<UserControl>()
            { 
                new OrderPage(),
                new ProductsPage(),
                new ReportsPage(),
                new UsersPage(),
                new СategoriesPage(),
                new ArrivalsPage(),
                new ListOrdersPage()
            };
            Auth.UserChanged += () =>
            {
                OnValueChanged("CurrentUser");
                if (Auth.CurrentUser != null)
                    if (Auth.CurrentUser.IsAdministrator)
                    { 
                        Pages.Clear();
                        Pages.Add(new ProductsPage());
                        Pages.Add(new ReportsPage());
                        Pages.Add(new UsersPage());
                        Pages.Add(new СategoriesPage());
                        Pages.Add(new ArrivalsPage());
                        Pages.Add(new ListOrdersPage());
                    }
                    else
                    {
                        Pages.Clear();
                        Pages.Add(new OrderPage());
                        Pages.Add(new ArrivalsPage());
                        Pages.Add(new ListOrdersPage());
                    }
            };
            ProductSearchVM.OpenProduct += (product) =>
            {
                var page = new ShowProductPage(product);
                Pages.Add(page);
                CurrentPage = page;
            };
        }
        
        /// <summary>
        /// Констуктор для инициализации статических свойств.
        /// </summary>
        static MainWindowVM()
        {
            Categories = new ObservableCollection<Category>();
            Tags = new ObservableCollection<Tag>();
            ProductTitles = new ObservableCollection<string>();
        }

        /// <summary>
        /// Для получение доступа к списку категорий.
        /// </summary>
        public static ObservableCollection<Category> Categories;

        /// <summary>
        /// Для получение доступа к списку тегов.
        /// </summary>
        public static ObservableCollection<Tag> Tags;

        /// <summary>
        /// Для получение доступа к заголовкам продуктов.
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
        public User CurrentUser => Auth.CurrentUser;

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
            get => new Command(() => Auth.SignOut());
        }

        /// <summary>
        /// Обновление списка категорий.
        /// </summary>
        void UpdateCategories()
        {
            Categories.Clear();
            foreach (var c in _db.GetCategoryList())
                Categories.Add(c);
        }

        /// <summary>
        /// Обновление списка тегов.
        /// </summary>
        void UpdateTags()
        {
            Tags.Clear();
            foreach (var c in _db.GetTagList())
                Tags.Add(c);
        }

        /// <summary>
        /// Обновление списка заголовков продуктов.
        /// </summary>
        void UpdateProductTitles()
        {
            ProductTitles.Clear();
            foreach (var t in _db.GetProductTitles())
                ProductTitles.Add(t);
        }
    }
}
