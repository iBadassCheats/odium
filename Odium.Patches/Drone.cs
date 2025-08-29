using System;
using System.Collections;
using HarmonyLib;
using MelonLoader;
using Odium.Components;
using Odium.Odium;
using Odium.UX;
using Odium.Wrappers;
using UnityEngine;
using VRC;
using VRC.PlayerDrone;

namespace Odium.Patches;

[HarmonyPatch(typeof(DroneManager))]
internal class Drone
{
	public static int androidUserCount;

	[HarmonyPrefix]
	[HarmonyPatch("Method_Private_Void_Player_Vector3_Vector3_String_Int32_Color_Color_Color_PDM_0")]
	private static void OnDroneSpawnPrefix(DroneManager __instance, Player param_1, Vector3 param_2, Vector3 param_3, string param_4, int param_5, Color param_6, Color param_7, Color param_8)
	{
		try
		{
			string text = param_1?.field_Private_APIUser_0?.displayName ?? "Unknown";
			InternalConsole.LogIntoConsole("<color=#31BCF0>[DroneManager]:</color> <color=#00AAFF>Drone Spawn</color> by <color=#00FF74>" + text + "</color>");
			if (AssignedVariables.autoDroneCrash)
			{
				InternalConsole.LogIntoConsole("<color=#31BCF0>[AntiQuest]:</color> <color=#00AAFF>Drone Crash</color> using <color=#00FF74>" + text + "</color>'s drone!");
				MelonCoroutines.Start(DelayedDroneCrash());
			}
		}
		catch (Exception ex)
		{
			InternalConsole.LogIntoConsole(" <color=#FF0000>[ERROR]:</color> <color=#FF0000>Error in drone patch: " + ex.Message + "</color>");
		}
	}

	private static IEnumerator DelayedDroneCrash()
	{
		androidUserCount = 0;
		float elapsed = 0f;
		while (elapsed < 1f)
		{
			elapsed += Time.deltaTime;
			yield return null;
		}
		DroneWrapper.DroneCrash();
		for (int i = 0; i < PlayerWrapper.Players.Count; i++)
		{
			if (NameplateModifier.GetPlayerPlatform(PlayerWrapper.Players[i].prop_VRCPlayer_0._player) != "standalonewindows")
			{
				androidUserCount++;
			}
		}
		InternalConsole.LogIntoConsole($"<color=#31BCF0>[AntiQuest]:</color> Removed <color=#00AAFF>{androidUserCount}</color> quest users from the instance!");
		androidUserCount = 0;
	}
}
