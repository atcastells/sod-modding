using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020003B3 RID: 947
public class FootprintController : MonoBehaviour
{
	// Token: 0x06001570 RID: 5488 RVA: 0x001393A4 File Offset: 0x001375A4
	public void Setup(GameplayController.Footprint newFootprint)
	{
		this.footprint = newFootprint;
		CityData.Instance.GetHuman(this.footprint.hID, out this.human, true);
		this.SetUseQuad(Game.Instance.useQuadsForFootprints);
		float num = Mathf.InverseLerp(CitizenControls.Instance.shoeSizeRange.x, CitizenControls.Instance.shoeSizeRange.y, (float)this.human.descriptors.shoeSize);
		float num2 = Mathf.Lerp(GameplayControls.Instance.footprintScaleRange.x, GameplayControls.Instance.footprintScaleRange.y, num);
		base.transform.localScale = new Vector3(num2, num2, num2);
		base.transform.position = this.footprint.wP;
		base.transform.eulerAngles = this.footprint.eU;
		Interactable newInteractable = null;
		if (GameplayController.Instance.confirmedFootprints.TryGetValue(this.footprint.wP, ref newInteractable))
		{
			this.scanProgress = 1f;
			this.printConfirmed = true;
			this.printInteractable = base.gameObject.AddComponent<InteractableController>();
			this.printInteractable.Setup(newInteractable);
			InteractionController.Instance.OnPlayerLookAtChange();
		}
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x001394D8 File Offset: 0x001376D8
	public void SetUseQuad(bool val)
	{
		this.useQuad = val;
		if (this.useQuad && this.quad != null)
		{
			this.quad.sharedMaterial = MaterialsController.Instance.GetFootprintMaterial(this);
			this.quad.enabled = true;
			if (this.projector != null)
			{
				this.projector.transform.gameObject.SetActive(false);
			}
			this.quad.transform.gameObject.SetActive(true);
			return;
		}
		if (!this.useQuad && this.projector != null)
		{
			this.projector.material = MaterialsController.Instance.GetFootprintMaterial(this);
			this.projector.enabled = true;
			this.projector.transform.gameObject.SetActive(true);
			if (this.quad != null)
			{
				this.quad.transform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x001395D1 File Offset: 0x001377D1
	public void ResetScan()
	{
		if (this.printConfirmed)
		{
			return;
		}
		this.scanProgress = 0f;
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x001395E8 File Offset: 0x001377E8
	public void PrintConfirmed()
	{
		if (!this.printConfirmed)
		{
			this.printConfirmed = true;
			if (GameplayController.Instance.confirmedFootprints.ContainsKey(this.footprint.wP))
			{
				return;
			}
			this.printInteractable = base.gameObject.AddComponent<InteractableController>();
			Game.Log("Player: Discovered print belonging to " + this.human.GetCitizenName(), 2);
			Interactable newInteractable = InteractableCreator.Instance.CreateFootprintInteractable(this.human, this.footprint.wP, this.footprint.eU, this.footprint);
			this.printInteractable.Setup(newInteractable);
			InteractionController.Instance.OnPlayerLookAtChange();
		}
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x00139698 File Offset: 0x00137898
	public static void InitialisePool()
	{
		for (int i = 0; i < 300; i++)
		{
			FootprintController component = Object.Instantiate<GameObject>(PrefabControls.Instance.footprint).GetComponent<FootprintController>();
			component.transform.SetParent(null);
			component.transform.position = new Vector3(0f, -1000f, 0f);
			FootprintController.footprintPool.Enqueue(component);
		}
	}

	// Token: 0x06001575 RID: 5493 RVA: 0x00139700 File Offset: 0x00137900
	public static FootprintController GetNewFootprint()
	{
		if (FootprintController.footprintPool.Count > 0)
		{
			while (FootprintController.footprintPool.Count > 0 && FootprintController.footprintPool.Peek() == null)
			{
				FootprintController.footprintPool.Dequeue();
			}
			if (FootprintController.footprintPool.Count > 0)
			{
				return FootprintController.footprintPool.Dequeue();
			}
		}
		FootprintController component = Object.Instantiate<GameObject>(PrefabControls.Instance.footprint).GetComponent<FootprintController>();
		component.transform.SetParent(null);
		return component;
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x00139780 File Offset: 0x00137980
	public static void RecycleFootprint(FootprintController footprintController)
	{
		footprintController.transform.SetParent(null);
		footprintController.transform.position = new Vector3(0f, -1000f, 0f);
		footprintController.scanProgress = 0f;
		FootprintController.footprintPool.Enqueue(footprintController);
	}

	// Token: 0x04001AA3 RID: 6819
	private const int INITIAL_POOL_SIZE = 300;

	// Token: 0x04001AA4 RID: 6820
	private const float RECYCLED_Y_POSITION = -1000f;

	// Token: 0x04001AA5 RID: 6821
	private static Queue<FootprintController> footprintPool = new Queue<FootprintController>();

	// Token: 0x04001AA6 RID: 6822
	[Header("Components")]
	public GameplayController.Footprint footprint;

	// Token: 0x04001AA7 RID: 6823
	public MeshRenderer quad;

	// Token: 0x04001AA8 RID: 6824
	public DecalProjector projector;

	// Token: 0x04001AA9 RID: 6825
	public Human human;

	// Token: 0x04001AAA RID: 6826
	[Tooltip("Use a quad instead of a decal projector")]
	[Header("Values/Settings")]
	public bool useQuad;

	// Token: 0x04001AAB RID: 6827
	public float scanProgress;

	// Token: 0x04001AAC RID: 6828
	public bool printConfirmed;

	// Token: 0x04001AAD RID: 6829
	public InteractableController printInteractable;
}
