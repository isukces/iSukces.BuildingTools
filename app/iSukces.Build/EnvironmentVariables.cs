using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace iSukces.Build;

public sealed class EnvironmentVariables : IEnumerable<KeyValuePair<string, string>>
{
    public EnvironmentVariables()
        : this(StringComparer.OrdinalIgnoreCase)
    {
    }

    public EnvironmentVariables(IEqualityComparer<string> keyComparer)
    {
        _values = new Dictionary<string, string>(keyComparer);
    }

    public int Count => _values.Count;

    public ICollection<string> Keys => _values.Keys;

    public ICollection<string> Values => _values.Values;

    public string this[string key]
    {
        get => _values[key];
        set => _values[key] = value;
    }

    /*public void Add(string key, string value)
    {
        _values.Add(key, value);
    }

    public void Add(string key, int value)
    {
        Add(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public void Add(string key, double value)
    {
        Add(key, value.ToString(CultureInfo.InvariantCulture));
    }*/

    public bool ContainsKey(string key)
    {
        return _values.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    public bool Remove(string key)
    {
        return _values.Remove(key);
    }

    public EnvironmentVariables Set(string key, string value)
    {
        _values[key] = value;
        return this;
    }

    public EnvironmentVariables Set(string key, int value)
    {
        return Set(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public EnvironmentVariables Set(string key, double value)
    {
        return Set(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>(_values, _values.Comparer);
    }

    public bool TryGetValue(string key, out string value)
    {
        return _values.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private readonly Dictionary<string, string> _values;
}
