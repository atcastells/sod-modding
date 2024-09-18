using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200057C RID: 1404
public class ItemSelectController : MonoBehaviour
{
	// Token: 0x06001EBA RID: 7866 RVA: 0x001AB6B0 File Offset: 0x001A98B0
	public void Setup(WindowContentController newWcc)
	{
		Game.Log("Interface: Setting up item select controller...", 2);
		this.wcc = newWcc;
		while (this.spawned.Count > 0)
		{
			Object.Destroy(this.spawned[0].gameObject);
			this.spawned.RemoveAt(0);
		}
		float num = 16f;
		float num2 = -16f;
		foreach (FirstPersonItemController.InventorySlot inventorySlot in FirstPersonItemController.Instance.slots)
		{
			if (inventorySlot != null && inventorySlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
			{
				Interactable interactable = inventorySlot.GetInteractable();
				if (interactable != null)
				{
					ItemSelectButtonController component = Object.Instantiate<GameObject>(this.selectPrefab, this.pageRect).GetComponent<ItemSelectButtonController>();
					component.rect.anchoredPosition = new Vector2(num, num2);
					component.Setup(interactable, this.wcc.window);
					num += component.rect.sizeDelta.x + 10f;
					if (num > 500f)
					{
						num = 12f;
						num2 -= component.rect.sizeDelta.y + 10f;
					}
					this.pageRect.sizeDelta = new Vector2(this.pageRect.sizeDelta.x, -num2 + component.rect.sizeDelta.y + 32f);
				}
			}
		}
	}

	// Token: 0x0400288E RID: 10382
	[Header("References")]
	public RectTransform pageRect;

	// Token: 0x0400288F RID: 10383
	public WindowContentController wcc;

	// Token: 0x04002890 RID: 10384
	[Header("Prefabs")]
	public GameObject selectPrefab;

	// Token: 0x04002891 RID: 10385
	private List<ItemSelectButtonController> spawned = new List<ItemSelectButtonController>();
}
