using System;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using Odium.ButtonAPI.MM;
using Odium.ButtonAPI.QM;
using Odium.IUserPage.MM;
using Odium.Odium;
using Odium.QMPages;
using UnityEngine;
using VRC.UI.Elements;

namespace Odium.Components;

public class QM
{
	public static List<string> ObjectsToFind;

	public static List<string> Menus;

	public static List<string> Buttons;

	public static int currentObjectIndex;

	public static void SetupMenu()
	{
		MelonCoroutines.Start(WaitForQM());
	}

	private static IEnumerator WaitForQM()
	{
		while (UnityEngine.Object.FindObjectOfType<QuickMenu>() == null)
		{
			yield return null;
		}
		CreateMenu();
		OdiumConsole.LogGradient("Odium", ObjectsToFind[currentObjectIndex] + " found!");
		currentObjectIndex++;
		OdiumConsole.Log("Odium", $"Waiting for {ObjectsToFind[currentObjectIndex]} [{currentObjectIndex}]...");
		AssignedVariables.quickMenu = AssignedVariables.userInterface.transform.Find(ObjectsToFind[currentObjectIndex])?.gameObject;
		if (AssignedVariables.quickMenu != null)
		{
			OdiumConsole.LogGradient("Odium", ObjectsToFind[currentObjectIndex] + " found!");
			OdiumConsole.Log("Odium", "Setting up " + ObjectsToFind[currentObjectIndex] + "...");
			OdiumConsole.Log("Odium", "Applying theme to " + Menus[0] + "...");
			SpriteUtil.ApplySpriteToMenu(Menus[1], "QMBackground.png");
			OdiumConsole.Log("Odium", "Applying theme to " + Buttons[0] + "...");
			for (int i = 1; i < Buttons.Count; i++)
			{
				SpriteUtil.ApplySpriteToButton(Buttons[i], "QMHalfButton.png");
			}
		}
	}

	public static void CreateMenu()
	{
		try
		{
			Sprite sprite = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\ButtonBackground.png");
			Sprite halfButtonImage = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\QMHalfButton.png");
			SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Deafen.png");
			SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Headphones.png");
			SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Mute.png");
			SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Microphone.png");
			Sprite sprite2 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Skip.png");
			Sprite onSprite = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Pause.png");
			Sprite sprite3 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Rewind.png");
			Sprite offSprite = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\Play.png");
			Sprite sprite4 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\OdiumIcon.png");
			List<QMNestedMenu> list = Entry.Initialize(sprite, halfButtonImage);
			World.InitializePage(list[0], sprite);
			Movement.InitializePage(list[1], sprite);
			Exploits.InitializePage(list[2], sprite);
			Settings.InitializePage(list[3], sprite);
			AppBot.InitializePage(list[4], sprite, halfButtonImage);
			Visuals.InitializePage(list[5], sprite);
			GameHacks.InitializePage(list[6], sprite);
			Protections.InitializePage(list[7], sprite);
			Functions.Initialize();
			WorldFunctions.Initialize();
			QMMainIconButton.CreateButton(sprite3, delegate
			{
				MediaControls.SpotifyRewind();
			});
			QMMainIconButton.CreateToggle(onSprite, offSprite, delegate
			{
				MediaControls.SpotifyPause();
			}, delegate
			{
				MediaControls.SpotifyPause();
			});
			QMMainIconButton.CreateButton(sprite2, delegate
			{
				MediaControls.SpotifySkip();
			});
			QMMainIconButton.CreateImage(sprite4, new Vector3(-150f, -50f), new Vector3(2.5f, 2.5f));
			AssignedVariables.userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/Header_H1").transform.localPosition = new Vector3(125.6729f, 1024f, 0f);
			Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/Header_H1/RightItemContainer");
			transform.Find("Button_QM_Report").gameObject.SetActive(value: false);
			AssignedVariables.userInterface.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/Header_H1/RightItemContainer");
			transform.Find("Button_QM_Expand").gameObject.SetActive(value: false);
			PlayerDebugUI.InitializeDebugMenu();
			SidebarListItemCloner.CreateSidebarItem("Odium Users");
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("Odium", "Error creating menu: " + ex.Message, LogLevel.Error);
		}
	}

	public static IEnumerator WaitForUI()
	{
		OdiumConsole.Log("Odium", $"Waiting for {ObjectsToFind[currentObjectIndex]} [{currentObjectIndex}]...");
		while ((AssignedVariables.userInterface = GameObject.Find("UserInterface")) == null)
		{
			yield return null;
		}
		OdiumConsole.LogGradient("Odium", ObjectsToFind[currentObjectIndex] + " found!");
		currentObjectIndex++;
		OdiumConsole.Log("Odium", $"Waiting for {ObjectsToFind[currentObjectIndex]} [{currentObjectIndex}]...");
		while (true)
		{
			AssignedVariables.quickMenu = AssignedVariables.userInterface.transform.Find(ObjectsToFind[currentObjectIndex])?.gameObject;
			if (AssignedVariables.quickMenu != null)
			{
				break;
			}
			yield return null;
		}
		OdiumConsole.LogGradient("Odium", ObjectsToFind[currentObjectIndex] + " found!");
		OdiumConsole.Log("Odium", "Setting up " + ObjectsToFind[currentObjectIndex] + "...");
		OdiumConsole.Log("Odium", "Applying theme to " + Menus[0] + "...");
		SpriteUtil.ApplySpriteToMenu(Menus[1], "QMBackground.png");
		OdiumConsole.Log("Odium", "Applying theme to " + Buttons[0] + "...");
		for (int i = 1; i < Buttons.Count; i++)
		{
			SpriteUtil.ApplySpriteToButton(Buttons[i], "QMHalfButton.png");
		}
	}

	static QM()
	{
		ObjectsToFind = new List<string> { "UserInterface", "Canvas_QuickMenu(Clone)" };
		Menus = new List<string> { "Menus", "Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/BackgroundLayer01" };
		Buttons = new List<string> { "Buttons", "Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks/Button_Worlds", "Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks/Button_Avatars", "Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks/Button_Social", "Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks/Button_ViewGroups", "Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/Button_Respawn", "Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/Button_GoHome", "Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/Button_SelectUser", "Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/Button_Safety" };
		currentObjectIndex = 0;
	}
}
