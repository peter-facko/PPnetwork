using System.Runtime.Serialization;
using System.IO;

namespace PPnetwork
{
	/// <summary>
	/// <see cref="IReaderWriter{}"/> that serializes and deserializes <typeparamref name="T"/> with <typeparamref name="Formatter"/> into a <see cref="Stream"/>.
	/// </summary>
	/// <typeparam name="Formatter"><see cref="IFormatter"/> to serialize with.</typeparam>
	/// <typeparam name="T">Type to serialize.</typeparam>
	public class FormatterStream<Formatter, T> : IReaderWriter<T>
		where Formatter : IFormatter, new()
		where T : class
	{
		readonly Stream stream;
		readonly IFormatter formatter;

		public FormatterStream(Stream stream)
		{
			this.stream = stream;
			formatter = new Formatter();
		}

		public void Write(T t)
		{
			formatter.Serialize(stream, t);
		}
		public T Read()
		{
			return (T)formatter.Deserialize(stream);
		}
	}
}
