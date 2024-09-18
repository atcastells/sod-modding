using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020004C2 RID: 1218
public class RainSheetController : MonoBehaviour
{
	// Token: 0x06001A5C RID: 6748 RVA: 0x00183988 File Offset: 0x00181B88
	private void Start()
	{
		this.material = Object.Instantiate<Material>(this.material);
		this.snowMaterial = Object.Instantiate<Material>(this.snowMaterial);
		this.rainBlockOnlyMask = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			27,
			28
		});
		this.rainBlockAndRoomMeshMask = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			27,
			28,
			29
		});
		this.SetSnowMode(false, true);
	}

	// Token: 0x06001A5D RID: 6749 RVA: 0x00183A04 File Offset: 0x00181C04
	public void SetSnowMode(bool val, bool forceUpdate = false)
	{
		if (val != this.snowMode || forceUpdate)
		{
			this.snowMode = val;
			foreach (RainSheetController.RainSheet rainSheet in this.sheets)
			{
				if (this.snowMode)
				{
					rainSheet.renderer.sharedMaterial = this.snowMaterial;
				}
				else
				{
					rainSheet.renderer.sharedMaterial = this.material;
				}
			}
		}
	}

	// Token: 0x06001A5E RID: 6750 RVA: 0x00183A94 File Offset: 0x00181C94
	public void SetEnabled(bool val)
	{
		base.gameObject.SetActive(val);
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x00183AA4 File Offset: 0x00181CA4
	private void Update()
	{
		if (SessionData.Instance.currentRain > 0f || SessionData.Instance.currentSnow > 0f)
		{
			base.transform.position = new Vector3(Player.Instance.transform.position.x, this.rainSheetHeight + 0.01f, Player.Instance.transform.position.z);
			if (this.indoorRaycast)
			{
				for (int i = 0; i < this.raycastsPerFrame; i++)
				{
					RainSheetController.RainSheet rainSheet = this.sheets[this.sheetCursor];
					rainSheet.rainSheetTransform.localScale = Vector3.one;
					float num = 1f;
					for (int j = 0; j < 3; j++)
					{
						Vector3 vector = rainSheet.rainSheetTransform.position;
						if (j == 1)
						{
							vector = rainSheet.rainSheetTransform.TransformPoint(new Vector3(-0.5f, 0f, 0f));
						}
						else if (j == 2)
						{
							vector = rainSheet.rainSheetTransform.TransformPoint(new Vector3(-1f, 0f, 0f));
						}
						Ray ray;
						ray..ctor(vector, Vector3.down);
						RaycastHit raycastHit;
						if (Physics.Raycast(ray, ref raycastHit, this.rainSheetHeight, this.rainBlockAndRoomMeshMask))
						{
							float num2 = ray.origin.y - raycastHit.point.y;
							float num3 = num2 / this.rainSheetHeight;
							num = Mathf.Min(num, Mathf.Clamp01(num3));
							if (Game.Instance.devMode && Game.Instance.collectDebugData)
							{
								Debug.DrawRay(ray.origin, Vector3.down * num2, Color.red, 0.1f);
							}
							Ray ray2;
							ray2..ctor(raycastHit.point, Vector3.up);
							RaycastHit raycastHit2;
							if (Physics.Raycast(ray2, ref raycastHit2, num2, this.rainBlockAndRoomMeshMask))
							{
								float num4 = raycastHit2.point.y - raycastHit.point.y;
								num3 = (num2 - num4) / this.rainSheetHeight;
								num = Mathf.Min(num, Mathf.Clamp01(num3));
								if (Game.Instance.devMode && Game.Instance.collectDebugData)
								{
									Debug.DrawRay(ray2.origin + new Vector3(0.02f, 0f, 0f), Vector3.up * (num2 - num4), Color.cyan, 0.1f);
								}
							}
						}
						else
						{
							vector += new Vector3(0f, -this.rainSheetHeight, 0f);
							Ray ray3;
							ray3..ctor(vector, Vector3.up);
							if (Physics.Raycast(ray3, ref raycastHit, this.rainSheetHeight, this.rainBlockAndRoomMeshMask))
							{
								float num5 = raycastHit.point.y - ray3.origin.y;
								float num6 = 1f - num5 / this.rainSheetHeight;
								num = Mathf.Min(num, Mathf.Clamp01(num6));
								if (Game.Instance.devMode && Game.Instance.collectDebugData)
								{
									Debug.DrawRay(ray3.origin + new Vector3(0.02f, 0f, 0f), Vector3.up * num5, Color.yellow, 0.1f);
								}
							}
						}
					}
					rainSheet.rainSheetTransform.localScale = new Vector3(1f, num, 1f);
					this.sheetCursor++;
					if (this.sheetCursor >= this.sheets.Count)
					{
						this.sheetCursor = 0;
						return;
					}
				}
			}
		}
	}

	// Token: 0x040022F8 RID: 8952
	[Header("Components")]
	[ReorderableList]
	public List<RainSheetController.RainSheet> sheets = new List<RainSheetController.RainSheet>();

	// Token: 0x040022F9 RID: 8953
	[Header("Settings")]
	public bool indoorRaycast = true;

	// Token: 0x040022FA RID: 8954
	public int raycastsPerFrame = 3;

	// Token: 0x040022FB RID: 8955
	private int rainBlockOnlyMask;

	// Token: 0x040022FC RID: 8956
	private int rainBlockAndRoomMeshMask;

	// Token: 0x040022FD RID: 8957
	private int sheetCursor;

	// Token: 0x040022FE RID: 8958
	public float rainSheetHeight = 10f;

	// Token: 0x040022FF RID: 8959
	public bool snowMode;

	// Token: 0x04002300 RID: 8960
	public Material material;

	// Token: 0x04002301 RID: 8961
	public Material snowMaterial;

	// Token: 0x020004C3 RID: 1219
	[Serializable]
	public class RainSheet
	{
		// Token: 0x04002302 RID: 8962
		public Transform rainSheetTransform;

		// Token: 0x04002303 RID: 8963
		public MeshRenderer renderer;
	}
}
