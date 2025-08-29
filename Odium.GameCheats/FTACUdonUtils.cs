using UnityEngine;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Odium.GameCheats;

internal class FTACUdonUtils
{
	public static void SendEvent(string eventName)
	{
		GameObject.Find("Partner Button  (4)")?.GetComponent<UdonBehaviour>()?.SendCustomNetworkEvent(NetworkEventTarget.All, eventName);
	}
}
