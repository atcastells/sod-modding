using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000452 RID: 1106
[Serializable]
public class CitySaveData
{
	// Token: 0x04001EC0 RID: 7872
	public string build;

	// Token: 0x04001EC1 RID: 7873
	public string cityName = "New City";

	// Token: 0x04001EC2 RID: 7874
	public string seed = "Abaos93rmnisf932";

	// Token: 0x04001EC3 RID: 7875
	public Vector2 citySize = new Vector2(5f, 5f);

	// Token: 0x04001EC4 RID: 7876
	public int population;

	// Token: 0x04001EC5 RID: 7877
	public int playersApartment = -1;

	// Token: 0x04001EC6 RID: 7878
	public List<CitySaveData.DistrictCitySave> districts = new List<CitySaveData.DistrictCitySave>();

	// Token: 0x04001EC7 RID: 7879
	public List<CitySaveData.StreetCitySave> streets = new List<CitySaveData.StreetCitySave>();

	// Token: 0x04001EC8 RID: 7880
	public List<CitySaveData.CityTileCitySave> cityTiles = new List<CitySaveData.CityTileCitySave>();

	// Token: 0x04001EC9 RID: 7881
	public List<CitySaveData.HumanCitySave> citizens = new List<CitySaveData.HumanCitySave>();

	// Token: 0x04001ECA RID: 7882
	public List<Interactable> interactables = new List<Interactable>();

	// Token: 0x04001ECB RID: 7883
	public List<GroupsController.SocialGroup> groups = new List<GroupsController.SocialGroup>();

	// Token: 0x04001ECC RID: 7884
	public List<PipeConstructor.PipeGroup> pipes = new List<PipeConstructor.PipeGroup>();

	// Token: 0x04001ECD RID: 7885
	public List<CitySaveData.OccupationCitySave> criminals = new List<CitySaveData.OccupationCitySave>();

	// Token: 0x04001ECE RID: 7886
	public List<CitySaveData.EvidenceStateSave> multiPage = new List<CitySaveData.EvidenceStateSave>();

	// Token: 0x04001ECF RID: 7887
	public List<MetaObject> metas = new List<MetaObject>();

	// Token: 0x02000453 RID: 1107
	[Serializable]
	public class DistrictCitySave
	{
		// Token: 0x04001ED0 RID: 7888
		public string name;

		// Token: 0x04001ED1 RID: 7889
		public string preset;

		// Token: 0x04001ED2 RID: 7890
		public int districtID;

		// Token: 0x04001ED3 RID: 7891
		public List<CitySaveData.BlockCitySave> blocks = new List<CitySaveData.BlockCitySave>();

		// Token: 0x04001ED4 RID: 7892
		public float averageLandValue;

		// Token: 0x04001ED5 RID: 7893
		public List<SocialStatistics.EthnicityFrequency> dominantEthnicities = new List<SocialStatistics.EthnicityFrequency>();
	}

	// Token: 0x02000454 RID: 1108
	[Serializable]
	public class BlockCitySave
	{
		// Token: 0x04001ED6 RID: 7894
		public string name;

		// Token: 0x04001ED7 RID: 7895
		public int blockID;

		// Token: 0x04001ED8 RID: 7896
		public float averageDensity;

		// Token: 0x04001ED9 RID: 7897
		public float averageLandValue;
	}

	// Token: 0x02000455 RID: 1109
	[Serializable]
	public class CityTileCitySave
	{
		// Token: 0x04001EDA RID: 7898
		public string name;

		// Token: 0x04001EDB RID: 7899
		public int blockID;

		// Token: 0x04001EDC RID: 7900
		public int districtID;

		// Token: 0x04001EDD RID: 7901
		public Vector2Int cityCoord;

		// Token: 0x04001EDE RID: 7902
		public CitySaveData.BuildingCitySave building;

		// Token: 0x04001EDF RID: 7903
		public List<CitySaveData.TileCitySave> outsideTiles = new List<CitySaveData.TileCitySave>();

		// Token: 0x04001EE0 RID: 7904
		public BuildingPreset.Density density = BuildingPreset.Density.medium;

		// Token: 0x04001EE1 RID: 7905
		public BuildingPreset.LandValue landValue = BuildingPreset.LandValue.medium;
	}

