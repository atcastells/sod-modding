using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000204 RID: 516
public class FurniturePhotoCaptureController : MonoBehaviour
{
	// Token: 0x06000BC2 RID: 3010 RVA: 0x000AAE58 File Offset: 0x000A9058
	[Button(null, 0)]
	public void LoadIndex()
	{
		int num = (int)this.captureIndex.x;
		List<FurniturePreset> validPresets = this.GetValidPresets();
		if (num >= 0 && num < validPresets.Count)
		{
			this.captureSingle = validPresets[num];
			this.LoadSingle();
		}
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x00003EEE File Offset: 0x000020EE
	private List<FurniturePreset> GetValidPresets()
	{
		return null;
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void LoadSingle()
	{
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void UpdatePositions()
	{
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x000AAE99 File Offset: 0x000A9099
	[Button(null, 0)]
	public void NextIndex()
	{
		this.captureIndex.x = this.captureIndex.x + 1f;
		this.LoadIndex();
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x000AAEB5 File Offset: 0x000A90B5
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

	// Token: 0x06000BC8 RID: 3016 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CaptureSingle()
	{
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CaptureAllSpawnableInteractables()
	{
	}

	// Token: 0x04000D1F RID: 3359
	[Header("Components")]
	public Transform spawnParent;

	// Token: 0x04000D20 RID: 3360
	public GameObject spawnedObject;

	// Token: 0x04000D21 RID: 3361
	public Camera captureCam;

	// Token: 0x04000D22 RID: 3362
	[Header("Settings")]
	public int resolution = 128;

	// Token: 0x04000D23 RID: 3363
	[Header("Capture")]
	[ShowAssetPreview(64, 64)]
	[InfoBox("Don't forget to turn film grain off before capturing, unless you want it featured", 0)]
	public Sprite captured;

	// Token: 0x04000D24 RID: 3364
	public InteractablePreset prefabOverrideObject;

	// Token: 0x04000D25 RID: 3365
	public GameObject prefabOverride;

	// Token: 0x04000D26 RID: 3366
	[Range(0.1f, 3f)]
	public float scale = 1f;

	// Token: 0x04000D27 RID: 3367
	[ReadOnly]
	public Vector3 itemPos;

	// Token: 0x04000D28 RID: 3368
	[ReadOnly]
	public Vector3 itemEuler;

	// Token: 0x04000D29 RID: 3369
	[Space(7f)]
	public Vector2 captureIndex;

	// Token: 0x04000D2A RID: 3370
	public FurniturePreset captureSingle;
}
