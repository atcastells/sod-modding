using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000406 RID: 1030
public class LaserGridController : SwitchSyncBehaviour
{
	// Token: 0x06001795 RID: 6037 RVA: 0x00162354 File Offset: 0x00160554
	private void Awake()
	{
		this.cycle = Toolbox.Instance.Rand(0f, 1f, false);
		this.randomMultiplier = Toolbox.Instance.Rand(0.8f, 1.2f, false);
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x0016238C File Offset: 0x0016058C
	private void Update()
	{
		if (this.isOn)
		{
			if (SessionData.Instance.play)
			{
				if (!this.bounce)
				{
					if (this.cycle < 1f)
					{
						this.cycle += Time.deltaTime * this.speed * this.randomMultiplier;
					}
					else
					{
						this.bounce = true;
					}
				}
				else if (this.bounce)
				{
					if (this.cycle > 0f)
					{
						this.cycle -= Time.deltaTime * this.speed * this.randomMultiplier;
					}
					else
					{
						this.bounce = false;
					}
				}
				Vector3 localPosition = this.movementParent.localPosition;
				if (this.useMovementX)
				{
					localPosition.x = this.movementX.Evaluate(this.cycle);
				}
				if (this.useMovementY)
				{
					localPosition.y = this.movementY.Evaluate(this.cycle);
				}
				if (this.useMovementZ)
				{
					localPosition.z = this.movementZ.Evaluate(this.cycle);
				}
				Vector3 localEulerAngles = this.movementParent.localEulerAngles;
				if (this.useRotationX)
				{
					localEulerAngles.x = this.rotationX.Evaluate(this.cycle);
				}
				if (this.useRotationY)
				{
					localEulerAngles.y = this.rotationY.Evaluate(this.cycle);
				}
				if (this.useRotationZ)
				{
					localEulerAngles.z = this.rotationZ.Evaluate(this.cycle);
				}
				this.movementParent.localPosition = localPosition;
				this.movementParent.localEulerAngles = localEulerAngles;
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(this.laserParent.transform.position, this.laserParent.transform.forward), ref raycastHit, this.range, Toolbox.Instance.aiSightingLayerMask, 2))
			{
				Transform parent = this.laser.transform.parent;
				this.laser.transform.parent = null;
				this.laser.transform.localScale = new Vector3(this.laser.transform.localScale.x, this.laser.transform.localScale.y, raycastHit.distance);
				this.laser.transform.parent = parent;
				Human human = null;
				Player componentInChildren = raycastHit.transform.GetComponentInChildren<Player>();
				if (componentInChildren != null && !Game.Instance.invisiblePlayer)
				{
					human = componentInChildren;
				}
				else
				{
					Citizen componentInChildren2 = raycastHit.transform.GetComponentInChildren<Citizen>();
					if (componentInChildren2 != null)
					{
						human = componentInChildren2;
					}
				}
				if (human != null && !SessionData.Instance.isFloorEdit)
				{
					human.UpdateTrespassing(true);
					if (human.isTrespassing && human.trespassingEscalation > 1)
					{
						human.currentNode.gameLocation.thisAsAddress.SetAlarm(true, human);
						return;
					}
				}
			}
			else
			{
				Transform parent2 = this.laser.transform.parent;
				this.laser.transform.parent = null;
				this.laser.transform.localScale = new Vector3(this.laser.transform.localScale.x, this.laser.transform.localScale.y, this.range);
				this.laser.transform.parent = parent2;
			}
		}
	}

	// Token: 0x04001CDE RID: 7390
	[Header("Components")]
	public Transform movementParent;

	// Token: 0x04001CDF RID: 7391
	public Transform laserParent;

	// Token: 0x04001CE0 RID: 7392
	public Transform laser;

	// Token: 0x04001CE1 RID: 7393
	public InteractableController controller;

	// Token: 0x04001CE2 RID: 7394
	[Header("Settings")]
	public float speed = 1f;

	// Token: 0x04001CE3 RID: 7395
	public float range = 4.78f;

	// Token: 0x04001CE4 RID: 7396
	public bool useMovementX;

	// Token: 0x04001CE5 RID: 7397
	[EnableIf("useMovementX")]
	public AnimationCurve movementX;

	// Token: 0x04001CE6 RID: 7398
	public bool useMovementY;

	// Token: 0x04001CE7 RID: 7399
	[EnableIf("useMovementY")]
	public AnimationCurve movementY;

	// Token: 0x04001CE8 RID: 7400
	public bool useMovementZ;

	// Token: 0x04001CE9 RID: 7401
	[EnableIf("useMovementZ")]
	public AnimationCurve movementZ;

	// Token: 0x04001CEA RID: 7402
	public bool useRotationX;

	// Token: 0x04001CEB RID: 7403
	[EnableIf("useRotationX")]
	public AnimationCurve rotationX;

	// Token: 0x04001CEC RID: 7404
	public bool useRotationY;

	// Token: 0x04001CED RID: 7405
	[EnableIf("useRotationY")]
	public AnimationCurve rotationY;

	// Token: 0x04001CEE RID: 7406
	public bool useRotationZ;

	// Token: 0x04001CEF RID: 7407
	[EnableIf("useRotationZ")]
	public AnimationCurve rotationZ;

	// Token: 0x04001CF0 RID: 7408
	[Header("State")]
	public float cycle;

	// Token: 0x04001CF1 RID: 7409
	public bool bounce;

	// Token: 0x04001CF2 RID: 7410
	public float randomMultiplier;
}
