using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

// Token: 0x020006BA RID: 1722
[CreateAssetMenu(fileName = "artpreset_data", menuName = "Database/Decor/Art Preset")]
public class ArtPreset : SoCustomComparison
{
	// Token: 0x060024ED RID: 9453 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void GenerateColourMatching()
	{
	}

	// Token: 0x040030E0 RID: 12512
	public bool disable;

	// Token: 0x040030E1 RID: 12513
	[ShowAssetPreview(64, 64)]
	[Header("Art Settings")]
	public Texture2D texturePreview;

	// Token: 0x040030E2 RID: 12514
	public Material material;

	// Token: 0x040030E3 RID: 12515
	public List<ArtPreset.ArtOrientation> orientationCompatibility = new List<ArtPreset.ArtOrientation>();

	// Token: 0x040030E4 RID: 12516
	public float pixelScaleMultiplier = 0.025f;

	// Token: 0x040030E5 RID: 12517
	[Header("Suitability")]
	public bool allowInResidential = true;

	// Token: 0x040030E6 RID: 12518
	public bool allowInCommerical = true;

	// Token: 0x040030E7 RID: 12519
	public bool allowInLobby = true;

	// Token: 0x040030E8 RID: 12520
	public bool allowOnStreet;

	// Token: 0x040030E9 RID: 12521
	[Range(0f, 3f)]
	public int basePriority = 1;

	// Token: 0x040030EA RID: 12522
	[InfoBox("Colour matching gives a score out of 5", 0)]
	[Tooltip("used to match with room colour scheme")]
	[Space(7f)]
	public List<Color> colourMatching = new List<Color>();

	// Token: 0x040030EB RID: 12523
	[Range(0f, 5f)]
	public int colourMatchingScale = 5;

	// Token: 0x040030EC RID: 12524
	[Range(0f, 1f)]
	[Space(7f)]
	public float minimumWealth;

	// Token: 0x040030ED RID: 12525
	[Range(0f, 1f)]
	public float maximumWealth = 1f;

	// Token: 0x040030EE RID: 12526
	[Range(0f, 5f)]
	[InfoBox("The following gives a score out of x", 0)]
	[Space(5f)]
	public int roomMatchingScale = 5;

	// Token: 0x040030EF RID: 12527
	[Range(0f, 10f)]
	[Tooltip("0 = old fashioned/conservative, 1 = modern/liberal: Driven by the design style")]
	public int modernity = 5;

	// Token: 0x040030F0 RID: 12528
	[Range(0f, 10f)]
	[Tooltip("0 = informal/cosy, 1 = clean/souless: Driven by the room type.")]
	public int cleanness = 5;

	// Token: 0x040030F1 RID: 12529
	[Range(0f, 10f)]
	[Tooltip("0 = understated/quiet, 1 = loud/bold: Driven by the owner's personality")]
	public int loudness = 5;

	// Token: 0x040030F2 RID: 12530
	[Range(0f, 10f)]
	[Tooltip("0 = cold/hard, 1 = warm/sensitive: Driven by the owner's personality")]
	public int emotive = 5;

	// Token: 0x040030F3 RID: 12531
	public bool mustRequireTraitFromBelow;

	// Token: 0x040030F4 RID: 12532
	public List<ArtPreset.ArtPreference> traitModifiers = new List<ArtPreset.ArtPreference>();

	// Token: 0x040030F5 RID: 12533
	[Header("Dynamic Text")]
	public bool useDynamicText;

	// Token: 0x040030F6 RID: 12534
	[EnableIf("useDynamicText")]
	public ArtPreset.DynamicTextSouce dynamicTextSource;

	// Token: 0x040030F7 RID: 12535
	[EnableIf("useDynamicText")]
	public TMP_FontAsset textFont;

	// Token: 0x040030F8 RID: 12536
	[EnableIf("useDynamicText")]
	public Color textColour = Color.white;

	// Token: 0x040030F9 RID: 12537
	[EnableIf("useDynamicText")]
	public float textSize = 24f;

	// Token: 0x020006BB RID: 1723
	public enum ArtOrientation
	{
		// Token: 0x040030FB RID: 12539
		portrait,
		// Token: 0x040030FC RID: 12540
		landscape,
		// Token: 0x040030FD RID: 12541
		square,
		// Token: 0x040030FE RID: 12542
		poster,
		// Token: 0x040030FF RID: 12543
		litter,
		// Token: 0x04003100 RID: 12544
		wallGrimeTop,
		// Token: 0x04003101 RID: 12545
		wallGrimeBottom,
		// Token: 0x04003102 RID: 12546
		dynamicClue,
		// Token: 0x04003103 RID: 12547
		graffiti
	}

	// Token: 0x020006BC RID: 1724
	[Serializable]
	public class ArtPreference
	{
		// Token: 0x04003104 RID: 12548
		public CharacterTrait trait;

		// Token: 0x04003105 RID: 12549
		public int modifier = 1;
	}

	// Token: 0x020006BD RID: 1725
	public enum DynamicTextSouce
	{
		// Token: 0x04003107 RID: 12551
		weaponsDealerPassword,
		// Token: 0x04003108 RID: 12552
		blackMarketTraderPassword
	}
}
