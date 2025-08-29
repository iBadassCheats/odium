using System.IO;

namespace Odium.Components;

public class FileHelper
{
	public static bool IsPath(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return false;
		}
		try
		{
			if (Path.IsPathRooted(input))
			{
				return true;
			}
			if (input.Length >= 2 && char.IsLetter(input[0]) && input[1] == ':')
			{
				return true;
			}
			if (input.StartsWith("\\\\") && input.Length > 2)
			{
				return true;
			}
			if (input.StartsWith("/"))
			{
				return true;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}
}
