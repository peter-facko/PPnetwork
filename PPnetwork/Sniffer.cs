using System;
using System.Collections.Generic;
using System.Linq;

namespace PPnetwork
{
	/// <summary>
	/// The base Sniffer class.
	/// Discovers implemented interfaces.
	/// </summary>
	/// <typeparam name="Identifier">Identifier type.</typeparam>
	/// <typeparam name="DescriptorLike">A Descriptor like type. Could be one Descriptor or another dictionary of Descriptors.</typeparam>
	/// <typeparam name="OrderHandler">Order Handler</typeparam>
	abstract class Sniffer<Identifier, DescriptorLike, OrderHandler> : SimpleDictionary<Identifier, DescriptorLike>
		where Identifier : notnull
		where DescriptorLike : class
	{
		static IEnumerable<Type> GetImplementedInterfacesTypeParameters<Implementation>(Type genericOHIType)
			=> typeof(Implementation).GetInterfaces()
			.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericOHIType)
			.Select(x => x.GetGenericArguments()[0]);

		/// <summary>
		/// Creates the Sniffer with <paramref name="map"/> as the initital Descriptor cache and discovers Order Types of the <typeparamref name="OrderHandler"/>.
		/// </summary>
		/// <param name="map"></param>
		/// <param name="genericInterfaceDefinitionType"></param>
		public Sniffer(IDictionary<Identifier, DescriptorLike> map, Type genericInterfaceDefinitionType)
			: base(map)
		{
			foreach (var t in GetImplementedInterfacesTypeParameters<OrderHandler>(genericInterfaceDefinitionType))
				Handle(t);
		}

		/// <summary>
		/// Method to handle a discovered handled Order Type of the <typeparamref name="OrderHandler"/>.
		/// </summary>
		protected abstract void Handle(Type orderType);
	}
}
