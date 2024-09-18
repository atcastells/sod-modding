using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200081A RID: 2074
public class WindowMapper : MonoBehaviour
{
	// Token: 0x0600269E RID: 9886 RVA: 0x001F7C24 File Offset: 0x001F5E24
	[Button(null, 0)]
	public void SpawnObjectsOnWindows()
	{
		for (int i = 0; i < this.preset.sortedWindows.Count; i++)
		{
			BuildingPreset.WindowUVFloor windowUVFloor = this.preset.sortedWindows[i];
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(this.buildingObject.transform, false);
			gameObject.name = "Floor " + (i + 1).ToString();
			foreach (BuildingPreset.WindowUVBlock windowUVBlock in windowUVFloor.front)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(this.debugWindow, gameObject.transform);
				gameObject2.transform.localPosition = windowUVBlock.localMeshPositionLeft;
				Object transform = gameObject2.transform;
				string[] array = new string[10];
				array[0] = windowUVBlock.floor.ToString();
				array[1] = " | ";
				int num = 2;
				Vector2 side = windowUVBlock.side;
				array[num] = side.ToString();
				array[3] = " | ";
				array[4] = windowUVBlock.horizonal.ToString();
				array[5] = " (";
				int num2 = 6;
				Vector3 vector = windowUVBlock.localMeshPositionLeft;
				array[num2] = vector.ToString();
				array[7] = " - ";
				int num3 = 8;
				vector = windowUVBlock.localMeshPositionRight;
				array[num3] = vector.ToString();
				array[9] = ")";
				transform.name = string.Concat(array);
			}
			foreach (BuildingPreset.WindowUVBlock windowUVBlock2 in windowUVFloor.back)
			{
				GameObject gameObject3 = Object.Instantiate<GameObject>(this.debugWindow, gameObject.transform);
				gameObject3.transform.localPosition = windowUVBlock2.localMeshPositionLeft;
				Object transform2 = gameObject3.transform;
				string[] array2 = new string[10];
				array2[0] = windowUVBlock2.floor.ToString();
				array2[1] = " | ";
				int num4 = 2;
				Vector2 side = windowUVBlock2.side;
				array2[num4] = side.ToString();
				array2[3] = " | ";
				array2[4] = windowUVBlock2.horizonal.ToString();
				array2[5] = " (";
				int num5 = 6;
				Vector3 vector = windowUVBlock2.localMeshPositionLeft;
				array2[num5] = vector.ToString();
				array2[7] = " - ";
				int num6 = 8;
				vector = windowUVBlock2.localMeshPositionRight;
				array2[num6] = vector.ToString();
				array2[9] = ")";
				transform2.name = string.Concat(array2);
			}
			foreach (BuildingPreset.WindowUVBlock windowUVBlock3 in windowUVFloor.left)
			{
				GameObject gameObject4 = Object.Instantiate<GameObject>(this.debugWindow, gameObject.transform);
				gameObject4.transform.localPosition = windowUVBlock3.localMeshPositionLeft;
				Object transform3 = gameObject4.transform;
				string[] array3 = new string[10];
				array3[0] = windowUVBlock3.floor.ToString();
				array3[1] = " | ";
				int num7 = 2;
				Vector2 side = windowUVBlock3.side;
				array3[num7] = side.ToString();
				array3[3] = " | ";
				array3[4] = windowUVBlock3.horizonal.ToString();
				array3[5] = " (";
				int num8 = 6;
				Vector3 vector = windowUVBlock3.localMeshPositionLeft;
				array3[num8] = vector.ToString();
				array3[7] = " - ";
				int num9 = 8;
				vector = windowUVBlock3.localMeshPositionRight;
				array3[num9] = vector.ToString();
				array3[9] = ")";
				transform3.name = string.Concat(array3);
			}
			foreach (BuildingPreset.WindowUVBlock windowUVBlock4 in windowUVFloor.right)
			{
				GameObject gameObject5 = Object.Instantiate<GameObject>(this.debugWindow, gameObject.transform);
				gameObject5.transform.localPosition = windowUVBlock4.localMeshPositionLeft;
				Object transform4 = gameObject5.transform;
				string[] array4 = new string[10];
				array4[0] = windowUVBlock4.floor.ToString();
				array4[1] = " | ";
				int num10 = 2;
				Vector2 side = windowUVBlock4.side;
				array4[num10] = side.ToString();
				array4[3] = " | ";
				array4[4] = windowUVBlock4.horizonal.ToString();
				array4[5] = " (";
				int num11 = 6;
				Vector3 vector = windowUVBlock4.localMeshPositionLeft;
				array4[num11] = vector.ToString();
				array4[7] = " - ";
				int num12 = 8;
				vector = windowUVBlock4.localMeshPositionRight;
				array4[num12] = vector.ToString();
				array4[9] = ")";
				transform4.name = string.Concat(array4);
			}
		}
	}

	// Token: 0x0600269F RID: 9887 RVA: 0x001F80D8 File Offset: 0x001F62D8
	[Button(null, 0)]
	public void GenerateCableLinkingPoints()
	{
		this.preset.cableLinkPoints.Clear();
		foreach (Transform transform in this.cableLinkingContainer.GetComponentsInChildren<Transform>())
		{
			Vector3 localPos = this.buildingModel.InverseTransformPoint(transform.position);
			Quaternion quaternion = Quaternion.Inverse(this.buildingModel.rotation) * transform.rotation;
			if (transform.gameObject.GetComponent<Collider>() != null)
			{
				this.preset.cableLinkPoints.Add(new BuildingPreset.CableLinkPoint
				{
					localPos = localPos,
					localRot = quaternion.eulerAngles
				});
			}
		}
	}

	// Token: 0x060026A0 RID: 9888 RVA: 0x001F8188 File Offset: 0x001F6388
	[Button(null, 0)]
	public void GenerateNeonSignSidePoints()
	{
		this.preset.sideSignPoints.Clear();
		foreach (Transform transform in this.neonSideSignContainer.GetComponentsInChildren<Transform>())
		{
			Vector3 localPos = this.buildingModel.InverseTransformPoint(transform.position);
			Quaternion quaternion = Quaternion.Inverse(this.buildingModel.rotation) * transform.rotation;
			if (transform.gameObject.GetComponent<Collider>() != null)
			{
				this.preset.sideSignPoints.Add(new BuildingPreset.CableLinkPoint
				{
					localPos = localPos,
					localRot = quaternion.eulerAngles
				});
			}
		}
	}

	// Token: 0x0400452C RID: 17708
	public GameObject buildingObject;

	// Token: 0x0400452D RID: 17709
	public GameObject debugWindow;

	// Token: 0x0400452E RID: 17710
	public BuildingPreset preset;

	// Token: 0x0400452F RID: 17711
	public Transform buildingModel;

	// Token: 0x04004530 RID: 17712
	public Transform cableLinkingContainer;

	// Token: 0x04004531 RID: 17713
	public Transform neonSideSignContainer;
}
