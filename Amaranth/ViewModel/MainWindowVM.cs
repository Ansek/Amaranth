using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Amaranth.Model;
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

        public MainWindowVM()
        {
            MySqlAdapter mySql = new MySqlAdapter();
            Auth.GetInstance();
            Auth.SetAdapter(mySql);
            DataBaseSinglFacade.GetInstance();
            DataBaseSinglFacade.SetAdapter(mySql);
            Pages = new ObservableCollection<UserControl>
            {
                new OrderPage(),
                new ProductsPage(),
                new ReportsPage(),
                new UsersPage(),
                new СategoriesPage()
            };
        }

        public Command<UserControl> SetPage
        {
            get => new Command<UserControl>((p) =>
            {
                CurrentPage = p;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
