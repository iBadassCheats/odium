using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Odium.Components;

public struct NameplateData
{
	public string userId;

	public List<TextMeshProUGUI> statsComponents;

	public List<Transform> tagPlates;

	public float lastSeen;

	public Vector3 lastPosition;

	public string platform;

	public List<string> userTags;

	public bool isClientUser;
}
