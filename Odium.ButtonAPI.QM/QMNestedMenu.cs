using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Pages.QM;

namespace Odium.ButtonAPI.QM;

public class QMNestedMenu : QMMenuBase
{
	protected bool IsMenuRoot;

	protected GameObject BackButton;

	protected QMSingleButton MainButton;

	protected Transform parent;

	public QMNestedMenu(QMMenuBase location, float posX, float posY, string btnText, string menuTitle, string tooltip, bool halfButton = false, Sprite sprite = null, Sprite bgImage = null)
	{
		btnQMLoc = location.GetMenuName();
		Initialize(isRoot: false, btnText, posX, posY, menuTitle, tooltip, halfButton, sprite, bgImage);
	}

	public QMNestedMenu(DefaultVRCMenu location, float posX, float posY, string btnText, string menuTitle, string tooltip, bool halfButton = false, Sprite sprite = null, Sprite bgImage = null)
	{
		btnQMLoc = "Menu_" + location;
		Initialize(isRoot: false, btnText, posX, posY, menuTitle, tooltip, halfButton, sprite, bgImage);
	}

	public QMNestedMenu(Transform target, float posX, float posY, string btnText, string menuTitle, string tooltip, bool halfButton = false, Sprite sprite = null, Sprite bgImage = null)
	{
		parent = target;
		Initialize(isRoot: false, btnText, posX, posY, menuTitle, tooltip, halfButton, sprite, bgImage);
	}

	private void Initialize(bool isRoot, string btnText, float btnPosX, float btnPosY, string menuTitle, string tooltip, bool halfButton, Sprite sprite, Sprite bgImage)
	{
		MenuName = string.Format("{0}-QMMenu-{1}", "Odium", ApiUtils.RandomNumbers());
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
		IsMenuRoot = isRoot;
		if (IsMenuRoot)
		{
			System.Collections.Generic.List<UIPage> list = ApiUtils.QuickMenu.prop_MenuStateController_0.field_Public_ArrayOf_UIPage_0.ToList();
			list.Add(MenuPage);
			ApiUtils.QuickMenu.prop_MenuStateController_0.field_Public_ArrayOf_UIPage_0 = list.ToArray();
		}
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
		SetMenuTitle(menuTitle);
		BackButton = MenuObject.transform.GetChild(0).Find("LeftItemContainer/Button_Back").gameObject;
		BackButton.SetActive(value: true);
		BackButton.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
		BackButton.GetComponentInChildren<Button>().onClick.AddListener((Action)delegate
		{
			if (isRoot)
			{
				if (btnQMLoc.StartsWith("Menu_"))
				{
					ApiUtils.QuickMenu.prop_MenuStateController_0.Method_Public_Void_String_Boolean_Boolean_PDM_0("QuickMenu" + btnQMLoc.Remove(0, 5));
				}
				else
				{
					ApiUtils.QuickMenu.prop_MenuStateController_0.Method_Public_Void_String_Boolean_Boolean_PDM_0(btnQMLoc);
				}
			}
			else
			{
				MenuPage.Method_Protected_Virtual_New_Void_0();
			}
		});
		MenuObject.transform.GetChild(0).Find("RightItemContainer/Button_QM_Expand").gameObject.SetActive(value: false);
		MenuObject.transform.GetChild(0).Find("RightItemContainer/Button_QM_Report").gameObject.SetActive(value: false);
		if (parent != null)
		{
			MainButton = new QMSingleButton(parent, btnPosX, btnPosY, btnText, OpenMe, tooltip, halfButton, sprite, bgImage);
		}
		else
		{
			MainButton = new QMSingleButton(btnQMLoc, btnPosX, btnPosY, btnText, OpenMe, tooltip, halfButton, sprite, bgImage);
		}
		ClearChildren();
		MenuObject.transform.Find("ScrollRect").GetComponent<ScrollRect>().enabled = false;
	}

	public void OpenMe()
	{
		ApiUtils.QuickMenu.prop_MenuStateController_0.Method_Public_Void_String_UIContext_Boolean_EnumNPublicSealedvaNoLeRiBoIn6vUnique_0(MenuPage.field_Public_String_0, null, param_3: false, UIPage.EnumNPublicSealedvaNoLeRiBoIn6vUnique.Left);
		MenuObject.SetActive(value: true);
		MenuObject.GetComponent<Canvas>().enabled = true;
		MenuObject.GetComponent<CanvasGroup>().enabled = true;
		MenuObject.GetComponent<GraphicRaycaster>().enabled = true;
	}

	public void CloseMe()
	{
		BackButton.GetComponent<Button>().onClick.Invoke();
	}

	public QMSingleButton GetMainButton()
	{
		return MainButton;
	}

	public GameObject GetBackButton()
	{
		return BackButton;
	}
}
