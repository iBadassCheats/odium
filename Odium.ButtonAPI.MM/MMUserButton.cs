using System;
using Odium.Odium;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI;

namespace Odium.ButtonAPI.MM;

public class MMUserButton
{
	public string Text { get; private set; }

	public Action Action { get; private set; }

	public Sprite Icon { get; private set; }

	public GameObject ButtonObject { get; private set; }

	public MMUserButton(MMUserActionRow actionRow, string text, Action action = null, Sprite icon = null)
	{
		Text = text;
		Action = action;
		Icon = icon;
		if (actionRow?.RowObject != null)
		{
			AttachToRow(actionRow.RowObject);
		}
		else
		{
			Debug.LogError("User action row is null or not properly initialized");
		}
	}

	private void AttachToRow(GameObject customRow)
	{
		try
		{
			if (customRow == null)
			{
				Debug.LogError("Custom user row is null");
				return;
			}
			Transform transform = customRow.transform.Find("CellGrid_MM_Content");
			if (transform == null)
			{
				Debug.LogError("Could not find CellGrid_MM_Content in custom user row");
				return;
			}
			Button button = ((AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_UserDetail/ScrollRect/Viewport/VerticalLayoutGroup")?.Find("Row3"))?.Find("CellGrid_MM_Content"))?.GetComponentInChildren<Button>();
			if (button == null)
			{
				Debug.LogError("Could not find template button to clone");
				return;
			}
			OdiumConsole.Log("Odium", "Adding button '" + Text + "' to custom user row");
			ButtonObject = UnityEngine.Object.Instantiate(button.gameObject);
			ButtonObject.transform.SetParent(transform, worldPositionStays: false);
			ButtonObject.transform.localScale = Vector3.one;
			ButtonObject.transform.localPosition = Vector3.zero;
			ButtonObject.transform.localRotation = Quaternion.identity;
			Button component = ButtonObject.GetComponent<Button>();
			if (component == null)
			{
				Debug.LogError("Cloned object doesn't have Button component");
				UnityEngine.Object.DestroyImmediate(ButtonObject);
				return;
			}
			UpdateButtonText();
			UpdateButtonIcon();
			component.onClick.RemoveAllListeners();
			component.onClick.AddListener((Action)delegate
			{
				OdiumConsole.Log("Odium", "Custom user button '" + Text + "' clicked");
				Action?.Invoke();
			});
			ButtonObject.name = "Odium_CustomUserButton_" + Text.Replace(" ", "");
			OdiumConsole.Log("Odium", "Successfully added button '" + Text + "' to custom user row");
		}
		catch (Exception ex)
		{
			Debug.LogError("Error adding button to custom user row: " + ex.Message);
		}
	}

	private void UpdateButtonText()
	{
		if (ButtonObject == null)
		{
			return;
		}
		TextMeshProUGUIEx textMeshProUGUIEx = ButtonObject.GetComponentInChildren<TextMeshProUGUIEx>();
		if (textMeshProUGUIEx == null)
		{
			Transform transform = ButtonObject.transform.Find("Text_ButtonName");
			if (transform != null)
			{
				textMeshProUGUIEx = transform.GetComponent<TextMeshProUGUIEx>();
			}
		}
		if (textMeshProUGUIEx != null)
		{
			textMeshProUGUIEx.text = Text;
			OdiumConsole.Log("Odium", "Set button text to: " + Text);
		}
		else
		{
			OdiumConsole.Log("Odium", "Warning: Could not find text component");
		}
	}

	private void UpdateButtonIcon()
	{
		if (ButtonObject == null)
		{
			return;
		}
		ImageEx val = null;
		Transform transform = ButtonObject.transform.Find("Text_ButtonName");
		if (transform != null)
		{
			Transform transform2 = transform.Find("Icon");
			if (transform2 != null)
			{
				val = transform2.GetComponent<ImageEx>();
			}
		}
		if (Icon != null && (UnityEngine.Object)(object)val != null)
		{
			((Image)(object)val).sprite = Icon;
			((Behaviour)(object)val).enabled = true;
			OdiumConsole.Log("Odium", "Icon set successfully");
		}
		else if ((UnityEngine.Object)(object)val != null)
		{
			((Behaviour)(object)val).enabled = false;
			OdiumConsole.Log("Odium", "Icon hidden (no sprite provided)");
		}
	}

	public void Destroy()
	{
		if (ButtonObject != null)
		{
			UnityEngine.Object.DestroyImmediate(ButtonObject);
			ButtonObject = null;
			OdiumConsole.Log("Odium", "Destroyed user button: " + Text);
		}
	}
}
