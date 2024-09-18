using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000749 RID: 1865
[CreateAssetMenu(fileName = "actions_data", menuName = "Database/Interactable Actions Preset")]
public class InteractableActionsPreset : SoCustomComparison
{
	// Token: 0x040036B3 RID: 14003
	[Tooltip("Additional actions able to be performed")]
	[BoxGroup("Primary Actions")]
	public List<InteractablePreset.InteractionAction> actions = new List<InteractablePreset.InteractionAction>();

	// Token: 0x040036B4 RID: 14004
	[Space(7f)]
	[BoxGroup("Locked-in Interaction 1")]
	[Tooltip("Disable the collider when locked-in")]
	public bool disableCollider;

	// Token: 0x040036B5 RID: 14005
	[BoxGroup("Locked-in Interaction 1")]
	public List<InteractablePreset.InteractionAction> lockedInActions1 = new List<InteractablePreset.InteractionAction>();

	// Token: 0x040036B6 RID: 14006
	[BoxGroup("Locked-in Interaction 2")]
	[Space(7f)]
	public List<InteractablePreset.InteractionAction> lockedInActions2 = new List<InteractablePreset.InteractionAction>();

	// Token: 0x040036B7 RID: 14007
	[BoxGroup("Physics Pick Up Actions")]
	[Space(7f)]
	public List<InteractablePreset.InteractionAction> physicsActions = new List<InteractablePreset.InteractionAction>();
}
