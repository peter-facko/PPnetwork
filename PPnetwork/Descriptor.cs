using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Threading;

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
		where OrderBase : notnull
	{
		readonly ThreadLocal<object[]> objectArray = new ThreadLocal<object[]>(() => new object[1]);

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
				var arr = objectArray.Value!;
				arr[0] = order;
				orderHandlerMethod.Invoke(orderHandler, arr);
			}
			catch (TargetInvocationException e)
			{
				throw e.GetBaseException();
			}
		}
	}
}
