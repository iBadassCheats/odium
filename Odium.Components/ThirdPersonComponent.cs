using System;
using MelonLoader;
using UnityEngine;
using VRC;

namespace Odium.Components;

public class ThirdPersonComponent
{
	private static bool isThirdPerson = false;

	private static Camera mainCamera;

	private static Player localPlayer;

	private static Transform originalCameraParent;

	private static Vector3 originalCameraPosition;

	private static Quaternion originalCameraRotation;

	private static float cameraHeight = 1.8f;

	private static float cameraDistance = 2.5f;

	private static float smoothTime = 0.3f;

	private static bool initialized = false;

	private static Vector3 currentVelocity;

	private static Transform headBone;

	private static Transform chestBone;

	private static Camera thirdPersonCamera;

	private static GameObject thirdPersonCameraObject;

	private static Renderer[] headRenderers;

	private static bool headVisible = true;

	public static bool IsThirdPerson => isThirdPerson;

	public static bool IsInitialized => initialized;

	public static bool IsHeadVisible => headVisible;

	public static float CameraDistance => cameraDistance;

	public static float CameraHeight => cameraHeight;

	public static float SmoothTime => smoothTime;

	public static string AttachedBone => mainCamera?.transform?.parent?.name ?? "None";

	public static void Initialize()
	{
		if (initialized)
		{
			return;
		}
		try
		{
			mainCamera = Camera.main;
			if (mainCamera == null)
			{
				MelonLogger.Error("Main camera not found!");
				return;
			}
			localPlayer = Player.prop_Player_0;
			if (localPlayer == null)
			{
				MelonLogger.Warning("Local player not found, will retry...");
				return;
			}
			originalCameraParent = mainCamera.transform.parent;
			originalCameraPosition = mainCamera.transform.localPosition;
			originalCameraRotation = mainCamera.transform.localRotation;
			GetHeadBone();
			CreateThirdPersonCamera();
			initialized = true;
			MelonLogger.Msg("Third Person Component initialized");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Failed to initialize Third Person Component: " + ex.Message);
		}
	}

	private static void CreateThirdPersonCamera()
	{
		try
		{
			thirdPersonCameraObject = new GameObject("ThirdPersonCamera");
			thirdPersonCamera = thirdPersonCameraObject.AddComponent<Camera>();
			thirdPersonCamera.CopyFrom(mainCamera);
			thirdPersonCamera.enabled = false;
			UnityEngine.Object.DontDestroyOnLoad(thirdPersonCameraObject);
			MelonLogger.Msg("Third person camera created");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Failed to create third person camera: " + ex.Message);
		}
	}

