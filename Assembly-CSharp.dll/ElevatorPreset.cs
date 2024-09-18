using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000711 RID: 1809
[CreateAssetMenu(fileName = "elevator_data", menuName = "Database/Elevator Preset")]
public class ElevatorPreset : SoCustomComparison
{
	// Token: 0x0400343E RID: 13374
	public List<GameObject> stairWellPrefabs;

	// Token: 0x0400343F RID: 13375
	public List<GameObject> stairsPrefabs;

	// Token: 0x04003440 RID: 13376
	public float rotationOffset;

	// Token: 0x04003441 RID: 13377
	public Material bottomMaterial;

	// Token: 0x04003442 RID: 13378
	public Material topMaterial;
}
