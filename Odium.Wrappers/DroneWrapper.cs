using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.PlayerDrone;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Odium.Wrappers;

internal class DroneWrapper
{
	public static DroneManager DroneManager;

	public static int DroneViewId = PlayerWrapper.ActorId + 10001;

	public static DroneManager GetDroneManager()
	{
		return GameObject.Find("UIManager/DroneManager").GetComponent<DroneManager>();
	}

	public static string GetDroneID(VRCPickup Drone)
	{
		DroneController component = Drone.gameObject.GetComponent<DroneController>();
		return component.field_Private_String_0;
	}

	public static void RemoveDrone(string id)
	{
		GetDroneManager().Method_Private_Void_String_PDM_0(id);
	}

	public static void SpawnDrone(Vector3 position, Vector3 Rotation)
	{
		VRCPlayer vrcplayer = PlayerWrapper.LocalPlayer._vrcplayer;
		int param_ = vrcplayer.Method_Public_Int32_0();
		vrcplayer.Method_Public_Int32_0();
		GetDroneManager().Method_Private_Void_Player_Vector3_Vector3_String_Int32_Color_Color_Color_PDM_0(PlayerWrapper.LocalPlayer, position, Rotation, Guid.NewGuid().ToString(), param_, Color.black, Color.black, Color.black);
	}

	public static List<VRCPickup> GetDrones()
	{
		List<VRCPickup> drones = new List<VRCPickup>();
		UnityEngine.Object.FindObjectsOfType<VRCPickup>().ToList().ForEach(delegate(VRCPickup pickup)
		{
			if (pickup.gameObject.name.Contains("VRCDrone"))
			{
				drones.Add(pickup);
			}
		});
		return drones;
	}

	public static void DroneCrash()
	{
		for (int i = 0; i < GetDrones().Count; i++)
		{
			Networking.SetOwner(Networking.LocalPlayer, GetDrones()[i].gameObject);
			GetDrones()[i].gameObject.transform.position = new Vector3(2.222224E+11f, 0f);
		}
	}

	public static void SetDronePosition(VRCPickup drone, Vector3 vector3)
	{
		Networking.SetOwner(Networking.LocalPlayer, drone.gameObject);
		drone.gameObject.transform.position = vector3;
	}

	public static void SetDroneRotation(VRCPickup drone, Quaternion quaternion)
	{
		Networking.SetOwner(Networking.LocalPlayer, drone.gameObject);
		drone.gameObject.transform.rotation = quaternion;
	}
}
