using System;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Token: 0x020002C2 RID: 706
public class PlayerUnstuck : MonoBehaviour
{
	// Token: 0x06000FA0 RID: 4000 RVA: 0x000DDF10 File Offset: 0x000DC110
	private void Start()
	{
		this.characterController = base.GetComponent<CharacterController>();
		this.firstPersonController = base.GetComponent<FirstPersonController>();
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x000DDF2C File Offset: 0x000DC12C
	private void Update()
	{
		if (!SessionData.Instance.play || !SessionData.Instance.startedGame || !this.firstPersonController.enableMovement || this.firstPersonController.ghostMovement || CutSceneController.Instance.cutSceneActive || InteractionController.Instance.lockedInInteraction != null || Player.Instance.transitionActive || !this.firstPersonController.m_CharacterController.enabled)
		{
			this.currentAttemptedSecondsOfMovement = 0f;
			return;
		}
		if (Mathf.Abs(this.firstPersonController.m_Input.x) > 0f || (Mathf.Abs(this.firstPersonController.m_Input.y) > 0f && this.previousPosition == base.transform.position))
		{
			Vector3 position = base.transform.position;
			float height = this.characterController.height;
			float radius = this.characterController.radius;
			if (Physics.OverlapCapsule(position + (this.characterController.center + Vector3.up * (-height / 2f)) * (this.ColliderSizePercent / 100f), position + (this.characterController.center + Vector3.up * (height / 2f)) * (this.ColliderSizePercent / 100f), radius * (this.ColliderSizePercent / 100f), this.layerMask, 1).Length != 0)
			{
				this.currentAttemptedSecondsOfMovement += Time.deltaTime;
				if (this.currentAttemptedSecondsOfMovement >= this.secondsUntilUnstuck && this.isAutomatic)
				{
					this.UnstuckTeleportPlayer();
				}
			}
			else
			{
				this.currentAttemptedSecondsOfMovement = 0f;
			}
		}
		this.previousPosition = base.transform.position;
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x000DE108 File Offset: 0x000DC308
	private void UnstuckTeleportPlayer()
	{
		for (float num = 0f; num <= this.maxTeleportDistance; num += 0.001f)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = new Vector3((float)Random.Range(-1, 1), 0f, (float)Random.Range(-1, 1)).normalized * num;
			float height = this.characterController.height;
			float radius = this.characterController.radius;
			if (Physics.OverlapCapsule(position + vector + this.characterController.center + Vector3.up * (-height / 2f), position + vector + this.characterController.center + Vector3.up * (height / 2f), radius, this.layerMask).Length == 0)
			{
				this.firstPersonController.enabled = false;
				base.transform.position = position + vector;
				this.firstPersonController.enabled = true;
				this.currentAttemptedSecondsOfMovement = 0f;
				Debug.Log("Unstuck controller: Teleported Player " + base.transform.position.ToString());
				return;
			}
		}
	}

	// Token: 0x04001325 RID: 4901
	public bool isAutomatic = true;

	// Token: 0x04001326 RID: 4902
	[Range(0f, 10f)]
	public float secondsUntilUnstuck = 2f;

	// Token: 0x04001327 RID: 4903
	[Range(0f, 200f)]
	public float maxTeleportDistance = 150f;

	// Token: 0x04001328 RID: 4904
	[Range(0f, 100f)]
	public float ColliderSizePercent = 90f;

	// Token: 0x04001329 RID: 4905
	private float currentAttemptedSecondsOfMovement;

	// Token: 0x0400132A RID: 4906
	public LayerMask layerMask;

	// Token: 0x0400132B RID: 4907
	private FirstPersonController firstPersonController;

	// Token: 0x0400132C RID: 4908
	private CharacterController characterController;

	// Token: 0x0400132D RID: 4909
	private Vector3 previousPosition;
}
