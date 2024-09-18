using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

// Token: 0x02000033 RID: 51
public class CameraController : MonoBehaviour
{
	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060001CF RID: 463 RVA: 0x0000EAC4 File Offset: 0x0000CCC4
	public static CameraController Instance
	{
		get
		{
			return CameraController._instance;
		}
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x0000EACB File Offset: 0x0000CCCB
	private void Awake()
	{
		if (CameraController._instance != null && CameraController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			CameraController._instance = this;
		}
		this.cam = this.cameraObj.GetComponent<Camera>();
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x0000EB0C File Offset: 0x0000CD0C
	public void NewHighlightScroll(Vector2 newScrollPosPathmap)
	{
		this.highlightTileHeight = CityData.Instance.GetTileHeight(newScrollPosPathmap);
		Vector3 vector = CityData.Instance.TileToRealpos(new Vector3Int(Mathf.RoundToInt(newScrollPosPathmap.x), Mathf.RoundToInt(newScrollPosPathmap.y), 0));
		this.highlightScroll = new Vector3(vector.x, this.container.transform.position.y, vector.z);
		this.highlightScrollCancelFlag = false;
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000EB84 File Offset: 0x0000CD84
	public void CancelHighlightScroll()
	{
		if (this.highlightScrollActive)
		{
			this.highlightScrollCancelFlag = true;
		}
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0000EB98 File Offset: 0x0000CD98
	public void ImmediateCancelHighlightScroll()
	{
		if (this.highlightScrollActive)
		{
			if (this.highlightScrollMarker != null)
			{
				Object.Destroy(this.highlightScrollMarker);
			}
			this.highlightScroll = this.container.transform.position;
			this.highlightScrollCancelFlag = false;
			this.highlightScrollActive = false;
		}
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x0000EBEC File Offset: 0x0000CDEC
	public void SetupFPS()
	{
		CameraController.Instance.cam.enabled = true;
		CameraController.Instance.cam.fieldOfView = (float)Game.Instance.fov;
		InterfaceController.Instance.RemoveAllMouseInteractionComponents();
		this.cam.transform.SetParent(PrefabControls.Instance.camHeightParent);
		if (!Player.Instance.inAirVent && !Game.Instance.freeCam && (CityConstructor.Instance == null || CityConstructor.Instance.saveState == null))
		{
			Player.Instance.SetPlayerHeight(Player.Instance.GetPlayerHeightNormal(), true);
			Player.Instance.SetCameraHeight(GameplayControls.Instance.cameraHeightNormal);
		}
		Player.Instance.enabled = true;
		Player.Instance.fpsMode = true;
		Player.Instance.OnCityTileChange();
		if (!SessionData.Instance.play)
		{
			SessionData.Instance.ResumeGame();
		}
		Player.Instance.EnablePlayerMovement(true, true);
		Player.Instance.EnablePlayerMouseLook(true, false);
		this.cam.transform.localEulerAngles = Vector3.zero;
		Player.Instance.fps.InitialiseController(true, true);
		if (SessionData.Instance.play)
		{
			InterfaceController.Instance.SetDesktopMode(false, true);
		}
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x0000ED2D File Offset: 0x0000CF2D
	public void FadeCamera(float fadeSpeed)
	{
		if (this.fadeImage == null)
		{
			return;
		}
		base.StopCoroutine("CameraFade");
		base.StartCoroutine(this.CameraFade(true, fadeSpeed));
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0000ED58 File Offset: 0x0000CF58
	public void UnFadeCamera(float fadeSpeed)
	{
		if (this.fadeImage == null)
		{
			return;
		}
		base.StopCoroutine("CameraFade");
		base.StartCoroutine(this.CameraFade(false, fadeSpeed));
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x0000ED83 File Offset: 0x0000CF83
	private IEnumerator CameraFade(bool fade = true, float fadeSpeed = 1f)
	{
		if (fade)
		{
			this.fadeImage.gameObject.SetActive(true);
			this.fadeActive = true;
		}
		float snapProgress = 0f;
		if (fade)
		{
			snapProgress = this.fadeImage.canvasRenderer.GetAlpha();
		}
		else
		{
			snapProgress = 1f - this.fadeImage.canvasRenderer.GetAlpha();
		}
		while (snapProgress < 1f)
		{
			snapProgress = Mathf.Clamp01((float)Mathf.CeilToInt(snapProgress * 100f) / 100f);
			if (fade)
			{
				this.fadeImage.canvasRenderer.SetAlpha(snapProgress);
			}
			else
			{
				this.fadeImage.canvasRenderer.SetAlpha(1f - snapProgress);
			}
			yield return null;
		}
		if (!fade)
		{
			this.fadeImage.gameObject.SetActive(false);
			this.fadeActive = false;
		}
		yield break;
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x0000ED9C File Offset: 0x0000CF9C
	public float GetPlayerLightLevel()
	{
		float num = 1f;
		if (!Player.Instance.isOnStreet)
		{
			num = GameplayControls.Instance.interiorAmbientLightMultiplier;
		}
		float num2 = Mathf.Clamp01(GameplayControls.Instance.stealthAmbientLightLevel.Evaluate(SessionData.Instance.dayProgress) * num);
		float num3 = 0f;
		int num4 = 0;
		float num5 = 0f;
		int num6 = 0;
		float num7 = 0f;
		this.lightRaycastDataCollection.Clear();
		this.raycastCommands.Clear();
		foreach (NewRoom newRoom in GameplayController.Instance.roomsVicinity)
		{
			if (newRoom.mainLightStatus)
			{
				foreach (Interactable interactable in newRoom.mainLights)
				{
					if (interactable.lightController != null && interactable.lightController.isOn && !interactable.lightController.isUnscrewed && !interactable.lightController.closedBreaker)
					{
						for (int i = 0; i < 2; i++)
						{
							Vector3 position = interactable.lightController.transform.position;
							Vector3 vector = Vector3.zero;
							if (i == 0)
							{
								vector = this.cam.transform.position;
							}
							else
							{
								vector = Player.Instance.transform.position;
							}
							float range = interactable.lightController.lightComponent.range;
							CameraController.LightRaycastData lightRaycastData = new CameraController.LightRaycastData(range, interactable.lightController.intensity * 0.006f, 0, false);
							this.lightRaycastDataCollection.Add(lightRaycastData);
							RaycastCommand raycastCommand;
							raycastCommand..ctor(interactable.wPos, vector - interactable.wPos, range, Toolbox.Instance.aiSightingLayerMask, 1);
							this.raycastCommands.Add(raycastCommand);
						}
					}
				}
			}
			foreach (Interactable interactable2 in newRoom.secondaryLights)
			{
				if (interactable2.lightController != null && interactable2.lightController.isOn && !interactable2.lightController.isUnscrewed && !interactable2.lightController.closedBreaker)
				{
					for (int j = 0; j < 2; j++)
					{
						Vector3 position2 = interactable2.lightController.transform.position;
						Vector3 vector2 = Vector3.zero;
						if (j == 0)
						{
							vector2 = this.cam.transform.position;
						}
						else
						{
							vector2 = Player.Instance.transform.position;
						}
						float range2 = interactable2.lightController.lightComponent.range;
						CameraController.LightRaycastData lightRaycastData2 = new CameraController.LightRaycastData(range2, interactable2.lightController.intensity * 0.006f, 1, false);
						this.lightRaycastDataCollection.Add(lightRaycastData2);
						RaycastCommand raycastCommand2;
						raycastCommand2..ctor(interactable2.wPos, vector2 - interactable2.wPos, range2, Toolbox.Instance.aiSightingLayerMask, 1);
						this.raycastCommands.Add(raycastCommand2);
					}
				}
			}
		}
		for (int k = 0; k < 2; k++)
		{
			Vector3 position3 = CityControls.Instance.sunPosition.position;
			Vector3 vector3 = Vector3.zero;
			if (k == 0)
			{
				vector3 = this.cam.transform.position;
			}
			else
			{
				vector3 = Player.Instance.transform.position;
			}
			float num8 = 2000f;
			CameraController.LightRaycastData lightRaycastData3 = new CameraController.LightRaycastData(num8, 0f, 2, false);
			this.lightRaycastDataCollection.Add(lightRaycastData3);
			RaycastCommand raycastCommand3;
			raycastCommand3..ctor(position3, vector3 - position3, num8, Toolbox.Instance.aiSightingLayerMask, 1);
			this.raycastCommands.Add(raycastCommand3);
			CameraController.LightRaycastData lightRaycastData4 = new CameraController.LightRaycastData(num8, 0f, 2, true);
			this.lightRaycastDataCollection.Add(lightRaycastData4);
			RaycastCommand raycastCommand4;
			raycastCommand4..ctor(vector3, position3 - vector3, num8, Toolbox.Instance.aiSightingLayerMask, 1);
			this.raycastCommands.Add(raycastCommand4);
		}
		NativeArray<RaycastHit> nativeArray = new NativeArray<RaycastHit>(this.raycastCommands.Count, 3, 1);
		NativeArray<RaycastCommand> nativeArray2 = new NativeArray<RaycastCommand>(this.raycastCommands.ToArray(), 3);
		RaycastCommand.ScheduleBatch(nativeArray2, nativeArray, 2, default(JobHandle)).Complete();
		bool flag = false;
		for (int l = 0; l < nativeArray.Length; l++)
		{
			if (nativeArray[l].collider != null)
			{
				if (this.lightRaycastDataCollection[l].Phase == 0)
				{
					try
					{
						if (nativeArray[l].collider.transform.CompareTag("MainCamera") || nativeArray[l].collider.transform.CompareTag("Player") || (nativeArray[l].collider.transform.parent != null && nativeArray[l].collider.transform.parent.CompareTag("Player")))
						{
							float num9 = 1f - nativeArray[l].distance / this.lightRaycastDataCollection[l].MaxRange;
							num3 += Mathf.Clamp01(num9 * this.lightRaycastDataCollection[l].LightMultiplier);
							num4++;
						}
						goto IL_84A;
					}
					catch
					{
						goto IL_84A;
					}
				}
				if (this.lightRaycastDataCollection[l].Phase == 1)
				{
					if (nativeArray[l].collider.transform.CompareTag("MainCamera") || nativeArray[l].collider.transform.CompareTag("Player") || (nativeArray[l].collider.transform.parent != null && nativeArray[l].collider.transform.parent.CompareTag("Player")))
					{
						float num10 = 1f - nativeArray[l].distance / this.lightRaycastDataCollection[l].MaxRange;
						num5 += Mathf.Clamp01(num10 * this.lightRaycastDataCollection[l].LightMultiplier);
						num6++;
					}
				}
				else if (this.lightRaycastDataCollection[l].Phase == 2)
				{
					if (this.lightRaycastDataCollection[l].IsReverseCheck && flag)
					{
						flag = false;
						if (nativeArray[l].collider.transform.CompareTag("Transparent"))
						{
							num7 += GameplayControls.Instance.stealthSunLightLevel.Evaluate(SessionData.Instance.dayProgress);
						}
					}
					else if (nativeArray[l].collider.transform.CompareTag("MainCamera") || nativeArray[l].collider.transform.CompareTag("Player") || (nativeArray[l].collider.transform.parent != null && nativeArray[l].collider.transform.parent.CompareTag("Player")))
					{
						if (Player.Instance.currentRoom.gameLocation.thisAsAddress != null)
						{
							flag = true;
						}
						else
						{
							num7 += GameplayControls.Instance.stealthSunLightLevel.Evaluate(SessionData.Instance.dayProgress);
						}
					}
				}
			}
			else if (this.lightRaycastDataCollection[l].Phase == 2 && this.lightRaycastDataCollection[l].IsReverseCheck && flag)
			{
				num7 += GameplayControls.Instance.stealthSunLightLevel.Evaluate(SessionData.Instance.dayProgress);
			}
			IL_84A:;
		}
		num3 = Mathf.Clamp01(num3);
		num5 = Mathf.Clamp01(num5);
		num7 = Mathf.Clamp01(num7);
		float result = Mathf.Clamp01(num2 + num3 + num5 + num7);
		nativeArray.Dispose();
		nativeArray2.Dispose();
		return result;
	}

	// Token: 0x04000118 RID: 280
	[Header("References")]
	public GameObject cameraObj;

	// Token: 0x04000119 RID: 281
	public GameObject container;

	// Token: 0x0400011A RID: 282
	public Camera cam;

	// Token: 0x0400011B RID: 283
	public HDAdditionalCameraData hdrpCam;

	// Token: 0x0400011C RID: 284
	[Header("Fade")]
	public bool fadeActive;

	// Token: 0x0400011D RID: 285
	public Image fadeImage;

	// Token: 0x0400011E RID: 286
	[Header("Editor Movement Settings")]
	public Vector2 camHeightLimit = new Vector2(1f, 20f);

	// Token: 0x0400011F RID: 287
	public float heightRatio;

	// Token: 0x04000120 RID: 288
	public Vector3 defaultCameraEuler = new Vector3(90f, 0f, 0f);

	// Token: 0x04000121 RID: 289
	[Space(5f)]
	public float scrollSensitivity = 0.5f;

	// Token: 0x04000122 RID: 290
	public float camScrollHeightModifier = 0.1f;

	// Token: 0x04000123 RID: 291
	[Space(5f)]
	public float rotateSensitivity = 0.5f;

	// Token: 0x04000124 RID: 292
	[Space(5f)]
	public float zoomSensitivity = 2f;

	// Token: 0x04000125 RID: 293
	[Header("Smoothing Speeds")]
	public float smoothRotateSpeed = 25f;

	// Token: 0x04000126 RID: 294
	public float smoothZoomSpeed = 25f;

	// Token: 0x04000127 RID: 295
	public float highlightScrollSpeed = 25f;

	// Token: 0x04000128 RID: 296
	[Header("Camera Boundary")]
	public float isoCamBoundaryMultiplier = 1.5f;

	// Token: 0x04000129 RID: 297
	public float topCamBoundaryMultiplier = 1f;

	// Token: 0x0400012A RID: 298
	[Header("Highlight scroll")]
	public bool highlightScrollActive;

	// Token: 0x0400012B RID: 299
	public bool highlightScrollCancelFlag;

	// Token: 0x0400012C RID: 300
	public Vector3 originalCameraPosition = Vector3.zero;

	// Token: 0x0400012D RID: 301
	public Vector3 highlightScroll = Vector2.zero;

	// Token: 0x0400012E RID: 302
	public GameObject highlightScrollMarker;

	// Token: 0x0400012F RID: 303
	public float highlightTileHeight;

	// Token: 0x04000130 RID: 304
	private static CameraController _instance;

	// Token: 0x04000131 RID: 305
	private List<CameraController.LightRaycastData> lightRaycastDataCollection = new List<CameraController.LightRaycastData>();

	// Token: 0x04000132 RID: 306
	private List<RaycastCommand> raycastCommands = new List<RaycastCommand>();

	// Token: 0x02000034 RID: 52
	private struct LightRaycastData
	{
		// Token: 0x060001DA RID: 474 RVA: 0x0000F776 File Offset: 0x0000D976
		public LightRaycastData(float maxRange, float lightMultiplier, int phase, bool isReverseCheck)
		{
			this.MaxRange = maxRange;
			this.LightMultiplier = lightMultiplier;
			this.Phase = phase;
			this.IsReverseCheck = isReverseCheck;
		}

		// Token: 0x04000133 RID: 307
		public float MaxRange;

		// Token: 0x04000134 RID: 308
		public float LightMultiplier;

		// Token: 0x04000135 RID: 309
		public int Phase;

		// Token: 0x04000136 RID: 310
		public bool IsReverseCheck;
	}
}
