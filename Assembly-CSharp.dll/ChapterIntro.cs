using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200025D RID: 605
public class ChapterIntro : Chapter
{
	// Token: 0x06000D6C RID: 3436 RVA: 0x000BC727 File Offset: 0x000BA927
	public override void OnLoaded()
	{
		base.OnLoaded();
		this.SetUpMission();
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x000BC738 File Offset: 0x000BA938
	public override void OnGameStart()
	{
		if (this.bed != null)
		{
			Interactable interactable = this.bed.integratedInteractables[Toolbox.Instance.Rand(0, this.bed.integratedInteractables.Count, false)];
			if (!this.loadedFromSave)
			{
				Player.Instance.Teleport(interactable.node, null, true, false);
			}
		}
		base.OnGameStart();
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x000BC79C File Offset: 0x000BA99C
	private void SetUpMission()
	{
		ControlsDisplayController.Instance.disableControlDisplay.Add(InteractablePreset.InteractionKey.flashlight);
		if (!this.loadedFromSave)
		{
			this.apartment = Player.Instance.home;
			this.playersAparment = this.apartment.id;
			this.PickCharacters();
		}
		(this.noteWriter as Citizen).alwaysPassDialogSuccess = true;
		if (this.kidnapper.partner != null)
		{
			this.kidnapper.partner.RemoveFromWorld(true);
		}
		this.kidnapper.unreportable = true;
		this.playerLounge = this.apartment.rooms.Find((NewRoom item) => item.preset == InteriorControls.Instance.lounge);
		this.playerKitchen = this.apartment.rooms.Find((NewRoom item) => item.preset == InteriorControls.Instance.kitchen);
		this.bed = Toolbox.Instance.FindFurnitureWithinGameLocation(this.apartment, InteriorControls.Instance.bed, out this.playerBedroom);
		Interactable interactable = null;
		if (this.bed != null)
		{
			interactable = this.bed.integratedInteractables[Toolbox.Instance.Rand(0, this.bed.integratedInteractables.Count, false)];
			if (!this.loadedFromSave)
			{
				Player.Instance.Teleport(interactable.node, null, true, false);
			}
		}
		this.apartmentEntrance = this.apartment.entrances.Find((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.door);
		this.interiorDoorNode = this.apartmentEntrance.fromNode;
		this.exteriorDoorNode = this.apartmentEntrance.toNode;
		if (this.apartmentEntrance.toNode.gameLocation == this.apartment)
		{
			this.interiorDoorNode = this.apartmentEntrance.toNode;
			this.exteriorDoorNode = this.apartmentEntrance.fromNode;
		}
		if (this.slophouse == null)
		{
			CitizenBehaviour.Instance.UpdateForSale();
			foreach (NewAddress newAddress in GameplayController.Instance.forSale)
			{
				newAddress.CalculateLandValue();
				if (this.slophouse == null || newAddress.normalizedLandValue < this.slophouse.normalizedLandValue)
				{
					this.slophouse = newAddress;
					this.slopHouseID = newAddress.id;
				}
			}
			if (this.slophouse != null)
			{
				if (Game.Instance.collectDebugData)
				{
					Game.Log("Chapter: Picked new flophouse is at " + this.slophouse.name, 2);
				}
			}
			else
			{
				Game.LogError("Chapter: Unable to find flophouse for story mission!", 2);
			}
		}
		float num;
		if (interactable != null)
		{
			this.cityDir = Toolbox.Instance.FindClosestObjectTo(InteriorControls.Instance.cityDirectory, interactable.GetWorldPosition(true), null, this.apartment, null, out num, false);
		}
		this.kidnappersEntrance = this.kidnapper.home.entrances.Find((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.door);
		this.kidnappersDoorNode = this.kidnappersEntrance.toNode;
		if (this.kidnappersEntrance.toNode.gameLocation == this.kidnapper.home)
		{
			this.kidnappersDoorNode = this.kidnappersEntrance.fromNode;
		}
		this.kidnappersDoor = this.kidnappersEntrance.wall.door;
		Toolbox.Instance.FindFurnitureWithinGameLocation(this.kidnapper.home, InteriorControls.Instance.bed, out this.kidnappersBedroom);
		NewRoom newRoom;
		this.kidnappersSafe = Toolbox.Instance.FindFurnitureWithinGameLocation(this.kidnapper.home, InteriorControls.Instance.safe, out newRoom);
		this.noteWritersBed = Toolbox.Instance.FindFurnitureWithinGameLocation(this.noteWriter.home, InteriorControls.Instance.bed, out this.noteWritersBedroom);
		NewRoom startRoom = this.kidnapper.home.entrances[0].fromNode.room;
		NewRoom newRoom2 = this.kidnapper.home.rooms.Find((NewRoom item) => item.preset == InteriorControls.Instance.kitchen);
		if (newRoom2 != null)
		{
			startRoom = newRoom2;
		}
		this.kidnapperBin = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.disposal, startRoom, this.kidnapper, AIActionPreset.FindSetting.homeOnly, true, null, null, null, false, InteractablePreset.SpecialCase.none, false, null, true, false, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
		if (this.kidnapperBin == null)
		{
			Game.LogError("Chapter: Unable to find kidnapper's bin", 2);
		}
		this.kidnapperPhone = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.makeCall, this.kidnapper.home.entrances[0].fromNode.room, this.kidnapper, AIActionPreset.FindSetting.homeOnly, true, null, null, null, false, InteractablePreset.SpecialCase.none, false, null, true, false, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false);
		if (this.kidnapperPhone == null)
		{
			Game.LogError("Chapter: Unable to find kidnapper's phone", 2);
		}
		this.kidnapperRouter = Toolbox.Instance.FindClosestObjectTo(InteriorControls.Instance.telephoneRouter, this.kidnapper.home.entrances[0].fromNode.position, this.kidnapper.home.building, null, null, out num, false);
		if (this.kidnapperRouter == null)
		{
			Game.LogError("Chapter: Unable to find kidnapper's telephone router", 2);
		}
		if (this.kidnapperRouter != null)
		{
			this.kidnapperRouterDoor = Toolbox.Instance.FindClosestObjectTo(InteriorControls.Instance.telephoneRouterDoor, this.kidnapperRouter.GetWorldPosition(true), this.kidnapper.home.building, null, this.kidnapperRouter.node.room, out num, false);
			if (this.kidnapperRouterDoor == null)
			{
				Game.LogError("Chapter: Unable to find kidnapper's telephone router door", 2);
			}
		}
		if (!this.loadedFromSave)
		{
			foreach (NewRoom newRoom3 in this.apartment.rooms)
			{
				foreach (FurnitureLocation furnitureLocation in newRoom3.individualFurniture)
				{
					int i = 0;
					while (i < furnitureLocation.spawnedInteractables.Count)
					{
						Interactable interactable2 = furnitureLocation.spawnedInteractables[i];
						if (this.loadedFromSave)
						{
							goto IL_68F;
						}
						if (interactable2.preset.name == "Key")
						{
							interactable2.Delete();
							i--;
						}
						else
						{
							if (!(interactable2.preset.name == "AddressBook"))
							{
								goto IL_68F;
							}
							interactable2.Delete();
							i--;
						}
						IL_6B7:
						i++;
						continue;
						IL_68F:
						if (this.playerCalendar == null && interactable2.preset.name == "Calendar")
						{
							this.playerCalendar = interactable2;
							goto IL_6B7;
						}
						goto IL_6B7;
					}
				}
			}
			Player.Instance.RemoveFromKeyring(this.apartmentEntrance.door);
			foreach (NewRoom newRoom4 in this.kidnapper.home.rooms)
			{
				foreach (FurnitureLocation furnitureLocation2 in newRoom4.individualFurniture)
				{
					for (int j = 0; j < furnitureLocation2.spawnedInteractables.Count; j++)
					{
						Interactable interactable3 = furnitureLocation2.spawnedInteractables[j];
						if (interactable3.preset.name == "MicroCruncher")
						{
							interactable3.Delete();
							j--;
						}
						else if (this.workID == null && interactable3.preset == InteriorControls.Instance.workID && interactable3.belongsTo == this.kidnapper)
						{
							this.workID = interactable3;
							Game.Log("Chaper: Found kidnapper's work ID...", 2);
						}
						else if (this.kidnappersCalendar == null && interactable3.preset.name == "Calendar")
						{
							this.kidnappersCalendar = interactable3;
						}
						else if (this.kidnappersAddressBook == null && interactable3.preset.name == "AddressBook")
						{
							this.kidnappersAddressBook = interactable3;
							this.addressBookID = this.kidnappersAddressBook.id;
						}
						else if (!this.loadedFromSave && interactable3.belongsTo != null && interactable3.belongsTo != this.kidnapper && interactable3.preset.spawnable && !(interactable3.preset.name == "AssortedBooks1") && !(interactable3.preset.name == "AssortedBooks2") && !(interactable3.preset.name == "KitchenRoll"))
						{
							Game.Log("Chapter: Removing " + interactable3.GetName() + " from kidnapper's apartment...", 2);
							interactable3.Delete();
							j--;
						}
					}
					for (int k = 0; k < furnitureLocation2.integratedInteractables.Count; k++)
					{
						Interactable interactable4 = furnitureLocation2.integratedInteractables[k];
						if (interactable4.preset.name == "Safe" && interactable4.belongsTo != this.kidnapper)
						{
							Game.Log("Chapter: Found a safe that doesn't belong to kidnapper; changing ownership...", 2);
							interactable4.SetOwner(this.kidnapper, true);
							interactable4.passwordSource = this.kidnapper;
							if (interactable4.lockInteractable != null)
							{
								interactable4.lockInteractable.SetOwner(this.kidnapper, true);
								interactable4.passwordSource = this.kidnapper;
							}
						}
					}
				}
				if (!this.loadedFromSave)
				{
					newRoom4.SetMainLights(true, "Chapter", null, false, false);
					foreach (NewNode.NodeAccess nodeAccess in newRoom4.entrances)
					{
						if (nodeAccess.door != null && !nodeAccess.door.isLocked)
						{
							nodeAccess.door.SetOpen(1f, null, true, 1f);
						}
					}
				}
			}
			foreach (Interactable interactable5 in this.kidnapper.inventory)
			{
				if (this.workID == null && interactable5.preset == InteriorControls.Instance.workID && interactable5.belongsTo == this.kidnapper)
				{
					this.workID = interactable5;
					Game.Log("Chapter: Found kidnapper's work ID in kidnapper's inventory", 2);
				}
			}
			if (this.restaurant == null)
			{
				float num2 = -99999f;
				foreach (Company company in CityData.Instance.companyDirectory)
				{
					if (company.publicFacing && company.preset.companyCategories.Contains(CompanyPreset.CompanyCategory.caffeine) && company.preset.name == "AmericanDiner")
					{
						float num3 = 1f;
						if (this.noteWriter.favouritePlaces[CompanyPreset.CompanyCategory.caffeine] == company.address)
						{
							num3 += 1f;
						}
						if (this.kidnapper.favouritePlaces[CompanyPreset.CompanyCategory.caffeine] == company.address)
						{
							num3 += 1f;
						}
						if (num3 > num2)
						{
							num2 = num3;
							this.restaurant = company.address;
							this.eatery = this.restaurant.id;
						}
					}
				}
			}
			List<InteractablePreset> list = new List<InteractablePreset>();
			list.Add(InteriorControls.Instance.handgun);
			list.Add(InteriorControls.Instance.silencer);
			list.Add(InteriorControls.Instance.ammo1);
			this.weaponSeller.company.AddSalesRecord(this.killer, list, SessionData.Instance.gameTime - 12f);
			if (this.weaponsSalesLedger == null)
			{
				foreach (NewRoom newRoom5 in this.weaponSeller.rooms)
				{
					foreach (NewNode newNode in newRoom5.nodes)
					{
						if (this.weaponsSalesLedger == null)
						{
							this.weaponsSalesLedger = newNode.interactables.Find((Interactable item) => item.preset == InteriorControls.Instance.salesLedger);
						}
						if (this.weaponsSalesLedger != null)
						{
							if (Game.Instance.collectDebugData)
							{
								Game.Log("Chapter: Found weapon seller sales ledger", 2);
								break;
							}
							break;
						}
					}
					if (this.weaponsSalesLedger != null)
					{
						break;
					}
				}
			}
		}
		if (this.restaurant != null)
		{
			foreach (NewRoom newRoom6 in this.restaurant.rooms)
			{
				foreach (FurnitureLocation furnitureLocation3 in newRoom6.individualFurniture)
				{
					for (int l = 0; l < furnitureLocation3.spawnedInteractables.Count; l++)
					{
						Interactable interactable6 = furnitureLocation3.spawnedInteractables[l];
						if (interactable6.preset.name == "MicroCruncher")
						{
							this.dinerCruncher = interactable6;
							this.restaurantBackroom = newRoom6;
						}
					}
				}
			}
			if (this.dinerCruncher == null)
			{
				foreach (NewRoom newRoom7 in this.restaurant.rooms)
				{
					foreach (FurnitureLocation furnitureLocation4 in newRoom7.individualFurniture)
					{
						for (int m = 0; m < furnitureLocation4.spawnedInteractables.Count; m++)
						{
							Interactable interactable7 = furnitureLocation4.spawnedInteractables[m];
							if (interactable7.preset.name == "MicroCruncher")
							{
								this.dinerCruncher = interactable7;
								this.restaurantBackroom = newRoom7;
							}
						}
					}
				}
			}
		}
		this.OnObjectsCreated();
		if (!this.loadedFromSave)
		{
			this.ExecutePreSim();
		}
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x000BD7FC File Offset: 0x000BB9FC
	public override StateSaveData.ChaperStateSave GetChapterSaveData()
	{
		StateSaveData.ChaperStateSave chaperStateSave = new StateSaveData.ChaperStateSave();
		chaperStateSave.AddData("notewriter", this.noteWriterID);
		chaperStateSave.AddData("kidnapper", this.kidnapperID);
		chaperStateSave.AddData("killer", this.killerID);
		chaperStateSave.AddData("slophouseowner", this.slopHouseOwnerID);
		chaperStateSave.AddData("apartment", this.apartment.id);
		chaperStateSave.AddData("killerbar", this.killerBarID);
		chaperStateSave.AddData("redgummeet", this.redGumMeetID);
		chaperStateSave.AddData("routeraddress", this.chosenRouterAddressID);
		chaperStateSave.AddData("weaponseller", this.weaponSellerID);
		chaperStateSave.AddData("restaurant", this.eatery);
		chaperStateSave.AddData("slophouse", this.slopHouseID);
		chaperStateSave.AddData("addressbook", this.addressBookID);
		if (this.murderWeapon != null)
		{
			chaperStateSave.AddData("murderweapon", this.murderWeapon.id);
		}
		if (this.note != null)
		{
			chaperStateSave.AddData("note", this.note.id);
		}
		if (this.key != null)
		{
			chaperStateSave.AddData("key", this.key.id);
		}
		if (this.detectiveStuff != null)
		{
			chaperStateSave.AddData("printReader", this.detectiveStuff.id);
		}
		if (this.policeBadge != null)
		{
			chaperStateSave.AddData("policebadge", this.policeBadge.id);
		}
		if (this.playersStorageBox != null)
		{
			chaperStateSave.AddData("box", this.playersStorageBox.id);
		}
		if (this.hairpin != null)
		{
			chaperStateSave.AddData("hairpin", this.hairpin.id);
		}
		if (this.paperclip != null)
		{
			chaperStateSave.AddData("paperclip", this.paperclip.id);
		}
		if (this.robItem != null)
		{
			chaperStateSave.AddData("robItem", this.robItem.id);
		}
		if (this.workID != null)
		{
			chaperStateSave.AddData("workID", this.workID.id);
		}
		if (this.meetingNote != null)
		{
			chaperStateSave.AddData("meetingNote", this.meetingNote.id);
		}
		if (this.noteOnNapkin != null)
		{
			chaperStateSave.AddData("noteOnNapkin", this.noteOnNapkin.id);
		}
		if (this.dinerFlyer != null)
		{
			chaperStateSave.AddData("dinerFlyer", this.dinerFlyer.id);
		}
		if (this.workplaceMessageNote != null)
		{
			chaperStateSave.AddData("workplaceMessageNote", this.workplaceMessageNote.id);
		}
		if (this.workplaceReceipt != null)
		{
			chaperStateSave.AddData("workplaceReceipt", this.workplaceReceipt.id);
		}
		if (this.rewardSyncDisk != null)
		{
			chaperStateSave.AddData("rewardDisk", this.rewardSyncDisk.id);
		}
		if (this.finalNoticeBill != null)
		{
			chaperStateSave.AddData("finalNoticeBill", this.finalNoticeBill.id);
		}
		if (this.evictionNotice != null)
		{
			chaperStateSave.AddData("evictionNotice", this.evictionNotice.id);
		}
		if (this.flophouseWelcomeLetter != null)
		{
			chaperStateSave.AddData("flophouseWelcome", this.flophouseWelcomeLetter.id);
		}
		if (this.flophouseSyncDiskNote != null)
		{
			chaperStateSave.AddData("flophouseSyncDiskNote", this.flophouseSyncDiskNote.id);
		}
		if (this.flophouseJobNote != null)
		{
			chaperStateSave.AddData("flophouseJob", this.flophouseJobNote.id);
		}
		if (this.flophouseSyncDisk != null)
		{
			chaperStateSave.AddData("flophouseSyncDisk", this.flophouseSyncDisk.id);
		}
		chaperStateSave.AddData("meettime", this.meetTime);
		chaperStateSave.AddData("enforcerEventsTrigger", this.enforcerEventsTrigger);
		chaperStateSave.AddData("lastCallPlaced", this.lastCallPlaced);
		chaperStateSave.AddData("notewriterMurderTimer", this.notewriterMurderTimer);
		chaperStateSave.AddData("notewriterManualMurderTrigger", this.notewriterManualMurderTrigger);
		chaperStateSave.AddData("notewriterMurderTriggered", this.notewriterMurderTriggered);
		chaperStateSave.AddData("findNoteWriter", this.findNotewriter);
		chaperStateSave.AddData("notewriterDialogAdded", this.notewriterDialogAdded);
		chaperStateSave.AddData("receiptSearchPromt", this.receiptSearchPromt);
		chaperStateSave.AddData("fingerprintSearchPrompt", this.fingerprintPrompt);
		chaperStateSave.AddData("addressBookSearchPromt", this.addressBookSearchPrompt);
		chaperStateSave.AddData("receiptSearchTimer", this.receiptSearchTimer);
		chaperStateSave.AddData("printsSearchTimer", this.printSearchTimer);
		chaperStateSave.AddData("addressBookSearchTimer", this.addressBookSearchTimer);
		chaperStateSave.AddData("receiptSearchActivated", this.receiptSearchActivated);
		chaperStateSave.AddData("printsSearchActivated", this.printSearchActivated);
		chaperStateSave.AddData("addressBookSearchActivated", this.addressBookSearchActivated);
		chaperStateSave.AddData("discoveredWeaponsDealer", this.discoveredWeaponsDealer);
		chaperStateSave.AddData("completed", this.completed);
		return chaperStateSave;
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x000BDCB8 File Offset: 0x000BBEB8
	public override void LoadStateSaveData(StateSaveData.ChaperStateSave newData)
	{
		this.noteWriterID = newData.GetDataInt("notewriter");
		this.kidnapperID = newData.GetDataInt("kidnapper");
		this.killerID = newData.GetDataInt("killer");
		this.slopHouseOwnerID = newData.GetDataInt("slophouseowner");
		CityData.Instance.GetHuman(this.noteWriterID, out this.noteWriter, true);
		CityData.Instance.GetHuman(this.kidnapperID, out this.kidnapper, true);
		CityData.Instance.GetHuman(this.killerID, out this.killer, true);
		CityData.Instance.GetHuman(this.slopHouseOwnerID, out this.slophouseOwner, true);
		this.killerBarID = newData.GetDataInt("killerbar");
		this.redGumMeetID = newData.GetDataInt("redgummeet");
		this.chosenRouterAddressID = newData.GetDataInt("routeraddress");
		this.playersAparment = newData.GetDataInt("apartment");
		this.weaponSellerID = newData.GetDataInt("weaponseller");
		this.eatery = newData.GetDataInt("restaurant");
		this.slopHouseID = newData.GetDataInt("slophouse");
		this.addressBookID = newData.GetDataInt("addressbook");
		CityData.Instance.addressDictionary.TryGetValue(this.eatery, ref this.restaurant);
		CityData.Instance.addressDictionary.TryGetValue(this.killerBarID, ref this.killerBar);
		CityData.Instance.addressDictionary.TryGetValue(this.redGumMeetID, ref this.redGumMeet);
		CityData.Instance.addressDictionary.TryGetValue(this.chosenRouterAddressID, ref this.chosenRouterAddress);
		CityData.Instance.addressDictionary.TryGetValue(this.playersAparment, ref this.apartment);
		CityData.Instance.addressDictionary.TryGetValue(this.weaponSellerID, ref this.weaponSeller);
		CityData.Instance.addressDictionary.TryGetValue(this.slopHouseID, ref this.slophouse);
		this.murderWeapon = base.LoadInteractableFromData("murderweapon", ref newData);
		this.note = base.LoadInteractableFromData("note", ref newData);
		this.key = base.LoadInteractableFromData("key", ref newData);
		this.detectiveStuff = base.LoadInteractableFromData("printReader", ref newData);
		this.policeBadge = base.LoadInteractableFromData("policebadge", ref newData);
		this.playersStorageBox = base.LoadInteractableFromData("box", ref newData);
		this.hairpin = base.LoadInteractableFromData("hairpin", ref newData);
		this.paperclip = base.LoadInteractableFromData("paperclip", ref newData);
		this.kidnappersAddressBook = base.LoadInteractableFromData("addressbook", ref newData);
		this.robItem = base.LoadInteractableFromData("robItem", ref newData);
		this.workID = base.LoadInteractableFromData("workID", ref newData);
		this.meetingNote = base.LoadInteractableFromData("meetingNote", ref newData);
		this.noteOnNapkin = base.LoadInteractableFromData("noteOnNapkin", ref newData);
		this.dinerFlyer = base.LoadInteractableFromData("dinerFlyer", ref newData);
		this.workplaceMessageNote = base.LoadInteractableFromData("workplaceMessageNote", ref newData);
		this.workplaceReceipt = base.LoadInteractableFromData("workplaceReceipt", ref newData);
		this.rewardSyncDisk = base.LoadInteractableFromData("rewardDisk", ref newData);
		this.finalNoticeBill = base.LoadInteractableFromData("finalNoticeBill", ref newData);
		this.evictionNotice = base.LoadInteractableFromData("evictionNotice", ref newData);
		this.flophouseWelcomeLetter = base.LoadInteractableFromData("flophouseWelcome", ref newData);
		this.flophouseSyncDiskNote = base.LoadInteractableFromData("flophouseSyncDiskNote", ref newData);
		this.flophouseSyncDisk = base.LoadInteractableFromData("flophouseSyncDisk", ref newData);
		this.flophouseJobNote = base.LoadInteractableFromData("flophouseJob", ref newData);
		this.meetTime = newData.GetDataFloat("meettime");
		SessionData.Instance.GameTimeToClock12String(this.meetTime, false);
		SessionData.Instance.ShortDateString(this.meetTime, true);
		this.meetingTimeEvidence = EvidenceCreator.Instance.GetTimeEvidence(this.meetTime, this.meetTime, "time", "", -1, -1);
		this.enforcerEventsTrigger = newData.GetDataBool("enforcerEventsTrigger");
		this.lastCallPlaced = newData.GetDataBool("lastCallPlaced");
		this.notewriterMurderTimer = newData.GetDataFloat("notewriterMurderTimer");
		this.notewriterManualMurderTrigger = newData.GetDataBool("notewriterManualMurderTrigger");
		this.notewriterMurderTriggered = newData.GetDataBool("notewriterMurderTriggered");
		this.receiptSearchPromt = newData.GetDataBool("receiptSearchPromt");
		this.fingerprintPrompt = newData.GetDataBool("fingerprintSearchPrompt");
		this.addressBookSearchPrompt = newData.GetDataBool("addressBookSearchPrompt");
		this.receiptSearchTimer = newData.GetDataFloat("receiptSearchTimer");
		this.receiptSearchActivated = newData.GetDataBool("receiptSearchActivated");
		this.printSearchTimer = newData.GetDataFloat("printsSearchTimer");
		this.printSearchActivated = newData.GetDataBool("printsSearchActivated");
		this.addressBookSearchTimer = newData.GetDataFloat("addressBookSearchTimer");
		this.addressBookSearchActivated = newData.GetDataBool("addressBookSearchActivated");
		this.findNotewriter = newData.GetDataBool("findNoteWriter");
		this.notewriterDialogAdded = newData.GetDataBool("notewriterDialogAdded");
		if (this.findNotewriter || this.notewriterDialogAdded)
		{
			this.noteWriter.evidenceEntry.AddDialogOption(Evidence.DataKey.name, ChapterController.Instance.loadedChapter.dialogEvents[1], null, null, false);
		}
		this.discoveredWeaponsDealer = newData.GetDataBool("discoveredWeaponsDealer");
		this.completed = newData.GetDataBool("completed");
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x000BE233 File Offset: 0x000BC433
	public override void OnObjectsCreated()
	{
		base.OnObjectsCreated();
		if (!this.loadedFromSave)
		{
			this.SpawnPlayerApartmentClues();
			this.SpawnKidnapperClues();
			this.SpawnNotewriterClues();
			this.SpawnKillerClues();
			this.SpawnMiscClues();
		}
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x000BE264 File Offset: 0x000BC464
	private void SpawnPlayerApartmentClues()
	{
		FurnitureLocation furnitureLocation;
		if (this.key == null)
		{
			this.key = this.apartment.PlaceObject(InteriorControls.Instance.keyTabletopOnly, Player.Instance, Player.Instance, null, out furnitureLocation, true, Interactable.PassedVarType.roomID, this.apartmentEntrance.door.passwordDoorsRoom.roomID, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
		}
		float num;
		this.playersStorageBox = Toolbox.Instance.FindClosestObjectTo(InteriorControls.Instance.storageBox, Player.Instance.transform.position, null, this.apartment, null, out num, false);
		if (this.playersStorageBox == null)
		{
			this.playersStorageBox = this.apartment.PlaceObject(InteriorControls.Instance.storageBox, Player.Instance, Player.Instance, null, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", true);
		}
		this.playersStorageBox.SetValue(0.07f);
		this.detectiveStuff = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.detectiveStuff, Player.Instance, Player.Instance, null, this.playersStorageBox.GetWorldPosition(false) + new Vector3(0f, 0.0315f, 0f), this.playersStorageBox.wEuler, null, null, "");
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Detective stuff cr: " + this.detectiveStuff.cr.ToString() + " WPOS: " + this.playersStorageBox.GetWorldPosition(false).ToString(), 2);
		}
		GameplayController.Instance.SetLockpicks(0);
		this.hairpin = this.apartment.PlaceObject(InteriorControls.Instance.hairpin, null, null, null, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", true);
		this.paperclip = this.apartment.PlaceObject(InteriorControls.Instance.paperclip, null, null, null, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", true);
		this.apartment.PlaceObject(InteriorControls.Instance.paperclip, null, null, null, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", true);
		this.apartment.PlaceObject(InteriorControls.Instance.hairpin, null, null, null, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", true);
		this.finalNoticeBill = this.apartment.PlaceObject(InteriorControls.Instance.letter, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.prioritiseNonOwned, 0, null, false, null, null, null, "ea869950-7590-4ca2-b472-9811dd583e57", false);
		this.policeCertificate = this.apartment.PlaceObject(InteriorControls.Instance.document, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 3, InteractablePreset.OwnedPlacementRule.prioritiseOwned, 0, null, false, null, null, null, "581dcd40-f88f-4c44-b52c-4f354807e881", false);
		this.fieldsAdvert = this.apartment.PlaceObject(InteriorControls.Instance.fieldsAd, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
		this.scientificPaper = this.apartment.PlaceObject(InteriorControls.Instance.document, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.prioritiseNonOwned, 0, null, false, null, null, null, "e5e08419-8c20-4aa1-8939-a39c1cb3fc07", false);
		this.apartment.PlaceObject(InteriorControls.Instance.policeSupportFlyer, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.prioritiseOwned, 0, null, false, null, null, null, "", false);
		this.playersPasscodeReminder = this.apartment.PlaceObject(InteriorControls.Instance.note, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.prioritiseNonOwned, 0, null, false, null, null, null, "443a1a96-3309-493a-8137-24c9b427af90", false);
		this.apartment.PlaceObject(InteriorControls.Instance.toothbrush, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
		this.apartment.PlaceObject(InteriorControls.Instance.painkillers, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
		this.apartment.PlaceObject(InteriorControls.Instance.bandage, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
		this.apartment.PlaceObject(InteriorControls.Instance.splint, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
		this.apartment.PlaceObject(InteriorControls.Instance.codebreaker, Player.Instance, Player.Instance, null, out furnitureLocation, null, true, 3, InteractablePreset.OwnedPlacementRule.both, 0, null, false, null, null, null, "", false);
		Toolbox.Instance.NewVmailThread(Player.Instance, Player.Instance, null, null, null, "4e8b5f98-c543-409e-ae34-e692a5ce753f", SessionData.Instance.gameTime - 48f, 999, StateSaveData.CustomDataSource.sender, -1);
		Toolbox.Instance.NewVmailThread(this.slophouseOwner, Player.Instance, null, null, null, "bd550805-e75f-41d1-b41d-a58e852a7ffa", SessionData.Instance.gameTime - 8f, 999, StateSaveData.CustomDataSource.sender, -1);
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x000BE770 File Offset: 0x000BC970
	private void SpawnKidnapperClues()
	{
		FurnitureLocation furnitureLocation;
		this.robItem = this.kidnapper.home.PlaceObject(InteriorControls.Instance.valuableItems[Toolbox.Instance.Rand(0, InteriorControls.Instance.valuableItems.Count, false)], this.kidnapper, this.kidnapper, null, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
		this.robItem.force = true;
		if (this.spareKeyDoormat == null)
		{
			this.spareKeyDoormat = Toolbox.Instance.SpawnSpareKey(this.kidnapper.home, null);
		}
		if (this.workID == null)
		{
			Game.Log("Chapter: Spawning a new work ID for kidnapper...", 2);
			this.workID = this.kidnapper.home.PlaceObject(InteriorControls.Instance.workID, this.kidnapper, this.kidnapper, null, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
		}
		if (this.safePasscode == null)
		{
			this.safePasscode = this.kidnapper.WriteNote(Human.NoteObject.note, "88098251-db09-4c6b-a4db-a1284502e11b", null, this.kidnapper.home, 0, InteractablePreset.OwnedPlacementRule.both, 2, null, false, 0, 0, null);
		}
		this.meetingNote = this.kidnapper.home.PlaceObject(InteriorControls.Instance.note, this.kidnapper, this.killer, this.kidnapper, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "4d51f137-9e56-4e93-8434-fccf3fc3d2f6", false);
		Toolbox.Instance.NewVmailThread(this.kidnapper, this.noteWriter, null, null, null, "63c2e755-2840-44f6-b4da-f72e9a84d890", SessionData.Instance.gameTime, 999, StateSaveData.CustomDataSource.sender, -1);
		Toolbox.Instance.NewVmailThread(this.killer, this.kidnapper, null, null, null, "37d3cbd8-9450-430f-8bf2-9e3efbf7f3f7", SessionData.Instance.gameTime - 24f, 999, StateSaveData.CustomDataSource.sender, -1);
		StateSaveData.MessageThreadSave messageThreadSave = Toolbox.Instance.NewVmailThread(this.kidnapper, this.kidnapper, null, null, null, "ddccb668-d79b-4a92-b106-a896ced366d5", SessionData.Instance.gameTime, 999, StateSaveData.CustomDataSource.sender, -1);
		List<Interactable.Passed> list = new List<Interactable.Passed>();
		Interactable.Passed passed = new Interactable.Passed(Interactable.PassedVarType.vmailThreadID, (float)messageThreadSave.threadID, null);
		list.Add(passed);
		this.printedVmail = this.kidnapper.home.PlaceObject(InteriorControls.Instance.vmailPrintoutStatic, this.kidnapper, this.kidnapper, this.kidnapper, out furnitureLocation, list, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, messageThreadSave.treeID, false);
		this.crumpledFlyer = this.kidnapper.home.PlaceObject(InteriorControls.Instance.crumpledPaper, this.kidnapper, this.kidnapper, null, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "1d64e27b-9992-4688-90ce-eab60bf88dd9", false);
		this.noteOnNapkin = this.kidnapper.home.PlaceObject(InteriorControls.Instance.crumpledPaper, this.kidnapper, this.killer, this.kidnapper, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "48e44a7b-86be-4d0e-8ed8-cd87d76cb64d", false);
		this.tornPhotograph = this.kidnapper.home.PlaceObject(InteriorControls.Instance.crumpledPaper, this.kidnapper, this.killer, this.kidnapper, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.prioritiseOwned, 0, null, false, null, null, null, "8cf8c5d6-8407-4c50-90e5-c88dd8b60af8", false);
		this.kidnapper.home.PlaceObject(InteriorControls.Instance.codebreaker, this.kidnapper, this.kidnapper, null, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
		this.kidnapper.job.employer.address.PlaceObject(InteriorControls.Instance.letter, this.kidnapper, this.kidnapper, null, out furnitureLocation, true, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.ownedOnly, 2, null, false, null, null, null, "b340345c-af9f-4694-a743-adaf78035c8b", false);
		this.workplaceMessageNote = this.kidnapper.job.employer.address.PlaceObject(InteriorControls.Instance.crumpledPaper, this.kidnapper, this.kidnapper, this.kidnapper, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.ownedOnly, 0, null, false, null, null, null, "2dff3b2a-501b-4e4d-afa2-a8eddd3fd3ee", false);
		List<Interactable.Passed> list2 = new List<Interactable.Passed>();
		list2.Add(new Interactable.Passed(Interactable.PassedVarType.companyID, (float)this.killerBar.company.companyID, null));
		list2.Add(new Interactable.Passed(Interactable.PassedVarType.time, SessionData.Instance.gameTime - 24f, null));
		list2.Add(new Interactable.Passed(Interactable.PassedVarType.stringInteractablePreset, -1f, "Gemsteader"));
		list2.Add(new Interactable.Passed(Interactable.PassedVarType.stringInteractablePreset, -1f, "Vodka"));
		this.workplaceReceipt = this.kidnapper.job.employer.address.PlaceObject(InteriorControls.Instance.receipt, this.kidnapper, this.kidnapper, this.kidnapper, out furnitureLocation, list2, false, 0, InteractablePreset.OwnedPlacementRule.ownedOnly, 0, null, false, null, null, null, "", false);
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x000BEC30 File Offset: 0x000BCE30
	private void SpawnNotewriterClues()
	{
		FurnitureLocation furnitureLocation = this.noteWritersBed;
		FurnitureLocation furnitureLocation2;
		if (this.rewardSyncDisk == null && this.noteWriter != null)
		{
			Interactable mailbox = Toolbox.Instance.GetMailbox(this.noteWriter);
			if (mailbox != null)
			{
				this.rewardSyncDisk = mailbox.node.gameLocation.PlaceObject(InteriorControls.Instance.chapterRewardSyncDisk.interactable, this.noteWriter, this.noteWriter, Player.Instance, out furnitureLocation2, null, true, 0, InteractablePreset.OwnedPlacementRule.ownedOnly, 3, InteriorControls.Instance.chapterRewardSyncDisk, false, null, null, null, "", true);
			}
		}
		Acquaintance acquaintance = null;
		float num = -9999999f;
		foreach (Acquaintance acquaintance2 in this.noteWriter.acquaintances)
		{
			if (acquaintance2.with != this.killer && acquaintance2.with != this.kidnapper && acquaintance2.like + acquaintance2.known > num)
			{
				acquaintance = acquaintance2;
				num = acquaintance2.like + acquaintance2.known;
			}
		}
		Toolbox.Instance.NewVmailThread(this.noteWriter, acquaintance.with, null, null, null, "4053b6b3-d28d-4286-bc3a-40b00eabda7f", SessionData.Instance.gameTime - 7f, 999, StateSaveData.CustomDataSource.sender, -1);
		this.travelreceipt = this.noteWriter.home.PlaceObject(InteriorControls.Instance.note, this.noteWriter, this.noteWriter, null, out furnitureLocation2, false, Interactable.PassedVarType.jobID, -1, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "00c130a7-d733-403a-b7fe-302041a5dc23", false);
		this.envelopeWithCredits = this.noteWriter.home.PlaceObject(InteriorControls.Instance.letter, this.noteWriter, this.kidnapper, this.noteWriter, out furnitureLocation2, false, Interactable.PassedVarType.jobID, -1, true, 3, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "b67d4478-2229-47a0-9f41-3d266d6b542a", false);
		this.noteWriter.home.PlaceObject(InteriorControls.Instance.moneyLots, this.noteWriter, this.noteWriter, null, out furnitureLocation2, true, Interactable.PassedVarType.jobID, -1, false, 3, InteractablePreset.OwnedPlacementRule.ownedOnly, 0, null, false, null, null, null, "", false);
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x000BEE54 File Offset: 0x000BD054
	private void SpawnKillerClues()
	{
		FurnitureLocation furnitureLocation;
		this.killerPropaganda = this.killer.home.PlaceObject(InteriorControls.Instance.letter, this.killer, this.killer, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "8ab3445f-a6c2-46ec-993c-f4dda9a47a0b", false);
		this.killerPoliceFines = this.killer.home.PlaceObject(InteriorControls.Instance.letter, this.killer, this.killer, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "af25d949-bce6-4ebf-9d94-0ced43a69aab", false);
		this.killerCorpSponsorship = this.killer.home.PlaceObject(InteriorControls.Instance.letter, this.killer, this.killer, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "a174ed64-3297-4fed-bcc5-0ff939168f69", false);
		this.killerNotewriterDetails = this.killer.home.PlaceObject(InteriorControls.Instance.note, this.killer, this.killer, this.noteWriter, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "3327d0c3-c8ae-4ca8-bcb9-285118273a05", false);
		this.killerBusinessCard = this.killer.home.PlaceObject(InteriorControls.Instance.businessCard, this.killer, this.killer, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "3d63c9fc-3c1d-4d65-abca-2029db278305", false);
		Toolbox.Instance.NewVmailThread(this.killer, this.killer, null, null, null, "cc972db9-1f6e-440e-b276-0bbacf0a8e73", SessionData.Instance.gameTime + 2f, 999, StateSaveData.CustomDataSource.sender, -1);
		Toolbox.Instance.NewVmailThread(this.killer, this.killer, null, null, null, "c821e6b0-8901-4381-a149-d15105e32aa8", SessionData.Instance.gameTime + 2.25f, 999, StateSaveData.CustomDataSource.sender, -1);
		if (this.killerBar != null)
		{
			if (Game.Instance.collectDebugData)
			{
				string text = "Chapter: Killer belongs to poker club, so placing bar tab evidence at ";
				NewAddress newAddress = this.killerBar;
				Game.Log(text + ((newAddress != null) ? newAddress.ToString() : null), 2);
			}
			Human director = this.killerBar.company.director;
			this.killerBarTab = this.killerBar.PlaceObject(InteriorControls.Instance.note, director, director, this.killer, out furnitureLocation, null, true, 0, InteractablePreset.OwnedPlacementRule.prioritiseOwned, 0, null, false, null, null, null, "860a4b02-79a8-4998-afd6-fe1629fd9a0b", false);
		}
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x000BF090 File Offset: 0x000BD290
	private void SpawnMiscClues()
	{
		if (this.playerCalendar != null)
		{
			EvidenceMultiPage evidenceMultiPage = this.playerCalendar.evidence as EvidenceMultiPage;
			evidenceMultiPage.AddStringContentToPage(Game.Instance.playerBirthMonth, Toolbox.Instance.GetNumbericalStringReference(Game.Instance.playerBirthDay) + " — " + Strings.ComposeText(Strings.Get("chapter.introduction", "playersbirthday", Strings.Casing.asIs, false, false, false, null), this.playerCalendar, Strings.LinkSetting.automatic, null, null, false), "\n\n", this.preset.startingDate);
			evidenceMultiPage.AddStringContentToPage(8, Toolbox.Instance.GetNumbericalStringReference(9) + " — " + Strings.ComposeText(Strings.Get("chapter.introduction", "partnersbirthday", Strings.Casing.asIs, false, false, false, null), this.playerCalendar, Strings.LinkSetting.automatic, null, null, false), "\n\n", this.preset.startingDate);
		}
		Interactable interactable = this.kidnappersCalendar;
		if (!this.noteWriter.job.employer.address.owners.Contains(this.noteWriter) && this.noteWriter.job.employer.address.owners.Contains(this.kidnapper))
		{
			new HashSet<NewRoom>(this.kidnapper.job.employer.address.rooms.FindAll((NewRoom item) => item.preset.name == "BusinessBackroom"));
		}
		float num;
		this.kidnapperDiary = Toolbox.Instance.FindClosestObjectTo(InteriorControls.Instance.diary, this.kidnapper.home.entrances[0].fromNode.position, null, this.kidnapper.home, null, out num, false);
		if (this.kidnapperDiary == null)
		{
			FurnitureLocation furnitureLocation;
			this.kidnapperDiary = this.kidnapper.home.PlaceObject(InteriorControls.Instance.diary, this.kidnapper, this.kidnapper, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 3, null, false, null, null, null, "3bc4ab3c-c6e7-4bf7-8c2d-51c4fbf7cc1b", false);
		}
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x000BF294 File Offset: 0x000BD494
	public void ExecutePreSim()
	{
		this.preSimPhase = 0;
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Setting restaurant open: " + this.restaurant.name, 2);
		}
		this.restaurant.company.SetOpen(true, true);
		this.restaurant.company.OnActualOpen();
		foreach (NewRoom newRoom in this.restaurant.rooms)
		{
			newRoom.SetMainLights(true, "Chapter", null, true, true);
		}
		foreach (NewNode.NodeAccess nodeAccess in this.restaurant.entrances)
		{
			if (nodeAccess.door != null)
			{
				nodeAccess.door.SetLocked(false, null, true);
			}
		}
		this.noteWriter.ai.EnableAI(false);
		this.kidnapper.ai.EnableAI(false);
		this.killer.ai.EnableAI(false);
		base.StartCoroutine(this.PreSimHandling());
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x000BF3E0 File Offset: 0x000BD5E0
	private IEnumerator PreSimHandling()
	{
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Handling pre sim...", 2);
		}
		this.handlePreSim = true;
		float prevTime = SessionData.Instance.gameTime;
		bool findRestaurantSeat = false;
		NewNode boothSeatNode = null;
		Interactable boothSeat = null;
		Interactable boothSeat2 = null;
		float sittingTime = 0f;
		float preMeetTime = 0f;
		bool findMeetTime = false;
		float preMeetingLength = 0.2f;
		float meetingTimeLength = 0.6f;
		float phase0LoadRatio = 0.5f;
		bool noteWriterPathingComplete = false;
		bool nWpathsCalc = false;
		int nWcaptureCursor = 0;
		PathFinder.PathData nWcapturePath = null;
		Dictionary<NewRoom, List<NewNode>> nWnodesPool = null;
		Dictionary<NewNode, List<Interactable>> nWcameraCoverage = null;
		HashSet<NewRoom> nWroutesCovered = null;
		bool nWwaitingForCapture = false;
		float nWwaitForCaptureTime = 0f;
		Interactable nWcam = null;
		bool kidnapperPathingComplete = false;
		bool kpathsCalc = false;
		int kcaptureCursor = 0;
		PathFinder.PathData kcapturePath = null;
		Dictionary<NewRoom, List<NewNode>> knodesPool = null;
		Dictionary<NewNode, List<Interactable>> kcameraCoverage = null;
		HashSet<NewRoom> kroutesCovered = null;
		bool kwaitingForCapture = false;
		float kwaitForCaptureTime = 0f;
		Interactable kcam = null;
		float nwAv = 0f;
		float kAv = 0f;
		float recordedDistance2 = 999f;
		float startDistance2 = 999f;
		float phase1LoadRatio = 0.33f;
		bool setupFinalPhase = false;
		float recordedDistance3 = 999f;
		float startDistance3 = 999f;
		float phase2LoadRatio = 0.33f;
		float murderTime = -1f;
		while (this.handlePreSim)
		{
			float num = SessionData.Instance.gameTime - prevTime;
			prevTime = SessionData.Instance.gameTime;
			if (this.preSimPhase == 0)
			{
				if (preMeetTime < preMeetingLength)
				{
					preMeetTime += num;
				}
				else if (!findRestaurantSeat)
				{
					NewRoom newRoom = this.restaurant.rooms.Find((NewRoom item) => item.roomType.name == "Diner" || item.roomType.name == "Eatery" || item.roomType.name == "Bar");
					if (newRoom != null)
					{
						if (newRoom.actionReference.ContainsKey(RoutineControls.Instance.takeConsumable))
						{
							foreach (Interactable interactable in newRoom.actionReference[RoutineControls.Instance.takeConsumable])
							{
								if (interactable.preset.specialCaseFlag != InteractablePreset.SpecialCase.fridge)
								{
									ActionController.Instance.TakeConsumable(interactable, interactable.node, this.noteWriter);
									ActionController.Instance.TakeConsumable(interactable, interactable.node, this.kidnapper);
									break;
								}
							}
						}
						boothSeatNode = null;
						float num2 = float.PositiveInfinity;
						foreach (Interactable interactable2 in this.restaurant.securityCameras)
						{
							if (interactable2.node.room == newRoom)
							{
								if (Game.Instance.collectDebugData)
								{
									Game.Log(string.Concat(new string[]
									{
										"Found camera ",
										interactable2.id.ToString(),
										" in diner that covers ",
										interactable2.sceneRecorder.coveredNodes.Count.ToString(),
										" nodes..."
									}), 2);
								}
								foreach (KeyValuePair<NewNode, List<int>> keyValuePair in interactable2.sceneRecorder.coveredNodes)
								{
									if (keyValuePair.Key.individualFurniture.Exists((FurnitureLocation item) => item.furniture.classes.Exists((FurnitureClass item) => item.name == "1x1DinerBoothTable")))
									{
										float num3 = Vector3.Distance(keyValuePair.Key.position, interactable2.wPos);
										if (num3 <= 4f)
										{
											num3 += 4f;
										}
										num3 -= Toolbox.Instance.Rand(0f, 1.8f, false);
										if (num3 < num2)
										{
											boothSeatNode = keyValuePair.Key;
											num2 = num3;
										}
									}
								}
								if (boothSeatNode == null && Game.Instance.collectDebugData)
								{
									Game.Log("Camera " + interactable2.id.ToString() + " does not cover any nodes featuring a diner table...", 2);
								}
							}
						}
						if (boothSeatNode != null)
						{
							if (newRoom.actionReference.ContainsKey(RoutineControls.Instance.sit))
							{
								foreach (Interactable interactable3 in newRoom.actionReference[RoutineControls.Instance.sit])
								{
									if (interactable3.node == boothSeatNode)
									{
										if (boothSeat == null)
										{
											boothSeat = interactable3;
										}
										else
										{
											if (boothSeat2 == null)
											{
												boothSeat2 = interactable3;
											}
											if (interactable3.furnitureParent != boothSeat.furnitureParent)
											{
												boothSeat2 = interactable3;
											}
										}
									}
								}
							}
							if (boothSeat != null && boothSeat2 != null)
							{
								this.noteWriter.Teleport(boothSeatNode, boothSeat.usagePoint, true, false);
								this.noteWriter.animationController.SetIdleAnimationState(CitizenAnimationController.IdleAnimationState.sitting);
								this.kidnapper.Teleport(boothSeatNode, boothSeat2.usagePoint, true, false);
								this.kidnapper.animationController.SetIdleAnimationState(CitizenAnimationController.IdleAnimationState.sitting);
								findRestaurantSeat = true;
								if (Game.Instance.collectDebugData)
								{
									Game.Log("Chapter: Found and teleported kidnapper, notewriter to booth seats: " + boothSeat.id.ToString() + " and " + boothSeat2.id.ToString(), 2);
								}
							}
							else
							{
								Game.LogError("Chapter: Unable to find 2 booth seats", 2);
							}
						}
						else
						{
							Game.LogError(string.Concat(new string[]
							{
								"Chapter: Unable to find a valid booth seat at ",
								newRoom.name,
								" ",
								newRoom.roomID.ToString(),
								" (",
								this.restaurant.securityCameras.Count.ToString(),
								" cams)"
							}), 2);
						}
					}
					else
					{
						Game.LogError("Chapter: Unable to find restaurant sitting room", 2);
					}
				}
				else
				{
					sittingTime += num;
					if (sittingTime < meetingTimeLength)
					{
						CityConstructor.Instance.loadingProgress = phase0LoadRatio * (sittingTime / meetingTimeLength);
						if (!findMeetTime)
						{
							if (Game.Instance.collectDebugData)
							{
								Game.Log("Chapter: Notewriter and kidnapper are both sitting at the restaurant " + SessionData.Instance.DecimalToClockString(SessionData.Instance.decimalClock, false), 2);
							}
							this.meetTime = SessionData.Instance.gameTime;
							this.meetTime = Mathf.Round(this.meetTime * 4f) / 4f;
							SessionData.Instance.GameTimeToClock12String(this.meetTime, false);
							SessionData.Instance.ShortDateString(this.meetTime, true);
							this.meetingTimeEvidence = EvidenceCreator.Instance.GetTimeEvidence(this.meetTime, this.meetTime, "time", "", -1, -1);
							if (this.kidnappersCalendar != null)
							{
								(this.kidnappersCalendar.evidence as EvidenceMultiPage).AddStringContentToPage(this.preset.startingMonth + 1, Toolbox.Instance.GetNumbericalStringReference(this.preset.startingDate) + " — " + Strings.ComposeText(Strings.Get("chapter.introduction", "kidnappercalendar", Strings.Casing.asIs, false, false, false, null), this.kidnappersCalendar, Strings.LinkSetting.automatic, null, null, false), "\n\n", this.preset.startingDate);
							}
							if (this.dinerFlyer == null)
							{
								Vector3 worldPos = boothSeatNode.position + new Vector3(0f, 1f, 0f);
								List<FurniturePreset.SubObject> list = new List<FurniturePreset.SubObject>();
								List<FurnitureLocation> list2 = new List<FurnitureLocation>();
								foreach (FurnitureLocation furnitureLocation in boothSeatNode.individualFurniture)
								{
									if (!(furnitureLocation.furniture.classes[0].name == "1x1WindowShelf"))
									{
										using (List<FurniturePreset.SubObject>.Enumerator enumerator4 = furnitureLocation.furniture.subObjects.GetEnumerator())
										{
											while (enumerator4.MoveNext())
											{
												FurniturePreset.SubObject sub = enumerator4.Current;
												if (!furnitureLocation.spawnedInteractables.Exists((Interactable item) => item.subObject == sub))
												{
													list.Add(sub);
													list2.Add(furnitureLocation);
												}
											}
										}
									}
								}
								if (list.Count > 0)
								{
									int num4 = Toolbox.Instance.Rand(0, list.Count, false);
									FurniturePreset.SubObject subObject = list[num4];
									FurnitureLocation furniture = list2[num4];
									this.dinerFlyer = InteractableCreator.Instance.CreateFurnitureSpawnedInteractable(InteriorControls.Instance.crumpledPaper, furniture, subObject, this.kidnapper, this.noteWriter, this.kidnapper, null, null, null, "f40252ed-0254-41da-98a1-a3f5d1d09673");
								}
								else
								{
									this.dinerFlyer = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.crumpledPaper, this.kidnapper, this.noteWriter, this.kidnapper, worldPos, Vector3.zero, null, null, "f40252ed-0254-41da-98a1-a3f5d1d09673");
								}
							}
							findMeetTime = true;
						}
					}
					else
					{
						if (Game.Instance.collectDebugData)
						{
							Game.Log("Chapter: Teleporting killer to their home so they can make a call to the kidnapper...", 2);
						}
						this.killer.Teleport(this.killer.FindSafeTeleport(this.killer.home, false, true), null, true, false);
						foreach (Telephone telephone in this.killer.home.telephones)
						{
							if (telephone != null && this.kidnapperPhone != null)
							{
								if (Game.Instance.collectDebugData)
								{
									Game.Log("Chapter: Killer making call to kidnapper...", 2);
								}
								TelephoneController.Instance.CreateNewCall(telephone, this.kidnapperPhone.t, this.killer, this.kidnapper, new TelephoneController.CallSource(TelephoneController.CallType.dds, "8a0d56c3-629a-40d2-9f7a-22aa07a18f48"), 0.1f, true);
								break;
							}
						}
						if (Game.Instance.collectDebugData)
						{
							Game.Log("Chapter: Notewriter heading to player's apartment...", 2);
						}
						this.preSimPhase = 1;
					}
				}
			}
			else if (this.preSimPhase == 1)
			{
				if (!noteWriterPathingComplete)
				{
					if (!nWpathsCalc)
					{
						nWcapturePath = PathFinder.Instance.GetPath(this.noteWriter.FindSafeTeleport(this.restaurant.streetAccess.GetOtherGameLocation(this.restaurant), false, true), this.exteriorDoorNode, this.noteWriter, null);
						if (nWcapturePath != null)
						{
							nWnodesPool = new Dictionary<NewRoom, List<NewNode>>();
							nWcameraCoverage = new Dictionary<NewNode, List<Interactable>>();
							foreach (NewNode.NodeAccess nodeAccess in nWcapturePath.accessList)
							{
								if (nodeAccess.toNode.gameLocation.securityCameras.Count > 0)
								{
									foreach (Interactable interactable4 in nodeAccess.toNode.gameLocation.securityCameras)
									{
										if (interactable4.sceneRecorder.coveredNodes.ContainsKey(nodeAccess.toNode))
										{
											if (!nWnodesPool.ContainsKey(nodeAccess.toNode.room))
											{
												nWnodesPool.Add(nodeAccess.toNode.room, new List<NewNode>());
											}
											nWnodesPool[nodeAccess.toNode.room].Add(nodeAccess.toNode);
											if (!nWcameraCoverage.ContainsKey(nodeAccess.toNode))
											{
												nWcameraCoverage.Add(nodeAccess.toNode, new List<Interactable>());
											}
											nWcameraCoverage[nodeAccess.toNode].Add(interactable4);
										}
									}
								}
							}
							nWroutesCovered = new HashSet<NewRoom>();
							foreach (NewRoom newRoom2 in this.restaurant.rooms)
							{
								nWroutesCovered.Add(newRoom2);
							}
							nWpathsCalc = true;
							nWwaitingForCapture = false;
							nWcaptureCursor = 0;
						}
					}
					else if (!nWwaitingForCapture)
					{
						if (nWcaptureCursor < nWcapturePath.accessList.Count - 1)
						{
							while (nWcaptureCursor < nWcapturePath.accessList.Count - 1 && (nWroutesCovered.Contains(nWcapturePath.accessList[nWcaptureCursor].toNode.room) || !nWnodesPool.ContainsKey(nWcapturePath.accessList[nWcaptureCursor].toNode.room)))
							{
								int num5 = nWcaptureCursor;
								nWcaptureCursor = num5 + 1;
							}
							if (nWcaptureCursor < nWcapturePath.accessList.Count - 1)
							{
								NewNode.NodeAccess nodeAccess2 = nWcapturePath.accessList[nWcaptureCursor];
								if (Game.Instance.collectDebugData)
								{
									Game.Log(string.Concat(new string[]
									{
										"Chapter: Notewriter waiting for cctv capture at ",
										nodeAccess2.toNode.room.name,
										" ",
										nWcaptureCursor.ToString(),
										"/",
										nWcapturePath.accessList.Count.ToString()
									}), 2);
								}
								NewNode newNode = nWnodesPool[nodeAccess2.toNode.room][Toolbox.Instance.Rand(0, nWnodesPool[nodeAccess2.toNode.room].Count, false)];
								this.noteWriter.Teleport(newNode, null, true, false);
								this.noteWriter.animationController.SetIdleAnimationState(CitizenAnimationController.IdleAnimationState.none);
								if (nWcaptureCursor < nWcapturePath.accessList.Count - 2)
								{
									Vector3 vector = nWcapturePath.accessList[nWcaptureCursor + 1].toNode.position - this.noteWriter.transform.position;
									vector.y = 0f;
									if (vector != Vector3.zero)
									{
										this.noteWriter.transform.rotation = Quaternion.LookRotation(vector);
									}
								}
								nWcam = nWcameraCoverage[newNode][0];
								nWwaitForCaptureTime = SessionData.Instance.gameTime;
								nWwaitingForCapture = true;
							}
						}
						else
						{
							noteWriterPathingComplete = true;
						}
					}
					else
					{
						if (nWcam != null && nWcam.sceneRecorder.lastCaptureAt >= nWwaitForCaptureTime)
						{
							if (Game.Instance.collectDebugData)
							{
								Game.Log(string.Concat(new string[]
								{
									"Chapter: Notewriter cctv capture at ",
									nWcam.node.room.name,
									" ",
									nWcaptureCursor.ToString(),
									"/",
									nWcapturePath.accessList.Count.ToString()
								}), 2);
							}
							nWroutesCovered.Add(nWcapturePath.accessList[nWcaptureCursor].toNode.room);
						}
						nWwaitingForCapture = false;
					}
				}
				if (!kidnapperPathingComplete)
				{
					if (!kpathsCalc)
					{
						kcapturePath = PathFinder.Instance.GetPath(this.kidnapper.FindSafeTeleport(this.restaurant.streetAccess.GetOtherGameLocation(this.restaurant), false, true), this.kidnappersDoorNode, this.kidnapper, null);
						if (kcapturePath != null)
						{
							knodesPool = new Dictionary<NewRoom, List<NewNode>>();
							kcameraCoverage = new Dictionary<NewNode, List<Interactable>>();
							foreach (NewNode.NodeAccess nodeAccess3 in kcapturePath.accessList)
							{
								if (nodeAccess3.toNode.gameLocation.securityCameras.Count > 0)
								{
									foreach (Interactable interactable5 in nodeAccess3.toNode.gameLocation.securityCameras)
									{
										if (interactable5.sceneRecorder.coveredNodes.ContainsKey(nodeAccess3.toNode))
										{
											if (!knodesPool.ContainsKey(nodeAccess3.toNode.room))
											{
												knodesPool.Add(nodeAccess3.toNode.room, new List<NewNode>());
											}
											knodesPool[nodeAccess3.toNode.room].Add(nodeAccess3.toNode);
											if (!kcameraCoverage.ContainsKey(nodeAccess3.toNode))
											{
												kcameraCoverage.Add(nodeAccess3.toNode, new List<Interactable>());
											}
											kcameraCoverage[nodeAccess3.toNode].Add(interactable5);
										}
									}
								}
							}
							kroutesCovered = new HashSet<NewRoom>();
							foreach (NewRoom newRoom3 in this.restaurant.rooms)
							{
								kroutesCovered.Add(newRoom3);
							}
							kpathsCalc = true;
							kwaitingForCapture = false;
							kcaptureCursor = 0;
						}
					}
					else if (!kwaitingForCapture)
					{
						if (kcaptureCursor < kcapturePath.accessList.Count - 1)
						{
							while (kcaptureCursor < kcapturePath.accessList.Count - 1 && (kroutesCovered.Contains(kcapturePath.accessList[kcaptureCursor].toNode.room) || !knodesPool.ContainsKey(kcapturePath.accessList[kcaptureCursor].toNode.room)))
							{
								int num5 = kcaptureCursor;
								kcaptureCursor = num5 + 1;
							}
							if (kcaptureCursor < kcapturePath.accessList.Count - 1)
							{
								NewNode.NodeAccess nodeAccess4 = kcapturePath.accessList[kcaptureCursor];
								if (Game.Instance.collectDebugData)
								{
									Game.Log(string.Concat(new string[]
									{
										"Chapter: Kidnapper waiting for cctv capture at ",
										nodeAccess4.toNode.room.name,
										" ",
										kcaptureCursor.ToString(),
										"/",
										kcapturePath.accessList.Count.ToString()
									}), 2);
								}
								NewNode newNode2 = knodesPool[nodeAccess4.toNode.room][Toolbox.Instance.Rand(0, knodesPool[nodeAccess4.toNode.room].Count, false)];
								this.kidnapper.Teleport(newNode2, null, true, false);
								this.kidnapper.animationController.SetIdleAnimationState(CitizenAnimationController.IdleAnimationState.none);
								if (kcaptureCursor < kcapturePath.accessList.Count - 2)
								{
									Vector3 vector2 = kcapturePath.accessList[kcaptureCursor + 1].toNode.position - this.kidnapper.transform.position;
									vector2.y = 0f;
									if (vector2 != Vector3.zero)
									{
										this.kidnapper.transform.rotation = Quaternion.LookRotation(vector2);
									}
								}
								kcam = kcameraCoverage[newNode2][0];
								kwaitForCaptureTime = SessionData.Instance.gameTime;
								kwaitingForCapture = true;
							}
						}
						else
						{
							kidnapperPathingComplete = true;
						}
					}
					else
					{
						if (kcam != null && kcam.sceneRecorder.lastCaptureAt >= kwaitForCaptureTime)
						{
							if (Game.Instance.collectDebugData)
							{
								Game.Log(string.Concat(new string[]
								{
									"Chapter: Kidnapper cctv capture at ",
									kcam.node.room.name,
									" ",
									kcaptureCursor.ToString(),
									"/",
									kcapturePath.accessList.Count.ToString()
								}), 2);
							}
							kroutesCovered.Add(kcapturePath.accessList[kcaptureCursor].toNode.room);
						}
						kwaitingForCapture = false;
					}
				}
				if (!noteWriterPathingComplete && !kidnapperPathingComplete)
				{
					float num6 = Vector3.Distance(this.noteWriter.transform.position, this.exteriorDoorNode.position);
					if (num6 < recordedDistance3)
					{
						recordedDistance3 = num6;
						if (startDistance3 >= 999f)
						{
							startDistance3 = recordedDistance3;
						}
						nwAv = 1f - recordedDistance3 / startDistance3;
					}
					num6 = Vector3.Distance(this.kidnapper.transform.position, this.kidnappersDoorNode.position);
					if (num6 < recordedDistance2)
					{
						recordedDistance2 = num6;
						if (startDistance2 >= 999f)
						{
							startDistance2 = recordedDistance2;
						}
						kAv = 1f - recordedDistance2 / startDistance2;
					}
					CityConstructor.Instance.loadingProgress = phase0LoadRatio + phase1LoadRatio * ((nwAv + kAv) / 2f);
				}
				else
				{
					recordedDistance3 = 999f;
					startDistance3 = 999f;
					this.preSimPhase = 2;
				}
			}
			else if (this.preSimPhase == 2)
			{
				if (!setupFinalPhase)
				{
					foreach (Interactable interactable6 in this.restaurant.securityCameras)
					{
						foreach (SceneRecorder.SceneCapture sceneCapture in interactable6.cap)
						{
							sceneCapture.k = true;
						}
					}
					this.kidnapper.Teleport(this.kidnapper.FindSafeTeleport(this.kidnapper.home, false, true), null, true, false);
					this.kidnapper.animationController.SetIdleAnimationState(CitizenAnimationController.IdleAnimationState.none);
					foreach (Human human in this.kidnapper.home.inhabitants)
					{
						if (!(human == this.kidnapper))
						{
							human.Teleport(human.FindSafeTeleport(this.restaurant, false, true), null, true, false);
							human.ai.AddAvoidLocation(this.kidnapper.home);
						}
					}
					this.noteWriter.Teleport(this.noteWriter.FindSafeTeleport(this.noteWriter.home, false, true), null, true, false);
					this.noteWriter.ai.SetConfineLocation(this.noteWriter.home);
					foreach (Human human2 in this.noteWriter.home.inhabitants)
					{
						if (!(human2 == this.noteWriter))
						{
							human2.Teleport(human2.FindSafeTeleport(this.restaurant, false, true), null, true, false);
							human2.ai.AddAvoidLocation(this.noteWriter.home);
						}
					}
					this.noteWriter.ai.EnableAI(true);
					this.kidnapper.ai.EnableAI(true);
					if (this.kidnapperBin != null)
					{
						if (Game.Instance.collectDebugData)
						{
							Game.Log("Chapter: Dispose of " + this.kidnapper.trash.Count.ToString() + " trash objects in bin...", 2);
						}
						ActionController.Instance.Dispose(this.kidnapperBin, this.kidnapperBin.node, this.kidnapper);
					}
					Toolbox.Instance.NewVmailThread(this.kidnapper, this.noteWriter, null, null, null, "0b71ed7f-88e4-4887-9fd4-dd371253a30d", SessionData.Instance.gameTime, 999, StateSaveData.CustomDataSource.sender, -1);
					for (int i = 0; i < this.kidnapper.trash.Count; i++)
					{
						MetaObject metaObject = CityData.Instance.FindMetaObject(this.kidnapper.trash[i]);
						if (metaObject != null)
						{
							InteractablePreset interactablePreset = null;
							if (Toolbox.Instance.objectPresetDictionary.TryGetValue(metaObject.preset, ref interactablePreset))
							{
								metaObject.passed.Add(new Interactable.Passed(Interactable.PassedVarType.isTrash, SessionData.Instance.gameTime, null));
								FurnitureLocation furnitureLocation2;
								this.kidnapper.home.PlaceObject(interactablePreset, this.kidnapper, this.kidnapper, null, out furnitureLocation2, metaObject.passed, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
								if (Game.Instance.collectDebugData)
								{
									Game.Log("Chapter: Dispose of " + interactablePreset.name + " in apartment...", 2);
								}
								if (interactablePreset.disposal == Human.DisposalType.anywhere)
								{
									this.kidnapper.anywhereTrash--;
								}
								this.kidnapper.trash.RemoveAt(i);
								i--;
								break;
							}
							Game.LogError("Could not find preset for " + metaObject.preset, 2);
						}
						this.kidnapper.trash.RemoveAt(i);
						i--;
					}
					if (this.kidnapper.ai != null)
					{
						this.kidnapper.ai.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
					}
					if (Game.Instance.collectDebugData)
					{
						Game.Log("Chapter: Ready to execute murder...", 2);
					}
					if (this.killer.currentBuilding != this.kidnapper.home.building && this.kidnapper.home.building.floors.ContainsKey(0))
					{
						if (Game.Instance.collectDebugData)
						{
							Game.Log("Chapter: Teleporting killer " + this.killer.GetCitizenName() + " to notewriter's building ground floor lobby...", 2);
						}
						this.killer.Teleport(this.killer.FindSafeTeleport(this.kidnapper.home.building.floors[0].GetLobbyAddress(), false, true), null, true, false);
						this.killer.footstepDirt = 1f;
					}
					if (this.murderWeapon.inInventory != this.killer)
					{
						this.murderWeapon.SetInInventory(this.killer);
					}
					this.killer.ai.EnableAI(true);
					this.murder = MurderController.Instance.ExecuteNewMurder(this.killer, this.kidnapper, this.preset.crimePool[Toolbox.Instance.Rand(0, this.preset.crimePool.Count, false)], this.preset.MOPool[Toolbox.Instance.Rand(0, this.preset.MOPool.Count, false)], null);
					this.murder.OnStateChanged += this.OnMurderStateChange;
					setupFinalPhase = true;
				}
				if (this.killer.ai.currentGoal != null)
				{
					float num7 = Vector3.Distance(this.killer.transform.position, this.kidnapper.home.GetMainEntrance().worldAccessPoint);
					if (num7 < recordedDistance3)
					{
						recordedDistance3 = num7;
						if (startDistance3 >= 999f)
						{
							startDistance3 = recordedDistance3;
						}
						CityConstructor.Instance.loadingProgress = phase0LoadRatio + phase1LoadRatio + phase2LoadRatio * (1f - recordedDistance3 / startDistance3);
					}
				}
			}
			if (this.murderPreSimPass)
			{
				if (murderTime <= 0f)
				{
					murderTime = SessionData.Instance.gameTime;
				}
				else if (SessionData.Instance.gameTime >= murderTime + 0.25f)
				{
					if (Game.Instance.collectDebugData)
					{
						Game.Log("Chapter: Pre sim complete! The time is " + SessionData.Instance.decimalClock.ToString(), 2);
					}
					CityConstructor.Instance.SetPreSim(false);
					this.handlePreSim = false;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x000BF3F0 File Offset: 0x000BD5F0
	public override void OnGameWorldLoop()
	{
		base.OnGameWorldLoop();
		float num = SessionData.Instance.gameTime - CitizenBehaviour.Instance.timeOnLastGameWorldUpdate;
		if (ChapterController.Instance.currentPart == 1 && InteractionController.Instance.lockedInInteraction == null)
		{
			SessionData.Instance.TutorialTrigger("movement", false);
		}
		if (this.playersStorageBox != null && this.playersStorageBox.locked)
		{
			int num2 = GameplayController.Instance.lockPicks;
			this.lockpicksNeeded = Toolbox.Instance.GetLockpicksNeeded(this.playersStorageBox.val) + 2;
			if (num2 < this.lockpicksNeeded)
			{
				foreach (NewNode newNode in this.apartment.nodes)
				{
					List<Interactable> list = newNode.interactables.FindAll((Interactable item) => item.preset.name == "Paperclip" || item.preset.name == "Hairpin");
					num2 += list.Count;
					if (num2 >= this.lockpicksNeeded)
					{
						break;
					}
				}
				if (num2 < this.lockpicksNeeded)
				{
					if (Game.Instance.collectDebugData)
					{
						Game.Log("Chapter: Spawned extra paperclip...", 2);
					}
					FurnitureLocation furnitureLocation;
					Player.Instance.home.PlaceObject(InteriorControls.Instance.paperclip, null, null, null, out furnitureLocation, false, Interactable.PassedVarType.jobID, -1, true, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", true);
				}
			}
		}
		if (!this.lastCallPlaced && this.kidnapper.isDead)
		{
			this.lastCallPlaced = true;
		}
		if (ChapterController.Instance.currentPart >= 1 && ChapterController.Instance.currentPart <= 20 && !Player.Instance.playerKOInProgress)
		{
			if (Player.Instance.currentGameLocation != Player.Instance.home && Player.Instance.previousGameLocation != Player.Instance.home && this.triggeredTutorialSkip <= 0f)
			{
				this.triggeredTutorialSkip += 0.15f;
				if (Game.Instance.collectDebugData)
				{
					Game.Log("Chapter: Asking to skip apartment tutorial section...", 2);
				}
				PopupMessageController.Instance.OnLeftButton += this.OnReturnToApartmentOption;
				PopupMessageController.Instance.OnRightButton += this.OnSkipAheadOption;
				PopupMessageController.Instance.OnOptionButton += this.OnCancelOption;
				PopupMessageController.Instance.PopupMessage("Tutorial skip", true, true, "Return to Apartment", "Skip to Next Tutorial Section", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", true, "Cancel", false, "", "");
			}
			else if (this.triggeredTutorialSkip > 0f)
			{
				this.triggeredTutorialSkip -= num;
			}
		}
		if (Player.Instance.currentGameLocation == this.kidnapper.home)
		{
			foreach (NewRoom newRoom in this.kidnapper.home.rooms)
			{
				foreach (FurnitureLocation furnitureLocation2 in newRoom.individualFurniture)
				{
					foreach (Interactable interactable in furnitureLocation2.integratedInteractables)
					{
						if (interactable.preset.fingerprintsEnabled && interactable.preset.enableDynamicFingerprints)
						{
							if (interactable.preset.actionsPreset.Exists((InteractableActionsPreset item) => item.actions.Exists((InteractablePreset.InteractionAction item) => item.interactionName.ToLower() == "open" || item.interactionName.ToLower() == "close")) && !interactable.df.Exists((Interactable.DynamicFingerprint item) => item.id == this.killer.humanID))
							{
								if (Game.Instance.collectDebugData)
								{
									Game.Log("Chapter: Adding 2x killer's prints to " + interactable.name + " on " + furnitureLocation2.furniture.name, 2);
								}
								interactable.AddNewDynamicFingerprint(this.killer, Interactable.PrintLife.timed);
								interactable.AddNewDynamicFingerprint(this.killer, Interactable.PrintLife.timed);
							}
						}
					}
				}
			}
		}
		if (Player.Instance.currentGameLocation == this.kidnapper.home && !this.enforcerEventsTrigger && GameplayController.Instance.enforcerCalls.ContainsKey(this.kidnapper.home) && (GameplayController.Instance.enforcerCalls[this.kidnapper.home].state == GameplayController.EnforcerCallState.responding || GameplayController.Instance.enforcerCalls[this.kidnapper.home].state == GameplayController.EnforcerCallState.arrived))
		{
			foreach (NewNode.NodeAccess nodeAccess in this.kidnapper.home.entrances)
			{
				foreach (NewNode.NodeSpace nodeSpace in nodeAccess.fromNode.occupiedSpace)
				{
					Human human = nodeSpace.occupier as Human;
					if (human != null && GameplayController.Instance.enforcerCalls[this.kidnapper.home].response.Contains(human.humanID))
					{
						this.TriggerEscapeEvents();
						break;
					}
					if (this.enforcerEventsTrigger)
					{
						break;
					}
				}
				if (this.enforcerEventsTrigger)
				{
					break;
				}
			}
			if (!this.enforcerEventsTrigger)
			{
				foreach (NewNode.NodeAccess nodeAccess2 in this.kidnapper.home.entrances)
				{
					foreach (NewNode.NodeSpace nodeSpace2 in nodeAccess2.toNode.occupiedSpace)
					{
						Human human2 = nodeSpace2.occupier as Human;
						if (human2 != null && GameplayController.Instance.enforcerCalls[this.kidnapper.home].response.Contains(human2.humanID))
						{
							this.TriggerEscapeEvents();
							break;
						}
						if (this.enforcerEventsTrigger)
						{
							break;
						}
					}
					if (this.enforcerEventsTrigger)
					{
						break;
					}
				}
			}
		}
		if (ChapterController.Instance.currentPartName == "CrimeSceneClues")
		{
			if (this.restaurantReceipt != null && this.receiptSearchPromt && !this.receiptSearchActivated)
			{
				this.receiptSearchTimer += num;
				if (this.receiptSearchTimer > 0.37f)
				{
					this.receiptSearchActivated = true;
					Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, this.restaurantReceipt, null, null, null, null, "", true, default(Vector3));
					if (this.receiptInBin)
					{
						this.AddObjective("Sometimes it's useful to search the trash!", trigger, true, this.kidnapperBin.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
					}
					else
					{
						this.AddObjective("Thought I saw something over here...", trigger, true, this.restaurantReceipt.interactable.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
					}
				}
			}
			if (this.kidnappersAddressBook != null && this.addressBookSearchPrompt && !this.addressBookSearchActivated)
			{
				this.addressBookSearchTimer += num;
				if (this.addressBookSearchTimer > 0.28f)
				{
					this.addressBookSearchActivated = true;
					Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.inspectInteractable, "", false, 0f, null, this.kidnappersAddressBook, null, null, null, null, null, "", true, default(Vector3));
					this.AddObjective("It's worth checking out the address book...", trigger2, true, this.kidnappersAddressBook.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
				}
			}
			if (this.fingerprintPrompt && !this.printSearchActivated)
			{
				this.printSearchTimer += num;
				if (this.printSearchTimer > 0.5f)
				{
					this.printSearchActivated = true;
					Objective.ObjectiveTrigger trigger3 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.findFingerprints, "", false, 0f, null, null, this.killer.evidenceEntry, null, null, null, null, "", false, default(Vector3));
					List<Interactable> list2 = new List<Interactable>();
					foreach (NewNode newNode2 in this.kidnapper.home.nodes)
					{
						foreach (Interactable interactable2 in newNode2.interactables)
						{
							if (interactable2.df != null && interactable2.df.Exists((Interactable.DynamicFingerprint item) => item.id == this.killer.humanID))
							{
								list2.Add(interactable2);
							}
						}
					}
					SessionData.Instance.TutorialTrigger("fingerprints", false);
					for (int i = 0; i < Mathf.Min(4, list2.Count); i++)
					{
						this.AddObjective("Scan for prints " + i.ToString(), trigger3, true, list2[i].GetWorldPosition(true), InterfaceControls.Icon.printScanner, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
					}
					ControlsDisplayController.Instance.DisplayControlIconAfterDelay(1f, InteractablePreset.InteractionKey.WeaponSelect, "Inventory", InterfaceControls.Instance.controlIconDisplayTime + 2f, false);
					this.PlayerVO("7c0dcfe5-f974-4ab8-bb0e-e3a5b526bad8", 0f, true, false, false, false, default(Color));
				}
			}
		}
		if (ChapterController.Instance.currentPartName == "ArrivalDiner" && !this.triggeredPasscodeNoteHint)
		{
			if (this.passcodeNoteTimer < 0.4f)
			{
				this.passcodeNoteTimer += num;
			}
			else if (this.restaurantBackroom.passcode != null)
			{
				foreach (int num3 in this.restaurantBackroom.passcode.notes)
				{
					Interactable interactable3 = null;
					if (CityData.Instance.savableInteractableDictionary.TryGetValue(num3, ref interactable3) && interactable3.node.gameLocation == this.restaurant && interactable3.node.room != this.restaurantBackroom)
					{
						Objective.ObjectiveTrigger trigger4 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.inspectInteractable, "", false, 0f, null, interactable3, null, null, null, null, null, "", false, default(Vector3));
						this.AddObjective("You may be able to find the passcode on a note somewhere...", trigger4, true, interactable3.GetWorldPosition(true), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
						this.triggeredPasscodeNoteHint = true;
					}
				}
			}
		}
		if ((ChapterController.Instance.currentPart >= 31 || this.notewriterManualMurderTrigger) && !this.notewriterMurderTriggered && !this.killer.ai.isConvicted)
		{
			this.notewriterMurderTimer += num;
			if (this.notewriterMurderTimer >= 2.5f || this.notewriterManualMurderTrigger)
			{
				if (Game.Instance.collectDebugData)
				{
					Game.Log("Chapter: Notewriter murder triggered....", 2);
				}
				this.murder2 = MurderController.Instance.ExecuteNewMurder(this.killer, this.noteWriter, this.preset.crimePool[Toolbox.Instance.Rand(0, this.preset.crimePool.Count, false)], this.preset.MOPool[Toolbox.Instance.Rand(0, this.preset.MOPool.Count, false)], null);
				this.notewriterMurderTriggered = true;
			}
		}
		if (ChapterController.Instance.currentPartName == "DisplayLeads" && !PopupMessageController.Instance.active && Player.Instance.speechController.speechQueue.Count <= 0 && this.thisCase != null && this.thisCase.caseStatus == Case.CaseStatus.handInCollected)
		{
			if (this.nextLeadDelay <= 0f)
			{
				this.ExecuteChangeLeadsManual();
				this.nextLeadDelay = 0.005f;
			}
			else
			{
				this.nextLeadDelay -= num;
			}
		}
		if (this.killer.ai.isConvicted && this.layLowGoal != null)
		{
			this.layLowGoal.Remove();
		}
		if (this.killer.ai.isConvicted && ChapterController.Instance.currentPart < 58)
		{
			if (this.endDelayTimer < 0.01f)
			{
				this.endDelayTimer += num;
				return;
			}
			ChapterController.Instance.LoadPart("ReturnHome");
		}
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x000C01E0 File Offset: 0x000BE3E0
	public void OnReturnToApartmentOption()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnReturnToApartmentOption;
		PopupMessageController.Instance.OnRightButton -= this.OnSkipAheadOption;
		PopupMessageController.Instance.OnOptionButton -= this.OnCancelOption;
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Teleporting back to apartment...", 2);
		}
		Player.Instance.Teleport(Player.Instance.FindSafeTeleport(this.playerBedroom, false), null, true, false);
		this.triggeredTutorialSkip = 0f;
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x000C0270 File Offset: 0x000BE470
	public void OnSkipAheadOption()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnReturnToApartmentOption;
		PopupMessageController.Instance.OnRightButton -= this.OnSkipAheadOption;
		PopupMessageController.Instance.OnOptionButton -= this.OnCancelOption;
		base.ClearAllObjectives();
		this.triggeredTutorialSkip = 0f;
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Skipping ahead...", 2);
		}
		ChapterController.Instance.LoadPart(21, false, true);
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x000C02F8 File Offset: 0x000BE4F8
	public void OnCancelOption()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnReturnToApartmentOption;
		PopupMessageController.Instance.OnRightButton -= this.OnSkipAheadOption;
		PopupMessageController.Instance.OnOptionButton -= this.OnCancelOption;
		Player.Instance.ClearAllDisabledActions();
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Cancel...", 2);
		}
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x000C0368 File Offset: 0x000BE568
	private void ChooseInvestigatePhone()
	{
		PopupMessageController.Instance.OnLeftButton -= this.ChooseInvestigateCCTV;
		PopupMessageController.Instance.OnRightButton -= this.ChooseInvestigatePhone;
		PopupMessageController.Instance.OnLeftButton2 -= this.ChooseInvestigateVmails;
		PopupMessageController.Instance.OnRightButton2 -= this.ChooseInvestigateMurderWeapon;
		PopupMessageController.Instance.OnOptionButton -= this.ChooseCancelLeads;
		bool clearPhone = true;
		if (ChapterController.Instance.currentPartName == "AccessCabinet" || ChapterController.Instance.currentPartName == "TraceCall" || ChapterController.Instance.currentPartName == "SearchCallSource" || ChapterController.Instance.currentPartName == "AccessOtherAddress" || ChapterController.Instance.currentPartName == "InvesitgatePhone")
		{
			clearPhone = false;
		}
		this.ClearLeads(true, true, clearPhone, true);
		ChapterController.Instance.LoadPart("InvesitgatePhone");
		base.Invoke("ChangeLeadTip", 4f);
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x000C0480 File Offset: 0x000BE680
	private void ChooseInvestigateCCTV()
	{
		PopupMessageController.Instance.OnLeftButton -= this.ChooseInvestigateCCTV;
		PopupMessageController.Instance.OnRightButton -= this.ChooseInvestigatePhone;
		PopupMessageController.Instance.OnLeftButton2 -= this.ChooseInvestigateVmails;
		PopupMessageController.Instance.OnRightButton2 -= this.ChooseInvestigateMurderWeapon;
		PopupMessageController.Instance.OnOptionButton -= this.ChooseCancelLeads;
		bool clearDiner = true;
		if (ChapterController.Instance.currentPartName == "InvestigateCCTV" || ChapterController.Instance.currentPartName == "ArrivalDiner" || ChapterController.Instance.currentPartName == "AccessBackroom" || ChapterController.Instance.currentPartName == "LaunchSurveillance" || ChapterController.Instance.currentPartName == "FoundRecords" || ChapterController.Instance.currentPartName == "KidnapperOnCam" || ChapterController.Instance.currentPartName == "OpenNotewirterEvidence" || ChapterController.Instance.currentPartName == "FindFlyer")
		{
			clearDiner = false;
		}
		this.ClearLeads(clearDiner, true, true, true);
		ChapterController.Instance.LoadPart("InvestigateCCTV");
		base.Invoke("ChangeLeadTip", 4f);
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x000C05E0 File Offset: 0x000BE7E0
	private void ChooseInvestigateVmails()
	{
		PopupMessageController.Instance.OnLeftButton -= this.ChooseInvestigateCCTV;
		PopupMessageController.Instance.OnRightButton -= this.ChooseInvestigatePhone;
		PopupMessageController.Instance.OnLeftButton2 -= this.ChooseInvestigateVmails;
		PopupMessageController.Instance.OnRightButton2 -= this.ChooseInvestigateMurderWeapon;
		PopupMessageController.Instance.OnOptionButton -= this.ChooseCancelLeads;
		bool clearOffice = true;
		if (ChapterController.Instance.currentPartName == "InvestigateVmails" || ChapterController.Instance.currentPartName == "ArrivalWorkplace" || ChapterController.Instance.currentPartName == "LaunchVmail" || ChapterController.Instance.currentPartName == "AccessKidnapperCruncher" || ChapterController.Instance.currentPartName == "FoundNotewriterID" || ChapterController.Instance.currentPartName == "AccessNotewriterCruncher")
		{
			clearOffice = false;
		}
		this.ClearLeads(true, clearOffice, true, true);
		ChapterController.Instance.LoadPart("InvestigateVmails");
		base.Invoke("ChangeLeadTip", 4f);
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x000C070C File Offset: 0x000BE90C
	private void ChooseInvestigateMurderWeapon()
	{
		PopupMessageController.Instance.OnLeftButton -= this.ChooseInvestigateCCTV;
		PopupMessageController.Instance.OnRightButton -= this.ChooseInvestigatePhone;
		PopupMessageController.Instance.OnLeftButton2 -= this.ChooseInvestigateVmails;
		PopupMessageController.Instance.OnRightButton2 -= this.ChooseInvestigateMurderWeapon;
		PopupMessageController.Instance.OnOptionButton -= this.ChooseCancelLeads;
		bool clearWeaponsDealer = true;
		if (ChapterController.Instance.currentPartName == "InvestigateMurderWeapon" || ChapterController.Instance.currentPartName == "SearchWeaponsDealer" || ChapterController.Instance.currentPartName == "SearchOtherAddress")
		{
			clearWeaponsDealer = false;
		}
		this.ClearLeads(true, true, true, clearWeaponsDealer);
		ChapterController.Instance.LoadPart("InvestigateMurderWeapon");
		base.Invoke("ChangeLeadTip", 4f);
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x000C07F8 File Offset: 0x000BE9F8
	private void ChooseCancelLeads()
	{
		PopupMessageController.Instance.OnLeftButton -= this.ChooseInvestigateCCTV;
		PopupMessageController.Instance.OnRightButton -= this.ChooseInvestigatePhone;
		PopupMessageController.Instance.OnLeftButton2 -= this.ChooseInvestigateVmails;
		PopupMessageController.Instance.OnRightButton2 -= this.ChooseInvestigateMurderWeapon;
		PopupMessageController.Instance.OnOptionButton -= this.ChooseCancelLeads;
		ChapterController.Instance.LoadPart("CancelLeads");
		base.Invoke("ChangeLeadTip", 4f);
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x000C0894 File Offset: 0x000BEA94
	public void ChangeLeadTip()
	{
		InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "tip-changelead", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.questionMark, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x000C08DC File Offset: 0x000BEADC
	private void OnDestroy()
	{
		PopupMessageController.Instance.OnLeftButton -= this.ChooseInvestigateCCTV;
		PopupMessageController.Instance.OnRightButton -= this.ChooseInvestigatePhone;
		PopupMessageController.Instance.OnLeftButton2 -= this.ChooseInvestigateVmails;
		PopupMessageController.Instance.OnRightButton2 -= this.ChooseInvestigateMurderWeapon;
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x000C0944 File Offset: 0x000BEB44
	public void OnMurderStateChange(MurderController.MurderState newState)
	{
		if (newState == MurderController.MurderState.executing)
		{
			if (Game.Instance.collectDebugData)
			{
				Game.Log("Chapter: Murder is complete...", 2);
			}
			this.murderPreSimPass = true;
			this.noteWriter.ai.disableTickRateUpdate = false;
			this.kidnapper.ai.disableTickRateUpdate = false;
			this.killer.ai.disableTickRateUpdate = false;
			this.murder.OnStateChanged -= this.OnMurderStateChange;
		}
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x000C09C0 File Offset: 0x000BEBC0
	private void PickCharacters()
	{
		List<ChapterIntro.IntoCharacterPick> list = new List<ChapterIntro.IntoCharacterPick>();
		this.killer = CityData.Instance.criminalJobDirectory.Find((Occupation item) => item.preset.name == "HiredKiller").employee;
		this.killerID = this.killer.humanID;
		foreach (GroupsController.SocialGroup socialGroup in GroupsController.Instance.groups)
		{
			if (socialGroup.preset == "PokerClub")
			{
				if (!socialGroup.members.Contains(this.killer.humanID))
				{
					socialGroup.members.Add(this.killer.humanID);
					this.killer.groups.Add(socialGroup);
				}
				this.killerBar = socialGroup.GetMeetingPlace();
				this.killerBarID = this.killerBar.id;
			}
			else if (socialGroup.preset == "RedGumsMeetup")
			{
				if (!socialGroup.members.Contains(this.killer.humanID))
				{
					socialGroup.members.Add(this.killer.humanID);
					this.killer.groups.Add(socialGroup);
				}
				this.redGumMeet = socialGroup.GetMeetingPlace();
				if (this.redGumMeet != null)
				{
					this.redGumMeetID = this.redGumMeet.id;
				}
				if (this.killerBar == null)
				{
					this.killerBar = socialGroup.GetMeetingPlace();
					this.killerBarID = this.killerBar.id;
				}
			}
			else if (socialGroup.preset == "GunClub")
			{
				if (!socialGroup.members.Contains(this.killer.humanID))
				{
					socialGroup.members.Add(this.killer.humanID);
					this.killer.groups.Add(socialGroup);
				}
				if (this.killerBar == null)
				{
					this.killerBar = socialGroup.GetMeetingPlace();
					this.killerBarID = this.killerBar.id;
				}
			}
		}
		if (this.killerBar == null)
		{
			Company company = CityData.Instance.companyDirectory.Find((Company item) => item.preset.name == "Bar");
			if (company != null)
			{
				this.killerBar = company.placeOfBusiness.thisAsAddress;
				this.killerBarID = this.killerBar.id;
			}
		}
		if (this.weaponSeller == null)
		{
			Company company2 = CityData.Instance.companyDirectory.Find((Company item) => item.preset.name == "WeaponsDealer");
			if (company2 != null)
			{
				this.weaponSeller = company2.placeOfBusiness.thisAsAddress;
				this.weaponSellerID = this.weaponSeller.id;
			}
		}
		if (this.murderWeapon == null)
		{
			this.murderWeapon = this.killer.inventory.Find((Interactable item) => item.preset == InteriorControls.Instance.handgun);
			if (this.murderWeapon == null)
			{
				foreach (NewRoom newRoom in this.killer.home.rooms)
				{
					foreach (NewNode newNode in newRoom.nodes)
					{
						foreach (Interactable interactable in newNode.interactables)
						{
							if (interactable.preset == InteriorControls.Instance.handgun)
							{
								if (Game.Instance.collectDebugData)
								{
									Game.Log("Chapter: Murder weapon found in apartment of killer...", 2);
								}
								this.murderWeapon = interactable;
								break;
							}
						}
						if (this.murderWeapon != null)
						{
							break;
						}
					}
					if (this.murderWeapon != null)
					{
						break;
					}
				}
				if (this.murderWeapon == null)
				{
					Game.LogError("Unable to locate murder weapon!", 2);
				}
			}
			else if (Game.Instance.collectDebugData)
			{
				Game.Log("Chapter: Murder weapon found in inventory of killer...", 2);
			}
		}
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Lopping killer social groups...", 2);
			foreach (GroupsController.SocialGroup socialGroup2 in this.killer.groups)
			{
				Game.Log("Chapter: ..." + socialGroup2.preset, 2);
			}
		}
		foreach (Citizen citizen in CityData.Instance.citizenDirectory)
		{
			if (citizen.job != null && citizen.job.employer != null && !(citizen.job.employer.preset.name != "MediumOffice") && !(citizen.home == null) && (!citizen.job.employer.preset.publicFacing || !citizen.job.preset.ownsWorkPosition || citizen.job.preset.jobPostion != InteractablePreset.SpecialCase.workDesk) && !SideJobController.Instance.exemptFromPosters.Contains(citizen) && !SideJobController.Instance.exemptFromPurps.Contains(citizen))
			{
				new List<Citizen>();
				List<Acquaintance> list2 = citizen.acquaintances.FindAll((Acquaintance item) => item.connections.Contains(Acquaintance.ConnectionType.boss) || item.connections.Contains(Acquaintance.ConnectionType.workOther) || item.connections.Contains(Acquaintance.ConnectionType.workTeam) || item.connections.Contains(Acquaintance.ConnectionType.familiarWork));
				if (list2.Count > 0)
				{
					float num = -99999f;
					Acquaintance acquaintance = null;
					foreach (Acquaintance acquaintance2 in list2)
					{
						if (!SideJobController.Instance.exemptFromPosters.Contains(acquaintance2.with) && !SideJobController.Instance.exemptFromPurps.Contains(acquaintance2.with) && !(acquaintance2.with.job.preset == null) && acquaintance2.with.job.preset.ownsWorkPosition && acquaintance2.with.job.preset.jobPostion == InteractablePreset.SpecialCase.workDesk && !(acquaintance2.with.home == null))
						{
							float num2 = acquaintance2.known + acquaintance2.like - acquaintance2.with.humility - acquaintance2.with.emotionality;
							if (num2 > num)
							{
								num = num2;
								acquaintance = acquaintance2;
							}
						}
					}
					if (acquaintance != null)
					{
						acquaintance.known = Mathf.Max(acquaintance.known, SocialControls.Instance.telephoneBookInclusionThreshold);
						acquaintance.like = Mathf.Max(acquaintance.known, 0.5f);
						ChapterIntro.IntoCharacterPick intoCharacterPick = new ChapterIntro.IntoCharacterPick();
						float num3 = citizen.humility + citizen.emotionality;
						float num4 = acquaintance.with.humility + acquaintance.with.emotionality;
						if (num3 <= num4)
						{
							intoCharacterPick.kidnapper = citizen;
							intoCharacterPick.noteWriter = acquaintance.with;
						}
						else
						{
							intoCharacterPick.noteWriter = citizen;
							intoCharacterPick.kidnapper = acquaintance.with;
						}
						intoCharacterPick.score = 1f - intoCharacterPick.kidnapper.humility + (1f - intoCharacterPick.kidnapper.emotionality) + intoCharacterPick.noteWriter.humility;
						if (intoCharacterPick.kidnapper.partner == null)
						{
							intoCharacterPick.score += 0.5f;
						}
						if (intoCharacterPick.kidnapper.home.building != this.apartment.building)
						{
							intoCharacterPick.score += 3f;
						}
						if (intoCharacterPick.kidnapper.home.building != intoCharacterPick.noteWriter.home.building)
						{
							intoCharacterPick.score += 1f;
						}
						if (intoCharacterPick.kidnapper.home.rooms.Count >= 4 && intoCharacterPick.kidnapper.home.rooms.Count <= 6)
						{
							intoCharacterPick.score += 2f;
						}
						if (intoCharacterPick.kidnapper.home.rooms.Exists((NewRoom item) => item.individualFurniture.Exists((FurnitureLocation item) => item.furnitureClasses[0].name == "1x1Safe")))
						{
							intoCharacterPick.score += 2f;
						}
						if (intoCharacterPick.kidnapper.partner == null)
						{
							intoCharacterPick.score += 2f;
						}
						if (intoCharacterPick.noteWriter.partner == null)
						{
							intoCharacterPick.score += 0.5f;
						}
						list.Add(intoCharacterPick);
					}
				}
			}
		}
		list.Sort((ChapterIntro.IntoCharacterPick p1, ChapterIntro.IntoCharacterPick p2) => p2.score.CompareTo(p1.score));
		this.noteWriter = list[0].noteWriter;
		this.noteWriterID = this.noteWriter.humanID;
		this.kidnapper = list[0].kidnapper;
		this.kidnapperID = this.kidnapper.humanID;
		Acquaintance acquaintance3;
		if (!this.kidnapper.FindAcquaintanceExists(this.killer, out acquaintance3))
		{
			this.kidnapper.AddAcquaintance(this.killer, 0.6f, Acquaintance.ConnectionType.friend, true, false, Acquaintance.ConnectionType.friend, null);
		}
		List<Citizen> list3 = CityData.Instance.citizenDirectory.FindAll((Citizen item) => item.home != null && item != this.noteWriter && item != this.kidnapper && item != this.killer && !item.isPlayer && item.home != this.kidnapper.home && item.home != this.killer.home && item.societalClass > 0.1f && item.societalClass < 0.8f);
		this.slophouseOwner = list3[Toolbox.Instance.GetPsuedoRandomNumber(0, list3.Count, this.noteWriter.citizenName + this.kidnapper.citizenName, false)];
		this.slopHouseOwnerID = this.slophouseOwner.humanID;
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Kidnapper is " + this.kidnapper.GetCitizenName(), 2);
			Game.Log("Chapter: Note Writer is " + this.noteWriter.GetCitizenName(), 2);
			Game.Log("Chapter: Killer is " + this.killer.GetCitizenName(), 2);
			Game.Log("Chapter: Slophouse owner is " + this.slophouseOwner.GetCitizenName(), 2);
		}
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x000C154C File Offset: 0x000BF74C
	public void TurnOnLight(int passedVar)
	{
		Player.Instance.energy = 0.7f;
		Player.Instance.hydration = 0.7f;
		Player.Instance.nourishment = 0.7f;
		InterfaceController.Instance.fade = 1f;
		InterfaceController.Instance.Fade(0f, 12f, true);
		AudioController.Instance.PassWeatherParams();
		FirstPersonItemController.Instance.RemoveSpecificStaticSlot(FirstPersonItemController.InventorySlot.StaticSlot.printReader);
		Player.Instance.SetActionDisable("Get In", true);
		Player.Instance.SetActionDisable("Get Up", true);
		Player.Instance.SetActionDisable("Sleep...", true);
		Player.Instance.SetActionDisable("Call", true);
		SessionData.Instance.tutorialTextTriggered.Add("citydirectory");
		SessionData.Instance.tutorialTextTriggered.Add("evidence");
		List<Interactable> list = new List<Interactable>();
		foreach (Interactable interactable in this.bed.integratedInteractables)
		{
			if (interactable.pt == 1 || interactable.pt == 2)
			{
				list.Add(interactable);
			}
		}
		this.closestSleep = list[0];
		float num = 9999f;
		foreach (Interactable interactable2 in list)
		{
			float num2 = Vector3.Distance(interactable2.node.position, Player.Instance.transform.position);
			if (num2 < num)
			{
				num = num2;
				this.closestSleep = interactable2;
			}
		}
		this.SetCurrentPartLocation(this.closestSleep.node);
		InteractablePreset.InteractionAction action = this.closestSleep.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.action == RoutineControls.Instance.hide);
		InteractionController.Instance.currentLookingAtInteractable = this.closestSleep.controller;
		this.closestSleep.OnInteraction(action, Player.Instance, true, 0f);
		InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, Strings.Get("ui.chapters", "Chapter I", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.agent, null, false, default(Color), -1, 3f, null, GameMessageController.PingOnComplete.none, null, null, null);
		InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, string.Concat(new string[]
		{
			SessionData.Instance.CurrentTimeString(false, true),
			", ",
			SessionData.Instance.LongDateString(SessionData.Instance.gameTime, true, true, true, true, true, false, false, true),
			", ",
			Strings.Get("ui.interface", "Your Apartment", Strings.Casing.asIs, false, false, false, null)
		}), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		this.PlayerVO("9992d16f-2d69-4821-bd31-2e8333f0d043", 16f, true, false, false, false, default(Color));
		this.PlayerVO("2bd1b9db-eefb-4cc5-ab0a-7e629c99b33d", 1f, true, false, false, false, default(Color));
		Interactable interactable3 = null;
		List<Interactable> list2 = new List<Interactable>();
		float num3 = 999999f;
		foreach (NewNode newNode in this.bed.anchorNode.room.nodes)
		{
			foreach (Interactable interactable4 in newNode.interactables)
			{
				if (interactable4.preset == InteriorControls.Instance.bedsideLamp)
				{
					float num4 = Vector3.Distance(interactable4.GetWorldPosition(true), CameraController.Instance.cam.transform.position);
					if (num4 < num3 && interactable3 != interactable4)
					{
						if (interactable3 != null && !list2.Contains(interactable3))
						{
							list2.Add(interactable3);
						}
						interactable3 = interactable4;
						num3 = num4;
					}
				}
			}
		}
		if (interactable3 != null)
		{
			Vector3 pointerPosition = interactable3.GetWorldPosition(true) + new Vector3(0f, 0.25f, 0f);
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.switchStateTrueForType, "", false, 0f, null, interactable3, null, null, null, this.apartment, null, "Turn On", false, default(Vector3));
			this.AddObjective("Turn on the bedside light", trigger, true, pointerPosition, InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
			for (int i = 0; i < list2.Count; i++)
			{
				Interactable newInteractable = list2[i];
				Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.switchStateTrueForType, "", false, 0f, null, newInteractable, null, null, null, this.apartment, null, "", false, default(Vector3));
				this.AddObjective("Turn on the bedside light " + i.ToString(), trigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "FindPartner", true, false);
			}
			return;
		}
		ChapterController.Instance.SkipToNextPart();
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x000C1A80 File Offset: 0x000BFC80
	public void FindPartner(int passedVar)
	{
		base.ClearAllObjectives();
		Player.Instance.SetActionDisable("Get Up", false);
		if (this.closestSleep != null)
		{
			this.closestSleep.SetActionHighlight("Get Up", true);
		}
		List<Objective.ObjectiveTrigger> list = new List<Objective.ObjectiveTrigger>();
		this.PlayerVO("42c76822-6efe-4510-a70d-28b9f07c6fa8", 0f, true, false, false, false, default(Color));
		this.PlayerVO("e4b4c96c-e858-4197-a31f-afedf5b3ca87", 0f, true, false, false, false, default(Color));
		this.SetCurrentPartLocation(this.playerBedroom.GetRandomEntranceNode());
		if (this.playerLounge != null)
		{
			this.SetCurrentPartLocation(this.playerLounge.GetRandomEntranceNode());
			NewWall newWall = null;
			float num = 9999f;
			foreach (NewWall newWall2 in this.playerLounge.lightswitches)
			{
				float num2 = Vector3.Distance(this.playerLounge.GetRandomEntranceNode().position, newWall2.position);
				if (num2 < num)
				{
					num = num2;
					newWall = newWall2;
				}
			}
			if (newWall != null)
			{
				Objective.ObjectiveTrigger objectiveTrigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.roomLightOn, "", false, 0f, this.playerLounge, newWall.lightswitchInteractable, null, null, null, null, null, "Turn On", false, default(Vector3));
				list.Add(objectiveTrigger);
				this.AddObjective("Turn on the light switch in the lounge", objectiveTrigger, true, newWall.lightswitchInteractable.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 1f, false, "", false, false);
			}
		}
		if (this.playerKitchen != null)
		{
			this.SetCurrentPartLocation(this.playerKitchen.GetRandomEntranceNode());
			NewWall newWall3 = null;
			float num3 = 9999f;
			foreach (NewWall newWall4 in this.playerKitchen.lightswitches)
			{
				float num4 = Vector3.Distance(this.playerKitchen.GetRandomEntranceNode().position, newWall4.position);
				if (num4 < num3)
				{
					num3 = num4;
					newWall3 = newWall4;
				}
			}
			if (newWall3 != null)
			{
				Objective.ObjectiveTrigger objectiveTrigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.roomLightOn, "", false, 0f, this.playerKitchen, newWall3.lightswitchInteractable, null, null, null, null, null, "Turn On", false, default(Vector3));
				list.Add(objectiveTrigger2);
				this.AddObjective("Turn on the light switch in the kitchen", objectiveTrigger2, true, newWall3.lightswitchInteractable.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 1f, false, "", false, false);
			}
		}
		this.AddObjective("Turned on lights", list, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "TelephoneRing", true, false, true);
		this.PlayerVO("3d5bc60d-a4de-42a6-bf8f-b71c15b9aa5c", 4f, true, false, false, false, default(Color));
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x000C1D5C File Offset: 0x000BFF5C
	public void TelephoneRing(int passedVar)
	{
		if (this.closestSleep != null)
		{
			this.closestSleep.SetActionHighlight("Get Up", false);
		}
		ControlsDisplayController.Instance.disableControlDisplay.Remove(InteractablePreset.InteractionKey.flashlight);
		if (this.playerLounge != null)
		{
			this.SetCurrentPartLocation(this.playerLounge.GetRandomEntranceNode());
		}
		else if (this.playerKitchen != null)
		{
			this.SetCurrentPartLocation(this.playerKitchen.GetRandomEntranceNode());
		}
		else
		{
			this.SetCurrentPartLocation(this.playerBedroom.GetRandomEntranceNode());
		}
		float delayRealSeconds = 0f;
		if (!this.loadedFromSave)
		{
			delayRealSeconds = Toolbox.Instance.Rand(1.5f, 2.5f, false);
		}
		base.InvokeAfterDelay("RingPhone", delayRealSeconds);
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x000C1E18 File Offset: 0x000C0018
	private void RingPhone()
	{
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Ring player's apartment...", 2);
		}
		List<Telephone> list = new List<Telephone>(CityData.Instance.phoneDictionary.Values);
		List<Telephone> list2 = list.FindAll((Telephone item) => item.interactable != null && item.interactable.preset.isPayphone && item.interactable.node.gameLocation.thisAsStreet != null);
		Telephone from = list[Toolbox.Instance.Rand(0, list.Count, false)];
		if (list2.Count > 0)
		{
			from = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
		}
		else
		{
			list2 = list.FindAll((Telephone item) => item.interactable != null && item.interactable.preset.isPayphone);
			if (list2.Count > 0)
			{
				from = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
			}
		}
		TelephoneController.Instance.CreateNewCall(from, this.apartment.telephones[0], null, Player.Instance, new TelephoneController.CallSource(TelephoneController.CallType.audioEvent, this.preset.audioEvents[0]), 999999f, false);
		Objective.ObjectiveTriggerType newType = Objective.ObjectiveTriggerType.answerPhone;
		string newName = "";
		bool newForceProgressAmount = false;
		float newProgressAdd = 0f;
		NewRoom newRoom = null;
		NewGameLocation newGameLocation = this.apartment;
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(newType, newName, newForceProgressAmount, newProgressAdd, newRoom, Enumerable.FirstOrDefault<Telephone>(this.apartment.telephones).interactable, null, null, null, newGameLocation, null, "Pick Up", false, default(Vector3));
		this.AddObjective("Answer the telephone", trigger, true, Enumerable.FirstOrDefault<Telephone>(this.apartment.telephones).interactable.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextChapterPart, 1.5f, false, "", false, false);
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x000C1FBC File Offset: 0x000C01BC
	public void AnswerCall(int passedVar)
	{
		if (this.note == null)
		{
			Vector3 worldPos = Vector3.Lerp(this.interiorDoorNode.position, this.apartmentEntrance.worldAccessPoint, 0.6f);
			this.note = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.note, this.noteWriter, this.noteWriter, Player.Instance, worldPos, new Vector3(0f, Toolbox.Instance.Rand(0f, 360f, false), 0f), null, null, "bc6fabb0-8ae4-44a1-b1e0-c8305a575b89");
			this.note.AddNewDynamicFingerprint(this.noteWriter, Interactable.PrintLife.manualRemoval);
			this.note.AddNewDynamicFingerprint(this.noteWriter, Interactable.PrintLife.manualRemoval);
			this.note.AddNewDynamicFingerprint(this.noteWriter, Interactable.PrintLife.manualRemoval);
		}
		this.PlayerVO("71c93b28-2bce-4915-afff-0635df4811c9", 0f, true, false, true, false, default(Color));
		this.PlayerVO("5f4b466c-b4e0-40aa-961c-fb168a0606be", 0f, true, false, true, false, default(Color));
		if (this.playerLounge != null)
		{
			this.SetCurrentPartLocation(this.playerLounge.GetRandomEntranceNode());
		}
		else if (this.playerKitchen != null)
		{
			this.SetCurrentPartLocation(this.playerKitchen.GetRandomEntranceNode());
		}
		else
		{
			this.SetCurrentPartLocation(this.playerBedroom.GetRandomEntranceNode());
		}
		Objective.ObjectiveTriggerType newType = Objective.ObjectiveTriggerType.answerPhoneAndEndCall;
		string newName = "";
		bool newForceProgressAmount = false;
		float newProgressAdd = 0f;
		NewRoom newRoom = null;
		NewGameLocation home = Player.Instance.home;
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(newType, newName, newForceProgressAmount, newProgressAdd, newRoom, Enumerable.FirstOrDefault<Telephone>(Player.Instance.home.telephones).interactable, null, null, null, home, null, "", false, default(Vector3));
		this.AddObjective("End Telephone Call", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", true, false);
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x000C2184 File Offset: 0x000C0384
	public void SomethingWrong(int passedVar)
	{
		if (this.playerLounge != null)
		{
			this.SetCurrentPartLocation(this.playerLounge.GetRandomEntranceNode());
		}
		else if (this.playerKitchen != null)
		{
			this.SetCurrentPartLocation(this.playerKitchen.GetRandomEntranceNode());
		}
		else
		{
			this.SetCurrentPartLocation(this.playerBedroom.GetRandomEntranceNode());
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.inspectInteractable, "", false, 0f, null, this.note, null, null, null, null, null, "Inspect", false, default(Vector3));
		this.AddObjective("Search the apartment for clues", trigger, false, this.note.GetWorldPosition(true), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "OpenName", false, false);
		this.AddObjective("Check the front door", trigger, true, this.note.GetWorldPosition(true), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 20f, true, "OpenName", false, true);
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x000C2264 File Offset: 0x000C0464
	public void OpenName(int passedVar)
	{
		base.ClearAllObjectives();
		if (this.playerLounge != null)
		{
			this.SetCurrentPartLocation(this.playerLounge.GetRandomEntranceNode());
		}
		else if (this.playerKitchen != null)
		{
			this.SetCurrentPartLocation(this.playerKitchen.GetRandomEntranceNode());
		}
		else
		{
			this.SetCurrentPartLocation(this.playerBedroom.GetRandomEntranceNode());
		}
		List<string> list = new List<string>();
		list.Add("35aac0af-338b-4c63-8843-1f987a68e336");
		PopupMessageController.Instance.TutorialMessage("caseboard", PopupMessageController.AffectPauseState.automatic, false, list);
		InfoWindow infoWindow = InterfaceController.Instance.activeWindows.Find((InfoWindow item) => item.passedEvidence == this.note.evidence);
		if (infoWindow != null)
		{
			infoWindow.forceDisableClose = true;
			infoWindow.forceDisablePin = true;
			LinkButtonController componentInChildren = infoWindow.GetComponentInChildren<LinkButtonController>();
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.tutorialPointer, componentInChildren.rect);
			this.pointer = gameObject.GetComponent<RectTransform>();
			this.pointer.sizeDelta = componentInChildren.rect.sizeDelta;
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.evidenceOpenAndDisplayed, "", false, 0f, null, null, this.kidnapper.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Select the name '|kidnappername|' on the note", trigger, false, default(Vector3), InterfaceControls.Icon.pin, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x000C23BC File Offset: 0x000C05BC
	public void PinCitizen(int passedVar)
	{
		if (this.pointer != null)
		{
			Object.Destroy(this.pointer.gameObject);
		}
		base.ClearAllObjectives();
		if (this.playerLounge != null)
		{
			this.SetCurrentPartLocation(this.playerLounge.GetRandomEntranceNode());
		}
		else if (this.playerKitchen != null)
		{
			this.SetCurrentPartLocation(this.playerKitchen.GetRandomEntranceNode());
		}
		else
		{
			this.SetCurrentPartLocation(this.playerBedroom.GetRandomEntranceNode());
		}
		InfoWindow infoWindow = InterfaceController.Instance.activeWindows.Find((InfoWindow item) => item.passedEvidence == this.note.evidence);
		if (infoWindow != null)
		{
			infoWindow.forceDisableClose = true;
			infoWindow.forceDisablePin = true;
			LinkButtonController componentInChildren = infoWindow.GetComponentInChildren<LinkButtonController>();
			if (componentInChildren != null)
			{
				JuiceController componentInChildren2 = componentInChildren.GetComponentInChildren<JuiceController>();
				if (componentInChildren2 != null)
				{
					Object.Destroy(componentInChildren2.gameObject);
				}
			}
		}
		InfoWindow infoWindow2 = InterfaceController.Instance.activeWindows.Find((InfoWindow item) => item.passedEvidence == this.kidnapper.evidenceEntry);
		if (infoWindow2 != null)
		{
			infoWindow2.forceDisableClose = true;
			PinFolderButtonController pinButton = infoWindow2.pinButton;
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.tutorialPointer, pinButton.rect);
			this.pointer = gameObject.GetComponent<RectTransform>();
			this.pointer.sizeDelta = pinButton.rect.sizeDelta;
		}
		if (infoWindow == null)
		{
			infoWindow2;
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.evidencePinned, "", false, 0f, null, null, this.kidnapper.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Pin |kidnappername| to your case board", trigger, false, default(Vector3), InterfaceControls.Icon.pin, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x000C257C File Offset: 0x000C077C
	public void PinNote(int passedVar)
	{
		if (this.pointer != null)
		{
			Object.Destroy(this.pointer.gameObject);
		}
		base.ClearAllObjectives();
		if (this.playerLounge != null)
		{
			this.SetCurrentPartLocation(this.playerLounge.GetRandomEntranceNode());
		}
		else if (this.playerKitchen != null)
		{
			this.SetCurrentPartLocation(this.playerKitchen.GetRandomEntranceNode());
		}
		else
		{
			this.SetCurrentPartLocation(this.playerBedroom.GetRandomEntranceNode());
		}
		InfoWindow infoWindow = InterfaceController.Instance.activeWindows.Find((InfoWindow item) => item.passedEvidence == this.note.evidence);
		if (infoWindow != null)
		{
			infoWindow.forceDisableClose = true;
			infoWindow.forceDisablePin = false;
			PinFolderButtonController pinButton = infoWindow.pinButton;
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.tutorialPointer, pinButton.rect);
			this.pointer = gameObject.GetComponent<RectTransform>();
			this.pointer.sizeDelta = pinButton.rect.sizeDelta;
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.evidencePinned, "", false, 0f, null, null, this.note.evidence, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Pin the note to your case board", trigger, false, default(Vector3), InterfaceControls.Icon.pin, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x000C26CC File Offset: 0x000C08CC
	public void CloseCaseBoard1(int passedVar)
	{
		if (this.pointer != null)
		{
			Object.Destroy(this.pointer.gameObject);
		}
		if (this.playerLounge != null)
		{
			this.SetCurrentPartLocation(this.playerLounge.GetRandomEntranceNode());
		}
		else if (this.playerKitchen != null)
		{
			this.SetCurrentPartLocation(this.playerKitchen.GetRandomEntranceNode());
		}
		else
		{
			this.SetCurrentPartLocation(this.playerBedroom.GetRandomEntranceNode());
		}
		foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows)
		{
			infoWindow.forceDisableClose = false;
			infoWindow.forceDisablePin = false;
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.gameUnpaused, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Close the case board", trigger, false, default(Vector3), InterfaceControls.Icon.pin, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x000C27E8 File Offset: 0x000C09E8
	public void InspectCityDirectory(int passedVar)
	{
		if (this.pointer != null)
		{
			Object.Destroy(this.pointer.gameObject);
		}
		this.PlayerVO("cec04341-f8a0-4878-9b56-f2d7c54ad773", 0f, true, false, false, false, default(Color));
		this.PlayerVO("917078a7-1fc6-4414-8274-bcac940c55fc", 0f, true, false, false, false, default(Color));
		this.SetCurrentPartLocation(this.cityDir.node);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.inspectInteractable, "", false, 0f, null, this.cityDir, null, null, null, null, null, "Inspect", false, default(Vector3));
		this.AddObjective("Inspect the City Directory", trigger, true, this.cityDir.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, true);
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x000C28B4 File Offset: 0x000C0AB4
	public void AddressLookup(int passedVar)
	{
		this.SetCurrentPartLocation(this.cityDir.node);
		InfoWindow infoWindow = InterfaceController.Instance.activeWindows.Find((InfoWindow item) => item.preset == InteriorControls.Instance.cityDirectory.useSingleton.windowStyle);
		if (infoWindow != null)
		{
			string text = this.kidnapper.GetSurName();
			if (text.Length > 1)
			{
				text = text.Substring(0, 1);
			}
			bool flag = false;
			foreach (WindowTabController windowTabController in infoWindow.tabs)
			{
				for (int i = 0; i < windowTabController.name.Length; i++)
				{
					string text2 = windowTabController.name;
					try
					{
						text2 = windowTabController.name.Substring(i, 1);
					}
					catch
					{
					}
					if (text.ToLower() == text2.ToLower())
					{
						this.glow = windowTabController.rect.gameObject.AddComponent<PulseGlowController>();
						this.glow.originalColour = windowTabController.rect.gameObject.GetComponent<Image>().color;
						this.glow.imageToGlow = this.glow.gameObject.GetComponentInChildren<Image>();
						this.glow.lerpColour = Color.white;
						this.glow.SetGlow(true);
						this.glow.glowActive = true;
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.evidenceOpenAndDisplayed, "", false, 0f, null, null, this.kidnapper.home.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Look up |notewritername| in your City Directory", trigger, true, this.cityDir.GetWorldPosition(true), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x000C2ABC File Offset: 0x000C0CBC
	public void PinAddress(int passedVar)
	{
		if (this.pointer != null && this.pointer.gameObject != null)
		{
			Object.Destroy(this.pointer.gameObject);
		}
		if (this.glow != null)
		{
			Object.Destroy(this.glow);
		}
		this.SetCurrentPartLocation(this.cityDir.node);
		InfoWindow infoWindow = InterfaceController.Instance.activeWindows.Find((InfoWindow item) => item.passedEvidence == this.kidnapper.home.evidenceEntry);
		if (infoWindow != null)
		{
			PinFolderButtonController pinButton = infoWindow.pinButton;
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.tutorialPointer, pinButton.rect);
			this.pointer = gameObject.GetComponent<RectTransform>();
			this.pointer.sizeDelta = pinButton.rect.sizeDelta;
		}
		List<Objective.ObjectiveTrigger> list = new List<Objective.ObjectiveTrigger>();
		Objective.ObjectiveTrigger objectiveTrigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.evidencePinned, "", false, 0f, null, null, this.kidnapper.home.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Pin |notewriteraddress| to your case board", objectiveTrigger, true, this.cityDir.GetWorldPosition(true), InterfaceControls.Icon.pin, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		list.Add(objectiveTrigger);
		Objective.ObjectiveTrigger objectiveTrigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.gameUnpaused, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Close the case board", objectiveTrigger2, false, default(Vector3), InterfaceControls.Icon.pin, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		list.Add(objectiveTrigger2);
		if (list.Count > 0)
		{
			this.AddObjective("Close the case board 2", list, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", true, false, true);
		}
		else
		{
			ChapterController.Instance.LoadPart(ChapterController.Instance.currentPart + 1, false, true);
		}
		SessionData.Instance.TutorialTrigger("citydirectory", false);
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x000C2CAC File Offset: 0x000C0EAC
	public void Pickup(int passedVar)
	{
		base.ClearAllObjectives();
		Player.Instance.ClearAllDisabledActions();
		if (this.key != null)
		{
			this.SetCurrentPartLocation(this.playersStorageBox.node);
		}
		this.PlayerVO("baf5a368-15fe-429a-b1e4-3c8118e2ebf9", 0f, false, false, false, false, default(Color));
		try
		{
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToNode, "", false, 0f, null, null, null, this.playersStorageBox.node, null, null, null, "", false, default(Vector3));
			this.AddObjective("Find old detective equipment", trigger, true, this.detectiveStuff.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
		}
		catch
		{
		}
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x000C2D70 File Offset: 0x000C0F70
	public void AcquireLockpicks(int passedVar)
	{
		Player.Instance.ClearAllDisabledActions();
		if (this.playersStorageBox != null)
		{
			this.SetCurrentPartLocation(this.playersStorageBox.node);
		}
		ControlsDisplayController.Instance.DisplayControlIcon(InteractablePreset.InteractionKey.flashlight, "Flashlight", InterfaceControls.Instance.controlIconDisplayTime, false);
		this.PlayerVO("84e52f42-6ef8-4734-9ae6-56e50023666c", 0f, true, false, false, false, default(Color));
		this.PlayerVO("166933ae-6cf4-484f-942c-1e2efdd1f99f", 0f, true, false, false, false, default(Color));
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.acquireLockpicks, "", false, 0f, null, this.playersStorageBox, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Look for hairpins or paperclips to pick the lock", trigger, false, default(Vector3), InterfaceControls.Icon.lockpick, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
		if (this.hairpin == null)
		{
			float num;
			this.hairpin = Toolbox.Instance.FindClosestObjectTo(InteriorControls.Instance.hairpin, Player.Instance.transform.position, null, this.apartment, null, out num, false);
		}
		if (this.paperclip == null)
		{
			float num;
			this.paperclip = Toolbox.Instance.FindClosestObjectTo(InteriorControls.Instance.paperclip, Player.Instance.transform.position, null, this.apartment, null, out num, false);
		}
		if (this.hairpin != null)
		{
			Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.interactableRemoved, "", false, 0f, null, this.hairpin, null, null, null, null, null, "", false, default(Vector3));
			this.AddObjective("Pick up hairpin", trigger2, true, this.hairpin.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 40f, false, "", false, false);
		}
		if (this.paperclip != null)
		{
			Objective.ObjectiveTrigger trigger3 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.interactableRemoved, "", false, 0f, null, this.paperclip, null, null, null, null, null, "", false, default(Vector3));
			this.AddObjective("Pick up paperclip", trigger3, true, this.paperclip.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 60f, false, "", false, false);
		}
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x000C2F84 File Offset: 0x000C1184
	public void UnlockBox(int passedVar)
	{
		base.ClearAllObjectives();
		Player.Instance.ClearAllDisabledActions();
		if (this.playersStorageBox != null)
		{
			this.SetCurrentPartLocation(this.playersStorageBox.node);
		}
		List<string> list = new List<string>();
		list.Add("143e3260-8728-47a0-b623-223be3bcb855");
		PopupMessageController.Instance.TutorialMessage("lockpicking", PopupMessageController.AffectPauseState.automatic, false, list);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.unlockInteractable, "", false, 0f, null, this.playersStorageBox, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Unlock the box", trigger, true, this.playersStorageBox.lockInteractable.GetWorldPosition(true), InterfaceControls.Icon.lockpick, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x000C303C File Offset: 0x000C123C
	public void GatherItems(int passedVar)
	{
		base.ClearAllObjectives();
		Player.Instance.ClearAllDisabledActions();
		if (this.playersStorageBox != null)
		{
			this.SetCurrentPartLocation(this.playersStorageBox.node);
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.interactableRemoved, "", false, 0f, null, this.detectiveStuff, null, null, null, null, null, "Take", false, default(Vector3));
		this.AddObjective("Pick up Equipment", trigger, true, this.detectiveStuff.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x000C30CC File Offset: 0x000C12CC
	public void TakeKey(int passedVar)
	{
		this.SetCurrentPartLocation(this.cityDir.node);
		PopupMessageController.Instance.TutorialMessage("inventory", PopupMessageController.AffectPauseState.automatic, false, null);
		if (this.key != null)
		{
			Objective.ObjectiveTriggerType newType = Objective.ObjectiveTriggerType.keyInventory;
			string newName = "";
			bool newForceProgressAmount = false;
			float newProgressAdd = 0f;
			NewRoom newRoom = null;
			NewDoor door = this.apartmentEntrance.door;
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(newType, newName, newForceProgressAmount, newProgressAdd, newRoom, this.key, null, null, door, null, null, "Take", false, default(Vector3));
			this.AddObjective("Pick up the key to your apartment", trigger, true, this.key.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
			return;
		}
		ChapterController.Instance.LoadPart(ChapterController.Instance.currentPart + 1, false, true);
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x000C3180 File Offset: 0x000C1380
	public void SetRouteOpenCaseBoard(int passedVar)
	{
		this.SetCurrentPartLocation(this.cityDir.node);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.evidenceOpenAndDisplayed, "", false, 0f, null, null, this.kidnapper.home.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Open the case board and select the pinned |kidnapperaddress|", trigger, false, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x000C31FC File Offset: 0x000C13FC
	public void SetRoute(int passedVar)
	{
		if (this.pointer != null)
		{
			Object.Destroy(this.pointer.gameObject);
		}
		if (this.glow != null)
		{
			Object.Destroy(this.glow);
		}
		this.SetCurrentPartLocation(this.cityDir.node);
		InfoWindow infoWindow = InterfaceController.Instance.activeWindows.Find((InfoWindow item) => item.passedEvidence == this.kidnapper.home.evidenceEntry);
		if (infoWindow != null)
		{
			EvidenceLocationalControls componentInChildren = infoWindow.GetComponentInChildren<EvidenceLocationalControls>();
			if (componentInChildren != null && componentInChildren.plotRouteButton != null)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.tutorialPointer, componentInChildren.plotRouteButton.rect);
				this.pointer = gameObject.GetComponent<RectTransform>();
				this.pointer.anchoredPosition = new Vector2(-4f, -4f);
				this.pointer.sizeDelta = componentInChildren.plotRouteButton.rect.sizeDelta + new Vector2(8f, 8f);
			}
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.plotRoute, "", false, 0f, null, null, null, null, null, this.kidnapper.home, null, "", false, default(Vector3));
		this.AddObjective("Plot a route to |notewriteraddress| on your map", trigger, false, this.cityDir.GetWorldPosition(true), InterfaceControls.Icon.location, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x000C3364 File Offset: 0x000C1564
	public void CloseCaseBoard2(int passedVar)
	{
		if (this.pointer != null)
		{
			Object.Destroy(this.pointer.gameObject);
		}
		if (this.glow != null)
		{
			Object.Destroy(this.glow);
		}
		this.SetCurrentPartLocation(this.cityDir.node);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.gameUnpaused, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Close the case board by pressing space", trigger, false, default(Vector3), InterfaceControls.Icon.pin, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x000C3408 File Offset: 0x000C1608
	public void UnlockFrontDoor(int passedVar)
	{
		this.SetCurrentPartLocation(this.apartmentEntrance.fromNode);
		Objective.ObjectiveTriggerType newType = Objective.ObjectiveTriggerType.unlockDoor;
		string newName = "";
		bool newForceProgressAmount = false;
		float newProgressAdd = 0f;
		NewRoom newRoom = null;
		NewDoor door = this.apartmentEntrance.door;
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(newType, newName, newForceProgressAmount, newProgressAdd, newRoom, this.apartmentEntrance.door.handleInteractable, null, null, door, null, null, "Unlock", false, default(Vector3));
		Vector3 pointerPosition = this.apartmentEntrance.door.transform.TransformPoint(new Vector3(-0.5f, 1f, 0.05f));
		this.AddObjective("Unlock the door to your apartment", trigger, true, pointerPosition, InterfaceControls.Icon.hand, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x000C34B4 File Offset: 0x000C16B4
	public void FindNoteWriter(int passedVar)
	{
		Player.Instance.ClearAllDisabledActions();
		this.SetCurrentPartLocation(this.apartmentEntrance.toNode);
		if (this.detectiveStuff != null && !this.detectiveStuff.rem)
		{
			ActionController.Instance.TakeDetectiveStuff(this.detectiveStuff, this.detectiveStuff.node, Player.Instance);
		}
		if (Game.Instance.demoMode && Game.Instance.demoChapterSkip)
		{
			GameplayController.Instance.AddMoney(100, false, "demo mode");
			GameplayController.Instance.AddLockpicks(30, false);
			if (this.detectiveStuff != null)
			{
				FirstPersonItemController.Instance.PickUpItem(this.detectiveStuff, false, false, true, true, true);
			}
			Player.Instance.AddToKeyring(Player.Instance.home, false);
			if (this.note == null)
			{
				Vector3 worldPos = Vector3.Lerp(this.interiorDoorNode.position, this.apartmentEntrance.worldAccessPoint, 0.6f);
				this.note = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.note, this.noteWriter, this.noteWriter, Player.Instance, worldPos, new Vector3(0f, Toolbox.Instance.Rand(0f, 360f, false), 0f), null, null, "bc6fabb0-8ae4-44a1-b1e0-c8305a575b89");
				this.note.AddNewDynamicFingerprint(this.noteWriter, Interactable.PrintLife.manualRemoval);
				this.note.AddNewDynamicFingerprint(this.noteWriter, Interactable.PrintLife.manualRemoval);
				this.note.AddNewDynamicFingerprint(this.noteWriter, Interactable.PrintLife.manualRemoval);
			}
			CasePanelController.Instance.PinToCasePanel(CasePanelController.Instance.activeCase, this.note.evidence, Evidence.DataKey.name, true, default(Vector2), false);
			CasePanelController.Instance.PinToCasePanel(CasePanelController.Instance.activeCase, this.kidnapper.evidenceEntry, Toolbox.Instance.GetList<Evidence.DataKey>(new Evidence.DataKey[]
			{
				Evidence.DataKey.name,
				Evidence.DataKey.address
			}), true, default(Vector2), false);
			CasePanelController.Instance.PinToCasePanel(CasePanelController.Instance.activeCase, this.kidnapper.home.evidenceEntry, Evidence.DataKey.name, true, default(Vector2), false);
			MapController.Instance.PlotPlayerRoute(this.kidnapper.home);
		}
		if (Game.Instance.timeLimited && Game.Instance.startTimerAfterApartmentExit)
		{
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "The demo timer starts now...", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToNode, "", false, 0f, null, null, null, this.kidnappersDoorNode, null, null, null, "", false, default(Vector3));
		this.AddObjective("Go to |notewriteraddress|", trigger, true, this.kidnappersEntrance.wall.position + new Vector3(0f, 1.3f, 0f), InterfaceControls.Icon.run, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
		Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.kidnapper.home, null, "", false, default(Vector3));
		this.AddObjective("Find a way inside...", trigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterWhenAllCompleted, 0f, false, "InvestigateWriterAddress", true, false);
		if (this.kidnappersDoorNode.interactables.Find((Interactable item) => item.name == "Key") == null)
		{
			List<Interactable.Passed> list = new List<Interactable.Passed>();
			list.Add(new Interactable.Passed(Interactable.PassedVarType.roomID, (float)this.kidnappersEntrance.fromNode.room.roomID, null));
			InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.key, this.kidnapper, null, null, this.kidnappersDoorNode.position, Vector3.zero, list, null, "");
		}
		NewRoom newRoom;
		if (Toolbox.Instance.Rand(0f, 1f, false) < 0.7f)
		{
			FurnitureLocation furnitureLocation = Toolbox.Instance.FindFurnitureWithinGameLocation(this.kidnapper.home, InteriorControls.Instance.television, out newRoom);
			if (furnitureLocation != null && furnitureLocation.integratedInteractables.Count > 0)
			{
				furnitureLocation.integratedInteractables[0].SetSwitchState(true, null, true, false, false);
			}
		}
		FurnitureLocation furnitureLocation2 = Toolbox.Instance.FindFurnitureWithinGameLocation(this.kidnapper.home, InteriorControls.Instance.telephoneTable, out newRoom);
		if (furnitureLocation2 != null)
		{
			Interactable interactable = furnitureLocation2.spawnedInteractables.Find((Interactable item) => item.preset == InteriorControls.Instance.deskLamp);
			if (interactable != null)
			{
				interactable.SetSwitchState(true, null, true, false, false);
			}
		}
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x000C3968 File Offset: 0x000C1B68
	public void Knock(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnappersDoorNode);
		this.kidnappersDoor.SetOpen(0f, null, true, 1f);
		this.kidnappersDoor.SetLocked(true, null, false);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.playerAction, "KnockOnDoor", false, 0f, null, this.kidnappersDoor.doorInteractable, null, null, this.kidnappersDoor, null, null, "Knock", false, default(Vector3));
		this.AddObjective("Knock on the door", trigger, true, this.kidnappersDoor.doorInteractable.GetWorldPosition(true) + new Vector3(0f, 1.3f, 0f), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
		Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.kidnapper.home, null, "", false, default(Vector3));
		this.AddObjective("Find a way inside...", trigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterWhenAllCompleted, 0f, false, "InvestigateWriterAddress", true, false);
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x000C3A7C File Offset: 0x000C1C7C
	public void FindWayInside(int passedVar)
	{
		base.ClearAllObjectives();
		this.SetCurrentPartLocation(this.kidnappersDoorNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.kidnapper.home, null, "", false, default(Vector3));
		this.AddObjective("Find a way inside...", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "InvestigateWriterAddress", false, false);
		PopupMessageController.Instance.TutorialMessage("breakingandentering", PopupMessageController.AffectPauseState.automatic, false, null);
		this.AddObjective("Hint: You can pick the lock by looking at the handle", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "InvestigateWriterAddress", false, false);
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x000C3B2C File Offset: 0x000C1D2C
	public void InvestigateWriterAddress(int passedVar)
	{
		base.ClearAllObjectives();
		this.SetCurrentPartLocation(this.kidnappersDoorNode);
		base.Invoke("EscapeTutorial", 3f);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToRoom, "", false, 0f, this.kidnapper.currentRoom, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Investigate the property", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "FoundBody", false, false);
		List<AirDuctGroup.AirVent> list = new List<AirDuctGroup.AirVent>();
		foreach (NewRoom newRoom in this.kidnapper.home.rooms)
		{
			list.AddRange(newRoom.airVents);
		}
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Found " + list.Count.ToString() + " vents...", 2);
		}
		if (list.Count > 0)
		{
			for (int i = 0; i < Mathf.Min(list.Count, 3); i++)
			{
				if (list[i].spawned != null)
				{
					Interactable interactable = list[i].spawned.interactable;
					Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToNode, "", false, 0f, null, null, null, interactable.node, null, null, null, "", false, default(Vector3));
					this.AddObjective("Look around for vents for a potential quick exit " + i.ToString(), trigger2, true, interactable.GetWorldPosition(true), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
				}
			}
		}
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x000C3CF8 File Offset: 0x000C1EF8
	private void EscapeTutorial()
	{
		PopupMessageController.Instance.TutorialMessage("escaping", PopupMessageController.AffectPauseState.automatic, false, null);
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x000C3D0C File Offset: 0x000C1F0C
	public void FoundBody(int passedVar)
	{
		this.PlayerVO("fb466dcb-e473-426e-8219-0c7734c56dd6", 0f, true, false, false, false, default(Color));
		this.SetCurrentPartLocation(this.noteWriter.currentNode);
		List<Objective.ObjectiveTrigger> list = new List<Objective.ObjectiveTrigger>();
		Objective.ObjectiveTrigger objectiveTrigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.linkImageWithName, "", false, 0f, null, null, this.kidnapper.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		list.Add(objectiveTrigger);
		this.AddObjective("Identify the body", objectiveTrigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		Objective.ObjectiveTrigger objectiveTrigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.discoverEvidence, "", false, 0f, null, null, this.kidnapper.death.GetTimeOfDeathEvidence(), null, null, null, null, "", false, default(Vector3));
		list.Add(objectiveTrigger2);
		this.AddObjective("Find time of death", objectiveTrigger2, true, this.kidnapper.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.upperTorso).position, InterfaceControls.Icon.skull, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.noMoreObjectives, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("NextPhase", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "CrimeSceneClues", true, false);
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Attempting to add notewriter dialog...", 2);
		}
		this.noteWriter.evidenceEntry.AddDialogOption(Evidence.DataKey.name, ChapterController.Instance.loadedChapter.dialogEvents[1], null, null, false);
		this.notewriterDialogAdded = true;
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x000C3EB8 File Offset: 0x000C20B8
	public void CrimeSceneClues(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnapper.currentNode);
		this.PlayerVO("93f06c61-37dd-4061-a31e-c63a6ead82b3", 0f, true, false, false, false, default(Color));
		this.PlayerVO("a1d82a1b-2137-4fb8-8d04-7cd41fcaa29d", 0f, true, false, false, false, default(Color));
		base.Invoke("PrintsTutorial", 8f);
		List<Objective.ObjectiveTrigger> list = new List<Objective.ObjectiveTrigger>();
		List<Objective.ObjectiveTrigger> list2 = new List<Objective.ObjectiveTrigger>();
		if (this.restaurantReceipt == null)
		{
			foreach (NewRoom newRoom in this.kidnapper.home.rooms)
			{
				foreach (NewNode newNode in newRoom.nodes)
				{
					foreach (Interactable interactable in newNode.interactables)
					{
						if (interactable.pv != null)
						{
							Interactable.Passed passed = interactable.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.time);
							if (passed != null)
							{
								passed.value = this.meetTime;
							}
						}
						if (interactable.preset == InteriorControls.Instance.receipt)
						{
							EvidenceReceipt evidenceReceipt = interactable.evidence as EvidenceReceipt;
							if (evidenceReceipt != null && evidenceReceipt.soldHere == this.restaurant.company)
							{
								if (Game.Instance.collectDebugData)
								{
									Game.Log("Chapter: Found restaurant receipt!", 2);
								}
								this.receiptInBin = false;
								this.restaurantReceipt = evidenceReceipt;
								evidenceReceipt.purchasedTime = this.meetTime;
								break;
							}
						}
						else
						{
							EvidenceMultiPage evidenceMultiPage = interactable.evidence as EvidenceMultiPage;
							if (evidenceMultiPage != null)
							{
								foreach (EvidenceMultiPage.MultiPageContent multiPageContent in evidenceMultiPage.pageContent)
								{
									if (multiPageContent.meta > 0)
									{
										MetaObject metaObject = CityData.Instance.FindMetaObject(multiPageContent.meta);
										if (metaObject != null && metaObject.preset == InteriorControls.Instance.receipt.name)
										{
											if (metaObject.passed != null)
											{
												Interactable.Passed passed2 = metaObject.passed.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.time);
												if (passed2 != null)
												{
													passed2.value = this.meetTime;
												}
											}
											foreach (Interactable.Passed passed3 in metaObject.passed)
											{
												if (passed3.varType == Interactable.PassedVarType.companyID && (int)passed3.value == this.restaurant.company.companyID)
												{
													if (Game.Instance.collectDebugData)
													{
														Game.Log("Chapter: Found restaurant receipt!", 2);
													}
													this.restaurantReceipt = metaObject.GetEvidence(true, interactable.node.nodeCoord);
													this.receiptInBin = true;
													break;
												}
											}
										}
									}
									if (this.restaurantReceipt != null)
									{
										break;
									}
								}
							}
						}
					}
					if (this.restaurantReceipt != null)
					{
						break;
					}
				}
				if (this.restaurantReceipt != null)
				{
					break;
				}
			}
		}
		if (this.restaurantReceipt != null)
		{
			Objective.ObjectiveTrigger objectiveTrigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, this.restaurantReceipt, null, null, null, null, "", true, default(Vector3));
			list.Add(objectiveTrigger);
			list2.Add(objectiveTrigger);
			this.AddObjective("FindReceipt", objectiveTrigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "FindReceipt", true, false);
			this.receiptSearchPromt = true;
		}
		if (this.kidnappersAddressBook != null)
		{
			Objective.ObjectiveTrigger objectiveTrigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.inspectInteractable, "", false, 0f, null, this.kidnappersAddressBook, null, null, null, null, null, "", false, default(Vector3));
			list.Add(objectiveTrigger2);
			list2.Add(objectiveTrigger2);
			this.AddObjective("FindAddressBook", objectiveTrigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "FindAddressBook", true, false);
			this.addressBookSearchPrompt = true;
		}
		if (this.kidnappersCalendar != null && this.meetingTimeEvidence != null)
		{
			Objective.ObjectiveTrigger objectiveTrigger3 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, this.meetingTimeEvidence, null, null, null, null, "", true, default(Vector3));
			list.Add(objectiveTrigger3);
			list2.Add(objectiveTrigger3);
			this.AddObjective("FindCalendar", objectiveTrigger3, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "FindCalendar", true, false);
		}
		if (this.meetingNote != null)
		{
			if (this.meetingNote.name != null && this.meetingNote.node != null && this.meetingNote.node.room != null && Game.Instance.collectDebugData)
			{
				Game.Log("Chapter: Meeting note: " + this.meetingNote.name + " placed in " + this.meetingNote.node.room.GetName(), 2);
			}
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, this.meetingNote.evidence, null, null, null, null, "", false, default(Vector3));
			this.AddObjective("FindMeetingNote", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "FindMeetingNote", true, false);
		}
		this.AddObjective("Search for clues at the crime scene", list, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nothing, 0f, false, "", false, false, true);
		if (this.kidnapperPhone != null)
		{
			Objective.ObjectiveTrigger objectiveTrigger4 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.checkRecentCalls, "", false, 0f, null, this.kidnapperPhone, null, null, null, null, null, "", false, default(Vector3));
			list2.Add(objectiveTrigger4);
			this.AddObjective("Check for recent calls", objectiveTrigger4, true, this.kidnapperPhone.GetWorldPosition(true), InterfaceControls.Icon.telephone, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		}
		Objective.ObjectiveTrigger objectiveTrigger5 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.identifyFinerprints, "", false, 0f, null, null, this.kidnapper.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		list2.Add(objectiveTrigger5);
		this.AddObjective("Get victim's prints", objectiveTrigger5, true, this.kidnapper.rightHandInteractable.controller.transform.position, InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		Objective.ObjectiveTrigger objectiveTrigger6 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.findFingerprints, "", false, 0f, null, null, this.killer.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		list2.Add(objectiveTrigger6);
		this.fingerprintPrompt = true;
		bool flag = false;
		Vector3 vector = Vector3.zero;
		foreach (NewNode newNode2 in this.kidnapper.home.nodes)
		{
			foreach (Interactable interactable2 in newNode2.interactables)
			{
				if (interactable2.df != null && interactable2.preset.name == "Safe" && interactable2.df.Exists((Interactable.DynamicFingerprint item) => item.id == this.killer.humanID))
				{
					vector = interactable2.GetWorldPosition(true) + new Vector3(0f, 0.1f, 0f);
					flag = true;
				}
			}
		}
		NewNode newNode3 = null;
		if (!flag)
		{
			foreach (NewNode newNode4 in this.kidnapper.home.nodes)
			{
				foreach (Interactable interactable3 in newNode4.interactables)
				{
					if (interactable3.df != null && interactable3.df.Exists((Interactable.DynamicFingerprint item) => item.id == this.killer.humanID) && (newNode3 == null || interactable3.sw0))
					{
						vector = interactable3.GetWorldPosition(true) + new Vector3(0f, 0.1f, 0f);
						newNode3 = interactable3.node;
						flag = true;
					}
				}
			}
		}
		this.AddObjective("Scan for fingerprints", objectiveTrigger6, flag, vector + new Vector3(0f, 0.25f, 0f), InterfaceControls.Icon.printScanner, Objective.OnCompleteAction.invokeFunction, 0f, false, "FindPrints", false, false);
		Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToNode, "", false, 0f, null, null, null, newNode3, null, null, null, "", false, default(Vector3));
		this.AddObjective("InventoryPrompt", trigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "InventoryPrompt", true, false);
		this.AddObjective("Police Knock", list2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "PoliceCall", true, false, true);
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x000C48F0 File Offset: 0x000C2AF0
	public void PrintsTutorial()
	{
		PopupMessageController.Instance.TutorialMessage("fingerprints", PopupMessageController.AffectPauseState.automatic, false, null);
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x000C4904 File Offset: 0x000C2B04
	private void InventoryPrompt()
	{
		ControlsDisplayController.Instance.DisplayControlIconAfterDelay(1f, InteractablePreset.InteractionKey.WeaponSelect, "Inventory", InterfaceControls.Instance.controlIconDisplayTime + 2f, false);
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x000C4930 File Offset: 0x000C2B30
	public void FindMeetingNote()
	{
		this.PlayerVO("1de86551-31de-4439-9ab5-24f2341eb33a", 0f, true, false, false, false, default(Color));
		this.discoveredWeaponsDealer = true;
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x000C4964 File Offset: 0x000C2B64
	public void FindAddressNote()
	{
		this.PlayerVO("d199791d-4ebb-4af4-bdce-39174d791d51", 0f, true, false, false, false, default(Color));
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x00002265 File Offset: 0x00000465
	public void FindAddressBook()
	{
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x000C4990 File Offset: 0x000C2B90
	public void FindWorkID()
	{
		this.PlayerVO("84f8c4a0-45eb-442a-90d7-20bb440f42c6", 0f, true, false, false, false, default(Color));
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x000C49BC File Offset: 0x000C2BBC
	public void FindReceipt()
	{
		this.PlayerVO("62c546bc-f8f3-47f8-b141-baba4ec2ebcc", 0f, true, false, false, false, default(Color));
		this.receiptSearchActivated = true;
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x000C49F0 File Offset: 0x000C2BF0
	public void FindCalendar()
	{
		this.PlayerVO("77e7055c-cf8d-42fe-a41a-8675377cff27", 0f, true, false, false, false, default(Color));
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x000C4A1C File Offset: 0x000C2C1C
	public void FindPrints()
	{
		this.printSearchActivated = true;
		this.PlayerVO("add6b43c-d21f-4bda-a020-f3883fd5f9e0", 0f, true, false, false, false, default(Color));
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x000C4A50 File Offset: 0x000C2C50
	public void PoliceCall(int passedVar)
	{
		base.ClearAllObjectives();
		this.SetCurrentPartLocation(this.kidnapper.currentNode);
		this.kidnapper.unreportable = false;
		this.kidnappersDoor.SetOpen(0f, null, true, 1f);
		this.kidnappersDoor.SetLocked(true, null, false);
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Locking kidnappers door for enforcer sequence...", 2);
		}
		GameplayController.Instance.CallEnforcers(this.kidnapper.home, false, true);
		this.PlayerVO("24c59b68-e667-4d37-ba6a-c84ca103e620", 1f, true, false, false, false, default(Color));
		this.PlayerVO("a9015c83-9133-4dc8-b219-b6500a8c7e66", 1f, true, false, false, false, default(Color));
		base.Invoke("StealthTutorial", 12f);
		if (Game.Instance.collectDebugData)
		{
			Game.Log("The kidnapper home is " + this.kidnapper.home.name, 2);
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.escapeBuilding, "", false, 0f, null, null, null, null, null, this.kidnapper.home, null, "", false, default(Vector3));
		this.AddObjective("Escape the apartment", trigger, false, default(Vector3), InterfaceControls.Icon.run, Objective.OnCompleteAction.specificChapterByString, 1f, false, "CollectHandIn", false, false);
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x000C4BA2 File Offset: 0x000C2DA2
	private void StealthTutorial()
	{
		PopupMessageController.Instance.TutorialMessage("custom_stealthcombat", PopupMessageController.AffectPauseState.automatic, false, null);
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x00002265 File Offset: 0x00000465
	private void TriggerEscapeEvents()
	{
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x000C4BB8 File Offset: 0x000C2DB8
	public void CollectHandIn(int passedVar)
	{
		base.ClearAllObjectives();
		this.SetCurrentPartLocation(this.kidnappersDoorNode);
		MurderController.Instance.AssignActiveCase(this.thisCase);
		this.thisCase.SetStatus(Case.CaseStatus.handInNotCollected, true);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.collectHandIn, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Hand In", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "ViewHandIn", true, false);
		if (MapController.Instance.playerRoute == null && this.thisCase.handIn.Count > 0)
		{
			Interactable interactable = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(this.thisCase.handIn[0], ref interactable))
			{
				MapController.Instance.PlotPlayerRoute(interactable.node.gameLocation);
			}
		}
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x000C4C9C File Offset: 0x000C2E9C
	public void ViewHandIn(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnappersDoorNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.viewHandIn, "View Hand In", false, 1f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("View Hand In", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "ViewedHandIn", false, false);
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x000C4D04 File Offset: 0x000C2F04
	public void ViewedHandIn(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnappersDoorNode);
		this.PlayerVO("8ba8c86a-8874-4195-a619-ca64f09f660a", 0f, true, false, false, false, default(Color));
		this.PlayerVO("be076959-6241-4dae-bbc1-55059a7947d9", 0f, true, false, false, false, default(Color));
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.submitCase, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("complete the hand in form", trigger, false, default(Vector3), InterfaceControls.Icon.resolve, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		ChapterController.Instance.LoadPart("DisplayLeads");
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x000C4DB8 File Offset: 0x000C2FB8
	public void DisplayLeads(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnappersDoorNode);
		this.kidnapper.unreportable = false;
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.submitCase, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("complete the hand in form", trigger, false, default(Vector3), InterfaceControls.Icon.resolve, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		this.nextLeadDelay = 0.005f;
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x000C4E38 File Offset: 0x000C3038
	public void ExecuteChangeLeadsManual()
	{
		bool enableLeftButton = true;
		bool enableRightButton = true;
		bool enableSecondaryLeftButton = true;
		bool enableSecondaryRightButton = this.discoveredWeaponsDealer;
		PopupMessageController.Instance.PopupMessage("Choose Next Lead", enableLeftButton, enableRightButton, "Investigate CCTV Records", "Investigate Phone Calls", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, enableSecondaryLeftButton, enableSecondaryRightButton, "Investigate VMails", "Investigate Murder Weapon", true, "Disable Leads", false, "", "");
		PopupMessageController.Instance.OnLeftButton += this.ChooseInvestigateCCTV;
		PopupMessageController.Instance.OnRightButton += this.ChooseInvestigatePhone;
		PopupMessageController.Instance.OnLeftButton2 += this.ChooseInvestigateVmails;
		PopupMessageController.Instance.OnRightButton2 += this.ChooseInvestigateMurderWeapon;
		PopupMessageController.Instance.OnOptionButton += this.ChooseCancelLeads;
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x000C4F04 File Offset: 0x000C3104
	public void ClearLeads(bool clearDiner, bool clearOffice, bool clearPhone, bool clearWeaponsDealer)
	{
		if (clearDiner)
		{
			base.ClearObjective("Restaurant");
			base.ClearObjective("Access the back office");
			base.ClearObjective("Disable security by tracing the red wires to the breaker box (optional)");
			base.ClearObjective("Bribe staff for access (optional)");
			base.ClearObjective("Disable security systems by using the lever (optional)");
			base.ClearObjective("Access the cruncher");
			base.ClearObjective("Open the 'Surveillance' application");
			base.ClearObjective("Look for surveillance entries from around |meettime|");
			base.ClearObjective("|story.kidnapper.name| is sitting with somebody. Open an evidence window by inspecting their mugshot");
			base.ClearObjective("Check the table where |story.kidnapper.name| was sitting");
			base.ClearObjective("FindMeetingNote");
		}
		if (clearOffice)
		{
			base.ClearObjective("Office");
			base.ClearObjective("Access kidnapper cruncher");
			base.ClearObjective("Find employee record for |story.kidnapper.name| for more info (optional)");
			base.ClearObjective("Disable security systems by using the lever (optional)");
			base.ClearObjective("Open the 'V Mail' application");
			base.ClearObjective("Print any relevent vmail threads");
		}
		if (clearPhone)
		{
			base.ClearObjective("Find cabinet");
			base.ClearObjective("Unlock cabinet");
			base.ClearObjective("Access cabinet");
			base.ClearObjective("Trace Call");
			base.ClearObjective("Access other cabinet");
			base.ClearObjective("Find other call");
			base.ClearObjective("Go to other address");
		}
		if (clearWeaponsDealer)
		{
			base.ClearObjective("Weapons Dealer");
			base.ClearObjective("Search Weapons Dealer");
		}
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x000C503C File Offset: 0x000C323C
	public void InvestigateCCTV(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnappersDoorNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.restaurant, null, "", false, default(Vector3));
		this.AddObjective("Restaurant", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "ArrivalDiner", false, false);
		Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.restaurant, null, "", false, default(Vector3));
		this.AddObjective("Hint: You can always find a city directory next to a public telelphone", trigger2, false, default(Vector3), InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		this.AddObjective("Hint: You also search for public or visited places by using |controls.notebook|", trigger2, false, default(Vector3), InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x000C5124 File Offset: 0x000C3324
	public void InvestigateVmails(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnappersDoorNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.kidnapper.job.employer.address, null, "", false, default(Vector3));
		this.AddObjective("Office", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "ArrivalWorkplace", false, false);
		Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.kidnapper.job.employer.address, null, "", false, default(Vector3));
		this.AddObjective("Hint: You can always find a city directory next to a public telelphone", trigger2, false, default(Vector3), InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		this.AddObjective("Hint: You also search for public or visited places by using |controls.notebook|", trigger2, false, default(Vector3), InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x000C5228 File Offset: 0x000C3428
	public void InvesitgatePhone(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnappersDoorNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToNode, "", false, 0f, null, null, null, this.kidnapperRouter.node, null, null, null, "", false, default(Vector3));
		Vector3 pointerPosition = Vector3.zero;
		if (this.kidnapperRouter != null)
		{
			pointerPosition = this.kidnapperRouter.GetWorldPosition(true);
		}
		this.AddObjective("Find cabinet", trigger, true, pointerPosition, InterfaceControls.Icon.hand, Objective.OnCompleteAction.specificChapterByString, 0f, false, "AccessCabinet", false, false);
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x000C52AC File Offset: 0x000C34AC
	public void InvestigateMurderWeapon(int passedVar)
	{
		NewRoom newRoom = this.weaponSeller.rooms.Find((NewRoom item) => item.entrances.Count > 0);
		this.SetCurrentPartLocation(newRoom.entrances[0].fromNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.weaponSeller, null, "", false, default(Vector3));
		this.AddObjective("Weapons Dealer", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "SearchWeaponsDealer", false, false);
		Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.weaponSeller, null, "", false, default(Vector3));
		this.AddObjective("Hint: You can always find a city directory next to a public telelphone", trigger2, false, default(Vector3), InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		this.AddObjective("Hint: You also search for public or visited places by using |controls.notebook|", trigger2, false, default(Vector3), InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x000C53D0 File Offset: 0x000C35D0
	public void ArrivalDiner(int passedVar)
	{
		NewRoom eateryRoom = this.restaurant.rooms.Find((NewRoom item) => item.roomType.name == "Diner" || item.roomType.name == "Eatery" || item.roomType.name == "Bar");
		this.SetCurrentPartLocation(eateryRoom.entrances[0].fromNode);
		this.PlayerVO("a5e5e68f-4a2c-4db9-8910-de678d41b919", 0f, true, false, false, false, default(Color));
		this.PlayerVO("79fe960e-6ae9-4e02-a8d9-1ecdbe0caca8", 0f, true, false, false, false, default(Color));
		Vector3 pointerPosition = this.dinerCruncher.GetWorldPosition(true);
		NewNode.NodeAccess nodeAccess = this.dinerCruncher.node.room.entrances.Find((NewNode.NodeAccess item) => item.GetOtherRoom(this.dinerCruncher.node.room) == eateryRoom);
		if (nodeAccess != null)
		{
			pointerPosition = nodeAccess.worldAccessPoint + new Vector3(0f, 1.5f, 0f);
		}
		PopupMessageController.Instance.TutorialMessage("commercialproperty", PopupMessageController.AffectPauseState.automatic, false, null);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToRoom, "", false, 0f, this.restaurantBackroom, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Access the back office", trigger, true, pointerPosition, InterfaceControls.Icon.location, Objective.OnCompleteAction.specificChapterByString, 0f, false, "AccessBackroom", false, false);
		Interactable breakerSecurity = this.restaurant.GetBreakerSecurity();
		if (breakerSecurity != null)
		{
			Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToNode, "", false, 0f, null, null, null, breakerSecurity.node, null, null, null, "", false, default(Vector3));
			this.AddObjective("Disable security by tracing the red wires to the breaker box (optional)", trigger2, true, breakerSecurity.GetWorldPosition(true), InterfaceControls.Icon.lockpick, Objective.OnCompleteAction.invokeFunction, 0f, false, "BreakerBox", false, false);
		}
		Objective.ObjectiveTrigger trigger3 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToRoom, "", false, 0f, this.restaurantBackroom, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Bribe staff for access (optional)", trigger3, false, Vector3.zero, InterfaceControls.Icon.citizen, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		if (this.dinerFlyer != null)
		{
			Objective.ObjectiveTrigger trigger4 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, this.dinerFlyer.evidence, null, null, null, null, "", false, default(Vector3));
			this.AddObjective("FlyerFind", trigger4, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "FindFlyer", true, false);
		}
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x000C564C File Offset: 0x000C384C
	private void BreakerBox()
	{
		Interactable breakerSecurity = this.restaurant.GetBreakerSecurity();
		if (breakerSecurity != null)
		{
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.switchStateFalse, "", false, 0f, null, breakerSecurity, null, null, null, null, null, "", false, default(Vector3));
			this.AddObjective("Disable security systems by using the lever (optional)", trigger, true, breakerSecurity.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.invokeFunction, 0f, false, "BreakerTip", false, false);
		}
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x000C56B4 File Offset: 0x000C38B4
	private void BreakerTip()
	{
		SessionData.Instance.TutorialTrigger("breakingandentering", false);
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x000C56C8 File Offset: 0x000C38C8
	public void AccessBackroom(int passedVar)
	{
		this.SetCurrentPartLocation(this.dinerCruncher.node);
		this.PlayerVO("e3be726f-53fc-48c5-b6fb-01a7516a60e2", 0f, true, false, false, false, default(Color));
		PopupMessageController.Instance.TutorialMessage("computers", PopupMessageController.AffectPauseState.automatic, false, null);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.accessCruncher, "", false, 0f, null, this.dinerCruncher, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Access the cruncher", trigger, true, this.dinerCruncher.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.specificChapterByString, 0f, false, "LaunchSurveillance", false, false);
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x000C576C File Offset: 0x000C396C
	public void LaunchSurveillance(int passedVar)
	{
		this.SetCurrentPartLocation(this.dinerCruncher.node);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.accessApp, "Surveillance", false, 0f, null, this.dinerCruncher, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Open the 'Surveillance' application", trigger, false, this.dinerCruncher.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.specificChapterByString, 0f, false, "FoundRecords", false, false);
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x000C57E4 File Offset: 0x000C39E4
	public void FoundRecords(int passedVar)
	{
		this.SetCurrentPartLocation(this.dinerCruncher.node);
		PopupMessageController.Instance.TutorialMessage("surveillance", PopupMessageController.AffectPauseState.automatic, false, null);
		this.UpdateCamReferences();
		if (this.kidnapperOnCam)
		{
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.findSurveillanceWith, "", false, 0f, null, null, this.kidnapper.evidenceEntry, null, null, this.restaurant, null, "", false, default(Vector3));
			this.AddObjective("Look for surveillance entries from around |meettime|", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "KidnapperOnCam", false, false);
			return;
		}
		PopupMessageController.Instance.PopupMessage("dud_lead", true, false, "Cancel", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
		this.PlayerVO("5d31cc22-5924-48c6-a117-2ada1b5c6074", 0f, true, false, false, false, default(Color));
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x000C58E0 File Offset: 0x000C3AE0
	private void UpdateCamReferences()
	{
		this.notewriterOnCam = false;
		this.kidnapperOnCam = false;
		foreach (Interactable interactable in this.restaurant.securityCameras)
		{
			if (interactable.sceneRecorder != null)
			{
				foreach (SceneRecorder.SceneCapture sceneCapture in interactable.sceneRecorder.interactable.cap)
				{
					if (sceneCapture.aCap.Exists((SceneRecorder.ActorCapture item) => item.id == this.kidnapper.humanID))
					{
						if (Game.Instance.collectDebugData)
						{
							Game.Log("Chapter: Kidnapper " + this.kidnapper.GetCitizenName() + " is on cam at " + SessionData.Instance.TimeStringOnDay(sceneCapture.t, true, true), 2);
						}
						this.kidnapperOnCam = true;
						if (sceneCapture.aCap.Exists((SceneRecorder.ActorCapture item) => item.id == this.noteWriter.humanID))
						{
							if (Game.Instance.collectDebugData)
							{
								Game.Log("Chapter: Notewriter " + this.noteWriter.GetCitizenName() + " is on also cam at " + SessionData.Instance.TimeStringOnDay(sceneCapture.t, true, true), 2);
							}
							this.notewriterOnCam = true;
						}
					}
					if (this.notewriterOnCam && this.kidnapperOnCam)
					{
						break;
					}
				}
			}
			if (this.notewriterOnCam && this.kidnapperOnCam)
			{
				break;
			}
		}
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x000C5A98 File Offset: 0x000C3C98
	public void KidnapperOnCam(int passedVar)
	{
		this.SetCurrentPartLocation(this.dinerCruncher.node);
		this.UpdateCamReferences();
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, this.noteWriter.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("|story.kidnapper.name| is sitting with somebody. Open an evidence window by inspecting their mugshot", trigger, false, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.specificChapterByString, 0f, false, "OpenNotewirterEvidence", false, false);
		this.PlayerVO("ff3d774e-0309-4521-b255-31acb2eacaed", 0f, true, false, false, false, default(Color));
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x000C5B34 File Offset: 0x000C3D34
	public void OpenNotewirterEvidence(int passedVar)
	{
		this.SetCurrentPartLocation(this.dinerCruncher.node);
		if (this.dinerFlyer != null)
		{
			Vector3 worldPosition = this.dinerFlyer.GetWorldPosition(true);
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, this.dinerFlyer.evidence, null, null, null, null, "", false, default(Vector3));
			this.AddObjective("Check the table where |story.kidnapper.name| was sitting", trigger, true, worldPosition, InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "FindFlyer", false, false);
			return;
		}
		PopupMessageController.Instance.PopupMessage("dud_lead", true, false, "Cancel", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
		ChapterController.Instance.LoadPart("DisplayLeads");
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x000C5C0C File Offset: 0x000C3E0C
	public void FindFlyer(int passedVar)
	{
		this.PlayerVO("7bb0e329-6311-49cf-8d7b-99e6a3d63993", 0f, true, false, false, false, default(Color));
		base.ClearObjective("Disable security by tracing the red wires to the breaker box (optional)");
		base.ClearObjective("Bribe staff for access (optional)");
		this.WarnNotewriter();
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x000C5C54 File Offset: 0x000C3E54
	public void ArrivalWorkplace(int passedVar)
	{
		NewRoom newRoom = this.kidnapper.job.employer.address.rooms.Find((NewRoom item) => item.entrances.Count > 0 && !item.isNullRoom);
		this.SetCurrentPartLocation(newRoom.entrances[0].fromNode);
		Interactable breakerSecurity = this.kidnapper.job.employer.address.GetBreakerSecurity();
		if (breakerSecurity != null)
		{
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToNode, "", false, 0f, null, null, null, breakerSecurity.node, null, null, null, "", false, default(Vector3));
			this.AddObjective("Disable security by tracing the red wires to the breaker box (optional)", trigger, true, breakerSecurity.GetWorldPosition(true), InterfaceControls.Icon.lockpick, Objective.OnCompleteAction.invokeFunction, 0f, false, "BreakerBoxWorkplace", false, false);
		}
		if (this.workplaceMessageNote != null)
		{
			Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, this.workplaceMessageNote.evidence, null, null, null, null, "", false, default(Vector3));
			this.AddObjective("FindMeetingNote", trigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "FindMeetingNote", true, false);
		}
		this.PlayerVO("6c3006c1-ce37-430d-b8ee-9feb73ecf196", 0f, true, false, false, false, default(Color));
		if (this.kidnapper.job.employer.IsOpenAtThisTime(SessionData.Instance.gameTime, SessionData.Instance.decimalClock, SessionData.Instance.day))
		{
			this.PlayerVO("3b444a2f-3eb5-484e-8bb0-08636f7fd16e", 0f, true, false, false, false, default(Color));
		}
		else
		{
			this.PlayerVO("8bd617a9-9d4c-479e-8e51-01de26ec9a74", 0f, true, false, false, false, default(Color));
		}
		foreach (int num in this.kidnapper.passcode.notes)
		{
			Interactable interactable = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(num, ref interactable) && interactable != null && interactable.node != null && this.kidnapper.job != null && this.kidnapper.job.employer != null && interactable.node.gameLocation == this.kidnapper.job.employer.address && Game.Instance.collectDebugData)
			{
				Game.Log("Chapter: Found kidnapper's passcode note at work!", 2);
			}
		}
		Objective.ObjectiveTrigger trigger3 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.accessAnyCruncher, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Access Cruncher", trigger3, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "ComputerTutorial", true, false);
		Objective.ObjectiveTrigger trigger4 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.accessCruncher, "", false, 0f, null, null, this.kidnapper.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Access kidnapper cruncher", trigger4, false, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.specificChapterByString, 0f, false, "LaunchVmail", false, false);
		if (this.kidnapper.job.employer.address.company.employeeRoster != null)
		{
			EvidenceMultiPage.MultiPageContent multiPageContent = this.kidnapper.job.employer.address.company.employeeRoster.pageContent.Find((EvidenceMultiPage.MultiPageContent item) => item.evID == "EmployeeRecord" + this.kidnapper.humanID.ToString());
			if (multiPageContent != null)
			{
				Objective.ObjectiveTrigger trigger5 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, multiPageContent.GetEvidence(), null, null, null, null, "", false, default(Vector3));
				this.AddObjective("Find employee record for |story.kidnapper.name| for more info (optional)", trigger5, true, this.kidnapper.job.employer.address.company.employeeRoster.interactable.GetWorldPosition(true) + new Vector3(0f, 0.25f, 0f), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
			}
		}
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x000C6090 File Offset: 0x000C4290
	private void BreakerBoxWorkplace()
	{
		Interactable breakerSecurity = this.kidnapper.job.employer.address.GetBreakerSecurity();
		if (breakerSecurity != null)
		{
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.switchStateFalse, "", false, 0f, null, breakerSecurity, null, null, null, null, null, "", false, default(Vector3));
			this.AddObjective("Disable security systems by using the lever (optional)", trigger, true, breakerSecurity.GetWorldPosition(true), InterfaceControls.Icon.hand, Objective.OnCompleteAction.invokeFunction, 0f, false, "BreakerTip", false, false);
		}
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x000C6107 File Offset: 0x000C4307
	private void ComputerTutorial()
	{
		PopupMessageController.Instance.TutorialMessage("computers", PopupMessageController.AffectPauseState.automatic, false, null);
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x000C611C File Offset: 0x000C431C
	public void LaunchVmail(int passedVar)
	{
		NewRoom newRoom = this.kidnapper.job.employer.address.rooms.Find((NewRoom item) => item.entrances.Count > 0 && !item.isNullRoom);
		this.SetCurrentPartLocation(newRoom.entrances[0].fromNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.accessApp, "VMail", false, 0f, null, null, this.kidnapper.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Open the 'V Mail' application", trigger, false, default(Vector3), InterfaceControls.Icon.hand, Objective.OnCompleteAction.specificChapterByString, 0f, false, "AccessKidnapperCruncher", false, false);
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x000C61D8 File Offset: 0x000C43D8
	public void AccessKidnapperCruncher(int passedVar)
	{
		NewRoom newRoom = this.kidnapper.job.employer.address.rooms.Find((NewRoom item) => item.entrances.Count > 0);
		this.SetCurrentPartLocation(newRoom.entrances[0].fromNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.printVmail, "63c2e755-2840-44f6-b4da-f72e9a84d890", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Print any relevent vmail threads", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "FoundNotewriterID", false, false);
		Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.printVmail, "37d3cbd8-9450-430f-8bf2-9e3efbf7f3f7", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Print any relevent vmail threads", trigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "FindMeetingNote", true, false);
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x000C62D8 File Offset: 0x000C44D8
	public void FoundNotewriterID(int passedVar)
	{
		this.PlayerVO("7bb0e329-6311-49cf-8d7b-99e6a3d63993", 0f, true, false, false, false, default(Color));
		this.findNotewriter = true;
		NewRoom newRoom = this.noteWriter.job.employer.address.rooms.Find((NewRoom item) => item.entrances.Count > 0);
		this.SetCurrentPartLocation(newRoom.entrances[0].fromNode);
		this.WarnNotewriter();
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x000C6368 File Offset: 0x000C4568
	public void WarnNotewriter()
	{
		if (!this.killer.ai.isConvicted)
		{
			this.findNotewriter = true;
			if (Game.Instance.collectDebugData)
			{
				Game.Log("Chapter: Attempting to add notewriter dialog...", 2);
			}
			this.noteWriter.evidenceEntry.AddDialogOption(Evidence.DataKey.name, ChapterController.Instance.loadedChapter.dialogEvents[1], null, null, false);
			this.notewriterDialogAdded = true;
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.notewriterWarned, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
			this.AddObjective("Find |notewritername|", trigger, false, default(Vector3), InterfaceControls.Icon.citizen, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
			Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.playerActionNobodyHome, "KnockOnDoor", false, 0f, null, null, null, null, null, this.noteWriter.home, null, "", false, default(Vector3));
			this.AddObjective("Nobody home", trigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "NobodyHome", true, false);
		}
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x000C6480 File Offset: 0x000C4680
	public void AccessCabinet(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnapperRouter.node);
		if (this.kidnapperRouterDoor.lockInteractable != null && this.kidnapperRouterDoor.lockInteractable.locked)
		{
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.unlockInteractable, "", false, 0f, null, this.kidnapperRouterDoor, null, null, null, null, null, "", false, default(Vector3));
			this.AddObjective("Unlock cabinet", trigger, true, this.kidnapperRouterDoor.lockInteractable.GetWorldPosition(true), InterfaceControls.Icon.lockpick, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
		}
		Vector3 pointerPosition = Vector3.zero;
		if (this.kidnapperRouter != null)
		{
			pointerPosition = this.kidnapperRouter.GetWorldPosition(true);
		}
		Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, this.kidnapperRouter.evidence, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Access cabinet", trigger2, true, pointerPosition, InterfaceControls.Icon.hand, Objective.OnCompleteAction.specificChapterByString, 0f, false, "TraceCall", false, false);
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x000C6588 File Offset: 0x000C4788
	public void TraceCall(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnapperRouter.node);
		Vector3 pointerPosition = Vector3.zero;
		if (this.kidnapperRouter != null)
		{
			pointerPosition = this.kidnapperRouter.GetWorldPosition(true);
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.plotRouteToCallInvolving, "", false, 0f, null, null, null, null, null, this.kidnapper.home, null, "", false, default(Vector3));
		this.AddObjective("Trace Call", trigger, true, pointerPosition, InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "SearchCallSource", false, false);
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x000C6614 File Offset: 0x000C4814
	public void SearchCallSource(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnapperRouter.node);
		Vector3 pointerPosition = Vector3.zero;
		try
		{
			if (this.kidnapper.home != null)
			{
				TelephoneController.PhoneCall phoneCall = this.kidnapper.home.building.callLog.Find((TelephoneController.PhoneCall item) => (item.toNS != null && item.fromNS != null && item.toNS.interactable.node.gameLocation == this.kidnapper.home && MapController.Instance.playerRoute.end.gameLocation.building == item.fromNS.interactable.node.gameLocation.building) || (item.fromNS != null && item.toNS != null && item.fromNS.interactable.node.gameLocation == this.kidnapper.home && MapController.Instance.playerRoute.end.gameLocation.building == item.toNS.interactable.node.gameLocation.building));
				if (phoneCall != null)
				{
					if (phoneCall.toNS.interactable.node.gameLocation == this.kidnapper.home)
					{
						this.chosenRouterAddress = phoneCall.fromNS.interactable.node.gameLocation.thisAsAddress;
					}
					else if (phoneCall.fromNS.interactable.node.gameLocation == this.kidnapper.home)
					{
						this.chosenRouterAddress = phoneCall.toNS.interactable.node.gameLocation.thisAsAddress;
					}
					if (this.chosenRouterAddress != null)
					{
						if (this.chosenRouterAddress.building != Player.Instance.currentBuilding)
						{
							float num;
							Interactable interactable = Toolbox.Instance.FindClosestObjectTo(InteriorControls.Instance.telephoneRouter, Player.Instance.transform.position, this.chosenRouterAddress.building, null, null, out num, false);
							if (interactable != null)
							{
								pointerPosition = interactable.GetWorldPosition(true);
								Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, interactable.evidence, null, null, null, null, "", false, default(Vector3));
								this.AddObjective("Access other cabinet", trigger, true, pointerPosition, InterfaceControls.Icon.hand, Objective.OnCompleteAction.specificChapterByString, 0f, false, "AccessOtherAddress", false, false);
							}
						}
						else
						{
							ChapterController.Instance.LoadPart("SearchOtherAddress");
						}
					}
				}
			}
			else if (Game.Instance.collectDebugData)
			{
				Game.Log("Chapter: The kidnapper no longer features a home...", 2);
			}
		}
		catch
		{
			if (Game.Instance.collectDebugData)
			{
				Game.Log("Chapter: Unable to load call source for chapter", 2);
			}
		}
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x000C6830 File Offset: 0x000C4A30
	public void AccessOtherAddress(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnapperRouter.node);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.openEvidence, "", false, 0f, null, null, this.chosenRouterAddress.evidenceEntry, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Find other call", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "SearchOtherAddress", false, false);
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x000C68A8 File Offset: 0x000C4AA8
	public void SearchOtherAddress(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnapperRouter.node);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.chosenRouterAddress, null, "", false, default(Vector3));
		this.AddObjective("Go to other address", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "SearchFail", false, false);
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x000C691C File Offset: 0x000C4B1C
	public void SearchFail(int passedVar)
	{
		this.SetCurrentPartLocation(this.kidnapperRouter.node);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.escapeGameLocation, "", false, 0f, null, null, null, null, null, this.chosenRouterAddress, null, "", false, default(Vector3));
		this.AddObjective("Search other address", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "DisplayLeads", false, false);
		Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.escapeGameLocation, "", false, 0f, null, null, null, null, null, this.chosenRouterAddress, null, "", false, default(Vector3));
		this.AddObjective("Dud lead", trigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "DudLead", true, false);
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x000C69E0 File Offset: 0x000C4BE0
	private void DudLead()
	{
		PopupMessageController.Instance.PopupMessage("dud_lead", true, false, "Cancel", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x000C6A30 File Offset: 0x000C4C30
	public void SearchWeaponsDealer(int passedVar)
	{
		NewRoom newRoom = this.weaponSeller.rooms.Find((NewRoom item) => item.entrances.Count > 0);
		this.SetCurrentPartLocation(newRoom.entrances[0].fromNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.inspectInteractable, "", false, 0f, null, this.weaponsSalesLedger, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Search Weapons Dealer", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "FoundKillerID", false, false);
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x000C6AD8 File Offset: 0x000C4CD8
	public void FoundKillerID(int passedVar)
	{
		NewRoom newRoom = this.killer.home.rooms.Find((NewRoom item) => item.entrances.Count > 0);
		this.SetCurrentPartLocation(newRoom.entrances[0].fromNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.killer.home, null, "", false, default(Vector3));
		this.AddObjective("Find Killer", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.specificChapterByString, 0f, false, "ProveKiller", false, false);
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x000C6B8C File Offset: 0x000C4D8C
	public void ProveKiller(int passedVar)
	{
		NewRoom newRoom = this.killer.home.rooms.Find((NewRoom item) => item.entrances.Count > 0);
		this.SetCurrentPartLocation(newRoom.entrances[0].fromNode);
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.findFingerprintsAtLocation, "", false, 0f, null, null, this.killer.evidenceEntry, null, null, this.killer.home, null, "", false, default(Vector3));
		this.AddObjective("Find Proof", trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x000C6C48 File Offset: 0x000C4E48
	public void ReturnHome(int passedVar)
	{
		this.SetCurrentPartLocation(this.apartmentEntrance.door.wall.node);
		base.ClearAllObjectives();
		this.apartmentEntrance.door.SetLocked(false, null, false);
		this.apartmentEntrance.door.OpenByActor(null, false, 1f);
		this.apartmentEntrance.door.SetForbidden(true);
		if (Player.Instance.residence != null && Player.Instance.residence.address != null && Player.Instance.residence.address.id == this.playersAparment)
		{
			Player.Instance.residence.address.RemoveAllInhabitantFurniture(false, FurnitureClusterLocation.RemoveInteractablesOption.remove);
		}
		if (this.evictionNotice == null)
		{
			Vector3 worldPos = this.apartmentEntrance.door.transform.TransformPoint(new Vector3(-1.2f, 1.6f, -0.2f));
			Vector3 worldEuler = this.apartmentEntrance.door.transform.eulerAngles + new Vector3(-90f, 0f, 0f);
			this.evictionNotice = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.letter, null, null, null, worldPos, worldEuler, null, null, "fdf6930b-0c20-434f-a181-dd4975944331");
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToRoom, "", false, 0f, this.exteriorDoorNode.room, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Return to Apartment", trigger, true, this.apartmentEntrance.worldAccessPoint + new Vector3(0f, 1f, 0f), InterfaceControls.Icon.run, Objective.OnCompleteAction.specificChapterByString, 6f, false, "VistSlophouseOwner", false, false);
		MapController.Instance.PlotPlayerRoute(this.exteriorDoorNode, false, null);
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x000C6E1C File Offset: 0x000C501C
	public void VistSlophouseOwner(int passedVar)
	{
		this.SetCurrentPartLocation(this.apartmentEntrance.door.wall.node);
		this.apartment.inhabitants.Clear();
		this.apartment.owners.Clear();
		Player.Instance.SetResidence(null, true);
		this.apartmentEntrance.door.SetLocked(false, null, false);
		this.apartmentEntrance.door.OpenByActor(null, false, 1f);
		this.apartmentEntrance.door.SetForbidden(true);
		foreach (Human human in this.slophouseOwner.home.inhabitants)
		{
			human.evidenceEntry.AddDialogOption(Evidence.DataKey.name, ChapterController.Instance.loadedChapter.dialogEvents[2], null, null, false);
			(human as Citizen).alwaysPassDialogSuccess = true;
		}
		if (this.slophouseOwner != null)
		{
			(this.slophouseOwner as Citizen).alwaysPassDialogSuccess = true;
		}
		this.PlayerVO("9112f9a1-d46d-4167-954d-a3ec74670b15", 0f, true, false, false, false, default(Color));
		this.PlayerVO("9c738588-19b2-495c-9968-e6458ff5ecb6", 0f, true, false, false, false, default(Color));
		this.PlayerVO("70e61f16-1db2-4269-a53f-11dba118bcac", 0f, true, false, false, false, default(Color));
		this.PlayerVO("2919fe0b-38da-4680-9705-1d4440e76273", 0f, true, false, false, false, default(Color));
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.onDialogSuccess, ChapterController.Instance.loadedChapter.dialogEvents[2].name, false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("Find and talk to |story.slophouseowner.name| about accommodation", trigger, false, default(Vector3), InterfaceControls.Icon.citizen, Objective.OnCompleteAction.specificChapterByString, 0f, false, "GoToSlophouse", false, false);
		Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.playerActionNobodyHome, "KnockOnDoor", false, 0f, null, null, null, null, null, this.slophouseOwner.home, null, "", false, default(Vector3));
		this.AddObjective("Nobody home", trigger2, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.invokeFunction, 0f, false, "NobodyHome", true, false);
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x000C7078 File Offset: 0x000C5278
	private void NobodyHome()
	{
		InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "nobody_home", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.questionMark, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x000C70C0 File Offset: 0x000C52C0
	public void GoToSlophouse(int passedVar)
	{
		NewRoom newRoom = this.slophouse.rooms.Find((NewRoom item) => item.entrances.Count > 0 && !item.isNullRoom);
		this.SetCurrentPartLocation(newRoom.entrances[0].fromNode);
		base.ClearAllObjectives();
		if (this.evictionNotice != null)
		{
			this.evictionNotice.MarkAsTrash(true, false, 0f);
			this.evictionNotice.RemoveFromPlacement();
		}
		foreach (Human human in this.slophouseOwner.home.inhabitants)
		{
			human.evidenceEntry.RemoveDialogOption(Evidence.DataKey.name, ChapterController.Instance.loadedChapter.dialogEvents[2], null, null);
			(human as Citizen).alwaysPassDialogSuccess = false;
		}
		if (Player.Instance.home != this.slophouse)
		{
			if (Game.Instance.collectDebugData)
			{
				Game.Log(string.Concat(new string[]
				{
					"Chapter: Setting flophouse ",
					this.slophouse.name,
					" ",
					this.slophouse.id.ToString(),
					" as player home..."
				}), 2);
			}
			PlayerApartmentController.Instance.BuyNewResidence(this.slophouse.residence, true);
			PlayerApartmentController.Instance.PlaceIndividualCluster(InteriorControls.Instance.bedCluster, this.slophouse);
			PlayerApartmentController.Instance.PlaceIndividualCluster(InteriorControls.Instance.noticeBoardCluster, this.slophouse);
		}
		if (this.flophouseWelcomeLetter == null)
		{
			FurnitureLocation furnitureLocation;
			this.flophouseWelcomeLetter = this.slophouse.PlaceObject(InteriorControls.Instance.note, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.prioritiseNonOwned, 0, null, false, null, null, null, "a35d5d04-fcb3-4302-902b-4e9c10bb101a", false);
		}
		if (this.flophouseSyncDiskNote == null)
		{
			FurnitureLocation furnitureLocation;
			this.flophouseSyncDiskNote = this.slophouse.PlaceObject(InteriorControls.Instance.note, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.prioritiseNonOwned, 0, null, false, null, null, null, "97523ad4-eac0-49c1-a21f-006cf06bd5b2", false);
		}
		if (this.flophouseJobNote == null)
		{
			FurnitureLocation furnitureLocation;
			this.flophouseJobNote = this.slophouse.PlaceObject(InteriorControls.Instance.note, Player.Instance, Player.Instance, null, out furnitureLocation, null, false, 0, InteractablePreset.OwnedPlacementRule.prioritiseNonOwned, 0, null, false, null, null, null, "0ab4cf29-d643-438f-832c-5443d8667f6f", false);
		}
		if (this.slophouse != null && this.slophouse.residence != null && this.slophouse.residence.mailbox != null && this.flophouseSyncDisk == null)
		{
			Interactable mailbox = Toolbox.Instance.GetMailbox(Player.Instance);
			if (mailbox != null)
			{
				FurnitureLocation furnitureLocation;
				this.flophouseSyncDisk = mailbox.node.gameLocation.PlaceObject(InteriorControls.Instance.chapterFlophouseSyncDisk.interactable, Player.Instance, Player.Instance, Player.Instance, out furnitureLocation, null, true, 0, InteractablePreset.OwnedPlacementRule.ownedOnly, 3, InteriorControls.Instance.chapterFlophouseSyncDisk, false, null, null, null, "", true);
				if (this.flophouseSyncDisk == null && Game.Instance.collectDebugData)
				{
					Game.Log("Chapter: Unable to place sync disk", 2);
				}
			}
			else if (Game.Instance.collectDebugData)
			{
				Game.Log("Chapter: Unable to get mailbox...", 2);
			}
		}
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToAddress, "", false, 0f, null, null, null, null, null, this.slophouse, null, "", false, default(Vector3));
		this.AddObjective("Visit |story.flophouse.name|", trigger, false, default(Vector3), InterfaceControls.Icon.building, Objective.OnCompleteAction.specificChapterByString, 0f, false, "ArrivedAtSlophouse", false, false);
		MapController.Instance.PlotPlayerRoute(this.slophouse);
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x000C747C File Offset: 0x000C567C
	public void ArrivedAtSlophouse(int passedVar)
	{
		NewRoom newRoom = this.slophouse.rooms.Find((NewRoom item) => item.entrances.Count > 0 && !item.isNullRoom);
		this.SetCurrentPartLocation(newRoom.entrances[0].fromNode);
		this.PlayerVO("da76dfd0-e337-4305-9afc-61b79291aa62", 0f, true, false, false, false, default(Color));
		FurnitureLocation furnitureLocation = null;
		foreach (NewRoom newRoom2 in this.slophouse.rooms)
		{
			furnitureLocation = newRoom2.individualFurniture.Find((FurnitureLocation item) => item.furniture.name == "PublicCorkboard");
			if (furnitureLocation != null)
			{
				break;
			}
		}
		if (furnitureLocation != null)
		{
			Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.goToNode, "", false, 0f, null, null, null, furnitureLocation.anchorNode, null, null, null, "", false, default(Vector3));
			this.AddObjective("Check notice board", trigger, true, furnitureLocation.anchorNode.position + new Vector3(0f, 1.5f, 0f), InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.specificChapterByString, 0f, false, "End", false, false);
			return;
		}
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: Unable to find corkboard in flophouse...", 2);
		}
		ChapterController.Instance.SkipToNextPart();
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x00002265 File Offset: 0x00000465
	public void CancelLeads(int passedVar)
	{
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x000C75F8 File Offset: 0x000C57F8
	public void End(int passedVar)
	{
		this.PlayerVO("c0c6baab-3617-4708-9281-9868740bc684", 0f, true, false, false, false, default(Color));
		Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.nothing, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
		this.AddObjective("You can change the decor of your apartment using the inventory menu (|controls.weaponselect|) (optional)", trigger, false, default(Vector3), InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
		this.AddObjective("Work towards retirement by advancing social ranks (optional)", trigger, false, default(Vector3), InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
		this.AddObjective("You can close this case when ready by using the resolve button", trigger, false, default(Vector3), InterfaceControls.Icon.questionMark, Objective.OnCompleteAction.nextChapterPart, 0f, false, "", false, false);
		if (Game.Instance.collectDebugData)
		{
			Game.Log("Chapter: End has been reached!", 2);
		}
		if (AchievementsController.Instance != null)
		{
			AchievementsController.Instance.UnlockAchievement("Rise & Shine", "complete_story");
		}
		if (this.noteWriter != null)
		{
			(this.noteWriter as Citizen).alwaysPassDialogSuccess = false;
		}
		if (this.slophouseOwner != null)
		{
			(this.slophouseOwner as Citizen).alwaysPassDialogSuccess = false;
		}
		Game.Instance.sandboxMode = true;
		if (this.thisCase != null)
		{
			this.thisCase.SetStatus(Case.CaseStatus.closable, false);
		}
		if (!this.completed)
		{
			this.completed = true;
			MurderController.Instance.currentActiveCase = null;
			this.killer.RemoveFromWorld(true);
			if (this.note != null)
			{
				this.note.MarkAsTrash(true, false, 0f);
				this.note.RemoveManuallyCreatedFingerprints();
			}
			for (int i = 0; i < MurderController.Instance.activeMurders.Count; i++)
			{
				MurderController.Murder murder = MurderController.Instance.activeMurders[i];
				murder.CancelCurrentMurder();
				murder.murderer.RemoveFromWorld(true);
				murder.SetMurderState(MurderController.MurderState.solved, true);
				i--;
			}
			foreach (Interactable interactable in this.restaurant.securityCameras)
			{
				foreach (SceneRecorder.SceneCapture sceneCapture in interactable.cap)
				{
					sceneCapture.k = false;
				}
			}
			if (this.kidnapper != null && this.kidnapper.home != null && this.kidnapper.home.inhabitants != null)
			{
				foreach (Human human in this.kidnapper.home.inhabitants)
				{
					human.ai.RemoveAvoidLocation(this.kidnapper.home);
				}
			}
			this.noteWriter.ai.SetConfineLocation(null);
			if (this.noteWriter != null && this.noteWriter.home != null && this.noteWriter.home.inhabitants != null)
			{
				foreach (Human human2 in this.noteWriter.home.inhabitants)
				{
					human2.ai.RemoveAvoidLocation(this.noteWriter.home);
				}
			}
			MurderController.Instance.SetProcGenKillerLoop(true);
		}
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x000C79A4 File Offset: 0x000C5BA4
	public void NotewriterLayLow()
	{
		if (this.layLowGoal == null)
		{
			this.noteWriter.ai.SetConfineLocation(null);
			(this.noteWriter as Citizen).alwaysPassDialogSuccess = false;
			this.findNotewriter = true;
			if (Game.Instance.collectDebugData)
			{
				Game.Log("Chapter: The notewriter will lay low!", 2);
			}
			this.layLowGoal = this.noteWriter.ai.CreateNewGoal(RoutineControls.Instance.layLow, 0f, 0f, null, null, this.restaurant, null, null, -2);
			for (int i = 0; i < MurderController.Instance.activeMurders.Count; i++)
			{
				MurderController.Instance.activeMurders[i].CancelCurrentMurder();
			}
			base.Invoke("NotewritersLeads", 35f);
		}
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x000C7A74 File Offset: 0x000C5C74
	private void NotewritersLeads()
	{
		SessionData.Instance.PauseGame(true, false, true);
		InterfaceController.Instance.SpawnWindow(this.killer.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
		InterfaceController.Instance.SpawnWindow(this.killerBar.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
		GameplayController.Instance.AddMoney(200, true, "warn notewriter");
		ChapterController.Instance.LoadPart("FoundKillerID");
		if (this.rewardSyncDisk != null)
		{
			this.rewardSyncDisk.SetOwner(Player.Instance, true);
			Interactable mailbox = Toolbox.Instance.GetMailbox(this.noteWriter);
			if (mailbox != null)
			{
				mailbox.SetLockedState(false, null, false, true);
				Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.itemInInventory, "", false, 0f, null, this.rewardSyncDisk, null, null, null, null, null, "", false, default(Vector3));
				this.AddObjective("Pick up the Sync Disk in the mailbox", trigger, true, this.rewardSyncDisk.GetWorldPosition(true), InterfaceControls.Icon.star, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
			}
			foreach (Company company in CityData.Instance.companyDirectory)
			{
				if (company.preset.name == "SyncClinic" && company.address != null)
				{
					bool flag = false;
					using (List<NewRoom>.Enumerator enumerator2 = company.address.rooms.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							NewRoom newRoom = enumerator2.Current;
							foreach (NewNode newNode in newRoom.nodes)
							{
								Interactable interactable = newNode.interactables.Find((Interactable item) => item.preset.specialCaseFlag == InteractablePreset.SpecialCase.syncBed);
								if (interactable != null)
								{
									Vector3 worldPosition = interactable.GetWorldPosition(true);
									Objective.ObjectiveTrigger trigger2 = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.syncDiskInstallTutorial, "", false, 0f, null, null, null, null, null, null, null, "", false, default(Vector3));
									this.AddObjective("Install sync disk", trigger2, true, worldPosition, InterfaceControls.Icon.hand, Objective.OnCompleteAction.nothing, 0f, false, "", false, false);
									flag = true;
									break;
								}
							}
							if (flag)
							{
								break;
							}
						}
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x000C7D50 File Offset: 0x000C5F50
	[Button(null, 0)]
	public void ManualTriggerNotewriterMurder()
	{
		this.notewriterManualMurderTrigger = true;
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x000C7D59 File Offset: 0x000C5F59
	[Button(null, 0)]
	public void SkipPreSim()
	{
		this.murderPreSimPass = true;
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x000C7D62 File Offset: 0x000C5F62
	[Button(null, 0)]
	public void TriggerNotewriterLeads()
	{
		this.NotewritersLeads();
	}

	// Token: 0x04000F9B RID: 3995
	[Header("Character")]
	public Human noteWriter;

	// Token: 0x04000F9C RID: 3996
	public Human kidnapper;

	// Token: 0x04000F9D RID: 3997
	public Human killer;

	// Token: 0x04000F9E RID: 3998
	public Human slophouseOwner;

	// Token: 0x04000F9F RID: 3999
	[Header("Presim")]
	private GroupsController.SocialGroup meetGroup;

	// Token: 0x04000FA0 RID: 4000
	private NewAIGoal meetFoodNotewriter;

	// Token: 0x04000FA1 RID: 4001
	private NewAIGoal meetFoodKidnapper;

	// Token: 0x04000FA2 RID: 4002
	private NewAIGoal postNote;

	// Token: 0x04000FA3 RID: 4003
	private NewAIGoal kidnapperGoHome;

	// Token: 0x04000FA4 RID: 4004
	private NewAIGoal kidnapperRunAway;

	// Token: 0x04000FA5 RID: 4005
	private NewAIGoal returnToApartment;

	// Token: 0x04000FA6 RID: 4006
	private MurderController.Murder murder;

	// Token: 0x04000FA7 RID: 4007
	private MurderController.Murder murder2;

	// Token: 0x04000FA8 RID: 4008
	private bool handlePreSim;

	// Token: 0x04000FA9 RID: 4009
	private bool murderPreSimPass;

	// Token: 0x04000FAA RID: 4010
	public int preSimPhase;

	// Token: 0x04000FAB RID: 4011
	public float timeSinceCallObjective;

	// Token: 0x04000FAC RID: 4012
	[Header("Saved variables")]
	public int noteWriterID = -1;

	// Token: 0x04000FAD RID: 4013
	public int kidnapperID = -1;

	// Token: 0x04000FAE RID: 4014
	public int killerID = -1;

	// Token: 0x04000FAF RID: 4015
	public int playersAparment = -1;

	// Token: 0x04000FB0 RID: 4016
	public int eatery = -1;

	// Token: 0x04000FB1 RID: 4017
	public int slopHouseOwnerID = -1;

	// Token: 0x04000FB2 RID: 4018
	public int slopHouseID = -1;

	// Token: 0x04000FB3 RID: 4019
	public int addressBookID = -1;

	// Token: 0x04000FB4 RID: 4020
	public float meetTime = -1f;

	// Token: 0x04000FB5 RID: 4021
	public bool enforcerEventsTrigger;

	// Token: 0x04000FB6 RID: 4022
	public bool findNotewriter;

	// Token: 0x04000FB7 RID: 4023
	public bool notewriterDialogAdded;

	// Token: 0x04000FB8 RID: 4024
	public bool lastCallPlaced;

	// Token: 0x04000FB9 RID: 4025
	public float notewriterMurderTimer = -1f;

	// Token: 0x04000FBA RID: 4026
	public bool notewriterManualMurderTrigger;

	// Token: 0x04000FBB RID: 4027
	public bool notewriterMurderTriggered;

	// Token: 0x04000FBC RID: 4028
	public bool receiptSearchPromt;

	// Token: 0x04000FBD RID: 4029
	public bool addressBookSearchPrompt;

	// Token: 0x04000FBE RID: 4030
	public bool fingerprintPrompt;

	// Token: 0x04000FBF RID: 4031
	public float receiptSearchTimer;

	// Token: 0x04000FC0 RID: 4032
	public float addressBookSearchTimer;

	// Token: 0x04000FC1 RID: 4033
	public float printSearchTimer;

	// Token: 0x04000FC2 RID: 4034
	public bool receiptSearchActivated;

	// Token: 0x04000FC3 RID: 4035
	public bool addressBookSearchActivated;

	// Token: 0x04000FC4 RID: 4036
	public bool printSearchActivated;

	// Token: 0x04000FC5 RID: 4037
	public int killerBarID = -1;

	// Token: 0x04000FC6 RID: 4038
	public int redGumMeetID = -1;

	// Token: 0x04000FC7 RID: 4039
	public int chosenRouterAddressID = -1;

	// Token: 0x04000FC8 RID: 4040
	public int weaponSellerID = -1;

	// Token: 0x04000FC9 RID: 4041
	public bool discoveredWeaponsDealer;

	// Token: 0x04000FCA RID: 4042
	public bool completed;

	// Token: 0x04000FCB RID: 4043
	[Header("Locations")]
	public NewAddress apartment;

	// Token: 0x04000FCC RID: 4044
	private NewRoom playerBedroom;

	// Token: 0x04000FCD RID: 4045
	private NewRoom playerLounge;

	// Token: 0x04000FCE RID: 4046
	private NewRoom playerKitchen;

	// Token: 0x04000FCF RID: 4047
	private NewRoom kidnappersBedroom;

	// Token: 0x04000FD0 RID: 4048
	private NewRoom noteWritersBedroom;

	// Token: 0x04000FD1 RID: 4049
	public NewAddress restaurant;

	// Token: 0x04000FD2 RID: 4050
	private NewRoom restaurantBackroom;

	// Token: 0x04000FD3 RID: 4051
	public NewAddress killerBar;

	// Token: 0x04000FD4 RID: 4052
	public NewAddress redGumMeet;

	// Token: 0x04000FD5 RID: 4053
	public NewAddress chosenRouterAddress;

	// Token: 0x04000FD6 RID: 4054
	public NewAddress weaponSeller;

	// Token: 0x04000FD7 RID: 4055
	public NewAddress slophouse;

	// Token: 0x04000FD8 RID: 4056
	[Header("Objects: Spawned")]
	private Interactable note;

	// Token: 0x04000FD9 RID: 4057
	private Interactable key;

	// Token: 0x04000FDA RID: 4058
	private Interactable detectiveStuff;

	// Token: 0x04000FDB RID: 4059
	private Interactable policeBadge;

	// Token: 0x04000FDC RID: 4060
	private Interactable hairpin;

	// Token: 0x04000FDD RID: 4061
	private Interactable paperclip;

	// Token: 0x04000FDE RID: 4062
	private Interactable spareKeyDoormat;

	// Token: 0x04000FDF RID: 4063
	private Interactable workID;

	// Token: 0x04000FE0 RID: 4064
	private Interactable safePasscode;

	// Token: 0x04000FE1 RID: 4065
	private Interactable rewardSyncDisk;

	// Token: 0x04000FE2 RID: 4066
	[NonSerialized]
	public Interactable murderWeapon;

	// Token: 0x04000FE3 RID: 4067
	private Interactable kidnapperDiary;

	// Token: 0x04000FE4 RID: 4068
	private Interactable envelopeWithCredits;

	// Token: 0x04000FE5 RID: 4069
	private Interactable corpLetter;

	// Token: 0x04000FE6 RID: 4070
	private Interactable crumpledFlyer;

	// Token: 0x04000FE7 RID: 4071
	private Interactable printedVmail;

	// Token: 0x04000FE8 RID: 4072
	private Interactable meetingNote;

	// Token: 0x04000FE9 RID: 4073
	private Interactable noteOnNapkin;

	// Token: 0x04000FEA RID: 4074
	private Interactable tornPhotograph;

	// Token: 0x04000FEB RID: 4075
	private Interactable travelreceipt;

	// Token: 0x04000FEC RID: 4076
	private List<Interactable> playerApartmentLockpicks = new List<Interactable>();

	// Token: 0x04000FED RID: 4077
	[NonSerialized]
	public Evidence restaurantReceipt;

	// Token: 0x04000FEE RID: 4078
	public bool receiptInBin;

	// Token: 0x04000FEF RID: 4079
	private Interactable noteWriterDiary;

	// Token: 0x04000FF0 RID: 4080
	public Interactable playersStorageBox;

	// Token: 0x04000FF1 RID: 4081
	private Interactable policeCertificate;

	// Token: 0x04000FF2 RID: 4082
	private Interactable fieldsAdvert;

	// Token: 0x04000FF3 RID: 4083
	private Interactable scientificPaper;

	// Token: 0x04000FF4 RID: 4084
	private Interactable playersPasscodeReminder;

	// Token: 0x04000FF5 RID: 4085
	private Interactable killerPropaganda;

	// Token: 0x04000FF6 RID: 4086
	private Interactable killerNotewriterDetails;

	// Token: 0x04000FF7 RID: 4087
	private Interactable killerPoliceFines;

	// Token: 0x04000FF8 RID: 4088
	private Interactable killerBusinessCard;

	// Token: 0x04000FF9 RID: 4089
	private Interactable killerCorpSponsorship;

	// Token: 0x04000FFA RID: 4090
	private Interactable killerBarTab;

	// Token: 0x04000FFB RID: 4091
	private Interactable robItem;

	// Token: 0x04000FFC RID: 4092
	private Interactable workplaceReceipt;

	// Token: 0x04000FFD RID: 4093
	private Interactable workplaceMessageNote;

	// Token: 0x04000FFE RID: 4094
	private Interactable dinerFlyer;

	// Token: 0x04000FFF RID: 4095
	private Interactable finalNoticeBill;

	// Token: 0x04001000 RID: 4096
	private Interactable evictionNotice;

	// Token: 0x04001001 RID: 4097
	private Interactable flophouseWelcomeLetter;

	// Token: 0x04001002 RID: 4098
	private Interactable flophouseSyncDiskNote;

	// Token: 0x04001003 RID: 4099
	private Interactable flophouseJobNote;

	// Token: 0x04001004 RID: 4100
	private Interactable flophouseSyncDisk;

	// Token: 0x04001005 RID: 4101
	[Header("Objects: Reference")]
	private FurnitureLocation kidnappersSafe;

	// Token: 0x04001006 RID: 4102
	private FurnitureLocation bed;

	// Token: 0x04001007 RID: 4103
	private Interactable closestSleep;

	// Token: 0x04001008 RID: 4104
	private Interactable closestLight;

	// Token: 0x04001009 RID: 4105
	private NewNode.NodeAccess apartmentEntrance;

	// Token: 0x0400100A RID: 4106
	private NewNode interiorDoorNode;

	// Token: 0x0400100B RID: 4107
	private NewNode exteriorDoorNode;

	// Token: 0x0400100C RID: 4108
	private Interactable playerCalendar;

	// Token: 0x0400100D RID: 4109
	private Interactable cityDir;

	// Token: 0x0400100E RID: 4110
	private FurnitureLocation noteWritersBed;

	// Token: 0x0400100F RID: 4111
	private Interactable dinerCruncher;

	// Token: 0x04001010 RID: 4112
	private NewNode.NodeAccess kidnappersEntrance;

	// Token: 0x04001011 RID: 4113
	private NewNode kidnappersDoorNode;

	// Token: 0x04001012 RID: 4114
	private NewDoor kidnappersDoor;

	// Token: 0x04001013 RID: 4115
	private Interactable kidnappersCalendar;

	// Token: 0x04001014 RID: 4116
	private Interactable kidnappersAddressBook;

	// Token: 0x04001015 RID: 4117
	private Interactable kidnapperBin;

	// Token: 0x04001016 RID: 4118
	private Interactable kidnapperPhone;

	// Token: 0x04001017 RID: 4119
	private Interactable weaponsSalesLedger;

	// Token: 0x04001018 RID: 4120
	private Interactable kidnapperRouter;

	// Token: 0x04001019 RID: 4121
	private Interactable kidnapperRouterDoor;

	// Token: 0x0400101A RID: 4122
	private EvidenceTime meetingTimeEvidence;

	// Token: 0x0400101B RID: 4123
	[NonSerialized]
	public NewAIGoal layLowGoal;

	// Token: 0x0400101C RID: 4124
	private RectTransform pointer;

	// Token: 0x0400101D RID: 4125
	private PulseGlowController glow;

	// Token: 0x0400101E RID: 4126
	private float nextLeadDelay;

	// Token: 0x0400101F RID: 4127
	private bool notewriterOnCam;

	// Token: 0x04001020 RID: 4128
	private bool kidnapperOnCam;

	// Token: 0x04001021 RID: 4129
	public int lockpicksNeeded;

	// Token: 0x04001022 RID: 4130
	private float endDelayTimer;

	// Token: 0x04001023 RID: 4131
	private float passcodeNoteTimer;

	// Token: 0x04001024 RID: 4132
	private bool triggeredPasscodeNoteHint;

	// Token: 0x04001025 RID: 4133
	private float triggeredTutorialSkip;

	// Token: 0x0200025E RID: 606
	public class IntoCharacterPick
	{
		// Token: 0x04001026 RID: 4134
		public Human noteWriter;

		// Token: 0x04001027 RID: 4135
		public Human kidnapper;

		// Token: 0x04001028 RID: 4136
		public float score;
	}
}
