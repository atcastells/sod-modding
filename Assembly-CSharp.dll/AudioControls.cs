using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020007E8 RID: 2024
public class AudioControls : MonoBehaviour
{
	// Token: 0x17000118 RID: 280
	// (get) Token: 0x060025EA RID: 9706 RVA: 0x001E8AA7 File Offset: 0x001E6CA7
	public static AudioControls Instance
	{
		get
		{
			return AudioControls._instance;
		}
	}

	// Token: 0x060025EB RID: 9707 RVA: 0x001E8AAE File Offset: 0x001E6CAE
	private void Awake()
	{
		if (AudioControls._instance != null && AudioControls._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		AudioControls._instance = this;
	}

	// Token: 0x04003ED1 RID: 16081
	[Header("Footsteps")]
	public AudioEvent footstepShoe;

	// Token: 0x04003ED2 RID: 16082
	public AudioEvent footstepBoot;

	// Token: 0x04003ED3 RID: 16083
	public AudioEvent footstepHeel;

	// Token: 0x04003ED4 RID: 16084
	public AudioEvent footstepWaterWade;

	// Token: 0x04003ED5 RID: 16085
	[Space(5f)]
	public AudioEvent playerFootstepShoe;

	// Token: 0x04003ED6 RID: 16086
	public AudioEvent playerFootstepBoot;

	// Token: 0x04003ED7 RID: 16087
	public AudioEvent playerFootstepHeel;

	// Token: 0x04003ED8 RID: 16088
	public AudioEvent playerFootstepDuct;

	// Token: 0x04003ED9 RID: 16089
	public AudioEvent playerWaterWade;

	// Token: 0x04003EDA RID: 16090
	[Space(5f)]
	public AudioEvent playerLandImpactMetal;

	// Token: 0x04003EDB RID: 16091
	public AudioEvent playerLandImpactConcrete;

	// Token: 0x04003EDC RID: 16092
	public AudioEvent playerLandImpactWood;

	// Token: 0x04003EDD RID: 16093
	[Space(5f)]
	public AudioEvent playerTripSound;

	// Token: 0x04003EDE RID: 16094
	[Header("AI: Sleeping")]
	public AudioEvent maleSnoreLight;

	// Token: 0x04003EDF RID: 16095
	public AudioEvent maleSnoreHeavy;

	// Token: 0x04003EE0 RID: 16096
	public AudioEvent maleYawn;

	// Token: 0x04003EE1 RID: 16097
	public AudioEvent maleSnort;

	// Token: 0x04003EE2 RID: 16098
	[Space(5f)]
	public AudioEvent femaleSnoreLight;

	// Token: 0x04003EE3 RID: 16099
	public AudioEvent femaleSnoreHeavy;

	// Token: 0x04003EE4 RID: 16100
	public AudioEvent femaleYawn;

	// Token: 0x04003EE5 RID: 16101
	public AudioEvent femaleSnort;

	// Token: 0x04003EE6 RID: 16102
	[Header("Objects")]
	public AudioEvent fridgeClose;

	// Token: 0x04003EE7 RID: 16103
	public AudioEvent fridegOpen;

	// Token: 0x04003EE8 RID: 16104
	[Space(5f)]
	public AudioEvent tvShow;

	// Token: 0x04003EE9 RID: 16105
	[Space(5f)]
	public List<AudioEvent> keypadButtons = new List<AudioEvent>();

	// Token: 0x04003EEA RID: 16106
	public AudioEvent keypadPress;

	// Token: 0x04003EEB RID: 16107
	public AudioEvent keypadClear;

	// Token: 0x04003EEC RID: 16108
	public AudioEvent keypadAccept;

	// Token: 0x04003EED RID: 16109
	public AudioEvent keypadDeny;

	// Token: 0x04003EEE RID: 16110
	public AudioEvent keypadArm;

	// Token: 0x04003EEF RID: 16111
	public AudioEvent payphoneMoneyIn;

	// Token: 0x04003EF0 RID: 16112
	[Space(5f)]
	public AudioEvent dialTone;

	// Token: 0x04003EF1 RID: 16113
	public AudioEvent hangUp;

	// Token: 0x04003EF2 RID: 16114
	public AudioEvent phoneLineActive;

	// Token: 0x04003EF3 RID: 16115
	public AudioEvent phoneLineRing;

	// Token: 0x04003EF4 RID: 16116
	public AudioEvent phoneLineEngaged;

	// Token: 0x04003EF5 RID: 16117
	public AudioEvent phoneConnect;

	// Token: 0x04003EF6 RID: 16118
	[Space(5f)]
	public AudioEvent bargeDoorContact;

	// Token: 0x04003EF7 RID: 16119
	public AudioEvent bargeDoorBreak;

	// Token: 0x04003EF8 RID: 16120
	[Space(5f)]
	public AudioEvent elevatorDing;

	// Token: 0x04003EF9 RID: 16121
	[Space(5f)]
	public AudioEvent neonSignLoopBig;

	// Token: 0x04003EFA RID: 16122
	public AudioEvent neonSignLoopSmall;

	// Token: 0x04003EFB RID: 16123
	[Space(5f)]
	public AudioEvent codebreakerSuccess;

	// Token: 0x04003EFC RID: 16124
	[Space(5f)]
	public AudioEvent elevatorMovement;

	// Token: 0x04003EFD RID: 16125
	[Space(5f)]
	public AudioEvent alarmPA;

	// Token: 0x04003EFE RID: 16126
	public AudioEvent drinkLoop;

	// Token: 0x04003EFF RID: 16127
	[Header("Player Actions")]
	public AudioEvent spawnPlayer;

	// Token: 0x04003F00 RID: 16128
	public AudioEvent throwObject;

	// Token: 0x04003F01 RID: 16129
	public AudioEvent lockpick;

	// Token: 0x04003F02 RID: 16130
	public AudioEvent lockpickMetal;

	// Token: 0x04003F03 RID: 16131
	public AudioEvent rummage;

	// Token: 0x04003F04 RID: 16132
	public AudioEvent flashlightOn;

	// Token: 0x04003F05 RID: 16133
	public AudioEvent flashlightOff;

	// Token: 0x04003F06 RID: 16134
	public AudioEvent handcuff;

	// Token: 0x04003F07 RID: 16135
	public AudioEvent handcuffArrestEnd;

	// Token: 0x04003F08 RID: 16136
	[FormerlySerializedAs("waterCoolerDrinkShort")]
	public AudioEvent waterCoolerRefillShort;

	// Token: 0x04003F09 RID: 16137
	[FormerlySerializedAs("waterCoolerDrinkLong")]
	public AudioEvent waterCoolerRefill;

	// Token: 0x04003F0A RID: 16138
	public AudioEvent moveObjectsToStorage;

	// Token: 0x04003F0B RID: 16139
	[Header("Player effects")]
	public AudioEvent brokenBone;

	// Token: 0x04003F0C RID: 16140
	[Header("Ambience")]
	public AudioEvent ambienceWind;

	// Token: 0x04003F0D RID: 16141
	public AudioEvent ambienceRain;

	// Token: 0x04003F0E RID: 16142
	public AudioEvent ambienceCity;

	// Token: 0x04003F0F RID: 16143
	public AudioEvent ambiencePA;

	// Token: 0x04003F10 RID: 16144
	[Header("Weather")]
	public AudioEvent thunder;

	// Token: 0x04003F11 RID: 16145
	[Header("Interface: FPS")]
	public AudioEvent gameMessage;

	// Token: 0x04003F12 RID: 16146
	public AudioEvent socialLevelUp;

	// Token: 0x04003F13 RID: 16147
	public AudioEvent revealCaseResults;

	// Token: 0x04003F14 RID: 16148
	public AudioEvent gainSocialCredit;

	// Token: 0x04003F15 RID: 16149
	public AudioEvent newMessage;

	// Token: 0x04003F16 RID: 16150
	public AudioEvent bountyAdded;

	// Token: 0x04003F17 RID: 16151
	public AudioEvent bountyEscapeComplete;

	// Token: 0x04003F18 RID: 16152
	public AudioEvent trespassMinor;

	// Token: 0x04003F19 RID: 16153
	public AudioEvent trespassMajor;

	// Token: 0x04003F1A RID: 16154
	public AudioEvent enforcerScannerMsg;

	// Token: 0x04003F1B RID: 16155
	[Space(5f)]
	public AudioEvent speakEvent;

	// Token: 0x04003F1C RID: 16156
	public AudioEvent shoutEvent;

	// Token: 0x04003F1D RID: 16157
	public AudioEvent screamEvent;

	// Token: 0x04003F1E RID: 16158
	[Space(5f)]
	public AudioEvent threatLoop;

	// Token: 0x04003F1F RID: 16159
	[Space(5f)]
	public AudioEvent typewriter;

	// Token: 0x04003F20 RID: 16160
	public AudioEvent typeWriterSpace;

	// Token: 0x04003F21 RID: 16161
	[Tooltip("The delay between audio event typewriter keystrokes.")]
	public float typewriterKeystrokeEventDelay = 0.03f;

	// Token: 0x04003F22 RID: 16162
	[Tooltip("The delay between audio event typewriter keystrokes.")]
	public float typewriterSpaceEventDelay = 0.03f;

	// Token: 0x04003F23 RID: 16163
	[Header("Snapshots")]
	public AudioEvent interfaceEvent;

	// Token: 0x04003F24 RID: 16164
	public AudioEvent combatSnapshot;

	// Token: 0x04003F25 RID: 16165
	public AudioEvent trespassingSnapshot;

	// Token: 0x04003F26 RID: 16166
	public AudioEvent syncMachineSnapshot;

	// Token: 0x04003F27 RID: 16167
	public AudioEvent musicOnlySnapshot;

	// Token: 0x04003F28 RID: 16168
	[Header("Interface: Case Board")]
	[Tooltip("Three main buttons in top left of case board")]
	public AudioEvent panelIconButton;

	// Token: 0x04003F29 RID: 16169
	[Tooltip("Create stick note button next to panel icons in case board")]
	public AudioEvent stickyNoteCreateButton;

	// Token: 0x04003F2A RID: 16170
	[Tooltip("Grabbing a folder by the pin")]
	public AudioEvent folderPickUp;

	// Token: 0x04003F2B RID: 16171
	[Tooltip("Placing a folder back down")]
	public AudioEvent folderPutDown;

	// Token: 0x04003F2C RID: 16172
	[Tooltip("Grabbing a sticky note by the pin")]
	public AudioEvent stickyNotePickUp;

	// Token: 0x04003F2D RID: 16173
	[Tooltip("Placing a sticky note back down")]
	public AudioEvent stickyNotePutDown;

	// Token: 0x04003F2E RID: 16174
	[Tooltip("Whenever the map slides into view")]
	public AudioEvent mapSlideIn;

	// Token: 0x04003F2F RID: 16175
	[Tooltip("Whenever the map slides out of view")]
	public AudioEvent mapSlideOut;

	// Token: 0x04003F30 RID: 16176
	[Tooltip("Crossing out an item on the caseboard")]
	public AudioEvent crossOut;

	// Token: 0x04003F31 RID: 16177
	[Tooltip("Unpinning an item (effectively deleting it)")]
	public AudioEvent unPin;

	// Token: 0x04003F32 RID: 16178
	[Header("Interface: Folder")]
	[Tooltip("'x' in the corner of items")]
	public AudioEvent closeButton;

	// Token: 0x04003F33 RID: 16179
	[Tooltip("Large vertically aligned tabs on the right of an open note or folder")]
	public AudioEvent tab;

	// Token: 0x04003F34 RID: 16180
	[Tooltip("Open a folder")]
	public AudioEvent folderOpen;

	// Token: 0x04003F35 RID: 16181
	[Tooltip("Close a folder")]
	public AudioEvent folderClose;

	// Token: 0x04003F36 RID: 16182
	[Tooltip("Open a sticky note")]
	public AudioEvent stickyOpen;

	// Token: 0x04003F37 RID: 16183
	[Tooltip("Close a sticky note")]
	public AudioEvent stickyClose;

	// Token: 0x04003F38 RID: 16184
	[Tooltip("Forward through a multipage document")]
	public AudioEvent pageForward;

	// Token: 0x04003F39 RID: 16185
	[Tooltip("Back through a multipage document")]
	public AudioEvent pageBack;

	// Token: 0x04003F3A RID: 16186
	[Tooltip("Minimise a folder")]
	public AudioEvent minimiseButton;

	// Token: 0x04003F3B RID: 16187
	[Tooltip("Right click on an item top line header to bring-up a pop-up list of vertically stacked buttons")]
	public AudioEvent itemEditAppear;

	// Token: 0x04003F3C RID: 16188
	[Tooltip("ItemEditButton")]
	public AudioEvent itemEditButton;

	// Token: 0x04003F3D RID: 16189
	[Tooltip("Central (set route) button at the bottom of a location folder page")]
	public AudioEvent locationSetRouteButton;

	// Token: 0x04003F3E RID: 16190
	[Tooltip("Left and Right hand side buttons at the bottom of a location folder page")]
	public AudioEvent locationButton;

	// Token: 0x04003F3F RID: 16191
	[Tooltip("Red highlighted links that appear within a page")]
	public AudioEvent inLineLink;

	// Token: 0x04003F40 RID: 16192
	[Header("Interface: Sticky Note")]
	[Tooltip("Vertical strip of icons in top left of a note (drawing colours etc.)")]
	public AudioEvent stickyNoteEdit;

	// Token: 0x04003F41 RID: 16193
	[Tooltip("Wiping a drawing from an item")]
	public AudioEvent clearDrawing;

	// Token: 0x04003F42 RID: 16194
	[Header("Interface: Menus")]
	[Tooltip("tickbox")]
	public AudioEvent tickbox;

	// Token: 0x04003F43 RID: 16195
	[Tooltip("e.g. cancel")]
	public AudioEvent mainButtonBack;

	// Token: 0x04003F44 RID: 16196
	[Tooltip("e.g. continue")]
	public AudioEvent mainButtonForward;

	// Token: 0x04003F45 RID: 16197
	[Tooltip("same style as ButtonMainBack/Forward but for buttons that don't have that back/forward paradigm attached")]
	public AudioEvent mainButton;

	// Token: 0x04003F46 RID: 16198
	[Tooltip("Vertical column of small buttons within map panel")]
	[Header("Interface: Map")]
	public AudioEvent mapControlButton;

	// Token: 0x04003F47 RID: 16199
	[Header("Interface: Upgrades")]
	public AudioEvent syncDiskInstall;

	// Token: 0x04003F48 RID: 16200
	public AudioEvent syncDiskUninstall;

	// Token: 0x04003F49 RID: 16201
	public AudioEvent syncDiskUpgrade;

	// Token: 0x04003F4A RID: 16202
	public AudioEvent syncDiskInstallStatus;

	// Token: 0x04003F4B RID: 16203
	[Header("Interface: Inventory")]
	public AudioEvent pickUpMoney;

	// Token: 0x04003F4C RID: 16204
	public AudioEvent pickUpItem;

	// Token: 0x04003F4D RID: 16205
	public AudioEvent pickUpLockpicks;

	// Token: 0x04003F4E RID: 16206
	public AudioEvent dropItem;

	// Token: 0x04003F4F RID: 16207
	public AudioEvent purchaseItem;

	// Token: 0x04003F50 RID: 16208
	public AudioEvent motionTrackerPing;

	// Token: 0x04003F51 RID: 16209
	public AudioEvent printScannerLoop;

	// Token: 0x04003F52 RID: 16210
	public AudioEvent printScannerHolster;

	// Token: 0x04003F53 RID: 16211
	public AudioEvent printScannerSelect;

	// Token: 0x04003F54 RID: 16212
	[Header("Interface: Misc")]
	public AudioEvent caseComplete;

	// Token: 0x04003F55 RID: 16213
	public AudioEvent caseUnsolved;

	// Token: 0x04003F56 RID: 16214
	public AudioEvent newMurderCase;

	// Token: 0x04003F57 RID: 16215
	public AudioEvent furniturePlacement;

	// Token: 0x04003F58 RID: 16216
	[Header("In-Game Computers")]
	public AudioEvent computerHDDLoading;

	// Token: 0x04003F59 RID: 16217
	public AudioEvent computerCursorClick;

	// Token: 0x04003F5A RID: 16218
	public AudioEvent computerKeyboardKey;

	// Token: 0x04003F5B RID: 16219
	public AudioEvent computerInvalidPasscode;

	// Token: 0x04003F5C RID: 16220
	public AudioEvent computerValidPasscode;

	// Token: 0x04003F5D RID: 16221
	public AudioEvent computerPrint;

	// Token: 0x04003F5E RID: 16222
	[Header("Watch")]
	public AudioEvent watchAlarm;

	// Token: 0x04003F5F RID: 16223
	public AudioEvent timeForward;

	// Token: 0x04003F60 RID: 16224
	public AudioEvent timeBackward;

	// Token: 0x04003F61 RID: 16225
	public AudioEvent watchToggleHoursMinutes;

	// Token: 0x04003F62 RID: 16226
	public AudioEvent setAlarm;

	// Token: 0x04003F63 RID: 16227
	public AudioEvent wristwatchTickTimeLoop;

	// Token: 0x04003F64 RID: 16228
	[Header("Security Systems")]
	public AudioEvent sentryGunFire;

	// Token: 0x04003F65 RID: 16229
	public AudioEvent sentryGunSearchPulse;

	// Token: 0x04003F66 RID: 16230
	public AudioEvent sentryGunTargetAcquire;

	// Token: 0x04003F67 RID: 16231
	public AudioEvent sentryGunTurnLoop;

	// Token: 0x04003F68 RID: 16232
	public AudioEvent securityCameraAlert;

	// Token: 0x04003F69 RID: 16233
	[Header("Combat")]
	public AudioEvent collapseOnFloor;

	// Token: 0x04003F6A RID: 16234
	[Space(5f)]
	public AudioEvent punchHitFabric;

	// Token: 0x04003F6B RID: 16235
	public AudioEvent punchHitWood;

	// Token: 0x04003F6C RID: 16236
	public AudioEvent punchHitCarpet;

	// Token: 0x04003F6D RID: 16237
	public AudioEvent punchHitPlaster;

	// Token: 0x04003F6E RID: 16238
	public AudioEvent punchHitConcrete;

	// Token: 0x04003F6F RID: 16239
	public AudioEvent punchHitTile;

	// Token: 0x04003F70 RID: 16240
	public AudioEvent punchHitGlass;

	// Token: 0x04003F71 RID: 16241
	public AudioEvent punchHitMetal;

	// Token: 0x04003F72 RID: 16242
	public AudioEvent punchHitFlesh;

	// Token: 0x04003F73 RID: 16243
	public AudioEvent punchHitPlayer;

	// Token: 0x04003F74 RID: 16244
	public AudioEvent punchHitWall;

	// Token: 0x04003F75 RID: 16245
	[Space(5f)]
	public AudioEvent sniperKillShot;

	// Token: 0x04003F76 RID: 16246
	public AudioEvent sniperStreetShot;

	// Token: 0x04003F77 RID: 16247
	[Header("Grenades")]
	public AudioEvent grenadeBeep;

	// Token: 0x04003F78 RID: 16248
	public AudioEvent flashBombDetonate;

	// Token: 0x04003F79 RID: 16249
	public AudioEvent incapacitatorDetonate;

	// Token: 0x04003F7A RID: 16250
	private static AudioControls _instance;
}
