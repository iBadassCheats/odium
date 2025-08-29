using System;
using System.Collections;
using System.IO;
using MelonLoader;
using Odium.Components;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Odium.Modules;

public static class OdiumNotificationLoader
{
	private static AssetBundle _notificationBundle;

	private static GameObject _notificationPrefab;

	private static bool _isInitialized = false;

	private static readonly string NotificationBundlePath = Path.Combine(ModSetup.GetOdiumFolderPath(), "AssetBundles", "notification");

	public static void Initialize()
	{
		if (_isInitialized)
		{
			OdiumConsole.Log("NotificationLoader", "Already initialized, skipping...");
			return;
		}
		OdiumConsole.Log("NotificationLoader", "Starting notification initialization...");
		LoadNotifications();
	}

	private static void LoadNotifications()
	{
		try
		{
			if (!File.Exists(NotificationBundlePath))
			{
				OdiumConsole.Log("NotificationLoader", "Notification bundle file not found at: " + NotificationBundlePath, LogLevel.Error);
				return;
			}
			OdiumConsole.Log("NotificationLoader", "Loading AssetBundle from file...");
			_notificationBundle = AssetBundle.LoadFromFile(NotificationBundlePath);
			if (_notificationBundle == null)
			{
				OdiumConsole.Log("NotificationLoader", "Failed to load AssetBundle!", LogLevel.Error);
				return;
			}
			OdiumConsole.Log("NotificationLoader", "AssetBundle loaded successfully!");
			string[] array = _notificationBundle.GetAllAssetNames();
			OdiumConsole.Log("NotificationLoader", $"Found {array.Length} assets in bundle:");
			string[] array2 = array;
			foreach (string text in array2)
			{
				OdiumConsole.Log("NotificationLoader", "  - " + text);
			}
			string[] array3 = new string[4] { "Notification", "notification", "assets/notification.prefab", "notification.prefab" };
			string[] array4 = array3;
			foreach (string text2 in array4)
			{
				OdiumConsole.Log("NotificationLoader", "Trying to load prefab with name: '" + text2 + "'");
				_notificationPrefab = _notificationBundle.LoadAsset<GameObject>(text2);
				if (_notificationPrefab != null)
				{
					OdiumConsole.Log("NotificationLoader", "Successfully loaded prefab with name: '" + text2 + "'");
					break;
				}
			}
			if (_notificationPrefab == null)
			{
				OdiumConsole.Log("NotificationLoader", "Standard names failed, trying to find any GameObject...", LogLevel.Warning);
				string[] array5 = array;
				foreach (string text3 in array5)
				{
					GameObject gameObject = _notificationBundle.LoadAsset<GameObject>(text3);
					if (gameObject != null)
					{
						_notificationPrefab = gameObject;
						OdiumConsole.Log("NotificationLoader", "Using GameObject asset: " + text3);
						break;
					}
				}
			}
			if (_notificationPrefab == null)
			{
				OdiumConsole.Log("NotificationLoader", "Failed to load any GameObject from AssetBundle!", LogLevel.Error);
				_notificationBundle.Unload(unloadAllLoadedObjects: true);
				_notificationBundle = null;
			}
			else
			{
				_notificationBundle.Unload(unloadAllLoadedObjects: false);
				_notificationBundle = null;
				_isInitialized = true;
				OdiumConsole.LogGradient("NotificationLoader", "Notification system initialized successfully!", LogLevel.Info, gradientCategory: true);
				OdiumConsole.Log("NotificationLoader", "Prefab reference: " + _notificationPrefab.name);
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "LoadNotifications");
			_isInitialized = false;
			_notificationPrefab = null;
			if (_notificationBundle != null)
			{
				_notificationBundle.Unload(unloadAllLoadedObjects: true);
				_notificationBundle = null;
			}
		}
	}

