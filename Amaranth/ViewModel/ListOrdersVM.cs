using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class ListOrdersVM : INotifyPropertyChanged
    {
        int _countAll;
        bool _onlyActive, _onlyCompleted;

        Order _order;
        public Order Order
        {
            get => _order;
            set { _order = value; OnValueChanged(); }
        }

        List<Order> _listOrders;
        public List<Order> ListOrders
        {
            get => _listOrders;
            set { _listOrders = value; OnValueChanged(); }
        }

        int _currentNumber;
        public int CurrentNumber
        {
            get => _currentNumber;
            set { _currentNumber = value; OnValueChanged(); }
        }

        int _maxNumber;
        public int MaxNumber
        {
            get => _maxNumber;
            set { _maxNumber = value; OnValueChanged(); }
        }

        string _message;
        public string Message
        {
            get => _message;
            set { _message = value; OnValueChanged(); }
        }

        public ListOrdersVM()
        {
            _countAll = 10;
            _currentNumber = 1;
            _maxNumber = 1;
            int pos = _countAll * (_currentNumber - 1);
            ListOrders = DataBaseSinglFacade.GetListOrder(pos, _countAll);
        }
        
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

        public Command<int> SetCountAll
        {
            get => new Command<int>((count) =>
            {
                _countAll = count;
                int pos = count * (_currentNumber - 1);
                ListOrders = DataBaseSinglFacade.GetListOrder(pos, count, _onlyActive, _onlyCompleted);
            });
        }

        public Command GoPrevious
        {
            get => new Command(() =>
            {
                CurrentNumber--;
                int pos = _countAll * (_currentNumber - 1);
                ListOrders = DataBaseSinglFacade.GetListOrder(pos, _countAll, _onlyActive, _onlyCompleted);
            }, () => _currentNumber != 1);
        }

        public Command GoNext
        {
            get => new Command(() =>
            {
                CurrentNumber++;
                int pos = _countAll * (_currentNumber - 1);
                ListOrders = DataBaseSinglFacade.GetListOrder(pos, _countAll, _onlyActive, _onlyCompleted);
            }, () => _currentNumber != _maxNumber);
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
