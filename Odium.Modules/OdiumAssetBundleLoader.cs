using System;
using System.Collections;
using System.IO;
using MelonLoader;
using Odium.Components;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Odium.Modules;

public static class OdiumAssetBundleLoader
{
	private static AssetBundle _loadingScreenBundle;

	private static GameObject _loadingScreenPrefab;

	private static GameObject _instantiatedLoadingScreen;

	public static AudioSource _customAudioSource;

	public static AudioClip _customAudioClip;

	private static readonly string LoadingScreenPath = Path.Combine(ModSetup.GetOdiumFolderPath(), "AssetBundles", "odium.loadingscreen");

	private static readonly string LoadingMusicPath = Path.Combine(ModSetup.GetOdiumFolderPath(), "Audio", "loadingmusic.mp3");

	private static readonly Color FemboyPink = new Color(0.792f, 0.008f, 0.988f, 1f);

	public static void Initialize()
	{
		OdiumConsole.Log("AssetLoader", "Starting loading screen asset loader...");
		LoadAndInstantiateLoadingScreen();
		GameObject.Find("MenuContent/Popups/LoadingPopup/3DElements").SetActive(value: false);
		GameObject.Find("MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/GoButton").GetComponent<Image>().m_Color = Color.black;
		GameObject.Find("MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>().m_Color = Color.black;
		GameObject.Find("MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>().m_Color = Color.black;
		string path = Path.Combine(Environment.CurrentDirectory, "Odium", "ButtonBackground.png");
		Sprite sprite = SpriteUtil.LoadFromDisk(path);
		GameObject.Find("MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>().m_Color = Color.black;
		GameObject.Find("MenuContent/Popups/LoadingPopup/ButtonMiddle").GetComponent<Image>().m_Color = Color.black;
		GameObject.Find("MenuContent/Popups/LoadingPopup/ButtonMiddle/Text").GetComponent<TextMeshProUGUIEx>().m_fontColor = Color.white;
		GameObject.Find("MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/GoButton/Text").GetComponent<TextMeshProUGUIEx>().m_fontColor = FemboyPink;
		GameObject.Find("MenuContent/Popups/LoadingPopup/ButtonMiddle/Text").GetComponent<TextMeshProUGUIEx>().m_fontColor = FemboyPink;
	}

	private static void LoadAndInstantiateLoadingScreen()
	{
		MelonCoroutines.Start(LoadAssetBundle());
		MelonCoroutines.Start(LoadPrefabFromBundle());
		ChangeLoadingScreen();
	}

	public static void ChangeLoadingScreen()
	{
		MelonCoroutines.Start(InstantiateLoadingScreen());
		MelonCoroutines.Start(ApplyToVRChatLoading());
	}

	private static IEnumerator LoadAssetBundle()
	{
		OdiumConsole.Log("AssetLoader", "Loading AssetBundle from file...");
		if (!File.Exists(LoadingScreenPath))
		{
			OdiumConsole.Log("AssetLoader", "Loading screen file not found at: " + LoadingScreenPath, LogLevel.Error);
			yield break;
		}
		AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(LoadingScreenPath);
		yield return bundleRequest;
		try
		{
			_loadingScreenBundle = bundleRequest.assetBundle;
			if (!(_loadingScreenBundle != null))
			{
				yield break;
			}
			OdiumConsole.LogGradient("AssetLoader", "AssetBundle loaded successfully!", LogLevel.Info, gradientCategory: true);
			Il2CppStringArray assetNames = _loadingScreenBundle.GetAllAssetNames();
			OdiumConsole.Log("AssetLoader", $"Found {assetNames.Length} assets in bundle:");
			foreach (string assetName in assetNames)
			{
				OdiumConsole.Log("AssetLoader", "  - " + assetName);
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "LoadAssetBundle");
		}
	}

	private static IEnumerator LoadPrefabFromBundle()
	{
		if (_loadingScreenBundle == null)
		{
			yield break;
		}
		OdiumConsole.Log("AssetLoader", "Loading prefab from AssetBundle...");
		try
		{
			string[] possibleNames = new string[6] { "Loading Screen", "ParticleSystem", "CustomLoadingScreen", "loadingscreen", "Loading", "Screen" };
			string[] array = possibleNames;
			foreach (string name in array)
			{
				_loadingScreenPrefab = _loadingScreenBundle.LoadAsset<GameObject>(name);
				if (_loadingScreenPrefab != null)
				{
					OdiumConsole.LogGradient("AssetLoader", "Found loading screen prefab: " + name, LogLevel.Info, gradientCategory: true);
					break;
				}
			}
			if (_loadingScreenPrefab == null)
			{
				Il2CppStringArray assetNames = _loadingScreenBundle.GetAllAssetNames();
				foreach (string assetName in assetNames)
				{
					GameObject asset = _loadingScreenBundle.LoadAsset<GameObject>(assetName);
					if (asset != null)
					{
						_loadingScreenPrefab = asset;
						OdiumConsole.LogGradient("AssetLoader", "Using first GameObject asset: " + assetName, LogLevel.Info, gradientCategory: true);
						break;
					}
				}
			}
			if (_loadingScreenPrefab == null)
			{
				OdiumConsole.Log("AssetLoader", "No GameObject prefab found in AssetBundle!", LogLevel.Error);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			OdiumConsole.LogException(ex2, "LoadPrefabFromBundle");
		}
		yield return null;
	}

	private static IEnumerator InstantiateLoadingScreen()
	{
		if (_loadingScreenPrefab == null)
		{
			yield break;
		}
		OdiumConsole.Log("AssetLoader", "Instantiating loading screen...");
		try
		{
			GameObject loadingPopup = GameObject.Find("MenuContent/Popups/LoadingPopup");
			if (loadingPopup == null)
			{
				OdiumConsole.Log("AssetLoader", "Could not find LoadingPopup parent", LogLevel.Warning);
				_instantiatedLoadingScreen = UnityEngine.Object.Instantiate(_loadingScreenPrefab);
			}
			else
			{
				_instantiatedLoadingScreen = UnityEngine.Object.Instantiate(_loadingScreenPrefab, loadingPopup.transform);
			}
			if (_instantiatedLoadingScreen != null)
			{
				_instantiatedLoadingScreen.name = "OdiumCustomLoadingScreen";
				_instantiatedLoadingScreen.transform.localPosition = Vector3.zero;
				_instantiatedLoadingScreen.transform.localRotation = Quaternion.identity;
				_instantiatedLoadingScreen.transform.localScale = Vector3.one;
				_instantiatedLoadingScreen.SetActive(value: true);
				ApplyFemboyPinkToParticles();
				OdiumConsole.LogGradient("AssetLoader", "Loading screen instantiated successfully!");
			}
			else
			{
				OdiumConsole.Log("AssetLoader", "Failed to instantiate loading screen!", LogLevel.Error);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			OdiumConsole.LogException(ex2, "InstantiateLoadingScreen");
		}
		yield return null;
	}

	private static void ApplyFemboyPinkToParticles()
	{
		if (_instantiatedLoadingScreen == null)
		{
			return;
		}
		try
		{
			Il2CppArrayBase<ParticleSystem> componentsInChildren = _instantiatedLoadingScreen.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
			OdiumConsole.Log("AssetLoader", $"Found {componentsInChildren.Length} particle systems to colorize");
			OdiumConsole.LogGradient("AssetLoader", "All particle systems colorized with femboy pink!");
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "ApplyFemboyPinkToParticles");
		}
	}

	private static IEnumerator InitLoadingScreenAudio()
	{
		if (_customAudioSource != null && _customAudioSource.isPlaying)
		{
			OdiumConsole.Log("AssetLoader", "Custom audio already playing, skipping initialization");
			yield break;
		}
		OdiumConsole.Log("AssetLoader", "Creating custom audio source...");
		if (!File.Exists(LoadingMusicPath))
		{
			OdiumConsole.Log("AssetLoader", "Audio file not found at: " + LoadingMusicPath, LogLevel.Warning);
			yield break;
		}
		if (_customAudioSource != null)
		{
			_customAudioSource.Stop();
			UnityEngine.Object.Destroy(_customAudioSource.gameObject);
			_customAudioSource = null;
			OdiumConsole.Log("AssetLoader", "Destroyed existing audio source");
		}
		GameObject customAudioObject = new GameObject("OdiumCustomAudio");
		_customAudioSource = customAudioObject.AddComponent<AudioSource>();
		_customAudioSource.loop = true;
		_customAudioSource.volume = 0.7f;
		_customAudioSource.spatialBlend = 0f;
		UnityEngine.Object.DontDestroyOnLoad(customAudioObject);
		OdiumConsole.Log("AssetLoader", "Custom audio source created!");
		UnityWebRequest www = UnityWebRequest.Get("file://" + LoadingMusicPath);
		www.SendWebRequest();
		while (!www.isDone)
		{
			yield return null;
		}
		_customAudioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(www.downloadHandler, www.url, stream: false, compressed: false, AudioType.UNKNOWN);
		while (!www.isDone || _customAudioClip.loadState == AudioDataLoadState.Loading)
		{
			yield return null;
		}
		if (_customAudioClip != null)
		{
			OdiumConsole.LogGradient("AssetLoader", "Custom audio loaded successfully!");
			_customAudioSource.clip = _customAudioClip;
			_customAudioSource.Play();
			OdiumConsole.LogGradient("AssetLoader", "Custom music now playing!");
		}
		else
		{
			OdiumConsole.Log("AssetLoader", "Failed to create AudioClip", LogLevel.Error);
		}
		www.Dispose();
	}

	private static IEnumerator ApplyToVRChatLoading()
	{
		if (!(_instantiatedLoadingScreen == null))
		{
			OdiumConsole.Log("AssetLoader", "Applying custom loading screen to VRChat...");
			try
			{
				HideOriginalLoadingElements();
				PositionCustomLoadingScreen();
				OdiumConsole.LogGradient("AssetLoader", "Custom loading screen applied to VRChat!");
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				OdiumConsole.LogException(ex2, "ApplyToVRChatLoading");
			}
			yield return null;
		}
	}

	private static void HideOriginalLoadingElements()
	{
		try
		{
			string[] array = new string[3] { "MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked", "MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles", "MenuContent/Popups/LoadingPopup/LoadingSound" };
			string[] array2 = array;
			foreach (string name in array2)
			{
				GameObject gameObject = GameObject.Find(name);
				if (gameObject != null)
				{
					gameObject.SetActive(value: false);
				}
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "HideOriginalLoadingElements");
		}
	}

	private static void PositionCustomLoadingScreen()
	{
		try
		{
			if (_instantiatedLoadingScreen != null)
			{
				_instantiatedLoadingScreen.transform.localScale = new Vector3(400f, 400f, 400f);
				_instantiatedLoadingScreen.transform.localPosition = Vector3.zero;
				_instantiatedLoadingScreen.transform.localRotation = Quaternion.identity;
				OdiumConsole.Log("AssetLoader", "Custom loading screen positioned");
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "PositionCustomLoadingScreen");
		}
	}

	public static IEnumerator ChangeLoadingScreenAudio()
	{
		if (!File.Exists(LoadingMusicPath))
		{
			OdiumConsole.Log("AssetLoader", "No audio file to change to", LogLevel.Warning);
			yield break;
		}
		if (_customAudioSource == null)
		{
			OdiumConsole.Log("AssetLoader", "No custom audio source available", LogLevel.Warning);
			yield break;
		}
		UnityWebRequest www = UnityWebRequest.Get("file://" + LoadingMusicPath);
		www.SendWebRequest();
		while (!www.isDone)
		{
			yield return null;
		}
		_customAudioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(www.downloadHandler, www.url, stream: false, compressed: false, AudioType.UNKNOWN);
		while (!www.isDone || _customAudioClip.loadState == AudioDataLoadState.Loading)
		{
			yield return null;
		}
		if (_customAudioClip != null)
		{
			_customAudioSource.Stop();
			_customAudioSource.clip = _customAudioClip;
			_customAudioSource.Play();
			OdiumConsole.LogGradient("AssetLoader", "Custom audio changed!");
		}
		www.Dispose();
	}

	public static void ShowLoadingScreen()
	{
		if (_instantiatedLoadingScreen != null)
		{
			_instantiatedLoadingScreen.SetActive(value: true);
			OdiumConsole.Log("AssetLoader", "Custom loading screen shown");
		}
	}

	public static void HideLoadingScreen()
	{
		if (_instantiatedLoadingScreen != null)
		{
			_instantiatedLoadingScreen.SetActive(value: false);
			OdiumConsole.Log("AssetLoader", "Custom loading screen hidden");
		}
	}

	public static bool IsLoadingScreenLoaded()
	{
		return _instantiatedLoadingScreen != null;
	}

	public static GameObject GetLoadingScreenInstance()
	{
		return _instantiatedLoadingScreen;
	}

	public static void StopCustomAudio()
	{
		if (_customAudioSource != null)
		{
			_customAudioSource.Stop();
			OdiumConsole.Log("AssetLoader", "Custom audio stopped");
		}
	}

	public static void PlayCustomAudio()
	{
		if (_customAudioSource != null && _customAudioClip != null)
		{
			_customAudioSource.Play();
			OdiumConsole.Log("AssetLoader", "Custom audio playing");
		}
	}

	public static void Cleanup()
	{
		try
		{
			if (_instantiatedLoadingScreen != null)
			{
				UnityEngine.Object.Destroy(_instantiatedLoadingScreen);
				_instantiatedLoadingScreen = null;
				OdiumConsole.Log("AssetLoader", "Loading screen instance destroyed");
			}
			if (_loadingScreenBundle != null)
			{
				_loadingScreenBundle.Unload(unloadAllLoadedObjects: true);
				_loadingScreenBundle = null;
				OdiumConsole.Log("AssetLoader", "AssetBundle unloaded");
			}
			if (_customAudioSource != null)
			{
				_customAudioSource.Stop();
				UnityEngine.Object.Destroy(_customAudioSource.gameObject);
				_customAudioSource = null;
				OdiumConsole.Log("AssetLoader", "Custom audio source destroyed");
			}
			if (_customAudioClip != null)
			{
				UnityEngine.Object.Destroy(_customAudioClip);
				_customAudioClip = null;
				OdiumConsole.Log("AssetLoader", "Custom audio clip destroyed");
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "Cleanup");
		}
	}

	public static void RestoreOriginalLoadingScreen()
	{
		try
		{
			OdiumConsole.Log("AssetLoader", "Restoring original loading screen...");
			HideLoadingScreen();
			if (_customAudioSource != null)
			{
				_customAudioSource.Stop();
				OdiumConsole.Log("AssetLoader", "Stopped custom audio");
			}
			string[] array = new string[2] { "MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked", "MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles" };
			string[] array2 = array;
			foreach (string text in array2)
			{
				GameObject gameObject = GameObject.Find(text);
				if (gameObject != null)
				{
					gameObject.SetActive(value: true);
					OdiumConsole.Log("AssetLoader", "Restored original element: " + text);
				}
			}
			OdiumConsole.LogGradient("AssetLoader", "Original loading screen restored!");
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "RestoreOriginalLoadingScreen");
		}
	}
}
