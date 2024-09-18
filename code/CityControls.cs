using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020007ED RID: 2029
public class CityControls : MonoBehaviour
{
	// Token: 0x1700011A RID: 282
	// (get) Token: 0x060025F5 RID: 9717 RVA: 0x001E8EC4 File Offset: 0x001E70C4
	public static CityControls Instance
	{
		get
		{
			return CityControls._instance;
		}
	}

	// Token: 0x060025F6 RID: 9718 RVA: 0x001E8ECB File Offset: 0x001E70CB
	private void Awake()
	{
		if (CityControls._instance != null && CityControls._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			CityControls._instance = this;
		}
		this.weatherSettings.skyboxGradientGrading.Sort();
	}

	// Token: 0x04003FE6 RID: 16358
	[Header("City")]
	public string wardName = "Downtown";

	// Token: 0x04003FE7 RID: 16359
	public string cityCustoms = "Whitaker Customs Department";

	// Token: 0x04003FE8 RID: 16360
	public string cityCustomsAbr = "WCD";

	// Token: 0x04003FE9 RID: 16361
	public string cityTax = "Whitaker Revenue Service";

	// Token: 0x04003FEA RID: 16362
	public string cityTaxAbr = "WRS";

	// Token: 0x04003FEB RID: 16363
	public string cityCurrency = "$";

	// Token: 0x04003FEC RID: 16364
	[Header("Size")]
	[ReorderableList]
	public List<CityControls.CitySize> citySizes = new List<CityControls.CitySize>();

	// Token: 0x04003FED RID: 16365
	[Header("Infrastructure")]
	[Tooltip("Exact measurements of groundmap tiles in unity units (metres)")]
	public Vector3 cityTileSize = new Vector3(3.15f, 3.15f, 2f);

	// Token: 0x04003FEE RID: 16366
	[Tooltip("Tiles are multiplied by this per ground map unit for pathmap grid")]
	public int tileMultiplier = 5;

	// Token: 0x04003FEF RID: 16367
	[Tooltip("Nodes are multiplied by this per tile")]
	public int nodeMultiplier = 3;

	// Token: 0x04003FF0 RID: 16368
	[Tooltip("Maximum size of a city block")]
	public int maxBlockSize = 7;

	// Token: 0x04003FF1 RID: 16369
	[Tooltip("Chance block will expand into adjacent tile")]
	public float blockExpandChance = 100f;

	// Token: 0x04003FF2 RID: 16370
	[Tooltip("?")]
	public float blockExpandCentreMultiplier = 2f;

	// Token: 0x04003FF3 RID: 16371
	[Tooltip("?")]
	public float nonFavouredExpandMultiplier = 0.25f;

	// Token: 0x04003FF4 RID: 16372
	[Tooltip("Minimum size of a district")]
	public int districtSizeMin = 4;

	// Token: 0x04003FF5 RID: 16373
	[Tooltip("Maximum size of a district")]
	public int districtSizeMax = 8;

	// Token: 0x04003FF6 RID: 16374
	[Tooltip("Chances of a side alley being formed")]
	public float sideAlleyChance = 0.33f;

	// Token: 0x04003FF7 RID: 16375
	[Tooltip("Chances of a side alley being extended matching a previous side alley")]
	public float sideAlleyExtentionChance = 0.33f;

	// Token: 0x04003FF8 RID: 16376
	[Tooltip("Enable overhead street placement")]
	public bool overheadStreet = true;

	// Token: 0x04003FF9 RID: 16377
	[Header("Population")]
	[Tooltip("Amount to multiply the travel time distance by for calculating travel time guesses. X and Y planes only, IE Descrepencey between as the crow flies, and actual street route distance.")]
	public float travelTimeCrowFliesMultiplierEstimate = 1.66f;

	// Token: 0x04003FFA RID: 16378
	[Tooltip("Multiplier for travel time vs distance. Applied for guesses and calculated")]
	public float travelTimeMultiplier = 1.2f;

	// Token: 0x04003FFB RID: 16379
	[Tooltip("The city size (tile count) * by this will be created")]
	public float homelessMultiplier = 2f;

	// Token: 0x04003FFC RID: 16380
	[Header("Zoning")]
	public float residentialRatio = 0.5f;

	// Token: 0x04003FFD RID: 16381
	public float commercialRatio = 0.22f;

	// Token: 0x04003FFE RID: 16382
	public float industrialRatio = 0.19f;

	// Token: 0x04003FFF RID: 16383
	public float municipalRatio = 0.04f;

	// Token: 0x04004000 RID: 16384
	public float parksRatio = 0.05f;

	// Token: 0x04004001 RID: 16385
	[Header("Buildings")]
	[Tooltip("The address preset for lobbies/hallways")]
	public AddressPreset lobbyPreset;

