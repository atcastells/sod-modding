using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002C4 RID: 708
public class RigidbodyDragObject : MonoBehaviour
{
	// Token: 0x06000FA7 RID: 4007 RVA: 0x000DE560 File Offset: 0x000DC760
	public void OnEnterRagdollState(NewAIController newAI)
	{
		this.ai = newAI;
		this._cam = CameraController.Instance.cam;
		this.mask = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			24
		});
		this.draggableDistance = GameplayControls.Instance.interactionRange * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.reachModifier) + 0.2f);
		if (Game.Instance.printDebug)
		{
			Game.Log("Player: " + this.ai.human.GetCitizenName() + " is entering a ragdoll state that can be manipulated by the player...", 2);
		}
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x000DE600 File Offset: 0x000DC800
	public void OnExitRagdollState()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("Player: " + this.ai.human.GetCitizenName() + " is exiting a ragdoll state and can no longer be manipulated by the player", 2);
		}
		if (this.dragIsActive)
		{
			this.CancelDrag();
		}
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x000DE64C File Offset: 0x000DC84C
	public void OnAttemptPlayerInteraction()
	{
		if (this.GetRigidbodyFromCamera(out this.targetRigidbody, out this._dragDistance, out this._screenTargetPos, out this._rigidbodyPos))
		{
			if (InteractionController.Instance.currentlyDragging != null && InteractionController.Instance.currentlyDragging != this)
			{
				InteractionController.Instance.currentlyDragging.CancelDrag();
			}
			if (Game.Instance.printDebug)
			{
				Game.Log("Player: " + this.ai.human.GetCitizenName() + " is dragging a ragdoll", 2);
			}
			FirstPersonItemController.Instance.ForceHolster();
			this.dragIsActive = true;
			base.enabled = true;
			this.UpdateMousePositionOffset();
			InteractionController.Instance.currentlyDragging = this;
			if (this.ai.human.outfitController != null)
			{
				foreach (KeyValuePair<CitizenOutfitController.CharacterAnchor, Transform> keyValuePair in this.ai.human.outfitController.anchorReference)
				{
					if (keyValuePair.Key != CitizenOutfitController.CharacterAnchor.Glasses && keyValuePair.Key != CitizenOutfitController.CharacterAnchor.Hat)
					{
						Collider component = keyValuePair.Value.gameObject.GetComponent<Collider>();
						if (component != null)
						{
							Physics.IgnoreCollision(component, Player.Instance.fps.m_CharacterController, true);
						}
					}
				}
			}
			InteractionController.Instance.UpdateInteractionText();
			return;
		}
		if (this.dragIsActive)
		{
			this.CancelDrag();
		}
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x000DE7D4 File Offset: 0x000DC9D4
	public void CancelDrag()
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("Player: " + this.ai.human.GetCitizenName() + " has stopped dragging a ragdoll", 2);
		}
		this.targetRigidbody = null;
		this.dragIsActive = false;
		base.enabled = false;
		if (this.ai.human.outfitController != null)
		{
			foreach (KeyValuePair<CitizenOutfitController.CharacterAnchor, Transform> keyValuePair in this.ai.human.outfitController.anchorReference)
			{
				if (keyValuePair.Key != CitizenOutfitController.CharacterAnchor.Glasses && keyValuePair.Key != CitizenOutfitController.CharacterAnchor.Hat)
				{
					Collider component = keyValuePair.Value.gameObject.GetComponent<Collider>();
					if (component != null)
					{
						Physics.IgnoreCollision(component, Player.Instance.fps.m_CharacterController, false);
					}
				}
			}
		}
		InteractionController.Instance.currentlyDragging = null;
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x000DE8EC File Offset: 0x000DCAEC
	private void Update()
	{
		if (this.dragIsActive)
		{
			if (this.targetRigidbody != null)
			{
				this.UpdateMousePositionOffset();
				if (this.ai.human.isDead)
				{
					Player.Instance.AddHygiene(-0.01f * Time.deltaTime);
				}
				InteractionController.Instance.SetIllegalActionActive(true);
			}
			if (InputController.Instance.player.GetButtonUp("Primary") || !SessionData.Instance.play || !this.ai.isRagdoll || Vector3.Distance(this._cam.transform.position, this.targetRigidbody.transform.position) > this.draggableDistance * 1.2f)
			{
				this.CancelDrag();
			}
		}
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x000DE9AC File Offset: 0x000DCBAC
	public bool IsValidRagdollDragable()
	{
		Rigidbody rigidbody;
		float num;
		Vector3 vector;
		Vector3 vector2;
		return Game.Instance.allowDraggableRagdolls && !(this.ai == null) && this.ai.isRagdoll && this.GetRigidbodyFromCamera(out rigidbody, out num, out vector, out vector2);
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x000DE9FC File Offset: 0x000DCBFC
	private void FixedUpdate()
	{
		if (this.targetRigidbody != null)
		{
			this.targetRigidbody.velocity = (this._rigidbodyPos + this.mousePositionOffset - this.targetRigidbody.transform.position) * (GameplayControls.Instance.dragForceAmount + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.increaseInventory) * 8f) * Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x000DEA6E File Offset: 0x000DCC6E
	private void UpdateMousePositionOffset()
	{
		this.mousePositionOffset = this._cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, this._dragDistance)) - this._screenTargetPos;
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x000DEAAC File Offset: 0x000DCCAC
	private bool GetRigidbodyFromCamera(out Rigidbody targetedRigidbody, out float dragDistance, out Vector3 screenTargetPos, out Vector3 rigidBodyPos)
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("Player: " + this.ai.human.GetCitizenName() + " checking for ragdoll player manipulation...", 2);
		}
		targetedRigidbody = null;
		dragDistance = 0f;
		screenTargetPos = Vector3.zero;
		rigidBodyPos = Vector3.zero;
		Ray ray = this._cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, ref raycastHit, this.draggableDistance, this.mask))
		{
			Citizen componentInParent = raycastHit.transform.gameObject.GetComponentInParent<Citizen>();
			if (componentInParent != null && componentInParent == this.ai.human && componentInParent.ai.isRagdoll)
			{
				targetedRigidbody = raycastHit.collider.gameObject.GetComponent<Rigidbody>();
				if (targetedRigidbody != null)
				{
					dragDistance = Mathf.Min(Vector3.Distance(ray.origin, raycastHit.point), GameplayControls.Instance.ragdollCarryMaxDistance);
					screenTargetPos = this._cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDistance));
					rigidBodyPos = raycastHit.collider.transform.position;
					targetedRigidbody = raycastHit.collider.gameObject.GetComponent<Rigidbody>();
					if (targetedRigidbody != null)
					{
						return true;
					}
				}
			}
		}
		if (Game.Instance.printDebug)
		{
			Game.Log("Player: Unable to find valid ragdoll component", 2);
		}
		return false;
	}

	// Token: 0x04001330 RID: 4912
	[Tooltip("Reference to the AI controller object attached to the citizen")]
	[Header("References")]
	public NewAIController ai;

	// Token: 0x04001331 RID: 4913
	public Rigidbody targetRigidbody;

	// Token: 0x04001332 RID: 4914
	private Camera _cam;

	// Token: 0x04001333 RID: 4915
	private Vector3 _screenTargetPos;

	// Token: 0x04001334 RID: 4916
	private Vector3 _rigidbodyPos;

	// Token: 0x04001335 RID: 4917
	public Vector3 mousePositionOffset;

	// Token: 0x04001336 RID: 4918
	private float _dragDistance;

	// Token: 0x04001337 RID: 4919
	public LayerMask mask;

	// Token: 0x04001338 RID: 4920
	public float draggableDistance;

	// Token: 0x04001339 RID: 4921
	public bool dragIsActive;
}
