using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Set<T> : IEnumerable<T>
{
    private List<T> _uni;
    private int _uniCount = 0;
    private bool[] _set;

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
    
    public void Invert()
    {
        _set = _set.Select(p => !p).ToArray();
        Count = _uniCount - Count;
    }

    #endregion

    #region Interops

    public Set<T> GetEmptySubset()
    {
        return new Set<T>(_uni);
    }
    
    public void UnionWith(Set<T> set)
    {
        _set = _set
            .Zip(set._set, (s1, s2) => (s1, s2))
            .Select(p => p.s1 || p.s2)
            .ToArray();
        Count = _set.Count(p => p);
    }
    
    public void IntersectWith(Set<T> set)
    {
        _set = _set
            .Zip(set._set, (s1, s2) => (s1, s2))
            .Select(p => p.s1 && p.s2)
            .ToArray();
        Count = _set.Count(p => p);
    }
    
    public void IsSubsetOf(Set<T> set)
    {
        _set = _set
            .Zip(set._set, (s1, s2) => (s1, s2))
            .Select(p => p.s1 && p.s2)
            .ToArray();
        Count = _set.Count(p => p);
    }

    #endregion
}