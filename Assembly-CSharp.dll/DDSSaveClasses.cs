using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001E0 RID: 480
public class DDSSaveClasses
{
	// Token: 0x020001E1 RID: 481
	public enum TreeTriggers
	{
		// Token: 0x04000C0E RID: 3086
		awake,
		// Token: 0x04000C0F RID: 3087
		dead,
		// Token: 0x04000C10 RID: 3088
		asleep,
		// Token: 0x04000C11 RID: 3089
		unconscious,
		// Token: 0x04000C12 RID: 3090
		noReactionState,
		// Token: 0x04000C13 RID: 3091
		investigating,
		// Token: 0x04000C14 RID: 3092
		investigatingVisual,
		// Token: 0x04000C15 RID: 3093
		investigatingSound,
		// Token: 0x04000C16 RID: 3094
		persuing,
		// Token: 0x04000C17 RID: 3095
		searching,
		// Token: 0x04000C18 RID: 3096
		notInCombat,
		// Token: 0x04000C19 RID: 3097
		inCombat,
		// Token: 0x04000C1A RID: 3098
		legal,
		// Token: 0x04000C1B RID: 3099
		illegal,
		// Token: 0x04000C1C RID: 3100
		travelling,
		// Token: 0x04000C1D RID: 3101
		sat,
		// Token: 0x04000C1E RID: 3102
		employee,
		// Token: 0x04000C1F RID: 3103
		nonEmployee,
		// Token: 0x04000C20 RID: 3104
		carrying,
		// Token: 0x04000C21 RID: 3105
		notCarrying,
		// Token: 0x04000C22 RID: 3106
		privateLocation,
		// Token: 0x04000C23 RID: 3107
		publicLocation,
		// Token: 0x04000C24 RID: 3108
		onStreet,
		// Token: 0x04000C25 RID: 3109
		atHome,
		// Token: 0x04000C26 RID: 3110
		atWork,
		// Token: 0x04000C27 RID: 3111
		lightOnAny,
		// Token: 0x04000C28 RID: 3112
		lightOnMain,
		// Token: 0x04000C29 RID: 3113
		allLightsOff,
		// Token: 0x04000C2A RID: 3114
		rain,
		// Token: 0x04000C2B RID: 3115
		indoors,
		// Token: 0x04000C2C RID: 3116
		brokenSign,
		// Token: 0x04000C2D RID: 3117
		travellingToWork,
		// Token: 0x04000C2E RID: 3118
		notPresent,
		// Token: 0x04000C2F RID: 3119
		atEatery,
		// Token: 0x04000C30 RID: 3120
		hasJob,
		// Token: 0x04000C31 RID: 3121
		unemployed,
		// Token: 0x04000C32 RID: 3122
		homeIntenseWallpaper,
		// Token: 0x04000C33 RID: 3123
		homeBrightSign,
		// Token: 0x04000C34 RID: 3124
		enforcerOnDuty,
		// Token: 0x04000C35 RID: 3125
		notEnforcerOnDuty,
		// Token: 0x04000C36 RID: 3126
		trespassing,
		// Token: 0x04000C37 RID: 3127
		locationOfAuthority,
		// Token: 0x04000C38 RID: 3128
		drunk,
		// Token: 0x04000C39 RID: 3129
		restrained,
		// Token: 0x04000C3A RID: 3130
		sober,
		// Token: 0x04000C3B RID: 3131
		hasRoomAtHotel,
		// Token: 0x04000C3C RID: 3132
		hotelPaymentDue,
		// Token: 0x04000C3D RID: 3133
		hasNoRoomAtHotel
	}

	// Token: 0x020001E2 RID: 482
	public enum RepeatSetting
	{
		// Token: 0x04000C3F RID: 3135
		oneHour,
		// Token: 0x04000C40 RID: 3136
		sixHours,
		// Token: 0x04000C41 RID: 3137
		twelveHours,
		// Token: 0x04000C42 RID: 3138
		oneDay,
		// Token: 0x04000C43 RID: 3139
		twoDays,
		// Token: 0x04000C44 RID: 3140
		threeDays,
		// Token: 0x04000C45 RID: 3141
		oneWeek,
		// Token: 0x04000C46 RID: 3142
		never,
		// Token: 0x04000C47 RID: 3143
		noLimit
	}

