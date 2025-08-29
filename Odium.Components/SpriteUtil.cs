using System;
using System.IO;
using MelonLoader;
using Odium.Odium;
using UnityEngine;
using UnityEngine.UI;

namespace Odium.Components;

public class SpriteUtil
{
	public static Texture2D CreateTextureFromBase64(string base64)
	{
		byte[] array = Convert.FromBase64String(base64);
		Texture2D texture2D = new Texture2D(2, 2);
		ImageConversion.LoadImage(texture2D, array);
		return texture2D;
	}

	public static Sprite Create(Texture2D texture, Rect rect, Vector2 size)
	{
		return Sprite.Create(texture, rect, size);
	}

	public static void ApplySpriteToButton(string objectName, string image)
	{
		try
		{
			Sprite overrideSprite;
			if (FileHelper.IsPath(image))
			{
				overrideSprite = LoadFromDisk(image);
			}
			else
			{
				image = Path.Combine(Environment.CurrentDirectory, "Odium", image);
				overrideSprite = LoadFromDisk(image);
			}
			AssignedVariables.userInterface.transform.Find(objectName).transform.Find("Background").GetComponent<Image>().overrideSprite = overrideSprite;
			OdiumConsole.LogGradient("Odium", "Sprite applied to " + objectName + " successfully!");
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex);
		}
	}

	public static void ApplySpriteToMenu(string objectName, string image)
	{
		try
		{
			Sprite overrideSprite;
			if (FileHelper.IsPath(image))
			{
				overrideSprite = LoadFromDisk(image);
			}
			else
			{
				image = Path.Combine(Environment.CurrentDirectory, "Odium", image);
				overrideSprite = LoadFromDisk(image);
			}
			AssignedVariables.userInterface.transform.Find(objectName).GetComponent<Image>().overrideSprite = overrideSprite;
			OdiumConsole.LogGradient("Odium", "Sprite applied to " + objectName + " successfully!");
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex);
		}
	}

	public static void ApplyColorToMenu(string objectName, Color color)
	{
		try
		{
			AssignedVariables.userInterface.transform.Find(objectName).GetComponent<Image>().m_Color = color;
			OdiumConsole.LogGradient("Odium", "Color applied to " + objectName + " successfully!");
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex);
		}
	}

	public static void ApplyColorToButton(string objectName, Color color)
	{
		try
		{
			AssignedVariables.userInterface.transform.Find(objectName).GetComponent<Image>().m_Color = color;
			OdiumConsole.LogGradient("Odium", "Color applied to " + objectName + " successfully!");
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex);
		}
	}

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
