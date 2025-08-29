using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using Odium.Components;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Odium.Modules;

public static class OdiumInputDialog
{
	public delegate void InputCallback(string input, bool wasSubmitted);

	private static AssetBundle _inputBundle;

	private static GameObject _inputDialogPrefab;

	private static bool _isInitialized = false;

	private static List<GameObject> _activeInputDialogs = new List<GameObject>();

	private static readonly string InputBundlePath = Path.Combine(ModSetup.GetOdiumFolderPath(), "AssetBundles", "inputdialog");

	public static void Initialize()
	{
		if (_isInitialized)
		{
			OdiumConsole.Log("InputDialog", "Already initialized, skipping...");
			return;
		}
		OdiumConsole.Log("InputDialog", "Starting input dialog initialization...");
		LoadInputDialog();
	}

	private static void LoadInputDialog()
	{
		try
		{
			if (!File.Exists(InputBundlePath))
			{
				OdiumConsole.Log("InputDialog", "Input dialog bundle file not found at: " + InputBundlePath, LogLevel.Error);
				return;
			}
			OdiumConsole.Log("InputDialog", "Loading AssetBundle from file...");
			_inputBundle = AssetBundle.LoadFromFile(InputBundlePath);
			if (_inputBundle == null)
			{
				OdiumConsole.Log("InputDialog", "Failed to load AssetBundle!", LogLevel.Error);
				return;
			}
			OdiumConsole.Log("InputDialog", "AssetBundle loaded successfully!");
			string[] array = _inputBundle.GetAllAssetNames();
			OdiumConsole.Log("InputDialog", $"Found {array.Length} assets in bundle:");
			string[] array2 = array;
			foreach (string text in array2)
			{
				OdiumConsole.Log("InputDialog", "  - " + text);
			}
			string[] array3 = new string[4] { "OdiumInputSystem", "inputdialog", "assets/inputdialog.prefab", "inputdialog.prefab" };
			string[] array4 = array3;
			foreach (string text2 in array4)
			{
				OdiumConsole.Log("InputDialog", "Trying to load prefab with name: '" + text2 + "'");
				_inputDialogPrefab = _inputBundle.LoadAsset<GameObject>(text2);
				if (_inputDialogPrefab != null)
				{
					OdiumConsole.Log("InputDialog", "Successfully loaded prefab with name: '" + text2 + "'");
					break;
				}
			}
			if (_inputDialogPrefab == null)
			{
				OdiumConsole.Log("InputDialog", "Standard names failed, trying to find any GameObject...", LogLevel.Warning);
				string[] array5 = array;
				foreach (string text3 in array5)
				{
					GameObject gameObject = _inputBundle.LoadAsset<GameObject>(text3);
					if (gameObject != null)
					{
						_inputDialogPrefab = gameObject;
						OdiumConsole.Log("InputDialog", "Using GameObject asset: " + text3);
						break;
					}
				}
			}
			if (_inputDialogPrefab == null)
			{
				OdiumConsole.Log("InputDialog", "Failed to load any GameObject from AssetBundle!", LogLevel.Error);
				_inputBundle.Unload(unloadAllLoadedObjects: true);
				_inputBundle = null;
			}
			else
			{
				_inputBundle.Unload(unloadAllLoadedObjects: false);
				_inputBundle = null;
				_isInitialized = true;
				OdiumConsole.LogGradient("InputDialog", "Input dialog system initialized successfully!", LogLevel.Info, gradientCategory: true);
				OdiumConsole.Log("InputDialog", "Prefab reference: " + _inputDialogPrefab.name);
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "LoadInputDialog");
			_isInitialized = false;
			_inputDialogPrefab = null;
			if (_inputBundle != null)
			{
				_inputBundle.Unload(unloadAllLoadedObjects: true);
				_inputBundle = null;
			}
		}
	}

