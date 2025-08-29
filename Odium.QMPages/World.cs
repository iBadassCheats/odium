using System;
using Odium.ButtonAPI.QM;
using Odium.Components;
using Odium.Wrappers;
using UnityEngine;

namespace Odium.QMPages;

internal class World
{
	public static QMToggleButton hidePickupsToggle;

	public static void InitializePage(QMNestedMenu worldButton, Sprite buttonImage)
	{
		Sprite sprite = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\PickupsIcon.png");
		QMNestedMenu qMNestedMenu = new QMNestedMenu(worldButton, 1f, 3f, "<color=#8d142b>Pickups</color>", "<color=#8d142b>Pickups</color>", "Opens Select User menu", halfButton: false, null, buttonImage);
		new QMToggleButton(worldButton, 1f, 0f, "Drone Swarm", delegate
		{
			DroneSwarmWrapper.isSwarmActive = true;
			DroneSwarmWrapper.ChangeSwarmTarget(PlayerWrapper.LocalPlayer.gameObject);
		}, delegate
		{
			DroneSwarmWrapper.isSwarmActive = false;
		}, "Swarms your player with every available drone in the instance", defaultState: false, buttonImage);
		new QMSingleButton(worldButton, 2f, 0f, "Drop Drones", delegate
		{
			PickupWrapper.DropDronePickups();
		}, "Drop all drones in the instance", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 1f, 0f, "Drop Pickups", delegate
		{
			PickupWrapper.DropAllPickups();
		}, "Drop all pickups in the instance", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 2f, 0f, "Bring Pickups", delegate
		{
			PickupWrapper.BringAllPickupsToPlayer(PlayerWrapper.LocalPlayer);
		}, "Brings all pickups in the instance", halfBtn: false, null, buttonImage);
		new QMSingleButton(qMNestedMenu, 3f, 0f, "Respawn Pickups", delegate
		{
			PickupWrapper.RespawnAllPickups();
		}, "Brings all pickups in the instance", halfBtn: false, null, buttonImage);
		hidePickupsToggle = new QMToggleButton(qMNestedMenu, 1f, 3f, "Hide Pickups", delegate
		{
			PickupWrapper.HideAllPickups();
			hidePickupsToggle.SetButtonText("Show Pickups");
		}, delegate
		{
			PickupWrapper.ShowAllPickups();
			hidePickupsToggle.SetButtonText("Hide Pickups");
		}, "Toggle visibility of all pickups in the instance", defaultState: false, buttonImage);
	}
}
