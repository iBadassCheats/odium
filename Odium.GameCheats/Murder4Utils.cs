using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Odium.GameCheats;

internal static class Murder4Utils
{
	private static bool _knifeShieldActive;

	public static bool KnifeShieldActive
	{
		get
		{
			return _knifeShieldActive;
		}
		set
		{
			_knifeShieldActive = value;
		}
	}

	private static void ExecuteDoorAction(string action)
	{
		(from go in Object.FindObjectsOfType<GameObject>()
			where go.name.StartsWith("Door")
			select go).ToList().ForEach(delegate(GameObject door)
		{
			door.transform.Find("Door Anim/Hinge/Interact " + action)?.GetComponent<UdonBehaviour>()?.Interact();
		});
	}

	public static void OpenDoors()
	{
		ExecuteDoorAction("open");
	}

	public static void CloseDoors()
	{
		ExecuteDoorAction("close");
		LockDoors();
	}

	public static void LockDoors()
	{
		ExecuteDoorAction("lock");
	}

	public static void ForceOpenDoors()
	{
		(from go in Object.FindObjectsOfType<GameObject>()
			where go.name.StartsWith("Door")
			select go).ToList().ForEach(delegate(GameObject door)
		{
			UdonBehaviour udonBehaviour = door.transform.Find("Door Anim/Hinge/Interact shove")?.GetComponent<UdonBehaviour>();
			for (int i = 0; i < 4; i++)
			{
				udonBehaviour?.Interact();
			}
		});
		OpenDoors();
	}

	private static void SendGameEvent(string eventName)
	{
		GameObject.Find("Game Logic")?.GetComponent<UdonBehaviour>()?.SendCustomNetworkEvent(NetworkEventTarget.All, eventName);
	}

	private static void SendWeaponEvent(string weaponPath, string eventName)
	{
		GameObject.Find(weaponPath)?.GetComponent<UdonBehaviour>()?.SendCustomNetworkEvent(NetworkEventTarget.All, eventName);
	}

	private static void SendPatreonEvent(string eventName)
	{
		GameObject.Find("Patreon Credits")?.GetComponent<UdonBehaviour>()?.SendCustomNetworkEvent(NetworkEventTarget.All, eventName);
	}

	public static void StartGame()
	{
		SendGameEvent("Btn_Start");
		SendGameEvent("SyncStartGame");
	}

	public static void AbortGame()
	{
		SendGameEvent("SyncAbort");
	}

	public static void RefreshRoles()
	{
		SendGameEvent("OnLocalPlayerAssignedRole");
	}

	public static void TriggerBystanderWin()
	{
		SendGameEvent("SyncVictoryB");
	}

	public static void TriggerMurdererWin()
	{
		SendGameEvent("SyncVictoryM");
	}

	public static void ExecuteAll()
	{
		SendGameEvent("KillLocalPlayer");
	}

	public static void BlindAll()
	{
		SendGameEvent("OnLocalPlayerBlinded");
	}

	public static void FlashAll()
	{
		SendGameEvent("OnLocalPlayerFlashbanged");
	}

	private static void TeleportWeapon(string path)
	{
		GameObject gameObject = GameObject.Find(path);
		if (!(gameObject == null))
		{
			Networking.SetOwner(Networking.LocalPlayer, gameObject);
			gameObject.transform.position = Networking.LocalPlayer.gameObject.transform.position + Vector3.up * 0.1f;
		}
	}

	public static void GetRevolver()
	{
		TeleportWeapon("Game Logic/Weapons/Revolver");
	}

	public static void GetShotgun()
	{
		TeleportWeapon("Game Logic/Weapons/Unlockables/Shotgun (0)");
	}

	public static void GetLuger()
	{
		TeleportWeapon("Game Logic/Weapons/Unlockables/Luger (0)");
	}

	public static void GetSmoke()
	{
		TeleportWeapon("Game Logic/Weapons/Unlockables/Smoke (0)");
	}

	public static void GetCamera()
	{
		TeleportWeapon("Game Logic/Polaroids Unlock Camera/FlashCamera");
	}

	public static void GetTraps()
	{
		Enumerable.Range(0, 3).ToList().ForEach(delegate(int i)
		{
			TeleportWeapon($"Game Logic/Weapons/Bear Trap ({i})");
		});
	}

