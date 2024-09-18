using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

// Token: 0x020007F8 RID: 2040
public class DDSControls : MonoBehaviour
{
	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06002606 RID: 9734 RVA: 0x001E9376 File Offset: 0x001E7576
	public static DDSControls Instance
	{
		get
		{
			return DDSControls._instance;
		}
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x001E937D File Offset: 0x001E757D
	private void Awake()
	{
		if (DDSControls._instance != null && DDSControls._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		DDSControls._instance = this;
	}

	// Token: 0x06002608 RID: 9736 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void Import()
	{
	}

	// Token: 0x04004088 RID: 16520
	[Header("Sprites")]
	[ReorderableList]
	public List<Sprite> backgroundSprites = new List<Sprite>();

	// Token: 0x04004089 RID: 16521
	[Header("Fonts")]
	public TMP_FontAsset defaultHandwritingFont;

	// Token: 0x0400408A RID: 16522
	public TMP_FontAsset clearModeFont;

	// Token: 0x0400408B RID: 16523
	[ReorderableList]
	public List<TMP_FontAsset> fonts = new List<TMP_FontAsset>();

	// Token: 0x0400408C RID: 16524
	[Header("Elements")]
	public GameObject textComponent;

	// Token: 0x0400408D RID: 16525
	public GameObject elementPrefab;

	// Token: 0x0400408E RID: 16526
	[ReorderableList]
	public List<GameObject> elementPrefabs = new List<GameObject>();

	// Token: 0x0400408F RID: 16527
	[Header("Import")]
	public string sourcePath;

	// Token: 0x04004090 RID: 16528
	private static DDSControls _instance;
}