	// Token: 0x04004002 RID: 16386
	[Tooltip("An internal address/unit of this many tiles is categorised as...")]
	public Vector2 smallUnitRange = new Vector2(0f, 4f);

	// Token: 0x04004003 RID: 16387
	[Tooltip("An internal address/unit of this many tiles is categorised as...")]
	public Vector2 mediumUnitRange = new Vector2(5f, 8f);

	// Token: 0x04004004 RID: 16388
	[Tooltip("An internal address/unit of this many tiles is categorised as...")]
	public Vector2 lageUnitRange = new Vector2(9f, 999f);

	// Token: 0x04004005 RID: 16389
	[Tooltip("The default design style")]
	[Space(5f)]
	public DesignStylePreset defaultStyle;

	// Token: 0x04004006 RID: 16390
	[Tooltip("Reference to default walls")]
	public DoorPairPreset defaultWalls;

	// Token: 0x04004007 RID: 16391
	public MaterialGroupPreset defaultFloorMaterialGroup;

	// Token: 0x04004008 RID: 16392
	public MaterialGroupPreset defaultCeilingMaterialGroup;

	// Token: 0x04004009 RID: 16393
	public MaterialGroupPreset defaultWallMaterialGroup;

	// Token: 0x0400400A RID: 16394
	[Space(5f)]
	[Tooltip("Setup for interior 'null space'")]
	public RoomConfiguration nullDefaultRoom;

	// Token: 0x0400400B RID: 16395
	[Tooltip("Setup for interior hallways")]
	public RoomConfiguration streetRoom;

	// Token: 0x0400400C RID: 16396
	public RoomConfiguration alleyRoom;

	// Token: 0x0400400D RID: 16397
	public RoomConfiguration backstreetRoom;

	// Token: 0x0400400E RID: 16398
	[Header("Layout Configs")]
	public LayoutConfiguration outsideLayoutConfig;

	// Token: 0x0400400F RID: 16399
	public LayoutConfiguration lobbyLayoutConfig;

	// Token: 0x04004010 RID: 16400
	[Tooltip("Street design styles")]
	public DesignStylePreset street;

	// Token: 0x04004011 RID: 16401
	[Space(5f)]
	public int lowestFloor = -2;

	// Token: 0x04004012 RID: 16402
	public float lowestFloorLightMultiplier = 0.5f;

	// Token: 0x04004013 RID: 16403
	public float lowestFloorIncreaseFlickerChance = 0.5f;

	// Token: 0x04004014 RID: 16404
	public float basementWaterLevel = -9.6f;

	// Token: 0x04004015 RID: 16405
	[Header("Interior fallback")]
	public DesignStylePreset fallbackStyle;

	// Token: 0x04004016 RID: 16406
	public ColourSchemePreset fallbackColourScheme;

	// Token: 0x04004017 RID: 16407
	public MaterialGroupPreset fallbackFloorMat;

	// Token: 0x04004018 RID: 16408
	public MaterialGroupPreset fallbackWallMat;

	// Token: 0x04004019 RID: 16409
	public MaterialGroupPreset fallbackCeilingMat;

	// Token: 0x0400401A RID: 16410
	[Header("Lighting")]
	[Tooltip("The directional light representing the sun")]
	public Light sunLight;

	// Token: 0x0400401B RID: 16411
	public Transform sunPosition;

	// Token: 0x0400401C RID: 16412
	public HDAdditionalLightData hdrpLightSunData;

	// Token: 0x0400401D RID: 16413
	public Light exteriorAmbientLight;

	// Token: 0x0400401E RID: 16414
	public HDAdditionalLightData exteriorAmbientHDRP;

	// Token: 0x0400401F RID: 16415
	public Light interiorAmbientLight;

	// Token: 0x04004020 RID: 16416
	public HDAdditionalLightData interiorAmbientHDRP;

	// Token: 0x04004021 RID: 16417
	[Header("Materials")]
	public Material seaMaterial;

	// Token: 0x04004022 RID: 16418
	public MeshRenderer seaRenderer;

	// Token: 0x04004023 RID: 16419
	public Material skylineMaterial;

	// Token: 0x04004024 RID: 16420
	public List<MeshRenderer> skylineRenderers = new List<MeshRenderer>();

	// Token: 0x04004025 RID: 16421
	public Material smokeMaterial;

	// Token: 0x04004026 RID: 16422
	[Space(7f)]
	public DesignStylePreset echelonDesignStyle;

	// Token: 0x04004027 RID: 16423
	public Color echelonWood;

	// Token: 0x04004028 RID: 16424
	public MaterialGroupPreset echelonFloorMaterial;

	// Token: 0x04004029 RID: 16425
	public MaterialGroupPreset.MaterialVariation echelonFloorVariation = new MaterialGroupPreset.MaterialVariation();

