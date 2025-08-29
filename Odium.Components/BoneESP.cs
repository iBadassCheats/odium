using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using VRC;

namespace Odium.Components;

public class BoneESP
{
	public struct PlayerBoneData
	{
		public Player player;

		public Animator animator;

		public List<BoneConnection> connections;

		public bool isValid;

		public PlayerBoneData(Player plr, Animator anim)
		{
			player = plr;
			animator = anim;
			connections = new List<BoneConnection>();
			isValid = anim != null;
		}
	}

	public struct BoneConnection
	{
		public Transform startBone;

		public Transform endBone;

		public HumanBodyBones startBoneType;

		public HumanBodyBones endBoneType;

		public BoneConnection(Transform start, Transform end, HumanBodyBones startType, HumanBodyBones endType)
		{
			startBone = start;
			endBone = end;
			startBoneType = startType;
			endBoneType = endType;
		}
	}

	private static bool isEnabled = false;

	private static Color boneColor = Color.white;

	private static List<PlayerBoneData> playerBones = new List<PlayerBoneData>();

	private static Material lineMaterial;

	private static readonly (HumanBodyBones, HumanBodyBones)[] boneConnections = new(HumanBodyBones, HumanBodyBones)[39]
	{
		(HumanBodyBones.Hips, HumanBodyBones.Spine),
		(HumanBodyBones.Spine, HumanBodyBones.Chest),
		(HumanBodyBones.Chest, HumanBodyBones.UpperChest),
		(HumanBodyBones.UpperChest, HumanBodyBones.Neck),
		(HumanBodyBones.Neck, HumanBodyBones.Head),
		(HumanBodyBones.UpperChest, HumanBodyBones.LeftShoulder),
		(HumanBodyBones.LeftShoulder, HumanBodyBones.LeftUpperArm),
		(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm),
		(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand),
		(HumanBodyBones.UpperChest, HumanBodyBones.RightShoulder),
		(HumanBodyBones.RightShoulder, HumanBodyBones.RightUpperArm),
		(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm),
		(HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand),
		(HumanBodyBones.Hips, HumanBodyBones.LeftUpperLeg),
		(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg),
		(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot),
		(HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes),
		(HumanBodyBones.Hips, HumanBodyBones.RightUpperLeg),
		(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg),
		(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot),
		(HumanBodyBones.RightFoot, HumanBodyBones.RightToes),
		(HumanBodyBones.LeftHand, HumanBodyBones.LeftThumbProximal),
		(HumanBodyBones.LeftThumbProximal, HumanBodyBones.LeftThumbIntermediate),
		(HumanBodyBones.LeftThumbIntermediate, HumanBodyBones.LeftThumbDistal),
		(HumanBodyBones.LeftHand, HumanBodyBones.LeftIndexProximal),
		(HumanBodyBones.LeftIndexProximal, HumanBodyBones.LeftIndexIntermediate),
		(HumanBodyBones.LeftIndexIntermediate, HumanBodyBones.LeftIndexDistal),
		(HumanBodyBones.LeftHand, HumanBodyBones.LeftMiddleProximal),
		(HumanBodyBones.LeftMiddleProximal, HumanBodyBones.LeftMiddleIntermediate),
		(HumanBodyBones.LeftMiddleIntermediate, HumanBodyBones.LeftMiddleDistal),
		(HumanBodyBones.RightHand, HumanBodyBones.RightThumbProximal),
		(HumanBodyBones.RightThumbProximal, HumanBodyBones.RightThumbIntermediate),
		(HumanBodyBones.RightThumbIntermediate, HumanBodyBones.RightThumbDistal),
		(HumanBodyBones.RightHand, HumanBodyBones.RightIndexProximal),
		(HumanBodyBones.RightIndexProximal, HumanBodyBones.RightIndexIntermediate),
		(HumanBodyBones.RightIndexIntermediate, HumanBodyBones.RightIndexDistal),
		(HumanBodyBones.RightHand, HumanBodyBones.RightMiddleProximal),
		(HumanBodyBones.RightMiddleProximal, HumanBodyBones.RightMiddleIntermediate),
		(HumanBodyBones.RightMiddleIntermediate, HumanBodyBones.RightMiddleDistal)
	};

	public static bool IsEnabled => isEnabled;

	public static int PlayerCount => playerBones.Count;

	public static void Initialize()
	{
		CreateLineMaterial();
		MelonLogger.Msg("Bone ESP initialized");
	}

