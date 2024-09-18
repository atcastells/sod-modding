using System;
using TMPro;
using UnityEngine;

// Token: 0x02000535 RID: 1333
public class InputBoxController : MonoBehaviour
{
	// Token: 0x06001D21 RID: 7457 RVA: 0x0019E131 File Offset: 0x0019C331
	public void OnButtonSelect()
	{
		this.input.interactable = true;
		this.input.ActivateInputField();
	}

	// Token: 0x040026E4 RID: 9956
	[Header("Components")]
	public TMP_InputField input;

	// Token: 0x040026E5 RID: 9957
	public ButtonController editButton;
}
