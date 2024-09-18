using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020003A7 RID: 935
public class ComputerController : MonoBehaviour
{
	// Token: 0x06001538 RID: 5432 RVA: 0x00134A1C File Offset: 0x00132C1C
	public void Setup(InteractableController newController)
	{
		this.ic = newController;
		this.ic.interactable.OnSwitchChange += this.OnSwitchStateChange;
		this.ic.interactable.OnState1Change += this.OnPlayerControlChange;
		if (this.ic.interactable.locked)
		{
			this.SetLoggedIn(null);
		}
		else
		{
			Human loggedIn = null;
			CityData.Instance.GetHuman((int)this.ic.interactable.val, out loggedIn, true);
			this.SetLoggedIn(loggedIn);
		}
		this.screenLightContainer.gameObject.SetActive(false);
		this.OnSwitchStateChange();
		this.OnPlayerControlChange();
		this.m_EventSystem = base.GetComponent<EventSystem>();
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x00134AD4 File Offset: 0x00132CD4
	private void OnDestroy()
	{
		if (this.ic != null && this.ic.interactable != null)
		{
			this.ic.interactable.OnSwitchChange -= this.OnSwitchStateChange;
			this.ic.interactable.OnState1Change -= this.OnPlayerControlChange;
		}
		if (this.loadLoop != null)
		{
			AudioController.Instance.StopSound(this.loadLoop, AudioController.StopType.triggerCue, "Computer destroy");
		}
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x00134B54 File Offset: 0x00132D54
	public void OnSwitchStateChange()
	{
		if (this.ic != null && this.ic.interactable != null)
		{
			if (this.ic.interactable.sw0)
			{
				if (this.loggedInAs != null)
				{
					this.SetComputerApp(this.ic.interactable.preset.desktopApp, true);
				}
				else
				{
					this.SetComputerApp(this.ic.interactable.preset.bootApp, true);
				}
				if (this.screenLightContainer != null)
				{
					this.screenLightContainer.gameObject.SetActive(true);
				}
				if (this != null)
				{
					base.enabled = true;
				}
				if (this.loadLoop != null)
				{
					AudioController.Instance.StopSound(this.loadLoop, AudioController.StopType.triggerCue, "Stop existing sound");
				}
				this.loadLoop = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.computerHDDLoading, null, this.ic.interactable, null, 1f, false, true, null, null);
				return;
			}
			if (!this.ic.interactable.sw0)
			{
				if (this.loadLoop != null)
				{
					AudioController.Instance.StopSound(this.loadLoop, AudioController.StopType.fade, "Computer load key off");
				}
				if (this.screenLightContainer != null)
				{
					this.screenLightContainer.gameObject.SetActive(false);
				}
				if (this.computerRenderer != null)
				{
					this.computerRenderer.sharedMaterial = this.lightOffMaterial;
				}
				this.SetComputerApp(this.ic.interactable.preset.bootApp, true);
				while (this.spawnedContent.Count > 0)
				{
					Object.Destroy(this.spawnedContent[0]);
					this.spawnedContent.RemoveAt(0);
				}
				this.SetLoggedIn(null);
				if (this != null)
				{
					base.enabled = false;
				}
			}
		}
	}

	// Token: 0x0600153B RID: 5435 RVA: 0x00134D30 File Offset: 0x00132F30
	public void OnPlayerControlChange()
	{
		if (this.ic != null && this.ic.interactable != null && this.ic.interactable.sw1)
		{
			this.playerControlled = true;
			if (this.raycaster == null && this.osCanvas != null)
			{
				this.osCanvas.enabled = true;
				this.osCanvas.worldCamera = Camera.main;
				this.raycaster = this.osCanvas.transform.gameObject.AddComponent<GraphicRaycaster>();
				this.raycaster.blockingObjects = 2;
			}
		}
		else
		{
			this.playerControlled = false;
			this.SetPlayerCrunchingDatabase(false);
			if (this.raycaster != null)
			{
				Object.Destroy(this.raycaster);
				if (this.osCanvas != null)
				{
					this.osCanvas.enabled = false;
				}
			}
			InterfaceController.Instance.SetCrosshairVisible(true);
		}
		if (this.currentApp != null)
		{
			this.EnableCursor(this.currentApp.useCursor);
		}
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x00134E4C File Offset: 0x0013304C
	public void SetComputerApp(CruncherAppPreset newApp, bool forceUpdate = false)
	{
		if (this.currentApp != newApp || forceUpdate)
		{
			this.currentApp = newApp;
			if (this.computerRenderer != null)
			{
				this.computerRenderer.sharedMaterial = this.lightOffMaterial;
			}
			while (this.spawnedContent.Count > 0)
			{
				Object.Destroy(this.spawnedContent[0]);
				this.spawnedContent.RemoveAt(0);
			}
			this.appLoadProgress = 0f;
			this.appTimerProgress = 0f;
			this.appLoaded = false;
			if (this.currentApp != null)
			{
				if (this.screenRenderer != null)
				{
					this.screenRenderer.sharedMaterial = this.currentApp.loadBackground;
				}
				this.EnableCursor(this.currentApp.useCursor);
				if (this.screenLight != null)
				{
					this.screenLight.SetColour(this.currentApp.screenLightColourOnLoad);
				}
				if (this.currentApp.onStartSound != null)
				{
					if (this.playerControlled)
					{
						AudioController.Instance.PlayWorldOneShot(this.currentApp.onStartSound, Player.Instance, this.ic.interactable.node, this.ic.interactable.wPos, null, null, 1f, null, false, null, false);
						return;
					}
					Human who = null;
					this.ic.interactable.usagePoint.TryGetUserAtSlot(Interactable.UsePointSlot.defaultSlot, out who);
					AudioController.Instance.PlayWorldOneShot(this.currentApp.onStartSound, who, this.ic.interactable.node, this.ic.interactable.wPos, null, null, 1f, null, false, null, false);
				}
			}
		}
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x00135008 File Offset: 0x00133208
	public void SetLoggedIn(Human newLogIn)
	{
		this.loggedInAs = newLogIn;
		if (this.loggedInAs != null)
		{
			this.ic.interactable.val = (float)this.loggedInAs.humanID;
			this.ic.interactable.SetLockedState(false, this.loggedInAs, true, false);
			return;
		}
		this.ic.interactable.val = 0f;
		this.ic.interactable.SetLockedState(true, null, true, false);
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x0013508C File Offset: 0x0013328C
	public void OnAppLoaded()
	{
		this.computerRenderer.sharedMaterial = this.lightOffMaterial;
		this.screenRenderer.sharedMaterial = this.currentApp.loadedBackground;
		if (this.screenLight != null)
		{
			this.screenLight.SetColour(this.currentApp.screenLightColourOnFinishLoad);
		}
		foreach (GameObject gameObject in this.currentApp.appContent)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, this.osCanvas.transform);
			CruncherAppContent component = gameObject2.GetComponent<CruncherAppContent>();
			if (component != null)
			{
				component.Setup(this);
			}
			gameObject2.transform.SetAsFirstSibling();
			this.spawnedContent.Add(gameObject2);
		}
		if (this.currentApp.onFinishedLoadingSound != null)
		{
			if (this.playerControlled)
			{
				AudioController.Instance.PlayWorldOneShot(this.currentApp.onFinishedLoadingSound, Player.Instance, this.ic.interactable.node, this.ic.interactable.wPos, null, null, 1f, null, false, null, false);
			}
			else
			{
				Human who = null;
				this.ic.interactable.usagePoint.TryGetUserAtSlot(Interactable.UsePointSlot.defaultSlot, out who);
				AudioController.Instance.PlayWorldOneShot(this.currentApp.onFinishedLoadingSound, who, this.ic.interactable.node, this.ic.interactable.wPos, null, null, 1f, null, false, null, false);
			}
		}
		this.appLoaded = true;
		this.EnableCursor(this.currentApp.useCursor);
		if (this.currentApp.name == "GovDatabase" || this.currentApp.name == "EmployeeDatabase")
		{
			this.SetPlayerCrunchingDatabase(true);
		}
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x00135278 File Offset: 0x00133478
	public void OnAppExit()
	{
		if (Player.Instance.isCrunchingDatabase)
		{
			this.SetPlayerCrunchingDatabase(false);
		}
		this.computerRenderer.sharedMaterial = this.lightOffMaterial;
		while (this.spawnedContent.Count > 0)
		{
			Object.Destroy(this.spawnedContent[0]);
			this.spawnedContent.RemoveAt(0);
		}
		if (this.currentApp.onExitSound != null)
		{
			if (this.playerControlled)
			{
				AudioController.Instance.PlayWorldOneShot(this.currentApp.onExitSound, Player.Instance, this.ic.interactable.node, this.ic.interactable.wPos, null, null, 1f, null, false, null, false);
			}
			else
			{
				Human who = null;
				this.ic.interactable.usagePoint.TryGetUserAtSlot(Interactable.UsePointSlot.defaultSlot, out who);
				AudioController.Instance.PlayWorldOneShot(this.currentApp.onExitSound, who, this.ic.interactable.node, this.ic.interactable.wPos, null, null, 1f, null, false, null, false);
			}
		}
		if (!(this.ic.interactable.preset.bootApp == this.currentApp))
		{
			if (this.currentApp.openOnEnd != null)
			{
				this.SetComputerApp(this.currentApp.openOnEnd, false);
			}
			return;
		}
		if (this.loggedInAs != null)
		{
			this.SetComputerApp(this.ic.interactable.preset.desktopApp, false);
			return;
		}
		this.SetComputerApp(this.ic.interactable.preset.logInApp, false);
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x00135428 File Offset: 0x00133628
	public void EnableCursor(bool val)
	{
		if (!this.playerControlled)
		{
			val = false;
		}
		this.useCursor = val;
		if (!this.useCursor)
		{
			if (this.cursorRect != null)
			{
				Object.Destroy(this.cursorRect.gameObject);
			}
			return;
		}
		if (this.cursorRect == null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(GameplayControls.Instance.OScursor, this.osCanvas.transform);
			this.cursorRect = gameObject.GetComponent<RectTransform>();
			this.cursorImage = gameObject.GetComponent<Image>();
		}
		if (this.appLoaded)
		{
			this.cursorImage.sprite = this.currentApp.cursorSprite;
			return;
		}
		this.cursorImage.sprite = GameplayControls.Instance.loadCursor;
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x001354E4 File Offset: 0x001336E4
	private void Update()
	{
		if (SessionData.Instance.play)
		{
			if (this.ic.interactable == null)
			{
				return;
			}
			if (!this.ic.interactable.sw0)
			{
				return;
			}
			if (this.currentApp == null)
			{
				return;
			}
			if (this.appLoadProgress < 1f)
			{
				if (this.currentApp.loadTime <= 0f)
				{
					this.appLoadProgress = 1f;
					this.OnAppLoaded();
				}
				else
				{
					if (Toolbox.Instance.Rand(0f, 1f, false) <= this.currentApp.loadDemand)
					{
						this.computerRenderer.sharedMaterial = this.lightOnMaterial;
						if (this.loadLoop != null)
						{
							this.loadLoop.audioEvent.setParameterByName("load", 1f, false);
						}
					}
					else
					{
						this.computerRenderer.sharedMaterial = this.lightOffMaterial;
						if (this.loadLoop != null)
						{
							this.loadLoop.audioEvent.setParameterByName("load", 0f, false);
						}
					}
					this.appLoadProgress += Time.deltaTime / this.currentApp.loadTime;
					this.appLoadProgress = Mathf.Clamp01(this.appLoadProgress);
					if (this.appLoadProgress >= 1f)
					{
						this.OnAppLoaded();
					}
				}
			}
			else
			{
				if (this.currentApp.alwaysLoad)
				{
					if (Toolbox.Instance.Rand(0f, 1f, false) <= this.currentApp.alwaysLoadDemand)
					{
						this.computerRenderer.sharedMaterial = this.lightOnMaterial;
						if (this.loadLoop != null)
						{
							this.loadLoop.audioEvent.setParameterByName("load", 1f, false);
						}
					}
					else
					{
						this.computerRenderer.sharedMaterial = this.lightOffMaterial;
						if (this.loadLoop != null)
						{
							this.loadLoop.audioEvent.setParameterByName("load", 0f, false);
						}
					}
				}
				else if (this.timedLoading > 0f)
				{
					this.timedLoading -= Time.deltaTime;
					if (Toolbox.Instance.Rand(0f, 1f, false) <= this.timedLoadingDemand)
					{
						this.computerRenderer.sharedMaterial = this.lightOnMaterial;
						if (this.loadLoop != null)
						{
							this.loadLoop.audioEvent.setParameterByName("load", 1f, false);
						}
					}
					else
					{
						this.computerRenderer.sharedMaterial = this.lightOffMaterial;
						if (this.loadLoop != null)
						{
							this.loadLoop.audioEvent.setParameterByName("load", 0f, false);
						}
					}
				}
				if (this.currentApp.useTimer)
				{
					this.appTimerProgress += Time.deltaTime / this.currentApp.timerLength;
					this.appTimerProgress = Mathf.Clamp01(this.appTimerProgress);
					if (this.appTimerProgress >= 1f)
					{
						this.OnAppExit();
					}
				}
			}
			if (this.playerControlled)
			{
				if (InputController.Instance.mouseInputMode)
				{
					if (Input.GetMouseButtonUp(0))
					{
						AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerCursorClick, Player.Instance, this.ic.interactable.node, this.ic.interactable.wPos, null, null, 1f, null, false, null, false);
					}
				}
				else if (InputController.Instance.player.GetButtonUp("Primary"))
				{
					AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerCursorClick, Player.Instance, this.ic.interactable.node, this.ic.interactable.wPos, null, null, 1f, null, false, null, false);
				}
			}
			if (this.printTimer > 0f)
			{
				this.printTimer -= Time.deltaTime * 0.5f;
				this.printTimer = Mathf.Clamp01(this.printTimer);
				this.printerParent.localPosition = Vector3.Lerp(this.printOutEndPos, this.printOutStartPos, this.printTimer);
				if (this.printedDocument != null)
				{
					this.printedDocument.MoveInteractable(this.printerParent.transform.position, this.ic.transform.eulerAngles, true);
				}
			}
			if (this.useCursor)
			{
				this.UpdateCursor();
			}
		}
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x00135944 File Offset: 0x00133B44
	public void SetTimedLoading(float forSeconds, float loadDemand = 0.33f)
	{
		this.timedLoading = forSeconds;
		this.timedLoadingDemand = loadDemand;
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x00135954 File Offset: 0x00133B54
	private void UpdateCursor()
	{
		if (!this.useCursor)
		{
			return;
		}
		if (this.cursorRect != null)
		{
			Ray ray;
			ray..ctor(CameraController.Instance.cam.transform.position, CameraController.Instance.cam.transform.forward);
			int num = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
			{
				26
			});
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, ref raycastHit, 2f, num, 2))
			{
				Vector2 textureCoord = raycastHit.textureCoord;
				textureCoord.x *= 256f;
				textureCoord.y *= 256f;
				textureCoord.y -= 6f;
				this.cursorRect.anchoredPosition = this.TexToCanvas(textureCoord);
				if (InterfaceController.Instance.crosshairVisible)
				{
					InterfaceController.Instance.SetCrosshairVisible(false);
				}
			}
			else if (!InterfaceController.Instance.crosshairVisible)
			{
				InterfaceController.Instance.SetCrosshairVisible(true);
			}
			if (this.appLoaded && !GameplayController.Instance.activeGadgets.Exists((Interactable item) => item.preset.specialCaseFlag == InteractablePreset.SpecialCase.codebreaker && item.objectRef == this.ic.interactable))
			{
				this.currentHover = null;
				this.m_PointerEventData = new PointerEventData(this.m_EventSystem);
				this.m_PointerEventData.position = Input.mousePosition;
				List<RaycastResult> list = new List<RaycastResult>();
				this.raycaster.Raycast(this.m_PointerEventData, list);
				foreach (RaycastResult raycastResult in list)
				{
					ComputerOSUIComponent component = raycastResult.gameObject.GetComponent<ComputerOSUIComponent>();
					if (component != null)
					{
						this.currentHover = component;
						if (InputController.Instance.mouseInputMode)
						{
							if (Input.GetMouseButtonUp(0))
							{
								this.currentHover.OnLeftClick();
								this.OnClickOnOSElement(this.currentHover);
								break;
							}
							break;
						}
						else
						{
							if (InputController.Instance.player.GetButtonDown("Primary"))
							{
								this.currentHover.OnLeftClick();
								this.OnClickOnOSElement(this.currentHover);
								break;
							}
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x00135B80 File Offset: 0x00133D80
	public Vector2 TexToCanvas(Vector2 texCoord)
	{
		texCoord.x = (float)Mathf.Clamp(Mathf.RoundToInt(texCoord.x), 0, 255);
		texCoord.y = (float)Mathf.Clamp(Mathf.RoundToInt(texCoord.y), 50, 255);
		texCoord.x = texCoord.x / 255f - 0.5f;
		texCoord.y = texCoord.y / 255f - 0.5f;
		return texCoord;
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x00135C00 File Offset: 0x00133E00
	public void OnClickOnOSElement(ComputerOSUIComponent c)
	{
		if (c.sfx != null)
		{
			if (this.playerControlled)
			{
				AudioController.Instance.PlayWorldOneShot(c.sfx, Player.Instance, this.ic.interactable.node, this.ic.interactable.wPos, null, null, 1f, null, false, null, false);
				return;
			}
			Human who = null;
			this.ic.interactable.usagePoint.TryGetUserAtSlot(Interactable.UsePointSlot.defaultSlot, out who);
			AudioController.Instance.PlayWorldOneShot(c.sfx, who, this.ic.interactable.node, this.ic.interactable.wPos, null, null, 1f, null, false, null, false);
		}
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x00135CC0 File Offset: 0x00133EC0
	private void SetPlayerCrunchingDatabase(bool condition)
	{
		if (condition)
		{
			Player.Instance.isCrunchingDatabase = true;
			return;
		}
		Player.Instance.isCrunchingDatabase = false;
	}

	// Token: 0x04001A2F RID: 6703
	[Header("Components")]
	public MeshRenderer computerRenderer;

	// Token: 0x04001A30 RID: 6704
	public MeshRenderer screenRenderer;

	// Token: 0x04001A31 RID: 6705
	public GameObject screenLightContainer;

	// Token: 0x04001A32 RID: 6706
	public InteractableController ic;

	// Token: 0x04001A33 RID: 6707
	public Material lightOffMaterial;

	// Token: 0x04001A34 RID: 6708
	public Material lightOnMaterial;

	// Token: 0x04001A35 RID: 6709
	public Canvas osCanvas;

	// Token: 0x04001A36 RID: 6710
	public GraphicRaycaster raycaster;

	// Token: 0x04001A37 RID: 6711
	private PointerEventData m_PointerEventData;

	// Token: 0x04001A38 RID: 6712
	private EventSystem m_EventSystem;

	// Token: 0x04001A39 RID: 6713
	public RectTransform cursorRect;

	// Token: 0x04001A3A RID: 6714
	public Image cursorImage;

	// Token: 0x04001A3B RID: 6715
	public Transform printerParent;

	// Token: 0x04001A3C RID: 6716
	public LightController screenLight;

	// Token: 0x04001A3D RID: 6717
	[Header("Operating System")]
	public CruncherAppPreset currentApp;

	// Token: 0x04001A3E RID: 6718
	public bool useCursor;

	// Token: 0x04001A3F RID: 6719
	public bool playerControlled;

	// Token: 0x04001A40 RID: 6720
	public bool appLoaded;

	// Token: 0x04001A41 RID: 6721
	public float appLoadProgress;

	// Token: 0x04001A42 RID: 6722
	public float appTimerProgress;

	// Token: 0x04001A43 RID: 6723
	public float timedLoading;

	// Token: 0x04001A44 RID: 6724
	public float timedLoadingDemand = 0.33f;

	// Token: 0x04001A45 RID: 6725
	public List<GameObject> spawnedContent = new List<GameObject>();

	// Token: 0x04001A46 RID: 6726
	public Human loggedInAs;

	// Token: 0x04001A47 RID: 6727
	public float printTimer;

	// Token: 0x04001A48 RID: 6728
	public Vector3 printOutStartPos;

	// Token: 0x04001A49 RID: 6729
	public Vector3 printOutEndPos;

	// Token: 0x04001A4A RID: 6730
	private AudioController.LoopingSoundInfo loadLoop;

	// Token: 0x04001A4B RID: 6731
	[NonSerialized]
	public Interactable printedDocument;

	// Token: 0x04001A4C RID: 6732
	[Space(7f)]
	public ComputerOSUIComponent currentHover;
}
