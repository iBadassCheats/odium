using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Odium.UX;

internal class InternalConsole
{
	private static List<string> ConsoleLogCache = new List<string>();

	public static void LogIntoConsole(string txt, string type = "<color=#8d142b>[Log]</color>", string color = "8d142b")
	{
		string text = DateTime.Now.ToString("HH:mm:ss");
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<size=14><color=#" + color + ">[" + text + "]</color> ");
		stringBuilder.Append(type);
		stringBuilder.Append(" ");
		stringBuilder.Append(txt);
		stringBuilder.Append("</size>");
		ConsoleLogCache.Add(stringBuilder.ToString());
	}

	public static void ProcessLogCache()
	{
		try
		{
			if (ConsoleLogCache.Count > 25)
			{
				ConsoleLogCache.RemoveRange(0, ConsoleLogCache.Count - 25);
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in ConsoleLogCache)
			{
				stringBuilder.AppendLine(item);
			}
			MainMenu.ConsoleObject.SetActive(value: false);
			Transform transform = MainMenu.ConsoleObject.transform.FindChild("TextLayoutParent/Text_H4");
			if (transform != null)
			{
				RectTransform component = transform.gameObject.GetComponent<RectTransform>();
				if (component != null)
				{
					component.localPosition = new Vector3(0f, 40f, 0f);
					component.sizeDelta = new Vector2(680f, 280f);
				}
				TextMeshProUGUIEx component2 = transform.gameObject.GetComponent<TextMeshProUGUIEx>();
				component2.alignment = TextAlignmentOptions.TopLeft;
				component2.richText = true;
				component2.enableWordWrapping = true;
				component2.fontSize = 14f;
				component2.lineSpacing = -10f;
				component2.overflowMode = TextOverflowModes.Overflow;
				component2.prop_String_0 = stringBuilder.ToString();
			}
			MainMenu.ConsoleObject.SetActive(value: true);
		}
		catch (Exception)
		{
		}
	}
}