	// Token: 0x02000456 RID: 1110
	[Serializable]
	public class BuildingCitySave
	{
		// Token: 0x04001EE2 RID: 7906
		public int buildingID;

		// Token: 0x04001EE3 RID: 7907
		public string name;

		// Token: 0x04001EE4 RID: 7908
		public List<CitySaveData.FloorCitySave> floors = new List<CitySaveData.FloorCitySave>();

		// Token: 0x04001EE5 RID: 7909
		public string preset;

		// Token: 0x04001EE6 RID: 7910
		public NewBuilding.Direction facing;

		// Token: 0x04001EE7 RID: 7911
		public bool isInaccessible;

		// Token: 0x04001EE8 RID: 7912
		public List<NewBuilding.SideSign> sideSigns;

		// Token: 0x04001EE9 RID: 7913
		public List<CitySaveData.AirDuctGroupCitySave> airDucts = new List<CitySaveData.AirDuctGroupCitySave>();

		// Token: 0x04001EEA RID: 7914
		public string designStyle;

		// Token: 0x04001EEB RID: 7915
		public Color wood;

		// Token: 0x04001EEC RID: 7916
		public string floorMaterial;

		// Token: 0x04001EED RID: 7917
		public Toolbox.MaterialKey floorMatKey;

		// Token: 0x04001EEE RID: 7918
		public string ceilingMaterial;

		// Token: 0x04001EEF RID: 7919
		public Toolbox.MaterialKey ceilingMatKey;

		// Token: 0x04001EF0 RID: 7920
		public string defaultWallMaterial;

		// Token: 0x04001EF1 RID: 7921
		public Toolbox.MaterialKey defaultWallKey;

		// Token: 0x04001EF2 RID: 7922
		public string extWallMaterial;

		// Token: 0x04001EF3 RID: 7923
		public Toolbox.MaterialKey extWallKey;

		// Token: 0x04001EF4 RID: 7924
		public string colourScheme;

		// Token: 0x04001EF5 RID: 7925
		public string floorMatOverride;

		// Token: 0x04001EF6 RID: 7926
		public string ceilingMatOverride;

		// Token: 0x04001EF7 RID: 7927
		public string wallMatOverride;

		// Token: 0x04001EF8 RID: 7928
		public string floorMatOverrideB;

		// Token: 0x04001EF9 RID: 7929
		public string ceilingMatOverrideB;

		// Token: 0x04001EFA RID: 7930
		public string wallMatOverrideB;
	}

	// Token: 0x02000457 RID: 1111
	[Serializable]
	public class AirDuctGroupCitySave
	{
		// Token: 0x04001EFB RID: 7931
		public int id;

		// Token: 0x04001EFC RID: 7932
		public bool ext;

		// Token: 0x04001EFD RID: 7933
		public List<int> airVents = new List<int>();

		// Token: 0x04001EFE RID: 7934
		public List<CitySaveData.AirDuctSegmentCitySave> airDucts = new List<CitySaveData.AirDuctSegmentCitySave>();

		// Token: 0x04001EFF RID: 7935
		public List<int> ventRooms = new List<int>();

		// Token: 0x04001F00 RID: 7936
		public List<int> adjoining = new List<int>();
	}

	// Token: 0x02000458 RID: 1112
	[Serializable]
	public class AirDuctSegmentCitySave
	{
		// Token: 0x04001F01 RID: 7937
		public int level;

		// Token: 0x04001F02 RID: 7938
		public int index;

		// Token: 0x04001F03 RID: 7939
		public Vector3Int duct;

		// Token: 0x04001F04 RID: 7940
		public Vector3Int previous;

		// Token: 0x04001F05 RID: 7941
		public Vector3Int next;

		// Token: 0x04001F06 RID: 7942
		public Vector3Int node;

		// Token: 0x04001F07 RID: 7943
		public bool peek;

		// Token: 0x04001F08 RID: 7944
		public Vector3Int addRot;
	}

	// Token: 0x02000459 RID: 1113
	[Serializable]
	public class AirVentSave
	{
		// Token: 0x04001F09 RID: 7945
		public int id;

		// Token: 0x04001F0A RID: 7946
		public NewAddress.AirVent ventType;

		// Token: 0x04001F0B RID: 7947
		public int wall = -1;

		// Token: 0x04001F0C RID: 7948
		public Vector3Int node;

