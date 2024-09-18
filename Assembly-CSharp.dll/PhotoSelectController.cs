using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000589 RID: 1417
public class PhotoSelectController : MonoBehaviour
{
	// Token: 0x06001EF8 RID: 7928 RVA: 0x001ADB58 File Offset: 0x001ABD58
	public void Setup(WindowContentController newWcc)
	{
		this.wcc = newWcc;
		while (this.spawned.Count > 0)
		{
			Object.Destroy(this.spawned[0].gameObject);
			this.spawned.RemoveAt(0);
		}
		List<PhotoSelectController.CitAsk> list = new List<PhotoSelectController.CitAsk>();
		foreach (Case @case in CasePanelController.Instance.activeCases)
		{
			foreach (Case.CaseElement caseElement in @case.caseElements)
			{
				Evidence evidence = null;
				if (GameplayController.Instance.evidenceDictionary.TryGetValue(caseElement.id, ref evidence))
				{
					EvidenceCitizen evidenceCitizen = evidence as EvidenceCitizen;
					if (evidenceCitizen != null)
					{
						list.Add(new PhotoSelectController.CitAsk
						{
							citizen = evidenceCitizen.witnessController,
							element = caseElement
						});
					}
				}
			}
		}
		float num = 16f;
		float num2 = -16f;
		foreach (PhotoSelectController.CitAsk citAsk in list)
		{
			Human citizen = citAsk.citizen;
			if (citizen != null)
			{
				PhotoSelectButtonController component = Object.Instantiate<GameObject>(this.photoPrefab, this.pageRect).GetComponent<PhotoSelectButtonController>();
				this.spawned.Add(component);
				component.rect.anchoredPosition = new Vector2(num, num2);
				component.Setup(citizen, citAsk.element, this.wcc.window);
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

	// Token: 0x040028C2 RID: 10434
	[Header("References")]
	public RectTransform pageRect;

	// Token: 0x040028C3 RID: 10435
	public WindowContentController wcc;

	// Token: 0x040028C4 RID: 10436
	[Header("Prefabs")]
	public GameObject photoPrefab;

	// Token: 0x040028C5 RID: 10437
	private List<PhotoSelectButtonController> spawned = new List<PhotoSelectButtonController>();

	// Token: 0x0200058A RID: 1418
	public class CitAsk
	{
		// Token: 0x040028C6 RID: 10438
		public Human citizen;

		// Token: 0x040028C7 RID: 10439
		public Case.CaseElement element;
	}
}
