namespace PPnetwork
{
	/// <summary>
	/// <see cref="ISimpleReadonlyDictionary{Key, Value}"/> which you can add key-value pairs to.
	/// </summary>
	interface ISimpleDictionary<Key, Value> : ISimpleReadonlyDictionary<Key, Value>
		where Value : class
	{
		void Add(Key key, Value value);
	}
}
