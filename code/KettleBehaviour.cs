using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000405 RID: 1029
public class KettleBehaviour : SwitchSyncBehaviour
{
	// Token: 0x06001792 RID: 6034 RVA: 0x00162080 File Offset: 0x00160280
	private void Update()
	{
		if (this.isOn)
		{
			Color color = this.steamColour;
			color.a = Mathf.Lerp(0f, 1f, this.syncInteractable.interactable.cs);
			this.rend.material.SetColor("Color_C6D75069", color);
			return;
		}
		if (this.syncInteractable.interactable.cs <= 0f)
		{
			using (List<GameObject>.Enumerator enumerator = this.basicBehaviourObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameObject gameObject = enumerator.Current;
					gameObject.SetActive(this.isOn);
				}
				return;
			}
		}
		Color color2 = this.steamColour;
		color2.a = Mathf.Lerp(0f, 1f, this.syncInteractable.interactable.cs);
		this.rend.material.SetColor("Color_C6D75069", color2);
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x0016217C File Offset: 0x0016037C
	public override void SetOn(bool val)
	{
		if (this.inverted)
		{
			val = !val;
		}
		this.isOn = val;
		if (this.syncInteractable != null)
		{
			if (this.syncWithState == InteractablePreset.Switch.switchState)
			{
				this.syncInteractable.interactable.SetSwitchState(this.isOn, null, true, false, false);
			}
			else if (this.syncWithState == InteractablePreset.Switch.custom1)
			{
				this.syncInteractable.interactable.SetCustomState1(this.isOn, null, true, false, false);
			}
			else if (this.syncWithState == InteractablePreset.Switch.custom2)
			{
				this.syncInteractable.interactable.SetCustomState2(this.isOn, null, true, false, false);
			}
			else if (this.syncWithState == InteractablePreset.Switch.custom3)
			{
				this.syncInteractable.interactable.SetCustomState3(this.isOn, null, true, false, false);
			}
			else if (this.syncWithState == InteractablePreset.Switch.lockState)
			{
				this.syncInteractable.interactable.SetLockedState(this.isOn, null, true, false);
			}
			else if (this.syncWithState == InteractablePreset.Switch.carryPhysicsObject)
			{
				this.syncInteractable.interactable.SetPhysicsPickupState(this.isOn, null, true, false);
			}
		}
		if (this.basicBehaviour == SwitchSyncBehaviour.BasicBehaviour.hideWhenOn)
		{
			using (List<GameObject>.Enumerator enumerator = this.basicBehaviourObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameObject gameObject = enumerator.Current;
					gameObject.SetActive(!this.isOn);
				}
				return;
			}
		}
		if (this.basicBehaviour == SwitchSyncBehaviour.BasicBehaviour.hideWhenOff && this.syncInteractable.interactable.cs <= 0f)
		{
			foreach (GameObject gameObject2 in this.basicBehaviourObjects)
			{
				gameObject2.SetActive(this.isOn);
			}
		}
	}

	// Token: 0x04001CDC RID: 7388
	[ColorUsage(true, true)]
	public Color steamColour;

	// Token: 0x04001CDD RID: 7389
	public Renderer rend;
}
