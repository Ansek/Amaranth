using System.Collections.ObjectModel;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы генерации отчётов.
    /// </summary>
    class ReportsVM : BindableBase
    {
        /// <summary>
        /// Для вызова функций генерации отчёта.
        /// </summary>
        Report _report;

        /// <summary>
        /// Конструктор посредника для формы генерации отчётов.
        /// </summary>
        public ReportsVM()
        {
            _report = new Report(new MySqlAdapter());
            _request = new ProductRequest();
            _request.CheckTitle = true;
            _request.CheckDateComplited = true;
        }

        ProductRequest _request;
        /// <summary>
        /// Запрос получения данных для отчёта.
        /// </summary>
        public ProductRequest Request
        {
            get => _request;
            set => SetValue(ref _request, value);
        }

        string _tagField;
        /// <summary>
        /// Для хранения поля ввода имени тега.
        /// </summary>
        public string TagField
        {
            get => _tagField;
            set => SetValue(ref _tagField, value);
        }

        /// <summary>
        /// Выводит документ отчёта.
        /// </summary>
        public Command Print
        {
            get => new Command(() =>
            {
                _report.Print(_request);
            });
        }

        /// <summary>
        /// Добавление тега для поиска.
        /// </summary>
        public Command AddTag
        {
            get => new Command(() =>
            {
                if (!Request.Tags.Contains(TagField))
                    Request.Tags.Add(TagField);
                TagField = "";
            }, () => TagField != "");
        }

        /// <summary>
        /// Удаление тега.
        /// </summary>
        public Command<string> RemoveTag
        {
            get => new Command<string>((t) =>
            {
                Request.Tags.Remove(t);
            }, (t) => t != "");
        }

        /// <summary>
        /// Список категорий.
        /// </summary>
        public ObservableCollection<Category> Categories => DataBaseSinglFacade.Categories;
        /// <summary>
        /// Список заголков товара.
        /// </summary>
        public ObservableCollection<string> ProductTitles => DataBaseSinglFacade.ProductTitles;
        /// <summary>
        /// Список тегов.
        /// </summary>
        public ObservableCollection<string> Tags => DataBaseSinglFacade.Tags;
    }
}
