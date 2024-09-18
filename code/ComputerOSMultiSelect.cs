using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000293 RID: 659
public class ComputerOSMultiSelect : MonoBehaviour
{
	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06000EC5 RID: 3781 RVA: 0x000D5334 File Offset: 0x000D3534
	// (remove) Token: 0x06000EC6 RID: 3782 RVA: 0x000D536C File Offset: 0x000D356C
	public event ComputerOSMultiSelect.NewSelection OnNewSelection;

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x06000EC7 RID: 3783 RVA: 0x000D53A4 File Offset: 0x000D35A4
	// (remove) Token: 0x06000EC8 RID: 3784 RVA: 0x000D53DC File Offset: 0x000D35DC
	public event ComputerOSMultiSelect.ChangePage OnChangePage;

	// Token: 0x06000EC9 RID: 3785 RVA: 0x000D5411 File Offset: 0x000D3611
	public void Setup(ComputerController newComp)
	{
		this.controller = newComp;
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x000D541A File Offset: 0x000D361A
	public void UpdateElements(List<ComputerOSMultiSelect.OSMultiOption> newOptions)
	{
		this.allOptions = newOptions;
		this.SpawnList();
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x000D542C File Offset: 0x000D362C
	private void SpawnList()
	{
		foreach (ComputerOSMultiSelectElement computerOSMultiSelectElement in this.options)
		{
			Object.Destroy(computerOSMultiSelectElement.gameObject);
		}
		this.options.Clear();
		float num = 0f;
		int num2 = 0;
		int num3 = this.allOptions.Count;
		if (this.usePages)
		{
			num2 = this.page * this.maxPerPage;
			num3 = num2 + this.maxPerPage;
		}
		int num4 = num2;
		while (num4 < num3 && num4 < this.allOptions.Count)
		{
			ComputerOSMultiSelect.OSMultiOption newOpt = this.allOptions[num4];
			ComputerOSMultiSelectElement component = Object.Instantiate<GameObject>(this.elementPrefab, this.elementParent).GetComponent<ComputerOSMultiSelectElement>();
			component.Setup(newOpt, this);
			component.rect.anchoredPosition = new Vector2(0f, num);
			num -= component.rect.sizeDelta.y;
			this.options.Add(component);
			num4++;
		}
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x000D554C File Offset: 0x000D374C
	public void NextPage(int newPage)
	{
		this.page += newPage;
		this.page = Mathf.Clamp(this.page, 0, Mathf.FloorToInt((float)(this.allOptions.Count / this.maxPerPage)));
		this.SpawnList();
		if (this.OnChangePage != null)
		{
			this.OnChangePage();
		}
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x000D55AC File Offset: 0x000D37AC
	public void SetSelected(ComputerOSMultiSelectElement newSelection)
	{
		this.selected = newSelection;
		foreach (ComputerOSMultiSelectElement computerOSMultiSelectElement in this.options)
		{
			computerOSMultiSelectElement.selected = false;
			computerOSMultiSelectElement.backgroundImage.color = computerOSMultiSelectElement.backgroundColourNormal;
		}
		if (this.selected != null)
		{
			this.selected.selected = true;
			this.selected.backgroundImage.color = this.selected.backgroundColourSelected;
			this.selected.multiSelect.selected = this.selected;
		}
		if (this.OnNewSelection != null)
		{
			this.OnNewSelection();
		}
	}

	// Token: 0x040011EC RID: 4588
	public ComputerController controller;

	// Token: 0x040011ED RID: 4589
	public GameObject elementPrefab;

	// Token: 0x040011EE RID: 4590
	public List<ComputerOSMultiSelectElement> options = new List<ComputerOSMultiSelectElement>();

	// Token: 0x040011EF RID: 4591
	public RectTransform elementParent;

	// Token: 0x040011F0 RID: 4592
	public ComputerOSMultiSelectElement selected;

	// Token: 0x040011F1 RID: 4593
	public bool usePages;

	// Token: 0x040011F2 RID: 4594
	public int page;

	// Token: 0x040011F3 RID: 4595
	public int maxPerPage = 999;

	// Token: 0x040011F4 RID: 4596
	public List<ComputerOSMultiSelect.OSMultiOption> allOptions;

	// Token: 0x02000294 RID: 660
	// (Invoke) Token: 0x06000ED0 RID: 3792
	public delegate void NewSelection();

	// Token: 0x02000295 RID: 661
	// (Invoke) Token: 0x06000ED4 RID: 3796
	public delegate void ChangePage();

	// Token: 0x02000296 RID: 662
	[Serializable]
	public class OSMultiOption
	{
		// Token: 0x040011F7 RID: 4599
		public string text;

		// Token: 0x040011F8 RID: 4600
		public Human human;

		// Token: 0x040011F9 RID: 4601
		public StateSaveData.MessageThreadSave msgThread;

		// Token: 0x040011FA RID: 4602
		public int msgIndex;

		// Token: 0x040011FB RID: 4603
		[NonSerialized]
		public Company.SalesRecord salesRecord;
	}
}
