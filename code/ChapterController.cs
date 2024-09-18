using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class ChapterController : MonoBehaviour
{
	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06000D30 RID: 3376 RVA: 0x000BB620 File Offset: 0x000B9820
	// (remove) Token: 0x06000D31 RID: 3377 RVA: 0x000BB658 File Offset: 0x000B9858
	public event ChapterController.NewPart OnNewPart;

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x06000D32 RID: 3378 RVA: 0x000BB68D File Offset: 0x000B988D
	public static ChapterController Instance
	{
		get
		{
			return ChapterController._instance;
		}
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x000BB694 File Offset: 0x000B9894
	private void Awake()
	{
		if (ChapterController._instance != null && ChapterController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			ChapterController._instance = this;
		}
		this.allChapters = AssetLoader.Instance.GetAllChapters();
		this.allChapters.Sort((ChapterPreset p1, ChapterPreset p2) => p1.chapterNumber.CompareTo(p2.chapterNumber));
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x000BB708 File Offset: 0x000B9908
	public void LoadChapter(ChapterPreset newChapter, bool newLoadFirstPartOnStart)
	{
		Game.Log("CityGen: Load chapter #" + newChapter.chapterNumber.ToString(), 2);
		this.loadedChapter = newChapter;
		this.loadFirstPartOnStart = newLoadFirstPartOnStart;
		this.currentPart = -1;
		this.currentPartName = string.Empty;
		if (this.chapterObject != null)
		{
			Object.Destroy(this.chapterObject);
		}
		if (this.loadedChapter.scriptObject != null)
		{
			this.chapterObject = Object.Instantiate<GameObject>(this.loadedChapter.scriptObject, base.transform);
		}
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x000BB798 File Offset: 0x000B9998
	public void LoadPart(int partNumber, bool teleportPlayer = false, bool delay = true)
	{
		if (this.currentPart != partNumber)
		{
			this.currentPart = partNumber;
			this.currentPartName = this.loadedChapter.partNames[this.currentPart];
			if (this.OnNewPart != null)
			{
				this.OnNewPart(delay, teleportPlayer);
			}
		}
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x000BB7E8 File Offset: 0x000B99E8
	public void LoadPart(string chapterString)
	{
		if (this.loadedChapter == null)
		{
			Game.Log("Chapter: No chapter is loaded while trying to load part " + chapterString + "...", 2);
		}
		int num = this.loadedChapter.partNames.FindIndex((string item) => item == chapterString);
		if (num > -1)
		{
			this.LoadPart(num, false, true);
		}
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x000BB855 File Offset: 0x000B9A55
	public void SkipToChapterPart(int newPart, bool teleport, bool delay)
	{
		if (this.loadedChapter != null)
		{
			Game.Log("Chapter: Skipping to chapter part " + newPart.ToString(), 2);
			Player.Instance.ClearAllDisabledActions();
			this.LoadPart(newPart, teleport, delay);
		}
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x000BB88F File Offset: 0x000B9A8F
	[Button(null, 0)]
	public void SkipToNextPart()
	{
		this.SkipToChapterPart(this.currentPart + 1, true, false);
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x000BB8A1 File Offset: 0x000B9AA1
	public void ResetThis()
	{
		if (this.chapterObject != null)
		{
			Object.Destroy(this.chapterObject);
		}
		this.loadedChapter = null;
		this.currentPart = -1;
		this.currentPartName = string.Empty;
	}

	// Token: 0x04000F77 RID: 3959
	public List<ChapterPreset> allChapters = new List<ChapterPreset>();

	// Token: 0x04000F78 RID: 3960
	[Header("Loaded")]
	public ChapterPreset loadedChapter;

	// Token: 0x04000F79 RID: 3961
	public Chapter chapterScript;

	// Token: 0x04000F7A RID: 3962
	public GameObject chapterObject;

	// Token: 0x04000F7B RID: 3963
	[ReadOnly]
	public int currentPart = -1;

	// Token: 0x04000F7C RID: 3964
	[ReadOnly]
	public string currentPartName = string.Empty;

	// Token: 0x04000F7D RID: 3965
	public bool loadFirstPartOnStart = true;

	// Token: 0x04000F7F RID: 3967
	private static ChapterController _instance;

	// Token: 0x02000253 RID: 595
	// (Invoke) Token: 0x06000D3C RID: 3388
	public delegate void NewPart(bool delay, bool teleport);
}
