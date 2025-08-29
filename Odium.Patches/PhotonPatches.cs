using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem;
using MelonLoader;
using Odium.Components;
using Odium.UX;
using Odium.Wrappers;
using UnityEngine;
using VRC;

namespace Odium.Patches;

public class PhotonPatches
{
	public static bool BlockUdon;

	public static Dictionary<int, int> blockedMessages;

	public static int blockedChatBoxMessages;

	public static Dictionary<int, int> blockedMessagesCount;

	public static Dictionary<int, int> blockedUSpeakPacketCount;

	public static Dictionary<int, int> blockedUSpeakPackets;

	public static List<string> blockedUserIds;

	public static List<string> mutedUserIds;

	public static Dictionary<string, int> crashAttemptCounts;

	public static Dictionary<string, System.DateTime> lastLogTimes;

	public static Dictionary<int, PlayerActivityData> playerActivityTracker;

	public static HashSet<string> crashedPlayerIds;

	private static object crashDetectionCoroutine;

	private const float CRASH_TIMEOUT = 5f;

	private const float CHECK_INTERVAL = 1f;

	public static Sprite LogoIcon;

	public static readonly Dictionary<string, System.DateTime> syncAssignMCooldowns;

	public static readonly System.TimeSpan SYNC_ASSIGN_M_COOLDOWN;

	public static Dictionary<string, System.DateTime> lastEventTimes;

	static PhotonPatches()
	{
		BlockUdon = false;
		blockedMessages = new Dictionary<int, int>();
		blockedChatBoxMessages = 0;
		blockedMessagesCount = new Dictionary<int, int>();
		blockedUSpeakPacketCount = new Dictionary<int, int>();
		blockedUSpeakPackets = new Dictionary<int, int>();
		blockedUserIds = new List<string>();
		mutedUserIds = new List<string>();
		crashAttemptCounts = new Dictionary<string, int>();
		lastLogTimes = new Dictionary<string, System.DateTime>();
		playerActivityTracker = new Dictionary<int, PlayerActivityData>();
		crashedPlayerIds = new HashSet<string>();
		LogoIcon = SpriteUtil.LoadFromDisk(System.Environment.CurrentDirectory + "\\Odium\\OdiumIcon.png");
		syncAssignMCooldowns = new Dictionary<string, System.DateTime>();
		SYNC_ASSIGN_M_COOLDOWN = System.TimeSpan.FromMinutes(2.0);
		lastEventTimes = new Dictionary<string, System.DateTime>();
		StartCrashDetection();
	}

	public static void StartCrashDetection()
	{
		if (crashDetectionCoroutine != null)
		{
			MelonCoroutines.Stop(crashDetectionCoroutine);
		}
		crashDetectionCoroutine = MelonCoroutines.Start(CrashDetectionLoop());
		OdiumConsole.Log("CrashDetection", "Crash detection system started");
	}

	public static void StopCrashDetection()
	{
		if (crashDetectionCoroutine != null)
		{
			MelonCoroutines.Stop(crashDetectionCoroutine);
			crashDetectionCoroutine = null;
			OdiumConsole.Log("CrashDetection", "Crash detection system stopped");
		}
	}