		// Token: 0x04001F0D RID: 7949
		public Vector3Int rNode;
	}

	// Token: 0x0200045A RID: 1114
	[Serializable]
	public class FloorCitySave
	{
		// Token: 0x04001F0E RID: 7950
		public string name;

		// Token: 0x04001F0F RID: 7951
		public int floorID;

		// Token: 0x04001F10 RID: 7952
		public int floor;

		// Token: 0x04001F11 RID: 7953
		public List<CitySaveData.AddressCitySave> addresses = new List<CitySaveData.AddressCitySave>();

		// Token: 0x04001F12 RID: 7954
		public List<CitySaveData.TileCitySave> tiles = new List<CitySaveData.TileCitySave>();

		// Token: 0x04001F13 RID: 7955
		public Vector2 size = new Vector2(1f, 1f);

		// Token: 0x04001F14 RID: 7956
		public int defaultFloorHeight;

		// Token: 0x04001F15 RID: 7957
		public int defaultCeilingHeight = 42;

		// Token: 0x04001F16 RID: 7958
		public int layoutIndex;

		// Token: 0x04001F17 RID: 7959
		public bool echelons;

		// Token: 0x04001F18 RID: 7960
		public int breakerSec = -1;

		// Token: 0x04001F19 RID: 7961
		public int breakerLights = -1;

		// Token: 0x04001F1A RID: 7962
		public int breakerDoors = -1;
	}

	// Token: 0x0200045B RID: 1115
	[Serializable]
	public class TileCitySave
	{
		// Token: 0x04001F1B RID: 7963
		public int tileID;

		// Token: 0x04001F1C RID: 7964
		public Vector2Int floorCoord;

		// Token: 0x04001F1D RID: 7965
		public Vector3Int globalTileCoord;

		// Token: 0x04001F1E RID: 7966
		public bool isOutside;

		// Token: 0x04001F1F RID: 7967
		public bool isObstacle;

		// Token: 0x04001F20 RID: 7968
		public bool isEdge;

		// Token: 0x04001F21 RID: 7969
		public int rotation;

		// Token: 0x04001F22 RID: 7970
		public bool isEntrance;

		// Token: 0x04001F23 RID: 7971
		public bool isMainEntrance;

		// Token: 0x04001F24 RID: 7972
		public bool isStairwell;

		// Token: 0x04001F25 RID: 7973
		public int stairwellRotation;

		// Token: 0x04001F26 RID: 7974
		public bool isElevator;

		// Token: 0x04001F27 RID: 7975
		public int elevatorRotation;

		// Token: 0x04001F28 RID: 7976
		public bool isTop;

		// Token: 0x04001F29 RID: 7977
		public bool isBottom;
	}

	// Token: 0x0200045C RID: 1116
	[Serializable]
	public class StreetCitySave
	{
		// Token: 0x04001F2A RID: 7978
		public string name;

		// Token: 0x04001F2B RID: 7979
		public int residenceNumber;

		// Token: 0x04001F2C RID: 7980
		public bool isLobby;

		// Token: 0x04001F2D RID: 7981
		public bool isMainLobby;

		// Token: 0x04001F2E RID: 7982
		public bool isOutside;

		// Token: 0x04001F2F RID: 7983
		public AddressPreset.AccessType access;

		// Token: 0x04001F30 RID: 7984
		public List<CitySaveData.RoomCitySave> rooms = new List<CitySaveData.RoomCitySave>();

		// Token: 0x04001F31 RID: 7985
		public string designStyle;

		// Token: 0x04001F32 RID: 7986
		public int streetID;

		// Token: 0x04001F33 RID: 7987
		public int district;

		// Token: 0x04001F34 RID: 7988
		public List<Vector3Int> tiles = new List<Vector3Int>();

		// Token: 0x04001F35 RID: 7989
		public string streetSuffix = string.Empty;

		// Token: 0x04001F36 RID: 7990
		public bool isAlley;

		// Token: 0x04001F37 RID: 7991
		public bool isBackstreet;

		// Token: 0x04001F38 RID: 7992
		public List<int> sharedGround;

		// Token: 0x04001F39 RID: 7993
		public List<StreetController.StreetTile> streetTiles;
	}

