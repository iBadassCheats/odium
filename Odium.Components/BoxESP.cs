using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using VRC;

namespace Odium.Components;

public class BoxESP
{
	public struct PlayerBoxData
	{
		public Player player;

		public Animator animator;

		public Transform rootBone;

		public Transform headBone;

		public bool isValid;

		public Bounds boundingBox;

		public float distanceToPlayer;

		public string playerName;

		public PlayerBoxData(Player plr, Animator anim)
		{
			player = plr;
			animator = anim;
			rootBone = null;
			headBone = null;
			isValid = anim != null;
			boundingBox = default(Bounds);
			distanceToPlayer = 0f;
			playerName = plr?.field_Private_APIUser_0?.displayName ?? "Unknown";
			if (anim != null && anim.isHuman)
			{
				rootBone = anim.GetBoneTransform(HumanBodyBones.Hips);
				headBone = anim.GetBoneTransform(HumanBodyBones.Head);
			}
		}
	}

	private static bool isEnabled = false;

	private static Color boxColor = Color.white;

	private static List<PlayerBoxData> playerBoxes = new List<PlayerBoxData>();

	private static Material lineMaterial;

	private static float boxHeight = 2f;

	private static float boxWidth = 0.6f;

	private static float boxDepth = 0.4f;

	private static bool showOnlyVisible = false;

	private static bool showPlayerNames = true;

	private static bool showDistance = true;

	public static bool IsEnabled => isEnabled;

	public static int PlayerCount => playerBoxes.Count;

	public static float BoxHeight => boxHeight;

	public static float BoxWidth => boxWidth;

	public static float BoxDepth => boxDepth;

	public static bool ShowPlayerNames => showPlayerNames;

	public static bool ShowDistance => showDistance;

