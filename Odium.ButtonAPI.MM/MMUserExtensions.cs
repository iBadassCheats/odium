using System;
using UnityEngine;

namespace Odium.ButtonAPI.MM;

public static class MMUserExtensions
{
	public static MMUserButton AddButton(this MMUserActionRow row, string text, Action action = null, Sprite icon = null)
	{
		return new MMUserButton(row, text, action, icon);
	}
}