	// Token: 0x0200045D RID: 1117
	[Serializable]
	public class AddressCitySave
	{
		// Token: 0x04001F3A RID: 7994
		public string name;

		// Token: 0x04001F3B RID: 7995
		public int residenceNumber;

		// Token: 0x04001F3C RID: 7996
		public bool isLobby;

		// Token: 0x04001F3D RID: 7997
		public bool isOutside;

		// Token: 0x04001F3E RID: 7998
		public AddressPreset.AccessType access;

		// Token: 0x04001F3F RID: 7999
		public List<CitySaveData.RoomCitySave> rooms = new List<CitySaveData.RoomCitySave>();

		// Token: 0x04001F40 RID: 8000
		public string designStyle;

		// Token: 0x04001F41 RID: 8001
		public bool neonHor;

		// Token: 0x04001F42 RID: 8002
		public bool neonVer;

		// Token: 0x04001F43 RID: 8003
		public int neonVerticalIndex = -1;

		// Token: 0x04001F44 RID: 8004
		public int neonColour;

		// Token: 0x04001F45 RID: 8005
		public string neonFont;

		// Token: 0x04001F46 RID: 8006
		public float landValue;

		// Token: 0x04001F47 RID: 8007
		public GameplayController.Passcode passcode;

		// Token: 0x04001F48 RID: 8008
		public List<Vector3> protectedNodes = new List<Vector3>();

		// Token: 0x04001F49 RID: 8009
		public int id;

		// Token: 0x04001F4A RID: 8010
		public string address;

		// Token: 0x04001F4B RID: 8011
		public string preset;

		// Token: 0x04001F4C RID: 8012
		public Color wood;

		// Token: 0x04001F4D RID: 8013
		public CitySaveData.ResidenceCitySave residence;

		// Token: 0x04001F4E RID: 8014
		public CitySaveData.CompanyCitySave company;

		// Token: 0x04001F4F RID: 8015
		public bool isOutsideAddress;

		// Token: 0x04001F50 RID: 8016
		public bool isLobbyAddress;

		// Token: 0x04001F51 RID: 8017
		public bool hkey;

		// Token: 0x04001F52 RID: 8018
		public int breakerSec = -1;

		// Token: 0x04001F53 RID: 8019
		public int breakerLights = -1;

		// Token: 0x04001F54 RID: 8020
		public int breakerDoors = -1;
	}

	// Token: 0x0200045E RID: 1118
	[Serializable]
	public class ResidenceCitySave
	{
		// Token: 0x04001F55 RID: 8021
		public string preset;

		// Token: 0x04001F56 RID: 8022
		public int mail;
	}

	// Token: 0x0200045F RID: 1119
	[Serializable]
	public class CompanyCitySave
	{
		// Token: 0x04001F57 RID: 8023
		public string preset;

		// Token: 0x04001F58 RID: 8024
		public int id;

		// Token: 0x04001F59 RID: 8025
		public List<CitySaveData.OccupationCitySave> companyRoster = new List<CitySaveData.OccupationCitySave>();

		// Token: 0x04001F5A RID: 8026
		public float topSalary;

		// Token: 0x04001F5B RID: 8027
		public float minimumSalary;

		// Token: 0x04001F5C RID: 8028
		public bool publicFacing = true;

		// Token: 0x04001F5D RID: 8029
		public string shortName;

		// Token: 0x04001F5E RID: 8030
		public List<string> nameAltTags;

		// Token: 0x04001F5F RID: 8031
		public bool monday = true;

		// Token: 0x04001F60 RID: 8032
		public bool tuesday = true;

		// Token: 0x04001F61 RID: 8033
		public bool wednesday = true;

		// Token: 0x04001F62 RID: 8034
		public bool thursday = true;

		// Token: 0x04001F63 RID: 8035
		public bool friday = true;

		// Token: 0x04001F64 RID: 8036
		public bool saturday = true;

		// Token: 0x04001F65 RID: 8037
		public bool sunday;

		// Token: 0x04001F66 RID: 8038
		public Vector2 retailOpenHours = new Vector2(8f, 17f);

		// Token: 0x04001F67 RID: 8039
		public int passedWorkLocation = -1;

		// Token: 0x04001F68 RID: 8040
		public List<string> menuItems = new List<string>();

		// Token: 0x04001F69 RID: 8041
		public List<int> itemCosts = new List<int>();
	}

