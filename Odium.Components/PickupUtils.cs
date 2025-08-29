using UnityEngine;
using VRC.SDKBase;

namespace Odium.Components;

public class PickupUtils
{
	public static VRC_Pickup[] array;

	public static float rotationAngle;

	public static void TakeOwnerShipPickup(VRC_Pickup pickup)
	{
		if (!(pickup == null))
		{
			Networking.SetOwner(Networking.LocalPlayer, pickup.gameObject);
		}
	}

	public static void Respawn()
	{
		foreach (VRC_Pickup item in Object.FindObjectsOfType<VRC_Pickup>())
		{
			Networking.LocalPlayer.TakeOwnership(item.gameObject);
			item.transform.localPosition = new Vector3(0f, -100000f, 0f);
		}
	}

	public static void BringPickups()
	{
		foreach (VRC_Pickup item in Object.FindObjectsOfType<VRC_Pickup>())
		{
			Networking.SetOwner(Networking.LocalPlayer, item.gameObject);
			item.transform.position = Networking.LocalPlayer.gameObject.transform.position;
		}
	}

	public static void rotateobjse()
	{
		rotationAngle += 45f;
		if (rotationAngle >= 360f)
		{
			rotationAngle -= 360f;
		}
		foreach (VRC_Pickup item in Object.FindObjectsOfType<VRC_Pickup>())
		{
			Networking.SetOwner(Networking.LocalPlayer, item.gameObject);
			item.transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
		}
	}
}
