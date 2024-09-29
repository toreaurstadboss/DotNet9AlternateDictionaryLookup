using System.Linq.Expressions;

namespace LookupDictionaryOptimized
{
    public class AlternateEqualityComparer<T, TKey> : IEqualityComparer<T>, IAlternateEqualityComparer<TKey, T>
        where T : new()
    {
        private readonly Expression<Func<T, TKey>> _keyAccessor;

        private TKey GetKey(T obj) => _keyAccessor.Compile().Invoke(obj);

        public AlternateEqualityComparer(Expression<Func<T, TKey>> keyAccessor)
        {
            _keyAccessor = keyAccessor;
        }

        public AlternateEqualityComparer<T, TKey> Instance
        {
            get
            {
                return new AlternateEqualityComparer<T, TKey>(_keyAccessor);
            }
        }

        T IAlternateEqualityComparer<TKey, T>.Create(TKey alternate)
        {
            //create a dummy default instance if the requested key is not contained in the dictionary
            return Activator.CreateInstance<T>();
        }

        public bool Equals(T? x, T? y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if ((x == null && y != null) || (x != null && y == null))
            {
                return false;
            }
            TKey xKey = GetKey(x!);
            TKey yKey = GetKey(y!);
            return xKey!.Equals(yKey);
        }

        public int GetHashCode(T obj) => GetKey(obj)?.GetHashCode() ?? default;

        public int GetHashCode(TKey alternate) => alternate?.GetHashCode() ?? default;

        public bool Equals(TKey alternate, T other)
        {
            if (alternate == null && other == null)
            {
                return true;
            }
            if ((alternate == null && other != null) || (alternate != null && other == null))
            {
                return false;
            }
            TKey otherKey = GetKey(other);
            return alternate!.Equals(otherKey);
        }
    }

}
