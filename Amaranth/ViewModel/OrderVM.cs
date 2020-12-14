using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы заказа.
    /// </summary>
    class OrderVM : BindableBase
    {
        /// <summary>
        /// Идентификатор выбранного продукта.
        /// </summary>
        int _idProduct;

        /// <summary>
        /// Конструктор посредника для формы заказа.
        /// </summary>
        public OrderVM()
        {
            _order = new Order();
            Message = "Новый заказ";
        }

        Order _order;
        /// <summary>
        /// Текущий заказ.
        /// </summary>
        public Order Order
        {
            get => _order;
            set => SetValue(ref _order, value);
        }

        int _editableCount;
        /// <summary>
        /// Редактируемое значение количества.
        /// </summary>
        public int EditableCount
        {
            get => _editableCount;
            set { if (value <= _maxCount) SetValue(ref _editableCount, value); }
        }

        int _maxCount;
        /// <summary>
        /// Значение максимально допустимого количества товаров.
        /// </summary>
        public int MaxCount
        {
            get => _maxCount;
            set => SetValue(ref _maxCount, value);
        }

        string _message;
        /// <summary>
        /// Сообщение о состоянии заказа.
        /// </summary>
        public string Message
        {
            get => _message;
            set => SetValue(ref _message, value);
        }

        /// <summary>
        /// Загрузка списка товаров.
        /// </summary>
        public Command<string> LoadOrder
        {
            get => new Command<string>((id) =>
            {
                int i = int.Parse(id);
                Order = DataBaseSinglFacade.GetOrder(i);
                if (_order == null)
                    Message = "Заказ не был найден";
                else if (_order.CompletionDate == null)
                    Message = $"Заказ от {_order.CreationDate:F}";
                else
                    Message = $"Заказ был завершен {_order.CompletionDate:F}";
            }, (id) => { int i; return int.TryParse(id, out i); });
        }

        /// <summary>
        /// Создание нового заказа.
        /// </summary>
        public Command Create
        {
            get => new Command(() =>
            {
                Order = new Order();
                Message = "Новый заказ";
            });
        }

        /// <summary>
        /// Установка значения редактируемого количества.
        /// </summary>
        public Command<Product> SetEditableCount
        {
            get => new Command<Product>((p) =>
            {
                _idProduct = p.Id;
                MaxCount = DataBaseSinglFacade.GetMaxCountProduct(p.Id);
                EditableCount = p.Count;
            }, (p) => p != null);
        }

        /// <summary>
        /// Добавление товара в список заказа.
        /// </summary>
        public Command<Product> SetProduct
        {
            get => new Command<Product>((p) =>
            {
                var product = new Product(p);
                product.Count = 1;
                Order.Add(product);
            }, (p) => p != null);
        }

        /// <summary>
        /// Удаление товара из списка заказа.
        /// </summary>
        public Command<Product> DeleteProduct
        {
            get => new Command<Product>((p) => Order.Delete(p), (p) => p != null);
        }

        /// <summary>
        /// Установка значения редактируемого количества.
        /// </summary>
        public Command SetCount
        {
            get => new Command(() =>
            {
                _order.ChangeCount(_idProduct, _editableCount);
            }, () => _editableCount > 0);
        }

        /// <summary>
        /// Завершает данный заказ.
        /// </summary>
        public Command CompleteOrder
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.CompleteOrder(_order);
                foreach (var p in _order)
                    DataBaseSinglFacade.SubMaxCountProduct(p.Id, p.Count);
                Order = new Order();
                Message = "Новый заказ";
            }, () => _order != null && _order.CompletionDate == null);
        }

        /// <summary>
        /// Производит отмену заказа.
        /// </summary>
        public Command СancelOrder
        {
            get => new Command(() =>
            {
                if (DialogueService.ShowWarning("Вы действительно хотите отменить заказ?"))
                {
                    DataBaseSinglFacade.CancelOrder(_order);
                    Order = new Order();
                    Message = "Новый заказ";
                }
            }, () => _order != null && _order.CompletionDate == null);
        }
    }
}
