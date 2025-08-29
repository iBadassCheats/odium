using System;
using ExitGames.Client.Photon;
using HarmonyLib;
using Il2CppSystem;
using Odium.ApplicationBot;
using Odium.Wrappers;
using Photon.Realtime;

namespace Odium.Patches;

[HarmonyPatch(typeof(LoadBalancingClient))]
public class PhotonNetworkPatches
{
	[HarmonyPrefix]
	[HarmonyPatch("Method_Public_Virtual_New_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0")]
	private static bool PrefixSendEvent(byte __0, Il2CppSystem.Object __1, RaiseEventOptions __2, SendOptions __3)
	{
		if ((__0 == 12 && ActionWrapper.serialize) || Bot.movementMimic)
		{
			return false;
		}
		if (__0 != 43)
		{
			return true;
		}
		try
		{
			if (__1 != null)
			{
				if (__1.TryCast<Il2CppSystem.Array>() != null)
				{
					Il2CppSystem.Array array = __1.TryCast<Il2CppSystem.Array>();
					if (array.Length > 1)
					{
						for (int i = 0; i < array.Length; i++)
						{
							OdiumConsole.LogGradient("PhotonEvent", array.GetValue(i)?.ToString() ?? "null");
						}
					}
					else
					{
						OdiumConsole.LogGradient("PhotonEvent", "Array too short");
					}
				}
				else
				{
					OdiumConsole.LogGradient("PhotonEvent", __1.ToString());
				}
			}
			else
			{
				OdiumConsole.LogGradient("PhotonEvent", "Event data is null");
			}
		}
		catch (System.Exception ex)
		{
			OdiumConsole.Log("PhotonEvent", "Error logging Photon event 43: " + ex.Message, LogLevel.Error);
		}
		return true;
	}
}
