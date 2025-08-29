using System.Collections.Generic;
using ExitGames.Client.Photon;
using Il2CppSystem;

namespace Odium.Components;

internal class Ratelimit
{
	private static List<byte> rateLimitedEvents = new List<byte>
	{
		1, 33, 40, 42, 43, 44, 50, 52, 53, 60,
		4, 5, 6, 8, 16, 17, 18, 7, 19, 12,
		11, 13, 14, 15, 202, 209, 210, 21, 22, 62,
		63, 64, 66, 67, 68, 69, 70, 71, 72, 73,
		74, 75, 76, 77, 78, 79, 80, 28, 29, 30
	};

	public static void ProcessRateLimit(ref EventData eventData)
	{
		if (eventData.Code != 34)
		{
			return;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		Dictionary<byte, int> dictionary2 = new Dictionary<byte, int>();
		foreach (byte rateLimitedEvent in rateLimitedEvents)
		{
			dictionary2.Add(rateLimitedEvent, int.MaxValue);
		}
		dictionary.Add(0, dictionary2);
		dictionary.Add(2, true);
		eventData.customData = dictionary.FromManagedToIL2CPP<Object>();
	}
}
