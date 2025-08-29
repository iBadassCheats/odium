using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using VRC.SDK3.Components;

namespace Odium.Wrappers;

public static class DroneSwarmWrapper
{
	public static bool isSwarmActive = false;

	private static GameObject targetObject = null;

	private static System.Random random = new System.Random();

	private static Dictionary<VRCPickup, Vector3> droneOffsets = new Dictionary<VRCPickup, Vector3>();

	private static float updateInterval = 0.1f;

	private static float lastUpdateTime = 0f;

	private static float swarmRadius = 1.5f;

	private static float maxSpeed = 0.5f;

	private static float minDistanceBetweenDrones = 0.5f;

	private static float verticalOffset = 0.5f;

	public static void StartDroneSwarm(GameObject target, float radius = 1.5f, float yOffset = 0.5f)
	{
		if (target == null)
		{
			MelonLogger.Msg("[DroneSwarm] Target object is null!");
			return;
		}
		targetObject = target;
		swarmRadius = radius;
		verticalOffset = yOffset;
		isSwarmActive = true;
		droneOffsets.Clear();
		List<VRCPickup> drones = DroneWrapper.GetDrones();
		foreach (VRCPickup item in drones)
		{
			Vector3 value = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(0.5f, swarmRadius);
			droneOffsets[item] = value;
		}
		MelonLogger.Msg($"[DroneSwarm] Started swarm with {drones.Count} drones around {target.name} with {yOffset} vertical offset");
	}

	public static void StopDroneSwarm()
	{
		isSwarmActive = false;
		targetObject = null;
		MelonLogger.Msg("[DroneSwarm] Stopped swarm");
	}

	public static void UpdateDroneSwarm()
	{
		if (!isSwarmActive || targetObject == null || Time.time - lastUpdateTime < updateInterval)
		{
			return;
		}
		lastUpdateTime = Time.time;
		List<VRCPickup> drones = DroneWrapper.GetDrones();
		if (drones.Count == 0)
		{
			return;
		}
		Vector3 position = targetObject.transform.position;
		position.y += verticalOffset;
		foreach (VRCPickup item in drones)
		{
			if (item == null || item.gameObject == null)
			{
				continue;
			}
			if (!droneOffsets.ContainsKey(item))
			{
				droneOffsets[item] = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(0.5f, swarmRadius);
			}
			Vector3 vector = position + droneOffsets[item];
			Vector3 position2 = item.transform.position;
			Vector3 vector2 = vector - position2;
			float magnitude = vector2.magnitude;
			if (magnitude > 0.01f)
			{
				float num = Mathf.Min(maxSpeed, magnitude);
				vector2 = vector2.normalized * num;
			}
			vector2 += new Vector3((float)(random.NextDouble() - 0.5) * 0.05f, (float)(random.NextDouble() - 0.5) * 0.05f, (float)(random.NextDouble() - 0.5) * 0.05f);
			Vector3 vector3 = position2 + vector2;
			foreach (VRCPickup item2 in drones)
			{
				if (!(item2 == item) && !(item2 == null))
				{
					float num2 = Vector3.Distance(vector3, item2.transform.position);
					if (num2 < minDistanceBetweenDrones)
					{
						Vector3 normalized = (vector3 - item2.transform.position).normalized;
						vector3 += normalized * (minDistanceBetweenDrones - num2) * 0.5f;
					}
				}
			}
			DroneWrapper.SetDronePosition(item, vector3);
			Vector3 normalized2 = (position - vector3).normalized;
			normalized2 += new Vector3((float)(random.NextDouble() - 0.5) * 0.1f, (float)(random.NextDouble() - 0.5) * 0.1f, (float)(random.NextDouble() - 0.5) * 0.1f);
			Quaternion quaternion = Quaternion.LookRotation(normalized2);
			DroneWrapper.SetDroneRotation(item, quaternion);
		}
	}

	public static void ChangeSwarmTarget(GameObject newTarget)
	{
		if (newTarget == null)
		{
			MelonLogger.Msg("[DroneSwarm] New target object is null!");
			return;
		}
		targetObject = newTarget;
		MelonLogger.Msg("[DroneSwarm] Changed swarm target to " + newTarget.name);
	}

	public static void AdjustSwarmParameters(float radius = 1.5f, float speed = 0.5f, float minDistance = 0.5f, float yOffset = 0.5f)
	{
		swarmRadius = radius;
		maxSpeed = speed;
		minDistanceBetweenDrones = minDistance;
		verticalOffset = yOffset;
		if (isSwarmActive)
		{
			List<VRCPickup> drones = DroneWrapper.GetDrones();
			foreach (VRCPickup item in drones)
			{
				droneOffsets[item] = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(0.5f, swarmRadius);
			}
		}
		MelonLogger.Msg($"[DroneSwarm] Parameters adjusted - Radius: {radius}, Speed: {speed}, MinDistance: {minDistance}, VerticalOffset: {yOffset}");
	}

	public static void SetVerticalOffset(float yOffset)
	{
		verticalOffset = yOffset;
		MelonLogger.Msg($"[DroneSwarm] Vertical offset set to: {yOffset}");
	}
}
