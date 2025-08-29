using System;
using Odium.Wrappers;
using UnityEngine;

namespace Odium.Modules;

public class portalSpam
{
	public static DateTime LastPortalSpawn = DateTime.Now;

	public static void OnUpdate()
	{
		if (ActionWrapper.portalSpamPlayer != null && ActionWrapper.portalSpam)
		{
			DateTime now = DateTime.Now;
			if ((now - LastPortalSpawn).TotalMilliseconds >= 1.0)
			{
				LastPortalSpawn = now;
				Vector3 bonePosition = PlayerWrapper.GetBonePosition(ActionWrapper.portalSpamPlayer, HumanBodyBones.Head);
				bonePosition.y -= 2f;
				Portal.SpawnPortal(bonePosition, "gghzak9f");
			}
		}
	}
}
