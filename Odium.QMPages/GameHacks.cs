using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using Odium.ButtonAPI.QM;
using Odium.Components;
using Odium.GameCheats;
using Odium.Odium;
using Odium.Patches;
using Odium.Wrappers;
using UnityEngine;
using VRC;
using VRC.SDKBase;

namespace Odium.QMPages;

internal class GameHacks
{
	public static void InitializePage(QMNestedMenu gameHacks, Sprite buttonImage)
	{
		Sprite sprite = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Murder.png");
		Sprite icon = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Kill.png");
		Sprite sprite2 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Win.png");
		Sprite sprite3 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\People.png");
		Sprite sprite4 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Gun.png");
		Sprite sprite5 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\WorldIcon.png");
		Sprite sprite6 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\FTAC.png");
		QMNestedMenu location = new QMNestedMenu(gameHacks, 1f, 0f, "<color=#8d142b>Murder 4</color>", "<color=#8d142b>Murder 4</color>", "Opens Select User menu", halfButton: false, sprite, buttonImage);
		QMNestedMenu btnMenu = new QMNestedMenu(location, 1f, 0f, "<color=#8d142b>Win Triggers</color>", "<color=#8d142b>Win Triggers</color>", "Opens Select User menu", halfButton: false, sprite2, buttonImage);
		QMNestedMenu btnMenu2 = new QMNestedMenu(location, 2f, 0f, "<color=#8d142b>Player Actions</color>", "<color=#8d142b>Player Actions</color>", "Opens Select User menu", halfButton: false, sprite3, buttonImage);
		QMNestedMenu qMNestedMenu = new QMNestedMenu(location, 3f, 0f, "<color=#8d142b>World Actions</color>", "<color=#8d142b>World Actions</color>", "Opens Select User menu", halfButton: false, sprite5, buttonImage);
		QMNestedMenu btnMenu3 = new QMNestedMenu(location, 4f, 0f, "<color=#8d142b>Gun Actions</color>", "<color=#8d142b>Gun Actions</color>", "Opens Select User menu", halfButton: false, sprite4, buttonImage);
		QMNestedMenu qMNestedMenu2 = new QMNestedMenu(location, 2.5f, 1f, "<color=#8d142b>Exploits</color>", "<color=#8d142b>Exploits</color>", "Opens Select User menu", halfButton: false, SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\ExploitIcon.png"), buttonImage);
		new QMSingleButton(qMNestedMenu2, 2f, 1.5f, "Crash All", delegate
		{
			try
			{
				string localPlayerName = Networking.LocalPlayer.displayName;
				List<Player> targets = (from player in PlayerManager.prop_PlayerManager_0.field_Private_List_1_Player_0.ToArray()
					where player != null && player.field_Private_APIUser_0 != null && player.field_Private_APIUser_0.displayName != localPlayerName && !player.field_Private_APIUser_0.isFriend
					select player).ToList();
				MelonCoroutines.Start(CrashPlayersWithDelay(targets));
			}
			catch (Exception)
			{
			}
		}, "Brings death to all players", halfBtn: false, icon, buttonImage);
		new QMToggleButton(qMNestedMenu2, 3f, 1.5f, "Instance Lock", delegate
		{
			AssignedVariables.instanceLock = true;
		}, delegate
		{
			AssignedVariables.instanceLock = false;
		}, "", defaultState: false, buttonImage);
		new QMSingleButton(btnMenu, 2f, 2f, "Murder", delegate
		{
			Murder4Utils.TriggerMurdererWin();
		}, "Brings death to all players", halfBtn: false, sprite2, buttonImage);
		new QMSingleButton(btnMenu, 3f, 2f, "Bystanders", delegate
		{
			Murder4Utils.TriggerBystanderWin();
		}, "Brings death to all players", halfBtn: false, sprite2, buttonImage);
		new QMSingleButton(btnMenu2, 1f, 0f, "Kill All", delegate
		{
			Murder4Utils.ExecuteAll();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(btnMenu2, 2f, 0f, "Blind All", delegate
		{
			Murder4Utils.BlindAll();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(btnMenu2, 3f, 0f, "Become Murderer", delegate
		{
			Murder4Utils.AssignRole(PlayerWrapper.LocalPlayer.field_Private_APIUser_0.displayName, "SyncAssignM");
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(btnMenu2, 4f, 0f, "Become Bystander", delegate
		{
			Murder4Utils.AssignRole(PlayerWrapper.LocalPlayer.field_Private_APIUser_0.displayName, "SyncAssignB");
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(btnMenu2, 1f, 1f, "Become Detective", delegate
		{
			Murder4Utils.AssignRole(PlayerWrapper.LocalPlayer.field_Private_APIUser_0.displayName, "SyncAssignD");
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 1f, 0f, "Open Doors", delegate
		{
			Murder4Utils.OpenDoors();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 2f, 0f, "Close Doors", delegate
		{
			Murder4Utils.CloseDoors();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 3f, 0f, "Unlock Doors", delegate
		{
			Murder4Utils.ForceOpenDoors();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 4f, 0f, "Lock Doors", delegate
		{
			Murder4Utils.LockDoors();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 1f, 1f, "Release Snake", delegate
		{
			Murder4Utils.SpawnSnake();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 2f, 1f, "Patreon Revolver", delegate
		{
			Murder4Utils.ApplyRevolverSkin();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 3f, 1f, "Start Match", delegate
		{
			Murder4Utils.StartGame();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 4f, 1f, "Find Murderer", delegate
		{
			Murder4Utils.IdentifyMurderer();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMToggleButton(qMNestedMenu, 1f, 2f, "Find Murderer", delegate
		{
			MelonCoroutines.Start(Murder4Utils.CreateKnifeShield(PlayerWrapper.LocalPlayer._vrcplayer));
		}, delegate
		{
			MelonCoroutines.Stop(Murder4Utils.CreateKnifeShield(PlayerWrapper.LocalPlayer._vrcplayer));
		}, "Brings death to all players", defaultState: false, buttonImage);
		new QMSingleButton(btnMenu3, 1f, 0f, "Fire Revolver", delegate
		{
			Murder4Utils.FireRevolver();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(btnMenu3, 2f, 0f, "Fire Shotgun", delegate
		{
			Murder4Utils.FireShotgun();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		new QMSingleButton(btnMenu3, 3f, 0f, "Fire Luger", delegate
		{
			Murder4Utils.FireLuger();
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
		QMNestedMenu btnMenu4 = new QMNestedMenu(gameHacks, 2f, 0f, "<color=#8d142b>FTAC</color>", "<color=#8d142b>FTAC</color>", "Opens Select User menu", halfButton: false, sprite6, buttonImage);
		new QMSingleButton(btnMenu4, 1f, 0f, "Trigger Group Board", delegate
		{
			FTACUdonUtils.SendEvent("OpenGroup");
		}, "Brings death to all players", halfBtn: false, null, buttonImage);
	}

	public static IEnumerator CrashPlayersWithDelay(List<Player> targets)
	{
		try
		{
			foreach (Player player in targets)
			{
				if (!(player == null))
				{
					for (int i = 0; i < 150; i++)
					{
						Murder4Utils.SendTargetedPatreonEvent(player, "ListPatrons");
						yield return new WaitForSeconds(0.4f);
					}
				}
			}
		}
		finally
		{
			PhotonPatches.BlockUdon = false;
		}
	}
}
