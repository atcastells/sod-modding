using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000450 RID: 1104
public class RestartSafeController : MonoBehaviour
{
	// Token: 0x170000AD RID: 173
	// (get) Token: 0x060018A8 RID: 6312 RVA: 0x0017073B File Offset: 0x0016E93B
	public static RestartSafeController Instance
	{
		get
		{
			return RestartSafeController._instance;
		}
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x00170742 File Offset: 0x0016E942
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
		if (RestartSafeController._instance != null && RestartSafeController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		RestartSafeController._instance = this;
	}

	// Token: 0x04001EA1 RID: 7841
	public bool loadFromDirty;

	// Token: 0x04001EA2 RID: 7842
	[Header("New Game")]
	public bool generateNew;

	// Token: 0x04001EA3 RID: 7843
	public bool newGameLoadCity;

	// Token: 0x04001EA4 RID: 7844
	public FileInfo loadCityFileInfo;

	// Token: 0x04001EA5 RID: 7845
	public string cityName;

	// Token: 0x04001EA6 RID: 7846
	public int cityX = 5;

	// Token: 0x04001EA7 RID: 7847
	public int cityY = 4;

	// Token: 0x04001EA8 RID: 7848
	public string seed = "NewSeed";

	// Token: 0x04001EA9 RID: 7849
	public bool sandbox;

	// Token: 0x04001EAA RID: 7850
	[Header("New Character")]
	public string newGamePlayerFirstName;

	// Token: 0x04001EAB RID: 7851
	public string newGamePlayerSurname;

	// Token: 0x04001EAC RID: 7852
	public Human.Gender newGamePlayerGender;

	// Token: 0x04001EAD RID: 7853
	public Human.Gender newGamePartnerGender;

	// Token: 0x04001EAE RID: 7854
	public Color newGamePlayerSkinTone;

	// Token: 0x04001EAF RID: 7855
	[Header("Load Save")]
	public bool loadSaveGame;

	// Token: 0x04001EB0 RID: 7856
	public FileInfo saveStateFileInfo;

	// Token: 0x04001EB1 RID: 7857
	[Header("Floor Edit New")]
	public bool newFloor;

	// Token: 0x04001EB2 RID: 7858
	public string newFloorName;

	// Token: 0x04001EB3 RID: 7859
	public Vector2 newFloorSize;

	// Token: 0x04001EB4 RID: 7860
	public int newFloorFloorHeight;

	// Token: 0x04001EB5 RID: 7861
	public int newFloorCeilingHeight;

	// Token: 0x04001EB6 RID: 7862
	[Header("Floor Edit Load")]
	public bool loadFloor;

	// Token: 0x04001EB7 RID: 7863
	public string loadFloorString;

	// Token: 0x04001EB8 RID: 7864
	[Header("Floor Edit Recalculate All")]
	public bool recalculateAll;

	// Token: 0x04001EB9 RID: 7865
	public List<string> floorList = new List<string>();

	// Token: 0x04001EBA RID: 7866
	private static RestartSafeController _instance;
}
