using System;
using Odium.ButtonAPI.QM;
using Odium.Components;
using Odium.Odium;
using UnityEngine;

namespace Odium.QMPages;

internal class Protections
{
	public static void InitializePage(QMNestedMenu gameHacks, Sprite buttonImage)
	{
		Sprite sprite = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Udon.png");
		Sprite sprite2 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Speaker.png");
		Sprite sprite3 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Pun.png");
		Sprite sprite4 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\People.png");
		QMNestedMenu location = new QMNestedMenu(gameHacks, 1f, 0f, "<color=#8d142b>Udon</color>", "<color=#8d142b>Udon</color>", "Opens Select User menu", halfButton: false, sprite, buttonImage);
		QMNestedMenu location2 = new QMNestedMenu(gameHacks, 2f, 0f, "<color=#8d142b>USpeak</color>", "<color=#8d142b>USpeak</color>", "Opens Select User menu", halfButton: false, sprite2, buttonImage);
		QMNestedMenu location3 = new QMNestedMenu(gameHacks, 3f, 0f, "<color=#8d142b>Photon</color>", "<color=#8d142b>Photon</color>", "Opens Select User menu", halfButton: false, sprite3, buttonImage);
		QMNestedMenu location4 = new QMNestedMenu(gameHacks, 4f, 0f, "<color=#8d142b>Avatars</color>", "<color=#8d142b>Avatars</color>", "Opens Select User menu", halfButton: false, sprite4, buttonImage);
		new QMToggleButton(location, 1f, 0f, "Prevent Patreon Crash", delegate
		{
			AssignedVariables.preventPatreonCrash = true;
		}, delegate
		{
			AssignedVariables.preventPatreonCrash = false;
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location, 2f, 0f, "Prevent Event Flooding", delegate
		{
			AssignedVariables.preventM4EventFlooding = true;
		}, delegate
		{
			AssignedVariables.preventM4EventFlooding = false;
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location, 3f, 0f, "Prevent Event Spam", delegate
		{
			AssignedVariables.preventM4EventSpam = true;
		}, delegate
		{
			AssignedVariables.preventM4EventSpam = false;
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location, 4f, 0f, "Ratelimit Events", delegate
		{
			AssignedVariables.ratelimitM4Events = true;
		}, delegate
		{
			AssignedVariables.ratelimitM4Events = false;
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location2, 1f, 0f, "Gain Check", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location2, 2f, 0f, "Block Common Packets", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location2, 3f, 0f, "Spam Prevention", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location3, 1f, 0f, "Ratelimit Common Events", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location3, 2f, 0f, "Advanced Evt Check", delegate
		{
			AssignedVariables.chatBoxAntis = true;
		}, delegate
		{
			AssignedVariables.chatBoxAntis = false;
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location3, 3f, 0f, "Anti Pen-Trail Crash", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location3, 4f, 0f, "Check Interest", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location4, 1f, 0f, "Check Shaders", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location4, 2f, 0f, "Check Materials", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location4, 3f, 0f, "Check Pollygons", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location4, 4f, 0f, "Check Lights", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location4, 1f, 1f, "Check Physbones", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location4, 2f, 1f, "Prevent CABs", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
		new QMToggleButton(location4, 3f, 1f, "Block Known Crashers", delegate
		{
		}, delegate
		{
		}, "Prevent common crash exploits in Murder 4", defaultState: true, buttonImage);
	}
}