	// Token: 0x020001E3 RID: 483
	public enum TriggerPoint
	{
		// Token: 0x04000C49 RID: 3145
		onNewTrackTarget,
		// Token: 0x04000C4A RID: 3146
		onNewAction,
		// Token: 0x04000C4B RID: 3147
		whileTickOnTrackTarget,
		// Token: 0x04000C4C RID: 3148
		vmail,
		// Token: 0x04000C4D RID: 3149
		telephone,
		// Token: 0x04000C4E RID: 3150
		never,
		// Token: 0x04000C4F RID: 3151
		newspaperArticle
	}

	// Token: 0x020001E4 RID: 484
	public enum TraitConditionType
	{
		// Token: 0x04000C51 RID: 3153
		IfAnyOfThese,
		// Token: 0x04000C52 RID: 3154
		IfAllOfThese,
		// Token: 0x04000C53 RID: 3155
		IfNoneOfThese,
		// Token: 0x04000C54 RID: 3156
		otherAnyOfThese,
		// Token: 0x04000C55 RID: 3157
		otherAllOfThese,
		// Token: 0x04000C56 RID: 3158
		otherNoneOfThese
	}

	// Token: 0x020001E5 RID: 485
	[Serializable]
	public class DDSComponent
	{
		// Token: 0x04000C57 RID: 3159
		public string name;

		// Token: 0x04000C58 RID: 3160
		public string id;
	}

