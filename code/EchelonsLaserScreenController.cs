using System;
using UnityEngine;

// Token: 0x020003A9 RID: 937
public class EchelonsLaserScreenController : MonoBehaviour
{
	// Token: 0x0600154C RID: 5452 RVA: 0x00135E3C File Offset: 0x0013403C
	public void Setup(InteractableController newController)
	{
		this.controller = newController;
		if (this.controller != null && !SessionData.Instance.isFloorEdit && this.controller.interactable != null && this.controller.interactable.node != null && this.controller.interactable.node.gameLocation != null && this.controller.interactable.node.gameLocation.thisAsAddress != null && this.controller.interactable.node.gameLocation.thisAsAddress.floor != null && Game.Instance.allowEchelons && this.controller.interactable.node.building != null && this.controller.interactable.node.building.preset.buildingFeaturesEchelonFloors && this.controller.interactable.node.gameLocation.thisAsAddress.floor.floor >= this.controller.interactable.node.building.preset.echelonFloorStart)
		{
			base.gameObject.SetActive(true);
			Toolbox.Instance.SetLightLayer(base.gameObject, this.controller.interactable.node.building, false);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001A51 RID: 6737
	private InteractableController controller;
}
