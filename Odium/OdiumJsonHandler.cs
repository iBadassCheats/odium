using System;
using System.Text;

namespace Odium;

public static class OdiumJsonHandler
{
	public static OdiumPreferences ParsePreferences(string jsonString)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(jsonString))
			{
				return null;
			}
			OdiumPreferences odiumPreferences = new OdiumPreferences();
			string text = jsonString.Trim().Replace(" ", "").Replace("\t", "")
				.Replace("\n", "")
				.Replace("\r", "");
			if (text.StartsWith("{"))
			{
				text = text.Substring(1);
			}
			if (text.EndsWith("}"))
			{
				text = text.Substring(0, text.Length - 1);
			}
			string[] array = text.Split(',');
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (string.IsNullOrWhiteSpace(text2))
				{
					continue;
				}
				string[] array3 = text2.Split(':');
				if (array3.Length == 2)
				{
					string text3 = RemoveQuotes(array3[0].Trim());
					string value = RemoveQuotes(array3[1].Trim());
					if (text3.Equals("allocConsole", StringComparison.OrdinalIgnoreCase) && bool.TryParse(value, out var result))
					{
						odiumPreferences.AllocConsole = result;
					}
				}
			}
			return odiumPreferences;
		}
		catch
		{
			return null;
		}
	}

	public static string SerializePreferences(OdiumPreferences preferences)
	{
		try
		{
			if (preferences == null)
			{
				return "{}";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine("  \"allocConsole\": " + preferences.AllocConsole.ToString().ToLower());
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}
		catch
		{
			return "{}";
		}
	}

	private static string RemoveQuotes(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		if (input.StartsWith("\"") && input.EndsWith("\"") && input.Length >= 2)
		{
			return input.Substring(1, input.Length - 2);
		}
		return input;
	}
}
