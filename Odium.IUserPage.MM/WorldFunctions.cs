using System;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Odium.ButtonAPI.MM;
using Odium.ButtonAPI.QM;
using Odium.Components;
using Odium.Modules;
using Odium.Threadding;
using UnityEngine;

namespace Odium.IUserPage.MM;

internal class WorldFunctions
{
	public static void Initialize()
	{
		Sprite icon = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\InfoIcon.png");
		Sprite icon2 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\DownloadIcon.png");
		MMWorldActionRow actionRow = new MMWorldActionRow("Odium Actions");
		new MMWorldButton(actionRow, "Copy World ID", delegate
		{
			OdiumConsole.Log("Odium", "Attempting to copy world ID...");
			try
			{
				string worldName = ApiUtils.GetMMWorldName();
				if (string.IsNullOrEmpty(worldName))
				{
					OdiumConsole.Log("Odium", "No world name found");
				}
				else
				{
					OdiumConsole.Log("Odium", "Fetching world ID for: " + worldName);
					MainThreadDispatcher.Enqueue(delegate
					{
						try
						{
							HttpClient httpClient = new HttpClient();
							string requestUri = "http://api.snoofz.net:3778/api/odium/vrc/getWorldByName?worldName=" + Uri.EscapeDataString(worldName);
							HttpResponseMessage result = httpClient.GetAsync(requestUri).Result;
							if (result.IsSuccessStatusCode)
							{
								string result2 = result.Content.ReadAsStringAsync().Result;
								JObject jObject = JObject.Parse(result2);
								string text = jObject["id"]?.ToString();
								if (!string.IsNullOrEmpty(text))
								{
									Clipboard.SetText(text);
									OdiumConsole.Log("Odium", "Copied world ID to clipboard: " + text);
									OdiumBottomNotification.ShowNotification("Copied World ID");
								}
								else
								{
									OdiumConsole.Log("Odium", "No world ID found in API response");
								}
							}
							else
							{
								OdiumConsole.Log("Odium", $"API request failed with status: {result.StatusCode}");
							}
						}
						catch (Exception ex2)
						{
							OdiumConsole.Log("Odium", "Error in main thread execution: " + ex2.Message);
						}
					});
				}
			}
			catch (Exception ex)
			{
				OdiumConsole.Log("Odium", "Error fetching world ID: " + ex.Message);
			}
		}, icon);
		new MMWorldButton(actionRow, "Download VRCW", delegate
		{
		}, icon2);
	}
}
