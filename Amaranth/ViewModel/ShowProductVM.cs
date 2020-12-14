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
        /// Конструктор посредника для формы вывода информации о товаре.
        /// </summary>
        /// <param name="product">Товар, для которого выводится информация.</param>
        public ShowProductVM(Product product)
        {
            Product = DataBaseSinglFacade.LoadInfo(product);
            ListTags = new ObservableCollection<string>(DataBaseSinglFacade.LoadTags(Product.Id));
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
