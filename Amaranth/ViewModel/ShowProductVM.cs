using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;

namespace Amaranth.ViewModel
{
    class ShowProductVM : INotifyPropertyChanged
    {
        public ShowProductVM(Product product)
        {
            Product = DataBaseSinglFacade.LoadInfo(product);
            ListTags = new ObservableCollection<string>(DataBaseSinglFacade.LoadTags(Product.Id));
        }

        ProductInfo _product;
        public ProductInfo Product
        {
            get => _product;
            set { _product = value; OnValueChanged(); }
        }

        public ObservableCollection<string> ListTags { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
