using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Odium;

public static class ChatboxAntis
{
	private static readonly HashSet<string> blockedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "spam", "crash", "lag", "nigger", "faggot", "gang" };

	private static readonly Dictionary<int, DateTime> lastMessageTime = new Dictionary<int, DateTime>();

	private static readonly Dictionary<int, int> messageCount = new Dictionary<int, int>();

	public static bool IsMessageValid(string message, int senderId = -1)
	{
		if (string.IsNullOrWhiteSpace(message))
		{
			return false;
		}
		if (message.Length > 200)
		{
			return false;
		}
		if (message.Length < 2)
		{
			return false;
		}
		if (UnicodeValidator.Sanitize(message))
		{
			return false;
		}
		if (ContainsBlockedWords(message))
		{
			return false;
		}
		if (senderId != -1 && !PassesRateLimit(senderId))
		{
			return false;
		}
		if (IsRepeatedCharacterSpam(message))
		{
			return false;
		}
		return true;
	}

	private static bool ContainsBlockedWords(string message)
	{
		foreach (string blockedWord in blockedWords)
		{
			if (Regex.IsMatch(message, "\\b" + Regex.Escape(blockedWord) + "\\b", RegexOptions.IgnoreCase))
			{
				OdiumConsole.Log("ChatBox", "Found blocked word: '" + blockedWord + "' in message: '" + message + "'", LogLevel.Debug);
				return true;
			}
		}
		return false;
	}

	private static bool PassesRateLimit(int senderId)
	{
		DateTime now = DateTime.Now;
		List<int> list = (from kvp in lastMessageTime
			where (now - kvp.Value).TotalMinutes > 1.0
			select kvp.Key).ToList();
		foreach (int item in list)
		{
			lastMessageTime.Remove(item);
			messageCount.Remove(item);
		}
		if (lastMessageTime.ContainsKey(senderId))
		{
			if ((now - lastMessageTime[senderId]).TotalSeconds < 2.0)
			{
				OdiumConsole.Log("ChatBox", $"Rate limit: Too fast (User {senderId})", LogLevel.Debug);
				return false;
			}
			if (messageCount.ContainsKey(senderId) && messageCount[senderId] >= 10)
			{
				OdiumConsole.Log("ChatBox", $"Rate limit: Too many messages (User {senderId})", LogLevel.Debug);
				return false;
			}
		}
		lastMessageTime[senderId] = now;
		messageCount[senderId] = ((!messageCount.ContainsKey(senderId)) ? 1 : (messageCount[senderId] + 1));
		return true;
	}

	private static bool IsRepeatedCharacterSpam(string message)
	{
		for (int i = 0; i < message.Length - 4; i++)
		{
			char c = message[i];
			int num = 1;
			for (int j = i + 1; j < message.Length && j < i + 10 && message[j] == c; j++)
			{
				num++;
			}
			if (num > 5)
			{
				return true;
			}
		}
		return false;
	}
}
