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
        readonly Report _report;

        /// <summary>
        /// Конструктор посредника для формы генерации отчётов.
        /// </summary>
        public ReportsVM()
        {
            _report = new Report(new MySqlAdapter());
            _request = new ProductRequest   // Установка флагов по умолчанию
            {
                CheckTitle = true,          // Проверка по наименованию
                CheckDateComplited = true   // Проверка по дате завершения
            };
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
        public Command<Tag> AddTag
        {
            get => new Command<Tag>((tag) =>
            {
                if (!Request.Tags.Contains(tag)) // Добавление только уникальных тегов
                    Request.Tags.Add(tag);
                TagField = "";
            }, (tag) => tag != null);
        }

        /// <summary>
        /// Удаление тега.
        /// </summary>
        public Command<Tag> RemoveTag
        {
            get => new Command<Tag>((tag) =>
            {
                Request.Tags.Remove(tag);
            }, (tag) => tag != null);
        }
    }
}
