using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002BF RID: 703
public class FileSystemController : MonoBehaviour
{
	// Token: 0x06000F9A RID: 3994 RVA: 0x000DD7E0 File Offset: 0x000DB9E0
	public void Setup(InteractableController newController)
	{
		this.controller = newController;
		this.ev = (this.controller.interactable.evidence as EvidenceMultiPage);
		if (this.ev != null)
		{
			foreach (EvidenceMultiPage.MultiPageContent multiPageContent in this.ev.pageContent)
			{
				if (!this.content.ContainsKey(multiPageContent.page))
				{
					this.content.Add(multiPageContent.page, new List<EvidenceMultiPage.MultiPageContent>());
				}
				this.content[multiPageContent.page].Add(multiPageContent);
				this.pageCount = Mathf.Max(this.pageCount, multiPageContent.page);
			}
			for (int i = 0; i < Mathf.Min(this.pageCount, 14); i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.filePrefab, this.rearBunch);
				Toolbox.Instance.SetLightLayer(this.filePrefab, this.controller.interactable.node.building, false);
				this.controller.pages.Add(gameObject.transform);
				gameObject.transform.localPosition = this.pagesOffset * (float)i;
			}
			for (int j = this.controller.pages.Count - 1; j >= 0; j--)
			{
				this.rearPages.Add(this.controller.pages[j]);
			}
			this.SetPage(this.ev.page, true);
			base.enabled = false;
			return;
		}
		Game.LogError("No multi page evidence found for " + this.controller.interactable.name + " (check evidence config!)", 2);
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x000DD9B4 File Offset: 0x000DBBB4
	public void SetPage(int newPage, bool instant = false)
	{
		this.currentPage = newPage;
		for (int i = 0; i < this.controller.pages.Count; i++)
		{
			Transform transform = this.controller.pages[i];
			if (i < this.currentPage)
			{
				transform.SetParent(this.frontBunch);
				this.rearPages.Remove(transform);
				if (!this.fontPages.Contains(transform))
				{
					this.fontPages.Add(transform);
				}
			}
			else
			{
				transform.SetParent(this.rearBunch);
				this.fontPages.Remove(transform);
				if (!this.rearPages.Contains(transform))
				{
					this.rearPages.Add(transform);
				}
			}
		}
		this.moveProgress = 0f;
		if (instant)
		{
			for (int j = 0; j < this.fontPages.Count; j++)
			{
				this.fontPages[j].localPosition = -this.pagesOffset * (float)j;
			}
			for (int k = 0; k < this.rearPages.Count; k++)
			{
				this.rearPages[k].localPosition = this.pagesOffset * (float)k;
			}
			return;
		}
		base.enabled = true;
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x000DDAF0 File Offset: 0x000DBCF0
	private void Update()
	{
		this.moveProgress += Time.deltaTime;
		int num = this.rearPages.Count + this.fontPages.Count;
		for (int i = 0; i < this.fontPages.Count; i++)
		{
			Transform transform = this.fontPages[i];
			int num2 = this.rearPages.Count + i;
			float num3 = (float)num2 / (float)num;
			Vector3 vector = this.frontPagesPos * num3;
			Vector3 vector2 = this.frontPagesEuler * num3;
			Vector3 vector3 = vector;
			if (this.stackMode == FileSystemController.StackType.filingSystem)
			{
				vector3 += -this.pagesOffset * (float)i;
			}
			else
			{
				vector3 += this.pagesOffset * (float)num2;
			}
			transform.localPosition = Vector3.Lerp(transform.localPosition, vector3, this.moveProgress);
			transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, vector2, this.moveProgress);
		}
		for (int j = 0; j < this.rearPages.Count; j++)
		{
			Transform transform2 = this.rearPages[j];
			int num4 = j;
			float num5 = (float)num4 / (float)num;
			Vector3 zero = Vector3.zero;
			Vector3 vector4 = Vector3.zero;
			if (this.stackMode == FileSystemController.StackType.filingSystem)
			{
				vector4 += this.pagesOffset * (float)j;
			}
			else
			{
				vector4 += this.pagesOffset * (float)num4;
			}
			transform2.localPosition = Vector3.Lerp(transform2.localPosition, vector4, this.moveProgress);
			transform2.localEulerAngles = Vector3.Lerp(transform2.localEulerAngles, zero, this.moveProgress);
		}
		if (this.moveProgress >= 1f)
		{
			this.moveProgress = 1f;
			base.enabled = false;
		}
	}

	// Token: 0x0400130E RID: 4878
	[Header("Setup")]
	public FileSystemController.StackType stackMode;

	// Token: 0x0400130F RID: 4879
	public InteractableController controller;

	// Token: 0x04001310 RID: 4880
	public GameObject filePrefab;

	// Token: 0x04001311 RID: 4881
	public Vector3 pagesOffset = new Vector3(0f, 0f, 0.02f);

	// Token: 0x04001312 RID: 4882
	public EvidenceMultiPage ev;

	// Token: 0x04001313 RID: 4883
	[Tooltip("Apply a postion & rotation to the top pages group")]
	[Space(5f)]
	public Vector3 frontPagesPos = Vector3.zero;

	// Token: 0x04001314 RID: 4884
	public Vector3 frontPagesEuler = Vector3.zero;

	// Token: 0x04001315 RID: 4885
	public Dictionary<int, List<EvidenceMultiPage.MultiPageContent>> content = new Dictionary<int, List<EvidenceMultiPage.MultiPageContent>>();

	// Token: 0x04001316 RID: 4886
	public int pageCount;

	// Token: 0x04001317 RID: 4887
	[Header("File System")]
	public Transform frontBunch;

	// Token: 0x04001318 RID: 4888
	public Transform rearBunch;

	// Token: 0x04001319 RID: 4889
	public int currentPage;

	// Token: 0x0400131A RID: 4890
	public List<Transform> fontPages = new List<Transform>();

	// Token: 0x0400131B RID: 4891
	public List<Transform> rearPages = new List<Transform>();

	// Token: 0x0400131C RID: 4892
	private float moveProgress;

	// Token: 0x020002C0 RID: 704
	public enum StackType
	{
		// Token: 0x0400131E RID: 4894
		filingSystem,
		// Token: 0x0400131F RID: 4895
		pile
	}
}
