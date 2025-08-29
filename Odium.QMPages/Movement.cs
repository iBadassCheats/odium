using System;
using Odium.ButtonAPI.QM;
using Odium.Components;
using Odium.Modules;
using UnityEngine;

namespace Odium.QMPages;

internal class Movement
{
	public static void InitializePage(QMNestedMenu movementButton, Sprite buttonImage)
	{
		Sprite icon = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\PlusIcon.png");
		Sprite icon2 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\MinusIcon.png");
		QMToggleButton qMToggleButton = new QMToggleButton(movementButton, 2.5f, 1f, "Flight", delegate
		{
			FlyComponent.FlyEnabled = true;
		}, delegate
		{
			FlyComponent.FlyEnabled = false;
		}, "Toggle Flight Mode", defaultState: false, buttonImage);
		QMToggleButton qMToggleButton2 = new QMToggleButton(movementButton, 2f, 0f, "Jetpack", delegate
		{
			Jetpack.Activate(state: true);
		}, delegate
		{
			Jetpack.Activate(state: false);
		}, "Allows you to fly", defaultState: false, buttonImage);
		QMToggleButton qMToggleButton3 = new QMToggleButton(movementButton, 3f, 0f, "SpinBot", delegate
		{
			SpinBotModule.SetActive(state: true);
		}, delegate
		{
			SpinBotModule.SetActive(state: false);
		}, "HvH mode", defaultState: false, buttonImage);
		QMSingleButton qMSingleButton = new QMSingleButton(movementButton, 2f, 3f, "Fly Speed", delegate
		{
			FlyComponent.FlySpeed += 0.1f;
		}, "Increase Fly Speed", halfBtn: false, icon, buttonImage);
		QMSingleButton qMSingleButton2 = new QMSingleButton(movementButton, 3f, 3f, "Fly Speed", delegate
		{
			FlyComponent.FlySpeed -= 0.1f;
		}, "Decrease Fly Speed", halfBtn: false, icon2, buttonImage);
	}
}
