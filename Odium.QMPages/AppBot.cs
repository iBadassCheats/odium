using System;
using System.Collections.Generic;
using Odium.ApplicationBot;
using Odium.ButtonAPI.QM;
using Odium.Components;
using Odium.IUserPage;
using Odium.Wrappers;
using UnityEngine;

namespace Odium.QMPages;

internal class AppBot
{
	public static float xCount = 1f;

	public static float yCount = 0f;

	public static int botIndex = 0;

	public static List<QMNestedMenu> activeBots = new List<QMNestedMenu>();

	public static string Current_World_id => RoomManager.prop_ApiWorldInstance_0.id;

	public static string get_selected_player_name()
	{
		GameObject gameObject = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/UserProfile_Compact/PanelBG/Info/Text_Username_NonFriend");
		if (gameObject == null)
		{
			return "";
		}
		TextMeshProUGUIEx component = gameObject.GetComponent<TextMeshProUGUIEx>();
		if (component == null)
		{
			return "";
		}
		return component.text;
	}

	public static void InitializePage(QMNestedMenu appBotsButton, Sprite bgImage, Sprite halfButtonImage)
	{
		Sprite TeleportIcon = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\TeleportIcon.png");
		Sprite icon = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\GoHomeIcon.png");
		Sprite icon2 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\JoinMeIcon.png");
		Sprite sprite = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\OrbitIcon.png");
		Sprite icon3 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\CogWheelIcon.png");
		Sprite sprite2 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\MovementIcon.png");
		Sprite sprite3 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\OdiumIcon.png");
		QMNestedMenu qMNestedMenu = new QMNestedMenu(ApiUtils.GetSelectedUserPageGrid().transform, 0f, 0f, "<color=#8d142b>Odium</color>", "<color=#8d142b>Odium</color>", "Opens Select User menu", halfButton: true, sprite3, bgImage);
		QMNestedMenu appBotsPage = OdiumPage.Initialize(qMNestedMenu, bgImage);
		QMSingleButton qMSingleButton = new QMSingleButton(appBotsButton, 1f, 0f, "Join Me", delegate
		{
			global::Odium.ApplicationBot.Entry.ActiveBotIds.ForEach(delegate(string botId)
			{
				SocketConnection.SendCommandToClients("JoinWorld " + Current_World_id + " " + botId);
			});
		}, "Make all bots join your instance", halfBtn: false, icon2, bgImage);
		QMSingleButton qMSingleButton2 = new QMSingleButton(appBotsButton, 2f, 0f, "Go Home", delegate
		{
			global::Odium.ApplicationBot.Entry.ActiveBotIds.ForEach(delegate(string botId)
			{
				SocketConnection.SendCommandToClients("JoinWorld wrld_aeef8228-4e86-4774-9cbb-02027cf73730:91363~region(us) " + botId);
			});
		}, "Send all bots back to their home", halfBtn: false, icon, bgImage);
		QMToggleButton qMToggleButton = new QMToggleButton(appBotsButton, 4f, 0f, "USpeak Spam", delegate
		{
			global::Odium.ApplicationBot.Entry.ActiveBotIds.ForEach(delegate(string botId)
			{
				SocketConnection.SendCommandToClients("USpeakSpam true " + botId);
			});
		}, delegate
		{
			global::Odium.ApplicationBot.Entry.ActiveBotIds.ForEach(delegate(string botId)
			{
				SocketConnection.SendCommandToClients("USpeakSpam false " + botId);
			});
		}, "Toggle bots orbiting around you", defaultState: false, bgImage);
		QMSingleButton qMSingleButton3 = new QMSingleButton(appBotsButton, 1f, 2f, "TP To Me", delegate
		{
			global::Odium.ApplicationBot.Entry.ActiveBotIds.ForEach(delegate(string botId)
			{
				SocketConnection.SendCommandToClients("TeleportToPlayer " + PlayerWrapper.LocalPlayer.field_Private_APIUser_0.id + " " + botId);
			});
		}, "Teleport all bots to your location", halfBtn: false, TeleportIcon, bgImage);
		QMToggleButton qMToggleButton2 = new QMToggleButton(appBotsButton, 2f, 3f, "Orbit Me", delegate
		{
			global::Odium.ApplicationBot.Entry.ActiveBotIds.ForEach(delegate(string botId)
			{
				SocketConnection.SendCommandToClients("OrbitSelected " + PlayerWrapper.LocalPlayer.field_Private_APIUser_0.id + " " + botId);
			});
		}, delegate
		{
			global::Odium.ApplicationBot.Entry.ActiveBotIds.ForEach(delegate(string botId)
			{
				SocketConnection.SendCommandToClients("OrbitSelected 0 " + botId);
			});
		}, "Toggle bots orbiting around you", defaultState: false, bgImage);
		new QMToggleButton(appBotsButton, 1f, 3f, "Chatbox Lagger", delegate
		{
			global::Odium.ApplicationBot.Entry.ActiveBotIds.ForEach(delegate(string botId)
			{
				SocketConnection.SendCommandToClients("ChatBoxLagger true " + botId);
			});
		}, delegate
		{
			global::Odium.ApplicationBot.Entry.ActiveBotIds.ForEach(delegate(string botId)
			{
				SocketConnection.SendCommandToClients("OrbitSelected false " + botId);
			});
		}, "Toggle bots orbiting around you", defaultState: false, bgImage);
		QMNestedMenu qMNestedMenu2 = new QMNestedMenu(appBotsButton, 4f, 3.5f, "Profiles", "<color=#8d142b>Profiles</color>", "Manage bot profiles", halfButton: true, null, halfButtonImage);
		QMNestedMenu btnMenu = new QMNestedMenu(qMNestedMenu2, 4f, 3f, "Setup", "<color=#8d142b>Setup</color>", "Manage bot profiles", halfButton: true, null, halfButtonImage);
		QMNestedMenu btnMenu2 = new QMNestedMenu(qMNestedMenu2, 4f, 3.5f, "Launch", "<color=#8d142b>Launch</color>", "Manage bot profiles", halfButton: true, null, halfButtonImage);
		new QMSingleButton(btnMenu, 1f, 0f, "Bot 1", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotLogin(20);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu, 2f, 0f, "Bot 2", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotLogin(21);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu, 3f, 0f, "Bot 3", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotLogin(22);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu, 4f, 0f, "Bot 4", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotLogin(23);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu, 1f, 1f, "Bot 5", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotLogin(24);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu, 2f, 1f, "Bot 6", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotLogin(25);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu, 3f, 1f, "Bot 7", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotLogin(26);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu, 4f, 1f, "Bot 8", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotLogin(27);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(qMNestedMenu2, 1f, 3f, "Clear Bots", delegate
		{
			activeBots.ForEach(delegate(QMNestedMenu botMenu)
			{
				if (botMenu != null)
				{
					UnityEngine.Object.Destroy(botMenu.GetMenuObject());
				}
			});
		}, "Manage bot profiles", halfBtn: false, null, bgImage);
		new QMSingleButton(btnMenu2, 1f, 0f, "Bot 1", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotHeadless(20);
			string bot = global::Odium.ApplicationBot.Entry.ActiveBotIds[0];
			string text = bot.Split('-')[0];
			QMNestedMenu qMNestedMenu3 = new QMNestedMenu(qMNestedMenu2, xCount, yCount, text, "<color=#8d142b>" + text + "</color>", "Manage bot profiles", halfButton: false, null, bgImage);
			new QMSingleButton(qMNestedMenu3, 2f, 1.5f, "TP To Me", delegate
			{
				SocketConnection.SendCommandToClients("TeleportToPlayer " + PlayerWrapper.LocalPlayer.field_Private_APIUser_0.id + " " + bot);
			}, "Teleport all bots to your location", halfBtn: false, TeleportIcon, bgImage);
			new QMToggleButton(qMNestedMenu3, 3f, 1.5f, "Chatbox Lagger", delegate
			{
				SocketConnection.SendCommandToClients("ChatBoxLagger true " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected false " + bot);
			}, "Toggle bots orbiting around you", defaultState: false, bgImage);
			QMNestedMenu qMNestedMenu4 = new QMNestedMenu(appBotsPage, xCount, yCount, text, "<color=#8d142b>" + text + "</color>", "Manage bot profiles", halfButton: false, null, bgImage);
			new QMSingleButton(qMNestedMenu4, 1f, 0f, "TP To", delegate
			{
				SocketConnection.SendCommandToClients("TeleportToPlayer " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, "Teleport all bots to your location", halfBtn: false, TeleportIcon, bgImage);
			new QMToggleButton(qMNestedMenu4, 2f, 0f, "Orbit", delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected 0");
			}, "Make bots orbit the selected player", defaultState: false, bgImage);
			new QMToggleButton(qMNestedMenu4, 3f, 0f, "Portal Spam", delegate
			{
				SocketConnection.SendCommandToClients("PortalSpam " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("PortalSpam 0");
			}, "Make bots mimic selected player's movement", defaultState: false, bgImage);
			new QMToggleButton(qMNestedMenu4, 4f, 0f, "IK Mimic", delegate
			{
				SocketConnection.SendCommandToClients("MovementMimic " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("MovementMimic 0");
			}, "Make bots mimic selected player's movement", defaultState: false, bgImage);
			activeBots.Add(qMNestedMenu3);
			xCount += 1f;
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu2, 2f, 0f, "Bot 2", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotHeadless(21);
			string bot = global::Odium.ApplicationBot.Entry.ActiveBotIds[1];
			string text = bot.Split('-')[0];
			QMNestedMenu qMNestedMenu3 = new QMNestedMenu(qMNestedMenu2, xCount, yCount, text, "<color=#8d142b>" + text + "</color>", "Manage bot profiles", halfButton: false, null, bgImage);
			new QMSingleButton(qMNestedMenu3, 2f, 1.5f, "TP To Me", delegate
			{
				SocketConnection.SendCommandToClients("TeleportToPlayer " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, "Teleport all bots to your location", halfBtn: false, TeleportIcon, bgImage);
			new QMToggleButton(qMNestedMenu3, 3f, 1.5f, "Chatbox Lagger", delegate
			{
				SocketConnection.SendCommandToClients("ChatBoxLagger true " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected false " + bot);
			}, "Toggle bots orbiting around you", defaultState: false, bgImage);
			QMNestedMenu qMNestedMenu4 = new QMNestedMenu(appBotsPage, xCount, yCount, text, "<color=#8d142b>" + text + "</color>", "Manage bot profiles", halfButton: false, null, bgImage);
			new QMSingleButton(qMNestedMenu4, 1f, 0f, "TP To", delegate
			{
				SocketConnection.SendCommandToClients("TeleportToPlayer " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, "Teleport all bots to your location", halfBtn: false, TeleportIcon, bgImage);
			new QMToggleButton(qMNestedMenu4, 2f, 0f, "Orbit", delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected 0");
			}, "Make bots orbit the selected player", defaultState: false, bgImage);
			new QMToggleButton(qMNestedMenu4, 3f, 0f, "Portal Spam", delegate
			{
				SocketConnection.SendCommandToClients("PortalSpam " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("PortalSpam 0");
			}, "Make bots mimic selected player's movement", defaultState: false, bgImage);
			new QMToggleButton(qMNestedMenu4, 4f, 0f, "IK Mimic", delegate
			{
				SocketConnection.SendCommandToClients("MovementMimic " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("MovementMimic 0");
			}, "Make bots mimic selected player's movement", defaultState: false, bgImage);
			activeBots.Add(qMNestedMenu3);
			xCount += 1f;
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu2, 3f, 0f, "Bot 3", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotHeadless(22);
			string bot = global::Odium.ApplicationBot.Entry.ActiveBotIds[2];
			string text = bot.Split('-')[0];
			QMNestedMenu qMNestedMenu3 = new QMNestedMenu(qMNestedMenu2, xCount, yCount, text, "<color=#8d142b>" + text + "</color>", "Manage bot profiles", halfButton: false, null, bgImage);
			new QMSingleButton(qMNestedMenu3, 2f, 1.5f, "TP To Me", delegate
			{
				SocketConnection.SendCommandToClients("TeleportToPlayer " + PlayerWrapper.LocalPlayer.field_Private_APIUser_0.id + " " + bot);
			}, "Teleport all bots to your location", halfBtn: false, TeleportIcon, bgImage);
			new QMToggleButton(qMNestedMenu3, 3f, 1.5f, "Chatbox Lagger", delegate
			{
				SocketConnection.SendCommandToClients("ChatBoxLagger true " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected false " + bot);
			}, "Toggle bots orbiting around you", defaultState: false, bgImage);
			QMNestedMenu qMNestedMenu4 = new QMNestedMenu(appBotsPage, xCount, yCount, text, "<color=#8d142b>" + text + "</color>", "Manage bot profiles", halfButton: false, null, bgImage);
			new QMSingleButton(qMNestedMenu4, 1f, 0f, "TP To", delegate
			{
				SocketConnection.SendCommandToClients("TeleportToPlayer " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, "Teleport all bots to your location", halfBtn: false, TeleportIcon, bgImage);
			new QMToggleButton(qMNestedMenu4, 2f, 0f, "Orbit", delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected 0");
			}, "Make bots orbit the selected player", defaultState: false, bgImage);
			new QMToggleButton(qMNestedMenu4, 3f, 0f, "Portal Spam", delegate
			{
				SocketConnection.SendCommandToClients("PortalSpam " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("PortalSpam 0");
			}, "Make bots mimic selected player's movement", defaultState: false, bgImage);
			new QMToggleButton(qMNestedMenu4, 4f, 0f, "IK Mimic", delegate
			{
				SocketConnection.SendCommandToClients("MovementMimic " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("MovementMimic 0");
			}, "Make bots mimic selected player's movement", defaultState: false, bgImage);
			activeBots.Add(qMNestedMenu3);
			xCount += 1f;
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu2, 4f, 0f, "Bot 4", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotHeadless(23);
			string bot = global::Odium.ApplicationBot.Entry.ActiveBotIds[3];
			string text = bot.Split('-')[0];
			QMNestedMenu qMNestedMenu3 = new QMNestedMenu(qMNestedMenu2, xCount, yCount, text, "<color=#8d142b>" + text + "</color>", "Manage bot profiles", halfButton: false, null, bgImage);
			new QMSingleButton(qMNestedMenu3, 2f, 1.5f, "TP To Me", delegate
			{
				SocketConnection.SendCommandToClients("TeleportToPlayer " + PlayerWrapper.LocalPlayer.field_Private_APIUser_0.id + " " + bot);
			}, "Teleport all bots to your location", halfBtn: false, TeleportIcon, bgImage);
			new QMToggleButton(qMNestedMenu3, 3f, 1.5f, "Chatbox Lagger", delegate
			{
				SocketConnection.SendCommandToClients("ChatBoxLagger true " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected false " + bot);
			}, "Toggle bots orbiting around you", defaultState: false, bgImage);
			QMNestedMenu qMNestedMenu4 = new QMNestedMenu(appBotsPage, xCount, yCount, text, "<color=#8d142b>" + text + "</color>", "Manage bot profiles", halfButton: false, null, bgImage);
			new QMSingleButton(qMNestedMenu4, 1f, 0f, "TP To", delegate
			{
				SocketConnection.SendCommandToClients("TeleportToPlayer " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, "Teleport all bots to your location", halfBtn: false, TeleportIcon, bgImage);
			new QMToggleButton(qMNestedMenu4, 2f, 0f, "Orbit", delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("OrbitSelected 0");
			}, "Make bots orbit the selected player", defaultState: false, bgImage);
			new QMToggleButton(qMNestedMenu4, 3f, 0f, "Portal Spam", delegate
			{
				SocketConnection.SendCommandToClients("PortalSpam " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("PortalSpam 0");
			}, "Make bots mimic selected player's movement", defaultState: false, bgImage);
			new QMToggleButton(qMNestedMenu4, 4f, 0f, "IK Mimic", delegate
			{
				SocketConnection.SendCommandToClients("MovementMimic " + PlayerWrapper.GetPlayerByDisplayName(get_selected_player_name()).field_Private_APIUser_0.id + " " + bot);
			}, delegate
			{
				SocketConnection.SendCommandToClients("MovementMimic 0");
			}, "Make bots mimic selected player's movement", defaultState: false, bgImage);
			activeBots.Add(qMNestedMenu3);
			xCount += 1f;
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu2, 1f, 1f, "Bot 5", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotHeadless(24);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu2, 2f, 1f, "Bot 6", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotHeadless(25);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu2, 3f, 1f, "Bot 7", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotHeadless(26);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
		new QMSingleButton(btnMenu2, 4f, 1f, "Bot 8", delegate
		{
			global::Odium.ApplicationBot.Entry.LaunchBotHeadless(27);
		}, "Send all bots back to their home", halfBtn: false, icon3, bgImage);
	}
}
