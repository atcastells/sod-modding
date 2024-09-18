using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200046D RID: 1133
[Serializable]
public class StateSaveData
{
	// Token: 0x0400201B RID: 8219
	[Header("Session Data")]
	public string build;

	// Token: 0x0400201C RID: 8220
	public string cityShare;

	// Token: 0x0400201D RID: 8221
	public List<string> compositionData;

	// Token: 0x0400201E RID: 8222
	public int dynamicPrintsCount;

	// Token: 0x0400201F RID: 8223
	public int sceneCaptureCount;

	// Token: 0x04002020 RID: 8224
	public int sceneCapMax;

	// Token: 0x04002021 RID: 8225
	public string saveTime;

	// Token: 0x04002022 RID: 8226
	public float gameTime;

	// Token: 0x04002023 RID: 8227
	public float timeLimit;

	// Token: 0x04002024 RID: 8228
	public int leapCycle;

	// Token: 0x04002025 RID: 8229
	public int fingerprintLoop;

	// Token: 0x04002026 RID: 8230
	public int assignCaptureID;

	// Token: 0x04002027 RID: 8231
	public int assignMessageThreadID;

	// Token: 0x04002028 RID: 8232
	public int assignGroupID;

	// Token: 0x04002029 RID: 8233
	public int assignStickNote = 1;

	// Token: 0x0400202A RID: 8234
	public int assignInteractableID = 1;

	// Token: 0x0400202B RID: 8235
	public int assignCaseID = 1;

	// Token: 0x0400202C RID: 8236
	public int assignMurderID = 1;

	// Token: 0x0400202D RID: 8237
	public int gameLength = 2;

	// Token: 0x0400202E RID: 8238
	public float currentRain;

	// Token: 0x0400202F RID: 8239
	public float desiredRain;

	// Token: 0x04002030 RID: 8240
	public float currentWind;

	// Token: 0x04002031 RID: 8241
	public float desiredWind;

	// Token: 0x04002032 RID: 8242
	public float currentSnow;

	// Token: 0x04002033 RID: 8243
	public float desiredSnow;

	// Token: 0x04002034 RID: 8244
	public float currentLightning;

	// Token: 0x04002035 RID: 8245
	public float desiredLightning;

	// Token: 0x04002036 RID: 8246
	public float currentFog;

	// Token: 0x04002037 RID: 8247
	public float desiredFog;

	// Token: 0x04002038 RID: 8248
	public float cityWetness;

	// Token: 0x04002039 RID: 8249
	public float citySnow;

	// Token: 0x0400203A RID: 8250
	public float weatherChange;

	// Token: 0x0400203B RID: 8251
	public List<SideJob> basicJobs = new List<SideJob>();

	// Token: 0x0400203C RID: 8252
	public List<SideJobAffair> affairJobs = new List<SideJobAffair>();

	// Token: 0x0400203D RID: 8253
	public List<SideJobSabotage> sabotageJobs = new List<SideJobSabotage>();

	// Token: 0x0400203E RID: 8254
	public List<SideJobStolenItem> stolenItemJobs = new List<SideJobStolenItem>();

	// Token: 0x0400203F RID: 8255
	public List<SideJobMissingPerson> missingPersonJobs = new List<SideJobMissingPerson>();

	// Token: 0x04002040 RID: 8256
	public List<SideJobRevenge> revengeJobs = new List<SideJobRevenge>();

	// Token: 0x04002041 RID: 8257
	public List<SideJobStealBriefcase> briefcaseJobs = new List<SideJobStealBriefcase>();

	// Token: 0x04002042 RID: 8258
	public int jobDiffLevel;

	// Token: 0x04002043 RID: 8259
	public int chapter;

	// Token: 0x04002044 RID: 8260
	public int chapterPart;

	// Token: 0x04002045 RID: 8261
	public StateSaveData.ChaperStateSave chapterSaveState;

	// Token: 0x04002046 RID: 8262
	public bool mapPathActive;

	// Token: 0x04002047 RID: 8263
	public bool mapPathNodeSpecific;

	// Token: 0x04002048 RID: 8264
	public Vector3Int mapPathNode = Vector3Int.zero;

	// Token: 0x04002049 RID: 8265
	public List<Case> activeCases = new List<Case>();

	// Token: 0x0400204A RID: 8266
	public List<Case> archivedCases = new List<Case>();

	// Token: 0x0400204B RID: 8267
	public int activeCase = -1;

