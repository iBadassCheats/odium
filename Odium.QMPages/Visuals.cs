using Odium.ButtonAPI.QM;
using Odium.Components;
using UnityEngine;

namespace Odium.QMPages;

internal class Visuals
{
	public static void InitializePage(QMNestedMenu movementButton, Sprite buttonImage)
	{
		new QMToggleButton(movementButton, 2f, 0f, "Bone ESP", delegate
		{
			BoneESP.SetEnabled(enabled: true);
		}, delegate
		{
			BoneESP.SetEnabled(enabled: false);
		}, "Toggle Flight Mode", defaultState: false, buttonImage);
		new QMToggleButton(movementButton, 3f, 0f, "Box ESP", delegate
		{
			BoxESP.SetEnabled(enabled: true);
		}, delegate
		{
			BoxESP.SetEnabled(enabled: false);
		}, "Toggle Flight Mode", defaultState: false, buttonImage);
	}
}
