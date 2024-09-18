using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public class CityTile : Controller, IComparable<CityTile>
{
	// Token: 0x0600060E RID: 1550 RVA: 0x0006038C File Offset: 0x0005E58C
	public void Setup(Vector2Int newCoord)
	{
		this.cityCoord = newCoord;
		HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles.Add(this.cityCoord, this);
		float num = (float)this.cityCoord.x * CityControls.Instance.cityTileSize.x - CityData.Instance.citySize.x * 0.5f * CityControls.Instance.cityTileSize.x + CityControls.Instance.cityTileSize.x * 0.5f;
		float num2 = (float)this.cityCoord.y * CityControls.Instance.cityTileSize.y - CityData.Instance.citySize.y * 0.5f * CityControls.Instance.cityTileSize.y + CityControls.Instance.cityTileSize.y * 0.5f;
		base.transform.position = new Vector3(num, 0f, num2);
		base.name = "CityTile " + Mathf.RoundToInt((float)this.cityCoord.x).ToString() + "," + Mathf.RoundToInt((float)this.cityCoord.y).ToString();
		base.transform.name = base.name;
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x000604D8 File Offset: 0x0005E6D8
	public void LoadTileOnly(CitySaveData.CityTileCitySave data)
	{
		base.name = data.name;
		this.cityCoord = data.cityCoord;
		this.districtID = data.districtID;
		this.blockID = data.blockID;
		this.density = data.density;
		this.landValue = data.landValue;
		HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles.Add(this.cityCoord, this);
		float num = (float)this.cityCoord.x * CityControls.Instance.cityTileSize.x - CityData.Instance.citySize.x * 0.5f * CityControls.Instance.cityTileSize.x + CityControls.Instance.cityTileSize.x * 0.5f;
		float num2 = (float)this.cityCoord.y * CityControls.Instance.cityTileSize.y - CityData.Instance.citySize.y * 0.5f * CityControls.Instance.cityTileSize.y + CityControls.Instance.cityTileSize.y * 0.5f;
		base.transform.position = new Vector3(num, 0f, num2);
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x0006060C File Offset: 0x0005E80C
	public void SetDensity(BuildingPreset.Density newDensity)
	{
		this.density = newDensity;
		if (this.district != null)
		{
			this.density = (BuildingPreset.Density)Mathf.Clamp((int)this.density, (int)this.district.preset.minimumDensity, (int)this.district.preset.maximumDensity);
		}
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00060660 File Offset: 0x0005E860
	public void SetLandVlaue(BuildingPreset.LandValue newLandvalue)
	{
		this.landValue = newLandvalue;
		if (this.district != null)
		{
			this.landValue = (BuildingPreset.LandValue)Mathf.Clamp((int)this.landValue, (int)this.district.preset.minimumLandValue, (int)this.district.preset.maximumLandValue);
		}
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x000606B3 File Offset: 0x0005E8B3
	public void AddOutsideTile(NewTile newTile)
	{
		if (!this.outsideTiles.Contains(newTile))
		{
			newTile.cityTile = this;
			this.outsideTiles.Add(newTile);
		}
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x000606D6 File Offset: 0x0005E8D6
	public int CompareTo(CityTile compare)
	{
		return this.landValue.CompareTo(compare.landValue);
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x000606F4 File Offset: 0x0005E8F4
	public void SetPlayerInVicinity(bool val)
	{
		this.isInPlayerVicinity = val;
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x00060700 File Offset: 0x0005E900
	public void SetPlayerPresentOnGroundmap(bool val)
	{
		this.playerPresent = val;
		if (this.building != null && Game.Instance.noShadowsWhenPlayerIsInDifferentGoundmapLocation)
		{
			int i = 0;
			while (i < this.building.allInteriorMainLights.Count)
			{
				if (this.playerPresent)
				{
					try
					{
						if (this.building.allInteriorMainLights[i].hdrpLightData != null)
						{
							this.building.allInteriorMainLights[i].hdrpLightData.EnableShadows(true);
						}
						if (this.building.allInteriorMainLights[i].lightComponent != null)
						{
							this.building.allInteriorMainLights[i].lightComponent.shadows = 1;
						}
						goto IL_129;
					}
					catch
					{
						goto IL_129;
					}
					goto IL_AF;
				}
				goto IL_AF;
				IL_129:
				i++;
				continue;
				IL_AF:
				try
				{
					if (this.building.allInteriorMainLights[i].hdrpLightData != null)
					{
						this.building.allInteriorMainLights[i].hdrpLightData.EnableShadows(false);
					}
					if (this.building.allInteriorMainLights[i].lightComponent != null)
					{
						this.building.allInteriorMainLights[i].lightComponent.shadows = 0;
					}
				}
				catch
				{
				}
				goto IL_129;
			}
		}
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x0006086C File Offset: 0x0005EA6C
	public CitySaveData.CityTileCitySave GenerateSaveData()
	{
		CitySaveData.CityTileCitySave cityTileCitySave = new CitySaveData.CityTileCitySave();
		cityTileCitySave.name = base.name;
		cityTileCitySave.cityCoord = this.cityCoord;
		cityTileCitySave.density = this.density;
		cityTileCitySave.landValue = this.landValue;
		cityTileCitySave.building = this.building.GenerateSaveData();
		foreach (NewTile newTile in this.outsideTiles)
		{
			cityTileCitySave.outsideTiles.Add(newTile.GenerateSaveData());
		}
		cityTileCitySave.blockID = this.block.blockID;
		cityTileCitySave.districtID = this.district.districtID;
		return cityTileCitySave;
	}

	// Token: 0x04000652 RID: 1618
	[Header("Location")]
	public Vector2Int cityCoord;

	// Token: 0x04000653 RID: 1619
	public DistrictController district;

	// Token: 0x04000654 RID: 1620
	public int districtID = -1;

	// Token: 0x04000655 RID: 1621
	public BlockController block;

	// Token: 0x04000656 RID: 1622
	public int blockID = -1;

	// Token: 0x04000657 RID: 1623
	public NewBuilding building;

	// Token: 0x04000658 RID: 1624
	public List<NewTile> outsideTiles = new List<NewTile>();

	// Token: 0x04000659 RID: 1625
	public bool isInPlayerVicinity;

	// Token: 0x0400065A RID: 1626
	public bool playerPresent;

	// Token: 0x0400065B RID: 1627
	[Header("Details")]
	public BuildingPreset.Density density = BuildingPreset.Density.medium;

	// Token: 0x0400065C RID: 1628
	public BuildingPreset.LandValue landValue = BuildingPreset.LandValue.medium;
}
