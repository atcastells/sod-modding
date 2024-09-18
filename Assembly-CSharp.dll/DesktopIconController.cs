using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A5 RID: 677
public class DesktopIconController : ComputerOSUIComponent
{
	// Token: 0x06000F0F RID: 3855 RVA: 0x000D703C File Offset: 0x000D523C
	public void Setup(DesktopApp newDesktop, CruncherAppPreset newApp)
	{
		this.desktop = newDesktop;
		this.preset = newApp;
		this.icon.sprite = this.preset.desktopIcon;
		this.iconText.text = Strings.Get("computer", this.preset.name, Strings.Casing.asIs, false, false, false, null);
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x000D7092 File Offset: 0x000D5292
	public override void OnLeftClick()
	{
		this.desktop.OnDesktopAppSelect(this.preset);
	}

	// Token: 0x04001234 RID: 4660
	public DesktopApp desktop;

	// Token: 0x04001235 RID: 4661
	public CruncherAppPreset preset;

	// Token: 0x04001236 RID: 4662
	public RectTransform rect;

	// Token: 0x04001237 RID: 4663
	public Image icon;

	// Token: 0x04001238 RID: 4664
	public TextMeshProUGUI iconText;
}
