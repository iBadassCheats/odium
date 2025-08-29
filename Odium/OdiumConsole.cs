using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Odium.Components;

namespace Odium;

public static class OdiumConsole
{
	private const int STD_OUTPUT_HANDLE = -11;

	private const int STD_INPUT_HANDLE = -10;

	private const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

	private const int ENABLE_ECHO_INPUT = 4;

	private const int ENABLE_LINE_INPUT = 2;

	private const int ENABLE_PROCESSED_INPUT = 1;

	private static bool _isInitialized;

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool AllocConsole();

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool FreeConsole();

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GetConsoleWindow();

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GetStdHandle(int nStdHandle);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

	public static void Initialize()
	{
		if (ShouldAllocateConsole() && !(GetConsoleWindow() != IntPtr.Zero))
		{
			try
			{
				AllocConsole();
				Thread.Sleep(200);
				InitializeStandardStreams();
				Console.Title = "Odium Console - Cracked by AllahLeaks";
				EnableVirtualTerminalProcessing();
				EnableInputMode();
				Console.CursorVisible = true;
				DisplayBanner();
				Log("System", "Console initialized successfully");
				Log("System", "Console ready for input commands");
				_isInitialized = true;
			}
			catch (Exception ex)
			{
				Log("System", "Failed to initialize console: " + ex.Message, LogLevel.Error);
			}
		}
	}