	// Token: 0x0400204C RID: 8268
	public List<GameplayController.Footprint> footprints = new List<GameplayController.Footprint>();

	// Token: 0x0400204D RID: 8269
	public List<GameplayController.History> history = new List<GameplayController.History>();

	// Token: 0x0400204E RID: 8270
	public List<GameplayController.Passcode> passcodes = new List<GameplayController.Passcode>();

	// Token: 0x0400204F RID: 8271
	public List<GameplayController.PhoneNumber> numbers = new List<GameplayController.PhoneNumber>();

	// Token: 0x04002050 RID: 8272
	public List<GameplayController.EnforcerCall> enforcerCalls = new List<GameplayController.EnforcerCall>();

	// Token: 0x04002051 RID: 8273
	public List<StateSaveData.CrimeSceneCleanup> crimeSceneCleanup = new List<StateSaveData.CrimeSceneCleanup>();

	// Token: 0x04002052 RID: 8274
	public List<GameplayController.HotelGuest> hotelGuests = new List<GameplayController.HotelGuest>();

	// Token: 0x04002053 RID: 8275
	public List<StateSaveData.BrokenWindowSave> brokenWindows = new List<StateSaveData.BrokenWindowSave>();

	// Token: 0x04002054 RID: 8276
	public NewspaperController.NewspaperState newspaperState;

	// Token: 0x04002055 RID: 8277
	public string playerFirstName = "Fred";

	// Token: 0x04002056 RID: 8278
	public string playerSurname = "Melrose";

	// Token: 0x04002057 RID: 8279
	public Human.Gender playerGender;

	// Token: 0x04002058 RID: 8280
	public Human.Gender partnerGender;

	// Token: 0x04002059 RID: 8281
	public Color playerSkinColour;

	// Token: 0x0400205A RID: 8282
	public int playerBirthDay = 19;

	// Token: 0x0400205B RID: 8283
	public int playerBirthMonth = 12;

	// Token: 0x0400205C RID: 8284
	public int playerBirthYear = 1947;

	// Token: 0x0400205D RID: 8285
	public int residence = -1;

	// Token: 0x0400205E RID: 8286
	public List<int> apartmentsOwned = new List<int>();

	// Token: 0x0400205F RID: 8287
	public bool accidentCover;

	// Token: 0x04002060 RID: 8288
	public List<int> foodH = new List<int>();

	// Token: 0x04002061 RID: 8289
	public List<int> sanitary = new List<int>();

	// Token: 0x04002062 RID: 8290
	public List<int> ops = new List<int>();

	// Token: 0x04002063 RID: 8291
	public List<int> knowsPasswords = new List<int>();

	// Token: 0x04002064 RID: 8292
	public List<GameplayController.LoanDebt> debt = new List<GameplayController.LoanDebt>();

	// Token: 0x04002065 RID: 8293
	public int carried = -1;

	// Token: 0x04002066 RID: 8294
	public bool tutorial;

	// Token: 0x04002067 RID: 8295
	public List<string> tutTextTriggered = new List<string>();

	// Token: 0x04002068 RID: 8296
	public List<FirstPersonItemController.InventorySlot> firstPersonItems = new List<FirstPersonItemController.InventorySlot>();

	// Token: 0x04002069 RID: 8297
	public List<StateSaveData.ScannedObjPrint> scannedPrints = new List<StateSaveData.ScannedObjPrint>();

	// Token: 0x0400206A RID: 8298
	public Vector3 playerPos;

	// Token: 0x0400206B RID: 8299
	public Quaternion playerRot;

	// Token: 0x0400206C RID: 8300
	public int money;

	// Token: 0x0400206D RID: 8301
	public int lockpicks;

	// Token: 0x0400206E RID: 8302
	public int socCredit;

	// Token: 0x0400206F RID: 8303
	public List<string> socCreditPerks = new List<string>();

	// Token: 0x04002070 RID: 8304
	public float health;

	// Token: 0x04002071 RID: 8305
	public float nourishment;

	// Token: 0x04002072 RID: 8306
	public float hydration;

	// Token: 0x04002073 RID: 8307
	public float alertness;

	// Token: 0x04002074 RID: 8308
	public float energy;

	// Token: 0x04002075 RID: 8309
	public float hygiene;

	// Token: 0x04002076 RID: 8310
	public float heat;

	// Token: 0x04002077 RID: 8311
	public float drunk;

