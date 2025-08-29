using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;

namespace Odium.UI;

public class PlayerRankTextDisplay
{
	public struct PlayerInfo
	{
		public string playerName;

		public Rank rank;

		public Player player;

		public string userId;

		public PlayerInfo(string name, Rank playerRank, Player plr, string id)
		{
			playerName = name;
			rank = playerRank;
			player = plr;
			userId = id;
		}
	}

	public enum Rank
	{
		Visitor,
		NewUser,
		User,
		Known,
		Trusted
	}

	private static GameObject canvasObject;

	private static Canvas canvas;

	private static GameObject textDisplayObject;

	private static TextMeshProUGUI textComponent;

	private static List<PlayerInfo> playerList = new List<PlayerInfo>();

	private static HashSet<string> clientUsers = new HashSet<string>();

	private static Color gradientColor1 = ColorFromHex("#D37CFE");

	private static Color gradientColor2 = ColorFromHex("#8900CE");

	private static HttpClient httpClient = new HttpClient();

	public static void Initialize()
	{
		if (!(canvasObject != null))
		{
			CreateStandaloneUI();
		}
	}

	private static void CreateStandaloneUI()
	{
		try
		{
			canvasObject = new GameObject("PlayerRankOverlayCanvas");
			UnityEngine.Object.DontDestroyOnLoad(canvasObject);
			canvas = canvasObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 999;
			CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			canvasScaler.matchWidthOrHeight = 0f;
			canvasObject.AddComponent<GraphicRaycaster>();
			textDisplayObject = new GameObject("PlayerRankText");
			textDisplayObject.transform.SetParent(canvasObject.transform, worldPositionStays: false);
			textComponent = textDisplayObject.AddComponent<TextMeshProUGUI>();
			textComponent.text = "";
			textComponent.fontSize = 18f;
			textComponent.richText = true;
			textComponent.enableAutoSizing = false;
			textComponent.alignment = TextAlignmentOptions.TopLeft;
			textComponent.verticalAlignment = VerticalAlignmentOptions.Top;
			textComponent.color = Color.white;
			textComponent.fontStyle = FontStyles.Bold;
			RectTransform component = textDisplayObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 1f);
			component.anchorMax = new Vector2(0f, 1f);
			component.pivot = new Vector2(0f, 1f);
			component.anchoredPosition = new Vector2(20f, -20f);
			component.sizeDelta = new Vector2(350f, 600f);
			Shadow shadow = textDisplayObject.AddComponent<Shadow>();
			shadow.effectColor = new Color(0f, 0f, 0f, 0.8f);
			shadow.effectDistance = new Vector2(2f, -2f);
			Outline outline = textDisplayObject.AddComponent<Outline>();
			outline.effectColor = Color.black;
			outline.effectDistance = new Vector2(1f, -1f);
			CanvasGroup canvasGroup = textDisplayObject.AddComponent<CanvasGroup>();
			canvasGroup.blocksRaycasts = false;
			canvasGroup.interactable = false;
			MelonLogger.Msg("Standalone player rank overlay created successfully");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Failed to create standalone UI: " + ex.Message);
		}
	}

	public static void AddPlayer(string playerName, APIUser apiUser, Player plr)
	{
		if (canvasObject == null)
		{
			Initialize();
		}
		try
		{
			Rank playerRank = GetPlayerRank(apiUser);
			string text = apiUser?.id ?? "";
			PlayerInfo playerInfo = new PlayerInfo(playerName, playerRank, plr, text);
			int num = playerList.FindIndex((PlayerInfo p) => p.playerName == playerName);
			if (num >= 0)
			{
				playerList[num] = playerInfo;
			}
			else
			{
				playerList.Add(playerInfo);
			}
			if (!string.IsNullOrEmpty(text))
			{
				CheckClientUserAsync(text);
			}
			UpdateDisplay();
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Failed to add player " + playerName + ": " + ex.Message);
		}
	}

	private static async void CheckClientUserAsync(string userId)
	{
		try
		{
			string url = "https://snoofz.net/api/odium/user/exists?id=" + userId;
			HttpResponseMessage response = await httpClient.GetAsync(url);
			if (response.IsSuccessStatusCode && (await response.Content.ReadAsStringAsync()).Contains("\"exists\":true"))
			{
				clientUsers.Add(userId);
				MelonLogger.Msg("Client user detected: " + userId);
			}
		}
		catch (Exception ex)
		{
			Exception e = ex;
			MelonLogger.Error("Error checking client user: " + e.Message);
		}
	}

	private static string GetAnimatedGradientText(string text, Color color1, Color color2, float speed = 3f, float waveLength = 1.5f)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		float num = Time.time * speed;
		for (int i = 0; i < text.Length; i++)
		{
			float num2 = (float)i / (float)(text.Length - 1);
			float value = num2 + (Mathf.Sin(num + (float)i * waveLength) * 0.5f + 0.5f) * 0.3f;
			value = Mathf.Clamp01(value);
			Color color3 = Color.Lerp(color1, color2, value);
			string arg = ColorToHex(color3);
			stringBuilder.Append($"<color={arg}>{text[i]}</color>");
		}
		return stringBuilder.ToString();
	}

	public static void RemovePlayer(string playerName)
	{
		try
		{
			PlayerInfo playerInfo = playerList.Find((PlayerInfo p) => p.playerName == playerName);
			if (!string.IsNullOrEmpty(playerInfo.userId))
			{
				clientUsers.Remove(playerInfo.userId);
			}
			playerList.RemoveAll((PlayerInfo p) => p.playerName == playerName);
			UpdateDisplay();
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Failed to remove player " + playerName + ": " + ex.Message);
		}
	}

	public static void ClearAll()
	{
		try
		{
			playerList.Clear();
			clientUsers.Clear();
			UpdateDisplay();
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Failed to clear player list: " + ex.Message);
		}
	}

	public static string GetPlayerPlatform(Player player)
	{
		try
		{
			APIUser field_Private_APIUser_ = player.field_Private_APIUser_0;
			if (field_Private_APIUser_?.last_platform != null)
			{
				return field_Private_APIUser_.last_platform;
			}
			return "Unknown";
		}
		catch
		{
			return "Unknown";
		}
	}

	public static string GetPlatformIcon(string platform)
	{
		return platform?.ToLower() switch
		{
			"standalonewindows" => "<size=8>[<color=#00BFFF>PC</color>]</size>", 
			"android" => "<size=8>[<color=#32CD32>QUEST</color>]</size>", 
			"ios" => "<size=8>[<color=#FF69B4>iOS</color>]</size>", 
			_ => "<size=8>[<color=#FFFFFF>UNK</color>]</size>", 
		};
	}

	public static bool IsFriend(Player player)
	{
		try
		{
			return player.field_Private_APIUser_0?.isFriend ?? false;
		}
		catch
		{
			return false;
		}
	}

	public static bool IsAdult(Player player)
	{
		try
		{
			APIUser field_Private_APIUser_ = player.field_Private_APIUser_0;
			return (field_Private_APIUser_ != null && field_Private_APIUser_.ageVerified) || (field_Private_APIUser_?.isAdult ?? false);
		}
		catch
		{
			return false;
		}
	}

	private static void UpdateDisplay()
	{
		if (textComponent == null)
		{
			return;
		}
		try
		{
			string text = "";
			if (playerList.Count > 0)
			{
				foreach (PlayerInfo player in playerList)
				{
					string playerPlatform = GetPlayerPlatform(player.player);
					string platformIcon = GetPlatformIcon(playerPlatform);
					bool flag = IsFriend(player.player);
					bool flag2 = IsAdult(player.player);
					bool flag3 = clientUsers.Contains(player.userId);
					string text2 = (flag ? "<size=8><color=#FFD700>[FRIEND]</color></size>" : "");
					string text3 = (flag2 ? "<size=8><color=#90EE90>[18+]</color></size>" : "");
					if (flag3)
					{
						string animatedGradientText = GetAnimatedGradientText(player.playerName, gradientColor1, gradientColor2);
						string text4 = "<size=8><color=#FF1493>[CLIENT]</color></size>";
						text = text + "<size=12>" + animatedGradientText + "</size> " + text4 + " " + platformIcon + " " + text2 + " " + text3 + "\n";
					}
					else
					{
						Color rankColor = GetRankColor(player.rank);
						string text5 = ColorToHex(rankColor);
						string rankDisplayName = GetRankDisplayName(player.rank);
						text = text + "<size=12><color=" + text5 + ">" + player.playerName + "</color></size> <size=8><color=#CCCCCC>[" + rankDisplayName + "]</color></size> " + platformIcon + " " + text2 + " " + text3 + "\n";
					}
				}
			}
			textComponent.text = text;
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Failed to update display: " + ex.Message);
		}
	}

	public static string GetRankDisplayName(Rank rank)
	{
		return rank switch
		{
			Rank.Visitor => "Visitor", 
			Rank.NewUser => "New User", 
			Rank.User => "User", 
			Rank.Known => "Known User", 
			Rank.Trusted => "Trusted User", 
			_ => "Unknown", 
		};
	}

	public static Rank GetPlayerRank(APIUser apiUser)
	{
		if (apiUser.hasLegendTrustLevel || apiUser.hasVeteranTrustLevel)
		{
			return Rank.Trusted;
		}
		if (apiUser.hasTrustedTrustLevel)
		{
			return Rank.Known;
		}
		if (apiUser.hasKnownTrustLevel)
		{
			return Rank.User;
		}
		if (apiUser.hasBasicTrustLevel)
		{
			return Rank.NewUser;
		}
		return Rank.Visitor;
	}

	public static Color GetRankColor(Rank rank)
	{
		return rank switch
		{
			Rank.Visitor => new Color(1f, 1f, 1f, 0.9f), 
			Rank.NewUser => ColorFromHex("#96ECFF", 0.9f), 
			Rank.User => ColorFromHex("#96FFA9", 0.9f), 
			Rank.Known => ColorFromHex("#FF5E50", 0.9f), 
			Rank.Trusted => ColorFromHex("#A900FE", 0.9f), 
			_ => new Color(1f, 1f, 1f, 0.9f), 
		};
	}

	public static Color ColorFromHex(string hex, float alpha = 1f)
	{
		if (hex.StartsWith("#"))
		{
			hex = hex.Substring(1);
		}
		if (hex.Length == 6)
		{
			float r = (float)int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) / 255f;
			float g = (float)int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber) / 255f;
			float b = (float)int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber) / 255f;
			return new Color(r, g, b, alpha);
		}
		return Color.white;
	}

	public static string ColorToHex(Color color)
	{
		int num = Mathf.RoundToInt(color.r * 255f);
		int num2 = Mathf.RoundToInt(color.g * 255f);
		int num3 = Mathf.RoundToInt(color.b * 255f);
		return $"#{num:X2}{num2:X2}{num3:X2}";
	}

	public static void SetPosition(float x, float y)
	{
		if (textDisplayObject != null)
		{
			RectTransform component = textDisplayObject.GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(x, y);
		}
	}

	public static void SetFontSize(float size)
	{
		if (textComponent != null)
		{
			textComponent.fontSize = size;
		}
	}

	public static void SetVisible(bool visible)
	{
		if (canvasObject != null)
		{
			canvasObject.SetActive(visible);
		}
	}

	public static void SetOpacity(float opacity)
	{
		if (textComponent != null)
		{
			Color color = textComponent.color;
			color.a = opacity;
			textComponent.color = color;
		}
	}

	public static void Destroy()
	{
		if (canvasObject != null)
		{
			UnityEngine.Object.DestroyImmediate(canvasObject);
			canvasObject = null;
			canvas = null;
			textDisplayObject = null;
			textComponent = null;
			playerList.Clear();
			clientUsers.Clear();
			httpClient?.Dispose();
			MelonLogger.Msg("Player rank overlay destroyed");
		}
	}
}