	// Token: 0x02000460 RID: 1120
	[Serializable]
	public class OccupationCitySave
	{
		// Token: 0x04001F6A RID: 8042
		public int id;

		// Token: 0x04001F6B RID: 8043
		public string preset;

		// Token: 0x04001F6C RID: 8044
		public string name = "worker";

		// Token: 0x04001F6D RID: 8045
		public bool teamLeader;

		// Token: 0x04001F6E RID: 8046
		public int boss;

		// Token: 0x04001F6F RID: 8047
		public float paygrade;

		// Token: 0x04001F70 RID: 8048
		public int teamID;

		// Token: 0x04001F71 RID: 8049
		public bool isOwner;

		// Token: 0x04001F72 RID: 8050
		public OccupationPreset.workCollar collar;

		// Token: 0x04001F73 RID: 8051
		public OccupationPreset.workType work;

		// Token: 0x04001F74 RID: 8052
		public List<OccupationPreset.workTags> tags = new List<OccupationPreset.workTags>();

		// Token: 0x04001F75 RID: 8053
		public int shift;

		// Token: 0x04001F76 RID: 8054
		public float startTime = 9f;

		// Token: 0x04001F77 RID: 8055
		public float endTime = 17f;

		// Token: 0x04001F78 RID: 8056
		public List<SessionData.WeekDay> workDaysList = new List<SessionData.WeekDay>();

		// Token: 0x04001F79 RID: 8057
		public float salary;

		// Token: 0x04001F7A RID: 8058
		public string salaryString = string.Empty;
	}

	// Token: 0x02000461 RID: 1121
	[Serializable]
	public class RoomCitySave
	{
		// Token: 0x04001F7B RID: 8059
		public string name;

		// Token: 0x04001F7C RID: 8060
		public List<CitySaveData.NodeCitySave> nodes = new List<CitySaveData.NodeCitySave>();

		// Token: 0x04001F7D RID: 8061
		public List<string> openPlanElements = new List<string>();

		// Token: 0x04001F7E RID: 8062
		public List<CitySaveData.LightZoneSave> lightZones = new List<CitySaveData.LightZoneSave>();

		// Token: 0x04001F7F RID: 8063
		public List<int> commonRooms = new List<int>();

		// Token: 0x04001F80 RID: 8064
		public int floorID = -1;

		// Token: 0x04001F81 RID: 8065
		public int id = -1;

		// Token: 0x04001F82 RID: 8066
		public int fID = 1;

		// Token: 0x04001F83 RID: 8067
		public int iID = 1;

		// Token: 0x04001F84 RID: 8068
		public string preset;

		// Token: 0x04001F85 RID: 8069
		public bool reachableFromEntrance;

		// Token: 0x04001F86 RID: 8070
		public bool isOutsideWindow;

		// Token: 0x04001F87 RID: 8071
		public bool allowCoving;

		// Token: 0x04001F88 RID: 8072
		public string floorMaterial;

		// Token: 0x04001F89 RID: 8073
		public Toolbox.MaterialKey floorMatKey;

		// Token: 0x04001F8A RID: 8074
		public string ceilingMaterial;

		// Token: 0x04001F8B RID: 8075
		public Toolbox.MaterialKey ceilingMatKey;

		// Token: 0x04001F8C RID: 8076
		public string defaultWallMaterial;

		// Token: 0x04001F8D RID: 8077
		public Toolbox.MaterialKey defaultWallKey;

		// Token: 0x04001F8E RID: 8078
		public Toolbox.MaterialKey miscKey;

		// Token: 0x04001F8F RID: 8079
		public string colourScheme;

		// Token: 0x04001F90 RID: 8080
		public string mainLightPreset;

		// Token: 0x04001F91 RID: 8081
		public bool isBaseNullRoom;

		// Token: 0x04001F92 RID: 8082
		public Vector3 middle;

		// Token: 0x04001F93 RID: 8083
		public List<CitySaveData.FurnitureClusterCitySave> f = new List<CitySaveData.FurnitureClusterCitySave>();

		// Token: 0x04001F94 RID: 8084
		public List<int> owners = new List<int>();

		// Token: 0x04001F95 RID: 8085
		public List<CitySaveData.AirVentSave> airVents = new List<CitySaveData.AirVentSave>();

