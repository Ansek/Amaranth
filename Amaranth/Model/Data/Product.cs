using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Amaranth.Model.Data
{
	public class Product : INotifyPropertyChanged
	{
		protected int _id, _count, _reserve;
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

		public Product(Product product)
		{
			_id = product._id;
			_count = product._count;
			_reserve = product._reserve;
			_title = product._title;
			_priceText = product._priceText;
			_price = product._price;
			_prescription = product._prescription;
			_category = product._category;
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

		public int Reserve
		{
			get => _reserve;
			set { _reserve = value; OnValueChanged(); }
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
