using System;
using System.Collections.Generic;
using Odium.ButtonAPI.QM;
using Odium.Components;
using TMPro;
using UnityEngine;

namespace Odium.QMPages;

public static class Entry
{
	public static QMTabMenu tabMenu;

	public static List<QMNestedMenu> Initialize(Sprite buttonImage, Sprite halfButtonImage)
	{
		tabMenu = new QMTabMenu("<color=#8d142b>Odium</color>", "Welcome to <color=#8d142b>Odium</color>", SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\OdiumIcon.png"));
		tabMenu.MenuTitleText.alignment = TextAlignmentOptions.Center;
		QMNestedMenu item = new QMNestedMenu(tabMenu, 1f, 0f, "<color=#8d142b>World</color>", "<color=#8d142b>World</color>", "World Utility Functions", halfButton: false, SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\WorldIcon.png"), buttonImage);
		QMNestedMenu item2 = new QMNestedMenu(tabMenu, 2f, 0f, "<color=#8d142b>Movement</color>", "<color=#8d142b>Movement</color>", "Movement Utility Functions", halfButton: false, SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\MovementIcon.png"), buttonImage);
		QMNestedMenu item3 = new QMNestedMenu(tabMenu, 3f, 0f, "<color=#8d142b>Exploits</color>", "<color=#8d142b>Exploits</color>", "World Utility Functions", halfButton: false, SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\ExploitIcon.png"), buttonImage);
		QMNestedMenu item4 = new QMNestedMenu(tabMenu, 4f, 3.5f, "<color=#8d142b>Settings</color>", "<color=#8d142b>Settings</color>", "World Utility Functions", halfButton: true, null, halfButtonImage);
		QMNestedMenu item5 = new QMNestedMenu(tabMenu, 1f, 3.5f, "<color=#8d142b>App Bots</color>", "<color=#8d142b>App Bots</color>", "App Bots Utility Functions", halfButton: true, null, halfButtonImage);
		QMNestedMenu item6 = new QMNestedMenu(tabMenu, 4f, 0f, "<color=#8d142b>Visuals</color>", "<color=#8d142b>Visuals</color>", "Visuals Utility Functions", halfButton: false, SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\VisualIcon.png"), buttonImage);
		QMNestedMenu item7 = new QMNestedMenu(tabMenu, 2f, 1f, "<color=#8d142b>Game Hacks</color>", "<color=#8d142b>Game Hacks</color>", "World Utility Functions", halfButton: false, SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\WorldCheats.png"), buttonImage);
		QMNestedMenu item8 = new QMNestedMenu(tabMenu, 3f, 1f, "<color=#8d142b>Protections</color>", "<color=#8d142b>Protections</color>", "World Utility Functions", halfButton: false, SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\ShieldIcon.png"), buttonImage);
		return new List<QMNestedMenu> { item, item2, item3, item4, item5, item6, item7, item8 };
	}
}
