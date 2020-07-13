# PPnetwork
.NET networking library for creating applications with network connections between them.

A school software project.

## Basic info

This library is a basic framework for applications which accept commands from console
and have network connections between themselves.

It uses reflection to make adding new [Commands](#Command) and [Packets](#Packet) as simple as possible.

## Conceptual model

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

Each Application handles an "exit" Command by default.
This command has no arguments and first closes all Connections
and then the Application.

See [Command Parsing](#Command-Parsing) and [Adding a command](#Adding-a-command)
for more code oriented explanations.

### Connection

Represents a communication channel between two [Applications](#Application).

Connection can send and receive a [Packet](#Packet).

Each Connection handles incoming Packets.

This library uses TCP to handle networking.

### Packet

A chunk of data representing a single message sent through a [Connection](#Connection).

## Code model

### abstract class Application\<SpecificApplication>

This is the base class for any Application.

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

	// closes all Connections by sending an EndPacket
	protected void CloseAllConnections(string reason);

	// the main loop, in which the Application accepts commands
	public void AcceptCommands();



	// handler for after the exit command
	// the default implementation does nothing
	protected virtual void HandleAfterExit();



	// gets the Connections handled by this Application
	protected abstract IEnumerable<IConnection> Connections { get; }

	// clears the references to all Connections
	protected abstract void ClearConnections();

	// clears a reference to the specific Connection
	public abstract void RemoveConnection(IConnection connection);

	// message to print after the exit command
	protected abstract string ExitMessage { get; }

	// handler for unrecognised input
	public abstract void Handle(NotFoundCommandArgument argument);
```

When inheriting this generic class, the type argument must be the derived class.

### abstract class Connection<SpecificApplication, ApplicationConnection>

This is the base class for any Connection.

It has the following synopsis:

```csharp
abstract class Connection<SpecificApplication, ApplicationConnection>
{
	// parent Application
	public SpecificApplication Application { get; }

	// stream which can read and write Packets
	public IReaderWriter<IPacket> Stream { get; }

	// creates a connection from a parent Application and a connected TcpClient
	protected Connection(SpecificApplication application, TcpClient tcpClient);

	// closes the Connection forcibly
	public void Close();



	// handler for when the opposite side of the connection closes the connection abruptly
	public abstract void HandleAbruptConnectionClose();

	// handler for when the opposite side of the connection sent an EndPacket
	public abstract void HandleNormalConnectionClose(string reason);
}
```

## Usage

### Application

#### Basic implementation

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

#### Adding a command

In this example we'll add a command to our Application
which accepts two integers as arguments and prints their sum.

This example doesn't do anything with networking purposefully
to show only things related to commands.

##### Creating the Command argument

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
For more information about these flags and priority see [this](#Command) part of the guide.

It is good practice to declare these Command arguments as readonly,
since they are just packages of values passed to your handler in the Application.

The parser assumes that you can create an instance of your Command argument
with N ReadOnlyMemory\<char>s as arguments,
where N is the number of fields in the argument type.

In the constructor you need to provide a way to parse the ReadOnlyMemory\<char>s
representing the text arguments from the console to your fields.

If you want to signal a parsing error (argument has invalid format)
throw a `ParseException`, or a derived class.

##### Providing a handler in the Application

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

#### Adding a packet

In this example we'll add a simple packet carrying a string message.

##### Creating the packet

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

## Command Parsing

When user of your application enters a line of text into the console,
this line is treated as a sequence of [tokens](#Token) by the parser.

The first of these tokens is treated as the [Command](#Command) name, the rest are command arguments.

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

If there are some Commands with the right name and argument count,
they are invoked in descending order by their priority until
a Command that doesn't cause an exception is found.\
If there is no such Command, the last exception thrown propagates out of the parser.

### Raw Token

A contiguous subsequence of characters from the input containing no spaces not enclosed in quotes.

### Token

A [raw token](#Raw-Token) with removed quotes.

### Formats

Here, '_' will mean a space character ' ' and 'A' will mean *not* a space character.\
Expressions are in paretheses.

#### Input Format

input is:

_*

or 

\_\*(raw_token)_*

or

\_\*(raw_token)_+(input)

#### Raw Token Formats

raw_token is:

A*

or

A*".*"(raw_token)

or

A*".*

### Example

The input ' a ab" "c ' has tokens:\
'a', 'ab c'

## Demo project

[PPchat](https://github.com/Petkr/PPchat) is a demo client-server project showcasing
how to use this library.

## License

[MIT License](LICENSE)
