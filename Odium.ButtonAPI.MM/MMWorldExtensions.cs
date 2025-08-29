using System;
using UnityEngine;

namespace Odium.ButtonAPI.MM;

public static class MMWorldExtensions
{
	public static MMWorldButton AddButton(this MMWorldActionRow row, string text, Action action = null, Sprite icon = null)
	{
		return new MMWorldButton(row, text, action, icon);
	}
}
