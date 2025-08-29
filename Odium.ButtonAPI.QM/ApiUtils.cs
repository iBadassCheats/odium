using System;
using System.IO;
using Odium.Components;
using Odium.Odium;
using Odium.Wrappers;
using UnityEngine;
using VRC;
using VRC.UI.Elements;

namespace Odium.ButtonAPI.QM;

public static class ApiUtils
{
	public const string Identifier = "Odium";

	public static readonly System.Random random = new System.Random();

	private static QuickMenu _quickMenu;

	private static MainMenu _socialMenu;

	private static GameObject _selectedUserPageGrid;

	private static GameObject _qmMenuTemplate;

	private static GameObject _qmTabTemplate;

	private static GameObject _qmButtonTemplate;

	private static Sprite _onSprite;

	private static Sprite _offSprite;

	public static QuickMenu QuickMenu
	{
		get
		{
			if (_quickMenu == null)
			{
				_quickMenu = Resources.FindObjectsOfTypeAll<QuickMenu>()[0];
			}
			return _quickMenu;
		}
	}

	public static MainMenu MainMenu
	{
		get
		{
			if (_socialMenu == null)
			{
				_socialMenu = Resources.FindObjectsOfTypeAll<MainMenu>()[0];
			}
			return _socialMenu;
		}
	}

	public static Player GetPlayerByDisplayName(string name)
	{
		return PlayerWrapper.GetAllPlayers().Find((Player plr) => plr.field_Private_APIUser_0.displayName == name);
	}

	public static Player GetIUser()
	{
		GameObject gameObject = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/UserProfile_Compact/PanelBG/Info/Text_Username_Friend");
		gameObject.SetActive(value: true);
		if (gameObject == null)
		{
			return null;
		}
		TextMeshProUGUIEx component = gameObject.GetComponent<TextMeshProUGUIEx>();
		if (component == null)
		{
			return null;
		}
		Player playerByDisplayName = GetPlayerByDisplayName(component.text);
		gameObject.SetActive(value: false);
		return playerByDisplayName;
	}

	public static string GetMMIUser()
	{
		Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_UserDetail/Header_MM_UserName/LeftItemContainer/Text_Title");
		if (transform == null)
		{
			OdiumConsole.Log("Odium", "Text object not found in GetMMIUser method", LogLevel.Error);
			return null;
		}
		TextMeshProUGUIEx component = transform.GetComponent<TextMeshProUGUIEx>();
		if (component == null)
		{
			OdiumConsole.Log("Odium", "Text component not found in GetMMIUser method", LogLevel.Error);
			return null;
		}
		return component.prop_String_0;
	}

	public static string GetMMWorldName()
	{
		Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_MM_WorldInformation/Header_MM_H1/Text_WorldName");
		if (transform == null)
		{
			OdiumConsole.Log("Odium", "Text object not found in GetMMWorldName method", LogLevel.Error);
			return null;
		}
		TextMeshProUGUIEx component = transform.GetComponent<TextMeshProUGUIEx>();
		if (component == null)
		{
			OdiumConsole.Log("Odium", "Text component not found in GetMMWorldName method", LogLevel.Error);
			return null;
		}
		return component.prop_String_0;
	}

	public static string GetFPS()
	{
		Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_Settings/Menu_MM_DynamicSidePanel/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Content/Viewport/VerticalLayoutGroup/Debug/Debug/Settings_Panel_1/VerticalLayoutGroup/DebugStats/LeftItemContainer/Cell_MM_SettingStat (1)/Text_Detail_Original");
		if (transform == null)
		{
			return null;
		}
		TextMeshProUGUIEx component = transform.GetComponent<TextMeshProUGUIEx>();
		if (component == null)
		{
			return null;
		}
		return component.prop_String_0;
	}

	public static string GetPing()
	{
		Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_Settings/Menu_MM_DynamicSidePanel/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Content/Viewport/VerticalLayoutGroup/Debug/Debug/Settings_Panel_1/VerticalLayoutGroup/DebugStats/LeftItemContainer/Cell_MM_SettingStat (2)/Text_Detail_Original");
		if (transform == null)
		{
			return null;
		}
		TextMeshProUGUIEx component = transform.GetComponent<TextMeshProUGUIEx>();
		if (component == null)
		{
			return null;
		}
		return component.prop_String_0;
	}

	public static string GetBuild()
	{
		Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_Settings/Menu_MM_DynamicSidePanel/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Content/Viewport/VerticalLayoutGroup/Debug/Debug/Settings_Panel_1/VerticalLayoutGroup/DebugStats/LeftItemContainer/Cell_MM_SettingStat (3)/Text_Detail_Original");
		if (transform == null)
		{
			return null;
		}
		TextMeshProUGUIEx component = transform.GetComponent<TextMeshProUGUIEx>();
		if (component == null)
		{
			return null;
		}
		return component.prop_String_0;
	}

	public static GameObject GetSelectedUserPageGrid()
	{
		if (_selectedUserPageGrid == null)
		{
			_selectedUserPageGrid = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_UserActions");
			return _selectedUserPageGrid;
		}
		return _selectedUserPageGrid;
	}

	public static GameObject GetQMMenuTemplate()
	{
		if (_qmMenuTemplate == null)
		{
			_qmMenuTemplate = QuickMenu.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Dashboard").gameObject;
		}
		return _qmMenuTemplate;
	}

	public static GameObject GetQMTabButtonTemplate()
	{
		if (_qmTabTemplate == null)
		{
			_qmTabTemplate = QuickMenu.transform.Find("CanvasGroup/Container/Window/Page_Buttons_QM/HorizontalLayoutGroup/Page_Settings").gameObject;
		}
		return _qmTabTemplate;
	}

	public static GameObject GetQMButtonTemplate()
	{
		if (_qmButtonTemplate == null)
		{
			_qmButtonTemplate = QuickMenu.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Here/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_WorldActions/Button_RejoinWorld").gameObject;
		}
		return _qmButtonTemplate;
	}

	public static GameObject GetQMSmalltTemplate()
	{
		if (_qmButtonTemplate == null)
		{
			_qmButtonTemplate = QuickMenu.transform.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/Header_H1/RightItemContainer/Button_QM_Report/").gameObject;
		}
		return _qmButtonTemplate;
	}

	public static Sprite OnIconSprite()
	{
		return SpriteUtil.LoadFromDisk(Path.Combine(Environment.CurrentDirectory, "Odium", "ToggleSwitchOn.png"));
	}

	public static Sprite OffIconSprite()
	{
		return SpriteUtil.LoadFromDisk(Path.Combine(Environment.CurrentDirectory, "Odium", "ToggleSwitchOff.png"));
	}

	public static int RandomNumbers()
	{
		return random.Next(100000, 999999);
	}

	public static string GetSelectedPageName()
	{
		return QuickMenu.prop_MenuStateController_0.field_Private_UIPage_0.field_Public_String_0;
	}
}
