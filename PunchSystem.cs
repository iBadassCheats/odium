using System.Collections;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using Odium;
using Odium.GameCheats;
using Odium.Patches;
using UnityEngine;
using VRC;

public static class PunchSystem
{
	private const float COOLDOWN_TIME = 0.3f;

	private const float DETECTION_INTERVAL = 0.05f;

	private const float PUNCH_DISTANCE = 0.25f;

	private static float _lastPunchTime;

	private static bool _isInitialized;

	private static Transform _leftHand;

	private static Transform _rightHand;

	public static void Initialize()
	{
		if (!_isInitialized)
		{
			MelonCoroutines.Start(SetupPunchDetection());
			_isInitialized = true;
		}
	}

	private static IEnumerator SetupPunchDetection()
	{
		OdiumConsole.Log("PunchSystem", "Initializing punch detection...");
		while (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
		{
			yield return null;
		}
		VRCPlayer vrcPlayer = VRCPlayer.field_Internal_Static_VRCPlayer_0;
		while (vrcPlayer.GetComponent<VRCAvatarManager>().field_Private_GameObject_0 == null)
		{
			yield return null;
		}
		Animator animator = vrcPlayer.GetComponent<VRCAvatarManager>().field_Private_GameObject_0.GetComponent<Animator>();
		if (animator == null)
		{
			OdiumConsole.Log("PunchSystem", "No animator found!");
			yield break;
		}
		_leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
		_rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
		if (_leftHand == null || _rightHand == null)
		{
			OdiumConsole.Log("PunchSystem", "Could not find hand transforms!");
			yield break;
		}
		MelonCoroutines.Start(PunchDetectionLoop());
		OdiumConsole.Log("PunchSystem", "Punch detection ready!");
	}

	private static IEnumerator PunchDetectionLoop()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.05f);
			CheckForPunches();
		}
	}

	private static void CheckForPunches()
	{
		if (Time.time - _lastPunchTime < 0.3f)
		{
			return;
		}
		System.Collections.Generic.List<Player> nonLocalPlayers = GetNonLocalPlayers();
		if (nonLocalPlayers.Count == 0)
		{
			return;
		}
		if (_rightHand != null)
		{
			foreach (Player item in nonLocalPlayers)
			{
				if (IsHandNearPlayer(_rightHand, item))
				{
					HandlePunch(item);
					return;
				}
			}
		}
		if (!(_leftHand != null))
		{
			return;
		}
		foreach (Player item2 in nonLocalPlayers)
		{
			if (IsHandNearPlayer(_leftHand, item2))
			{
				HandlePunch(item2);
				break;
			}
		}
	}

	private static bool IsHandNearPlayer(Transform hand, Player player)
	{
		try
		{
			Vector3 bonePosition = player.field_Private_VRCPlayerApi_0.GetBonePosition(HumanBodyBones.Head);
			Vector3 bonePosition2 = player.field_Private_VRCPlayerApi_0.GetBonePosition(HumanBodyBones.Chest);
			return Vector3.Distance(hand.position, bonePosition) < 0.25f || Vector3.Distance(hand.position, bonePosition2) < 0.25f;
		}
		catch
		{
			return false;
		}
	}

	private static System.Collections.Generic.List<Player> GetNonLocalPlayers()
	{
		System.Collections.Generic.List<Player> list = new System.Collections.Generic.List<Player>();
		Il2CppSystem.Collections.Generic.List<Player>.Enumerator enumerator = PlayerManager.prop_PlayerManager_0.field_Private_List_1_Player_0.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Player current = enumerator.Current;
			if (current != null && !current.field_Private_VRCPlayerApi_0.isLocal)
			{
				list.Add(current);
			}
		}
		return list;
	}

	private static void HandlePunch(Player targetPlayer)
	{
		_lastPunchTime = Time.time;
		OdiumConsole.Log("PunchSystem", "Punched " + targetPlayer.field_Private_VRCPlayerApi_0.displayName + "!");
		SendUdonEvent(targetPlayer);
	}

	private static void SendUdonEvent(Player targetPlayer)
	{
		PhotonPatches.BlockUdon = true;
		for (int i = 0; i < 100; i++)
		{
			Murder4Utils.SendTargetedPatreonEvent(targetPlayer, "ListPatrons");
		}
		PhotonPatches.BlockUdon = false;
	}
}
