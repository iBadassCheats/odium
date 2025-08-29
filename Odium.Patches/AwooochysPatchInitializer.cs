using System;
using System.Reflection;
using ExitGames.Client.Photon;
using HarmonyLib;
using MelonLoader;
using VRC.Economy;
using VRC.SDKBase;

namespace Odium.Patches;

public class AwooochysPatchInitializer
{
	public static string ModuleName = "HookManager";

	public static readonly HarmonyLib.Harmony instance = new HarmonyLib.Harmony("DeepCoreV2.ultrapatch");

	public static int pass = 0;

	public static int fail = 0;

	private static HarmonyMethod GetPreFix(string methodName)
	{
		return new HarmonyMethod(typeof(AwooochysPatchInitializer).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic));
	}

	[Obsolete]
	public static void Start()
	{
		Console.WriteLine("StartupStarting Hooks...");
		try
		{
			ClonePatch.Patch();
			pass++;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ModuleName + "allowAvatarCopying:" + ex.Message);
			fail++;
		}
		try
		{
			RoomManagerPatch.Patch();
			pass++;
		}
		catch (Exception ex2)
		{
			Console.WriteLine(ModuleName + "RoomManager:" + ex2.Message);
			fail++;
		}
		try
		{
			EasyPatching.DeepCoreInstance.Patch(typeof(VRCPlusStatus).GetProperty("prop_Object1PublicTYBoTYUnique_1_Boolean_0").GetGetMethod(), null, GetLocalPatch("GetVRCPlusStatus"));
			pass++;
		}
		catch (Exception ex3)
		{
			Console.WriteLine(ModuleName + "VRCPlusStatus:" + ex3.Message);
			fail++;
		}
		try
		{
			instance.Patch(typeof(Store).GetMethod("Method_Private_Boolean_VRCPlayerApi_IProduct_PDM_0"), GetPreFix("RetrunPrefix"));
			instance.Patch(typeof(Store).GetMethod("Method_Private_Boolean_IProduct_PDM_0"), GetPreFix("RetrunPrefix"));
			pass++;
		}
		catch (Exception ex4)
		{
			Console.WriteLine(ModuleName + "Store:" + ex4.Message);
			fail++;
		}
		try
		{
			pass++;
		}
		catch (Exception ex5)
		{
			Console.WriteLine(ModuleName + "QuickMenu:" + ex5.Message);
			fail++;
		}
		Console.WriteLine(ModuleName + $"Placed {pass} hook successfully, with {fail} failed.");
	}

	private static bool MarketPatch(VRCPlayerApi __0, IProduct __1, ref bool __result)
	{
		__result = true;
		return false;
	}

	private static bool RetrunPrefix(ref bool __result)
	{
		__result = true;
		return false;
	}

	internal static bool Patch_OnEventSent(byte __0, object __1, RaiseEventOptions __2, SendOptions __3)
	{
		if (PhotonDebugger.IsOnEventSendDebug)
		{
			return PhotonDebugger.OnEventSent(__0, __1, __2, __3);
		}
		return true;
	}

	public static HarmonyMethod GetLocalPatch(string name)
	{
		try
		{
			return typeof(AwooochysPatchInitializer).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic).ToNewHarmonyMethod();
		}
		catch (Exception arg)
		{
			Console.WriteLine(ModuleName + $"{name}: {arg}");
			return null;
		}
	}
}
