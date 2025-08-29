using System;
using Il2CppSystem;
using Odium.ButtonAPI.QM;
using Odium.QMPages;
using Odium.Wrappers;
using UnityEngine;
using VRC;

namespace Odium.Components;

public class PlayerExtraMethods
{
	public static void setInfiniteVoiceRange(Player player, bool state)
	{
		string selected_player_name = AppBot.get_selected_player_name();
		player = ApiUtils.GetPlayerByDisplayName(selected_player_name);
		try
		{
			if (state)
			{
				OdiumConsole.Log("SetInfiniteVoiceRange: ", "You are no longer listening to  " + selected_player_name);
				Il2CppSystem.Console.WriteLine("SetInfiniteVoiceRange: You are no longer listening to  " + selected_player_name);
				player.field_Private_VRCPlayerApi_0.SetVoiceDistanceFar(25f);
			}
			else
			{
				OdiumConsole.Log("SetInfiniteVoiceRange: ", "Listening to  " + selected_player_name);
				Il2CppSystem.Console.WriteLine("SetInfiniteVoiceRange: Listening to  " + selected_player_name);
				player.field_Private_VRCPlayerApi_0.SetVoiceDistanceFar(float.PositiveInfinity);
			}
		}
		catch (System.Exception ex)
		{
			OdiumConsole.Log("PlayerExtraMethods: ", "SetInfiniteVoiceRange shat itself.", LogLevel.Warning);
			OdiumConsole.LogException(ex);
			System.Console.WriteLine("PlayerExtraMethods: " + ex);
		}
	}

	public static void focusTargetAudio(Player targetPlayer, bool state)
	{
		float defaultVoiceGain = 0f;
		try
		{
			if (state)
			{
				defaultVoiceGain = targetPlayer.field_Private_VRCPlayerApi_0.GetVoiceGain();
				targetPlayer.field_Private_VRCPlayerApi_0.SetVoiceDistanceFar(float.PositiveInfinity);
				PlayerWrapper.Players.ForEach(delegate(Player player)
				{
					player.field_Private_VRCPlayerApi_0.SetVoiceGain(0f);
				});
			}
			else
			{
				targetPlayer.field_Private_VRCPlayerApi_0.SetVoiceDistanceFar(25f);
				PlayerWrapper.Players.ForEach(delegate(Player player)
				{
					player.field_Private_VRCPlayerApi_0.SetVoiceGain(defaultVoiceGain);
				});
			}
		}
		catch (System.Exception ex)
		{
			OdiumConsole.Log("PlayerExtraMethods: ", "focusTargetAudio shat itself.", LogLevel.Warning);
			OdiumConsole.LogException(ex);
			System.Console.WriteLine("PlayerExtraMethods: " + ex);
		}
	}

	public static void teleportBehind(Player targetPlayer)
	{
		Vector3 position = targetPlayer.transform.position;
		Vector3 forward = targetPlayer.transform.forward;
		float num = 2f;
		Vector3 vector = position - forward * num;
		PlayerWrapper.LocalPlayer.transform.position = vector;
		Vector3 normalized = (position - vector).normalized;
		PlayerWrapper.LocalPlayer.transform.rotation = Quaternion.LookRotation(normalized);
	}

	public static void teleportTo(Player targetPlayer)
	{
		PlayerWrapper.LocalPlayer.transform.position = targetPlayer.transform.position;
	}
}
