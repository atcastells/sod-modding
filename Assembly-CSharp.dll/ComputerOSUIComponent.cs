using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000298 RID: 664
public class ComputerOSUIComponent : MonoBehaviour
{
	// Token: 0x06000EDC RID: 3804 RVA: 0x000D58EC File Offset: 0x000D3AEC
	public virtual void OnLeftClick()
	{
		Game.Log("Player: Computer clicked on " + base.name, 2);
		if (this.button != null)
		{
			this.button.onClick.Invoke();
		}
	}

	// Token: 0x04001205 RID: 4613
	public Button button;

	// Token: 0x04001206 RID: 4614
	public AudioEvent sfx;
}