	public static void Initialize()
	{
		CreateLineMaterial();
		MelonLogger.Msg("Box ESP initialized");
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
			playerBoxes.Clear();
		}
	}

	public static void SetBoxColor(Color color)
	{
		boxColor = color;
	}

	public static void SetBoxDimensions(float height, float width, float depth)
	{
		boxHeight = height;
		boxWidth = width;
		boxDepth = depth;
	}

	public static void SetShowOnlyVisible(bool onlyVisible)
	{
		showOnlyVisible = onlyVisible;
	}

	public static void SetShowPlayerNames(bool show)
	{
		showPlayerNames = show;
	}

	public static void SetShowDistance(bool show)
	{
		showDistance = show;
	}

	public static void RefreshPlayerList()
	{
		if (!isEnabled)
		{
			return;
		}
		playerBoxes.Clear();
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
						PlayerBoxData boxData = new PlayerBoxData(player, componentInChildren);
						CalculateBoundingBox(ref boxData);
						playerBoxes.Add(boxData);
					}
				}
			}
			MelonLogger.Msg($"Found {playerBoxes.Count} players for box ESP");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error refreshing player list: " + ex.Message);
		}
	}

	private static void CalculateBoundingBox(ref PlayerBoxData boxData)
	{
		if (!(boxData.rootBone == null))
		{
			Vector3 vector = boxData.rootBone.position;
			if (boxData.headBone != null)
			{
				vector = Vector3.Lerp(boxData.rootBone.position, boxData.headBone.position, 0.5f);
			}
			Vector3 size = new Vector3(boxWidth, boxHeight, boxDepth);
			boxData.boundingBox = new Bounds(vector, size);
			Player player = Player.prop_Player_0;
			if (player != null)
			{
				boxData.distanceToPlayer = Vector3.Distance(player.transform.position, vector);
			}
		}
	}

	public static void Update()
	{
		if (!isEnabled)
		{
			return;
		}
		for (int num = playerBoxes.Count - 1; num >= 0; num--)
		{
			if (playerBoxes[num].player == null || playerBoxes[num].animator == null)
			{
				playerBoxes.RemoveAt(num);
			}
			else
			{
				PlayerBoxData boxData = playerBoxes[num];
				CalculateBoundingBox(ref boxData);
				playerBoxes[num] = boxData;
			}
		}
	}

	public static void OnGUI()
	{
		if (!isEnabled || playerBoxes.Count == 0)
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
			GL.Color(boxColor);
			foreach (PlayerBoxData playerBox in playerBoxes)
			{
				if (playerBox.isValid)
				{
					DrawBoundingBox(playerBox.boundingBox, current);
				}
			}
			GL.End();
			GL.PopMatrix();
			if (showPlayerNames || showDistance)
			{
				DrawTextOverlays(current);
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error drawing box ESP: " + ex.Message);
		}
	}

	private static void DrawBoundingBox(Bounds bounds, Camera camera)
	{
		Vector3 center = bounds.center;
		Vector3 size = bounds.size;
		Vector3[] array = new Vector3[8]
		{
			center + new Vector3(0f - size.x, 0f - size.y, 0f - size.z) * 0.5f,
			center + new Vector3(size.x, 0f - size.y, 0f - size.z) * 0.5f,
			center + new Vector3(size.x, 0f - size.y, size.z) * 0.5f,
			center + new Vector3(0f - size.x, 0f - size.y, size.z) * 0.5f,
			center + new Vector3(0f - size.x, size.y, 0f - size.z) * 0.5f,
			center + new Vector3(size.x, size.y, 0f - size.z) * 0.5f,
			center + new Vector3(size.x, size.y, size.z) * 0.5f,
			center + new Vector3(0f - size.x, size.y, size.z) * 0.5f
		};
		Vector3[] array2 = new Vector3[8];
		bool flag = true;
		for (int i = 0; i < 8; i++)
		{
			array2[i] = camera.WorldToScreenPoint(array[i]);
			if (array2[i].z > 0f)
			{
				flag = false;
			}
		}
		if (!flag || !showOnlyVisible)
		{
			DrawLine(array2[0], array2[1]);
			DrawLine(array2[1], array2[2]);
			DrawLine(array2[2], array2[3]);
			DrawLine(array2[3], array2[0]);
			DrawLine(array2[4], array2[5]);
			DrawLine(array2[5], array2[6]);
			DrawLine(array2[6], array2[7]);
			DrawLine(array2[7], array2[4]);
			DrawLine(array2[0], array2[4]);
			DrawLine(array2[1], array2[5]);
			DrawLine(array2[2], array2[6]);
			DrawLine(array2[3], array2[7]);
		}
	}

	private static void DrawLine(Vector3 start, Vector3 end)
	{
		if (start.z > 0f && end.z > 0f)
		{
			GL.Vertex3(start.x, start.y, 0f);
			GL.Vertex3(end.x, end.y, 0f);
		}
	}

	private static void DrawTextOverlays(Camera camera)
	{
		foreach (PlayerBoxData playerBox in playerBoxes)
		{
			if (!playerBox.isValid)
			{
				continue;
			}
			Bounds boundingBox = playerBox.boundingBox;
			Vector3 center = boundingBox.center;
			Vector3 up = Vector3.up;
			boundingBox = playerBox.boundingBox;
			Vector3 position = center + up * (boundingBox.size.y * 0.5f);
			Vector3 vector = camera.WorldToScreenPoint(position);
			if (!(vector.z > 0f))
			{
				continue;
			}
			vector.y = (float)Screen.height - vector.y;
			string text = "";
			if (showPlayerNames)
			{
				text += playerBox.playerName;
			}
			if (showDistance)
			{
				if (text.Length > 0)
				{
					text += "\n";
				}
				text += $"{playerBox.distanceToPlayer:F1}m";
			}
			if (text.Length > 0)
			{
				GUI.color = boxColor;
				GUI.Label(new Rect(vector.x - 50f, vector.y - 30f, 100f, 50f), text);
				GUI.color = Color.white;
			}
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
			playerBoxes.RemoveAll((PlayerBoxData p) => p.player == player);
		}
	}

	public static void Destroy()
	{
		isEnabled = false;
		playerBoxes.Clear();
		if (lineMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(lineMaterial);
			lineMaterial = null;
		}
	}

	public static void SetBoxHeight(float height)
	{
		boxHeight = Mathf.Clamp(height, 0.5f, 4f);
	}

	public static void SetBoxWidth(float width)
	{
		boxWidth = Mathf.Clamp(width, 0.2f, 2f);
	}

	public static void SetBoxDepth(float depth)
	{
		boxDepth = Mathf.Clamp(depth, 0.2f, 2f);
	}
}
