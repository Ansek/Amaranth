using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы поступления товара.
    /// </summary>
    class ArrivalsVM : BindableBase
    {
        /// <summary>
        /// Для доступа к функциям БД.
        /// </summary>
        readonly DataBaseSinglFacade _db;

        /// <summary>
        /// Конструктор посредника для формы поступления товара.
        /// </summary>
        public ArrivalsVM()
        {
            _db = DataBaseSinglFacade.GetInstance(); // Получение экземпляра Singleton
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

        int _editableCount;
        /// <summary>
        /// Редактируемое значение количества.
        /// </summary>
        public int EditableCount
        {
            get => _editableCount;
            set => SetValue(ref _editableCount, value);
        }

        /// <summary>
        /// Установка значения выбранного товара.
        /// </summary>
        public Command<Product> SetProduct
        {
            get => new Command<Product>((p) =>
            {
                EditableCount = p.ProductCount;
                Product = _db.LoadInfo(p);
            }, (p) => p != null);
        }

        /// <summary>
        /// Установка значения редактируемого количества.
        /// </summary>
        public Command SetCount
        {
            get => new Command(() =>
            {
                _product.ProductCount = _editableCount;
                _product.SaveOnlyCount = true;
                _db.Update(_product);
            }, () => _editableCount > 0);
        }
    }
}
