using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Amaranth.Model.Data
{
    /// <summary>
    /// Абстрактный класс для элементов, связывающихся с формой.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Устанавливает значение с оповещением формы об изменении.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="var">Поле, которому присваивается значение.</param>
        /// <param name="value">Переданное значение.</param>
        /// <param name="name">Имя вызванного свойства.</param>
        protected void SetValue<T>(ref T var, T value, [CallerMemberName] string name = "")
        {
            var = value;
            OnValueChanged(name);
        }

        /// <summary>
        /// Оповещает об изменении значения свойства. 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Вызов оповещения о смене значения свойства.
        /// </summary>
        /// <param name="name">Имя измененного свойства.</param>
        public void OnValueChanged(string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
