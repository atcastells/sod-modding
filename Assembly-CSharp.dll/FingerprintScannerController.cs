using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Token: 0x020003B0 RID: 944
public class FingerprintScannerController : MonoBehaviour
{
	// Token: 0x06001562 RID: 5474 RVA: 0x00137308 File Offset: 0x00135508
	private void Start()
	{
		FirstPersonItemController.Instance.fingerprintLights.SetActive(false);
		this.screenLight.SetActive(false);
		this.SetOn(false);
		while (this.spawnedPrints.Count > 0)
		{
			Object.Destroy(this.spawnedPrints[0].gameObject);
			this.spawnedPrints.RemoveAt(0);
		}
		this.OnHoverOnNewPrint();
		for (int i = -4; i <= 4; i++)
		{
			for (int j = -5; j <= 5; j++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.pixelPrefab, this.printTransform);
				gameObject.transform.localPosition = new Vector3((float)i * 0.1f, (float)j * 0.1f, -0.01f);
				this.pixels.Add(gameObject);
				this.blockedPixelsActive.Add(gameObject);
			}
		}
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x001373DC File Offset: 0x001355DC
	public void SetOn(bool val)
	{
		this.isOn = val;
		this.printTransform.gameObject.SetActive(this.isOn);
		this.progressBar.gameObject.SetActive(this.isOn);
		this.screenText.gameObject.SetActive(this.isOn);
		FirstPersonItemController.Instance.fingerprintLights.SetActive(this.isOn);
		this.screenLight.SetActive(this.isOn);
		this.scanLight.enabled = this.isOn;
		if (this.isOn)
		{
			this.Flash(1, false, default(Color), 2f);
		}
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x00137488 File Offset: 0x00135688
	private void OnDestroy()
	{
		if (this.progressLoopEvent != null)
		{
			AudioController.Instance.StopSound(this.progressLoopEvent, AudioController.StopType.immediate, "end print scan");
		}
		if (FirstPersonItemController.Instance != null && FirstPersonItemController.Instance.fingerprintLights != null)
		{
			FirstPersonItemController.Instance.fingerprintLights.SetActive(false);
		}
		while (this.spawnedPrints.Count > 0)
		{
			if (this.spawnedPrints[0] != null)
			{
				Object.Destroy(this.spawnedPrints[0].gameObject);
			}
			this.spawnedPrints.RemoveAt(0);
		}
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x00137528 File Offset: 0x00135728
	private void Update()
	{
		Ray ray;
		ray..ctor(CameraController.Instance.cam.transform.position, CameraController.Instance.cam.transform.forward);
		HashSet<Transform> hashSet = new HashSet<Transform>();
		HashSet<Interactable> hashSet2 = new HashSet<Interactable>();
		HashSet<FingerprintScannerController.Print> hashSet3 = new HashSet<FingerprintScannerController.Print>();
		HashSet<Vector3> hashSet4 = new HashSet<Vector3>();
		if (this.screenOnDelay > 0f)
		{
			this.screenOnDelay -= Time.deltaTime;
			if (this.screenOnDelay <= 0f)
			{
				this.SetOn(true);
				this.screenOnDelay = 0f;
			}
		}
		if (this.isOn)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, ref raycastHit, 1.7f, Toolbox.Instance.interactionRayLayerMask))
			{
				PrintController component = raycastHit.transform.GetComponent<PrintController>();
				if (component != this.hoverPrint)
				{
					if (this.hoverPrint != null)
					{
						this.hoverPrint.ResetScan();
					}
					this.hoverPrint = component;
					this.OnHoverOnNewPrint();
				}
				FootprintController component2 = raycastHit.transform.GetComponent<FootprintController>();
				if (component2 != this.hoverFootPrint)
				{
					if (this.hoverFootPrint != null)
					{
						this.hoverFootPrint.ResetScan();
					}
					this.hoverFootPrint = component2;
					this.OnHoverOnNewPrint();
				}
			}
			else
			{
				if (this.hoverPrint != null)
				{
					this.hoverPrint.ResetScan();
					this.hoverPrint = null;
					this.OnHoverOnNewPrint();
				}
				if (this.hoverFootPrint != null)
				{
					this.hoverFootPrint.ResetScan();
					this.hoverFootPrint = null;
					this.OnHoverOnNewPrint();
				}
			}
			if (Physics.Raycast(ray, ref raycastHit, 1.7f, Toolbox.Instance.printDetectionRayLayerMask))
			{
				FirstPersonItemController.Instance.scannerRayPoint = raycastHit.point;
				foreach (Collider collider in Physics.OverlapSphere(raycastHit.point, FirstPersonItemController.Instance.printDetectionRadius))
				{
					hashSet.Add(collider.transform);
					if (!this.lookingAt.Contains(collider.transform))
					{
						this.lookingAt.Add(collider.transform);
					}
					if (!this.cachedStaticPrints.ContainsKey(collider.transform))
					{
						this.cachedStaticPrints.Add(collider.transform, this.GetPrintPoints(collider.transform));
					}
					foreach (FingerprintScannerController.Print print in this.cachedStaticPrints[collider.transform])
					{
						if (Vector3.Distance(print.worldPos, raycastHit.point) <= FirstPersonItemController.Instance.printDetectionRadius)
						{
							if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
							{
								ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
								if (chapterIntro != null && Player.Instance.currentGameLocation == chapterIntro.kidnapper.home)
								{
									Human owner = print.GetOwner();
									if (owner != chapterIntro.kidnapper && owner != chapterIntro.killer)
									{
										string text = "Chapter: Skipping print as it belong to ";
										Human human = owner;
										Game.Log(text + ((human != null) ? human.ToString() : null), 2);
										continue;
									}
								}
							}
							if (!hashSet4.Contains(print.worldPos))
							{
								hashSet3.Add(print);
								hashSet4.Add(print.worldPos);
							}
						}
					}
					InteractableController component3 = collider.transform.GetComponent<InteractableController>();
					if (component3 != null)
					{
						if (component3.interactable != null)
						{
							hashSet2.Add(component3.interactable);
							if (!component3.interactable.preset.enableDynamicFingerprints || component3.interactable.df.Count <= 0)
							{
								goto IL_56C;
							}
							if (!this.cachedDynamicPrints.ContainsKey(component3.interactable))
							{
								Game.Log("Player: Cache new dynamic prints for " + component3.interactable.name, 2);
								this.cachedDynamicPrints.Add(component3.interactable, this.GetDynamicPrints(component3));
							}
							using (HashSet<FingerprintScannerController.Print>.Enumerator enumerator = this.cachedDynamicPrints[component3.interactable].GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									FingerprintScannerController.Print print2 = enumerator.Current;
									if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
									{
										ChapterIntro chapterIntro2 = ChapterController.Instance.chapterScript as ChapterIntro;
										if (chapterIntro2 != null && Player.Instance.currentGameLocation == chapterIntro2.kidnapper.home)
										{
											Human owner2 = print2.GetOwner();
											if (owner2 != chapterIntro2.kidnapper && owner2 != chapterIntro2.killer)
											{
												string text2 = "Chapter: Skipping print as it belong to ";
												Human human2 = owner2;
												Game.Log(text2 + ((human2 != null) ? human2.ToString() : null), 2);
												continue;
											}
										}
									}
									if (Vector3.Distance(print2.worldPos, raycastHit.point) <= FirstPersonItemController.Instance.printDetectionRadius && !hashSet4.Contains(print2.worldPos))
									{
										hashSet3.Add(print2);
										hashSet4.Add(print2.worldPos);
									}
								}
								goto IL_56C;
							}
						}
						Game.LogError("No interatable has been assigned to " + collider.transform.name, 2);
					}
					IL_56C:;
				}
			}
			else
			{
				FirstPersonItemController.Instance.scannerRayPoint = new Vector3(0f, -99999f, 0f);
			}
		}
		List<Interactable> list = new List<Interactable>();
		using (Dictionary<Interactable, HashSet<FingerprintScannerController.Print>>.Enumerator enumerator2 = this.cachedDynamicPrints.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				KeyValuePair<Interactable, HashSet<FingerprintScannerController.Print>> keyValuePair = enumerator2.Current;
				if (keyValuePair.Key.controller == null)
				{
					list.Add(keyValuePair.Key);
				}
				else if (!hashSet2.Contains(keyValuePair.Key))
				{
					list.Add(keyValuePair.Key);
				}
			}
			goto IL_664;
		}
		IL_616:
		if (this.cachedDynamicPrints.ContainsKey(list[0]))
		{
			Game.Log("Player: Removing dynamic prints cache for " + list[0].name, 2);
			this.cachedDynamicPrints.Remove(list[0]);
		}
		list.RemoveAt(0);
		IL_664:
		if (list.Count <= 0)
		{
			float num = 5f;
			float intensity = 1f;
			if (this.hoverPrint != null)
			{
				this.hoverPrint.scanProgress += this.scanSpeed * Time.deltaTime;
				if (this.hoverPrint.scanProgress >= 1f)
				{
					if (!this.hoverPrint.printConfirmed)
					{
						this.hoverPrint.PrintConfirmed();
						this.Flash(2, false, default(Color), 10f);
						this.screenText.text = Strings.Get("misc", "Confirmed", Strings.Casing.asIs, false, false, false, null);
						foreach (GameObject gameObject in this.pixels)
						{
							gameObject.SetActive(false);
						}
						this.blockedPixelsActive.Clear();
						AudioController.Instance.Play2DSound(this.success, null, 1f);
					}
					this.progressBar.localScale = new Vector3(0.9f, 0.1f, 1f);
					if (this.progressLoopEvent != null)
					{
						AudioController.Instance.StopSound(this.progressLoopEvent, AudioController.StopType.immediate, "end print scan");
					}
				}
				else
				{
					TMP_Text tmp_Text = this.screenText;
					string text3 = Strings.Get("misc", "Scan", Strings.Casing.asIs, false, false, false, null);
					string text4 = " ";
					int i = Mathf.FloorToInt(this.hoverPrint.scanProgress * 100f);
					tmp_Text.text = text3 + text4 + i.ToString() + "%";
					this.progressBar.localScale = new Vector3(0.9f * this.hoverPrint.scanProgress, 0.1f, 1f);
					int num2 = Mathf.CeilToInt(this.hoverPrint.scanProgress * 99f);
					while (this.blockedPixelsActive.Count > 99 - num2)
					{
						int num3 = Random.Range(0, this.blockedPixelsActive.Count);
						this.blockedPixelsActive[num3].SetActive(false);
						this.blockedPixelsActive.RemoveAt(num3);
					}
					num = Random.Range(0f, Mathf.Max(350f * this.hoverPrint.scanProgress, 100f));
					intensity = num * 0.1f;
				}
			}
			if (this.hoverFootPrint != null)
			{
				this.hoverFootPrint.scanProgress += this.scanSpeed * Time.deltaTime;
				if (this.hoverFootPrint.scanProgress >= 1f)
				{
					if (!this.hoverFootPrint.printConfirmed)
					{
						this.hoverFootPrint.PrintConfirmed();
						this.Flash(2, false, default(Color), 10f);
						this.screenText.text = Strings.Get("misc", "Confirmed", Strings.Casing.asIs, false, false, false, null);
						foreach (GameObject gameObject2 in this.pixels)
						{
							gameObject2.SetActive(false);
						}
						this.blockedPixelsActive.Clear();
						AudioController.Instance.Play2DSound(this.success, null, 1f);
					}
					this.progressBar.localScale = new Vector3(0.9f, 0.1f, 1f);
					if (this.progressLoopEvent != null)
					{
						AudioController.Instance.StopSound(this.progressLoopEvent, AudioController.StopType.immediate, "end print scan");
					}
				}
				else
				{
					TMP_Text tmp_Text2 = this.screenText;
					string text5 = Strings.Get("misc", "Scan", Strings.Casing.asIs, false, false, false, null);
					string text6 = " ";
					int i = Mathf.FloorToInt(this.hoverFootPrint.scanProgress * 100f);
					tmp_Text2.text = text5 + text6 + i.ToString() + "%";
					this.progressBar.localScale = new Vector3(0.9f * this.hoverFootPrint.scanProgress, 0.1f, 1f);
					int num4 = Mathf.CeilToInt(this.hoverFootPrint.scanProgress * 99f);
					while (this.blockedPixelsActive.Count > 99 - num4)
					{
						int num5 = Random.Range(0, this.blockedPixelsActive.Count);
						this.blockedPixelsActive[num5].SetActive(false);
						this.blockedPixelsActive.RemoveAt(num5);
					}
					num = Random.Range(0f, Mathf.Max(350f * this.hoverFootPrint.scanProgress, 100f));
					intensity = num * 0.1f;
				}
			}
			FirstPersonItemController.Instance.printScannerPulseLight.intensity = num;
			this.scanLight.intensity = intensity;
			for (int j = 0; j < this.lookingAt.Count; j++)
			{
				Transform transform = this.lookingAt[j];
				if (!hashSet.Contains(transform))
				{
					this.lookingAt.RemoveAt(j);
					j--;
				}
			}
			for (int k = 0; k < this.spawnedPrints.Count; k++)
			{
				PrintController printController = this.spawnedPrints[k];
				if (printController == null)
				{
					this.spawnedPrints.RemoveAt(k);
					k--;
				}
				else if (!hashSet.Contains(printController.printData.parentTranform))
				{
					if (printController.gameObject != null)
					{
						Object.Destroy(printController.gameObject);
					}
					this.spawnedPrints.RemoveAt(k);
					k--;
				}
				else if (hashSet4.Contains(printController.printData.worldPos))
				{
					hashSet3.Remove(printController.printData);
					hashSet4.Remove(printController.printData.worldPos);
				}
				else
				{
					if (printController.gameObject != null)
					{
						Object.Destroy(printController.gameObject);
					}
					this.spawnedPrints.RemoveAt(k);
					k--;
				}
			}
			foreach (FingerprintScannerController.Print print3 in hashSet3)
			{
				GameObject gameObject3 = Object.Instantiate<GameObject>(PrefabControls.Instance.fingerprint, print3.parentTranform);
				gameObject3.transform.position = print3.worldPos;
				PrintController component4 = gameObject3.GetComponent<PrintController>();
				component4.Setup(print3);
				this.spawnedPrints.Add(component4);
			}
			if (this.flashActive)
			{
				if (this.cycle < this.flashRepeat && this.flashProgress < 2f)
				{
					this.flashProgress += this.flashSpeed * Time.deltaTime;
					if (this.flashProgress <= 1f)
					{
						this.flashF = this.flashProgress;
					}
					else
					{
						this.flashF = 2f - this.flashProgress;
					}
					this.screen.sharedMaterial.SetColor("_EmissiveColor", Color.Lerp(Color.black, this.flashColour, this.flashF));
					if (this.flashProgress >= 2f)
					{
						this.cycle++;
						this.flashProgress = 0f;
						return;
					}
				}
				else
				{
					this.flashActive = false;
				}
			}
			return;
		}
		goto IL_616;
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x00138358 File Offset: 0x00136558
	public void Flash(int newRepeat, bool colourOverride, Color colour = default(Color), float speed = 10f)
	{
		if (colourOverride)
		{
			this.flashColour = colour;
		}
		this.flashSpeed = speed;
		this.flashRepeat = newRepeat;
		if (this.flashActive)
		{
			this.flashRepeat += this.flashRepeat;
			return;
		}
		this.flashActive = true;
		this.cycle = 0;
		this.flashProgress = 0f;
		this.flashF = 0f;
		base.enabled = true;
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x001383C8 File Offset: 0x001365C8
	public void OnHoverOnNewPrint()
	{
		this.blockedPixelsActive.Clear();
		if (this.progressLoopEvent != null)
		{
			AudioController.Instance.StopSound(this.progressLoopEvent, AudioController.StopType.immediate, "end print scan");
		}
		foreach (GameObject gameObject in this.pixels)
		{
			gameObject.SetActive(true);
			this.blockedPixelsActive.Add(gameObject);
		}
		if (this.hoverPrint == null && this.hoverFootPrint == null)
		{
			this.screenText.text = Strings.Get("misc", "Null", Strings.Casing.asIs, false, false, false, null);
			this.progressBar.localScale = new Vector3(0f, 0.1f, 1f);
			if (this.isOn)
			{
				AudioController.Instance.Play2DSound(this.hoverOff, null, 1f);
				return;
			}
		}
		else
		{
			if ((this.hoverPrint != null && this.hoverPrint.printConfirmed) || (this.hoverFootPrint != null && this.hoverFootPrint.printConfirmed))
			{
				this.screenText.text = Strings.Get("misc", "Confirmed", Strings.Casing.asIs, false, false, false, null);
				foreach (GameObject gameObject2 in this.pixels)
				{
					gameObject2.SetActive(false);
				}
				this.blockedPixelsActive.Clear();
				AudioController.Instance.Play2DSound(this.detectExisting, null, 1f);
				this.Flash(1, false, default(Color), 12f);
				return;
			}
			AudioController.Instance.Play2DSound(this.detect, null, 1f);
			this.progressLoopEvent = AudioController.Instance.Play2DLooping(this.progressLoop, null, 1f);
		}
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x001385D4 File Offset: 0x001367D4
	private HashSet<FingerprintScannerController.Print> GetDynamicPrints(InteractableController interactable)
	{
		HashSet<FingerprintScannerController.Print> hashSet = new HashSet<FingerprintScannerController.Print>();
		if (interactable == null)
		{
			return hashSet;
		}
		MeshFilter component = interactable.transform.GetComponent<MeshFilter>();
		if (component == null)
		{
			return hashSet;
		}
		if (component.mesh == null)
		{
			return hashSet;
		}
		List<Vector3> list = new List<Vector3>();
		List<Human> list2 = new List<Human>();
		List<string> list3 = new List<string>();
		foreach (Interactable.DynamicFingerprint dynamicFingerprint in interactable.interactable.df)
		{
			Human human = null;
			if (CityData.Instance.GetHuman(dynamicFingerprint.id, out human, true))
			{
				list2.Add(human);
				list3.Add(dynamicFingerprint.seed);
			}
			else
			{
				Game.LogError("Cannot find citizen " + dynamicFingerprint.id.ToString(), 2);
			}
		}
		Game.Log("Player: Getting " + list2.Count.ToString() + " dynamic prints for " + interactable.name, 2);
		List<Vector3> printLocationsOnMesh = this.GetPrintLocationsOnMesh(component, list2.Count, out list, false, null, list3);
		for (int i = 0; i < printLocationsOnMesh.Count; i++)
		{
			Vector3 vector = printLocationsOnMesh[i];
			FingerprintScannerController.Print print = new FingerprintScannerController.Print();
			print.parentTranform = interactable.transform;
			print.worldPos = interactable.transform.TransformPoint(vector);
			print.normal = list[i];
			print.interactable = interactable.interactable;
			print.type = FingerprintScannerController.Print.PrintType.fingerPrint;
			print.dynamicOwner = list2[0];
			list2.RemoveAt(0);
			hashSet.Add(print);
		}
		return hashSet;
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x00138790 File Offset: 0x00136990
	private HashSet<FingerprintScannerController.Print> GetPrintPoints(Transform checkTransform)
	{
		HashSet<FingerprintScannerController.Print> hashSet = new HashSet<FingerprintScannerController.Print>();
		MeshFilter component = checkTransform.GetComponent<MeshFilter>();
		if (component == null)
		{
			return hashSet;
		}
		if (component.mesh == null)
		{
			return hashSet;
		}
		Game.Log("Player: Attempt to get prints for " + checkTransform.name + "...", 2);
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		NewRoom newRoom = null;
		Interactable interactable = null;
		FurnitureLocation furnitureLocation = null;
		FingerprintScannerController.Print.PrintType type = FingerprintScannerController.Print.PrintType.fingerPrint;
		RoomConfiguration.PrintsSource printsSource = RoomConfiguration.PrintsSource.inhabitants;
		if (checkTransform.gameObject.layer == 29)
		{
			Game.Log("Player: ...Object is on combined meshes layer...", 2);
			if (checkTransform.CompareTag("WallsMesh"))
			{
				foreach (NewRoom newRoom2 in CityData.Instance.visibleRooms)
				{
					if (newRoom2.combinedWalls != null && newRoom2.combinedWalls.transform == checkTransform)
					{
						newRoom = newRoom2;
						break;
					}
					foreach (KeyValuePair<NewBuilding, GameObject> keyValuePair in newRoom2.additionalWalls)
					{
						if (keyValuePair.Value.transform == checkTransform)
						{
							newRoom = newRoom2;
							break;
						}
					}
					if (newRoom != null)
					{
						break;
					}
				}
				if (newRoom != null && newRoom.preset.fingerprintsEnabled)
				{
					list = this.GetPrintLocationsOnMeshNonDynamic(component, newRoom.preset.fingerprintWallDensity, out list2, true, newRoom);
					type = FingerprintScannerController.Print.PrintType.fingerPrint;
					printsSource = newRoom.preset.printsSource;
				}
			}
		}
		else
		{
			InteractableController component2 = checkTransform.GetComponent<InteractableController>();
			if (component2 != null)
			{
				Game.Log("Player: Found interactable to scan...", 2);
				interactable = component2.interactable;
				if (interactable != null && interactable.preset.fingerprintsEnabled)
				{
					list = this.GetPrintLocationsOnMeshNonDynamic(component, interactable.preset.fingerprintDensity, out list2, false, null);
					if (list != null)
					{
						Game.Log("Player: ... Found " + list.Count.ToString() + " print locations...", 2);
					}
					type = FingerprintScannerController.Print.PrintType.fingerPrint;
					printsSource = interactable.preset.printsSource;
				}
			}
			else
			{
				BreakableWindowController component3 = checkTransform.GetComponent<BreakableWindowController>();
				if (component3 != null)
				{
					NewWall wall = component3.GetWall();
					if (wall != null)
					{
						MeshFilter component4 = checkTransform.GetComponent<MeshFilter>();
						if (component4 != null)
						{
							list = this.GetPrintLocationsOnMeshNonDynamic(component4, 2f, out list2, true, wall.node.room);
							type = FingerprintScannerController.Print.PrintType.fingerPrint;
							printsSource = RoomConfiguration.PrintsSource.inhabitants;
							foreach (MurderController.Murder murder in MurderController.Instance.activeMurders)
							{
								if (murder.sniperKillShotNode != Vector3Int.zero)
								{
									NewNode newNode = null;
									if (PathFinder.Instance.nodeMap.TryGetValue(murder.sniperKillShotNode, ref newNode) && (newNode == wall.node || newNode == wall.otherWall.node))
									{
										printsSource = RoomConfiguration.PrintsSource.killer;
										if (Game.Instance.printDebug)
										{
											Game.Log("Found killer sniper source for prints...", 2);
											break;
										}
										break;
									}
								}
							}
							if (printsSource != RoomConfiguration.PrintsSource.killer && wall.node.nodeCoord.z != 0)
							{
								return hashSet;
							}
						}
					}
				}
				else
				{
					foreach (NewRoom newRoom3 in CityData.Instance.visibleRooms)
					{
						foreach (FurnitureLocation furnitureLocation2 in newRoom3.individualFurniture)
						{
							if (furnitureLocation2 != null && !(furnitureLocation2.spawnedObject == null) && furnitureLocation2.spawnedObject.transform == checkTransform)
							{
								furnitureLocation = furnitureLocation2;
								break;
							}
						}
						if (furnitureLocation != null)
						{
							break;
						}
					}
					if (furnitureLocation != null && furnitureLocation.furniture.fingerprintsEnabled)
					{
						Game.Log("Player: Found furniture to scan...", 2);
						list = this.GetPrintLocationsOnMeshNonDynamic(component, furnitureLocation.furniture.fingerprintDensity, out list2, true, furnitureLocation.anchorNode.room);
						type = FingerprintScannerController.Print.PrintType.fingerPrint;
						printsSource = furnitureLocation.furniture.printsSource;
					}
					else
					{
						Game.Log("Player: ... Unable to find furniture spawned that matches this transform", 2);
					}
				}
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			Vector3 vector = list[i];
			hashSet.Add(new FingerprintScannerController.Print
			{
				parentTranform = checkTransform,
				worldPos = checkTransform.TransformPoint(vector),
				normal = list2[i],
				room = newRoom,
				interactable = interactable,
				furniture = furnitureLocation,
				type = type,
				source = printsSource
			});
		}
		return hashSet;
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x00138CA0 File Offset: 0x00136EA0
	private List<Vector3> GetPrintLocationsOnMeshNonDynamic(MeshFilter meshFilter, float printDensityPerUnit, out List<Vector3> normals, bool useHeightThreshold = false, NewRoom heightThresholdRoom = null)
	{
		normals = new List<Vector3>();
		if (meshFilter == null)
		{
			return new List<Vector3>();
		}
		Mesh mesh = meshFilter.mesh;
		List<Vector3> result = new List<Vector3>();
		if (mesh == null)
		{
			return result;
		}
		if (!mesh.isReadable)
		{
			Game.Log("Mesh " + mesh.name + " is not readable, this could be because of static batching. Searching for collider mesh we can use instead...", 2);
			MeshCollider component = meshFilter.gameObject.GetComponent<MeshCollider>();
			if (!(component != null))
			{
				Game.LogError("Mesh " + mesh.name + " is not readable! Fingerprints cannot be gathered as the verts aren't readable...", 2);
				return result;
			}
			if (!(component.sharedMesh != null) || !component.sharedMesh.isReadable)
			{
				Game.LogError("Mesh " + mesh.name + " is not readable! Fingerprints cannot be gathered as the verts aren't readable...", 2);
				return result;
			}
			Game.Log("...Using mesh from mesh collider instead: " + component.sharedMesh.name, 2);
			mesh = component.sharedMesh;
		}
		float[] triSizes = this.GetTriSizes(mesh.triangles, mesh.vertices);
		float[] array = new float[triSizes.Length];
		float num = 0f;
		for (int i = 0; i < triSizes.Length; i++)
		{
			num += triSizes[i];
			array[i] = num;
		}
		int prints = Mathf.Max(Mathf.RoundToInt(num * printDensityPerUnit), 1);
		return this.GetPrintLocationsOnMesh(meshFilter, prints, out normals, useHeightThreshold, heightThresholdRoom, null);
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x00138DFC File Offset: 0x00136FFC
	private List<Vector3> GetPrintLocationsOnMesh(MeshFilter meshFilter, int prints, out List<Vector3> normals, bool useHeightThreshold = false, NewRoom heightThresholdRoom = null, List<string> seeds = null)
	{
		Game.Log(string.Concat(new string[]
		{
			"Player: Looking for ",
			prints.ToString(),
			" on mesh ",
			meshFilter.sharedMesh.name,
			"..."
		}), 2);
		normals = new List<Vector3>();
		if (meshFilter == null)
		{
			return new List<Vector3>();
		}
		Mesh mesh = meshFilter.mesh;
		List<Vector3> list = new List<Vector3>();
		if (mesh == null)
		{
			return list;
		}
		if (!mesh.isReadable)
		{
			Game.Log("Mesh " + mesh.name + " is not readable, this could be because of static batching. Searching for collider mesh we can use instead...", 2);
			MeshCollider component = meshFilter.gameObject.GetComponent<MeshCollider>();
			if (!(component != null))
			{
				Game.LogError("Mesh " + mesh.name + " is not readable! Fingerprints cannot be gathered as the verts aren't readable...", 2);
				return list;
			}
			if (!(component.sharedMesh != null) || !component.sharedMesh.isReadable)
			{
				Game.LogError("Mesh " + mesh.name + " is not readable! Fingerprints cannot be gathered as the verts aren't readable...", 2);
				return list;
			}
			Game.Log("...Using mesh from mesh collider instead: " + component.sharedMesh.name, 2);
			mesh = component.sharedMesh;
		}
		float[] triSizes = this.GetTriSizes(mesh.triangles, mesh.vertices);
		float[] array = new float[triSizes.Length];
		float num = 0f;
		for (int i = 0; i < triSizes.Length; i++)
		{
			num += triSizes[i];
			array[i] = num;
		}
		string str = meshFilter.gameObject.transform.position.ToString();
		for (int j = 0; j < prints; j++)
		{
			if (seeds != null && seeds.Count > 0)
			{
				str = seeds[0];
				seeds.RemoveAt(0);
			}
			float psuedoRandomNumber = Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, str, false);
			str = psuedoRandomNumber.ToString();
			float num2 = psuedoRandomNumber * num;
			int num3 = -1;
			for (int k = 0; k < triSizes.Length; k++)
			{
				if (num2 <= array[k])
				{
					num3 = k;
					break;
				}
			}
			if (num3 == -1)
			{
				Game.LogError("triIndex should never be -1", 2);
			}
			Vector3 vector = mesh.vertices[mesh.triangles[num3 * 3]];
			Vector3 vector2 = mesh.vertices[mesh.triangles[num3 * 3 + 1]];
			Vector3 vector3 = mesh.vertices[mesh.triangles[num3 * 3 + 2]];
			float num4 = Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, str, false);
			str = num4.ToString();
			float num5 = Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, str, false);
			if (num4 + num5 >= 1f)
			{
				num4 = 1f - num4;
				num5 = 1f - num5;
			}
			Vector3 vector4 = vector + num4 * (vector2 - vector) + num5 * (vector3 - vector);
			Vector3 vector5 = mesh.normals[mesh.triangles[num3 * 3]];
			Vector3 vector6 = mesh.normals[mesh.triangles[num3 * 3 + 1]];
			Vector3 vector7 = mesh.normals[mesh.triangles[num3 * 3 + 2]];
			Vector3 vector8 = (vector5 + vector6 + vector7) / 3f;
			vector8.Normalize();
			vector4..ctor(vector4.x * meshFilter.transform.lossyScale.x, vector4.y * meshFilter.transform.lossyScale.y, vector4.z * meshFilter.transform.lossyScale.z);
			if (!useHeightThreshold || !(heightThresholdRoom != null) || vector4.y <= Enumerable.First<NewNode>(heightThresholdRoom.nodes).position.y + 1.8f || vector4.y >= Enumerable.First<NewNode>(heightThresholdRoom.nodes).position.y + 0.35f)
			{
				str = vector4.ToString();
				list.Add(vector4);
				normals.Add(vector8);
			}
		}
		return list;
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x00139234 File Offset: 0x00137434
	private float[] GetTriSizes(int[] tris, Vector3[] verts)
	{
		int num = tris.Length / 3;
		float[] array = new float[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = 0.5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
		}
		return array;
	}

	// Token: 0x04001A78 RID: 6776
	[Header("Components")]
	public TextMeshPro screenText;

	// Token: 0x04001A79 RID: 6777
	public Transform progressBar;

	// Token: 0x04001A7A RID: 6778
	public MeshRenderer screen;

	// Token: 0x04001A7B RID: 6779
	public Transform printTransform;

	// Token: 0x04001A7C RID: 6780
	public GameObject pixelPrefab;

	// Token: 0x04001A7D RID: 6781
	private List<GameObject> pixels = new List<GameObject>();

	// Token: 0x04001A7E RID: 6782
	public List<GameObject> blockedPixelsActive = new List<GameObject>();

	// Token: 0x04001A7F RID: 6783
	public GameObject screenLight;

	// Token: 0x04001A80 RID: 6784
	public Light scanLight;

	// Token: 0x04001A81 RID: 6785
	public bool isOn;

	// Token: 0x04001A82 RID: 6786
	public float screenOnDelay = 1.65f;

	// Token: 0x04001A83 RID: 6787
	[Header("Audio")]
	public AudioEvent progressLoop;

	// Token: 0x04001A84 RID: 6788
	public AudioEvent detect;

	// Token: 0x04001A85 RID: 6789
	public AudioEvent detectExisting;

	// Token: 0x04001A86 RID: 6790
	public AudioEvent success;

	// Token: 0x04001A87 RID: 6791
	public AudioEvent hoverOff;

	// Token: 0x04001A88 RID: 6792
	private AudioController.LoopingSoundInfo progressLoopEvent;

	// Token: 0x04001A89 RID: 6793
	[Header("Prints")]
	[Tooltip("List of valid objects being looked at")]
	public List<Transform> lookingAt = new List<Transform>();

	// Token: 0x04001A8A RID: 6794
	[Tooltip("Spawned prints")]
	public List<PrintController> spawnedPrints = new List<PrintController>();

	// Token: 0x04001A8B RID: 6795
	[Tooltip("Hovered over this print")]
	public PrintController hoverPrint;

	// Token: 0x04001A8C RID: 6796
	[Tooltip("Hovered over this footprint")]
	public FootprintController hoverFootPrint;

	// Token: 0x04001A8D RID: 6797
	[Tooltip("How fast a print is scanned (seconds)")]
	public float scanSpeed = 1f;

	// Token: 0x04001A8E RID: 6798
	[Tooltip("Flash the screen")]
	private bool flashActive;

	// Token: 0x04001A8F RID: 6799
	private float flashSpeed = 10f;

	// Token: 0x04001A90 RID: 6800
	[ColorUsage(true, true)]
	public Color flashColour;

	// Token: 0x04001A91 RID: 6801
	private int cycle;

	// Token: 0x04001A92 RID: 6802
	private float flashProgress;

	// Token: 0x04001A93 RID: 6803
	private float flashF;

	// Token: 0x04001A94 RID: 6804
	private int flashRepeat = 3;

	// Token: 0x04001A95 RID: 6805
	private Dictionary<Transform, HashSet<FingerprintScannerController.Print>> cachedStaticPrints = new Dictionary<Transform, HashSet<FingerprintScannerController.Print>>();

	// Token: 0x04001A96 RID: 6806
	private Dictionary<Interactable, HashSet<FingerprintScannerController.Print>> cachedDynamicPrints = new Dictionary<Interactable, HashSet<FingerprintScannerController.Print>>();

	// Token: 0x020003B1 RID: 945
	[Serializable]
	public class Print
	{
		// Token: 0x0600156E RID: 5486 RVA: 0x0013932C File Offset: 0x0013752C
		public Human GetOwner()
		{
			if (this.dynamicOwner == null)
			{
				List<Human> fingerprintOwnerPool = Toolbox.Instance.GetFingerprintOwnerPool(this.room, this.furniture, this.interactable, this.source, this.worldPos, true);
				return fingerprintOwnerPool[Toolbox.Instance.GetPsuedoRandomNumber(0, fingerprintOwnerPool.Count, this.worldPos.ToString(), false)];
			}
			return this.dynamicOwner;
		}

		// Token: 0x04001A97 RID: 6807
		[Header("Serialized")]
		public Vector3 worldPos;

		// Token: 0x04001A98 RID: 6808
		public Vector3 normal;

		// Token: 0x04001A99 RID: 6809
		public FingerprintScannerController.Print.PrintType type;

		// Token: 0x04001A9A RID: 6810
		public RoomConfiguration.PrintsSource source;

		// Token: 0x04001A9B RID: 6811
		[Header("Non-Serialized")]
		[NonSerialized]
		public Transform parentTranform;

		// Token: 0x04001A9C RID: 6812
		[NonSerialized]
		public NewRoom room;

		// Token: 0x04001A9D RID: 6813
		[NonSerialized]
		public Interactable interactable;

		// Token: 0x04001A9E RID: 6814
		[NonSerialized]
		public FurnitureLocation furniture;

		// Token: 0x04001A9F RID: 6815
		[NonSerialized]
		public Human dynamicOwner;

		// Token: 0x020003B2 RID: 946
		public enum PrintType
		{
			// Token: 0x04001AA1 RID: 6817
			fingerPrint,
			// Token: 0x04001AA2 RID: 6818
			footPrint
		}
	}
}
