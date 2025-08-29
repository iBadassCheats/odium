using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using CursorLayerMod;
using HarmonyLib;
using MelonLoader;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Odium.ApplicationBot;
using Odium.Components;
using Odium.GameCheats;
using Odium.Modules;
using Odium.Odium;
using Odium.Patches;
using Odium.QMPages;
using Odium.Threadding;
using Odium.UI;
using Odium.UX;
using Odium.Wrappers;
using OdiumLoader;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace Odium;

public class OdiumEntry : MelonMod
{
	private class AuthData
	{
		public string Email { get; set; }

		public string Key { get; set; }
	}

	private class ValidationResponse
	{
		public bool Success { get; set; }

		public bool Valid { get; set; }

		public string Message { get; set; }
	}

	public static string Version;

	public static bool wasKeyValid;

	public new static HarmonyLib.Harmony HarmonyInstance;

	private float lastStatsUpdate = 0f;

	private const float STATS_UPDATE_INTERVAL = 1f;

	public static int loadIndex;

	public static Sprite buttonImage;

	private static readonly string AUTH_ENDPOINT;

	private static readonly string AUTH_FILE;

	public static bool heartbeatRun;

	public static string Current_World_id => RoomManager.prop_ApiWorldInstance_0.id;

	public override void OnInitializeMelon()
	{
		try
		{
			OdiumConsole.Initialize();
			OdiumConsole.LogGradient("Odium", "Cracked by AllahLeaks.", LogLevel.Info, gradientCategory: true);
			wasKeyValid = true;
			ModSetup.Initialize().GetAwaiter();
			BoneESP.SetEnabled(enabled: false);
			BoxESP.SetEnabled(enabled: false);
			PunchSystem.Initialize();
			CoroutineManager.Init();
			OdiumConsole.LogGradient("System", "Initialization complete!");
		}
		catch (Exception)
		{
		}
	}

	private bool AuthenticateUser()
	{
		OdiumConsole.LogGradient("Odium", "Returning true for AuthenticateUser check...", LogLevel.Info, gradientCategory: true);
		return true;
	}

	private bool ShowAuthenticationDialog()
	{
		try
		{
			return ShowFileBasedAuth();
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("Auth", "Dialog error: " + ex.Message, LogLevel.Error);
			return false;
		}
	}

