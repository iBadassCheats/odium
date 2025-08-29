using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace CursorLayerMod;

public class CursorLayerMod : MelonMod
{
	public const int TARGET_LAYER = 9999;

	public const string CURSOR_PATH = "CursorManager/MouseArrow/VRCUICursorIcon";

	public static bool hasSetLayer;

	public new static void OnUpdate()
	{
		if (!hasSetLayer)
		{
			SetCursorLayer();
		}
	}

	public new static void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
		hasSetLayer = false;
		MelonLogger.Msg("Scene loaded: " + sceneName + ", resetting cursor layer flag");
	}

	public static void SetCursorLayer()
	{
		GameObject gameObject = FindCursorIcon();
		if (gameObject != null)
		{
			bool flag = false;
			Canvas component = gameObject.GetComponent<Canvas>();
			if (component != null)
			{
				component.sortingOrder = 9999;
				component.overrideSorting = true;
				flag = true;
				MelonLogger.Msg($"Set Canvas sorting order to {9999}");
			}
			Renderer component2 = gameObject.GetComponent<Renderer>();
			if (component2 != null)
			{
				component2.sortingOrder = 9999;
				flag = true;
				MelonLogger.Msg($"Set Renderer sorting order to {9999}");
			}
			Graphic component3 = gameObject.GetComponent<Graphic>();
			if (component3 != null && component3.canvas != null)
			{
				component3.canvas.sortingOrder = 9999;
				component3.canvas.overrideSorting = true;
				flag = true;
				MelonLogger.Msg($"Set Graphic Canvas sorting order to {9999}");
			}
			Transform parent = gameObject.transform.parent;
			if (parent != null)
			{
				gameObject.transform.SetAsLastSibling();
				flag = true;
				MelonLogger.Msg("Set cursor as last sibling in hierarchy");
			}
			if (flag)
			{
				hasSetLayer = true;
				MelonLogger.Msg("Successfully set cursor to highest layer!");
			}
		}
	}

	public static GameObject FindCursorIcon()
	{
		GameObject gameObject = GameObject.Find("CursorManager/MouseArrow/VRCUICursorIcon");
		if (gameObject != null)
		{
			return gameObject;
		}
		GameObject gameObject2 = GameObject.Find("CursorManager");
		if (gameObject2 != null)
		{
			Transform transform = gameObject2.transform.Find("MouseArrow/VRCUICursorIcon");
			if (transform != null)
			{
				return transform.gameObject;
			}
			transform = gameObject2.transform.Find("VRCUICursorIcon");
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		GameObject[] array = Resources.FindObjectsOfTypeAll<GameObject>();
		GameObject[] array2 = array;
		foreach (GameObject gameObject3 in array2)
		{
			if (gameObject3.name.Contains("VRCUICursorIcon") || gameObject3.name.Contains("CursorIcon"))
			{
				return gameObject3;
			}
		}
		return null;
	}
}