	private static void CreateLineMaterial()
	{
		if (lineMaterial == null)
		{
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			lineMaterial = new Material(shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.SetInt("_SrcBlend", 5);
			lineMaterial.SetInt("_DstBlend", 10);
			lineMaterial.SetInt("_Cull", 0);
			lineMaterial.SetInt("_ZTest", 8);
			lineMaterial.SetInt("_ZWrite", 0);
		}
	}

	public static void SetEnabled(bool enabled)
	{
		isEnabled = enabled;
		if (enabled)
		{
			RefreshPlayerList();
		}
		else
		{
			playerBones.Clear();
		}
	}

	public static void SetBoneColor(Color color)
	{
		boneColor = color;
	}

	public static void RefreshPlayerList()
	{
		if (!isEnabled)
		{
			return;
		}
		playerBones.Clear();
		try
		{
			Player[] array = UnityEngine.Object.FindObjectsOfType<Player>();
			Player[] array2 = array;
			foreach (Player player in array2)
			{
				if (player == null || player.gameObject == null)
				{
					continue;
				}
				Player player2 = Player.prop_Player_0;
				if (!(player2 != null) || !(player.gameObject == player2.gameObject))
				{
					Animator componentInChildren = player.GetComponentInChildren<Animator>();
					if (componentInChildren != null && componentInChildren.isHuman)
					{
						PlayerBoneData boneData = new PlayerBoneData(player, componentInChildren);
						SetupBoneConnections(ref boneData);
						playerBones.Add(boneData);
					}
				}
			}
			MelonLogger.Msg($"Found {playerBones.Count} players with valid bone data");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error refreshing player list: " + ex.Message);
		}
	}

	private static void SetupBoneConnections(ref PlayerBoneData boneData)
	{
		boneData.connections.Clear();
		(HumanBodyBones, HumanBodyBones)[] array = boneConnections;
		for (int i = 0; i < array.Length; i++)
		{
			(HumanBodyBones, HumanBodyBones) tuple = array[i];
			Transform boneTransform = boneData.animator.GetBoneTransform(tuple.Item1);
			Transform boneTransform2 = boneData.animator.GetBoneTransform(tuple.Item2);
			if (boneTransform != null && boneTransform2 != null)
			{
				BoneConnection item = new BoneConnection(boneTransform, boneTransform2, tuple.Item1, tuple.Item2);
				boneData.connections.Add(item);
			}
		}
	}

	public static void Update()
	{
		if (!isEnabled)
		{
			return;
		}
		for (int num = playerBones.Count - 1; num >= 0; num--)
		{
			if (playerBones[num].player == null || playerBones[num].animator == null)
			{
				playerBones.RemoveAt(num);
			}
		}
	}

	public static void OnGUI()
	{
		if (!isEnabled || playerBones.Count == 0)
		{
			return;
		}
		Camera current = Camera.current;
		if (current == null)
		{
			return;
		}
		try
		{
			GL.PushMatrix();
			lineMaterial.SetPass(0);
			GL.LoadPixelMatrix();
			GL.Begin(1);
			GL.Color(boneColor);
			foreach (PlayerBoneData playerBone in playerBones)
			{
				if (!playerBone.isValid || playerBone.connections == null)
				{
					continue;
				}
				foreach (BoneConnection connection in playerBone.connections)
				{
					if (!(connection.startBone == null) && !(connection.endBone == null))
					{
						Vector3 vector = current.WorldToScreenPoint(connection.startBone.position);
						Vector3 vector2 = current.WorldToScreenPoint(connection.endBone.position);
						if (vector.z > 0f && vector2.z > 0f)
						{
							GL.Vertex3(vector.x, vector.y, 0f);
							GL.Vertex3(vector2.x, vector2.y, 0f);
						}
					}
				}
			}
			GL.End();
			GL.PopMatrix();
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error drawing bone ESP: " + ex.Message);
		}
	}

	public static void OnPlayerJoined(Player player)
	{
		if (isEnabled)
		{
			RefreshPlayerList();
		}
	}

	public static void OnPlayerLeft(Player player)
	{
		if (isEnabled)
		{
			playerBones.RemoveAll((PlayerBoneData p) => p.player == player);
		}
	}

	public static void Destroy()
	{
		isEnabled = false;
		playerBones.Clear();
		if (lineMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(lineMaterial);
			lineMaterial = null;
		}
	}
}
