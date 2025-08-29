using UnityEngine;

namespace Odium.ButtonAPI.QM;

public class CoroutineStarter : MonoBehaviour
{
	private static CoroutineStarter _instance;

	public static CoroutineStarter Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("CoroutineStarter");
				_instance = gameObject.AddComponent<CoroutineStarter>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}
	}
}
