using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class SubObjectPlacement : MonoBehaviour
{
	// Token: 0x06000BDC RID: 3036 RVA: 0x000AB0C0 File Offset: 0x000A92C0
	public void OnClassChanged()
	{
		if (this.preset != null)
		{
			base.name = "Spawn: " + this.preset.name;
		}
		else
		{
			base.name = "Null";
		}
		this.text.text = this.preset.name;
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x000AB119 File Offset: 0x000A9319
	[Button("Random Direction", 0)]
	public void RandomDir()
	{
		base.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x000AB144 File Offset: 0x000A9344
	[Button("Random Object", 0)]
	public void SpawnRandomObject()
	{
		this.RemoveRandomObject();
		object[] array = AssetLoader.Instance.GetAllInteractables().ToArray();
		object[] array2 = array;
		List<InteractablePreset> list = new List<InteractablePreset>();
		array = array2;
		for (int i = 0; i < array.Length; i++)
		{
			InteractablePreset interactablePreset = array[i] as InteractablePreset;
			if (interactablePreset.spawnable && !(interactablePreset.prefab == null) && interactablePreset.subObjectClasses.Contains(this.preset))
			{
				list.Add(interactablePreset);
			}
		}
		if (list.Count > 0)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(list[Random.Range(0, list.Count)].prefab, base.transform);
			gameObject.transform.localPosition = Vector3.zero;
			this.spawnedObject = gameObject.transform;
			if (this.mainObject == null)
			{
				this.mainObject = base.gameObject.GetComponent<MeshRenderer>();
			}
			this.mainObject.enabled = false;
			this.text.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x000AB240 File Offset: 0x000A9440
	[Button("Remove Random Object", 0)]
	public void RemoveRandomObject()
	{
		if (this.spawnedObject != null)
		{
			Object.DestroyImmediate(this.spawnedObject.gameObject);
		}
		if (this.mainObject == null)
		{
			this.mainObject = base.gameObject.GetComponent<MeshRenderer>();
		}
		this.mainObject.enabled = true;
		this.text.gameObject.SetActive(true);
	}

	// Token: 0x04000D3C RID: 3388
	[OnValueChanged("OnClassChanged")]
	[Header("Setup")]
	public SubObjectClassPreset preset;

	// Token: 0x04000D3D RID: 3389
	public FurniturePreset.SubObjectOwnership belongsTo;

	// Token: 0x04000D3E RID: 3390
	public int security;

	// Token: 0x04000D3F RID: 3391
	[Header("Components")]
	public TextMesh text;

	// Token: 0x04000D40 RID: 3392
	public Transform spawnedObject;

	// Token: 0x04000D41 RID: 3393
	public MeshRenderer mainObject;
}
