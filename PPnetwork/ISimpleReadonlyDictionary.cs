namespace PPnetwork
{
	/// <summary>
	/// An interface for a simple dictionary.
	/// </summary>
	interface ISimpleReadonlyDictionary<Key, out Value>
		where Value : class
	{
		Value? GetValue(Key key);
	}
}
