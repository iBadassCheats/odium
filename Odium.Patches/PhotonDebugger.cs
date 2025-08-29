using System;
using ExitGames.Client.Photon;

namespace Odium.Patches;

public class PhotonDebugger
{
	public static bool IsOnEventSendDebug;

	public static bool OnEventSent(byte code, object data, RaiseEventOptions options, SendOptions sendOptions)
	{
		Console.WriteLine("Photon:OnEventSent----------------------");
		Console.WriteLine("Photon:OnEventSent" + $"Code:{code}");
		Console.WriteLine("Photon:OnEventSent" + $"Data:{data}");
		Console.WriteLine("Photon:OnEventSent" + $"Data:{data}");
		Console.WriteLine("Photon:OnEventSent" + $"RaiseEventOptions:{options}");
		Console.WriteLine("Photon:OnEventSent" + $"SendOptions:{sendOptions}");
		Console.WriteLine("Photon:OnEventSent----------------------");
		return true;
	}
}
