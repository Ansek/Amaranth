using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Amaranth.Model.Data
{
	public class Product : INotifyPropertyChanged
	{
		protected int _id, _count;
		protected string _title, _priceText;
		protected double _price;
		protected bool _prescription;
		protected Category _category;

		public Product(Category category)
		{
			_id = -1;
			_category = category;
			_prescription = false;
		}

		public int Id
		{
			get => _id;
			set { _id = value; OnValueChanged(); }
		}

		public string Title
		{
			get => _title;
			set { _title = value; OnValueChanged(); }
		}

		public double Price
		{
			get => _price;
			set { _price = value; OnValueChanged(); OnValueChanged("PriceText"); }
		}

		public string PriceText
		{
			get => _priceText;
			set { _priceText = value; double.TryParse(value.Replace('.', ','), out _price); OnValueChanged(); OnValueChanged("Price"); }
		}

		public int Count
		{
			get => _count;
			set { _count = value; OnValueChanged(); }
		}

		public bool Prescription
		{
			get => _prescription;
			set { _prescription = value; OnValueChanged(); }
		}

		public Category Category
		{
			get => _category;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnValueChanged([CallerMemberName] string name = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
