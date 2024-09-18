using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class BuildingSwapController : MonoBehaviour
{
	// Token: 0x06000ADA RID: 2778 RVA: 0x000A4030 File Offset: 0x000A2230
	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			Transform gameObjectAtPosition = this.GetGameObjectAtPosition();
			if (gameObjectAtPosition != null)
			{
				this.activeTile = gameObjectAtPosition.GetComponentInParent<CityTile>();
				int buildingID = this.activeTile.building.buildingID;
				this.ReRollBuilding(buildingID);
			}
		}
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x000A4074 File Offset: 0x000A2274
	private void ReRollBuilding(int buildingID)
	{
		new List<CityTile>();
		List<BuildingPreset> allBuildingPresets = AssetLoader.Instance.GetAllBuildingPresets();
		this.selectionList.Clear();
		this.activeTile.building.RemoveBuilding();
		for (int i = 0; i < allBuildingPresets.Count; i++)
		{
			BuildingPreset item = allBuildingPresets[i];
			if (!item.disable)
			{
				bool flag = false;
				if (this.activeTile.cityCoord.x == 0 || (float)this.activeTile.cityCoord.x == CityData.Instance.citySize.x - 1f || this.activeTile.cityCoord.y == 0 || (float)this.activeTile.cityCoord.y == CityData.Instance.citySize.y - 1f)
				{
					flag = true;
					if (!item.boundary)
					{
						goto IL_348;
					}
					if ((this.activeTile.cityCoord.x == 0 || (float)this.activeTile.cityCoord.x == CityData.Instance.citySize.x - 1f) && (this.activeTile.cityCoord.y == 0 || (float)this.activeTile.cityCoord.y == CityData.Instance.citySize.y - 1f))
					{
						if (!item.boundaryCorner)
						{
							goto IL_348;
						}
					}
					else if (item.boundaryCorner)
					{
						goto IL_348;
					}
				}
				else if (item.boundary)
				{
					goto IL_348;
				}
				if (item.allowedInAllDistricts || item.allowedInDistricts.Contains(this.activeTile.district.preset))
				{
					int count = HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.FindAll((NewBuilding n) => n.preset == item).Count;
					if (count < item.hardLimit || flag)
					{
						float num = Toolbox.Instance.GetPsuedoRandomNumber(0f, 0.2f, this.activeTile.cityCoord.ToString() + NewBuilding.assignID.ToString(), false);
						if (this.activeTile.density >= item.densityMinimum && this.activeTile.density <= item.densityMaximum)
						{
							num += 10f;
						}
						if (this.activeTile.landValue >= item.landValueMinimum && this.activeTile.landValue <= item.landValueMaximum)
						{
							num += 10f;
						}
						float num2 = (float)count / ((CityData.Instance.citySize.x - 2f) * (CityData.Instance.citySize.y - 2f));
						if (float.IsNaN(num2))
						{
							num2 = 0f;
						}
						num += (item.desiredRatio - num2) * 10f;
						if (count < item.minimum)
						{
							num += (float)item.featureImportance * 10f;
						}
						this.selectionList.Add(new CityBuildings.PickBuilding
						{
							preset = item,
							rank = num
						});
					}
				}
			}
			IL_348:;
		}
		this.selectionList.Sort((CityBuildings.PickBuilding p1, CityBuildings.PickBuilding p2) => p2.rank.CompareTo(p1.rank));
		if (this.selectionList.Count > 0)
		{
			CityBuildings.PickBuilding pickBuilding = this.selectionList[0];
			Object.Instantiate<GameObject>(HighlanderSingleton<CityBuildings>.Instance.buildingPrefab, this.activeTile.transform).GetComponent<NewBuilding>().Setup(this.activeTile, pickBuilding.preset, true, buildingID);
		}
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x000A4454 File Offset: 0x000A2654
	private Transform GetGameObjectAtPosition()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(HighlanderSingleton<CityEditorController>.Instance.cityEditorCam.ScreenPointToRay(Input.mousePosition), ref raycastHit))
		{
			Debug.Log("found " + raycastHit.transform.name + " at distance: " + raycastHit.distance.ToString());
		}
		return raycastHit.transform;
	}

	// Token: 0x04000B70 RID: 2928
	public CityTile activeTile;

	// Token: 0x04000B71 RID: 2929
	private List<CityBuildings.PickBuilding> selectionList = new List<CityBuildings.PickBuilding>();
}
