using System.Collections;
using MelonLoader;
using UnityEngine;
using VRC.Localization;

namespace VampClient.Api;

public class ToastBase
{
	public static void Toast(string content, string description = null, Sprite icon = null, float duration = 5f)
	{
		LocalizableString param_ = LocalizableStringExtensions.Localize(content);
		LocalizableString param_2 = LocalizableStringExtensions.Localize(description);
		VRCUiManager.field_Private_Static_VRCUiManager_0.field_Private_HudController_0.notification.Method_Public_Void_Sprite_LocalizableString_LocalizableString_Single_Object1PublicTBoTUnique_1_Boolean_0(icon, param_, param_2, duration);
		MelonLogger.Msg("\n" + content + "\n" + description + "\n\n");
	}

	public static IEnumerator DelayToast(float delay, string content, string description = null, Sprite icon = null, float duration = 5f)
	{
		float startTime = Time.time;
		while (Time.time < startTime + delay)
		{
			yield return null;
		}
		_ = Time.time;
		LocalizableString param_ = LocalizableStringExtensions.Localize(content);
		LocalizableString param_2 = LocalizableStringExtensions.Localize(description);
		VRCUiManager.field_Private_Static_VRCUiManager_0.field_Private_HudController_0.notification.Method_Public_Void_Sprite_LocalizableString_LocalizableString_Single_Object1PublicTBoTUnique_1_Boolean_0(icon, param_, param_2, duration);
		MelonLogger.Msg("\n" + content + "\n" + description + "\n\n");
	}
}
