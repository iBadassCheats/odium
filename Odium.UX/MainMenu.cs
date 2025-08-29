using System;
using System.Collections.Generic;
using System.IO;
using Odium.Components;
using Odium.Odium;
using Odium.Wrappers;
using TMPro;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.UI;
using VRC.Localization;
using VRC.UI.Client.Marketplace;
using VRC.UI.Controls;
using VRC.UI.Elements.Controls;

namespace Odium.UX;

public class MainMenu
{
	public static GameObject ExampleButton;

	public static GameObject AdBanner;

	public static GameObject QuickLinksHeader;

	public static GameObject QuickActionsHeader;

	public static GameObject LinksButtons;

	public static GameObject ActionsButtons;

	public static GameObject buttonsQuickLinksGrid;

	public static GameObject buttonsQuickActionsGrid;

	public static GameObject LaunchPadText;

	public static GameObject ConsoleParent;

	public static GameObject ConsoleTemplate;

	public static GameObject ConsoleObject;

	public static GameObject SafteyButton;

	public static GameObject UserInterface;

	public static bool ui_ready = false;

	private static List<string> processed_buttons = new List<string>();

	private static DateTime lastGradientChange;

	private static DateTime lastTimeCheck = DateTime.Now;

	private static float gradientShift = 0f;

	public static GameObject MenuInstance;

	public static MenuStateController _menuStateController;

	public static MenuStateController MenuStateControllerInstance
	{
		get
		{
			if (_menuStateController == null)
			{
				_menuStateController = MenuInstance.GetComponent<MenuStateController>();
			}
			return _menuStateController;
		}
	}

