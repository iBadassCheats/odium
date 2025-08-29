using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Odium.Components;

public static class ModSetup
{
	private static readonly string OdiumFolderPath = Path.Combine(Environment.CurrentDirectory, "Odium");

	private static readonly string AssetBundlesFolderPath = Path.Combine(Environment.CurrentDirectory, "Odium", "AssetBundles");

	private static readonly string OdiumPrefsPath = Path.Combine(OdiumFolderPath, "odium_prefs.json");

	private static readonly string QMBackgroundPath = Path.Combine(OdiumFolderPath, "ButtonBackground.png");

	private static readonly string ButtonBackgroundPath = Path.Combine(OdiumFolderPath, "QMBackground.png");

	private static readonly string QMHalfButtonPath = Path.Combine(OdiumFolderPath, "QMHalfButton.png");

	private static readonly string QMConsolePath = Path.Combine(OdiumFolderPath, "QMConsole.png");

	private static readonly string TabImagePath = Path.Combine(OdiumFolderPath, "OdiumIcon.png");

	private static readonly string NameplatePath = Path.Combine(OdiumFolderPath, "Nameplate.png");

	private static readonly string NotificationAssetBundlePath = Path.Combine(OdiumFolderPath, "AssetBundles", "notification");

	private const string AssetsZipUrl = "https://odiumvrc.com/files/odium-build-060.zip";

	private static readonly string TempZipPath = Path.Combine(Path.GetTempPath(), "odium_assets.zip");

	public static async Task Initialize()
	{
		try
		{
			OdiumConsole.Log("ModSetup", "Starting mod setup initialization...");
			await CheckAndCreateOdiumFolder();
			await CheckAndCreatePreferencesFile();
			if (!AssetsDownloaded())
			{
				await DownloadAndExtractAssets();
			}
			else
			{
				OdiumConsole.LogGradient("ModSetup", "Assets found, skipping download!");
			}
			OdiumConsole.LogGradient("ModSetup", "Mod setup completed successfully!");
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			OdiumConsole.LogException(ex2, "ModSetup.Initialize");
		}
	}

	private static async Task CheckAndCreateOdiumFolder()
	{
		try
		{
			if (!Directory.Exists(OdiumFolderPath))
			{
				OdiumConsole.Log("ModSetup", "Odium folder couldn't be found.. Creating folder");
				Directory.CreateDirectory(OdiumFolderPath);
				if (Directory.Exists(OdiumFolderPath))
				{
					OdiumConsole.LogGradient("ModSetup", "Odium folder created successfully at: " + OdiumFolderPath);
				}
				else
				{
					OdiumConsole.Log("ModSetup", "Failed to create Odium folder!", LogLevel.Error);
				}
			}
			else
			{
				OdiumConsole.LogGradient("ModSetup", "Odium folder found!");
			}
			if (!Directory.Exists(AssetBundlesFolderPath))
			{
				OdiumConsole.Log("ModSetup", "Odium's AssetBundles folder couldn't be found.. Creating folder");
				Directory.CreateDirectory(AssetBundlesFolderPath);
				if (Directory.Exists(AssetBundlesFolderPath))
				{
					OdiumConsole.LogGradient("ModSetup", "Odium's AssetBundles folder created successfully at: " + AssetBundlesFolderPath, LogLevel.Info, gradientCategory: true);
				}
				else
				{
					OdiumConsole.Log("ModSetup", "Failed to create Odium's AssetBundles folder!", LogLevel.Error);
				}
			}
			else
			{
				OdiumConsole.LogGradient("ModSetup", "Odium's AssetBundles folder found!");
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			OdiumConsole.LogException(ex2, "CheckAndCreateOdiumFolder");
		}
	}

	private static async Task CheckAndCreatePreferencesFile()
	{
		try
		{
			if (!File.Exists(OdiumPrefsPath))
			{
				OdiumConsole.Log("ModSetup", "odium_prefs.json not found. Creating default preferences file...");
				if (await CreateDefaultPreferencesFile())
				{
					OdiumConsole.LogGradient("ModSetup", "Default preferences file created successfully!");
				}
				else
				{
					OdiumConsole.Log("ModSetup", "Failed to create default preferences file!", LogLevel.Error);
				}
			}
			else
			{
				OdiumConsole.LogGradient("ModSetup", "odium_prefs.json found!");
				await ValidatePreferencesFile();
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			OdiumConsole.LogException(ex2, "CheckAndCreatePreferencesFile");
		}
	}

	private static async Task<bool> CreateDefaultPreferencesFile()
	{
		try
		{
			OdiumPreferences defaultPrefs = new OdiumPreferences
			{
				AllocConsole = true
			};
			File.WriteAllText(contents: OdiumJsonHandler.SerializePreferences(defaultPrefs), path: OdiumPrefsPath);
			if (File.Exists(OdiumPrefsPath))
			{
				FileInfo fileInfo = new FileInfo(OdiumPrefsPath);
				OdiumConsole.LogGradient("ModSetup", $"Preferences file saved successfully! Size: {fileInfo.Length} bytes");
				return true;
			}
			OdiumConsole.Log("ModSetup", "Preferences file creation completed but file verification failed!", LogLevel.Error);
			return false;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			OdiumConsole.LogException(ex2, "CreateDefaultPreferencesFile");
			return false;
		}
	}

	private static async Task ValidatePreferencesFile()
	{
		try
		{
			string jsonContent = File.ReadAllText(OdiumPrefsPath);
			OdiumPreferences preferences = OdiumJsonHandler.ParsePreferences(jsonContent);
			if (preferences != null)
			{
				OdiumConsole.Log("ModSetup", $"Preferences validated - Console allocation: {preferences.AllocConsole}");
				return;
			}
			OdiumConsole.Log("ModSetup", "Warning: Preferences file exists but could not be parsed correctly", LogLevel.Warning);
			OdiumConsole.Log("ModSetup", "Attempting to recreate preferences file with defaults...");
			await CreateDefaultPreferencesFile();
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "ValidatePreferencesFile");
			OdiumConsole.Log("ModSetup", "Attempting to recreate preferences file due to validation error...");
			await CreateDefaultPreferencesFile();
		}
	}

	private static bool AssetsDownloaded()
	{
		try
		{
			bool flag = File.Exists(ButtonBackgroundPath);
			bool flag2 = File.Exists(QMBackgroundPath);
			bool flag3 = Directory.Exists(AssetBundlesFolderPath) && Directory.GetFiles(AssetBundlesFolderPath).Length != 0;
			OdiumConsole.Log("ModSetup", $"Asset check - ButtonBackground: {flag}, QMBackground: {flag2}, AssetBundles folder has files: {flag3}");
			return flag && flag2 && flag3;
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "AssetsDownloaded");
			return false;
		}
	}