	public static void DeployFrag(VRCPlayer target, bool detonate = false)
	{
		GameObject gameObject = GameObject.Find("Game Logic/Weapons/Unlockables/Frag (0)");
		if (gameObject != null)
		{
			Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject);
			gameObject.transform.position = target.transform.position + Vector3.up * 0.1f;
			if (detonate)
			{
				DetonateFrag();
			}
		}
	}

	public static IEnumerator CreateKnifeShield(VRCPlayer player)
	{
		List<GameObject> knives = (from num in Enumerable.Range(0, 6)
			select GameObject.Find($"Game Logic/Weapons/Knife ({num})")).ToList();
		GameObject gameObject = new GameObject();
		gameObject.transform.position = player.transform.position + Vector3.up * 0.35f;
		GameObject shield = gameObject;
		while (_knifeShieldActive)
		{
			shield.transform.SetPositionAndRotation(player.transform.position + Vector3.up * 0.35f, Quaternion.Euler(0f, 360f * Time.time, 0f));
			for (int i = 0; i < knives.Count; i++)
			{
				Networking.LocalPlayer.TakeOwnership(knives[i]);
				knives[i].transform.SetPositionAndRotation(shield.transform.position + shield.transform.forward, Quaternion.LookRotation(player.transform.position - knives[i].transform.position));
				shield.transform.Rotate(0f, 360f / (float)knives.Count, 0f);
			}
			yield return null;
		}
		Object.Destroy(shield);
	}

	public static void FireShotgun()
	{
		SendWeaponEvent("Game Logic/Weapons/Unlockables/Shotgun (0)", "Fire");
	}

	public static void FireRevolver()
	{
		SendWeaponEvent("Game Logic/Weapons/Revolver", "Fire");
	}

	public static void FireLuger()
	{
		SendWeaponEvent("Game Logic/Weapons/Unlockables/Luger (0)", "Fire");
	}

	public static void DetonateFrag()
	{
		SendWeaponEvent("Game Logic/Weapons/Unlockables/Frag (0)", "Explode");
	}

	public static void ApplyRevolverSkin()
	{
		SendWeaponEvent("Game Logic/Weapons/Revolver", "PatronSkin");
	}

	public static void SpawnSnake()
	{
		SendWeaponEvent("Game Logic/Snakes/SnakeDispenser", "DispenseSnake");
	}

	public static void IdentifyMurderer()
	{
		string text = (Resources.FindObjectsOfTypeAll<Transform>().FirstOrDefault((Transform t) => t.gameObject.name == "Murderer Name")?.gameObject)?.GetComponent<TextMeshProUGUI>()?.text + ", Is the murder.";
	}

	public static void ExplodeAtTarget(Player target)
	{
		GameObject gameObject = GameObject.Find("Frag (0)");
		Networking.LocalPlayer.TakeOwnership(gameObject);
		gameObject.transform.position = target.transform.position;
		gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(NetworkEventTarget.All, "Explode");
	}

	public static void BlindTarget(Player target)
	{
		SendTargetedEvent(target, "SyncFlashbang");
	}

	public static void AssignRole(string username, string role)
	{
		for (int i = 0; i < 24; i++)
		{
			string text = GameObject.Find($"Game Logic/Game Canvas/Game In Progress/Player List/Player List Group/Player Entry ({i})/Player Name Text").GetComponent<TextMeshProUGUI>().text;
			if (text == username)
			{
				GameObject.Find($"Player Node ({i})").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(NetworkEventTarget.All, "SyncAssignM");
				break;
			}
		}
	}

	public static IEnumerator InitializeTheme()
	{
		while (!GameObject.Find("Game Logic/Game Canvas"))
		{
			yield return null;
		}
		SetRedText("Game Logic/Game Canvas/Pregame/Title Text", "HABIBI 4");
		SetRedText("Game Logic/Game Canvas/Pregame/Author Text", "By Osama");
		GameObject.Find("Game Logic/Game Canvas/Background Panel Border").GetComponent<Image>().color = Color.red;
		GameObject.Find("Game Logic/Player HUD/Death HUD Anim").SetActive(value: false);
		GameObject.Find("Game Logic/Player HUD/Blind HUD Anim").SetActive(value: false);
		static void SetRedText(string path, string text)
		{
			TextMeshProUGUI component = GameObject.Find(path).GetComponent<TextMeshProUGUI>();
			component.text = text;
			component.color = Color.red;
		}
	}

	public static void SendTargetedPatreonEvent(Player target, string eventName)
	{
		GameObject gameObject = GameObject.Find("Patreon Credits");
		gameObject.GetComponent<UdonBehaviour>().enabled = true;
		gameObject.SendUdon(eventName, target);
		gameObject.GetComponent<UdonBehaviour>().enabled = false;
	}

	public static void SendTargetedEvent(Player target, string eventName)
	{
		GameObject go = GameObject.Find("Game Logic");
		go.SendUdon(eventName, target);
	}
}
