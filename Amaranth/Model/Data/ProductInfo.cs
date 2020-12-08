using System.Collections;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
	public class ProductInfo : Product, IEnumerable<Description>
	{
		List<string> _values;

        public ProductInfo(Category category) : base(category)
        {
            _values = new List<string>();
            for (int i = 0; i < category.Count; i++)
                _values.Add(string.Empty);
        }

        public ProductInfo(Product product, List<string> values) : base(product.Category)
        {
            _id = product.Id;
            _title = product.Title;
            _price = product.Price;
            _count = product.Count;
            _prescription = product.Prescription;
            _values = values;
        }

        public IEnumerator<Description> GetEnumerator()
        {
            int i = 0;
            foreach (var desc in _category)
            {
                desc.Index = i;
                desc.Value = _values[i];
                desc.ValueChanged += (s, j) => { _values[j] = s; }; 
                yield return desc;
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
