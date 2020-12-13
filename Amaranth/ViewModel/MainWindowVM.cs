using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.View;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class MainWindowVM : INotifyPropertyChanged
    {
        UserControl _page;

        public ObservableCollection<UserControl> Pages { get; set; }

        public UserControl CurrentPage
        {
            get => _page;
            set { _page = value; OnValueChanged(); }
        }

        public User CurrentUser => Auth.CurrentUser;

        public MainWindowVM()
        {
            MySqlAdapter mySql = new MySqlAdapter();
            Auth.GetInstance();
            Auth.SetAdapter(mySql);
            DataBaseSinglFacade.GetInstance();
            DataBaseSinglFacade.SetAdapter(mySql);

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

        public Command<UserControl> CloseInfo
        {
            get => new Command<UserControl>((p) =>
            {
                Pages.Remove(p);
            });
        }

        public Command SignIn
        {
            get => new Command(() => { DialogueService.ShowLoginWindow(); });
        }

        public Command SignOut
        {
            get => new Command(() => Auth.SignOut());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
