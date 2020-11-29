using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;

namespace Amaranth.Model.Data
{
	public class Product : INotifyPropertyChanged
	{
		int _id, _count;
		string _title;
		double _price;
		bool _prescription;
		BitmapImage _image;
		protected Category _category;

		public Product(Category category)
		{
			_id = -1;
			_category = category;
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

		public BitmapImage Image
		{
			get => _image;
			set { _image = value; OnValueChanged(); }
		}

		public double Price
		{
			get => _price;
			set { _price = value; OnValueChanged(); }
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
