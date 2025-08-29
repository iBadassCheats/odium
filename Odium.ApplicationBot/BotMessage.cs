using System;

namespace Odium.ApplicationBot;

public class BotMessage
{
	public string MessageId { get; set; } = Guid.NewGuid().ToString();

	public string Command { get; set; }

	public string TargetBotId { get; set; }

	public DateTime Timestamp { get; set; } = DateTime.UtcNow;

	public object Parameters { get; set; }
}
