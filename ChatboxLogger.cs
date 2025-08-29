using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Odium.UX;
using UnityEngine;

public static class ChatboxLogger
{
	private static readonly Dictionary<int, DateTime> lastLogTime = new Dictionary<int, DateTime>();

	private static readonly Dictionary<int, int> blockedCount = new Dictionary<int, int>();

	private static readonly Dictionary<int, int> sessionBlockedCount = new Dictionary<int, int>();

	private static readonly TimeSpan logCooldown = TimeSpan.FromSeconds(30.0);

	public static void LogBlockedMessage(int senderId, string playerName)
	{
		DateTime now = DateTime.Now;
		if (blockedCount.ContainsKey(senderId))
		{
			blockedCount[senderId]++;
		}
		else
		{
			blockedCount[senderId] = 1;
		}
		if (sessionBlockedCount.ContainsKey(senderId))
		{
			sessionBlockedCount[senderId]++;
		}
		else
		{
			sessionBlockedCount[senderId] = 1;
		}
		bool flag = false;
		if (!lastLogTime.ContainsKey(senderId))
		{
			flag = true;
		}
		else if (now - lastLogTime[senderId] >= logCooldown)
		{
			flag = true;
		}
		if (flag)
		{
			int num = blockedCount[senderId];
			int num2 = sessionBlockedCount[senderId];
			string txt = ((num == 1) ? $"<color=#31BCF0>[ChatBox]:</color> <color=red>Blocked message from {playerName} (ID: {senderId})</color>" : ((num2 != 1) ? $"<color=#31BCF0>[ChatBox]:</color> <color=red>Blocked {num2} messages from {playerName} (ID: {senderId}) - Total blocked: {num}</color>" : $"<color=#31BCF0>[ChatBox]:</color> <color=red>Blocked message from {playerName} (ID: {senderId}) - Total blocked: {num}</color>"));
			InternalConsole.LogIntoConsole(txt);
			lastLogTime[senderId] = now;
			sessionBlockedCount[senderId] = 0;
		}
		if (UnityEngine.Random.Range(0, 100) < 5)
		{
			CleanupOldEntries();
		}
	}

	public static string PrintByteArray(byte[] bytes)
	{
		StringBuilder stringBuilder = new StringBuilder("");
		foreach (byte b in bytes)
		{
			stringBuilder.Append(b.ToString("X2")).Append(" ");
		}
		return stringBuilder.ToString().TrimEnd();
	}

	public static string ConvertBytesToText(byte[] bytes)
	{
		try
		{
			string source = Encoding.UTF8.GetString(bytes).TrimEnd(default(char));
			source = new string(source.Where((char c) => !char.IsControl(c) || char.IsWhiteSpace(c)).ToArray());
			if (string.IsNullOrWhiteSpace(source))
			{
				return "[Empty or whitespace-only message]";
			}
			return source;
		}
		catch (Exception ex)
		{
			return "[Invalid Encoding: " + ex.Message + "]";
		}
	}

	public static void CleanupOldEntries()
	{
		DateTime now = DateTime.Now;
		List<int> list = (from kvp in lastLogTime
			where (now - kvp.Value).TotalMinutes > 5.0
			select kvp.Key).ToList();
		foreach (int item in list)
		{
			lastLogTime.Remove(item);
			blockedCount.Remove(item);
			sessionBlockedCount.Remove(item);
		}
	}

	public static void ResetCounts()
	{
		lastLogTime.Clear();
		blockedCount.Clear();
		sessionBlockedCount.Clear();
	}
}
