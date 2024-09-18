using System;
using System.Collections.Generic;
using BrainFailProductions.PolyFewRuntime;
using UnityEngine;

namespace BrainFailProductions.PolyFew
{
	// Token: 0x020008E2 RID: 2274
	[ExecuteInEditMode]
	public class ObjectMaterialLinks : MonoBehaviour
	{
		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06003092 RID: 12434 RVA: 0x00216E94 File Offset: 0x00215094
		// (set) Token: 0x06003093 RID: 12435 RVA: 0x00216E9C File Offset: 0x0021509C
		public List<CombiningInformation.MaterialEntity> linkedMaterialEntities
		{
			get
			{
				return this.linkedEntities;
			}
			set
			{
				this.linkedEntities = value;
				if (value == null)
				{
					return;
				}
				this.materialsProperties = new List<PolyfewRuntime.MaterialProperties>();
				for (int i = 0; i < value.Count; i++)
				{
					CombiningInformation.MaterialEntity materialEntity = value[i];
					if (materialEntity != null)
					{
						for (int j = 0; j < materialEntity.combinedMats.Count; j++)
						{
							CombiningInformation.MaterialProperties materialProperties = materialEntity.combinedMats[j].materialProperties;
							PolyfewRuntime.MaterialProperties materialProperties2 = new PolyfewRuntime.MaterialProperties(materialProperties.texArrIndex, materialProperties.matIndex, materialProperties.materialName, materialProperties.originalMaterial, materialProperties.albedoTint, materialProperties.uvTileOffset, materialProperties.normalIntensity, materialProperties.occlusionIntensity, materialProperties.smoothnessIntensity, materialProperties.glossMapScale, materialProperties.metalIntensity, materialProperties.emissionColor, materialProperties.detailUVTileOffset, materialProperties.alphaCutoff, materialProperties.specularColor, materialProperties.detailNormalScale, materialProperties.heightIntensity, materialProperties.uvSec);
							this.materialsProperties.Add(materialProperties2);
						}
					}
				}
			}
		}

		// Token: 0x06003094 RID: 12436 RVA: 0x00216F94 File Offset: 0x00215194
		private void Start()
		{
			MeshRenderer component = base.GetComponent<MeshRenderer>();
			SkinnedMeshRenderer component2 = base.GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				Material[] sharedMaterials = component.sharedMaterials;
				if (sharedMaterials == null || sharedMaterials.Length == 0)
				{
					Object.DestroyImmediate(this);
					return;
				}
				bool flag = false;
				foreach (Material material in sharedMaterials)
				{
					if (!(material == null))
					{
						string text = material.shader.name.ToLower();
						if (text == "batchfewstandard" || text == "batchfewstandardspecular")
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					Object.DestroyImmediate(this);
					return;
				}
			}
			else if (component2 != null)
			{
				Material[] sharedMaterials = component2.sharedMaterials;
				if (sharedMaterials == null || sharedMaterials.Length == 0)
				{
					Object.DestroyImmediate(this);
					return;
				}
				bool flag2 = false;
				foreach (Material material2 in sharedMaterials)
				{
					if (!(material2 == null))
					{
						string text2 = material2.shader.name.ToLower();
						if (text2 == "batchfewstandard" || text2 == "batchfewstandardspecular")
						{
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					Object.DestroyImmediate(this);
					return;
				}
			}
			else
			{
				Object.DestroyImmediate(this);
			}
		}

		// Token: 0x04004B9D RID: 19357
		[SerializeField]
		private List<CombiningInformation.MaterialEntity> linkedEntities;

		// Token: 0x04004B9E RID: 19358
		public List<PolyfewRuntime.MaterialProperties> materialsProperties;

		// Token: 0x04004B9F RID: 19359
		public Texture2D linkedAttrImg;
	}
}