	private static void GetHeadBone()
	{
		try
		{
			if (!(localPlayer != null))
			{
				return;
			}
			Animator componentInChildren = localPlayer.GetComponentInChildren<Animator>();
			if (componentInChildren != null && componentInChildren.isHuman)
			{
				headBone = componentInChildren.GetBoneTransform(HumanBodyBones.Head);
				if (headBone == null)
				{
					headBone = componentInChildren.GetBoneTransform(HumanBodyBones.Neck);
				}
				chestBone = componentInChildren.GetBoneTransform(HumanBodyBones.Chest);
				if (chestBone == null)
				{
					chestBone = componentInChildren.GetBoneTransform(HumanBodyBones.Spine);
				}
				if (chestBone == null)
				{
					chestBone = componentInChildren.GetBoneTransform(HumanBodyBones.Hips);
				}
				MelonLogger.Msg("Head bone: " + headBone?.name + ", Chest bone: " + chestBone?.name);
				FindHeadRenderers();
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error getting bones: " + ex.Message);
		}
	}

	private static void FindHeadRenderers()
	{
		try
		{
			if (headBone != null)
			{
				headRenderers = headBone.GetComponentsInChildren<Renderer>();
				Renderer[] array = headRenderers;
				MelonLogger.Msg($"Found {((array != null) ? array.Length : 0)} head renderers");
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error finding head renderers: " + ex.Message);
		}
	}

	private static void SetHeadVisibility(bool visible)
	{
		try
		{
			if (headRenderers == null)
			{
				return;
			}
			Renderer[] array = headRenderers;
			foreach (Renderer renderer in array)
			{
				if (renderer != null)
				{
					renderer.enabled = visible;
				}
			}
			headVisible = visible;
			MelonLogger.Msg($"Head visibility set to: {visible}");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error setting head visibility: " + ex.Message);
		}
	}

	public static void SetThirdPerson(bool enabled)
	{
		if (!initialized)
		{
			Initialize();
			if (!initialized)
			{
				return;
			}
		}
		isThirdPerson = enabled;
		try
		{
			if (enabled)
			{
				if (thirdPersonCamera == null)
				{
					CreateThirdPersonCamera();
				}
				if (thirdPersonCamera != null && mainCamera != null)
				{
					mainCamera.enabled = false;
					thirdPersonCamera.enabled = true;
					SetInitialThirdPersonPosition();
					MelonLogger.Msg("Enabled third person view");
				}
			}
			else
			{
				RestoreFirstPerson();
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error setting third person: " + ex.Message);
		}
	}

	private static void SetInitialThirdPersonPosition()
	{
		if (thirdPersonCamera == null || localPlayer == null)
		{
			return;
		}
		try
		{
			Vector3 position = localPlayer.transform.position;
			Vector3 forward = localPlayer.transform.forward;
			Vector3 vector = position + Vector3.up * 1.7f;
			if (headBone != null)
			{
				vector = headBone.position;
			}
			Vector3 vector2 = vector + -forward * cameraDistance + Vector3.up * cameraHeight;
			thirdPersonCamera.transform.position = vector2;
			Vector3 forward2 = vector - vector2;
			if (forward2.magnitude > 0.01f)
			{
				thirdPersonCamera.transform.rotation = Quaternion.LookRotation(forward2, Vector3.up);
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error setting initial position: " + ex.Message);
		}
	}

	public static void ToggleThirdPerson()
	{
		SetThirdPerson(!isThirdPerson);
	}

	public static void Update()
	{
		if (!initialized)
		{
			Initialize();
			if (!initialized)
			{
				return;
			}
		}
		if (Input.GetKeyDown(KeyCode.F5))
		{
			ToggleThirdPerson();
		}
		if (isThirdPerson && thirdPersonCamera != null && thirdPersonCamera.enabled)
		{
			UpdateThirdPersonCamera();
		}
	}

	private static void UpdateThirdPersonCamera()
	{
		if (thirdPersonCamera == null || localPlayer == null)
		{
			return;
		}
		try
		{
			Vector3 position = localPlayer.transform.position;
			Vector3 forward = localPlayer.transform.forward;
			Vector3 vector = position + Vector3.up * 1.7f;
			if (headBone != null)
			{
				vector = headBone.position;
			}
			Vector3 vector2 = vector + -forward * cameraDistance + Vector3.up * cameraHeight;
			Vector3 forward2 = vector - vector2;
			Quaternion quaternion = Quaternion.identity;
			if (forward2.magnitude > 0.01f)
			{
				quaternion = Quaternion.LookRotation(forward2, Vector3.up);
			}
			if (smoothTime > 0f)
			{
				thirdPersonCamera.transform.position = Vector3.SmoothDamp(thirdPersonCamera.transform.position, vector2, ref currentVelocity, smoothTime);
				thirdPersonCamera.transform.rotation = Quaternion.Slerp(thirdPersonCamera.transform.rotation, quaternion, Time.deltaTime * (1f / smoothTime));
			}
			else
			{
				thirdPersonCamera.transform.position = vector2;
				thirdPersonCamera.transform.rotation = quaternion;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error updating third person camera: " + ex.Message);
		}
	}

	private static void RestoreFirstPerson()
	{
		try
		{
			if (thirdPersonCamera != null)
			{
				thirdPersonCamera.enabled = false;
			}
			if (mainCamera != null)
			{
				mainCamera.enabled = true;
			}
			currentVelocity = Vector3.zero;
			MelonLogger.Msg("Restored first person view");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error restoring first person: " + ex.Message);
		}
	}

	public static void SetThirdPersonOffset(bool enabled)
	{
		if (!initialized)
		{
			Initialize();
			if (!initialized)
			{
				return;
			}
		}
		try
		{
			if (enabled)
			{
				if (mainCamera != null && headBone != null)
				{
					mainCamera.transform.SetParent(headBone, worldPositionStays: false);
					mainCamera.transform.localPosition = new Vector3(0f, 0.8f, -2.5f);
					mainCamera.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
					isThirdPerson = true;
					MelonLogger.Msg("Enabled third person offset view");
				}
			}
			else
			{
				RestoreFirstPersonOffset();
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error setting third person offset: " + ex.Message);
		}
	}

	private static void RestoreFirstPersonOffset()
	{
		try
		{
			if (mainCamera != null)
			{
				if (originalCameraParent != null)
				{
					mainCamera.transform.SetParent(originalCameraParent, worldPositionStays: false);
				}
				mainCamera.transform.localPosition = originalCameraPosition;
				mainCamera.transform.localRotation = originalCameraRotation;
			}
			SetHeadVisibility(visible: true);
			isThirdPerson = false;
			MelonLogger.Msg("Restored first person offset view");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error restoring first person offset: " + ex.Message);
		}
	}

	public static void SetCameraDistance(float distance)
	{
		cameraDistance = Mathf.Clamp(distance, 0.5f, 10f);
	}

	public static void SetCameraHeight(float height)
	{
		cameraHeight = Mathf.Clamp(height, -2f, 5f);
	}

	public static void SetSmoothTime(float time)
	{
		smoothTime = Mathf.Clamp(time, 0f, 1f);
	}

	public static void SetOffsetCameraPosition(Vector3 localPosition)
	{
		if (isThirdPerson && mainCamera != null && headBone != null)
		{
			mainCamera.transform.localPosition = localPosition;
		}
	}

	public static void SetOffsetCameraRotation(Vector3 eulerAngles)
	{
		if (isThirdPerson && mainCamera != null && headBone != null)
		{
			mainCamera.transform.localRotation = Quaternion.Euler(eulerAngles);
		}
	}

	public static void SetThirdPersonPreset(string preset)
	{
		if (isThirdPerson)
		{
			switch (preset.ToLower())
			{
			case "default":
				SetOffsetCameraPosition(new Vector3(0f, 0.8f, -2.5f));
				SetOffsetCameraRotation(new Vector3(15f, 0f, 0f));
				break;
			case "close":
				SetOffsetCameraPosition(new Vector3(0f, 0.5f, -1.5f));
				SetOffsetCameraRotation(new Vector3(10f, 0f, 0f));
				break;
			case "high":
				SetOffsetCameraPosition(new Vector3(0f, 1.5f, -3f));
				SetOffsetCameraRotation(new Vector3(25f, 0f, 0f));
				break;
			case "side":
				SetOffsetCameraPosition(new Vector3(1.5f, 0.5f, -1f));
				SetOffsetCameraRotation(new Vector3(0f, -30f, 0f));
				break;
			case "overhead":
				SetOffsetCameraPosition(new Vector3(0f, 3f, -1f));
				SetOffsetCameraRotation(new Vector3(45f, 0f, 0f));
				break;
			}
		}
	}

	public static void ToggleHeadVisibility()
	{
		SetHeadVisibility(!headVisible);
	}

	public static void ForceHeadVisibility(bool visible)
	{
		SetHeadVisibility(visible);
	}

	public static void DebugAvatarStructure()
	{
		try
		{
			if (!(localPlayer != null))
			{
				return;
			}
			Animator componentInChildren = localPlayer.GetComponentInChildren<Animator>();
			if (!(componentInChildren != null))
			{
				return;
			}
			MelonLogger.Msg("=== Avatar Debug Info ===");
			MelonLogger.Msg("Head bone: " + (headBone?.name ?? "NULL"));
			MelonLogger.Msg("Chest bone: " + (chestBone?.name ?? "NULL"));
			Renderer[] array = headRenderers;
			MelonLogger.Msg($"Head renderers found: {((array != null) ? array.Length : 0)}");
			if (headRenderers != null)
			{
				for (int i = 0; i < headRenderers.Length; i++)
				{
					Renderer renderer = headRenderers[i];
					MelonLogger.Msg(string.Format("  Renderer {0}: {1} - Enabled: {2}", i, renderer?.name ?? "NULL", renderer?.enabled ?? false));
				}
			}
			MelonLogger.Msg("Camera parent: " + (mainCamera?.transform?.parent?.name ?? "NULL"));
			MelonLogger.Msg($"Camera local pos: {mainCamera?.transform?.localPosition}");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error debugging avatar: " + ex.Message);
		}
	}

	public static void TryDifferentBone(string boneName)
	{
		try
		{
			if (!(localPlayer != null) || !isThirdPerson)
			{
				return;
			}
			Animator componentInChildren = localPlayer.GetComponentInChildren<Animator>();
			if (componentInChildren != null && Enum.TryParse<HumanBodyBones>(boneName, out var result))
			{
				Transform boneTransform = componentInChildren.GetBoneTransform(result);
				if (boneTransform != null && mainCamera != null)
				{
					mainCamera.transform.SetParent(boneTransform, worldPositionStays: false);
					mainCamera.transform.localPosition = new Vector3(0f, 1f, -2f);
					mainCamera.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
					MelonLogger.Msg("Attached camera to " + boneName + " bone: " + boneTransform.name);
				}
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error trying bone " + boneName + ": " + ex.Message);
		}
	}

	public static void OnPlayerJoined()
	{
		if (localPlayer == null)
		{
			localPlayer = Player.prop_Player_0;
			GetHeadBone();
		}
	}

	public static void OnAvatarChanged()
	{
		GetHeadBone();
		if (isThirdPerson)
		{
			SetHeadVisibility(visible: false);
		}
	}

	public static void Destroy()
	{
		if (isThirdPerson)
		{
			RestoreFirstPerson();
			RestoreFirstPersonOffset();
		}
		SetHeadVisibility(visible: true);
		if (thirdPersonCameraObject != null)
		{
			UnityEngine.Object.Destroy(thirdPersonCameraObject);
			thirdPersonCamera = null;
			thirdPersonCameraObject = null;
		}
		initialized = false;
		isThirdPerson = false;
		mainCamera = null;
		localPlayer = null;
		headBone = null;
		chestBone = null;
		headRenderers = null;
		currentVelocity = Vector3.zero;
		MelonLogger.Msg("Third Person Component destroyed");
	}
}
