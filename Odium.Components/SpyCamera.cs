using UnityEngine;
using VRC.SDKBase;

namespace Odium.Components;

public class SpyCamera : MonoBehaviour
{
	public static SpyCamera Instance;

	public static Camera _spyCam;

	public static VRCPlayerApi _targetPlayer;

	public static bool _isActive;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.DestroyImmediate(this);
		}
	}

	public static void Toggle(VRCPlayerApi player, bool state)
	{
		if (state)
		{
			EnableSpyCamera(player);
		}
		else
		{
			DisableSpyCamera();
		}
	}

	private static void EnableSpyCamera(VRCPlayerApi player)
	{
		if (!_isActive && player != null)
		{
			_targetPlayer = player;
			GameObject gameObject = new GameObject("SpyCamera");
			gameObject.transform.SetParent(_targetPlayer.GetBoneTransform(HumanBodyBones.Head));
			_spyCam = gameObject.AddComponent<Camera>();
			_spyCam.fieldOfView = 60f;
			_spyCam.nearClipPlane = 0.1f;
			_spyCam.farClipPlane = 1000f;
			_spyCam.depth = 1f;
			_isActive = true;
		}
	}

	private static void DisableSpyCamera()
	{
		if (_isActive)
		{
			if (_spyCam != null)
			{
				Object.Destroy(_spyCam.gameObject);
				_spyCam = null;
			}
			_targetPlayer = null;
			_isActive = false;
		}
	}

	public static void LateUpdate()
	{
		if (_isActive && _targetPlayer != null && _targetPlayer.IsValid())
		{
			VRCPlayerApi.TrackingData trackingData = _targetPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
			if (_spyCam != null)
			{
				_spyCam.transform.position = trackingData.position;
				_spyCam.transform.rotation = trackingData.rotation;
			}
		}
	}

	public static void OnDestroy()
	{
		DisableSpyCamera();
	}
}
