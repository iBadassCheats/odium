using System;
using UnityEngine;

public class USpeakVoiceManager : MonoBehaviour
{
	[Serializable]
	public class VoiceSettings
	{
		public float gain = 1f;

		public float pitch = 1f;

		public bool muted = false;

		public bool robotVoice = false;

		public bool whisper = false;
	}

	public VoiceSettings voiceSettings = new VoiceSettings();

	public void ProcessIncomingVoicePacket(string base64Packet)
	{
		try
		{
			byte[] packetData = Convert.FromBase64String(base64Packet);
			USpeakPacketHandler.USpeakVoicePacket packet = USpeakPacketHandler.ParseUSpeakPacket(packetData);
			if (voiceSettings.robotVoice)
			{
				packet.audioData = USpeakPacketHandler.ApplyAudioEffects(packet.audioData, voiceSettings.gain, distortion: true);
			}
			byte[] array = USpeakPacketHandler.SerializePacket(packet);
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to process voice packet: " + ex.Message);
		}
	}
}
