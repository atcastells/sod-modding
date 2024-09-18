using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000411 RID: 1041
[Serializable]
public class MetaObject
{
	// Token: 0x060017C0 RID: 6080 RVA: 0x0016442C File Offset: 0x0016262C
	public MetaObject(InteractablePreset newPreset, Human newOwner, Human newWriter, Human newReciever, List<Interactable.Passed> newPassed)
	{
		this.id = Interactable.worldAssignID;
		while (CityData.Instance.savableInteractableDictionary.ContainsKey(this.id) || CityData.Instance.metaObjectDictionary.ContainsKey(this.id))
		{
			this.id++;
		}
		Interactable.worldAssignID = this.id + 1;
		this.preset = newPreset.name;
		if (newOwner != null)
		{
			this.owner = newOwner.humanID;
		}
		if (newWriter != null)
		{
			this.writer = newWriter.humanID;
		}
		if (newReciever != null)
		{
			this.reciever = newReciever.humanID;
		}
		this.passed = newPassed;
		if (this.passed == null)
		{
			this.passed = new List<Interactable.Passed>();
		}
		this.passed.Add(new Interactable.Passed(Interactable.PassedVarType.metaObjectID, (float)this.id, null));
		CityData.Instance.metaObjectDictionary.Add(this.id, this);
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x00164549 File Offset: 0x00162749
	public void Remove()
	{
		if (CityData.Instance.metaObjectDictionary.ContainsKey(this.id))
		{
			CityData.Instance.metaObjectDictionary.Remove(this.id);
		}
	}

	// Token: 0x060017C2 RID: 6082 RVA: 0x00164578 File Offset: 0x00162778
	public Evidence GetEvidence(bool setPosition = false, Vector3Int nodeCoord = default(Vector3Int))
	{
		Human belongsTo = null;
		Human human = null;
		Human human2 = null;
		if (this.owner > -1)
		{
			CityData.Instance.GetHuman(this.owner, out belongsTo, true);
		}
		if (this.writer > -1)
		{
			CityData.Instance.GetHuman(this.writer, out human, true);
		}
		if (this.reciever > -1)
		{
			CityData.Instance.GetHuman(this.reciever, out human2, true);
		}
		NewGameLocation gameLocation = null;
		if (setPosition)
		{
			this.n = nodeCoord;
		}
		NewNode newNode = null;
		if (PathFinder.Instance.nodeMap.TryGetValue(this.n, ref newNode))
		{
			gameLocation = newNode.gameLocation;
		}
		InteractablePreset interactablePreset = null;
		if (Toolbox.Instance.objectPresetDictionary.TryGetValue(this.preset, ref interactablePreset))
		{
			return Toolbox.Instance.GetOrCreateEvidenceForInteractable(interactablePreset, "I" + this.id.ToString(), null, belongsTo, human, human2, null, gameLocation, interactablePreset.retailItem, this.passed);
		}
		return null;
	}

	// Token: 0x060017C3 RID: 6083 RVA: 0x0016466C File Offset: 0x0016286C
	public InteractablePreset GetPreset()
	{
		InteractablePreset result = null;
		Toolbox.Instance.objectPresetDictionary.TryGetValue(this.preset, ref result);
		return result;
	}

	// Token: 0x04001D25 RID: 7461
	public int id = -1;

	// Token: 0x04001D26 RID: 7462
	public string preset;

	// Token: 0x04001D27 RID: 7463
	public int owner = -1;

	// Token: 0x04001D28 RID: 7464
	public int writer = -1;

	// Token: 0x04001D29 RID: 7465
	public int reciever = -1;

	// Token: 0x04001D2A RID: 7466
	public string dds;

	// Token: 0x04001D2B RID: 7467
	public List<Interactable.Passed> passed;

	// Token: 0x04001D2C RID: 7468
	public Vector3Int n;

	// Token: 0x04001D2D RID: 7469
	public bool cd;
}