	// Token: 0x0400402A RID: 16426
	public MaterialGroupPreset echelonCeilingMaterial;

	// Token: 0x0400402B RID: 16427
	public MaterialGroupPreset.MaterialVariation echelonCeilingVariation = new MaterialGroupPreset.MaterialVariation();

	// Token: 0x0400402C RID: 16428
	public MaterialGroupPreset echelonDefaultWallMaterial;

	// Token: 0x0400402D RID: 16429
	public MaterialGroupPreset.MaterialVariation echelonWallVariation = new MaterialGroupPreset.MaterialVariation();

	// Token: 0x0400402E RID: 16430
	public ColourSchemePreset echelonColourScheme;

	// Token: 0x0400402F RID: 16431
	[Header("PP Profiles")]
	public List<CityControls.PPProfile> sceneProfileSetup = new List<CityControls.PPProfile>();

	// Token: 0x04004030 RID: 16432
	public CityControls.PPProfile captureSceneNormal;

	// Token: 0x04004031 RID: 16433
	public CityControls.PPProfile captureSceneCCTV;

	// Token: 0x04004032 RID: 16434
	[Header("Skybox")]
	public Transform ships1;

	// Token: 0x04004033 RID: 16435
	[Tooltip("Angle of North")]
	public float angleOfSun;

	// Token: 0x04004034 RID: 16436
	[Tooltip("Interior/Street Lights off")]
	public Vector2 lightsOff = new Vector2(8.5f, 9f);

	// Token: 0x04004035 RID: 16437
	[Tooltip("Interior/Street Lights on")]
	public Vector2 lightsOn = new Vector2(15f, 16f);

	// Token: 0x04004036 RID: 16438
	[Tooltip("Alley blocking wall preset")]
	[Header("Street Furniture")]
	public DoorPairPreset alleyBlockWallPreset;

	// Token: 0x04004037 RID: 16439
	[Header("Fog")]
	public FogPreset weatherSettings;

	// Token: 0x04004038 RID: 16440
	[Tooltip("How long it takes in gametime for the city to get wet on max rain (1)")]
	[Header("Weather")]
	public float timeForCityToGetWet = 0.1f;

	// Token: 0x04004039 RID: 16441
	[Tooltip("How long it takes in gametime for the city to get dry on min rain (0)")]
	public float timeForCityToGetDry = 0.7f;

	// Token: 0x0400403A RID: 16442
	[Tooltip("How long it takes in gametime for the city to get snowy on max rain (1)")]
	public float timeForCityToGetSnow = 0.1f;

	// Token: 0x0400403B RID: 16443
	[Tooltip("How long it takes in gametime for the city to get not snowy on min rain (0)")]
	public float timeForCityToGetNotSnow = 0.7f;

	// Token: 0x0400403C RID: 16444
	[Tooltip("The default neon material")]
	[Header("Signage")]
	public Material neonMaterial;

	// Token: 0x0400403D RID: 16445
	[Tooltip("Neon HDR intensity")]
	public float neonIntensity = 1.85f;

	// Token: 0x0400403E RID: 16446
	[Tooltip("Neon colours that can appear in signage, along with generated material references")]
	public List<CityControls.NeonMaterial> neonColours = new List<CityControls.NeonMaterial>();

	// Token: 0x0400403F RID: 16447
	[Header("Street Cables")]
	public List<CityControls.StreetCable> cables = new List<CityControls.StreetCable>();

	// Token: 0x04004040 RID: 16448
	public float maximumCableAngle = 0.25f;

	// Token: 0x04004041 RID: 16449
	[Header("Misc. References")]
	public LayoutConfiguration park;

	// Token: 0x04004042 RID: 16450
	[Header("Hotels")]
	[Tooltip("Upper and lower ends for hotel rooms in the city")]
	public int hotelCostLower = 100;

	// Token: 0x04004043 RID: 16451
	[Tooltip("Upper and lower ends for hotel rooms in the city")]
	public int hotelCostUpper = 200;

	// Token: 0x04004044 RID: 16452
	[Tooltip("Time until the player is kicked out of their hotel room for not paying")]
	public float kickoutTime = 8f;

	// Token: 0x04004045 RID: 16453
	[Header("Basement Water")]
	public Transform basementWaterTransform;

	// Token: 0x04004046 RID: 16454
	[Header("Lost & Found")]
	public InteractablePreset lostAndFoundNote;

	// Token: 0x04004047 RID: 16455
	[Tooltip("Items that can be lost and posted about")]
	public List<InteractablePreset> lostAndFoundItems = new List<InteractablePreset>();

	// Token: 0x04004048 RID: 16456
	private static CityControls _instance;

