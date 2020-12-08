using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Amaranth.Model.Data
{
    public class ProductRequest : INotifyPropertyChanged
    {
        string _title, _fromPriceText, _toPriceText;
        int _category;
        double _fromPrice, _toPrice;
        bool _checkTitle, _checkTags, _checkPrice, _checkCategory;

        public ProductRequest()
        {
            _checkTitle = true;
            Tags = new ObservableCollection<string>();
        }

        public bool CheckTitle
        {
            get => _checkTitle;
            set { _checkTitle = value; OnValueChanged(); }
        }

        public bool CheckTags
        {
            get => _checkTags;
            set { _checkTags = value; OnValueChanged(); }
        }

        public bool CheckPrice
        {
            get => _checkPrice;
            set { _checkPrice = value; OnValueChanged(); }
        }

        public bool CheckCategory
        {
            get => _checkCategory;
            set { _checkCategory = value; OnValueChanged(); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnValueChanged(); }
        }

        public double FromPrice
        {
            get => _fromPrice;
            set { _fromPrice = value;  OnValueChanged(); }
        }

        public double ToPrice
        {
            get => _toPrice;
            set { _toPrice = value; OnValueChanged(); }
        }

        public string FromPriceText
        {
            get => _fromPriceText;
            set { _fromPriceText = value; double.TryParse(value, out _fromPrice); OnValueChanged(); }
        }

        public string ToPriceText
        {
            get => _toPriceText;
            set { _toPriceText = value; double.TryParse(value, out _toPrice); OnValueChanged(); }
        }

        public int Category
        {
            get => _category;
            set { _category = value; OnValueChanged(); }
        }

        public ObservableCollection<string> Tags { get; }

        public void Copy(ProductRequest request)
        {
            _title = request._title;
            _category = request._category;
            _fromPrice = request._fromPrice;
            _toPrice = request._toPrice;
            _checkTitle = request._checkTitle;
            _checkTags = request._checkTags;
            _checkPrice = request._checkPrice;
            _checkCategory = request._checkCategory;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
