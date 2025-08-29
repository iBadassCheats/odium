using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using Odium.Components;
using Odium.Odium;
using Odium.UX;
using Odium.Wrappers;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace Odium.ApplicationBot;

public class Bot
{
	private const float MoveSpeed = 0.1f;

	public static bool voiceMimic = false;

	public static bool movementMimic = false;

	public static int voiceMimicActorNr = 0;

	public static int movementMimicActorNr = 0;

	public static bool chatBoxLagger = false;

	private static float BotOrbitOffset = 0f;

	private static Dictionary<string, Action<string, string>> Commands = new Dictionary<string, Action<string, string>>
	{
		{
			"JoinWorld",
			delegate(string WorldID, string botId)
			{
				Console.WriteLine("[Client] Joining World " + WorldID);
				if (botId == BotId && Current_World != null)
				{
					Networking.GoToRoom(WorldID);
				}
			}
		},
		{
			"ToggleBlockAll",
			delegate(string UserID, string botId)
			{
				if (botId == BotId)
				{
					foreach (Player player in PlayerWrapper.Players)
					{
						if (player.field_Private_APIUser_0.id != UserID)
						{
							player.Method_Public_Void_Boolean_0(UserID != string.Empty);
						}
					}
				}
			}
		},
		{
			"OrbitSelected",
			delegate(string UserID, string botId)
			{
				if (botId == BotId)
				{
					OrbitTarget = ((UserID == string.Empty) ? null : PlayerWrapper.GetVRCPlayerFromId(UserID)._player);
				}
			}
		},
		{
			"PortalSpam",
			delegate(string UserID, string botId)
			{
				if (botId == BotId)
				{
					if (!voiceMimic)
					{
						ActionWrapper.portalSpamPlayer = PlayerWrapper.GetVRCPlayerFromId(UserID)._player;
						voiceMimic = true;
					}
					else
					{
						ActionWrapper.portalSpamPlayer = null;
						voiceMimic = true;
					}
				}
			}
		},
		{
			"MovementMimic",
			delegate(string UserID, string botId)
			{
				if (botId == BotId)
				{
					OdiumConsole.Log("OdiumBot", $"Movement mimic called for actor -> {movementMimicActorNr} ({UserID})");
					if (!movementMimic)
					{
						movementMimic = true;
						movementMimicActorNr = PlayerWrapper.GetVRCPlayerFromId(UserID).prop_Player_0.prop_Int32_0;
						OdiumConsole.Log("OdiumBot", $"Movement mimic enabled for actor -> {movementMimicActorNr} ({UserID})");
					}
					else
					{
						movementMimicActorNr = 0;
						movementMimic = false;
					}
				}
			}
		},
		{
			"ChatBoxLagger",
			delegate(string boolean, string botId)
			{
				if (botId == BotId)
				{
					if (!AssignedVariables.chatboxLagger)
					{
						InternalConsole.LogIntoConsole("Chatbox lagger was enabled!");
						AssignedVariables.chatboxLagger = true;
						chatboxLaggerCoroutine = MelonCoroutines.Start(OptimizedChatboxLaggerCoroutine());
					}
					else
					{
						InternalConsole.LogIntoConsole("Chatbox lagger was disabled!");
						AssignedVariables.chatboxLagger = false;
						if (chatboxLaggerCoroutine != null)
						{
							MelonCoroutines.Stop(chatboxLaggerCoroutine);
							chatboxLaggerCoroutine = null;
						}
						preGeneratedMessages.Clear();
					}
				}
			}
		},
		{
			"ClickTP",
			delegate(string Position, string botId)
			{
				if (PlayerWrapper.LocalPlayer != null)
				{
					string[] array = Position.Split(':');
					float x = float.Parse(array[0]);
					float y = float.Parse(array[1]);
					float z = float.Parse(array[2]);
					PlayerWrapper.LocalPlayer.transform.position = new Vector3(x, y, z);
				}
			}
		},
		{
			"TeleportToPlayer",
			delegate(string UserID, string botId)
			{
				if (botId == BotId && PlayerWrapper.LocalPlayer != null)
				{
					Networking.LocalPlayer.TeleportTo(PlayerWrapper.GetVRCPlayerFromId(UserID)._player.transform.position, PlayerWrapper.GetVRCPlayerFromId(UserID)._player.transform.rotation);
				}
			}
		},
		{
			"USpeakSpam",
			delegate(string Enabled, string botId)
			{
				USpeakSpam.ToggleUSpeakSpam(bool.Parse(Enabled));
			}
		},
		{
			"SpinbotToggle",
			delegate(string Enabled, string botId)
			{
				if (botId == BotId)
				{
					Spinbot = Enabled != string.Empty;
				}
			}
		},
		{
			"SpinbotSpeed",
			delegate(string Speed, string botId)
			{
				SpinbotSpeed = int.Parse(Speed);
			}
		},
		{
			"SetTargetFramerate",
			delegate(string Framerate, string botId)
			{
				if (int.TryParse(Framerate, out var result))
				{
					Application.targetFrameRate = result;
				}
			}
		},
		{
			"SetOrbitOffset",
			delegate(string Offset, string botId)
			{
				if (botId == BotId && float.TryParse(Offset, out var result))
				{
					BotOrbitOffset = result;
				}
			}
		}
	};