	public static void ShowInputDialog(string promptText, InputCallback callback, string defaultValue = "", string placeholder = "Enter text...")
	{
		OdiumConsole.Log("InputDialog", $"ShowInputDialog called - Initialized: {_isInitialized}, Prefab null: {_inputDialogPrefab == null}");
		if (!_isInitialized)
		{
			OdiumConsole.Log("InputDialog", "Input dialog system not initialized!", LogLevel.Error);
			callback?.Invoke("", wasSubmitted: false);
			return;
		}
		if (_inputDialogPrefab == null)
		{
			OdiumConsole.Log("InputDialog", "Input dialog prefab is null! Attempting to reinitialize...", LogLevel.Error);
			_isInitialized = false;
			Initialize();
			if (_inputDialogPrefab == null)
			{
				OdiumConsole.Log("InputDialog", "Reinitialization failed - prefab still null!", LogLevel.Error);
				callback?.Invoke("", wasSubmitted: false);
				return;
			}
		}
		try
		{
			Scene activeScene = SceneManager.GetActiveScene();
			OdiumConsole.Log("InputDialog", "Target scene: " + activeScene.name);
			GameObject dialogInstance = UnityEngine.Object.Instantiate(_inputDialogPrefab);
			dialogInstance.name = $"OdiumInputDialog_{DateTime.Now.Ticks}";
			SceneManager.MoveGameObjectToScene(dialogInstance, activeScene);
			OdiumConsole.Log("InputDialog", "Instantiated input dialog in scene: " + dialogInstance.scene.name);
			Canvas component = dialogInstance.GetComponent<Canvas>();
			if (component != null)
			{
				component.enabled = true;
				component.renderMode = RenderMode.ScreenSpaceOverlay;
				component.sortingOrder = 32770;
				component.overrideSorting = true;
				component.pixelPerfect = false;
				OdiumConsole.Log("InputDialog", $"Configured Canvas - Enabled: {component.enabled}, SortOrder: {component.sortingOrder}");
			}
			else
			{
				OdiumConsole.Log("InputDialog", "No Canvas component found on input dialog prefab!", LogLevel.Warning);
			}
			dialogInstance.SetActive(value: true);
			_activeInputDialogs.Add(dialogInstance);
			RectTransform component2 = dialogInstance.GetComponent<RectTransform>();
			if (component2 != null)
			{
				component2.localScale = Vector3.one;
				component2.localPosition = Vector3.zero;
				component2.localEulerAngles = Vector3.zero;
				component2.anchorMin = new Vector2(0.5f, 0.5f);
				component2.anchorMax = new Vector2(0.5f, 0.5f);
				component2.pivot = new Vector2(0.5f, 0.5f);
				component2.anchoredPosition = Vector2.zero;
				if (component2.sizeDelta.x <= 0f || component2.sizeDelta.y <= 0f)
				{
					component2.sizeDelta = new Vector2(400f, 200f);
				}
				OdiumConsole.Log("InputDialog", $"Positioned input dialog - AnchoredPos: {component2.anchoredPosition}, Size: {component2.sizeDelta}");
			}
			TMP_InputField inputField = dialogInstance.GetComponentInChildren<TMP_InputField>();
			if (inputField != null)
			{
				inputField.text = defaultValue;
				TextMeshProUGUI textMeshProUGUI = inputField.placeholder as TextMeshProUGUI;
				if (textMeshProUGUI != null)
				{
					textMeshProUGUI.text = placeholder;
				}
				inputField.onSubmit.AddListener((Action<string>)delegate(string value)
				{
					CloseInputDialog(dialogInstance);
					callback?.Invoke(value, wasSubmitted: true);
				});
				inputField.Select();
				inputField.ActivateInputField();
				OdiumConsole.Log("InputDialog", "Set up input field with listeners");
			}
			else
			{
				OdiumConsole.Log("InputDialog", "Could not find TMP_InputField component!", LogLevel.Warning);
			}
			Button componentInChildren = dialogInstance.GetComponentInChildren<Button>();
			if (componentInChildren != null)
			{
				componentInChildren.onClick.AddListener((Action)delegate
				{
					string input = ((inputField != null) ? inputField.text : "");
					CloseInputDialog(dialogInstance);
					callback?.Invoke(input, wasSubmitted: true);
				});
				OdiumConsole.Log("InputDialog", "Set up button with click listener");
			}
			else
			{
				OdiumConsole.Log("InputDialog", "Could not find Button component!", LogLevel.Warning);
			}
			Transform[] array = dialogInstance.GetComponentsInChildren<Transform>(includeInactive: true);
			Transform[] array2 = array;
			foreach (Transform transform in array2)
			{
				transform.gameObject.SetActive(value: true);
			}
			Behaviour[] array3 = dialogInstance.GetComponentsInChildren<Behaviour>(includeInactive: true);
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
			OdiumConsole.Log("InputDialog", "Final input dialog scene: " + dialogInstance.scene.name);
			MelonCoroutines.Start(HandleInputDialogInput(dialogInstance, callback));
			OdiumConsole.LogGradient("InputDialog", "Input dialog shown with prompt: " + promptText);
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "ShowInputDialog");
			callback?.Invoke("", wasSubmitted: false);
		}
	}

	private static IEnumerator HandleInputDialogInput(GameObject dialog, InputCallback callback)
	{
		while (dialog != null && _activeInputDialogs.Contains(dialog))
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				CloseInputDialog(dialog);
				callback?.Invoke("", wasSubmitted: false);
				break;
			}
			yield return null;
		}
	}

	private static void CloseInputDialog(GameObject dialog)
	{
		try
		{
			if (dialog != null)
			{
				_activeInputDialogs.Remove(dialog);
				UnityEngine.Object.Destroy(dialog);
				OdiumConsole.Log("InputDialog", "Input dialog closed");
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "CloseInputDialog");
		}
	}

	public static bool IsInitialized()
	{
		bool flag = _isInitialized && _inputDialogPrefab != null;
		OdiumConsole.Log("InputDialog", $"IsInitialized check - _isInitialized: {_isInitialized}, _inputDialogPrefab != null: {_inputDialogPrefab != null}, Result: {flag}");
		return flag;
	}

	public static void CloseAllInputDialogs()
	{
		try
		{
			GameObject[] array = _activeInputDialogs.ToArray();
			foreach (GameObject gameObject in array)
			{
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			_activeInputDialogs.Clear();
			OdiumConsole.Log("InputDialog", "All input dialogs closed");
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "CloseAllInputDialogs");
		}
	}

	public static void Cleanup()
	{
		try
		{
			CloseAllInputDialogs();
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
					if (gameObject.name.StartsWith("OdiumInputDialog"))
					{
						UnityEngine.Object.Destroy(gameObject);
						OdiumConsole.Log("InputDialog", "Destroyed input dialog in scene: " + sceneAt.name);
					}
				}
			}
			if (_inputBundle != null)
			{
				_inputBundle.Unload(unloadAllLoadedObjects: true);
				_inputBundle = null;
			}
			_inputDialogPrefab = null;
			_isInitialized = false;
			OdiumConsole.Log("InputDialog", "Input dialog system cleaned up");
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "InputDialogCleanup");
		}
	}
}
