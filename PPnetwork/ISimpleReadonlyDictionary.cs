namespace PPnetwork
{
	interface ISimpleReadonlyDictionary<Key, out Value>
		where Value : class
	{
		Value? GetValue(Key key);
	}
}
