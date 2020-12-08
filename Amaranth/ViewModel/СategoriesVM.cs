using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class СategoriesVM : INotifyPropertyChanged
    {
        bool _isSelect;

        Category _category;
        public Category Category
        {
            get => _category;
            set { _category = value; OnValueChanged(); }
        }

        List<Category> _listСategories;
        public List<Category> ListСategories
        {
            get => _listСategories;
            set { _listСategories = value; OnValueChanged(); }
        }

        string _descriptionTitle;
        public string DescriptionTitle
        {
            get => _descriptionTitle;
            set { _descriptionTitle = value; OnValueChanged(); }
        }

        public СategoriesVM()
        {
            _isSelect = false;
            _descriptionTitle = string.Empty;
            ListСategories = DataBaseSinglFacade.GetListCategory();
        }

        public Command<Category> SetCategory
        {
            get => new Command<Category>((u) =>
            {
                Category = new Category(u);
                _isSelect = true;
            }, (u) => u != null);
        }

        public Command Create
        {
            get => new Command(() =>
            {
                Category = new Category();
                _isSelect = false;
            });
        }

        public Command Add
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Insert(_category);
                ListСategories = DataBaseSinglFacade.GetListCategory();
                Category = null;
            }, () => _category != null && !_isSelect);
        }

        public Command Change
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Update(_category);
                ListСategories = DataBaseSinglFacade.GetListCategory();
                Category = null;
            }, () => _category != null && _isSelect);
        }

        public Command Delete
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.Delete(_category);
                ListСategories = DataBaseSinglFacade.GetListCategory();
                Category = null;
            }, () => _category != null && _isSelect);
        }

        public Command AddDescription
        {
            get => new Command(() =>
            {
                _category.AddDescription(_descriptionTitle);
                DescriptionTitle = "";
            }, () => _category != null && _descriptionTitle != string.Empty);
        }

        public Command<Description> RemoveDescription
        {
            get => new Command<Description>((desc) =>
            {
                _category.RemoveDescription(desc);
            }, (desc) => _category != null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
