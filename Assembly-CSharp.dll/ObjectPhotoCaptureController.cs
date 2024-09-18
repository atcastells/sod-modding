using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000206 RID: 518
public class ObjectPhotoCaptureController : MonoBehaviour
{
	// Token: 0x06000BD1 RID: 3025 RVA: 0x000AAFB0 File Offset: 0x000A91B0
	[Button(null, 0)]
	public void LoadIndex()
	{
		int num = (int)this.captureIndex.x;
		List<InteractablePreset> validPresets = this.GetValidPresets();
		if (num >= 0 && num < validPresets.Count)
		{
			this.captureSingle = validPresets[num];
			this.LoadSingle();
		}
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x00003EEE File Offset: 0x000020EE
	private List<InteractablePreset> GetValidPresets()
	{
		return null;
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void LoadSingle()
	{
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void UpdatePositions()
	{
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x000AAFF1 File Offset: 0x000A91F1
	[Button(null, 0)]
	public void NextIndex()
	{
		this.captureIndex.x = this.captureIndex.x + 1f;
		this.LoadIndex();
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x000AB00D File Offset: 0x000A920D
	[Button(null, 0)]
	public void PreviousIndex()
	{
		this.captureIndex.x = this.captureIndex.x - 1f;
		if (this.captureIndex.y < 0f)
		{
			this.captureIndex.y = 0f;
		}
		this.LoadIndex();
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CaptureSingle()
	{
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CaptureAllSpawnableInteractables()
	{
	}

	// Token: 0x04000D2F RID: 3375
	[Header("Components")]
	public Transform spawnParent;

	// Token: 0x04000D30 RID: 3376
	public GameObject spawnedObject;

	// Token: 0x04000D31 RID: 3377
	public Camera captureCam;

	// Token: 0x04000D32 RID: 3378
	[Header("Settings")]
	public int resolution = 128;

	// Token: 0x04000D33 RID: 3379
	[ShowAssetPreview(64, 64)]
	[Header("Capture")]
	[InfoBox("Don't forget to turn film grain off before capturing, unless you want it featured", 0)]
	public Sprite captured;

	// Token: 0x04000D34 RID: 3380
	[ShowAssetPreview(64, 64)]
	public Sprite icon;

	// Token: 0x04000D35 RID: 3381
	public InteractablePreset prefabOverrideObject;

	// Token: 0x04000D36 RID: 3382
	public GameObject prefabOverride;

	// Token: 0x04000D37 RID: 3383
	[Range(0.1f, 3f)]
	public float scale = 1f;

	// Token: 0x04000D38 RID: 3384
	[ReadOnly]
	public Vector3 itemPos;

	// Token: 0x04000D39 RID: 3385
	[ReadOnly]
	public Vector3 itemEuler;

	// Token: 0x04000D3A RID: 3386
	[Space(7f)]
	public Vector2 captureIndex;

	// Token: 0x04000D3B RID: 3387
	public InteractablePreset captureSingle;
}
