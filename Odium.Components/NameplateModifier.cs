using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using MelonLoader;
using Newtonsoft.Json;
using Odium.Odium;
using Odium.Patches;
using Odium.Wrappers;
using TMPro;
using UnhollowerBaseLib;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace Odium.Components;

public static class NameplateModifier
{
	private static List<NameplateData> playerStats;

	private static HttpClient httpClient;

	private static string API_BASE;

	private static Dictionary<string, List<string>> tagCache;

	private static HashSet<string> clientUsers;

	private static string CLIENT_API_BASE;

	private static bool autoRefreshEnabled;

	private static float lastRefreshTime;

	private static readonly float REFRESH_INTERVAL;

	public static void ModifyPlayerNameplate(Player player, Sprite newDevCircleSprite = null)
	{
		try
		{
			APIUser aPIUser = player.prop_APIUser_0;
			Transform transform = player._vrcplayer.field_Public_GameObject_0.transform.FindChild("PlayerNameplate/Canvas");
			if (transform == null)
			{
				MelonLogger.Warning("Could not find PlayerNameplate/Canvas");
				return;
			}
			CleanupPlayerStats(aPIUser.id);
			Rank playerRank = GetPlayerRank(aPIUser);
			DestroyIconIfEnabled(transform);
			DisableBackground(transform);
			if (newDevCircleSprite != null)
			{
				ChangeDevCircleSprite(transform, newDevCircleSprite, playerRank);
			}
			else
			{
				ApplyRankColoring(transform, playerRank);
			}
			SetupStatsComponent(transform);
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error in ModifyPlayerNameplate: " + ex.Message);
		}
	}

	private static IEnumerator AddStatsToNameplateCoroutine(Player player, Transform playerNameplateCanvas)
	{
		yield return new WaitForSeconds(0.1f);
		try
		{
			Transform nameplateGroup = playerNameplateCanvas.FindChild("NameplateGroup/Nameplate");
			if (nameplateGroup == null)
			{
				MelonLogger.Warning("Could not find NameplateGroup/Nameplate");
				yield break;
			}
			Transform quickStats = nameplateGroup.FindChild("Contents/Quick Stats");
			if (quickStats == null)
			{
				MelonLogger.Warning("Could not find Quick Stats to clone");
				yield break;
			}
			string userId = player.field_Private_APIUser_0.id;
			CleanupPlayerStats(userId);
			List<TextMeshProUGUI> statsComponents = new List<TextMeshProUGUI>();
			List<Transform> tagPlates = new List<Transform>();
			Transform mainStatsTransform = CreateStatsPlate(quickStats, nameplateGroup, "Player Stats Info", 0);
			if (mainStatsTransform != null)
			{
				TextMeshProUGUI mainStatsComponent = SetupStatsComponent(mainStatsTransform);
				if (mainStatsComponent != null)
				{
					statsComponents.Add(mainStatsComponent);
					tagPlates.Add(mainStatsTransform);
				}
			}
			NameplateData statsData = new NameplateData
			{
				userId = userId,
				statsComponents = statsComponents,
				tagPlates = tagPlates,
				lastSeen = Time.time,
				lastPosition = player.transform.position,
				platform = GetPlayerPlatform(player),
				userTags = new List<string>(),
				isClientUser = false
			};
			playerStats.Add(statsData);
			UpdateSinglePlayerStats(player, statsData);
			MelonCoroutines.Start(CheckClientUserCoroutine(userId));
			MelonCoroutines.Start(FetchAndApplyTagsCoroutine(userId, quickStats, nameplateGroup));
			MelonLogger.Msg("Added basic stats display for player: " + player.field_Private_APIUser_0.displayName);
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			MelonLogger.Error("Error in AddStatsToNameplateCoroutine: " + ex2.Message);
		}
	}

	private static IEnumerator CheckClientUserCoroutine(string userId)
	{
		return null;
	}

	[DebuggerStepThrough]
	private static Task<bool> CheckClientUserAsync(string userId)
	{
		return null;
	}

	private static IEnumerator FetchAndApplyTagsCoroutine(string userId, Transform quickStats, Transform nameplateGroup)
	{
		return null;
	}

