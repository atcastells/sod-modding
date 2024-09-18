using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000432 RID: 1074
public class SwitchSyncBehaviour : MonoBehaviour
{
	// Token: 0x06001832 RID: 6194 RVA: 0x00169C6C File Offset: 0x00167E6C
	public virtual void SetOn(bool val)
	{
		if (this.inverted)
		{
			val = !val;
		}
		this.isOn = val;
		if (this.syncInteractable != null && this.syncInteractable.interactable != null)
		{
			this.syncInteractable.interactable.SetSwtichByType(this.syncWithState, this.isOn, null, true, false);
		}
		if (this.basicBehaviour == SwitchSyncBehaviour.BasicBehaviour.hideWhenOn)
		{
			using (List<GameObject>.Enumerator enumerator = this.basicBehaviourObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameObject gameObject = enumerator.Current;
					if (!(gameObject == null))
					{
						gameObject.SetActive(!this.isOn);
					}
				}
				return;
			}
		}
		if (this.basicBehaviour == SwitchSyncBehaviour.BasicBehaviour.hideWhenOff)
		{
			foreach (GameObject gameObject2 in this.basicBehaviourObjects)
			{
				if (!(gameObject2 == null))
				{
					gameObject2.SetActive(this.isOn);
				}
			}
		}
	}

	// Token: 0x04001E1B RID: 7707
	[Header("State")]
	public InteractablePreset.Switch syncWithState;

	// Token: 0x04001E1C RID: 7708
	public bool isOn;

	// Token: 0x04001E1D RID: 7709
	public bool inverted;

	// Token: 0x04001E1E RID: 7710
	[Header("Basic Behaviour")]
	public SwitchSyncBehaviour.BasicBehaviour basicBehaviour;

	// Token: 0x04001E1F RID: 7711
	public List<GameObject> basicBehaviourObjects = new List<GameObject>();

	// Token: 0x04001E20 RID: 7712
	[Tooltip("Sync this interactable")]
	public InteractableController syncInteractable;

	// Token: 0x04001E21 RID: 7713
	public bool onlySyncWhenParentIsOn;

	// Token: 0x02000433 RID: 1075
	public enum BasicBehaviour
	{
		// Token: 0x04001E23 RID: 7715
		none,
		// Token: 0x04001E24 RID: 7716
		hideWhenOn,
		// Token: 0x04001E25 RID: 7717
		hideWhenOff
	}
}
