using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Odium;

public class MediaControls
{
	private static readonly HttpClient httpClient = new HttpClient();

	private static bool isSpotifyPlaying = false;

	private static bool isDiscordMuted = false;

	private static bool isDiscordDeafened = false;

	private const int KEYEVENTF_KEYUP = 2;

	private const byte VK_MEDIA_PLAY_PAUSE = 179;

	private const byte VK_MEDIA_PREV_TRACK = 177;

	private const byte VK_MEDIA_NEXT_TRACK = 176;

	[DllImport("user32.dll")]
	private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

	public static async Task ToggleDiscordMute()
	{
		SendDiscordHotkey("^+m");
		isDiscordMuted = !isDiscordMuted;
		OdiumConsole.Log("Discord", "Microphone " + (isDiscordMuted ? "muted" : "unmuted"));
	}

	public static async Task ToggleDiscordDeafen()
	{
		SendDiscordHotkey("^+d");
		isDiscordDeafened = !isDiscordDeafened;
		OdiumConsole.Log("Discord", "Audio " + (isDiscordDeafened ? "deafened" : "undeafened"));
	}

	public static void SendDiscordHotkey(string hotkey)
	{
		Process[] processesByName = Process.GetProcessesByName("Discord");
		if (processesByName.Length != 0)
		{
			SetForegroundWindow(processesByName[0].MainWindowHandle);
			Thread.Sleep(100);
			SendKeys.SendWait(hotkey);
		}
	}

	[DllImport("user32.dll")]
	private static extern bool SetForegroundWindow(IntPtr hWnd);

	public static async Task ToggleSpotifyPlayback()
	{
		try
		{
			await SpotifyWebAPIToggle();
		}
		catch
		{
			keybd_event(179, 0, 0u, UIntPtr.Zero);
			keybd_event(179, 0, 2u, UIntPtr.Zero);
		}
		isSpotifyPlaying = !isSpotifyPlaying;
		OdiumConsole.Log("Spotify", "Playback " + (isSpotifyPlaying ? "resumed" : "paused"));
	}

	public static async Task SpotifyWebAPIToggle()
	{
		string accessToken = GetSpotifyAccessToken();
		if (!string.IsNullOrEmpty(accessToken))
		{
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			string action = (isSpotifyPlaying ? "pause" : "play");
			await httpClient.PutAsync("https://api.spotify.com/v1/me/player/" + action, null);
		}
	}

	public static void SpotifyRewind()
	{
		keybd_event(177, 0, 0u, UIntPtr.Zero);
		keybd_event(177, 0, 2u, UIntPtr.Zero);
		OdiumConsole.Log("Spotify", "Previous track");
	}

	public static void SpotifySkip()
	{
		keybd_event(176, 0, 0u, UIntPtr.Zero);
		keybd_event(176, 0, 176u, UIntPtr.Zero);
		OdiumConsole.Log("Spotify", "Next track");
	}

	public static void SpotifyPause()
	{
		keybd_event(179, 0, 0u, UIntPtr.Zero);
		keybd_event(179, 0, 2u, UIntPtr.Zero);
		OdiumConsole.Log("Spotify", "Paused");
	}

	private static string GetSpotifyAccessToken()
	{
		return null;
	}
}
