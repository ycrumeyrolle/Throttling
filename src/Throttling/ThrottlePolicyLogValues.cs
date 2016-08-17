using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Throttling
{
    public class ThrottlePolicyLogValues : IReadOnlyList<KeyValuePair<string, object>>
    {
        public ThrottlePolicyLogValues(ThrottlePolicy inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            Name = inner.Name;
        }

        string Name { get; set; }

        string Category { get; set; }

        public int Count
        {
            get
            {
                return 2;
            }
        }

        public KeyValuePair<string, object> this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return new KeyValuePair<string, object>("Name", Name);
                }
                else if (index == 1)
                {
                    return new KeyValuePair<string, object>("Category", Category);
                }

                throw new IndexOutOfRangeException(nameof(index));
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}