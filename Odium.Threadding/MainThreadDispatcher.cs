using System;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;

namespace Odium.Threadding;

public static class MainThreadDispatcher
{
	private static readonly Queue<Action> _executionQueue = new Queue<Action>();

	private static readonly object _lock = new object();

	private static bool _initialized = false;

	public static void Initialize()
	{
		if (!_initialized)
		{
			MelonEvents.OnGUI.Subscribe(ProcessQueue);
			_initialized = true;
		}
	}

	private static void ProcessQueue()
	{
		lock (_lock)
		{
			while (_executionQueue.Count > 0)
			{
				try
				{
					_executionQueue.Dequeue()();
				}
				catch (Exception arg)
				{
					MelonLogger.Error($"Error executing main thread action: {arg}");
				}
			}
		}
	}

	public static void Enqueue(Action action)
	{
		if (action == null)
		{
			return;
		}
		lock (_lock)
		{
			_executionQueue.Enqueue(action);
		}
	}

	public static void EnqueueCoroutine(IEnumerator coroutine)
	{
		Enqueue(delegate
		{
			MelonCoroutines.Start(coroutine);
		});
	}
}
