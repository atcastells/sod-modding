using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Rewired;
using TMPro;
using UnityEngine;

// Token: 0x020004EC RID: 1260
public class ControlDetectController : MonoBehaviour
{
	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x06001B40 RID: 6976 RVA: 0x0018BD90 File Offset: 0x00189F90
	public static ControlDetectController Instance
	{
		get
		{
			return ControlDetectController._instance;
		}
	}

	// Token: 0x06001B41 RID: 6977 RVA: 0x0018BD97 File Offset: 0x00189F97
	private void Awake()
	{
		if (ControlDetectController._instance != null && ControlDetectController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		ControlDetectController._instance = this;
	}

	// Token: 0x06001B42 RID: 6978 RVA: 0x0018BDC8 File Offset: 0x00189FC8
	private void Start()
	{
		QualitySettings.vSyncCount = 1;
		bool flag = false;
		flag = Convert.ToBoolean(PlayerPrefs.GetInt("playedBefore", 0));
		string text = string.Empty;
		if (flag)
		{
			text = PlayerPrefs.GetString("language");
		}
		foreach (LanguageConfigLoader.LocInput locInput in LanguageConfigLoader.Instance.fileInputConfig)
		{
			try
			{
				if (flag)
				{
					if (locInput.languageCode.ToLower() == text.ToLower())
					{
						this.pressAnyKeyText.text = locInput.startText;
						break;
					}
				}
				else if (locInput.systemLanguage == Application.systemLanguage)
				{
					this.pressAnyKeyText.text = locInput.startText;
					break;
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06001B43 RID: 6979 RVA: 0x0018BEA4 File Offset: 0x0018A0A4
	private void Update()
	{
		if (!ReInput.isReady)
		{
			return;
		}
		if (!Caching.ready)
		{
			return;
		}
		if (this.player == null)
		{
			this.player = ReInput.players.GetPlayer(0);
			Player player = this.player;
			return;
		}
		if (!this.loadSceneTriggered)
		{
			if (!this.loadSceneTriggered && (ReInput.controllers.GetAnyButtonUp(1) || ReInput.controllers.GetAnyButtonUp(0)))
			{
				this.loadSceneTriggered = true;
				PlayerPrefs.SetInt("controlMethod", 1);
				Debug.Log("Set control method to 1: Mouse & Keybaord");
			}
			if (!this.loadSceneTriggered && ReInput.controllers.GetAnyButtonUp(2))
			{
				Debug.Log("...Controller input...");
				Controller lastActiveController = ReInput.controllers.GetLastActiveController();
				if (lastActiveController != null && !this.loadSceneTriggered)
				{
					Debug.Log("Last controller with input: " + lastActiveController.hardwareName);
					this.loadSceneTriggered = true;
					PlayerPrefs.SetInt("controlMethod", 0);
					Debug.Log("Set control method to 0: Controller");
					return;
				}
				Debug.Log("...Last active controller is null...");
				return;
			}
		}
		else
		{
			if (!this.loadingIcon.gameObject.activeSelf)
			{
				this.loadingIcon.gameObject.SetActive(true);
			}
			this.loadingIcon.localEulerAngles = new Vector3(0f, 0f, this.loadingIconAnimCurve.Evaluate(this.fadeOut));
			if (this.fadeOut < 1f)
			{
				this.fadeOut += Time.deltaTime * 2f;
				this.fadeOut = Mathf.Clamp01(this.fadeOut);
				foreach (CanvasRenderer canvasRenderer in this.fadeOutRenderers)
				{
					canvasRenderer.SetAlpha(1f - this.fadeOut);
				}
				foreach (CanvasRenderer canvasRenderer2 in this.fadeInRenderers)
				{
					canvasRenderer2.SetAlpha(this.fadeOut);
				}
				if (this.pressAnyKeyText.text.Length > 0)
				{
					this.pressAnyKeyText.text = this.pressAnyKeyText.text.Substring(0, this.pressAnyKeyText.text.Length - 1);
					return;
				}
			}
			else if (!this.loadingScene)
			{
				this.LoadMainScene();
			}
		}
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x0018C110 File Offset: 0x0018A310
	private void LoadMainScene()
	{
		ControlDetectController.<LoadMainScene>d__15 <LoadMainScene>d__;
		<LoadMainScene>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadMainScene>d__.<>4__this = this;
		<LoadMainScene>d__.<>1__state = -1;
		<LoadMainScene>d__.<>t__builder.Start<ControlDetectController.<LoadMainScene>d__15>(ref <LoadMainScene>d__);
	}

	// Token: 0x040023E8 RID: 9192
	[Header("Components")]
	public TextMeshProUGUI pressAnyKeyText;

	// Token: 0x040023E9 RID: 9193
	public List<CanvasRenderer> fadeOutRenderers = new List<CanvasRenderer>();

	// Token: 0x040023EA RID: 9194
	public List<CanvasRenderer> fadeInRenderers = new List<CanvasRenderer>();

	// Token: 0x040023EB RID: 9195
	public RectTransform loadingIcon;

	// Token: 0x040023EC RID: 9196
	public AnimationCurve loadingIconAnimCurve;

	// Token: 0x040023ED RID: 9197
	[NonSerialized]
	public Player player;

	// Token: 0x040023EE RID: 9198
	[Header("Variables")]
	public bool loadSceneTriggered;

	// Token: 0x040023EF RID: 9199
	private bool loadingScene;

	// Token: 0x040023F0 RID: 9200
	public float fadeOut;

	// Token: 0x040023F1 RID: 9201
	private static ControlDetectController _instance;
}
