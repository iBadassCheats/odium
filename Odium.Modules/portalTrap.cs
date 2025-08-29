using System;
using Odium.Wrappers;
using UnityEngine;

namespace Odium.Modules;

public class portalTrap
{
	public static DateTime LastPortalSpawn = DateTime.Now;

	public static void OnUpdate()
	{
		if (!(ActionWrapper.portalTrapPlayer != null) || !ActionWrapper.portalTrap)
		{
			return;
		}
		DateTime now = DateTime.Now;
		if ((now - LastPortalSpawn).TotalMilliseconds >= 500.0)
		{
			LastPortalSpawn = now;
			Vector3 velocity = PlayerWrapper.GetVelocity(ActionWrapper.portalTrapPlayer);
			float magnitude = velocity.magnitude;
			if (magnitude > 2.5f)
			{
				LastPortalSpawn = now;
				Vector3 normalized = velocity.normalized;
				Vector3 position = PlayerWrapper.GetPosition(ActionWrapper.portalTrapPlayer);
				Vector3 positon = position + normalized * 3f;
				Portal.SpawnPortal(positon, "aywxh5ah");
			}
		}
	}
}
