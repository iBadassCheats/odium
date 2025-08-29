using System;
using System.Net.Http;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using Odium.UX;
using VRC.Core;
using VRC.SDK3.StringLoading;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Odium.Patches;

internal class Patching
{
	public static int patchCount;

	public static void Initialize()
	{
		OdiumEntry.HarmonyInstance.Patch(typeof(VRCPlusStatus).GetProperty("prop_Object1PublicTBoTUnique_1_Boolean_0").GetGetMethod(), null, new HarmonyMethod(typeof(Patching).GetMethod("VRCPlusOverride", BindingFlags.Static | BindingFlags.NonPublic)));
		patchCount++;
		OdiumEntry.HarmonyInstance.Patch(typeof(UdonBehaviour).GetMethod("SendCustomNetworkEvent", new Type[2]
		{
			typeof(NetworkEventTarget),
			typeof(string)
		}), new HarmonyMethod(typeof(Patching).GetMethod("OnUdonNetworkEvent", BindingFlags.Static | BindingFlags.NonPublic)));
		patchCount++;
		OdiumEntry.HarmonyInstance.Patch(typeof(VRCStringDownloader).GetMethod("LoadUrl"), new HarmonyMethod(typeof(Patching).GetMethod("OnStringDownload", BindingFlags.Static | BindingFlags.NonPublic)));
		patchCount++;
		OdiumEntry.HarmonyInstance.Patch(typeof(HttpClient).GetMethod("Get"), new HarmonyMethod(typeof(Patching).GetMethod("OnGet", BindingFlags.Static | BindingFlags.NonPublic)));
		patchCount++;
		MelonLogger.Msg($"Patches initialized successfully. Total patches: {patchCount}");
	}

	private static bool OnGet(string url)
	{
		try
		{
			ApiWorld field_Internal_Static_ApiWorld_ = RoomManager.field_Internal_Static_ApiWorld_0;
			if (field_Internal_Static_ApiWorld_ == null)
			{
				return true;
			}
			string authorId = field_Internal_Static_ApiWorld_.authorId;
			if (authorId == "LyCh6jlK6X")
			{
				OdiumConsole.LogGradient("BLOCKED", "Prevented string download in Jar's world (URL: " + url + ")");
				return false;
			}
		}
		catch (Exception arg)
		{
			MelonLogger.Error($"Error in OnStringDownload: {arg}");
		}
		return true;
	}

	private static void VRCPlusOverride(ref Object1PublicTBoTUnique<bool> __result)
	{
		if (__result != null)
		{
			__result.prop_T_0 = true;
			__result.field_Protected_T_0 = true;
		}
	}

	private static bool OnUdonNetworkEvent(UdonBehaviour __instance, NetworkEventTarget target, string eventName)
	{
		if (eventName != "ListPatrons")
		{
			return true;
		}
		VRCPlayer componentInParent = __instance.GetComponentInParent<VRCPlayer>();
		if (componentInParent == null || componentInParent == VRCPlayer.field_Internal_Static_VRCPlayer_0)
		{
			return true;
		}
		InternalConsole.LogIntoConsole("[BLOCKED] Crash attempt from " + componentInParent.field_Private_VRCPlayerApi_0.displayName + "!", "[Udon]");
		return false;
	}

	private static bool OnUdonRunProgram(UdonBehaviour __instance, string programName)
	{
		if (programName != "ListPatrons")
		{
			return true;
		}
		VRCPlayer componentInParent = __instance.GetComponentInParent<VRCPlayer>();
		if (componentInParent == null || componentInParent == VRCPlayer.field_Internal_Static_VRCPlayer_0)
		{
			return true;
		}
		InternalConsole.LogIntoConsole("[BLOCKED] Crash attempt from " + componentInParent.field_Private_VRCPlayerApi_0.displayName + "!", "[Udon]");
		return false;
	}

	private static bool OnStringDownload(string url)
	{
		try
		{
			ApiWorld field_Internal_Static_ApiWorld_ = RoomManager.field_Internal_Static_ApiWorld_0;
			if (field_Internal_Static_ApiWorld_ == null)
			{
				return true;
			}
			string authorId = field_Internal_Static_ApiWorld_.authorId;
			if (authorId == "LyCh6jlK6X")
			{
				OdiumConsole.LogGradient("BLOCKED", "Prevented string download in Jar's world (URL: " + url + ")");
				return false;
			}
		}
		catch (Exception arg)
		{
			MelonLogger.Error($"Error in OnStringDownload: {arg}");
		}
		return true;
	}
}
