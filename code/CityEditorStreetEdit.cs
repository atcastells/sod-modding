using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001DD RID: 477
public class CityEditorStreetEdit : MonoBehaviour
{
	// Token: 0x06000B61 RID: 2913 RVA: 0x000A8F04 File Offset: 0x000A7104
	private void OnEnable()
	{
		if (HighlanderSingleton<CityEditorController>.Instance != null)
		{
			HighlanderSingleton<CityEditorController>.Instance.OnNewCityEditorData += this.OnGenerateNewCityMap;
		}
		this.ResetSelection();
		this.RepopulateStreetList();
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x000A8F38 File Offset: 0x000A7138
	private void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		this.currentlyMousedOverStreet = this.TryGetStreet();
		if (this.currentlyMousedOverStreet != null)
		{
			if (this.previouslyMousedOverStreet != this.currentlyMousedOverStreet)
			{
				this.DrawStreetSelection(this.currentlyMousedOverStreet, true);
			}
			if (InputController.Instance.player.GetButtonDown("Primary") && this.currentlySelectedStreet != this.currentlyMousedOverStreet)
			{
				this.SetSelectedStreet(this.currentlyMousedOverStreet);
				return;
			}
		}
		else
		{
			this.RemoveStreetSelection(true);
			this.previouslyMousedOverStreet = null;
		}
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x000A8FD0 File Offset: 0x000A71D0
	private StreetController TryGetStreet()
	{
		Ray ray = HighlanderSingleton<CityEditorController>.Instance.cityEditorCam.ScreenPointToRay(Input.mousePosition);
		int num = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			3
		});
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, ref raycastHit, 1000f, num))
		{
			Vector3 point = raycastHit.point;
			point.y = 0f;
			Vector3 vector = CityData.Instance.RealPosToNode(point);
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector, ref newNode) && newNode.tile != null)
			{
				return newNode.tile.streetController;
			}
		}
		return null;
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x000A9068 File Offset: 0x000A7268
	public void SetSelectedStreet(StreetController newSt)
	{
		this.currentlySelectedStreet = newSt;
		this.DrawStreetSelection(this.currentlySelectedStreet, false);
		foreach (CityEditorStreetsEditListElement cityEditorStreetsEditListElement in this.spawnedStreetListElements)
		{
			cityEditorStreetsEditListElement.UpdateSelection();
		}
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x000A90CC File Offset: 0x000A72CC
	private void DrawStreetSelection(StreetController street, bool isMouseOver)
	{
		this.RemoveStreetSelection(isMouseOver);
		if (street != null)
		{
			foreach (NewTile newTile in street.tiles)
			{
				GameObject gameObject = this.streetSelectionDisplayPrefab;
				if (isMouseOver)
				{
					gameObject = this.streetMouseOverDisplayPrefab;
				}
				GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, base.transform);
				gameObject2.transform.localScale = new Vector3(5.4f, 5.4f, 5.4f);
				gameObject2.transform.position = newTile.position + new Vector3(0f, 0.2f, 0f);
				if (isMouseOver)
				{
					this.spawnedStreetMouseOverObjects.Add(gameObject2);
				}
				else
				{
					this.spawnedStreetSelectionObjects.Add(gameObject2);
				}
			}
		}
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x000A91B8 File Offset: 0x000A73B8
	private void RemoveStreetSelection(bool isMouseOver)
	{
		if (isMouseOver)
		{
			while (this.spawnedStreetMouseOverObjects.Count > 0)
			{
				if (this.spawnedStreetMouseOverObjects[0] != null)
				{
					Object.Destroy(this.spawnedStreetMouseOverObjects[0]);
				}
				this.spawnedStreetMouseOverObjects.RemoveAt(0);
			}
			return;
		}
		while (this.spawnedStreetSelectionObjects.Count > 0)
		{
			if (this.spawnedStreetSelectionObjects[0] != null)
			{
				Object.Destroy(this.spawnedStreetSelectionObjects[0]);
			}
			this.spawnedStreetSelectionObjects.RemoveAt(0);
		}
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x000A924C File Offset: 0x000A744C
	public void RenameSelectedStreet(string newStreetName)
	{
		if (string.IsNullOrEmpty(newStreetName))
		{
			HighlanderSingleton<CityEditorController>.Instance.SetCityEditorWarning("Building Name contains no valid text characters. (Only alphabet characters allowed)");
			return;
		}
		if (this.currentlySelectedStreet == null)
		{
			HighlanderSingleton<CityEditorController>.Instance.SetCityEditorWarning("No building selected to rename.");
			return;
		}
		if (Game.Instance.printDebug)
		{
			Game.Log("City Edit: New street name: " + newStreetName, 2);
		}
		this.currentlySelectedStreet.playerEditedStreetName = newStreetName;
		this.currentlySelectedStreet.isPlayerEditedName = true;
		this.currentlySelectedStreet.UpdateName(false);
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x000A92CA File Offset: 0x000A74CA
	private void ResetSelection()
	{
		this.currentlySelectedStreet = null;
		this.RemoveStreetSelection(false);
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x000A92DA File Offset: 0x000A74DA
	public void OnGenerateNewCityMap()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("OnGenerateNewCityMap", 2);
		}
		this.RepopulateStreetList();
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x000A92FC File Offset: 0x000A74FC
	public void RepopulateStreetList()
	{
		for (int i = 0; i < this.spawnedStreetListElements.Count; i++)
		{
			Object.Destroy(this.spawnedStreetListElements[i].gameObject);
		}
		this.spawnedStreetListElements.Clear();
		foreach (StreetController newStreet in CityData.Instance.streetDirectory)
		{
			CityEditorStreetsEditListElement component = Object.Instantiate<GameObject>(this.streetListElementPrefab, this.streetListContentRect).GetComponent<CityEditorStreetsEditListElement>();
			component.Setup(newStreet, this);
			this.spawnedStreetListElements.Add(component);
		}
		this.streetListContentRect.sizeDelta = new Vector2(this.streetListContentRect.sizeDelta.x, (float)(this.spawnedStreetListElements.Count * 60 + 50));
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x000A93E0 File Offset: 0x000A75E0
	private void OnDisable()
	{
		if (HighlanderSingleton<CityEditorController>.Instance != null)
		{
			HighlanderSingleton<CityEditorController>.Instance.OnNewCityEditorData -= this.OnGenerateNewCityMap;
		}
		this.ResetSelection();
		this.RemoveStreetSelection(true);
	}

	// Token: 0x04000BF8 RID: 3064
	[Header("References")]
	public GameObject streetListElementPrefab;

	// Token: 0x04000BF9 RID: 3065
	public GameObject streetSelectionDisplayPrefab;

	// Token: 0x04000BFA RID: 3066
	public GameObject streetMouseOverDisplayPrefab;

	// Token: 0x04000BFB RID: 3067
	public RectTransform streetListContentRect;

	// Token: 0x04000BFC RID: 3068
	public VerticalLayoutGroup listLayout;

	// Token: 0x04000BFD RID: 3069
	[Header("State")]
	public StreetController currentlySelectedStreet;

	// Token: 0x04000BFE RID: 3070
	public StreetController currentlyMousedOverStreet;

	// Token: 0x04000BFF RID: 3071
	private StreetController previouslyMousedOverStreet;

	// Token: 0x04000C00 RID: 3072
	private List<CityEditorStreetsEditListElement> spawnedStreetListElements = new List<CityEditorStreetsEditListElement>();

	// Token: 0x04000C01 RID: 3073
	private List<GameObject> spawnedStreetSelectionObjects = new List<GameObject>();

	// Token: 0x04000C02 RID: 3074
	private List<GameObject> spawnedStreetMouseOverObjects = new List<GameObject>();
}
