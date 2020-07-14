# Implementation Details

Here are explained the implementation details of the library.

You should read this part of the documentation only after
getting familiar with the basic concepts in the [main guide](#README).

## Main Concepts

After reading the main guide, one might notice
a strong similarity between Commands and Packets.

Both have a corresponding type which handles them by implementing
a "handler" interface.

This is because both use the same system of discovering the handled types
and invoking their handlers.

For this purpose the library uses the following concepts:

### Order

An object which orders an action and carries information (parameters) for the action.

Commands and Packets are groups of Orders.

### Order Interface (OI)

An interface common for a group of Orders.

```csharp
// OI for Packets
interface IPacket
{ }

// OI for Commands
interface ICommandArgument
{ }
```

### Order Type (OT)

A specific implementation of an Order Interface.

Carries the information for the Order action, as described in the [Order](#Order) section.

### Order Handler (OH)

A class which can handle an Order.

Connections are OHs for Packets.

Applications are OHs for Commands.

### Invoker

A class that can invoke a method on an object (caller) with certain parameters.

##### Implementation

```csharp
interface IInvoker<in Caller, in Parameter>
{
	void Invoke(Caller caller, Parameter parameter);
}
```

### Order Handler Interface (OHI)

An interface which is used to denote that the Order Handler handles the specified [Order](#Order),
which means that it has a method which accepts the Order type as the only argument.

OHI must be a generic interface with one type parameter which represents the Order Type.

In pseudo-code, OHIs bind Orders to OHs like this:\
`Order_Handler : Order_Handler_Interface<Order>`

##### Implementation

```csharp
interface IPacketHandler<Packet>
	where Packet : IPacket
{
	void Handle(Packet packet);
}

interface ICommandHandler<CommandArgument>
	where CommandArgument : ICommandArgument
{
	void Handle(CommandArgument argument);
}
```

### Descriptor

An Invoker that invokes the implemented handler method on an Order Handler.

These are the objects that are cached by the library at the start of the program.

##### Implementation

```csharp
// OrderHandlerBase - the Order Handler base type
// OrderBase - the Order interface (or any common base class for all Orders of this type)
// OrderHandler - the specific OH
class Descriptor<OrderHandlerBase, OrderBase, OrderHandler> : IInvoker<OrderHandlerBase, OrderBase>
	where OrderHandler : OrderHandlerBase
{
	// creates the Descriptor from the generic type of the OHI
	// and the specific type of the order
	public Descriptor(Type genericOHIType, Type orderType);

	// invokes the handler
	public void Invoke(OrderHandlerBase caller, OrderBase parameter);
}
```

### Sniffing

The proccess in which the library discovers the Order Handler Interfaces
of an Order Handler and by that discovers the handled Orders of the OH.

### Sniffer

The class that does the Sniffing, caches the Descriptors
and serves as a dictionary between identifiers for the Descriptors
and Descriptors themselves.

These identifiers are:
* the name and argument count for a Command
* a `Type` object for a Packet.

Note that the identifier doesn't need to be unique to the Descriptor (i.e. the Order)
and the Sniffer can return a collection of Decriptors with such identifier.\
This is the case for Commands as multiple Commands can have the same name and argument count.

##### Implementation

```csharp
// Key - identifier
// Value - Descriptor (or some collection of them)
// OrderHandler - the specific OH
abstract class Sniffer<Key, Value, OrderHandler> : SimpleDictionary<Key, Value>
	where Key : notnull
	where Value : class
{
	// creates the Sniffer with the initial dictionary and the generic type of the OHI
	public Sniffer(IDictionary<Key, Value> dictionary, Type genericOHIType);

	// the Sniffer "sniffs" the OrderHandler type for implemented genericOHIType interfaces
	// and for each OT it runs this method
	protected abstract void Handle(Type orderType);
}
```

### Parser

In a more specific sense, a class that can parse an input into an Invoker
and parameters for the Invoker.

Note that Parser doesn't provide the caller (in this case the Ordre Handler)
for the Invoker.

Parsers use Sniffers to retrieve a Descriptor based on an identifier.
