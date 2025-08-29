using System;
using System.IO;
using Odium.Components;
using Odium.UX;
using Odium.Wrappers;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.Localization;

public class SelectedUser
{
	public static bool ui_ready;

	public static GameObject PageGrid;

	public static string get_selected_player_name()
	{
		GameObject gameObject = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/UserProfile_Compact/PanelBG/Info/Text_Username_NonFriend");
		if (gameObject == null)
		{
			return "";
		}
		TextMeshProUGUIEx component = gameObject.GetComponent<TextMeshProUGUIEx>();
		if (component == null)
		{
			return "";
		}
		return component.text;
	}

	public static GameObject CreateButton(GameObject parentGrid, string title, Action<Player> OnClick, string tooltip = "", bool DisplayIcon = false, Sprite sprite = null)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(MainMenu.ExampleButton, parentGrid.transform);
		gameObject.name = $"Button_{Guid.NewGuid()}";
		Transform transform = gameObject.transform.FindChild("TextLayoutParent/Text_H4");
		if (transform != null)
		{
			TextMeshProUGUIEx component = transform.gameObject.GetComponent<TextMeshProUGUIEx>();
			component.richText = true;
			component.prop_String_0 = title;
		}
		Transform transform2 = gameObject.transform.FindChild("Badge_MMJump");
		if (transform2 != null)
		{
			transform2.gameObject.SetActive(value: false);
		}
		Il2CppArrayBase<ToolTip> components = gameObject.GetComponents<ToolTip>();
		foreach (ToolTip item in components)
		{
			LocalizableString localizableString = new LocalizableString();
			localizableString._localizationKey = tooltip;
			item._localizableString = localizableString;
		}
		VRCButtonHandle component2 = gameObject.GetComponent<VRCButtonHandle>();
		if (component2 != null)
		{
			component2._sendAnalytics = false;
			component2.m_OnClick.RemoveAllListeners();
			component2.onClick.RemoveAllListeners();
			component2.onClick.AddListener((Action)delegate
			{
				string selected_player_name = get_selected_player_name();
				if (!string.IsNullOrEmpty(selected_player_name))
				{
					try
					{
						Player player = null;
						foreach (Player player2 in PlayerWrapper.Players)
						{
							if (!(player2 == null))
							{
								APIUser field_Private_APIUser_ = player2.field_Private_APIUser_0;
								if (field_Private_APIUser_ != null && field_Private_APIUser_.displayName == selected_player_name)
								{
									player = player2;
									break;
								}
							}
						}
						if (player != null)
						{
							OnClick?.Invoke(player);
						}
					}
					catch
					{
					}
				}
			});
		}
		Transform transform3 = gameObject.transform.FindChild("Icons");
		if (transform3 != null)
		{
			transform3.gameObject.SetActive(DisplayIcon);
			if (DisplayIcon && sprite != null)
			{
				Transform transform4 = transform3.FindChild("Icon");
				if (transform4 != null)
				{
					GameObject gameObject2 = transform4.gameObject;
					Image component3 = gameObject2.GetComponent<Image>();
					if (component3 != null)
					{
						component3.overrideSprite = sprite;
					}
				}
			}
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	public static GameObject CreateToggle(GameObject parentGrid, string title, Action<Player> OnEnable, Action<Player> OnDisable, string tooltip = "", bool DisplayIcon = false, Sprite sprite = null)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(MainMenu.ExampleButton, parentGrid.transform);
		gameObject.name = $"Button_{Guid.NewGuid()}";
		Transform transform = gameObject.transform.FindChild("TextLayoutParent/Text_H4");
		if (transform != null)
		{
			TextMeshProUGUIEx component = transform.gameObject.GetComponent<TextMeshProUGUIEx>();
			component.richText = true;
			component.prop_String_0 = title;
		}
		Il2CppArrayBase<ToolTip> components = gameObject.GetComponents<ToolTip>();
		foreach (ToolTip item in components)
		{
			LocalizableString localizableString = new LocalizableString();
			localizableString._localizationKey = tooltip;
			item._localizableString = localizableString;
		}
		Transform transform2 = gameObject.transform.FindChild("Badge_MMJump");
		CanvasRenderer badgeRenderer = transform2.GetComponent<CanvasRenderer>();
		VRCButtonHandle component2 = gameObject.GetComponent<VRCButtonHandle>();
		if (component2 != null)
		{
			badgeRenderer.SetColor(new Color(1f, 0f, 0f, 1f));
			component2._sendAnalytics = false;
			component2.m_OnClick.RemoveAllListeners();
			component2.onClick.RemoveAllListeners();
			component2.onClick.AddListener((Action)delegate
			{
				string playerName = get_selected_player_name();
				if (!string.IsNullOrEmpty(playerName))
				{
					try
					{
						Player player = PlayerWrapper.Players.Find((Player plr) => plr?.field_Private_APIUser_0?.displayName == playerName);
						if (player != null)
						{
							bool flag = ToggleStateManager.ToggleStates.ContainsKey(playerName) && ToggleStateManager.ToggleStates[playerName];
							bool flag2 = !flag;
							ToggleStateManager.ToggleStates[playerName] = flag2;
							if (flag2)
							{
								badgeRenderer.SetColor(new Color(0f, 1f, 0f, 1f));
								OnEnable?.Invoke(player);
							}
							else
							{
								badgeRenderer.SetColor(new Color(1f, 0f, 0f, 1f));
								OnDisable?.Invoke(player);
							}
						}
					}
					catch
					{
					}
				}
			});
		}
		Transform transform3 = gameObject.transform.FindChild("Icons");
		if (transform3 != null)
		{
			transform3.gameObject.SetActive(DisplayIcon);
			if (DisplayIcon && sprite != null)
			{
				Transform transform4 = transform3.FindChild("Icon");
				if (transform4 != null)
				{
					GameObject gameObject2 = transform4.gameObject;
					Image component3 = gameObject2.GetComponent<Image>();
					if (component3 != null)
					{
						component3.overrideSprite = sprite;
					}
				}
			}
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	public static void Setup()
	{
		if (ui_ready)
		{
			return;
		}
		string path = Path.Combine(Directory.GetCurrentDirectory(), "Odium", "ButtonBackground.png");
		Sprite sprite = SpriteUtil.LoadFromDisk(path);
		if (!(MainMenu.ExampleButton == null))
		{
			if (PageGrid == null)
			{
				PageGrid = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_UserActions");
			}
			else
			{
				ui_ready = true;
			}
		}
	}
}
