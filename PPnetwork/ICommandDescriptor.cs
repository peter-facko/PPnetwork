using System.Diagnostics.CodeAnalysis;
using System;

namespace PPnetwork
{
	/// <summary>
	/// Interface for Command Descriptor.
	/// Used by the Parser, that needs it to accept object[]? as parameter, not ICommandArgument.
	/// Also implements comparison on Descriptors.
	/// </summary>
	interface ICommandDescriptor : IInvoker<IApplication, object[]?>, IComparable<ICommandDescriptor>
	{
		int ArgumentCount { get; }
		int Priority { get; }

		int IComparable<ICommandDescriptor>.CompareTo([AllowNull] ICommandDescriptor other)
		{
			if (other != null)
			{
				var argumentCountCompare = ArgumentCount.CompareTo(other.ArgumentCount);
				if (argumentCountCompare == 0)
					return -Priority.CompareTo(other.Priority);
				else
					return argumentCountCompare;
			}
			else
				return 1;
		}
	}
}
