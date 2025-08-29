using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using Newtonsoft.Json;
using Odium.AwooochysResourceManagement;
using Odium.Odium;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VRC.UI;
using VRC.UI.Core.Styles;

namespace Odium.ButtonAPI.QM;

public class DebugUI
{
	public static GameObject label;

	public static GameObject background;

	public static TextMeshProUGUI text;

	private static string cachedPing = "0";

	private static string cachedFPS = "0";

	private static string cachedBuild = "Unknown";

	private static int cachedPlayerTags = 0;

	private static int cachedOdiumUsers = 0;

	private static bool isUpdating = false;

	private static float lastUpdateTime = 0f;

	private static readonly float UPDATE_INTERVAL = 1f;

	public static async void InitializeDebugMenu()
	{
		try
		{
			GameObject userInterface = AssignedVariables.userInterface;
			if (userInterface == null)
			{
				OdiumConsole.Log("DebugUI", "User interface not found");
				return;
			}
			Transform dashboardHeader = userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickLinks");
			dashboardHeader.gameObject.SetActive(value: true);
			dashboardHeader.transform.Find("HeaderBackground").gameObject.SetActive(value: true);
			if (dashboardHeader == null)
			{
				OdiumConsole.Log("DebugUI", "Dashboard header template not found");
				return;
			}
			label = UnityEngine.Object.Instantiate(dashboardHeader.gameObject);
			label.SetActive(value: true);
			label.transform.SetParent(userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/Wing_Right"));
			label.transform.localPosition = new Vector3(400f, -525f, 0f);
			label.transform.localRotation = Quaternion.identity;
			label.transform.localScale = new Vector3(1f, 1f, 1f);
			UIInvisibleGraphic invisibleGraphic = label.GetComponent<UIInvisibleGraphic>();
			if (invisibleGraphic != null)
			{
				invisibleGraphic.enabled = false;
			}
			text = label.GetComponentInChildren<TextMeshProUGUIEx>();
			if (text == null)
			{
				OdiumConsole.Log("DebugUI", "Text component not found");
				return;
			}
			text.alignment = TextAlignmentOptions.TopLeft;
			text.outlineWidth = 0.2f;
			text.fontSize = 20f;
			text.fontSizeMax = 25f;
			text.fontSizeMin = 18f;
			text.richText = true;
			Transform LeftItemContainer = label.transform.Find("LeftItemContainer");
			if (LeftItemContainer != null)
			{
				LayoutGroup layoutGroup = LeftItemContainer.GetComponent<LayoutGroup>();
				if (layoutGroup != null)
				{
					layoutGroup.enabled = false;
					OdiumConsole.Log("DebugUI", "Disabled LayoutGroup on LeftItemContainer");
				}
				ContentSizeFitter contentSizeFitter = LeftItemContainer.GetComponent<ContentSizeFitter>();
				if (contentSizeFitter != null)
				{
					contentSizeFitter.enabled = false;
					OdiumConsole.Log("DebugUI", "Disabled ContentSizeFitter on LeftItemContainer");
				}
			}
			LayoutGroup labelLayoutGroup = label.GetComponent<LayoutGroup>();
			if (labelLayoutGroup != null)
			{
				labelLayoutGroup.enabled = false;
				OdiumConsole.Log("DebugUI", "Disabled LayoutGroup on label");
			}
			if (text != null)
			{
				RectTransform rectTransform = text.GetComponent<RectTransform>();
				if (rectTransform != null)
				{
					rectTransform.anchoredPosition = new Vector2(295f, 220f);
					OdiumConsole.Log("DebugUI", $"Set anchored position to: {rectTransform.anchoredPosition}");
				}
			}
			Mask mask = label.AddComponent<Mask>();
			mask.showMaskGraphic = false;
			background = label.transform.Find("HeaderBackground")?.gameObject;
			if (background == null)
			{
				OdiumConsole.Log("DebugUI", "Background object not found");
				return;
			}
			background.transform.localPosition = new Vector3(0f, 0f, 0f);
			background.transform.localScale = new Vector3(0.4f, 7.5f, 1f);
			background.transform.localRotation = Quaternion.identity;
			background.SetActive(value: true);
			ImageEx bgImage = background.GetComponent<ImageEx>();
			if ((UnityEngine.Object)(object)bgImage == null)
			{
				OdiumConsole.Log("DebugUI", "Background image component not found");
				return;
			}
			string bgPath = Path.Combine(Environment.CurrentDirectory, "Odium", "QMDebugUIBackground.png");
			Sprite sprite = bgPath.LoadSpriteFromDisk();
			if (sprite == null)
			{
				OdiumConsole.Log("DebugUI", "Failed to load background sprite from path: " + bgPath);
				((Graphic)(object)bgImage).m_Color = new Color(0.443f, 0.133f, 1f, 1f);
			}
			else
			{
				((Image)(object)bgImage).overrideSprite = sprite;
			}
			StyleElement styleElement = background.GetComponent<StyleElement>();
			if (styleElement != null)
			{
				UnityEngine.Object.Destroy(styleElement);
			}
			label.SetActive(value: true);
			background.SetActive(value: true);
			MelonCoroutines.Start(UpdateLoop());
			OdiumConsole.Log("DebugUI", "Debug menu positioned correctly!");
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			OdiumConsole.Log("DebugUI", "Failed to initialize debug menu: " + ex2.Message);
		}
	}

	public static IEnumerator UpdateLoop()
	{
		while (true)
		{
			yield return new WaitForSecondsRealtime(5f);
			yield return MelonCoroutines.Start(GetUserCountCoroutine());
			isUpdating = true;
			cachedPing = ApiUtils.GetPing();
			cachedFPS = ApiUtils.GetFPS();
			cachedBuild = ApiUtils.GetBuild();
			cachedPlayerTags = AssignedVariables.playerTagsCount;
			cachedOdiumUsers = AssignedVariables.odiumUsersCount;
			UpdateDisplay();
			isUpdating = false;
		}
	}

	private static void UpdateDisplay()
	{
		if (!(Time.time - lastUpdateTime < UPDATE_INTERVAL))
		{
			lastUpdateTime = Time.time;
			if (text != null)
			{
				text.text = $"\r\nSubscription: <color=green>Active</color>\r\n\r\nPlayer Tags: <color=#e91f42>{cachedPlayerTags}</color>\r\n\r\nOdium Users: <color=#e91f42>{cachedOdiumUsers}</color>\r\n\r\nDuration: <color=#e91f42>Lifetime</color>\r\n\r\nPing: <color=#e91f42>{cachedPing}</color>\r\n\r\nFPS: <color=#e91f42>{cachedFPS}</color>\r\n\r\nBuild: <color=#e91f42>{cachedBuild}</color>\r\n\r\nServer: <color=#e91f42>Connected</color>\r\n\r\nClient: <color=#e91f42>Connected</color>\r\n\r\nDrones: <color=#e91f42>0</color>\r\n        ";
			}
		}
	}

	private static IEnumerator GetUserCountCoroutine()
	{
		UnityWebRequest www = UnityWebRequest.Get("https://snoofz.net/api/odium/users/list");
		yield return www.SendWebRequest();
		if (www.isDone)
		{
			if (string.IsNullOrEmpty(www.error))
			{
				try
				{
					string jsonContent = www.downloadHandler.text;
					AssignedVariables.odiumUsersCount = (cachedOdiumUsers = JsonConvert.DeserializeObject<List<object>>(jsonContent)?.Count ?? 0);
				}
				catch (Exception)
				{
					cachedOdiumUsers = 0;
				}
			}
			else
			{
				cachedOdiumUsers = 0;
			}
		}
		www.Dispose();
	}

	public static void AdjustPosition(float x, float y, float z)
	{
		if (label != null)
		{
			label.transform.localPosition = new Vector3(x, y, z);
			OdiumConsole.Log("DebugUI", $"Position adjusted to: {x}, {y}, {z}");
		}
	}

	public static void AdjustBackgroundScale(float x, float y, float z)
	{
		if (background != null)
		{
			background.transform.localScale = new Vector3(x, y, z);
			OdiumConsole.Log("DebugUI", $"Background scale adjusted to: {x}, {y}, {z}");
		}
	}

	public static void FixBackgroundWidth()
	{
		if (background != null)
		{
			background.transform.localScale = new Vector3(1f, 8f, 1f);
			OdiumConsole.Log("DebugUI", "Background width adjusted");
		}
	}
}
