using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace FCSPlayout.AppInfrastructure
{
    public class BitmapSourceCache
    {
        public struct CacheKey
        {
            public static CacheKey Create(string file,double pos)
            {
                return new CacheKey { File = file, Position = pos };
            }

            public string File { get; set; }
            public double Position { get; set; }
        }

        public class CacheValue
        {
            public BitmapSource Source { get; set; }
            public long Timestamp { get; set; }
        }

        private Dictionary<CacheKey, CacheValue> _cache;

        public BitmapSourceCache()
        {
            _cache = new Dictionary<CacheKey, CacheValue>(BitmapSourceCacheKeyEqualityComparer.Instance);
        }

        public BitmapSource Get(string file, double begin, double end)
        {
            CacheKey key;
            if(FindCacheKey(file,begin,end,out key))
            {
                var result = _cache[key];
                result.Timestamp = DateTime.Now.Ticks;
                return result.Source;
            }
            return null;
        }

        private bool FindCacheKey(string file,double begin,double end,out CacheKey outKey)
        {
            var keys = _cache.Keys;
            foreach(var key in keys)
            {
                if (string.Equals(key.File, file, StringComparison.OrdinalIgnoreCase) && key.Position >= begin && key.Position <= end)
                {
                    outKey = key;
                    return true;
                }
            }
            outKey = default(CacheKey);
            return false;
        }

        private int _capability = 150;
        private int _cleanCount = 50;

        public void Add(string file, double pos, BitmapSource bmpSource)
        {
            var key = CacheKey.Create(file, pos);
            if (_cache.Count > _capability)
            {
                Clean();
            }

            _cache[key] = new CacheValue { Source=bmpSource,Timestamp=DateTime.Now.Ticks };
        }

        private void Clean()
        {
            var keys = _cache.OrderBy(kvp => kvp.Value.Timestamp).ToList();
            for (int i = 0; i < _cleanCount; i++)
            {
                var key = keys[i].Key;
                _cache.Remove(key);
            }
        }

        class BitmapSourceCacheKeyEqualityComparer : IEqualityComparer<CacheKey>
        {
            public static readonly BitmapSourceCacheKeyEqualityComparer Instance = new BitmapSourceCacheKeyEqualityComparer();

            public bool Equals(CacheKey x, CacheKey y)
            {
                return string.Equals(x.File, y.File, StringComparison.OrdinalIgnoreCase) && x.Position == y.Position;
            }

            public int GetHashCode(CacheKey obj)
            {
                return obj.File.ToUpperInvariant().GetHashCode() ^ obj.Position.GetHashCode();
            }
        }
    }
}
