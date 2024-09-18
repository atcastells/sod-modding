using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class ResidenceController : Controller, IComparable<ResidenceController>
{
	// Token: 0x060008D7 RID: 2263 RVA: 0x00086D9C File Offset: 0x00084F9C
	public void Setup(ResidencePreset newPreset, NewAddress newAddress)
	{
		this.preset = newPreset;
		this.address = newAddress;
		this.building = this.address.building;
		CityData.Instance.residenceDirectory.Add(this);
		this.CreateEvidence();
		foreach (NewRoom newBedroom in this.address.rooms.FindAll((NewRoom item) => item.roomType == InteriorControls.Instance.bedroomType))
		{
			this.AddBedroom(newBedroom);
		}
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00086E50 File Offset: 0x00085050
	public string GetResidenceString()
	{
		string text = this.address.residenceNumber.ToString();
		if (text.Length < 2)
		{
			text = "0" + text;
		}
		if (!(this.address != null) || !(this.address.floor != null))
		{
			return text;
		}
		if (this.address.floor.floor < 0)
		{
			return Strings.Get("names.rooms", "Basement", Strings.Casing.asIs, false, false, false, null) + " " + text;
		}
		return this.address.floor.floor.ToString() + text;
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x00086EF4 File Offset: 0x000850F4
	public int GetResidenceNumber()
	{
		int num = this.address.residenceNumber;
		if (this.address != null && this.address.floor != null)
		{
			if (this.address.floor.floor < 0)
			{
				num -= 100;
			}
			else
			{
				num += this.address.floor.floor * 100;
			}
		}
		return num;
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x00086F5F File Offset: 0x0008515F
	public void AddBedroom(NewRoom newBedroom)
	{
		if (!this.bedrooms.Contains(newBedroom))
		{
			this.bedrooms.Add(newBedroom);
		}
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x00086F7B File Offset: 0x0008517B
	public void Load(CitySaveData.ResidenceCitySave data, NewAddress newAddress)
	{
		this.address = newAddress;
		Toolbox.Instance.LoadDataFromResources<ResidencePreset>(data.preset, out this.preset);
		CityData.Instance.residenceDirectory.Add(this);
		this.CreateEvidence();
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x00086FB4 File Offset: 0x000851B4
	public CitySaveData.ResidenceCitySave GenerateSaveData()
	{
		CitySaveData.ResidenceCitySave residenceCitySave = new CitySaveData.ResidenceCitySave();
		residenceCitySave.preset = this.preset.name;
		if (this.mailbox != null)
		{
			residenceCitySave.mail = this.mailbox.id;
		}
		return residenceCitySave;
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00002265 File Offset: 0x00000465
	public override void CreateEvidence()
	{
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x00086FF2 File Offset: 0x000851F2
	public int CompareTo(ResidenceController other)
	{
		return this.address.normalizedLandValue.CompareTo(other.address.normalizedLandValue);
	}

	// Token: 0x04000929 RID: 2345
	public ResidencePreset preset;

	// Token: 0x0400092A RID: 2346
	[Header("Location")]
	public NewBuilding building;

	// Token: 0x0400092B RID: 2347
	public NewAddress address;

	// Token: 0x0400092C RID: 2348
	public List<NewRoom> bedrooms = new List<NewRoom>();

	// Token: 0x0400092D RID: 2349
	[NonSerialized]
	public int bedroomsTaken;

	// Token: 0x0400092E RID: 2350
	[NonSerialized]
	public FurnitureLocation mailbox;

	// Token: 0x0400092F RID: 2351
	public static Comparison<ResidenceController> RoommateComparison = delegate(ResidenceController object1, ResidenceController object2)
	{
		float num = 0f;
		foreach (Human human in object1.address.inhabitants)
		{
			Citizen citizen = (Citizen)human;
			num += citizen.societalClass;
		}
		num /= (float)object1.address.inhabitants.Count;
		num += (float)(3 - object1.address.inhabitants.Count);
		float num2 = 0f;
		foreach (Human human2 in object2.address.inhabitants)
		{
			Citizen citizen2 = (Citizen)human2;
			num2 += citizen2.societalClass;
		}
		num2 /= (float)object2.address.inhabitants.Count;
		num2 += (float)(3 - object2.address.inhabitants.Count);
		return num.CompareTo(num2);
	};
}
