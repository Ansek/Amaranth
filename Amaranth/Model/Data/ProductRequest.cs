using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Amaranth.Model.Data
{
    public class ProductRequest : INotifyPropertyChanged
    {
        string _title;
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
            set { _fromPrice = value; OnValueChanged(); }
        }

        public double ToPrice
        {
            get => _toPrice;
            set { _toPrice = value; OnValueChanged(); }
        }

        public int Category
        {
            get => _category;
            set { _category = value; OnValueChanged(); }
        }

        public ObservableCollection<string> Tags { get; }

        public void Copy(ProductRequest request)
        {
            _title = request.Title;
            _category = request.Category;
            _fromPrice = request.FromPrice;
            _toPrice = request.ToPrice;
            _checkTitle = request.CheckTitle;
            _checkTags = request.CheckTags;
            _checkPrice = request.CheckPrice;
            _checkCategory = request.CheckCategory;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
