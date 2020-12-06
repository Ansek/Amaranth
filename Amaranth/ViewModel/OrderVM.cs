using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Amaranth.Model;
using Amaranth.Model.Data;
using Amaranth.Service;

namespace Amaranth.ViewModel
{
    class OrderVM : INotifyPropertyChanged
    {
        int _idProduct;

        Order _order;
        public Order Order
        {
            get => _order;
            set { _order = value; OnValueChanged(); }
        }

        int _editableCount;
        public int EditableCount
        {
            get => _editableCount;
            set { _editableCount = value; OnValueChanged(); }
        }

        string _message;
        public string Message
        {
            get => _message;
            set { _message = value; OnValueChanged(); }
        }

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

        public Command Create
        {
            get => new Command(() =>
            {
                Order = new Order();
                Message = "Новый заказ";
            });
        }

        public Command<Product> SetEditableCount
        {
            get => new Command<Product>((p) =>
            {
                _idProduct = p.Id;
                EditableCount = p.Count;
            }, (p) => p != null);
        }

        public Command SetCount
        {
            get => new Command(() =>
            {
                _order.ChangeCount(_idProduct, _editableCount);
            }, () => _editableCount > 0);
        }

        public Command CompleteOrder
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.CompleteOrder(_order);
                Order = null;
            }, () => _order != null && _order.CompletionDate == null);
        }

        public Command СancelOrder
        {
            get => new Command(() =>
            {
                DataBaseSinglFacade.CancelOrder(_order);
                Order = null;
            }, () => _order != null && _order.CompletionDate == null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnValueChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