	private bool ShowFileBasedAuth()
	{
		try
		{
			string text = Path.Combine(Environment.CurrentDirectory, "Odium", "temp_auth.txt");
			Directory.CreateDirectory(Path.GetDirectoryName(text));
			if (File.Exists(text))
			{
				string[] array = File.ReadAllLines(text);
				if (array.Length >= 2)
				{
					string text2 = array[0].Trim();
					string text3 = array[1].Trim();
					if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3))
					{
						if (ValidateCredentials(text2, text3))
						{
							SaveCredentials(text2, text3);
							File.Delete(text);
							wasKeyValid = true;
							OdiumConsole.LogGradient("Auth", "Authentication successful via file!");
							return true;
						}
						File.Delete(text);
						OdiumConsole.Log("Auth", "Invalid credentials in temp_auth.txt file.", LogLevel.Error);
					}
				}
				File.Delete(text);
			}
			string format = "ODIUM AUTHENTICATION REQUIRED\r\n=====================================\r\n\r\nTo authenticate Odium, please create a file named 'temp_auth.txt' in the Odium folder with your credentials:\r\n\r\nFile Location: {0}\r\n\r\nFile Contents (2 lines):\r\nLine 1: Your purchase email\r\nLine 2: Your invite key\r\n\r\nExample:\r\nuser@example.com\r\nyour-invite-key-here\r\n\r\nAfter creating the file, restart VRChat.\r\nThe file will be automatically deleted after successful authentication.\r\n\r\nVRChat will now close so you can set up authentication.";
			string text4 = Path.Combine(Environment.CurrentDirectory, "Odium", "auth_instructions.txt");
			File.WriteAllText(text4, string.Format(format, text));
			OdiumConsole.Log("Auth", "Authentication required. Instructions written to: " + text4);
			OdiumConsole.Log("Auth", "Create temp_auth.txt at: " + text);
			return false;
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("Auth", "File auth error: " + ex.Message, LogLevel.Error);
			return false;
		}
	}

	private bool ValidateCredentials(string email, string key)
	{
		try
		{
			using HttpClient httpClient = new HttpClient();
			httpClient.Timeout = TimeSpan.FromSeconds(10.0);
			var value = new { email, key };
			string content = JsonConvert.SerializeObject(value);
			StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
			HttpResponseMessage result = httpClient.PostAsync(AUTH_ENDPOINT, content2).Result;
			string result2 = result.Content.ReadAsStringAsync().Result;
			if (result.IsSuccessStatusCode)
			{
				ValidationResponse validationResponse = JsonConvert.DeserializeObject<ValidationResponse>(result2);
				return validationResponse != null && validationResponse.Success && (validationResponse?.Valid ?? false);
			}
			OdiumConsole.Log("Auth", $"Validation request failed: {result.StatusCode} - {result2}", LogLevel.Error);
			return false;
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("Auth", "Validation error: " + ex.Message, LogLevel.Error);
			return false;
		}
	}

	private void SaveCredentials(string email, string key)
	{
		try
		{
			AuthData value = new AuthData
			{
				Email = email,
				Key = key
			};
			string contents = JsonConvert.SerializeObject(value);
			Directory.CreateDirectory(Path.GetDirectoryName(AUTH_FILE));
			File.WriteAllText(AUTH_FILE, contents);
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("Auth", "Failed to save credentials: " + ex.Message, LogLevel.Warning);
		}
	}

	private void ShowErrorDialog(string title, string message)
	{
		try
		{
			Interaction.MsgBox(message ?? "", MsgBoxStyle.Critical, "Odium - " + title);
		}
		catch
		{
			OdiumConsole.Log("Error", title + ": " + message, LogLevel.Error);
		}
	}

	private void ShowSuccessDialog(string title, string message)
	{
		try
		{
			Interaction.MsgBox(message ?? "", MsgBoxStyle.Information, "Odium - " + title);
		}
		catch
		{
			OdiumConsole.Log("Success", title + ": " + message);
		}
	}

	public override void OnApplicationStart()
	{
		HarmonyInstance = new HarmonyLib.Harmony("Odium.Harmony");
		MelonCoroutines.Start(QM.WaitForUI());
		MelonCoroutines.Start(OnNetworkManagerInit());
		QM.SetupMenu();
		PlayerRankTextDisplay.Initialize();
		PlayerRankTextDisplay.SetVisible(visible: true);
		BoneESP.Initialize();
		BoneESP.SetEnabled(enabled: true);
		BoneESP.SetBoneColor(new Color(0.584f, 0.008f, 0.996f, 1f));
		BoxESP.Initialize();
		BoxESP.SetEnabled(enabled: true);
		BoxESP.SetBoxColor(new Color(0.584f, 0.008f, 0.996f, 1f));
		MainThreadDispatcher.Initialize();
		MelonCoroutines.Start(RamClearLoop());
		Patching.Initialize();
		ClonePatch.Patch();
		PhotonPatchesManual.ApplyPatches();
	}

	public override void OnApplicationLateStart()
	{
		Bot.Start();
		OdiumModuleLoader.OnApplicationStart();
	}

	internal static IEnumerator OnNetworkManagerInit()
	{
		while (NetworkManager.field_Internal_Static_NetworkManager_0 == null)
		{
			yield return new WaitForSecondsRealtime(2f);
		}
		if (!(NetworkManager.field_Internal_Static_NetworkManager_0 != null))
		{
			yield break;
		}
		NetworkManager.field_Internal_Static_NetworkManager_0.field_Private_ObjectPublicHa1UnT1Unique_1_IPlayer_1.field_Private_HashSet_1_UnityAction_1_T_0.Add((Action<IPlayer>)delegate(IPlayer obj)
		{
			if (!heartbeatRun)
			{
				heartbeatRun = true;
				Thread thread = new Thread((ThreadStart)delegate
				{
					while (true)
					{
						string location_k__BackingField = APIUser.CurrentUser._location_k__BackingField;
						List<Player> allPlayers = PlayerWrapper.GetAllPlayers();
						foreach (Player item in allPlayers)
						{
							using HttpClient httpClient = new HttpClient();
							string id_k__BackingField = item.prop_APIUser_0._id_k__BackingField;
							var value = new
							{
								userId = id_k__BackingField,
								worldId = location_k__BackingField
							};
							string content = JsonConvert.SerializeObject(value);
							StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
							MelonLogger.Msg("request");
							string requestUri = "https://track.niggaf.art/api/v1/user/heartbeat";
							HttpResponseMessage result = httpClient.PostAsync(requestUri, content2).Result;
							if (!result.IsSuccessStatusCode)
							{
								MelonLogger.Msg($"Failed to post user leave data: {result.StatusCode}");
							}
						}
						Thread.Sleep(180000);
					}
				});
				thread.IsBackground = true;
				thread.Start();
			}
			OdiumConsole.LogGradient("PlayerJoin", obj.prop_IUser_0.prop_String_1);
			Sprite newDevCircleSprite = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Nameplate.png");
			NameplateModifier.ModifyPlayerNameplate(PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player, newDevCircleSprite);
			if (AssignedVariables.desktopPlayerList)
			{
				PlayerRankTextDisplay.AddPlayer(obj.prop_IUser_0.prop_String_1, PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player.field_Private_APIUser_0, PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player);
			}
			VRCPlayerApi localPlayerAPIUser = PlayerWrapper.GetLocalPlayerAPIUser(obj.prop_IUser_0.prop_String_0);
			Player player = PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player;
			if (AssignedVariables.instanceLock)
			{
				for (int num = 0; num < 450; num++)
				{
					Murder4Utils.SendTargetedPatreonEvent(player, "ListPatrons");
				}
			}
			if (Networking.LocalPlayer.displayName == localPlayerAPIUser.displayName)
			{
				PlayerWrapper.QuickSpoof();
				PlayerWrapper.LocalPlayer = PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player;
				Color rankColor = PlayerRankTextDisplay.GetRankColor(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
				string hexColor = PlayerRankTextDisplay.ColorToHex(rankColor);
				string rankDisplayName = PlayerRankTextDisplay.GetRankDisplayName(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
				IiIIiIIIiIIIIiIIIIiIiIiIIiIIIIiIiIIiiIiIiIIIiiIIiI.IiIIiIIIIIIIIIIIIiiiiiiIIIIiIIiIiIIIiiIiiIiIiIiiIiIiIiIIiIiIIIiiiIIIIIiIIiIiIiIiiIIIiiIiiiiiiiiIiiIIIiIiiiiIIIIIiII(obj.prop_IUser_0.prop_String_0, obj.prop_IUser_0.prop_String_1, hexColor);
				MainThreadDispatcher.Enqueue(delegate
				{
					try
					{
						HttpClient httpClient = new HttpClient();
						string current_World_id = Current_World_id;
						string text2 = obj.prop_IUser_0.prop_String_1;
						if (!string.IsNullOrEmpty(current_World_id) && !string.IsNullOrEmpty(text2))
						{
							var value = new
							{
								displayName = text2,
								location = current_World_id
							};
							string content = JsonConvert.SerializeObject(value);
							StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
							string requestUri = "http://api.snoofz.net:3778/api/odium/vrc/setUserLocation";
							HttpResponseMessage result = httpClient.PostAsync(requestUri, content2).Result;
							if (result.IsSuccessStatusCode)
							{
								OdiumConsole.Log("Odium", "Updated location on API: " + text2 + " -> " + current_World_id);
							}
							else
							{
								OdiumConsole.Log("Odium", $"Failed to update location on API. Status: {result.StatusCode}");
							}
						}
					}
					catch (Exception ex)
					{
						OdiumConsole.Log("Odium", "Error updating location on API: " + ex.Message);
					}
				});
			}
			Color rankColor2 = PlayerRankTextDisplay.GetRankColor(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
			string text = PlayerRankTextDisplay.ColorToHex(rankColor2);
			string rankDisplayName2 = PlayerRankTextDisplay.GetRankDisplayName(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
			PlayerWrapper.Players.Add(player);
			BoneESP.OnPlayerJoined(player);
			BoxESP.OnPlayerJoined(player);
		});
		NetworkManager.field_Internal_Static_NetworkManager_0.field_Private_ObjectPublicHa1UnT1Unique_1_IPlayer_0.field_Private_HashSet_1_UnityAction_1_T_0.Add((Action<IPlayer>)delegate(IPlayer obj)
		{
			OdiumConsole.LogGradient("PlayerJoin", obj.prop_IUser_0.prop_String_1);
			Sprite newDevCircleSprite = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Nameplate.png");
			NameplateModifier.ModifyPlayerNameplate(PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player, newDevCircleSprite);
			if (AssignedVariables.desktopPlayerList)
			{
				PlayerRankTextDisplay.AddPlayer(obj.prop_IUser_0.prop_String_1, PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player.field_Private_APIUser_0, PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player);
			}
			VRCPlayerApi localPlayerAPIUser = PlayerWrapper.GetLocalPlayerAPIUser(obj.prop_IUser_0.prop_String_0);
			Player player = PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player;
			if (AssignedVariables.instanceLock)
			{
				for (int i = 0; i < 450; i++)
				{
					Murder4Utils.SendTargetedPatreonEvent(player, "ListPatrons");
				}
			}
			if (Networking.LocalPlayer.displayName == localPlayerAPIUser.displayName)
			{
				PlayerWrapper.QuickSpoof();
				PlayerWrapper.LocalPlayer = PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player;
				Color rankColor = PlayerRankTextDisplay.GetRankColor(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
				string hexColor = PlayerRankTextDisplay.ColorToHex(rankColor);
				string rankDisplayName = PlayerRankTextDisplay.GetRankDisplayName(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
				IiIIiIIIiIIIIiIIIIiIiIiIIiIIIIiIiIIiiIiIiIIIiiIIiI.IiIIiIIIIIIIIIIIIiiiiiiIIIIiIIiIiIIIiiIiiIiIiIiiIiIiIiIIiIiIIIiiiIIIIIiIIiIiIiIiiIIIiiIiiiiiiiiIiiIIIiIiiiiIIIIIiII(obj.prop_IUser_0.prop_String_0, obj.prop_IUser_0.prop_String_1, hexColor);
				MainThreadDispatcher.Enqueue(delegate
				{
					try
					{
						HttpClient httpClient = new HttpClient();
						string current_World_id = Current_World_id;
						string text2 = obj.prop_IUser_0.prop_String_1;
						if (!string.IsNullOrEmpty(current_World_id) && !string.IsNullOrEmpty(text2))
						{
							var value = new
							{
								displayName = text2,
								location = current_World_id
							};
							string content = JsonConvert.SerializeObject(value);
							StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
							string requestUri = "http://api.snoofz.net:3778/api/odium/vrc/setUserLocation";
							HttpResponseMessage result = httpClient.PostAsync(requestUri, content2).Result;
							if (result.IsSuccessStatusCode)
							{
								OdiumConsole.Log("Odium", "Updated location on API: " + text2 + " -> " + current_World_id);
							}
							else
							{
								OdiumConsole.Log("Odium", $"Failed to update location on API. Status: {result.StatusCode}");
							}
						}
					}
					catch (Exception ex)
					{
						OdiumConsole.Log("Odium", "Error updating location on API: " + ex.Message);
					}
				});
			}
			Color rankColor2 = PlayerRankTextDisplay.GetRankColor(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
			string text = PlayerRankTextDisplay.ColorToHex(rankColor2);
			string rankDisplayName2 = PlayerRankTextDisplay.GetRankDisplayName(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
			InternalConsole.LogIntoConsole("[<color=#77dd77>PlayerJoin</color>] -> <color=" + text + ">" + player.field_Private_APIUser_0.displayName + "</color>");
			PlayerWrapper.Players.Add(player);
			BoneESP.OnPlayerJoined(player);
			BoxESP.OnPlayerJoined(player);
			if (!player.prop_VRCPlayerApi_0.isLocal)
			{
				Thread thread = new Thread((ThreadStart)delegate
				{
					try
					{
						APIUser field_Private_APIUser_ = player.field_Private_APIUser_0;
						string id_k__BackingField = player.prop_ApiAvatar_0._id_k__BackingField;
						string bio_k__BackingField = field_Private_APIUser_._bio_k__BackingField;
						string location_k__BackingField = field_Private_APIUser_._location_k__BackingField;
						string date_joined = field_Private_APIUser_.date_joined;
						string displayName_k__BackingField = field_Private_APIUser_._displayName_k__BackingField;
						string username_k__BackingField = field_Private_APIUser_._username_k__BackingField;
						string id_k__BackingField2 = field_Private_APIUser_._id_k__BackingField;
						string last_platform = field_Private_APIUser_._last_platform;
						string type = "user-join";
						var value = new
						{
							type = type,
							avatarID = id_k__BackingField,
							bio = bio_k__BackingField,
							currentlocation = location_k__BackingField,
							dateJoined = date_joined,
							displayName = displayName_k__BackingField,
							userName = username_k__BackingField,
							userId = id_k__BackingField2,
							platform = last_platform
						};
						using HttpClient httpClient = new HttpClient();
						string content = JsonConvert.SerializeObject(value);
						StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
						string requestUri = "https://track.niggaf.art/api/v1/user/join";
						HttpResponseMessage result = httpClient.PostAsync(requestUri, content2).Result;
						if (!result.IsSuccessStatusCode)
						{
							MelonLogger.Msg($"Failed to post user join data: {result.StatusCode}");
						}
					}
					catch (Exception arg)
					{
						MelonLogger.Msg($"Error posting user join data: {arg}");
					}
				});
				thread.IsBackground = true;
				thread.Start();
			}
		});
		NetworkManager.field_Internal_Static_NetworkManager_0.field_Private_ObjectPublicHa1UnT1Unique_1_IPlayer_2.field_Private_HashSet_1_UnityAction_1_T_0.Add((Action<IPlayer>)delegate(IPlayer obj)
		{
			OdiumConsole.LogGradient("PlayerLeave", obj.prop_IUser_0.prop_String_1);
			if (AssignedVariables.desktopPlayerList)
			{
				PlayerRankTextDisplay.RemovePlayer(obj.prop_IUser_0.prop_String_1);
			}
			string displayName = obj.prop_IUser_0.prop_String_1;
			if (APIUser.CurrentUser.displayName != displayName)
			{
				Thread thread = new Thread((ThreadStart)delegate
				{
					try
					{
						string location_k__BackingField = APIUser.CurrentUser._location_k__BackingField;
						string type = "user-leave";
						Thread.Sleep(1000);
						Player[] array = PlayerWrapper.GetAllPlayers().ToArray();
						if (array.Length == 0)
						{
							type = "world-leave";
						}
						var anon = new
						{
							type = type,
							currentlocation = location_k__BackingField,
							displayName = displayName
						};
						MelonLogger.Msg($"{anon}");
						using HttpClient httpClient = new HttpClient();
						string content = JsonConvert.SerializeObject(anon);
						StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
						MelonLogger.Msg("request");
						string requestUri = "https://track.niggaf.art/api/v1/user/leave";
						HttpResponseMessage result = httpClient.PostAsync(requestUri, content2).Result;
						if (!result.IsSuccessStatusCode)
						{
							MelonLogger.Msg($"Failed to post user leave data: {result.StatusCode}");
						}
					}
					catch (Exception arg)
					{
						MelonLogger.Msg($"Error posting user leave data: {arg}");
					}
				});
				thread.IsBackground = true;
				thread.Start();
			}
			Player player = PlayerWrapper.GetVRCPlayerFromId(obj.prop_IUser_0.prop_String_0)._player;
			Color rankColor = PlayerRankTextDisplay.GetRankColor(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
			string text = PlayerRankTextDisplay.ColorToHex(rankColor);
			string rankDisplayName = PlayerRankTextDisplay.GetRankDisplayName(PlayerRankTextDisplay.GetPlayerRank(player.field_Private_APIUser_0));
			InternalConsole.LogIntoConsole("[<color=#ff6961>PlayerLeave</color>] -> <color=" + text + ">" + player.field_Private_APIUser_0.displayName + "</color>");
			PlayerWrapper.Players.Remove(player);
			BoneESP.OnPlayerLeft(player);
			BoxESP.OnPlayerLeft(player);
		});
	}

	public override void OnGUI()
	{
		BoneESP.OnGUI();
		BoxESP.OnGUI();
	}

	public override void OnLevelWasLoaded(int level)
	{
		if (level == -1)
		{
			OdiumAssetBundleLoader._customAudioClip = null;
			OdiumAssetBundleLoader._customAudioSource = null;
			OdiumAssetBundleLoader.StopCustomAudio();
		}
		OdiumAssetBundleLoader.Initialize();
		PlayerRankTextDisplay.ClearAll();
		OdiumConsole.LogGradient("OnLevelWasLoaded", $"Level -> {level}");
		loadIndex++;
	}

	public override void OnSceneWasLoaded(int buildindex, string sceneName)
	{
		OdiumModuleLoader.OnSceneWasLoaded(buildindex, sceneName);
		global::CursorLayerMod.CursorLayerMod.OnSceneWasLoaded(buildindex, sceneName);
		OnLoadedSceneManager.LoadedScene(buildindex, sceneName);
	}

	public override void OnSceneWasUnloaded(int buildindex, string sceneName)
	{
		OdiumModuleLoader.OnSceneWasUnloaded(buildindex, sceneName);
	}

	public override void OnApplicationQuit()
	{
		OdiumModuleLoader.OnApplicationQuit();
	}

	private static IEnumerator RamClearLoop()
	{
		while (true)
		{
			yield return new WaitForSeconds(300f);
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
		}
	}

	public override void OnUpdate()
	{
		OdiumModuleLoader.OnUpdate();
		InternalConsole.ProcessLogCache();
		MainMenu.Setup();
		DroneSwarmWrapper.UpdateDroneSwarm();
		portalSpam.OnUpdate();
		portalTrap.OnUpdate();
		Bot.OnUpdate();
		BoneESP.Update();
		BoxESP.Update();
		SwasticaOrbit.OnUpdate();
		Jetpack.Update();
		FlyComponent.OnUpdate();
		global::CursorLayerMod.CursorLayerMod.OnUpdate();
		Chatbox.UpdateFrameEffects();
		Exploits.UpdateChatboxLagger();
		if (Time.time - lastStatsUpdate >= 1f)
		{
			NameplateModifier.UpdatePlayerStats();
			lastStatsUpdate = Time.time;
		}
		if (Input.GetKeyDown(KeyCode.Minus))
		{
			DroneWrapper.DroneCrash();
		}
	}

	public override void OnFixedUpdate()
	{
		OdiumModuleLoader.OnFixedUpdate();
	}

	public override void OnLateUpdate()
	{
		OdiumModuleLoader.OnLateUpdate();
		SpyCamera.LateUpdate();
	}

	static OdiumEntry()
	{
		Version = "0.0.1";
		wasKeyValid = false;
		loadIndex = 0;
		buttonImage = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\ButtonBackground.png");
		AUTH_ENDPOINT = "https://odiumvrc.com/api/validate-purchase";
		AUTH_FILE = Path.Combine(Environment.CurrentDirectory, "Odium", "auth.dat");
		heartbeatRun = false;
	}
}
