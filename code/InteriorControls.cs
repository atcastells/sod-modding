using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000800 RID: 2048
public class InteriorControls : MonoBehaviour
{
	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06002613 RID: 9747 RVA: 0x001EA16C File Offset: 0x001E836C
	public static InteriorControls Instance
	{
		get
		{
			return InteriorControls._instance;
		}
	}

	// Token: 0x06002614 RID: 9748 RVA: 0x001EA174 File Offset: 0x001E8374
	private void Awake()
	{
		if (InteriorControls._instance != null && InteriorControls._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			InteriorControls._instance = this;
		}
		if (!SessionData.Instance.isFloorEdit)
		{
			this.pulsingLightswitch = Object.Instantiate<Material>(this.newLightswitchMaterial);
			this.pulsingLightswitchSwitch = Object.Instantiate<Material>(this.newLightswithSwitchMaterial);
		}
	}

	// Token: 0x040042A0 RID: 17056
	[Header("Colours")]
	public List<Color> woods = new List<Color>();

	// Token: 0x040042A1 RID: 17057
	[Space(7f)]
	[Tooltip("Default variation used for things like ceiling lights")]
	public MaterialGroupPreset.MaterialVariation defaultVariation;

	// Token: 0x040042A2 RID: 17058
	[Header("Presets")]
	public InteractablePreset cctvPreset;

	// Token: 0x040042A3 RID: 17059
	public InteractablePreset peekUnderDoor;

	// Token: 0x040042A4 RID: 17060
	public InteractablePreset lightswitch;

	// Token: 0x040042A5 RID: 17061
	public AddressPreset lobbyAddressPreset;

	// Token: 0x040042A6 RID: 17062
	public StairwellPreset defaultStairwell;

	// Token: 0x040042A7 RID: 17063
	public StairwellPreset defaultStairwellInverted;

	// Token: 0x040042A8 RID: 17064
	public StairwellPreset defaultStairwellLarge;

	// Token: 0x040042A9 RID: 17065
	public InteractablePreset elevatorUpButton;

	// Token: 0x040042AA RID: 17066
	public InteractablePreset elevatorDownButton;

	// Token: 0x040042AB RID: 17067
	public InteractablePreset elevatorControls;

	// Token: 0x040042AC RID: 17068
	public InteractablePreset doorC;

	// Token: 0x040042AD RID: 17069
	public InteractablePreset keypad;

	// Token: 0x040042AE RID: 17070
	public InteractablePreset cruncher;

	// Token: 0x040042AF RID: 17071
	public InteractablePreset door;

	// Token: 0x040042B0 RID: 17072
	public InteractablePreset activeCodebreaker;

	// Token: 0x040042B1 RID: 17073
	public InteractablePreset activeDoorWedge;

	// Token: 0x040042B2 RID: 17074
	public InteractablePreset activeTracker;

	// Token: 0x040042B3 RID: 17075
	public InteractablePreset activeFlashbomb;

	// Token: 0x040042B4 RID: 17076
	public InteractablePreset activeIncapacitator;

	// Token: 0x040042B5 RID: 17077
	public InteractablePreset flowersAffair;

	// Token: 0x040042B6 RID: 17078
	public InteractablePreset storageBox;

	// Token: 0x040042B7 RID: 17079
	public InteractablePreset paperclip;

	// Token: 0x040042B8 RID: 17080
	public InteractablePreset salesLedger;

	// Token: 0x040042B9 RID: 17081
	public InteractablePreset telephoneRouter;

	// Token: 0x040042BA RID: 17082
	public InteractablePreset telephoneRouterDoor;

	// Token: 0x040042BB RID: 17083
	public InteractablePreset saleNote;

	// Token: 0x040042BC RID: 17084
	public CruncherAppPreset loginApp;

	// Token: 0x040042BD RID: 17085
	[Header("Gadgets")]
	public InteractablePreset codebreaker;

	// Token: 0x040042BE RID: 17086
	public InteractablePreset doorWedge;

	// Token: 0x040042BF RID: 17087
	public InteractablePreset tracker;

	// Token: 0x040042C0 RID: 17088
	public InteractablePreset flashbomb;

	// Token: 0x040042C1 RID: 17089
	public InteractablePreset incapacitator;

	// Token: 0x040042C2 RID: 17090
	public InteractablePreset printReader;

