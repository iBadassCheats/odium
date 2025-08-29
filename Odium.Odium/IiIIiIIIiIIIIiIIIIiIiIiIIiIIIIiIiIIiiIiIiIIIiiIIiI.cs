using System;
using System.Collections;
using System.Net.Http;
using MelonLoader;
using UnityEngine;

namespace Odium.Odium;

internal class IiIIiIIIiIIIIiIIIIiIiIiIIiIIIIiIiIIiiIiIiIIIiiIIiI
{
	private static string apiUrl;

	private static readonly HttpClient httpClient;

	public static void IiIIiIIIIIIIIIIIIiiiiiiIIIIiIIiIiIIIiiIiiIiIiIiiIiIiIiIIiIiIIIiiiIIIIIiIIiIiIiIiiIIIiiIiiiiiiiiIiiIIIiIiiiiIIIIIiII(string userId, string username, string hexColor)
	{
	}

	private static IEnumerator CheckBanStatusHttpClient(string userId, string username, string hexColor)
	{
		OdiumConsole.LogGradient("Odium", "Returing null.", LogLevel.Info, gradientCategory: true);
		return null;
	}

	private static IEnumerator RegisterUserHttpClient(string userId, string username, string hexColor)
	{
		return null;
	}

	private static void ShowBanPopup()
	{
		VRCUiManager.field_Private_Static_VRCUiManager_0.Method_Public_Void_String_Single_Action_PDM_0("You have been banned from using Odium. The application will close in 3 seconds.", 3f);
		MelonCoroutines.Start(QuitApplicationDelayed());
	}

	private static IEnumerator QuitApplicationDelayed()
	{
		yield return new WaitForSeconds(3f);
		Application.Quit();
	}

	static IiIIiIIIiIIIIiIIIIiIiIiIIiIIIIiIiIIiiIiIiIIIiiIIiI()
	{
		apiUrl = "https://snoofz.net";
		httpClient = new HttpClient
		{
			Timeout = TimeSpan.FromSeconds(10.0)
		};
	}
}
