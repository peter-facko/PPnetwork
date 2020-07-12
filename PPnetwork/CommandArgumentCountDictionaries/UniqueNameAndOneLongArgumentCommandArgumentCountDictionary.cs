using System;

namespace PPnetwork
{
	class UniqueNameAndOneLongArgumentCommandArgumentCountDictionary : UniqueNameCommandArgumentCountDictionary
	{
		public UniqueNameAndOneLongArgumentCommandArgumentCountDictionary(ICommandDescriptor descriptor)
			: base(descriptor)
		{ }

		public override ISimpleCollection<ICommandDescriptor>? GetIfOneLongArgument
			=> DescriptorCollection;

		public override ISimpleCollection<ICommandDescriptor>? GetValue(int _)
			=> throw new Exception();
	}
}
