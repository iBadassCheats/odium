using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.Localization;
using VRC.UI.Core.Styles;
using VRC.UI.Elements;
using VRC.UI.Elements.Controls;
using VRC.UI.Pages.QM;

namespace Odium.ButtonAPI.QM;

public class QMMenuPage : QMMenuBase
{
	protected GameObject MainButton;

	protected GameObject BadgeObject;

	protected TextMeshProUGUI BadgeText;

	protected MenuTab MenuTabComp;

	public QMMenuPage(string MenuTitle, string tooltip, Sprite ButtonImage = null)
	{
		Initialize(MenuTitle, tooltip, ButtonImage);
	}

	private void Initialize(string MenuTitle, string tooltip, Sprite ButtonImage)
	{
		MenuName = string.Format("{0}-Tab-Menu-{1}", "Odium", ApiUtils.RandomNumbers());
		MenuObject = UnityEngine.Object.Instantiate(ApiUtils.GetQMMenuTemplate(), ApiUtils.GetQMMenuTemplate().transform.parent);
		MenuObject.name = MenuName;
		MenuObject.SetActive(value: false);
		MenuObject.transform.SetSiblingIndex(19);
		InterfacePublicAbstractObBoObVoStObInVoStBoUnique field_Protected_InterfacePublicAbstractObBoObVoStObInVoStBoUnique_ = MenuObject.GetComponent<Dashboard>().field_Protected_InterfacePublicAbstractObBoObVoStObInVoStBoUnique_0;
		UnityEngine.Object.DestroyImmediate(MenuObject.GetComponent<Dashboard>());
		MenuPage = MenuObject.AddComponent<UIPage>();
		MenuPage.field_Public_String_0 = MenuName;
		MenuPage.field_Protected_InterfacePublicAbstractObBoObVoStObInVoStBoUnique_0 = field_Protected_InterfacePublicAbstractObBoObVoStObInVoStBoUnique_;
		MenuPage.field_Private_List_1_UIPage_0 = new Il2CppSystem.Collections.Generic.List<UIPage>();
		MenuPage.field_Private_List_1_UIPage_0.Add(MenuPage);
		ApiUtils.QuickMenu.prop_MenuStateController_0.field_Private_Dictionary_2_String_UIPage_0.Add(MenuName, MenuPage);
		System.Collections.Generic.List<UIPage> list = ApiUtils.QuickMenu.prop_MenuStateController_0.field_Public_ArrayOf_UIPage_0.ToList();
		list.Add(MenuPage);
		ApiUtils.QuickMenu.prop_MenuStateController_0.field_Public_ArrayOf_UIPage_0 = list.ToArray();
		Transform transform = MenuObject.transform.Find("ScrollRect/Viewport/VerticalLayoutGroup");
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!(child == null))
			{
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
		MenuTitleText = MenuObject.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
		SetMenuTitle(MenuTitle);
		MenuObject.transform.GetChild(0).Find("RightItemContainer/Button_QM_Expand").gameObject.SetActive(value: false);
		MenuObject.transform.GetChild(0).Find("RightItemContainer/Button_QM_Report").gameObject.SetActive(value: false);
		ClearChildren();
		MenuObject.transform.Find("ScrollRect").GetComponent<ScrollRect>().enabled = false;
		MainButton = UnityEngine.Object.Instantiate(ApiUtils.GetQMTabButtonTemplate(), ApiUtils.GetQMTabButtonTemplate().transform.parent);
		MainButton.name = MenuName;
		MenuTabComp = MainButton.GetComponent<MenuTab>();
		MenuTabComp.field_Private_MenuStateController_0 = ApiUtils.QuickMenu.prop_MenuStateController_0;
		MenuTabComp._controlName = MenuName;
		MenuTabComp.GetComponent<StyleElement>().field_Private_Selectable_0 = MenuTabComp.GetComponent<Button>();
		BadgeObject = MainButton.transform.GetChild(0).gameObject;
		BadgeText = BadgeObject.GetComponentInChildren<TextMeshProUGUI>();
		MainButton.GetComponent<Button>().onClick.AddListener((Action)delegate
		{
			MenuObject.SetActive(value: true);
			MenuObject.GetComponent<Canvas>().enabled = true;
			MenuObject.GetComponent<CanvasGroup>().enabled = true;
			MenuObject.GetComponent<GraphicRaycaster>().enabled = true;
			MenuTabComp.GetComponent<StyleElement>().field_Private_Selectable_0 = MenuTabComp.GetComponent<Button>();
		});
		UnityEngine.Object.Destroy(MainButton.GetComponent<MonoBehaviour1PublicVoVo5>());
		SetTooltip(tooltip);
		if (ButtonImage != null)
		{
			SetImage(ButtonImage);
		}
	}

	public void SetImage(Sprite newImg)
	{
		MainButton.transform.Find("Icon").GetComponent<Image>().sprite = newImg;
		MainButton.transform.Find("Icon").GetComponent<Image>().overrideSprite = newImg;
		MainButton.transform.Find("Icon").GetComponent<Image>().color = Color.white;
		MainButton.transform.Find("Icon").GetComponent<Image>().m_Color = Color.white;
	}

	public void SetIndex(int newPosition)
	{
		MainButton.transform.SetSiblingIndex(newPosition);
	}

	public void SetActive(bool newState)
	{
		MainButton.SetActive(newState);
	}

	public void SetBadge(bool showing = true, string text = "")
	{
		if (BadgeObject != null && BadgeText != null)
		{
			BadgeObject.SetActive(showing);
			BadgeText.text = text;
		}
	}

	public void OpenMe()
	{
		MainButton.GetComponent<Button>().onClick.Invoke();
	}

	public void SetTooltip(string tooltip)
	{
		foreach (ToolTip component in MainButton.GetComponents<ToolTip>())
		{
			component._localizableString = LocalizableStringExtensions.Localize(tooltip);
			component._alternateLocalizableString = LocalizableStringExtensions.Localize(tooltip);
		}
	}

	public GameObject GetMainButton()
	{
		return MainButton;
	}
}
