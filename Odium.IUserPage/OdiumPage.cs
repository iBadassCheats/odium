using System;
using System.Windows.Forms;
using Odium.ButtonAPI.QM;
using Odium.Components;
using Odium.GameCheats;
using Odium.Patches;
using Odium.Wrappers;
using UnityEngine;
using VRC;

namespace Odium.IUserPage;

internal class OdiumPage
{
	public static float defaultVoiceGain;

	public static QMNestedMenu Initialize(QMNestedMenu qMNestedMenu1, Sprite bgImage)
	{
		Sprite icon = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\TeleportIcon.png");
		Sprite sprite = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\GoHomeIcon.png");
		Sprite sprite2 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\JoinMeIcon.png");
		Sprite sprite3 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\OrbitIcon.png");
		Sprite sprite4 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\CogWheelIcon.png");
		Sprite sprite5 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\MovementIcon.png");
		Sprite sprite6 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\OdiumIcon.png");
		Sprite icon2 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\InfoIcon.png");
		Sprite sprite7 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Murder.png");
		QMNestedMenu result = new QMNestedMenu(qMNestedMenu1, 1f, 1f, "App Bots", "<color=#8d142b>App Bots</color>", "Opens Select User menu", halfButton: false, null, bgImage);
		QMNestedMenu qMNestedMenu2 = new QMNestedMenu(qMNestedMenu1, 2f, 1f, "Pickups", "<color=#8d142b>Pickups</color>", "Opens Select User menu", halfButton: false, null, bgImage);
		QMNestedMenu qMNestedMenu3 = new QMNestedMenu(qMNestedMenu1, 3f, 1f, "Functions", "<color=#8d142b>Functions</color>", "Opens Select User menu", halfButton: false, null, bgImage);
		QMNestedMenu location = new QMNestedMenu(qMNestedMenu1, 4f, 1f, "Spy Utils", "<color=#8d142b>Spy Utils</color>", "Opens Select User menu", halfButton: false, null, bgImage);
		QMNestedMenu location2 = new QMNestedMenu(qMNestedMenu1, 2.5f, 2f, "Murder 4", "<color=#8d142b>Murder 4</color>", "Opens Select User menu", halfButton: false, sprite7, bgImage);
		Sprite sprite8 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Kill.png");
		Sprite sprite9 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Win.png");
		Sprite sprite10 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\People.png");
		Sprite sprite11 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Gun.png");
		Sprite sprite12 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\WorldIcon.png");
		Sprite sprite13 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\FTAC.png");
		QMNestedMenu btnMenu = new QMNestedMenu(location2, 2f, 0f, "<color=#8d142b>Win Triggers</color>", "<color=#8d142b>Win Triggers</color>", "Opens Select User menu", halfButton: false, sprite9, bgImage);
		QMNestedMenu btnMenu2 = new QMNestedMenu(location2, 3f, 0f, "<color=#8d142b>Player Actions</color>", "<color=#8d142b>Player Actions</color>", "Opens Select User menu", halfButton: false, sprite10, bgImage);
		QMNestedMenu btnMenu3 = new QMNestedMenu(location2, 2.5f, 1f, "<color=#8d142b>Exploits</color>", "<color=#8d142b>Exploits</color>", "Opens Select User menu", halfButton: false, SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\ExploitIcon.png"), bgImage);
		new QMSingleButton(btnMenu3, 2.5f, 1.5f, "Crash", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			PhotonPatches.BlockUdon = true;
			for (int i = 0; i < 300; i++)
			{
				Murder4Utils.SendTargetedPatreonEvent(iUser, "ListPatrons");
			}
			PhotonPatches.BlockUdon = false;
		}, "Brings death to all players", halfBtn: false, null, bgImage);
		new QMSingleButton(btnMenu, 2f, 2f, "Assign Murderer", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			Murder4Utils.SendTargetedEvent(iUser, "SyncAssignM");
		}, "Brings death to all players", halfBtn: false, sprite9, bgImage);
		new QMSingleButton(btnMenu, 3f, 2f, "Assign Bystander", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			Murder4Utils.SendTargetedEvent(iUser, "SyncAssignB");
		}, "Brings death to all players", halfBtn: false, sprite9, bgImage);
		new QMSingleButton(btnMenu2, 1f, 0f, "Assign Detective", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			Murder4Utils.SendTargetedEvent(iUser, "SyncAssignD");
		}, "Brings death to all players", halfBtn: false, null, bgImage);
		new QMSingleButton(btnMenu2, 2f, 0f, "Blind", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			Murder4Utils.SendTargetedEvent(iUser, "SyncAssignD");
		}, "Brings death to all players", halfBtn: false, null, bgImage);
		new QMSingleButton(btnMenu2, 3f, 0f, "Explode", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			Murder4Utils.ExplodeAtTarget(iUser);
		}, "Brings death to all players", halfBtn: false, null, bgImage);
		new QMToggleButton(location, 1.5f, 2f, "Spy USpeak", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			PlayerExtraMethods.focusTargetAudio(iUser, state: true);
		}, delegate
		{
			Player iUser = ApiUtils.GetIUser();
			PlayerExtraMethods.focusTargetAudio(iUser, state: false);
		}, "Focus audio on a single user and mutes everyone else", defaultState: false, bgImage);
		new QMToggleButton(location, 2.5f, 2f, "Max Voice Range", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			PlayerExtraMethods.setInfiniteVoiceRange(iUser, state: true);
		}, delegate
		{
			Player iUser = ApiUtils.GetIUser();
			PlayerExtraMethods.setInfiniteVoiceRange(iUser, state: false);
		}, "Hear people from whatever distance they are", defaultState: false, bgImage);
		new QMToggleButton(location, 3.5f, 2f, "Spy Camera", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			SpyCamera.Toggle(iUser.field_Private_VRCPlayerApi_0, state: true);
		}, delegate
		{
			Player iUser = ApiUtils.GetIUser();
			SpyCamera.Toggle(iUser.field_Private_VRCPlayerApi_0, state: false);
		}, "Allows to see from the point of view of other users", defaultState: false, bgImage);
		new QMSingleButton(qMNestedMenu3, 1.5f, 1f, "TP Behind", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			PlayerExtraMethods.teleportBehind(iUser);
		}, "Teleport behind selected player facing them", halfBtn: false, icon, bgImage);
		new QMToggleButton(qMNestedMenu3, 2.5f, 1f, "Portal Spam", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			ActionWrapper.portalSpam = true;
			ActionWrapper.portalSpamPlayer = iUser;
		}, delegate
		{
			ActionWrapper.portalSpam = false;
			ActionWrapper.portalSpamPlayer = null;
		}, "Spams portals on the target, be careful your name is still shown", defaultState: false, bgImage);
		new QMSingleButton(qMNestedMenu3, 3.5f, 1f, "TP To", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			PlayerExtraMethods.teleportTo(iUser);
		}, "Teleport behind selected player facing them", halfBtn: false, icon, bgImage);
		new QMSingleButton(qMNestedMenu3, 1.5f, 2f, "Copy ID", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			Clipboard.SetText(iUser.field_Private_APIUser_0.id.ToString());
		}, "Copy the id of the avatar the selected user is wearing", halfBtn: false, icon2, bgImage);
		new QMSingleButton(qMNestedMenu3, 2.5f, 2f, "Copy Avatar ID", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			Clipboard.SetText(iUser.prop_ApiAvatar_0.id.ToString());
		}, "Copies to clipboard the selected user name", halfBtn: false, icon2, bgImage);
		new QMSingleButton(qMNestedMenu3, 3.5f, 2f, "Copy Display Name", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			Clipboard.SetText(iUser.prop_APIUser_0.displayName.ToString());
		}, "Teleport behind selected player facing them", halfBtn: false, icon2, bgImage);
		new QMSingleButton(qMNestedMenu2, 1.5f, 1.5f, "Bring Pickups", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			PickupWrapper.BringAllPickupsToPlayer(iUser);
		}, "Bring all pickups in world to your position", halfBtn: false, icon, bgImage);
		new QMToggleButton(qMNestedMenu2, 2.5f, 1.5f, "Pickup Swastika", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			SwasticaOrbit.Activated(iUser, state: true);
		}, delegate
		{
			Player iUser = ApiUtils.GetIUser();
			SwasticaOrbit.Activated(iUser, state: false);
		}, "Creates a Swastika with pickups and places it on top of the selected user.", defaultState: false, bgImage);
		new QMToggleButton(qMNestedMenu2, 3.5f, 1.5f, "Drone Swarm", delegate
		{
			Player iUser = ApiUtils.GetIUser();
			DroneSwarmWrapper.isSwarmActive = true;
			DroneSwarmWrapper.ChangeSwarmTarget(iUser.gameObject);
		}, delegate
		{
			DroneSwarmWrapper.isSwarmActive = false;
		}, "Swarms your player with every available drone in the instance", defaultState: false, bgImage);
		return result;
	}
}
