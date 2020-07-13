# PPnetwork
.NET networking library for creating applications with network connections between them.

A school software project.

## Basic info

This library is a basic framework for applications which accept commands from console
and have network connections between themselves.

It uses reflection to make adding new [Commands](#Command) and [Packets](#Packet) as simple as possible.

## Main idea structure

Many terms used in this part of the documentation as a name of a concept specific to the project
have also a regular meaning,
so these specific uses will be denoted with a capital letter.

### Application

One executable should correspond to one Application.

It has two main characteristics:

1) handles [Commands](#Command) from console
2) is connected to any number of other Applications through [Connections](#Connection)

### Command

A command to the [Application](#Application) entered through console, which has:

1) name
2) any number of arguments
3) a way in which arguments are parsed from text
4) priority

Each two Commands must differ in at least one of: name, argument count, priority.

For more information see [Command Parsing](#Command-Parsing).

### Connection

Represents a communication channel between two [Applications](#Application).

Connection can send and receive a [Packet](#Packet).

Each Connection handles incoming Packets.

This library uses TCP to handle networking.

### Packet

A chunk of data representing a single message sent through a [Connection](#Connection).

## Usage

### Application

A bare bone implementation of an [Application](#Application) with no connections might look like this:

```csharp
using PPnetwork;
using System.Collections.Generic;
using System.Linq;

class MyApplication : Application<MyApplication>
{
	protected override IEnumerable<IConnection> Connections => Enumerable.Empty<IConnection>();

	protected override string ExitMessage => "Exit command was entered";

	public override void Handle(NotFoundCommandArgument argument)
	{
		Write($"{argument.Input} was entered, but this is not recognised as a command by the parser");
	}

	public override void RemoveConnection(IConnection connection)
	{ }

	protected override void ClearConnections()
	{ }
}
```

#### Adding a command

In this example we'll add a command which adds two integers and prints the result.

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
There you need to specify a name for the command and optimization flags or a priority.
For more information about these flags and priority see [this](#Command) part of the guide
or the comments in the code.

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
