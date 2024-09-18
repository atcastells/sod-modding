using System;
using UnityEngine;

// Token: 0x0200032C RID: 812
public class LoadResolutionController : MonoBehaviour
{
	// Token: 0x06001246 RID: 4678 RVA: 0x00103D5C File Offset: 0x00101F5C
	private void Awake()
	{
		Resolution currentResolution = Screen.currentResolution;
		FullScreenMode fullScreenMode = Screen.fullScreenMode;
		Screen.SetResolution(PlayerPrefs.GetInt("width", currentResolution.width), PlayerPrefs.GetInt("height", currentResolution.height), PlayerPrefs.GetInt("fullscreen", fullScreenMode), PlayerPrefs.GetInt("refresh", currentResolution.refreshRate));
	}
}
