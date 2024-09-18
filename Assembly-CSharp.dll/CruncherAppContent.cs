using System;
using UnityEngine;

// Token: 0x0200029A RID: 666
public class CruncherAppContent : MonoBehaviour
{
	// Token: 0x06000EE1 RID: 3809 RVA: 0x000D5AFF File Offset: 0x000D3CFF
	public virtual void Setup(ComputerController cc)
	{
		this.controller = cc;
		this.OnSetup();
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnSetup()
	{
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x000D5B0E File Offset: 0x000D3D0E
	public virtual void PrintButton()
	{
		this.controller.SetTimedLoading(Toolbox.Instance.Rand(0.5f, 1f, false), 0.33f);
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x000D5B38 File Offset: 0x000D3D38
	public void OnPlayerTakePrint()
	{
		this.controller.printedDocument.OnRemovedFromWorld -= this.OnPlayerTakePrint;
		this.controller.printedDocument = null;
		this.controller.printerParent.localPosition = this.controller.printOutStartPos;
	}

	// Token: 0x04001210 RID: 4624
	public ComputerController controller;
}
