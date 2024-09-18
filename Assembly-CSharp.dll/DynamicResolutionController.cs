using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020002BD RID: 701
public class DynamicResolutionController : MonoBehaviour
{
	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000F8E RID: 3982 RVA: 0x000DD530 File Offset: 0x000DB730
	// (set) Token: 0x06000F8F RID: 3983 RVA: 0x000DD538 File Offset: 0x000DB738
	public bool DynamicResolutionEnabled
	{
		get
		{
			return this.dynamicResolutionEnabled;
		}
		set
		{
			this.SetDynamicResolutionEnabled(value);
		}
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000F90 RID: 3984 RVA: 0x000DD541 File Offset: 0x000DB741
	// (set) Token: 0x06000F91 RID: 3985 RVA: 0x000DD549 File Offset: 0x000DB749
	public bool DLSSEnabled
	{
		get
		{
			return this.dlssEnabled;
		}
		set
		{
			this.SetDLSSEnabled(value);
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000F92 RID: 3986 RVA: 0x000DD552 File Offset: 0x000DB752
	public static DynamicResolutionController Instance
	{
		get
		{
			return DynamicResolutionController._instance;
		}
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x000DD559 File Offset: 0x000DB759
	private void Awake()
	{
		if (DynamicResolutionController._instance != null && DynamicResolutionController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		DynamicResolutionController._instance = this;
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x00002265 File Offset: 0x00000465
	private void Start()
	{
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x000DD588 File Offset: 0x000DB788
	public void SetDynamicResolutionEnabled(bool enable)
	{
		Game.Log("Menu: Dynamic resolution enabled: " + enable.ToString(), 2);
		this.dynamicResolutionEnabled = enable;
		foreach (HDAdditionalCameraData hdadditionalCameraData in this.AllHDCameras)
		{
			hdadditionalCameraData.allowDynamicResolution = enable;
		}
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x000DD5F8 File Offset: 0x000DB7F8
	public void SetDLSSEnabled(bool enable)
	{
		Game.Log("Menu: DLSS enabled: " + enable.ToString(), 2);
		this.dlssEnabled = enable;
		foreach (HDAdditionalCameraData hdadditionalCameraData in this.AllHDCameras)
		{
			if (enable)
			{
				hdadditionalCameraData.allowDynamicResolution = true;
			}
			hdadditionalCameraData.allowDeepLearningSuperSampling = enable;
			hdadditionalCameraData.deepLearningSuperSamplingUseOptimalSettings = true;
		}
		foreach (DropdownController dropdownController in MainMenuController.Instance.disableWithDynamicResolution)
		{
			if (!(dropdownController == null))
			{
				dropdownController.SetInteractalbe(!this.dlssEnabled);
			}
		}
		foreach (DropdownController dropdownController2 in MainMenuController.Instance.enableWithDynamicResolution)
		{
			if (!(dropdownController2 == null))
			{
				dropdownController2.SetInteractalbe(this.dlssEnabled);
			}
		}
		if (!this.dlssEnabled)
		{
			Game.Instance.SetAAMode(Game.Instance.aaMode);
		}
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x000DD744 File Offset: 0x000DB944
	public void SetDLSSQualityMode(DynamicResolutionController.DLSSQuality quality)
	{
		Game.Log("Menu: Set DLSS mode: " + quality.ToString(), 2);
		foreach (HDAdditionalCameraData hdadditionalCameraData in this.AllHDCameras)
		{
			uint deepLearningSuperSamplingQuality = this.ConvertDLSSQualityValue(quality);
			hdadditionalCameraData.deepLearningSuperSamplingQuality = deepLearningSuperSamplingQuality;
		}
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x000DD7BC File Offset: 0x000DB9BC
	private uint ConvertDLSSQualityValue(DynamicResolutionController.DLSSQuality quality)
	{
		switch (quality)
		{
		case DynamicResolutionController.DLSSQuality.MaximumPerformance:
			return 0U;
		case DynamicResolutionController.DLSSQuality.Balanced:
			return 1U;
		case DynamicResolutionController.DLSSQuality.MaximumQuality:
			return 2U;
		case DynamicResolutionController.DLSSQuality.UltraPerformance:
			return 3U;
		default:
			return 1U;
		}
	}

	// Token: 0x04001305 RID: 4869
	public List<HDAdditionalCameraData> AllHDCameras;

	// Token: 0x04001306 RID: 4870
	[SerializeField]
	private bool dynamicResolutionEnabled;

	// Token: 0x04001307 RID: 4871
	[SerializeField]
	private bool dlssEnabled;

	// Token: 0x04001308 RID: 4872
	private static DynamicResolutionController _instance;

	// Token: 0x020002BE RID: 702
	public enum DLSSQuality
	{
		// Token: 0x0400130A RID: 4874
		MaximumPerformance,
		// Token: 0x0400130B RID: 4875
		Balanced,
		// Token: 0x0400130C RID: 4876
		MaximumQuality,
		// Token: 0x0400130D RID: 4877
		UltraPerformance
	}
}
