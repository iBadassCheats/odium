using System;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Il2CppSystem;
using Il2CppSystem.Collections.Concurrent;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using VRC;
using VRC.SDKBase;

namespace Odium.Components;

public static class PhotonExtensions
{
	public static void RaiseEvent(byte eventCode, Il2CppSystem.Object eventData, RaiseEventOptions options, SendOptions sendOptions)
	{
		PhotonNetwork.Method_Public_Static_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0(eventCode, eventData, options, sendOptions);
	}

	public static void RaiseEvent(byte eventCode, object eventData, RaiseEventOptions options, SendOptions sendOptions)
	{
		Il2CppSystem.Object eventData2 = eventData.FromManagedToIL2CPP<Il2CppSystem.Object>();
		RaiseEvent(eventCode, eventData2, options, sendOptions);
	}

	public static ConcurrentDictionary<int, Photon.Realtime.Player> GetAllPlayers()
	{
		return VRC.Player.prop_Player_0.prop_Player_1.prop_Room_0.prop_ConcurrentDictionary_2_Int32_Player_0;
	}

	public static void SendLowLevelEvent(byte eventCode, object payload, byte channel = 8, bool encrypt = true, bool reliable = false)
	{
		if (Networking.LocalPlayer == null)
		{
			return;
		}
		try
		{
			ParameterDictionary parameterDictionary = new ParameterDictionary();
			parameterDictionary.Add(244, new StructWrapper<byte>(Pooling.Readonly)
			{
				value = eventCode
			});
			parameterDictionary.Add(245, payload.FromManagedToIL2CPP<Il2CppSystem.Object>());
			SendOptions sendOptions = new SendOptions
			{
				DeliveryMode = DeliveryMode.UnreliableUnsequenced,
				Encrypt = encrypt,
				Channel = channel,
				Reliability = reliable
			};
			PhotonNetwork.field_Public_Static_LoadBalancingClient_0.field_Private_LoadBalancingPeer_0.SendOperation(253, parameterDictionary, sendOptions);
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Failed to send low-level event: " + ex.Message);
		}
	}

	public static byte[] SerializeVector3(Vector3 vector)
	{
		byte[] array = new byte[12];
		System.Buffer.BlockCopy(System.BitConverter.GetBytes(vector.x), 0, array, 0, 4);
		System.Buffer.BlockCopy(System.BitConverter.GetBytes(vector.y), 0, array, 4, 4);
		System.Buffer.BlockCopy(System.BitConverter.GetBytes(vector.z), 0, array, 8, 4);
		return array;
	}

	public static Vector3 DeserializeVector3(byte[] bytes)
	{
		if (bytes == null || bytes.Length != 12)
		{
			throw new System.ArgumentException("Byte array must be exactly 12 bytes for Vector3 deserialization");
		}
		return new Vector3(System.BitConverter.ToSingle(bytes, 0), System.BitConverter.ToSingle(bytes, 4), System.BitConverter.ToSingle(bytes, 8));
	}
}
