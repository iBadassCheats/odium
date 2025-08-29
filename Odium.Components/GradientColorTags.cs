using System.Text;
using UnityEngine;

namespace Odium.Components;

public class GradientColorTags
{
	public static string GetAnimatedGradientText(string text, Color color1, Color color2, float speed = 1f, float waveLength = 2f)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		float num = Time.time * speed;
		for (int i = 0; i < text.Length; i++)
		{
			float num2 = (float)i / (float)(text.Length - 1);
			float value = num2 + (Mathf.Sin(num + (float)i * waveLength) * 0.5f + 0.5f) * 0.3f;
			value = Mathf.Clamp01(value);
			Color color3 = Color.Lerp(color1, color2, value);
			string arg = ColorUtility.ToHtmlStringRGB(color3);
			stringBuilder.Append($"<color=#{arg}>{text[i]}</color>");
		}
		return stringBuilder.ToString();
	}

	public static string GetWaveGradientText(string text, Color color1, Color color2, float speed = 2f, float frequency = 0.5f)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		float num = Time.time * speed;
		for (int i = 0; i < text.Length; i++)
		{
			float t = Mathf.Sin(num + (float)i * frequency) * 0.5f + 0.5f;
			Color color3 = Color.Lerp(color1, color2, t);
			string arg = ColorUtility.ToHtmlStringRGB(color3);
			stringBuilder.Append($"<color=#{arg}>{text[i]}</color>");
		}
		return stringBuilder.ToString();
	}

	public static string GetPulseGradientText(string text, Color color1, Color color2, float speed = 1.5f)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		float num = Mathf.Sin(Time.time * speed) * 0.5f + 0.5f;
		for (int i = 0; i < text.Length; i++)
		{
			float t = (float)i / (float)(text.Length - 1);
			float t2 = Mathf.Lerp(num * 0.3f, 1f - num * 0.3f, t);
			Color color3 = Color.Lerp(color1, color2, t2);
			string arg = ColorUtility.ToHtmlStringRGB(color3);
			stringBuilder.Append($"<color=#{arg}>{text[i]}</color>");
		}
		return stringBuilder.ToString();
	}

	public static string GetRainbowText(string text, float speed = 1f, float frequency = 0.3f)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		float num = Time.time * speed;
		for (int i = 0; i < text.Length; i++)
		{
			float h = (num + (float)i * frequency) % 1f;
			Color color = Color.HSVToRGB(h, 1f, 1f);
			string arg = ColorUtility.ToHtmlStringRGB(color);
			stringBuilder.Append($"<color=#{arg}>{text[i]}</color>");
		}
		return stringBuilder.ToString();
	}

	public static string GetFireText(string text, float speed = 2f)
	{
		Color a = new Color(1f, 0.2f, 0f);
		Color b = new Color(1f, 1f, 0f);
		Color b2 = new Color(1f, 0.5f, 0f);
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		float num = Time.time * speed;
		for (int i = 0; i < text.Length; i++)
		{
			float t = Mathf.Sin(num + (float)i * 0.5f) * 0.5f + 0.5f;
			float t2 = Mathf.Cos(num * 1.3f + (float)i * 0.3f) * 0.5f + 0.5f;
			Color a2 = Color.Lerp(a, b, t);
			Color color = Color.Lerp(a2, b2, t2);
			string arg = ColorUtility.ToHtmlStringRGB(color);
			stringBuilder.Append($"<color=#{arg}>{text[i]}</color>");
		}
		return stringBuilder.ToString();
	}

	public static string GetGlitchText(string text, float speed = 5f)
	{
		Color red = Color.red;
		Color cyan = Color.cyan;
		Color white = Color.white;
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		float num = Time.time * speed;
		for (int i = 0; i < text.Length; i++)
		{
			float num2 = Mathf.Sin(num * 7f + (float)i * 2f) * 0.5f + 0.5f;
			Color color = ((!(num2 < 0.33f)) ? ((!(num2 < 0.66f)) ? white : cyan) : red);
			string arg = ColorUtility.ToHtmlStringRGB(color);
			stringBuilder.Append($"<color=#{arg}>{text[i]}</color>");
		}
		return stringBuilder.ToString();
	}
}
