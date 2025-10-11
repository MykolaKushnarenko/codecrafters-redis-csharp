namespace DotRedis.BuildingBlocks.Core;

internal class RedisSortedSet
{
    private Dictionary<string, double> _dictionary = new Dictionary<string, double>();
    private SortedSet<SortedSetItem> _items = new SortedSet<SortedSetItem>(new SortedSetItemComparer());

    public bool Contains(string key)
    {
        return _dictionary.ContainsKey(key);
    }
    
    private class SortedSetItem
    {
        public string Key { get; set; }
        public double Weight { get; set; }
    }

    private class SortedSetItemComparer : IComparer<SortedSetItem>
    {
        public int Compare(SortedSetItem x, SortedSetItem y)
        {
            var weightCompareResult = x!.Weight.CompareTo(y!.Weight);
            if (weightCompareResult != 0)
            {
                return weightCompareResult;
            }
            
            return string.Compare(x.Key, y.Key, StringComparison.Ordinal);
        }
    }

    public double this[string scoreKey]
    {
        get => _dictionary[scoreKey];
        set
        {
            _dictionary[scoreKey] = value;
            _items.Add(
                new SortedSetItem
                {
                    Key = scoreKey,
                    Weight = value
                });
        }
    }

    public int Rank(string scoreKey)
    {
        _dictionary.TryGetValue(scoreKey, out var weight);
        var element = new SortedSetItem
        {
            Key = scoreKey,
            Weight = weight
        };
        
        var min = _items.Min;
        
        return _items.GetViewBetween(min, element).Count - 1;
    }
}

