using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Set<T> : IEnumerable<T>
{
    private List<T> _uni;
    private int _uniCount = 0;
    private bool[] _set;

    public IEnumerable<int> Futures => _set.Select(p => p ? 1 : 0);

    public int Count { get; private set; }

    public Set(IEnumerable<T> uni)
    {
        this._uni = uni.ToList();
        _uniCount = this._uni.Count;
        _set = new bool[_uniCount];
    }

    private IEnumerable<T> Enumerate() => _uni
        .Zip(_set, (u, s) => (u, s))
        .Where(p => p.s)
        .Select(p => p.u);

    public IEnumerator<T> GetEnumerator() => Enumerate().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #region SelfOps

    public int Include(params T[] items)
    {
        var repeatedCount = 0;
        var count = 0;

        var lastValidSet = _set;
        _set = (bool[]) _set.Clone();

        foreach (var i in items)
        {
            var uIndex = _uni.IndexOf(i);

            if (uIndex == -1)
            {
                _set = lastValidSet;
                throw new ArgumentOutOfRangeException($"Item {i} are out of range of the universum");
            }

            if (_set[uIndex])
                repeatedCount++;

            _set[uIndex] = true;
            count++;
        }

        Count += count - repeatedCount;

        return repeatedCount;
    }

    public void Clear()
    {
        _set = new bool[_uniCount];
        Count = 0;
    }

    public bool Contains(T item) => _set[_uni.IndexOf(item)];

    public int Exclude(params T[] items)
    {
        var repeatedCount = 0;
        var count = 0;

        var lastValidSet = _set;
        _set = (bool[]) _set.Clone();

        foreach (var i in items)
        {
            var uIndex = _uni.IndexOf(i);

            if (uIndex == -1)
            {
                _set = lastValidSet;
                throw new ArgumentOutOfRangeException($"Item {i} are out of range of the universum");
            }

            if (!_set[uIndex])
                repeatedCount++;

            _set[uIndex] = false;
            count++;
        }

        Count -= count - repeatedCount;

        return repeatedCount;
    }

    public Set<T> Inverted
    {
        get
        {
            var ns = GetEmptySubset();
            ns._set = _set.Select(p => !p).ToArray();
            ns.Count = _uniCount - Count;
            return ns;
        }
    }

    public IEnumerable<(T, T)> Relations =>
        Enumerate().SelectMany(f =>
            Enumerate().Select(s => (f, s))
        );
    
    #endregion

    #region Interops

    public Set<T> GetEmptySubset()
    {
        return new Set<T>(_uni);
    }

    public bool IsSubsetOf(Set<T> set) => _set
        .Zip(set._set, (s1, s2) => (s1, s2))
        .All(p => !p.s1 || p.s2) && Count != set.Count;

    #endregion

    #region binops

    public static Set<T> Union(Set<T> set1, Set<T> set2)
    {
        var ns = set1.GetEmptySubset();
        ns._set = set1._set
            .Zip(set2._set, (s1, s2) => (s1, s2))
            .Select(p => p.s1 || p.s2)
            .ToArray();
        ns.Count = ns._set.Count(p => p);
        return ns;
    }

    public static Set<T> Intersection(Set<T> set1, Set<T> set2)
    {
        var ns = set1.GetEmptySubset();
        ns._set = set1._set
            .Zip(set2._set, (s1, s2) => (s1, s2))
            .Select(p => p.s1 && p.s2)
            .ToArray();
        ns.Count = ns._set.Count(p => p);
        return ns;
    }

    #endregion
}