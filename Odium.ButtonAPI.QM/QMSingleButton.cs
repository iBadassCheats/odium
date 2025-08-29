using System;
using TMPro;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Odium.ButtonAPI.QM;

public class QMSingleButton : QMButtonBase
{
	public QMSingleButton(QMMenuBase btnMenu, float btnXLocation, float btnYLocation, string btnText, Action btnAction, string tooltip, bool halfBtn = false, Sprite icon = null, Sprite bgImage = null)
	{
		btnQMLoc = btnMenu.GetMenuName();
		if (halfBtn)
		{
			btnYLocation -= 0.21f;
		}
		Initialize(btnXLocation, btnYLocation, btnText, btnAction, tooltip, icon, halfBtn, bgImage);
		if (halfBtn)
		{
			button.GetComponentInChildren<RectTransform>().sizeDelta /= new Vector2(1f, 2f);
		}
	}

	public QMSingleButton(DefaultVRCMenu btnMenu, float btnXLocation, float btnYLocation, string btnText, Action btnAction, string tooltip, bool halfBtn = false, Sprite sprite = null, Sprite bgImage = null)
	{
		btnQMLoc = "Menu_" + btnMenu;
		if (halfBtn)
		{
			btnYLocation -= 0.21f;
		}
		Initialize(btnXLocation, btnYLocation, btnText, btnAction, tooltip, sprite, halfBtn, bgImage);
		if (halfBtn)
		{
			button.GetComponentInChildren<RectTransform>().sizeDelta /= new Vector2(1f, 2f);
		}
	}

	public QMSingleButton(string btnMenu, float btnXLocation, float btnYLocation, string btnText, Action btnAction, string tooltip, bool halfBtn = false, Sprite sprite = null, Sprite bgImage = null)
	{
		btnQMLoc = btnMenu;
		if (halfBtn)
		{
			btnYLocation -= 0.21f;
		}
		Initialize(btnXLocation, btnYLocation, btnText, btnAction, tooltip, sprite, halfBtn, bgImage);
		if (halfBtn)
		{
			button.GetComponentInChildren<RectTransform>().sizeDelta /= new Vector2(1f, 2f);
		}
	}

	public QMSingleButton(Transform target, float btnXLocation, float btnYLocation, string btnText, Action btnAction, string tooltip, bool halfBtn = false, Sprite sprite = null, Sprite bgImage = null)
	{
		parent = target;
		if (halfBtn)
		{
			btnYLocation -= 0.21f;
		}
		Initialize(btnXLocation, btnYLocation, btnText, btnAction, tooltip, sprite, halfBtn, bgImage);
		if (halfBtn)
		{
			button.GetComponentInChildren<RectTransform>().sizeDelta /= new Vector2(1f, 2f);
		}
	}

	private void Initialize(float btnXLocation, float btnYLocation, string btnText, Action btnAction, string tooltip, Sprite sprite, bool halfBtn, Sprite bgImage = null)
	{
		if (parent == null)
		{
			parent = ApiUtils.QuickMenu.transform.Find("CanvasGroup/Container/Window/QMParent/" + btnQMLoc).transform;
		}
		button = UnityEngine.Object.Instantiate(ApiUtils.GetQMButtonTemplate(), parent, worldPositionStays: true);
		button.transform.Find("Badge_MMJump").gameObject.SetActive(value: false);
		button.name = string.Format("{0}-Single-Button-{1}", "Odium", ApiUtils.RandomNumbers());
		button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 30f;
		button.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
		button.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 176f);
		button.GetComponent<RectTransform>().anchoredPosition = new Vector2(-68f, -250f);
		if (sprite == null)
		{
			button.transform.Find("Icons/Icon").GetComponentInChildren<Image>().gameObject.SetActive(value: false);
		}
		else
		{
			button.transform.Find("Icons/Icon").GetComponentInChildren<Image>().overrideSprite = sprite;
			button.transform.Find("Icons/Icon").GetComponentInChildren<Image>().sprite = sprite;
		}
		button.GetComponentInChildren<TextMeshProUGUI>().rectTransform.anchoredPosition += new Vector2(0f, 50f);
		initShift[0] = 0;
		initShift[1] = 0;
		SetLocation(btnXLocation, btnYLocation);
		if (sprite == null)
		{
			SetButtonText(btnText, hasIcon: false, halfBtn);
		}
		else
		{
			SetButtonText(btnText, hasIcon: true, halfBtn);
		}
		SetAction(btnAction);
		SetActive(state: true);
		SetTooltip(tooltip);
		if (bgImage != null)
		{
			ToggleBackgroundImage(state: true);
			SetBackgroundImage(bgImage);
		}
		else
		{
			ToggleBackgroundImage(state: false);
		}
	}

	public void SetBackgroundImage(Sprite newImg)
	{
		button.transform.Find("Background").GetComponent<Image>().sprite = newImg;
		button.transform.Find("Background").GetComponent<Image>().overrideSprite = newImg;
		RefreshButton();
	}

	public void ToggleBackgroundImage(bool state)
	{
		button.transform.Find("Background").gameObject.SetActive(state);
	}

	public void SetButtonText(string buttonText, bool hasIcon, bool halfBtn)
	{
		TextMeshProUGUI componentInChildren = button.gameObject.GetComponentInChildren<TextMeshProUGUI>();
		componentInChildren.richText = true;
		componentInChildren.text = buttonText;
		if (hasIcon)
		{
			componentInChildren.gameObject.transform.position = new Vector3(componentInChildren.gameObject.transform.position.x, componentInChildren.gameObject.transform.position.y - 0.025f, componentInChildren.gameObject.transform.position.z);
		}
		if (halfBtn)
		{
			componentInChildren.gameObject.transform.position = new Vector3(componentInChildren.gameObject.transform.position.x, componentInChildren.gameObject.transform.position.y, componentInChildren.gameObject.transform.position.z);
		}
	}

	public void SetAction(Action buttonAction)
	{
		button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
		if (buttonAction != null)
		{
			button.GetComponent<Button>().onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(buttonAction));
		}
	}

	public void SetInteractable(bool newState)
	{
		button.GetComponent<Button>().interactable = newState;
		RefreshButton();
	}

	public void SetFontSize(float size)
	{
		button.GetComponentInChildren<TextMeshProUGUI>().fontSize = size;
	}

	public void ClickMe()
	{
		button.GetComponent<Button>().onClick.Invoke();
	}

	public Image GetBackgroundImage()
	{
		return button.transform.Find("Background").GetComponent<Image>();
	}

	private void RefreshButton()
	{
		button.SetActive(value: false);
		button.SetActive(value: true);
	}
}
