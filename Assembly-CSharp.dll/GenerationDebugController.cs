using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000222 RID: 546
public class GenerationDebugController : MonoBehaviour
{
	// Token: 0x06000C5C RID: 3164 RVA: 0x000B10C4 File Offset: 0x000AF2C4
	public void Setup(string newName, RoomTypePreset newPreset)
	{
		base.name = newName;
		this.preset = newPreset;
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x000B10D4 File Offset: 0x000AF2D4
	public void Log(string newLog)
	{
		this.log.Add(newLog);
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x000B10E4 File Offset: 0x000AF2E4
	[Button(null, 0)]
	public void DisplayAttempedArea()
	{
		if (FloorEditController.Instance.currentlyDisplayingArea != null)
		{
			FloorEditController.Instance.currentlyDisplayingArea.RemoveAttempedArea();
		}
		this.RemoveAttempedArea();
		foreach (NewNode newNode in this.attemptedValidNodes)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.debugNodeDisplay, base.transform);
			gameObject.transform.position = newNode.position;
			this.spawnedObjects.Add(gameObject);
			if (this.overridenNodes.ContainsKey(newNode))
			{
				gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.blue);
				gameObject.name = "OVR " + this.overridenNodes[newNode] + " Valid";
			}
			else
			{
				gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
				gameObject.name = "Valid";
			}
		}
		foreach (KeyValuePair<NewNode, string> keyValuePair in this.attemptedInvalidNodes)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(PrefabControls.Instance.debugNodeDisplay, base.transform);
			gameObject2.transform.position = keyValuePair.Key.position;
			gameObject2.name = keyValuePair.Value;
			this.spawnedObjects.Add(gameObject2);
			gameObject2.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
		}
		FloorEditController.Instance.currentlyDisplayingArea = this;
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x000B12AC File Offset: 0x000AF4AC
	[Button(null, 0)]
	public void RemoveAttempedArea()
	{
		while (this.spawnedObjects.Count > 0)
		{
			Object.Destroy(this.spawnedObjects[0]);
			this.spawnedObjects.RemoveAt(0);
		}
		if (FloorEditController.Instance.currentlyDisplayingArea == this)
		{
			FloorEditController.Instance.currentlyDisplayingArea = null;
		}
	}

	// Token: 0x04000DE5 RID: 3557
	[Tooltip("This room attempt was valid")]
	public bool valid;

	// Token: 0x04000DE6 RID: 3558
	[Tooltip("This room attempt was successful")]
	public bool executed;

	// Token: 0x04000DE7 RID: 3559
	[Tooltip("The configuration of the attempted room")]
	public RoomTypePreset preset;

	// Token: 0x04000DE8 RID: 3560
	[Tooltip("The generated room location")]
	public GenerationController.PossibleRoomLocation location;

	// Token: 0x04000DE9 RID: 3561
	public List<string> log = new List<string>();

	// Token: 0x04000DEA RID: 3562
	[NonSerialized]
	public List<NewNode> attemptedValidNodes;

	// Token: 0x04000DEB RID: 3563
	public Dictionary<NewNode, string> overridenNodes = new Dictionary<NewNode, string>();

	// Token: 0x04000DEC RID: 3564
	public Dictionary<NewNode, string> attemptedInvalidNodes = new Dictionary<NewNode, string>();

	// Token: 0x04000DED RID: 3565
	private List<GameObject> spawnedObjects = new List<GameObject>();
}
