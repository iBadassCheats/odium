using System;
using System.Collections.Generic;
using UnityEngine;

namespace Odium.Components;

public class Chatbox
{
	private class FrameEffect
	{
		public string[] frames;

		public int currentFrameIndex;

		public float frameTimer;

		public Action onComplete;

		public bool shouldLoop;

		public bool isWaiting;

		public float waitTime;

		public FrameEffect(string[] frameArray, Action callback, bool loop = false, float loopWaitTime = 3f)
		{
			frames = frameArray;
			currentFrameIndex = 0;
			frameTimer = 0f;
			onComplete = callback;
			shouldLoop = loop;
			isWaiting = false;
			waitTime = loopWaitTime;
		}

		public void Reset()
		{
			frameTimer = 0f;
			currentFrameIndex = 0;
			isWaiting = false;
		}

		public string GetCurrentFrame()
		{
			if (frames != null && currentFrameIndex < frames.Length)
			{
				return frames[currentFrameIndex];
			}
			return "";
		}

		public void MoveToNextFrame()
		{
			currentFrameIndex++;
		}

		public bool HasMoreFrames()
		{
			return currentFrameIndex < frames.Length;
		}
	}

	private static Dictionary<string, FrameEffect> activeFrameEffects = new Dictionary<string, FrameEffect>();

	public static void SendFrameAnimation(string[] frames, string effectId = null, Action onComplete = null, bool loop = false, float loopWaitTime = 3f)
	{
		if (effectId != null && activeFrameEffects.ContainsKey(effectId))
		{
			activeFrameEffects.Remove(effectId);
		}
		FrameEffect value = new FrameEffect(frames, onComplete, loop, loopWaitTime);
		if (effectId != null)
		{
			activeFrameEffects[effectId] = value;
			return;
		}
		string key = "auto_" + Guid.NewGuid().ToString("N").Substring(0, 8);
		activeFrameEffects[key] = value;
	}

	public static void CancelFrameEffect(string effectId)
	{
		if (effectId != null && activeFrameEffects.ContainsKey(effectId))
		{
			activeFrameEffects.Remove(effectId);
		}
	}

	public static void UpdateFrameEffects()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, FrameEffect> activeFrameEffect in activeFrameEffects)
		{
			string key = activeFrameEffect.Key;
			FrameEffect value = activeFrameEffect.Value;
			value.frameTimer += Time.deltaTime;
			if (value.isWaiting)
			{
				if (value.frameTimer >= value.waitTime)
				{
					value.Reset();
				}
			}
			else
			{
				if (!(value.frameTimer >= 0.12f))
				{
					continue;
				}
				value.frameTimer = 0f;
				if (!value.HasMoreFrames())
				{
					value.onComplete?.Invoke();
					if (value.shouldLoop)
					{
						value.isWaiting = true;
						value.frameTimer = 0f;
					}
					else
					{
						list.Add(key);
					}
				}
				else
				{
					string currentFrame = value.GetCurrentFrame();
					SendCustomChatMessage(currentFrame);
					value.MoveToNextFrame();
				}
			}
		}
		foreach (string item in list)
		{
			activeFrameEffects.Remove(item);
		}
	}

	public static void SendCustomChatMessage(string message)
	{
		try
		{
			PhotonExtensions.SendLowLevelEvent(43, message, 8);
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("PhotonEvent", "Failed to send custom message: " + ex.Message, LogLevel.Error);
		}
	}
}
