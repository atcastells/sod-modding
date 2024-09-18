using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E6 RID: 2022
[CreateAssetMenu(fileName = "windowtab_data", menuName = "Database/Window Tab Style")]
public class WindowTabPreset : SoCustomComparison
{
	// Token: 0x04003EAE RID: 16046
	[Header("Naming")]
	public string tabName = "New Tab";

	// Token: 0x04003EAF RID: 16047
	public Color colour = Color.white;

	// Token: 0x04003EB0 RID: 16048
	public GameObject contentPrefab;

	// Token: 0x04003EB1 RID: 16049
	public WindowTabPreset.TabContentType contentType;

	// Token: 0x04003EB2 RID: 16050
	[Header("Scripts")]
	public bool scalableContent = true;

	// Token: 0x04003EB3 RID: 16051
	public bool fitToScaleX = true;

	// Token: 0x04003EB4 RID: 16052
	public bool fitToScaleY = true;

	// Token: 0x04003EB5 RID: 16053
	public bool zoomWithMouseWheel = true;

	// Token: 0x04003EB6 RID: 16054
	[Header("Scroll")]
	public bool scrollBars = true;

	// Token: 0x04003EB7 RID: 16055
	public ScrollRect.MovementType scrollRestrcition;

	// Token: 0x04003EB8 RID: 16056
	[Header("Content")]
	public string displayContentWithTag = string.Empty;

	// Token: 0x020007E7 RID: 2023
	public enum TabContentType
	{
		// Token: 0x04003EBA RID: 16058
		generated,
		// Token: 0x04003EBB RID: 16059
		message,
		// Token: 0x04003EBC RID: 16060
		facts,
		// Token: 0x04003EBD RID: 16061
		history,
		// Token: 0x04003EBE RID: 16062
		help,
		// Token: 0x04003EBF RID: 16063
		photoSelect,
		// Token: 0x04003EC0 RID: 16064
		shop,
		// Token: 0x04003EC1 RID: 16065
		objectives,
		// Token: 0x04003EC2 RID: 16066
		callLogsIncoming,
		// Token: 0x04003EC3 RID: 16067
		callLogsOutgoing,
		// Token: 0x04003EC4 RID: 16068
		passcodes,
		// Token: 0x04003EC5 RID: 16069
		phoneNumbers,
		// Token: 0x04003EC6 RID: 16070
		resolve,
		// Token: 0x04003EC7 RID: 16071
		results,
		// Token: 0x04003EC8 RID: 16072
		decor,
		// Token: 0x04003EC9 RID: 16073
		furnishings,
		// Token: 0x04003ECA RID: 16074
		colourPicker,
		// Token: 0x04003ECB RID: 16075
		floors,
		// Token: 0x04003ECC RID: 16076
		ceiling,
		// Token: 0x04003ECD RID: 16077
		materialKey,
		// Token: 0x04003ECE RID: 16078
		caseOptions,
		// Token: 0x04003ECF RID: 16079
		items,
		// Token: 0x04003ED0 RID: 16080
		itemSelect
	}
}
