using Il2CppSystem.Collections.Generic;
using UnityEngine;
using VRC;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public static class UdonExtensions
{
	public static void SendUdon(this GameObject go, string evt, Player player = null, bool check = false)
	{
		UdonBehaviour component = go.GetComponent<UdonBehaviour>();
		if (player == null)
		{
			if (!check)
			{
				if (player == VRCPlayer.field_Internal_Static_VRCPlayer_0._player)
				{
					component.SendCustomEvent(evt);
				}
				else
				{
					component.SendCustomNetworkEvent(NetworkEventTarget.All, evt);
				}
			}
		}
		else
		{
			go.SetOwner(player);
			component.SendCustomNetworkEvent(NetworkEventTarget.Owner, evt);
		}
	}

	public static void SetOwner(this GameObject go, Player player)
	{
		if (!(go.GetOwner() == player))
		{
			Networking.SetOwner(player.field_Private_VRCPlayerApi_0, go);
		}
	}

	public static Player GetOwner(this GameObject go)
	{
		List<Player>.Enumerator enumerator = PlayerManager.prop_PlayerManager_0.field_Private_List_1_Player_0.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Player current = enumerator.Current;
			if (current.field_Private_VRCPlayerApi_0.IsOwner(go))
			{
				return current;
			}
		}
		return null;
	}
}
