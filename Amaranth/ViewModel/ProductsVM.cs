using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class ProductsVM : INotifyPropertyChanged
    {
        bool _isSelect;
        List<string> _oldTags;

        ProductInfo _product;
        public ProductInfo Product
        {
            get => _product;
            set { _product = value; OnValueChanged(); }
        }

        string _tagField;
        public string TagField
        {
            get => _tagField;
            set { _tagField = value; OnValueChanged(); }
        }

        public ObservableCollection<string> ListTags { get; }

        public ObservableCollection<Category> Categories => DataBaseSinglFacade.Categories;

        public ObservableCollection<string> Tags => DataBaseSinglFacade.Tags;

        public ProductsVM()
        {
            _isSelect = false;
            ListTags = new ObservableCollection<string>();
        }

        public Command<Category> Create
        {
            get => new Command<Category>((c) =>
            {
                Product = new ProductInfo(c);
                ListTags.Clear();
                _oldTags = null;
                _isSelect = false;
            }, (c) => c != null);
        }

        public Command<Product> SetProduct
        {
            get => new Command<Product>((p) =>
            {
                Product = DataBaseSinglFacade.LoadInfo(p);
                _oldTags = DataBaseSinglFacade.LoadTags(p.Id);
                ListTags.Clear();
                foreach (var t in _oldTags)
                    ListTags.Add(t);
                _isSelect = true;
            }, (p) => p != null);
        }

        public Command Add
        {
            get => new Command(() =>
            {
                int id = DataBaseSinglFacade.Insert(_product);
                DataBaseSinglFacade.AddTags(id, new List<string>(ListTags));
                Product = null;
            }, () => _product != null && !_isSelect);
        }

        public Command Change
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Update(_product);
                var list = new List<string>();
                foreach (var t in ListTags)
                    if (!_oldTags.Contains(t))
                        list.Add(t);
                DataBaseSinglFacade.AddTags(_product.Id, list);
                list.Clear();
                foreach (var t in _oldTags)
                    if (!ListTags.Contains(t))
                        list.Add(t);
                DataBaseSinglFacade.DeleteTags(_product.Id, list);
                Product = null;
            }, () => _product != null && _isSelect);
        }

        public Command Delete
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Delete(_product);
                var list = new List<string>();
                Product = null;
            }, () => _product != null && _isSelect);
        }

        public Command AddTag
        {
            get => new Command(() =>
            {
                if (!ListTags.Contains(TagField))
                    ListTags.Add(TagField);
                TagField = "";
            }, () => TagField != "");
        }

        public Command<string> RemoveTag
        {
            get => new Command<string>((t) =>
            {
                ListTags.Remove(t);
            }, (t) => t != "");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}