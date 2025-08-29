using System;
using System.Collections.Generic;

namespace Odium.Components;

[Serializable]
public class TagResponse
{
	public bool success;

	public string userId;

	public List<string> tags;
}
