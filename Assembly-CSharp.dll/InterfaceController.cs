using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000506 RID: 1286
public class InterfaceController : MonoBehaviour
{
	// Token: 0x1400002E RID: 46
	// (add) Token: 0x06001BB7 RID: 7095 RVA: 0x0018F2F4 File Offset: 0x0018D4F4
	// (remove) Token: 0x06001BB8 RID: 7096 RVA: 0x0018F32C File Offset: 0x0018D52C
	public event InterfaceController.InputCode OnInputCode;

	// Token: 0x1400002F RID: 47
	// (add) Token: 0x06001BB9 RID: 7097 RVA: 0x0018F364 File Offset: 0x0018D564
	// (remove) Token: 0x06001BBA RID: 7098 RVA: 0x0018F39C File Offset: 0x0018D59C
	public event InterfaceController.NewActiveCodeInput OnNewActiveCodeInput;

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x06001BBB RID: 7099 RVA: 0x0018F3D1 File Offset: 0x0018D5D1
	public static InterfaceController Instance
	{
		get
		{
			return InterfaceController._instance;
		}
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x0018F3D8 File Offset: 0x0018D5D8
	private void Awake()
	{
		if (InterfaceController._instance != null && InterfaceController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		InterfaceController._instance = this;
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x0018F408 File Offset: 0x0018D608
	private void Start()
	{
		if (SessionData.Instance != null && !SessionData.Instance.isDialogEdit)
		{
			this.UpdateCursorSprite();
		}
		else if (SessionData.Instance.isDialogEdit)
		{
			Game.Log("Disabling interface controller as this is dialog editor...", 2);
			base.enabled = false;
		}
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		if (!SessionData.Instance.isFloorEdit)
		{
			this.popupController.Setup();
		}
		this.SetShowDesktopCaseBoard(true);
		if (this.controlPanelCanvas != null)
		{
			this.controlPanelCanvas.gameObject.SetActive(false);
		}
		if (this.timerText != null)
		{
			this.timerText.gameObject.SetActive(Game.Instance.timeLimited);
			this.timerText.text = string.Empty;
		}
		if (!SessionData.Instance.isFloorEdit)
		{
			this.compassActualAlpha = this.compassDesiredAlpha;
			this.compassMeshRend.sharedMaterial = Object.Instantiate<Material>(this.compassMeshRend.sharedMaterial);
			Color color = this.compassMeshRend.sharedMaterial.GetColor("_UnlitColor");
			this.compassMeshRend.sharedMaterial.SetColor("_UnlitColor", new Color(color.r, color.g, color.b, this.compassActualAlpha));
		}
		if (this.titleText != null)
		{
			this.titleText.text = string.Empty;
		}
	}

	// Token: 0x06001BBE RID: 7102 RVA: 0x0018F570 File Offset: 0x0018D770
	private void Update()
	{
		try
		{
			if (PopupMessageController.Instance != null && !PopupMessageController.Instance.active && !MainMenuController.Instance.mainMenuActive)
			{
				this.UpdateAnchoredSpeechPositions();
				bool flag = false;
				if (this.objectivesDisplayTimer > 0f)
				{
					if (!this.desktopMode)
					{
						this.objectivesDisplayTimer -= Time.deltaTime;
					}
					if (this.objectivesAlpha < 1f)
					{
						this.objectivesAlpha += Time.deltaTime / 0.1f;
						this.objectivesAlpha = Mathf.Clamp01(this.objectivesAlpha);
						flag = true;
					}
				}
				else if (this.objectivesDisplayTimer <= 0f && this.objectivesAlpha > 0f)
				{
					this.objectivesDisplayTimer = Mathf.Max(this.objectivesDisplayTimer, 0f);
					this.objectivesAlpha -= Time.deltaTime;
					this.objectivesAlpha = Mathf.Clamp01(this.objectivesAlpha);
					flag = true;
				}
				float num = 0f;
				if (CasePanelController.Instance.activeCase != null)
				{
					for (int i = 0; i < CasePanelController.Instance.activeCase.inactiveCurrentObjectives.Count; i++)
					{
						Objective objective = CasePanelController.Instance.activeCase.inactiveCurrentObjectives[i];
						if (!this.displayedObjectives.Contains(objective) && !objective.isCancelled && !objective.isComplete)
						{
							InterfaceController.Instance.ActivateObjectivesDisplay();
							objective.Activate(true);
							i--;
						}
					}
					for (int j = 0; j < CasePanelController.Instance.activeCase.currentActiveObjectives.Count; j++)
					{
						Objective objective2 = CasePanelController.Instance.activeCase.currentActiveObjectives[j];
						if (!this.displayedObjectives.Contains(objective2) && !objective2.isCancelled && !objective2.isComplete)
						{
							objective2.Activate(true);
						}
						if (!objective2.clearedForAnimation && !this.messageCoroutineRunning && this.currentNotification == null && !this.gameSceenDisplayed && !this.gameScreenQueued)
						{
							objective2.clearedForAnimation = true;
						}
						if (objective2.clearedForAnimation)
						{
							objective2.CheckingLoop();
						}
						if (objective2.objectiveList != null)
						{
							objective2.objectiveList.desiredAnchoredPosition = new Vector2(0f, num);
							num -= objective2.objectiveList.rect.sizeDelta.y + 4f;
							if (objective2.objectiveList.rect.anchoredPosition != objective2.objectiveList.desiredAnchoredPosition)
							{
								objective2.objectiveList.rect.anchoredPosition = Vector2.Lerp(objective2.objectiveList.rect.anchoredPosition, objective2.objectiveList.desiredAnchoredPosition, Time.deltaTime * 12f);
							}
						}
					}
				}
				for (int k = 0; k < this.displayedObjectives.Count; k++)
				{
					Objective objective3 = this.displayedObjectives[k];
					if (CasePanelController.Instance.activeCase != objective3.thisCase)
					{
						objective3.Remove();
						k--;
					}
				}
				for (int l = 0; l < this.objectiveList.Count; l++)
				{
					ChecklistButtonController checklistButtonController = this.objectiveList[l];
					if (flag)
					{
						if (checklistButtonController.bgRend != null)
						{
							checklistButtonController.bgRend.SetAlpha(this.objectivesAlpha);
						}
						if (checklistButtonController.textRend != null)
						{
							checklistButtonController.textRend.SetAlpha(this.objectivesAlpha);
						}
						if (checklistButtonController.iconRend != null)
						{
							checklistButtonController.iconRend.SetAlpha(this.objectivesAlpha);
						}
						if (checklistButtonController.barRend != null)
						{
							checklistButtonController.barRend.SetAlpha(this.objectivesAlpha);
						}
						if (checklistButtonController.progressBGrend != null)
						{
							checklistButtonController.progressBGrend.SetAlpha(this.objectivesAlpha);
						}
					}
					if (checklistButtonController.fadeOut)
					{
						this.ActivateObjectivesDisplay();
						if (checklistButtonController.strikeThroughProgress < (float)checklistButtonController.objective.name.Length)
						{
							checklistButtonController.strikeThroughProgress += Time.deltaTime * 5f;
							checklistButtonController.text.text = "<s>" + checklistButtonController.objective.name;
							checklistButtonController.text.text.Insert(Mathf.RoundToInt(checklistButtonController.strikeThroughProgress), "</s>");
						}
						if (checklistButtonController.fadeInProgress > 0f)
						{
							checklistButtonController.fadeInProgress -= Time.deltaTime * 0.75f;
							checklistButtonController.fadeInProgress = Mathf.Clamp01(checklistButtonController.fadeInProgress);
							if (checklistButtonController.bgRend != null)
							{
								checklistButtonController.bgRend.SetAlpha(checklistButtonController.fadeInProgress * this.objectivesAlpha);
							}
							if (checklistButtonController.textRend != null)
							{
								checklistButtonController.textRend.SetAlpha(checklistButtonController.fadeInProgress * this.objectivesAlpha);
							}
							if (checklistButtonController.iconRend != null)
							{
								checklistButtonController.iconRend.SetAlpha(checklistButtonController.fadeInProgress * this.objectivesAlpha);
							}
							if (checklistButtonController.barRend != null)
							{
								checklistButtonController.barRend.SetAlpha(checklistButtonController.fadeInProgress * this.objectivesAlpha);
							}
							if (checklistButtonController.progressBGrend != null)
							{
								checklistButtonController.progressBGrend.SetAlpha(checklistButtonController.fadeInProgress * this.objectivesAlpha);
							}
						}
						else
						{
							if (checklistButtonController != null && checklistButtonController.gameObject != null)
							{
								Object.Destroy(checklistButtonController.gameObject);
							}
							this.objectiveList.RemoveAt(l);
							l--;
						}
					}
					else if (checklistButtonController.objective != null && checklistButtonController.objective.thisCase != null && checklistButtonController.objective.thisCase.job != null && checklistButtonController.objective.thisCase.job.state == SideJob.JobState.ended && (checklistButtonController.objective.isCancelled || checklistButtonController.objective.isComplete))
					{
						checklistButtonController.Remove();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Interface controller error 1: " + ex.ToString());
		}
		try
		{
			if (!SessionData.Instance.isFloorEdit && SessionData.Instance.play)
			{
				this.backgroundTransform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
				this.compassDesiredAlpha = 0f;
				HashSet<object> hashSet = new HashSet<object>();
				for (int m = 0; m < this.awarenessIcons.Count; m++)
				{
					InterfaceController.AwarenessIcon awarenessIcon = this.awarenessIcons[m];
					Vector3 vector = Vector3.zero;
					Vector3 vector2 = awarenessIcon.targetPosition;
					object obj = null;
					if (awarenessIcon.awarenessType == InterfaceController.AwarenessType.actor)
					{
						vector2 = awarenessIcon.actor.transform.position;
						obj = awarenessIcon.actor;
					}
					else if (awarenessIcon.awarenessType == InterfaceController.AwarenessType.transform)
					{
						vector2 = awarenessIcon.targetTransform.position;
						obj = awarenessIcon.targetTransform;
					}
					if (!hashSet.Contains(obj))
					{
						hashSet.Add(obj);
						if (!awarenessIcon.spawned.activeSelf)
						{
							awarenessIcon.spawned.SetActive(true);
						}
						Vector3 vector3 = CameraController.Instance.cam.transform.position - vector2;
						vector3.y = 0f;
						awarenessIcon.spawned.transform.rotation = Quaternion.LookRotation(vector3, Vector3.up);
						if (awarenessIcon.triggerAlert)
						{
							if (awarenessIcon.alertProgress < 1f)
							{
								awarenessIcon.alertProgress += Time.deltaTime * 5f;
								awarenessIcon.alertProgress = Mathf.Clamp01(awarenessIcon.alertProgress);
								if (awarenessIcon.removalProgress > 0f)
								{
									awarenessIcon.removalProgress -= Time.deltaTime * 7f;
									awarenessIcon.removalProgress = Mathf.Clamp01(awarenessIcon.removalProgress);
								}
								float num2 = 2f + awarenessIcon.alertProgress;
								awarenessIcon.imageTransform.localScale = new Vector3(num2, num2, num2);
								awarenessIcon.arrowTransform.localScale = new Vector3(0.066f, 0.1f, 0.1f + awarenessIcon.alertProgress * 0.1f);
								awarenessIcon.imageMaterial.SetColor("_EmissiveColor", Color.Lerp(InterfaceControls.Instance.spottedNormalEmission, InterfaceControls.Instance.awarenessAlertEmission, awarenessIcon.alertProgress));
								awarenessIcon.arrowMaterial.SetColor("_EmissiveColor", Color.Lerp(InterfaceControls.Instance.arrowNormalEmission, InterfaceControls.Instance.awarenessAlertEmission, awarenessIcon.alertProgress));
							}
							else
							{
								awarenessIcon.triggerAlert = false;
							}
						}
						else if (awarenessIcon.alertProgress > 0f)
						{
							awarenessIcon.alertProgress -= Time.deltaTime * 0.5f;
							awarenessIcon.alertProgress = Mathf.Clamp01(awarenessIcon.alertProgress);
							float num3 = 2f + awarenessIcon.alertProgress;
							awarenessIcon.imageTransform.localScale = new Vector3(num3, num3, num3);
							awarenessIcon.arrowTransform.localScale = new Vector3(0.066f, 0.1f, 0.1f + awarenessIcon.alertProgress * 0.1f);
							awarenessIcon.imageMaterial.SetColor("_EmissiveColor", Color.Lerp(InterfaceControls.Instance.spottedNormalEmission, InterfaceControls.Instance.awarenessAlertEmission, awarenessIcon.alertProgress));
							awarenessIcon.arrowMaterial.SetColor("_EmissiveColor", Color.Lerp(InterfaceControls.Instance.arrowNormalEmission, InterfaceControls.Instance.awarenessAlertEmission, awarenessIcon.alertProgress));
						}
						vector = vector2 - Player.Instance.transform.position;
						vector.y = 0f;
						if (awarenessIcon.awarenessBehaviour == InterfaceController.AwarenessBehaviour.invisibleInfront)
						{
							float num4 = Vector3.SignedAngle(vector, Player.Instance.transform.forward, Vector3.up);
							awarenessIcon.displayAlpha = Mathf.Clamp01(Mathf.Abs(num4) / 75f - 1f + awarenessIcon.alertProgress * 2f);
						}
						else if (awarenessIcon.awarenessBehaviour == InterfaceController.AwarenessBehaviour.alwaysVisible)
						{
							awarenessIcon.displayAlpha = 1f;
						}
						if (Vector3.Distance(Player.Instance.transform.position, vector2) >= awarenessIcon.maxDistance)
						{
							awarenessIcon.displayAlpha = 0f;
						}
						float num5 = -3f;
						if (awarenessIcon.fadeIn < 1f)
						{
							awarenessIcon.fadeIn += Time.deltaTime * 5f;
							awarenessIcon.fadeIn = Mathf.Max(awarenessIcon.fadeIn, awarenessIcon.alertProgress * 2f);
							awarenessIcon.fadeIn = Mathf.Clamp01(awarenessIcon.fadeIn);
							awarenessIcon.SetAlpha(Mathf.Clamp01(awarenessIcon.fadeIn * 1.5f));
							num5 = Mathf.SmoothStep(-3f, -5f, awarenessIcon.fadeIn);
							awarenessIcon.arrowTransform.localPosition = new Vector3(0f, 0f, num5);
						}
						else if (awarenessIcon.springAction < 1f)
						{
							awarenessIcon.springAction += Time.deltaTime * 0.5f;
							num5 = Mathf.SmoothStep(-6f, -5f, awarenessIcon.springAction);
							awarenessIcon.arrowTransform.localPosition = new Vector3(0f, 0f, num5);
						}
						if (awarenessIcon.triggerAlert)
						{
							awarenessIcon.arrowTransform.localPosition = new Vector3(0f, 0f, Mathf.SmoothStep(num5, -5f, awarenessIcon.alertProgress));
						}
						else if (awarenessIcon.alertProgress > 0f)
						{
							awarenessIcon.arrowTransform.localPosition = new Vector3(0f, 0f, Mathf.SmoothStep(num5, -5f, awarenessIcon.alertProgress));
						}
						if (awarenessIcon.removalFlag && !awarenessIcon.triggerAlert)
						{
							if (awarenessIcon.removalProgress < 1f)
							{
								awarenessIcon.removalProgress += Time.deltaTime * 2f;
								awarenessIcon.removalProgress = Mathf.Clamp01(awarenessIcon.removalProgress);
							}
							if (awarenessIcon.removalProgress >= 1f)
							{
								Object.Destroy(awarenessIcon.spawned);
								this.awarenessIcons.RemoveAt(m);
								InterfaceController.Instance.awarenessIcons.Sort((InterfaceController.AwarenessIcon p1, InterfaceController.AwarenessIcon p2) => p2.priority.CompareTo(p1.priority));
								m--;
								goto IL_CFC;
							}
						}
						awarenessIcon.SetAlpha(awarenessIcon.alpha);
						awarenessIcon.imageTransform.rotation = CameraController.Instance.cam.transform.rotation;
						this.compassDesiredAlpha = Mathf.Max(this.compassDesiredAlpha, awarenessIcon.GetActualAlpha());
					}
					else
					{
						awarenessIcon.spawned.SetActive(false);
					}
					IL_CFC:;
				}
				this.compassDesiredAlpha = Mathf.Clamp01(this.compassDesiredAlpha);
				if (this.compassDesiredAlpha > this.compassActualAlpha)
				{
					this.compassActualAlpha = this.compassDesiredAlpha;
					Color color = this.compassMeshRend.sharedMaterial.GetColor("_UnlitColor");
					this.compassMeshRend.sharedMaterial.SetColor("_UnlitColor", new Color(color.r, color.g, color.b, this.compassActualAlpha));
				}
				else if (this.compassDesiredAlpha < this.compassActualAlpha)
				{
					this.compassActualAlpha -= Time.deltaTime;
					if (this.compassDesiredAlpha > this.compassActualAlpha)
					{
						this.compassActualAlpha = this.compassDesiredAlpha;
					}
					Color color2 = this.compassMeshRend.sharedMaterial.GetColor("_UnlitColor");
					this.compassMeshRend.sharedMaterial.SetColor("_UnlitColor", new Color(color2.r, color2.g, color2.b, this.compassActualAlpha));
				}
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError("Interface controller error 2: " + ex2.ToString());
		}
		try
		{
			if (this.desktopMode)
			{
				Vector2 zero = Vector2.zero;
				if (InputController.Instance.mouseInputMode)
				{
					RectTransformUtility.ScreenPointToLocalPointInRectangle(InterfaceControls.Instance.caseBoardContentContainer, Input.mousePosition, null, ref zero);
					InterfaceControls.Instance.caseBoardCursorRBContainer.localPosition = zero;
				}
				else if (this.selectedElement != null)
				{
					InterfaceControls.Instance.caseBoardCursorRBContainer.position = this.selectedElement.transform.position;
				}
				else
				{
					InterfaceControls.Instance.caseBoardCursorRBContainer.position = CasePanelController.Instance.caseboardScroll.transform.position;
				}
				if (Input.GetMouseButtonDown(0) && this.pinnedBeingDragged == null && ContextMenuController.activeMenu == null && InputController.Instance.mouseInputMode && this.currentMouseOverElement.Count <= 0 && !this.boxSelectActive)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.boxSelect, InterfaceControls.Instance.caseBoardContentContainer);
					this.boxSelect = gameObject.GetComponent<RectTransform>();
					this.boxSelect.localPosition = zero;
					this.boxSelectActive = true;
					Game.Log("Interface: New box select active...", 2);
				}
				if (Input.GetMouseButton(0) && this.boxSelectActive && InputController.Instance.mouseInputMode)
				{
					if (zero.x - this.boxSelect.localPosition.x < 0f)
					{
						this.boxSelect.pivot = new Vector2(1f, this.boxSelect.pivot.y);
					}
					else
					{
						this.boxSelect.pivot = new Vector2(0f, this.boxSelect.pivot.y);
					}
					if (zero.y - this.boxSelect.localPosition.y < 0f)
					{
						this.boxSelect.pivot = new Vector2(this.boxSelect.pivot.x, 1f);
					}
					else
					{
						this.boxSelect.pivot = new Vector2(this.boxSelect.pivot.x, 0f);
					}
					this.boxSelect.sizeDelta = new Vector2(Mathf.Abs(zero.x - this.boxSelect.localPosition.x), Mathf.Abs(zero.y - this.boxSelect.localPosition.y));
					List<PinnedItemController> list = new List<PinnedItemController>();
					Rect worldRect = Toolbox.Instance.GetWorldRect(this.boxSelect, InterfaceControls.Instance.caseBoardContentContainer.localScale);
					foreach (PinnedItemController pinnedItemController in CasePanelController.Instance.spawnedPins)
					{
						if (pinnedItemController.pinButtonController != null && worldRect.Contains(pinnedItemController.pinButtonController.rect.position, true) && !list.Contains(pinnedItemController))
						{
							list.Add(pinnedItemController);
						}
					}
					for (int n = 0; n < this.selectedPinned.Count; n++)
					{
						PinnedItemController pinnedItemController2 = this.selectedPinned[n];
						if (pinnedItemController2 == null)
						{
							this.selectedPinned.RemoveAt(n);
							InteractionController.Instance.UpdateInteractionText();
							n--;
						}
						else if (!list.Contains(pinnedItemController2))
						{
							Game.Log("Interface: Deselect " + pinnedItemController2.name, 2);
							pinnedItemController2.SetSelected(false, false);
							n--;
						}
					}
					using (List<PinnedItemController>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							PinnedItemController pinnedItemController3 = enumerator.Current;
							pinnedItemController3.SetSelected(true, true);
						}
						goto IL_123E;
					}
				}
				if (this.boxSelectActive && this.boxSelect != null)
				{
					Game.Log("Interface: Box select inactive", 2);
					Object.Destroy(this.boxSelect.gameObject);
					this.boxSelectActive = false;
				}
			}
			IL_123E:;
		}
		catch (Exception ex3)
		{
			Debug.LogError("Interface controller error 3: " + ex3.ToString());
		}
		try
		{
			if (this.pinnedBeingDragged != null && !this.windowFullFade)
			{
				this.windowFullFade = true;
			}
		}
		catch (Exception ex4)
		{
			Debug.LogError("Interface controller error 4: " + ex4.ToString());
		}
		try
		{
			if (this.windowFullFade)
			{
				if (this.windowFadeProgress < 0.95f)
				{
					this.windowFadeProgress += 3.75f * Time.deltaTime;
					this.windowFadeProgress = Mathf.Clamp01(this.windowFadeProgress);
					InterfaceController.Instance.windowCanvasGroup.alpha = 1f - this.windowFadeProgress;
				}
				else
				{
					this.windowFullFade = false;
				}
			}
			else if (this.windowFadeProgress > 0f)
			{
				this.windowFadeProgress -= 4.75f * Time.deltaTime;
				this.windowFadeProgress = Mathf.Clamp01(this.windowFadeProgress);
				InterfaceController.Instance.windowCanvasGroup.alpha = 1f - this.windowFadeProgress;
			}
		}
		catch (Exception ex5)
		{
			Debug.LogError("Interface controller error 5: " + ex5.ToString());
		}
		try
		{
			if (this.dofProgress < 1f && Game.Instance.depthBlur)
			{
				this.dofProgress += Time.deltaTime / GameplayControls.Instance.dofChangeTime;
				SessionData.Instance.dof.nearFocusStart.value = Mathf.SmoothStep(SessionData.Instance.dof.focusDistance.value, this.desiredDofNearStart, this.dofProgress);
				SessionData.Instance.dof.nearFocusEnd.value = Mathf.SmoothStep(SessionData.Instance.dof.focusDistance.value, this.desiredDofNearEnd, this.dofProgress);
				SessionData.Instance.dof.farFocusStart.value = Mathf.SmoothStep(SessionData.Instance.dof.focusDistance.value, this.desiredDofFarStart, this.dofProgress);
				SessionData.Instance.dof.farFocusEnd.value = Mathf.SmoothStep(SessionData.Instance.dof.focusDistance.value, this.desiredDofFarEnd, this.dofProgress);
				if (this.dofProgress >= 1f)
				{
					if (this.desiredDofNearStart == GameplayControls.Instance.dofNormalNearStart && this.desiredDofNearEnd == GameplayControls.Instance.dofNormalNearEnd && this.desiredDofFarStart == GameplayControls.Instance.dofNormalFarStart && this.desiredDofFarEnd == GameplayControls.Instance.dofNormalFarEnd)
					{
						SessionData.Instance.dof.active = false;
					}
				}
				else
				{
					SessionData.Instance.dof.active = true;
				}
			}
		}
		catch (Exception ex6)
		{
			Debug.LogError("Interface controller error 6: " + ex6.ToString());
		}
	}

	// Token: 0x06001BBF RID: 7103 RVA: 0x00190B5C File Offset: 0x0018ED5C
	public void UpdateDOF()
	{
		if (!Game.Instance.depthBlur)
		{
			SessionData.Instance.dof.active = false;
			return;
		}
		float num = GameplayControls.Instance.dofNormalNearStart;
		float num2 = GameplayControls.Instance.dofNormalNearEnd;
		float num3 = GameplayControls.Instance.dofNormalFarStart;
		float num4 = GameplayControls.Instance.dofNormalFarEnd;
		if (!SessionData.Instance.play && !SessionData.Instance.isDecorEdit && !SessionData.Instance.isCityEdit && PlayerApartmentController.Instance != null && !PlayerApartmentController.Instance.decoratingMode && !PlayerApartmentController.Instance.furniturePlacementMode)
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("Update dof: pause mode", 2);
			}
			num = GameplayControls.Instance.dofPausedNearStart;
			num2 = GameplayControls.Instance.dofPausedNearEnd;
			num3 = GameplayControls.Instance.dofPausedFarStart;
			num4 = GameplayControls.Instance.dofPausedFarEnd;
		}
		else if (this.desiredFade > 0f)
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("Update dof: pause fade", 2);
			}
			num = Mathf.Lerp(GameplayControls.Instance.dofNormalNearStart, GameplayControls.Instance.dofPausedNearStart, this.desiredFade);
			num2 = Mathf.Lerp(GameplayControls.Instance.dofNormalNearEnd, GameplayControls.Instance.dofPausedNearEnd, this.desiredFade);
			num3 = Mathf.Lerp(GameplayControls.Instance.dofNormalFarStart, GameplayControls.Instance.dofPausedFarStart, this.desiredFade);
			num4 = Mathf.Lerp(GameplayControls.Instance.dofNormalFarEnd, GameplayControls.Instance.dofPausedFarEnd, this.desiredFade);
		}
		else if (SessionData.Instance.isCityEdit && HighlanderSingleton<CityEditorController>.Instance != null)
		{
			if (HighlanderSingleton<CityEditorController>.Instance.isLoading)
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("Update dof: city ed loading", 2);
				}
				num = GameplayControls.Instance.dofTalkingNearStart;
				num2 = GameplayControls.Instance.dofTalkingNearEnd;
				num3 = GameplayControls.Instance.dofTalkingFarStart;
				num4 = GameplayControls.Instance.dofTalkingFarEnd;
			}
			else
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("Update dof: city editor editing", 2);
				}
				num = GameplayControls.Instance.dofCityEditNearStart;
				num2 = GameplayControls.Instance.dofCityEditNearEnd;
				num3 = GameplayControls.Instance.dofCityEditFarStart;
				num4 = GameplayControls.Instance.dofCityEditFarEnd;
			}
		}
		else if (InteractionController.Instance != null && InteractionController.Instance.dialogMode)
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("Update dof: talking", 2);
			}
			num = GameplayControls.Instance.dofTalkingNearStart;
			num2 = GameplayControls.Instance.dofTalkingNearEnd;
			num3 = GameplayControls.Instance.dofTalkingFarStart;
			num4 = GameplayControls.Instance.dofTalkingFarEnd;
		}
		if (num != this.desiredDofNearEnd || num2 != this.desiredDofNearEnd || num3 != this.desiredDofFarStart || num4 != this.desiredDofFarEnd)
		{
			this.dofProgress = 0f;
			SessionData.Instance.dof.active = true;
			this.desiredDofNearStart = num;
			this.desiredDofNearEnd = num2;
			this.desiredDofFarStart = num3;
			this.desiredDofFarEnd = num4;
		}
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x00190E64 File Offset: 0x0018F064
	public void DeselectAllPins()
	{
		Game.Log("Interface: Deselect all pinned", 2);
		new List<PinnedItemController>(this.selectedPinned);
		for (int i = 0; i < this.selectedPinned.Count; i++)
		{
			PinnedItemController pinnedItemController = this.selectedPinned[i];
			if (pinnedItemController != null)
			{
				pinnedItemController.SetSelected(false, false);
			}
		}
		this.selectedPinned.Clear();
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x00190ED4 File Offset: 0x0018F0D4
	public void UpdateAnchoredSpeechPositions()
	{
		if (this.anchoredSpeech.Count > 0)
		{
			this.anchoredSpeech.Sort((SpeechBubbleController p1, SpeechBubbleController p2) => p1.timeStamp.CompareTo(p2.timeStamp));
			float num = 0f;
			foreach (SpeechBubbleController speechBubbleController in this.anchoredSpeech.FindAll((SpeechBubbleController item) => item.isPlayer))
			{
				this.anchoredSpeech.Remove(speechBubbleController);
				this.anchoredSpeech.Insert(0, speechBubbleController);
			}
			for (int i = 0; i < this.anchoredSpeech.Count; i++)
			{
				SpeechBubbleController speechBubbleController2 = this.anchoredSpeech[i];
				num -= speechBubbleController2.bubbleRect.sizeDelta.y * 0.5f;
				speechBubbleController2.desiredPosition = this.speechDisplayAnchor.TransformPoint(new Vector3(0f, num, 0f));
				num -= speechBubbleController2.bubbleRect.sizeDelta.y * 0.5f + 4f;
				if (i > 1 || speechBubbleController2.isPlayer)
				{
					num -= 300f;
				}
			}
		}
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x0019103C File Offset: 0x0018F23C
	public InterfaceController.AwarenessIcon AddAwarenessIcon(InterfaceController.AwarenessType newType, InterfaceController.AwarenessBehaviour newBehaviour, Actor newActor, Transform newTransform, Vector3 newPosition, Material newMat, int newPriority, bool forceMaxDistance = false, float maxDist = 20f)
	{
		InterfaceController.AwarenessIcon awarenessIcon = this.awarenessIcons.Find((InterfaceController.AwarenessIcon item) => item.awarenessType == newType && item.awarenessBehaviour == newBehaviour && item.actor == newActor && item.targetTransform == newTransform && item.targetPosition == newPosition);
		if (awarenessIcon != null)
		{
			string text = "Interface: Found existing awareness icon for ";
			Actor newActor2 = newActor;
			Game.Log(text + ((newActor2 != null) ? newActor2.ToString() : null), 2);
			if (awarenessIcon.removalFlag)
			{
				awarenessIcon.removalFlag = false;
				awarenessIcon.removalProgress = 0f;
			}
			return awarenessIcon;
		}
		InterfaceController.AwarenessIcon awarenessIcon2 = new InterfaceController.AwarenessIcon();
		awarenessIcon2.awarenessType = newType;
		awarenessIcon2.awarenessBehaviour = newBehaviour;
		awarenessIcon2.actor = newActor;
		awarenessIcon2.targetTransform = newTransform;
		awarenessIcon2.targetPosition = newPosition;
		awarenessIcon2.priority = newPriority;
		awarenessIcon2.maxDistance = InterfaceControls.Instance.awarenessDistanceThreshold;
		if (forceMaxDistance)
		{
			awarenessIcon2.maxDistance = maxDist;
		}
		awarenessIcon2.spawned = Object.Instantiate<GameObject>(PrefabControls.Instance.awarenessIndicator, this.backgroundTransform);
		awarenessIcon2.imageMaterial = Object.Instantiate<Material>(newMat);
		awarenessIcon2.arrowMaterial = Object.Instantiate<Material>(InterfaceControls.Instance.arrow);
		awarenessIcon2.setup = true;
		if (awarenessIcon2.overrideTexture != null)
		{
			awarenessIcon2.imageMaterial.SetTexture("_UnlitColorMap", awarenessIcon2.overrideTexture);
		}
		foreach (object obj in awarenessIcon2.spawned.transform)
		{
			foreach (object obj2 in ((Transform)obj).transform)
			{
				Transform transform = (Transform)obj2;
				if (transform.tag == "AwarenessIndicator")
				{
					awarenessIcon2.imageTransform = transform;
					awarenessIcon2.imageTransform.gameObject.GetComponent<MeshRenderer>().material = awarenessIcon2.imageMaterial;
				}
				else
				{
					awarenessIcon2.arrowTransform = transform;
					awarenessIcon2.arrowTransform.gameObject.GetComponent<MeshRenderer>().material = awarenessIcon2.arrowMaterial;
				}
			}
		}
		if (awarenessIcon2.actor != null)
		{
			awarenessIcon2.targetPosition = awarenessIcon2.actor.transform.position;
		}
		else if (awarenessIcon2.targetTransform != null)
		{
			awarenessIcon2.targetPosition = awarenessIcon2.targetTransform.position;
		}
		Vector3 vector = awarenessIcon2.targetPosition - Player.Instance.transform.position;
		vector.y = 0f;
		if (awarenessIcon2.awarenessBehaviour == InterfaceController.AwarenessBehaviour.invisibleInfront)
		{
			float num = Vector3.SignedAngle(vector, Player.Instance.transform.forward, Vector3.up);
			awarenessIcon2.displayAlpha = Mathf.Clamp01(Mathf.Abs(num) / 75f - 1f);
		}
		else if (awarenessIcon2.awarenessBehaviour == InterfaceController.AwarenessBehaviour.alwaysVisible)
		{
			awarenessIcon2.displayAlpha = 1f;
		}
		this.awarenessIcons.Add(awarenessIcon2);
		InterfaceController.Instance.awarenessIcons.Sort((InterfaceController.AwarenessIcon p1, InterfaceController.AwarenessIcon p2) => p2.priority.CompareTo(p1.priority));
		return awarenessIcon2;
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x00191394 File Offset: 0x0018F594
	public UIPointerController AddUIPointer(Objective newObjective)
	{
		UIPointerController component = Object.Instantiate<GameObject>(PrefabControls.Instance.uiPointer, this.uiPointerContainer).GetComponent<UIPointerController>();
		component.Setup(newObjective);
		return component;
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x001913B8 File Offset: 0x0018F5B8
	public InfoWindow SpawnWindow(Evidence passedEvidence, Evidence.DataKey passedEvidenceKey = Evidence.DataKey.name, List<Evidence.DataKey> passedEvidenceKeys = null, string presetName = "", bool worldInteraction = false, bool autoPosition = true, Vector2 forcePosition = default(Vector2), Interactable passedInteractable = null, Case passedCase = null, Case.CaseElement forcedPinnedElement = null, bool passDialogSuccess = true)
	{
		WindowStylePreset windowStylePreset = null;
		if (presetName.Length <= 0 && passedEvidence != null)
		{
			if (passedEvidenceKeys == null)
			{
				passedEvidenceKeys = new List<Evidence.DataKey>();
				passedEvidenceKeys.Add(passedEvidenceKey);
			}
			passedEvidenceKeys = passedEvidence.GetTiedKeys(passedEvidenceKeys);
			if (!passedEvidence.isFound && (worldInteraction || passedEvidence.preset.markAsDiscoveredOnAnyInteraction))
			{
				passedEvidence.SetFound(true);
			}
			List<InfoWindow> list = this.activeWindows.FindAll((InfoWindow item) => item.passedEvidence == passedEvidence);
			InfoWindow infoWindow = null;
			if (ContextMenuController.activeMenu != null)
			{
				ContextMenuController.activeMenu.ForceClose();
			}
			Game.Log(string.Concat(new string[]
			{
				"Interface: Checking against ",
				this.activeWindows.Count.ToString(),
				" existing windows, with ",
				passedEvidenceKeys.Count.ToString(),
				" passed keys..."
			}), 2);
			foreach (InfoWindow infoWindow2 in list)
			{
				foreach (Evidence.DataKey dataKey in passedEvidenceKeys)
				{
					if (passedEvidence.preset.IsKeyUnique(dataKey) && infoWindow2.evidenceKeys.Contains(dataKey))
					{
						Game.Log(string.Concat(new string[]
						{
							"Interface: Exists already: ",
							infoWindow2.name,
							" (",
							dataKey.ToString(),
							")"
						}), 2);
						infoWindow2.transform.SetAsLastSibling();
						if (worldInteraction && !infoWindow2.isWorldInteraction)
						{
							infoWindow2.SetWorldInteraction(true);
						}
						infoWindow = infoWindow2;
						break;
					}
				}
				if (infoWindow != null)
				{
					break;
				}
			}
			if (infoWindow != null)
			{
				return infoWindow;
			}
			windowStylePreset = passedEvidence.preset.windowStyle;
		}
		else
		{
			presetName = presetName.ToLower();
			try
			{
				windowStylePreset = this.windowDictionary[presetName];
			}
			catch
			{
				string text = "Window: Could not load window preset, there is no type: ";
				WindowStylePreset windowStylePreset2 = windowStylePreset;
				Game.Log(text + ((windowStylePreset2 != null) ? windowStylePreset2.ToString() : null) + " Dict count: " + this.windowDictionary.Count.ToString(), 2);
				return null;
			}
		}
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.infoWindow, InterfaceController.Instance.windowCanvas.transform);
		InfoWindow component = gameObject.GetComponent<InfoWindow>();
		component.SetPivot(new Vector2(0f, 1f));
		this.activeWindows.Add(component);
		component.Setup(windowStylePreset, passedEvidence, passedEvidenceKeys, worldInteraction, passedInteractable, passedCase, forcedPinnedElement, passDialogSuccess);
		gameObject.GetComponent<RectTransform>();
		Vector2 vector = Vector2.zero;
		Vector2 zero = Vector2.zero;
		if (autoPosition)
		{
			vector..ctor(InterfaceControls.Instance.hudCanvasRect.rect.width * InterfaceControls.Instance.defaultWindowLocation.x, -InterfaceControls.Instance.hudCanvasRect.rect.height * InterfaceControls.Instance.defaultWindowLocation.y);
			zero..ctor(InterfaceControls.Instance.hudCanvasRect.rect.width * (InterfaceControls.Instance.windowCountOffset.x * (float)this.activeWindows.Count), -InterfaceControls.Instance.hudCanvasRect.rect.height * (InterfaceControls.Instance.windowCountOffset.y * (float)this.activeWindows.Count));
		}
		else
		{
			vector = new Vector2(InterfaceControls.Instance.hudCanvasRect.rect.width * forcePosition.x, -InterfaceControls.Instance.hudCanvasRect.rect.height * forcePosition.y) - new Vector2(windowStylePreset.defaultSize.x * 0.5f, windowStylePreset.defaultSize.y * -0.5f);
			string[] array = new string[6];
			array[0] = "Interface: Forcing position for ";
			array[1] = component.name;
			array[2] = ": ";
			int num = 3;
			Vector2 vector2 = forcePosition;
			array[num] = vector2.ToString();
			array[4] = " = ";
			int num2 = 5;
			vector2 = vector;
			array[num2] = vector2.ToString();
			Game.Log(string.Concat(array), 2);
		}
		component.SetAnchoredPosition(vector + zero);
		if (passedEvidence != null && !passedEvidence.preset.disableHistory)
		{
			GameplayController.Instance.AddHistory(passedEvidence, passedEvidenceKeys);
		}
		gameObject.transform.SetAsLastSibling();
		if (component.pinned)
		{
			InterfaceController.Instance.RestoreWindow(component);
		}
		if (UpgradesController.Instance.isOpen)
		{
			UpgradesController.Instance.CloseUpgrades(true);
		}
		if (CasePanelController.Instance.controllerMode)
		{
			if (CasePanelController.Instance.currentSelectMode != CasePanelController.ControllerSelectMode.windows)
			{
				CasePanelController.Instance.SetControllerMode(true, CasePanelController.ControllerSelectMode.windows);
			}
			CasePanelController.Instance.SetSelectedWindow(component, false);
		}
		if (PinnedItemController.activeQuickMenu != null)
		{
			PinnedItemController.activeQuickMenu.Remove(false);
		}
		return component;
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x00191934 File Offset: 0x0018FB34
	public void SetDragged(GameObject drag, string tag, Vector2 dCursorOffset)
	{
		this.dragged = drag;
		this.draggedTag = tag;
		this.dragCursorOffset = dCursorOffset;
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x0019194C File Offset: 0x0018FB4C
	public InfoWindow GetWindow(Evidence winEntry)
	{
		if (winEntry == null)
		{
			return null;
		}
		int num = this.activeWindows.FindIndex((InfoWindow item) => item.passedEvidence == winEntry);
		if (num > -1)
		{
			return this.activeWindows[num];
		}
		return null;
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x0019199C File Offset: 0x0018FB9C
	public InfoWindow GetWindow(Evidence winEntry, List<Evidence.DataKey> evKeys)
	{
		foreach (InfoWindow infoWindow in this.activeWindows.FindAll((InfoWindow item) => item.passedEvidence == winEntry))
		{
			foreach (Evidence.DataKey dataKey in evKeys)
			{
				if (infoWindow.evidenceKeys.Contains(dataKey))
				{
					return infoWindow;
				}
			}
		}
		return null;
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x00191A58 File Offset: 0x0018FC58
	public void MinimizeWindow(InfoWindow window)
	{
		if (window == null || window.currentPinnedCaseElement == null || window.currentPinnedCaseElement.pinnedController == null)
		{
			Game.Log("Interface: Destroying window as there is no case element to minimize to...", 2);
			Object.Destroy(window.gameObject);
			return;
		}
		Vector2 toScale;
		toScale..ctor(window.currentPinnedCaseElement.pinnedController.rect.sizeDelta.x / window.rect.sizeDelta.x, window.currentPinnedCaseElement.pinnedController.rect.sizeDelta.y / window.rect.sizeDelta.y);
		base.StartCoroutine(this.WindowScaleAnimation(window, window.currentPinnedCaseElement.pinnedController.rect.position, window.currentPinnedCaseElement.pinnedController.rect.pivot, toScale, true));
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x00191B3C File Offset: 0x0018FD3C
	public void RestoreWindow(InfoWindow window)
	{
		if (window == null || window.currentPinnedCaseElement == null || window.currentPinnedCaseElement.pinnedController == null)
		{
			Game.Log("Interface: Destroying window as there is no case element to restore to...", 2);
			Object.Destroy(window.gameObject);
			return;
		}
		RectTransform component = window.windowCanvas.GetComponent<RectTransform>();
		component.pivot = window.currentPinnedCaseElement.pinnedController.rect.pivot;
		component.localScale = new Vector2(window.currentPinnedCaseElement.pinnedController.rect.sizeDelta.x / window.rect.sizeDelta.x, window.currentPinnedCaseElement.pinnedController.rect.sizeDelta.y / window.rect.sizeDelta.y);
		component.position = window.currentPinnedCaseElement.pinnedController.rect.position;
		base.StartCoroutine(this.WindowScaleAnimation(window, window.currentPinnedCaseElement.resPos, window.currentPinnedCaseElement.resPiv, Vector2.one, false));
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x00191C59 File Offset: 0x0018FE59
	private IEnumerator WindowScaleAnimation(InfoWindow window, Vector2 toPosition, Vector2 toPivot, Vector2 toScale, bool removeAtEnd)
	{
		window.gameObject.SetActive(true);
		RectTransform itemCanvas = window.windowCanvas.GetComponent<RectTransform>();
		GraphicRaycaster gr = window.GetComponent<GraphicRaycaster>();
		if (gr != null)
		{
			gr.enabled = false;
		}
		Vector2 movementDirection = new Vector2(toPosition.x - itemCanvas.position.x, toPosition.y - itemCanvas.position.y);
		Vector2 startScale = itemCanvas.localScale;
		string[] array = new string[10];
		array[0] = "Interface: Window scale animation start: ";
		array[1] = ((window != null) ? window.ToString() : null);
		array[2] = ", ";
		int num = 3;
		Vector2 vector = toPosition;
		array[num] = vector.ToString();
		array[4] = ", ";
		int num2 = 5;
		vector = toPivot;
		array[num2] = vector.ToString();
		array[6] = ", ";
		int num3 = 7;
		vector = toScale;
		array[num3] = vector.ToString();
		array[8] = ", ";
		array[9] = removeAtEnd.ToString();
		Game.Log(string.Concat(array), 2);
		float progress = 0f;
		while (progress < 1f)
		{
			progress += InterfaceControls.Instance.minimizingAnimationSpeed * Time.deltaTime;
			progress = Mathf.Clamp01(progress);
			itemCanvas.pivot = Vector2.Lerp(itemCanvas.pivot, toPivot, progress);
			itemCanvas.localScale = Vector2.Lerp(startScale, toScale, progress);
			itemCanvas.position = Vector3.Lerp(itemCanvas.position, toPosition, progress);
			float num4 = Mathf.Clamp(itemCanvas.localPosition.x, InterfaceControls.Instance.hudCanvasRect.rect.width * -0.5f, InterfaceControls.Instance.hudCanvasRect.rect.width * 0.5f);
			float num5 = Mathf.Clamp(itemCanvas.localPosition.y, InterfaceControls.Instance.hudCanvasRect.rect.height * -0.5f, InterfaceControls.Instance.hudCanvasRect.rect.height * 0.5f);
			itemCanvas.localPosition = new Vector3(num4, num5, 0f);
			yield return null;
		}
		if (removeAtEnd)
		{
			if (window != null && window.currentPinnedCaseElement != null && window.currentPinnedCaseElement.pinnedController != null)
			{
				window.currentPinnedCaseElement.pinnedController.juice.Nudge(new Vector2(1.2f, 1.2f), Vector2.zero, true, true, false);
				if (window.currentPinnedCaseElement.pinnedController.rb != null)
				{
					window.currentPinnedCaseElement.pinnedController.rb.AddForce(movementDirection * 13f);
				}
			}
			Game.Log("Interface: destroy window " + window.name + " (upon minimize animation)", 2);
			Object.Destroy(window.gameObject);
		}
		else if (gr != null)
		{
			gr.enabled = true;
		}
		yield break;
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x00191C86 File Offset: 0x0018FE86
	public void RemoveAllMouseInteractionComponents()
	{
		if (TooltipController.activeTooltip != null)
		{
			TooltipController.activeTooltip.ForceClose();
		}
		if (ContextMenuController.activeMenu != null)
		{
			ContextMenuController.activeMenu.ForceClose();
		}
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x00191CB6 File Offset: 0x0018FEB6
	public void DisplayLocationText(float duration, bool forceUpdate)
	{
		if (this.displayedTextCoroutine != null)
		{
			base.StopCoroutine(this.displayedTextCoroutine);
		}
		this.displayedTextCoroutine = base.StartCoroutine(this.DisplayLocText(duration, forceUpdate));
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x00191CE0 File Offset: 0x0018FEE0
	private IEnumerator DisplayLocText(float duration, bool forceUpdate = false)
	{
		float timeDisplayed = 0f;
		if (forceUpdate)
		{
			if (Player.Instance.currentGameLocation != null && this.locationText != null)
			{
				this.locationText.text = Player.Instance.currentGameLocation.name;
			}
			else if (this.locationText != null)
			{
				this.locationText.text = string.Empty;
			}
		}
		this.ShowLocationText(1f);
		while (timeDisplayed < duration)
		{
			if (SessionData.Instance.play)
			{
				timeDisplayed += Time.deltaTime;
				Mathf.Clamp01(timeDisplayed / duration);
			}
			yield return null;
		}
		this.HideLocationText(1f);
		yield break;
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x00191CFD File Offset: 0x0018FEFD
	public void ShowLocationText(float fadeSpeed)
	{
		if (this.locationText == null)
		{
			return;
		}
		base.StopCoroutine("LocationTextFade");
		base.StartCoroutine(this.LocationTextFade(true, fadeSpeed));
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x00191D28 File Offset: 0x0018FF28
	public void HideLocationText(float fadeSpeed)
	{
		if (this.locationText == null)
		{
			return;
		}
		base.StopCoroutine("LocationTextFade");
		base.StartCoroutine(this.LocationTextFade(false, fadeSpeed));
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x00191D53 File Offset: 0x0018FF53
	private IEnumerator LocationTextFade(bool show = true, float fadeSpeed = 1f)
	{
		if (show)
		{
			if (this.locationText != null)
			{
				this.locationText.gameObject.SetActive(true);
			}
			this.locationTextDisplayed = true;
		}
		float snapProgress = 0f;
		if (show)
		{
			snapProgress = this.locationText.canvasRenderer.GetAlpha();
		}
		else
		{
			snapProgress = 1f - this.locationText.canvasRenderer.GetAlpha();
		}
		while (snapProgress < 1f)
		{
			if (!SessionData.Instance.play)
			{
				yield return null;
			}
			snapProgress += fadeSpeed * Time.deltaTime;
			snapProgress = Mathf.Clamp01((float)Mathf.CeilToInt(snapProgress * 100f) / 100f);
			if (this.locationText != null)
			{
				if (show)
				{
					this.locationText.canvasRenderer.SetAlpha(snapProgress);
				}
				else
				{
					this.locationText.canvasRenderer.SetAlpha(1f - snapProgress);
				}
			}
			yield return null;
		}
		if (!show)
		{
			if (this.locationText != null)
			{
				this.locationText.gameObject.SetActive(false);
			}
			this.locationTextDisplayed = false;
		}
		yield break;
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x00191D70 File Offset: 0x0018FF70
	public void OpenCurrentLocationAsEvidence()
	{
		if (Player.Instance.currentGameLocation != null)
		{
			this.SpawnWindow(Player.Instance.currentGameLocation.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
			return;
		}
		if (Player.Instance.currentGameLocation != null && Player.Instance.currentGameLocation != null)
		{
			this.SpawnWindow(Player.Instance.currentGameLocation.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
		}
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x00191E0C File Offset: 0x0019000C
	public void OpenApartmentAsEvidence()
	{
		if (Player.Instance.home != null)
		{
			this.SpawnWindow(Player.Instance.home.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
			return;
		}
		GameplayController.HotelGuest hotelGuest = GameplayController.Instance.hotelGuests.Find((GameplayController.HotelGuest item) => item.GetHuman() == Player.Instance);
		if (hotelGuest != null)
		{
			NewAddress address = hotelGuest.GetAddress();
			if (address != null)
			{
				this.SpawnWindow(address.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
			}
		}
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x00191EC0 File Offset: 0x001900C0
	public void SetInterfaceActive(bool val)
	{
		this.interfaceIsActive = val;
		if (this.interfaceIsActive)
		{
			InterfaceControls.Instance.hudCanvas.gameObject.SetActive(true);
			Player.Instance.firstFrame = true;
			return;
		}
		InterfaceControls.Instance.hudCanvas.gameObject.SetActive(false);
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x00191F14 File Offset: 0x00190114
	public void SetDesktopMode(bool val, bool showPanels)
	{
		this.desktopMode = val;
		Game.Log("Player: Set desktop mode: " + this.desktopMode.ToString() + ", showing panels: " + showPanels.ToString(), 2);
		if (SessionData.Instance.isFloorEdit)
		{
			this.desktopMode = false;
		}
		if (this.desktopMode)
		{
			InterfaceController.Instance.ActivateObjectivesDisplay();
			ControlsDisplayController.Instance.SetControlDisplayArea(60f, 0f, 300f, 300f);
			this.desktopModeDesiredTransition = 1f;
			if (this.desktopModeTransition != this.desktopModeDesiredTransition)
			{
				base.StopCoroutine("DesktopModeTransition");
				base.StartCoroutine("DesktopModeTransition");
			}
			this.RemoveAllMouseInteractionComponents();
			Player.Instance.EnablePlayerMouseLook(false, false);
			if (this.showDesktopCaseBoard && showPanels)
			{
				this.ShowCaseBoard(true);
			}
			else
			{
				this.ShowCaseBoard(false);
			}
			if (this.showDesktopMap && showPanels)
			{
				this.ShowDesktopMap(true, false);
			}
			else
			{
				this.ShowDesktopMap(false, false);
			}
			CasePanelController.Instance.OnShowCaseBoard();
			this.uiPointerContainer.gameObject.SetActive(false);
			this.controlPanelCanvas.gameObject.SetActive(true);
			this.dialogCanvas.gameObject.SetActive(false);
			this.interactionProgressCanvas.gameObject.SetActive(false);
			if (Player.Instance.currentGameLocation == Player.Instance.home || Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, Player.Instance.currentGameLocation))
			{
				if (this.activeWindows.Exists((InfoWindow item) => item.preset.name == "ApartmentDecor"))
				{
					SessionData.Instance.isDecorEdit = true;
					this.UpdateDOF();
					return;
				}
			}
			else
			{
				InfoWindow infoWindow = this.activeWindows.Find((InfoWindow item) => item.preset.name == "ApartmentDecor");
				if (infoWindow != null)
				{
					infoWindow.CloseWindow(false);
					return;
				}
			}
		}
		else
		{
			if (ContextMenuController.activeMenu != null)
			{
				ContextMenuController.activeMenu.ForceClose();
			}
			if (UpgradesController.Instance.isOpen)
			{
				UpgradesController.Instance.CloseUpgrades(true);
			}
			if (ControlsDisplayController.Instance != null)
			{
				if (InteractionController.Instance.talkingTo != null)
				{
					ControlsDisplayController.Instance.SetControlDisplayArea(82f, 0f, 300f, 300f);
				}
				else if (Player.Instance.setAlarmMode)
				{
					ControlsDisplayController.Instance.SetControlDisplayArea(450f, 0f, 300f, 300f);
				}
				else
				{
					ControlsDisplayController.Instance.RestoreDefaultDisplayArea();
				}
			}
			this.RemoveWindowFocus();
			if (CasePanelController.Instance != null)
			{
				if (CasePanelController.Instance.customLinkSelectionMode)
				{
					CasePanelController.Instance.CancelCustomStringLinkSelection();
				}
				if (CasePanelController.Instance.pickModeActive)
				{
					CasePanelController.Instance.SetPickModeActive(false, null);
				}
			}
			for (int i = 0; i < this.activeWindows.Count; i++)
			{
				InfoWindow infoWindow2 = this.activeWindows[i];
				if (infoWindow2.isWorldInteraction && !infoWindow2.pinnable)
				{
					infoWindow2.CloseWindow(true);
					i--;
				}
				else if (infoWindow2.pinButton != null && infoWindow2.pinButton.pointerDown)
				{
					infoWindow2.pinButton.ForcePointerUp();
				}
			}
			if (CasePanelController.Instance != null)
			{
				foreach (PinnedItemController pinnedItemController in CasePanelController.Instance.spawnedPins)
				{
					if (pinnedItemController.rb != null)
					{
						pinnedItemController.rb.simulated = false;
					}
					if (pinnedItemController.isDragging)
					{
						pinnedItemController.ForceCancelDrag();
					}
				}
				if (PinnedItemController.activeQuickMenu != null)
				{
					PinnedItemController.activeQuickMenu.Remove(true);
				}
			}
			this.desktopModeDesiredTransition = 0f;
			if (this.desktopModeTransition != this.desktopModeDesiredTransition)
			{
				try
				{
					base.StopCoroutine("DesktopModeTransition");
					base.StartCoroutine("DesktopModeTransition");
				}
				catch
				{
				}
			}
			this.ShowCaseBoard(false);
			if (CasePanelController.Instance != null)
			{
				CasePanelController.Instance.OnHideCaseBoard();
			}
			if (MapController.Instance != null)
			{
				MapController.Instance.displayFirstPerson = true;
			}
			this.ShowDesktopMap(false, false);
			if (this.dialogCanvas != null)
			{
				this.dialogCanvas.gameObject.SetActive(true);
			}
			if (this.interactionProgressCanvas != null)
			{
				this.interactionProgressCanvas.gameObject.SetActive(true);
			}
			this.RemoveAllMouseInteractionComponents();
			if (this.uiPointerContainer != null)
			{
				this.uiPointerContainer.gameObject.SetActive(true);
			}
			Player.Instance.EnablePlayerMouseLook(true, false);
		}
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x001923D8 File Offset: 0x001905D8
	public void ToggleSetShowDesktopMap()
	{
		if (this.showDesktopMap)
		{
			this.SetShowDesktopMap(false, true);
			return;
		}
		this.SetShowDesktopMap(true, true);
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x001923F3 File Offset: 0x001905F3
	public void SetShowDesktopMap(bool val, bool playSound)
	{
		this.showDesktopMap = val;
		this.mapButton.icon.enabled = this.showDesktopMap;
		if (this.desktopMode)
		{
			this.ShowDesktopMap(this.showDesktopMap, playSound);
		}
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x00192428 File Offset: 0x00190628
	public void ShowDesktopMap(bool val, bool playSound)
	{
		if (this.minimapCanvas == null)
		{
			return;
		}
		if (val)
		{
			if (MapController.Instance != null)
			{
				MapController.Instance.OpenMap(false, playSound);
				return;
			}
		}
		else if (MapController.Instance != null)
		{
			MapController.Instance.CloseMap(playSound);
		}
	}

	// Token: 0x06001BD8 RID: 7128 RVA: 0x00192479 File Offset: 0x00190679
	public void ToggleShowInventory()
	{
		BioScreenController.Instance.SetInventoryOpen(!BioScreenController.Instance.isOpen, true, true);
	}

	// Token: 0x06001BD9 RID: 7129 RVA: 0x00192494 File Offset: 0x00190694
	public void ToggleSetShowDesktopCaseBoard()
	{
		if (this.showDesktopCaseBoard)
		{
			this.SetShowDesktopCaseBoard(false);
			return;
		}
		this.SetShowDesktopCaseBoard(true);
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x001924AD File Offset: 0x001906AD
	public void SetShowDesktopCaseBoard(bool val)
	{
		if (this.caseCanvas == null)
		{
			return;
		}
		if (Player.Instance.playerKOInProgress)
		{
			val = false;
		}
		this.showDesktopCaseBoard = val;
		if (this.desktopMode)
		{
			this.ShowCaseBoard(this.showDesktopCaseBoard);
		}
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x001924E8 File Offset: 0x001906E8
	public void ShowCaseBoard(bool val)
	{
		if (this.caseCanvas == null)
		{
			return;
		}
		if (Player.Instance.playerKOInProgress)
		{
			val = false;
		}
		if (val)
		{
			this.caseCanvas.gameObject.SetActive(true);
			foreach (PinnedItemController pinnedItemController in CasePanelController.Instance.spawnedPins)
			{
				pinnedItemController.transform.localScale = Vector3.one;
				pinnedItemController.transform.localPosition = pinnedItemController.caseElement.v;
			}
			CasePanelController.Instance.UpdatePinned();
			return;
		}
		if (this.boxSelectActive)
		{
			Object.Destroy(this.boxSelect.gameObject);
			this.boxSelectActive = false;
		}
		this.DeselectAllPins();
		this.caseCanvas.gameObject.SetActive(false);
	}

	// Token: 0x06001BDC RID: 7132 RVA: 0x001925D8 File Offset: 0x001907D8
	public void SetBackgroundBlur(bool val)
	{
		if (this.backgroundBlur == null)
		{
			return;
		}
		this.backgroundBlur.SetActive(val);
	}

	// Token: 0x06001BDD RID: 7133 RVA: 0x001925F8 File Offset: 0x001907F8
	public void NewHelpPointer(string helpSection)
	{
		HelpContentPage helpContentPage = null;
		if (Toolbox.Instance.allHelpContent.TryGetValue(helpSection, ref helpContentPage))
		{
			new StringBuilder();
			string newMessage = string.Empty;
			List<int> list2;
			List<string> list = Player.Instance.ParseDDSMessage(helpContentPage.messageID, null, out list2, false, null, false);
			if (list.Count > 0)
			{
				newMessage = Strings.ComposeText(Strings.Get("dds.blocks", list[0], Strings.Casing.asIs, false, false, false, null), null, Strings.LinkSetting.forceNoLinks, null, null, false);
				this.NewGameMessage(InterfaceController.GameMessageType.helpPointer, 0, newMessage, InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
			}
		}
	}

	// Token: 0x06001BDE RID: 7134 RVA: 0x0019268C File Offset: 0x0019088C
	public void NewGameMessage(InterfaceController.GameMessageType newType, int newNumerical, string newMessage, InterfaceControls.Icon newIcon = InterfaceControls.Icon.agent, AudioEvent additionalSFX = null, bool colourOverride = false, Color col = default(Color), int newMergeType = -1, float newMessageDelay = 0f, RectTransform moveToOnDestroy = null, GameMessageController.PingOnComplete ping = GameMessageController.PingOnComplete.none, Evidence keyMergeEvidence = null, List<Evidence.DataKey> keyMergeKeys = null, Sprite iconOverride = null)
	{
		if (Game.Instance.printDebug)
		{
			Game.Log("Adding new game message: " + newType.ToString() + ": " + newMessage, 2);
		}
		InterfaceController.GameMessage gameMessage = new InterfaceController.GameMessage();
		gameMessage.messageType = newType;
		gameMessage.numerical = newNumerical;
		gameMessage.graphic = InterfaceControls.Instance.iconReference.Find((InterfaceControls.IconConfig item) => item.iconType == newIcon).sprite;
		if (iconOverride != null)
		{
			gameMessage.graphic = iconOverride;
		}
		gameMessage.message = newMessage;
		gameMessage.additionalSFX = additionalSFX;
		gameMessage.colourOverride = colourOverride;
		gameMessage.col = col;
		gameMessage.mergeType = newMergeType;
		gameMessage.delay = newMessageDelay;
		gameMessage.moveOnDestroy = moveToOnDestroy;
		gameMessage.ping = ping;
		gameMessage.keyMergeEvidence = keyMergeEvidence;
		gameMessage.mergedKeys = keyMergeKeys;
		if (newType == InterfaceController.GameMessageType.keyMerge)
		{
			gameMessage.keyMerge = true;
		}
		else if (newType == InterfaceController.GameMessageType.socialCredit)
		{
			gameMessage.socCredit = true;
		}
		if (newType == InterfaceController.GameMessageType.notification || newType == InterfaceController.GameMessageType.keyMerge || newType == InterfaceController.GameMessageType.socialCredit)
		{
			if (this.notificationQueue.Exists((InterfaceController.GameMessage item) => item.message == newMessage))
			{
				return;
			}
			this.notificationQueue.Add(gameMessage);
		}
		else if (newType == InterfaceController.GameMessageType.gameHeader)
		{
			this.gameHeaderQueue.Add(gameMessage);
		}
		else if (newType == InterfaceController.GameMessageType.helpPointer)
		{
			this.helpPointerQueue.Add(gameMessage);
		}
		if (!this.messageCoroutineRunning)
		{
			base.StartCoroutine(this.GameMessages());
		}
	}

	// Token: 0x06001BDF RID: 7135 RVA: 0x00192804 File Offset: 0x00190A04
	private IEnumerator GameMessages()
	{
		while (this.gameSceenDisplayed)
		{
			yield return null;
		}
		this.messageCoroutineRunning = true;
		bool waitedAFrame = false;
		AudioController.LoopingSoundInfo typewriterSoundTriggered = null;
		while (this.notificationQueue.Count + this.gameHeaderQueue.Count + this.helpPointerQueue.Count > 0)
		{
			if (!waitedAFrame)
			{
				waitedAFrame = true;
				yield return null;
			}
			if (this.helpPointerQueue.Count > 0 && !PopupMessageController.Instance.active && !CutSceneController.Instance.cutSceneActive && (SessionData.Instance.play || !BioScreenController.Instance.isOpen))
			{
				if (this.currentHelpPointer == null)
				{
					this.helpPointerRect.gameObject.SetActive(true);
					this.currentHelpPointer = this.helpPointerQueue[0];
					this.helpPointerTextDisplay = this.currentHelpPointer.message;
					this.helpPointerProgress = 0f;
					this.helpPointerFadeOut = 1f;
					this.helpPointerTimer = 0f;
					this.helpPointerText.SetText(this.currentHelpPointer.message, true);
					this.helpPointerDesiredHeight = this.helpPointerText.preferredHeight + 20f;
					this.helpPointerText.SetText(string.Empty, true);
					this.helpPointerRect.sizeDelta = new Vector2(this.helpPointerRect.sizeDelta.x, 44f);
					using (List<CanvasRenderer>.Enumerator enumerator = this.helpPointerRenderers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CanvasRenderer canvasRenderer = enumerator.Current;
							canvasRenderer.SetAlpha(0f);
						}
						goto IL_93F;
					}
				}
				if (this.helpPointerProgress < 1f)
				{
					this.helpPointerProgress += Time.deltaTime / 0.5f;
					this.helpPointerProgress = Mathf.Clamp01(this.helpPointerProgress);
					this.helpPointerText.text = this.helpPointerTextDisplay.Substring(0, Mathf.CeilToInt(Mathf.Lerp(0f, (float)this.helpPointerTextDisplay.Length, this.helpPointerProgress)));
					this.helpPointerRect.sizeDelta = new Vector2(this.helpPointerRect.sizeDelta.x, Mathf.Lerp(this.helpPointerRect.sizeDelta.y, this.helpPointerDesiredHeight, this.helpPointerProgress));
					using (List<CanvasRenderer>.Enumerator enumerator = this.helpPointerRenderers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CanvasRenderer canvasRenderer2 = enumerator.Current;
							canvasRenderer2.SetAlpha(this.helpPointerProgress);
						}
						goto IL_93F;
					}
				}
				this.helpPointerTimer += Time.deltaTime;
				if (this.helpPointerTimer >= (float)this.helpPointerTextDisplay.Length * 0.045f)
				{
					if (this.helpPointerFadeOut > 0f)
					{
						this.helpPointerFadeOut -= Time.deltaTime / 0.5f;
						this.helpPointerFadeOut = Mathf.Clamp01(this.helpPointerFadeOut);
						using (List<CanvasRenderer>.Enumerator enumerator = this.helpPointerRenderers.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								CanvasRenderer canvasRenderer3 = enumerator.Current;
								canvasRenderer3.SetAlpha(this.helpPointerFadeOut);
							}
							goto IL_93F;
						}
					}
					this.currentHelpPointer = null;
					this.helpPointerQueue.RemoveAt(0);
					this.helpPointerRect.gameObject.SetActive(false);
				}
			}
			else if (this.currentNotification == null && this.notificationQueue.Count > 0 && SessionData.Instance.play && !PopupMessageController.Instance.active && !CutSceneController.Instance.cutSceneActive)
			{
				InterfaceController.GameMessage gameMessage = this.notificationQueue[0];
				List<InterfaceController.GameMessage> list = new List<InterfaceController.GameMessage>();
				if (gameMessage.mergeType > -1)
				{
					for (int i = 1; i < this.notificationQueue.Count; i++)
					{
						InterfaceController.GameMessage gameMessage2 = this.notificationQueue[i];
						if (gameMessage2.mergeType != gameMessage.mergeType)
						{
							break;
						}
						gameMessage.numerical += gameMessage2.numerical;
						list.Add(gameMessage2);
						this.notificationQueue.RemoveAt(i);
						i--;
					}
				}
				string text = string.Empty;
				string text2 = string.Empty;
				if (gameMessage.numerical != 0)
				{
					string text3 = "+";
					if (gameMessage.numerical < 0)
					{
						text3 = string.Empty;
					}
					text = text3 + gameMessage.numerical.ToString() + " ";
				}
				if (list.Count > 0)
				{
					text2 = "... x" + list.Count.ToString();
				}
				if (this.notificationQueue[0].keyMerge)
				{
					this.currentNotification = Object.Instantiate<GameObject>(PrefabControls.Instance.keyMergeGameMessage, this.gameMessageParent);
				}
				else if (this.notificationQueue[0].socCredit)
				{
					this.currentNotification = Object.Instantiate<GameObject>(PrefabControls.Instance.socialCreditGameMessage, this.gameMessageParent);
				}
				else
				{
					this.currentNotification = Object.Instantiate<GameObject>(PrefabControls.Instance.gameMessage, this.gameMessageParent);
				}
				this.currentNotification.GetComponent<GameMessageController>().Setup(this.notificationQueue[0].graphic, text + this.notificationQueue[0].message + text2, this.notificationQueue[0].moveOnDestroy, this.notificationQueue[0].colourOverride, this.notificationQueue[0].col, this.notificationQueue[0].ping, this.notificationQueue[0].keyMergeEvidence, this.notificationQueue[0].mergedKeys, this.notificationQueue[0].numerical);
				AudioController.Instance.Play2DSound(AudioControls.Instance.gameMessage, null, 1f);
				if (this.notificationQueue[0].additionalSFX != null)
				{
					AudioController.Instance.Play2DSound(this.notificationQueue[0].additionalSFX, null, 1f);
				}
				this.notificationQueue.RemoveAt(0);
			}
			else if (this.gameHeaderQueue.Count > 0 && SessionData.Instance.play && !PopupMessageController.Instance.active && !CutSceneController.Instance.cutSceneActive)
			{
				if (this.currentGameHeader == null)
				{
					this.currentGameHeader = this.gameHeaderQueue[0];
					this.gameHeaderDisplayed = false;
					this.gameHeaderTimer = 2f;
					this.titleTextRenderer.SetAlpha(1f);
					this.gameHeaderFadeDelay = 1f;
					this.titleText.text = string.Empty;
					this.gameHeaderDelay = this.currentGameHeader.delay;
					Game.Log("Interface: New game header: " + this.currentGameHeader.message, 2);
				}
				if (this.gameHeaderDelay > 0f)
				{
					this.gameHeaderDelay -= Time.deltaTime;
				}
				else if (!this.gameHeaderDisplayed)
				{
					if (this.typewriterDelay > 0f)
					{
						this.typewriterDelay -= Time.deltaTime;
					}
					else
					{
						int length = this.titleText.text.Length;
						if (this.currentGameHeader.message.Length > length)
						{
							this.titleText.text = this.currentGameHeader.message.Substring(0, length + 1);
							this.typewriterDelay = 0.06f;
							if (this.titleText.text.get_Chars(this.titleText.text.Length - 1) != ' ')
							{
								base.Invoke("PlayTypewriterKey", AudioControls.Instance.typewriterKeystrokeEventDelay);
							}
						}
						if (this.titleText.text.Length >= this.currentGameHeader.message.Length)
						{
							Game.Log("Interface: Game header length " + this.titleText.text.Length.ToString() + " >= " + this.currentGameHeader.message.Length.ToString(), 2);
							this.gameHeaderDisplayed = true;
						}
					}
				}
				else if (this.gameHeaderDisplayed)
				{
					if (typewriterSoundTriggered != null)
					{
						AudioController.Instance.StopSound(typewriterSoundTriggered, AudioController.StopType.fade, "Stop typewriter");
						typewriterSoundTriggered = null;
					}
					if (this.gameHeaderTimer > 0f)
					{
						this.gameHeaderTimer -= Time.deltaTime;
					}
					else if (this.gameHeaderFadeDelay > 0f)
					{
						this.gameHeaderFadeDelay -= Time.deltaTime * 3f;
						this.gameHeaderFadeDelay = Mathf.Clamp01(this.gameHeaderFadeDelay);
						this.titleTextRenderer.SetAlpha(this.gameHeaderFadeDelay);
					}
					else
					{
						this.gameHeaderQueue.RemoveAt(0);
						this.currentGameHeader = null;
						this.gameHeaderDisplayed = false;
						this.titleText.text = string.Empty;
					}
				}
			}
			IL_93F:
			yield return null;
		}
		if (typewriterSoundTriggered != null)
		{
			AudioController.Instance.StopSound(typewriterSoundTriggered, AudioController.StopType.fade, "Stop typewriter");
			typewriterSoundTriggered = null;
		}
		this.messageCoroutineRunning = false;
		yield break;
	}

	// Token: 0x06001BE0 RID: 7136 RVA: 0x00192813 File Offset: 0x00190A13
	private void PlayTypewriterKey()
	{
		AudioController.Instance.Play2DSound(AudioControls.Instance.typewriter, null, 1f);
	}

	// Token: 0x06001BE1 RID: 7137 RVA: 0x00002265 File Offset: 0x00000465
	private void PlayTypewriterSpace()
	{
	}

	// Token: 0x06001BE2 RID: 7138 RVA: 0x00192830 File Offset: 0x00190A30
	public void ToggleNotebookButton()
	{
		this.ToggleNotebook("", false);
	}

	// Token: 0x06001BE3 RID: 7139 RVA: 0x00192840 File Offset: 0x00190A40
	public void ToggleNotebook(string startingPage = "", bool openHelpSection = false)
	{
		this.detectiveNotebook = this.activeWindows.Find((InfoWindow item) => item.preset.name == "DetectivesNotebook");
		if (this.detectiveNotebook == null)
		{
			bool worldInteraction = true;
			if (!SessionData.Instance.play)
			{
				worldInteraction = false;
			}
			this.notebookButton.icon.enabled = true;
			if (!Player.Instance.autoTravelActive || Game.Instance.autoTravelPause)
			{
				SessionData.Instance.PauseGame(true, false, true);
			}
			this.openHelpToPage = startingPage.ToLower();
			Evidence passedEvidence = null;
			Evidence.DataKey passedEvidenceKey = Evidence.DataKey.name;
			List<Evidence.DataKey> passedEvidenceKeys = null;
			string presetName = "DetectivesNotebook";
			Vector2 handbookWindowPosition = InterfaceControls.Instance.handbookWindowPosition;
			this.detectiveNotebook = this.SpawnWindow(passedEvidence, passedEvidenceKey, passedEvidenceKeys, presetName, worldInteraction, false, handbookWindowPosition, null, null, null, true);
			if (openHelpSection)
			{
				try
				{
					this.detectiveNotebook.SetActiveContent(this.detectiveNotebook.tabs[1].content);
				}
				catch
				{
				}
			}
			this.detectiveNotebook.OnWindowClosed += this.ResetToggleNotebookButton;
			return;
		}
		this.notebookButton.icon.enabled = false;
		this.ResetToggleNotebookButton();
		this.detectiveNotebook.CloseWindow(true);
		if (!InterfaceController.Instance.desktopMode && !PopupMessageController.Instance.active && !MainMenuController.Instance.mainMenuActive)
		{
			SessionData.Instance.ResumeGame();
		}
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x001929A8 File Offset: 0x00190BA8
	public void OpenNotebookNoPause(string startingPage = "", bool openHelpSection = false)
	{
		this.detectiveNotebook = this.activeWindows.Find((InfoWindow item) => item.preset.name == "DetectivesNotebook");
		if (this.detectiveNotebook == null)
		{
			this.notebookButton.icon.enabled = true;
			this.openHelpToPage = startingPage.ToLower();
			this.detectiveNotebook = this.SpawnWindow(null, Evidence.DataKey.name, null, "DetectivesNotebook", false, false, InterfaceControls.Instance.handbookWindowPosition, null, null, null, true);
			if (openHelpSection)
			{
				try
				{
					this.detectiveNotebook.SetActiveContent(this.detectiveNotebook.tabs[1].content);
				}
				catch
				{
				}
			}
			this.detectiveNotebook.OnWindowClosed += this.ResetToggleNotebookButton;
		}
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x00192A88 File Offset: 0x00190C88
	public void ResetToggleNotebookButton()
	{
		this.notebookButton.icon.enabled = false;
		this.detectiveNotebook.OnWindowClosed -= this.ResetToggleNotebookButton;
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x00192AB2 File Offset: 0x00190CB2
	public void ToggleUpgrades()
	{
		if (this.upgradesCanvas == null)
		{
			return;
		}
		if (!UpgradesController.Instance.isOpen)
		{
			UpgradesController.Instance.OpenUpgrades(true);
			return;
		}
		UpgradesController.Instance.CloseUpgrades(true);
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x00192AE8 File Offset: 0x00190CE8
	public void Fade(float fadeVal, float newFadeTime = 2f, bool newFadeAudio = false)
	{
		this.desiredFade = fadeVal;
		this.fadeTime = newFadeTime;
		this.fadeAudio = newFadeAudio;
		Game.Log("Interface: Fade to " + this.desiredFade.ToString() + " with time " + this.fadeTime.ToString(), 2);
		base.StopCoroutine("FadeGame");
		base.StartCoroutine("FadeGame");
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x00192B4C File Offset: 0x00190D4C
	private IEnumerator FadeGame()
	{
		if (this.fadeOverlay != null)
		{
			this.fadeOverlay.gameObject.SetActive(true);
		}
		this.desiredFade = Mathf.Clamp01(this.desiredFade);
		this.UpdateDOF();
		while (this.desiredFade != this.fade)
		{
			if (!SessionData.Instance.play)
			{
				yield return null;
			}
			else
			{
				if (this.fade < this.desiredFade)
				{
					this.fade += Time.deltaTime / this.fadeTime;
				}
				else if (this.fade > this.desiredFade)
				{
					this.fade -= Time.deltaTime / this.fadeTime;
				}
				this.fade = Mathf.Clamp01(this.fade);
				if (this.fadeOverlay != null)
				{
					float alpha = this.fadeOverlayAlphaCurve.Evaluate(this.fade);
					this.fadeOverlay.SetAlpha(alpha);
				}
				SessionData.Instance.vignette.active = true;
				SessionData.Instance.vignette.intensity.value = Mathf.Max(this.fade, StatusController.Instance.vignetteAmount);
				bool flag = this.fadeAudio;
			}
			InteractionController.Instance.InteractionRaycastCheck();
			yield return null;
		}
		if (StatusController.Instance.vignetteAmount <= 0f)
		{
			SessionData.Instance.vignette.active = false;
		}
		if (this.fade <= 0f)
		{
			if (this.fadeOverlay != null)
			{
				this.fadeOverlay.gameObject.SetActive(false);
			}
			InteractionController.Instance.InteractionRaycastCheck();
		}
		this.UpdateDOF();
		yield break;
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x00192B5B File Offset: 0x00190D5B
	private IEnumerator DesktopModeTransition()
	{
		RectTransform canvasRect = this.caseCanvas.GetComponent<RectTransform>();
		RectTransform windowRect = this.windowCanvas.GetComponent<RectTransform>();
		bool setControlDisplay = false;
		if (!this.desktopMode)
		{
		}
		while (this.desktopModeTransition != this.desktopModeDesiredTransition)
		{
			if (this.desktopModeTransition < this.desktopModeDesiredTransition)
			{
				this.desktopModeTransition += ((this.desktopModeDesiredTransition - this.desktopModeTransition) * 5f + 1f) * Time.deltaTime;
			}
			else if (this.desktopModeTransition > this.desktopModeDesiredTransition)
			{
				this.desktopModeTransition -= ((this.desktopModeTransition - this.desktopModeDesiredTransition) * 10f + 1f) * Time.deltaTime;
			}
			this.desktopModeTransition = Mathf.Clamp01(this.desktopModeTransition);
			this.desktopModeDesiredTransition = Mathf.Clamp01(this.desktopModeDesiredTransition);
			float num = Mathf.Max(this.desktopModeTransition * this.desktopModeTransition * this.desktopModeTransition, 0.2f);
			Vector3 localScale;
			localScale..ctor(num, num, num);
			canvasRect.localScale = localScale;
			windowRect.localScale = localScale;
			this.caseCanvasGroup.alpha = this.desktopModeTransition;
			this.controlPanelCanvasGroup.alpha = this.desktopModeTransition;
			this.windowCanvasGroup.alpha = this.desktopModeTransition;
			this.upgradesCanvasGroup.alpha = this.desktopModeTransition;
			foreach (InfoWindow infoWindow in this.activeWindows)
			{
				infoWindow.windowCanvasGroup.alpha = this.desktopModeTransition;
				infoWindow.contentCanvasGroup.alpha = this.desktopModeTransition;
				infoWindow.RestoreAnchoredPosition();
			}
			this.gameWorldCanvasGroup.alpha = 1f - this.desktopModeTransition;
			if (this.desktopModeTransition <= 0.5f)
			{
				if (!setControlDisplay && !this.desktopMode)
				{
					InteractionController.Instance.UpdateInteractionText();
					setControlDisplay = true;
				}
				this.controlsCanvasGroup.alpha = 1f - this.desktopModeTransition * 2f;
			}
			else
			{
				if (!setControlDisplay && this.desktopMode)
				{
					InteractionController.Instance.UpdateInteractionText();
					setControlDisplay = true;
				}
				this.controlsCanvasGroup.alpha = (this.desktopModeTransition - 0.5f) * 2f;
			}
			SessionData.Instance.vignette.active = true;
			SessionData.Instance.vignette.intensity.value = Mathf.Lerp(0f, 0.5f, this.desktopModeTransition) + StatusController.Instance.vignetteAmount;
			if (this.desktopModeTransition <= 0f && this.desktopModeDesiredTransition <= 0f)
			{
				if (StatusController.Instance.vignetteAmount <= 0f)
				{
					SessionData.Instance.vignette.active = false;
				}
				this.controlPanelCanvas.gameObject.SetActive(false);
			}
			yield return null;
		}
		if (this.desktopMode)
		{
			foreach (PinnedItemController pinnedItemController in CasePanelController.Instance.spawnedPins)
			{
				if (pinnedItemController.rb != null)
				{
					pinnedItemController.rb.simulated = true;
				}
			}
			this.speechAnchor.gameObject.SetActive(false);
			this.objectCycleAnchor.gameObject.SetActive(false);
		}
		else
		{
			this.speechAnchor.gameObject.SetActive(true);
			this.objectCycleAnchor.gameObject.SetActive(!InteractionController.Instance.dialogMode);
		}
		InteractionController.Instance.UpdateInteractionText();
		yield break;
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x00192B6A File Offset: 0x00190D6A
	public void InputCodeButton(List<int> code)
	{
		if (this.OnInputCode != null)
		{
			this.OnInputCode(code);
		}
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x00192B80 File Offset: 0x00190D80
	public void AddMouseOverElement(MonoBehaviour mono)
	{
		if (mono == null)
		{
			return;
		}
		if (!this.currentMouseOverElement.Contains(mono))
		{
			this.currentMouseOverElement.Add(mono);
		}
		this.UpdateCursorSprite();
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x00192BAC File Offset: 0x00190DAC
	public void RemoveMouseOverElement(MonoBehaviour mono)
	{
		if (mono == null)
		{
			return;
		}
		this.currentMouseOverElement.Remove(mono);
		this.UpdateCursorSprite();
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x00192BCB File Offset: 0x00190DCB
	public void ClearAllMouseOverElements()
	{
		Game.Log("Interface: Clearing mouse over elements", 2);
		this.currentMouseOverElement.Clear();
		this.UpdateCursorSprite();
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x00192BEC File Offset: 0x00190DEC
	public void UpdateCursorSprite()
	{
		if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() && this.currentMouseOverElement.Count > 0)
		{
			MonoBehaviour monoBehaviour = this.currentMouseOverElement[this.currentMouseOverElement.Count - 1];
			if (CasePanelController.Instance != null && (CasePanelController.Instance.customLinkSelectionMode || CasePanelController.Instance.pickModeActive) && !PopupMessageController.Instance.active)
			{
				Cursor.SetCursor(InterfaceControls.Instance.cursorTarget, new Vector2(64f, 64f), 0);
				InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
				return;
			}
			if (monoBehaviour as DragPanel != null)
			{
				Cursor.SetCursor(InterfaceControls.Instance.cursorMove, new Vector2(64f, 64f), 0);
				InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
				return;
			}
			if (monoBehaviour as PinnedPinButtonController != null)
			{
				Cursor.SetCursor(InterfaceControls.Instance.cursorMove, new Vector2(64f, 64f), 0);
				InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
				return;
			}
			if (monoBehaviour as PinnedItemController != null)
			{
				Cursor.SetCursor(InterfaceControls.Instance.cursorMove, new Vector2(64f, 64f), 0);
				InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
				return;
			}
			if (monoBehaviour as CustomScrollRect != null && (monoBehaviour as CustomScrollRect).isScrolling)
			{
				Cursor.SetCursor(InterfaceControls.Instance.cursorMove, new Vector2(64f, 64f), 0);
				InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
				return;
			}
			if (monoBehaviour as ResizePanel != null)
			{
				ResizePanel resizePanel = monoBehaviour as ResizePanel;
				if (resizePanel.pivot.x == 0f && resizePanel.pivot.y == 0f)
				{
					Cursor.SetCursor(InterfaceControls.Instance.cursorResizeDiagonalRightLeft, new Vector2(64f, 64f), 0);
					InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
					return;
				}
				if (resizePanel.pivot.x == 1f && resizePanel.pivot.y == 0f)
				{
					Cursor.SetCursor(InterfaceControls.Instance.cursorResizeDiagonalLeftRight, new Vector2(64f, 64f), 0);
					InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
					return;
				}
				if (resizePanel.pivot.x == 0f && resizePanel.pivot.y == 1f)
				{
					Cursor.SetCursor(InterfaceControls.Instance.cursorResizeDiagonalLeftRight, new Vector2(64f, 64f), 0);
					InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
					return;
				}
				if (resizePanel.pivot.x == 1f && resizePanel.pivot.y == 1f)
				{
					Cursor.SetCursor(InterfaceControls.Instance.cursorResizeDiagonalRightLeft, new Vector2(64f, 64f), 0);
					InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
					return;
				}
				if (resizePanel.pivot.x == 0.5f)
				{
					Cursor.SetCursor(InterfaceControls.Instance.cursorResizeVertical, new Vector2(64f, 64f), 0);
					InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
					return;
				}
				if (resizePanel.pivot.y == 0.5f)
				{
					Cursor.SetCursor(InterfaceControls.Instance.cursorResizeHorizonal, new Vector2(64f, 64f), 0);
					InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
					return;
				}
			}
			else
			{
				if (monoBehaviour as DragCoverage != null)
				{
					Cursor.SetCursor(InterfaceControls.Instance.cursorResizeHorizonal, new Vector2(64f, 64f), 0);
					InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
					return;
				}
				if (monoBehaviour as ButtonController != null && (monoBehaviour as ButtonController).interactable)
				{
					Cursor.SetCursor(InterfaceControls.Instance.cursorButton, new Vector2(49f, 15f), 0);
					InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
					return;
				}
				if (monoBehaviour as ForceMouseOverInput != null)
				{
					ForceMouseOverInput forceMouseOverInput = monoBehaviour as ForceMouseOverInput;
					if (forceMouseOverInput.cursorType == 0)
					{
						Cursor.SetCursor(InterfaceControls.Instance.cursorTextEdit, new Vector2(64f, 64f), 0);
						InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
						return;
					}
					if (forceMouseOverInput.cursorType == -1)
					{
						Cursor.SetCursor(InterfaceControls.Instance.normalCursor, new Vector2(10f, 13f), 0);
						InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
						return;
					}
				}
				else if (monoBehaviour as WindowRenameTitleController != null)
				{
					Cursor.SetCursor(InterfaceControls.Instance.cursorTextEdit, new Vector2(64f, 64f), 0);
					InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
					return;
				}
			}
		}
		try
		{
			Cursor.SetCursor(InterfaceControls.Instance.normalCursor, new Vector2(10f, 13f), 0);
			InputController.Instance.SetCursorVisible(InputController.Instance.cursorVisible);
		}
		catch
		{
		}
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x00193170 File Offset: 0x00191370
	public void MinimizeAll()
	{
		int num = 999;
		while (this.activeWindows.Count > 0 && num > 0)
		{
			this.activeWindows[0].CloseWindow(true);
			num--;
		}
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x001931B0 File Offset: 0x001913B0
	public void ShowWindowFocus()
	{
		if (this.windowFocus != null && !this.windowFocus.gameObject.activeSelf)
		{
			Game.Log("Interface: Show window focus", 2);
			this.windowFocus.gameObject.SetActive(true);
			this.windowFocus.SetAsLastSibling();
		}
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x00193204 File Offset: 0x00191404
	public void RemoveWindowFocus()
	{
		if (this.windowFocus != null)
		{
			Game.Log("Interface: Remove window focus", 2);
			this.windowFocus.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x00193230 File Offset: 0x00191430
	public void CrosshairReaction()
	{
		Object.Instantiate<GameObject>(PrefabControls.Instance.crosshairReaction, InterfaceControls.Instance.lightOrbRect);
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x0019324C File Offset: 0x0019144C
	public Color GetEvidenceColour(InterfaceControls.EvidenceColours col)
	{
		return InterfaceControls.Instance.pinColours.Find((InterfaceControls.PinColours item) => item.colour == col).actualColour;
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x00193288 File Offset: 0x00191488
	public void PingLockpicks()
	{
		if (!this.lockpickNotificationActive)
		{
			base.StartCoroutine(this.ExecutePing(this.lockpicksNotificationIcon, this.lockpicksNotificationJuice, this.lockpicksNotificationText, this.lastLockpicks, this.lockpicksNotificationRenderers, false));
		}
		this.lastLockpicks = GameplayController.Instance.lockPicks;
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x001932DC File Offset: 0x001914DC
	public void PingMoney()
	{
		if (!this.moneyNotificationActive)
		{
			base.StartCoroutine(this.ExecutePing(this.moneyNotificationIcon, this.moneyNotificationJuice, this.moneyNotificationText, this.lastMoney, this.moneyNotificationRenderers, true));
		}
		this.lastMoney = GameplayController.Instance.money;
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x0019332D File Offset: 0x0019152D
	private IEnumerator ExecutePing(RectTransform pingRect, JuiceController pingJuice, TextMeshProUGUI textPing, int originalValue, List<CanvasRenderer> renderers, bool isMoney)
	{
		float progress = 0f;
		pingRect.gameObject.SetActive(true);
		pingJuice.Flash(2, false, default(Color), 10f);
		textPing.text = originalValue.ToString();
		string moneyStr = string.Empty;
		if (isMoney)
		{
			moneyStr = CityControls.Instance.cityCurrency;
		}
		using (List<CanvasRenderer>.Enumerator enumerator = renderers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CanvasRenderer canvasRenderer = enumerator.Current;
				canvasRenderer.SetAlpha(1f);
			}
			goto IL_290;
		}
		IL_CC:
		int num = GameplayController.Instance.money;
		if (!isMoney)
		{
			num = GameplayController.Instance.lockPicks;
		}
		if (originalValue < num)
		{
			originalValue += Mathf.CeilToInt(Time.deltaTime * 60f);
			if (originalValue + 100 < num)
			{
				originalValue += Mathf.CeilToInt(10f * Time.deltaTime * 60f);
			}
			originalValue = Mathf.Min(originalValue, num);
			textPing.text = moneyStr + originalValue.ToString();
		}
		else if (originalValue > num)
		{
			originalValue -= Mathf.CeilToInt(Time.deltaTime * 60f);
			if (originalValue - 100 > num)
			{
				originalValue -= Mathf.CeilToInt(10f * Time.deltaTime * 60f);
			}
			originalValue = Mathf.Max(originalValue, num);
			textPing.text = moneyStr + originalValue.ToString();
		}
		else
		{
			progress += Time.deltaTime / 1f;
			if (progress > 1f)
			{
				foreach (CanvasRenderer canvasRenderer2 in renderers)
				{
					canvasRenderer2.SetAlpha(1f - (progress - 1f));
				}
			}
		}
		yield return null;
		IL_290:
		if (progress >= 2f)
		{
			pingRect.gameObject.SetActive(false);
			if (isMoney)
			{
				this.moneyNotificationActive = false;
			}
			else
			{
				this.lockpickNotificationActive = false;
			}
			yield break;
		}
		goto IL_CC;
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x0019336C File Offset: 0x0019156C
	public void SetCrosshairVisible(bool val)
	{
		this.crosshairVisible = val;
		if (!SessionData.Instance.play)
		{
			this.crosshairVisible = false;
		}
		Game.Log("Player: Set crosshair visible: " + this.crosshairVisible.ToString(), 2);
		if (InterfaceControls.Instance.reticleContainer != null)
		{
			InterfaceControls.Instance.reticleContainer.gameObject.SetActive(this.crosshairVisible);
		}
		if (this.crosshairVisible)
		{
			if (InterfaceControls.Instance.seenRenderer != null)
			{
				InterfaceControls.Instance.seenRenderer.SetAlpha(Player.Instance.seenIconLag);
			}
			if (InterfaceControls.Instance.lightOrbFillImg != null)
			{
				InterfaceControls.Instance.lightOrbFillImg.canvasRenderer.SetAlpha(Player.Instance.visibilityLag * 0.9f);
			}
		}
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x00193443 File Offset: 0x00191643
	public void SetPlayerTextInput(bool val)
	{
		this.playerTextInputActive = val;
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x0019344C File Offset: 0x0019164C
	public void SetActiveCodeInput(KeypadController keypad)
	{
		this.activeCodeInput = keypad;
		if (this.OnNewActiveCodeInput != null)
		{
			this.OnNewActiveCodeInput(keypad);
		}
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x0019346C File Offset: 0x0019166C
	public void ActivateObjectivesDisplay()
	{
		this.objectivesDisplayTimer = 7f;
		int num = 0;
		Chapter chapter;
		if (Toolbox.Instance.IsStoryMissionActive(out chapter, out num) && num < 31)
		{
			this.objectivesDisplayTimer = 99999f;
		}
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x001934A6 File Offset: 0x001916A6
	[Button(null, 0)]
	public void NewMurderCaseDisplay()
	{
		Game.Log("Interface: New murder case display", 2);
		base.StartCoroutine(this.DisplayMissionEndText(InterfaceController.ScreenDisplayType.newMurderCase, null));
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x001934C2 File Offset: 0x001916C2
	[Button(null, 0)]
	public void MissionCompleteDisplay()
	{
		this.ExecuteMissionCompleteDisplay(CasePanelController.Instance.activeCase);
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x001934D4 File Offset: 0x001916D4
	[Button(null, 0)]
	public void ApartmentPurchaseDisplay()
	{
		Game.Log("Interface: New apartment purchased", 2);
		base.StartCoroutine(this.DisplayMissionEndText(InterfaceController.ScreenDisplayType.apartmentPurchase, null));
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x001934F0 File Offset: 0x001916F0
	public void ExecuteMissionCompleteDisplay(Case forCase)
	{
		Game.Log("Interface: Mission complete display", 2);
		base.StartCoroutine(this.DisplayMissionEndText(InterfaceController.ScreenDisplayType.missionComplete, forCase));
		if (forCase.caseType == Case.CaseType.mainStory || forCase.caseType == Case.CaseType.murder)
		{
			GameplayController.Instance.AddSocialCredit(GameplayControls.Instance.socialCreditForMurders, true, "Mission complete debug");
			return;
		}
		if (forCase.caseType == Case.CaseType.sideJob)
		{
			GameplayController.Instance.AddSocialCredit(GameplayControls.Instance.socialCreditForSideJobs, true, "Mission complete debug");
		}
	}

	// Token: 0x06001BFF RID: 7167 RVA: 0x00193566 File Offset: 0x00191766
	[Button(null, 0)]
	public void SocialCreditLevelUpDisplay()
	{
		if (!this.levelUpScreenActive)
		{
			Game.Log("Interface: Social credit level up display", 2);
			base.StartCoroutine(this.DisplayMissionEndText(InterfaceController.ScreenDisplayType.socialCreditLevelUp, null));
		}
	}

	// Token: 0x06001C00 RID: 7168 RVA: 0x0019358A File Offset: 0x0019178A
	[Button(null, 0)]
	public void MissionFailedDisplay()
	{
		this.ExecuteMissionFailedDisplay(CasePanelController.Instance.activeCase);
	}

	// Token: 0x06001C01 RID: 7169 RVA: 0x0019359C File Offset: 0x0019179C
	public void ExecuteMissionFailedDisplay(Case forCase)
	{
		Game.Log("Interface: Mission failed display", 2);
		base.StartCoroutine(this.DisplayMissionEndText(InterfaceController.ScreenDisplayType.missionFailed, forCase));
	}

	// Token: 0x06001C02 RID: 7170 RVA: 0x001935B8 File Offset: 0x001917B8
	public void ExecuteGameOverDisplay()
	{
		Game.Log("Interface: Game over display", 2);
		base.StartCoroutine(this.DisplayMissionEndText(InterfaceController.ScreenDisplayType.gameOver, null));
	}

	// Token: 0x06001C03 RID: 7171 RVA: 0x001935D4 File Offset: 0x001917D4
	[Button(null, 0)]
	public void UnsolvedDisplay()
	{
		this.ExecuteMissionUnsolvedDisplay(CasePanelController.Instance.activeCase);
	}

	// Token: 0x06001C04 RID: 7172 RVA: 0x001935E6 File Offset: 0x001917E6
	public void ExecuteMissionUnsolvedDisplay(Case forCase)
	{
		Game.Log("Interface: Mission unsolved display", 2);
		base.StartCoroutine(this.DisplayMissionEndText(InterfaceController.ScreenDisplayType.unsolved, forCase));
	}

	// Token: 0x06001C05 RID: 7173 RVA: 0x00193602 File Offset: 0x00191802
	public void ExecuteResolveDisplay(Case forCase)
	{
		Game.Log("Interface: Mission resolve display", 2);
		base.StartCoroutine(this.DisplayMissionEndText(InterfaceController.ScreenDisplayType.displayResolve, forCase));
	}

	// Token: 0x06001C06 RID: 7174 RVA: 0x0019361E File Offset: 0x0019181E
	public void ExecuteCoverUpFailedDisplay()
	{
		Game.Log("Interface: Cover-up failed display", 2);
		base.StartCoroutine(this.DisplayMissionEndText(InterfaceController.ScreenDisplayType.coverUpFailed, null));
	}

	// Token: 0x06001C07 RID: 7175 RVA: 0x0019363B File Offset: 0x0019183B
	public void ExecuteCoverUpSuccessDisplay()
	{
		Game.Log("Interface: Cover-up success display", 2);
		base.StartCoroutine(this.DisplayMissionEndText(InterfaceController.ScreenDisplayType.coverUpSuccess, null));
	}

	// Token: 0x06001C08 RID: 7176 RVA: 0x00193657 File Offset: 0x00191857
	[Button(null, 0)]
	public void DisplayCreditThresholdForLevel()
	{
		Game.Log(GameplayController.Instance.GetSocialCreditThresholdForLevel(this.debugLevel), 2);
	}

	// Token: 0x06001C09 RID: 7177 RVA: 0x00193674 File Offset: 0x00191874
	private IEnumerator DisplayMissionEndText(InterfaceController.ScreenDisplayType newType, Case forCase = null)
	{
		Game.Log("Displaying " + newType.ToString(), 2);
		if (newType == InterfaceController.ScreenDisplayType.socialCreditLevelUp)
		{
			this.levelUpScreenActive = true;
		}
		while (this.gameSceenDisplayed || this.gameScreenQueued)
		{
			yield return null;
		}
		CanvasRenderer rend = InterfaceControls.Instance.caseSolvedText.canvasRenderer;
		float lastsFor = 5.5f;
		float timer = lastsFor;
		float num = 0f;
		rend.SetAlpha(InterfaceControls.Instance.caseSolvedAlphaAnim.Evaluate(1f - num));
		InterfaceControls.Instance.caseSolvedText.characterSpacing = InterfaceControls.Instance.caseSolvedKerningAnim.Evaluate(num);
		this.gameSceenDisplayed = false;
		bool firstFrame = true;
		this.gameScreenQueued = true;
		while (timer > 0f)
		{
			if (!this.messageCoroutineRunning && this.currentNotification == null && !CutSceneController.Instance.cutSceneActive && Player.Instance.searchInteractable == null && !InteractionController.Instance.dialogMode && SessionData.Instance.play)
			{
				if (firstFrame)
				{
					this.currentGameScreen = newType;
					if (newType == InterfaceController.ScreenDisplayType.missionComplete)
					{
						AudioController.Instance.Play2DSound(AudioControls.Instance.caseComplete, null, 1f);
						InterfaceControls.Instance.caseSolvedText.text = Strings.Get("ui.gamemessage", "Case Completed", Strings.Casing.asIs, false, false, false, null);
						MusicController.Instance.MusicTriggerCheck(MusicCue.MusicTriggerEvent.caseComplete);
					}
					else if (newType == InterfaceController.ScreenDisplayType.coverUpSuccess)
					{
						AudioController.Instance.Play2DSound(AudioControls.Instance.caseComplete, null, 1f);
						InterfaceControls.Instance.caseSolvedText.text = Strings.Get("ui.gamemessage", "CoverUp_Success", Strings.Casing.asIs, false, false, false, null);
					}
					else if (newType == InterfaceController.ScreenDisplayType.missionFailed)
					{
						AudioController.Instance.Play2DSound(AudioControls.Instance.caseUnsolved, null, 1f);
						InterfaceControls.Instance.caseSolvedText.text = "<color=#FF7575>" + Strings.Get("ui.gamemessage", "Job Failed", Strings.Casing.asIs, false, false, false, null);
						MusicController.Instance.MusicTriggerCheck(MusicCue.MusicTriggerEvent.caseFailed);
					}
					else if (newType == InterfaceController.ScreenDisplayType.coverUpFailed)
					{
						AudioController.Instance.Play2DSound(AudioControls.Instance.caseUnsolved, null, 1f);
						InterfaceControls.Instance.caseSolvedText.text = "<color=#FF7575>" + Strings.Get("ui.gamemessage", "CoverUp_Fail", Strings.Casing.asIs, false, false, false, null);
					}
					else if (newType == InterfaceController.ScreenDisplayType.unsolved)
					{
						AudioController.Instance.Play2DSound(AudioControls.Instance.caseUnsolved, null, 1f);
						InterfaceControls.Instance.caseSolvedText.text = "<color=#FF7575>" + Strings.Get("ui.gamemessage", "Unsolved", Strings.Casing.asIs, false, false, false, null);
						MusicController.Instance.MusicTriggerCheck(MusicCue.MusicTriggerEvent.caseUnsolved);
					}
					else if (newType == InterfaceController.ScreenDisplayType.newMurderCase)
					{
						AudioController.Instance.Play2DSound(AudioControls.Instance.newMurderCase, null, 1f);
						InterfaceControls.Instance.caseSolvedText.text = Strings.Get("ui.gamemessage", "New Murder Case", Strings.Casing.asIs, false, false, false, null);
						if (MurderController.Instance.currentActiveCase != null && MurderController.Instance.murderPreset != null && MurderController.Instance.murderPreset.caseType == MurderPreset.CaseType.kidnap)
						{
							InterfaceControls.Instance.caseSolvedText.text = Strings.Get("ui.gamemessage", "New Kidnapping Case", Strings.Casing.asIs, false, false, false, null);
						}
						MusicController.Instance.MusicTriggerCheck(MusicCue.MusicTriggerEvent.newMurderCase);
					}
					else if (newType == InterfaceController.ScreenDisplayType.socialCreditLevelUp)
					{
						AudioController.Instance.Play2DSound(AudioControls.Instance.socialLevelUp, null, 1f);
						InterfaceControls.Instance.caseSolvedText.text = Strings.Get("ui.gamemessage", "Social Credit Level Up", Strings.Casing.asIs, false, false, false, null) + ": " + GameplayController.Instance.GetCurrentSocialCreditLevel().ToString();
						MusicController.Instance.MusicTriggerCheck(MusicCue.MusicTriggerEvent.socialCreditLevelUp);
					}
					else if (newType == InterfaceController.ScreenDisplayType.displayResolve)
					{
						InterfaceControls.Instance.resolveQuestionsDisplayParent.gameObject.SetActive(true);
						AudioController.Instance.Play2DSound(AudioControls.Instance.revealCaseResults, null, 1f);
						if (forCase != null)
						{
							for (int i = 0; i < forCase.resolveQuestions.Count; i++)
							{
								Object.Instantiate<GameObject>(PrefabControls.Instance.revealQuestionObject, InterfaceControls.Instance.resolveQuestionsDisplayParent).GetComponent<RevealResolveController>().Setup(forCase.resolveQuestions[i], forCase, (float)i * 0.5f);
							}
						}
						lastsFor = Mathf.Max((float)forCase.resolveQuestions.Count * 1.5f, 5.5f);
						timer = lastsFor;
						MusicController.Instance.MusicTriggerCheck(MusicCue.MusicTriggerEvent.resolveScreen);
					}
					else if (newType == InterfaceController.ScreenDisplayType.apartmentPurchase)
					{
						AudioController.Instance.Play2DSound(AudioControls.Instance.socialLevelUp, null, 1f);
						InterfaceControls.Instance.caseSolvedText.text = Strings.Get("ui.gamemessage", "New Apartment", Strings.Casing.asIs, false, false, false, null);
					}
					else if (newType == InterfaceController.ScreenDisplayType.gameOver)
					{
						AudioController.Instance.Play2DSound(AudioControls.Instance.caseUnsolved, null, 1f);
						InterfaceControls.Instance.caseSolvedText.text = "<color=#FF7575>" + Strings.Get("ui.gamemessage", "You Died", Strings.Casing.asIs, false, false, false, null);
						MusicController.Instance.MusicTriggerCheck(MusicCue.MusicTriggerEvent.caseFailed);
					}
					if (newType != InterfaceController.ScreenDisplayType.displayResolve)
					{
						InterfaceControls.Instance.caseSolvedText.gameObject.SetActive(true);
					}
					firstFrame = false;
					this.gameSceenDisplayed = true;
					this.gameScreenQueued = false;
				}
				timer -= Time.deltaTime;
				num = (lastsFor - timer) / lastsFor;
				if (num <= 0.4f)
				{
					using (List<CanvasRenderer>.Enumerator enumerator = InterfaceControls.Instance.screenMessageFadeRenderers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CanvasRenderer canvasRenderer = enumerator.Current;
							canvasRenderer.SetAlpha(num / 0.4f);
						}
						goto IL_6DC;
					}
					goto IL_689;
				}
				goto IL_689;
				IL_6DC:
				if (newType != InterfaceController.ScreenDisplayType.displayResolve)
				{
					rend.SetAlpha(InterfaceControls.Instance.caseSolvedAlphaAnim.Evaluate(1f - num));
					InterfaceControls.Instance.caseSolvedText.characterSpacing = InterfaceControls.Instance.caseSolvedKerningAnim.Evaluate(num);
					goto IL_732;
				}
				goto IL_732;
				IL_689:
				if (num >= 0.8f)
				{
					foreach (CanvasRenderer canvasRenderer2 in InterfaceControls.Instance.screenMessageFadeRenderers)
					{
						canvasRenderer2.SetAlpha(1f - (num - 0.8f) / 0.2f);
					}
					goto IL_6DC;
				}
				goto IL_6DC;
			}
			else
			{
				bool collectDebugData = Game.Instance.collectDebugData;
			}
			IL_732:
			yield return null;
		}
		InterfaceControls.Instance.caseSolvedText.gameObject.SetActive(false);
		InterfaceControls.Instance.resolveQuestionsDisplayParent.gameObject.SetActive(false);
		if (newType == InterfaceController.ScreenDisplayType.missionComplete)
		{
			this.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, Strings.Get("ui.gamemessage", "Case Closed", Strings.Casing.upperCase, false, false, false, null), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
		else if (newType == InterfaceController.ScreenDisplayType.unsolved)
		{
			this.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, Strings.Get("ui.gamemessage", "Case Unsolved", Strings.Casing.upperCase, false, false, false, null), InterfaceControls.Icon.agent, null, true, Color.red, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
		else if (newType == InterfaceController.ScreenDisplayType.newMurderCase)
		{
			this.NewGameMessage(InterfaceController.GameMessageType.gameHeader, 0, Strings.Get("ui.gamemessage", "Case Opened", Strings.Casing.upperCase, false, false, false, null), InterfaceControls.Icon.agent, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
		foreach (CanvasRenderer canvasRenderer3 in InterfaceControls.Instance.screenMessageFadeRenderers)
		{
			canvasRenderer3.SetAlpha(0f);
		}
		this.gameSceenDisplayed = false;
		this.gameScreenQueued = false;
		if (newType == InterfaceController.ScreenDisplayType.socialCreditLevelUp)
		{
			this.levelUpScreenActive = false;
		}
		yield break;
	}

	// Token: 0x06001C0A RID: 7178 RVA: 0x00193694 File Offset: 0x00191894
	public bool StupidUnityChangeToTheWayOnPointerExitHandles(PointerEventData eventData, Transform t)
	{
		return eventData == null || !(eventData.pointerCurrentRaycast.gameObject != null) || !eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(t);
	}

	// Token: 0x0400247F RID: 9343
	[Header("Canvases")]
	public Canvas caseCanvas;

	// Token: 0x04002480 RID: 9344
	public CanvasGroup caseCanvasGroup;

	// Token: 0x04002481 RID: 9345
	public GraphicRaycaster caseCanvasRaycaster;

	// Token: 0x04002482 RID: 9346
	public Canvas minimapCanvas;

	// Token: 0x04002483 RID: 9347
	public CanvasGroup minimapCanvasGroup;

	// Token: 0x04002484 RID: 9348
	public Canvas controlsCanvas;

	// Token: 0x04002485 RID: 9349
	public CanvasGroup controlsCanvasGroup;

	// Token: 0x04002486 RID: 9350
	public Canvas controlPanelCanvas;

	// Token: 0x04002487 RID: 9351
	public CanvasGroup controlPanelCanvasGroup;

	// Token: 0x04002488 RID: 9352
	public Canvas gameWorldCanvas;

	// Token: 0x04002489 RID: 9353
	public CanvasGroup gameWorldCanvasGroup;

	// Token: 0x0400248A RID: 9354
	public Canvas windowCanvas;

	// Token: 0x0400248B RID: 9355
	public GraphicRaycaster windowRaycaster;

	// Token: 0x0400248C RID: 9356
	public CanvasGroup windowCanvasGroup;

	// Token: 0x0400248D RID: 9357
	public Canvas statusCanvas;

	// Token: 0x0400248E RID: 9358
	public CanvasGroup statusCanvasGroup;

	// Token: 0x0400248F RID: 9359
	public Canvas upgradesCanvas;

	// Token: 0x04002490 RID: 9360
	public CanvasGroup upgradesCanvasGroup;

	// Token: 0x04002491 RID: 9361
	public Canvas dialogCanvas;

	// Token: 0x04002492 RID: 9362
	public CanvasGroup dialogCanvasGroup;

	// Token: 0x04002493 RID: 9363
	public Canvas interactionProgressCanvas;

	// Token: 0x04002494 RID: 9364
	public CanvasGroup interactionProgressCanvasGroup;

	// Token: 0x04002495 RID: 9365
	[Header("UI Scaling Transforms")]
	[ReorderableList]
	public List<RectTransform> uiScaling = new List<RectTransform>();

	// Token: 0x04002496 RID: 9366
	[Header("References")]
	public ButtonController notebookButton;

	// Token: 0x04002497 RID: 9367
	public ButtonController upgradesButton;

	// Token: 0x04002498 RID: 9368
	public ButtonController mapButton;

	// Token: 0x04002499 RID: 9369
	public ButtonController personButton;

	// Token: 0x0400249A RID: 9370
	public RectTransform firstPersonUI;

	// Token: 0x0400249B RID: 9371
	public RectTransform caseReferenceAnchor;

	// Token: 0x0400249C RID: 9372
	public GameObject backgroundBlur;

	// Token: 0x0400249D RID: 9373
	public RectTransform speechDisplayAnchor;

	// Token: 0x0400249E RID: 9374
	public RectTransform objectiveSideAnchor;

	// Token: 0x0400249F RID: 9375
	public RectTransform objectiveTextBackground;

	// Token: 0x040024A0 RID: 9376
	public TextMeshProUGUI objectiveTitleText;

	// Token: 0x040024A1 RID: 9377
	public CanvasRenderer objectiveTitleTextRenderer;

	// Token: 0x040024A2 RID: 9378
	public CanvasRenderer objectiveBackgroundRenderer;

	// Token: 0x040024A3 RID: 9379
	public RectTransform uiPointerContainer;

	// Token: 0x040024A4 RID: 9380
	public Image takeDamageIndicatorImg;

	// Token: 0x040024A5 RID: 9381
	public JuiceController takeDamageIndicatorJuice;

	// Token: 0x040024A6 RID: 9382
	public Image lowHealthIndicatorImg;

	// Token: 0x040024A7 RID: 9383
	public RectTransform movieBarTop;

	// Token: 0x040024A8 RID: 9384
	public RectTransform movieBarBottom;

	// Token: 0x040024A9 RID: 9385
	public JuiceController movieBarJuice;

	// Token: 0x040024AA RID: 9386
	public TextMeshProUGUI timeText;

	// Token: 0x040024AB RID: 9387
	public RectTransform speechAnchor;

	// Token: 0x040024AC RID: 9388
	public RectTransform objectCycleAnchor;

	// Token: 0x040024AD RID: 9389
	public TextMeshProUGUI timerText;

	// Token: 0x040024AE RID: 9390
	public ControllerViewRectScroll caseScrollingViewRect;

	// Token: 0x040024AF RID: 9391
	[Space(7f)]
	public SoundIndicatorController footstepAudioIndicator;

	// Token: 0x040024B0 RID: 9392
	[Header("States")]
	public bool desktopMode;

	// Token: 0x040024B1 RID: 9393
	public float desktopModeTransition;

	// Token: 0x040024B2 RID: 9394
	public float desktopModeDesiredTransition;

	// Token: 0x040024B3 RID: 9395
	public bool showDesktopMap;

	// Token: 0x040024B4 RID: 9396
	public bool showDesktopCaseBoard;

	// Token: 0x040024B5 RID: 9397
	public ButtonController selectedElement;

	// Token: 0x040024B6 RID: 9398
	public string selectedElementTag;

	// Token: 0x040024B7 RID: 9399
	public List<MonoBehaviour> currentMouseOverElement = new List<MonoBehaviour>();

	// Token: 0x040024B8 RID: 9400
	private InfoWindow detectiveNotebook;

	// Token: 0x040024B9 RID: 9401
	public bool crosshairVisible = true;

	// Token: 0x040024BA RID: 9402
	public bool playerTextInputActive;

	// Token: 0x040024BB RID: 9403
	public List<SpeechBubbleController> activeSpeechBubbles = new List<SpeechBubbleController>();

	// Token: 0x040024BC RID: 9404
	public bool interfaceIsActive = true;

	// Token: 0x040024BD RID: 9405
	public static int assignStickyNoteID = 1;

	// Token: 0x040024BE RID: 9406
	[Header("Location Text")]
	public TextMeshProUGUI locationText;

	// Token: 0x040024BF RID: 9407
	private Coroutine displayedTextCoroutine;

	// Token: 0x040024C0 RID: 9408
	public bool locationTextDisplayed;

	// Token: 0x040024C1 RID: 9409
	[Header("In-Game Title Text")]
	public TextMeshProUGUI titleText;

	// Token: 0x040024C2 RID: 9410
	public CanvasRenderer titleTextRenderer;

	// Token: 0x040024C3 RID: 9411
	[Header("Game Message System")]
	public RectTransform gameMessageParent;

	// Token: 0x040024C4 RID: 9412
	public bool messageCoroutineRunning;

	// Token: 0x040024C5 RID: 9413
	public List<InterfaceController.GameMessage> notificationQueue = new List<InterfaceController.GameMessage>();

	// Token: 0x040024C6 RID: 9414
	public List<InterfaceController.GameMessage> gameHeaderQueue = new List<InterfaceController.GameMessage>();

	// Token: 0x040024C7 RID: 9415
	public List<InterfaceController.GameMessage> helpPointerQueue = new List<InterfaceController.GameMessage>();

	// Token: 0x040024C8 RID: 9416
	public GameObject currentNotification;

	// Token: 0x040024C9 RID: 9417
	public InterfaceController.GameMessage currentGameHeader;

	// Token: 0x040024CA RID: 9418
	private float gameHeaderDelay;

	// Token: 0x040024CB RID: 9419
	public float gameHeaderTimer;

	// Token: 0x040024CC RID: 9420
	private float typewriterDelay;

	// Token: 0x040024CD RID: 9421
	private float gameHeaderFadeDelay;

	// Token: 0x040024CE RID: 9422
	public bool gameHeaderDisplayed;

	// Token: 0x040024CF RID: 9423
	public bool gameSceenDisplayed;

	// Token: 0x040024D0 RID: 9424
	public bool gameScreenQueued;

	// Token: 0x040024D1 RID: 9425
	public bool levelUpScreenActive;

	// Token: 0x040024D2 RID: 9426
	public InterfaceController.ScreenDisplayType currentGameScreen = InterfaceController.ScreenDisplayType.displayResolve;

	// Token: 0x040024D3 RID: 9427
	[Space(7f)]
	public RectTransform notebookNotificationIcon;

	// Token: 0x040024D4 RID: 9428
	public JuiceController notebookNotificationJuice;

	// Token: 0x040024D5 RID: 9429
	public RectTransform syncDiskNotificationIcon;

	// Token: 0x040024D6 RID: 9430
	public JuiceController syncDiskNotificationJuice;

	// Token: 0x040024D7 RID: 9431
	public RectTransform lockpicksNotificationIcon;

	// Token: 0x040024D8 RID: 9432
	public TextMeshProUGUI lockpicksNotificationText;

	// Token: 0x040024D9 RID: 9433
	public JuiceController lockpicksNotificationJuice;

	// Token: 0x040024DA RID: 9434
	public List<CanvasRenderer> lockpicksNotificationRenderers;

	// Token: 0x040024DB RID: 9435
	public bool lockpickNotificationActive;

	// Token: 0x040024DC RID: 9436
	public int lastLockpicks;

	// Token: 0x040024DD RID: 9437
	public RectTransform moneyNotificationIcon;

	// Token: 0x040024DE RID: 9438
	public TextMeshProUGUI moneyNotificationText;

	// Token: 0x040024DF RID: 9439
	public JuiceController moneyNotificationJuice;

	// Token: 0x040024E0 RID: 9440
	public List<CanvasRenderer> moneyNotificationRenderers;

	// Token: 0x040024E1 RID: 9441
	public bool moneyNotificationActive;

	// Token: 0x040024E2 RID: 9442
	public int lastMoney;

	// Token: 0x040024E3 RID: 9443
	public RectTransform bioNotificationIcon;

	// Token: 0x040024E4 RID: 9444
	[Space(7f)]
	private InterfaceController.GameMessage currentHelpPointer;

	// Token: 0x040024E5 RID: 9445
	public RectTransform helpPointerRect;

	// Token: 0x040024E6 RID: 9446
	public List<CanvasRenderer> helpPointerRenderers = new List<CanvasRenderer>();

	// Token: 0x040024E7 RID: 9447
	public TextMeshProUGUI helpPointerText;

	// Token: 0x040024E8 RID: 9448
	private string helpPointerTextDisplay;

	// Token: 0x040024E9 RID: 9449
	private float helpPointerProgress = 1f;

	// Token: 0x040024EA RID: 9450
	private float helpPointerFadeOut = 1f;

	// Token: 0x040024EB RID: 9451
	private float helpPointerTimer;

	// Token: 0x040024EC RID: 9452
	private float helpPointerDesiredHeight = 44f;

	// Token: 0x040024ED RID: 9453
	[Header("Objective System")]
	[NonSerialized]
	public Objective currentlyDisplaying;

	// Token: 0x040024EE RID: 9454
	public List<Objective> displayedObjectives = new List<Objective>();

	// Token: 0x040024EF RID: 9455
	public List<ChecklistButtonController> objectiveList = new List<ChecklistButtonController>();

	// Token: 0x040024F0 RID: 9456
	[Header("Radial Selection")]
	public RectTransform selectionArrowPivot;

	// Token: 0x040024F1 RID: 9457
	public List<CanvasRenderer> radialRenderers = new List<CanvasRenderer>();

	// Token: 0x040024F2 RID: 9458
	public AnimationCurve radialActivateScale;

	// Token: 0x040024F3 RID: 9459
	public AnimationCurve radialDeactivateScale;

	// Token: 0x040024F4 RID: 9460
	public AnimationCurve radialActivateRotate;

	// Token: 0x040024F5 RID: 9461
	public AnimationCurve radialDeactivateRotate;

	// Token: 0x040024F6 RID: 9462
	[Header("Dragging")]
	public GameObject dragged;

	// Token: 0x040024F7 RID: 9463
	public string draggedTag = string.Empty;

	// Token: 0x040024F8 RID: 9464
	public Vector2 dragCursorOffset;

	// Token: 0x040024F9 RID: 9465
	public PinnedItemController pinnedBeingDragged;

	// Token: 0x040024FA RID: 9466
	public float windowFadeProgress;

	// Token: 0x040024FB RID: 9467
	public bool windowFullFade;

	// Token: 0x040024FC RID: 9468
	[Header("Display Objectives")]
	public float objectivesDisplayTimer;

	// Token: 0x040024FD RID: 9469
	public float objectivesAlpha;

	// Token: 0x040024FE RID: 9470
	[Header("Box Select")]
	public bool boxSelectActive;

	// Token: 0x040024FF RID: 9471
	public RectTransform boxSelect;

	// Token: 0x04002500 RID: 9472
	public List<PinnedItemController> selectedPinned = new List<PinnedItemController>();

	// Token: 0x04002501 RID: 9473
	[Header("Active Windows")]
	public Dictionary<string, WindowStylePreset> windowDictionary = new Dictionary<string, WindowStylePreset>();

	// Token: 0x04002502 RID: 9474
	public List<InfoWindow> activeWindows = new List<InfoWindow>();

	// Token: 0x04002503 RID: 9475
	public string openHelpToPage = "";

	// Token: 0x04002504 RID: 9476
	public RectTransform windowFocus;

	// Token: 0x04002505 RID: 9477
	public KeypadController activeCodeInput;

	// Token: 0x04002506 RID: 9478
	[Header("Game Fading")]
	public CanvasRenderer fadeOverlay;

	// Token: 0x04002507 RID: 9479
	public AnimationCurve fadeOverlayAlphaCurve;

	// Token: 0x04002508 RID: 9480
	public float desiredFade;

	// Token: 0x04002509 RID: 9481
	private float fadeTime = 2f;

	// Token: 0x0400250A RID: 9482
	private bool fadeAudio;

	// Token: 0x0400250B RID: 9483
	public float fade;

	// Token: 0x0400250C RID: 9484
	[Header("Pause Rendering")]
	private CameraClearFlags savedCameraClear;

	// Token: 0x0400250D RID: 9485
	[Header("Awareness Compass")]
	public GameObject compassContainer;

	// Token: 0x0400250E RID: 9486
	public Transform backgroundTransform;

	// Token: 0x0400250F RID: 9487
	public MeshRenderer compassMeshRend;

	// Token: 0x04002510 RID: 9488
	public float compassDesiredAlpha;

	// Token: 0x04002511 RID: 9489
	public float compassActualAlpha;

	// Token: 0x04002512 RID: 9490
	public List<InterfaceController.AwarenessIcon> awarenessIcons = new List<InterfaceController.AwarenessIcon>();

	// Token: 0x04002513 RID: 9491
	public List<SpeechBubbleController> anchoredSpeech = new List<SpeechBubbleController>();

	// Token: 0x04002514 RID: 9492
	[Header("First Person Item")]
	public GameObject firstPersonModel;

	// Token: 0x04002515 RID: 9493
	public Animator firstPersonAnimator;

	// Token: 0x04002516 RID: 9494
	[Header("Depth of Field")]
	public float desiredDofNearStart;

	// Token: 0x04002517 RID: 9495
	public float desiredDofNearEnd;

	// Token: 0x04002518 RID: 9496
	public float desiredDofFarStart;

	// Token: 0x04002519 RID: 9497
	public float desiredDofFarEnd;

	// Token: 0x0400251A RID: 9498
	public float dofProgress;

	// Token: 0x0400251B RID: 9499
	[Header("Popup Messages")]
	public PopupMessageController popupController;

	// Token: 0x0400251C RID: 9500
	[Header("Debug")]
	public int debugLevel;

	// Token: 0x0400251F RID: 9503
	private static InterfaceController _instance;

	// Token: 0x02000507 RID: 1287
	public enum GameMessageType
	{
		// Token: 0x04002521 RID: 9505
		notification,
		// Token: 0x04002522 RID: 9506
		gameHeader,
		// Token: 0x04002523 RID: 9507
		keyMerge,
		// Token: 0x04002524 RID: 9508
		helpPointer,
		// Token: 0x04002525 RID: 9509
		socialCredit
	}

	// Token: 0x02000508 RID: 1288
	public class GameMessage
	{
		// Token: 0x04002526 RID: 9510
		public InterfaceController.GameMessageType messageType;

		// Token: 0x04002527 RID: 9511
		public int numerical;

		// Token: 0x04002528 RID: 9512
		public string message;

		// Token: 0x04002529 RID: 9513
		public Sprite graphic;

		// Token: 0x0400252A RID: 9514
		public AudioEvent additionalSFX;

		// Token: 0x0400252B RID: 9515
		public bool colourOverride;

		// Token: 0x0400252C RID: 9516
		public Color col;

		// Token: 0x0400252D RID: 9517
		public int mergeType;

		// Token: 0x0400252E RID: 9518
		public float delay;

		// Token: 0x0400252F RID: 9519
		public RectTransform moveOnDestroy;

		// Token: 0x04002530 RID: 9520
		public GameMessageController.PingOnComplete ping;

		// Token: 0x04002531 RID: 9521
		public bool keyMerge;

		// Token: 0x04002532 RID: 9522
		public bool socCredit;

		// Token: 0x04002533 RID: 9523
		public Evidence keyMergeEvidence;

		// Token: 0x04002534 RID: 9524
		public List<Evidence.DataKey> mergedKeys;
	}

	// Token: 0x02000509 RID: 1289
	public enum AwarenessType
	{
		// Token: 0x04002536 RID: 9526
		actor,
		// Token: 0x04002537 RID: 9527
		transform,
		// Token: 0x04002538 RID: 9528
		position
	}

	// Token: 0x0200050A RID: 1290
	public enum AwarenessBehaviour
	{
		// Token: 0x0400253A RID: 9530
		alwaysVisible,
		// Token: 0x0400253B RID: 9531
		invisibleInfront
	}

	// Token: 0x0200050B RID: 1291
	[Serializable]
	public class AwarenessIcon
	{
		// Token: 0x06001C0E RID: 7182 RVA: 0x001937F0 File Offset: 0x001919F0
		public void Remove(bool instant = false)
		{
			this.priority = -99;
			if (instant)
			{
				Object.Destroy(this.spawned);
				InterfaceController.Instance.awarenessIcons.Remove(this);
			}
			this.removalFlag = true;
			InterfaceController.Instance.awarenessIcons.Sort((InterfaceController.AwarenessIcon p1, InterfaceController.AwarenessIcon p2) => p2.priority.CompareTo(p1.priority));
		}

		// Token: 0x06001C0F RID: 7183 RVA: 0x0019385C File Offset: 0x00191A5C
		public void SetAlpha(float val)
		{
			float num = 1f - this.removalProgress;
			Color color = this.imageMaterial.GetColor("_UnlitColor");
			this.imageMaterial.SetColor("_UnlitColor", new Color(color.r, color.g, color.b, val * this.displayAlpha * num));
			this.arrowMaterial.SetColor("_UnlitColor", new Color(color.r, color.g, color.b, val * this.displayAlpha * num));
			this.alpha = val;
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x001938F0 File Offset: 0x00191AF0
		public void SetTexture(Texture tex)
		{
			this.overrideTexture = tex;
			if (this.imageMaterial != null)
			{
				this.imageMaterial.SetTexture("_UnlitColorMap", tex);
			}
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x00193918 File Offset: 0x00191B18
		public float GetActualAlpha()
		{
			float num = 1f - this.removalProgress;
			return this.alpha * this.displayAlpha * num;
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x00193941 File Offset: 0x00191B41
		public void TriggerAlert()
		{
			this.triggerAlert = true;
		}

		// Token: 0x0400253C RID: 9532
		public InterfaceController.AwarenessType awarenessType;

		// Token: 0x0400253D RID: 9533
		public InterfaceController.AwarenessBehaviour awarenessBehaviour;

		// Token: 0x0400253E RID: 9534
		public Actor actor;

		// Token: 0x0400253F RID: 9535
		public Transform targetTransform;

		// Token: 0x04002540 RID: 9536
		public Vector3 targetPosition;

		// Token: 0x04002541 RID: 9537
		public GameObject spawned;

		// Token: 0x04002542 RID: 9538
		public Transform imageTransform;

		// Token: 0x04002543 RID: 9539
		public Material imageMaterial;

		// Token: 0x04002544 RID: 9540
		public Transform arrowTransform;

		// Token: 0x04002545 RID: 9541
		public Material arrowMaterial;

		// Token: 0x04002546 RID: 9542
		public Texture overrideTexture;

		// Token: 0x04002547 RID: 9543
		public float fadeIn;

		// Token: 0x04002548 RID: 9544
		public float springAction;

		// Token: 0x04002549 RID: 9545
		public float removalProgress;

		// Token: 0x0400254A RID: 9546
		public bool removalFlag;

		// Token: 0x0400254B RID: 9547
		public float alpha;

		// Token: 0x0400254C RID: 9548
		public float displayAlpha;

		// Token: 0x0400254D RID: 9549
		public float maxDistance = 20f;

		// Token: 0x0400254E RID: 9550
		public bool setup;

		// Token: 0x0400254F RID: 9551
		public int priority;

		// Token: 0x04002550 RID: 9552
		public bool triggerAlert;

		// Token: 0x04002551 RID: 9553
		public float alertProgress;
	}

	// Token: 0x0200050D RID: 1293
	public enum ScreenDisplayType
	{
		// Token: 0x04002555 RID: 9557
		missionComplete,
		// Token: 0x04002556 RID: 9558
		missionFailed,
		// Token: 0x04002557 RID: 9559
		newMurderCase,
		// Token: 0x04002558 RID: 9560
		socialCreditLevelUp,
		// Token: 0x04002559 RID: 9561
		unsolved,
		// Token: 0x0400255A RID: 9562
		displayResolve,
		// Token: 0x0400255B RID: 9563
		apartmentPurchase,
		// Token: 0x0400255C RID: 9564
		gameOver,
		// Token: 0x0400255D RID: 9565
		coverUpSuccess,
		// Token: 0x0400255E RID: 9566
		coverUpFailed
	}

	// Token: 0x0200050E RID: 1294
	// (Invoke) Token: 0x06001C18 RID: 7192
	public delegate void InputCode(List<int> code);

	// Token: 0x0200050F RID: 1295
	// (Invoke) Token: 0x06001C1C RID: 7196
	public delegate void NewActiveCodeInput(KeypadController keypad);
}
