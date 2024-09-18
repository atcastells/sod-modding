using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200060E RID: 1550
public class CityInfoController : MonoBehaviour
{
	// Token: 0x06002232 RID: 8754 RVA: 0x001D0450 File Offset: 0x001CE650
	private void Start()
	{
		this.UpdateSelf();
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x001D0458 File Offset: 0x001CE658
	public void UpdateSelf()
	{
		this.text.text = "";
		Text text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "City Size", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.citySize.x.ToString(),
			" x ",
			CityData.Instance.citySize.y.ToString(),
			"\n"
		});
		float num = (float)Mathf.RoundToInt((float)CityData.Instance.residentialBuildings / (float)HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count * 100f);
		text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "Residential Buildings", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.residentialBuildings.ToString(),
			" (",
			num.ToString(),
			"%)\n"
		});
		num = (float)Mathf.RoundToInt((float)CityData.Instance.commercialBuildings / (float)HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count * 100f);
		text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "Commercial Buildings", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.commercialBuildings.ToString(),
			" (",
			num.ToString(),
			"%)\n"
		});
		num = (float)Mathf.RoundToInt((float)CityData.Instance.industrialBuildings / (float)HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count * 100f);
		text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "Industrial Buildings", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.industrialBuildings.ToString(),
			" (",
			num.ToString(),
			"%)\n"
		});
		num = (float)Mathf.RoundToInt((float)CityData.Instance.municipalBuildings / (float)HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count * 100f);
		text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "Municipal Buildings", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.municipalBuildings.ToString(),
			" (",
			num.ToString(),
			"%)\n"
		});
		num = (float)Mathf.RoundToInt((float)CityData.Instance.parkBuildings / (float)HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Count * 100f);
		text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "Park Buildings", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.parkBuildings.ToString(),
			" (",
			num.ToString(),
			"%)\n"
		});
		Text text2 = this.text;
		text2.text += "\n";
		text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "Population", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.citizenDirectory.Count.ToString(),
			"\n"
		});
		num = (float)Mathf.RoundToInt((float)CityData.Instance.inhabitedResidences / (float)CityData.Instance.residenceDirectory.Count * 100f);
		text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "Residences/Inhabited", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.residenceDirectory.Count.ToString(),
			"/",
			CityData.Instance.inhabitedResidences.ToString(),
			" (",
			num.ToString(),
			"%)\n"
		});
		num = (float)Mathf.RoundToInt((float)CityData.Instance.employedCitizens / (float)CityData.Instance.jobsDirectory.Count * 100f);
		text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "Employed/Jobs", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.employedCitizens.ToString(),
			"/",
			CityData.Instance.jobsDirectory.Count.ToString(),
			" (",
			num.ToString(),
			"%)\n"
		});
		num = (float)Mathf.RoundToInt((float)CityData.Instance.unemployedDirectory.Count / (float)CityData.Instance.citizenDirectory.Count * 100f);
		text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "Unemployed", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.unemployedDirectory.Count.ToString(),
			" (",
			num.ToString(),
			"%)\n"
		});
		text = this.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			Strings.Get("ui.interface", "Extra Unemployed Created", Strings.Casing.asIs, false, false, false, null),
			": ",
			CityData.Instance.extraUnemloyedCreated.ToString(),
			"\n"
		});
	}

	// Token: 0x04002CBE RID: 11454
	public Text text;
}
