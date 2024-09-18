using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000229 RID: 553
public class AnimationFrameReference : MonoBehaviour
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x06000C83 RID: 3203 RVA: 0x000B1DBC File Offset: 0x000AFFBC
	public static AnimationFrameReference Instance
	{
		get
		{
			return AnimationFrameReference._instance;
		}
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x000B1DC3 File Offset: 0x000AFFC3
	private void Awake()
	{
		if (AnimationFrameReference._instance != null && AnimationFrameReference._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		AnimationFrameReference._instance = this;
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x000B1DF4 File Offset: 0x000AFFF4
	public AnimationFrameReference.AnimationReference GetAnimationReference(CitizenAnimationController.ArmsBoolSate arms, string seed)
	{
		List<AnimationFrameReference.AnimationReference> list = this.reference.FindAll((AnimationFrameReference.AnimationReference item) => item.isArms && item.arms == arms);
		if (list.Count <= 0)
		{
			return this.reference.Find((AnimationFrameReference.AnimationReference item) => item.isArms && item.arms == CitizenAnimationController.ArmsBoolSate.none);
		}
		return list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, seed, false)];
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x000B1E74 File Offset: 0x000B0074
	public AnimationFrameReference.AnimationReference GetAnimationReference(CitizenAnimationController.IdleAnimationState idle, string seed)
	{
		List<AnimationFrameReference.AnimationReference> list = this.reference.FindAll((AnimationFrameReference.AnimationReference item) => !item.isArms && item.idle == idle);
		if (list.Count <= 0)
		{
			return null;
		}
		return list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, seed, false)];
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x000B1ECA File Offset: 0x000B00CA
	[Button(null, 0)]
	public void CaptureState()
	{
		this.captureFrom == null;
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x000B1ECA File Offset: 0x000B00CA
	[Button(null, 0)]
	public void CaptureWalkingState()
	{
		this.captureFrom == null;
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x000B1ECA File Offset: 0x000B00CA
	[Button(null, 0)]
	public void CaptureRunningState()
	{
		this.captureFrom == null;
	}

	// Token: 0x04000E21 RID: 3617
	[Header("Database")]
	public List<AnimationFrameReference.AnimationReference> reference = new List<AnimationFrameReference.AnimationReference>();

	// Token: 0x04000E22 RID: 3618
	public List<AnimationFrameReference.AnimationReference> walkingReference = new List<AnimationFrameReference.AnimationReference>();

	// Token: 0x04000E23 RID: 3619
	public List<AnimationFrameReference.AnimationReference> runningReference = new List<AnimationFrameReference.AnimationReference>();

	// Token: 0x04000E24 RID: 3620
	[Header("Capture")]
	public CitizenOutfitController captureFrom;

	// Token: 0x04000E25 RID: 3621
	public CitizenAnimationController.IdleAnimationState captureIdle;

	// Token: 0x04000E26 RID: 3622
	public CitizenAnimationController.ArmsBoolSate captureArms;

	// Token: 0x04000E27 RID: 3623
	private static AnimationFrameReference _instance;

	// Token: 0x0200022A RID: 554
	[Serializable]
	public class AnimationReference
	{
		// Token: 0x04000E28 RID: 3624
		public string name;

		// Token: 0x04000E29 RID: 3625
		public bool isArms;

		// Token: 0x04000E2A RID: 3626
		public CitizenAnimationController.IdleAnimationState idle;

		// Token: 0x04000E2B RID: 3627
		public CitizenAnimationController.ArmsBoolSate arms;

		// Token: 0x04000E2C RID: 3628
		public List<AnimationFrameReference.AnimationAnchorRef> anim;
	}

	// Token: 0x0200022B RID: 555
	[Serializable]
	public class AnimationAnchorRef
	{
		// Token: 0x04000E2D RID: 3629
		public CitizenOutfitController.CharacterAnchor anchor;

		// Token: 0x04000E2E RID: 3630
		public Vector3 localPos;

		// Token: 0x04000E2F RID: 3631
		public Quaternion localRot;
	}
}
