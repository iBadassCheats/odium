using System;
using System.Collections;
using System.Linq;
using UnhollowerBaseLib;
using UnityEngine;
using VRC;
using VRC.Networking;
using VRC.SDK.Internal;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRCSDK2;

namespace Odium.Components;

public class OnLoadedSceneManager
{
	internal static float oldRespawnHight;

	internal static Il2CppArrayBase<VRCSDK2.VRC_Pickup> sdk2Items;

	internal static Il2CppArrayBase<VRCPickup> sdk3Items;

	internal static VRC_ObjectSync[] allSyncItems;

	internal static Il2CppArrayBase<VRCObjectSync> allSDK3SyncItems;

	internal static Il2CppArrayBase<VRCObjectPool> allPoolItems;

	internal static VRC.SDKBase.VRC_Pickup[] allBaseUdonItem;

	internal static Il2CppArrayBase<VRCInteractable> allInteractable;

	internal static Il2CppArrayBase<VRC.SDKBase.VRC_Interactable> allBaseInteractable;

	internal static Il2CppArrayBase<VRCSDK2.VRC_Interactable> allSDK2Interactable;

	internal static Il2CppArrayBase<VRC.SDKBase.VRC_Trigger> allTriggers;

	internal static Il2CppArrayBase<VRCSDK2.VRC_Trigger> allSDK2Triggers;

	internal static Il2CppArrayBase<VRC.SDKBase.VRC_TriggerColliderEventTrigger> allTriggerCol;

	private static VRC.SDKBase.VRC_SceneDescriptor SceneDescriptor;

	private static VRCSDK2.VRC_SceneDescriptor SDK2SceneDescriptor;

	private static VRCSceneDescriptor SDK3SceneDescriptor;

	internal static HighlightsFXStandalone highlightsFX;

	internal static UdonBehaviour[] udonBehaviours;

	internal static Il2CppArrayBase<UdonSync> udonSync;

	internal static Il2CppArrayBase<UdonManager> udonManagers = Resources.FindObjectsOfTypeAll<UdonManager>();

	internal static Il2CppArrayBase<OnTriggerStayProxy> udonOnTrigger;

	internal static Il2CppArrayBase<OnCollisionStayProxy> udonOnCol;

	internal static Il2CppArrayBase<OnRenderObjectProxy> udonOnRender;

	internal static Il2CppArrayBase<VRCUdonAnalytics> allSDKUdon;

	private static readonly string[] toSkip = new string[5] { "PhotoCamera", "MirrorPickup", "ViewFinder", "AvatarDebugConsole", "OscDebugConsole" };

	internal static GameObject DeepCoreRpcObject;

	public static void LoadedScene(int buildindex, string sceneName)
	{
		Console.WriteLine("Loaded Scene: /n" + buildindex + sceneName);
	}

	public static IEnumerator WaitForLocalPlayer()
	{
		Console.WriteLine("SceneManagerWaiting for localplayer...");
		while (Player.prop_Player_0 == null)
		{
			yield return null;
		}
		DeepCoreRpcObject = new GameObject("[DO NOT TOUCH] DeepClientRPC");
		allTriggers = Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Trigger>();
		allSDK2Triggers = Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_Trigger>();
		allTriggerCol = Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_TriggerColliderEventTrigger>();
		allInteractable = Resources.FindObjectsOfTypeAll<VRCInteractable>();
		allBaseInteractable = Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Interactable>();
		allSDK2Interactable = Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_Interactable>();
		sdk2Items = Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_Pickup>();
		sdk3Items = Resources.FindObjectsOfTypeAll<VRCPickup>();
		allSyncItems = Resources.FindObjectsOfTypeAll<VRC_ObjectSync>();
		allSDK3SyncItems = Resources.FindObjectsOfTypeAll<VRCObjectSync>();
		allPoolItems = Resources.FindObjectsOfTypeAll<VRCObjectPool>();
		sdk3Items = UnityEngine.Object.FindObjectsOfType<VRCPickup>();
		allSyncItems = (from x in Resources.FindObjectsOfTypeAll<VRC_ObjectSync>()
			where !toSkip.Any((string y) => y.Contains(x.gameObject.name))
			select x).ToArray();
		allBaseUdonItem = (from x in Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Pickup>()
			where !toSkip.Any((string y) => y.Contains(x.gameObject.name))
			select x).ToArray();
		SceneDescriptor = UnityEngine.Object.FindObjectOfType<VRC.SDKBase.VRC_SceneDescriptor>(includeInactive: true);
		SDK2SceneDescriptor = UnityEngine.Object.FindObjectOfType<VRCSDK2.VRC_SceneDescriptor>(includeInactive: true);
		SDK3SceneDescriptor = UnityEngine.Object.FindObjectOfType<VRCSceneDescriptor>(includeInactive: true);
		udonBehaviours = UnityEngine.Object.FindObjectsOfType<UdonBehaviour>();
		udonSync = Resources.FindObjectsOfTypeAll<UdonSync>();
		udonManagers = Resources.FindObjectsOfTypeAll<UdonManager>();
		udonOnTrigger = Resources.FindObjectsOfTypeAll<OnTriggerStayProxy>();
		udonOnCol = Resources.FindObjectsOfTypeAll<OnCollisionStayProxy>();
		udonOnRender = Resources.FindObjectsOfTypeAll<OnRenderObjectProxy>();
		allSDKUdon = Resources.FindObjectsOfTypeAll<VRCUdonAnalytics>();
		if (highlightsFX == null)
		{
			highlightsFX = Resources.FindObjectsOfTypeAll<HighlightsFXStandalone>().FirstOrDefault();
		}
	}
}
