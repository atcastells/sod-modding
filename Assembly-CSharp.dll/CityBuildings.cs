using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public class CityBuildings : HighlanderSingleton<CityBuildings>
{
	// Token: 0x06000B02 RID: 2818 RVA: 0x000A57EC File Offset: 0x000A39EC
	public UniTask GenerateBuildings()
	{
		CityBuildings.<GenerateBuildings>d__6 <GenerateBuildings>d__;
		<GenerateBuildings>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
		<GenerateBuildings>d__.<>4__this = this;
		<GenerateBuildings>d__.<>1__state = -1;
		<GenerateBuildings>d__.<>t__builder.Start<CityBuildings.<GenerateBuildings>d__6>(ref <GenerateBuildings>d__);
		return <GenerateBuildings>d__.<>t__builder.Task;
	}

	// Token: 0x04000B9A RID: 2970
	public int loadChunk = 10;

	// Token: 0x04000B9B RID: 2971
	private List<BuildingPreset> buildingPresets;

	// Token: 0x04000B9C RID: 2972
	public List<NewBuilding> buildingDirectory = new List<NewBuilding>();

	// Token: 0x04000B9D RID: 2973
	public GameObject buildingPrefab;

	// Token: 0x04000B9E RID: 2974
	private List<CityBuildings.PickBuilding> selectionList = new List<CityBuildings.PickBuilding>();

	// Token: 0x020001C7 RID: 455
	public class PickBuilding
	{
		// Token: 0x04000B9F RID: 2975
		public BuildingPreset preset;

		// Token: 0x04000BA0 RID: 2976
		public float rank;
	}
}
