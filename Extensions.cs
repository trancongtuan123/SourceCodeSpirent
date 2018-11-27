using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RxAgent
{
    public static class Extensions
    {

        public static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue ret;

            dictionary.TryGetValue(key, out ret);
            return ret;
        }
    }
}