	// Token: 0x04002078 RID: 8312
	public float sick;

	// Token: 0x04002079 RID: 8313
	public float headache;

	// Token: 0x0400207A RID: 8314
	public float wet;

	// Token: 0x0400207B RID: 8315
	public float brokenLeg;

	// Token: 0x0400207C RID: 8316
	public float bruised;

	// Token: 0x0400207D RID: 8317
	public float blackEye;

	// Token: 0x0400207E RID: 8318
	public float blackedOut;

	// Token: 0x0400207F RID: 8319
	public float numb;

	// Token: 0x04002080 RID: 8320
	public float poisoned;

	// Token: 0x04002081 RID: 8321
	public float bleeding;

	// Token: 0x04002082 RID: 8322
	public float wellRested;

	// Token: 0x04002083 RID: 8323
	public float starchAddiction;

	// Token: 0x04002084 RID: 8324
	public float syncDiskInstall;

	// Token: 0x04002085 RID: 8325
	public float blinded;

	// Token: 0x04002086 RID: 8326
	public bool crouched;

	// Token: 0x04002087 RID: 8327
	public List<UpgradesController.Upgrades> upgrades = new List<UpgradesController.Upgrades>();

	// Token: 0x04002088 RID: 8328
	public List<string> sabotaged = new List<string>();

	// Token: 0x04002089 RID: 8329
	public List<string> booksRead = new List<string>();

	// Token: 0x0400208A RID: 8330
	public List<SceneRecorder.SceneCapture> playerSavedCaptures = new List<SceneRecorder.SceneCapture>();

	// Token: 0x0400208B RID: 8331
	public List<SpeechController.QueueElement> speech = new List<SpeechController.QueueElement>();

	// Token: 0x0400208C RID: 8332
	public List<int> keyring = new List<int>();

	// Token: 0x0400208D RID: 8333
	public List<int> keyringInt = new List<int>();

	// Token: 0x0400208E RID: 8334
	public List<StateSaveData.FakeTelephone> fakeTelephone = new List<StateSaveData.FakeTelephone>();

	// Token: 0x0400208F RID: 8335
	public int hideInteractable = -1;

	// Token: 0x04002090 RID: 8336
	public int hideRef = -1;

	// Token: 0x04002091 RID: 8337
	public int phoneInteractable = -1;

	// Token: 0x04002092 RID: 8338
	public int computerInteractable = -1;

	// Token: 0x04002093 RID: 8339
	public int duct = -1;

	// Token: 0x04002094 RID: 8340
	public Vector3 storedTransPos;

	// Token: 0x04002095 RID: 8341
	public List<StateSaveData.BuildingStateSav> buildings = new List<StateSaveData.BuildingStateSav>();

	// Token: 0x04002096 RID: 8342
	public List<StateSaveData.CompanyStateSave> companies = new List<StateSaveData.CompanyStateSave>();

	// Token: 0x04002097 RID: 8343
	public List<StateSaveData.MessageThreadSave> messageThreads = new List<StateSaveData.MessageThreadSave>();

	// Token: 0x04002098 RID: 8344
	public bool pgLoop;

	// Token: 0x04002099 RID: 8345
	public int currentMurderer = -1;

	// Token: 0x0400209A RID: 8346
	public int currentVictim = -1;

	// Token: 0x0400209B RID: 8347
	public int currentActiveCase = -1;

	// Token: 0x0400209C RID: 8348
	public string murderPreset;

	// Token: 0x0400209D RID: 8349
	public string chosenMO;

	// Token: 0x0400209E RID: 8350
	public List<int> previousMurderers = new List<int>();

	// Token: 0x0400209F RID: 8351
	public float pauseBetweenMurders;

	// Token: 0x040020A0 RID: 8352
	public float pauseForKidnapperKill;

	// Token: 0x040020A1 RID: 8353
	public bool murderRoutineActive;

	// Token: 0x040020A2 RID: 8354
	public int maxMurderDiffLevel = 1;

	// Token: 0x040020A3 RID: 8355
	public int currentVictimSite = -1;

	// Token: 0x040020A4 RID: 8356
	public bool victimSiteIsStreet;

	// Token: 0x040020A5 RID: 8357
	public bool triggerCoverUpCall;

	// Token: 0x040020A6 RID: 8358
	public bool playerAcceptedCoverUp;

	// Token: 0x040020A7 RID: 8359
	public bool triggerCoverUpSuccess;

