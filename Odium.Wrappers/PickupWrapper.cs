using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Odium.Wrappers;

internal class PickupWrapper
{
	public static List<VRCPickup> cachedPickups = new List<VRCPickup>();

	public static List<VRCPickup> GetVRCPickups()
	{
		List<VRCPickup> pickups = new List<VRCPickup>();
		Object.FindObjectsOfType<VRCPickup>().ToList().ForEach(delegate(VRCPickup pickup)
		{
			if (pickup.gameObject != null)
			{
				pickups.Add(pickup);
			}
		});
		return pickups;
	}

	public static void DropAllPickupsInRange(float range)
	{
		Vector3 position = PlayerWrapper.LocalPlayer.transform.position;
		foreach (VRCPickup vRCPickup in GetVRCPickups())
		{
			if (vRCPickup != null && vRCPickup.gameObject != null)
			{
				float num = Vector3.Distance(position, vRCPickup.transform.position);
				if (num <= range)
				{
					Networking.SetOwner(Networking.LocalPlayer, vRCPickup.gameObject);
					vRCPickup.Drop();
				}
			}
		}
	}

	public static void DropDronePickups()
	{
		Vector3 position = PlayerWrapper.LocalPlayer.transform.position;
		foreach (VRCPickup vRCPickup in GetVRCPickups())
		{
			if (vRCPickup != null && vRCPickup.gameObject != null && vRCPickup.gameObject.name.Contains("Drone"))
			{
				Networking.SetOwner(Networking.LocalPlayer, vRCPickup.gameObject);
				vRCPickup.Drop();
			}
		}
	}

	public static void DropAllPickups()
	{
		foreach (VRCPickup vRCPickup in GetVRCPickups())
		{
			if (vRCPickup != null && vRCPickup.gameObject != null)
			{
				Networking.SetOwner(Networking.LocalPlayer, vRCPickup.gameObject);
				vRCPickup.Drop();
			}
		}
	}

	public static void BringAllPickupsToPlayer(Player player)
	{
		Vector3 position = player.transform.position;
		foreach (VRCPickup vRCPickup in GetVRCPickups())
		{
			if (vRCPickup != null && vRCPickup.gameObject != null)
			{
				Networking.SetOwner(Networking.LocalPlayer, vRCPickup.gameObject);
				vRCPickup.transform.position = position + Vector3.up * 0.5f;
			}
		}
	}

	public static void HideAllPickups()
	{
		foreach (VRCPickup vRCPickup in GetVRCPickups())
		{
			if (vRCPickup != null && vRCPickup.gameObject != null)
			{
				cachedPickups.Add(vRCPickup);
				vRCPickup.gameObject.SetActive(value: false);
			}
		}
	}

	public static void ShowAllPickups()
	{
		foreach (VRCPickup vRCPickup in GetVRCPickups())
		{
			if (vRCPickup != null && cachedPickups != null)
			{
				vRCPickup.gameObject.SetActive(value: true);
			}
		}
	}

	public static void RespawnAllPickups()
	{
		foreach (VRCPickup vRCPickup in GetVRCPickups())
		{
			if (vRCPickup != null && vRCPickup.gameObject != null)
			{
				Networking.SetOwner(Networking.LocalPlayer, vRCPickup.gameObject);
				vRCPickup.transform.position = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			}
		}
	}
}