	// Token: 0x040042C3 RID: 17091
	public InteractablePreset handcuffs;

	// Token: 0x040042C4 RID: 17092
	public InteractablePreset policeBadge;

	// Token: 0x040042C5 RID: 17093
	public InteractablePreset lockpickKit;

	// Token: 0x040042C6 RID: 17094
	public InteractablePreset detectiveStuff;

	// Token: 0x040042C7 RID: 17095
	public InteractablePreset briefcaseCustom;

	// Token: 0x040042C8 RID: 17096
	public InteractablePreset codebreakerUsed;

	// Token: 0x040042C9 RID: 17097
	public InteractablePreset doorWedgeUsed;

	// Token: 0x040042CA RID: 17098
	[Space(7f)]
	public FirstPersonItem FPCodebreaker;

	// Token: 0x040042CB RID: 17099
	public FirstPersonItem FPCamera;

	// Token: 0x040042CC RID: 17100
	public FirstPersonItem FPHandcuffs;

	// Token: 0x040042CD RID: 17101
	public FirstPersonItem FPNewspaper;

	// Token: 0x040042CE RID: 17102
	[Space(7f)]
	public MurderWeaponPreset fistsWeapon;

	// Token: 0x040042CF RID: 17103
	[Header("Generation")]
	public float roomRankingRandomThreshold = 0.66f;

	// Token: 0x040042D0 RID: 17104
	[Tooltip("The minimum distance of two light zones (how close ceiling lights can spawn")]
	public float lightZoneMinDistance = 5f;

	// Token: 0x040042D1 RID: 17105
	[Tooltip("Limit to one area light per room (currently causing a weird bug when areas intersect)")]
	public bool oneAreaLightPerRoom = true;

	// Token: 0x040042D2 RID: 17106
	[Tooltip("The maximum number of furniture clusters per room (default)")]
	public int maxClustersPerRoom = 18;

	// Token: 0x040042D3 RID: 17107
	[Tooltip("Multiply the number of nodes by this to get the default number of cluster placement attempts")]
	public float roomSizeClusterAttemptMultiplier = 1.75f;

	// Token: 0x040042D4 RID: 17108
	public GameObject bug;

	// Token: 0x040042D5 RID: 17109
	[Header("Clue Items")]
	public InteractablePreset businessCard;

	// Token: 0x040042D6 RID: 17110
	public InteractablePreset workRota;

	// Token: 0x040042D7 RID: 17111
	public InteractablePreset employmentContractHome;

	// Token: 0x040042D8 RID: 17112
	public InteractablePreset employmentContractWork;

	// Token: 0x040042D9 RID: 17113
	public InteractablePreset workID;

	// Token: 0x040042DA RID: 17114
	public InteractablePreset diary;

	// Token: 0x040042DB RID: 17115
	public InteractablePreset photo;

	// Token: 0x040042DC RID: 17116
	public InteractablePreset namePlacard;

	// Token: 0x040042DD RID: 17117
	public InteractablePreset employeePhoto;

	// Token: 0x040042DE RID: 17118
	public InteractablePreset birthdayCard;

	// Token: 0x040042DF RID: 17119
	public InteractablePreset note;

	// Token: 0x040042E0 RID: 17120
	public InteractablePreset letter;

	// Token: 0x040042E1 RID: 17121
	public InteractablePreset moneyLots;

	// Token: 0x040042E2 RID: 17122
	public InteractablePreset travelReceipt;

	// Token: 0x040042E3 RID: 17123
	public InteractablePreset vmailLetter;

	// Token: 0x040042E4 RID: 17124
	public InteractablePreset vmailPrintout;

	// Token: 0x040042E5 RID: 17125
	public InteractablePreset surveillancePrintout;

	// Token: 0x040042E6 RID: 17126
	public InteractablePreset vmailPrintoutStatic;

	// Token: 0x040042E7 RID: 17127
	public InteractablePreset key;

	// Token: 0x040042E8 RID: 17128
	public InteractablePreset keyTabletopOnly;

	// Token: 0x040042E9 RID: 17129
	public SubObjectClassPreset keyHidingPlace;

	// Token: 0x040042EA RID: 17130
	public InteractablePreset noodleBox;

	// Token: 0x040042EB RID: 17131
	public SubObjectClassPreset telephone;

	// Token: 0x040042EC RID: 17132
	public SubObjectClassPreset payphone;