	// Token: 0x020007EE RID: 2030
	[Serializable]
	public struct WindowColour
	{
		// Token: 0x04004049 RID: 16457
		public Color colourOne;

		// Token: 0x0400404A RID: 16458
		public Color colourTwo;
	}

	// Token: 0x020007EF RID: 2031
	[Serializable]
	public class NeonMaterial
	{
		// Token: 0x0400404B RID: 16459
		public Color neonColour = Color.white;

		// Token: 0x0400404C RID: 16460
		public Color altColour2 = Color.white;

		// Token: 0x0400404D RID: 16461
		public Color altColour3 = Color.white;

		// Token: 0x0400404E RID: 16462
		public Material regularMat;

		// Token: 0x0400404F RID: 16463
		public Material flickingMat;

		// Token: 0x04004050 RID: 16464
		public AudioEvent flickerAudio;

		// Token: 0x04004051 RID: 16465
		[Tooltip("Does this light flicker?")]
		public bool flicker;

		// Token: 0x04004052 RID: 16466
		[Tooltip("When flickering, use this multiplier on the flicker colour to determin the actual colour (basically a darker version of flicker colour)")]
		public float flickerColourMultiplier;

		// Token: 0x04004053 RID: 16467
		public float pulseSpeed = 1f;

		// Token: 0x04004054 RID: 16468
		public float flickerState = 1f;

		// Token: 0x04004055 RID: 16469
		public bool flickerSwitch;

		// Token: 0x04004056 RID: 16470
		public bool flickerInterval;

		// Token: 0x04004057 RID: 16471
		public float interval;

		// Token: 0x04004058 RID: 16472
		public float intervalTime;

		// Token: 0x04004059 RID: 16473
		public float brightness;

		// Token: 0x0400405A RID: 16474
		[Space(5f)]
		public string colourTag;
	}

	// Token: 0x020007F0 RID: 2032
	[Serializable]
	public class CitySize
	{
		// Token: 0x0400405B RID: 16475
		public CityControls.Size size;

		// Token: 0x0400405C RID: 16476
		public Vector2 v2;
	}

	// Token: 0x020007F1 RID: 2033
	public enum Size
	{
		// Token: 0x0400405E RID: 16478
		small,
		// Token: 0x0400405F RID: 16479
		medium,
		// Token: 0x04004060 RID: 16480
		large,
		// Token: 0x04004061 RID: 16481
		veryLarge
	}

	// Token: 0x020007F2 RID: 2034
	[Serializable]
	public class PPProfile
	{
		// Token: 0x04004062 RID: 16482
		public SessionData.SceneProfile profile;

		// Token: 0x04004063 RID: 16483
		public Volume volume;

		// Token: 0x04004064 RID: 16484
		public GameObject objectRef;
	}

	// Token: 0x020007F3 RID: 2035
	[Serializable]
	public class StreetCable
	{
		// Token: 0x04004065 RID: 16485
		public GameObject prefab;

		// Token: 0x04004066 RID: 16486
		public float maximumWidth = 15f;

		// Token: 0x04004067 RID: 16487
		public int frequency = 1;

		// Token: 0x04004068 RID: 16488
		[Tooltip("The maximum angle deviation for cables. 0 is only straight.")]
		public float maximumCableAngle = 0.25f;

		// Token: 0x04004069 RID: 16489
		public float minimumHeight;

		// Token: 0x0400406A RID: 16490
		public float maximumHeight = 999f;

		// Token: 0x0400406B RID: 16491
		[Space(7f)]
		public bool onlyFromZoneType;

		// Token: 0x0400406C RID: 16492
		public BuildingPreset.ZoneType zone;

		// Token: 0x0400406D RID: 16493
		[Space(7f)]
		public bool disitrctFrequencyModifier;

		// Token: 0x0400406E RID: 16494
		public List<DistrictPreset> districts = new List<DistrictPreset>();

		// Token: 0x0400406F RID: 16495
		public int frequencyModifier;

		// Token: 0x04004070 RID: 16496
		[Space(7f)]
		[Tooltip("Change area colours")]
		public bool alterAreaLighting;

		// Token: 0x04004071 RID: 16497
		[EnableIf("alterAreaLighting")]
		public List<Color> possibleColours = new List<Color>();

		// Token: 0x04004072 RID: 16498
		[Tooltip("This is used in combination with the following to adjust street area lighting")]
		[EnableIf("alterAreaLighting")]
		public DistrictPreset.AffectStreetAreaLights lightOperation;

		// Token: 0x04004073 RID: 16499
		[EnableIf("alterAreaLighting")]
		public float lightAmount = 0.12f;

		// Token: 0x04004074 RID: 16500
		[Tooltip("This is added to brightness")]
		[EnableIf("alterAreaLighting")]
		public float brightnessModifier = 10f;
	}
}