	private static async Task<bool> DownloadAndExtractAssets()
	{
		try
		{
			OdiumConsole.Log("ModSetup", "Downloading assets package...");
			using (HttpClient httpClient = new HttpClient())
			{
				httpClient.Timeout = TimeSpan.FromMinutes(5.0);
				OdiumConsole.Log("ModSetup", "Connecting to download server...");
				HttpResponseMessage response = await httpClient.GetAsync("https://odiumvrc.com/files/odium-build-060.zip");
				if (!response.IsSuccessStatusCode)
				{
					OdiumConsole.Log("ModSetup", $"Download failed with status: {response.StatusCode}", LogLevel.Error);
					return false;
				}
				byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
				OdiumConsole.Log("ModSetup", $"Downloaded {fileBytes.Length} bytes. Saving to temp location...");
				File.WriteAllBytes(TempZipPath, fileBytes);
				if (!File.Exists(TempZipPath))
				{
					OdiumConsole.Log("ModSetup", "Failed to save zip file to temp location!", LogLevel.Error);
					return false;
				}
				OdiumConsole.LogGradient("ModSetup", "Assets package downloaded successfully!");
			}
			OdiumConsole.Log("ModSetup", "Extracting all assets...");
			using (ZipArchive archive = ZipFile.OpenRead(TempZipPath))
			{
				int extractedCount = 0;
				foreach (ZipArchiveEntry entry in archive.Entries)
				{
					if (!string.IsNullOrEmpty(entry.Name))
					{
						string extractPath = Path.Combine(OdiumFolderPath, entry.FullName);
						string directoryPath = Path.GetDirectoryName(extractPath);
						if (!Directory.Exists(directoryPath))
						{
							Directory.CreateDirectory(directoryPath);
						}
						OdiumConsole.Log("ModSetup", "Extracting: " + entry.Name);
						entry.ExtractToFile(extractPath, overwrite: true);
						extractedCount++;
					}
				}
				OdiumConsole.LogGradient("ModSetup", $"Extracted {extractedCount} files successfully!");
			}
			try
			{
				if (File.Exists(TempZipPath))
				{
					File.Delete(TempZipPath);
					OdiumConsole.Log("ModSetup", "Temp zip file cleaned up");
				}
			}
			catch (Exception ex)
			{
				OdiumConsole.Log("ModSetup", "Warning: Could not delete temp file: " + ex.Message, LogLevel.Warning);
			}
			bool assetsExtracted = AssetsDownloaded();
			if (assetsExtracted)
			{
				OdiumConsole.LogGradient("ModSetup", "Assets verified successfully!");
			}
			else
			{
				OdiumConsole.Log("ModSetup", "Warning: Some key assets may not have been extracted correctly", LogLevel.Warning);
			}
			return assetsExtracted;
		}
		catch (HttpRequestException ex2)
		{
			HttpRequestException httpEx = ex2;
			OdiumConsole.Log("ModSetup", "Network error during download: " + httpEx.Message, LogLevel.Error);
			return false;
		}
		catch (TaskCanceledException)
		{
			OdiumConsole.Log("ModSetup", "Download timed out after 5 minutes", LogLevel.Error);
			return false;
		}
		catch (Exception ex4)
		{
			Exception ex5 = ex4;
			OdiumConsole.LogException(ex5, "DownloadAndExtractAssets");
			return false;
		}
	}

