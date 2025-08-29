using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Odium.Components;
using Odium.Odium;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace Odium.Wrappers;

public static class PlayerWrapper
{
	public static Player LocalPlayer = null;

	public static int ActorId = 0;

	public static List<Player> Players = new List<Player>();

	public static int LocalPlayerActorNr => LocalPlayer.prop_Player_1.prop_Int32_0;

	public static List<Player> GetAllPlayers()
	{
		return PlayerManager.prop_PlayerManager_0.field_Private_List_1_Player_0.ToArray().ToList();
	}

	public static Vector3 GetPosition(Player player)
	{
		return player.gameObject.transform.position;
	}

	public static Player GetPlayerById(string playerId)
	{
		return Players.Find((Player player) => player.field_Private_APIUser_0.id == playerId);
	}

	public static Vector3 GetBonePosition(Player player, HumanBodyBones bone)
	{
		return player.field_Private_VRCPlayerApi_0.GetBonePosition(bone);
	}

	public static Vector3 GetVelocity(Player player)
	{
		return player.field_Private_VRCPlayerApi_0.GetVelocity();
	}

	public static Transform GetBoneTransform(Player player, HumanBodyBones bone)
	{
		return player.field_Private_VRCPlayerApi_0.GetBoneTransform(bone);
	}

	public static void QuickSpoof()
	{
		if (AssignedVariables.adminSpoof)
		{
			string displayName = Networking.LocalPlayer.displayName;
			string text = "eZbake";
			VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0.displayName = text;
			OdiumConsole.Log("OwnerSpoof", "Spoofed as: " + displayName + " | CustomName: " + text);
			if (VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0.displayName == text)
			{
				OdiumConsole.Log("QuickSpoof", "Spoofing successful!");
			}
			else
			{
				OdiumConsole.Log("QuickSpoof", "Spoofing failed. DisplayName mismatch.");
			}
		}
	}

	public static int GetViewID()
	{
		VRCPlayer vrcplayer = LocalPlayer._vrcplayer;
		return vrcplayer.Method_Public_Int32_0();
	}

	public static GameObject GetNamePlateContainer(Player player)
	{
		return player._vrcplayer.field_Public_GameObject_0;
	}

	public static VRCPlayerApi GetLocalPlayerAPIUser(string userId)
	{
		return GetAllPlayers().ToList().Find((Player plr) => plr.field_Private_APIUser_0.id == userId).field_Private_VRCPlayerApi_0;
	}

	public static Player GetPlayerByDisplayName(string name)
	{
		return GetAllPlayers().ToList().Find((Player plr) => plr.field_Private_APIUser_0.displayName == name);
	}

	public static VRCPlayer GetVRCPlayerFromId(string userId)
	{
		return GetAllPlayers().ToList().Find((Player plr) => plr.field_Private_APIUser_0.id == userId).prop_VRCPlayer_0;
	}

	public static VRCPlayer GetVRCPlayerFromPhotonId(int plrId)
	{
		return GetAllPlayers().ToList().Find((Player plr) => plr.field_Private_VRCPlayerApi_0.playerId == plrId).prop_VRCPlayer_0;
	}

	public static VRCPlayer GetPlayerFromPhotonId(int id)
	{
		return GetAllPlayers().ToList().Find((Player plr) => plr.field_Private_VRCPlayerApi_0.playerId == id).prop_VRCPlayer_0;
	}

	public static Player GetVRCPlayerFromActorNr(int id)
	{
		return GetAllPlayers().ToList().Find((Player plr) => plr.prop_Player_1.prop_Int32_0 == id).prop_VRCPlayer_0._player;
	}

	public static VRCPlayer GetPlayerFromActorNr(int id)
	{
		return GetAllPlayers().ToList().Find((Player plr) => plr.field_Private_VRCPlayerApi_0.playerId == id).prop_VRCPlayer_0;
	}

	public static Transform GetNamePlateCanvas(Player player)
	{
		GameObject namePlateContainer = GetNamePlateContainer(player);
		if (namePlateContainer == null)
		{
			return null;
		}
		return namePlateContainer.transform.FindChild("PlayerNameplate/Canvas");
	}

	private static Rank GetPlayerRank(APIUser apiUser)
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

	private static Color GetRankColor(Rank rank)
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
}
