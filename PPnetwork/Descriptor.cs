using System;
using System.Reflection;

namespace PPnetwork
{
	class Descriptor<OrderHandlerBase, OrderBase, OrderHandler> : IInvoker<OrderHandlerBase, OrderBase>
		where OrderHandler : OrderHandlerBase
	{
		readonly MethodInfo MethodInfo;

		public Descriptor(Type genericOHIType, Type orderType)
		{
			MethodInfo = typeof(OrderHandler)
							.GetInterfaceMap(genericOHIType.MakeGenericType(orderType))
							.TargetMethods[0];
		}

		public void Invoke(OrderHandlerBase caller, OrderBase parameter)
		{
			try
			{
				MethodInfo.Invoke(caller, new object[] { parameter! });
			}
			catch (TargetInvocationException e)
			{
				throw e.GetBaseException();
			}
		}
	}
}
