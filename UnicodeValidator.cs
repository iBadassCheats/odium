public static class UnicodeValidator
{
	public static bool Sanitize(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return false;
		}
		foreach (char c in input)
		{
			if (c > '\u007f')
			{
				return true;
			}
			if (char.IsControl(c) && !IsAllowedWhitespace(c))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsAllowedWhitespace(char c)
	{
		return c == ' ' || c == '\t' || c == '\r' || c == '\n';
	}
}
