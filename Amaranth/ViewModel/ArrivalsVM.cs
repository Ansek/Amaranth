using System.ComponentModel;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class ArrivalsVM : INotifyPropertyChanged
    {
        ProductInfo _product;
        public ProductInfo Product
        {
            get => _product;
            set { _product = value; OnValueChanged(); }
        }

        int _editableCount;
        public int EditableCount
        {
            get => _editableCount;
            set { _editableCount = value; OnValueChanged(); }
        }

        public Command<Product> SetProduct
        {
            get => new Command<Product>((p) =>
            {
                EditableCount = p.Count;
                Product = DataBaseSinglFacade.LoadInfo(p);
            }, (p) => p != null);
        }

        public Command SetCount
        {
            get => new Command(() =>
            {
                _product.Count = _editableCount;
                DataBaseSinglFacade.Update(_product, true);
            }, () => _editableCount > 0);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