	// Token: 0x040042ED RID: 17133
	public SubObjectClassPreset fridge;

	// Token: 0x040042EE RID: 17134
	public InteractablePreset suitcase;

	// Token: 0x040042EF RID: 17135
	public InteractablePreset hairpin;

	// Token: 0x040042F0 RID: 17136
	public InteractablePreset residentsContract;

	// Token: 0x040042F1 RID: 17137
	public InteractablePreset birthCertificate;

	// Token: 0x040042F2 RID: 17138
	public InteractablePreset bankStatement;

	// Token: 0x040042F3 RID: 17139
	public InteractablePreset medicalDetails;

	// Token: 0x040042F4 RID: 17140
	public InteractablePreset homeFilePreset;

	// Token: 0x040042F5 RID: 17141
	public EvidencePreset homeFile;

	// Token: 0x040042F6 RID: 17142
	public List<InteractablePreset> clothesOnFloor = new List<InteractablePreset>();

	// Token: 0x040042F7 RID: 17143
	public InteractablePreset bookShelf;

	// Token: 0x040042F8 RID: 17144
	public InteractablePreset bookNonShelf;

	// Token: 0x040042F9 RID: 17145
	public InteractablePreset bookNonShelfSecret;

	// Token: 0x040042FA RID: 17146
	public InteractablePreset receipt;

	// Token: 0x040042FB RID: 17147
	public InteractablePreset flyer;

	// Token: 0x040042FC RID: 17148
	public InteractablePreset document;

	// Token: 0x040042FD RID: 17149
	public InteractablePreset fieldsAd;

	// Token: 0x040042FE RID: 17150
	public InteractablePreset policeSupportFlyer;

	// Token: 0x040042FF RID: 17151
	public InteractablePreset toothbrush;

	// Token: 0x04004300 RID: 17152
	public InteractablePreset painkillers;

	// Token: 0x04004301 RID: 17153
	public InteractablePreset bandage;

	// Token: 0x04004302 RID: 17154
	public InteractablePreset splint;

	// Token: 0x04004303 RID: 17155
	public InteractablePreset binNote;

	// Token: 0x04004304 RID: 17156
	public InteractablePreset crumpledPaper;

	// Token: 0x04004305 RID: 17157
	public InteractablePreset handgun;

	// Token: 0x04004306 RID: 17158
	public InteractablePreset silencer;

	// Token: 0x04004307 RID: 17159
	public InteractablePreset ammo1;

	// Token: 0x04004308 RID: 17160
	public InteractablePreset coffeeHomemade;

	// Token: 0x04004309 RID: 17161
	public InteractablePreset teaHomemade;

	// Token: 0x0400430A RID: 17162
	public InteractablePreset stovetopKettle;

	// Token: 0x0400430B RID: 17163
	public InteractablePreset streetCrimeScene;

	// Token: 0x0400430C RID: 17164
	public InteractablePreset creditCard;

	// Token: 0x0400430D RID: 17165
	public InteractablePreset donorCard;

	// Token: 0x0400430E RID: 17166
	public SubObjectClassPreset sideJobHiddenItemClass;

	// Token: 0x0400430F RID: 17167
	[Header("Misc References")]
	public DoorPreset defaultDoor;

	// Token: 0x04004310 RID: 17168
	public RoomConfiguration bedroom;

	// Token: 0x04004311 RID: 17169
	public RoomConfiguration lounge;

	// Token: 0x04004312 RID: 17170
	public RoomConfiguration kitchen;

	// Token: 0x04004313 RID: 17171
	public RoomConfiguration closet;

	// Token: 0x04004314 RID: 17172
	public FurnitureClass bed;

	// Token: 0x04004315 RID: 17173
	public FurnitureClass bedsideCabinet;

	// Token: 0x04004316 RID: 17174
	public FurnitureClass safe;

	// Token: 0x04004317 RID: 17175
	public FurnitureClass television;

	// Token: 0x04004318 RID: 17176
	public FurnitureClass telephoneTable;

	// Token: 0x04004319 RID: 17177
	public InteractablePreset deskLamp;

	// Token: 0x0400431A RID: 17178
	public InteractablePreset bedsideLamp;

	// Token: 0x0400431B RID: 17179
	public InteractablePreset cityDirectory;

	// Token: 0x0400431C RID: 17180
	public GameObject roomAreaLight;

