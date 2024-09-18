using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200039F RID: 927
public class ActiveCodebreakerController : MonoBehaviour
{
	// Token: 0x0600151E RID: 5406 RVA: 0x00134128 File Offset: 0x00132328
	private void Update()
	{
		if (!this.cracked)
		{
			string text = Mathf.RoundToInt(this.controller.interactable.cs).ToString();
			if (text.Length == 0)
			{
				text = "0000";
			}
			else if (text.Length == 1)
			{
				text = "000" + text;
			}
			else if (text.Length == 2)
			{
				text = "00" + text;
			}
			else if (text.Length == 3)
			{
				text = "0" + text;
			}
			this.text.text = text;
			this.rend.sharedMaterial = this.activeMaterials[Toolbox.Instance.Rand(0, this.activeMaterials.Count, false)];
		}
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x001341EA File Offset: 0x001323EA
	public void OnCrack(string codeStr)
	{
		this.cracked = true;
		this.rend.sharedMaterial = this.activeMaterials[0];
		this.text.text = codeStr;
		this.text.color = Color.green;
	}

	// Token: 0x04001A0E RID: 6670
	public InteractableController controller;

	// Token: 0x04001A0F RID: 6671
	public TextMeshPro text;

	// Token: 0x04001A10 RID: 6672
	public bool cracked;

	// Token: 0x04001A11 RID: 6673
	public MeshRenderer rend;

	// Token: 0x04001A12 RID: 6674
	public List<Material> activeMaterials = new List<Material>();
}
