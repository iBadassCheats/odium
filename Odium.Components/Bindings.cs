using System;
using Valve.VR;

namespace Odium.Components;

public class Bindings
{
	public static SteamVR_Action_Boolean Button_Jump;

	public static SteamVR_Action_Boolean Button_QM;

	public static SteamVR_Action_Boolean Button_Mic;

	public static SteamVR_Action_Boolean Button_Grab;

	public static SteamVR_Action_Boolean Button_Interact;

	public static SteamVR_Action_Single Trigger;

	public static SteamVR_Action_Vector2 MoveJoystick;

	public static SteamVR_Action_Vector2 RotateJoystick;

	public static void Register()
	{
		Button_Jump = SteamVR_Input.GetBooleanAction("jump");
		Button_Mic = SteamVR_Input.GetBooleanAction("Toggle Microphone");
		Button_QM = SteamVR_Input.GetBooleanAction("Menu");
		Button_Grab = SteamVR_Input.GetBooleanAction("Grab");
		Button_Interact = SteamVR_Input.GetBooleanAction("Interact");
		Trigger = SteamVR_Input.GetSingleAction("gesture_trigger_axis");
		MoveJoystick = SteamVR_Input.GetVector2Action("Move");
		RotateJoystick = SteamVR_Input.GetVector2Action("Rotate");
		Console.WriteLine("VRBinds: Binds Registered.");
	}
}
