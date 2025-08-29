using System;
using Odium.Odium;
using UnityEngine;
using UnityEngine.UI;

namespace Odium.ButtonAPI.QM;

internal class QMMainIconButton
{
	public static void CreateButton(Sprite sprite, Action onClick = null)
	{
		try
		{
			Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/Header_H1/RightItemContainer");
			Transform transform2 = transform.Find("Button_QM_Report");
			if (transform == null || transform2 == null)
			{
				Debug.LogError("Could not find required QuickMenu elements!");
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(transform2.gameObject, transform);
			gameObject.name = "Button_QMOdium" + Guid.NewGuid().ToString();
			gameObject.SetActive(value: true);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.localRotation = Quaternion.identity;
			component.localScale = Vector3.one;
			Transform transform3 = gameObject.transform.Find("Icon");
			if (transform3 != null)
			{
				Image component2 = transform3.GetComponent<Image>();
				if (component2 != null)
				{
					component2.sprite = sprite;
					component2.overrideSprite = sprite;
				}
			}
			Button component3 = gameObject.GetComponent<Button>();
			if (component3 != null && onClick != null)
			{
				component3.onClick.RemoveAllListeners();
				component3.onClick.AddListener(onClick);
			}
			gameObject.transform.SetSiblingIndex(transform2.GetSiblingIndex());
		}
		catch (Exception arg)
		{
			Debug.LogError($"Error creating QM icon button: {arg}");
		}
	}

	public static void CreateImage(Sprite sprite, Vector3 position, Vector3 size, bool includeBackground = false)
	{
		try
		{
			Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/Header_H1/RightItemContainer");
			Transform transform2 = transform.Find("Button_QM_Report");
			if (transform == null || transform2 == null)
			{
				Debug.LogError("Could not find required QuickMenu elements!");
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(transform2.gameObject, transform);
			gameObject.name = "Button_QMOdium" + Guid.NewGuid().ToString();
			gameObject.SetActive(value: true);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.localPosition = position;
			component.localRotation = Quaternion.identity;
			component.localScale = size;
			Transform transform3 = gameObject.transform.Find("Icon");
			transform3.localPosition = new Vector3(-208.8547f, -22.7455f, 0f);
			if (transform3 != null)
			{
				Image component2 = transform3.GetComponent<Image>();
				if (component2 != null)
				{
					component2.sprite = sprite;
					component2.overrideSprite = sprite;
				}
			}
			Transform transform4 = gameObject.transform.FindChild("Background");
			transform4.gameObject.SetActive(includeBackground);
		}
		catch (Exception arg)
		{
			Debug.LogError($"Error creating QM icon button: {arg}");
		}
	}

	public static void CreateToggle(Sprite onSprite, Sprite offSprite, Action onAction = null, Action offAction = null)
	{
		try
		{
			Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/Header_H1/RightItemContainer");
			Transform transform2 = transform.Find("Button_QM_Report");
			if (transform == null || transform2 == null)
			{
				Debug.LogError("Could not find required QuickMenu elements!");
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(transform2.gameObject, transform);
			gameObject.name = "Toggle_QMOdium" + Guid.NewGuid().ToString();
			gameObject.SetActive(value: true);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.localRotation = Quaternion.identity;
			component.localScale = Vector3.one;
			Transform transform3 = gameObject.transform.Find("Icon");
			if (transform3 == null)
			{
				Debug.LogError("Could not find Icon transform!");
				return;
			}
			Image iconImage = transform3.GetComponent<Image>();
			if (iconImage == null)
			{
				Debug.LogError("Could not find Image component on Icon!");
				return;
			}
			bool isToggled = false;
			iconImage.sprite = offSprite;
			iconImage.overrideSprite = offSprite;
			Button component2 = gameObject.GetComponent<Button>();
			if (component2 != null)
			{
				component2.onClick.RemoveAllListeners();
				component2.onClick.AddListener((Action)delegate
				{
					isToggled = !isToggled;
					if (isToggled)
					{
						iconImage.sprite = onSprite;
						iconImage.overrideSprite = onSprite;
						onAction?.Invoke();
					}
					else
					{
						iconImage.sprite = offSprite;
						iconImage.overrideSprite = offSprite;
						offAction?.Invoke();
					}
				});
			}
			gameObject.transform.SetSiblingIndex(transform2.GetSiblingIndex());
		}
		catch (Exception arg)
		{
			Debug.LogError($"Error creating QM toggle button: {arg}");
		}
	}
}
