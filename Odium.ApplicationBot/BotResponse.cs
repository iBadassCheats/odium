using System;

namespace Odium.ApplicationBot;

public class BotResponse
{
	public string MessageId { get; set; }

	public string BotId { get; set; }

	public bool Success { get; set; }

	public string Error { get; set; }

	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
