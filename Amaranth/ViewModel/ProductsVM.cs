using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class ProductsVM : INotifyPropertyChanged
    {
        bool _isSelect;
        int _countAll;

        int _currentNumber;
        public int CurrentNumber
        {
            get => _currentNumber;
            set { _currentNumber = value; OnValueChanged(); }
        }

        int _maxNumber;
        public int MaxNumber
        {
            get => _maxNumber;
            set { _maxNumber = value; OnValueChanged(); }
        }

        ProductInfo _product;
        public ProductInfo Product
        {
            get => _product;
            set { _product = value; OnValueChanged(); }
        }

        List<Product> _listProducts;
        public List<Product> ListProducts
        {
            get => _listProducts;
            set { _listProducts = value; OnValueChanged(); }
        }

        public List<Category> Categories => DataBaseSinglFacade.Categories;

        public ProductsVM()
        {
            _isSelect = false;
            _countAll = 10;
            _currentNumber = 1;
            _maxNumber = 1;
            ListProducts = DataBaseSinglFacade.GetListProduct(1, 10);
        }

        public Command<int> SetCountAll
        {
            get => new Command<int>((count) =>
            {
                _countAll = count;
                int pos = count * (_currentNumber - 1);
                ListProducts = DataBaseSinglFacade.GetListProduct(pos, count);
            });
        }

        public Command GoPrevious
        {
            get => new Command(() => CurrentNumber--, () => _currentNumber != 1);
        }

        public Command GoNext
        {
            get => new Command(() => CurrentNumber++, () => _currentNumber != _maxNumber);
        }

        public Command<Category> Create
        {
            get => new Command<Category>((c) =>
            {
                Product = new ProductInfo(c);
                _isSelect = false;
            }, (c) => c != null);
        }

        public Command<Product> SetProduct
        {
            get => new Command<Product>((p) =>
            {
                Product = DataBaseSinglFacade.LoadInfo(p);
                _isSelect = true;
            }, (p) => p != null);
        }

        public Command Add
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Insert(_product);
                int pos = _countAll * (_currentNumber - 1);
                ListProducts = DataBaseSinglFacade.GetListProduct(pos, _countAll);
                Product = null;
            }, () => _product != null && !_isSelect);
        }

        public Command Change
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Update(_product);
                int pos = _countAll * (_currentNumber - 1);
                ListProducts = DataBaseSinglFacade.GetListProduct(pos, _countAll);
                Product = null;
            }, () => _product != null && _isSelect);
        }

        public Command Delete
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Delete(_product);
                int pos = _countAll * (_currentNumber - 1);
                ListProducts = DataBaseSinglFacade.GetListProduct(pos, _countAll);
                Product = null;
            }, () => _product != null && _isSelect);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
