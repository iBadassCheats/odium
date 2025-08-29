using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using Odium.AwooochysResourceManagement;
using Odium.Odium;
using Odium.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.UI;
using VRC.UI.Core.Styles;

namespace Odium.ButtonAPI.QM;

public class PlayerDebugUI
{
	private const int MAX_LINES = 33;

	private const int MAX_CHARACTERS_PER_LINE = 68;

	private const int MAX_DISPLAYED_USERS = 38;

	public static GameObject label;

	public static GameObject background;

	public static TextMeshProUGUI text;

	public static List<string> messageList = new List<string>();

	private static string displayText = "";

	private static object playerListCoroutine;

	private static readonly Dictionary<string, Color> keywordColors = new Dictionary<string, Color>
	{
		{
			"Join",
			Color.green
		},
		{
			"Leave",
			Color.red
		},
		{
			"+",
			Color.green
		},
		{
			"-",
			Color.red
		},
		{
			"Debug",
			Color.yellow
		},
		{
			"Log",
			Color.magenta
		},
		{
			"Photon",
			Color.magenta
		},
		{
			"Warn",
			Color.cyan
		},
		{
			"Error",
			Color.red
		},
		{
			"RPC",
			Color.white
		}
	};

	public static void InitializeDebugMenu()
	{
		try
		{
			GameObject userInterface = AssignedVariables.userInterface;
			if (userInterface == null)
			{
				OdiumConsole.Log("DebugUI", "User interface not found");
				return;
			}
			Transform transform = userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickLinks");
			transform.gameObject.SetActive(value: true);
			transform.transform.Find("HeaderBackground").gameObject.SetActive(value: true);
			if (transform == null)
			{
				OdiumConsole.Log("DebugUI", "Dashboard header template not found");
				return;
			}
			label = UnityEngine.Object.Instantiate(transform.gameObject);
			label.SetActive(value: true);
			label.transform.SetParent(userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/Wing_Left"));
			label.transform.localPosition = new Vector3(-450f, -500f, 0f);
			label.transform.localRotation = Quaternion.identity;
			label.transform.localScale = new Vector3(1f, 1f, 1f);
			UIInvisibleGraphic component = label.GetComponent<UIInvisibleGraphic>();
			if (component != null)
			{
				component.enabled = false;
			}
			PlayerDebugUI.text = label.GetComponentInChildren<TextMeshProUGUIEx>();
			if (PlayerDebugUI.text == null)
			{
				OdiumConsole.Log("DebugUI", "Text component not found");
				return;
			}
			PlayerDebugUI.text.alignment = TextAlignmentOptions.TopLeft;
			PlayerDebugUI.text.outlineWidth = 0.2f;
			PlayerDebugUI.text.fontSize = 20f;
			PlayerDebugUI.text.fontSizeMax = 25f;
			PlayerDebugUI.text.fontSizeMin = 18f;
			PlayerDebugUI.text.richText = true;
			Transform transform2 = label.transform.Find("LeftItemContainer");
			if (transform2 != null)
			{
				LayoutGroup component2 = transform2.GetComponent<LayoutGroup>();
				if (component2 != null)
				{
					component2.enabled = false;
					OdiumConsole.Log("DebugUI", "Disabled LayoutGroup on LeftItemContainer");
				}
				ContentSizeFitter component3 = transform2.GetComponent<ContentSizeFitter>();
				if (component3 != null)
				{
					component3.enabled = false;
					OdiumConsole.Log("DebugUI", "Disabled ContentSizeFitter on LeftItemContainer");
				}
			}
			LayoutGroup component4 = label.GetComponent<LayoutGroup>();
			if (component4 != null)
			{
				component4.enabled = false;
				OdiumConsole.Log("DebugUI", "Disabled LayoutGroup on label");
			}
			if (PlayerDebugUI.text != null)
			{
				RectTransform component5 = PlayerDebugUI.text.GetComponent<RectTransform>();
				if (component5 != null)
				{
					component5.anchoredPosition = new Vector2(180f, 390f);
					OdiumConsole.Log("DebugUI", $"Set anchored position to: {component5.anchoredPosition}");
				}
			}
			Mask mask = label.AddComponent<Mask>();
			mask.showMaskGraphic = false;
			background = label.transform.Find("HeaderBackground")?.gameObject;
			if (background == null)
			{
				OdiumConsole.Log("DebugUI", "Background object not found");
				return;
			}
			background.transform.localPosition = new Vector3(0f, 0f, 0f);
			background.transform.localScale = new Vector3(0.6f, 10f, 1f);
			background.transform.localRotation = Quaternion.identity;
			background.SetActive(value: true);
			ImageEx component6 = background.GetComponent<ImageEx>();
			if ((UnityEngine.Object)(object)component6 == null)
			{
				OdiumConsole.Log("DebugUI", "Background image component not found");
				return;
			}
			string text = Path.Combine(Environment.CurrentDirectory, "Odium", "QMPlayerList.png");
			Sprite sprite = text.LoadSpriteFromDisk();
			if (sprite == null)
			{
				OdiumConsole.Log("DebugUI", "Failed to load background sprite from path: " + text);
				((Graphic)(object)component6).m_Color = new Color(0.443f, 0.133f, 1f, 1f);
			}
			else
			{
				((Image)(object)component6).overrideSprite = sprite;
			}
			StyleElement component7 = background.GetComponent<StyleElement>();
			if (component7 != null)
			{
				UnityEngine.Object.Destroy(component7);
			}
			label.SetActive(value: true);
			background.SetActive(value: true);
			StartPlayerListLoop();
			OdiumConsole.Log("DebugUI", "Debug menu positioned correctly!");
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("DebugUI", "Failed to initialize debug menu: " + ex.Message);
		}
	}

	public static void StartPlayerListLoop()
	{
		if (playerListCoroutine != null)
		{
			StopPlayerListLoop();
		}
		playerListCoroutine = MelonCoroutines.Start(PlayerListLoop());
	}

	public static void StopPlayerListLoop()
	{
		if (playerListCoroutine != null)
		{
			MelonCoroutines.Stop(playerListCoroutine);
			playerListCoroutine = null;
		}
	}

	public static void AdjustPosition(float x, float y, float z)
	{
		if (label != null)
		{
			label.transform.localPosition = new Vector3(x, y, z);
			OdiumConsole.Log("DebugUI", $"Position adjusted to: {x}, {y}, {z}");
		}
	}

	public static void AdjustBackgroundScale(float x, float y, float z)
	{
		if (background != null)
		{
			background.transform.localScale = new Vector3(x, y, z);
			OdiumConsole.Log("DebugUI", $"Background scale adjusted to: {x}, {y}, {z}");
		}
	}

	public static void FixBackgroundWidth()
	{
		if (background != null)
		{
			background.transform.localScale = new Vector3(1f, 8f, 1f);
			OdiumConsole.Log("DebugUI", "Background width adjusted");
		}
	}

	public static string ColorToHex(Color color, bool includeHash = false)
	{
		try
		{
			string text = Mathf.RoundToInt(color.r * 255f).ToString("X2");
			string text2 = Mathf.RoundToInt(color.g * 255f).ToString("X2");
			string text3 = Mathf.RoundToInt(color.b * 255f).ToString("X2");
			return includeHash ? ("#" + text + text2 + text3) : (text + text2 + text3);
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("DebugUI", "Failed to convert color to hex: " + ex.Message);
			return includeHash ? "#FFFFFF" : "FFFFFF";
		}
	}

	public static string FormatMessage(string message)
	{
		if (string.IsNullOrEmpty(message))
		{
			OdiumConsole.Log("DebugUI", "Message is null or empty");
			return string.Empty;
		}
		try
		{
			string text = message;
			if (message.Length > 68)
			{
				int num = message.LastIndexOf(' ', 68);
				if (num == -1)
				{
					num = 68;
				}
				text = message.Substring(0, num) + "\n" + FormatMessage(message.Substring(num + 1));
			}
			foreach (KeyValuePair<string, Color> keywordColor in keywordColors)
			{
				if (text.Contains(keywordColor.Key))
				{
					string text2 = "<color=" + ColorToHex(keywordColor.Value) + ">";
					text = text.Replace(keywordColor.Key, text2 + keywordColor.Key + "</color>");
				}
			}
			return text + "\n";
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("DebugUI", "Failed to format message: " + ex.Message);
			return message + "\n";
		}
	}

	public static string GetPlatformIcon(string platform)
	{
		return platform?.ToLower() switch
		{
			"standalonewindows" => "[<color=#00BFFF>PC</color>]", 
			"android" => "[<color=#32CD32>QUEST</color>]", 
			"ios" => "[<color=#FF69B4>iOS</color>]", 
			_ => "[<color=#FFFFFF>UNK</color>]", 
		};
	}

	public static IEnumerator PlayerListLoop()
	{
		while (true)
		{
			try
			{
				displayText = "";
				if (PlayerManager.prop_PlayerManager_0?.field_Private_List_1_Player_0 != null && PlayerManager.prop_PlayerManager_0.field_Private_List_1_Player_0.Count > 0)
				{
					int totalPlayerCount = PlayerManager.prop_PlayerManager_0.field_Private_List_1_Player_0.Count;
					int displayedCount = Math.Min(totalPlayerCount, 38);
					if (totalPlayerCount > 38)
					{
						displayText += $"<color=#00FF00>Players Online: {totalPlayerCount}</color> <color=#FFFF00>(Showing {displayedCount}/{totalPlayerCount})</color>\n\n";
					}
					else
					{
						displayText += $"<color=#00FF00>Players Online: {totalPlayerCount}</color>\n\n";
					}
					for (int i = 0; i < displayedCount; i++)
					{
						Player player = PlayerManager.prop_PlayerManager_0.field_Private_List_1_Player_0[i];
						if (player?.field_Private_APIUser_0 != null)
						{
							string platform = PlayerRankTextDisplay.GetPlayerPlatform(player);
							string platformText = GetPlatformIcon(platform);
							bool friend = PlayerRankTextDisplay.IsFriend(player);
							bool adult = PlayerRankTextDisplay.IsAdult(player);
							string friendText = (friend ? "<color=#FFD700>[FRIEND]</color>" : "");
							string adultText = (adult ? "<color=#90EE90>[18+]</color>" : "");
							string rankDisplay = PlayerRankTextDisplay.GetRankDisplayName(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
							Color rankColor = PlayerRankTextDisplay.GetRankColor(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
							string hexColor = PlayerRankTextDisplay.ColorToHex(rankColor);
							PlayerRankTextDisplay.GetRankDisplayName(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
							displayText = displayText + "<size=16><color=" + hexColor + ">" + player.field_Private_APIUser_0.displayName + "</color></size> [" + rankDisplay + "] <size=16>" + platformText + "</size> <size=16>" + friendText + "</color> <size=16>" + adultText + "</size>\n";
						}
					}
				}
				else
				{
					displayText = "<color=#FF0000>No players found</color>\n";
				}
				messageList = new List<string> { displayText };
				UpdateDisplay();
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				OdiumConsole.Log("DebugUI", "Error in PlayerListLoop: " + ex2.Message);
				displayText = "<color=#FF0000>Error: " + ex2.Message + "</color>\n";
				UpdateDisplay();
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private static void UpdateDisplay()
	{
		try
		{
			if (text == null)
			{
				OdiumConsole.Log("DebugUI", "Text component is null, cannot update display");
			}
			else
			{
				text.text = string.Join("", messageList);
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("DebugUI", "Failed to update display: " + ex.Message);
		}
	}

	public static void Cleanup()
	{
		StopPlayerListLoop();
		if (label != null)
		{
			UnityEngine.Object.Destroy(label);
			label = null;
		}
		background = null;
		text = null;
		messageList.Clear();
		displayText = "";
	}
}
