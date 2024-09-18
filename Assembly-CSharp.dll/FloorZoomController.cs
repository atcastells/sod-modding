using System;
using TMPro;
using UnityEngine;

// Token: 0x020005A7 RID: 1447
public class FloorZoomController : MonoBehaviour
{
	// Token: 0x06001F94 RID: 8084 RVA: 0x001B24F3 File Offset: 0x001B06F3
	public void AddFloor(int addVal)
	{
		MapController.Instance.SetFloorLayer(MapController.Instance.load + addVal, false);
	}

	// Token: 0x06001F95 RID: 8085 RVA: 0x001B250C File Offset: 0x001B070C
	public void OnSliderChangeFloor()
	{
		MapController.Instance.SetFloorLayer(Mathf.RoundToInt(this.floorSlider.slider.value), false);
	}

	// Token: 0x06001F96 RID: 8086 RVA: 0x001B252E File Offset: 0x001B072E
	public void CentreOnPlayer()
	{
		MapController.Instance.CentreOnTrackedObject(Player.Instance.transform, true);
	}

	// Token: 0x06001F97 RID: 8087 RVA: 0x001B2545 File Offset: 0x001B0745
	public void CancelRouteButton()
	{
		if (MapController.Instance.playerRoute != null)
		{
			MapController.Instance.CancelRoute();
		}
	}

	// Token: 0x06001F98 RID: 8088 RVA: 0x001B255D File Offset: 0x001B075D
	public void AutoTravelButton()
	{
		if (Player.Instance.autoTravelActive)
		{
			Player.Instance.EndAutoTravel();
			return;
		}
		if (MapController.Instance.playerRoute != null)
		{
			Player.Instance.ExecuteAutoTravel(MapController.Instance.playerRoute.end, false);
		}
	}

	// Token: 0x04002983 RID: 10627
	public SliderController floorSlider;

	// Token: 0x04002984 RID: 10628
	public TextMeshProUGUI floorText;
}
