using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class CityBoundaryAndTiles : HighlanderSingleton<CityBoundaryAndTiles>
{
	// Token: 0x06000AFE RID: 2814 RVA: 0x000A54B8 File Offset: 0x000A36B8
	public UniTask SetupCityBoundary()
	{
		CityBoundaryAndTiles.<SetupCityBoundary>d__6 <SetupCityBoundary>d__;
		<SetupCityBoundary>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
		<SetupCityBoundary>d__.<>4__this = this;
		<SetupCityBoundary>d__.<>1__state = -1;
		<SetupCityBoundary>d__.<>t__builder.Start<CityBoundaryAndTiles.<SetupCityBoundary>d__6>(ref <SetupCityBoundary>d__);
		return <SetupCityBoundary>d__.<>t__builder.Task;
	}

	// Token: 0x04000B90 RID: 2960
	public float boundaryLeft;

	// Token: 0x04000B91 RID: 2961
	public float boundaryRight;

	// Token: 0x04000B92 RID: 2962
	public float boundaryUp;

	// Token: 0x04000B93 RID: 2963
	public float boundaryDown;

	// Token: 0x04000B94 RID: 2964
	public GameObject cityTilePrefab;

	// Token: 0x04000B95 RID: 2965
	public Dictionary<Vector2Int, CityTile> cityTiles = new Dictionary<Vector2Int, CityTile>();
}
