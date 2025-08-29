using System.IO;
using System.Reflection;
using UnhollowerBaseLib;
using UnityEngine;

namespace Odium.AwooochysResourceManagement;

public static class ReModResourceManager
{
	public static Sprite LoadSpriteFromDisk(this string path, int width = 512, int height = 512)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		byte[] array = File.ReadAllBytes(path);
		if (array == null || array.Length == 0)
		{
			return null;
		}
		Texture2D texture2D = new Texture2D(width, height);
		if (!ImageConversion.LoadImage(texture2D, array))
		{
			return null;
		}
		Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), 100000f, 1000u, SpriteMeshType.FullRect, Vector4.zero, generateFallbackPhysicsShape: false);
		sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
		return sprite;
	}

	public static Sprite LoadSpriteFromByteArray(this byte[] bytes, int width = 512, int height = 512)
	{
		if (bytes == null || bytes.Length == 0)
		{
			return null;
		}
		Texture2D texture2D = new Texture2D(width, height);
		if (!ImageConversion.LoadImage(texture2D, bytes))
		{
			return null;
		}
		Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), 100000f, 1000u, SpriteMeshType.FullRect, Vector4.zero, generateFallbackPhysicsShape: false);
		sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
		return sprite;
	}

	public static Sprite LoadSpriteFromByteArray(this Il2CppStructArray<byte> bytes, int width = 512, int height = 512)
	{
		if (bytes == null || bytes.Length == 0)
		{
			return null;
		}
		Texture2D texture2D = new Texture2D(width, height);
		if (!ImageConversion.LoadImage(texture2D, bytes))
		{
			return null;
		}
		Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), 100000f, 1000u, SpriteMeshType.FullRect, Vector4.zero, generateFallbackPhysicsShape: false);
		sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
		return sprite;
	}

	public static Sprite LoadSpriteFromBundledResource(this string path, int width = 512, int height = 512)
	{
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
		MemoryStream memoryStream = new MemoryStream((int)manifestResourceStream.Length);
		manifestResourceStream.CopyTo(memoryStream);
		Texture2D texture2D = new Texture2D(width, height);
		if (!ImageConversion.LoadImage(texture2D, memoryStream.ToArray()))
		{
			return null;
		}
		Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), 100000f, 1000u, SpriteMeshType.FullRect, Vector4.zero, generateFallbackPhysicsShape: false);
		sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
		return sprite;
	}
}
