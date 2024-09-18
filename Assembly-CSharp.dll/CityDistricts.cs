using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;

// Token: 0x020001CE RID: 462
public class CityDistricts : HighlanderSingleton<CityDistricts>
{
	// Token: 0x06000B12 RID: 2834 RVA: 0x000A6344 File Offset: 0x000A4544
	public UniTask GenerateDistricts()
	{
		CityDistricts.<GenerateDistricts>d__3 <GenerateDistricts>d__;
		<GenerateDistricts>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
		<GenerateDistricts>d__.<>4__this = this;
		<GenerateDistricts>d__.<>1__state = -1;
		<GenerateDistricts>d__.<>t__builder.Start<CityDistricts.<GenerateDistricts>d__3>(ref <GenerateDistricts>d__);
		return <GenerateDistricts>d__.<>t__builder.Task;
	}

	// Token: 0x04000BB0 RID: 2992
	public GameObject districtPrefab;

	// Token: 0x04000BB1 RID: 2993
	public List<DistrictController> districtDirectory = new List<DistrictController>();

	// Token: 0x020001CF RID: 463
	public class DistrictPlacement
	{
		// Token: 0x04000BB2 RID: 2994
		public float score;

		// Token: 0x04000BB3 RID: 2995
		public List<CityTile> tiles = new List<CityTile>();

		// Token: 0x04000BB4 RID: 2996
		public List<CityTile> innerTiles = new List<CityTile>();

		// Token: 0x04000BB5 RID: 2997
		public List<CityTile> edgeTiles = new List<CityTile>();
	}
}
