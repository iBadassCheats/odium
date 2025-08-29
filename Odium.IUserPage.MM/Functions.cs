using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Odium.ButtonAPI.MM;
using Odium.ButtonAPI.QM;
using Odium.Components;
using Odium.Modules;
using Odium.Threadding;
using UnityEngine;
using VRC.SDKBase;

namespace Odium.IUserPage.MM;

internal class Functions
{
	public static void Initialize()
	{
		Sprite icon = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\InfoIcon.png");
		Sprite icon2 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\JoinMeIcon.png");
		Sprite icon3 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\PlusIcon.png");
		Sprite icon4 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\MinusIcon.png");
		Sprite icon5 = SpriteUtil.LoadFromDisk(Environment.CurrentDirectory + "\\Odium\\DownloadIcon.png");
		MMUserActionRow actionRow = new MMUserActionRow("Odium Actions");
		MMUserActionRow actionRow2 = new MMUserActionRow("Odium Tags");
		new MMUserButton(actionRow, "Copy ID", async delegate
		{
			OdiumConsole.Log("Odium", "Attempting to copy user ID...");
			try
			{
				string displayName = ApiUtils.GetMMIUser();
				if (string.IsNullOrEmpty(displayName))
				{
					OdiumConsole.Log("Odium", "No display name found");
				}
				else
				{
					OdiumConsole.Log("Odium", "Fetching user ID for: " + displayName);
					MainThreadDispatcher.Enqueue(delegate
					{
						try
						{
							HttpClient httpClient = new HttpClient();
							string requestUri = "http://api.snoofz.net:3778/api/odium/vrc/getByDisplayName?displayName=" + Uri.EscapeDataString(displayName);
							HttpResponseMessage result = httpClient.GetAsync(requestUri).Result;
							if (result.IsSuccessStatusCode)
							{
								string result2 = result.Content.ReadAsStringAsync().Result;
								JObject jObject = JObject.Parse(result2);
								string text = jObject["id"]?.ToString();
								if (!string.IsNullOrEmpty(text))
								{
									Clipboard.SetText(text);
									OdiumConsole.Log("Odium", "Copied user ID to clipboard: " + text);
									OdiumBottomNotification.ShowNotification("Copied User ID");
								}
								else
								{
									OdiumConsole.Log("Odium", "No user ID found in API response");
								}
							}
							else
							{
								OdiumConsole.Log("Odium", $"API request failed with status: {result.StatusCode}");
							}
						}
						catch (Exception ex3)
						{
							OdiumConsole.Log("Odium", "Error in main thread execution: " + ex3.Message);
						}
					});
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				OdiumConsole.Log("Odium", "Error fetching user ID: " + ex2.Message);
			}
		}, icon);
		new MMUserButton(actionRow, "Copy Last Platform", async delegate
		{
			OdiumConsole.Log("Odium", "Attempting to copy Last Platform...");
			try
			{
				string displayName = ApiUtils.GetMMIUser();
				if (string.IsNullOrEmpty(displayName))
				{
					OdiumConsole.Log("Odium", "No display name found");
				}
				else
				{
					OdiumConsole.Log("Odium", "Fetching user Last Platform for: " + displayName);
					MainThreadDispatcher.Enqueue(delegate
					{
						try
						{
							HttpClient httpClient = new HttpClient();
							string requestUri = "http://api.snoofz.net:3778/api/odium/vrc/getByDisplayName?displayName=" + Uri.EscapeDataString(displayName);
							HttpResponseMessage result = httpClient.GetAsync(requestUri).Result;
							if (result.IsSuccessStatusCode)
							{
								string result2 = result.Content.ReadAsStringAsync().Result;
								JObject jObject = JObject.Parse(result2);
								string text = jObject["last_platform"]?.ToString();
								if (!string.IsNullOrEmpty(text))
								{
									Clipboard.SetText(text);
									OdiumBottomNotification.ShowNotification("User was last on -> " + text);
								}
								else
								{
									OdiumConsole.Log("Odium", "No user ID found in API response");
								}
							}
							else
							{
								OdiumConsole.Log("Odium", $"API request failed with status: {result.StatusCode}");
							}
						}
						catch (Exception ex3)
						{
							OdiumConsole.Log("Odium", "Error in main thread execution: " + ex3.Message);
						}
					});
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				OdiumConsole.Log("Odium", "Error fetching user ID: " + ex2.Message);
			}
		}, icon);
		new MMUserButton(actionRow, "Join User", async delegate
		{
			try
			{
				string displayName = ApiUtils.GetMMIUser();
				if (string.IsNullOrEmpty(displayName))
				{
					OdiumConsole.Log("Odium", "No display name found");
				}
				else
				{
					OdiumConsole.Log("Odium", "Fetching location for -> " + displayName);
					MainThreadDispatcher.Enqueue(delegate
					{
						try
						{
							HttpClient httpClient = new HttpClient();
							string requestUri = "http://api.snoofz.net:3778/api/odium/vrc/getUserLocation?displayName=" + Uri.EscapeDataString(displayName);
							HttpResponseMessage result = httpClient.GetAsync(requestUri).Result;
							if (result.IsSuccessStatusCode)
							{
								string result2 = result.Content.ReadAsStringAsync().Result;
								JObject jObject = JObject.Parse(result2);
								OdiumConsole.Log("Odium", "Joining user at location: " + result2);
								string text = jObject["location"]?.ToString();
								if (!string.IsNullOrEmpty(text) && text != "none")
								{
									OdiumConsole.Log("Odium", "Joining user at location: " + text);
									Networking.GoToRoom(text);
									OdiumBottomNotification.ShowNotification("Joining " + displayName);
								}
								else
								{
									OdiumConsole.Log("Odium", "User location is 'none' or empty - cannot join");
									OdiumBottomNotification.ShowNotification("User location unavailable");
								}
							}
							else if (result.StatusCode == HttpStatusCode.NotFound)
							{
								OdiumConsole.Log("Odium", "User location not tracked");
								OdiumBottomNotification.ShowNotification("User location not tracked");
							}
							else
							{
								OdiumConsole.Log("Odium", $"API request failed with status: {result.StatusCode}");
							}
						}
						catch (Exception ex3)
						{
							OdiumConsole.Log("Odium", "Error in main thread execution: " + ex3.Message);
						}
					});
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				OdiumConsole.Log("Odium", "Error fetching user location: " + ex2.Message);
			}
		}, icon2);
		new MMUserButton(actionRow2, "Add Tag", async delegate
		{
			OdiumInputDialog.ShowInputDialog("Enter tag", delegate(string input, bool wasSubmitted)
			{
				if (wasSubmitted)
				{
					try
					{
						string displayName = ApiUtils.GetMMIUser();
						if (string.IsNullOrEmpty(displayName))
						{
							OdiumConsole.Log("Odium", "No display name found");
						}
						else
						{
							MainThreadDispatcher.Enqueue(delegate
							{
								try
								{
									HttpClient httpClient = new HttpClient();
									string requestUri = "http://api.snoofz.net:3778/api/odium/vrc/getByDisplayName?displayName=" + Uri.EscapeDataString(displayName);
									HttpResponseMessage result = httpClient.GetAsync(requestUri).Result;
									if (result.IsSuccessStatusCode)
									{
										string result2 = result.Content.ReadAsStringAsync().Result;
										JObject jObject = JObject.Parse(result2);
										string text = jObject["id"]?.ToString();
										if (!string.IsNullOrEmpty(text))
										{
											var value = new
											{
												userId = text,
												tag = input
											};
											string content = JsonConvert.SerializeObject(value);
											StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
											string requestUri2 = "https://odiumvrc.com/api/odium/tags/add";
											HttpResponseMessage result3 = httpClient.PostAsync(requestUri2, content2).Result;
											if (result3.IsSuccessStatusCode)
											{
												string result4 = result3.Content.ReadAsStringAsync().Result;
												JObject jObject2 = JObject.Parse(result4);
												OdiumInputDialog.CloseAllInputDialogs();
												OdiumBottomNotification.ShowNotification("Tag '" + input + "' added to " + displayName);
												OdiumConsole.Log("Odium", "Tag '" + input + "' successfully added to user " + text);
											}
											else
											{
												string result5 = result3.Content.ReadAsStringAsync().Result;
												JObject jObject3 = JObject.Parse(result5);
												string text2 = jObject3["message"]?.ToString() ?? "Failed to add tag";
												OdiumInputDialog.CloseAllInputDialogs();
												OdiumBottomNotification.ShowNotification("Error: " + text2);
												OdiumConsole.Log("Odium", "Failed to add tag: " + text2);
											}
										}
										else
										{
											OdiumInputDialog.CloseAllInputDialogs();
											OdiumBottomNotification.ShowNotification("Error: Could not find user ID");
											OdiumConsole.Log("Odium", "User ID not found in response");
										}
									}
									else
									{
										OdiumInputDialog.CloseAllInputDialogs();
										OdiumBottomNotification.ShowNotification("Error: Could not find user");
										OdiumConsole.Log("Odium", $"Failed to get user data: {result.StatusCode}");
									}
								}
								catch (Exception ex2)
								{
									OdiumInputDialog.CloseAllInputDialogs();
									OdiumBottomNotification.ShowNotification("Error: " + ex2.Message);
									OdiumConsole.Log("Odium", "Exception in tag operation: " + ex2.Message);
								}
							});
						}
						return;
					}
					catch (Exception ex)
					{
						OdiumConsole.Log("Odium", "Error fetching user ID: " + ex.Message);
						OdiumInputDialog.CloseAllInputDialogs();
						OdiumBottomNotification.ShowNotification("Error: " + ex.Message);
						return;
					}
				}
				OdiumConsole.Log("Input", "User cancelled input");
				OdiumInputDialog.CloseAllInputDialogs();
			}, "Enter tag", "Enter tag");
		}, icon3);
		new MMUserButton(actionRow2, "Remove Tag", async delegate
		{
			OdiumInputDialog.ShowInputDialog("Enter tag", delegate(string input, bool wasSubmitted)
			{
				if (wasSubmitted)
				{
					try
					{
						string displayName = ApiUtils.GetMMIUser();
						if (string.IsNullOrEmpty(displayName))
						{
							OdiumConsole.Log("Odium", "No display name found");
						}
						else
						{
							MainThreadDispatcher.Enqueue(delegate
							{
								try
								{
									HttpClient httpClient = new HttpClient();
									string requestUri = "http://api.snoofz.net:3778/api/odium/vrc/getByDisplayName?displayName=" + Uri.EscapeDataString(displayName);
									HttpResponseMessage result = httpClient.GetAsync(requestUri).Result;
									if (result.IsSuccessStatusCode)
									{
										string result2 = result.Content.ReadAsStringAsync().Result;
										JObject jObject = JObject.Parse(result2);
										string text = jObject["id"]?.ToString();
										if (!string.IsNullOrEmpty(text))
										{
											var value = new
											{
												userId = text,
												tag = input
											};
											string content = JsonConvert.SerializeObject(value);
											StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
											string requestUri2 = "https://odiumvrc.com/odium/tags/remove";
											HttpResponseMessage result3 = httpClient.PostAsync(requestUri2, content2).Result;
											if (result3.IsSuccessStatusCode)
											{
												string result4 = result3.Content.ReadAsStringAsync().Result;
												JObject jObject2 = JObject.Parse(result4);
												OdiumInputDialog.CloseAllInputDialogs();
												OdiumBottomNotification.ShowNotification("Tag '" + input + "' removed from " + displayName);
												OdiumConsole.Log("Odium", "Tag '" + input + "' successfully removed from user " + text);
											}
											else
											{
												string result5 = result3.Content.ReadAsStringAsync().Result;
												JObject jObject3 = JObject.Parse(result5);
												string text2 = jObject3["message"]?.ToString() ?? "Failed to removed tag";
												OdiumInputDialog.CloseAllInputDialogs();
												OdiumBottomNotification.ShowNotification("Error: " + text2);
												OdiumConsole.Log("Odium", "Failed to removed tag: " + text2);
											}
										}
										else
										{
											OdiumInputDialog.CloseAllInputDialogs();
											OdiumBottomNotification.ShowNotification("Error: Could not find user ID");
											OdiumConsole.Log("Odium", "User ID not found in response");
										}
									}
									else
									{
										OdiumInputDialog.CloseAllInputDialogs();
										OdiumBottomNotification.ShowNotification("Error: Could not find user");
										OdiumConsole.Log("Odium", $"Failed to get user data: {result.StatusCode}");
									}
								}
								catch (Exception ex2)
								{
									OdiumInputDialog.CloseAllInputDialogs();
									OdiumBottomNotification.ShowNotification("Error: " + ex2.Message);
									OdiumConsole.Log("Odium", "Exception in tag operation: " + ex2.Message);
								}
							});
						}
						return;
					}
					catch (Exception ex)
					{
						OdiumConsole.Log("Odium", "Error fetching user ID: " + ex.Message);
						OdiumInputDialog.CloseAllInputDialogs();
						OdiumBottomNotification.ShowNotification("Error: " + ex.Message);
						return;
					}
				}
				OdiumConsole.Log("Input", "User cancelled input");
				OdiumInputDialog.CloseAllInputDialogs();
			}, "Enter tag", "Enter tag");
		}, icon4);
		new MMUserButton(actionRow, "Download VRCA", async delegate
		{
		}, icon5);
	}
}
