using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000645 RID: 1605
public class EvidenceCitizen : EvidenceWitness
{
	// Token: 0x060023A1 RID: 9121 RVA: 0x001D8E62 File Offset: 0x001D7062
	public EvidenceCitizen(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		this.witnessController = (newController as Human);
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x001D8E7C File Offset: 0x001D707C
	public override string GetNote(List<Evidence.DataKey> keys)
	{
		if (keys == null)
		{
			keys = new List<Evidence.DataKey>();
		}
		if (this.witnessController == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		List<Evidence.DataKey> list = this.GetTiedKeys(keys);
		if (list == null)
		{
			list = new List<Evidence.DataKey>();
		}
		string text = "<sprite=\"icons\" name=\"Checkbox Empty\">";
		string text2 = "<sprite=\"icons\" name=\"Checkbox Checked\">";
		string text3 = "<sprite=\"icons\" name=\"Name Empty\">";
		string text4 = "<sprite=\"icons\" name=\"Name Checked\">";
		string text5 = "<font=\"PapaManAOE SDF\">";
		string text6 = "</font>";
		string text7 = string.Empty;
		bool useGenderReference = false;
		if (list.Contains(Evidence.DataKey.name))
		{
			text7 = text4;
		}
		else
		{
			text7 = text3;
		}
		stringBuilder.Append(text7 + Strings.Get("descriptors", "Name", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.name))
		{
			stringBuilder.Append(text5 + this.GetNameForDataKey(keys) + text6);
			useGenderReference = true;
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.photo))
		{
			text7 = text4;
			useGenderReference = true;
		}
		else
		{
			text7 = text3;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Photo", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.photo))
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "Yes", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.voice))
		{
			text7 = text4;
		}
		else
		{
			text7 = text3;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Voice ID", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.voice))
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "Yes", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.fingerprints))
		{
			text7 = text4;
		}
		else
		{
			text7 = text3;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Fingerprint", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.fingerprints))
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "Yes", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		stringBuilder.Append("\n<sprite=\"icons\" name=\"Link\">");
		if (list.Contains(Evidence.DataKey.age))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Age", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.age))
		{
			stringBuilder.Append(string.Concat(new string[]
			{
				text5,
				this.witnessController.GetAge().ToString(),
				" (",
				Strings.Get("descriptors", this.witnessController.GetAgeGroup().ToString(), Strings.Casing.asIs, false, false, false, null),
				")",
				text6
			}));
		}
		else if (list.Contains(Evidence.DataKey.ageGroup))
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", this.witnessController.GetAgeGroup().ToString(), Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.dateOfBirth))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Date of Birth", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.dateOfBirth))
		{
			stringBuilder.Append(text5 + this.witnessController.birthday + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.sex))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Gender", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.sex))
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", this.witnessController.gender.ToString(), Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.height))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Height", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.height))
		{
			stringBuilder.Append(string.Concat(new string[]
			{
				text5,
				Mathf.RoundToInt(this.witnessController.descriptors.heightCM).ToString(),
				" (",
				Strings.Get("descriptors", this.witnessController.descriptors.height.ToString(), Strings.Casing.asIs, false, false, false, null),
				")",
				text6
			}));
		}
		else if (list.Contains(Evidence.DataKey.heightEstimate))
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", this.witnessController.descriptors.height.ToString(), Strings.Casing.asIs, false, false, useGenderReference, this.witnessController) + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.build))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Build", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.build))
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", this.witnessController.descriptors.build.ToString(), Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.hair))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Hair", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.hair))
		{
			stringBuilder.Append(string.Concat(new string[]
			{
				text5,
				Strings.Get("descriptors", this.witnessController.descriptors.hairType.ToString(), Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController),
				", ",
				Strings.Get("descriptors", this.witnessController.descriptors.hairColourCategory.ToString(), Strings.Casing.lowerCase, false, false, false, null),
				text6
			}));
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.eyes))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Eyes", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.eyes))
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", this.witnessController.descriptors.eyeColour.ToString(), Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.shoeSize))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Shoe Size", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.shoeSize) || (list.Contains(Evidence.DataKey.shoeSizeEstimate) && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.footSizePerception) > 0f))
		{
			stringBuilder.Append(text5 + this.witnessController.descriptors.shoeSize.ToString() + text6);
		}
		else if (list.Contains(Evidence.DataKey.shoeSizeEstimate) && !list.Contains(Evidence.DataKey.shoeSize) && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.footSizePerception) <= 0f)
		{
			Vector2 vector = Toolbox.Instance.CreateTimeRange((float)this.witnessController.descriptors.shoeSize, 2f, false, true, 60);
			stringBuilder.Append(string.Concat(new string[]
			{
				text5,
				Mathf.FloorToInt(vector.x).ToString(),
				" - ",
				Mathf.CeilToInt(vector.y).ToString(),
				" (",
				Strings.Get("descriptors", "shoe size estimate", Strings.Casing.asIs, false, false, useGenderReference, this.witnessController),
				")",
				text6
			}));
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.glasses))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Glasses", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.glasses))
		{
			if (this.witnessController.characterTraits.Exists((Human.Trait item) => item.name == "Affliction-ShortSighted" || item.name == "Affliction-FarSighted"))
			{
				stringBuilder.Append(text5 + Strings.Get("descriptors", "Yes", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
			}
			else
			{
				stringBuilder.Append(text5 + Strings.Get("descriptors", "No", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
			}
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.facialHair))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Facial Hair", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.facialHair))
		{
			if (this.witnessController.characterTraits.Exists((Human.Trait item) => item.name == "Quirk-FacialHair"))
			{
				stringBuilder.Append(text5 + Strings.Get("descriptors", "Yes", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
			}
			else
			{
				stringBuilder.Append(text5 + Strings.Get("descriptors", "No", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
			}
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.bloodType))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Blood Type", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.bloodType))
		{
			stringBuilder.Append(text5 + "|bloodtype|" + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.address))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Address", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.address))
		{
			if (this.witnessController.home != null)
			{
				stringBuilder.Append(text5 + "|home.name|" + text6);
			}
			else
			{
				stringBuilder.Append(text5 + Strings.Get("descriptors", "None", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
			}
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.telephoneNumber))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Telephone", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.telephoneNumber))
		{
			if (this.witnessController.home != null && this.witnessController.home.telephones != null && this.witnessController.home.telephones.Count > 0)
			{
				stringBuilder.Append(text5 + "|home.telephone|" + text6);
			}
			else
			{
				stringBuilder.Append(text5 + Strings.Get("descriptors", "None", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
			}
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.work))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Workplace", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.work))
		{
			if (this.witnessController.job != null && this.witnessController.job.employer != null && this.witnessController.job.employer.address != null)
			{
				stringBuilder.Append(text5 + "|job.employer.location.name|" + text6);
			}
			else
			{
				stringBuilder.Append(text5 + Strings.Get("descriptors", "None", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
			}
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.jobTitle))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Job Title", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.jobTitle))
		{
			if (this.witnessController.job != null && this.witnessController.job.employer != null)
			{
				stringBuilder.Append(text5 + "|job.title|" + text6);
			}
			else
			{
				stringBuilder.Append(text5 + Strings.Get("descriptors", "None", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + text6);
			}
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.workHours))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Work Hours", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.workHours))
		{
			if (this.witnessController.job != null && this.witnessController.job.employer != null)
			{
				stringBuilder.Append(text5 + "|job.hours|" + text6);
			}
			else
			{
				stringBuilder.Append(text5 + Strings.Get("descriptors", "None", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + text6);
			}
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.salary))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Salary", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + ": ");
		if (list.Contains(Evidence.DataKey.salary))
		{
			if (this.witnessController.job != null && this.witnessController.job.employer != null)
			{
				stringBuilder.Append(text5 + "|job.salary|" + text6);
			}
			else
			{
				stringBuilder.Append(text5 + Strings.Get("descriptors", "None", Strings.Casing.firstLetterCaptial, false, false, false, null) + text6);
			}
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.handwriting))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		string text8 = string.Empty;
		if (this.witnessController.handwriting != null)
		{
			text8 = "<font=\"" + this.witnessController.handwriting.fontAsset.name + "\">";
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Handwriting", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.handwriting))
		{
			int myNumber = Toolbox.Instance.allHandwriting.IndexOf(this.witnessController.handwriting);
			stringBuilder.Append(string.Concat(new string[]
			{
				text8,
				Strings.Get("descriptors", "Sample", Strings.Casing.asIs, false, false, false, null),
				text6,
				"(",
				Strings.Get("evidence.generic", "Type", Strings.Casing.firstLetterCaptial, false, false, false, null),
				" ",
				Toolbox.Instance.ToBase26(myNumber),
				")"
			}));
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		if (list.Contains(Evidence.DataKey.code))
		{
			text7 = text2;
		}
		else
		{
			text7 = text;
		}
		stringBuilder.Append("\n" + text7 + Strings.Get("descriptors", "Passcode", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": ");
		if (list.Contains(Evidence.DataKey.code))
		{
			stringBuilder.Append(text8 + "|passcode|" + text6);
		}
		else
		{
			stringBuilder.Append(text5 + Strings.Get("descriptors", "?", Strings.Casing.asIs, false, false, false, null) + text6);
		}
		stringBuilder.Append("\n");
		stringBuilder.Append("\n" + Strings.Get("descriptors", "Additional Notes", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": \n" + text5);
		bool flag = false;
		if (list.Contains(Evidence.DataKey.randomInterest))
		{
			stringBuilder.Append(Strings.Get("descriptors", "Interest", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": |Interest|");
			flag = true;
		}
		if (list.Contains(Evidence.DataKey.randomAffliction))
		{
			stringBuilder.Append(Strings.Get("descriptors", "Affliction", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": |Affliction|");
			flag = true;
		}
		if (list.Contains(Evidence.DataKey.randomSocialClub))
		{
			if (flag)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(Strings.Get("descriptors", "member", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": |group1.name|");
			flag = true;
		}
		if (list.Contains(Evidence.DataKey.firstNameInitial))
		{
			if (flag)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(Strings.Get("descriptors", "First name initial", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": |initial|");
			flag = true;
		}
		if (list.Contains(Evidence.DataKey.partnerFirstName) && this.witnessController.partner != null)
		{
			if (flag)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(Strings.Get("descriptors", "Partner first name", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": " + this.witnessController.partner.GetFirstName());
			flag = true;
		}
		if (list.Contains(Evidence.DataKey.partnerJobTitle) && this.witnessController.partner != null && this.witnessController.partner != null && this.witnessController.partner.job != null && this.witnessController.partner.job.employer != null)
		{
			if (flag)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(Strings.Get("descriptors", "Partner job", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": |partner.job.title|");
			flag = true;
		}
		if (list.Contains(Evidence.DataKey.partnerSocialClub) && this.witnessController.partner != null)
		{
			if (flag)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(Strings.Get("descriptors", "Partner is member", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": |partner.group1.name|");
			flag = true;
		}
		if (list.Contains(Evidence.DataKey.livesOnFloor) && this.witnessController.home != null)
		{
			if (flag)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(Strings.Get("descriptors", "Lives on the ", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + " |home.floor| of |home.building|");
			flag = true;
		}
		if (list.Contains(Evidence.DataKey.livesInBuilding) && this.witnessController.home != null)
		{
			if (flag)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(Strings.Get("descriptors", "Lives in ", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + " |home.building|");
			flag = true;
		}
		if (list.Contains(Evidence.DataKey.worksInBuilding) && this.witnessController.job != null && this.witnessController.job.employer != null && this.witnessController.job.employer.address != null)
		{
			if (flag)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(Strings.Get("descriptors", "Works in ", Strings.Casing.firstLetterCaptial, false, false, useGenderReference, this.witnessController) + " |job.employer.location.building|");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x001DA593 File Offset: 0x001D8793
	public override string GenerateName()
	{
		return this.witnessController.GetCitizenName();
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x001DA5A0 File Offset: 0x001D87A0
	public override string GetNameForDataKey(List<Evidence.DataKey> inputKeys)
	{
		string text = string.Empty;
		string result;
		try
		{
			if (inputKeys == null)
			{
				result = Strings.Get("evidence.names", this.preset.name, Strings.Casing.asIs, false, false, false, null);
			}
			else
			{
				List<Evidence.DataKey> tiedKeys = this.GetTiedKeys(inputKeys);
				if (this.customNames.Count > 0)
				{
					foreach (Evidence.CustomName customName in this.customNames)
					{
						if (tiedKeys.Contains(customName.key))
						{
							return customName.name;
						}
					}
				}
				text = string.Empty;
				if (tiedKeys.Contains(Evidence.DataKey.name))
				{
					result = this.witnessController.GetCitizenName();
				}
				else if (tiedKeys.Contains(Evidence.DataKey.firstName))
				{
					result = this.witnessController.GetFirstName() + " " + Strings.Get("evidence.generic", "?", Strings.Casing.firstLetterCaptial, false, false, false, null);
				}
				else if (tiedKeys.Contains(Evidence.DataKey.surname))
				{
					result = Strings.Get("evidence.generic", "?", Strings.Casing.firstLetterCaptial, false, false, false, null) + " " + this.witnessController.GetSurName();
				}
				else if (tiedKeys.Contains(Evidence.DataKey.initialedName))
				{
					result = this.witnessController.GetInitialledName();
				}
				else if (tiedKeys.Contains(Evidence.DataKey.initials))
				{
					result = this.witnessController.GetInitials();
				}
				else if (base.GetTiedKeys(Evidence.DataKey.name).Exists((Evidence.DataKey item) => inputKeys.Contains(item)))
				{
					result = this.witnessController.GetCitizenName();
				}
				else if (base.GetTiedKeys(Evidence.DataKey.firstName).Exists((Evidence.DataKey item) => inputKeys.Contains(item)))
				{
					result = this.witnessController.GetFirstName() + " " + Strings.Get("evidence.generic", "?", Strings.Casing.firstLetterCaptial, false, false, false, null);
				}
				else if (base.GetTiedKeys(Evidence.DataKey.surname).Exists((Evidence.DataKey item) => inputKeys.Contains(item)))
				{
					result = Strings.Get("evidence.generic", "?", Strings.Casing.firstLetterCaptial, false, false, false, null) + " " + this.witnessController.GetSurName();
				}
				else if (base.GetTiedKeys(Evidence.DataKey.initialedName).Exists((Evidence.DataKey item) => inputKeys.Contains(item)))
				{
					result = this.witnessController.GetInitialledName();
				}
				else if (base.GetTiedKeys(Evidence.DataKey.initials).Exists((Evidence.DataKey item) => inputKeys.Contains(item)))
				{
					result = this.witnessController.GetInitials();
				}
				else
				{
					text = Strings.Get("evidence.generic", "Unknown", Strings.Casing.firstLetterCaptial, false, false, false, null) + " " + Strings.Get("evidence.generic", this.preset.name, Strings.Casing.firstLetterCaptial, false, false, false, null);
					result = text;
				}
			}
		}
		catch
		{
			result = text;
		}
		return result;
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x001DA8A0 File Offset: 0x001D8AA0
	public override void NamePhotoMerge()
	{
		this.witnessController.interactable.UpdateName(true, Evidence.DataKey.name);
		this.witnessController.interactable.drm = true;
	}

	// Token: 0x060023A6 RID: 9126 RVA: 0x001DA8C8 File Offset: 0x001D8AC8
	public override string GetNoteComposed(List<Evidence.DataKey> keys, bool useLinks = true)
	{
		Strings.LinkSetting linkSetting = Strings.LinkSetting.forceNoLinks;
		if (useLinks)
		{
			linkSetting = Strings.LinkSetting.forceLinks;
		}
		return Strings.ComposeText(this.GetNote(keys), this.witnessController, linkSetting, keys, null, false);
	}

	// Token: 0x04002DB9 RID: 11705
	public Human witnessController;
}
