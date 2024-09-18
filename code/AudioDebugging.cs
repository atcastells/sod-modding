using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020001F4 RID: 500
public class AudioDebugging : MonoBehaviour
{
	// Token: 0x17000066 RID: 102
	// (get) Token: 0x06000B8F RID: 2959 RVA: 0x000A9B35 File Offset: 0x000A7D35
	public static AudioDebugging Instance
	{
		get
		{
			return AudioDebugging._instance;
		}
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x000A9B3C File Offset: 0x000A7D3C
	private void Awake()
	{
		if (AudioDebugging._instance != null && AudioDebugging._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		AudioDebugging._instance = this;
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x000A9B6C File Offset: 0x000A7D6C
	[Button(null, 0)]
	public void SpawnObject()
	{
		if (this.spawnObject != null)
		{
			if (!this.spawnObject.spawnable || !(this.spawnObject.prefab != null))
			{
				Game.Log("Unable to spawn object: This object is not spawnable!", 2);
				return;
			}
			Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(this.spawnObject, Player.Instance, null, null, Player.Instance.transform.position + new Vector3(Toolbox.Instance.Rand(-1f, 1f, false), 0f, Toolbox.Instance.Rand(-1f, 1f, false)), Vector3.zero, null, null, "");
			if (interactable != null)
			{
				interactable.ForcePhysicsActive(false, false, default(Vector3), 2, false);
				return;
			}
		}
		else
		{
			Game.Log("Unable to spawn object: Nothing selected!", 2);
		}
	}

	// Token: 0x04000CBA RID: 3258
	[Header("Debug Controls")]
	public bool overrideSmokeStackEmissionFrequency;

	// Token: 0x04000CBB RID: 3259
	[InfoBox("Controls how often the chem plant smoke plumes appear in in-game minutes (roughly). This will only take effect after the last plume.", 0)]
	[EnableIf("overrideSmokeStackEmissionFrequency")]
	public float chemSmokeStackEmissionFrequency = 4f;

	// Token: 0x04000CBC RID: 3260
	[Space(7f)]
	public bool overrideThunderDelay;

	// Token: 0x04000CBD RID: 3261
	[InfoBox("Controls how often thunder happens in storms", 0)]
	[EnableIf("overrideThunderDelay")]
	public float thunderDelay = 4f;

	// Token: 0x04000CBE RID: 3262
	[InfoBox("The distance threshold at which the ThunderDistance param passes 1 instead of 0 (1 = 1m) 2D distance with world height not taken into account", 0)]
	public float thunderDistanceThreshold = 40f;

	// Token: 0x04000CBF RID: 3263
	[InfoBox("At what point in the closing door animation does the closeDoor event trigger? 0 = completely closed, 1 = completely open", 0)]
	[Space(7f)]
	public float doorCloseTriggerPoint = 0.1f;

	// Token: 0x04000CC0 RID: 3264
	[Space(7f)]
	[InfoBox("A multiplier that controls how far a citizen moves before creating a footstep sound & footprint", 0)]
	public float citizenFootstepDistanceMultiplier = 1f;

	// Token: 0x04000CC1 RID: 3265
	[Header("Object Spawn")]
	[InfoBox("Spawn an object infront of the player by choosing a config using the below, then use the spawn object button.", 0)]
	public InteractablePreset spawnObject;

	// Token: 0x04000CC2 RID: 3266
	private static AudioDebugging _instance;
}
