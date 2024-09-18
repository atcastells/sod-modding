using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BrainFailProductions.PolyFewRuntime
{
	// Token: 0x02000939 RID: 2361
	public class PolygonReduction : MonoBehaviour
	{
		// Token: 0x0600321F RID: 12831 RVA: 0x002239D8 File Offset: 0x00221BD8
		private void Start()
		{
			if (Application.platform == 17)
			{
				this.isWebGL = true;
			}
			this.uninteractivePanel.SetActive(false);
			this.exportButton.interactable = false;
			this.barabarianRef = this.targetObject;
			this.objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(this.targetObject, true);
			this.trianglesCount.text = (PolyfewRuntime.CountTriangles(true, this.targetObject).ToString() ?? "");
		}

		// Token: 0x06003220 RID: 12832 RVA: 0x00223A54 File Offset: 0x00221C54
		private void Update()
		{
			if (!this.eventSystem)
			{
				return;
			}
			if (this.eventSystem.currentSelectedGameObject && this.eventSystem.currentSelectedGameObject.GetComponent<RectTransform>())
			{
				FlyCamera.deactivated = true;
			}
			else
			{
				FlyCamera.deactivated = false;
			}
			if (this.isWebGL)
			{
				this.exportButton.gameObject.SetActive(false);
				this.importFromFileSystem.gameObject.SetActive(false);
			}
		}

		// Token: 0x06003221 RID: 12833 RVA: 0x00223AD0 File Offset: 0x00221CD0
		public void OnReductionChange(float value)
		{
			if (this.disableTemporary)
			{
				return;
			}
			this.didApplyLosslessLast = false;
			if (this.targetObject == null)
			{
				return;
			}
			if (Mathf.Approximately(0f, value))
			{
				this.AssignMeshesFromPairs();
				this.trianglesCount.text = (PolyfewRuntime.CountTriangles(true, this.targetObject).ToString() ?? "");
				return;
			}
			PolyfewRuntime.SimplificationOptions simplificationOptions = new PolyfewRuntime.SimplificationOptions();
			simplificationOptions.simplificationStrength = value;
			simplificationOptions.enableSmartlinking = this.enableSmartLinking.isOn;
			simplificationOptions.preserveBorderEdges = this.preserveBorders.isOn;
			simplificationOptions.preserveUVSeamEdges = this.preserveUVSeams.isOn;
			simplificationOptions.preserveUVFoldoverEdges = this.preserveUVFoldover.isOn;
			simplificationOptions.recalculateNormals = this.recalculateNormals.isOn;
			simplificationOptions.regardCurvature = this.regardCurvature.isOn;
			if (this.preserveFace.isOn)
			{
				simplificationOptions.regardPreservationSpheres = true;
				simplificationOptions.preservationSpheres.Add(new PolyfewRuntime.PreservationSphere(this.preservationSphere.position, this.preservationSphere.lossyScale.x, this.preservationStrength.value));
			}
			else
			{
				simplificationOptions.regardPreservationSpheres = false;
			}
			this.trianglesCount.text = (PolyfewRuntime.SimplifyObjectDeep(this.objectMeshPairs, simplificationOptions, delegate(GameObject go, PolyfewRuntime.MeshRendererPair mInfo)
			{
			}).ToString() ?? "");
		}

		// Token: 0x06003222 RID: 12834 RVA: 0x00223C48 File Offset: 0x00221E48
		public void SimplifyLossless()
		{
			this.disableTemporary = true;
			this.reductionStrength.value = 0f;
			this.disableTemporary = false;
			this.didApplyLosslessLast = true;
			PolyfewRuntime.SimplificationOptions simplificationOptions = new PolyfewRuntime.SimplificationOptions
			{
				enableSmartlinking = this.enableSmartLinking.isOn,
				preserveBorderEdges = this.preserveBorders.isOn,
				preserveUVSeamEdges = this.preserveUVSeams.isOn,
				preserveUVFoldoverEdges = this.preserveUVFoldover.isOn,
				recalculateNormals = this.recalculateNormals.isOn,
				regardCurvature = this.regardCurvature.isOn,
				simplifyMeshLossless = true
			};
			if (this.preserveFace.isOn)
			{
				simplificationOptions.regardPreservationSpheres = true;
			}
			else
			{
				simplificationOptions.regardPreservationSpheres = false;
			}
			this.trianglesCount.text = (PolyfewRuntime.SimplifyObjectDeep(this.objectMeshPairs, simplificationOptions, delegate(GameObject go, PolyfewRuntime.MeshRendererPair mInfo)
			{
			}).ToString() ?? "");
		}

		// Token: 0x06003223 RID: 12835 RVA: 0x00223D54 File Offset: 0x00221F54
		public void ImportOBJ()
		{
			PolyfewRuntime.OBJImportOptions objimportOptions = new PolyfewRuntime.OBJImportOptions();
			objimportOptions.zUp = false;
			objimportOptions.localPosition = new Vector3(-2.199f, -1f, -1.7349f);
			objimportOptions.localScale = new Vector3(0.045f, 0.045f, 0.045f);
			string objAbsolutePath = Application.dataPath + "/PolyFew/demo/TestModels/Meat.obj";
			string texturesFolderPath = Application.dataPath + "/PolyFew/demo/TestModels/textures";
			string materialsFolderPath = Application.dataPath + "/PolyFew/demo/TestModels/materials";
			GameObject importedObject;
			PolyfewRuntime.ImportOBJFromFileSystem(objAbsolutePath, texturesFolderPath, materialsFolderPath, delegate(GameObject imp)
			{
				importedObject = imp;
				Debug.Log("Successfully imported GameObject:   " + importedObject.name);
				this.barabarianRef.SetActive(false);
				this.targetObject = importedObject;
				this.ResetSettings();
				this.objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(this.targetObject, true);
				this.trianglesCount.text = (PolyfewRuntime.CountTriangles(true, this.targetObject).ToString() ?? "");
				this.exportButton.interactable = true;
				this.importFromWeb.interactable = false;
				this.importFromFileSystem.interactable = false;
				this.preserveFace.interactable = false;
				this.preservationStrength.interactable = false;
				this.disableTemporary = true;
				this.preservationSphere.gameObject.SetActive(false);
				this.disableTemporary = false;
			}, delegate(Exception ex)
			{
				Debug.LogError("Failed to load OBJ file.   " + ex.ToString());
			}, objimportOptions);
		}

		// Token: 0x06003224 RID: 12836 RVA: 0x00223E14 File Offset: 0x00222014
		public void ImportOBJFromNetwork()
		{
			this.isImportingFromNetwork = true;
			PolyfewRuntime.OBJImportOptions objimportOptions = new PolyfewRuntime.OBJImportOptions();
			objimportOptions.zUp = false;
			objimportOptions.localPosition = new Vector3(0.87815f, 1.4417f, -4.4708f);
			objimportOptions.localScale = new Vector3(0.0042f, 0.0042f, 0.0042f);
			string objURL = "https://dl.dropbox.com/s/v09bh0hiivja10e/onion.obj?dl=1";
			string objName = "onion";
			string diffuseTexURL = "https://dl.dropbox.com/s/0u4ij6sddi7a3gc/onion.jpg?dl=1";
			string bumpTexURL = "";
			string specularTexURL = "";
			string opacityTexURL = "";
			string materialURL = "https://dl.dropbox.com/s/fuzryqigs4gxwvv/onion.mtl?dl=1";
			this.progressSlider.value = 0f;
			this.uninteractivePanel.SetActive(true);
			this.downloadProgress = new PolyfewRuntime.ReferencedNumeric<float>(0f);
			base.StartCoroutine(this.UpdateProgress());
			GameObject importedObject;
			PolyfewRuntime.ImportOBJFromNetwork(objURL, objName, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL, materialURL, this.downloadProgress, delegate(GameObject imp)
			{
				this.AssignMeshesFromPairs();
				this.isImportingFromNetwork = false;
				importedObject = imp;
				this.barabarianRef.SetActive(false);
				this.targetObject = importedObject;
				this.ResetSettings();
				this.objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(this.targetObject, true);
				this.trianglesCount.text = (PolyfewRuntime.CountTriangles(true, this.targetObject).ToString() ?? "");
				this.exportButton.interactable = true;
				this.uninteractivePanel.SetActive(false);
				this.importFromWeb.interactable = false;
				this.importFromFileSystem.interactable = false;
				this.preserveFace.interactable = false;
				this.preservationStrength.interactable = false;
				this.disableTemporary = true;
				this.preservationSphere.gameObject.SetActive(false);
				this.disableTemporary = false;
			}, delegate(Exception ex)
			{
				this.uninteractivePanel.SetActive(false);
				this.isImportingFromNetwork = false;
				Debug.LogError("Failed to download and import OBJ file.   " + ex.Message);
			}, objimportOptions);
		}

		// Token: 0x06003225 RID: 12837 RVA: 0x00223F0C File Offset: 0x0022210C
		public void ExportGameObjectToOBJ()
		{
			string persistentDataPath = Application.persistentDataPath;
			GameObject exportObject = GameObject.Find("onion");
			if (exportObject)
			{
				exportObject = exportObject.transform.GetChild(0).GetChild(0).gameObject;
			}
			else
			{
				exportObject = GameObject.Find("Meat");
				if (!exportObject)
				{
					return;
				}
				exportObject = exportObject.transform.GetChild(0).GetChild(0).gameObject;
			}
			PolyfewRuntime.OBJExportOptions exportOptions = new PolyfewRuntime.OBJExportOptions(true, true, true, true, true);
			PolyfewRuntime.ExportGameObjectToOBJ(exportObject, persistentDataPath, delegate
			{
				Debug.Log("Successfully exported GameObject:  " + exportObject.name);
				string text = "Successfully exported the file to:  \n" + Application.persistentDataPath;
				this.StartCoroutine(this.ShowMessage(text));
			}, delegate(Exception ex)
			{
				Debug.LogError("Failed to export OBJ. " + ex.ToString());
			}, exportOptions);
		}

		// Token: 0x06003226 RID: 12838 RVA: 0x00223FF0 File Offset: 0x002221F0
		public void OnToggleStateChanged(bool isOn)
		{
			if (this.disableTemporary)
			{
				return;
			}
			this.preservationSphere.gameObject.SetActive(this.preserveFace.isOn);
			if (this.didApplyLosslessLast)
			{
				this.SimplifyLossless();
				return;
			}
			this.preservationStrength.interactable = this.preserveFace.isOn;
			this.OnReductionChange(this.reductionStrength.value);
		}

		// Token: 0x06003227 RID: 12839 RVA: 0x00224057 File Offset: 0x00222257
		public void OnPreservationStrengthChange(float value)
		{
			this.OnToggleStateChanged(true);
		}

		// Token: 0x06003228 RID: 12840 RVA: 0x00224060 File Offset: 0x00222260
		public void Reset()
		{
			this.ResetSettings();
			this.AssignMeshesFromPairs();
			if (GameObject.Find("onion"))
			{
				this.targetObject.SetActive(false);
			}
			else if (GameObject.Find("Meat"))
			{
				this.targetObject.SetActive(false);
			}
			this.targetObject = this.barabarianRef;
			this.preserveFace.interactable = true;
			this.preservationStrength.interactable = this.preserveFace.isOn;
			this.targetObject.SetActive(true);
			this.objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(this.targetObject, true);
			this.trianglesCount.text = (PolyfewRuntime.CountTriangles(true, this.targetObject).ToString() ?? "");
			this.exportButton.interactable = false;
			this.importFromWeb.interactable = true;
			this.importFromFileSystem.interactable = true;
		}

		// Token: 0x06003229 RID: 12841 RVA: 0x0022414D File Offset: 0x0022234D
		public static void OnSliderSelect()
		{
			FlyCamera.deactivated = true;
		}

		// Token: 0x0600322A RID: 12842 RVA: 0x00224155 File Offset: 0x00222355
		public static void OnSliderDeselect()
		{
			FlyCamera.deactivated = false;
		}

		// Token: 0x0600322B RID: 12843 RVA: 0x00224160 File Offset: 0x00222360
		private bool IsMouseOverUI(RectTransform uiElement)
		{
			Vector2 vector = uiElement.InverseTransformPoint(Input.mousePosition);
			return uiElement.rect.Contains(vector);
		}

		// Token: 0x0600322C RID: 12844 RVA: 0x00224192 File Offset: 0x00222392
		private IEnumerator ShowMessage(string message)
		{
			Debug.Log(message);
			this.message.text = message;
			yield return new WaitForSeconds(4.5f);
			this.message.text = "";
			yield break;
		}

		// Token: 0x0600322D RID: 12845 RVA: 0x002241A8 File Offset: 0x002223A8
		private void ResetSettings()
		{
			this.disableTemporary = true;
			this.reductionStrength.value = 0f;
			this.preserveUVSeams.isOn = false;
			this.preserveUVFoldover.isOn = false;
			this.preserveBorders.isOn = false;
			this.enableSmartLinking.isOn = true;
			this.preserveFace.isOn = false;
			this.preservationSphere.gameObject.SetActive(false);
			this.disableTemporary = false;
			this.preservationStrength.value = 100f;
		}

		// Token: 0x0600322E RID: 12846 RVA: 0x00224230 File Offset: 0x00222430
		private IEnumerator UpdateProgress()
		{
			for (;;)
			{
				yield return new WaitForSeconds(0.1f);
				this.progressSlider.value = this.downloadProgress.Value;
				this.progress.text = ((int)this.downloadProgress.Value).ToString() + "%";
			}
			yield break;
		}

		// Token: 0x0600322F RID: 12847 RVA: 0x00224240 File Offset: 0x00222440
		private void AssignMeshesFromPairs()
		{
			if (this.objectMeshPairs != null)
			{
				foreach (GameObject gameObject in this.objectMeshPairs.Keys)
				{
					if (gameObject != null)
					{
						PolyfewRuntime.MeshRendererPair meshRendererPair = this.objectMeshPairs[gameObject];
						if (!(meshRendererPair.mesh == null))
						{
							if (meshRendererPair.attachedToMeshFilter)
							{
								MeshFilter component = gameObject.GetComponent<MeshFilter>();
								if (!(component == null))
								{
									component.sharedMesh = meshRendererPair.mesh;
								}
							}
							else if (!meshRendererPair.attachedToMeshFilter)
							{
								SkinnedMeshRenderer component2 = gameObject.GetComponent<SkinnedMeshRenderer>();
								if (!(component2 == null))
								{
									component2.sharedMesh = meshRendererPair.mesh;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x04004DB4 RID: 19892
		public Slider reductionStrength;

		// Token: 0x04004DB5 RID: 19893
		public Slider preservationStrength;

		// Token: 0x04004DB6 RID: 19894
		public Toggle preserveUVFoldover;

		// Token: 0x04004DB7 RID: 19895
		public Toggle preserveUVSeams;

		// Token: 0x04004DB8 RID: 19896
		public Toggle preserveBorders;

		// Token: 0x04004DB9 RID: 19897
		public Toggle enableSmartLinking;

		// Token: 0x04004DBA RID: 19898
		public Toggle preserveFace;

		// Token: 0x04004DBB RID: 19899
		public Toggle recalculateNormals;

		// Token: 0x04004DBC RID: 19900
		public Toggle regardCurvature;

		// Token: 0x04004DBD RID: 19901
		public InputField trianglesCount;

		// Token: 0x04004DBE RID: 19902
		public Text message;

		// Token: 0x04004DBF RID: 19903
		public Text progress;

		// Token: 0x04004DC0 RID: 19904
		public Button exportButton;

		// Token: 0x04004DC1 RID: 19905
		public Button importFromFileSystem;

		// Token: 0x04004DC2 RID: 19906
		public Button importFromWeb;

		// Token: 0x04004DC3 RID: 19907
		public Slider progressSlider;

		// Token: 0x04004DC4 RID: 19908
		public GameObject uninteractivePanel;

		// Token: 0x04004DC5 RID: 19909
		public GameObject targetObject;

		// Token: 0x04004DC6 RID: 19910
		public Transform preservationSphere;

		// Token: 0x04004DC7 RID: 19911
		public EventSystem eventSystem;

		// Token: 0x04004DC8 RID: 19912
		private PolyfewRuntime.ObjectMeshPairs objectMeshPairs;

		// Token: 0x04004DC9 RID: 19913
		private bool didApplyLosslessLast;

		// Token: 0x04004DCA RID: 19914
		private bool disableTemporary;

		// Token: 0x04004DCB RID: 19915
		private GameObject barabarianRef;

		// Token: 0x04004DCC RID: 19916
		private PolyfewRuntime.ReferencedNumeric<float> downloadProgress = new PolyfewRuntime.ReferencedNumeric<float>(0f);

		// Token: 0x04004DCD RID: 19917
		private bool isImportingFromNetwork;

		// Token: 0x04004DCE RID: 19918
		private bool isWebGL;
	}
}
