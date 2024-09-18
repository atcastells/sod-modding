using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200023B RID: 571
public class AutomaticDoorOpener : MonoBehaviour
{
	// Token: 0x06000CE2 RID: 3298 RVA: 0x000B7EF8 File Offset: 0x000B60F8
	private void OnTriggerEnter(Collider other)
	{
		if (this.door == null)
		{
			return;
		}
		Citizen componentInParent = other.GetComponentInParent<Citizen>();
		if (componentInParent != null)
		{
			if (!this.overlapping.Contains(componentInParent))
			{
				this.overlapping.Add(componentInParent);
			}
			base.enabled = true;
			if (!this.door.isOpen && !this.door.isOpening)
			{
				this.door.SetOpen(1f, componentInParent, false);
			}
		}
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x000B7F74 File Offset: 0x000B6174
	private void OnTriggerExit(Collider other)
	{
		if (this.door == null)
		{
			return;
		}
		Citizen componentInParent = other.GetComponentInParent<Citizen>();
		if (componentInParent != null)
		{
			this.overlapping.Remove(componentInParent);
		}
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x000B7FAD File Offset: 0x000B61AD
	private void Awake()
	{
		base.enabled = false;
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x000B7FB8 File Offset: 0x000B61B8
	private void Update()
	{
		if (this.door == null)
		{
			base.enabled = false;
			return;
		}
		if (this.overlapping.Count <= 0)
		{
			if (this.door.isOpen || this.door.isOpening)
			{
				this.door.SetOpen(0f, null, false);
				base.enabled = false;
				return;
			}
			base.enabled = false;
		}
	}

	// Token: 0x04000ED6 RID: 3798
	[Header("Setup")]
	public DoorMovementController door;

	// Token: 0x04000ED7 RID: 3799
	[Header("State")]
	public List<Citizen> overlapping = new List<Citizen>();
}
