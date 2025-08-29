using System;
using Odium.Odium;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRC.Localization;
using VRC.UI;

namespace Odium.Components;

internal class MMWButton
{
	public static GameObject CreateCustomWorldButton(string buttonText, string iconPath, UnityAction clickAction)
	{
		try
		{
			GameObject gameObject = AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_MM_WorldInformation/Panel_World_Information/Content/Viewport/BodyContainer_World_Details/ScrollRect/Viewport/VerticalLayoutGroup/Actions/AddToFavorites").gameObject;
			Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_MM_WorldInformation/Panel_World_Information/Content/Viewport/BodyContainer_World_Details/ScrollRect/Viewport/VerticalLayoutGroup/Actions").transform;
			if (gameObject == null || transform == null)
			{
				Debug.LogError("Failed to find template or parent transform");
				return null;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, transform);
			gameObject2.name = "MMWButton_" + Guid.NewGuid().ToString();
			LocalizableString localizableString = new LocalizableString();
			localizableString._localizationKey = buttonText;
			TextMeshProUGUIEx textMeshProUGUIEx = gameObject2.transform.Find("Text_ButtonName")?.GetComponent<TextMeshProUGUIEx>();
			if (textMeshProUGUIEx != null)
			{
				textMeshProUGUIEx._localizableString = localizableString;
			}
			if (!string.IsNullOrEmpty(iconPath))
			{
				Sprite sprite = SpriteUtil.LoadFromDisk(iconPath);
				ImageEx val = gameObject2.transform.Find("Text_ButtonName/Icon_Add")?.GetComponent<ImageEx>();
				if ((UnityEngine.Object)(object)val != null && sprite != null)
				{
					((Image)(object)val).sprite = sprite;
				}
			}
			Button component = gameObject2.GetComponent<Button>();
			if (component != null)
			{
				component.onClick.RemoveAllListeners();
				if (clickAction != null)
				{
					component.onClick.AddListener(clickAction);
				}
			}
			gameObject2.SetActive(value: true);
			return gameObject2;
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to create custom world button: " + ex.Message);
			return null;
		}
	}
}
