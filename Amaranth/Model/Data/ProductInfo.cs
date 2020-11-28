using System.Collections;
using System.Collections.Generic;

namespace Amaranth.Model.Data
{
	public class ProductInfo : Product, IEnumerable<Description>
	{
		List<string> _values;

        public ProductInfo(Category category) : base(category)
        {
        }

        public IEnumerator<Description> GetEnumerator()
        {
            int i = 0;
            foreach (var desc in _category)
            {
                desc.Value = _values[i];
                desc.ValueChanged += (s) => { _values[i] = s; }; 
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
