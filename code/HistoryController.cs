using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005E8 RID: 1512
public class HistoryController : MonoBehaviour
{
	// Token: 0x170000FE RID: 254
	// (get) Token: 0x0600214A RID: 8522 RVA: 0x001C6430 File Offset: 0x001C4630
	public static HistoryController Instance
	{
		get
		{
			return HistoryController._instance;
		}
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x001C6438 File Offset: 0x001C4638
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		if (HistoryController._instance != null && HistoryController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			HistoryController._instance = this;
		}
		this.wcc = newContentController;
		this.SetPageSize(new Vector2(740f, 738f));
		this.contentsText.text = Strings.Get("ui.handbook", "historyheader", Strings.Casing.asIs, false, false, false, null);
		this.isSetup = true;
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x001C64C5 File Offset: 0x001C46C5
	public void SetPageSize(Vector2 newSize)
	{
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x001C64E9 File Offset: 0x001C46E9
	private void OnEnable()
	{
		if (this.isSetup)
		{
			GameplayController.Instance.OnNewEvidenceHistory += this.UpdateListDisplay;
			this.UpdateListDisplay();
		}
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x001C650F File Offset: 0x001C470F
	private void OnDisable()
	{
		GameplayController.Instance.OnNewEvidenceHistory -= this.UpdateListDisplay;
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x001C650F File Offset: 0x001C470F
	private void OnDestroy()
	{
		GameplayController.Instance.OnNewEvidenceHistory -= this.UpdateListDisplay;
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x001C6528 File Offset: 0x001C4728
	public void UpdateListDisplay()
	{
		Game.Log("Interface: Updating history list...", 2);
		if (InterfaceController.Instance.selectedElement != null)
		{
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, 0f);
		List<GameplayController.History> list = new List<GameplayController.History>();
		foreach (GameplayController.History history in GameplayController.Instance.history)
		{
			if (list.Count >= 40)
			{
				break;
			}
			if (!list.Contains(history))
			{
				if (this.searchInputField.text.Length <= 0)
				{
					list.Add(history);
				}
				else
				{
					Evidence evidence = null;
					if (GameplayController.Instance.evidenceDictionary.TryGetValue(history.evID, ref evidence) && evidence.GetNameForDataKey(history.keys).ToLower().Contains(this.searchInputField.text.ToLower()))
					{
						list.Add(history);
					}
				}
			}
		}
		if (this.searchInputField.text.Length > 0)
		{
			using (List<NewGameLocation>.Enumerator enumerator2 = CityData.Instance.gameLocationDirectory.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					NewGameLocation location = enumerator2.Current;
					if ((!(location.thisAsStreet == null) || location.rooms.Count > 0) && location.entrances.Count > 0 && location.evidenceEntry != null)
					{
						List<Evidence.DataKey> tiedKeys = location.evidenceEntry.GetTiedKeys(Evidence.DataKey.location);
						if (tiedKeys.Contains(Evidence.DataKey.name) && location.evidenceEntry.GetNameForDataKey(Evidence.DataKey.name).ToLower().Contains(this.searchInputField.text.ToLower()) && !list.Exists((GameplayController.History item) => item.evID == location.evidenceEntry.evID))
						{
							list.Add(new GameplayController.History
							{
								evID = location.evidenceEntry.evID,
								keys = tiedKeys,
								lastAccess = -1f
							});
						}
					}
				}
			}
			using (List<NewBuilding>.Enumerator enumerator3 = HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					NewBuilding building = enumerator3.Current;
					if (building.evidenceEntry != null && building.floors.Count > 0)
					{
						List<Evidence.DataKey> tiedKeys2 = building.evidenceEntry.GetTiedKeys(Evidence.DataKey.location);
						if (building.evidenceEntry.GetNameForDataKey(Evidence.DataKey.location).ToLower().Contains(this.searchInputField.text.ToLower()) && !list.Exists((GameplayController.History item) => item.evID == building.evidenceEntry.evID))
						{
							list.Add(new GameplayController.History
							{
								evID = building.evidenceEntry.evID,
								keys = tiedKeys2,
								lastAccess = -1f
							});
						}
					}
				}
			}
		}
		for (int i = 0; i < this.spawnedEntries.Count; i++)
		{
			SuspectListEntryController suspectListEntryController = this.spawnedEntries[i];
			if (suspectListEntryController == null)
			{
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
			else if (list.Contains(suspectListEntryController.key))
			{
				list.Remove(suspectListEntryController.key);
				suspectListEntryController.VisualUpdate();
			}
			else
			{
				Object.Destroy(suspectListEntryController.gameObject);
				this.spawnedEntries.RemoveAt(i);
				i--;
			}
		}
		foreach (GameplayController.History history2 in list)
		{
			Evidence evidence2;
			if (history2 != null && history2.evID != null && history2.evID.Length > 0 && GameplayController.Instance.evidenceDictionary.TryGetValue(history2.evID, ref evidence2))
			{
				SuspectListEntryController component = Object.Instantiate<GameObject>(PrefabControls.Instance.suspectWindowEntry, this.entryParent).GetComponent<SuspectListEntryController>();
				component.Setup(history2);
				this.spawnedEntries.Add(component);
			}
		}
		this.spawnedEntries.Sort((SuspectListEntryController p2, SuspectListEntryController p1) => p1.key.lastAccess.CompareTo(p2.key.lastAccess));
		for (int j = 0; j < this.spawnedEntries.Count; j++)
		{
			this.spawnedEntries[j].transform.SetAsLastSibling();
		}
		this.entryParent.sizeDelta = new Vector2(this.entryParent.sizeDelta.x, Mathf.Max((float)this.spawnedEntries.Count * 130f + 24f, 466f));
		this.SetPageSize(new Vector2(this.rect.sizeDelta.x, this.entryParent.sizeDelta.y + 400f));
	}

	// Token: 0x06002151 RID: 8529 RVA: 0x001C6AEC File Offset: 0x001C4CEC
	public void ClearSearchButton()
	{
		this.searchInputField.text = string.Empty;
	}

	// Token: 0x04002B91 RID: 11153
	public RectTransform rect;

	// Token: 0x04002B92 RID: 11154
	public WindowContentController wcc;

	// Token: 0x04002B93 RID: 11155
	public bool isSetup;

	// Token: 0x04002B94 RID: 11156
	public TextMeshProUGUI contentsText;

	// Token: 0x04002B95 RID: 11157
	public RectTransform entryParent;

	// Token: 0x04002B96 RID: 11158
	public TMP_InputField searchInputField;

	// Token: 0x04002B97 RID: 11159
	public List<SuspectListEntryController> spawnedEntries = new List<SuspectListEntryController>();

	// Token: 0x04002B98 RID: 11160
	private static HistoryController _instance;
}
