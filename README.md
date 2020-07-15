# PPnetwork
.NET networking library for creating applications with network connections between them.

A school software project.

## Basic Info

This library is a basic framework for applications which accept commands from console
and have network connections between themselves.

It uses reflection to make adding new [Commands](#Command) and [Packets](#Packet) as simple as possible.

## Conceptual Model

Many terms used in this part of the documentation as a name of a concept specific to the project
have also a regular meaning,
so these specific uses will be denoted with a capital letter.

### Application

One executable should correspond to one Application.

It has two main characteristics:

1) handles [Commands](#Command) from console
2) is connected to any number of other Applications through [Connections](#Connection)

### Command

Command is a command to the [Application](#Application) entered through the console.

It is uniquely identified by:

1) name
2) any number of arguments
3) priority

No two Commands can share all of these three properties,
because such Commands would be treated as equal by the parser.

Each Command must also have a way in which arguments are parsed from text
and a corresponding handler in the Application,
which specifies the behaviour after the command is entered.

#### Exit Command

Each Application handles an "exit" Command by default.
This command has no arguments and first closes all Connections by sending an [`EndPacket`](#EndPacket)
and then closes the Application.

See [Command Parsing](#Command-Parsing) and [Adding a command](#Adding-a-command)
for more code oriented explanations.

### Connection

Represents a communication channel between two [Applications](#Application).

Connection can send and receive a [Packet](#Packet).

The Application that handles a particular Connection is called its parent Application.

Each Connection handles incoming Packets.

This library uses TCP to handle networking.

### Packet

A chunk of data representing a single message sent through a [Connection](#Connection).

#### EndPacket

There is one default Packet defined in the library, called `EndPacket`.
This Packet signals that the sender wishes to end the [Connection](#Connection).
It accepts "reason" string as an argument.

The `EndPacket` is used internally by the library
and it's handling on receiving cannot be modified.
However, Connections can send an `EndPacket`.
The other Connection will close it's end of the channel after receiving an `EndPacket`,
so the only valid action after sending an `EndPacket` is to close the Connection.

## Code Model

### interface IWriter\<T>

```csharp
interface IWriter<in T>
{
	void Write(T t);
}
```

### interface IReader\<T>

```csharp
interface IReader<out T>
{
	T Read();
}
```

### interface IApplication

This is the interface for all [Applications](#Application).

Not intended for direct implementation, use
[`Application<>`](#abstract-class-applicationspecificapplication)
as the base class for your Application.

```csharp
interface IApplication : IReaderWriter<string>
{
	// clears the reference to the Connection
	void RemoveConnection(IConnection connection);
}
```

### abstract class Application\<SpecificApplication>

This is the base class for any [Application](#Application).

It has the following synopsis:

```csharp
abstract class Application<SpecificApplication> : IApplication
	where SpecificApplication : IApplication
{
	// writes a line to the console
	public void Write(string line);

	// reads a line from the console
	public string Read();

	// writes a line if in debug mode, otherwise does nothing
	public void WriteDebug(string s);

	// the main loop, in which the Application accepts commands
	public void AcceptCommands();

	// closes all Connections by sending an EndPacket
	protected void CloseAllConnections(string reason);



	// handler for after the exit command
	// the default implementation does nothing
	protected virtual void HandleAfterExit();



	// clears a reference to the specific Connection
	public abstract void RemoveConnection(IConnection connection);

	// handler for unrecognised input
	public abstract void Handle(NotFoundCommandArgument argument);

	// gets the Connections handled by this Application
	protected abstract IEnumerable<IConnection> Connections { get; }

	// clears the references to all Connections
	protected abstract void ClearConnections();

	// message to print after the exit command
	protected abstract string ExitMessage { get; }
```

When inheriting this generic class in your Application implementation,
the type argument must be the derived class.

#### Example

```csharp
class MyApplication : Application<MyApplication>
{
	// ...
}
```

### interface IConnection

This is the interface for all [Connections](#Connection).

Don't implement this directly, use the abstract generic class
[`Connection<,>`](#abstract-class-connectionspecificapplication-applicationconnection).

```csharp
interface IConnection
{
	IReaderWriter<IPacket> Stream { get; }

	void Close();
}
```

### abstract class Connection<SpecificApplication, ApplicationConnection>

This is the base class for any [Connection](#Connection).

It has the following synopsis:

```csharp
abstract class Connection<SpecificApplication, ApplicationConnection> : IConnection
	where SpecificApplication : IApplication
	where ApplicationConnection : IConnection
{
	// parent Application
	public SpecificApplication Application { get; }

	// stream which can read and write Packets
	public IReaderWriter<IPacket> Stream { get; }

	// forcibly closes the Connection
	public void Close();

	// creates a connection from a parent Application and a connected TcpClient
	protected Connection(SpecificApplication application, TcpClient tcpClient);



	// handler for when the opposite side of the connection closes the connection abruptly
	public abstract void HandleAbruptConnectionClose();

	// handler for when the opposite side of the connection sent an EndPacket
	public abstract void HandleNormalConnectionClose(string reason);
}
```

When deriving this class, the generic type `SpecificApplication`
should be the type of the parent [Application](#Application) of your Connection
and `ApplicationConnection` should be the type of your Connection.

#### Example

```csharp
class MyApplication : Application<MyApplication>
{
	// ...
}

class MyConnection : Connection<MyApplication, MyConnection>
{
	// ...
}
```

### interface IPacket

This is the interface all [Packets](#Packet) should implement.

```csharp
interface IPacket
{}
```

## Usage

### Application

#### Basic Implementation

A bare bone implementation of an [Application](#Application) with no connections
might look like this:

```csharp
using System.Collections.Generic;
using System.Linq;
using PPnetwork;

class MyApplication : Application<MyApplication>
{
	protected override IEnumerable<IConnection> Connections => Enumerable.Empty<IConnection>();

	protected override string ExitMessage => "Exit command was entered";

	public override void Handle(NotFoundCommandArgument argument)
	{
		Write($"{argument.Input} was entered, but this is not recognised as a command");
	}

	public override void RemoveConnection(IConnection connection)
	{ }

	protected override void ClearConnections()
	{ }
}
```

#### Running the Application

With your implementation of an Application,
you can create and run your Application in the `Main` function.

```csharp
using PPnetwork;

static void Main(string[] _)
{
	NativeParsing.SetEncoding();

	var app = new MyApplication();
	app.AcceptCommands();
}
```

This library only works with ASCII encoding, so to ensure this,
call `NativeParsing.SetEncoding()` before running your Application.

#### Adding a Command

In this example we'll add a [Command](#Command) to our Application
which accepts two integers as arguments and prints their sum.

This example doesn't do anything with networking purposefully
to show only things related to commands.

##### Creating the Command Argument

First you need to create a type that will hold the arguments.\
This type must inherit `ICommandArgument`. It is an empty interface in this state of the library.

The library will use reflection to infer the number of arguments
from the number of fields your type has.
(unless you set the flag `CommandFlags.OneLongArgument`).

```csharp
[Command("add", CommandFlags.UniqueName)]
public readonly struct AddCommandArgument : ICommandArgument
{
	public readonly int First;
	public readonly int Second;

	public AddCommandArgument(ReadOnlyMemory<char> first, ReadOnlyMemory<char> second)
	{
		if (!int.TryParse(first.Span, out First))
			throw new ParseException($"couldn't parse the first argument as an int");

		if (!int.TryParse(second.Span, out Second))
			throw new ParseException($"couldn't parse the second argument as an int");
	}
}
```

You will also need to add a `CommandAttribute` to your Command argument.\
There you need to specify a name for the command and flags or a priority.
For more information about these flags and priority see the next part of the guide.

It is good practice to declare these Command arguments as readonly,
since they are just packages of values passed to your handler in the Application.

The parser assumes that you can create an instance of your Command argument
with N `ReadOnlyMemory<char>`s as arguments,
where N is the number of fields in the argument type.

In the constructor you need to provide a way to parse the `ReadOnlyMemory<char>`s
representing the text arguments from the console to your fields.

If you want to signal a parsing error (argument has invalid format)
throw a `ParseException`, or its derived class.

##### CommandAttribute

When creating a Command, you must add the attribute `CommandAttribute` to your class.

The first parameter is the name of your Command.

The second can be:
* Command Flags
* Priority

###### Command Flags

Command Flags are mostly optimization only flags, but one flag does change the behavior of the Command.\
Also note that while only being optimizations, the parser relies on them,
so don't set a flag which is not true for the Command
(i.e. unique name when there are two Commands with such name).

The one flag that also changes the behavior is `CommandFlags.OneLongArgument`,
which makes the Command accept one argument
but also changes the parsing of it's arguments. For more information see
[Command Parsing](#Command-Parsing).

Other flags are:
* `CommandFlags.UniqueName`: use if the name of the Command is unique
* `CommandFlags.UniqueArgumentCount`: use if the argument count of this
  Command is unique among Commands with the same name

Note that `UniqueName` implies `UniqueArgumentCount` so you don't need to set both.

There is also a flag `CommandFlags.None` but **don't** use this directly in the constructor,
as you need to set priority in this case.

###### Priority

When no Command Flag applies to your Command `C`, that means that there is another Command `D`
that has the same name and argument count as `C`.

In this case, you need to distinguish these Commands with different priority.

A higher priority means, that the Command will be considered sooner during invocation.\
For more details about the invocation see [this](#Invocation-Order).

##### Providing a Handler to the Application

Secondly you will need to provide a method which accepts your new Command argument in your Application.

To do this, implement an interface `ICommandHandler<YourCommandArgument>` in your Application,
where `YourCommandArgument` is the Command argument you just created..

```csharp
// ...

class MyApplication : Application<MyApplication>,
	ICommandHandler<AddCommandArgument>
{
	// ...

	public void Handle(AddCommandArgument argument)
	{
		Write($"{argument.First} + {argument.Second} = {argument.First + argument.Second}");
	}
}
```

That's it. Your application can now do this:

```
$ add 15 20
15 + 20 = 35
```

### Connection

A bare bone implementation of a [Connection](#Connection) that is maintained by `MyApplication`
might look like this:

```csharp
using PPnetwork;
using System.Net.Sockets;

class MyConnection : Connection<MyApplication, MyConnection>
{
	public MyConnection(MyApplication parent, TcpClient client)
		: base(parent, client)
	{ }

	public override void HandleAbruptConnectionClose()
	{
		Application.Write("the other side closed the connection abruptly");
	}

	public override void HandleNormalConnectionClose(string reason)
	{
		Application.Write($"the other side closed the connection because: {reason}");
	}
}
```

It is assumed that the Connection will be created by it's parent [Application](#Application),
so the connected `TcpClient` is passed in the constructor.

#### Adding a Packet

In this example we'll add a simple packet carrying a string message.

##### Creating a Packet

First, you need to create the type holding the information
you intend to send through the [Connection](#Connection).

```csharp
using System;
using PPnetwork;

[Serializable]
public readonly struct MessagePacket : IPacket
{
	public readonly string Message;

	public MessagePacket(string message)
	{
		Message = message;
	}
}
```

You need to mark it as `Serializable`.

It is also good practice to make it readonly, similarly as with [Commands](#Command).

##### Adding a handler

Secondly, you need to provide a method that handles you new incoming [Packet](#Packet).\
This is handled in the [Connection](#Connection) that receives it.

To do this, you need to implement `IPacketHandler<YourPacket>` in your Connection,
where `YourPacket` is your newly created [Packet](#Packet).

```csharp
// ...
class MyConnection : Connection<MyApplication, MyConnection>,
	IPacketHandler<MessagePacket>
{
	// ...
	public void Handle(MessagePacket packet)
	{
		Application.Write($"this message was received: {packet.Message}");
	}
}
```

##### Sending a Packet

Packets, unlike Commands, are sent by the user of this library.

`IConnection` interface provides an `IReaderWriter<IPacket> Stream`
for reading and writing Packets.

So, in your Connection, you can send a Packet like this:

```csharp
// ...
class MyConnection : Connection<MyApplication, MyConnection>,
	IPacketHandler<MessagePacket>
{
	// ...
	public void SendMessage(string message)
	{
		Stream.Write(new MessagePacket(message));
	}
}
```

### SimpleSerializerStream

The library also offers a class for serialization of some types to Streams.

The SimpleSerializerStream supports reading and writing of these types to a Stream:
* `int`
* `string`
* `IPAddress`

Additionally it supports reading a `Memory<char>` and writing a `ReadOnlyMemory<char>`.

This class is not used inside the library and is intended to ease serializing data
from Application (e.g. a server stores an index of usernames on disk).

## Command Parsing

When user of your application enters a line of text into the console,
this line is treated either as:

1) a sequence of [Tokens](#Token), or
2) the first Token and the "rest" (the remaining substring after the end of the first Token)

An input that contains no Tokens is ingored.

When [Command](#Command) has flag `OneLongArgument`,
the input is treated as 2. The first token is the Command name
and the remaining substring is the Command's only argument.

If the [Command](#Command) doesn't have flag `OneLongArgument` set, 1.
applies: the first of the tokens in the sequence is treated as the Command name,
the rest are command arguments.

The parser will find all commands with the entered name and argument count.

If there is no Command with such name,
`Handle(NotFoundCommandArgument)` is invoked in your [Application](#Application).\
If your Application is implemented like this:

```csharp
// ...
class MyApplication : Application<MyApplication>
{
	// ...
	public override void Handle(NotFoundCommandArgument argument)
	{
		Write($"{argument.Input} wasn't recognised");
	}
}
```

and no command has name `x`, then:

```
$ x example
x example wasn't recognised
```

If there is a Command with such name but none has the entered number of arguments,
an error with the entered argument count is printed.\
For example, if there is one command with name `x` and it has argument count 2:

```
$ x example
bad argument count: 1
```

### Invocation Order

If there are some Commands with the right name and argument count,
they are invoked in descending order by their priority until
a Command that doesn't cause an exception is found.\
If there is no such Command, the last exception thrown propagates out of the parser.

### Definitions

#### Raw Token

A contiguous subsequence of characters from the input containing no spaces not enclosed in quotes.

#### Token

A [raw token](#Raw-Token) with removed quotes.

#### Formats

Here, '_' will mean a space character ' ' and 'A' will mean *not* a space character.\
Expressions are in paretheses.

##### Input Format

input is:

_*

or 

\_\*(raw_token)_*

or

\_\*(raw_token)_+(input)

##### Raw Token Formats

raw_token is:

A*

or

A*".*"(raw_token)

or

A*".*

#### Example

The input ' a ab" "c ' has tokens:\
'a', 'ab c'

## Implementation Details

[here](DETAILS)

## Demo Project

[PPchat](https://github.com/Petkr/PPchat) is a demo client-server project showcasing
how to use this library.

## Potential Improvements

This library was split from the PPchat project after it was finished,
so it may still carry some illogical design decisions stemming from originally
only targetting one client-server application.

## License

[MIT License](LICENSE)
