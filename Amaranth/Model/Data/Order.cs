using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using System.Collections;

namespace Amaranth.Model.Data
{
    public class Order : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable<Product>
    {
        int _id;
        double _finalPrice;
        DateTime? _creationDate, _completionDate;
        List<Product> _products;

        public Order()
        {
            _id = -1;
            _finalPrice = 0;
            _completionDate = null;
            _products = new List<Product>();
        }

        public int Id
        {
            get => _id;
            set { _id = value; OnValueChanged(); }
        }

        public DateTime? CreationDate
        {
            get => _creationDate;
            set { _creationDate = value; OnValueChanged(); }
        }

        public DateTime? CompletionDate
        {
            get => _completionDate;
            set { _completionDate = value; OnValueChanged(); }
        }

        public double FinalPrice
        {
            get => _finalPrice;
            set { _finalPrice = value; OnValueChanged(); }
        }

        public void Add(Product product)
        {
            foreach (var p in _products)
                if (p.Id == product.Id)
                    return;

            _products.Add(product);
            FinalPrice += product.Count * product.Price;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Delete(Product product)
        {
            _products.Remove(product);
            FinalPrice -= product.Count * product.Price;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ChangeCount(int id, int count)
        {
            for (int i = 0; i < _products.Count; i++)
                if (_products[i].Id == id)
                {
                    FinalPrice += (count - _products[i].Count) * _products[i].Price;
                    _products[i].Count = count;
                    break;
                }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public IEnumerator<Product> GetEnumerator()
        {
            return _products.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _products.GetEnumerator();
        }
    }
}
