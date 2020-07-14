using System.Collections.Generic;

namespace PPnetwork
{
	/// <summary>
	/// Simple implementation of <see cref="ISimpleDictionary{Key, Value}"/>
	/// that forwards it's methods to the underlying <see cref="IDictionary{TKey, TValue}"/>
	/// it gets in the constructor
	/// </summary>
	class SimpleDictionary<Key, Value> : ISimpleDictionary<Key, Value>
	where Value : class
	{
		readonly IDictionary<Key, Value> Dictionary;

		public SimpleDictionary(IDictionary<Key, Value> dictionary)
			=> Dictionary = dictionary;
		public void Add(Key key, Value value)
			=> Dictionary.Add(key, value);

		public Value? GetValue(Key key)
		{
			Dictionary.TryGetValue(key, out var value);
			return value;
		}
	}

	/// <summary>
	/// Version of <see cref="SimpleDictionary{Key, Value}"/>
	/// with default constructed internal <see cref="IDictionary{TKey, TValue}"/>
	/// </summary>
	/// <typeparam name="Dictionary">Specific <see cref="IDictionary{TKey, TValue}"/> type.</typeparam>
	class SimpleDictionary<Dictionary, Key, Value> : SimpleDictionary<Key, Value>
		where Value : class
		where Dictionary : IDictionary<Key, Value>, new()
	{
		/// <summary>
		/// Constructs the dictionary with default constructed instance of <typeparamref name="Dictionary"/>.
		/// </summary>
		public SimpleDictionary()
			: base(new Dictionary())
		{ }
	}
}
