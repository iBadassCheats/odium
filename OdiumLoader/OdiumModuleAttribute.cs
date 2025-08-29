using System;

namespace OdiumLoader;

[AttributeUsage(AttributeTargets.Class)]
public class OdiumModuleAttribute : Attribute
{
	public string Name { get; set; }

	public string Version { get; set; }

	public string Author { get; set; }

	public string Description { get; set; }

	public bool AutoLoad { get; set; } = true;

	public OdiumModuleAttribute(string name, string version = "1.0.0", string author = "Unknown")
	{
		Name = name;
		Version = version;
		Author = author;
	}
}