	public static void Setup()
	{
		DateTime now = DateTime.Now;
		if (LaunchPadText == null)
		{
			LaunchPadText = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/Header_H1/LeftItemContainer/Text_Title");
			return;
		}
		if (ConsoleTemplate == null)
		{
			ConsoleTemplate = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks/Button_Worlds");
			return;
		}
		if (ConsoleParent == null)
		{
			ConsoleParent = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners");
			return;
		}
		if (AdBanner == null)
		{
			AdBanner = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners/Image_MASK/Image");
			return;
		}
		if (QuickActionsHeader == null)
		{
			QuickActionsHeader = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickActions");
			return;
		}
		if (QuickLinksHeader == null)
		{
			QuickLinksHeader = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickLinks");
			return;
		}
		if (LinksButtons == null)
		{
			LinksButtons = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks");
			return;
		}
		if (ActionsButtons == null)
		{
			ActionsButtons = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions");
			return;
		}
		if (buttonsQuickLinksGrid == null)
		{
			buttonsQuickLinksGrid = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks");
			return;
		}
		if (buttonsQuickActionsGrid == null)
		{
			buttonsQuickActionsGrid = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions");
			return;
		}
		if (SafteyButton == null)
		{
			SafteyButton = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/SitStandCalibrateButton");
		}
		if (!ui_ready)
		{
			QuickActionsHeader.SetActive(value: false);
			QuickLinksHeader.SetActive(value: false);
			AdBanner.SetActive(value: false);
			ConsoleObject = UnityEngine.Object.Instantiate(ConsoleTemplate, ConsoleParent.transform);
			InitConsole(ConsoleObject);
			SetupButton(SafteyButton, "SitStandCalibrateButton");
			ExampleButton = UnityEngine.Object.Instantiate(ConsoleTemplate, ConsoleParent.transform);
			ExampleButton.name = "ExampleButton";
			ExampleButton.SetActive(value: false);
			GameObject gameObject = GameObject.Find("UserInterface/Canvas_MainMenu(Clone)/Container/PageButtons/HorizontalLayoutGroup/Marketplace_Button_Tab");
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.parent);
			gameObject2.name = "Heart_Button_Tab";
			Transform transform = gameObject2.transform.Find("Icon");
			if (transform != null)
			{
				GameObject gameObject3 = transform.gameObject;
				Image component = gameObject3.GetComponent<Image>();
				if (component != null)
				{
					string path = Path.Combine(Directory.GetCurrentDirectory(), "Odium", "OdiumIcon.png");
					Sprite sprite = SpriteUtil.LoadFromDisk(path);
					component.sprite = sprite;
				}
			}
			Transform transform2 = gameObject2.transform.Find("Text_H4");
			if (transform2 != null)
			{
				GameObject gameObject4 = transform2.gameObject;
				TextMeshProUGUIEx component2 = gameObject4.GetComponent<TextMeshProUGUIEx>();
				if (component2 != null)
				{
					component2.text = "Odium Client";
				}
			}
			SubscriptionNotifierComponent component3 = gameObject2.GetComponent<SubscriptionNotifierComponent>();
			if (component3 != null)
			{
				component3.enabled = false;
			}
			MenuInstance = GameObject.Find("UserInterface/Canvas_MainMenu(Clone)");
			GameObject gameObject5 = GameObject.Find("UserInterface/Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_MM_Marketplace");
			GameObject CustomPage = UnityEngine.Object.Instantiate(gameObject5, gameObject5.transform.parent);
			MonoBehaviour1PublicOb_sGa_ppaObwapuBuObUnique component4 = CustomPage.GetComponent<MonoBehaviour1PublicOb_sGa_ppaObwapuBuObUnique>();
			GameObject gameObject6 = GameObject.Find("UserInterface/Canvas_MainMenu(Clone)");
			MenuStateController component5 = gameObject6.GetComponent<MenuStateController>();
			component5.field_Private_Dictionary_2_String_UIPage_0.Add("XD", component4);
			component5.field_Private_HashSet_1_UIPage_0.Add(component4);
			MenuTab component6 = gameObject2.GetComponent<MenuTab>();
			component6._sendAnalytics = false;
			component6.prop_String_1 = "Odium";
			Button component7 = gameObject2.GetComponent<Button>();
			if (component7 != null)
			{
				component7.onClick.RemoveAllListeners();
				component7.onClick.AddListener((Action)delegate
				{
					CustomPage.SetActive(value: true);
				});
			}
		}
		if (ConsoleObject != null)
		{
			ConsoleObject.transform.localPosition = new Vector3(0f, -272f, 0f);
		}
		if (LaunchPadText != null)
		{
			if ((now - lastGradientChange).TotalMilliseconds >= 250.0)
			{
				TextMeshProUGUIEx component8 = LaunchPadText.GetComponent<TextMeshProUGUIEx>();
				if (component8 != null)
				{
					component8.enableVertexGradient = true;
					gradientShift += Time.deltaTime * 0.5f;
					gradientShift += Time.deltaTime * 0.5f;
					float num = Mathf.Sin(gradientShift + 0f) * 0.5f + 1.5f;
					float num2 = Mathf.Sin(gradientShift + 1f) * 0.5f + 1.5f;
					float num3 = Mathf.Sin(gradientShift + 2f) * 0.5f + 1.5f;
					float num4 = Mathf.Sin(gradientShift + 3f) * 0.5f + 1.5f;
					VertexGradient colorGradient = new VertexGradient(new Color(num, num, num), new Color(num2, num2, num2), new Color(num3, num3, num3), new Color(num4, num4, num4));
					component8.colorGradient = colorGradient;
				}
				lastGradientChange = now;
			}
			TextMeshProUGUIEx component9 = LaunchPadText.GetComponent<TextMeshProUGUIEx>();
			if (component9 != null)
			{
				SetText(component9);
			}
		}
		int childCount = buttonsQuickLinksGrid.transform.GetChildCount();
		for (int num5 = 0; num5 < childCount; num5++)
		{
			Transform child = buttonsQuickLinksGrid.transform.GetChild(num5);
			if (!(child == null))
			{
				string name = child.gameObject.name;
				if (name.Contains("Button_Worlds"))
				{
					child.localPosition = new Vector3(-348f, -25f, 0f);
				}
				else if (name.Contains("Button_Avatars"))
				{
					child.localPosition = new Vector3(-116f, -25f, 0f);
				}
				else if (name.Contains("Button_Social"))
				{
					child.localPosition = new Vector3(116f, -25f, 0f);
				}
				else if (name.Contains("Button_ViewGroups"))
				{
					child.localPosition = new Vector3(348f, -25f, 0f);
				}
				if (!ui_ready)
				{
					SetupButton(child.gameObject);
					processed_buttons.Add(name);
				}
				else if (!processed_buttons.Contains(name))
				{
					SetupButton(child.gameObject);
				}
			}
		}
		int childCount2 = buttonsQuickActionsGrid.transform.GetChildCount();
		for (int num6 = 0; num6 < childCount; num6++)
		{
			Transform child2 = buttonsQuickActionsGrid.transform.GetChild(num6);
			if (!(child2 == null))
			{
				string name2 = child2.gameObject.name;
				if (name2.Contains("Button_GoHome"))
				{
					child2.localPosition = new Vector3(-225f, -15f, 0f);
				}
				else if (name2.Contains("Button_Respawn"))
				{
					child2.localPosition = new Vector3(0f, -15f, 0f);
				}
				else if (name2.Contains("Button_SelectUser"))
				{
					child2.localPosition = new Vector3(225f, -15f, 0f);
				}
				else if (name2.Contains("SitStandCalibrateButton"))
				{
					child2.localPosition = new Vector3(225f, -15f, 0f);
				}
				if (!ui_ready)
				{
					SetupButton(child2.gameObject);
					processed_buttons.Add(name2);
				}
				else if (!processed_buttons.Contains(name2))
				{
					SetupButton(child2.gameObject);
				}
				else if (name2.Contains("SitStandCalibrateButton"))
				{
					SetupButton(child2.gameObject);
				}
			}
		}
		LinksButtons.transform.localPosition = new Vector3(0f, -100f, 0f);
		ActionsButtons.transform.localPosition = new Vector3(0f, -780f, 0f);
		GameObject gameObject7 = AssignedVariables.userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/Button_Safety").gameObject;
		gameObject7.gameObject.SetActive(value: false);
		ui_ready = true;
	}

	public static void SetText(TextMeshProUGUIEx textComponent)
	{
		textComponent.text = "";
	}

	private static void SetupButton(GameObject button, string name = "")
	{
		Transform transform = button.transform.FindChild("Background");
		if (transform != null)
		{
			RectTransform component = transform.gameObject.GetComponent<RectTransform>();
			if (component != null)
			{
				component.sizeDelta = new Vector2(0f, -80f);
			}
		}
		Transform transform2 = button.transform.FindChild("Icons");
		if (transform2 != null)
		{
			transform2.gameObject.SetActive(value: false);
		}
		Transform transform3 = button.transform.FindChild("Badge_MMJump");
		if (transform3 != null)
		{
			transform3.gameObject.SetActive(value: false);
		}
	}

	private static void InitConsole(GameObject consoleClone)
	{
		Transform transform = consoleClone.transform.FindChild("Background");
		if (transform != null)
		{
			RectTransform component = transform.gameObject.GetComponent<RectTransform>();
			if (component != null)
			{
				component.sizeDelta = new Vector2(725f, 380f);
				component.transform.localPosition = new Vector2(0f, -50f);
			}
			Image component2 = transform.gameObject.GetComponent<Image>();
			if (component2 != null)
			{
				string path = Path.Combine(Directory.GetCurrentDirectory(), "Odium", "QMConsole.png");
				Sprite overrideSprite = SpriteLoader.LoadFromDisk(path);
				component2.overrideSprite = overrideSprite;
			}
		}
		Transform transform2 = consoleClone.transform.FindChild("Icons");
		if (transform2 != null)
		{
			transform2.gameObject.SetActive(value: false);
		}
		Transform transform3 = consoleClone.transform.FindChild("Badge_MMJump");
		if (transform3 != null)
		{
			transform3.gameObject.SetActive(value: false);
		}
		Transform transform4 = consoleClone.transform.FindChild("TextLayoutParent/Text_H4");
		if (transform4 != null)
		{
			RectTransform component3 = transform4.gameObject.GetComponent<RectTransform>();
			if (component3 != null)
			{
				component3.localPosition = new Vector3(0f, 40f, 0f);
				component3.sizeDelta = new Vector2(680f, 280f);
			}
			TextMeshProUGUIEx component4 = transform4.gameObject.GetComponent<TextMeshProUGUIEx>();
			component4.alignment = TextAlignmentOptions.TopLeft;
			component4.richText = true;
			component4.enableWordWrapping = true;
			component4.fontSize = 14f;
			component4.lineSpacing = -10f;
			component4.prop_String_0 = "";
		}
		Il2CppArrayBase<ToolTip> components = consoleClone.GetComponents<ToolTip>();
		foreach (ToolTip item in components)
		{
			LocalizableString localizableString = new LocalizableString();
			localizableString._localizationKey = "[ CONSOLE ]";
			item._localizableString = localizableString;
		}
	}

	private static void ResizeButton(Transform button)
	{
		Transform transform = button.Find("Background");
		if (transform != null)
		{
			RectTransform component = transform.GetComponent<RectTransform>();
			if (component != null)
			{
				component.sizeDelta = new Vector2(0f, -90f);
			}
		}
		Transform transform2 = button.Find("Icons/Icon");
		if (transform2 != null)
		{
			transform2.localPosition -= new Vector3(0f, 50f, 0f);
			transform2.localScale = Vector3.zero;
		}
		Transform transform3 = button.Find("TextLayoutParent/Text_H4");
		if (transform3 != null)
		{
			transform3.localPosition += new Vector3(0f, 50f, 0f);
			transform3.localScale = Vector3.one;
		}
		Transform transform4 = button.Find("Badge_MMJump");
		if (transform4 != null)
		{
			transform4.localScale = Vector3.zero;
		}
	}
}
