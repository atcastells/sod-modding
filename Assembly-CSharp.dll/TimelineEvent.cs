using System;
using System.Collections.Generic;

// Token: 0x0200059F RID: 1439
[Serializable]
public class TimelineEvent : IComparable<TimelineEvent>
{
	// Token: 0x1400003A RID: 58
	// (add) Token: 0x06001F67 RID: 8039 RVA: 0x001B1DA4 File Offset: 0x001AFFA4
	// (remove) Token: 0x06001F68 RID: 8040 RVA: 0x001B1DDC File Offset: 0x001AFFDC
	public event TimelineEvent.OnNameChange OnNameChanged;

	// Token: 0x1400003B RID: 59
	// (add) Token: 0x06001F69 RID: 8041 RVA: 0x001B1E14 File Offset: 0x001B0014
	// (remove) Token: 0x06001F6A RID: 8042 RVA: 0x001B1E4C File Offset: 0x001B004C
	public event TimelineEvent.RecallAccuracyChange OnRecallAccuracyChange;

	// Token: 0x1400003C RID: 60
	// (add) Token: 0x06001F6B RID: 8043 RVA: 0x001B1E84 File Offset: 0x001B0084
	// (remove) Token: 0x06001F6C RID: 8044 RVA: 0x001B1EBC File Offset: 0x001B00BC
	public event TimelineEvent.OnCalledUponTimeUpdate OnCalledUponTimeUpdated;

	// Token: 0x06001F6D RID: 8045 RVA: 0x001B1EF4 File Offset: 0x001B00F4
	public TimelineEvent(TimelineEvent.EventType newType, NewNode newLocation, TimelineEvent newParentEvent, bool autoCallUpon, bool overrideHappenedAt = false, float happenedOverride = 0f)
	{
		this.intialised = true;
		this.eventType = newType;
		if (this.eventType == TimelineEvent.EventType.selfArrive || this.eventType == TimelineEvent.EventType.selfDepart || this.eventType == TimelineEvent.EventType.delayBegin || this.eventType == TimelineEvent.EventType.delayEnd)
		{
			this.isSelfLocational = true;
			this.eventID = TimelineEvent.assignEventID;
			TimelineEvent.assignEventID++;
		}
		else
		{
			this.isSelfLocational = false;
		}
		if (!overrideHappenedAt)
		{
			this.happenedAt = SessionData.Instance.gameTime;
		}
		else
		{
			this.happenedAt = happenedOverride;
		}
		this.location = newLocation;
		if (this.location != null)
		{
			this.debugLocationName = this.location.name;
		}
		this.parentEvent = newParentEvent;
		if (this.parentEvent != null && this.parentEvent != this)
		{
			this.parentEvent.AddChildEventToThis(this);
		}
		if (autoCallUpon)
		{
			this.CallUpon(false, 0);
		}
	}

