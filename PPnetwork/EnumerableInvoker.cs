using System;
using System.Collections.Generic;

namespace PPnetwork
{
	/// <summary>
	/// Helper for Parsers. Can take an IEnumerable of Invokers
	/// and act as an Invoker that invokes until successful.
	/// </summary>
	class EnumerableInvoker<Caller, Parameter> : IInvoker<Caller, Parameter>
	{
		readonly IEnumerable<IInvoker<Caller, Parameter>> Invokers;

		public EnumerableInvoker(IEnumerable<IInvoker<Caller, Parameter>> invokers)
		{
			Invokers = invokers;
		}

		/// <summary>
		/// Invokes the elements in order, until an invocation doesn't throw.
		/// If all throw, the last exception propagates out.
		/// </summary>
		public void Invoke(Caller caller, Parameter parameter)
		{
			Exception? last_exception = new Exception();

			foreach (var invoker in Invokers)
			{
				try
				{
					invoker.Invoke(caller, parameter);
					last_exception = null;
					break;
				}
				catch (Exception e)
				{
					last_exception = e;
				}
			}

			if (last_exception != null)
				throw last_exception;
		}
	}
}
