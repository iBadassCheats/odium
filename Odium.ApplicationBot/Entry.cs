using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Odium.ApplicationBot;

internal class Entry
{
	public static List<string> ActiveBotIds = new List<string>();

	public static string LaunchBotLogin(int profile)
	{
		try
		{
			string text = Guid.NewGuid().ToString();
			string fileName = Path.Combine(Directory.GetCurrentDirectory(), "launch.exe");
			string arguments = $"--profile={profile} --id={text} --appBot --fps=25 --no-vr";
			Process.Start(fileName, arguments);
			ActiveBotIds.Add(text);
			OdiumConsole.LogGradient("ApplicationBot", $"Launching login bot on profile {profile} with ID: {text}");
			return text;
		}
		catch (Exception ex)
		{
			OdiumConsole.LogGradient("ApplicationBot", "Failed to launch bot: " + ex.Message);
			return null;
		}
	}

	public static string LaunchBotHeadless(int profile)
	{
		try
		{
			string text = Guid.NewGuid().ToString();
			string fileName = Path.Combine(Directory.GetCurrentDirectory(), "launch.exe");
			string arguments = $"--profile={profile} --id={text} --appBot --fps=25 --no-vr -batchmode -noUpm -nographics -disable-gpu-skinning -no-stereo-rendering -nolog";
			Process.Start(fileName, arguments);
			ActiveBotIds.Add(text);
			OdiumConsole.LogGradient("ApplicationBot", $"Launching headless bot on profile {profile} with ID: {text}");
			return text;
		}
		catch (Exception ex)
		{
			OdiumConsole.LogGradient("ApplicationBot", "Failed to launch headless bot: " + ex.Message);
			return null;
		}
	}

	public static bool RemoveBotId(string botId)
	{
		return ActiveBotIds.Remove(botId);
	}

	public static List<string> GetActiveBotIds()
	{
		return new List<string>(ActiveBotIds);
	}

	public static bool IsBotActive(string botId)
	{
		return ActiveBotIds.Contains(botId);
	}
}
