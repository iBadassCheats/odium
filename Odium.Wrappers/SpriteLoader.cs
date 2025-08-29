using System;
using System.IO;
using MelonLoader;
using UnityEngine;

namespace Odium.Wrappers;

internal static class SpriteLoader
{
	public static Sprite LoadFromDisk(string path, float pixelsPerUnit = 100f)
	{
		if (string.IsNullOrEmpty(path))
		{
			MelonLogger.Warning("Cannot load sprite: Path is null or empty");
			return null;
		}
		try
		{
			byte[] array = File.ReadAllBytes(path);
			if (array == null || array.Length == 0)
			{
				MelonLogger.Warning("Cannot load sprite: No data found at path '" + path + "'");
				return null;
			}
			Texture2D texture2D = new Texture2D(2, 2);
			if (!ImageConversion.LoadImage(texture2D, array))
			{
				MelonLogger.Error("Failed to convert image data to texture from path '" + path + "'");
				return null;
			}
			Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
			Vector2 pivot = new Vector2(0.5f, 0.5f);
			Sprite sprite = Sprite.Create(texture2D, rect, pivot, pixelsPerUnit, 0u, SpriteMeshType.FullRect, Vector4.zero, generateFallbackPhysicsShape: false);
			sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
			texture2D.hideFlags |= HideFlags.DontUnloadUnusedAsset;
			return sprite;
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error loading sprite from '" + path + "': " + ex.Message, ex);
			return null;
		}
	}
}