	// Token: 0x020001E6 RID: 486
	[Serializable]
	public class DDSBlockSave : DDSSaveClasses.DDSComponent
	{
		// Token: 0x06000B79 RID: 2937 RVA: 0x000A96B4 File Offset: 0x000A78B4
		public DDSSaveClasses.DDSReplacement AddReplacement()
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("Add replacement", 2);
			}
			DDSSaveClasses.DDSReplacement ddsreplacement = new DDSSaveClasses.DDSReplacement
			{
				replaceWithID = Toolbox.Instance.GenerateUniqueID()
			};
			this.replacements.Add(ddsreplacement);
			Strings.WriteToDictionary("dds.blocks", ddsreplacement.replaceWithID, string.Empty, "New Replacement", string.Empty, 0, false, string.Empty);
			return ddsreplacement;
		}

		// Token: 0x04000C59 RID: 3161
		public List<DDSSaveClasses.DDSReplacement> replacements = new List<DDSSaveClasses.DDSReplacement>();
	}

	// Token: 0x020001E7 RID: 487
	[Serializable]
	public class DDSReplacement
	{
		// Token: 0x04000C5A RID: 3162
		public string replaceWithID;

		// Token: 0x04000C5B RID: 3163
		public bool useConnection;

		// Token: 0x04000C5C RID: 3164
		public Acquaintance.ConnectionType connection = Acquaintance.ConnectionType.anyoneNotPlayer;

		// Token: 0x04000C5D RID: 3165
		public bool useDislikeLike;

		// Token: 0x04000C5E RID: 3166
		public float strangerKnown = 0.5f;

		// Token: 0x04000C5F RID: 3167
		public float dislikeLike = 0.5f;

		// Token: 0x04000C60 RID: 3168
		public bool useTraits;

		// Token: 0x04000C61 RID: 3169
		public DDSSaveClasses.TraitConditionType traitCondition;

		// Token: 0x04000C62 RID: 3170
		public List<string> traits = new List<string>();
	}

	// Token: 0x020001E8 RID: 488
	[Serializable]
	public class DDSMessageSave : DDSSaveClasses.DDSComponent
	{
		// Token: 0x06000B7C RID: 2940 RVA: 0x000A9768 File Offset: 0x000A7968
		public void AddBlock(string newBlockID)
		{
			string text = Toolbox.Instance.GenerateUniqueID();
			this.blocks.Add(new DDSSaveClasses.DDSBlockCondition
			{
				blockID = newBlockID,
				instanceID = text
			});
			if (Game.Instance.printDebug)
			{
				Game.Log("DDS: Added new block to message with instance: " + text, 2);
			}
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x000A97BC File Offset: 0x000A79BC
		public void RemoveBlock(string instID)
		{
			int num = this.blocks.FindIndex((DDSSaveClasses.DDSBlockCondition item) => item.instanceID == instID);
			if (num > -1)
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("DDS: Removed block with instance " + instID + " from message", 2);
				}
				this.blocks.RemoveAt(num);
			}
		}

		// Token: 0x04000C63 RID: 3171
		public List<DDSSaveClasses.DDSBlockCondition> blocks = new List<DDSSaveClasses.DDSBlockCondition>();
	}

	// Token: 0x020001EA RID: 490
	[Serializable]
	public class DDSBlockCondition
	{
		// Token: 0x04000C65 RID: 3173
		public string blockID;

		// Token: 0x04000C66 RID: 3174
		public string instanceID;

		// Token: 0x04000C67 RID: 3175
		public bool alwaysDisplay = true;

		// Token: 0x04000C68 RID: 3176
		public int group;

		// Token: 0x04000C69 RID: 3177
		public bool useTraits;

		// Token: 0x04000C6A RID: 3178
		public DDSSaveClasses.TraitConditionType traitConditions;

		// Token: 0x04000C6B RID: 3179
		public List<string> traits = new List<string>();
	}

	// Token: 0x020001EB RID: 491
	public enum TreeType
	{
		// Token: 0x04000C6D RID: 3181
		conversation,
		// Token: 0x04000C6E RID: 3182
		vmail,
		// Token: 0x04000C6F RID: 3183
		document,
		// Token: 0x04000C70 RID: 3184
		newspaper,
		// Token: 0x04000C71 RID: 3185
		misc
	}

	// Token: 0x020001EC RID: 492
	[Serializable]
	public class DDSTreeSave : DDSSaveClasses.DDSComponent
	{
		// Token: 0x06000B82 RID: 2946 RVA: 0x000A9868 File Offset: 0x000A7A68
		public string AddMessage(string newMsgID)
		{
			string text = Toolbox.Instance.GenerateUniqueID();
			if (Game.Instance.printDebug)
			{
				Game.Log("DDS: New message instance: " + text, 2);
			}
			this.messages.Add(new DDSSaveClasses.DDSMessageSettings
			{
				msgID = newMsgID,
				instanceID = text,
				order = this.messages.Count
			});
			return text;
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x000A98D0 File Offset: 0x000A7AD0
		public void RemoveMessage(string instID)
		{
			int num = this.messages.FindIndex((DDSSaveClasses.DDSMessageSettings item) => item.instanceID == instID);
			if (num > -1)
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("DDS: Removing message/element", 2);
				}
				this.messages.RemoveAt(num);
			}
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x000A992C File Offset: 0x000A7B2C
		public string AddElement(string elementName)
		{
			string text = Toolbox.Instance.GenerateUniqueID();
			if (Game.Instance.printDebug)
			{
				Game.Log("DDS: New element instance: " + text, 2);
			}
			this.messages.Add(new DDSSaveClasses.DDSMessageSettings
			{
				elementName = elementName,
				instanceID = text,
				col = Color.white,
				order = this.messages.Count
			});
			return text;
		}

		// Token: 0x04000C72 RID: 3186
		public DDSSaveClasses.DDSParticipant participantA = new DDSSaveClasses.DDSParticipant();

		// Token: 0x04000C73 RID: 3187
		public DDSSaveClasses.DDSParticipant participantB = new DDSSaveClasses.DDSParticipant();

		// Token: 0x04000C74 RID: 3188
		public DDSSaveClasses.DDSParticipant participantC = new DDSSaveClasses.DDSParticipant();

		// Token: 0x04000C75 RID: 3189
		public DDSSaveClasses.DDSParticipant participantD = new DDSSaveClasses.DDSParticipant();

		// Token: 0x04000C76 RID: 3190
		public DDSSaveClasses.RepeatSetting repeat = DDSSaveClasses.RepeatSetting.sixHours;

		// Token: 0x04000C77 RID: 3191
		public DDSSaveClasses.TriggerPoint triggerPoint;

		// Token: 0x04000C78 RID: 3192
		public List<DDSSaveClasses.DDSMessageSettings> messages = new List<DDSSaveClasses.DDSMessageSettings>();

		// Token: 0x04000C79 RID: 3193
		public bool stopMovement = true;

		// Token: 0x04000C7A RID: 3194
		public bool ignoreGlobalRepeat;

		// Token: 0x04000C7B RID: 3195
		public DDSSaveClasses.TreeType treeType;

		// Token: 0x04000C7C RID: 3196
		public DDSSaveClasses.DDSDocument document;

		// Token: 0x04000C7D RID: 3197
		public string startingMessage;

		// Token: 0x04000C7E RID: 3198
		public float treeChance = 1f;

		// Token: 0x04000C7F RID: 3199
		public int priority = 3;

		// Token: 0x04000C80 RID: 3200
		public int newspaperCategory;

		// Token: 0x04000C81 RID: 3201
		public int newspaperContext;

		// Token: 0x04000C82 RID: 3202
		[NonSerialized]
		public Dictionary<string, DDSSaveClasses.DDSMessageSettings> messageRef;
	}

	// Token: 0x020001EE RID: 494
	[Serializable]
	public class DDSDocument
	{
		// Token: 0x04000C84 RID: 3204
		public string background;

		// Token: 0x04000C85 RID: 3205
		public Image.Type fill = 1;

		// Token: 0x04000C86 RID: 3206
		public Vector2 size;

		// Token: 0x04000C87 RID: 3207
		public Color colour = Color.white;
	}

	// Token: 0x020001EF RID: 495
	public enum ElementType
	{
		// Token: 0x04000C89 RID: 3209
		messageText,
		// Token: 0x04000C8A RID: 3210
		special
	}

	// Token: 0x020001F0 RID: 496
	[Serializable]
	public class DDSMessageSettings
	{
		// Token: 0x04000C8B RID: 3211
		public string msgID;

		// Token: 0x04000C8C RID: 3212
		public string elementName;

		// Token: 0x04000C8D RID: 3213
		public string instanceID;

		// Token: 0x04000C8E RID: 3214
		public int saidBy;

		// Token: 0x04000C8F RID: 3215
		public int saidTo = 1;

		// Token: 0x04000C90 RID: 3216
		public Vector2 pos = new Vector2(0f, -64f);

		// Token: 0x04000C91 RID: 3217
		public Vector2 size = new Vector2(320f, 300f);

		// Token: 0x04000C92 RID: 3218
		public float rot;

		// Token: 0x04000C93 RID: 3219
		public string font = "Halogen";

		// Token: 0x04000C94 RID: 3220
		public Color col = Color.black;

		// Token: 0x04000C95 RID: 3221
		public float fontSize = 22f;

		// Token: 0x04000C96 RID: 3222
		public float charSpace;

		// Token: 0x04000C97 RID: 3223
		public float wordSpace;

		// Token: 0x04000C98 RID: 3224
		public float lineSpace = 16f;

		// Token: 0x04000C99 RID: 3225
		public float paraSpace;

		// Token: 0x04000C9A RID: 3226
		public int alignH;

		// Token: 0x04000C9B RID: 3227
		public int alignV;

		// Token: 0x04000C9C RID: 3228
		public int fontStyle;

		// Token: 0x04000C9D RID: 3229
		public int order;

		// Token: 0x04000C9E RID: 3230
		public bool usePages;

		// Token: 0x04000C9F RID: 3231
		public bool isHandwriting;

		// Token: 0x04000CA0 RID: 3232
		public List<DDSSaveClasses.DDSMessageLink> links = new List<DDSSaveClasses.DDSMessageLink>();
	}

	// Token: 0x020001F1 RID: 497
	[Serializable]
	public class DDSMessageLink
	{
		// Token: 0x04000CA1 RID: 3233
		public string from;

		// Token: 0x04000CA2 RID: 3234
		public string to;

		// Token: 0x04000CA3 RID: 3235
		public Vector2 delayInterval = new Vector2(0f, 0.01f);

		// Token: 0x04000CA4 RID: 3236
		public bool useWeights;

		// Token: 0x04000CA5 RID: 3237
		public float choiceWeight = 0.5f;

		// Token: 0x04000CA6 RID: 3238
		public bool useKnowLike;

		// Token: 0x04000CA7 RID: 3239
		public float know = 0.5f;

		// Token: 0x04000CA8 RID: 3240
		public float like = 0.5f;

		// Token: 0x04000CA9 RID: 3241
		public bool useTraits;

		// Token: 0x04000CAA RID: 3242
		public List<string> traits = new List<string>();

		// Token: 0x04000CAB RID: 3243
		public DDSSaveClasses.TraitConditionType traitConditions;
	}

	// Token: 0x020001F2 RID: 498
	[Serializable]
	public class DDSParticipant
	{
		// Token: 0x04000CAC RID: 3244
		public bool required;

		// Token: 0x04000CAD RID: 3245
		public Acquaintance.ConnectionType connection = Acquaintance.ConnectionType.anyoneNotPlayer;

		// Token: 0x04000CAE RID: 3246
		public bool useJobs;

		// Token: 0x04000CAF RID: 3247
		public bool disableInbox;

		// Token: 0x04000CB0 RID: 3248
		public List<string> jobs = new List<string>();

		// Token: 0x04000CB1 RID: 3249
		public bool useTraits;

		// Token: 0x04000CB2 RID: 3250
		public List<string> traits = new List<string>();

		// Token: 0x04000CB3 RID: 3251
		public DDSSaveClasses.TraitConditionType traitConditions;

		// Token: 0x04000CB4 RID: 3252
		public List<DDSSaveClasses.TreeTriggers> triggers = new List<DDSSaveClasses.TreeTriggers>();
	}
}
