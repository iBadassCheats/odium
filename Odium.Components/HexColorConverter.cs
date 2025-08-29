using System;
using UnityEngine;

namespace Odium.Components;

public static class HexColorConverter
{
	public static Color HexToColor(string hex)
	{
		if (string.IsNullOrEmpty(hex))
		{
			throw new ArgumentException("Hex string cannot be null or empty");
		}
		hex = hex.TrimStart('#');
		if (!IsValidHex(hex))
		{
			throw new ArgumentException("Invalid hex color: #" + hex);
		}
		switch (hex.Length)
		{
		case 3:
			hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
			break;
		case 4:
			hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}{hex[3]}{hex[3]}";
			break;
		case 6:
			hex += "FF";
			break;
		default:
			throw new ArgumentException("Invalid hex color length: #" + hex);
		case 8:
			break;
		}
		byte b = Convert.ToByte(hex.Substring(0, 2), 16);
		byte b2 = Convert.ToByte(hex.Substring(2, 2), 16);
		byte b3 = Convert.ToByte(hex.Substring(4, 2), 16);
		byte b4 = Convert.ToByte(hex.Substring(6, 2), 16);
		return new Color((float)(int)b / 255f, (float)(int)b2 / 255f, (float)(int)b3 / 255f, (float)(int)b4 / 255f);
	}

	public static Color32 HexToColor32(string hex)
	{
		Color color = HexToColor(hex);
		return new Color32((byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), (byte)(color.a * 255f));
	}

	public static string ColorToHex(Color color, bool includeAlpha = true)
	{
		Color32 color2 = color;
		if (includeAlpha)
		{
			return $"#{color2.r:X2}{color2.g:X2}{color2.b:X2}{color2.a:X2}";
		}
		return $"#{color2.r:X2}{color2.g:X2}{color2.b:X2}";
	}

	public static string Color32ToHex(Color32 color, bool includeAlpha = true)
	{
		if (includeAlpha)
		{
			return $"#{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}";
		}
		return $"#{color.r:X2}{color.g:X2}{color.b:X2}";
	}

	public static bool TryHexToColor(string hex, out Color color)
	{
		color = Color.white;
		try
		{
			color = HexToColor(hex);
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static bool IsValidHex(string hex)
	{
		if (string.IsNullOrEmpty(hex))
		{
			return false;
		}
		if (hex.Length != 3 && hex.Length != 4 && hex.Length != 6 && hex.Length != 8)
		{
			return false;
		}
		foreach (char c in hex)
		{
			if ((c < '0' || c > '9') && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
			{
				return false;
			}
		}
		return true;
	}
}
