using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class CityData : MonoBehaviour
{
	// Token: 0x17000027 RID: 39
	// (get) Token: 0x060005D3 RID: 1491 RVA: 0x0005D4D6 File Offset: 0x0005B6D6
	public static CityData Instance
	{
		get
		{
			return CityData._instance;
		}
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x0005D4DD File Offset: 0x0005B6DD
	private void Awake()
	{
		if (CityData._instance != null && CityData._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CityData._instance = this;
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x0005D50B File Offset: 0x0005B70B
	private void Start()
	{
		this.maxCoord = new Vector2(this.CityTileToRealpos(CityData.Instance.citySize).x, this.CityTileToRealpos(CityData.Instance.citySize).z);
		this.ParseFloorData();
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x0005D548 File Offset: 0x0005B748
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
		CityData._instance = null;
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x0005D55C File Offset: 0x0005B75C
	public void ParseFloorData()
	{
		this.floorData.Clear();
		List<TextAsset> allFloorData = AssetLoader.Instance.GetAllFloorData();
		for (int i = 0; i < allFloorData.Count; i++)
		{
			TextAsset textAsset = allFloorData[i];
			FloorSaveData floorSaveData = JsonUtility.FromJson<FloorSaveData>(textAsset.text);
			this.floorData.Add(textAsset.name, floorSaveData);
		}
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x0005D5B8 File Offset: 0x0005B7B8
	public Vector3Int CityTileToTile(Vector2Int coords)
	{
		return new Vector3Int(Mathf.FloorToInt(Mathf.Clamp((float)(coords.x * CityControls.Instance.tileMultiplier) + Mathf.Floor((float)CityControls.Instance.tileMultiplier * 0.5f), 0f, PathFinder.Instance.tileCitySize.x - 1f)), Mathf.FloorToInt(Mathf.Clamp((float)(coords.y * CityControls.Instance.tileMultiplier) + Mathf.Floor((float)CityControls.Instance.tileMultiplier * 0.5f), 0f, PathFinder.Instance.tileCitySize.y - 1f)), 0);
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x0005D668 File Offset: 0x0005B868
	public Vector2Int PathmapToGroundmap(Vector3Int coords)
	{
		return new Vector2Int(Mathf.FloorToInt(Mathf.Clamp(Mathf.Floor((float)(coords.x / CityControls.Instance.tileMultiplier)), 0f, CityData.Instance.citySize.x - 1f)), Mathf.FloorToInt(Mathf.Clamp(Mathf.Floor((float)(coords.y / CityControls.Instance.tileMultiplier)), 0f, CityData.Instance.citySize.y - 1f)));
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x0005D6F4 File Offset: 0x0005B8F4
	public Vector2Int RealPosToGroundmap(Vector3 coords)
	{
		return new Vector2Int(Mathf.FloorToInt(Mathf.Clamp((float)Mathf.FloorToInt((PathFinder.Instance.halfCitySizeReal.x + coords.x) / CityControls.Instance.cityTileSize.x), 0f, CityData.Instance.citySize.x - 1f)), Mathf.FloorToInt(Mathf.Clamp((float)Mathf.FloorToInt((PathFinder.Instance.halfCitySizeReal.y + coords.z) / CityControls.Instance.cityTileSize.y), 0f, CityData.Instance.citySize.y - 1f)));
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x0005D7A8 File Offset: 0x0005B9A8
	public Vector3Int RealPosToPathmap(Vector3 coords)
	{
		return new Vector3Int(Mathf.FloorToInt(Mathf.Clamp((float)Mathf.FloorToInt((PathFinder.Instance.halfCitySizeReal.x + coords.x) / PathFinder.Instance.tileSize.x), 0f, PathFinder.Instance.tileCitySize.x - 1f)), Mathf.FloorToInt(Mathf.Clamp((float)Mathf.FloorToInt((PathFinder.Instance.halfCitySizeReal.y + coords.z) / PathFinder.Instance.tileSize.y), 0f, PathFinder.Instance.tileCitySize.y - 1f)), 0);
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x0005D85C File Offset: 0x0005BA5C
	public Vector3Int RealPosToPathmapIncludingZ(Vector3 coords)
	{
		return new Vector3Int(Mathf.FloorToInt(Mathf.Clamp((float)Mathf.FloorToInt((PathFinder.Instance.halfCitySizeReal.x + coords.x) / PathFinder.Instance.tileSize.x), 0f, PathFinder.Instance.tileCitySize.x - 1f)), Mathf.FloorToInt(Mathf.Clamp((float)Mathf.FloorToInt((PathFinder.Instance.halfCitySizeReal.y + coords.z) / PathFinder.Instance.tileSize.y), 0f, PathFinder.Instance.tileCitySize.y - 1f)), Mathf.FloorToInt(coords.y / PathFinder.Instance.tileSize.z));
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x0005D92C File Offset: 0x0005BB2C
	public Vector3 RealPosToNode(Vector3 coords)
	{
		coords.y += 0.01f;
		return new Vector3(Mathf.Clamp((float)Mathf.FloorToInt((PathFinder.Instance.halfCitySizeReal.x + coords.x) / PathFinder.Instance.nodeSize.x), 0f, PathFinder.Instance.nodeCitySize.x - 1f), Mathf.Clamp((float)Mathf.FloorToInt((PathFinder.Instance.halfCitySizeReal.y + coords.z) / PathFinder.Instance.nodeSize.y), 0f, PathFinder.Instance.nodeCitySize.y - 1f), (float)Mathf.FloorToInt(coords.y / PathFinder.Instance.nodeSize.z));
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x0005DA00 File Offset: 0x0005BC00
	public Vector3 RealPosToNodeFloat(Vector3 coords)
	{
		coords.y += 0.01f;
		return new Vector3(Mathf.Clamp((PathFinder.Instance.halfCitySizeReal.x + coords.x) / PathFinder.Instance.nodeSize.x, 0f, PathFinder.Instance.nodeCitySize.x - 1f), Mathf.Clamp((PathFinder.Instance.halfCitySizeReal.y + coords.z) / PathFinder.Instance.nodeSize.y, 0f, PathFinder.Instance.nodeCitySize.y - 1f), coords.y / PathFinder.Instance.nodeSize.z);
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x0005DAC4 File Offset: 0x0005BCC4
	public Vector3Int RealPosToNodeInt(Vector3 coords)
	{
		coords.y += 0.01f;
		return new Vector3Int(Mathf.FloorToInt(Mathf.Clamp((float)Mathf.FloorToInt((PathFinder.Instance.halfCitySizeReal.x + coords.x) / PathFinder.Instance.nodeSize.x), 0f, PathFinder.Instance.nodeCitySize.x - 1f)), Mathf.FloorToInt(Mathf.Clamp((float)Mathf.FloorToInt((PathFinder.Instance.halfCitySizeReal.y + coords.z) / PathFinder.Instance.nodeSize.y), 0f, PathFinder.Instance.nodeCitySize.y - 1f)), Mathf.FloorToInt(coords.y / PathFinder.Instance.nodeSize.z));
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x0005DBA4 File Offset: 0x0005BDA4
	public Vector3 CityTileToRealpos(Vector2 coords)
	{
		return new Vector3(coords.x * CityControls.Instance.cityTileSize.x - CityData.Instance.citySize.x * 0.5f * CityControls.Instance.cityTileSize.x + CityControls.Instance.cityTileSize.x * 0.5f, 0f, coords.y * CityControls.Instance.cityTileSize.y - CityData.Instance.citySize.y * 0.5f * CityControls.Instance.cityTileSize.y + CityControls.Instance.cityTileSize.y * 0.5f);
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x0005DC60 File Offset: 0x0005BE60
	public Vector3 TileToRealpos(Vector3Int coords)
	{
		return new Vector3((float)coords.x * PathFinder.Instance.tileSize.x - CityData.Instance.citySize.x * 0.5f * CityControls.Instance.cityTileSize.x + PathFinder.Instance.tileSize.x * 0.5f, (float)coords.z * PathFinder.Instance.tileSize.z, (float)coords.y * PathFinder.Instance.tileSize.y - CityData.Instance.citySize.y * 0.5f * CityControls.Instance.cityTileSize.y + PathFinder.Instance.tileSize.y * 0.5f);
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x0005DD34 File Offset: 0x0005BF34
	public Vector3 TileToRealpos(Vector3 coords)
	{
		return new Vector3(coords.x * PathFinder.Instance.tileSize.x - CityData.Instance.citySize.x * 0.5f * CityControls.Instance.cityTileSize.x + PathFinder.Instance.tileSize.x * 0.5f, coords.z * PathFinder.Instance.tileSize.z, coords.y * PathFinder.Instance.tileSize.y - CityData.Instance.citySize.y * 0.5f * CityControls.Instance.cityTileSize.y + PathFinder.Instance.tileSize.y * 0.5f);
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x0005DE00 File Offset: 0x0005C000
	public Vector3 NodeToRealpos(Vector3 coords)
	{
		return new Vector3(coords.x * PathFinder.Instance.nodeSize.x - CityData.Instance.citySize.x * 0.5f * CityControls.Instance.cityTileSize.x + PathFinder.Instance.nodeSize.x * 0.5f, coords.z * PathFinder.Instance.nodeSize.z, coords.y * PathFinder.Instance.nodeSize.y - CityData.Instance.citySize.y * 0.5f * CityControls.Instance.cityTileSize.y + PathFinder.Instance.nodeSize.y * 0.5f);
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x0005DECC File Offset: 0x0005C0CC
	public Vector3 NodeToRealposInt(Vector3Int coords)
	{
		return new Vector3((float)coords.x * PathFinder.Instance.nodeSize.x - CityData.Instance.citySize.x * 0.5f * CityControls.Instance.cityTileSize.x + PathFinder.Instance.nodeSize.x * 0.5f, (float)coords.z * PathFinder.Instance.nodeSize.z, (float)coords.y * PathFinder.Instance.nodeSize.y - CityData.Instance.citySize.y * 0.5f * CityControls.Instance.cityTileSize.y + PathFinder.Instance.nodeSize.y * 0.5f);
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x0005DFA0 File Offset: 0x0005C1A0
	public float GetTileHeight(Vector2 coords)
	{
		float result = 0f;
		Vector3 vector = this.TileToRealpos(new Vector3Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y), 0));
		vector..ctor(vector.x, CameraController.Instance.camHeightLimit.y, vector.z);
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, Vector3.down, ref raycastHit))
		{
			result = raycastHit.transform.position.y;
		}
		return result;
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x0005E01C File Offset: 0x0005C21C
	public void CreateSingletons()
	{
		this.cityDirectory = EvidenceCreator.Instance.CreateEvidence("CityDirectory", "CityDirectory", null, null, null, null, null, false, null);
		this.elevatorControls = EvidenceCreator.Instance.CreateEvidence("ElevatorControls", "ElevatorControls", null, null, null, null, null, false, null);
		this.telephone = (EvidenceCreator.Instance.CreateEvidence("Telephone", "Telephone", null, null, null, null, null, false, null) as EvidenceWitness);
		this.telephone.dialogOptions.Clear();
		this.hospitalBed = (EvidenceCreator.Instance.CreateEvidence("HospitalBed", "HospitalBed", null, null, null, null, null, false, null) as EvidenceWitness);
		this.hospitalBed.dialogOptions.Clear();
		foreach (DialogPreset dialogPreset in Toolbox.Instance.defaultDialogOptions)
		{
			if (dialogPreset.telephoneCallOption)
			{
				this.telephone.AddDialogOption(dialogPreset.tiedToKey, dialogPreset, null, null, true);
			}
			if (dialogPreset.hospitalDecisionOption)
			{
				this.hospitalBed.AddDialogOption(dialogPreset.tiedToKey, dialogPreset, null, null, true);
			}
		}
		foreach (RetailItemPreset retailItemPreset in Toolbox.Instance.allItems)
		{
			if (!(retailItemPreset.itemPreset.spawnEvidence == GameplayControls.Instance.retailItemSoldDiscovery))
			{
				List<object> list = new List<object>();
				list.Add(retailItemPreset.itemPreset);
				list.Add(retailItemPreset);
				Evidence evidence = EvidenceCreator.Instance.CreateEvidence("RetailItemSingleton", retailItemPreset.name, null, null, null, null, null, false, list);
				this.itemSingletons.Add(retailItemPreset, evidence);
			}
		}
		GameplayController.Instance.AddOrMergePhoneNumberData(1540000, false, null, "Indentify Number", false);
		TelephoneController.Instance.AddFakeNumber(1540000, new TelephoneController.CallSource(TelephoneController.CallType.fakeOutbound, CitizenControls.Instance.identifyNumberDialog, InteractionController.ConversationType.normal));
		GameplayController.Instance.AddOrMergePhoneNumberData(5410000, false, null, "Indentify Last Caller", false);
		TelephoneController.Instance.AddFakeNumber(5410000, new TelephoneController.CallSource(TelephoneController.CallType.fakeOutbound, CitizenControls.Instance.lastCallerDialog, InteractionController.ConversationType.normal));
		GameplayController.Instance.AddOrMergePhoneNumberData(9110000, false, null, "Enforcers", false);
		TelephoneController.Instance.AddFakeNumber(9110000, new TelephoneController.CallSource(TelephoneController.CallType.fakeOutbound, CitizenControls.Instance.policeDialog, InteractionController.ConversationType.normal));
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x0005E2A0 File Offset: 0x0005C4A0
	public void CreateCityDirectory()
	{
		Game.Log("CityGen: Generating city directory text...", 2);
		this.cityDirText.Clear();
		foreach (Company company in CityData.Instance.companyDirectory)
		{
			if (company == null)
			{
				Game.LogError("Null entry found in company directory", 2);
			}
			else if (company.preset == null)
			{
				Game.LogError(string.Concat(new string[]
				{
					"Company ",
					company.companyID.ToString(),
					": ",
					company.name,
					" has no preset!"
				}), 2);
			}
			else if (company.name == null || company.name.Length <= 0)
			{
				string[] array = new string[5];
				array[0] = "Company ";
				array[1] = company.companyID.ToString();
				array[2] = ": ";
				int num = 3;
				CompanyPreset preset = company.preset;
				array[num] = ((preset != null) ? preset.ToString() : null);
				array[4] = " Has no name!";
				Game.LogError(string.Concat(array), 2);
			}
		}
		for (int i = 0; i < Toolbox.Instance.alphabet.Length + 1; i++)
		{
			List<CityData.CityDirectoryEntry> list = new List<CityData.CityDirectoryEntry>();
			char character = '0';
			if (i < Toolbox.Instance.alphabet.Length)
			{
				character = Toolbox.Instance.alphabet[i];
				foreach (Company company2 in CityData.Instance.companyDirectory.FindAll((Company item) => item.preset != null && (!item.preset.isSelfEmployed || item.preset.publicFacing) && item.name != null && item.name.Length > 0 && item.name.StartsWith(character.ToString(), 1)))
				{
					Evidence evidenceEntry = company2.placeOfBusiness.evidenceEntry;
					Toolbox instance = Toolbox.Instance;
					Evidence.DataKey[] array2 = new Evidence.DataKey[2];
					array2[0] = Evidence.DataKey.location;
					Strings.LinkData linkData = Strings.AddOrGetLink(evidenceEntry, instance.GetList<Evidence.DataKey>(array2));
					list.Add(new CityData.CityDirectoryEntry
					{
						linkID = linkData.id,
						entryName = company2.name,
						sortString = company2.name
					});
				}
				foreach (NewAddress newAddress in CityData.Instance.addressDirectory.FindAll((NewAddress item) => item.addressPreset != null && item.addressPreset.forceCityDirectoryInclusion && item.name.Length > 0 && item.name.StartsWith(character.ToString(), 1)))
				{
					Strings.LinkData linkData2 = Strings.AddOrGetLink(newAddress.evidenceEntry, null);
					list.Add(new CityData.CityDirectoryEntry
					{
						linkID = linkData2.id,
						entryName = newAddress.name,
						sortString = newAddress.name
					});
				}
				foreach (Citizen citizen in CityData.Instance.citizenDirectory.FindAll((Citizen item) => item.home != null && item.GetSurName().StartsWith(character.ToString(), 1)))
				{
					Strings.LinkData linkData3 = Strings.AddOrGetLink(citizen.home.evidenceEntry, null);
					list.Add(new CityData.CityDirectoryEntry
					{
						linkID = linkData3.id,
						entryName = citizen.GetInitialledName(),
						sortString = citizen.GetSurName()
					});
				}
				if (Player.Instance.home != null && Player.Instance.GetSurName().StartsWith(character.ToString(), 1))
				{
					Game.Log("CityGen: Adding player to city directory under " + Player.Instance.GetSurName(), 2);
					Strings.LinkData linkData4 = Strings.AddOrGetLink(Player.Instance.home.evidenceEntry, null);
					list.Add(new CityData.CityDirectoryEntry
					{
						linkID = linkData4.id,
						entryName = Player.Instance.GetInitialledName(),
						sortString = Player.Instance.GetSurName()
					});
				}
			}
			else
			{
				foreach (Company company3 in CityData.Instance.companyDirectory.FindAll((Company item) => item.preset != null && (!item.preset.isSelfEmployed || item.preset.publicFacing) && item.name != null && item.name.Length > 0 && !Enumerable.Contains<char>(Toolbox.Instance.alphabet, item.name.Substring(0, 1).ToUpper().ToCharArray()[0])))
				{
					Strings.LinkData linkData5 = Strings.AddOrGetLink(company3.placeOfBusiness.evidenceEntry, null);
					list.Add(new CityData.CityDirectoryEntry
					{
						linkID = linkData5.id,
						entryName = company3.name,
						sortString = company3.name
					});
				}
				foreach (NewAddress newAddress2 in CityData.Instance.addressDirectory.FindAll((NewAddress item) => item.addressPreset != null && item.addressPreset.forceCityDirectoryInclusion && item.name.Length > 0 && item.name.StartsWith(character.ToString(), 1)))
				{
					Strings.LinkData linkData6 = Strings.AddOrGetLink(newAddress2.evidenceEntry, null);
					list.Add(new CityData.CityDirectoryEntry
					{
						linkID = linkData6.id,
						entryName = newAddress2.name,
						sortString = newAddress2.name
					});
				}
				foreach (Citizen citizen2 in CityData.Instance.citizenDirectory.FindAll((Citizen item) => item.home != null && item.GetSurName().Length > 0 && !Enumerable.Contains<char>(Toolbox.Instance.alphabet, item.GetSurName().Substring(0, 1).ToUpper().ToCharArray()[0])))
				{
					Strings.LinkData linkData7 = Strings.AddOrGetLink(citizen2.home.evidenceEntry, null);
					list.Add(new CityData.CityDirectoryEntry
					{
						linkID = linkData7.id,
						entryName = citizen2.GetInitialledName(),
						sortString = citizen2.GetSurName()
					});
				}
			}
			list.Sort(new Comparison<CityData.CityDirectoryEntry>(CityData.CityDirectoryEntry.PhoneBookSort));
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < list.Count; j++)
			{
				CityData.CityDirectoryEntry cityDirectoryEntry = list[j];
				if (j > 0)
				{
					stringBuilder.Append("\n");
				}
				string text = string.Concat(new string[]
				{
					"<link=",
					cityDirectoryEntry.linkID.ToString(),
					">",
					cityDirectoryEntry.entryName,
					"</link>"
				});
				stringBuilder.Append(text);
			}
			this.cityDirText.Add(i, stringBuilder.ToString());
		}
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0005E96C File Offset: 0x0005CB6C
	public void GenerateEchelonDecorData()
	{
		this.echelonDefaultWallKey = MaterialsController.Instance.GenerateMaterialKey(CityControls.Instance.echelonWallVariation, CityControls.Instance.echelonColourScheme, null, false, null);
		this.echelonCeilingMatKey = MaterialsController.Instance.GenerateMaterialKey(CityControls.Instance.echelonCeilingVariation, CityControls.Instance.echelonColourScheme, null, false, null);
		this.echelonFloorMatKey = MaterialsController.Instance.GenerateMaterialKey(CityControls.Instance.echelonFloorVariation, CityControls.Instance.echelonColourScheme, null, false, null);
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x0005E9EE File Offset: 0x0005CBEE
	public Vector2Int GetOffsetFromDirection(CityData.BlockingDirection dir)
	{
		if (dir == CityData.BlockingDirection.none)
		{
			return Vector2Int.zero;
		}
		return this.offsetArrayX8[dir - CityData.BlockingDirection.behindLeft];
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x0005EA08 File Offset: 0x0005CC08
	public MetaObject FindMetaObject(int id)
	{
		MetaObject metaObject = null;
		this.metaObjectDictionary.TryGetValue(id, ref metaObject);
		if (metaObject == null)
		{
			Game.Log("Unable to find meta object " + id.ToString(), 2);
		}
		return metaObject;
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x0005EA44 File Offset: 0x0005CC44
	public bool GetHuman(int id, out Human output, bool includePlayer = true)
	{
		output = null;
		if (includePlayer && id == 1)
		{
			output = Player.Instance;
			return true;
		}
		this.citizenDictionary.TryGetValue(id, ref output);
		if (!includePlayer && output != null && output.isPlayer)
		{
			output = null;
			return false;
		}
		return output != null;
	}

	// Token: 0x040005AE RID: 1454
	public string cityName = "New City";

	// Token: 0x040005AF RID: 1455
	public Vector2 citySize = new Vector2(2f, 2f);

	// Token: 0x040005B0 RID: 1456
	public string cityBuiltWith;

	// Token: 0x040005B1 RID: 1457
	public int citizensToGenerate;

	// Token: 0x040005B2 RID: 1458
	public float populationMultiplier = 1f;

	// Token: 0x040005B3 RID: 1459
	public string seed;

	// Token: 0x040005B4 RID: 1460
	public Vector2 maxCoord;

	// Token: 0x040005B5 RID: 1461
	public float boundaryLeft;

	// Token: 0x040005B6 RID: 1462
	public float boundaryRight;

	// Token: 0x040005B7 RID: 1463
	public float boundaryUp;

	// Token: 0x040005B8 RID: 1464
	public float boundaryDown;

	// Token: 0x040005B9 RID: 1465
	public BlockController borderBlock;

	// Token: 0x040005BA RID: 1466
	public Dictionary<string, FloorSaveData> floorData = new Dictionary<string, FloorSaveData>();

	// Token: 0x040005BB RID: 1467
	public List<StreetController> streetDirectory = new List<StreetController>();

	// Token: 0x040005BC RID: 1468
	public List<NewAddress> addressDirectory = new List<NewAddress>();

	// Token: 0x040005BD RID: 1469
	public List<NewFloor> floorDirectory = new List<NewFloor>();

	// Token: 0x040005BE RID: 1470
	public Dictionary<int, NewAddress> addressDictionary = new Dictionary<int, NewAddress>();

	// Token: 0x040005BF RID: 1471
	public List<NewGameLocation> gameLocationDirectory = new List<NewGameLocation>();

	// Token: 0x040005C0 RID: 1472
	public List<NewRoom> roomDirectory = new List<NewRoom>();

	// Token: 0x040005C1 RID: 1473
	public Dictionary<int, NewRoom> roomDictionary = new Dictionary<int, NewRoom>();

	// Token: 0x040005C2 RID: 1474
	public List<ResidenceController> residenceDirectory = new List<ResidenceController>();

	// Token: 0x040005C3 RID: 1475
	public List<Company> companyDirectory = new List<Company>();

	// Token: 0x040005C4 RID: 1476
	public List<Citizen> citizenDirectory = new List<Citizen>();

	// Token: 0x040005C5 RID: 1477
	public List<Citizen> homelessDirectory = new List<Citizen>();

	// Token: 0x040005C6 RID: 1478
	public List<Citizen> homedDirectory = new List<Citizen>();

	// Token: 0x040005C7 RID: 1479
	public Dictionary<int, Human> citizenDictionary = new Dictionary<int, Human>();

	// Token: 0x040005C8 RID: 1480
	public List<Human> deadCitizensDirectory = new List<Human>();

	// Token: 0x040005C9 RID: 1481
	public List<Occupation> jobsDirectory = new List<Occupation>();

	// Token: 0x040005CA RID: 1482
	public List<Occupation> assignedJobsDirectory = new List<Occupation>();

	// Token: 0x040005CB RID: 1483
	public List<Occupation> unemployedDirectory = new List<Occupation>();

	// Token: 0x040005CC RID: 1484
	public List<Occupation> criminalJobDirectory = new List<Occupation>();

	// Token: 0x040005CD RID: 1485
	public List<ReflectionProbeController> reflectionProbeDirectory = new List<ReflectionProbeController>();

	// Token: 0x040005CE RID: 1486
	public List<FurnitureLocation> jobBoardsDirectory = new List<FurnitureLocation>();

	// Token: 0x040005CF RID: 1487
	public Dictionary<int, NewDoor> doorDictionary = new Dictionary<int, NewDoor>();

	// Token: 0x040005D0 RID: 1488
	public List<AirDuctGroup> airDuctGroupDirectory = new List<AirDuctGroup>();

	// Token: 0x040005D1 RID: 1489
	public List<AirDuctGroup.AirVent> airVentDirectory = new List<AirDuctGroup.AirVent>();

	// Token: 0x040005D2 RID: 1490
	public List<Interactable> interactableDirectory = new List<Interactable>();

	// Token: 0x040005D3 RID: 1491
	public List<SceneRecorder> surveillanceDirectory = new List<SceneRecorder>();

	// Token: 0x040005D4 RID: 1492
	public Dictionary<int, Telephone> phoneDictionary = new Dictionary<int, Telephone>();

	// Token: 0x040005D5 RID: 1493
	public Dictionary<int, Interactable> savableInteractableDictionary = new Dictionary<int, Interactable>();

	// Token: 0x040005D6 RID: 1494
	public List<Interactable> caseTrays = new List<Interactable>();

	// Token: 0x040005D7 RID: 1495
	public Dictionary<int, MetaObject> metaObjectDictionary = new Dictionary<int, MetaObject>();

	// Token: 0x040005D8 RID: 1496
	public List<LightController> dynamicShadowSystemLights = new List<LightController>();

	// Token: 0x040005D9 RID: 1497
	public List<Citizen> homlessAssign = new List<Citizen>();

	// Token: 0x040005DA RID: 1498
	public Dictionary<AddressPreset, List<NewAddress>> addressTypeReference = new Dictionary<AddressPreset, List<NewAddress>>();

	// Token: 0x040005DB RID: 1499
	public Dictionary<RetailItemPreset, Evidence> itemSingletons = new Dictionary<RetailItemPreset, Evidence>();

	// Token: 0x040005DC RID: 1500
	public HashSet<NewRoom> visibleRooms = new HashSet<NewRoom>();

	// Token: 0x040005DD RID: 1501
	public List<Actor> visibleActors = new List<Actor>();

	// Token: 0x040005DE RID: 1502
	public Vector2 floorRange = Vector2.zero;

	// Token: 0x040005DF RID: 1503
	public int residentialBuildings;

	// Token: 0x040005E0 RID: 1504
	public int commercialBuildings;

	// Token: 0x040005E1 RID: 1505
	public int industrialBuildings;

	// Token: 0x040005E2 RID: 1506
	public int municipalBuildings;

	// Token: 0x040005E3 RID: 1507
	public int parkBuildings;

	// Token: 0x040005E4 RID: 1508
	public int inhabitedResidences;

	// Token: 0x040005E5 RID: 1509
	public int employedCitizens;

	// Token: 0x040005E6 RID: 1510
	public int extraUnemloyedCreated;

	// Token: 0x040005E7 RID: 1511
	public float averageShoeSize;

	// Token: 0x040005E8 RID: 1512
	public Evidence cityDirectory;

	// Token: 0x040005E9 RID: 1513
	public Evidence elevatorControls;

	// Token: 0x040005EA RID: 1514
	public EvidenceWitness telephone;

	// Token: 0x040005EB RID: 1515
	public EvidenceWitness hospitalBed;

	// Token: 0x040005EC RID: 1516
	public Dictionary<int, string> cityDirText = new Dictionary<int, string>();

	// Token: 0x040005ED RID: 1517
	public Toolbox.MaterialKey echelonFloorMatKey;

	// Token: 0x040005EE RID: 1518
	public Toolbox.MaterialKey echelonCeilingMatKey;

	// Token: 0x040005EF RID: 1519
	public Toolbox.MaterialKey echelonDefaultWallKey;

	// Token: 0x040005F0 RID: 1520
	private static CityData _instance;

	// Token: 0x040005F1 RID: 1521
	public Vector2Int[] offsetArrayX4 = new Vector2Int[]
	{
		new Vector2Int(0, -1),
		new Vector2Int(-1, 0),
		new Vector2Int(1, 0),
		new Vector2Int(0, 1)
	};

	// Token: 0x040005F2 RID: 1522
	public Vector2[] offsetArrayX4StreetJunction = new Vector2[]
	{
		new Vector2(-0.5f, 0.5f),
		new Vector2(0.5f, 0.5f),
		new Vector2(0.5f, -0.5f),
		new Vector2(-0.5f, -0.5f)
	};

	// Token: 0x040005F3 RID: 1523
	public Vector2Int[] offsetArrayX4Diagonal = new Vector2Int[]
	{
		new Vector2Int(-1, -1),
		new Vector2Int(-1, 1),
		new Vector2Int(1, -1),
		new Vector2Int(1, 1)
	};

	// Token: 0x040005F4 RID: 1524
	public Vector2Int[] offsetArrayX8 = new Vector2Int[]
	{
		new Vector2Int(-1, -1),
		new Vector2Int(0, -1),
		new Vector2Int(1, -1),
		new Vector2Int(-1, 0),
		new Vector2Int(1, 0),
		new Vector2Int(-1, 1),
		new Vector2Int(0, 1),
		new Vector2Int(1, 1)
	};

	// Token: 0x040005F5 RID: 1525
	public Vector3Int[] offsetArrayX6 = new Vector3Int[]
	{
		new Vector3Int(0, -1, 0),
		new Vector3Int(-1, 0, 0),
		new Vector3Int(1, 0, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(0, 0, 1),
		new Vector3Int(0, 0, -1)
	};

	// Token: 0x040005F6 RID: 1526
	public Vector2Int[] offsetArrayX24 = new Vector2Int[]
	{
		new Vector2Int(-1, -1),
		new Vector2Int(0, -1),
		new Vector2Int(1, -1),
		new Vector2Int(-1, 0),
		new Vector2Int(1, 0),
		new Vector2Int(-1, 1),
		new Vector2Int(0, 1),
		new Vector2Int(1, 1),
		new Vector2Int(-2, -2),
		new Vector2Int(-1, -2),
		new Vector2Int(0, -2),
		new Vector2Int(1, -2),
		new Vector2Int(2, -2),
		new Vector2Int(2, -1),
		new Vector2Int(2, 0),
		new Vector2Int(2, 1),
		new Vector2Int(2, 2),
		new Vector2Int(1, 2),
		new Vector2Int(0, 2),
		new Vector2Int(-1, 2),
		new Vector2Int(-2, 2),
		new Vector2Int(-2, 1),
		new Vector2Int(-2, 0),
		new Vector2Int(-2, -1)
	};

	// Token: 0x040005F7 RID: 1527
	public int[] angleArrayX4 = new int[]
	{
		0,
		90,
		180,
		270
	};

	// Token: 0x040005F8 RID: 1528
	public int[] angleArrayX8 = new int[]
	{
		0,
		45,
		90,
		135,
		180,
		225,
		270,
		315
	};

	// Token: 0x020000CB RID: 203
	public struct ParsedFloorTile
	{
		// Token: 0x040005F9 RID: 1529
		public Vector2 tileLocation;

		// Token: 0x040005FA RID: 1530
		public int roomID;

		// Token: 0x040005FB RID: 1531
		public List<Vector2> tileAccessList;

		// Token: 0x040005FC RID: 1532
		public int designation;

		// Token: 0x040005FD RID: 1533
		public int tileType;

		// Token: 0x040005FE RID: 1534
		public bool addressAnchor;

		// Token: 0x040005FF RID: 1535
		public float floorRotation;

		// Token: 0x04000600 RID: 1536
		public List<Vector2> doorsAccess;

		// Token: 0x04000601 RID: 1537
		public Dictionary<Vector2, int> windowsAccess;

		// Token: 0x04000602 RID: 1538
		public bool lightswitch;

		// Token: 0x04000603 RID: 1539
		public int cctv;
	}

	// Token: 0x020000CC RID: 204
	public class ParsedFloorData
	{
		// Token: 0x04000604 RID: 1540
		public Dictionary<int, List<CityData.ParsedFloorTile>> unitData;

		// Token: 0x04000605 RID: 1541
		public List<CityData.ParsedFloorTile> allTiles = new List<CityData.ParsedFloorTile>();

		// Token: 0x04000606 RID: 1542
		public Vector2 mainEntranceOutside = new Vector2(-1f, -1f);

		// Token: 0x04000607 RID: 1543
		public Vector2 mainEntranceInside = new Vector2(-1f, -1f);

		// Token: 0x04000608 RID: 1544
		public Dictionary<Vector2, Vector2> additionalEntrances = new Dictionary<Vector2, Vector2>();

		// Token: 0x04000609 RID: 1545
		public float floorHeight;

		// Token: 0x0400060A RID: 1546
		public float ceilingHeight = 0.8f;
	}

	// Token: 0x020000CD RID: 205
	public class CityDirectoryEntry
	{
		// Token: 0x060005EE RID: 1518 RVA: 0x0005F04E File Offset: 0x0005D24E
		public static int PhoneBookSort(CityData.CityDirectoryEntry other1, CityData.CityDirectoryEntry other2)
		{
			if (other2.sortString == other1.sortString)
			{
				return other1.entryName.CompareTo(other2.entryName);
			}
			return other1.sortString.CompareTo(other2.sortString);
		}

		// Token: 0x0400060B RID: 1547
		public int linkID;

		// Token: 0x0400060C RID: 1548
		public string entryName;

		// Token: 0x0400060D RID: 1549
		public string sortString;
	}

	// Token: 0x020000CE RID: 206
	public enum BlockingDirection
	{
		// Token: 0x0400060F RID: 1551
		none,
		// Token: 0x04000610 RID: 1552
		behindLeft,
		// Token: 0x04000611 RID: 1553
		behind,
		// Token: 0x04000612 RID: 1554
		behindRight,
		// Token: 0x04000613 RID: 1555
		left,
		// Token: 0x04000614 RID: 1556
		right,
		// Token: 0x04000615 RID: 1557
		frontLeft,
		// Token: 0x04000616 RID: 1558
		front,
		// Token: 0x04000617 RID: 1559
		frontRight
	}
}
