using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class ProductSearchVM : INotifyPropertyChanged
    {
        int _countAll;
        ProductRequest _oldRequest;

        public ProductSearchVM()
        {
            _countAll = 10;
            _currentNumber = 1;
            _maxNumber = 1;
            _request = new ProductRequest();
            _oldRequest = new ProductRequest();
            DataBaseSinglFacade.ProductChanged += () =>
            {
                int pos = _countAll * (_currentNumber - 1);
                ListProducts = DataBaseSinglFacade.GetListProduct(pos, _countAll, _oldRequest);
            };
        }

        public static event Action<Product> OpenProduct;

        List<Product> _listProducts;
        public List<Product> ListProducts
        {
            get => _listProducts;
            set { _listProducts = value; OnValueChanged(); }
        }

        ProductRequest _request;
        public ProductRequest Request
        {
            get => _request;
            set { _request = value; OnValueChanged(); }
        }

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

        string _tagField;
        public string TagField
        {
            get => _tagField;
            set { _tagField = value; OnValueChanged(); }
        }

        Command<Product> _setProduct;
        public Command<Product> SetProduct
        {
            get => _setProduct;
            set { _setProduct = value; OnValueChanged(); }
        }

        public Command Find
        {
            get => new Command(() =>
            {
                int pos = _countAll * (_currentNumber - 1);
                ListProducts = DataBaseSinglFacade.GetListProduct(pos, _countAll, _request);
                _oldRequest.Copy(_request);
            });
        }

        public Command AddTag
        {
            get => new Command(() =>
            {
                if (!Request.Tags.Contains(TagField))
                    Request.Tags.Add(TagField);
                TagField = "";
            }, () => TagField != "");
        }

        public Command<string> RemoveTag
        {
            get => new Command<string>((t) =>
            {
                Request.Tags.Remove(t);
            }, (t) => t != "");
        }

        public Command<int> SetCountAll
        {
            get => new Command<int>((count) =>
            {
                _countAll = count;
                int pos = count * (_currentNumber - 1);
                ListProducts = DataBaseSinglFacade.GetListProduct(pos, count, _oldRequest);
            });
        }

        public Command GoPrevious
        {
            get => new Command(() =>
            {
                CurrentNumber--;
                int pos = _countAll * (_currentNumber - 1);
                ListProducts = DataBaseSinglFacade.GetListProduct(pos, _countAll, _oldRequest);
            }, () => _currentNumber != 1);
        }

        public Command GoNext
        {
            get => new Command(() =>
            {
                CurrentNumber++;
                int pos = _countAll * (_currentNumber - 1);
                ListProducts = DataBaseSinglFacade.GetListProduct(pos, _countAll, _oldRequest);
            }, () => _currentNumber != _maxNumber);
        }

        public Command<Product> OpenInfo
        {
            get => new Command<Product>((i) => OpenProduct?.Invoke(i)); 
        }

        public ObservableCollection<Category> Categories => DataBaseSinglFacade.Categories;
        public ObservableCollection<string> ProductTitles => DataBaseSinglFacade.ProductTitles;
        public ObservableCollection<string> Tags => DataBaseSinglFacade.Tags;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
