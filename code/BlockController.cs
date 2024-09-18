using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class BlockController : Controller, IComparable<BlockController>
{
	// Token: 0x060006F1 RID: 1777 RVA: 0x00069D98 File Offset: 0x00067F98
	public void Setup(DistrictController newDistrict)
	{
		this.blockID = BlockController.assignID;
		BlockController.assignID++;
		base.name = "Block " + this.blockID.ToString();
		base.transform.name = base.name;
		newDistrict.AddBlock(this);
		this.favourVertical = Toolbox.Instance.GetPsuedoRandomNumberContained(0, 2, newDistrict.seed, out newDistrict.seed);
		HighlanderSingleton<CityBlocks>.Instance.blocksDirectory.Add(this);
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x00069E20 File Offset: 0x00068020
	public void Load(CitySaveData.BlockCitySave data, DistrictController newDistrict)
	{
		base.name = data.name;
		this.blockID = data.blockID;
		this.averageDensity = data.averageDensity;
		this.averageLandValue = data.averageLandValue;
		newDistrict.AddBlock(this);
		HighlanderSingleton<CityBlocks>.Instance.blocksDirectory.Add(this);
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x00069E74 File Offset: 0x00068074
	public void AddCityTile(CityTile newTile)
	{
		if (!this.cityTiles.Contains(newTile))
		{
			if (newTile.block != null)
			{
				newTile.block.cityTiles.Remove(newTile);
			}
			newTile.block = this;
			this.cityTiles.Add(newTile);
		}
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x00069EC4 File Offset: 0x000680C4
	public void UpdateAverageDensity()
	{
		float num = 0f;
		foreach (CityTile cityTile in this.cityTiles)
		{
			num += (float)cityTile.density;
		}
		this.averageDensity = num / (float)this.cityTiles.Count;
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x00069F38 File Offset: 0x00068138
	public void UpdateAverageLandValue()
	{
		float num = 0f;
		foreach (CityTile cityTile in this.cityTiles)
		{
			num += (float)cityTile.landValue;
		}
		this.averageLandValue = num / (float)this.cityTiles.Count;
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x00069FAC File Offset: 0x000681AC
	public int CompareTo(BlockController compare)
	{
		return this.averageDensity.CompareTo(compare.averageDensity);
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x00069FBF File Offset: 0x000681BF
	public CitySaveData.BlockCitySave GenerateSaveData()
	{
		return new CitySaveData.BlockCitySave
		{
			name = base.name,
			blockID = this.blockID,
			averageDensity = this.averageDensity,
			averageLandValue = this.averageLandValue
		};
	}

	// Token: 0x0400070F RID: 1807
	[Header("ID")]
	public int blockID;

	// Token: 0x04000710 RID: 1808
	public static int assignID = 0;

	// Token: 0x04000711 RID: 1809
	[Header("Location")]
	public int favourVertical;

	// Token: 0x04000712 RID: 1810
	public List<CityTile> cityTiles = new List<CityTile>();

	// Token: 0x04000713 RID: 1811
	[NonSerialized]
	public float averageDensity;

	// Token: 0x04000714 RID: 1812
	[NonSerialized]
	public float averageLandValue;

	// Token: 0x04000715 RID: 1813
	public static Comparison<BlockController> LandValueComparison = (BlockController object1, BlockController object2) => object1.averageLandValue.CompareTo(object2.averageLandValue);
}
