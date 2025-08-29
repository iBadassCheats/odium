using VRC.Localization;

namespace Odium.Wrappers;

internal class LocalizableStringWrapper
{
	public static LocalizableString Create(string str)
	{
		LocalizableString localizableString = new LocalizableString();
		localizableString._localizationKey = str;
		return localizableString;
	}
}
