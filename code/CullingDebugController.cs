using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class CullingDebugController : MonoBehaviour
{
	// Token: 0x06000B95 RID: 2965 RVA: 0x000A9CAC File Offset: 0x000A7EAC
	public void Setup(NewRoom newRoom, NewNode.NodeAccess newPEntrance, List<NewDoor> newDoors, CullingDebugController.CullDebugType newCullType, NewRoom newAtriumTopOf = null, NewNode.NodeAccess newOEntrance = null)
	{
		this.room = newRoom;
		this.parentEntrance = newPEntrance;
		this.otherEntrance = newOEntrance;
		this.dependentDoors = newDoors;
		this.cullType = newCullType;
		this.atriumTopOf = newAtriumTopOf;
		if (this.cullType == CullingDebugController.CullDebugType.none)
		{
			this.rend.sharedMaterial = this.white;
		}
		else if (this.cullType == CullingDebugController.CullDebugType.succeededNew)
		{
			this.rend.sharedMaterial = this.green;
		}
		else if (this.cullType == CullingDebugController.CullDebugType.succeededOvr)
		{
			this.rend.sharedMaterial = this.yellow;
		}
		else if (this.cullType == CullingDebugController.CullDebugType.adjacent)
		{
			this.rend.sharedMaterial = this.blue;
		}
		if (this.dependentDoors != null && this.dependentDoors.Count > 0)
		{
			GameObject gameObject = Toolbox.Instance.SpawnObject(PrefabControls.Instance.debugNodeDisplay, base.transform);
			gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			gameObject.transform.localPosition = new Vector3(0f, 1f, 0f);
			gameObject.GetComponent<MeshRenderer>().sharedMaterial = this.red;
		}
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x000A9DD4 File Offset: 0x000A7FD4
	[Button(null, 0)]
	public void ToggleParentsEntrance()
	{
		if (this.parentObjectMarker != null)
		{
			Object.Destroy(this.parentObjectMarker);
			return;
		}
		this.parentObjectMarker = Toolbox.Instance.SpawnObject(PrefabControls.Instance.debugNodeDisplay, base.transform);
		this.parentObjectMarker.transform.position = this.parentEntrance.worldAccessPoint;
		this.parentObjectMarker.GetComponent<MeshRenderer>().sharedMaterial = this.white;
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x000A9E4C File Offset: 0x000A804C
	[Button(null, 0)]
	public void RunDataRaycast()
	{
		List<DataRaycastController.NodeRaycastHit> list = new List<DataRaycastController.NodeRaycastHit>();
		Game.Log(DataRaycastController.Instance.EntranceRaycast(this.parentEntrance, this.otherEntrance, out list, true).ToString() + "...", 2);
		for (int i = 0; i < list.Count; i++)
		{
			DataRaycastController.NodeRaycastHit nodeRaycastHit = list[i];
			Game.Log(CityData.Instance.NodeToRealpos(nodeRaycastHit.coord), 2);
			GameObject gameObject = Toolbox.Instance.SpawnObject(PrefabControls.Instance.debugNodeDisplay, base.transform);
			gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			gameObject.transform.position = CityData.Instance.NodeToRealpos(nodeRaycastHit.coord);
			gameObject.GetComponent<MeshRenderer>().sharedMaterial = this.green;
			gameObject.name = CityData.Instance.NodeToRealpos(nodeRaycastHit.coord).ToString();
		}
	}

	// Token: 0x04000CC5 RID: 3269
	public MeshRenderer rend;

	// Token: 0x04000CC6 RID: 3270
	public NewRoom room;

	// Token: 0x04000CC7 RID: 3271
	public NewNode.NodeAccess parentEntrance;

	// Token: 0x04000CC8 RID: 3272
	public NewNode.NodeAccess otherEntrance;

	// Token: 0x04000CC9 RID: 3273
	public List<NewDoor> dependentDoors;

	// Token: 0x04000CCA RID: 3274
	public NewRoom atriumTopOf;

	// Token: 0x04000CCB RID: 3275
	public GameObject parentObjectMarker;

	// Token: 0x04000CCC RID: 3276
	public CullingDebugController.CullDebugType cullType;

	// Token: 0x04000CCD RID: 3277
	[Header("Config")]
	public Material red;

	// Token: 0x04000CCE RID: 3278
	public Material white;

	// Token: 0x04000CCF RID: 3279
	public Material yellow;

	// Token: 0x04000CD0 RID: 3280
	public Material green;

	// Token: 0x04000CD1 RID: 3281
	public Material blue;

	// Token: 0x020001F7 RID: 503
	public enum CullDebugType
	{
		// Token: 0x04000CD3 RID: 3283
		none,
		// Token: 0x04000CD4 RID: 3284
		succeededNew,
		// Token: 0x04000CD5 RID: 3285
		succeededOvr,
		// Token: 0x04000CD6 RID: 3286
		adjacent,
		// Token: 0x04000CD7 RID: 3287
		atriumTop
	}
}
