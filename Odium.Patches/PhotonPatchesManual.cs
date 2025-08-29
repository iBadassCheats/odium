using System;
using System.Reflection;
using ExitGames.Client.Photon;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Odium.Components;
using Odium.Modules;
using Odium.Odium;
using Odium.UX;
using Odium.Wrappers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using VampClient.Api;
using VRC;
using VRC.Udon;

namespace Odium.Patches;

public static class PhotonPatchesManual
{
	public static void ApplyPatches()
	{
		OdiumEntry.HarmonyInstance.Patch(typeof(LoadBalancingClient).GetMethod("OnEvent", new System.Type[1] { typeof(EventData) }), new HarmonyMethod(typeof(PhotonPatchesManual).GetMethod("OnEvent", BindingFlags.Static | BindingFlags.Public)));
	}

	public static bool OnEvent(LoadBalancingClient __instance, ref EventData param_1)
	{
		byte code = param_1.Code;
		switch (code)
		{
		case 12:
			PhotonPatches.UpdatePlayerActivity(param_1.sender, code);
			break;
		case 34:
			Ratelimit.ProcessRateLimit(ref param_1);
			break;
		case 11:
			if (PhotonPatches.BlockUdon)
			{
				return false;
			}
			break;
		case 18:
			try
			{
				if (param_1?.Parameters == null || !param_1.Parameters.ContainsKey(param_1.CustomDataKey))
				{
					object arg = param_1?.Parameters == null;
					EventData obj3 = param_1;
					bool? obj4;
					if (obj3 == null)
					{
						obj4 = null;
					}
					else
					{
						ParameterDictionary parameters = obj3.Parameters;
						obj4 = ((parameters != null) ? new bool?(!parameters.ContainsKey(param_1.CustomDataKey)) : ((bool?)null));
					}
					InternalConsole.LogIntoConsole($"[BLOCK] Invalid event parameters - Parameters null: {arg}, Missing key: {obj4}");
					return false;
				}
				Dictionary<byte, Il2CppSystem.Object> dictionary2 = param_1.Parameters[param_1.CustomDataKey].Cast<Dictionary<byte, Il2CppSystem.Object>>();
				if (dictionary2 == null || !dictionary2.ContainsKey(0) || !dictionary2.ContainsKey(1) || !dictionary2.ContainsKey(2))
				{
					InternalConsole.LogIntoConsole($"[BLOCK] Missing required dictionary keys - Dict null: {dictionary2 == null}, Has key 0: {dictionary2?.ContainsKey(0)}, Has key 1: {dictionary2?.ContainsKey(1)}, Has key 2: {dictionary2?.ContainsKey(2)}");
					return false;
				}
				if (!PhotonPatches.TryUnboxInt(dictionary2[2], out var result) || !PhotonPatches.TryUnboxByte(dictionary2[0], out var result2) || !PhotonPatches.TryUnboxInt(dictionary2[1], out var result3))
				{
					InternalConsole.LogIntoConsole($"[BLOCK] Invalid data types - viewId unbox: {PhotonPatches.TryUnboxInt(dictionary2[2], out var result4)}, eventType unbox: {PhotonPatches.TryUnboxByte(dictionary2[0], out var _)}, eventHash unbox: {PhotonPatches.TryUnboxInt(dictionary2[1], out result4)}");
					return false;
				}
				if (result < 0 || result > 999999)
				{
					InternalConsole.LogIntoConsole($"[BLOCK] Suspicious viewId: {result} (range: 0-999999)");
					return false;
				}
				if (result2 == 1 || result2 == 2)
				{
					PhotonView photonView = PhotonView.Method_Public_Static_PhotonView_Int32_0(result);
					if (photonView == null || photonView.gameObject == null)
					{
						InternalConsole.LogIntoConsole($"[BLOCK] Invalid PhotonView for viewId: {result} - PhotonView null: {photonView == null}, GameObject null: {photonView?.gameObject == null}");
						return false;
					}
					UdonBehaviour component = photonView.gameObject.GetComponent<UdonBehaviour>();
					if (component == null)
					{
						return true;
					}
					string name = component.gameObject.name;
					uint num2 = (uint)result3;
					VRC.Player vRCPlayerFromActorNr3 = PlayerWrapper.GetVRCPlayerFromActorNr(param_1.sender);
					if (vRCPlayerFromActorNr3?.field_Private_APIUser_0 == null)
					{
						InternalConsole.LogIntoConsole($"[BLOCK] Invalid player data - Player null: {vRCPlayerFromActorNr3 == null}, APIUser null: {vRCPlayerFromActorNr3?.field_Private_APIUser_0 == null}");
						return false;
					}
					string text7 = vRCPlayerFromActorNr3.field_Private_APIUser_0.displayName ?? "Unknown";
					string text8 = vRCPlayerFromActorNr3.field_Private_APIUser_0.id ?? "Unknown";
					if (!PhotonPatches.crashAttemptCounts.ContainsKey(text7))
					{
						PhotonPatches.crashAttemptCounts[text7] = 0;
					}
					if (!PhotonPatches.lastLogTimes.ContainsKey(text7))
					{
						PhotonPatches.lastLogTimes[text7] = System.DateTime.MinValue;
					}
					bool flag3 = false;
					string text9 = "";
					if (component.TryGetEntrypointNameFromHash(num2, out var entrypoint))
					{
						switch (entrypoint)
						{
						default:
							if (!(entrypoint == "KillSync"))
							{
								break;
							}
							goto case "SyncAssignM";
						case "SyncAssignM":
						case "SyncAssignD":
						case "SyncAssignB":
						{
							string key = text8 + "_" + entrypoint;
							System.DateTime now = System.DateTime.Now;
							if (PhotonPatches.syncAssignMCooldowns.ContainsKey(key))
							{
								System.DateTime dateTime = PhotonPatches.syncAssignMCooldowns[key];
								System.TimeSpan timeSpan = now - dateTime;
								if (timeSpan < PhotonPatches.SYNC_ASSIGN_M_COOLDOWN)
								{
									System.TimeSpan timeSpan2 = PhotonPatches.SYNC_ASSIGN_M_COOLDOWN - timeSpan;
									InternalConsole.LogIntoConsole($"[RATELIMIT] SyncAssignM blocked for {text7} - Cooldown remaining: {timeSpan2.TotalSeconds:F1}s");
									if ((now - PhotonPatches.lastLogTimes[text7]).TotalSeconds >= 30.0)
									{
										ToastBase.Toast("Odium Protection", $"SyncAssignM rate limited for '{text7}' - {timeSpan2.TotalSeconds:F0}s remaining", PhotonPatches.LogoIcon, 3f);
										PhotonPatches.lastLogTimes[text7] = now;
									}
									return false;
								}
							}
							PhotonPatches.syncAssignMCooldowns[key] = now;
							InternalConsole.LogIntoConsole("[RATELIMIT] SyncAssignM allowed for " + text7 + " - Cooldown set for 2 minutes");
							break;
						}
						}
						OdiumConsole.Log("Udon", string.Format("\r\n======= {0} =======\r\nPlayer ID -> {1}\r\nBehavior Name -> {2}\r\nEntry Point Name -> {3}\r\nEvent Hash -> {4}\r\nEvent Type -> {5}\r\nGameObject Path -> {6}\r\nTimestamp -> {7:yyyy-MM-dd HH:mm:ss.fff}\r\nRate Limited -> {8}\r\n", text7, text8, name, entrypoint, num2, result2, PhotonPatches.GetGameObjectPath(photonView.gameObject), System.DateTime.Now, (entrypoint == "SyncAssignM") ? "Yes (2min)" : "No"));
						if (entrypoint == "ListPatrons" && name == "Patreon Credits")
						{
							flag3 = true;
							text9 = "ListPatrons exploit";
						}
						else if (PhotonPatches.IsKnownExploitPattern(entrypoint, name))
						{
							if (!AssignedVariables.preventPatreonCrash)
							{
								flag3 = false;
							}
							else
							{
								flag3 = true;
								text9 = "Known exploit pattern";
							}
						}
					}
					if (!flag3)
					{
						if (PhotonPatches.IsRapidFireEvent(text7))
						{
							flag3 = true;
							text9 = "Rapid-fire events";
						}
						if (PhotonPatches.IsUnusualHash(num2))
						{
							flag3 = true;
							text9 = "Unusual hash value";
						}
					}
					if (flag3)
					{
						PhotonPatches.crashAttemptCounts[text7]++;
						int num3 = PhotonPatches.crashAttemptCounts[text7];
						System.DateTime now2 = System.DateTime.Now;
						if ((now2 - PhotonPatches.lastLogTimes[text7]).TotalSeconds >= 15.0)
						{
							InternalConsole.LogIntoConsole("-> Prevented: " + text7 + " [Reason: " + text9 + "]");
							ToastBase.Toast("Odium Protection", "Potentially harmful event blocked from user '" + text7 + "' (Reason: " + text9 + ")", PhotonPatches.LogoIcon);
							PhotonPatches.lastLogTimes[text7] = now2;
						}
						return false;
					}
				}
			}
			catch (System.Exception ex3)
			{
				InternalConsole.LogIntoConsole("Error in event protection: " + ex3.Message);
				return false;
			}
			return true;
		case 43:
		{
			if (!AssignedVariables.chatBoxAntis)
			{
				return true;
			}
			string text6 = "";
			try
			{
				byte[] bytes = param_1.CustomData.Il2ToByteArray();
				text6 = ChatboxLogger.ConvertBytesToText(bytes);
				text6 = text6.Replace("\ufffd", "").Replace("\v", "").Replace("\"", "")
					.Trim();
			}
			catch (System.Exception ex2)
			{
				OdiumConsole.Log("ChatBox", "Error extracting message: " + ex2.Message);
				text6 = "[Error extracting message]";
			}
			if (ChatboxAntis.IsMessageValid(text6))
			{
				return true;
			}
			if (!PhotonPatches.blockedMessagesCount.ContainsKey(param_1.sender))
			{
				PhotonPatches.blockedMessagesCount[param_1.sender] = 0;
				PhotonPatches.blockedMessages[param_1.sender] = 0;
			}
			PhotonPatches.blockedMessagesCount[param_1.sender]++;
			PhotonPatches.blockedMessages[param_1.sender]++;
			if (PhotonPatches.blockedMessagesCount[param_1.sender] == 1)
			{
				VRC.Player vRCPlayerFromActorNr = PlayerWrapper.GetVRCPlayerFromActorNr(param_1.sender);
				InternalConsole.LogIntoConsole("<color=red>Blocked chatbox message from user -> " + vRCPlayerFromActorNr.field_Private_APIUser_0.displayName + "</color>");
			}
			else if (PhotonPatches.blockedMessages[param_1.sender] >= 100)
			{
				VRC.Player vRCPlayerFromActorNr2 = PlayerWrapper.GetVRCPlayerFromActorNr(param_1.sender);
				InternalConsole.LogIntoConsole($"<color=red>Blocked {PhotonPatches.blockedMessagesCount[param_1.sender]} total chatbox messages from user -> {vRCPlayerFromActorNr2.field_Private_APIUser_0.displayName}</color>");
				PhotonPatches.blockedMessages[param_1.sender] = 0;
			}
			return false;
		}
		case 33:
			if (param_1.Parameters != null && param_1.Parameters.ContainsKey(245))
			{
				Dictionary<byte, Il2CppSystem.Object> dictionary = param_1.Parameters[245].TryCast<Dictionary<byte, Il2CppSystem.Object>>();
				if (dictionary != null)
				{
					if (dictionary.ContainsKey(0))
					{
						byte b = dictionary[0].Unbox<byte>();
					}
					if (dictionary.ContainsKey(1))
					{
						int id = dictionary[1].Unbox<int>();
						if (dictionary.ContainsKey(10))
						{
							bool flag = dictionary[10].Unbox<bool>();
							if (flag && !PhotonPatches.blockedUserIds.Contains(id.ToString()))
							{
								PhotonPatches.blockedUserIds.Add(id.ToString());
								VRCPlayer playerFromPhotonId2 = PlayerWrapper.GetPlayerFromPhotonId(id);
								if (AssignedVariables.announceBlocks)
								{
									Chatbox.SendCustomChatMessage("[Odium] -> " + playerFromPhotonId2.field_Private_VRCPlayerApi_0.displayName + " BLOCKED me");
								}
								Color rankColor2 = NameplateModifier.GetRankColor(NameplateModifier.GetPlayerRank(playerFromPhotonId2._player.field_Private_APIUser_0));
								string text2 = NameplateModifier.ColorToHex(rankColor2);
								OdiumBottomNotification.ShowNotification("<color=#FF5151>BLOCKED</color> by <color=" + text2 + ">" + playerFromPhotonId2.field_Private_VRCPlayerApi_0.displayName);
								InternalConsole.LogIntoConsole("<color=#7B02FE>" + playerFromPhotonId2.field_Private_VRCPlayerApi_0.displayName + "</color> <color=red>BLOCKED</color> you!");
							}
							else if (!flag && PhotonPatches.blockedUserIds.Contains(id.ToString()))
							{
								PhotonPatches.blockedUserIds.Remove(id.ToString());
								VRCPlayer playerFromPhotonId3 = PlayerWrapper.GetPlayerFromPhotonId(id);
								if (AssignedVariables.announceBlocks)
								{
									Chatbox.SendCustomChatMessage("[Odium] -> " + playerFromPhotonId3.field_Private_VRCPlayerApi_0.displayName + " UNBLOCKED me");
								}
								Color rankColor3 = NameplateModifier.GetRankColor(NameplateModifier.GetPlayerRank(playerFromPhotonId3._player.field_Private_APIUser_0));
								string text3 = NameplateModifier.ColorToHex(rankColor3);
								OdiumBottomNotification.ShowNotification("<color=#FF5151>UNBLOCKED</color> by <color=" + text3 + ">" + playerFromPhotonId3.field_Private_VRCPlayerApi_0.displayName);
								InternalConsole.LogIntoConsole("<color=#7B02FE>" + playerFromPhotonId3.field_Private_VRCPlayerApi_0.displayName + "</color> <color=red>UNBLOCKED</color> you!");
							}
						}
						if (dictionary.ContainsKey(11))
						{
							bool flag2 = dictionary[11].Unbox<bool>();
							if (flag2 && !PhotonPatches.mutedUserIds.Contains(id.ToString()))
							{
								PhotonPatches.mutedUserIds.Add(id.ToString());
								VRCPlayer playerFromPhotonId4 = PlayerWrapper.GetPlayerFromPhotonId(id);
								if (AssignedVariables.announceMutes)
								{
									Chatbox.SendCustomChatMessage("[Odium] -> " + playerFromPhotonId4.field_Private_VRCPlayerApi_0.displayName + " MUTED me");
								}
								Color rankColor4 = NameplateModifier.GetRankColor(NameplateModifier.GetPlayerRank(playerFromPhotonId4._player.field_Private_APIUser_0));
								string text4 = NameplateModifier.ColorToHex(rankColor4);
								OdiumBottomNotification.ShowNotification("<color=#FF5151>MUTED</color> by <color=" + text4 + ">" + playerFromPhotonId4.field_Private_VRCPlayerApi_0.displayName);
								InternalConsole.LogIntoConsole("<color=#7B02FE>" + playerFromPhotonId4.field_Private_VRCPlayerApi_0.displayName + "</color> <color=red>MUTED</color> you!");
							}
							else if (!flag2 && PhotonPatches.mutedUserIds.Contains(id.ToString()))
							{
								PhotonPatches.mutedUserIds.Remove(id.ToString());
								VRCPlayer playerFromPhotonId5 = PlayerWrapper.GetPlayerFromPhotonId(id);
								if (AssignedVariables.announceMutes)
								{
									Chatbox.SendCustomChatMessage("[Odium] -> " + playerFromPhotonId5.field_Private_VRCPlayerApi_0.displayName + " unfortunately UNMUTED me");
								}
								Color rankColor5 = NameplateModifier.GetRankColor(NameplateModifier.GetPlayerRank(playerFromPhotonId5._player.field_Private_APIUser_0));
								string text5 = NameplateModifier.ColorToHex(rankColor5);
								OdiumBottomNotification.ShowNotification("<color=#FF5151>UNMUTED</color> by <color=" + text5 + ">" + playerFromPhotonId5.field_Private_VRCPlayerApi_0.displayName);
								InternalConsole.LogIntoConsole("<color=#7B02FE>" + playerFromPhotonId5.field_Private_VRCPlayerApi_0.displayName + "</color> <color=red>UNMUTED</color> you!");
							}
						}
					}
				}
			}
			if (param_1.Parameters != null && param_1.Parameters.ContainsKey(254))
			{
				Il2CppSystem.Object obj2 = param_1.Parameters[254];
			}
			break;
		case 208:
			try
			{
				InternalConsole.LogIntoConsole("<color=#31BCF0>[MasterClient]:</color> Master client switch detected");
				if (param_1.Parameters == null || !param_1.Parameters.ContainsKey(254))
				{
					break;
				}
				Il2CppSystem.Object obj = param_1.Parameters[254];
				if (obj != null)
				{
					int num = obj.Unbox<int>();
					VRCPlayer playerFromPhotonId = PlayerWrapper.GetPlayerFromPhotonId(num);
					if (playerFromPhotonId != null)
					{
						Color rankColor = NameplateModifier.GetRankColor(NameplateModifier.GetPlayerRank(playerFromPhotonId._player.field_Private_APIUser_0));
						string text = NameplateModifier.ColorToHex(rankColor);
						InternalConsole.LogIntoConsole($"<color=#31BCF0>[MasterClient]:</color> New master: <color={text}>{playerFromPhotonId.field_Private_VRCPlayerApi_0.displayName}</color> (ID: {num})");
						OdiumBottomNotification.ShowNotification("<color=" + text + ">" + playerFromPhotonId.field_Private_VRCPlayerApi_0.displayName + "</color> is the new <color=#FFECA1>Master</color>");
					}
					else
					{
						InternalConsole.LogIntoConsole($"<color=#31BCF0>[MasterClient]:</color> New master actor ID: {num} (Player not found)");
					}
				}
			}
			catch (System.Exception ex)
			{
				InternalConsole.LogIntoConsole("<color=#31BCF0>[MasterClient]:</color> <color=red>Error processing master client switch: " + ex.Message + "</color>");
			}
			break;
		}
		return true;
	}
}