	private static void ApplyTagsToNameplate(string userId, List<string> userTags, Transform quickStats, Transform nameplateGroup)
	{
		try
		{
			int num = playerStats.FindIndex((NameplateData s) => s.userId == userId);
			if (num == -1)
			{
				return;
			}
			NameplateData value = playerStats[num];
			if (userId == PlayerWrapper.LocalPlayer.field_Private_APIUser_0.id)
			{
				AssignedVariables.playerTagsCount = value.tagPlates.Count;
			}
			if (value.tagPlates.Count > 1)
			{
				for (int num2 = value.tagPlates.Count - 1; num2 >= 1; num2--)
				{
					try
					{
						if (value.tagPlates[num2] != null && value.tagPlates[num2].gameObject != null)
						{
							UnityEngine.Object.Destroy(value.tagPlates[num2].gameObject);
						}
					}
					catch (Exception ex)
					{
						MelonLogger.Warning("Error destroying old tag plate: " + ex.Message);
					}
				}
				value.tagPlates.RemoveRange(1, value.tagPlates.Count - 1);
				value.statsComponents.RemoveRange(1, value.statsComponents.Count - 1);
			}
			bool flag = PhotonPatches.HasPlayerCrashed(userId);
			int num3 = 1;
			if (flag)
			{
				Transform transform = CreateStatsPlate(quickStats, nameplateGroup, "Crash Tag", num3);
				if (transform != null)
				{
					TextMeshProUGUI textMeshProUGUI = SetupStatsComponent(transform);
					if (textMeshProUGUI != null)
					{
						textMeshProUGUI.text = "<color=#e91f42>CRASHED</color>";
						value.statsComponents.Add(textMeshProUGUI);
						value.tagPlates.Add(transform);
					}
				}
				num3++;
			}
			for (int num4 = 0; num4 < userTags.Count; num4++)
			{
				Transform transform2 = CreateStatsPlate(quickStats, nameplateGroup, $"Tag Stats {num4}", num3 + num4);
				if (transform2 != null)
				{
					TextMeshProUGUI textMeshProUGUI2 = SetupStatsComponent(transform2);
					if (textMeshProUGUI2 != null)
					{
						textMeshProUGUI2.text = "<color=#e91e63>" + userTags[num4] + "</color>";
						value.statsComponents.Add(textMeshProUGUI2);
						value.tagPlates.Add(transform2);
					}
				}
			}
			value.userTags = userTags;
			playerStats[num] = value;
			bool flag2 = clientUsers.Contains(userId) || value.isClientUser;
			int num5 = userTags.Count + (flag ? 1 : 0);
			MelonLogger.Msg($"Applied {num5} tags for player: {userId} (Client: {flag2}, Crashed: {flag})");
		}
		catch (Exception ex2)
		{
			MelonLogger.Error("Error applying tags to nameplate: " + ex2.Message);
		}
	}

	public static string ColorToHex(Color color)
	{
		int num = Mathf.RoundToInt(color.r * 255f);
		int num2 = Mathf.RoundToInt(color.g * 255f);
		int num3 = Mathf.RoundToInt(color.b * 255f);
		return $"#{num:X2}{num2:X2}{num3:X2}";
	}

	public static void CheckAndRefreshTags()
	{
		if (autoRefreshEnabled)
		{
			_ = Time.time - lastRefreshTime >= REFRESH_INTERVAL;
		}
	}

	private static IEnumerator RefreshAllTagsCoroutine()
	{
		MelonLogger.Msg("Starting auto-refresh of all player tags...");
		List<NameplateData> playersToRefresh = new List<NameplateData>(playerStats);
		foreach (NameplateData statsData in playersToRefresh)
		{
			if (string.IsNullOrEmpty(statsData.userId))
			{
				continue;
			}
			Player player = GetPlayerById(statsData.userId);
			if (player == null)
			{
				continue;
			}
			GameObject nameplateContainer = player._vrcplayer.field_Public_GameObject_0;
			Transform playerNameplateCanvas = nameplateContainer.transform.FindChild("PlayerNameplate/Canvas");
			if (playerNameplateCanvas == null)
			{
				continue;
			}
			Transform nameplateGroup = playerNameplateCanvas.FindChild("NameplateGroup/Nameplate");
			if (!(nameplateGroup == null))
			{
				Transform quickStats = nameplateGroup.FindChild("Contents/Quick Stats");
				if (!(quickStats == null))
				{
					MelonCoroutines.Start(FetchAndApplyTagsCoroutine(statsData.userId, quickStats, nameplateGroup));
					yield return new WaitForSeconds(0.1f);
				}
			}
		}
		tagCache.Clear();
		MelonLogger.Msg($"Initiated tag refresh for {playersToRefresh.Count} players");
	}

