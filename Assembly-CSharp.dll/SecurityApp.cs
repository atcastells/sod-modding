using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002AB RID: 683
public class SecurityApp : CruncherAppContent
{
	// Token: 0x06000F22 RID: 3874 RVA: 0x000D7C38 File Offset: 0x000D5E38
	public override void OnSetup()
	{
		base.OnSetup();
		this.titleText.text = Strings.Get("computer", "Security Systems", Strings.Casing.asIs, false, false, false, null);
		this.camOnText.text = Strings.Get("computer", "On", Strings.Casing.upperCase, false, false, false, null);
		this.camOffText.text = Strings.Get("computer", "Off", Strings.Casing.upperCase, false, false, false, null);
		this.alarmOnText.text = Strings.Get("computer", "On", Strings.Casing.upperCase, false, false, false, null);
		this.alarmOffText.text = Strings.Get("computer", "Off", Strings.Casing.upperCase, false, false, false, null);
		this.cameras.AddRange(this.controller.ic.interactable.node.gameLocation.securityCameras);
		if (this.controller.ic.interactable.node.gameLocation.thisAsAddress != null && this.controller.ic.interactable.node.gameLocation.thisAsAddress.company != null && this.controller.ic.interactable.node.gameLocation.thisAsAddress.company.preset.controlsBuildingSurveillance)
		{
			foreach (NewAddress newAddress in this.controller.ic.interactable.node.building.lobbies)
			{
				this.cameras.AddRange(newAddress.securityCameras);
			}
		}
		this.locationstampText.text = string.Empty;
		this.locationstampTextShadow.text = this.locationstampText.text;
		if (this.cameras.Count > 0)
		{
			this.SetCamera(this.cameras[0]);
		}
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x000D7E44 File Offset: 0x000D6044
	public void SetCamera(Interactable newSelection)
	{
		this.selectedCamera = newSelection;
		if (this.selectedCamera != null)
		{
			Game.Log("Gameplay: Security set new camera: " + this.selectedCamera.node.room.name, 2);
			this.selectedSentries.Clear();
			if (this.selectedCamera.node.gameLocation.thisAsAddress != null)
			{
				this.selectedSentries.AddRange(this.selectedCamera.node.gameLocation.thisAsAddress.sentryGuns.FindAll((Interactable item) => item.node.room == this.selectedCamera.node.room));
			}
			this.cameraSelectionText.text = Strings.Get("names.rooms", this.selectedCamera.node.room.preset.name, Strings.Casing.asIs, false, false, false, null);
			this.locationstampText.text = this.selectedCamera.node.room.GetName();
			this.locationstampTextShadow.text = this.locationstampText.text;
			this.camOnButton.interactable = true;
			this.camOffButton.interactable = true;
			this.camUpdateTimer = 0f;
			this.UpdateCamStatus();
		}
		else
		{
			Game.Log("Gameplay: Security set null camera", 2);
			this.selectedSentries.Clear();
			this.locationstampText.text = string.Empty;
			this.locationstampTextShadow.text = this.locationstampText.text;
			this.camOnButton.interactable = false;
			this.camOffButton.interactable = false;
		}
		if (this.selectedCamera != null && this.selectedCamera.sw0)
		{
			this.captureDisplay.color = Color.white;
			return;
		}
		this.captureDisplay.color = Color.black;
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x000D8008 File Offset: 0x000D6208
	private void Update()
	{
		if (SessionData.Instance.play && Player.Instance.computerInteractable == this.controller.ic.interactable && this.selectedCamera != null && this.selectedCamera.sw0)
		{
			if (this.camUpdateTimer <= 0f)
			{
				SceneRecorder.SceneCapture scene = this.selectedCamera.sceneRecorder.ExecuteCapture(false, false, false);
				this.captureDisplay.texture = SceneCapture.Instance.GetSurveillanceScene(scene, false);
				this.camUpdateTimer = 1f;
			}
			this.camUpdateTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x000D80AC File Offset: 0x000D62AC
	public void CameraSelection(int addSelection)
	{
		int num = this.cameras.IndexOf(this.selectedCamera);
		num += addSelection;
		if (num < 0)
		{
			num += this.cameras.Count;
		}
		else if (num >= this.cameras.Count)
		{
			num -= this.cameras.Count;
		}
		if (this.cameras.Count > 0)
		{
			this.SetCamera(this.cameras[num]);
		}
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x000D8120 File Offset: 0x000D6320
	public void AlarmTargetSelection(int addSelection)
	{
		if (this.selectedCamera != null && this.selectedCamera.node.gameLocation.thisAsAddress != null)
		{
			int num = 0;
			if (this.selectedCamera.node.gameLocation.thisAsAddress.addressPreset != null && this.selectedCamera.node.gameLocation.thisAsAddress.addressPreset.useOwnSecuritySystem)
			{
				num = (int)this.selectedCamera.node.gameLocation.thisAsAddress.targetMode;
			}
			else if (this.selectedCamera.node.gameLocation.building != null)
			{
				num = (int)this.selectedCamera.node.building.targetMode;
			}
			num += addSelection;
			if (num < 0)
			{
				num = 4;
			}
			else if (num > 4)
			{
				num = 0;
			}
			if (this.selectedCamera.node.gameLocation.thisAsAddress.addressPreset != null && this.selectedCamera.node.gameLocation.thisAsAddress.addressPreset.useOwnSecuritySystem)
			{
				this.selectedCamera.node.gameLocation.thisAsAddress.SetTargetMode((NewBuilding.AlarmTargetMode)num, true);
			}
			else if (this.selectedCamera.node.gameLocation.building != null)
			{
				this.selectedCamera.node.building.SetTargetMode((NewBuilding.AlarmTargetMode)num, true);
			}
		}
		this.UpdateCamStatus();
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x000D6564 File Offset: 0x000D4764
	public void ExitButton()
	{
		this.controller.OnAppExit();
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x000D829C File Offset: 0x000D649C
	public void SetCamActiveButton(bool val)
	{
		if (this.selectedCamera != null)
		{
			this.selectedCamera.SetSwitchState(val, Player.Instance, true, false, false);
			if (val)
			{
				this.selectedCamera.SetValue(this.selectedCamera.GetSecurityStrength());
			}
			else
			{
				this.selectedCamera.SetValue(0f);
			}
		}
		this.UpdateCamStatus();
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x000D82F8 File Offset: 0x000D64F8
	public void SetAlarmActiveButton(bool val)
	{
		if (this.selectedCamera != null && this.selectedCamera.node.gameLocation.thisAsAddress != null)
		{
			float num;
			NewBuilding.AlarmTargetMode alarmTargetMode;
			List<Human> list;
			if (this.selectedCamera.node.gameLocation.IsAlarmActive(out num, out alarmTargetMode, out list))
			{
				if (!val)
				{
					this.selectedCamera.node.gameLocation.thisAsAddress.SetAlarm(false, null);
				}
			}
			else if (val)
			{
				this.selectedCamera.node.gameLocation.thisAsAddress.SetAlarm(true, null);
			}
		}
		this.UpdateCamStatus();
	}

	// Token: 0x06000F2A RID: 3882 RVA: 0x000D8390 File Offset: 0x000D6590
	private void UpdateCamStatus()
	{
		if (this.selectedCamera != null)
		{
			if (this.selectedCamera.sw0)
			{
				this.camOnText.text = ">" + Strings.Get("computer", "On", Strings.Casing.upperCase, false, false, false, null) + "<";
				this.camOffText.text = Strings.Get("computer", "Off", Strings.Casing.upperCase, false, false, false, null);
			}
			else
			{
				this.camOnText.text = Strings.Get("computer", "On", Strings.Casing.upperCase, false, false, false, null);
				this.camOffText.text = ">" + Strings.Get("computer", "Off", Strings.Casing.upperCase, false, false, false, null) + "<";
			}
			float num;
			NewBuilding.AlarmTargetMode alarmTargetMode;
			List<Human> list;
			if (this.selectedCamera.node.gameLocation.IsAlarmActive(out num, out alarmTargetMode, out list))
			{
				this.alarmOnText.text = ">" + Strings.Get("computer", "On", Strings.Casing.upperCase, false, false, false, null) + "<";
				this.alarmOffText.text = Strings.Get("computer", "Off", Strings.Casing.upperCase, false, false, false, null);
			}
			else
			{
				this.alarmOnText.text = Strings.Get("computer", "On", Strings.Casing.upperCase, false, false, false, null);
				this.alarmOffText.text = ">" + Strings.Get("computer", "Off", Strings.Casing.upperCase, false, false, false, null) + "<";
			}
			if (this.selectedCamera.node.gameLocation.thisAsAddress != null && this.selectedCamera.node.gameLocation.thisAsAddress.addressPreset != null && this.selectedCamera.node.gameLocation.thisAsAddress.addressPreset.useOwnSecuritySystem)
			{
				this.targetSelectionText.text = Strings.Get("computer", this.selectedCamera.node.gameLocation.thisAsAddress.targetMode.ToString(), Strings.Casing.asIs, false, false, false, null);
			}
			else if (this.selectedCamera.node.gameLocation.building != null)
			{
				this.targetSelectionText.text = Strings.Get("computer", this.selectedCamera.node.gameLocation.building.targetMode.ToString(), Strings.Casing.asIs, false, false, false, null);
			}
			else
			{
				this.targetSelectionText.text = string.Empty;
			}
		}
		if (this.selectedCamera != null && this.selectedCamera.sw0)
		{
			this.captureDisplay.color = Color.white;
			return;
		}
		this.captureDisplay.color = Color.black;
	}

	// Token: 0x0400124A RID: 4682
	[Header("Components")]
	public TextMeshProUGUI titleText;

	// Token: 0x0400124B RID: 4683
	public TextMeshProUGUI cameraSelectionText;

	// Token: 0x0400124C RID: 4684
	public TextMeshProUGUI targetSelectionText;

	// Token: 0x0400124D RID: 4685
	public TextMeshProUGUI locationstampText;

	// Token: 0x0400124E RID: 4686
	public TextMeshProUGUI locationstampTextShadow;

	// Token: 0x0400124F RID: 4687
	public RenderTexture renderTexturePrefab;

	// Token: 0x04001250 RID: 4688
	public RawImage captureDisplay;

	// Token: 0x04001251 RID: 4689
	public RectTransform captureRect;

	// Token: 0x04001252 RID: 4690
	public Button camOnButton;

	// Token: 0x04001253 RID: 4691
	public Button camOffButton;

	// Token: 0x04001254 RID: 4692
	public Button alarmOnButton;

	// Token: 0x04001255 RID: 4693
	public Button alarmOffButton;

	// Token: 0x04001256 RID: 4694
	public RectTransform camDisplayPageRect;

	// Token: 0x04001257 RID: 4695
	[Space(5f)]
	public TextMeshProUGUI camOnText;

	// Token: 0x04001258 RID: 4696
	public TextMeshProUGUI camOffText;

	// Token: 0x04001259 RID: 4697
	public TextMeshProUGUI alarmOnText;

	// Token: 0x0400125A RID: 4698
	public TextMeshProUGUI alarmOffText;

	// Token: 0x0400125B RID: 4699
	[Header("State")]
	public List<Interactable> cameras = new List<Interactable>();

	// Token: 0x0400125C RID: 4700
	[NonSerialized]
	public Interactable selectedCamera;

	// Token: 0x0400125D RID: 4701
	public List<Interactable> selectedSentries = new List<Interactable>();

	// Token: 0x0400125E RID: 4702
	private float camUpdateTimer;
}
