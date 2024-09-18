using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007FB RID: 2043
public class InterfaceControls : MonoBehaviour
{
	// Token: 0x17000120 RID: 288
	// (get) Token: 0x0600260E RID: 9742 RVA: 0x001E9D76 File Offset: 0x001E7F76
	public static InterfaceControls Instance
	{
		get
		{
			return InterfaceControls._instance;
		}
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x001E9D80 File Offset: 0x001E7F80
	private void Awake()
	{
		if (InterfaceControls._instance != null && InterfaceControls._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			InterfaceControls._instance = this;
		}
		this.interactionControlTextNormalHex = ColorUtility.ToHtmlStringRGB(this.interactionControlTextColourNormal);
		this.interactionControlTextIllegalHex = ColorUtility.ToHtmlStringRGB(this.interactionControlTextColourIllegal);
	}

	// Token: 0x040041BF RID: 16831
	[Header("First Person")]
	[Tooltip("Minimum size of the first person interaction cursor")]
	public Vector2 interactionCursorMin = new Vector2(72f, 72f);

	// Token: 0x040041C0 RID: 16832
	[Tooltip("Maximum size of the first person interaction cursor")]
	public Vector2 interactionCursorMax = new Vector2(700f, 700f);

	// Token: 0x040041C1 RID: 16833
	[Tooltip("Speed of the first person interaction cursor")]
	public float interactionCursorSpeed = 1f;

	// Token: 0x040041C2 RID: 16834
	[Tooltip("Interaction text normal colour")]
	public Color interactionTextColour = Color.white;

	// Token: 0x040041C3 RID: 16835
	public Color interactionTextDistanceColour = Color.grey;

	// Token: 0x040041C4 RID: 16836
	public Color interactionTextIllegalColour = Color.red;

	// Token: 0x040041C5 RID: 16837
	[Tooltip("Low health indicator displays if player health is under this (normalized)")]
	public float lowHealthIndicatorThreshold = 0.1f;

	// Token: 0x040041C6 RID: 16838
	[Tooltip("Display control icons for this long (seconds)")]
	public float controlIconDisplayTime = 2f;

	// Token: 0x040041C7 RID: 16839
	[Header("Tooltips")]
	[Tooltip("Globally enable tooltips")]
	public bool enableTooltips = true;

	// Token: 0x040041C8 RID: 16840
	[Tooltip("The default tooltip width")]
	public float tooltipWidth = 300f;

	// Token: 0x040041C9 RID: 16841
	[Tooltip("The tooltip prefab")]
	public GameObject tooltipObjectPrefab;

	// Token: 0x040041CA RID: 16842
	[Tooltip("Delay before tooltip appears")]
	public float toolTipDelay = 0.35f;

	// Token: 0x040041CB RID: 16843
	[Tooltip("How fast the tooltip fades in")]
	public float toolTipFadeInSpeed = 3.85f;

	// Token: 0x040041CC RID: 16844
	[Tooltip("Default colour")]
	public Color defaultTextColour = Color.black;

	// Token: 0x040041CD RID: 16845
	public float contextMenuWidth = 190f;

	// Token: 0x040041CE RID: 16846
	[Header("Map")]
	public Sprite playerApartmentSprite;

	// Token: 0x040041CF RID: 16847
	[Header("Buttons & Icons")]
	[Tooltip("Unknown icon")]
	public Sprite unknownIconLarge;

	// Token: 0x040041D0 RID: 16848
	[Tooltip("Default company icon")]
	public Sprite companyIconLarge;

	// Token: 0x040041D1 RID: 16849
	[Tooltip("Double click delay")]
	public float doubleClickDelay = 0.4f;

	// Token: 0x040041D2 RID: 16850
	public Sprite stickyNoteButtonSprite;

	// Token: 0x040041D3 RID: 16851
	public Sprite lockedSprite;

	// Token: 0x040041D4 RID: 16852
	public Sprite unlockedSprite;

	// Token: 0x040041D5 RID: 16853
	[Tooltip("The HUD Canvas")]
	[Header("References")]
	public Canvas hudCanvas;

	// Token: 0x040041D6 RID: 16854
	[Tooltip("The HUD Canvas Rect")]
	public RectTransform hudCanvasRect;

	// Token: 0x040041D7 RID: 16855
	[Tooltip("The parent of speech bubbles")]
	public RectTransform speechBubbleParent;

	// Token: 0x040041D8 RID: 16856
	[Tooltip("Container for reticle")]
	public RectTransform reticleContainer;

	// Token: 0x040041D9 RID: 16857
	[Tooltip("Container for location text")]
	public RectTransform locationTextContainer;

	// Token: 0x040041DA RID: 16858
	[Tooltip("Toggles on/off for screenshot mode")]
	public List<RectTransform> screenshotModeToggleObjects = new List<RectTransform>();

	// Token: 0x040041DB RID: 16859
	public List<RectTransform> screenShotModeAllowDialogObjects = new List<RectTransform>();

	// Token: 0x040041DC RID: 16860
	[Header("HUD")]
	[Tooltip("Interaction control text colour")]
	public Color interactionControlTextColourNormal = Color.white;

	// Token: 0x040041DD RID: 16861
	public Color windowTakeItemIconDefaultColor = Color.black;

	// Token: 0x040041DE RID: 16862
	[NonSerialized]
	public string interactionControlTextNormalHex;

	// Token: 0x040041DF RID: 16863
	[Tooltip("Interaction control text colour")]
	public Color interactionControlTextColourIllegal = Color.red;

	// Token: 0x040041E0 RID: 16864
	[NonSerialized]
	public string interactionControlTextIllegalHex;

	// Token: 0x040041E1 RID: 16865
	[Tooltip("Game message system text display speed")]
	public float gameMessageTextRevealSpeed = 5f;

	// Token: 0x040041E2 RID: 16866
	[Tooltip("How long to display game message before removing")]
	public float gameMessageDestroyDelay = 1.5f;

	// Token: 0x040041E3 RID: 16867
	[Tooltip("Anchor for the weapon switch display")]
	public RectTransform weaponSwitchAnchor;

	// Token: 0x040041E4 RID: 16868
	[Tooltip("Anchor of the first person item")]
	public Transform firstPersonItemsParent;

	// Token: 0x040041E5 RID: 16869
	public Color interactionTextNormalColour = Color.white;

	// Token: 0x040041E6 RID: 16870
	[Tooltip("Colour of trespassing alert escalaction 0")]
	public Color trespassingEscalationZero;

	// Token: 0x040041E7 RID: 16871
	[Tooltip("Colour of trespassing alert escalaction 1")]
	public Color trespassingEscalationOne;

	// Token: 0x040041E8 RID: 16872
	public RectTransform fastForwardArrow;

	// Token: 0x040041E9 RID: 16873
	[Tooltip("Height of the movie bars when spotted")]
	public float movieBarHeight = 32f;

	// Token: 0x040041EA RID: 16874
	[Header("UI References")]
	public TextMeshProUGUI lockpicksText;

	// Token: 0x040041EB RID: 16875
	public TextMeshProUGUI cashText;

	// Token: 0x040041EC RID: 16876
	public TextMeshProUGUI socialRankText;

	// Token: 0x040041ED RID: 16877
	public TextMeshProUGUI plottedRouteText;

	// Token: 0x040041EE RID: 16878
	[Space(7f)]
	public AnimationCurve notificationGlowCurve;

	// Token: 0x040041EF RID: 16879
	public Color notificationColorMax = Color.white;

	// Token: 0x040041F0 RID: 16880
	public Color notificationColorMin = Color.clear;

	// Token: 0x040041F1 RID: 16881
	[Space(7f)]
	public Color messageGrey = Color.grey;

	// Token: 0x040041F2 RID: 16882
	public Color messageRed = Color.red;

	// Token: 0x040041F3 RID: 16883
	public Color messageGreen = Color.green;

	// Token: 0x040041F4 RID: 16884
	public Color messageBlue = Color.blue;

	// Token: 0x040041F5 RID: 16885
	public Color messageYellow = Color.yellow;

	// Token: 0x040041F6 RID: 16886
	[Header("Icons")]
	public Sprite starchLogo;

	// Token: 0x040041F7 RID: 16887
	public Sprite elGenLogo;

	// Token: 0x040041F8 RID: 16888
	public Sprite kensingtonLogo;

	// Token: 0x040041F9 RID: 16889
	public Sprite KaizenLogo;

	// Token: 0x040041FA RID: 16890
	public Sprite candorLogo;

	// Token: 0x040041FB RID: 16891
	public Sprite blackMarketLogo;

	// Token: 0x040041FC RID: 16892
	public List<InterfaceControls.IconConfig> iconReference = new List<InterfaceControls.IconConfig>();

	// Token: 0x040041FD RID: 16893
	[Header("Awareness HUD")]
	public Material arrow;

	// Token: 0x040041FE RID: 16894
	public Material spotted;

	// Token: 0x040041FF RID: 16895
	public Material speech;

	// Token: 0x04004200 RID: 16896
	public float awarenessDistanceThreshold = 20f;

	// Token: 0x04004201 RID: 16897
	[ColorUsage(true, true)]
	public Color spottedNormalEmission;

	// Token: 0x04004202 RID: 16898
	[ColorUsage(true, true)]
	public Color arrowNormalEmission;

	// Token: 0x04004203 RID: 16899
	[ColorUsage(true, true)]
	public Color awarenessAlertEmission;

	// Token: 0x04004204 RID: 16900
	[Header("UI Speech")]
	public Vector2 textSpaceBuffer = new Vector2(18f, 6f);

	// Token: 0x04004205 RID: 16901
	public float textBubbleMinWidth = 70f;

	// Token: 0x04004206 RID: 16902
	public float textBubbleMaxWidth = 282f;

	// Token: 0x04004207 RID: 16903
	public Color playerSpeechColour = Color.blue;

	// Token: 0x04004208 RID: 16904
	public Color callerSpeechColour = Color.red;

	// Token: 0x04004209 RID: 16905
	[Tooltip("On-screen speech talk speed")]
	public float visualTalkDisplaySpeed = 5f;

	// Token: 0x0400420A RID: 16906
	[Tooltip("How long to display speech before removing")]
	public float visualTalkDisplayDestroyDelay = 1.5f;

	// Token: 0x0400420B RID: 16907
	[Tooltip("Give extra on-screen time adding this amount per character")]
	public float visualTalkDisplayStringLengthModifier = 0.05f;

	// Token: 0x0400420C RID: 16908
	[Tooltip("On-screen speech text size")]
	public float visualTalkTextSize = 16f;

	// Token: 0x0400420D RID: 16909
	[Tooltip("The min and max scale of the speech bubble based on distance")]
	public Vector2 speechMinMaxScale = Vector2.one;

	// Token: 0x0400420E RID: 16910
	[Tooltip("The min and max scale of the ai indicator based on distance")]
	public Vector2 indicatorMinMaxScale = Vector2.one;

	// Token: 0x0400420F RID: 16911
	[Tooltip("The max distance at which ai indicators are active")]
	public float maxIndicatorDistance = 20f;

	// Token: 0x04004210 RID: 16912
	[Header("Objectives")]
	public Vector2 uiPointerDistanceRange = new Vector2(32f, 64f);

	// Token: 0x04004211 RID: 16913
	public TextMeshProUGUI caseSolvedText;

	// Token: 0x04004212 RID: 16914
	public List<CanvasRenderer> screenMessageFadeRenderers = new List<CanvasRenderer>();

	// Token: 0x04004213 RID: 16915
	public RectTransform resolveQuestionsDisplayParent;

	// Token: 0x04004214 RID: 16916
	public AnimationCurve caseSolvedAlphaAnim;

	// Token: 0x04004215 RID: 16917
	public AnimationCurve caseSolvedKerningAnim;

	// Token: 0x04004216 RID: 16918
	[Header("Handbook")]
	public Vector2 handbookWindowPosition = new Vector2(0f, 0f);

	// Token: 0x04004217 RID: 16919
	[Header("Player")]
	public Vector2 lightOrbSize = new Vector2(16f, 32f);

	// Token: 0x04004218 RID: 16920
	public AnimationCurve stealthModeOrbSizeTransitionIn;

	// Token: 0x04004219 RID: 16921
	public AnimationCurve stealthModeOrbSizeTransitionOut;

	// Token: 0x0400421A RID: 16922
	public RectTransform lightOrbRect;

	// Token: 0x0400421B RID: 16923
	public Image lightOrbFillImg;

	// Token: 0x0400421C RID: 16924
	public Image lightOrbOutline;

	// Token: 0x0400421D RID: 16925
	public Image seenImg;

	// Token: 0x0400421E RID: 16926
	public CanvasRenderer seenRenderer;

	// Token: 0x0400421F RID: 16927
	public JuiceController seenJuice;

	// Token: 0x04004220 RID: 16928
	[Space(7f)]
	public RectTransform interactionRect;

	// Token: 0x04004221 RID: 16929
	public RectTransform interactionULRect;

	// Token: 0x04004222 RID: 16930
	public RectTransform interactionURRect;

	// Token: 0x04004223 RID: 16931
	public RectTransform interactionBLRect;

	// Token: 0x04004224 RID: 16932
	public RectTransform interactionBRRect;

	// Token: 0x04004225 RID: 16933
	public List<Image> interactionFadeInImages = new List<Image>();

	// Token: 0x04004226 RID: 16934
	public List<Image> interactionBoundImages = new List<Image>();

	// Token: 0x04004227 RID: 16935
	public RectTransform interactionTextContainer;

	// Token: 0x04004228 RID: 16936
	public TextMeshProUGUI interactionText;

	// Token: 0x04004229 RID: 16937
	public RectTransform readingTextContainer;

	// Token: 0x0400422A RID: 16938
	public CanvasRenderer readingContainerRend;

	// Token: 0x0400422B RID: 16939
	public TextMeshProUGUI readingText;

	// Token: 0x0400422C RID: 16940
	public CanvasRenderer readingTextRend;

	// Token: 0x0400422D RID: 16941
	public Vector2 readingBoxMaxSize = new Vector2(820f, 300f);

	// Token: 0x0400422E RID: 16942
	public RectTransform haveKeyIcon;

	// Token: 0x0400422F RID: 16943
	public RectTransform lockedIcon;

	// Token: 0x04004230 RID: 16944
	public Image lockedImg;

	// Token: 0x04004231 RID: 16945
	public RectTransform forbiddenIcon;

	// Token: 0x04004232 RID: 16946
	public RectTransform seenIcon;

	// Token: 0x04004233 RID: 16947
	public TextMeshProUGUI lockStrengthText;

	// Token: 0x04004234 RID: 16948
	[Space(7f)]
	public RectTransform actionInteractionDisplay;

	// Token: 0x04004235 RID: 16949
	public RectTransform actionInteractionAnchor;

	// Token: 0x04004236 RID: 16950
	public TextMeshProUGUI actionInteractionText;

	// Token: 0x04004237 RID: 16951
	[Space(7f)]
	public Color unheardSoundIconColour = Color.white;

	// Token: 0x04004238 RID: 16952
	public Color heardSoundIconColour = Color.red;

	// Token: 0x04004239 RID: 16953
	[Tooltip("Width of the string")]
	[Header("Case Panel")]
	public Vector2 stringWidthRange = new Vector2(8f, 32f);

	// Token: 0x0400423A RID: 16954
	[Tooltip("How far away another evidence entry is pinned automatically")]
	public float autoPinDistance = 128f;

	// Token: 0x0400423B RID: 16955
	[Tooltip("When auto-pinning, the radius space needed to spawn evidence")]
	public float pinnedEvidenceRadius = 64f;

	// Token: 0x0400423C RID: 16956
	[Tooltip("When auto-pinning, the number of possible angle steps to position test")]
	public int angleStepsCount = 36;

	// Token: 0x0400423D RID: 16957
	public Rigidbody2D caseBoardRigidbody;

	// Token: 0x0400423E RID: 16958
	public RectTransform caseBoardCursorRBContainer;

	// Token: 0x0400423F RID: 16959
	public Rigidbody2D caseBoardCursorRigidbody;

	// Token: 0x04004240 RID: 16960
	public RectTransform caseBoardContentContainer;

	// Token: 0x04004241 RID: 16961
	public float pinnedLinearDrag = 1f;

	// Token: 0x04004242 RID: 16962
	public float movingLinearDrag;

	// Token: 0x04004243 RID: 16963
	[Tooltip("Image displayed as a screenshot to replace the rendered camera when in case mode")]
	public RawImage cameraScreenshot;

	// Token: 0x04004244 RID: 16964
	public RenderTexture cameraScreenshotRenderTex;

	// Token: 0x04004245 RID: 16965
	public float pinnedMovementIntertiaMultiplier = -30f;

	// Token: 0x04004246 RID: 16966
	[Header("Evidence")]
	[Tooltip("Default case file colour")]
	public Color defaultCaseFileColour = Color.white;

	// Token: 0x04004247 RID: 16967
	[Tooltip("Maximum number of item entries in evidence history")]
	public int maximumEvidenceItemHistory = 80;

	// Token: 0x04004248 RID: 16968
	[Tooltip("Interface customisable colours")]
	[ReorderableList]
	public List<InterfaceControls.PinColours> pinColours = new List<InterfaceControls.PinColours>();

	// Token: 0x04004249 RID: 16969
	[Tooltip("Displayed when the player has photograph information for a citizen")]
	public Sprite citizenPhoto;

	// Token: 0x0400424A RID: 16970
	[Tooltip("When true minimize evidence as soon as you pin it")]
	public bool minimizeEvidenceOnPinned;

	// Token: 0x0400424B RID: 16971
	[Tooltip("Evidence link colour")]
	public Color markedLinkColour;

	// Token: 0x0400424C RID: 16972
	[Tooltip("Neutral/Inactive colour")]
	public Color neutralColour = Color.white;

	// Token: 0x0400424D RID: 16973
	[Tooltip("Incriminating colour")]
	public Color incriminatingColour = Color.red;

	// Token: 0x0400424E RID: 16974
	[Tooltip("Innocent colour")]
	public Color innocentColour = Color.green;

	// Token: 0x0400424F RID: 16975
	public Texture2D nullPhotoReference;

	// Token: 0x04004250 RID: 16976
	[Header("Windows")]
	[Tooltip("The default window location")]
	public Vector2 defaultWindowLocation = new Vector2(0.66f, 0.15f);

	// Token: 0x04004251 RID: 16977
	[Tooltip("Offset applied to default window location per active window")]
	public Vector2 windowCountOffset = new Vector2(-0.05f, 0.05f);

	// Token: 0x04004252 RID: 16978
	[Tooltip("Speed of the minimize/restore animation")]
	public float minimizingAnimationSpeed = 12f;

	// Token: 0x04004253 RID: 16979
	[Tooltip("Colour of selection buttons when selected")]
	public Color selectionColour = Color.yellow;

	// Token: 0x04004254 RID: 16980
	[Tooltip("Colour of selection buttons when not selected")]
	public Color nonSelectionColour = Color.white;

	// Token: 0x04004255 RID: 16981
	[Tooltip("The close button X")]
	public Sprite closeSprite;

	// Token: 0x04004256 RID: 16982
	public Color closeColour = Color.red;

	// Token: 0x04004257 RID: 16983
	[Tooltip("The minimize sprite for the close button")]
	public Sprite minimizeSprite;

	// Token: 0x04004258 RID: 16984
	public Color minimizeColour = Color.blue;

	// Token: 0x04004259 RID: 16985
	[Header("Cursor Sprites")]
	public Texture2D normalCursor;

	// Token: 0x0400425A RID: 16986
	[Tooltip("Displayed when mousing over something that moves")]
	public Texture2D cursorMove;

	// Token: 0x0400425B RID: 16987
	[Tooltip("Displayed when mousing over something that can be resized")]
	public Texture2D cursorResizeHorizonal;

	// Token: 0x0400425C RID: 16988
	[Tooltip("Displayed when mousing over something that can be resized")]
	public Texture2D cursorResizeVertical;

	// Token: 0x0400425D RID: 16989
	[Tooltip("Displayed when mousing over something that can be resized")]
	public Texture2D cursorResizeDiagonalRightLeft;

	// Token: 0x0400425E RID: 16990
	[Tooltip("Displayed when mousing over something that can be resized")]
	public Texture2D cursorResizeDiagonalLeftRight;

	// Token: 0x0400425F RID: 16991
	[Tooltip("Displayed when mousing over something that needs targeting")]
	public Texture2D cursorTarget;

	// Token: 0x04004260 RID: 16992
	[Tooltip("Displayed when mousing over a button by default")]
	public Texture2D cursorButton;

	// Token: 0x04004261 RID: 16993
	[Tooltip("Displayed when mousing over text input field")]
	public Texture2D cursorTextEdit;

	// Token: 0x04004262 RID: 16994
	[Space(7f)]
	public Sprite reactionInvestigateSightSprite;

	// Token: 0x04004263 RID: 16995
	public Sprite reactionInvestigateSoundSprite;

	// Token: 0x04004264 RID: 16996
	public Sprite reactionPersueSprite;

	// Token: 0x04004265 RID: 16997
	public Sprite reactionSearchSprite;

	// Token: 0x04004266 RID: 16998
	public Sprite reactionAvoidSprite;

	// Token: 0x04004267 RID: 16999
	[Space(7f)]
	public Texture reactionInvestigateSightTex;

	// Token: 0x04004268 RID: 17000
	public Texture reactionInvestigateSoundTex;

	// Token: 0x04004269 RID: 17001
	public Texture reactionPersueTex;

	// Token: 0x0400426A RID: 17002
	public Texture reactionSearchTex;

	// Token: 0x0400426B RID: 17003
	public Texture reactionAvoidTex;

	// Token: 0x0400426C RID: 17004
	private static InterfaceControls _instance;

	// Token: 0x020007FC RID: 2044
	public enum Icon
	{
		// Token: 0x0400426E RID: 17006
		lookingGlass,
		// Token: 0x0400426F RID: 17007
		lightBulb,
		// Token: 0x04004270 RID: 17008
		key,
		// Token: 0x04004271 RID: 17009
		agent,
		// Token: 0x04004272 RID: 17010
		citizen,
		// Token: 0x04004273 RID: 17011
		pin,
		// Token: 0x04004274 RID: 17012
		footprint,
		// Token: 0x04004275 RID: 17013
		document,
		// Token: 0x04004276 RID: 17014
		door,
		// Token: 0x04004277 RID: 17015
		location,
		// Token: 0x04004278 RID: 17016
		questionMark,
		// Token: 0x04004279 RID: 17017
		eye,
		// Token: 0x0400427A RID: 17018
		books,
		// Token: 0x0400427B RID: 17019
		star,
		// Token: 0x0400427C RID: 17020
		building,
		// Token: 0x0400427D RID: 17021
		hand,
		// Token: 0x0400427E RID: 17022
		run,
		// Token: 0x0400427F RID: 17023
		money,
		// Token: 0x04004280 RID: 17024
		message,
		// Token: 0x04004281 RID: 17025
		lockpick,
		// Token: 0x04004282 RID: 17026
		notebook,
		// Token: 0x04004283 RID: 17027
		empty,
		// Token: 0x04004284 RID: 17028
		skull,
		// Token: 0x04004285 RID: 17029
		passedOut,
		// Token: 0x04004286 RID: 17030
		telephone,
		// Token: 0x04004287 RID: 17031
		printScanner,
		// Token: 0x04004288 RID: 17032
		resolve,
		// Token: 0x04004289 RID: 17033
		time,
		// Token: 0x0400428A RID: 17034
		tick,
		// Token: 0x0400428B RID: 17035
		cross,
		// Token: 0x0400428C RID: 17036
		camera,
		// Token: 0x0400428D RID: 17037
		vandalism,
		// Token: 0x0400428E RID: 17038
		robbery,
		// Token: 0x0400428F RID: 17039
		picture,
		// Token: 0x04004290 RID: 17040
		fist,
		// Token: 0x04004291 RID: 17041
		handcuffs,
		// Token: 0x04004292 RID: 17042
		trash,
		// Token: 0x04004293 RID: 17043
		food
	}

	// Token: 0x020007FD RID: 2045
	[Serializable]
	public class IconConfig
	{
		// Token: 0x04004294 RID: 17044
		public InterfaceControls.Icon iconType;

		// Token: 0x04004295 RID: 17045
		public Sprite sprite;
	}

	// Token: 0x020007FE RID: 2046
	public enum EvidenceColours
	{
		// Token: 0x04004297 RID: 17047
		red,
		// Token: 0x04004298 RID: 17048
		blue,
		// Token: 0x04004299 RID: 17049
		yellow,
		// Token: 0x0400429A RID: 17050
		green,
		// Token: 0x0400429B RID: 17051
		purple,
		// Token: 0x0400429C RID: 17052
		white,
		// Token: 0x0400429D RID: 17053
		black
	}

	// Token: 0x020007FF RID: 2047
	[Serializable]
	public class PinColours
	{
		// Token: 0x0400429E RID: 17054
		public InterfaceControls.EvidenceColours colour;

		// Token: 0x0400429F RID: 17055
		public Color actualColour;
	}
}