	private static Transform FindOrCreateNotificationCanvas()
	{
		try
		{
			Scene activeScene = SceneManager.GetActiveScene();
			OdiumConsole.Log("NotificationLoader", "Current active scene: " + activeScene.name);
			Canvas canvas = FindBestExistingCanvas(activeScene);
			if (canvas != null)
			{
				OdiumConsole.Log("NotificationLoader", "Using existing canvas: " + canvas.name + " in scene: " + activeScene.name);
				return canvas.transform;
			}
			GameObject gameObject = new GameObject("OdiumNotificationCanvas");
			SceneManager.MoveGameObjectToScene(gameObject, activeScene);
			Canvas canvas2 = gameObject.AddComponent<Canvas>();
			canvas2.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas2.sortingOrder = 32767;
			canvas2.pixelPerfect = false;
			CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			canvasScaler.matchWidthOrHeight = 0.5f;
			GraphicRaycaster graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
			graphicRaycaster.ignoreReversedGraphics = true;
			graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
			OdiumConsole.Log("NotificationLoader", "Created new notification canvas in active scene: " + activeScene.name);
			return gameObject.transform;
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "FindOrCreateNotificationCanvas");
			return null;
		}
	}

	private static Canvas FindBestExistingCanvas(Scene scene)
	{
		try
		{
			GameObject[] array = scene.GetRootGameObjects();
			Canvas canvas = null;
			int num = -1;
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				Canvas[] array3 = gameObject.GetComponentsInChildren<Canvas>();
				Canvas[] array4 = array3;
				foreach (Canvas canvas2 in array4)
				{
					if (canvas2.renderMode == RenderMode.ScreenSpaceOverlay && canvas2.sortingOrder > num)
					{
						canvas = canvas2;
						num = canvas2.sortingOrder;
					}
				}
			}
			if (canvas != null)
			{
				OdiumConsole.Log("NotificationLoader", $"Found suitable existing canvas: {canvas.name} with sort order: {num}");
			}
			return canvas;
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "FindBestExistingCanvas");
			return null;
		}
	}

	public static void ShowNotification(string text)
	{
		OdiumConsole.Log("NotificationLoader", $"ShowNotification called - Initialized: {_isInitialized}, Prefab null: {_notificationPrefab == null}");
		if (!_isInitialized)
		{
			OdiumConsole.Log("NotificationLoader", "Notification system not initialized!", LogLevel.Error);
			return;
		}
		if (_notificationPrefab == null)
		{
			OdiumConsole.Log("NotificationLoader", "Notification prefab is null! Attempting to reinitialize...", LogLevel.Error);
			_isInitialized = false;
			Initialize();
			if (_notificationPrefab == null)
			{
				OdiumConsole.Log("NotificationLoader", "Reinitialization failed - prefab still null!", LogLevel.Error);
				return;
			}
		}
		try
		{
			Scene activeScene = SceneManager.GetActiveScene();
			OdiumConsole.Log("NotificationLoader", "Target scene: " + activeScene.name);
			GameObject gameObject = UnityEngine.Object.Instantiate(_notificationPrefab);
			gameObject.name = $"OdiumNotification_{DateTime.Now.Ticks}";
			SceneManager.MoveGameObjectToScene(gameObject, activeScene);
			OdiumConsole.Log("NotificationLoader", "Instantiated notification in scene: " + gameObject.scene.name);
			Canvas component = gameObject.GetComponent<Canvas>();
			if (component != null)
			{
				component.enabled = true;
				component.renderMode = RenderMode.ScreenSpaceOverlay;
				component.sortingOrder = 32768;
				component.overrideSorting = true;
				component.pixelPerfect = false;
				OdiumConsole.Log("NotificationLoader", $"Configured Canvas - Enabled: {component.enabled}, SortOrder: {component.sortingOrder}");
			}
			else
			{
				OdiumConsole.Log("NotificationLoader", "No Canvas component found on notification prefab!", LogLevel.Warning);
			}
			gameObject.SetActive(value: true);
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			if (component2 != null)
			{
				component2.localScale = Vector3.one;
				component2.localPosition = Vector3.zero;
				component2.localEulerAngles = Vector3.zero;
				component2.anchorMin = new Vector2(1f, 1f);
				component2.anchorMax = new Vector2(1f, 1f);
				component2.pivot = new Vector2(1f, 1f);
				component2.anchoredPosition = new Vector2(-50f, -50f);
				if (component2.sizeDelta.x <= 0f || component2.sizeDelta.y <= 0f)
				{
					component2.sizeDelta = new Vector2(300f, 100f);
				}
				component2.localScale = new Vector3(1.1f, 1.1f, 1f);
				OdiumConsole.Log("NotificationLoader", $"Positioned notification - AnchoredPos: {component2.anchoredPosition}, Size: {component2.sizeDelta}");
			}
			TextMeshProUGUI componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>();
			if (componentInChildren != null)
			{
				componentInChildren.text = text;
				componentInChildren.enabled = true;
				componentInChildren.gameObject.SetActive(value: true);
				OdiumConsole.Log("NotificationLoader", "Set notification text: '" + text + "'");
			}
			else
			{
				Text componentInChildren2 = gameObject.GetComponentInChildren<Text>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.text = text;
					componentInChildren2.enabled = true;
					componentInChildren2.gameObject.SetActive(value: true);
					OdiumConsole.Log("NotificationLoader", "Set notification text using legacy Text component: '" + text + "'");
				}
				else
				{
					OdiumConsole.Log("NotificationLoader", "Could not find any text component!", LogLevel.Warning);
				}
			}
			Transform[] array = gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
			Transform[] array2 = array;
			foreach (Transform transform in array2)
			{
				transform.gameObject.SetActive(value: true);
			}
			Behaviour[] array3 = gameObject.GetComponentsInChildren<Behaviour>(includeInactive: true);
			Behaviour[] array4 = array3;
			foreach (Behaviour behaviour in array4)
			{
				behaviour.enabled = true;
			}
			Canvas.ForceUpdateCanvases();
			if (component2 != null)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(component2);
			}
			OdiumConsole.Log("NotificationLoader", "Final notification scene: " + gameObject.scene.name);
			MelonCoroutines.Start(DestroyNotificationAfterDelay(gameObject, 9f));
			OdiumConsole.LogGradient("NotificationLoader", "Notification shown: " + text);
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "ShowNotification");
		}
	}

	private static IEnumerator DestroyNotificationAfterDelay(GameObject notification, float delay)
	{
		yield return new WaitForSeconds(delay);
		try
		{
			if (notification != null)
			{
				OdiumConsole.Log("NotificationLoader", "Auto-destroying notification: " + notification.name);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			OdiumConsole.LogException(ex2, "DestroyNotificationAfterDelay");
		}
	}

	public static bool IsInitialized()
	{
		bool flag = _isInitialized && _notificationPrefab != null;
		OdiumConsole.Log("NotificationLoader", $"IsInitialized check - _isInitialized: {_isInitialized}, _notificationPrefab != null: {_notificationPrefab != null}, Result: {flag}");
		return flag;
	}

	public static void Cleanup()
	{
		try
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (!sceneAt.IsValid() || !sceneAt.isLoaded)
				{
					continue;
				}
				GameObject[] array = sceneAt.GetRootGameObjects();
				GameObject[] array2 = array;
				foreach (GameObject gameObject in array2)
				{
					if (gameObject.name == "OdiumNotificationCanvas")
					{
						UnityEngine.Object.Destroy(gameObject);
						OdiumConsole.Log("NotificationLoader", "Destroyed notification canvas in scene: " + sceneAt.name);
					}
				}
			}
			if (_notificationBundle != null)
			{
				_notificationBundle.Unload(unloadAllLoadedObjects: true);
				_notificationBundle = null;
			}
			_notificationPrefab = null;
			_isInitialized = false;
			OdiumConsole.Log("NotificationLoader", "Notification system cleaned up");
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "NotificationCleanup");
		}
	}
}
