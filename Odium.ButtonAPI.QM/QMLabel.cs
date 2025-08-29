using System;
using MelonLoader;
using Odium.Odium;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Odium.ButtonAPI.QM;

public class QMLabel
{
	private static Transform quickActionsTransform;

	public static bool InitializeQuickActions()
	{
		try
		{
			quickActionsTransform = AssignedVariables.userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions");
			if (quickActionsTransform == null)
			{
				MelonLogger.Error("QuickActions transform not found!");
				return false;
			}
			MelonLogger.Msg("QuickActions transform found successfully!");
			return true;
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error finding QuickActions: " + ex.Message);
			return false;
		}
	}

	public static void InsertTextIntoExistingComponent(string text, bool append = false)
	{
		if (quickActionsTransform == null && !InitializeQuickActions())
		{
			return;
		}
		try
		{
			Text componentInChildren = quickActionsTransform.GetComponentInChildren<Text>();
			if (componentInChildren != null)
			{
				if (append)
				{
					componentInChildren.text += text;
				}
				else
				{
					componentInChildren.text = text;
				}
				MelonLogger.Msg("Text updated: " + componentInChildren.text);
				return;
			}
			TextMeshProUGUI componentInChildren2 = quickActionsTransform.GetComponentInChildren<TextMeshProUGUI>();
			if (componentInChildren2 != null)
			{
				if (append)
				{
					componentInChildren2.text += text;
				}
				else
				{
					componentInChildren2.text = text;
				}
				MelonLogger.Msg("TextMeshPro updated: " + componentInChildren2.text);
			}
			else
			{
				MelonLogger.Warning("No Text or TextMeshPro component found in QuickActions!");
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error inserting text: " + ex.Message);
		}
	}

	public static GameObject CreateNewTextElement(string text, int fontSize = 14, Color? color = null)
	{
		if (quickActionsTransform == null && !InitializeQuickActions())
		{
			return null;
		}
		try
		{
			GameObject gameObject = new GameObject("QuickAction_Text_" + DateTime.Now.Ticks);
			gameObject.transform.SetParent(quickActionsTransform, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			try
			{
				TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
				textMeshProUGUI.text = text;
				textMeshProUGUI.fontSize = fontSize;
				textMeshProUGUI.color = color ?? Color.white;
				textMeshProUGUI.alignment = TextAlignmentOptions.Center;
				textMeshProUGUI.raycastTarget = false;
			}
			catch
			{
				Text text2 = gameObject.AddComponent<Text>();
				text2.text = text;
				text2.fontSize = fontSize;
				text2.color = color ?? Color.white;
				text2.alignment = TextAnchor.MiddleCenter;
				text2.raycastTarget = false;
				text2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			}
			MelonLogger.Msg("Created new text element: " + text);
			return gameObject;
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error creating text element: " + ex.Message);
			return null;
		}
	}

	public static void InsertTextIntoButton(int buttonIndex, string text)
	{
		if (quickActionsTransform == null && !InitializeQuickActions())
		{
			return;
		}
		try
		{
			Button[] array = quickActionsTransform.GetComponentsInChildren<Button>();
			if (buttonIndex >= 0 && buttonIndex < array.Length)
			{
				Button button = array[buttonIndex];
				Text componentInChildren = button.GetComponentInChildren<Text>();
				if (componentInChildren != null)
				{
					componentInChildren.text = text;
					MelonLogger.Msg($"Button {buttonIndex} text updated: {text}");
					return;
				}
				TextMeshProUGUI componentInChildren2 = button.GetComponentInChildren<TextMeshProUGUI>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.text = text;
					MelonLogger.Msg($"Button {buttonIndex} TextMeshPro updated: {text}");
				}
				else
				{
					MelonLogger.Warning($"No text component found in button {buttonIndex}");
				}
			}
			else
			{
				MelonLogger.Error($"Button index {buttonIndex} is out of range. Found {array.Length} buttons.");
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error updating button text: " + ex.Message);
		}
	}

	public static string[] GetAllButtonTexts()
	{
		if (quickActionsTransform == null && !InitializeQuickActions())
		{
			return new string[0];
		}
		try
		{
			Button[] array = quickActionsTransform.GetComponentsInChildren<Button>();
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Text componentInChildren = array[i].GetComponentInChildren<Text>();
				TextMeshProUGUI componentInChildren2 = array[i].GetComponentInChildren<TextMeshProUGUI>();
				array2[i] = componentInChildren?.text ?? componentInChildren2?.text ?? $"Button_{i}";
			}
			MelonLogger.Msg($"Found {array.Length} buttons in QuickActions");
			return array2;
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error getting button texts: " + ex.Message);
			return new string[0];
		}
	}

	public static void ClearAllText()
	{
		if (quickActionsTransform == null && !InitializeQuickActions())
		{
			return;
		}
		try
		{
			int num = 0;
			Text[] array = quickActionsTransform.GetComponentsInChildren<Text>();
			Text[] array2 = array;
			foreach (Text text in array2)
			{
				text.text = "";
				num++;
			}
			TextMeshProUGUI[] array3 = quickActionsTransform.GetComponentsInChildren<TextMeshProUGUI>();
			TextMeshProUGUI[] array4 = array3;
			foreach (TextMeshProUGUI textMeshProUGUI in array4)
			{
				textMeshProUGUI.text = "";
				num++;
			}
			MelonLogger.Msg($"Cleared {num} text components");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error clearing text: " + ex.Message);
		}
	}

	public static void DebugQuickActionsStructure()
	{
		if (quickActionsTransform == null && !InitializeQuickActions())
		{
			return;
		}
		try
		{
			MelonLogger.Msg("=== QuickActions Structure ===");
			MelonLogger.Msg("Transform name: " + quickActionsTransform.name);
			MelonLogger.Msg($"Child count: {quickActionsTransform.childCount}");
			for (int i = 0; i < quickActionsTransform.childCount; i++)
			{
				Transform child = quickActionsTransform.GetChild(i);
				MelonLogger.Msg($"Child {i}: {child.name} (Active: {child.gameObject.activeInHierarchy})");
				Text componentInChildren = child.GetComponentInChildren<Text>();
				TextMeshProUGUI componentInChildren2 = child.GetComponentInChildren<TextMeshProUGUI>();
				Button component = child.GetComponent<Button>();
				if (componentInChildren != null)
				{
					MelonLogger.Msg("  - Has Text: '" + componentInChildren.text + "'");
				}
				if (componentInChildren2 != null)
				{
					MelonLogger.Msg("  - Has TextMeshPro: '" + componentInChildren2.text + "'");
				}
				if (component != null)
				{
					MelonLogger.Msg("  - Has Button component");
				}
			}
			MelonLogger.Msg("=== End Structure ===");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error debugging structure: " + ex.Message);
		}
	}
}
