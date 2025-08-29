using System;
using Odium.ButtonAPI.QM;
using Odium.QMPages;
using UnityEngine;
using VRC;
using VRC.SDKBase;

namespace Odium.Components;

public class SwasticaOrbit
{
	public static Player _target;

	public static bool _blind = false;

	public static bool _instance = false;

	public static bool _itemOrbit;

	public static bool _swastika;

	public static GameObject _targetItem;

	internal static Vector3 _setLocation;

	public static float _swastikaSize = 45f;

	public static float _hasTakenOwner = 1999f;

	public static float _setMultiplier = 160f;

	public static float _rotateState;

	public static Vector3 _originalVelocity;

	public static bool _returnedValue;

	public static void Activated(Player player, bool state)
	{
		string selected_player_name = AppBot.get_selected_player_name();
		player = ApiUtils.GetPlayerByDisplayName(selected_player_name);
		if (state)
		{
			_target = player;
			_swastika = true;
		}
		else
		{
			_swastika = false;
			_target = null;
		}
	}

	public static void OnUpdate()
	{
		try
		{
			if (_swastika)
			{
				try
				{
					if (_target != null)
					{
						Vector3 bonePosition = _target.field_Private_VRCPlayerApi_0.GetBonePosition(HumanBodyBones.Head);
						bonePosition.Set(bonePosition.x, bonePosition.y + 2f, bonePosition.z);
						_setLocation = bonePosition;
					}
					if (Input.GetKeyDown(KeyCode.UpArrow))
					{
						_swastikaSize += 2f;
					}
					else if (Input.GetKeyDown(KeyCode.DownArrow))
					{
						_swastikaSize -= 2f;
					}
					if (_rotateState >= 360f)
					{
						_rotateState = Time.deltaTime;
					}
					else
					{
						_rotateState += Time.deltaTime;
					}
					if (_hasTakenOwner >= 90f)
					{
						_hasTakenOwner = 0f;
						for (int i = 0; i < OnLoadedSceneManager.sdk3Items.Length; i++)
						{
							VRC_Pickup vRC_Pickup = OnLoadedSceneManager.sdk3Items[i];
							Networking.SetOwner(Player.prop_Player_0.field_Private_VRCPlayerApi_0, vRC_Pickup.gameObject);
						}
					}
					else
					{
						_hasTakenOwner += 1f;
					}
					float num = Convert.ToInt16(OnLoadedSceneManager.sdk3Items.Length / 8);
					float num2 = (float)OnLoadedSceneManager.sdk3Items.Length / _swastikaSize;
					for (int j = 0; j < OnLoadedSceneManager.sdk3Items.Length; j++)
					{
						VRC_Pickup vRC_Pickup2 = OnLoadedSceneManager.sdk3Items[j];
						float num3 = j % 8;
						float num4 = j / 8;
						float num5 = num3;
						float num6 = num5;
						if (num6 != 6f)
						{
							if (num6 != 5f)
							{
								if (num6 != 4f)
								{
									if (num6 != 3f)
									{
										if (num6 != 2f)
										{
											if (num6 != 1f)
											{
												if (num6 != 0f)
												{
													vRC_Pickup2.transform.position = _setLocation + new Vector3((0f - Mathf.Cos(_rotateState)) * num2 * (num4 / num), num2, Mathf.Sin(_rotateState) * num2 * (num4 / num));
												}
												else
												{
													vRC_Pickup2.transform.position = _setLocation + new Vector3(0f, num2 * (num4 / num), 0f);
												}
											}
											else
											{
												vRC_Pickup2.transform.position = _setLocation + new Vector3(0f, (0f - num2) * (num4 / num), 0f);
											}
										}
										else
										{
											vRC_Pickup2.transform.position = _setLocation + new Vector3((0f - Mathf.Cos(_rotateState)) * num2 * (num4 / num), 0f, Mathf.Sin(_rotateState) * num2 * (num4 / num));
										}
									}
									else
									{
										vRC_Pickup2.transform.position = _setLocation + new Vector3((0f - Mathf.Cos(_rotateState + _setMultiplier)) * num2 * (num4 / num), 0f, Mathf.Sin(_rotateState + _setMultiplier) * num2 * (num4 / num));
									}
								}
								else
								{
									vRC_Pickup2.transform.position = _setLocation + new Vector3((0f - Mathf.Cos(_rotateState + _setMultiplier)) * num2, num2 * (num4 / num), Mathf.Sin(_rotateState + _setMultiplier) * num2);
								}
							}
							else
							{
								vRC_Pickup2.transform.position = _setLocation + new Vector3((0f - Mathf.Cos(_rotateState)) * num2, (0f - num2) * (num4 / num), Mathf.Sin(_rotateState) * num2);
							}
						}
						else
						{
							vRC_Pickup2.transform.position = _setLocation + new Vector3((0f - Mathf.Cos(_rotateState + _setMultiplier)) * num2 * (num4 / num), 0f - num2, Mathf.Sin(_rotateState + _setMultiplier) * (num2 * (num4 / num)));
						}
						Vector3 originalVelocity = _originalVelocity;
						if (false)
						{
							_originalVelocity = vRC_Pickup2.GetComponent<Rigidbody>().velocity;
						}
						_returnedValue = false;
						vRC_Pickup2.GetComponent<Rigidbody>().velocity = Vector3.zero;
						vRC_Pickup2.transform.rotation = Quaternion.Euler(0f, _rotateState * -90f, 0f);
					}
					return;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Module : SwasticaOrbit" + ex.Message);
					return;
				}
			}
			if (_returnedValue)
			{
				for (int k = 0; k < OnLoadedSceneManager.sdk3Items.Length; k++)
				{
					OnLoadedSceneManager.sdk3Items[k].GetComponent<Rigidbody>().velocity = _originalVelocity;
				}
				_returnedValue = true;
			}
		}
		catch (Exception ex2)
		{
			Console.WriteLine("Module : SwasticaOrbit 2" + ex2.Message);
			_itemOrbit = false;
		}
	}
}
