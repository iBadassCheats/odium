using System;
using Odium.Odium;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI;

namespace Odium.ButtonAPI.MM;

public class MMWorldButton
{
	public string Text { get; private set; }

	public Action Action { get; private set; }

	public Sprite Icon { get; private set; }

	public GameObject ButtonObject { get; private set; }

	public MMWorldButton(MMWorldActionRow actionRow, string text, Action action = null, Sprite icon = null)
	{
		Text = text;
		Action = action;
		Icon = icon;
		if (actionRow?.ActionsObject != null)
		{
			AttachToRow(actionRow.ActionsObject);
		}
		else
		{
			Debug.LogError("Action row is null or not properly initialized");
		}
	}

	private void AttachToRow(GameObject actionsContainer)
	{
		try
		{
			if (actionsContainer == null)
			{
				Debug.LogError("Actions container is null");
				return;
			}
			Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_MM_WorldInformation/Panel_World_Information/Content/Viewport/BodyContainer_World_Details/ScrollRect/Viewport/VerticalLayoutGroup")?.Find("Actions");
			GameObject gameObject = null;
			if (transform != null)
			{
				Transform transform2 = transform.Find("MakeHome");
				if (transform2 != null)
				{
					gameObject = transform2.gameObject;
				}
				else
				{
					for (int i = 0; i < transform.childCount; i++)
					{
						Transform child = transform.GetChild(i);
						if (child.GetComponent<Button>() != null)
						{
							gameObject = child.gameObject;
							break;
						}
					}
				}
			}
			if (gameObject == null)
			{
				Debug.LogError("Could not find template button");
				return;
			}
			OdiumConsole.Log("Odium", "Adding button '" + Text + "' to actions container");
			ButtonObject = UnityEngine.Object.Instantiate(gameObject);
			ButtonObject.transform.SetParent(actionsContainer.transform, worldPositionStays: false);
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
				OdiumConsole.Log("Odium", "Custom button '" + Text + "' clicked");
				Action?.Invoke();
			});
			ButtonObject.name = "Odium_CustomButton_" + Text.Replace(" ", "");
			OdiumConsole.Log("Odium", "Successfully added button '" + Text + "' to actions container");
		}
		catch (Exception ex)
		{
			Debug.LogError("Error adding button to actions container: " + ex.Message);
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
			OdiumConsole.Log("Odium", "Destroyed button: " + Text);
		}
	}
}