	private static IEnumerator CrashDetectionLoop()
	{
		while (true)
		{
			try
			{
				CheckForCrashedPlayers();
				CleanupDisconnectedPlayers();
			}
			catch (System.Exception ex)
			{
				System.Exception ex2 = ex;
				OdiumConsole.Log("CrashDetection", "Error in crash detection loop: " + ex2.Message);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private static void CheckForCrashedPlayers()
	{
		float time = Time.time;
		List<int> list = new List<int>(playerActivityTracker.Keys);
		foreach (int item in list)
		{
			PlayerActivityData value = playerActivityTracker[item];
			if (!value.wasActive || value.hasCrashed)
			{
				continue;
			}
			float num = time - value.lastEvent1Time;
			float num2 = time - value.lastEvent12Time;
			if (num > 5f && num2 > 5f)
			{
				value.hasCrashed = true;
				playerActivityTracker[item] = value;
				if (!string.IsNullOrEmpty(value.userId))
				{
					crashedPlayerIds.Add(value.userId);
					OdiumConsole.LogGradient("CrashDetection", $"Player CRASHED: {value.displayName} (Actor: {item}, UserID: {value.userId})");
					InternalConsole.LogIntoConsole($"{value.displayName} has crashed (no events for {5f}s)");
				}
			}
		}
	}

	private static void CleanupDisconnectedPlayers()
	{
		try
		{
			if (PlayerManager.prop_PlayerManager_0?.field_Private_List_1_Player_0 == null)
			{
				return;
			}
			HashSet<string> currentPlayers = (from p in PlayerManager.prop_PlayerManager_0.field_Private_List_1_Player_0.ToArray()
				where p?.field_Private_APIUser_0?.id != null
				select p.field_Private_APIUser_0.id).ToHashSet();
			List<string> list = crashedPlayerIds.Where((string userId) => !currentPlayers.Contains(userId)).ToList();
			foreach (string item in list)
			{
				crashedPlayerIds.Remove(item);
				OdiumConsole.LogGradient("CrashDetection", "Removed disconnected crashed player: " + item);
			}
			List<int> list2 = playerActivityTracker.Keys.Where(delegate(int actorId)
			{
				PlayerActivityData playerActivityData = playerActivityTracker[actorId];
				return !string.IsNullOrEmpty(playerActivityData.userId) && !currentPlayers.Contains(playerActivityData.userId);
			}).ToList();
			foreach (int item2 in list2)
			{
				playerActivityTracker.Remove(item2);
			}
		}
		catch (System.Exception ex)
		{
			OdiumConsole.Log("CrashDetection", "Error in cleanup: " + ex.Message);
		}
	}

	public static void UpdatePlayerActivity(int actorId, int eventCode)
	{
		try
		{
			Player vRCPlayerFromActorNr = PlayerWrapper.GetVRCPlayerFromActorNr(actorId);
			if (!playerActivityTracker.ContainsKey(actorId))
			{
				PlayerActivityData value = new PlayerActivityData
				{
					actorId = actorId,
					userId = (vRCPlayerFromActorNr?.field_Private_APIUser_0?.id ?? ""),
					displayName = (vRCPlayerFromActorNr?.field_Private_APIUser_0?.displayName ?? "Unknown"),
					lastEvent1Time = 0f,
					lastEvent12Time = 0f,
					hasCrashed = false,
					wasActive = false
				};
				playerActivityTracker[actorId] = value;
			}
			PlayerActivityData value2 = playerActivityTracker[actorId];
			float time = Time.time;
			switch (eventCode)
			{
			case 1:
				value2.lastEvent1Time = time;
				value2.wasActive = true;
				break;
			case 12:
				value2.lastEvent12Time = time;
				value2.wasActive = true;
				break;
			}
			if (value2.hasCrashed && value2.wasActive)
			{
				value2.hasCrashed = false;
				if (!string.IsNullOrEmpty(value2.userId))
				{
					crashedPlayerIds.Remove(value2.userId);
					OdiumConsole.LogGradient("CrashDetection", "Player RECOVERED: " + value2.displayName + " is active again");
				}
			}
			playerActivityTracker[actorId] = value2;
		}
		catch (System.Exception ex)
		{
			OdiumConsole.Log("CrashDetection", "Error updating player activity: " + ex.Message);
		}
	}

	public static bool HasPlayerCrashed(string userId)
	{
		return !string.IsNullOrEmpty(userId) && crashedPlayerIds.Contains(userId);
	}

	public static string GetCrashStatusInfo()
	{
		int num = playerActivityTracker.Values.Where((PlayerActivityData p) => p.wasActive && !p.hasCrashed).Count();
		int count = crashedPlayerIds.Count;
		return $"Active: {num}, Crashed: {count}";
	}

	public static void MarkPlayerAsCrashed(string userId)
	{
		if (!string.IsNullOrEmpty(userId))
		{
			crashedPlayerIds.Add(userId);
		}
	}

	public static void UnmarkPlayerAsCrashed(string userId)
	{
		if (!string.IsNullOrEmpty(userId))
		{
			crashedPlayerIds.Remove(userId);
		}
	}

	public static bool TryUnboxInt(Il2CppSystem.Object obj, out int result)
	{
		result = 0;
		try
		{
			if (obj == null)
			{
				return false;
			}
			result = obj.Unbox<int>();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool TryUnboxByte(Il2CppSystem.Object obj, out byte result)
	{
		result = 0;
		try
		{
			if (obj == null)
			{
				return false;
			}
			result = obj.Unbox<byte>();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool IsKnownExploitPattern(string entryPointName, string behaviourName)
	{
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		dictionary.Add("ListPatrons", new string[1] { "Patreon Credits" });
		Dictionary<string, string[]> dictionary2 = dictionary;
		return dictionary2.ContainsKey(entryPointName) && dictionary2[entryPointName].Contains(behaviourName);
	}

	public static bool IsRapidFireEvent(string playerName)
	{
		System.DateTime now = System.DateTime.Now;
		if (lastEventTimes.ContainsKey(playerName) && (now - lastEventTimes[playerName]).TotalMilliseconds < 100.0)
		{
			return true;
		}
		lastEventTimes[playerName] = now;
		return false;
	}

	public static string GetGameObjectPath(GameObject obj)
	{
		if (obj == null)
		{
			return "null";
		}
		string text = obj.name;
		Transform parent = obj.transform.parent;
		while (parent != null)
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		return text;
	}

	private static void CleanupOldCooldowns()
	{
		System.DateTime cutoff = System.DateTime.Now - System.TimeSpan.FromHours(1.0);
		List<string> list = (from kvp in syncAssignMCooldowns
			where kvp.Value < cutoff
			select kvp.Key).ToList();
		foreach (string item in list)
		{
			syncAssignMCooldowns.Remove(item);
		}
		if (list.Count > 0)
		{
			InternalConsole.LogIntoConsole($"[CLEANUP] Removed {list.Count} old cooldown entries");
		}
	}

	public static bool IsUnusualHash(uint eventHash)
	{
		if (eventHash == 0 || eventHash == uint.MaxValue)
		{
			return true;
		}
		uint[] source = new uint[1] { 236258089u };
		return source.Contains(eventHash);
	}
}