	public static string GetOdiumFolderPath()
	{
		return OdiumFolderPath;
	}

	public static string GetButtonBackgroundPath()
	{
		return ButtonBackgroundPath;
	}

	public static string GetQMBackgroundPath()
	{
		return QMBackgroundPath;
	}

	public static string GetOdiumPrefsPath()
	{
		return OdiumPrefsPath;
	}

	public static string GetQMHalfButtonPath()
	{
		return QMHalfButtonPath;
	}

	public static string GetQMConsolePath()
	{
		return QMConsolePath;
	}

	public static string GetTabImagePath()
	{
		return TabImagePath;
	}

	public static string GetNameplatePath()
	{
		return NameplatePath;
	}

	public static bool ValidateSetup()
	{
		try
		{
			bool flag = Directory.Exists(OdiumFolderPath);
			bool flag2 = File.Exists(OdiumPrefsPath);
			bool flag3 = AssetsDownloaded();
			OdiumConsole.Log("ModSetup", $"Setup validation - Folder: {flag}, Preferences: {flag2}, Assets: {flag3}");
			return flag && flag2 && flag3;
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "ValidateSetup");
			return false;
		}
	}

	public static void CleanUp()
	{
		try
		{
			if (Directory.Exists(OdiumFolderPath))
			{
				Directory.Delete(OdiumFolderPath, recursive: true);
				OdiumConsole.Log("ModSetup", "Odium folder and contents deleted");
			}
			if (File.Exists(TempZipPath))
			{
				File.Delete(TempZipPath);
				OdiumConsole.Log("ModSetup", "Temp zip file cleaned up");
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex, "CleanUp");
		}
	}

	public static async Task ForceUpdateAllAssets()
	{
		try
		{
			OdiumConsole.Log("ModSetup", "Starting force update of all assets...");
			if (Directory.Exists(OdiumFolderPath))
			{
				string[] allFiles = Directory.GetFiles(OdiumFolderPath, "*", SearchOption.AllDirectories);
				string[] array = allFiles;
				foreach (string file in array)
				{
					if (!file.Equals(OdiumPrefsPath, StringComparison.OrdinalIgnoreCase))
					{
						try
						{
							File.Delete(file);
							OdiumConsole.Log("ModSetup", "Deleted existing file: " + Path.GetFileName(file));
						}
						catch (Exception ex)
						{
							Exception deleteEx = ex;
							OdiumConsole.Log("ModSetup", "Warning: Could not delete " + Path.GetFileName(file) + ": " + deleteEx.Message, LogLevel.Warning);
						}
					}
				}
				string[] allDirs = Directory.GetDirectories(OdiumFolderPath, "*", SearchOption.AllDirectories);
				string[] array2 = allDirs;
				foreach (string dir in array2)
				{
					try
					{
						if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
						{
							Directory.Delete(dir);
							OdiumConsole.Log("ModSetup", "Deleted empty directory: " + Path.GetFileName(dir));
						}
					}
					catch (Exception ex2)
					{
						OdiumConsole.Log("ModSetup", "Warning: Could not delete directory " + Path.GetFileName(dir) + ": " + ex2.Message, LogLevel.Warning);
					}
				}
			}
			if (await DownloadAndExtractAssets())
			{
				OdiumConsole.LogGradient("ModSetup", "All assets force updated successfully!");
			}
			else
			{
				OdiumConsole.Log("ModSetup", "Force update failed!", LogLevel.Error);
			}
		}
		catch (Exception ex)
		{
			Exception ex3 = ex;
			OdiumConsole.LogException(ex3, "ForceUpdateAllAssets");
		}
	}

	public static async Task ForceRecreatePreferences()
	{
		try
		{
			if (File.Exists(OdiumPrefsPath))
			{
				File.Delete(OdiumPrefsPath);
				OdiumConsole.Log("ModSetup", "Existing preferences file deleted");
			}
			await CheckAndCreatePreferencesFile();
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			OdiumConsole.LogException(ex2, "ForceRecreatePreferences");
		}
	}

	public static async Task ForceUpdateQMBackground()
	{
		await ForceUpdateAllAssets();
	}

	public static async Task ForceUpdateButtonBackground()
	{
		await ForceUpdateAllAssets();
	}

	public static async Task ForceUpdateQMHalfButton()
	{
		await ForceUpdateAllAssets();
	}

	public static async Task ForceUpdateQMConsole()
	{
		await ForceUpdateAllAssets();
	}

	public static async Task ForceUpdateTabImage()
	{
		await ForceUpdateAllAssets();
	}

	public static async Task ForceUpdateNameplate()
	{
		await ForceUpdateAllAssets();
	}

	public static async Task ForceUpdateAllImages()
	{
		await ForceUpdateAllAssets();
	}
}
