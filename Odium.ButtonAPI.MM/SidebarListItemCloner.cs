using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Odium.ButtonAPI.MM;

public class SidebarListItemCloner
{
	private const string SIDEBAR_ITEM_PATH = "UserInterface/Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_Social/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Navigation/Viewport/VerticalLayoutGroup/Cell_MM_SidebarListItem (1)";

	private const string PARENT_CONTAINER_PATH = "UserInterface/Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_Social/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Navigation/Viewport/VerticalLayoutGroup";

	public static string VIEWPORT = "UserInterface/Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_Social/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Content/Viewport/VerticalLayoutGroup/Content";

	public static string USER_TEMPLATE = "UserInterface/Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_Social/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Content/Viewport/VerticalLayoutGroup/Content/Online_CellGrid_MM_Content/Cell_MM_User";

	public static List<string> strings = new List<string> { "Locations_Vertical_Content", "Online_CellGrid_MM_Content", "ActiveOnAnotherPlatform", "MM_Foldout_Offline", "Offline" };

	public static GameObject CreateUserCard(string username, Sprite userThumbnail)
	{
		GameObject gameObject = GameObject.Find(USER_TEMPLATE);
		gameObject.transform.SetParent(GameObject.Find(VIEWPORT).transform, worldPositionStays: false);
		gameObject.name = "ODIUM_Cell_MM_User - " + username;
		gameObject.SetActive(value: true);
		return gameObject;
	}

	public static GameObject CreateSidebarItem(string title)
	{
		GameObject original = GameObject.Find("UserInterface/Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_Social/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Navigation/Viewport/VerticalLayoutGroup/Cell_MM_SidebarListItem (1)");
		GameObject gameObject = GameObject.Find("UserInterface/Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_Social/Panel_SectionList/ScrollRect_Navigation_Container/ScrollRect_Navigation/Viewport/VerticalLayoutGroup");
		GameObject gameObject2 = UnityEngine.Object.Instantiate(original);
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		gameObject2.name = "ODIUM_Cell_MM_SidebarListItem - " + title;
		gameObject2.transform.Find("Mask/Text_Name").GetComponent<TextMeshProUGUIEx>().prop_String_0 = title;
		gameObject2.GetComponent<Button>().onClick.RemoveAllListeners();
		gameObject2.GetComponent<Button>().onClick.AddListener((Action)delegate
		{
			strings.ForEach(delegate(string str)
			{
				GameObject gameObject3 = GameObject.Find(VIEWPORT + "/" + str);
				if (gameObject3 != null)
				{
					gameObject3.SetActive(value: false);
				}
			});
			OdiumConsole.LogGradient("OWIJRFUWEHR", "Sidebar item clicked: " + title);
		});
		return gameObject2;
	}
}
