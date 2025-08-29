using System;

namespace Odium.Odium;

[Serializable]
public class BanCheckResponse
{
	public bool success;

	public string message;

	public bool isBanned;

	public UserInfo user;
}
