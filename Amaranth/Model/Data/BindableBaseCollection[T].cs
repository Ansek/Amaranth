using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Amaranth.Model.Data
{
    /// <summary>
    /// Абстрактный класс для контейнеров, связывающихся с формой.
    /// </summary>
    public abstract class BindableBaseCollection<T> : BindableBase, INotifyCollectionChanged, IEnumerable<T>
    {
        /// <summary>
        /// Cписок для хранения элементов.
        /// </summary>
        protected List<T> _list;

        public BindableBaseCollection()
        {
            _list = new List<T>(); 
        }

        /// <summary>
        /// Возвращает количество элементов.
        /// </summary>
        public int Count => _list.Count;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>
        /// Вызов оповещения об изменении в коллекции.
        /// </summary>
        public void OnCollectionChanged()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Возвращает перечислитель, выполняющий перебор элементов по коллекции.
        /// </summary>
        /// <returns>Перечислитель, который можно использовать для итерации по коллекции.</returns>
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        /// Возвращает перечислитель, выполняющий перебор элементов по коллекции.
        /// </summary>
        /// <returns>Перечислитель, который можно использовать для итерации по коллекции.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
