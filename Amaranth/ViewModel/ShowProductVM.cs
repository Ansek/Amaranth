using System.Collections.ObjectModel;
using Amaranth.Model;
using Amaranth.Model.Data;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы вывода информации о товаре.
    /// </summary>
    class ShowProductVM : BindableBase
    {
        /// <summary>
        /// Для доступа к функциям БД.
        /// </summary>
        readonly DataBaseSinglFacade _db;

        /// <summary>
        /// Конструктор посредника для формы вывода информации о товаре.
        /// </summary>
        /// <param name="product">Товар, для которого выводится информация.</param>
        public ShowProductVM(Product product)
        {
            _db = DataBaseSinglFacade.GetInstance(); // Получение экземпляра Singleton
            Product = _db.LoadInfo(product);
        }

        ProductInfo _product;
        /// <summary>
        /// Выбранный товар.
        /// </summary>
        public ProductInfo Product
        {
            get => _product;
            set => SetValue(ref _product, value);
        }

        /// <summary>
        /// Список тегов.
        /// </summary>
        public ObservableCollection<string> ListTags { get; }
    }
}