		// Token: 0x04001F96 RID: 8086
		public GameplayController.Passcode password;

		// Token: 0x04001F97 RID: 8087
		public int cf = -1;

		// Token: 0x04001F98 RID: 8088
		public List<CitySaveData.CullTreeSave> cullTree = new List<CitySaveData.CullTreeSave>();

		// Token: 0x04001F99 RID: 8089
		public List<int> above = new List<int>();

		// Token: 0x04001F9A RID: 8090
		public List<int> below = new List<int>();

		// Token: 0x04001F9B RID: 8091
		public List<int> adj = new List<int>();

		// Token: 0x04001F9C RID: 8092
		public List<int> occ = new List<int>();
	}

	// Token: 0x02000462 RID: 1122
	[Serializable]
	public class CullTreeSave
	{
		// Token: 0x04001F9D RID: 8093
		public int r;

		// Token: 0x04001F9E RID: 8094
		public List<int> d;
	}

	// Token: 0x02000463 RID: 1123
	[Serializable]
	public class LightZoneSave
	{
		// Token: 0x04001F9F RID: 8095
		public List<Vector3Int> n = new List<Vector3Int>();

		// Token: 0x04001FA0 RID: 8096
		public Color areaLightColour;

		// Token: 0x04001FA1 RID: 8097
		public float areaLightBright;
	}

	// Token: 0x02000464 RID: 1124
	[Serializable]
	public class NodeCitySave
	{
		// Token: 0x04001FA2 RID: 8098
		public Vector2Int fc;

		// Token: 0x04001FA3 RID: 8099
		public Vector2Int ltc;

		// Token: 0x04001FA4 RID: 8100
		public Vector3Int nc;

		// Token: 0x04001FA5 RID: 8101
		public List<CitySaveData.WallCitySave> w = new List<CitySaveData.WallCitySave>();

		// Token: 0x04001FA6 RID: 8102
		public int fh;

		// Token: 0x04001FA7 RID: 8103
		public NewNode.FloorTileType ft = NewNode.FloorTileType.floorAndCeiling;

		// Token: 0x04001FA8 RID: 8104
		public bool io;

		// Token: 0x04001FA9 RID: 8105
		public bool ios;

		// Token: 0x04001FAA RID: 8106
		public bool sll;

		// Token: 0x04001FAB RID: 8107
		public bool sul;

		// Token: 0x04001FAC RID: 8108
		public string fr;

		// Token: 0x04001FAD RID: 8109
		public string frr;

		// Token: 0x04001FAE RID: 8110
		public bool anf = true;

		// Token: 0x04001FAF RID: 8111
		public bool cav;

		// Token: 0x04001FB0 RID: 8112
		public bool fav;
	}

	// Token: 0x02000465 RID: 1125
	[Serializable]
	public class WallCitySave
	{
		// Token: 0x04001FB1 RID: 8113
		public Vector2 wo;

		// Token: 0x04001FB2 RID: 8114
		public int id = -1;

		// Token: 0x04001FB3 RID: 8115
		public string p;

		// Token: 0x04001FB4 RID: 8116
		public int ow;

		// Token: 0x04001FB5 RID: 8117
		public int pw;

		// Token: 0x04001FB6 RID: 8118
		public int cw;

		// Token: 0x04001FB7 RID: 8119
		public bool oo;

		// Token: 0x04001FB8 RID: 8120
		public bool oa;

		// Token: 0x04001FB9 RID: 8121
		public int nos;

		// Token: 0x04001FBA RID: 8122
		public bool isw;

		// Token: 0x04001FBB RID: 8123
		public int cl = -1;

		// Token: 0x04001FBC RID: 8124
		public bool sw;

		// Token: 0x04001FBD RID: 8125
		public List<CitySaveData.WallFrontageSave> fr = new List<CitySaveData.WallFrontageSave>();

		// Token: 0x04001FBE RID: 8126
		public bool dm;

		// Token: 0x04001FBF RID: 8127
		public Toolbox.MaterialKey dmk;

		// Token: 0x04001FC0 RID: 8128
		public float ds;

		// Token: 0x04001FC1 RID: 8129
		public float ls;
	}

	// Token: 0x02000466 RID: 1126
	[Serializable]
	public class WallFrontageSave
	{
		// Token: 0x04001FC2 RID: 8130
		public string str;

