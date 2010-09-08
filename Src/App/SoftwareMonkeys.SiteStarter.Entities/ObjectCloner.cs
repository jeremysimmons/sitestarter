using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	public static class ObjectCloner
	{
		
		
		/// <summary>
		/// Perform a deep Copy of the object.
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static T Clone<T>(T source)
		{
			if (!typeof(T).IsSerializable)
			{
				throw new ArgumentException("The type must be serializable.", "source");
			}

			// Don't serialize a null object, simply return the default for that object
			if (Object.ReferenceEquals(source, null))
			{
				return default(T);
			}

			IFormatter formatter = new BinaryFormatter();
			Stream stream = new MemoryStream();
			using (stream)
			{
				formatter.Serialize(stream, source);
				stream.Seek(0, SeekOrigin.Begin);
				return (T)formatter.Deserialize(stream);
			}
		}
		
		/// <summary>
		/// Perform a deep Copy of the object.
		/// </summary>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static IEntity Clone(IEntity source)
		{
			if (source.GetType().IsSerializable)
			{
				throw new ArgumentException("The type must be serializable.", "source");
			}

			// Don't serialize a null object, simply return the default for that object
			if (Object.ReferenceEquals(source, null))
			{
				return null;
			}

			IFormatter formatter = new BinaryFormatter();
			Stream stream = new MemoryStream();
			using (stream)
			{
				formatter.Serialize(stream, source);
				stream.Seek(0, SeekOrigin.Begin);
				return (IEntity)formatter.Deserialize(stream);
			}
		}
	}
}
