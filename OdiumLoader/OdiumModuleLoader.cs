using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader;
using Odium;
using UnityEngine;

namespace OdiumLoader;

public class OdiumModuleLoader
{
	private static List<OdiumModule> loadedModules = new List<OdiumModule>();

	private static Dictionary<Type, object> typeCache = new Dictionary<Type, object>();

	public static void OnApplicationStart()
	{
		OdiumConsole.LogGradient("ModuleLoader", "Odium Module Loader starting...");
		LoadModules();
		foreach (OdiumModule loadedModule in loadedModules)
		{
			if (loadedModule.IsEnabled)
			{
				try
				{
					loadedModule.OnApplicationStart();
				}
				catch (Exception arg)
				{
					OdiumConsole.LogGradient("ModuleLoader", $"Error in {loadedModule.ModuleName}.OnApplicationStart(): {arg}", LogLevel.Error);
				}
			}
		}
	}

	public static void OnUpdate()
	{
		foreach (OdiumModule loadedModule in loadedModules)
		{
			if (loadedModule.IsEnabled)
			{
				try
				{
					loadedModule.OnUpdate();
				}
				catch (Exception arg)
				{
					OdiumConsole.LogGradient("ModuleLoader", $"Error in {loadedModule.ModuleName}.OnUpdate(): {arg}", LogLevel.Error);
				}
			}
		}
	}

	public static void OnFixedUpdate()
	{
		foreach (OdiumModule loadedModule in loadedModules)
		{
			if (loadedModule.IsEnabled)
			{
				try
				{
					loadedModule.OnFixedUpdate();
				}
				catch (Exception arg)
				{
					OdiumConsole.LogGradient("ModuleLoader", $"Error in {loadedModule.ModuleName}.OnFixedUpdate(): {arg}", LogLevel.Error);
				}
			}
		}
	}

	public static void OnLateUpdate()
	{
		foreach (OdiumModule loadedModule in loadedModules)
		{
			if (loadedModule.IsEnabled)
			{
				try
				{
					loadedModule.OnLateUpdate();
				}
				catch (Exception arg)
				{
					OdiumConsole.LogGradient("ModuleLoader", $"Error in {loadedModule.ModuleName}.OnLateUpdate(): {arg}", LogLevel.Error);
				}
			}
		}
	}

	public static void OnApplicationQuit()
	{
		foreach (OdiumModule loadedModule in loadedModules)
		{
			if (loadedModule.IsEnabled)
			{
				try
				{
					loadedModule.OnApplicationQuit();
				}
				catch (Exception arg)
				{
					OdiumConsole.LogGradient("ModuleLoader", $"Error in {loadedModule.ModuleName}.OnApplicationQuit(): {arg}", LogLevel.Error);
				}
			}
		}
	}