		// Token: 0x04001FC3 RID: 8131
		public Toolbox.MaterialKey matKey;

		// Token: 0x04001FC4 RID: 8132
		public Vector3 o;
	}

	// Token: 0x02000467 RID: 1127
	[Serializable]
	public class FurnitureClusterCitySave
	{
		// Token: 0x04001FC5 RID: 8133
		public string cluster;

		// Token: 0x04001FC6 RID: 8134
		public Vector3Int anchorNode;

		// Token: 0x04001FC7 RID: 8135
		public int angle;

		// Token: 0x04001FC8 RID: 8136
		public float ranking;

		// Token: 0x04001FC9 RID: 8137
		public List<CitySaveData.FurnitureClusterObjectCitySave> objs = new List<CitySaveData.FurnitureClusterObjectCitySave>();
	}

	// Token: 0x02000468 RID: 1128
	[Serializable]
	public class FurnitureClusterObjectCitySave
	{
		// Token: 0x04001FCA RID: 8138
		public int id;

		// Token: 0x04001FCB RID: 8139
		public List<string> furnitureClasses;

		// Token: 0x04001FCC RID: 8140
		public int angle;

		// Token: 0x04001FCD RID: 8141
		public Vector3Int anchorNode;

		// Token: 0x04001FCE RID: 8142
		public List<Vector3Int> coversNodes;

		// Token: 0x04001FCF RID: 8143
		public Vector3 offset;

		// Token: 0x04001FD0 RID: 8144
		public string furniture;

		// Token: 0x04001FD1 RID: 8145
		public string art;

		// Token: 0x04001FD2 RID: 8146
		public bool useFOVBLock;

		// Token: 0x04001FD3 RID: 8147
		public Vector2 fovDirection;

		// Token: 0x04001FD4 RID: 8148
		public int fovMaxDistance = 5;

		// Token: 0x04001FD5 RID: 8149
		public bool up;

		// Token: 0x04001FD6 RID: 8150
		public Vector3 scale;

		// Token: 0x04001FD7 RID: 8151
		public Toolbox.MaterialKey matKey;

		// Token: 0x04001FD8 RID: 8152
		public Toolbox.MaterialKey artMatKet;

		// Token: 0x04001FD9 RID: 8153
		public List<int> owners = new List<int>();
	}

	// Token: 0x02000469 RID: 1129
	[Serializable]
	public class HumanCitySave
	{
		// Token: 0x04001FDA RID: 8154
		public int humanID = -1;

		// Token: 0x04001FDB RID: 8155
		public int home;

		// Token: 0x04001FDC RID: 8156
		public string debugHome;

		// Token: 0x04001FDD RID: 8157
		public float speedModifier = 0.12f;

		// Token: 0x04001FDE RID: 8158
		public int job;

		// Token: 0x04001FDF RID: 8159
		public string birthday;

		// Token: 0x04001FE0 RID: 8160
		public float societalClass;

		// Token: 0x04001FE1 RID: 8161
		public Descriptors descriptors;

		// Token: 0x04001FE2 RID: 8162
		public Human.BloodType blood;

		// Token: 0x04001FE3 RID: 8163
		public string citizenName = string.Empty;

		// Token: 0x04001FE4 RID: 8164
		public string firstName = string.Empty;

		// Token: 0x04001FE5 RID: 8165
		public string casualName = string.Empty;

		// Token: 0x04001FE6 RID: 8166
		public string surName = string.Empty;

		// Token: 0x04001FE7 RID: 8167
		public bool homeless;

		// Token: 0x04001FE8 RID: 8168
		public float slangUsage = 0.5f;

		// Token: 0x04001FE9 RID: 8169
		public float genderScale = 0.5f;

		// Token: 0x04001FEA RID: 8170
		public Human.Gender gender;

		// Token: 0x04001FEB RID: 8171
		public Human.Gender bGender;

		// Token: 0x04001FEC RID: 8172
		public float sexuality = 0.5f;

		// Token: 0x04001FED RID: 8173
		public float homosexuality = 0.5f;

		// Token: 0x04001FEE RID: 8174
		public List<Human.Gender> attractedTo = new List<Human.Gender>();

		// Token: 0x04001FEF RID: 8175
		public int partner = -1;

