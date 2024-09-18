using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

// Token: 0x02000745 RID: 1861
[CreateAssetMenu(fileName = "handwriting_data", menuName = "Database/Handwriting Preset")]
public class HandwritingPreset : SoCustomComparison
{
	// Token: 0x040036A6 RID: 13990
	[Header("Font")]
	public TMP_FontAsset fontAsset;

	// Token: 0x040036A7 RID: 13991
	[Header("Suitability")]
	public float baseChance = 0.1f;

	// Token: 0x040036A8 RID: 13992
	[InfoBox("If enabled: The below traits will be used to calculate the likihood of this being chosen vs others.", 0)]
	[ReorderableList]
	public List<CharacterTrait.TraitPickRule> characterTraits = new List<CharacterTrait.TraitPickRule>();
}
