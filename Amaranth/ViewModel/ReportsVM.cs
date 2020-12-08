using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class ReportsVM : INotifyPropertyChanged
    {
        Report _report;

        ProductRequest _request;
        public ProductRequest Request
        {
            get => _request;
            set { _request = value; OnValueChanged(); }
        }

        string _tagField;
        public string TagField
        {
            get => _tagField;
            set { _tagField = value; OnValueChanged(); }
        }

        public ReportsVM()
        {
            _report = new Report(new MySqlAdapter());
            _request = new ProductRequest();
            _request.CheckTitle = true;
            _request.CheckDateComplited = true;
        }

        public Command Print
        {
            get => new Command(() =>
            {
                _report.Print(_request);
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
