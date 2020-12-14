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
        /// Констуктор посредника для формы основного окна.
        /// </summary>
        public MainWindowVM()
        {
            MySqlAdapter mySql = new MySqlAdapter();
            Auth.GetInstance();
            Auth.SetAdapter(mySql);
            var db = DataBaseSinglFacade.GetInstance();
            db.SetAdapter(mySql);

            Pages = new ObservableCollection<UserControl>();
            //    new OrderPage(),
            //    new ProductsPage(),
            //    new ReportsPage(),
            //    new UsersPage(),
            //    new СategoriesPage(),
            //    new ArrivalsPage(),
            //    new ListOrdersPage()
            //};
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
    }
}