		// Token: 0x04001FF0 RID: 8176
		public int paramour = -1;

		// Token: 0x04001FF1 RID: 8177
		public string anniversary;

		// Token: 0x04001FF2 RID: 8178
		public float sleepNeedMultiplier = 1f;

		// Token: 0x04001FF3 RID: 8179
		public float snoring = 1f;

		// Token: 0x04001FF4 RID: 8180
		public float snoreDelay = 2f;

		// Token: 0x04001FF5 RID: 8181
		public float humility;

		// Token: 0x04001FF6 RID: 8182
		public float emotionality;

		// Token: 0x04001FF7 RID: 8183
		public float extraversion;

		// Token: 0x04001FF8 RID: 8184
		public float agreeableness;

		// Token: 0x04001FF9 RID: 8185
		public float conscientiousness;

		// Token: 0x04001FFA RID: 8186
		public float creativity;

		// Token: 0x04001FFB RID: 8187
		public List<CitySaveData.AcquaintanceCitySave> acquaintances = new List<CitySaveData.AcquaintanceCitySave>();

		// Token: 0x04001FFC RID: 8188
		public List<CitySaveData.CharTraitSave> traits = new List<CitySaveData.CharTraitSave>();

		// Token: 0x04001FFD RID: 8189
		public GameplayController.Passcode password;

		// Token: 0x04001FFE RID: 8190
		public float maxHealth = 1f;

		// Token: 0x04001FFF RID: 8191
		public float recoveryRate = 0.1f;

		// Token: 0x04002000 RID: 8192
		public float combatSkill = 1f;

		// Token: 0x04002001 RID: 8193
		public float combatHeft = 0.25f;

		// Token: 0x04002002 RID: 8194
		public float maxNerve = 1f;

		// Token: 0x04002003 RID: 8195
		public float breathRecovery = 1f;

		// Token: 0x04002004 RID: 8196
		public string handwriting;

		// Token: 0x04002005 RID: 8197
		public int sightingMemory = 100;

		// Token: 0x04002006 RID: 8198
		public List<string> favItems = new List<string>();

		// Token: 0x04002007 RID: 8199
		public List<int> favItemRanks = new List<int>();

		// Token: 0x04002008 RID: 8200
		public List<CompanyPreset.CompanyCategory> favCat = new List<CompanyPreset.CompanyCategory>();

		// Token: 0x04002009 RID: 8201
		public List<int> favAddresses = new List<int>();

		// Token: 0x0400200A RID: 8202
		public List<CitizenOutfitController.Outfit> outfits = new List<CitizenOutfitController.Outfit>();

		// Token: 0x0400200B RID: 8203
		public int favCol;
	}

	// Token: 0x0200046A RID: 1130
	[Serializable]
	public class CharTraitSave
	{
		// Token: 0x0400200C RID: 8204
		public int traitID;

		// Token: 0x0400200D RID: 8205
		public string trait;

		// Token: 0x0400200E RID: 8206
		public int reason = -1;

		// Token: 0x0400200F RID: 8207
		public string date;
	}

	// Token: 0x0200046B RID: 1131
	[Serializable]
	public class AcquaintanceCitySave
	{
		// Token: 0x04002010 RID: 8208
		public int from;

		// Token: 0x04002011 RID: 8209
		public int with;

		// Token: 0x04002012 RID: 8210
		public List<Acquaintance.ConnectionType> connections = new List<Acquaintance.ConnectionType>();

		// Token: 0x04002013 RID: 8211
		public Acquaintance.ConnectionType secret;

		// Token: 0x04002014 RID: 8212
		public float compatible;

		// Token: 0x04002015 RID: 8213
		public float known = 0.1f;

		// Token: 0x04002016 RID: 8214
		public float like;

		// Token: 0x04002017 RID: 8215
		public List<Evidence.DataKey> dataKeys = new List<Evidence.DataKey>();
	}

	// Token: 0x0200046C RID: 1132
	[Serializable]
	public class EvidenceStateSave
	{
		// Token: 0x04002018 RID: 8216
		public string id;

		// Token: 0x04002019 RID: 8217
		public int page;

		// Token: 0x0400201A RID: 8218
		public List<EvidenceMultiPage.MultiPageContent> mpContent = new List<EvidenceMultiPage.MultiPageContent>();
	}
}
