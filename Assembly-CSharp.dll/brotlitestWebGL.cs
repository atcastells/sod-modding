using System;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class brotlitestWebGL : MonoBehaviour
{
	// Token: 0x06000043 RID: 67 RVA: 0x00004868 File Offset: 0x00002A68
	private void OnGUI()
	{
		GUI.Label(new Rect(10f, 10f, 500f, 40f), "Only for WebGL or tvOS.");
	}
}
