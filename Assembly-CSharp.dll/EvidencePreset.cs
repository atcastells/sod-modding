using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000712 RID: 1810
[CreateAssetMenu(fileName = "evidence_data", menuName = "Database/Evidence/Evidence Preset")]
public class EvidencePreset : SoCustomComparison
{
	// Token: 0x0600253E RID: 9534 RVA: 0x001E5740 File Offset: 0x001E3940
	public List<Evidence.DataKey> GetValidProfileKeys()
	{
		List<Evidence.DataKey> list = new List<Evidence.DataKey>();
		foreach (DataKeyControls.DataKeySettings dataKeySettings in this.validKeys)
		{
			if (dataKeySettings.countTowardsProfile)
			{
				list.Add(dataKeySettings.key);
			}
		}
		return list;
	}

	// Token: 0x0600253F RID: 9535 RVA: 0x001E57A8 File Offset: 0x001E39A8
	public List<Evidence.DataKey> GetUniqueProfileKeys()
	{
		List<Evidence.DataKey> list = new List<Evidence.DataKey>();
		foreach (DataKeyControls.DataKeySettings dataKeySettings in this.validKeys)
		{
			if (dataKeySettings.uniqueKey)
			{
				list.Add(dataKeySettings.key);
			}
		}
		return list;
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x001E5810 File Offset: 0x001E3A10
	public bool IsKeyValid(Evidence.DataKey key, out bool countTowardsProfile)
	{
		DataKeyControls.DataKeySettings dataKeySettings = this.validKeys.Find((DataKeyControls.DataKeySettings item) => item.key == key);
		countTowardsProfile = false;
		if (dataKeySettings != null)
		{
			countTowardsProfile = dataKeySettings.countTowardsProfile;
			return true;
		}
		return false;
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x001E5854 File Offset: 0x001E3A54
	public bool IsKeyUnique(Evidence.DataKey key)
	{
		if (!this.useDataKeys)
		{
			return true;
		}
		DataKeyControls.DataKeySettings dataKeySettings = this.validKeys.Find((DataKeyControls.DataKeySettings item) => item.key == key);
		return dataKeySettings != null && dataKeySettings.uniqueKey;
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x001E589C File Offset: 0x001E3A9C
	public int GetProfileKeyCount(List<Evidence.DataKey> keyList)
	{
		int num = 0;
		foreach (Evidence.DataKey key in keyList)
		{
			bool flag = false;
			if (this.IsKeyValid(key, out flag) && flag)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x04003443 RID: 13379
	[Tooltip("Spawn this subclass. If left empty it will use the base class.")]
	[Header("Setup")]
	public string subClass = string.Empty;

	// Token: 0x04003444 RID: 13380
	[Tooltip("The window style this evidence should use.")]
	public WindowStylePreset windowStyle;

	// Token: 0x04003445 RID: 13381
	[Tooltip("Should this evidence use data key instances? If false, all keys will be tied together on creation.")]
	public bool useDataKeys;

	// Token: 0x04003446 RID: 13382
	[EnableIf("useDataKeys")]
	public List<DataKeyControls.DataKeySettings> validKeys = new List<DataKeyControls.DataKeySettings>();

	// Token: 0x04003447 RID: 13383
	[Tooltip("The below keys act as if merged when retrieved")]
	[EnableIf("useDataKeys")]
	public List<EvidencePreset.DataKeyAutomaticTies> passiveTies = new List<EvidencePreset.DataKeyAutomaticTies>();

	// Token: 0x04003448 RID: 13384
	[EnableIf("useDataKeys")]
	public bool notifyOfTies;

	// Token: 0x04003449 RID: 13385
	[Tooltip("Item Evidence class only: Should the person who this belongs to be featured in the name?")]
	public bool useBelongsToInName;

	// Token: 0x0400344A RID: 13386
	[Tooltip("Does only one instance of this evidence exist?")]
	public bool isSingleton;

	// Token: 0x0400344B RID: 13387
	[Tooltip("If true this does appear in history when inspected")]
	public bool disableHistory;

	// Token: 0x0400344C RID: 13388
	[Tooltip("Allow this evidence to be given custom names")]
	public bool allowCustomNames = true;

	// Token: 0x0400344D RID: 13389
	[Tooltip("If true this will be marked as discovered on any interaction, as opposed to just world interactions")]
	public bool markAsDiscoveredOnAnyInteraction;

	// Token: 0x0400344E RID: 13390
	[Tooltip("If true this will always and only be able to be a world interaction")]
	public bool forceWorldInteraction;

	// Token: 0x0400344F RID: 13391
	[Tooltip("Use window focus mode (black screen behind the window)")]
	public bool useWindowFocusMode;

	// Token: 0x04003450 RID: 13392
	[Header("Graphics")]
	[Tooltip("The icon for this evidence")]
	public Sprite iconSpriteLarge;

	// Token: 0x04003451 RID: 13393
	public Texture2D defaultNullImage;

	// Token: 0x04003452 RID: 13394
	[Tooltip("Take in-game shot of this item for use in evidence (only used if photo key present)")]
	public bool useInGamePhoto;

	// Token: 0x04003453 RID: 13395
	[Tooltip("Instead of 'this' use a photo of the person this belongs to")]
	public bool useWriter;

	// Token: 0x04003454 RID: 13396
	[EnableIf("useInGamePhoto")]
	public Vector3 relativeCamPhotoPos = Vector3.zero;

	// Token: 0x04003455 RID: 13397
	[EnableIf("useInGamePhoto")]
	public Vector3 relativeCamPhotoEuler = Vector3.zero;

	// Token: 0x04003456 RID: 13398
	[EnableIf("useInGamePhoto")]
	public EvidencePreset.CaptureRules captureRules;

	// Token: 0x04003457 RID: 13399
	[EnableIf("useInGamePhoto")]
	public bool changeTimeOfDay;

	// Token: 0x04003458 RID: 13400
	[EnableIf("useInGamePhoto")]
	public float captureTimeOfDay = 12f;

	// Token: 0x04003459 RID: 13401
	public bool useCaptureLight = true;

	// Token: 0x0400345A RID: 13402
	[Tooltip("Use image from a CCTV capture")]
	[DisableIf("useInGamePhoto")]
	public bool useSurveillanceCapture;

	// Token: 0x0400345B RID: 13403
	[BoxGroup("Facts")]
	[Tooltip("Item evidence only: The 'belongs to' reference is set to this relation.")]
	public EvidencePreset.BelongsToSetting itemOwner;

	// Token: 0x0400345C RID: 13404
	[BoxGroup("Facts")]
	[Tooltip("Item evidence only: The 'belongs to' reference is set to this relation.")]
	public EvidencePreset.BelongsToSetting itemWriter;

	// Token: 0x0400345D RID: 13405
	[Tooltip("Item evidence only: The 'subject' reference is set to this relation.")]
	[BoxGroup("Facts")]
	public EvidencePreset.BelongsToSetting itemReceiver;

	// Token: 0x0400345E RID: 13406
	[ReorderableList]
	[BoxGroup("Facts")]
	[Tooltip("Automatically create these facts...")]
	public List<EvidencePreset.EvidenceFactSetup> factSetup = new List<EvidencePreset.EvidenceFactSetup>();

	// Token: 0x0400345F RID: 13407
	[BoxGroup("Facts")]
	[Tooltip("Automatically add a link to these facts (doesn't have to feature this evidence)")]
	[ReorderableList]
	public List<EvidencePreset.FactLinkSetup> addFactLinks = new List<EvidencePreset.FactLinkSetup>();

	// Token: 0x04003460 RID: 13408
	[BoxGroup("Discovery")]
	[Tooltip("Discover this evidence when it is created.")]
	public bool discoverOnCreate;

	// Token: 0x04003461 RID: 13409
	[BoxGroup("Discovery")]
	[ReorderableList]
	[Tooltip("On discovery, merge these keys (this evidence)")]
	public List<EvidencePreset.MergeKeysSetup> keyMergeOnDiscovery = new List<EvidencePreset.MergeKeysSetup>();

	// Token: 0x04003462 RID: 13410
	[BoxGroup("Discovery")]
	[Tooltip("Conditions for discovery of this evidence (ANY of these)")]
	[ReorderableList]
	public List<Evidence.Discovery> discoveryTriggers = new List<Evidence.Discovery>();

	// Token: 0x04003463 RID: 13411
	[ReorderableList]
	[BoxGroup("Discovery")]
	[Tooltip("Apply these discoveries on discovery")]
	public List<EvidencePreset.DiscoveryApplication> applicationOnDiscover = new List<EvidencePreset.DiscoveryApplication>();

	// Token: 0x04003464 RID: 13412
	[Header("Content")]
	[Tooltip("Use this ID for content (do the rest in DDS editor)")]
	public string ddsDocumentID = "715b743b-ee3e-4d93-99c5-fc5b1882b2f0";

	// Token: 0x04003465 RID: 13413
	[Header("Matching")]
	[Tooltip("Some matching types below will only match to-from a match parent.")]
	public bool isMatchParent;

	// Token: 0x04003466 RID: 13414
	[Tooltip("List of match types for auto-creating matches")]
	public List<MatchPreset> matchTypes = new List<MatchPreset>();

	// Token: 0x04003467 RID: 13415
	[Header("Evidence Folder")]
	public bool enableSummary = true;

	// Token: 0x04003468 RID: 13416
	public bool enableFacts = true;

	// Token: 0x04003469 RID: 13417
	[Tooltip("The type of pinned evidence style")]
	public EvidencePreset.PinnedStyle pinnedStyle;

	// Token: 0x0400346A RID: 13418
	[Tooltip("Colour multiplier for pinned evidence background")]
	public Color pinnedBackgroundColour = Color.white;

	// Token: 0x02000713 RID: 1811
	public enum CaptureRules
	{
		// Token: 0x0400346C RID: 13420
		building,
		// Token: 0x0400346D RID: 13421
		location,
		// Token: 0x0400346E RID: 13422
		item,
		// Token: 0x0400346F RID: 13423
		citizen
	}

	// Token: 0x02000714 RID: 1812
	public enum BelongsToSetting
	{
		// Token: 0x04003471 RID: 13425
		self,
		// Token: 0x04003472 RID: 13426
		partner,
		// Token: 0x04003473 RID: 13427
		paramour,
		// Token: 0x04003474 RID: 13428
		boss,
		// Token: 0x04003475 RID: 13429
		doctor,
		// Token: 0x04003476 RID: 13430
		landlord
	}

	// Token: 0x02000715 RID: 1813
	public enum Subject
	{
		// Token: 0x04003478 RID: 13432
		self,
		// Token: 0x04003479 RID: 13433
		writer,
		// Token: 0x0400347A RID: 13434
		receiver,
		// Token: 0x0400347B RID: 13435
		parent,
		// Token: 0x0400347C RID: 13436
		interactable,
		// Token: 0x0400347D RID: 13437
		interactableLocation
	}

	// Token: 0x02000716 RID: 1814
	[Serializable]
	public class EvidenceFactSetup
	{
		// Token: 0x0400347E RID: 13438
		public FactPreset preset;

		// Token: 0x0400347F RID: 13439
		public EvidencePreset.Subject link;

		// Token: 0x04003480 RID: 13440
		[Tooltip("Item evidence only: Only create the belongsTo fact if this is placed in an owned position.")]
		public bool onlyIfInOwnedPosition;

		// Token: 0x04003481 RID: 13441
		[Tooltip("Create this fact on discovery")]
		public bool createOnDiscovery = true;

		// Token: 0x04003482 RID: 13442
		[Tooltip("Force discovery of this fact when this is created")]
		public bool forceDiscoveryOnCreation = true;

		// Token: 0x04003483 RID: 13443
		[Tooltip("When creating the above, switch the from (this) and to (link) evidence.")]
		public bool switchFindingFactToFrom;
	}

	// Token: 0x02000717 RID: 1815
	[Serializable]
	public class FactLinkSetup
	{
		// Token: 0x04003484 RID: 13444
		public EvidencePreset.FactLinkSubject subject;

		// Token: 0x04003485 RID: 13445
		public string factDictionary;

		// Token: 0x04003486 RID: 13446
		public Evidence.DataKey key;

		// Token: 0x04003487 RID: 13447
		public bool discovery = true;
	}

	// Token: 0x02000718 RID: 1816
	[Serializable]
	public class DataKeyAutomaticTies
	{
		// Token: 0x04003488 RID: 13448
		public Evidence.DataKey mainKey;

		// Token: 0x04003489 RID: 13449
		public List<Evidence.DataKey> mergeAtStart = new List<Evidence.DataKey>();
	}

	// Token: 0x02000719 RID: 1817
	public enum FactLinkSubject
	{
		// Token: 0x0400348B RID: 13451
		writer,
		// Token: 0x0400348C RID: 13452
		receiver
	}

	// Token: 0x0200071A RID: 1818
	[Serializable]
	public class MergeKeysSetup
	{
		// Token: 0x0400348D RID: 13453
		public EvidencePreset.Subject link;

		// Token: 0x0400348E RID: 13454
		public List<Evidence.DataKey> mergeKeys;
	}

	// Token: 0x0200071B RID: 1819
	[Serializable]
	public class DiscoveryApplication
	{
		// Token: 0x0400348F RID: 13455
		public EvidencePreset.Subject link;

		// Token: 0x04003490 RID: 13456
		public Evidence.Discovery applyDiscoveryTrigger;
	}

	// Token: 0x0200071C RID: 1820
	public enum PinnedStyle
	{
		// Token: 0x04003492 RID: 13458
		polaroid,
		// Token: 0x04003493 RID: 13459
		stickNote
	}
}
