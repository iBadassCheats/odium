using System;
using System.Collections.Generic;
using Odium.Wrappers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Odium.Components;

public class GhostMode
{
	public static Vector3 originalGhostPosition;

	public static GameObject avatarClone;

	public static List<GameObject> clonedAvatarObjects = new List<GameObject>();

	public static Quaternion originalGhostRotation;

	public static bool isEnabled = false;

	public static void ToggleGhost(bool enable)
	{
		ActionWrapper.serialize = enable;
		isEnabled = enable;
		if (enable)
		{
			GameObject gameObject = null;
			foreach (GameObject rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
			{
				if (rootGameObject.name.StartsWith("VRCPlayer[Local]"))
				{
					gameObject = rootGameObject;
					break;
				}
			}
			if (gameObject == null)
			{
				return;
			}
			originalGhostPosition = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position;
			originalGhostRotation = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.rotation;
			try
			{
				avatarClone = UnityEngine.Object.Instantiate(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCAvatarManager_0.field_Private_GameObject_0, null, worldPositionStays: true);
				Animator component = avatarClone.GetComponent<Animator>();
				avatarClone.transform.position = originalGhostPosition;
				avatarClone.transform.rotation = originalGhostRotation;
				if (component != null && component.isHuman)
				{
					Transform boneTransform = component.GetBoneTransform(HumanBodyBones.Head);
					if (boneTransform != null)
					{
						boneTransform.localScale = Vector3.one;
					}
				}
				avatarClone.name = "Cloned Avatar";
				component.enabled = false;
				avatarClone.GetComponent<VRCVrIkController>().enabled = false;
				avatarClone.transform.position = gameObject.transform.position;
				avatarClone.transform.rotation = gameObject.transform.rotation;
				return;
			}
			catch (Exception)
			{
				return;
			}
		}
		UnityEngine.Object.Destroy(avatarClone);
	}
}