	// Token: 0x06001F6E RID: 8046 RVA: 0x001B2004 File Offset: 0x001B0204
	public virtual void UpdateName()
	{
		this.name = Strings.Get("evidence.generic", this.eventType.ToString(), Strings.Casing.asIs, false, false, false, null);
		if (this.eventTime != null)
		{
			this.name = string.Concat(new string[]
			{
				this.name,
				" ",
				this.eventTime.startString,
				" - ",
				this.eventTime.endString
			});
			this.name = this.name + " (Accurate:" + this.eventTime.accurateString + ")";
		}
		if (this.OnNameChanged != null)
		{
			this.OnNameChanged();
		}
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x001B20BE File Offset: 0x001B02BE
	public void AddChildEventToThis(TimelineEvent newTied)
	{
		if (!this.childEvents.Contains(newTied))
		{
			this.childEvents.Add(newTied);
			newTied.parentEvent = this;
			newTied.eventTime = this.eventTime;
		}
	}

	// Token: 0x06001F70 RID: 8048 RVA: 0x001B20F0 File Offset: 0x001B02F0
	public virtual void CallUpon(bool forceAccuracy = false, int forceAccuracyToMinutes = 0)
	{
		if (this.calledUpon)
		{
			return;
		}
		if (this.eventTime == null)
		{
			this.eventTime = new EventTime(this, forceAccuracy, forceAccuracyToMinutes, false, 0f, 0f);
			this.eventTime.OnCalledUponTimeUpdated += this.UpdateName;
			this.eventTime.OnCalledUponTimeUpdated += this.OnTimeUpdated;
			this.OnTimeUpdated();
		}
		this.calledUpon = true;
		this.UpdateName();
	}

	// Token: 0x06001F71 RID: 8049 RVA: 0x001B2169 File Offset: 0x001B0369
	public void SetTimeRecallAccuracy(float newVal)
	{
		this.timeAccuracy = newVal;
		if (this.OnRecallAccuracyChange != null)
		{
			this.OnRecallAccuracyChange();
		}
	}

	// Token: 0x06001F72 RID: 8050 RVA: 0x001B2185 File Offset: 0x001B0385
	public void OnTimeUpdated()
	{
		if (this.OnCalledUponTimeUpdated != null)
		{
			this.OnCalledUponTimeUpdated();
		}
	}

	// Token: 0x06001F73 RID: 8051 RVA: 0x001B219A File Offset: 0x001B039A
	public virtual void OnAppearInTimeline()
	{
		this.discoveredByQuestioned = true;
	}

	// Token: 0x06001F74 RID: 8052 RVA: 0x001B21A3 File Offset: 0x001B03A3
	public int CompareTo(TimelineEvent otherObject)
	{
		if (this.happenedAt == otherObject.happenedAt)
		{
			return this.eventID.CompareTo(otherObject.eventID);
		}
		return this.happenedAt.CompareTo(otherObject.happenedAt);
	}

	// Token: 0x0400294B RID: 10571
	public string name = "TimelineEvent";

	// Token: 0x0400294C RID: 10572
	public string detail = "TimelieDetail";

	// Token: 0x0400294D RID: 10573
	[NonSerialized]
	public bool intialised;

	// Token: 0x0400294E RID: 10574
	public TimelineEvent.EventType eventType;

	// Token: 0x0400294F RID: 10575
	public bool isSelfLocational;

	// Token: 0x04002950 RID: 10576
	public bool isGlobalEvent;

	// Token: 0x04002951 RID: 10577
	public int eventID;

	// Token: 0x04002952 RID: 10578
	public static int assignEventID;

	// Token: 0x04002953 RID: 10579
	public NewNode location;

	// Token: 0x04002954 RID: 10580
	public float happenedAt;

	// Token: 0x04002955 RID: 10581
	public float timeAccuracy = 1f;

	// Token: 0x04002956 RID: 10582
	[NonSerialized]
	public float totalSuspicion;

	// Token: 0x04002957 RID: 10583
	public bool calledUpon;

	// Token: 0x04002958 RID: 10584
	[NonSerialized]
	public EventTime eventTime;

	// Token: 0x04002959 RID: 10585
	[NonSerialized]
	public List<TimelineEvent> childEvents = new List<TimelineEvent>();

	// Token: 0x0400295A RID: 10586
	[NonSerialized]
	public TimelineEvent parentEvent;

	// Token: 0x0400295B RID: 10587
	public bool discoveredByQuestioned;

	// Token: 0x0400295F RID: 10591
	public int debugLocationID = -1;

	// Token: 0x04002960 RID: 10592
	public string debugLocationName;

	// Token: 0x020005A0 RID: 1440
	public enum EventType
	{
		// Token: 0x04002962 RID: 10594
		sightingStreet,
		// Token: 0x04002963 RID: 10595
		sightingWindow,
		// Token: 0x04002964 RID: 10596
		sightingHere,
		// Token: 0x04002965 RID: 10597
		sightingArrive,
		// Token: 0x04002966 RID: 10598
		sightingDepart,
		// Token: 0x04002967 RID: 10599
		selfArrive,
		// Token: 0x04002968 RID: 10600
		selfDepart,
		// Token: 0x04002969 RID: 10601
		wakeUp,
		// Token: 0x0400296A RID: 10602
		goToBed,
		// Token: 0x0400296B RID: 10603
		heardSound,
		// Token: 0x0400296C RID: 10604
		nonPersonSighting,
		// Token: 0x0400296D RID: 10605
		smell,
		// Token: 0x0400296E RID: 10606
		questioned,
		// Token: 0x0400296F RID: 10607
		delayBegin,
		// Token: 0x04002970 RID: 10608
		delayEnd,
		// Token: 0x04002971 RID: 10609
		timeOfDeath,
		// Token: 0x04002972 RID: 10610
		sightingWentToBed,
		// Token: 0x04002973 RID: 10611
		sightingWokeUp,
		// Token: 0x04002974 RID: 10612
		forcedEntryInvestigate
	}

	// Token: 0x020005A1 RID: 1441
	// (Invoke) Token: 0x06001F76 RID: 8054
	public delegate void OnNameChange();

	// Token: 0x020005A2 RID: 1442
	// (Invoke) Token: 0x06001F7A RID: 8058
	public delegate void RecallAccuracyChange();

	// Token: 0x020005A3 RID: 1443
	// (Invoke) Token: 0x06001F7E RID: 8062
	public delegate void OnCalledUponTimeUpdate();
}
