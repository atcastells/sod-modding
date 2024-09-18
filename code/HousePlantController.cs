using System;
using UnityEngine;

// Token: 0x020003E6 RID: 998
public class HousePlantController : MonoBehaviour
{
	// Token: 0x06001689 RID: 5769 RVA: 0x00150CA4 File Offset: 0x0014EEA4
	private void OnEnable()
	{
		if (!this.isLoaded && this != null && InteriorControls.Instance != null && InteriorControls.Instance.housePlantPool.Count > 0)
		{
			string str = base.transform.position.ToString();
			this.poolIndex = Mathf.Clamp(Toolbox.Instance.GetPsuedoRandomNumber(0, InteriorControls.Instance.housePlantPool.Count, str, false), 0, InteriorControls.Instance.housePlantPool.Count - 1);
			this.scaleIndex = Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, str, false);
			this.rotation = Toolbox.Instance.GetPsuedoRandomNumber(0f, 360f, str, false);
			this.colourLerp = this.scaleIndex;
			this.spawnedPlant = Object.Instantiate<GameObject>(InteriorControls.Instance.housePlantPool[this.poolIndex], base.transform);
			float num = Mathf.Lerp(this.sizeScale.x, this.sizeScale.y, this.scaleIndex);
			this.spawnedPlant.transform.localScale = new Vector3(num, num, num);
			this.spawnedPlant.transform.localPosition = this.spawnLocalPosition;
			this.spawnedPlant.transform.localEulerAngles = new Vector3(0f, this.rotation, 0f);
			this.spawnedPlant.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.Lerp(InteriorControls.Instance.housePlantColour1, InteriorControls.Instance.housePlantColour2, this.colourLerp));
			Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(base.transform.position);
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
			{
				Toolbox.Instance.SetLightLayer(this.spawnedPlant, newNode.building, false);
			}
			this.isLoaded = true;
		}
	}

	// Token: 0x04001B7E RID: 7038
	[Header("Configuration")]
	public Vector3 spawnLocalPosition = Vector3.zero;

	// Token: 0x04001B7F RID: 7039
	public Vector2 sizeScale = new Vector2(0.85f, 1.15f);

	// Token: 0x04001B80 RID: 7040
	[Header("Deterministic Values")]
	public int poolIndex;

	// Token: 0x04001B81 RID: 7041
	public float scaleIndex;

	// Token: 0x04001B82 RID: 7042
	public float rotation;

	// Token: 0x04001B83 RID: 7043
	public float colourLerp;

	// Token: 0x04001B84 RID: 7044
	[Header("State")]
	public GameObject spawnedPlant;

	// Token: 0x04001B85 RID: 7045
	public bool isLoaded;
}