	public static void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
		foreach (OdiumModule loadedModule in loadedModules)
		{
			if (loadedModule.IsEnabled)
			{
				try
				{
					loadedModule.OnSceneLoaded(buildIndex, sceneName);
				}
				catch (Exception arg)
				{
					OdiumConsole.LogGradient("ModuleLoader", $"Error in {loadedModule.ModuleName}.OnSceneLoaded(): {arg}", LogLevel.Error);
				}
			}
		}
	}

	public static void OnSceneWasUnloaded(int buildIndex, string sceneName)
	{
		foreach (OdiumModule loadedModule in loadedModules)
		{
			if (loadedModule.IsEnabled)
			{
				try
				{
					loadedModule.OnSceneUnloaded(buildIndex, sceneName);
				}
				catch (Exception arg)
				{
					OdiumConsole.LogGradient("ModuleLoader", $"Error in {loadedModule.ModuleName}.OnSceneUnloaded(): {arg}", LogLevel.Error);
				}
			}
		}
	}

	public static void LoadModules()
	{
		try
		{
			string text = Path.Combine(Environment.CurrentDirectory, "OdiumModules");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
				OdiumConsole.LogGradient("ModuleLoader", "Created OdiumModules directory at: " + text);
				OdiumConsole.LogGradient("ModuleLoader", "Place your module DLL files in this folder to load them.");
				return;
			}
			OdiumConsole.LogGradient("ModuleLoader", "Loading modules from: " + text);
			string[] files = Directory.GetFiles(text, "*.dll", SearchOption.TopDirectoryOnly);
			if (files.Length == 0)
			{
				OdiumConsole.LogGradient("ModuleLoader", "No module DLL files found in OdiumModules folder.", LogLevel.Warning);
				return;
			}
			List<Assembly> list = new List<Assembly>();
			string[] array = files;
			foreach (string text2 in array)
			{
				try
				{
					string fileName = Path.GetFileName(text2);
					OdiumConsole.LogGradient("ModuleLoader", "Loading assembly: " + fileName);
					Assembly item = Assembly.LoadFrom(text2);
					list.Add(item);
					OdiumConsole.LogGradient("ModuleLoader", "Successfully loaded assembly: " + fileName);
				}
				catch (Exception ex)
				{
					OdiumConsole.LogGradient("ModuleLoader", "Failed to load assembly " + Path.GetFileName(text2) + ": " + ex.Message);
				}
			}
			List<Assembly> collection = (from a in AppDomain.CurrentDomain.GetAssemblies()
				where !a.IsDynamic && a.Location.Contains("OdiumModules")
				select a).ToList();
			list.AddRange(collection);
			foreach (Assembly item2 in list)
			{
				try
				{
					List<Type> list2 = (from t in item2.GetTypes()
						where t.IsSubclassOf(typeof(OdiumModule)) && !t.IsAbstract
						select t).ToList();
					if (list2.Any())
					{
						MelonLogger.Msg($"Found {list2.Count} module(s) in {item2.GetName().Name}");
					}
					foreach (Type item3 in list2)
					{
						try
						{
							OdiumModuleAttribute customAttribute = item3.GetCustomAttribute<OdiumModuleAttribute>();
							if (customAttribute != null && !customAttribute.AutoLoad)
							{
								OdiumConsole.LogGradient("ModuleLoader", "Skipping module " + item3.Name + " (AutoLoad disabled)");
								continue;
							}
							OdiumModule odiumModule = (OdiumModule)Activator.CreateInstance(item3);
							MelonLogger.Instance logger = new MelonLogger.Instance("[" + odiumModule.ModuleName + "]");
							odiumModule.SetLogger(logger);
							loadedModules.Add(odiumModule);
							OdiumConsole.LogGradient("ModuleLoader", "Loaded module: " + odiumModule.ModuleName + " v" + odiumModule.ModuleVersion + " by " + odiumModule.ModuleAuthor);
							odiumModule.OnModuleLoad();
						}
						catch (Exception arg)
						{
							OdiumConsole.LogGradient("ModuleLoader", $"Failed to instantiate module {item3.Name}: {arg}");
						}
					}
				}
				catch (Exception ex2)
				{
					OdiumConsole.LogGradient("ModuleLoader", "Could not process assembly " + item2.GetName().Name + ": " + ex2.Message, LogLevel.Error);
				}
			}
			MelonLogger.Msg($"Successfully loaded {loadedModules.Count} modules total from OdiumModules folder");
			if (loadedModules.Count == 0)
			{
				OdiumConsole.LogGradient("ModuleLoader", "No valid modules found. Make sure your module DLLs:");
				OdiumConsole.LogGradient("ModuleLoader", "  - Are placed in the OdiumModules folder");
				OdiumConsole.LogGradient("ModuleLoader", "  - Contain classes that inherit from OdiumModule");
				OdiumConsole.LogGradient("ModuleLoader", "  - Are compiled for the same .NET version");
			}
		}
		catch (Exception arg2)
		{
			OdiumConsole.LogGradient("ModuleLoader", $"Error loading modules: {arg2}", LogLevel.Error);
		}
	}

	public static T GetModule<T>() where T : OdiumModule
	{
		return loadedModules.OfType<T>().FirstOrDefault();
	}

	public static List<OdiumModule> GetAllModules()
	{
		return new List<OdiumModule>(loadedModules);
	}

	public static void SetModuleEnabled<T>(bool enabled) where T : OdiumModule
	{
		T module = GetModule<T>();
		if (module != null)
		{
			module.IsEnabled = enabled;
			OdiumConsole.LogGradient("ModuleLoader", "Module " + module.ModuleName + " " + (enabled ? "enabled" : "disabled"));
		}
	}

	public static T GetOrCreateCachedType<T>() where T : class, new()
	{
		Type typeFromHandle = typeof(T);
		if (!typeCache.ContainsKey(typeFromHandle))
		{
			typeCache[typeFromHandle] = new T();
		}
		return (T)typeCache[typeFromHandle];
	}

	public static T[] FindObjectsOfType<T>() where T : UnityEngine.Object
	{
		return UnityEngine.Object.FindObjectsOfType<T>();
	}

	public static T FindObjectOfType<T>() where T : UnityEngine.Object
	{
		return UnityEngine.Object.FindObjectOfType<T>();
	}

	public static void ReloadModules()
	{
		OdiumConsole.LogGradient("ModuleLoader", "Reloading modules...");
		foreach (OdiumModule loadedModule in loadedModules)
		{
			try
			{
				loadedModule.OnApplicationQuit();
			}
			catch (Exception arg)
			{
				OdiumConsole.LogGradient("ModuleLoader", $"Error during {loadedModule.ModuleName} cleanup: {arg}", LogLevel.Error);
			}
		}
		loadedModules.Clear();
		typeCache.Clear();
		LoadModules();
		foreach (OdiumModule loadedModule2 in loadedModules)
		{
			if (loadedModule2.IsEnabled)
			{
				try
				{
					loadedModule2.OnApplicationStart();
				}
				catch (Exception arg2)
				{
					OdiumConsole.LogGradient("ModuleLoader", $"Error in {loadedModule2.ModuleName}.OnApplicationStart() during reload: {arg2}", LogLevel.Error);
				}
			}
		}
	}

	public static string GetModulesPath()
	{
		return Path.Combine(Environment.CurrentDirectory, "OdiumModules");
	}
}
