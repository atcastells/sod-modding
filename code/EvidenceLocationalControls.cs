using System;
using System.Linq;
using TMPro;
using UnityEngine;

// Token: 0x02000566 RID: 1382
public class EvidenceLocationalControls : MonoBehaviour
{
	// Token: 0x06001E0E RID: 7694 RVA: 0x001A5A30 File Offset: 0x001A3C30
	private void OnEnable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange += this.CheckEnabled;
		this.parentWindow.OnWindowRefresh += this.CheckEnabled;
		if (this.acceptJobText != null)
		{
			if (this.parentWindow != null && this.parentWindow.passedEvidence != null && this.parentWindow.passedEvidence.interactable != null && this.parentWindow.passedEvidence.interactable.jobParent != null && this.parentWindow.passedEvidence.interactable.jobParent.accepted)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.acceptJobText.text = Strings.Get("ui.interface", "Accept Job", Strings.Casing.asIs, false, false, false, null);
		}
		if (this.takeItemText != null)
		{
			this.takeItemText.text = Strings.Get("ui.interface", "Take Item", Strings.Casing.asIs, false, false, false, null);
			if (this.parentWindow.pinned)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.SetActive(true);
			}
		}
		if (this.plotRouteButton != null)
		{
			if (MapController.Instance.playerRoute != null)
			{
				this.plotRouteJuice.gameObject.SetActive(true);
				this.plotRouteJuice.Pulsate(true, false);
			}
			else
			{
				this.plotRouteJuice.Pulsate(false, false);
				this.plotRouteJuice.gameObject.SetActive(false);
			}
			MapController.Instance.OnPlotRoute += this.OnNewRoutePlotted;
			MapController.Instance.OnRemoveRoute += this.OnRouteRemoved;
			this.UpdateRouteTooltip();
		}
		if (this.fastTravelButton != null)
		{
			if (Game.Instance.allowAutoTravel)
			{
				this.autoTravelJuice.gameObject.SetActive(Player.Instance.autoTravelActive);
				this.autoTravelJuice.Pulsate(Player.Instance.autoTravelActive, false);
				Player.Instance.OnExecuteAutoTravel += this.OnFastTravelStarted;
				Player.Instance.OnEndAutoTravel += this.OnFastTravelEnded;
				Player.Instance.OnNewGameLocation += this.UpdateFastTravelAvailability;
				this.UpdateAutoTravelTooltip();
			}
			else
			{
				Object.Destroy(this.fastTravelButton.gameObject);
			}
		}
		this.CheckEnabled();
	}

	// Token: 0x06001E0F RID: 7695 RVA: 0x001A5CB8 File Offset: 0x001A3EB8
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.CheckEnabled;
		this.parentWindow.OnWindowRefresh -= this.CheckEnabled;
		if (this.plotRouteButton != null)
		{
			MapController.Instance.OnPlotRoute -= this.OnNewRoutePlotted;
			MapController.Instance.OnRemoveRoute -= this.OnRouteRemoved;
		}
		if (this.fastTravelButton != null)
		{
			Player.Instance.OnExecuteAutoTravel -= this.OnFastTravelStarted;
			Player.Instance.OnEndAutoTravel -= this.OnFastTravelEnded;
			Player.Instance.OnNewGameLocation -= this.UpdateFastTravelAvailability;
		}
	}

	// Token: 0x06001E10 RID: 7696 RVA: 0x001A5DA1 File Offset: 0x001A3FA1
	public void OnNewRoutePlotted()
	{
		this.plotRouteJuice.gameObject.SetActive(true);
		this.plotRouteJuice.Pulsate(true, false);
		this.UpdateRouteTooltip();
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x001A5DC7 File Offset: 0x001A3FC7
	public void OnRouteRemoved()
	{
		this.plotRouteJuice.Pulsate(false, false);
		this.plotRouteJuice.gameObject.SetActive(false);
		this.UpdateRouteTooltip();
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x001A5DED File Offset: 0x001A3FED
	public void OnFastTravelStarted()
	{
		this.autoTravelJuice.gameObject.SetActive(true);
		this.autoTravelJuice.Pulsate(true, false);
		this.UpdateAutoTravelTooltip();
	}

	// Token: 0x06001E13 RID: 7699 RVA: 0x001A5E13 File Offset: 0x001A4013
	public void OnFastTravelEnded()
	{
		this.autoTravelJuice.Pulsate(false, false);
		this.autoTravelJuice.gameObject.SetActive(false);
		this.UpdateAutoTravelTooltip();
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x001A5E3C File Offset: 0x001A403C
	private void UpdateRouteTooltip()
	{
		if (this.plotRouteButton != null && this.plotRouteButton.tooltip != null)
		{
			this.plotRouteButton.tooltip.mainDictionaryKey = "plotroute";
			this.plotRouteButton.tooltip.detailDictionaryKey = "plotroute_detail";
			this.plotRouteButton.tooltip.GetText();
		}
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x001A5EA4 File Offset: 0x001A40A4
	private void UpdateAutoTravelTooltip()
	{
		if (this.fastTravelButton != null && this.fastTravelButton.tooltip != null)
		{
			if (this.fastTravelEnabled)
			{
				this.fastTravelButton.tooltip.mainDictionaryKey = "fasttravelenable";
				this.fastTravelButton.tooltip.detailDictionaryKey = "fasttravel_detail";
			}
			else
			{
				this.fastTravelButton.tooltip.mainDictionaryKey = "autotravelenable";
				this.fastTravelButton.tooltip.detailDictionaryKey = "autotravel_detail";
			}
			this.fastTravelButton.tooltip.GetText();
		}
	}

	// Token: 0x06001E16 RID: 7702 RVA: 0x001A5F44 File Offset: 0x001A4144
	public void CheckEnabled()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("Interface: Check for location key in parent window...", 2);
		}
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		if (this.locateOnMapButton != null && this.plotRouteButton != null)
		{
			if (this.parentWindow.evidenceKeys.Contains(Evidence.DataKey.location))
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("Interface: Location key found", 2);
				}
				this.locateOnMapButton.SetInteractable(true);
				this.plotRouteButton.SetInteractable(true);
				this.fastTravelButton.SetInteractable(true);
			}
			else
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("Interface: Location key not found", 2);
				}
				this.locateOnMapButton.SetInteractable(false);
				this.plotRouteButton.SetInteractable(false);
				this.fastTravelButton.SetInteractable(false);
			}
		}
		this.UpdateFastTravelAvailability();
	}

	// Token: 0x06001E17 RID: 7703 RVA: 0x001A603C File Offset: 0x001A423C
	private void UpdateFastTravelAvailability()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.fastTravelEnabled = false;
		if (this.fastTravelButton != null && this.parentWindow != null)
		{
			EvidenceLocation evidenceLocation = this.parentWindow.passedEvidence as EvidenceLocation;
			if (evidenceLocation != null && evidenceLocation.locationController != null)
			{
				if ((Player.Instance.home == Player.Instance.currentGameLocation || Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, Player.Instance.currentGameLocation)) && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.fastTravelFromApartment) > 0f)
				{
					this.fastTravelEnabled = true;
				}
				if (!this.fastTravelEnabled && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.fastTravelToApartment) > 0f)
				{
					if (Player.Instance.home == evidenceLocation.locationController || Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, evidenceLocation.locationController))
					{
						this.fastTravelEnabled = true;
					}
					else
					{
						this.fastTravelEnabled = false;
					}
				}
				if (this.fastTravelEnabled)
				{
					this.fastTravelButton.icon.sprite = MapController.Instance.fastTravelIcon;
				}
				else
				{
					this.fastTravelButton.icon.sprite = MapController.Instance.autoTravelIcon;
				}
				this.UpdateAutoTravelTooltip();
			}
		}
	}

	// Token: 0x06001E18 RID: 7704 RVA: 0x001A61A6 File Offset: 0x001A43A6
	public void OnLocateOnMap()
	{
		MapController.Instance.LocateEvidenceOnMap(this.parentWindow.passedEvidence);
	}

	// Token: 0x06001E19 RID: 7705 RVA: 0x001A61C0 File Offset: 0x001A43C0
	public void OnPlotRoute()
	{
		if (this.parentWindow == null)
		{
			return;
		}
		bool flag = this.parentWindow.passedEvidence is EvidenceLocation;
		EvidenceBuilding evidenceBuilding = this.parentWindow.passedEvidence as EvidenceBuilding;
		NewGameLocation newGameLocation = null;
		if (MapController.Instance.playerRoute != null)
		{
			newGameLocation = MapController.Instance.playerRoute.GetDestinationLocation();
		}
		if (flag)
		{
			if (MapController.Instance.playerRoute != null)
			{
				MapController.Instance.playerRoute.Remove();
			}
		}
		else if (evidenceBuilding != null && MapController.Instance.playerRoute != null)
		{
			MapController.Instance.playerRoute.Remove();
		}
		if (newGameLocation != null && this.parentWindow.passedEvidence.controller == newGameLocation)
		{
			return;
		}
		MapController.Instance.PlotPlayerRoute(this.parentWindow.passedEvidence);
	}

	// Token: 0x06001E1A RID: 7706 RVA: 0x001A6290 File Offset: 0x001A4490
	public void OnFastTravel()
	{
		if (this.fastTravelButton != null && this.fastTravelButton.interactable && Game.Instance.allowAutoTravel)
		{
			NewGameLocation newGameLocation = null;
			if (Player.Instance.autoTravelActive)
			{
				if (MapController.Instance.playerRoute != null)
				{
					newGameLocation = MapController.Instance.playerRoute.GetDestinationLocation();
				}
				Player.Instance.EndAutoTravel();
			}
			if (this.parentWindow != null)
			{
				if (newGameLocation != null && this.parentWindow.passedEvidence.controller == newGameLocation)
				{
					return;
				}
				Player.Instance.ExecuteAutoTravel(this.parentWindow.passedEvidence, this.fastTravelEnabled);
			}
		}
	}

	// Token: 0x06001E1B RID: 7707 RVA: 0x001A634C File Offset: 0x001A454C
	public void OnAcceptJob()
	{
		if (CasePanelController.Instance.activeCases.Count < GameplayControls.Instance.maxCases)
		{
			this.parentWindow.passedEvidence.interactable.jobParent.AcceptJob();
			this.parentWindow.SetWorldInteraction(false);
			Object.Destroy(base.gameObject);
			this.parentWindow.passedEvidence.interactable.RemoveFromPlacement();
			return;
		}
		PopupMessageController.Instance.PopupMessage("TooManyCases", true, false, "Cancel", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
	}

	// Token: 0x06001E1C RID: 7708 RVA: 0x001A6400 File Offset: 0x001A4600
	public void OnTakeItem()
	{
		Interactable interactable = this.parentWindow.passedEvidence.interactable;
		if (interactable.preset.name == "Key")
		{
			ActionController.Instance.TakeKey(interactable, null, Player.Instance);
		}
		else
		{
			interactable.SetInInventory(Player.Instance);
			CasePanelController.Instance.PinToCasePanel(CasePanelController.Instance.activeCase, this.parentWindow.passedEvidence, this.parentWindow.passedKeys, true, default(Vector2), false);
		}
		this.parentWindow.CloseWindow(true);
		if (this.parentWindow.pinned)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x040027E7 RID: 10215
	public InfoWindow parentWindow;

	// Token: 0x040027E8 RID: 10216
	[Header("Location Controls")]
	public ButtonController locateOnMapButton;

	// Token: 0x040027E9 RID: 10217
	public ButtonController plotRouteButton;

	// Token: 0x040027EA RID: 10218
	public ButtonController fastTravelButton;

	// Token: 0x040027EB RID: 10219
	public JuiceController plotRouteJuice;

	// Token: 0x040027EC RID: 10220
	public JuiceController autoTravelJuice;

	// Token: 0x040027ED RID: 10221
	public bool fastTravelEnabled;

	// Token: 0x040027EE RID: 10222
	[Header("Job Posting Controls")]
	public ButtonController acceptJobButton;

	// Token: 0x040027EF RID: 10223
	public TextMeshProUGUI acceptJobText;

	// Token: 0x040027F0 RID: 10224
	[Header("Take Item Controls")]
	public ButtonController takeItemButton;

	// Token: 0x040027F1 RID: 10225
	public TextMeshProUGUI takeItemText;
}
