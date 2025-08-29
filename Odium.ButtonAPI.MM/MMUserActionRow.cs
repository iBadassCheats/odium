using System;
using Odium.Odium;
using UnityEngine;

namespace Odium.ButtonAPI.MM;

public class MMUserActionRow
{
	public GameObject RowObject { get; private set; }

	public string Title { get; private set; }

	public MMUserActionRow(string title)
	{
		Title = title;
		CreateRow();
	}

	private void CreateRow()
	{
		try
		{
			Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_UserDetail/ScrollRect/Viewport/VerticalLayoutGroup");
			if (transform == null)
			{
				Debug.LogError("Could not find user detail menu container");
				return;
			}
			Transform transform2 = transform.Find("Row3");
			if (transform2 == null)
			{
				Debug.LogError("Could not find Row3 to clone");
				return;
			}
			OdiumConsole.Log("Odium", "Creating new user action row with title: " + Title);
			RowObject = UnityEngine.Object.Instantiate(transform2.gameObject);
			RowObject.transform.SetParent(transform, worldPositionStays: false);
			RowObject.transform.localScale = Vector3.one;
			RowObject.transform.localPosition = Vector3.zero;
			RowObject.transform.localRotation = Quaternion.identity;
			ClearAllButtons();
			SetRowTitle();
			RowObject.name = "Odium_CustomUserRow_" + Title.Replace(" ", "");
			OdiumConsole.Log("Odium", "Successfully created custom user action row: " + Title);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error creating custom user action row: " + ex.Message);
		}
	}

	private void ClearAllButtons()
	{
		if (RowObject == null)
		{
			return;
		}
		Transform transform = RowObject.transform.Find("CellGrid_MM_Content");
		if (transform != null)
		{
			for (int num = transform.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.DestroyImmediate(transform.GetChild(num).gameObject);
			}
			OdiumConsole.Log("Odium", "Cleared existing buttons from cloned user row");
		}
	}

	private void SetRowTitle()
	{
		if (RowObject == null)
		{
			return;
		}
		try
		{
			Transform transform = RowObject.transform.Find("Header_H2");
			if (transform != null)
			{
				Transform transform2 = transform.Find("LeftItemContainer");
				if (transform2 != null)
				{
					Transform transform3 = transform2.Find("Text_Title");
					if (transform3 != null)
					{
						TextMeshProUGUIEx component = transform3.GetComponent<TextMeshProUGUIEx>();
						if (component != null)
						{
							component.text = Title;
							OdiumConsole.Log("Odium", "Set user row title to: " + Title);
						}
						else
						{
							OdiumConsole.Log("Odium", "Warning: Could not find TextMeshProUGUIEx component on Text_Title");
						}
					}
					else
					{
						OdiumConsole.Log("Odium", "Warning: Could not find Text_Title");
					}
				}
				else
				{
					OdiumConsole.Log("Odium", "Warning: Could not find LeftItemContainer");
				}
			}
			else
			{
				OdiumConsole.Log("Odium", "Warning: Could not find Header_H2");
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("Odium", "Error setting row title: " + ex.Message);
		}
	}

	public MMUserButton AddButton(string text, Action action = null, Sprite icon = null)
	{
		return new MMUserButton(this, text, action, icon);
	}

	public void Destroy()
	{
		if (RowObject != null)
		{
			UnityEngine.Object.DestroyImmediate(RowObject);
			RowObject = null;
			OdiumConsole.Log("Odium", "Destroyed user action row: " + Title);
		}
	}
}
