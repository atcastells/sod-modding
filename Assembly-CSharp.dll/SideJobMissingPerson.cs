using System;
using UnityEngine;

// Token: 0x0200031B RID: 795
[Serializable]
public class SideJobMissingPerson : SideJob
{
	// Token: 0x06001209 RID: 4617 RVA: 0x000FFD84 File Offset: 0x000FDF84
	public SideJobMissingPerson(JobPreset newPreset, SideJobController.JobPickData newData, bool immediatePost) : base(newPreset, newData, immediatePost)
	{
	}

	// Token: 0x0600120A RID: 4618 RVA: 0x00101086 File Offset: 0x000FF286
	public override void PostJob()
	{
		if (this.readyToPost)
		{
			base.PostJob();
		}
	}

	// Token: 0x0600120B RID: 4619 RVA: 0x00101098 File Offset: 0x000FF298
	public override void AcceptJob()
	{
		base.AcceptJob();
		this.OnAcquireJobInfo(string.Empty);
		this.thisCase.handIn.Clear();
		foreach (NewNode.NodeAccess nodeAccess in this.poster.home.entrances)
		{
			if (nodeAccess.door != null && nodeAccess.door.peekInteractable != null)
			{
				this.thisCase.handIn.Add(-nodeAccess.door.wall.id);
			}
		}
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x0010114C File Offset: 0x000FF34C
	public override void GameWorldLoop()
	{
		base.GameWorldLoop();
		if (!this.readyToPost)
		{
			if (this.motiveStr == "Suicide-Depression" || this.motiveStr == "Suicide-Debt")
			{
				if (this.exitBuilding == null)
				{
					this.exitBuilding = this.purp.ai.CreateNewGoal(RoutineControls.Instance.exitBuilding, 0f, 0f, null, null, null, null, null, -2);
					Game.Log("Jobs: Created exit building goal for " + this.purp.name, 2);
				}
				else if (!this.purp.ai.goals.Contains(this.exitBuilding))
				{
					Game.Log("Jobs: Disappearing " + this.purp.name, 2);
					this.purp.RemoveFromWorld(true);
					this.readyToPost = true;
				}
			}
		}
		else if (this.readyToPost && this.post == null)
		{
			this.PostJob();
		}
		if (this.thisCase != null && this.thisCase.isActive && this.phase == 0 && this.phase != this.phaseChange)
		{
			this.phaseChange = this.phase;
		}
	}

	// Token: 0x0400161A RID: 5658
	[Header("Saved Values")]
	public bool readyToPost;

	// Token: 0x0400161B RID: 5659
	[Header("Unsaved Values")]
	private NewAIGoal exitBuilding;
}
