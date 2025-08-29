using System;
using System.Collections;
using MelonLoader;
using Photon.Realtime;
using UnityEngine;
using VRC.SDKBase;

namespace Odium.Components;

public class USpeakSpam
{
	public static bool isEnabled;

	public static void ToggleUSpeakSpam(bool state)
	{
		isEnabled = state;
		if (state)
		{
			MelonCoroutines.Start(USpeakSpamLoop());
		}
		else
		{
			MelonCoroutines.Stop(USpeakSpamLoop());
		}
	}

	public static IEnumerator USpeakSpamLoop()
	{
		while (isEnabled)
		{
			byte[] array = Convert.FromBase64String("AAAAAGfp+Lv2GRkA+MrI08yxTwBkxqwATk9LRU0wTk9LM00wTg==");
			byte[] trimmedArray = new byte[array.Length - 4];
			Buffer.BlockCopy(array, 4, trimmedArray, 0, array.Length - 4);
			byte[] bytes = BitConverter.GetBytes(Networking.GetServerTimeInMilliseconds());
			Buffer.BlockCopy(bytes, 0, trimmedArray, 0, 4);
			new RaiseEventOptions
			{
				field_Public_EventCaching_0 = EventCaching.DoNotCache,
				field_Public_ReceiverGroup_0 = ReceiverGroup.Others
			};
			PhotonExtensions.SendLowLevelEvent(1, trimmedArray, 8);
			yield return new WaitForSecondsRealtime(0.05f);
		}
	}
}