	// Token: 0x040020A8 RID: 8360
	public List<MurderController.Murder> murders = new List<MurderController.Murder>();

	// Token: 0x040020A9 RID: 8361
	public List<MurderController.Murder> iaMurders = new List<MurderController.Murder>();

	// Token: 0x040020AA RID: 8362
	public List<StateSaveData.EvidenceStateSave> evidence = new List<StateSaveData.EvidenceStateSave>();

	// Token: 0x040020AB RID: 8363
	public List<string> timeEvidence = new List<string>();

	// Token: 0x040020AC RID: 8364
	public List<string> dateEvidence = new List<string>();

	// Token: 0x040020AD RID: 8365
	public List<string> customStrings = new List<string>();

	// Token: 0x040020AE RID: 8366
	public List<SpatterSimulation> spatter = new List<SpatterSimulation>();

	// Token: 0x040020AF RID: 8367
	public List<CitySaveData.FurnitureClusterObjectCitySave> furnitureStorage = new List<CitySaveData.FurnitureClusterObjectCitySave>();

	// Token: 0x040020B0 RID: 8368
	public bool freeHealthCareFlag;

	// Token: 0x040020B1 RID: 8369
	public int notTheAnswerFlag = -1;

	// Token: 0x040020B2 RID: 8370
	public int privateSlyFlag = -1;

	// Token: 0x040020B3 RID: 8371
	public List<string> allConnectedReference = new List<string>();

	// Token: 0x040020B4 RID: 8372
	public bool pacifistFlag;

	// Token: 0x040020B5 RID: 8373
	public bool notAScratchFlag;

	// Token: 0x040020B6 RID: 8374
	public List<int> spareNoOneReference = new List<int>();

	// Token: 0x040020B7 RID: 8375
	public List<StateSaveData.FloorStateSave> floors = new List<StateSaveData.FloorStateSave>();

	// Token: 0x040020B8 RID: 8376
	public List<StateSaveData.AddressStateSave> addresses = new List<StateSaveData.AddressStateSave>();

	// Token: 0x040020B9 RID: 8377
	public List<StateSaveData.GuestPassStateSave> guestPasses = new List<StateSaveData.GuestPassStateSave>();

	// Token: 0x040020BA RID: 8378
	public List<StateSaveData.RoomStateSave> rooms = new List<StateSaveData.RoomStateSave>();

	// Token: 0x040020BB RID: 8379
	public List<MetaObject> metas = new List<MetaObject>();

	// Token: 0x040020BC RID: 8380
	public List<Interactable> interactables = new List<Interactable>();

	// Token: 0x040020BD RID: 8381
	public List<int> removedCityData = new List<int>();

	// Token: 0x040020BE RID: 8382
	public List<StateSaveData.CitizenStateSave> citizens = new List<StateSaveData.CitizenStateSave>();

	// Token: 0x040020BF RID: 8383
	public List<StateSaveData.DoorStateSave> doors = new List<StateSaveData.DoorStateSave>();

	// Token: 0x0200046E RID: 1134
	[Serializable]
	public class CrimeSceneCleanup
	{
		// Token: 0x040020C0 RID: 8384
		public bool isStreet;

		// Token: 0x040020C1 RID: 8385
		public int id;
	}

	// Token: 0x0200046F RID: 1135
	[Serializable]
	public class BrokenWindowSave
	{
		// Token: 0x040020C2 RID: 8386
		public Vector3 pos;

		// Token: 0x040020C3 RID: 8387
		public float brokenAt;
	}

	// Token: 0x02000470 RID: 1136
	[Serializable]
	public class ScannedObjPrint
	{
		// Token: 0x040020C4 RID: 8388
		public int objID;

		// Token: 0x040020C5 RID: 8389
		public List<int> prints;
	}

