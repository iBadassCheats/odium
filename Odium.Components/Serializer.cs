using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Il2CppSystem;
using Il2CppSystem.IO;
using Il2CppSystem.Reflection;
using Il2CppSystem.Runtime.Serialization.Formatters.Binary;

namespace Odium.Components;

public static class Serializer
{
	public static byte[] Il2ToByteArray(this Il2CppSystem.Object obj)
	{
		if (obj == null)
		{
			return null;
		}
		try
		{
			Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			Il2CppSystem.IO.MemoryStream memoryStream = new Il2CppSystem.IO.MemoryStream();
			binaryFormatter.Serialize(memoryStream, obj);
			return memoryStream.ToArray();
		}
		catch (System.Exception ex)
		{
			OdiumConsole.LogException(ex);
			return null;
		}
	}

	public static byte[] ManagedToByteArray(this object obj)
	{
		if (obj == null)
		{
			return null;
		}
		try
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
			binaryFormatter.Serialize(memoryStream, obj);
			return memoryStream.ToArray();
		}
		catch (System.Exception ex)
		{
			OdiumConsole.LogException(ex);
			return null;
		}
	}

	public static T FromByteArray<T>(this byte[] data)
	{
		if (data == null)
		{
			return default(T);
		}
		try
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			using System.IO.MemoryStream serializationStream = new System.IO.MemoryStream(data);
			object obj = binaryFormatter.Deserialize(serializationStream);
			return (T)obj;
		}
		catch (System.Exception ex)
		{
			OdiumConsole.LogException(ex);
			return default(T);
		}
	}

	public static T IL2CPPFromByteArray<T>(this byte[] data)
	{
		if (data == null)
		{
			return default(T);
		}
		try
		{
			Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			Il2CppSystem.IO.MemoryStream serializationStream = new Il2CppSystem.IO.MemoryStream(data);
			object obj = binaryFormatter.Deserialize(serializationStream);
			return (T)obj;
		}
		catch (System.Exception ex)
		{
			OdiumConsole.LogException(ex);
			return default(T);
		}
	}

	public static T FromIL2CPPToManaged<T>(this Il2CppSystem.Object obj)
	{
		return obj.Il2ToByteArray().FromByteArray<T>();
	}

	public static T FromManagedToIL2CPP<T>(this object obj)
	{
		return obj.ManagedToByteArray().IL2CPPFromByteArray<T>();
	}

	public static object[] FromIL2CPPArrayToManagedArray(this Il2CppSystem.Object[] obj)
	{
		object[] array = new object[obj.Length];
		for (int i = 0; i < obj.Length; i++)
		{
			if (obj[i].GetIl2CppType().Attributes == Il2CppSystem.Reflection.TypeAttributes.Serializable)
			{
				array[i] = obj[i].FromIL2CPPToManaged<object>();
			}
			else
			{
				array[i] = obj[i];
			}
		}
		return array;
	}

	public static Il2CppSystem.Object[] FromManagedArrayToIL2CPPArray(this object[] obj)
	{
		Il2CppSystem.Object[] array = new Il2CppSystem.Object[obj.Length];
		for (int i = 0; i < obj.Length; i++)
		{
			if (obj[i].GetType().Attributes == System.Reflection.TypeAttributes.Serializable)
			{
				array[i] = obj[i].FromManagedToIL2CPP<Il2CppSystem.Object>();
			}
			else
			{
				array[i] = (Il2CppSystem.Object)obj[i];
			}
		}
		return array;
	}
}
