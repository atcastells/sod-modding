using System;
using UnityEngine;

// Token: 0x0200059C RID: 1436
public class EventTime
{
	// Token: 0x14000039 RID: 57
	// (add) Token: 0x06001F5F RID: 8031 RVA: 0x001B192C File Offset: 0x001AFB2C
	// (remove) Token: 0x06001F60 RID: 8032 RVA: 0x001B1964 File Offset: 0x001AFB64
	public event EventTime.OnCalledUponTimeUpdate OnCalledUponTimeUpdated;

	// Token: 0x06001F61 RID: 8033 RVA: 0x001B199C File Offset: 0x001AFB9C
	public EventTime(TimelineEvent newParent, bool forceAccuracy = false, int forceAccuracyToMinutes = 0, bool forceRange = false, float forcedFrom = 0f, float forcedTo = 0f)
	{
		this.parentMemory = newParent;
		this.forcedAccuracy = forceAccuracy;
		this.forcedAccuracyToMinutes = forceAccuracyToMinutes;
		this.forcedRange = forceRange;
		this.forcedTimeRange = new Vector2(forcedFrom, forcedTo);
		this.CalculateTimings();
		if (!forceAccuracy && !forceRange)
		{
			this.parentMemory.OnRecallAccuracyChange += this.CalculateTimings;
		}
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x001B1A38 File Offset: 0x001AFC38
	public void CalculateTimings()
	{
		if (this.forcedRange)
		{
			this.roundedTo = 0.083333336f;
			this.timeStart = this.forcedTimeRange.x;
			this.timeEnd = this.forcedTimeRange.y;
		}
		else if (this.forcedAccuracy)
		{
			this.roundedTo = (float)this.forcedAccuracyToMinutes / 60f;
			this.timeStart = (float)Mathf.FloorToInt(this.parentMemory.happenedAt / this.roundedTo) * this.roundedTo;
			this.timeEnd = this.timeStart + this.roundedTo;
		}
		else if (this.parentMemory.timeAccuracy > 0.8f)
		{
			this.recallAccuracy = EventTime.RecallAccuracy.veryHigh;
			this.roundedTo = SocialControls.Instance.accuracy1 / 60f;
			this.timeStart = (float)Mathf.FloorToInt(this.parentMemory.happenedAt / this.roundedTo) * this.roundedTo;
			this.timeEnd = this.timeStart + this.roundedTo;
		}
		else if (this.parentMemory.timeAccuracy > 0.6f && this.parentMemory.timeAccuracy <= 0.8f)
		{
			this.recallAccuracy = EventTime.RecallAccuracy.high;
			this.roundedTo = SocialControls.Instance.accuracy2 / 60f;
			this.timeStart = (float)Mathf.FloorToInt(this.parentMemory.happenedAt / this.roundedTo) * this.roundedTo;
			this.timeEnd = this.timeStart + this.roundedTo;
		}
		else if (this.parentMemory.timeAccuracy > 0.4f && this.parentMemory.timeAccuracy <= 0.6f)
		{
			this.recallAccuracy = EventTime.RecallAccuracy.med;
			this.roundedTo = SocialControls.Instance.accuracy3 / 60f;
			this.timeStart = (float)Mathf.FloorToInt(this.parentMemory.happenedAt / this.roundedTo) * this.roundedTo;
			this.timeEnd = this.timeStart + this.roundedTo;
		}
		else if (this.parentMemory.timeAccuracy > 0.2f && this.parentMemory.timeAccuracy <= 0.4f)
		{
			this.recallAccuracy = EventTime.RecallAccuracy.low;
			this.roundedTo = SocialControls.Instance.accuracy4 / 60f;
			this.timeStart = (float)Mathf.FloorToInt(this.parentMemory.happenedAt / this.roundedTo) * this.roundedTo;
			this.timeEnd = this.timeStart + this.roundedTo;
		}
		else
		{
			this.recallAccuracy = EventTime.RecallAccuracy.veryLow;
			this.roundedTo = SocialControls.Instance.accuracy5 / 60f;
			this.timeStart = (float)Mathf.FloorToInt(this.parentMemory.happenedAt / this.roundedTo) * this.roundedTo;
			this.timeEnd = this.timeStart + this.roundedTo;
		}
		this.startString = SessionData.Instance.GameTimeToClock24String(this.timeStart, true);
		this.endString = SessionData.Instance.GameTimeToClock24String(this.timeEnd, true);
		if (this.parentMemory != null)
		{
			this.accurateString = SessionData.Instance.GameTimeToClock24String(this.parentMemory.happenedAt, true);
		}
		this.timeRange = new Vector2(this.timeStart, this.timeEnd);
		this.timeMidPoint = (this.timeEnd + this.timeStart) * 0.5f;
		if (this.OnCalledUponTimeUpdated != null)
		{
			this.OnCalledUponTimeUpdated();
		}
	}

	// Token: 0x04002936 RID: 10550
	public TimelineEvent parentMemory;

	// Token: 0x04002937 RID: 10551
	public bool forcedAccuracy;

	// Token: 0x04002938 RID: 10552
	public int forcedAccuracyToMinutes;

	// Token: 0x04002939 RID: 10553
	public bool forcedRange;

	// Token: 0x0400293A RID: 10554
	public Vector2 forcedTimeRange = Vector2.zero;

	// Token: 0x0400293B RID: 10555
	public float timeStart;

	// Token: 0x0400293C RID: 10556
	public float timeEnd;

	// Token: 0x0400293D RID: 10557
	public float timeMidPoint;

	// Token: 0x0400293E RID: 10558
	public Vector2 timeRange;

	// Token: 0x0400293F RID: 10559
	public string accurateString = string.Empty;

	// Token: 0x04002940 RID: 10560
	public string startString = string.Empty;

	// Token: 0x04002941 RID: 10561
	public string endString = string.Empty;

	// Token: 0x04002942 RID: 10562
	public float roundedTo = 0.1f;

	// Token: 0x04002943 RID: 10563
	public EventTime.RecallAccuracy recallAccuracy;

	// Token: 0x0200059D RID: 1437
	public enum RecallAccuracy
	{
		// Token: 0x04002946 RID: 10566
		veryLow,
		// Token: 0x04002947 RID: 10567
		low,
		// Token: 0x04002948 RID: 10568
		med,
		// Token: 0x04002949 RID: 10569
		high,
		// Token: 0x0400294A RID: 10570
		veryHigh
	}

	// Token: 0x0200059E RID: 1438
	// (Invoke) Token: 0x06001F64 RID: 8036
	public delegate void OnCalledUponTimeUpdate();
}
