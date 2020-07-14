using System;
using System.Reflection;

namespace PPnetwork
{
	/// <summary>
	/// Base for Descriptors
	/// </summary>
	/// <typeparam name="OrderHandlerBase">Order Handler base type, e.g. IConnection</typeparam>
	/// <typeparam name="OrderBase">Order base type, e.g. IPacket</typeparam>
	/// <typeparam name="OrderHandler">Specific Order Handler, e.g. MyConnection</typeparam>
	class Descriptor<OrderHandlerBase, OrderBase, OrderHandler> : IInvoker<OrderHandlerBase, OrderBase>
		where OrderHandler : OrderHandlerBase
	{
		readonly MethodInfo orderHandlerMethod;
		/// <summary>
		/// Creates the Descriptor from the generic type of the OHI
		/// and the specific type of the Order.
		/// </summary>
		public Descriptor(Type genericOHIType, Type orderType)
		{
			orderHandlerMethod = typeof(OrderHandler)
								 .GetInterfaceMap(genericOHIType.MakeGenericType(orderType))
								 .TargetMethods[0];
		}

		/// <summary>
		/// Invokes the handler.
		/// </summary>
		public void Invoke(OrderHandlerBase orderHandler, OrderBase order)
		{
			// Invoke throws exceptions packed in a TargetInvocationException
			try
			{
				// TODO: cache these one object arrays
				orderHandlerMethod.Invoke(orderHandler, new object[] { order! });
			}
			catch (TargetInvocationException e)
			{
				throw e.GetBaseException();
			}
		}
	}
}