	private static List<string> preGeneratedMessages = new List<string>();

	private static System.Random random = new System.Random();

	private static object chatboxLaggerCoroutine = null;

	private static Player OrbitTarget;

	private static Action LastActionOnMainThread;

	private static bool EventCachingDC = false;

	private static bool Spinbot = false;

	private static int SpinbotSpeed = 20;

	public static float OrbitSpeed = 5f;

	public static float alpha = 0f;

	public static float a = 1f;

	public static float b = 1f;

	public static float Range = 1f;

	public static float Height = 0f;

	public static VRCPlayer currentPlayer;

	public static Player selectedPlayer;

	public static Player LagTarget;

	public static bool IsApplicationBot = false;

	public static int BotProfile { get; set; }

	public static string BotId { get; set; }

	public string SessionId { get; set; }

	public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

	public DateTime LastPing { get; set; } = DateTime.UtcNow;

	public static bool IsHeadlessBot { get; set; }

	public bool IsAlive { get; set; } = true;

	public static ApiWorld Current_World => RoomManager.field_Internal_Static_ApiWorld_0;

	private static void MovePlayer(Vector3 pos)
	{
		if (PlayerWrapper.LocalPlayer != null)
		{
			PlayerWrapper.LocalPlayer.transform.position += pos;
		}
	}

	public static void ReceiveCommand(string Command)
	{
		int num = Command.IndexOf(" ");
		string CMD = Command.Substring(0, num);
		string text = Command.Substring(num + 1);
		int num2 = text.IndexOf(" ");
		if (num2 == -1)
		{
			return;
		}
		string Parameters = text.Substring(0, num2);
		string Parameters2 = text.Substring(num2 + 1);
		if (Parameters2.Contains(BotId))
		{
			HandleActionOnMainThread(delegate
			{
				Commands[CMD](Parameters, Parameters2);
			});
		}
	}

	private static void HandleActionOnMainThread(Action action)
	{
		LastActionOnMainThread = action;
	}

	private static void PreGenerateMessages(int count)
	{
		for (int i = 0; i < count; i++)
		{
			char[] array = new char[144];
			for (int j = 0; j < 144; j++)
			{
				array[j] = (char)random.Next(19968, 40960);
			}
			preGeneratedMessages.Add(new string(array));
		}
	}

	private static IEnumerator OptimizedChatboxLaggerCoroutine()
	{
		while (AssignedVariables.chatboxLagger)
		{
			if (preGeneratedMessages.Count == 0)
			{
				PreGenerateMessages(10);
			}
			Chatbox.SendCustomChatMessage(preGeneratedMessages[0]);
			preGeneratedMessages.RemoveAt(0);
			yield return new WaitForSecondsRealtime(0.5f);
		}
	}

	public static void OnUpdate()
	{
		if (IsApplicationBot)
		{
			if (LastActionOnMainThread != null)
			{
				LastActionOnMainThread();
				LastActionOnMainThread = null;
			}
			HandleBotFunctions();
		}
	}

	private static IEnumerator RamClearLoop()
	{
		while (true)
		{
			yield return new WaitForSeconds(5f);
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
		}
	}