	// Token: 0x0400431D RID: 17181
	public Material cameraOffMaterial;

	// Token: 0x0400431E RID: 17182
	public Material cameraOnMaterial;

	// Token: 0x0400431F RID: 17183
	public Material cameraFocusMaterial;

	// Token: 0x04004320 RID: 17184
	public Material cameraAlertMaterial;

	// Token: 0x04004321 RID: 17185
	public Material newLightswitchMaterial;

	// Token: 0x04004322 RID: 17186
	public Material newLightswithSwitchMaterial;

	// Token: 0x04004323 RID: 17187
	public Material pulsingLightswitch;

	// Token: 0x04004324 RID: 17188
	public Material pulsingLightswitchSwitch;

	// Token: 0x04004325 RID: 17189
	public Color pulseColor = Color.white;

	// Token: 0x04004326 RID: 17190
	public FurnitureCluster bedCluster;

	// Token: 0x04004327 RID: 17191
	public FurnitureCluster noticeBoardCluster;

	// Token: 0x04004328 RID: 17192
	public FurnitureCluster deskCluster;

	// Token: 0x04004329 RID: 17193
	public FurnitureCluster breakerBoxCluster;

	// Token: 0x0400432A RID: 17194
	[ReorderableList]
	public List<GameObject> housePlantPool = new List<GameObject>();

	// Token: 0x0400432B RID: 17195
	public Color housePlantColour1 = Color.white;

	// Token: 0x0400432C RID: 17196
	public Color housePlantColour2 = Color.white;

	// Token: 0x0400432D RID: 17197
	[Tooltip("Use this in various places where we want to set up a robbery")]
	[ReorderableList]
	public List<InteractablePreset> valuableItems = new List<InteractablePreset>();

	// Token: 0x0400432E RID: 17198
	public SyncDiskPreset chapterRewardSyncDisk;

	// Token: 0x0400432F RID: 17199
	public SyncDiskPreset chapterFlophouseSyncDisk;

	// Token: 0x04004330 RID: 17200
	public List<InteractablePreset> meetupConsumables = new List<InteractablePreset>();

	// Token: 0x04004331 RID: 17201
	[Header("Room Types")]
	public RoomConfiguration nullConfig;

	// Token: 0x04004332 RID: 17202
	public RoomTypePreset nullRoomType;

	// Token: 0x04004333 RID: 17203
	public RoomTypePreset bedroomType;

	// Token: 0x04004334 RID: 17204
	[Header("Air Ducts")]
	public float airDuctYOffset = 0.2f;

	// Token: 0x04004335 RID: 17205
	public DoorPairPreset wallVentTop;

	// Token: 0x04004336 RID: 17206
	public DoorPairPreset wallVentUpper;

	// Token: 0x04004337 RID: 17207
	public DoorPairPreset wallVentLower;

	// Token: 0x04004338 RID: 17208
	public DoorPairPreset wallNormal;

	// Token: 0x04004339 RID: 17209
	public DoorPairPreset wallVentUpperWithTopSpace;

	// Token: 0x0400433A RID: 17210
	public DoorPairPreset wallVentLowerWithTopSpace;

	// Token: 0x0400433B RID: 17211
	public Material ductMaterial;

	// Token: 0x0400433C RID: 17212
	public GameObject ductStraightModel;

	// Token: 0x0400433D RID: 17213
	public GameObject ductStraightWithPeekVent;

	// Token: 0x0400433E RID: 17214
	[Header("Misc Controls")]
	public float ceilingFanSpeed = 10f;

	// Token: 0x0400433F RID: 17215
	[Space(7f)]
	public List<InteriorControls.AirDuctOffset> airDuctModels = new List<InteriorControls.AirDuctOffset>();

	// Token: 0x04004340 RID: 17216
	private static InteriorControls _instance;

	// Token: 0x02000801 RID: 2049
	[Serializable]
	public class AirDuctOffset
	{
		// Token: 0x04004341 RID: 17217
		public string name;

		// Token: 0x04004342 RID: 17218
		public List<Vector3> offsets = new List<Vector3>();

		// Token: 0x04004343 RID: 17219
		public Vector3 rotation = Vector3.zero;

		// Token: 0x04004344 RID: 17220
		public List<GameObject> prefabs = new List<GameObject>();

		// Token: 0x04004345 RID: 17221
		public List<Texture2D> maps = new List<Texture2D>();
	}
}
