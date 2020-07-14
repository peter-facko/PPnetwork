using System;
using System.Collections.Generic;
using System.Linq;

namespace PPnetwork
{
	abstract class Sniffer<Key, Value, OrderHandler> : SimpleDictionary<Key, Value>
		where Key : notnull
		where Value : class
	{
		static IEnumerable<Type> GetImplementedInterfaces<Implementation>(Type genericOHIType)
			=> typeof(Implementation).GetInterfaces()
			.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericOHIType)
			.Select(x => x.GetGenericArguments()[0]);

		public Sniffer(IDictionary<Key, Value> map, Type genericInterfaceDefinitionType)
			: base(map)
		{
			foreach (var t in GetImplementedInterfaces<OrderHandler>(genericInterfaceDefinitionType))
				Handle(t);
		}

		protected abstract void Handle(Type orderInterface);
	}
}