	private static void HandleBotFunctions()
	{
		if (OrbitTarget != null && PlayerWrapper.LocalPlayer != null)
		{
			Physics.gravity = new Vector3(0f, 0f, 0f);
			alpha += Time.deltaTime * OrbitSpeed;
			float num = alpha + BotOrbitOffset;
			PlayerWrapper.LocalPlayer.transform.position = new Vector3(OrbitTarget.transform.position.x + a * (float)Math.Cos(num), OrbitTarget.transform.position.y + Height, OrbitTarget.transform.position.z + b * (float)Math.Sin(num));
		}
		if (Spinbot && PlayerWrapper.LocalPlayer != null)
		{
			PlayerWrapper.LocalPlayer.transform.Rotate(new Vector3(0f, SpinbotSpeed, 0f));
		}
	}

	public static void Start()
	{
		if (IsLaunchedAsBot())
		{
			IsApplicationBot = true;
			GenerateUniqueOrbitOffset();
			OdiumConsole.LogGradient("Odium", $"Running as Application Bot with assigned ID: {BotId}, Orbit Offset: {BotOrbitOffset:F2}");
			SocketConnection.Client();
			RamClearLoop();
			MelonCoroutines.Start(WaitForWorldJoin());
		}
		else
		{
			OdiumConsole.LogGradient("Odium", "Starting bot server...");
			SocketConnection.StartServer();
		}
	}

	private static void GenerateUniqueOrbitOffset()
	{
		if (!string.IsNullOrEmpty(BotId))
		{
			int hashCode = BotId.GetHashCode();
			BotOrbitOffset = (float)(hashCode % 360) * ((float)Math.PI / 180f);
		}
		else if (BotProfile > 0)
		{
			BotOrbitOffset = (float)BotProfile * 60f * ((float)Math.PI / 180f);
		}
		else
		{
			BotOrbitOffset = (float)(new System.Random().NextDouble() * 2.0 * Math.PI);
		}
	}

	public static bool IsLaunchedAsBot()
	{
		try
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			bool flag = commandLineArgs.Any((string arg) => arg.ToLower() == "--appbot");
			if (flag)
			{
				MelonLogger.Log("Found --appBot launch parameter");
				ExtractLaunchParameters(commandLineArgs);
			}
			return flag;
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error checking launch parameters: " + ex.Message);
			return false;
		}
	}

	public static IEnumerator WaitForWorldJoin()
	{
		while (RoomManager.field_Private_Static_ApiWorldInstance_0 == null)
		{
			yield return new WaitForSeconds(0.5f);
		}
		try
		{
			string worldId = Current_World?.id ?? "unknown";
			string message = "WORLD_JOINED:" + PlayerWrapper.LocalPlayer.field_Private_APIUser_0.displayName + ":" + RoomManager.field_Internal_Static_ApiWorld_0.name;
			SocketConnection.SendMessageToServer(message);
			OdiumConsole.LogGradient("OdiumBot", "Notified server that bot " + BotId + " joined world " + worldId);
			string statusMessage = "BOT_STATUS:" + BotId + ":READY";
			SocketConnection.SendMessageToServer(statusMessage);
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			OdiumConsole.LogException(ex2);
		}
	}

	public static void NotifyWorldLeave()
	{
		try
		{
			string message = "WORLD_LEFT:" + BotId;
			SocketConnection.SendMessageToServer(message);
			OdiumConsole.LogGradient("OdiumBot", "Notified server that bot " + BotId + " left world");
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex);
		}
	}

	public static void ExtractLaunchParameters(string[] args)
	{
		try
		{
			string text = args.FirstOrDefault((string arg) => arg.StartsWith("--profile="));
			if (text != null)
			{
				string s = text.Split('=')[1];
				if (int.TryParse(s, out var result))
				{
					BotProfile = result;
					MelonLogger.Log($"Bot Profile: {result}");
				}
			}
			string text2 = args.FirstOrDefault((string arg) => arg.StartsWith("--id="));
			if (text2 != null)
			{
				BotId = text2.Split('=')[1];
				MelonLogger.Log("Bot ID: " + BotId);
			}
			if (args.Any((string arg) => arg.ToLower() == "-batchmode"))
			{
				IsHeadlessBot = true;
				MelonLogger.Log("Running in headless mode");
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error extracting launch parameters: " + ex.Message);
		}
	}
}
