using System;
using TMPro;
using UnityEngine;

// Token: 0x02000583 RID: 1411
public class ObjectiveContentListEntry : ButtonController
{
	// Token: 0x06001EDB RID: 7899 RVA: 0x001ACF6F File Offset: 0x001AB16F
	public void Setup(ObjectivesContentController newController, Case.ResolveQuestion newStarting)
	{
		this.SetupReferences();
		this.objectivesController = newController;
		this.question = newStarting;
		this.VisualUpdate();
	}

	// Token: 0x06001EDC RID: 7900 RVA: 0x001ACF8B File Offset: 0x001AB18B
	public override void VisualUpdate()
	{
		if (this.question != null)
		{
			this.objectiveText.text = this.question.GetText(null, true, true);
		}
	}

	// Token: 0x040028B0 RID: 10416
	[Header("References")]
	public TextMeshProUGUI objectiveText;

	// Token: 0x040028B1 RID: 10417
	public Case.ResolveQuestion question;

	// Token: 0x040028B2 RID: 10418
	public ObjectivesContentController objectivesController;
}
