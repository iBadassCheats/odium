using System;
using System.Reflection;
using Harmony;
using HarmonyLib;

namespace Odium.Patches;

public class EasyPatching
{
	public static HarmonyLib.Harmony DeepCoreInstance = new HarmonyLib.Harmony("DeePatch");

	public static void EasyPatchPropertyPost(Type inputclass, string InputMethodName, Type outputclass, string outputmethodname)
	{
		DeepCoreInstance.Patch(Harmony.AccessTools.Property(inputclass, InputMethodName).GetMethod, null, new Harmony.HarmonyMethod(outputclass, outputmethodname));
	}

	public static void EasyPatchPropertyPre(Type inputclass, string InputMethodName, Type outputclass, string outputmethodname)
	{
		DeepCoreInstance.Patch(Harmony.AccessTools.Property(inputclass, InputMethodName).GetMethod, new Harmony.HarmonyMethod(outputclass, outputmethodname));
	}

	public static void EasyPatchMethodPre(Type inputclass, string InputMethodName, Type outputclass, string outputmethodname)
	{
		DeepCoreInstance.Patch(inputclass.GetMethod(InputMethodName), new Harmony.HarmonyMethod(Harmony.AccessTools.Method(outputclass, outputmethodname)));
	}

	public static void EasyPatchMethodPost(Type inputclass, string InputMethodName, Type outputclass, string outputmethodname)
	{
		DeepCoreInstance.Patch(inputclass.GetMethod(InputMethodName), null, new Harmony.HarmonyMethod(Harmony.AccessTools.Method(outputclass, outputmethodname)));
	}

	[Obsolete]
	internal static Harmony.HarmonyMethod GetLocalPatch<T>(string name)
	{
		return new Harmony.HarmonyMethod(typeof(T).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic));
	}
}
