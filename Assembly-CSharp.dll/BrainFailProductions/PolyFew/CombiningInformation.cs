using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainFailProductions.PolyFew
{
	// Token: 0x020008D7 RID: 2263
	public class CombiningInformation
	{
		// Token: 0x0600307C RID: 12412 RVA: 0x002161FC File Offset: 0x002143FC
		public bool ShouldGenerateMetallicArray()
		{
			using (List<CombiningInformation.MaterialEntity>.Enumerator enumerator = this.materialEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.metallicMap != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600307D RID: 12413 RVA: 0x0021625C File Offset: 0x0021445C
		public bool ShouldGenerateSpecularArray()
		{
			using (List<CombiningInformation.MaterialEntity>.Enumerator enumerator = this.materialEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.specularMap != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600307E RID: 12414 RVA: 0x002162BC File Offset: 0x002144BC
		public bool ShouldGenerateNormalArray()
		{
			using (List<CombiningInformation.MaterialEntity>.Enumerator enumerator = this.materialEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.normalMap != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600307F RID: 12415 RVA: 0x0021631C File Offset: 0x0021451C
		public bool ShouldGenerateHeightArray()
		{
			using (List<CombiningInformation.MaterialEntity>.Enumerator enumerator = this.materialEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.heightMap != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003080 RID: 12416 RVA: 0x0021637C File Offset: 0x0021457C
		public bool ShouldGenerateOcclusionArray()
		{
			using (List<CombiningInformation.MaterialEntity>.Enumerator enumerator = this.materialEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.occlusionMap != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003081 RID: 12417 RVA: 0x002163DC File Offset: 0x002145DC
		public bool ShouldGenerateEmissionArray()
		{
			using (List<CombiningInformation.MaterialEntity>.Enumerator enumerator = this.materialEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.emissionMap != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003082 RID: 12418 RVA: 0x0021643C File Offset: 0x0021463C
		public bool ShouldGenerateDetailMaskArray()
		{
			using (List<CombiningInformation.MaterialEntity>.Enumerator enumerator = this.materialEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.detailMaskMap != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003083 RID: 12419 RVA: 0x0021649C File Offset: 0x0021469C
		public bool ShouldGenerateDetailAlbedoArray()
		{
			using (List<CombiningInformation.MaterialEntity>.Enumerator enumerator = this.materialEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.detailAlbedoMap != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003084 RID: 12420 RVA: 0x002164FC File Offset: 0x002146FC
		public bool ShouldGenerateDetailNormalArray()
		{
			using (List<CombiningInformation.MaterialEntity>.Enumerator enumerator = this.materialEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.detailNormalMap != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04004B4B RID: 19275
		public List<CombiningInformation.MaterialEntity> materialEntities = new List<CombiningInformation.MaterialEntity>();

		// Token: 0x04004B4C RID: 19276
		public CombiningInformation.TextureArrayGroup textureArraysSettings = new CombiningInformation.TextureArrayGroup();

		// Token: 0x04004B4D RID: 19277
		public CombiningInformation.DiffuseColorSpace diffuseColorSpace;

		// Token: 0x04004B4E RID: 19278
		public Material[] combinedMaterials;

		// Token: 0x020008D8 RID: 2264
		public enum DiffuseColorSpace
		{
			// Token: 0x04004B50 RID: 19280
			NON_LINEAR,
			// Token: 0x04004B51 RID: 19281
			LINEAR
		}

		// Token: 0x020008D9 RID: 2265
		public enum CompressionType
		{
			// Token: 0x04004B53 RID: 19283
			UNCOMPRESSED,
			// Token: 0x04004B54 RID: 19284
			DXT1,
			// Token: 0x04004B55 RID: 19285
			ETC2_RGB,
			// Token: 0x04004B56 RID: 19286
			PVRTC_RGB4,
			// Token: 0x04004B57 RID: 19287
			ASTC_RGB
		}

		// Token: 0x020008DA RID: 2266
		public enum CompressionQuality
		{
			// Token: 0x04004B59 RID: 19289
			LOW,
			// Token: 0x04004B5A RID: 19290
			MEDIUM,
			// Token: 0x04004B5B RID: 19291
			HIGH
		}

		// Token: 0x020008DB RID: 2267
		[Serializable]
		public struct Resolution
		{
			// Token: 0x04004B5C RID: 19292
			public int width;

			// Token: 0x04004B5D RID: 19293
			public int height;
		}

		// Token: 0x020008DC RID: 2268
		[Serializable]
		public class TextureArrayUserSettings
		{
			// Token: 0x06003086 RID: 12422 RVA: 0x0021657C File Offset: 0x0021477C
			public TextureArrayUserSettings(CombiningInformation.Resolution resolution, FilterMode filteringMode, CombiningInformation.CompressionType compressionType, CombiningInformation.CompressionQuality compressionQuality = CombiningInformation.CompressionQuality.MEDIUM, int anisotropicFilteringLevel = 1)
			{
				this.resolution = resolution;
				this.filteringMode = filteringMode;
				this.compressionType = compressionType;
				this.compressionQuality = compressionQuality;
				this.anisotropicFilteringLevel = anisotropicFilteringLevel;
			}

			// Token: 0x04004B5E RID: 19294
			public CombiningInformation.Resolution resolution;

			// Token: 0x04004B5F RID: 19295
			public FilterMode filteringMode;

			// Token: 0x04004B60 RID: 19296
			public CombiningInformation.CompressionType compressionType;

			// Token: 0x04004B61 RID: 19297
			public CombiningInformation.CompressionQuality compressionQuality;

			// Token: 0x04004B62 RID: 19298
			public int anisotropicFilteringLevel;

			// Token: 0x04004B63 RID: 19299
			public int choiceResolutionW = 4;

			// Token: 0x04004B64 RID: 19300
			public int choiceResolutionH = 4;

			// Token: 0x04004B65 RID: 19301
			public int choiceFilteringMode;

			// Token: 0x04004B66 RID: 19302
			public int choiceCompressionQuality = 1;

			// Token: 0x04004B67 RID: 19303
			public int choiceCompressionType;
		}

		// Token: 0x020008DD RID: 2269
		[Serializable]
		public class TextureArrayGroup
		{
			// Token: 0x06003087 RID: 12423 RVA: 0x002165CC File Offset: 0x002147CC
			public void InitializeDefaultArraySettings(CombiningInformation.Resolution resolution, FilterMode filteringMode, CombiningInformation.CompressionType compressionType, CombiningInformation.CompressionQuality compressionQuality = CombiningInformation.CompressionQuality.MEDIUM, int anisotropicFilteringLevel = 1)
			{
				this.diffuseArraySettings = new CombiningInformation.TextureArrayUserSettings(resolution, filteringMode, compressionType, compressionQuality, anisotropicFilteringLevel);
				this.metallicArraySettings = new CombiningInformation.TextureArrayUserSettings(resolution, filteringMode, compressionType, compressionQuality, anisotropicFilteringLevel);
				this.specularArraySettings = new CombiningInformation.TextureArrayUserSettings(resolution, filteringMode, compressionType, compressionQuality, anisotropicFilteringLevel);
				this.normalArraySettings = new CombiningInformation.TextureArrayUserSettings(resolution, filteringMode, compressionType, compressionQuality, anisotropicFilteringLevel);
				this.heightArraySettings = new CombiningInformation.TextureArrayUserSettings(resolution, filteringMode, compressionType, compressionQuality, anisotropicFilteringLevel);
				this.occlusionArraySettings = new CombiningInformation.TextureArrayUserSettings(resolution, filteringMode, compressionType, compressionQuality, anisotropicFilteringLevel);
				this.emissiveArraySettings = new CombiningInformation.TextureArrayUserSettings(resolution, filteringMode, compressionType, compressionQuality, anisotropicFilteringLevel);
				this.detailMaskArraySettings = new CombiningInformation.TextureArrayUserSettings(resolution, filteringMode, compressionType, compressionQuality, anisotropicFilteringLevel);
				this.detailAlbedoArraySettings = new CombiningInformation.TextureArrayUserSettings(resolution, filteringMode, compressionType, compressionQuality, anisotropicFilteringLevel);
				this.detailNormalArraySettings = new CombiningInformation.TextureArrayUserSettings(resolution, filteringMode, compressionType, compressionQuality, anisotropicFilteringLevel);
			}

			// Token: 0x04004B68 RID: 19304
			public CombiningInformation.TextureArrayUserSettings diffuseArraySettings;

			// Token: 0x04004B69 RID: 19305
			public CombiningInformation.TextureArrayUserSettings metallicArraySettings;

			// Token: 0x04004B6A RID: 19306
			public CombiningInformation.TextureArrayUserSettings specularArraySettings;

			// Token: 0x04004B6B RID: 19307
			public CombiningInformation.TextureArrayUserSettings normalArraySettings;

			// Token: 0x04004B6C RID: 19308
			public CombiningInformation.TextureArrayUserSettings heightArraySettings;

			// Token: 0x04004B6D RID: 19309
			public CombiningInformation.TextureArrayUserSettings occlusionArraySettings;

			// Token: 0x04004B6E RID: 19310
			public CombiningInformation.TextureArrayUserSettings emissiveArraySettings;

			// Token: 0x04004B6F RID: 19311
			public CombiningInformation.TextureArrayUserSettings detailMaskArraySettings;

			// Token: 0x04004B70 RID: 19312
			public CombiningInformation.TextureArrayUserSettings detailAlbedoArraySettings;

			// Token: 0x04004B71 RID: 19313
			public CombiningInformation.TextureArrayUserSettings detailNormalArraySettings;
		}

		// Token: 0x020008DE RID: 2270
		[Serializable]
		public class MaterialProperties
		{
			// Token: 0x06003089 RID: 12425 RVA: 0x00216690 File Offset: 0x00214890
			public bool IsSameAs(CombiningInformation.MaterialProperties toCompare)
			{
				return this.originalMaterial == toCompare.originalMaterial || (!(toCompare.albedoTint != this.albedoTint) && toCompare.normalIntensity == this.normalIntensity && toCompare.occlusionIntensity == this.occlusionIntensity && toCompare.smoothnessIntensity == this.smoothnessIntensity && toCompare.glossMapScale == this.glossMapScale && !(toCompare.uvTileOffset != this.uvTileOffset) && toCompare.metalIntensity == this.metalIntensity && !(toCompare.emissionColor != this.emissionColor) && !(toCompare.detailUVTileOffset != this.detailUVTileOffset) && toCompare.alphaCutoff == this.alphaCutoff && !(toCompare.specularColor != this.specularColor) && toCompare.detailNormalScale == this.detailNormalScale && toCompare.heightIntensity == this.heightIntensity && toCompare.uvSec == this.uvSec && toCompare.alphaMode == this.alphaMode);
			}

			// Token: 0x0600308A RID: 12426 RVA: 0x002167BC File Offset: 0x002149BC
			public static Texture2D NewTexture()
			{
				Texture2D texture2D = new Texture2D(8, 4, 17, false, true);
				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						texture2D.SetPixel(i, j, Color.black);
					}
				}
				texture2D.Apply();
				return texture2D;
			}

			// Token: 0x0600308B RID: 12427 RVA: 0x00216804 File Offset: 0x00214A04
			public void BurnAttrToImg(ref Texture2D burnOn, int index, int textureArrayIndex)
			{
				if (index >= burnOn.height)
				{
					Texture2D texture2D = new Texture2D(burnOn.width, index + 1, 17, false, true);
					Color[] pixels = burnOn.GetPixels();
					texture2D.SetPixels(0, 0, burnOn.width, burnOn.height, pixels);
					burnOn = texture2D;
				}
				if (burnOn.width < 8)
				{
					Texture2D texture2D2 = new Texture2D(8, burnOn.height, 17, false, true);
					Color[] pixels2 = burnOn.GetPixels();
					texture2D2.SetPixels(0, 0, burnOn.width, burnOn.height, pixels2);
					burnOn = texture2D2;
				}
				burnOn.SetPixel(0, index, new Color(this.uvTileOffset.x - 1f, this.uvTileOffset.y - 1f, this.uvTileOffset.z, this.uvTileOffset.w));
				burnOn.SetPixel(1, index, new Color(this.normalIntensity, this.occlusionIntensity, this.smoothnessIntensity, this.metalIntensity));
				burnOn.SetPixel(2, index, this.albedoTint);
				burnOn.SetPixel(3, index, this.emissionColor);
				burnOn.SetPixel(4, index, new Color(this.specularColor.r, this.specularColor.g, this.specularColor.b, this.glossMapScale));
				burnOn.SetPixel(5, index, new Color(this.detailUVTileOffset.x, this.detailUVTileOffset.y, this.detailUVTileOffset.z, this.detailUVTileOffset.w));
				burnOn.SetPixel(6, index, new Color(this.alphaCutoff, this.detailNormalScale, this.heightIntensity, this.uvSec));
				burnOn.SetPixel(7, index, new Color((float)textureArrayIndex, (float)textureArrayIndex, (float)textureArrayIndex, (float)textureArrayIndex));
				burnOn.Apply();
			}

			// Token: 0x0600308C RID: 12428 RVA: 0x002169CC File Offset: 0x00214BCC
			public void FillPropertiesFromMaterial(Material material, CombiningInformation combineInfo)
			{
				this.materialName = material.name;
				this.originalMaterial = material;
				this.normalIntensity = 1f;
				this.occlusionIntensity = 1f;
				this.smoothnessIntensity = 1f;
				this.albedoTint = Color.white;
				this.metalIntensity = 1f;
				this.uvTileOffset = new Vector4(1f, 1f, 0f, 0f);
				this.detailUVTileOffset = new Vector4(1f, 1f, 0f, 0f);
				this.emissionColor = Color.black;
				this.alphaCutoff = 0.5f;
				this.specularColor = Color.black;
				this.detailNormalScale = 1f;
				this.heightIntensity = 0.05f;
				this.alphaMode = 0;
				this.glossMapScale = 0f;
				if (material.shader.name.ToLower() == "standard (specular setup)")
				{
					this.specularWorkflow = true;
				}
				if (material.HasProperty("_Color"))
				{
					this.albedoTint = material.GetColor("_Color");
				}
				if (material.HasProperty("_MainTex") && material.HasProperty("_MainTex_ST"))
				{
					this.uvTileOffset = material.GetVector("_MainTex_ST");
				}
				if (material.HasProperty("_GlossMapScale"))
				{
					this.glossMapScale = material.GetFloat("_GlossMapScale");
				}
				if (material.HasProperty("_Glossiness"))
				{
					this.smoothnessIntensity = material.GetFloat("_Glossiness");
				}
				if (material.HasProperty("_Smoothness"))
				{
					this.smoothnessIntensity = material.GetFloat("_Smoothness");
				}
				if (material.HasProperty("_MetallicGlossMap") && material.GetTexture("_MetallicGlossMap") != null)
				{
					this.smoothnessIntensity = this.glossMapScale;
				}
				if (material.HasProperty("_SpecColor"))
				{
					this.specularColor = material.GetColor("_SpecColor");
				}
				if (material.HasProperty("_Metallic"))
				{
					this.metalIntensity = material.GetFloat("_Metallic");
				}
				if (material.HasProperty("_OcclusionStrength"))
				{
					this.occlusionIntensity = material.GetFloat("_OcclusionStrength") + 1f;
				}
				if (material.HasProperty("_BumpScale"))
				{
					this.normalIntensity = material.GetFloat("_BumpScale");
				}
				if (material.HasProperty("_DetailNormalMapScale"))
				{
					this.detailNormalScale = material.GetFloat("_DetailNormalMapScale");
				}
				if (material.HasProperty("_EmissionColor") && material.HasProperty("_EmissionMap") && combineInfo.ShouldGenerateEmissionArray())
				{
					this.emissionColor = Color.black;
				}
				else if (material.HasProperty("_EmissionColor"))
				{
					this.emissionColor = material.GetColor("_EmissionColor");
				}
				if (material.HasProperty("_Parallax"))
				{
					this.heightIntensity = material.GetFloat("_Parallax");
				}
				if (material.HasProperty("_UVSec"))
				{
					this.uvSec = material.GetFloat("_UVSec");
				}
				if (material.HasProperty("_DetailAlbedoMap") && material.HasProperty("_DetailAlbedoMap_ST"))
				{
					this.detailUVTileOffset = material.GetVector("_DetailAlbedoMap_ST");
				}
				if (material.HasProperty("_Mode"))
				{
					this.alphaMode = (int)material.GetFloat("_Mode");
				}
			}

			// Token: 0x04004B72 RID: 19314
			public bool foldOut = true;

			// Token: 0x04004B73 RID: 19315
			public int texArrIndex;

			// Token: 0x04004B74 RID: 19316
			public int matIndex;

			// Token: 0x04004B75 RID: 19317
			public string materialName;

			// Token: 0x04004B76 RID: 19318
			public Material originalMaterial;

			// Token: 0x04004B77 RID: 19319
			public Color albedoTint;

			// Token: 0x04004B78 RID: 19320
			public Vector4 uvTileOffset = new Vector4(1f, 1f, 0f, 0f);

			// Token: 0x04004B79 RID: 19321
			public float normalIntensity = 1f;

			// Token: 0x04004B7A RID: 19322
			public float occlusionIntensity = 1f;

			// Token: 0x04004B7B RID: 19323
			public float smoothnessIntensity = 1f;

			// Token: 0x04004B7C RID: 19324
			public float glossMapScale = 1f;

			// Token: 0x04004B7D RID: 19325
			public float metalIntensity = 1f;

			// Token: 0x04004B7E RID: 19326
			public Color emissionColor = Color.black;

			// Token: 0x04004B7F RID: 19327
			public Vector4 detailUVTileOffset = new Vector4(1f, 1f, 0f, 0f);

			// Token: 0x04004B80 RID: 19328
			public float alphaCutoff = 0.5f;

			// Token: 0x04004B81 RID: 19329
			public Color specularColor = Color.black;

			// Token: 0x04004B82 RID: 19330
			public float detailNormalScale = 1f;

			// Token: 0x04004B83 RID: 19331
			public float heightIntensity = 0.05f;

			// Token: 0x04004B84 RID: 19332
			public float uvSec;

			// Token: 0x04004B85 RID: 19333
			public int alphaMode;

			// Token: 0x04004B86 RID: 19334
			public bool specularWorkflow;
		}

		// Token: 0x020008DF RID: 2271
		[Serializable]
		public class MeshData
		{
			// Token: 0x04004B87 RID: 19335
			public List<MeshFilter> meshFilters;

			// Token: 0x04004B88 RID: 19336
			public List<MeshRenderer> meshRenderers;

			// Token: 0x04004B89 RID: 19337
			public List<SkinnedMeshRenderer> skinnedMeshRenderers;

			// Token: 0x04004B8A RID: 19338
			public Material[] originalMaterials;

			// Token: 0x04004B8B RID: 19339
			public Mesh[] outputMeshes;

			// Token: 0x04004B8C RID: 19340
			public Matrix4x4[] outputMatrices;
		}

		// Token: 0x020008E0 RID: 2272
		[Serializable]
		public class CombineMetaData
		{
			// Token: 0x04004B8D RID: 19341
			public Material material;

			// Token: 0x04004B8E RID: 19342
			public CombiningInformation.MaterialProperties materialProperties;

			// Token: 0x04004B8F RID: 19343
			public CombiningInformation.MaterialProperties tempMaterialProperties;

			// Token: 0x04004B90 RID: 19344
			public List<CombiningInformation.MeshData> meshesData = new List<CombiningInformation.MeshData>();
		}

		// Token: 0x020008E1 RID: 2273
		[Serializable]
		public class MaterialEntity
		{
			// Token: 0x06003090 RID: 12432 RVA: 0x00216DE8 File Offset: 0x00214FE8
			public bool HasAnyTextures()
			{
				return this.diffuseMap != null || this.heightMap != null || this.normalMap != null || this.metallicMap != null || this.detailAlbedoMap != null || this.detailNormalMap != null || this.detailMaskMap != null || this.emissionMap != null || this.specularMap != null || this.occlusionMap != null;
			}

			// Token: 0x04004B91 RID: 19345
			public List<CombiningInformation.CombineMetaData> combinedMats = new List<CombiningInformation.CombineMetaData>();

			// Token: 0x04004B92 RID: 19346
			public int textArrIndex;

			// Token: 0x04004B93 RID: 19347
			public Texture2D diffuseMap;

			// Token: 0x04004B94 RID: 19348
			public Texture2D metallicMap;

			// Token: 0x04004B95 RID: 19349
			public Texture2D specularMap;

			// Token: 0x04004B96 RID: 19350
			public Texture2D normalMap;

			// Token: 0x04004B97 RID: 19351
			public Texture2D heightMap;

			// Token: 0x04004B98 RID: 19352
			public Texture2D occlusionMap;

			// Token: 0x04004B99 RID: 19353
			public Texture2D emissionMap;

			// Token: 0x04004B9A RID: 19354
			public Texture2D detailMaskMap;

			// Token: 0x04004B9B RID: 19355
			public Texture2D detailAlbedoMap;

			// Token: 0x04004B9C RID: 19356
			public Texture2D detailNormalMap;
		}
	}
}
