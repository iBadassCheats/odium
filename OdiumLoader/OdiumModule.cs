using MelonLoader;

namespace OdiumLoader;

public abstract class OdiumModule
{
	public virtual string ModuleName => GetType().Name;

	public virtual string ModuleVersion => "1.0.0";

	public virtual string ModuleAuthor => "Unknown";

	public bool IsEnabled { get; set; } = true;

	protected MelonLogger.Instance Logger { get; private set; }

	public virtual void OnModuleLoad()
	{
	}

	public virtual void OnApplicationStart()
	{
	}

	public virtual void OnUpdate()
	{
	}

	public virtual void OnFixedUpdate()
	{
	}

	public virtual void OnLateUpdate()
	{
	}

	public virtual void OnApplicationQuit()
	{
	}

	public virtual void OnSceneLoaded(int buildIndex, string sceneName)
	{
	}

	public virtual void OnSceneUnloaded(int buildIndex, string sceneName)
	{
	}

	internal void SetLogger(MelonLogger.Instance logger)
	{
		Logger = logger;
	}
}