	private static Player GetPlayerById(string userId)
	{
		try
		{
			if (PlayerManager.prop_PlayerManager_0?.field_Private_List_1_Player_0 == null)
			{
				return null;
			}
			Il2CppArrayBase<Player> source = PlayerManager.prop_PlayerManager_0.field_Private_List_1_Player_0.ToArray();
			return source.FirstOrDefault((Player p) => p?.field_Private_APIUser_0?.id == userId);
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error getting player by ID " + userId + ": " + ex.Message);
			return null;
		}
	}

	public static void EnableAutoRefresh()
	{
		autoRefreshEnabled = true;
		MelonLogger.Msg("Auto-refresh enabled");
	}

	public static void DisableAutoRefresh()
	{
		autoRefreshEnabled = false;
		MelonLogger.Msg("Auto-refresh disabled");
	}

	public static void SetRefreshInterval(float seconds)
	{
		if (seconds > 0f)
		{
			typeof(NameplateModifier).GetField("REFRESH_INTERVAL", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, seconds);
			MelonLogger.Msg($"Refresh interval set to {seconds} seconds");
		}
	}

	public static void ManualRefreshAllTags()
	{
		MelonCoroutines.Start(RefreshAllTagsCoroutine());
	}

	public static void CheckClientStatus(string userId)
	{
	}

	public static void ClearClientCache()
	{
		clientUsers.Clear();
		MelonLogger.Msg("Cleared client user cache");
	}

	private static Transform CreateStatsPlate(Transform quickStats, Transform nameplateGroup, string plateName, int stackIndex)
	{
		try
		{
			Transform transform = UnityEngine.Object.Instantiate(quickStats, nameplateGroup.FindChild("Contents"));
			if (transform == null)
			{
				MelonLogger.Warning("Failed to instantiate " + plateName + " transform");
				return null;
			}
			transform.name = plateName;
			transform.gameObject.SetActive(value: true);
			float y = 180f + (float)stackIndex * 30f;
			transform.localPosition = new Vector3(0f, y, 0f);
			Transform transform2 = transform.FindChild("Trust Icon");
			if (transform2 != null)
			{
				transform2.gameObject.SetActive(value: false);
			}
			Transform transform3 = transform.FindChild("Performance Icon");
			if (transform3 != null)
			{
				transform3.gameObject.SetActive(value: false);
			}
			Transform transform4 = transform.FindChild("Performance Text");
			if (transform4 != null)
			{
				transform4.gameObject.SetActive(value: false);
			}
			Transform transform5 = transform.FindChild("Friend Anchor Stats");
			if (transform5 != null)
			{
				transform5.gameObject.SetActive(value: false);
			}
			ImageThreeSlice component = transform.GetComponent<ImageThreeSlice>();
			if (component != null)
			{
				if (stackIndex == 0)
				{
					component.color = new Color(0f, 0f, 0f, 0.6f);
				}
				else
				{
					component.color = new Color(0.9f, 0.1f, 0.4f, 0.3f);
				}
			}
			return transform;
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error creating stats plate " + plateName + ": " + ex.Message);
			return null;
		}
	}

	private static TextMeshProUGUI SetupStatsComponent(Transform statsTransform)
	{
		try
		{
			Transform transform = statsTransform.FindChild("Trust Text");
			if (transform == null)
			{
				MelonLogger.Warning("Could not find Trust Text component");
				return null;
			}
			TextMeshProUGUI component = transform.GetComponent<TextMeshProUGUI>();
			if (component == null)
			{
				MelonLogger.Warning("Could not get TextMeshProUGUI component");
				return null;
			}
			component.color = Color.white;
			component.fontSize = 12f;
			component.fontStyle = FontStyles.Bold;
			return component;
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error setting up stats component: " + ex.Message);
			return null;
		}
	}

