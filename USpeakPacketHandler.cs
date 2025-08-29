using System;
using ExitGames.Client.Photon;
using Odium.Components;
using Photon.Realtime;
using UnityEngine;

public class USpeakPacketHandler
{
	public struct USpeakVoicePacket
	{
		public uint timestamp;

		public byte gain;

		public byte flags;

		public byte reserved1;

		public byte reserved2;

		public byte[] audioData;

		public USpeakVoicePacket(uint timestamp, byte gain, byte flags, byte[] audioData)
		{
			this.timestamp = timestamp;
			this.gain = gain;
			this.flags = flags;
			reserved1 = 45;
			reserved2 = 0;
			this.audioData = audioData ?? new byte[0];
		}
	}

	public float gain = 1f;

	public float volume = 1f;

	public bool muted = false;

	public bool whispering = false;

	public static byte[] CreateUSpeakPacket(float gainValue, bool isMuted, bool isWhispering, byte[] audioData)
	{
		uint timestamp = (uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFFu);
		byte b = (byte)Mathf.Clamp(gainValue * 127.5f, 0f, 255f);
		byte b2 = 55;
		if (isMuted)
		{
			b2 |= 0x80;
		}
		if (isWhispering)
		{
			b2 |= 0x40;
		}
		USpeakVoicePacket packet = new USpeakVoicePacket(timestamp, b, b2, audioData);
		return SerializePacket(packet);
	}

	public static byte[] SerializePacket(USpeakVoicePacket packet)
	{
		byte[] array = new byte[8 + packet.audioData.Length];
		array[0] = (byte)(packet.timestamp & 0xFF);
		array[1] = (byte)((packet.timestamp >> 8) & 0xFF);
		array[2] = (byte)((packet.timestamp >> 16) & 0xFF);
		array[3] = (byte)((packet.timestamp >> 24) & 0xFF);
		array[4] = packet.gain;
		array[5] = packet.flags;
		array[6] = packet.reserved1;
		array[7] = packet.reserved2;
		if (packet.audioData.Length != 0)
		{
			Array.Copy(packet.audioData, 0, array, 8, packet.audioData.Length);
		}
		return array;
	}

	public static USpeakVoicePacket ParseUSpeakPacket(byte[] packetData)
	{
		if (packetData.Length < 8)
		{
			throw new ArgumentException("USpeak packet too short");
		}
		uint timestamp = (uint)(packetData[0] | (packetData[1] << 8) | (packetData[2] << 16) | (packetData[3] << 24));
		byte b = packetData[4];
		byte flags = packetData[5];
		byte[] array = new byte[packetData.Length - 8];
		if (array.Length != 0)
		{
			Array.Copy(packetData, 8, array, 0, array.Length);
		}
		return new USpeakVoicePacket(timestamp, b, flags, array);
	}

	public void SendCustomVoicePacket(byte[] audioData)
	{
		byte[] array = CreateUSpeakPacket(gain, muted, whispering, audioData);
		string text = Convert.ToBase64String(array);
		RaiseEventOptions options = new RaiseEventOptions
		{
			field_Public_EventCaching_0 = EventCaching.DoNotCache,
			field_Public_ReceiverGroup_0 = ReceiverGroup.Others
		};
		PhotonExtensions.RaiseEvent(1, array, options, default(SendOptions));
	}

	public static byte[] ModifyUSpeakPacket(string base64Packet, float newGain, bool newMuted)
	{
		byte[] packetData = Convert.FromBase64String(base64Packet);
		USpeakVoicePacket packet = ParseUSpeakPacket(packetData);
		packet.gain = (byte)Mathf.Clamp(newGain * 127.5f, 0f, 255f);
		if (newMuted)
		{
			packet.flags |= 128;
		}
		else
		{
			packet.flags &= 127;
		}
		return SerializePacket(packet);
	}

	public static byte[] ApplyAudioEffects(byte[] audioData, float gainMultiplier, bool distortion = false)
	{
		byte[] array = new byte[audioData.Length];
		for (int i = 0; i < audioData.Length; i++)
		{
			int num = audioData[i] - 128;
			num = (int)((float)num * gainMultiplier);
			if (distortion)
			{
				num = ((num > 0) ? Mathf.Min(num * 2, 127) : Mathf.Max(num * 2, -128));
			}
			array[i] = (byte)Mathf.Clamp(num + 128, 0, 255);
		}
		return array;
	}
}
