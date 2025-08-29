using System;
using MelonLoader;
using UnityEngine;
using Valve.VR;
using VRC.SDKBase;

namespace Odium.Components;

public static class Jetpack
{
	private static bool jetpackEnabled;

	public static bool IsEnabled => jetpackEnabled;

	public static void Activate(bool state)
	{
		jetpackEnabled = state;
		if (state)
		{
			MelonLogger.Msg("Jetpack ON");
		}
		else
		{
			MelonLogger.Msg("Jetpack OFF");
		}
	}

	public static void Update()
	{
		if (!jetpackEnabled || Networking.LocalPlayer == null)
		{
			return;
		}
		bool flag = Input.GetKey(KeyCode.Space);
		try
		{
			if (Bindings.Button_Jump != null)
			{
				flag = flag || Bindings.Button_Jump.GetState(SteamVR_Input_Sources.Any);
			}
		}
		catch
		{
		}
		if (flag)
		{
			ApplyJetpack();
		}
	}

	private static void ApplyJetpack()
	{
		try
		{
			VRCPlayerApi localPlayer = Networking.LocalPlayer;
			Vector3 velocity = localPlayer.GetVelocity();
			velocity.y = localPlayer.GetJumpImpulse();
			localPlayer.SetVelocity(velocity);
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Jetpack error: " + ex.Message);
		}
	}

	public static void Toggle()
	{
		Activate(!jetpackEnabled);
	}
}
