using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000212 RID: 530
public class WindowDebugger : MonoBehaviour
{
	// Token: 0x06000C05 RID: 3077 RVA: 0x000AC6B4 File Offset: 0x000AA8B4
	[Button(null, 0)]
	public void SpawnObjectsOnWindows()
	{
		foreach (Transform transform in base.transform.GetComponentsInChildren<Transform>(true))
		{
			MeshFilter component = transform.gameObject.GetComponent<MeshFilter>();
			if (component != null && component.sharedMesh == this.preset.captureMesh)
			{
				string[] array = new string[10];
				array[0] = "Capture mesh found in prefab! Local scale is ";
				int num = 1;
				Vector3 vector = transform.localScale;
				array[num] = vector.x.ToString();
				array[2] = ", ";
				int num2 = 3;
				vector = transform.localScale;
				array[num2] = vector.y.ToString();
				array[4] = ", ";
				int num3 = 5;
				vector = transform.localScale;
				array[num3] = vector.z.ToString();
				array[6] = ", rotation: ";
				int num4 = 7;
				vector = transform.localEulerAngles;
				array[num4] = vector.ToString();
				array[8] = ",  pos: ";
				int num5 = 9;
				vector = transform.localPosition;
				array[num5] = vector.ToString();
				Debug.Log(string.Concat(array));
				break;
			}
		}
		for (int j = 0; j < this.preset.sortedWindows.Count; j++)
		{
			BuildingPreset.WindowUVFloor windowUVFloor = this.preset.sortedWindows[j];
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(base.transform, false);
			gameObject.name = "Floor " + (j + 1).ToString();
			foreach (BuildingPreset.WindowUVBlock windowUVBlock in windowUVFloor.front)
			{
				GameObject gameObject2 = GameObject.CreatePrimitive(3);
				gameObject2.transform.SetParent(base.transform, false);
				gameObject2.transform.localPosition = (windowUVBlock.localMeshPositionLeft + windowUVBlock.localMeshPositionRight) / 2f;
				gameObject2.transform.SetParent(gameObject.transform, true);
				Object transform2 = gameObject2.transform;
				string[] array2 = new string[10];
				array2[0] = windowUVBlock.floor.ToString();
				array2[1] = " | ";
				int num6 = 2;
				Vector2 side = windowUVBlock.side;
				array2[num6] = side.ToString();
				array2[3] = " | ";
				array2[4] = windowUVBlock.horizonal.ToString();
				array2[5] = " (";
				int num7 = 6;
				Vector3 vector = windowUVBlock.localMeshPositionLeft;
				array2[num7] = vector.ToString();
				array2[7] = " - ";
				int num8 = 8;
				vector = windowUVBlock.localMeshPositionRight;
				array2[num8] = vector.ToString();
				array2[9] = ")";
				transform2.name = string.Concat(array2);
				MeshRenderer component2 = gameObject2.GetComponent<MeshRenderer>();
				Material material = Object.Instantiate<Material>(component2.sharedMaterial);
				float num9 = (float)windowUVBlock.horizonal / 10f;
				Color color = Color.Lerp(new Color(0f, 0f, 0.25f), new Color(0f, 0f, 1f), num9);
				material.SetColor("_BaseColor", color);
				component2.sharedMaterial = material;
			}
			foreach (BuildingPreset.WindowUVBlock windowUVBlock2 in windowUVFloor.back)
			{
				GameObject gameObject3 = GameObject.CreatePrimitive(3);
				gameObject3.transform.SetParent(base.transform, false);
				gameObject3.transform.localPosition = (windowUVBlock2.localMeshPositionLeft + windowUVBlock2.localMeshPositionRight) / 2f;
				gameObject3.transform.SetParent(gameObject.transform, true);
				Object transform3 = gameObject3.transform;
				string[] array3 = new string[10];
				array3[0] = windowUVBlock2.floor.ToString();
				array3[1] = " | ";
				int num10 = 2;
				Vector2 side = windowUVBlock2.side;
				array3[num10] = side.ToString();
				array3[3] = " | ";
				array3[4] = windowUVBlock2.horizonal.ToString();
				array3[5] = " (";
				int num11 = 6;
				Vector3 vector = windowUVBlock2.localMeshPositionLeft;
				array3[num11] = vector.ToString();
				array3[7] = " - ";
				int num12 = 8;
				vector = windowUVBlock2.localMeshPositionRight;
				array3[num12] = vector.ToString();
				array3[9] = ")";
				transform3.name = string.Concat(array3);
				MeshRenderer component3 = gameObject3.GetComponent<MeshRenderer>();
				Material material2 = Object.Instantiate<Material>(component3.sharedMaterial);
				float num13 = (float)windowUVBlock2.horizonal / 10f;
				Color color2 = Color.Lerp(new Color(0.25f, 0.25f, 0.25f), new Color(1f, 1f, 1f), num13);
				material2.SetColor("_BaseColor", color2);
				component3.sharedMaterial = material2;
			}
			foreach (BuildingPreset.WindowUVBlock windowUVBlock3 in windowUVFloor.left)
			{
				GameObject gameObject4 = GameObject.CreatePrimitive(3);
				gameObject4.transform.SetParent(base.transform, false);
				gameObject4.transform.localPosition = (windowUVBlock3.localMeshPositionLeft + windowUVBlock3.localMeshPositionRight) / 2f;
				gameObject4.transform.SetParent(gameObject.transform, true);
				Object transform4 = gameObject4.transform;
				string[] array4 = new string[10];
				array4[0] = windowUVBlock3.floor.ToString();
				array4[1] = " | ";
				int num14 = 2;
				Vector2 side = windowUVBlock3.side;
				array4[num14] = side.ToString();
				array4[3] = " | ";
				array4[4] = windowUVBlock3.horizonal.ToString();
				array4[5] = " (";
				int num15 = 6;
				Vector3 vector = windowUVBlock3.localMeshPositionLeft;
				array4[num15] = vector.ToString();
				array4[7] = " - ";
				int num16 = 8;
				vector = windowUVBlock3.localMeshPositionRight;
				array4[num16] = vector.ToString();
				array4[9] = ")";
				transform4.name = string.Concat(array4);
				MeshRenderer component4 = gameObject4.GetComponent<MeshRenderer>();
				Material material3 = Object.Instantiate<Material>(component4.sharedMaterial);
				float num17 = (float)windowUVBlock3.horizonal / 10f;
				Color color3 = Color.Lerp(new Color(0.25f, 0f, 0f), new Color(1f, 0f, 0f), num17);
				material3.SetColor("_BaseColor", color3);
				component4.sharedMaterial = material3;
			}
			foreach (BuildingPreset.WindowUVBlock windowUVBlock4 in windowUVFloor.right)
			{
				GameObject gameObject5 = GameObject.CreatePrimitive(3);
				gameObject5.transform.SetParent(base.transform, false);
				gameObject5.transform.localPosition = (windowUVBlock4.localMeshPositionLeft + windowUVBlock4.localMeshPositionRight) / 2f;
				gameObject5.transform.SetParent(gameObject.transform, true);
				Object transform5 = gameObject5.transform;
				string[] array5 = new string[10];
				array5[0] = windowUVBlock4.floor.ToString();
				array5[1] = " | ";
				int num18 = 2;
				Vector2 side = windowUVBlock4.side;
				array5[num18] = side.ToString();
				array5[3] = " | ";
				array5[4] = windowUVBlock4.horizonal.ToString();
				array5[5] = " (";
				int num19 = 6;
				Vector3 vector = windowUVBlock4.localMeshPositionLeft;
				array5[num19] = vector.ToString();
				array5[7] = " - ";
				int num20 = 8;
				vector = windowUVBlock4.localMeshPositionRight;
				array5[num20] = vector.ToString();
				array5[9] = ")";
				transform5.name = string.Concat(array5);
				MeshRenderer component5 = gameObject5.GetComponent<MeshRenderer>();
				Material material4 = Object.Instantiate<Material>(component5.sharedMaterial);
				float num21 = (float)windowUVBlock4.horizonal / 10f;
				Color color4 = Color.Lerp(new Color(0f, 0.25f, 0f), new Color(0f, 1f, 0f), num21);
				material4.SetColor("_BaseColor", color4);
				component5.sharedMaterial = material4;
			}
		}
	}

	// Token: 0x04000D5C RID: 3420
	public BuildingPreset preset;
}
