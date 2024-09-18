using System;
using UnityEngine;

// Token: 0x020006E2 RID: 1762
[CreateAssetMenu(fileName = "cloudsave_data", menuName = "Database/Dev Cloud Save Data")]
public class CloudSaveData : SoCustomComparison
{
	// Token: 0x040032A2 RID: 12962
	[Header("Dev Cloud Save Path")]
	[Tooltip("Path to dropbox save folder")]
	public string path;
}