	private static void InitializeStandardStreams()
	{
		try
		{
			Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
			{
				AutoFlush = true
			});
			Console.SetIn(new StreamReader(Console.OpenStandardInput()));
		}
		catch (Exception ex)
		{
			Log("System", "Failed to initialize streams: " + ex.Message, LogLevel.Error);
		}
	}

	private static void EnableInputMode()
	{
		try
		{
			IntPtr stdHandle = GetStdHandle(-10);
			if (stdHandle != IntPtr.Zero && GetConsoleMode(stdHandle, out var _))
			{
				SetConsoleMode(stdHandle, 7);
			}
		}
		catch (Exception ex)
		{
			Log("System", "Failed to set input mode: " + ex.Message, LogLevel.Warning);
		}
	}

	private static bool ShouldAllocateConsole()
	{
		try
		{
			string text = Path.Combine(ModSetup.GetOdiumFolderPath(), "odium_prefs.json");
			if (!File.Exists(text))
			{
				CreateDefaultPreferencesFile(text);
				return true;
			}
			return OdiumJsonHandler.ParsePreferences(File.ReadAllText(text))?.AllocConsole ?? true;
		}
		catch
		{
			return true;
		}
	}

	private static void CreateDefaultPreferencesFile(string filePath)
	{
		try
		{
			string contents = OdiumJsonHandler.SerializePreferences(new OdiumPreferences
			{
				AllocConsole = true
			});
			File.WriteAllText(filePath, contents);
		}
		catch
		{
		}
	}

	public static void Log(string category, string message, LogLevel level = LogLevel.Info)
	{
		if (_isInitialized)
		{
			try
			{
				string text = DateTime.Now.ToString("HH:mm:ss");
				Console.ResetColor();
				Console.Write("[" + text + "] ");
				Console.ForegroundColor = GetCategoryColor(category);
				Console.Write("[" + category + "] ");
				Console.ResetColor();
				Console.ForegroundColor = GetLevelColor(level);
				Console.WriteLine(message);
				Console.ResetColor();
			}
			catch
			{
				Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{category}] {message}");
			}
		}
	}

	public static string Readline(string category, string message, LogLevel level = LogLevel.Info)
	{
		string result = string.Empty;
		if (!_isInitialized)
		{
			return result;
		}
		try
		{
			result = Console.ReadLine();
		}
		catch
		{
			Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{category}] {message}");
		}
		return result;
	}

	public static void LogGradient(string category, string message, LogLevel level = LogLevel.Info, bool gradientCategory = false)
	{
		if (!_isInitialized)
		{
			return;
		}
		try
		{
			string text = DateTime.Now.ToString("HH:mm:ss");
			Console.ResetColor();
			Console.Write("[" + text + "] ");
			if (gradientCategory)
			{
				Console.Write("[");
				LogMessageWithGradient(category, addNewline: false);
				Console.Write("] ");
				Console.ResetColor();
				Console.WriteLine(message);
			}
			else
			{
				Console.ForegroundColor = GetCategoryColor(category);
				Console.Write("[" + category + "] ");
				Console.ResetColor();
				LogMessageWithGradient(message);
			}
		}
		catch
		{
			Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{category}] {message}");
		}
	}

	private static void LogMessageWithGradient(string text, bool addNewline = true)
	{
		int length = text.Length;
		for (int i = 0; i < length; i++)
		{
			int num = 255;
			int num2 = 204 - i * 204 / length;
			int num3 = 203 + i * 52 / length;
			Console.Write($"\u001b[38;2;{num};{num2};{num3}m{text[i]}");
		}
		Console.Write("\u001b[0m");
		if (addNewline)
		{
			Console.WriteLine();
		}
	}

	private static void LogWithGradient(string text, (int R, int G, int B) startColor, (int R, int G, int B) endColor)
	{
		int length = text.Length;
		if (length == 0)
		{
			Console.WriteLine();
			return;
		}
		for (int i = 0; i < length; i++)
		{
			float num = ((length == 1) ? 0f : ((float)i / (float)(length - 1)));
			int num2 = (int)((float)startColor.R + num * (float)(endColor.R - startColor.R));
			int num3 = (int)((float)startColor.G + num * (float)(endColor.G - startColor.G));
			int num4 = (int)((float)startColor.B + num * (float)(endColor.B - startColor.B));
			Console.Write($"\u001b[38;2;{num2};{num3};{num4}m{text[i]}");
		}
		Console.WriteLine("\u001b[0m");
	}

	public static void LogException(Exception ex, string context = null)
	{
		if (_isInitialized)
		{
			string text = DateTime.Now.ToString("HH:mm:ss");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\n[" + text + "] ============ EXCEPTION ============");
			Console.WriteLine("Context: " + (context ?? "None"));
			Console.WriteLine("Type: " + ex.GetType().Name);
			Console.WriteLine("Message: " + ex.Message);
			Console.WriteLine("Stack Trace:\n" + ex.StackTrace);
			Console.WriteLine("===================================\n");
			Console.ResetColor();
		}
	}

	private static void EnableVirtualTerminalProcessing()
	{
		try
		{
			IntPtr stdHandle = GetStdHandle(-11);
			if (GetConsoleMode(stdHandle, out var lpMode))
			{
				lpMode |= 4;
				SetConsoleMode(stdHandle, lpMode);
			}
		}
		catch
		{
			Log("System", "Failed to enable VT processing", LogLevel.Warning);
		}
	}

	private static ConsoleColor GetCategoryColor(string category)
	{
		if (category.StartsWith("System", StringComparison.OrdinalIgnoreCase))
		{
			return ConsoleColor.Cyan;
		}
		if (category.StartsWith("Network", StringComparison.OrdinalIgnoreCase))
		{
			return ConsoleColor.Magenta;
		}
		if (category.StartsWith("UI", StringComparison.OrdinalIgnoreCase))
		{
			return ConsoleColor.Green;
		}
		return ConsoleColor.White;
	}

	private static ConsoleColor GetLevelColor(LogLevel level)
	{
		return level switch
		{
			LogLevel.Debug => ConsoleColor.Blue, 
			LogLevel.Warning => ConsoleColor.Yellow, 
			LogLevel.Error => ConsoleColor.Red, 
			_ => ConsoleColor.Gray, 
		};
	}

	private static void DisplayBanner()
	{
		Console.ForegroundColor = ConsoleColor.DarkCyan;
		LogWithGradient("\r\n                    /================================================================================\\\r\n                    ||                                                                              ||\r\n                    ||                                                                              ||\r\n                    ||                                                                              ||\r\n                    ||                                                                              ||\r\n                    ||                                                                              ||\r\n                    ||                 ______    _______   __   __    __  .___  ___.                ||\r\n                    ||                /  __  \\  |       \\ |  | |  |  |  | |   \\/   |                ||\r\n                    ||               |  |  |  | |  .--.  ||  | |  |  |  | |  \\  /  |                ||\r\n                    ||               |  |  |  | |  |  |  ||  | |  |  |  | |  |\\/|  |                ||\r\n                    ||               |  `--'  | |  '--'  ||  | |  `--'  | |  |  |  |                ||\r\n                    ||                \\______/  |_______/ |__|  \\______/  |__|  |__|                ||\r\n                    ||                                                                              ||\r\n                    ||                                                                              ||\r\n                    ||                                                                              ||\r\n                    ||                                                                              ||\r\n                    ||                                                                              ||\r\n                    \\================================================================================/\r\n                         ", (R: 255, G: 192, B: 203), (R: 255, G: 20, B: 147));
		Console.ResetColor();
		LogWithGradient($"                    Odium Console - {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n", (R: 255, G: 192, B: 203), (R: 255, G: 20, B: 147));
	}
}
