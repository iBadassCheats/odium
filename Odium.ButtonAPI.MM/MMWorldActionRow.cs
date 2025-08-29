using System;
using Odium.Odium;
using UnityEngine;
using UnityEngine.UI;

namespace Odium.ButtonAPI.MM;

public class MMWorldActionRow
{
	public GameObject HeaderObject { get; private set; }

	public GameObject ActionsObject { get; private set; }

	public string Title { get; private set; }

	public MMWorldActionRow(string title)
	{
		Title = title;
		CreateRow();
	}

	private void CreateRow()
	{
		try
		{
			Transform transform = AssignedVariables.userInterface.transform.Find("Canvas_MainMenu(Clone)/Container/MMParent/HeaderOffset/Menu_MM_WorldInformation/Panel_World_Information/Content/Viewport/BodyContainer_World_Details/ScrollRect/Viewport/VerticalLayoutGroup");
			if (transform == null)
			{
				Debug.LogError("Could not find menu container");
				return;
			}
			Transform transform2 = transform.Find("Header_Actions");
			Transform transform3 = transform.Find("Actions");
			if (transform2 == null)
			{
				Debug.LogError("Could not find Header_Actions to clone");
				return;
			}
			if (transform3 == null)
			{
				Debug.LogError("Could not find Actions to clone");
				return;
			}
			OdiumConsole.Log("Odium", "Creating new action row with title: " + Title);
			HeaderObject = UnityEngine.Object.Instantiate(transform2.gameObject);
			HeaderObject.transform.SetParent(transform, worldPositionStays: false);
			HeaderObject.transform.localScale = Vector3.one;
			HeaderObject.name = "Odium_Header_" + Title.Replace(" ", "");
			ActionsObject = UnityEngine.Object.Instantiate(transform3.gameObject);
			ActionsObject.transform.SetParent(transform, worldPositionStays: false);
			ActionsObject.transform.localScale = Vector3.one;
			ActionsObject.name = "Odium_Actions_" + Title.Replace(" ", "");
			int siblingIndex = transform3.GetSiblingIndex();
			HeaderObject.transform.SetSiblingIndex(siblingIndex + 1);
			ActionsObject.transform.SetSiblingIndex(siblingIndex + 2);
			UpdateHeaderTitle();
			ClearAllButtons();
			OdiumConsole.Log("Odium", "Successfully created custom action row: " + Title);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error creating custom action row: " + ex.Message);
		}
	}

	private void UpdateHeaderTitle()
	{
		if (HeaderObject == null)
		{
			return;
		}
		try
		{
			TextMeshProUGUIEx textMeshProUGUIEx = HeaderObject.GetComponentInChildren<TextMeshProUGUIEx>();
			if (textMeshProUGUIEx == null)
			{
				Transform transform = HeaderObject.transform.Find("LeftItemContainer");
				if (transform != null)
				{
					Transform transform2 = transform.Find("Text_Title");
					if (transform2 != null)
					{
						textMeshProUGUIEx = transform2.GetComponent<TextMeshProUGUIEx>();
					}
				}
			}
			if (textMeshProUGUIEx != null)
			{
				textMeshProUGUIEx.text = Title;
				OdiumConsole.Log("Odium", "Set header title to: " + Title);
			}
			else
			{
				OdiumConsole.Log("Odium", "Warning: Could not find text component in header");
			}
		}
		catch (Exception ex)
		{
			OdiumConsole.Log("Odium", "Error updating header title: " + ex.Message);
		}
	}

	private void ClearAllButtons()
	{
		if (ActionsObject == null)
		{
			return;
		}
		for (int num = ActionsObject.transform.childCount - 1; num >= 0; num--)
		{
			Transform child = ActionsObject.transform.GetChild(num);
			if (child.GetComponent<Button>() != null)
			{
				UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}
		OdiumConsole.Log("Odium", "Cleared existing buttons from cloned actions container");
	}

	public MMWorldButton AddButton(string text, Action action = null, Sprite icon = null)
	{
		return new MMWorldButton(this, text, action, icon);
	}

	public void Destroy()
	{
		if (HeaderObject != null)
		{
			UnityEngine.Object.DestroyImmediate(HeaderObject);
			HeaderObject = null;
		}
		if (ActionsObject != null)
		{
			UnityEngine.Object.DestroyImmediate(ActionsObject);
			ActionsObject = null;
		}
		OdiumConsole.Log("Odium", "Destroyed action row: " + Title);
	}
}
