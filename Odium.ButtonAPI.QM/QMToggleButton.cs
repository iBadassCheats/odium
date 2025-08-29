using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Odium.ButtonAPI.QM;

public class QMToggleButton : QMButtonBase
{
	protected TextMeshProUGUI btnTextComp;

	protected Button btnComp;

	protected Image btnImageComp;

	protected bool currentState;

	protected Action OnAction;

	protected Action OffAction;

	public QMToggleButton(QMMenuBase location, float btnXPos, float btnYPos, string btnText, Action onAction, Action offAction, string tooltip, bool defaultState = false, Sprite bgImage = null)
	{
		btnQMLoc = location.GetMenuName();
		Initialize(btnXPos, btnYPos, btnText, onAction, offAction, tooltip, defaultState, bgImage);
	}

	public QMToggleButton(DefaultVRCMenu location, float btnXPos, float btnYPos, string btnText, Action onAction, Action offAction, string tooltip, bool defaultState = false, Sprite bgImage = null)
	{
		btnQMLoc = "Menu_" + location;
		Initialize(btnXPos, btnYPos, btnText, onAction, offAction, tooltip, defaultState, bgImage);
	}

	public QMToggleButton(Transform target, float btnXPos, float btnYPos, string btnText, Action onAction, Action offAction, string tooltip, bool defaultState = false, Sprite bgImage = null)
	{
		parent = target;
		Initialize(btnXPos, btnYPos, btnText, onAction, offAction, tooltip, defaultState, bgImage);
	}

	private void Initialize(float btnXLocation, float btnYLocation, string btnText, Action onAction, Action offAction, string tooltip, bool defaultState, Sprite bgImage = null)
	{
		if (parent == null)
		{
			parent = ApiUtils.QuickMenu.transform.Find("CanvasGroup/Container/Window/QMParent/" + btnQMLoc).transform;
		}
		button = UnityEngine.Object.Instantiate(ApiUtils.GetQMButtonTemplate(), parent, worldPositionStays: true);
		button.name = string.Format("{0}-Toggle-Button-{1}", "Odium", ApiUtils.RandomNumbers());
		button.transform.Find("Badge_MMJump").gameObject.SetActive(value: false);
		button.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 176f);
		button.GetComponent<RectTransform>().anchoredPosition = new Vector2(-68f, -250f);
		btnTextComp = button.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
		btnTextComp.color = Color.white;
		btnComp = button.GetComponentInChildren<Button>(includeInactive: true);
		btnComp.onClick = new Button.ButtonClickedEvent();
		btnComp.onClick.AddListener((Action)HandleClick);
		btnImageComp = button.transform.Find("Icons/Icon").GetComponentInChildren<Image>(includeInactive: true);
		btnImageComp.gameObject.SetActive(value: true);
		initShift[0] = 0;
		initShift[1] = 0;
		SetLocation(btnXLocation, btnYLocation);
		SetButtonText(btnText);
		SetButtonActions(onAction, offAction);
		SetTooltip(tooltip);
		SetActive(state: true);
		currentState = defaultState;
		Sprite sprite = (currentState ? ApiUtils.OnIconSprite() : ApiUtils.OffIconSprite());
		btnImageComp.sprite = sprite;
		btnImageComp.overrideSprite = sprite;
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

	private void HandleClick()
	{
		currentState = !currentState;
		Sprite sprite = (currentState ? ApiUtils.OnIconSprite() : ApiUtils.OffIconSprite());
		btnImageComp.sprite = sprite;
		btnImageComp.overrideSprite = sprite;
		if (currentState)
		{
			OnAction();
		}
		else
		{
			OffAction();
		}
	}

	public void SetButtonText(string buttonText)
	{
		TextMeshProUGUI componentInChildren = button.gameObject.GetComponentInChildren<TextMeshProUGUI>();
		componentInChildren.richText = true;
		componentInChildren.text = buttonText;
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

	public void SetButtonActions(Action onAction, Action offAction)
	{
		OnAction = onAction;
		OffAction = offAction;
	}

	public void SetToggleState(bool newState, bool shouldInvoke = false)
	{
		try
		{
			Sprite sprite = (newState ? ApiUtils.OnIconSprite() : ApiUtils.OffIconSprite());
			btnImageComp.sprite = sprite;
			btnImageComp.overrideSprite = sprite;
			currentState = newState;
			if (shouldInvoke)
			{
				if (newState)
				{
					OnAction();
				}
				else
				{
					OffAction();
				}
			}
		}
		catch
		{
		}
	}

	public void SetInteractable(bool newState)
	{
		button.GetComponent<Button>().interactable = newState;
		RefreshButton();
	}

	public void ClickMe()
	{
		HandleClick();
	}

	public bool GetCurrentState()
	{
		return currentState;
	}

	private void RefreshButton()
	{
		button.SetActive(value: false);
		button.SetActive(value: true);
	}
}
