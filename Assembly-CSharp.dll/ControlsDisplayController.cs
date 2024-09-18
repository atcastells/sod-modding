using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

// Token: 0x020002DD RID: 733
public class ControlsDisplayController : MonoBehaviour
{
	// Token: 0x17000083 RID: 131
	// (get) Token: 0x060010A5 RID: 4261 RVA: 0x000EBBED File Offset: 0x000E9DED
	public static ControlsDisplayController Instance
	{
		get
		{
			return ControlsDisplayController._instance;
		}
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x000EBBF4 File Offset: 0x000E9DF4
	private void Awake()
	{
		if (ControlsDisplayController._instance != null && ControlsDisplayController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		ControlsDisplayController._instance = this;
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x000EBC24 File Offset: 0x000E9E24
	public void UpdateControlDisplay()
	{
		List<InteractablePreset.InteractionKey> list = new List<InteractablePreset.InteractionKey>();
		List<InteractionController.InteractionSetting> list2 = new List<InteractionController.InteractionSetting>();
		foreach (KeyValuePair<InteractablePreset.InteractionKey, InteractionController.InteractionSetting> keyValuePair in InteractionController.Instance.currentInteractions)
		{
			if (keyValuePair.Key != InteractablePreset.InteractionKey.none && (keyValuePair.Value.currentAction == null || !(keyValuePair.Value.currentAction.action != null) || !keyValuePair.Value.currentAction.action.disableUIDisplay))
			{
				bool flag = true;
				ControlDisplayController.ControlPositioning controlPositioning;
				ControlsDisplayController.Instance.GetControlIcon(keyValuePair.Key, out controlPositioning, out flag);
				if (flag && keyValuePair.Value.currentSetting != null && keyValuePair.Value.currentSetting.enabled && keyValuePair.Value.currentSetting.display)
				{
					list.Add(keyValuePair.Key);
					list2.Add(keyValuePair.Value);
				}
			}
		}
		for (int i = 0; i < this.spawned.Count; i++)
		{
			ControlDisplayController controlDisplayController = this.spawned[i];
			int num = list.IndexOf(controlDisplayController.key);
			if (num > -1)
			{
				controlDisplayController.UpdateDisplay(controlDisplayController.key, list2[num]);
				list.RemoveAt(num);
				list2.RemoveAt(num);
			}
			else
			{
				controlDisplayController.Remove();
				i--;
			}
		}
		while (list.Count > 0)
		{
			ControlDisplayController component = Object.Instantiate<GameObject>(this.controlDisplayPrefab, this.anchor).GetComponent<ControlDisplayController>();
			foreach (CanvasRenderer canvasRenderer in component.renderers)
			{
				canvasRenderer.SetAlpha(0f);
			}
			component.UpdateDisplay(list[0], list2[0]);
			this.spawned.Add(component);
			list.RemoveAt(0);
			list2.RemoveAt(0);
		}
		Dictionary<ControlDisplayController.ControlPositioning, List<ControlDisplayController>> dictionary = new Dictionary<ControlDisplayController.ControlPositioning, List<ControlDisplayController>>();
		foreach (ControlDisplayController controlDisplayController2 in this.spawned)
		{
			if (!dictionary.ContainsKey(controlDisplayController2.positioning))
			{
				dictionary.Add(controlDisplayController2.positioning, new List<ControlDisplayController>());
			}
			dictionary[controlDisplayController2.positioning].Add(controlDisplayController2);
		}
		foreach (KeyValuePair<ControlDisplayController.ControlPositioning, List<ControlDisplayController>> keyValuePair2 in dictionary)
		{
			keyValuePair2.Value.Sort((ControlDisplayController p2, ControlDisplayController p1) => p1.interactionSetting.priority.CompareTo(p2.interactionSetting.priority));
		}
		List<ControlDisplayController> list3 = new List<ControlDisplayController>();
		List<ControlDisplayController> list4 = new List<ControlDisplayController>();
		List<ControlDisplayController> list5 = new List<ControlDisplayController>();
		List<ControlDisplayController> list6 = new List<ControlDisplayController>();
		if (dictionary.ContainsKey(ControlDisplayController.ControlPositioning.up))
		{
			list3.AddRange(dictionary[ControlDisplayController.ControlPositioning.up]);
		}
		if (dictionary.ContainsKey(ControlDisplayController.ControlPositioning.down))
		{
			list6.AddRange(dictionary[ControlDisplayController.ControlPositioning.down]);
		}
		if (dictionary.ContainsKey(ControlDisplayController.ControlPositioning.left))
		{
			list4.InsertRange(0, dictionary[ControlDisplayController.ControlPositioning.left]);
		}
		if (dictionary.ContainsKey(ControlDisplayController.ControlPositioning.right))
		{
			list4.AddRange(dictionary[ControlDisplayController.ControlPositioning.right]);
		}
		if (dictionary.ContainsKey(ControlDisplayController.ControlPositioning.neutral))
		{
			list5.AddRange(dictionary[ControlDisplayController.ControlPositioning.neutral]);
		}
		Vector2 zero = Vector2.zero;
		if (list6.Count > 0)
		{
			float num2 = Mathf.Min(this.rect.rect.width / (float)list6.Count - (float)(list6.Count - 1) * this.padding.x, 330f);
			float num3 = num2 * (float)list6.Count + (float)(list6.Count - 1) * this.padding.x;
			zero.x = num3 * -0.5f + num2 * 0.5f;
			for (int j = 0; j < list6.Count; j++)
			{
				ControlDisplayController controlDisplayController3 = list6[j];
				controlDisplayController3.desiredPosition = new Vector2(zero.x, zero.y);
				controlDisplayController3.rect.sizeDelta = new Vector2(num2, controlDisplayController3.rect.sizeDelta.y);
				zero.x += num2 + this.padding.x;
				if (!controlDisplayController3.assignedSpawnPosition)
				{
					controlDisplayController3.spawnPosition = controlDisplayController3.desiredPosition + new Vector2(64f, 0f);
					if (controlDisplayController3.desiredPosition.x < 0f)
					{
						controlDisplayController3.spawnPosition = controlDisplayController3.desiredPosition - new Vector2(64f, 0f);
					}
					controlDisplayController3.rect.anchoredPosition = controlDisplayController3.spawnPosition;
					controlDisplayController3.assignedSpawnPosition = true;
				}
			}
			zero.y += 56f + this.padding.y;
		}
		if (list5.Count > 0)
		{
			float num4 = Mathf.Min(this.rect.rect.width / (float)list5.Count - (float)(list5.Count - 1) * this.padding.x, 330f);
			float num5 = num4 * (float)list5.Count + (float)(list5.Count - 1) * this.padding.x;
			zero.x = num5 * -0.5f + num4 * 0.5f;
			for (int k = 0; k < list5.Count; k++)
			{
				ControlDisplayController controlDisplayController4 = list5[k];
				controlDisplayController4.desiredPosition = new Vector2(zero.x, zero.y);
				controlDisplayController4.rect.sizeDelta = new Vector2(num4, controlDisplayController4.rect.sizeDelta.y);
				zero.x += num4 + this.padding.x;
				if (!controlDisplayController4.assignedSpawnPosition)
				{
					controlDisplayController4.spawnPosition = controlDisplayController4.desiredPosition + new Vector2(64f, 0f);
					if (controlDisplayController4.desiredPosition.x < 0f)
					{
						controlDisplayController4.spawnPosition = controlDisplayController4.desiredPosition - new Vector2(64f, 0f);
					}
					controlDisplayController4.rect.anchoredPosition = controlDisplayController4.spawnPosition;
					controlDisplayController4.assignedSpawnPosition = true;
				}
			}
			zero.y += 56f + this.padding.y;
		}
		if (list4.Count > 0)
		{
			float num6 = Mathf.Min(this.rect.rect.width / (float)list4.Count - (float)(list4.Count - 1) * this.padding.x, 330f);
			float num7 = num6 * (float)list4.Count + (float)(list4.Count - 1) * this.padding.x;
			zero.x = num7 * -0.5f + num6 * 0.5f;
			for (int l = 0; l < list4.Count; l++)
			{
				ControlDisplayController controlDisplayController5 = list4[l];
				controlDisplayController5.desiredPosition = new Vector2(zero.x, zero.y);
				controlDisplayController5.rect.sizeDelta = new Vector2(num6, controlDisplayController5.rect.sizeDelta.y);
				zero.x += num6 + this.padding.x;
				if (!controlDisplayController5.assignedSpawnPosition)
				{
					controlDisplayController5.spawnPosition = controlDisplayController5.desiredPosition + new Vector2(64f, 0f);
					if (controlDisplayController5.desiredPosition.x < 0f)
					{
						controlDisplayController5.spawnPosition = controlDisplayController5.desiredPosition - new Vector2(64f, 0f);
					}
					controlDisplayController5.rect.anchoredPosition = controlDisplayController5.spawnPosition;
					controlDisplayController5.assignedSpawnPosition = true;
				}
			}
			zero.y += 56f + this.padding.y;
		}
		if (list3.Count > 0)
		{
			float num8 = Mathf.Min(this.rect.rect.width / (float)list3.Count - (float)(list3.Count - 1) * this.padding.x, 330f);
			float num9 = num8 * (float)list3.Count + (float)(list3.Count - 1) * this.padding.x;
			zero.x = num9 * -0.5f + num8 * 0.5f;
			for (int m = 0; m < list3.Count; m++)
			{
				ControlDisplayController controlDisplayController6 = list3[m];
				controlDisplayController6.desiredPosition = new Vector2(zero.x, zero.y);
				controlDisplayController6.rect.sizeDelta = new Vector2(num8, controlDisplayController6.rect.sizeDelta.y);
				zero.x += num8 + this.padding.x;
				if (!controlDisplayController6.assignedSpawnPosition)
				{
					controlDisplayController6.spawnPosition = controlDisplayController6.desiredPosition + new Vector2(64f, 0f);
					if (controlDisplayController6.desiredPosition.x < 0f)
					{
						controlDisplayController6.spawnPosition = controlDisplayController6.desiredPosition - new Vector2(64f, 0f);
					}
					controlDisplayController6.rect.anchoredPosition = controlDisplayController6.spawnPosition;
					controlDisplayController6.assignedSpawnPosition = true;
				}
			}
			zero.y += 56f + this.padding.y;
		}
		if (this.spawned.Count > 0)
		{
			base.enabled = true;
		}
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x000EC648 File Offset: 0x000EA848
	private void Update()
	{
		foreach (ControlDisplayController controlDisplayController in this.spawned)
		{
			if (!(controlDisplayController == null) && controlDisplayController.rect.anchoredPosition != controlDisplayController.desiredPosition)
			{
				controlDisplayController.rect.anchoredPosition = Vector2.Lerp(controlDisplayController.rect.anchoredPosition, controlDisplayController.desiredPosition, Time.deltaTime / 0.1f);
			}
		}
		if (this.posChangeProgress < 1f)
		{
			this.posChangeProgress += Time.deltaTime / 0.1f;
			this.posChangeProgress = Mathf.Clamp01(this.posChangeProgress);
			this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, Mathf.Lerp(this.rect.anchoredPosition.y, this.desiredYPos, this.posChangeProgress));
			this.rect.sizeDelta = new Vector2(this.rect.sizeDelta.x, Mathf.Lerp(this.rect.sizeDelta.y, this.desiredHeight, this.posChangeProgress));
			this.rect.offsetMin = new Vector2(Mathf.Lerp(this.rect.offsetMin.x, this.desiredRectFromLeft, this.posChangeProgress), this.rect.offsetMin.y);
			this.rect.offsetMax = new Vector2(Mathf.Lerp(this.rect.offsetMax.x, -this.desiredRectFromRight, this.posChangeProgress), this.rect.offsetMax.y);
		}
		if (this.spawned.Count <= 0 && this.posChangeProgress >= 1f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x000EC844 File Offset: 0x000EAA44
	public void RestoreDefaultDisplayArea()
	{
		Game.Log("Interface: Restore default control display area...", 2);
		this.SetControlDisplayArea(144f, 220f, 300f, 300f);
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x000EC86B File Offset: 0x000EAA6B
	public void SetControlDisplayArea(float yPos, float height, float rectFromLeft, float rectFromRight)
	{
		this.desiredYPos = yPos;
		this.desiredHeight = height;
		this.desiredRectFromLeft = rectFromLeft;
		this.desiredRectFromRight = rectFromRight;
		this.posChangeProgress = 0f;
		base.enabled = true;
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x000EC89C File Offset: 0x000EAA9C
	public void DisplayControlIconAfterDelay(float afterSeconds, InteractablePreset.InteractionKey key, string interactionName, float forTime, bool overrideMinDisplayTime = false)
	{
		base.StartCoroutine(this.DisplayControlIconAfter(afterSeconds, key, interactionName, forTime, overrideMinDisplayTime));
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x000EC8B2 File Offset: 0x000EAAB2
	private IEnumerator DisplayControlIconAfter(float afterSeconds, InteractablePreset.InteractionKey key, string interactionName, float forTime, bool overrideMinDisplayTime)
	{
		while (afterSeconds > 0f)
		{
			afterSeconds -= Time.deltaTime;
			yield return null;
		}
		this.DisplayControlIcon(key, interactionName, forTime, overrideMinDisplayTime);
		yield break;
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x000EC8E8 File Offset: 0x000EAAE8
	public void DisplayControlIcon(InteractablePreset.InteractionKey key, string interactionName, float forTime, bool overrideMinDisplayTime = false)
	{
		if (!SessionData.Instance.startedGame || (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive))
		{
			return;
		}
		if (this.disableControlDisplay.Contains(key))
		{
			return;
		}
		ControlsDisplayController.CustomActionsDisplayed customActionsDisplayed = this.customActionsDisplayed.Find((ControlsDisplayController.CustomActionsDisplayed item) => item.key == key);
		if (customActionsDisplayed == null || overrideMinDisplayTime || SessionData.Instance.gameTime > customActionsDisplayed.lastDisplayedAt + this.minimumCustomControlDisplayTimeInterval)
		{
			if (customActionsDisplayed == null)
			{
				ControlsDisplayController.CustomActionsDisplayed customActionsDisplayed2 = new ControlsDisplayController.CustomActionsDisplayed();
				customActionsDisplayed2.key = key;
				customActionsDisplayed2.interactionName = interactionName;
				customActionsDisplayed2.displayTime = forTime;
				customActionsDisplayed2.lastDisplayedAt = SessionData.Instance.gameTime;
				customActionsDisplayed2.action = new Interactable.InteractableCurrentAction
				{
					display = true,
					enabled = true
				};
				customActionsDisplayed2.action.overrideInteractionName = interactionName;
				this.customActionsDisplayed.Add(customActionsDisplayed2);
			}
			else
			{
				customActionsDisplayed.interactionName = interactionName;
				customActionsDisplayed.displayTime = forTime;
				customActionsDisplayed.lastDisplayedAt = SessionData.Instance.gameTime;
				customActionsDisplayed.action.overrideInteractionName = interactionName;
			}
			InteractionController.Instance.UpdateInteractionText();
		}
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x000ECA14 File Offset: 0x000EAC14
	public string GetControlIcon(InteractablePreset.InteractionKey key, out ControlDisplayController.ControlPositioning positioning, out bool foundControl)
	{
		foundControl = true;
		positioning = ControlDisplayController.ControlPositioning.neutral;
		if (key == InteractablePreset.InteractionKey.scrollAxisUp)
		{
			positioning = ControlDisplayController.ControlPositioning.up;
		}
		else if (key == InteractablePreset.InteractionKey.scrollAxisDown)
		{
			positioning = ControlDisplayController.ControlPositioning.down;
		}
		string text = key.ToString();
		ActionElementMap actionElementMap = null;
		if (InputController.Instance.player != null)
		{
			actionElementMap = InputController.Instance.player.controllers.maps.GetFirstElementMapWithAction(text, true);
		}
		if (actionElementMap != null)
		{
			string text2 = actionElementMap.elementIdentifierName;
			string text3 = string.Empty;
			string text4;
			if (actionElementMap.controllerMap.controllerType == null)
			{
				text4 = "desktop";
				if (text2.Length <= 1)
				{
					text2 = "Keyboard Key";
					text3 = actionElementMap.elementIdentifierName.ToUpper() + "  ";
				}
				else if (text2.Length <= 3)
				{
					int num;
					if (text2.Length > 0 && text2.Substring(0, 1) == "F" && int.TryParse(text2.Substring(1, text2.Length - 1), ref num))
					{
						text2 = "Keyboard Key";
						text3 = actionElementMap.elementIdentifierName.ToUpper() + "  ";
					}
				}
				else if (text2.Length >= 7 && text2.Substring(0, 6) == "Keypad")
				{
					text2 = "Keyboard Key";
					text3 = actionElementMap.elementIdentifierName.ToUpper().Substring(7) + "  ";
				}
			}
			else if (actionElementMap.controllerMap.controllerType == 1)
			{
				text4 = "desktop";
				if (text2 == "Left Mouse Button")
				{
					positioning = ControlDisplayController.ControlPositioning.left;
				}
				else if (text2 == "Right Mouse Button")
				{
					positioning = ControlDisplayController.ControlPositioning.right;
				}
			}
			else
			{
				text4 = "controller";
				if (text2 == "Left Stick" || text2 == "Right Stick")
				{
					text2 += " Button";
				}
				if (text2 == "Left Stick X" || text2 == "Left Stick Y")
				{
					text2 = "Left Stick";
				}
				else if (text2 == "Right Stick X" || text2 == "Right Stick Y")
				{
					text2 = "Right Stick";
				}
				if (actionElementMap.controllerMap.controller.name == "Pro Controller" && (text2 == "X" || text2 == "Y" || text2 == "A" || text2 == "B"))
				{
					text2 += "_Nintendo";
				}
			}
			return string.Concat(new string[]
			{
				"<sprite=\"",
				text4,
				"\" name=\"",
				text2,
				"\">",
				text3
			});
		}
		foundControl = false;
		return string.Empty;
	}

	// Token: 0x040014CA RID: 5322
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x040014CB RID: 5323
	public RectTransform anchor;

	// Token: 0x040014CC RID: 5324
	public GameObject controlDisplayPrefab;

	// Token: 0x040014CD RID: 5325
	[Header("Settings")]
	public Vector2 padding = new Vector2(24f, 24f);

	// Token: 0x040014CE RID: 5326
	public float animationSelectTime = 0.5f;

	// Token: 0x040014CF RID: 5327
	public AnimationCurve controlSelectAnimation;

	// Token: 0x040014D0 RID: 5328
	public float controlSelectScaleLerp = 2f;

	// Token: 0x040014D1 RID: 5329
	public Color controlSelectColorLerp = Color.white;

	// Token: 0x040014D2 RID: 5330
	public Color audioFullColor = Color.white;

	// Token: 0x040014D3 RID: 5331
	public Color audioEmptyColor = Color.white;

	// Token: 0x040014D4 RID: 5332
	private float posChangeProgress = 1f;

	// Token: 0x040014D5 RID: 5333
	private float desiredYPos;

	// Token: 0x040014D6 RID: 5334
	private float desiredHeight;

	// Token: 0x040014D7 RID: 5335
	private float desiredRectFromLeft;

	// Token: 0x040014D8 RID: 5336
	private float desiredRectFromRight;

	// Token: 0x040014D9 RID: 5337
	[Tooltip("Minimum display time in gametime")]
	public float minimumCustomControlDisplayTimeInterval = 0.15f;

	// Token: 0x040014DA RID: 5338
	[Header("State")]
	public List<ControlDisplayController> spawned = new List<ControlDisplayController>();

	// Token: 0x040014DB RID: 5339
	public List<ControlsDisplayController.CustomActionsDisplayed> customActionsDisplayed = new List<ControlsDisplayController.CustomActionsDisplayed>();

	// Token: 0x040014DC RID: 5340
	public List<InteractablePreset.InteractionKey> disableControlDisplay = new List<InteractablePreset.InteractionKey>();

	// Token: 0x040014DD RID: 5341
	private static ControlsDisplayController _instance;

	// Token: 0x020002DE RID: 734
	public class CustomActionsDisplayed
	{
		// Token: 0x040014DE RID: 5342
		public InteractablePreset.InteractionKey key;

		// Token: 0x040014DF RID: 5343
		public string interactionName;

		// Token: 0x040014E0 RID: 5344
		public float displayTime;

		// Token: 0x040014E1 RID: 5345
		public float lastDisplayedAt;

		// Token: 0x040014E2 RID: 5346
		public Interactable.InteractableCurrentAction action;
	}
}
