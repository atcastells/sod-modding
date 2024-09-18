using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000804 RID: 2052
public class SocialControls : MonoBehaviour
{
	// Token: 0x17000124 RID: 292
	// (get) Token: 0x0600261D RID: 9757 RVA: 0x001EA42A File Offset: 0x001E862A
	public static SocialControls Instance
	{
		get
		{
			return SocialControls._instance;
		}
	}

	// Token: 0x0600261E RID: 9758 RVA: 0x001EA431 File Offset: 0x001E8631
	private void Awake()
	{
		if (SocialControls._instance != null && SocialControls._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SocialControls._instance = this;
	}

	// Token: 0x04004432 RID: 17458
	[Tooltip("Random ranges for knowning different acquaintances")]
	[Header("Relationships/Acquaintances")]
	public Vector2 knowLoverRange = new Vector2(0.9f, 1f);

	// Token: 0x04004433 RID: 17459
	public Vector2 knowHousemateRange = new Vector2(0.4f, 0.88f);

	// Token: 0x04004434 RID: 17460
	public Vector2 knowFriendRange = new Vector2(0.5f, 0.93f);

	// Token: 0x04004435 RID: 17461
	public Vector2 knowNeighborRange = new Vector2(0f, 0.66f);

	// Token: 0x04004436 RID: 17462
	public Vector2 knowBossRange = new Vector2(0.1f, 0.72f);

	// Token: 0x04004437 RID: 17463
	public Vector2 knowWorkTeamRange = new Vector2(0.1f, 0.8f);

	// Token: 0x04004438 RID: 17464
	public Vector2 knowWorkRange = new Vector2(0f, 0.7f);

	// Token: 0x04004439 RID: 17465
	public Vector2 knowWorkOtherRange = new Vector2(0.1f, 0.2f);

	// Token: 0x0400443A RID: 17466
	public Vector2 knowRegularCustomerRange = new Vector2(0.1f, 0.5f);

	// Token: 0x0400443B RID: 17467
	public Vector2 knowParamourRange = new Vector2(0.4f, 1f);

	// Token: 0x0400443C RID: 17468
	public Vector2 knowGroupRange = new Vector2(0f, 0.8f);

	// Token: 0x0400443D RID: 17469
	[Header("Traits Reference")]
	public CharacterTrait paramour;

	// Token: 0x0400443E RID: 17470
	[Header("Culture")]
	public int basePreferredBookCount = 3;

	// Token: 0x0400443F RID: 17471
	[Tooltip("Paygrades (see company preset wage enum")]
	[Header("Businesses")]
	public List<float> wageRanges = new List<float>();

	// Token: 0x04004440 RID: 17472
	[Tooltip("Overtime ranges (see occupation preset enum")]
	public List<Vector2> overtimeRanges = new List<Vector2>();

	// Token: 0x04004441 RID: 17473
	[Tooltip("0.8 - 1 accuracy (minutes)")]
	[Header("Memory Accuracy Steps")]
	[Space(7f)]
	public float accuracy1 = 5f;

	// Token: 0x04004442 RID: 17474
	[Tooltip("0.6 - 0.8 accuracy (minutes)")]
	public float accuracy2 = 10f;

	// Token: 0x04004443 RID: 17475
	[Tooltip("0.4 - 0.6 accuracy (minutes)")]
	public float accuracy3 = 15f;

	// Token: 0x04004444 RID: 17476
	[Tooltip("0.2 - 0.4 accuracy (minutes)")]
	public float accuracy4 = 30f;

	// Token: 0x04004445 RID: 17477
	[Tooltip("0.0 - 0.2 accuracy (minutes)")]
	public float accuracy5 = 60f;

	// Token: 0x04004446 RID: 17478
	[Space(7f)]
	[Header("Know Thresholds")]
	[Range(0f, 1f)]
	[Tooltip("How well known a connection has to be before they are included in a citizen's telephone book")]
	public float telephoneBookInclusionThreshold = 0.35f;

	// Token: 0x04004447 RID: 17479
	[Tooltip("How well known a connection has to be before they know the others' place of work")]
	[Range(0f, 1f)]
	public float knowPlaceOfWorkThreshold = 0.4f;

	// Token: 0x04004448 RID: 17480
	[Tooltip("How well known a connection has to be before they know the others' address")]
	[Range(0f, 1f)]
	public float knowAddressThreshold = 0.75f;

	// Token: 0x04004449 RID: 17481
	[Tooltip("How well known a connection has to be before a citizen mourn's another's death")]
	[Range(0f, 1f)]
	public float knowMournThreshold = 0.35f;

	// Token: 0x0400444A RID: 17482
	[Tooltip("How well known a connection has to be before a citizen sends the other birthday cards or has their birthday listed on the calendar")]
	[Range(0f, 1f)]
	public float knowBirthdayThreshold = 0.7f;

	// Token: 0x0400444B RID: 17483
	[Tooltip("How well known a connection before they can reveal their immediate location")]
	[Range(0f, 1f)]
	public float knowImmediateLocationThreshold = 0.8f;

	// Token: 0x0400444C RID: 17484
	[Tooltip("If true; social credit buffs are selected within random groups. If false, they are ordered per the list below.")]
	[Header("Social Credit Buffs")]
	public bool randomSocialCreditBuffs = true;

	// Token: 0x0400444D RID: 17485
	public AudioEvent perkNotificationAudioEvent;

	// Token: 0x0400444E RID: 17486
	public List<SocialControls.SocialCreditBuff> socialCreditBuffs = new List<SocialControls.SocialCreditBuff>();

	// Token: 0x0400444F RID: 17487
	private static SocialControls _instance;

	// Token: 0x02000805 RID: 2053
	[Serializable]
	public class SocialCreditBuff
	{
		// Token: 0x06002620 RID: 9760 RVA: 0x001EA602 File Offset: 0x001E8802
		public UpgradeEffectController.AppliedEffect GetEffect()
		{
			return new UpgradeEffectController.AppliedEffect
			{
				effect = this.effect,
				value = this.value,
				disk = null
			};
		}

		// Token: 0x04004450 RID: 17488
		public string name;

		// Token: 0x04004451 RID: 17489
		public string description;

		// Token: 0x04004452 RID: 17490
		public SyncDiskPreset.Effect effect;

		// Token: 0x04004453 RID: 17491
		public float value;

		// Token: 0x04004454 RID: 17492
		[Range(0f, 5f)]
		public int randomGrouping;
	}
}