	private static async Task<List<string>> GetUserTags(string userId)
	{
		try
		{
			using HttpClient client = new HttpClient();
			client.Timeout = TimeSpan.FromSeconds(5.0);
			TagResponse tagResponse = JsonConvert.DeserializeObject<TagResponse>(await client.GetStringAsync(API_BASE + "/get?userId=" + userId));
			if (tagResponse.success && tagResponse.tags != null)
			{
				return tagResponse.tags;
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			MelonLogger.Warning("Failed to get tags for user " + userId + ": " + ex2.Message);
		}
		return new List<string>();
	}

	public static void UpdatePlayerStats()
	{
		try
		{
			CheckAndRefreshTags();
			if (PlayerManager.prop_PlayerManager_0?.field_Private_List_1_Player_0 == null)
			{
				return;
			}
			Il2CppArrayBase<Player> il2CppArrayBase = PlayerManager.prop_PlayerManager_0.field_Private_List_1_Player_0.ToArray();
			foreach (Player player in il2CppArrayBase)
			{
				if (player?.field_Private_APIUser_0?.id == null)
				{
					continue;
				}
				int num = playerStats.FindIndex((NameplateData s) => s.userId == player.field_Private_APIUser_0.id);
				if (num == -1)
				{
					continue;
				}
				NameplateData statsData = playerStats[num];
				if (statsData.statsComponents == null || statsData.statsComponents.Count == 0)
				{
					continue;
				}
				if (!ValidateStatsComponents(statsData))
				{
					MelonLogger.Warning("Invalid stats components detected for player " + player.field_Private_APIUser_0.displayName + ", cleaning up...");
					CleanupPlayerStats(player.field_Private_APIUser_0.id);
					continue;
				}
				bool flag = PhotonPatches.HasPlayerCrashed(player.field_Private_APIUser_0.id);
				bool flag2 = clientUsers.Contains(player.field_Private_APIUser_0.id) || statsData.isClientUser;
				bool flag3 = false;
				for (int num2 = 1; num2 < statsData.statsComponents.Count; num2++)
				{
					if (statsData.statsComponents[num2] != null && statsData.statsComponents[num2].text.Contains("CRASHED"))
					{
						flag3 = true;
					}
				}
				if (flag != flag3)
				{
					GameObject field_Public_GameObject_ = player._vrcplayer.field_Public_GameObject_0;
					Transform transform = field_Public_GameObject_.transform.FindChild("PlayerNameplate/Canvas");
					if (transform != null)
					{
						Transform transform2 = transform.FindChild("NameplateGroup/Nameplate");
						if (transform2 != null)
						{
							Transform transform3 = transform2.FindChild("Contents/Quick Stats");
							if (transform3 != null)
							{
								List<string> userTags = (tagCache.ContainsKey(player.field_Private_APIUser_0.id) ? tagCache[player.field_Private_APIUser_0.id] : (statsData.userTags ?? new List<string>()));
								ApplyTagsToNameplate(player.field_Private_APIUser_0.id, userTags, transform3, transform2);
							}
						}
					}
				}
				UpdateSinglePlayerStats(player, statsData);
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error in UpdatePlayerStats: " + ex.Message);
		}
	}

	private static bool ValidateStatsComponents(NameplateData statsData)
	{
		try
		{
			if (statsData.statsComponents.Count > 0 && statsData.statsComponents[0] != null)
			{
				string text = statsData.statsComponents[0].text;
				return true;
			}
		}
		catch (Exception)
		{
			return false;
		}
		return false;
	}

	private static void UpdateSinglePlayerStats(Player player, NameplateData statsData)
	{
		try
		{
			statsData.lastSeen = Time.time;
			statsData.lastPosition = player.transform.position;
			if (statsData.statsComponents.Count > 0 && statsData.statsComponents[0] != null)
			{
				List<string> list = new List<string>();
				string platformIcon = GetPlatformIcon(statsData.platform);
				if (!string.IsNullOrEmpty(platformIcon))
				{
					list.Add(platformIcon);
				}
				if (clientUsers.Contains(statsData.userId) || statsData.isClientUser)
				{
					list.Add("[<color=#e91f42>C</color>]");
				}
				VRCPlayerApi field_Private_VRCPlayerApi_ = player.field_Private_VRCPlayerApi_0;
				if (field_Private_VRCPlayerApi_ != null && field_Private_VRCPlayerApi_.isMaster)
				{
					list.Add("[<color=#FFD700>M</color>]");
				}
				if (IsFriend(player))
				{
					list.Add("[<color=#FF69B4>F</color>]");
				}
				if (IsAdult(player))
				{
					list.Add("[<color=#9966FF>18+</color>]");
				}
				string avatarReleaseStatus = GetAvatarReleaseStatus(player);
				list.Add("[<color=#9966FF>" + avatarReleaseStatus + "</color>]");
				string text = string.Join(" | ", list);
				statsData.statsComponents[0].text = text;
			}
			int num = playerStats.FindIndex((NameplateData s) => s.userId == statsData.userId);
			if (num != -1)
			{
				playerStats[num] = statsData;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error updating stats for player " + player.field_Private_APIUser_0.displayName + ": " + ex.Message);
			CleanupPlayerStats(player.field_Private_APIUser_0.id);
		}
	}

	public static void CleanupPlayerStats(string userId)
	{
		try
		{
			int num = playerStats.FindIndex((NameplateData s) => s.userId == userId);
			if (num == -1)
			{
				return;
			}
			NameplateData nameplateData = playerStats[num];
			if (nameplateData.tagPlates != null)
			{
				foreach (Transform tagPlate in nameplateData.tagPlates)
				{
					try
					{
						if (tagPlate != null && tagPlate.gameObject != null)
						{
							UnityEngine.Object.Destroy(tagPlate.gameObject);
						}
					}
					catch (Exception ex)
					{
						MelonLogger.Warning("Error destroying tag plate: " + ex.Message);
					}
				}
			}
			playerStats.RemoveAt(num);
			clientUsers.Remove(userId);
		}
		catch (Exception ex2)
		{
			MelonLogger.Error("Error cleaning up stats for user " + userId + ": " + ex2.Message);
		}
	}

	public static void ClearTagCache(string userId = null)
	{
		try
		{
			if (string.IsNullOrEmpty(userId))
			{
				tagCache.Clear();
				MelonLogger.Msg("Cleared all tag cache");
			}
			else if (tagCache.ContainsKey(userId))
			{
				tagCache.Remove(userId);
				MelonLogger.Msg("Cleared tag cache for user: " + userId);
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error clearing tag cache: " + ex.Message);
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

	private static string GetPlatformIcon(string platform)
	{
		return platform?.ToLower() switch
		{
			"standalonewindows" => "[<color=#00BFFF>PC</color>]", 
			"android" => "[<color=#32CD32>Q</color>]", 
			"ios" => "[<color=#FF69B4>iOS</color>]", 
			_ => "[<color=#FFFFFF>UNK</color>]", 
		};
	}

	private static bool IsFriend(Player player)
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

	private static bool IsAdult(Player player)
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

	private static string GetAvatarReleaseStatus(Player player)
	{
		try
		{
			return player.prop_ApiAvatar_0?.releaseStatus;
		}
		catch
		{
			return "ERR";
		}
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

	private static void DestroyIconIfEnabled(Transform playerNameplateCanvas)
	{
	}

	private static void DisableBackground(Transform playerNameplateCanvas)
	{
	}

	private static void ChangeDevCircleSprite(Transform playerNameplateCanvas, Sprite newSprite, Rank rank)
	{
		Transform transform = playerNameplateCanvas.FindChild("NameplateGroup/Nameplate/Contents/Main/Dev Circle");
		if (!(transform != null))
		{
			return;
		}
		ImageThreeSlice component = transform.GetComponent<ImageThreeSlice>();
		if (component != null)
		{
			component.prop_Sprite_0 = newSprite;
			component._sprite = newSprite;
			Color rankColor = GetRankColor(rank);
			CanvasRenderer component2 = transform.GetComponent<CanvasRenderer>();
			if (component2 != null)
			{
				component2.SetColor(rankColor);
			}
			transform.gameObject.SetActive(value: true);
		}
	}

	private static void ApplyRankColoring(Transform playerNameplateCanvas, Rank rank)
	{
		Transform transform = playerNameplateCanvas.FindChild("NameplateGroup/Nameplate/Contents/Main/Dev Circle");
		if (transform != null)
		{
			Color rankColor = GetRankColor(rank);
			CanvasRenderer component = transform.GetComponent<CanvasRenderer>();
			if (component != null)
			{
				component.SetColor(rankColor);
				transform.gameObject.SetActive(value: true);
				MelonLogger.Msg($"Applied {rank} coloring to Dev Circle");
			}
		}
	}

	public static Color GetRankColor(Rank rank)
	{
		return rank switch
		{
			Rank.Visitor => new Color(1f, 1f, 1f, 0.8f), 
			Rank.NewUser => ColorFromHex("#96ECFF", 0.8f), 
			Rank.User => ColorFromHex("#96FFA9", 0.8f), 
			Rank.Known => ColorFromHex("#FF5E50", 0.8f), 
			Rank.Trusted => ColorFromHex("#A900FE", 0.8f), 
			_ => new Color(1f, 1f, 1f, 0.8f), 
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

	static NameplateModifier()
	{
		playerStats = new List<NameplateData>();
		httpClient = new HttpClient();
		API_BASE = "https://odiumvrc.com/api/odium/tags";
		tagCache = new Dictionary<string, List<string>>();
		clientUsers = new HashSet<string>();
		CLIENT_API_BASE = "https://snoofz.net/api/odium/user/exists";
		autoRefreshEnabled = true;
		lastRefreshTime = 0f;
		REFRESH_INTERVAL = 10f;
	}
}
