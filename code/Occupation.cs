using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000145 RID: 325
public class Occupation : IComparable<Occupation>
{
	// Token: 0x060008CA RID: 2250 RVA: 0x00086498 File Offset: 0x00084698
	public void Setup()
	{
		this.id = Occupation.idAssign;
		Occupation.idAssign++;
		this.name = Strings.Get("jobs", this.preset.name, Strings.Casing.asIs, false, false, false, null);
		this.collar = this.preset.collar;
		this.work = this.preset.work;
		this.tags.AddRange(this.preset.tags);
		if (this.employer != null)
		{
			this.startTimeDecimalHour = this.shift.decimalHours.x;
			this.endTimeDecialHour = this.shift.decimalHours.y;
			if (this.endTimeDecialHour >= this.startTimeDecimalHour)
			{
				this.workHours = this.endTimeDecialHour - this.startTimeDecimalHour;
			}
			else
			{
				this.workHours = 24f + this.endTimeDecialHour - this.startTimeDecimalHour;
			}
			this.workDaysList.Clear();
			if (this.shift.monday)
			{
				this.workDaysList.Add(SessionData.WeekDay.monday);
			}
			if (this.shift.tuesday)
			{
				this.workDaysList.Add(SessionData.WeekDay.tuesday);
			}
			if (this.shift.wednesday)
			{
				this.workDaysList.Add(SessionData.WeekDay.wednesday);
			}
			if (this.shift.thursday)
			{
				this.workDaysList.Add(SessionData.WeekDay.thursday);
			}
			if (this.shift.friday)
			{
				this.workDaysList.Add(SessionData.WeekDay.friday);
			}
			if (this.shift.saturday)
			{
				this.workDaysList.Add(SessionData.WeekDay.saturday);
			}
			if (this.shift.sunday)
			{
				this.workDaysList.Add(SessionData.WeekDay.sunday);
			}
			this.lunchBreak = this.preset.lunchBreakAllowed;
			if (this.lunchBreak)
			{
				this.lunchBreakHoursAfterStart = (float)Mathf.RoundToInt(this.workHours * 0.45f * 2f) / 2f;
			}
			float num = this.employer.preset.payGradeCurve.Evaluate(this.paygrade);
			if (this.teamLeader)
			{
				num += Toolbox.Instance.GetPsuedoRandomNumber(this.salary * 0.1f, this.salary * 0.2f, this.id.ToString(), false);
			}
			this.salary = Mathf.Lerp(this.employer.minimumSalary, this.employer.topSalary, num);
			this.salary = (float)Mathf.RoundToInt(this.salary * 10f) / 10f;
			this.salaryString = CityControls.Instance.cityCurrency + Mathf.RoundToInt(this.salary * 1000f).ToString();
		}
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x00086744 File Offset: 0x00084944
	public void Load(CitySaveData.OccupationCitySave data, Company newCompany)
	{
		this.id = data.id;
		this.employer = newCompany;
		if (!Toolbox.Instance.LoadDataFromResources<OccupationPreset>(data.preset, out this.preset))
		{
			Game.LogError("Could not find occupation preset: " + data.preset, 2);
		}
		this.name = data.name;
		this.collar = data.collar;
		this.work = data.work;
		this.tags = data.tags;
		this.startTimeDecimalHour = data.startTime;
		this.endTimeDecialHour = data.endTime;
		if (this.endTimeDecialHour >= this.startTimeDecimalHour)
		{
			this.workHours = this.endTimeDecialHour - this.startTimeDecimalHour;
		}
		else
		{
			this.workHours = 24f + this.endTimeDecialHour - this.startTimeDecimalHour;
		}
		this.lunchBreak = this.preset.lunchBreakAllowed;
		if (this.lunchBreak)
		{
			this.lunchBreakHoursAfterStart = (float)Mathf.RoundToInt(this.workHours * 0.45f * 2f) / 2f;
		}
		this.workDaysList = data.workDaysList;
		this.isOwner = data.isOwner;
		this.salary = data.salary;
		this.salaryString = data.salaryString;
		this.paygrade = data.paygrade;
		this.teamLeader = data.teamLeader;
		this.teamID = data.teamID;
		if (this.employer != null)
		{
			this.shift = this.employer.shifts[data.shift];
		}
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x000868CC File Offset: 0x00084ACC
	public bool IsAtWork(float atTime)
	{
		if (this.employer == null)
		{
			return false;
		}
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		SessionData.Instance.ParseTimeData(atTime, out num, out num2, out num3, out num4, out num5);
		return num >= this.startTimeDecimalHour - 0.25f && num <= this.endTimeDecialHour + 0.25f && (!this.preset.lunchBreakAllowed || num < (this.endTimeDecialHour - this.startTimeDecimalHour) * 0.5f || num > (this.endTimeDecialHour - this.startTimeDecimalHour) * 0.5f + 1f) && ((num2 == 0 && this.workDaysList.Contains(SessionData.WeekDay.monday)) || (num2 == 1 && this.workDaysList.Contains(SessionData.WeekDay.tuesday)) || (num2 == 2 && this.workDaysList.Contains(SessionData.WeekDay.wednesday)) || (num2 == 3 && this.workDaysList.Contains(SessionData.WeekDay.thursday)) || (num2 == 4 && this.workDaysList.Contains(SessionData.WeekDay.friday)) || (num2 == 5 && this.workDaysList.Contains(SessionData.WeekDay.saturday)) || (num2 == 6 && this.workDaysList.Contains(SessionData.WeekDay.sunday)));
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x000869F8 File Offset: 0x00084BF8
	public CitySaveData.OccupationCitySave GenerateSaveData()
	{
		CitySaveData.OccupationCitySave occupationCitySave = new CitySaveData.OccupationCitySave();
		occupationCitySave.id = this.id;
		occupationCitySave.preset = this.preset.name;
		occupationCitySave.name = this.name;
		occupationCitySave.teamLeader = this.teamLeader;
		if (this.boss != null)
		{
			occupationCitySave.boss = this.boss.id;
		}
		occupationCitySave.paygrade = this.paygrade;
		occupationCitySave.teamID = this.teamID;
		occupationCitySave.collar = this.collar;
		occupationCitySave.isOwner = this.isOwner;
		occupationCitySave.work = this.work;
		occupationCitySave.tags = this.tags;
		if (this.employer != null)
		{
			occupationCitySave.shift = this.employer.shifts.FindIndex((CompanyOpenHoursPreset.CompanyShift item) => item == this.shift);
		}
		occupationCitySave.startTime = this.startTimeDecimalHour;
		occupationCitySave.endTime = this.endTimeDecialHour;
		occupationCitySave.workDaysList = this.workDaysList;
		occupationCitySave.salary = this.salary;
		occupationCitySave.salaryString = this.salaryString;
		return occupationCitySave;
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00086B08 File Offset: 0x00084D08
	public string GetWorkingHoursString()
	{
		string text = string.Empty;
		text = text + SessionData.Instance.DecimalToClockString(this.startTimeDecimalHour, false) + " - " + SessionData.Instance.DecimalToClockString(this.startTimeDecimalHour + this.workHours, false);
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = 1;
		for (int i = 0; i < this.workDaysList.Count; i++)
		{
			int num2 = (int)this.workDaysList[i];
			if (dictionary.ContainsKey(num2 - num))
			{
				dictionary[num2 - num] = num2;
				num++;
			}
			else
			{
				dictionary.Add(num2, num2);
				num = 1;
			}
		}
		foreach (KeyValuePair<int, int> keyValuePair in dictionary)
		{
			if (keyValuePair.Value == keyValuePair.Key)
			{
				text = text + ", " + Strings.Get("ui.interface", ((SessionData.WeekDay)keyValuePair.Key).ToString(), Strings.Casing.asIs, false, false, false, null);
			}
			else
			{
				text = string.Concat(new string[]
				{
					text,
					", ",
					Strings.Get("ui.interface", ((SessionData.WeekDay)keyValuePair.Key).ToString(), Strings.Casing.asIs, false, false, false, null),
					" - ",
					Strings.Get("ui.interface", ((SessionData.WeekDay)keyValuePair.Value).ToString(), Strings.Casing.asIs, false, false, false, null)
				});
			}
		}
		return text.Trim();
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00086CA4 File Offset: 0x00084EA4
	public int CompareTo(Occupation paygrade)
	{
		return this.paygrade.CompareTo(paygrade.paygrade);
	}

	// Token: 0x0400090E RID: 2318
	public int id;

	// Token: 0x0400090F RID: 2319
	[NonSerialized]
	public static int idAssign = 1;

	// Token: 0x04000910 RID: 2320
	public OccupationPreset preset;

	// Token: 0x04000911 RID: 2321
	public string name = "worker";

	// Token: 0x04000912 RID: 2322
	public Company employer;

	// Token: 0x04000913 RID: 2323
	public bool isAgent;

	// Token: 0x04000914 RID: 2324
	public bool teamLeader;

	// Token: 0x04000915 RID: 2325
	public Occupation boss;

	// Token: 0x04000916 RID: 2326
	public float paygrade;

	// Token: 0x04000917 RID: 2327
	public int teamID;

	// Token: 0x04000918 RID: 2328
	public bool isOwner;

	// Token: 0x04000919 RID: 2329
	public OccupationPreset.workCollar collar;

	// Token: 0x0400091A RID: 2330
	public OccupationPreset.workType work;

	// Token: 0x0400091B RID: 2331
	public List<OccupationPreset.workTags> tags = new List<OccupationPreset.workTags>();

	// Token: 0x0400091C RID: 2332
	public CompanyOpenHoursPreset.CompanyShift shift;

	// Token: 0x0400091D RID: 2333
	public float workHours = 8f;

	// Token: 0x0400091E RID: 2334
	public float startTimeDecimalHour = 9f;

	// Token: 0x0400091F RID: 2335
	public float endTimeDecialHour = 17f;

	// Token: 0x04000920 RID: 2336
	public bool lunchBreak;

	// Token: 0x04000921 RID: 2337
	public float lunchBreakHoursAfterStart = 3.5f;

	// Token: 0x04000922 RID: 2338
	[NonSerialized]
	public List<SessionData.WeekDay> workDaysList = new List<SessionData.WeekDay>();

	// Token: 0x04000923 RID: 2339
	public float salary;

	// Token: 0x04000924 RID: 2340
	public string salaryString = string.Empty;

	// Token: 0x04000925 RID: 2341
	public Human employee;

	// Token: 0x04000926 RID: 2342
	public static Comparison<Occupation> SalaryComparison = (Occupation object1, Occupation object2) => object1.salary.CompareTo(object2.salary);

	// Token: 0x04000927 RID: 2343
	public static Comparison<Occupation> FillPriorityComparison = (Occupation object1, Occupation object2) => object1.preset.jobFillPriority.CompareTo(object2.preset.jobFillPriority);
}
