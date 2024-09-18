using System;
using TMPro;
using UnityEngine;

// Token: 0x020004E5 RID: 1253
public class ComputerAutoTextController : MonoBehaviour
{
	// Token: 0x06001B27 RID: 6951 RVA: 0x0018B47C File Offset: 0x0018967C
	private void Start()
	{
		TextMeshProUGUI component = base.gameObject.GetComponent<TextMeshProUGUI>();
		component.text = Strings.Get("computer", component.text, Strings.Casing.asIs, false, false, false, null);
	}
}