	// Token: 0x02000471 RID: 1137
	[Serializable]
	public class ChaperStateSave
	{
		// Token: 0x060018CB RID: 6347 RVA: 0x00171101 File Offset: 0x0016F301
		public void AddData(string reference, int integer)
		{
			this.data.Add(new StateSaveData.ChapterSaveData
			{
				reference = reference,
				data = integer.ToString()
			});
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x00171127 File Offset: 0x0016F327
		public void AddData(string reference, float floatP)
		{
			this.data.Add(new StateSaveData.ChapterSaveData
			{
				reference = reference,
				data = floatP.ToString()
			});
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x0017114D File Offset: 0x0016F34D
		public void AddData(string reference, string str)
		{
			this.data.Add(new StateSaveData.ChapterSaveData
			{
				reference = reference,
				data = str
			});
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x0017116D File Offset: 0x0016F36D
		public void AddData(string reference, bool b)
		{
			if (b)
			{
				this.AddData(reference, 1);
				return;
			}
			this.AddData(reference, 0);
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x00171184 File Offset: 0x0016F384
		public bool GetDataBool(string reference)
		{
			bool result = false;
			if (this.GetDataInt(reference) >= 1)
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060018D0 RID: 6352 RVA: 0x001711A0 File Offset: 0x0016F3A0
		public int GetDataInt(string reference)
		{
			int result = -1;
			StateSaveData.ChapterSaveData chapterSaveData = this.data.Find((StateSaveData.ChapterSaveData item) => item.reference == reference);
			if (chapterSaveData != null)
			{
				int.TryParse(chapterSaveData.data, ref result);
			}
			return result;
		}

		// Token: 0x060018D1 RID: 6353 RVA: 0x001711E8 File Offset: 0x0016F3E8
		public float GetDataFloat(string reference)
		{
			float result = -1f;
			StateSaveData.ChapterSaveData chapterSaveData = this.data.Find((StateSaveData.ChapterSaveData item) => item.reference == reference);
			if (chapterSaveData != null)
			{
				float.TryParse(chapterSaveData.data, ref result);
			}
			return result;
		}

		// Token: 0x060018D2 RID: 6354 RVA: 0x00171234 File Offset: 0x0016F434
		public string GetDataString(string reference)
		{
			string empty = string.Empty;
			StateSaveData.ChapterSaveData chapterSaveData = this.data.Find((StateSaveData.ChapterSaveData item) => item.reference == reference);
			if (chapterSaveData != null)
			{
				empty = chapterSaveData.data;
			}
			return empty;
		}

		// Token: 0x040020C6 RID: 8390
		public List<StateSaveData.ChapterSaveData> data = new List<StateSaveData.ChapterSaveData>();
	}

	// Token: 0x02000475 RID: 1141
	[Serializable]
	public class ChapterSaveData
	{
		// Token: 0x040020CA RID: 8394
		public string reference;

		// Token: 0x040020CB RID: 8395
		public string data;
	}

	// Token: 0x02000476 RID: 1142
	[Serializable]
	public class EvidenceStateSave
	{
		// Token: 0x040020CC RID: 8396
		public string id;

		// Token: 0x040020CD RID: 8397
		public string dds;

		// Token: 0x040020CE RID: 8398
		public bool found;

		// Token: 0x040020CF RID: 8399
		public List<StateSaveData.EvidenceDataKeyTie> keyTies = new List<StateSaveData.EvidenceDataKeyTie>();

		// Token: 0x040020D0 RID: 8400
		public List<Evidence.Discovery> discovery = new List<Evidence.Discovery>();

		// Token: 0x040020D1 RID: 8401
		public bool fs;

		// Token: 0x040020D2 RID: 8402
		public string n;

		// Token: 0x040020D3 RID: 8403
		public List<Evidence.CustomName> customName = new List<Evidence.CustomName>();

		// Token: 0x040020D4 RID: 8404
		public List<EvidenceMultiPage.MultiPageContent> mpContent = new List<EvidenceMultiPage.MultiPageContent>();
	}

	// Token: 0x02000477 RID: 1143
	[Serializable]
	public class EvidenceDataKeyTie
	{
		// Token: 0x040020D5 RID: 8405
		public Evidence.DataKey key;

		// Token: 0x040020D6 RID: 8406
		public List<Evidence.DataKey> tied = new List<Evidence.DataKey>();
	}

	// Token: 0x02000478 RID: 1144
	[Serializable]
	public class FakeTelephone
	{
		// Token: 0x040020D7 RID: 8407
		public int number;

		// Token: 0x040020D8 RID: 8408
		public TelephoneController.CallSource source;
	}

	// Token: 0x02000479 RID: 1145
	[Serializable]
	public class BuildingStateSav
	{
		// Token: 0x040020D9 RID: 8409
		public int id;

		// Token: 0x040020DA RID: 8410
		public bool alarmActive;

		// Token: 0x040020DB RID: 8411
		public float alarmTimer;

		// Token: 0x040020DC RID: 8412
		public NewBuilding.AlarmTargetMode targetMode;

		// Token: 0x040020DD RID: 8413
		public float targetModeSetAt;

		// Token: 0x040020DE RID: 8414
		public List<int> targets = new List<int>();

		// Token: 0x040020DF RID: 8415
		public float wanted;

		// Token: 0x040020E0 RID: 8416
		public List<StateSaveData.ElevatorStateSave> elevators = new List<StateSaveData.ElevatorStateSave>();

		// Token: 0x040020E1 RID: 8417
		public List<TelephoneController.PhoneCall> callLog;

		// Token: 0x040020E2 RID: 8418
		public List<GameplayController.LostAndFound> lostAndFound = new List<GameplayController.LostAndFound>();
	}

	// Token: 0x0200047A RID: 1146
	[Serializable]
	public class ElevatorStateSave
	{
		// Token: 0x040020E3 RID: 8419
		public int tileID;

		// Token: 0x040020E4 RID: 8420
		public float yPos;

		// Token: 0x040020E5 RID: 8421
		public int floor;
	}

	// Token: 0x0200047B RID: 1147
	[Serializable]
	public class FloorStateSave
	{
		// Token: 0x040020E6 RID: 8422
		public int id;

		// Token: 0x040020E7 RID: 8423
		public bool alarmLockdown;
	}

	// Token: 0x0200047C RID: 1148
	[Serializable]
	public class AddressStateSave
	{
		// Token: 0x040020E8 RID: 8424
		public int id;

		// Token: 0x040020E9 RID: 8425
		public int sale = -1;

		// Token: 0x040020EA RID: 8426
		public List<NewAddress.Vandalism> vandalism = new List<NewAddress.Vandalism>();

		// Token: 0x040020EB RID: 8427
		public bool alarmActive;

		// Token: 0x040020EC RID: 8428
		public float alarmTimer;

		// Token: 0x040020ED RID: 8429
		public NewBuilding.AlarmTargetMode targetMode;

		// Token: 0x040020EE RID: 8430
		public float targetModeSetAt;

		// Token: 0x040020EF RID: 8431
		public List<int> targets = new List<int>();

		// Token: 0x040020F0 RID: 8432
		public List<NewGameLocation.TrespassEscalation> escalation = new List<NewGameLocation.TrespassEscalation>();

		// Token: 0x040020F1 RID: 8433
		public float loiter;
	}

	// Token: 0x0200047D RID: 1149
	[Serializable]
	public class CompanyStateSave
	{
		// Token: 0x040020F2 RID: 8434
		public int id;

		// Token: 0x040020F3 RID: 8435
		public List<Company.SalesRecord> sales = new List<Company.SalesRecord>();
	}

	// Token: 0x0200047E RID: 1150
	[Serializable]
	public class GuestPassStateSave
	{
		// Token: 0x040020F4 RID: 8436
		public int id;

		// Token: 0x040020F5 RID: 8437
		public Vector2 guestPassUntil;
	}

	// Token: 0x0200047F RID: 1151
	[Serializable]
	public class RoomStateSave
	{
		// Token: 0x040020F6 RID: 8438
		public int id;

		// Token: 0x040020F7 RID: 8439
		public int ex;

		// Token: 0x040020F8 RID: 8440
		public bool ml = true;

		// Token: 0x040020F9 RID: 8441
		public float gl;

		// Token: 0x040020FA RID: 8442
		public int fID = 1;

		// Token: 0x040020FB RID: 8443
		public int iID = 1;

		// Token: 0x040020FC RID: 8444
		public List<CitySaveData.RoomCitySave> decorOverride;
	}

	// Token: 0x02000480 RID: 1152
	[Serializable]
	public class CitizenStateSave
	{
		// Token: 0x040020FD RID: 8445
		public int id;

		// Token: 0x040020FE RID: 8446
		public Vector3 pos;

		// Token: 0x040020FF RID: 8447
		public Quaternion rot;

		// Token: 0x04002100 RID: 8448
		public int trespassingEscalation;

		// Token: 0x04002101 RID: 8449
		public ClothesPreset.OutfitCategory currentOutfit;

		// Token: 0x04002102 RID: 8450
		public float nourishment;

		// Token: 0x04002103 RID: 8451
		public float hydration;

		// Token: 0x04002104 RID: 8452
		public float alertness;

		// Token: 0x04002105 RID: 8453
		public float energy;

		// Token: 0x04002106 RID: 8454
		public float excitement;

		// Token: 0x04002107 RID: 8455
		public float chores;

		// Token: 0x04002108 RID: 8456
		public float hygiene;

		// Token: 0x04002109 RID: 8457
		public float bladder;

		// Token: 0x0400210A RID: 8458
		public float heat;

		// Token: 0x0400210B RID: 8459
		public float drunk;

		// Token: 0x0400210C RID: 8460
		public float breath;

		// Token: 0x0400210D RID: 8461
		public float poisoned;

		// Token: 0x0400210E RID: 8462
		public float blinded;

		// Token: 0x0400210F RID: 8463
		public int poisoner = -1;

		// Token: 0x04002110 RID: 8464
		public int den = -1;

		// Token: 0x04002111 RID: 8465
		public int kidnapper = -1;

		// Token: 0x04002112 RID: 8466
		public bool remFromWorld;

		// Token: 0x04002113 RID: 8467
		public float currentHealth = 1f;

		// Token: 0x04002114 RID: 8468
		public float currentNerve = 0.1f;

		// Token: 0x04002115 RID: 8469
		public float fsDirt;

		// Token: 0x04002116 RID: 8470
		public float fsBlood;

		// Token: 0x04002117 RID: 8471
		public List<Human.Wound> wounds = new List<Human.Wound>();

		// Token: 0x04002118 RID: 8472
		public Vector3Int investigateLocation;

		// Token: 0x04002119 RID: 8473
		public Vector3 investigatePosition;

		// Token: 0x0400211A RID: 8474
		public Vector3 investigatePositionProjection;

		// Token: 0x0400211B RID: 8475
		public float lastInvestigate;

		// Token: 0x0400211C RID: 8476
		public bool persuit;

		// Token: 0x0400211D RID: 8477
		public bool seesPlayerOnPersuit;

		// Token: 0x0400211E RID: 8478
		public float persuitChaseLogicUses;

		// Token: 0x0400211F RID: 8479
		public int persuitTarget;

		// Token: 0x04002120 RID: 8480
		public bool persuitPlayer;

		// Token: 0x04002121 RID: 8481
		public int escalationLevel;

		// Token: 0x04002122 RID: 8482
		public float minimumInvestigationTimeMultiplier = 1f;

		// Token: 0x04002123 RID: 8483
		public NewAIController.ReactionState reactionState;

		// Token: 0x04002124 RID: 8484
		public List<int> atHome;

		// Token: 0x04002125 RID: 8485
		public bool convicted;

		// Token: 0x04002126 RID: 8486
		public bool unreportable;

		// Token: 0x04002127 RID: 8487
		public bool ko;

		// Token: 0x04002128 RID: 8488
		public float koTime;

		// Token: 0x04002129 RID: 8489
		public bool res;

		// Token: 0x0400212A RID: 8490
		public float resTime;

		// Token: 0x0400212B RID: 8491
		public float spooked;

		// Token: 0x0400212C RID: 8492
		public int spookCount;

		// Token: 0x0400212D RID: 8493
		public Human.Death death;

		// Token: 0x0400212E RID: 8494
		public List<CitizenAnimationController.RagdollSnapshot> ragdollSnapshot;

		// Token: 0x0400212F RID: 8495
		public List<CitizenAnimationController.RagdollSnapshotWorld> ragdollSnapshotWorld;

		// Token: 0x04002130 RID: 8496
		public List<Human.WalletItem> wallet = new List<Human.WalletItem>();

		// Token: 0x04002131 RID: 8497
		public StateSaveData.CurrentGoalStateSave currentGoal;

		// Token: 0x04002132 RID: 8498
		public int fingerprintLoop = -1;

		// Token: 0x04002133 RID: 8499
		public List<string> currentConsumable;

		// Token: 0x04002134 RID: 8500
		public List<int> trash;

		// Token: 0x04002135 RID: 8501
		public List<int> putDown;

		// Token: 0x04002136 RID: 8502
		public List<int> sightingCit = new List<int>();

		// Token: 0x04002137 RID: 8503
		public List<Human.Sighting> sightings = new List<Human.Sighting>();

		// Token: 0x04002138 RID: 8504
		public StateSaveData.AvoidConfineStateSave confine;

		// Token: 0x04002139 RID: 8505
		public List<StateSaveData.AvoidConfineStateSave> avoid = new List<StateSaveData.AvoidConfineStateSave>();
	}

	// Token: 0x02000481 RID: 1153
	[Serializable]
	public class AvoidConfineStateSave
	{
		// Token: 0x0400213A RID: 8506
		public int id = -1;

		// Token: 0x0400213B RID: 8507
		public bool st;
	}

	// Token: 0x02000482 RID: 1154
	[Serializable]
	public class CurrentGoalStateSave
	{
		// Token: 0x0400213C RID: 8508
		public string preset;

		// Token: 0x0400213D RID: 8509
		public float priority;

		// Token: 0x0400213E RID: 8510
		public float trigerTime;

		// Token: 0x0400213F RID: 8511
		public float duration;

		// Token: 0x04002140 RID: 8512
		public Vector3Int passedNode;

		// Token: 0x04002141 RID: 8513
		public int passedInteractable = -1;

		// Token: 0x04002142 RID: 8514
		public int gameLocation = -1;

		// Token: 0x04002143 RID: 8515
		public int room = -1;

		// Token: 0x04002144 RID: 8516
		public bool isAddress;

		// Token: 0x04002145 RID: 8517
		public int passedGroup = -1;

		// Token: 0x04002146 RID: 8518
		public int jobID = -1;

		// Token: 0x04002147 RID: 8519
		public int var = -2;

		// Token: 0x04002148 RID: 8520
		public float activeTime;

		// Token: 0x04002149 RID: 8521
		public List<StateSaveData.AIActionStateSave> actions = new List<StateSaveData.AIActionStateSave>();
	}

	// Token: 0x02000483 RID: 1155
	[Serializable]
	public class AIActionStateSave
	{
		// Token: 0x0400214A RID: 8522
		public string preset;

		// Token: 0x0400214B RID: 8523
		public Vector3 node;

		// Token: 0x0400214C RID: 8524
		public int interactable = -1;

		// Token: 0x0400214D RID: 8525
		public int passedInteractable;

		// Token: 0x0400214E RID: 8526
		public int passedRoom = -1;

		// Token: 0x0400214F RID: 8527
		public int passedGroup = -1;

		// Token: 0x04002150 RID: 8528
		public Vector3Int forcedNode;

		// Token: 0x04002151 RID: 8529
		public bool repeat;

		// Token: 0x04002152 RID: 8530
		public bool inserted;

		// Token: 0x04002153 RID: 8531
		public int iap = 3;
	}

	// Token: 0x02000484 RID: 1156
	[Serializable]
	public class DoorStateSave
	{
		// Token: 0x04002154 RID: 8532
		public int id;

		// Token: 0x04002155 RID: 8533
		public bool l;

		// Token: 0x04002156 RID: 8534
		public float ds;

		// Token: 0x04002157 RID: 8535
		public float ls;

		// Token: 0x04002158 RID: 8536
		public float ajar;

		// Token: 0x04002159 RID: 8537
		public bool cs;
	}

	// Token: 0x02000485 RID: 1157
	[Serializable]
	public class MessageThreadSave
	{
		// Token: 0x0400215A RID: 8538
		public int threadID;

		// Token: 0x0400215B RID: 8539
		public DDSSaveClasses.TreeType msgType = DDSSaveClasses.TreeType.vmail;

		// Token: 0x0400215C RID: 8540
		public string treeID;

		// Token: 0x0400215D RID: 8541
		public int participantA = -1;

		// Token: 0x0400215E RID: 8542
		public int participantB = -1;

		// Token: 0x0400215F RID: 8543
		public int participantC = -1;

		// Token: 0x04002160 RID: 8544
		public int participantD = -1;

		// Token: 0x04002161 RID: 8545
		public List<int> cc = new List<int>();

		// Token: 0x04002162 RID: 8546
		public List<string> messages = new List<string>();

		// Token: 0x04002163 RID: 8547
		public List<int> senders = new List<int>();

		// Token: 0x04002164 RID: 8548
		public List<int> recievers = new List<int>();

		// Token: 0x04002165 RID: 8549
		public List<float> timestamps = new List<float>();

		// Token: 0x04002166 RID: 8550
		public StateSaveData.CustomDataSource ds;

		// Token: 0x04002167 RID: 8551
		public int dsID = -1;
	}

	// Token: 0x02000486 RID: 1158
	public enum CustomDataSource
	{
		// Token: 0x04002169 RID: 8553
		sender,
		// Token: 0x0400216A RID: 8554
		groupID
	}
}
