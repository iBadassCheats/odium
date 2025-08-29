using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Odium.Modules;
using Odium.Threadding;

namespace Odium.ApplicationBot;

internal class SocketConnection
{
	private static readonly int botCount = 8;

	private static Socket clientSocket;

	private static List<Socket> ServerHandlers = new List<Socket>();

	public static void SendCommandToClients(string Command)
	{
		OdiumConsole.LogGradient("BotServer", $"[{DateTime.Now}] Sending Message ({Command})");
		ServerHandlers.Where((Socket s) => s != null).ToList().ForEach(delegate(Socket s)
		{
			s.Send(Encoding.ASCII.GetBytes(Command));
		});
	}

	public static void SendMessageToServer(string message)
	{
		if (clientSocket != null && clientSocket.Connected)
		{
			try
			{
				byte[] bytes = Encoding.ASCII.GetBytes(message);
				clientSocket.Send(bytes);
				OdiumConsole.LogGradient("BotClient", $"[{DateTime.Now}] Sent Message to Server ({message})");
			}
			catch (Exception ex)
			{
				OdiumConsole.LogException(ex);
			}
		}
	}

	public static void OnClientReceiveCommand(string Command)
	{
		OdiumConsole.LogGradient("BotServer", $"[{DateTime.Now}] Received Message ({Command})");
		Bot.ReceiveCommand(Command);
	}

	public static void OnServerReceiveMessage(string message, Socket clientSocket)
	{
		OdiumConsole.LogGradient("BotServer", $"[{DateTime.Now}] Received from Bot ({message})");
		if (message.StartsWith("WORLD_JOINED:"))
		{
			string[] array = message.Split(':');
			if (array.Length >= 3)
			{
				string botName = array[1];
				string worldName = array[2];
				OdiumConsole.LogGradient("BotServer", "Bot " + botName + " joined world " + worldName);
				MainThreadDispatcher.Enqueue(delegate
				{
					OdiumBottomNotification.ShowNotification("[<color=#7A00FE>Bot</color>] <color=#FC7C93>" + botName + "</color> joined <color=#00FE9C>" + worldName + "</color>");
				});
			}
		}
		else if (message.StartsWith("BOT_STATUS:"))
		{
			string[] array2 = message.Split(':');
			if (array2.Length >= 3)
			{
				string text = array2[1];
				string text2 = array2[2];
				OdiumConsole.LogGradient("BotServer", "Bot " + text + " status: " + text2);
			}
		}
	}

	public static void StartServer()
	{
		ServerHandlers.Clear();
		Task.Run((Action)HandleServer);
	}

	private static void HandleServer()
	{
		IPHostEntry hostEntry = Dns.GetHostEntry("localhost");
		IPAddress iPAddress = hostEntry.AddressList[0];
		IPEndPoint localEP = new IPEndPoint(iPAddress, 11000);
		try
		{
			Socket socket = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socket.Bind(localEP);
			socket.Listen(10);
			for (int i = 0; i < botCount; i++)
			{
				Socket handler = socket.Accept();
				ServerHandlers.Add(handler);
				Task.Run(delegate
				{
					HandleClientMessages(handler);
				});
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.LogException(ex);
		}
	}

	private static void HandleClientMessages(Socket clientHandler)
	{
		byte[] array = new byte[1024];
		try
		{
			while (clientHandler.Connected)
			{
				int num = clientHandler.Receive(array);
				if (num > 0)
				{
					string message = Encoding.ASCII.GetString(array, 0, num);
					OnServerReceiveMessage(message, clientHandler);
				}
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.LogGradient("BotServer", "Client disconnected or error: " + ex.Message);
		}
	}

	public static void Client()
	{
		Task.Run((Action)HandleClient);
	}

	private static void HandleClient()
	{
		byte[] array = new byte[1024];
		OdiumConsole.LogGradient("BotServer", "Connecting to server!");
		try
		{
			IPHostEntry hostEntry = Dns.GetHostEntry("localhost");
			IPAddress iPAddress = hostEntry.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(iPAddress, 11000);
			clientSocket = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				clientSocket.Connect(remoteEP);
				OdiumConsole.LogGradient("BotServer", "Socket connected to " + clientSocket.RemoteEndPoint.ToString());
				while (true)
				{
					int num = clientSocket.Receive(array);
					if (num > 0)
					{
						OnClientReceiveCommand(Encoding.ASCII.GetString(array, 0, num));
					}
				}
			}
			catch (ArgumentNullException ex)
			{
				OdiumConsole.LogException(ex);
			}
			catch (SocketException ex2)
			{
				OdiumConsole.LogException(ex2);
			}
			catch (Exception ex3)
			{
				OdiumConsole.LogException(ex3);
			}
		}
		catch (Exception ex4)
		{
			OdiumConsole.LogException(ex4);
		}
	}
}
