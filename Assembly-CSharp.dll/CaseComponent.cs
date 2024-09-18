using System;
using UnityEngine;

// Token: 0x02000629 RID: 1577
public class CaseComponent
{
	// Token: 0x1400004C RID: 76
	// (add) Token: 0x060022F3 RID: 8947 RVA: 0x001D5958 File Offset: 0x001D3B58
	// (remove) Token: 0x060022F4 RID: 8948 RVA: 0x001D5990 File Offset: 0x001D3B90
	public event CaseComponent.DiscoveredThis OnDiscoveredThis;

	// Token: 0x1400004D RID: 77
	// (add) Token: 0x060022F5 RID: 8949 RVA: 0x001D59C8 File Offset: 0x001D3BC8
	// (remove) Token: 0x060022F6 RID: 8950 RVA: 0x001D5A00 File Offset: 0x001D3C00
	public event CaseComponent.NewName OnNewName;

	// Token: 0x1400004E RID: 78
	// (add) Token: 0x060022F7 RID: 8951 RVA: 0x001D5A38 File Offset: 0x001D3C38
	// (remove) Token: 0x060022F8 RID: 8952 RVA: 0x001D5A70 File Offset: 0x001D3C70
	public event CaseComponent.NewSprite OnNewSprite;

	// Token: 0x060022FA RID: 8954 RVA: 0x001D5AA5 File Offset: 0x001D3CA5
	public virtual void SetFound(bool newVal)
	{
		if (this.isFound != newVal)
		{
			int num = this.isFound ? 1 : 0;
			this.isFound = newVal;
			if (num == 0 && newVal)
			{
				this.OnDiscovery();
				if (this.OnDiscoveredThis != null)
				{
					this.OnDiscoveredThis(this);
				}
			}
		}
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x001D5ADE File Offset: 0x001D3CDE
	public virtual string GetIdentifier()
	{
		return string.Empty;
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnDiscovery()
	{
	}

	// Token: 0x060022FD RID: 8957 RVA: 0x001D5AE8 File Offset: 0x001D3CE8
	public virtual void UpdateName()
	{
		string text = this.GenerateName() + this.FoundAtName() + this.GenerateNameSuffix();
		if (text != this.name)
		{
			this.name = text.Trim();
			if (this.OnNewName != null)
			{
				this.OnNewName();
			}
		}
	}

	// Token: 0x060022FE RID: 8958 RVA: 0x001D5ADE File Offset: 0x001D3CDE
	public virtual string GenerateName()
	{
		return string.Empty;
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x001D5ADE File Offset: 0x001D3CDE
	public virtual string FoundAtName()
	{
		return string.Empty;
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x001D5ADE File Offset: 0x001D3CDE
	public virtual string GenerateNameSuffix()
	{
		return string.Empty;
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x001D5B3A File Offset: 0x001D3D3A
	public void SetNewIcon(Sprite newLarge)
	{
		if (newLarge != null)
		{
			this.iconSprite = newLarge;
		}
		if (this.OnNewSprite != null)
		{
			this.OnNewSprite();
		}
	}

	// Token: 0x04002D41 RID: 11585
	public string name;

	// Token: 0x04002D42 RID: 11586
	public bool isFound;

	// Token: 0x04002D43 RID: 11587
	public Sprite iconSprite;

	// Token: 0x0200062A RID: 1578
	// (Invoke) Token: 0x06002303 RID: 8963
	public delegate void DiscoveredThis(CaseComponent discovered);

	// Token: 0x0200062B RID: 1579
	// (Invoke) Token: 0x06002307 RID: 8967
	public delegate void NewName();

	// Token: 0x0200062C RID: 1580
	// (Invoke) Token: 0x0600230B RID: 8971
	public delegate void NewSprite();
}
