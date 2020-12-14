using System.Collections.Generic;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    /// <summary>
    /// Класс посредник для формы списка заказов.
    /// </summary>
    class ListOrdersVM : BindableBase
    {
        /// <summary>
        /// Задает максимальное количество отображаемых заказов в таблице.
        /// </summary>
        int _countAll;

        /// <summary>
        /// Флаг, показать активные заказы.
        /// </summary>
        bool _onlyActive;

        /// <summary>
        /// Флаг, показать завершенные заказы.
        /// </summary>
        bool _onlyCompleted;

        /// <summary>
        /// Конструктор посредника для формы списка заказов.
        /// </summary>
        public ListOrdersVM()
        {
            _countAll = 10;
            _currentNumber = 1;
            _maxNumber = 1;
            int pos = _countAll * (_currentNumber - 1);
            ListOrders = DataBaseSinglFacade.GetListOrder(pos, _countAll);
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

        List<Order> _listOrders;
        /// <summary>
        /// Список заказов.
        /// </summary>
        public List<Order> ListOrders
        {
            get => _listOrders;
            set => SetValue(ref _listOrders, value);
        }

        int _currentNumber;
        /// <summary>
        /// Текущий номер страницы.
        /// </summary>
        public int CurrentNumber
        {
            get => _currentNumber;
            set => SetValue(ref _currentNumber, value);
        }

        int _maxNumber;
        /// <summary>
        /// Максимальный номер страницы.
        /// </summary>
        public int MaxNumber
        {
            get => _maxNumber;
            set => SetValue(ref _maxNumber, value);
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
        /// Обновляет список заказов.
        /// </summary>
        public Command<int> UpdateList
        {
            get => new Command<int>((i) =>
            {
                switch (i)
                {
                    case 1:
                        _onlyActive = true;
                        _onlyCompleted = false;
                        break;
                    case 2:
                        _onlyActive = false;
                        _onlyCompleted = true;
                        break;
                    default:
                        _onlyActive = false;
                        _onlyCompleted = false;
                        break;
                }
                int pos = _countAll * (_currentNumber - 1);
                ListOrders = DataBaseSinglFacade.GetListOrder(pos, _countAll, _onlyActive, _onlyCompleted);
            });
        }

        /// <summary>
        /// Устанавливает максимальное количество отображаемых заказов в таблице.
        /// </summary>
        public Command<int> SetCountAll
        {
            get => new Command<int>((count) =>
            {
                _countAll = count;
                int pos = count * (_currentNumber - 1);
                ListOrders = DataBaseSinglFacade.GetListOrder(pos, count, _onlyActive, _onlyCompleted);
            });
        }

        /// <summary>
        /// Переход на станицу назад.
        /// </summary>
        public Command GoPrevious
        {
            get => new Command(() =>
            {
                CurrentNumber--;
                int pos = _countAll * (_currentNumber - 1);
                ListOrders = DataBaseSinglFacade.GetListOrder(pos, _countAll, _onlyActive, _onlyCompleted);
            }, () => _currentNumber != 1);
        }

        /// <summary>
        /// Переход на станицу вперед.
        /// </summary>
        public Command GoNext
        {
            get => new Command(() =>
            {
                CurrentNumber++;
                int pos = _countAll * (_currentNumber - 1);
                ListOrders = DataBaseSinglFacade.GetListOrder(pos, _countAll, _onlyActive, _onlyCompleted);
            }, () => _currentNumber != _maxNumber);
        }

        /// <summary>
        /// Устанавливает рассматриваемый заказ.
        /// </summary>
        public Command<Order> SetOrder
        {
            get => new Command<Order>((o) =>
            {
                Order = DataBaseSinglFacade.GetOrder(o.Id);
                if (_order.CompletionDate == null)
                    Message = $"Заказ от {_order.CreationDate:F}";
                else
                    Message = $"Заказ был завершен {_order.CompletionDate:F}";
            }, (o) => o != null);
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
                    Order = null;
                    int pos = _countAll * (_currentNumber - 1);
                    ListOrders = DataBaseSinglFacade.GetListOrder(pos, _countAll, _onlyActive, _onlyCompleted);
                }
            }, () => _order != null && _order.CompletionDate == null);
        }
    }
}
