using System;
using Odium.Wrappers;
using UnityEngine;

namespace Odium.Modules;

public class FlyComponent
{
	private static bool setupedNormalFly = true;

	private static float cachedGravity = 0f;

	public static bool FlyEnabled = false;

	public static float FlySpeed = 5f;

	public static DateTime LastKeyCheck = DateTime.Now;

	public static void OnUpdate()
	{
		DateTime now = DateTime.Now;
		if ((now - LastKeyCheck).TotalMilliseconds >= 10.0)
		{
			if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftControl))
			{
				ToggleFly();
			}
			LastKeyCheck = now;
		}
		if (!(PlayerWrapper.LocalPlayer == null) && PlayerWrapper.LocalPlayer.field_Private_VRCPlayerApi_0 != null)
		{
			if (!FlyEnabled)
			{
				DisableFly();
				return;
			}
			EnableFly();
			HandleMovement();
		}
	}

	private static void ToggleFly()
	{
		FlyEnabled = !FlyEnabled;
		if (!FlyEnabled)
		{
			DisableFly();
		}
	}

	private static void EnableFly()
	{
		if (setupedNormalFly)
		{
			cachedGravity = PlayerWrapper.LocalPlayer.field_Private_VRCPlayerApi_0.GetGravityStrength();
			PlayerWrapper.LocalPlayer.field_Private_VRCPlayerApi_0.SetGravityStrength(0f);
			Collider component = PlayerWrapper.LocalPlayer.gameObject.GetComponent<Collider>();
			if (component != null)
			{
				component.enabled = false;
			}
			setupedNormalFly = false;
		}
	}

	private static void DisableFly()
	{
		if (!setupedNormalFly)
		{
			PlayerWrapper.LocalPlayer.field_Private_VRCPlayerApi_0.SetGravityStrength(cachedGravity);
			Collider collider = PlayerWrapper.LocalPlayer.gameObject?.GetComponent<Collider>();
			if (collider != null)
			{
				collider.enabled = true;
			}
			setupedNormalFly = true;
		}
	}

	private static void HandleMovement()
	{
		Transform transform = PlayerWrapper.LocalPlayer.gameObject.transform;
		Transform transform2 = Camera.main?.transform;
		if (!(transform2 == null))
		{
			float num = FlySpeed;
			if (Input.GetKey(KeyCode.LeftShift))
			{
				num *= 2f;
			}
			Vector3 zero = Vector3.zero;
			float axis = Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical");
			float axis2 = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal");
			float axis3 = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical");
			Vector3 forward = transform2.forward;
			forward.y = 0f;
			forward.Normalize();
			Vector3 right = transform2.right;
			right.y = 0f;
			right.Normalize();
			if (axis != 0f)
			{
				zero += Vector3.up * axis * num * Time.deltaTime;
			}
			if (axis2 != 0f)
			{
				zero += right * axis2 * num * Time.deltaTime;
			}
			if (axis3 != 0f)
			{
				zero += forward * axis3 * num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.Q))
			{
				zero += Vector3.down * num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.E))
			{
				zero += Vector3.up * num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.A))
			{
				zero += -right * num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.D))
			{
				zero += right * num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.S))
			{
				zero += -forward * num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.W))
			{
				zero += forward * num * Time.deltaTime;
			}
			transform.position += zero;
		}
	}
}
