using System;
using UnityEngine;

// Token: 0x020006E5 RID: 1765
[CreateAssetMenu(fileName = "colourscheme_data", menuName = "Database/Colour Scheme")]
public class ColourSchemePreset : SoCustomComparison
{
	// Token: 0x040032A7 RID: 12967
	[Header("Colours")]
	public Color primary1;

	// Token: 0x040032A8 RID: 12968
	public Color secondary1;

	// Token: 0x040032A9 RID: 12969
	public Color neutral;

	// Token: 0x040032AA RID: 12970
	public Color secondary2;

	// Token: 0x040032AB RID: 12971
	public Color primary2;

	// Token: 0x040032AC RID: 12972
	[Tooltip("0 = old fashioned/conservative, 1 = modern/liberal: Driven by the design style")]
	[Range(0f, 10f)]
	[Header("Settings")]
	public int modernity = 5;

	// Token: 0x040032AD RID: 12973
	[Range(0f, 10f)]
	[Tooltip("0 = informal/cosy, 1 = clean/souless: Driven by the room type.")]
	public int cleanness = 5;

	// Token: 0x040032AE RID: 12974
	[Range(0f, 10f)]
	[Tooltip("0 = understated/quiet, 1 = loud/bold: Driven by the owner's personality")]
	public int loudness = 5;

	// Token: 0x040032AF RID: 12975
	[Tooltip("0 = cold/hard, 1 = warm/sensitive: Driven by the owner's personality")]
	[Range(0f, 10f)]
	public int emotive = 5;
}
