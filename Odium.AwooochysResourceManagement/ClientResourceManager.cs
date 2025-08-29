using System;
using System.IO;
using System.Net;

namespace Odium.AwooochysResourceManagement;

public static class ClientResourceManager
{
	private const string ClientDirectory = "Odium";

	private const string ResourceBaseUrl = "https://nigga.rest/DeepOdium/";

	private static readonly (string FileName, string TargetDirectory)[] RequiredResources = new(string, string)[1] { ("LoadingBackgrund.png", "Odium") };

	public static void EnsureAllResourcesExist()
	{
		EnsureDirectoryStructure();
		(string, string)[] requiredResources = RequiredResources;
		for (int i = 0; i < requiredResources.Length; i++)
		{
			(string, string) tuple = requiredResources[i];
			string path = Path.Combine(tuple.Item2, tuple.Item1);
			if (!File.Exists(path))
			{
				DownloadFile(tuple.Item1, tuple.Item2);
			}
		}
	}

	public static bool TryGetResourcePath(string fileName, string subDirectory, out string fullPath)
	{
		string path = (string.IsNullOrEmpty(subDirectory) ? "Odium" : Path.Combine("Odium", subDirectory));
		fullPath = Path.Combine(path, fileName);
		return File.Exists(fullPath);
	}

	private static void EnsureDirectoryStructure()
	{
		CreateDirectoryIfNotExists("Odium");
	}

	private static void CreateDirectoryIfNotExists(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
			Console.WriteLine("ClientResourceManager: Created directory: " + path);
		}
	}

	private static void DownloadFile(string fileName, string targetDirectory)
	{
		string text = Path.Combine(targetDirectory, fileName);
		string url = "https://nigga.rest/DeepOdium/" + fileName;
		Console.WriteLine("ClientResourceManager: File not found: " + text + ", Downloading...");
		try
		{
			byte[] bytes = DownloadFileData(url);
			File.WriteAllBytes(text, bytes);
			Console.WriteLine("ClientResourceManager: Successfully downloaded: " + text);
		}
		catch (Exception ex)
		{
			Console.WriteLine("ClientResourceManager: Failed to download " + fileName + " to " + targetDirectory + ": " + ex.Message);
			if (fileName.EndsWith(".dll"))
			{
				Console.WriteLine("ClientResourceManager: CRITICAL: Failed to download a dependency DLL. Some features may not work.");
			}
		}
	}

	private static byte[] DownloadFileData(string url)
	{
		using WebClient webClient = new WebClient();
		return webClient.DownloadData(url);
	}
}
