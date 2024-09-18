using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007E5 RID: 2021
[CreateAssetMenu(fileName = "windowstyle_data", menuName = "Database/Window Style")]
public class WindowStylePreset : SoCustomComparison
{
	// Token: 0x04003EA3 RID: 16035
	[Header("Interaction")]
	public bool closable = true;

	// Token: 0x04003EA4 RID: 16036
	public bool pinnable = true;

	// Token: 0x04003EA5 RID: 16037
	[Tooltip("If true this will always and only be able to be a world interaction")]
	public bool forceWorldInteraction;

	// Token: 0x04003EA6 RID: 16038
	[Tooltip("Use window focus mode (black screen behind the window)")]
	public bool useWindowFocusMode;

	// Token: 0x04003EA7 RID: 16039
	[Header("Resizing")]
	public bool resizable = true;

	// Token: 0x04003EA8 RID: 16040
	public Vector2 defaultSize = new Vector2(514f, 658f);

	// Token: 0x04003EA9 RID: 16041
	public Vector2 minSize = new Vector2(514f, 514f);

	// Token: 0x04003EAA RID: 16042
	public Vector2 maxSize = new Vector2(1000f, 1000f);

	// Token: 0x04003EAB RID: 16043
	[Space(7f)]
	[InfoBox("Used to make the window size relative to DDS document sizes: Adds this on to the document size to make the window size.", 0)]
	public Vector2 DDSadditionalSize = new Vector2(172f, 176f);

	// Token: 0x04003EAC RID: 16044
	[Header("Icons")]
	public Sprite overrideIcon;

	// Token: 0x04003EAD RID: 16045
	[Header("Tabs")]
	public List<WindowTabPreset> tabs = new List<WindowTabPreset>();
}
