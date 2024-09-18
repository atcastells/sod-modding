using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002BB RID: 699
public class DoorMovementController : MonoBehaviour
{
	// Token: 0x06000F7C RID: 3964 RVA: 0x000DC76C File Offset: 0x000DA96C
	private void Start()
	{
		if (!this.isSetup)
		{
			InteractableController componentInParent = base.gameObject.GetComponentInParent<InteractableController>();
			if (componentInParent != null && componentInParent.interactable != null)
			{
				this.Setup(componentInParent.interactable, false);
			}
		}
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x000DC7AC File Offset: 0x000DA9AC
	public void Setup(Interactable newInteractable, bool inheritOpenStatusFromInteractable = true)
	{
		this.interactable = newInteractable;
		if (this.door == null)
		{
			this.door = base.transform;
		}
		if (this.door != null && this.removePlayerCollisionsWhileAnimating)
		{
			Collider[] componentsInChildren = this.door.GetComponentsInChildren<Collider>();
			this.spawnedDoorColliders.Clear();
			foreach (Collider collider in componentsInChildren)
			{
				this.spawnedDoorColliders.Add(collider, collider.gameObject.layer);
			}
		}
		this.closedLocalPos = this.door.transform.localPosition + this.preset.closedRelativePos;
		this.openLocalPos = this.door.transform.localPosition + this.preset.openRelativePos;
		this.closedLocalEuler = this.door.transform.localEulerAngles + this.preset.closedRelativeEuler;
		this.openLocalEuler = this.door.transform.localEulerAngles + this.preset.openRelativeEuler;
		this.closedLocalScale = this.door.transform.localScale + this.preset.closedRelativeScale;
		this.openLocalScale = this.door.transform.localScale + this.preset.openRelativeScale;
		if (inheritOpenStatusFromInteractable)
		{
			if (this.interactable.sw0)
			{
				this.SetOpen(1f, null, true);
			}
			else if (!this.interactable.sw0)
			{
				this.SetOpen(0f, null, true);
			}
		}
		this.SetDoorPosition();
		this.isSetup = true;
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x000DC95C File Offset: 0x000DAB5C
	public virtual void SetOpen(float newAjar, Actor interactor, bool skipAnimation = false)
	{
		newAjar = (float)Mathf.RoundToInt(newAjar * 100f) / 100f;
		this.desiredTransition = Mathf.Clamp(newAjar, 0f, 1f);
		this.desiredEuler = Vector3.Lerp(this.closedLocalEuler, this.openLocalEuler, this.desiredTransition);
		this.desiredPos = Vector3.Lerp(this.closedLocalPos, this.openLocalPos, this.desiredTransition);
		this.desiredScale = Vector3.Lerp(this.closedLocalScale, this.openLocalScale, this.desiredTransition);
		if (skipAnimation || !base.gameObject.activeInHierarchy)
		{
			this.currentTransition = this.desiredTransition;
			this.door.localEulerAngles = this.desiredEuler;
			this.door.localPosition = this.desiredPos;
			this.door.localScale = this.desiredScale;
			if (this.desiredTransition == (float)Mathf.Abs(0))
			{
				this.OnClose(interactor, false);
				return;
			}
		}
		else
		{
			if (this.interactable != null)
			{
				if (this.desiredTransition < this.currentTransition && this.preset.closeAction != null)
				{
					AudioController.Instance.PlayWorldOneShot(this.preset.closeAction, interactor, this.interactable.node, base.transform.position, null, null, 1f, null, this.preset.ignoreOcclusion, null, false);
				}
				else if (this.desiredTransition > this.currentTransition && this.preset.openAction != null)
				{
					AudioController.Instance.PlayWorldOneShot(this.preset.openAction, interactor, this.interactable.node, base.transform.position, null, null, 1f, null, this.preset.ignoreOcclusion, null, false);
				}
			}
			base.StopAllCoroutines();
			base.StartCoroutine(this.OpenDoor(interactor));
		}
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x000DCB3C File Offset: 0x000DAD3C
	public void SetCollisionsWithPlayerActive(bool val)
	{
		if (val)
		{
			using (Dictionary<Collider, int>.Enumerator enumerator = this.spawnedDoorColliders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<Collider, int> keyValuePair = enumerator.Current;
					keyValuePair.Key.gameObject.layer = keyValuePair.Value;
				}
				return;
			}
		}
		foreach (KeyValuePair<Collider, int> keyValuePair2 in this.spawnedDoorColliders)
		{
			keyValuePair2.Key.gameObject.layer = 6;
		}
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x000DCBF0 File Offset: 0x000DADF0
	private void OnEnable()
	{
		if (this.isAnimating)
		{
			base.StartCoroutine(this.OpenDoor(null));
		}
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x000DCC08 File Offset: 0x000DAE08
	private IEnumerator OpenDoor(Actor interactor)
	{
		this.isAnimating = true;
		if (this.removePlayerCollisionsWhileAnimating)
		{
			this.SetCollisionsWithPlayerActive(false);
		}
		if (this.preset.switchState1AnimationSync && this.interactable != null)
		{
			this.interactable.SetCustomState1(true, null, true, false, false);
		}
		while (this.interactable != null && this.desiredTransition != this.currentTransition)
		{
			if (SessionData.Instance.play && this.isAnimating)
			{
				if (this.preset.collisionBehaviour != DoorMovementPreset.PhysicsBehaviour.ignore && this.interactable != null && this.interactable.furnitureParent != null)
				{
					foreach (NewNode newNode in this.interactable.furnitureParent.coversNodes)
					{
						foreach (Interactable interactable in newNode.interactables)
						{
							if (interactable.preset.physicsProfile != null && interactable.preset.reactWithExternalStimuli && interactable.controller != null && !interactable.controller.physicsOn)
							{
								Game.Log("Object: Set physics on for " + interactable.name, 2);
								interactable.controller.SetPhysics(true, null);
							}
						}
						foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in newNode.accessToOtherNodes)
						{
							foreach (Interactable interactable2 in keyValuePair.Key.interactables)
							{
								if (interactable2.preset.physicsProfile != null && interactable2.preset.reactWithExternalStimuli && interactable2.controller != null && !interactable2.controller.physicsOn)
								{
									Game.Log("Object: Set physics on for " + interactable2.name, 2);
									interactable2.controller.SetPhysics(true, null);
								}
							}
						}
					}
				}
				if (this.currentTransition < this.desiredTransition)
				{
					this.currentTransition += Time.deltaTime * this.preset.doorOpenSpeed;
					if (this.currentTransition > this.desiredTransition)
					{
						this.currentTransition = this.desiredTransition;
					}
					this.isOpening = true;
					this.isClosing = false;
				}
				else if (this.currentTransition > this.desiredTransition)
				{
					this.currentTransition -= Time.deltaTime * this.preset.doorCloseSpeed;
					if (this.currentTransition < this.desiredTransition)
					{
						this.currentTransition = this.desiredTransition;
					}
					this.isClosing = true;
					this.isOpening = false;
				}
				this.SetDoorPosition();
				if (this.updateLoopingParams)
				{
					this.interactable.UpdateLoopingAudioParams();
				}
			}
			yield return null;
		}
		this.isAnimating = false;
		this.isOpening = false;
		this.isClosing = false;
		if (this.preset.switchState1AnimationSync && this.interactable != null)
		{
			this.interactable.SetCustomState1(false, null, true, false, false);
		}
		if (this.desiredTransition == (float)Mathf.Abs(0))
		{
			this.OnClose(interactor, true);
		}
		if (this.desiredTransition == (float)Mathf.Abs(1))
		{
			this.OnOpen(interactor, true);
		}
		yield break;
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x000DCC20 File Offset: 0x000DAE20
	public void SetDoorPosition()
	{
		float num = this.preset.animationCurve.Evaluate(Mathf.Clamp01(this.currentTransition));
		this.door.localEulerAngles = Vector3.Lerp(this.closedLocalEuler, this.openLocalEuler, num);
		this.door.localPosition = Vector3.Lerp(this.closedLocalPos, this.openLocalPos, num);
		this.door.localScale = Vector3.Lerp(this.closedLocalScale, this.openLocalScale, num);
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x000DCCA0 File Offset: 0x000DAEA0
	public void OnClose(Actor interactor, bool playSound = true)
	{
		this.isOpen = false;
		this.isAnimating = false;
		this.isOpening = false;
		this.isClosing = false;
		if (this.removePlayerCollisionsWhileAnimating)
		{
			this.SetCollisionsWithPlayerActive(true);
		}
		if (this.interactable != null)
		{
			this.interactable.OnDoorMovementClosed();
		}
		if (this.preset.switchState1AnimationSync && this.interactable != null)
		{
			this.interactable.SetCustomState1(false, null, true, false, false);
		}
		if (this.preset.closeFinished != null && playSound)
		{
			AudioController.Instance.PlayWorldOneShot(this.preset.closeFinished, interactor, this.interactable.node, base.transform.position, null, null, 1f, null, this.preset.ignoreOcclusion, null, false);
		}
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x000DCD68 File Offset: 0x000DAF68
	public void OnOpen(Actor interactor, bool playSound = true)
	{
		this.isOpen = true;
		this.isOpening = false;
		this.isClosing = false;
		if (this.interactable != null)
		{
			this.interactable.OnDoorMovementOpened();
		}
		if (this.preset.switchState1AnimationSync && this.interactable != null)
		{
			this.interactable.SetCustomState1(false, null, true, false, false);
		}
		if (this.preset.openFinished != null && playSound)
		{
			AudioController.Instance.PlayWorldOneShot(this.preset.openFinished, interactor, this.interactable.node, base.transform.position, null, null, 1f, null, this.preset.ignoreOcclusion, null, false);
		}
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x000DCE1C File Offset: 0x000DB01C
	public void OnCollisionEnter(Collision collision)
	{
		if (this.preset.collisionBehaviour == DoorMovementPreset.PhysicsBehaviour.ignore)
		{
			return;
		}
		if (Game.Instance.collectDebugData)
		{
			Game.Log(string.Concat(new string[]
			{
				"Door movement ",
				base.name,
				" collision: ",
				collision.gameObject.name,
				": ",
				this.preset.collisionBehaviour.ToString(),
				" @ ",
				collision.GetContact(0).point.ToString()
			}), 2);
		}
		if (this.preset.objectImpact != null)
		{
			NewNode location = null;
			if (this.interactable != null)
			{
				location = this.interactable.node;
			}
			AudioController.Instance.PlayWorldOneShot(this.preset.objectImpact, null, location, base.transform.position, null, null, 1f, null, this.preset.ignoreOcclusion, null, false);
		}
		if (((this.isOpening && this.preset.behaviourAppliesWhenOpening) || (this.isClosing && this.preset.behaviourAppliesWhenClosing)) && this.preset.collisionBehaviour == DoorMovementPreset.PhysicsBehaviour.stopDoorMovement)
		{
			if (collision.rigidbody == null || collision.rigidbody.velocity.magnitude < 0.1f)
			{
				this.isAnimating = false;
				if (this.preset.switchState1AnimationSync && this.interactable != null)
				{
					this.interactable.SetCustomState1(false, null, true, false, false);
					return;
				}
			}
			else
			{
				Game.Log(string.Concat(new string[]
				{
					"Door movement collision: ",
					collision.gameObject.name,
					" is moving too fast to stop the door (",
					collision.rigidbody.velocity.magnitude.ToString(),
					")"
				}), 2);
			}
		}
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x000DD014 File Offset: 0x000DB214
	public void OnCollisionExit(Collision collision)
	{
		if (this.preset.collisionBehaviour == DoorMovementPreset.PhysicsBehaviour.ignore)
		{
			return;
		}
		if (this.desiredTransition != this.currentTransition)
		{
			this.isAnimating = true;
			if (this.preset.switchState1AnimationSync && this.interactable != null)
			{
				this.interactable.SetCustomState1(true, null, true, false, false);
			}
		}
	}

	// Token: 0x040012EA RID: 4842
	[Header("State Positions")]
	public Transform door;

	// Token: 0x040012EB RID: 4843
	[NonSerialized]
	public Interactable interactable;

	// Token: 0x040012EC RID: 4844
	public Dictionary<Collider, int> spawnedDoorColliders = new Dictionary<Collider, int>();

	// Token: 0x040012ED RID: 4845
	[Space(7f)]
	public Vector3 closedLocalPos = Vector3.zero;

	// Token: 0x040012EE RID: 4846
	public Vector3 openLocalPos = Vector3.zero;

	// Token: 0x040012EF RID: 4847
	public Vector3 closedLocalEuler = Vector3.zero;

	// Token: 0x040012F0 RID: 4848
	public Vector3 openLocalEuler = Vector3.zero;

	// Token: 0x040012F1 RID: 4849
	public Vector3 closedLocalScale = Vector3.zero;

	// Token: 0x040012F2 RID: 4850
	public Vector3 openLocalScale = Vector3.zero;

	// Token: 0x040012F3 RID: 4851
	[Space(7f)]
	public Vector3 desiredPos = Vector3.zero;

	// Token: 0x040012F4 RID: 4852
	public Vector3 desiredEuler = Vector3.zero;

	// Token: 0x040012F5 RID: 4853
	public Vector3 desiredScale = Vector3.zero;

	// Token: 0x040012F6 RID: 4854
	[Header("Door State")]
	public DoorMovementPreset preset;

	// Token: 0x040012F7 RID: 4855
	[Tooltip("0 = Closed, 1 = Open")]
	public float desiredTransition;

	// Token: 0x040012F8 RID: 4856
	public float currentTransition;

	// Token: 0x040012F9 RID: 4857
	[Tooltip("True if open")]
	public bool isOpen;

	// Token: 0x040012FA RID: 4858
	public Actor interacting;

	// Token: 0x040012FB RID: 4859
	public bool isAnimating;

	// Token: 0x040012FC RID: 4860
	public bool isSetup;

	// Token: 0x040012FD RID: 4861
	public bool isOpening;

	// Token: 0x040012FE RID: 4862
	public bool isClosing;

	// Token: 0x040012FF RID: 4863
	[Tooltip("If true this will update looping audio params while animating. Useful for fridge doors etc where the animation is tied to a sfx param.")]
	public bool updateLoopingParams;

	// Token: 0x04001300 RID: 4864
	[Tooltip("If true this will remove collisions with player while animating")]
	public bool removePlayerCollisionsWhileAnimating;
}
