using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

// Token: 0x020005FC RID: 1532
public class TextToImageController : MonoBehaviour
{
	// Token: 0x17000101 RID: 257
	// (get) Token: 0x060021BB RID: 8635 RVA: 0x001CA3E9 File Offset: 0x001C85E9
	public static TextToImageController Instance
	{
		get
		{
			return TextToImageController._instance;
		}
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x001CA3F0 File Offset: 0x001C85F0
	private void Awake()
	{
		if (TextToImageController._instance != null && TextToImageController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		TextToImageController._instance = this;
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x001CA41E File Offset: 0x001C861E
	private void Start()
	{
		this.newsTickerMaterial = Object.Instantiate<Material>(this.newsTickerMaterial);
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x001CA434 File Offset: 0x001C8634
	[Button(null, 0)]
	public Texture2D CaptureTextToImage(TextToImageController.TextToImageSettings settings = null)
	{
		if (settings == null)
		{
			settings = this.defaultSettings;
		}
		this.captureText.enableAutoSizing = false;
		this.captureText.rectTransform.sizeDelta = new Vector2(9999f, 0f);
		this.captureText.font = settings.font;
		this.captureText.fontSize = settings.textSize;
		this.captureText.SetText(settings.textString, true);
		this.captureText.UpdateFontAsset();
		this.captureText.UpdateMeshPadding();
		this.captureText.ForceMeshUpdate(false, false);
		Vector2 preferredValues = this.captureText.GetPreferredValues();
		int num = Mathf.CeilToInt(preferredValues.x + 1f);
		int num2 = Mathf.CeilToInt(preferredValues.y + 1f);
		if ((float)num > this.maxSize.x || (float)num2 > this.maxSize.y)
		{
			num = Mathf.Min(num, (int)this.maxSize.x);
			num2 = Mathf.Min(num2, (int)this.maxSize.y);
			this.captureText.rectTransform.sizeDelta = new Vector2((float)num, (float)num2);
			this.captureText.enableAutoSizing = true;
			this.captureText.fontSizeMax = settings.textSize;
			this.captureText.fontSizeMin = 1f;
		}
		else
		{
			this.captureText.rectTransform.sizeDelta = new Vector2((float)num, (float)num2);
		}
		this.captureText.UpdateMeshPadding();
		this.captureText.ForceMeshUpdate(false, false);
		this.lastText = this.captureText.text;
		this.lastFontSize = this.captureText.fontSize;
		this.lastDimenstions = new Vector2((float)num, (float)num2);
		SceneCapture.Instance.photoRoomParent.SetActive(true);
		CameraController.Instance.cam.orthographic = true;
		RenderTexture renderTexture = new RenderTexture((int)this.maxSize.x, (int)this.maxSize.y, 24);
		renderTexture.antiAliasing = 1;
		renderTexture.filterMode = 0;
		renderTexture.autoGenerateMips = false;
		renderTexture.useDynamicScale = false;
		this.captureTextCanvasRect.sizeDelta = new Vector2((float)num, (float)num2);
		this.currentShot = SceneCapture.Instance.CaptureScene(SceneCapture.Instance.cameraTransform.position, SceneCapture.Instance.cameraTransform.eulerAngles, Toolbox.Instance.textToImageMask, false, 0f, renderTexture, 70f, null, null, false, true, true, SceneCapture.PostProcessingProfile.captureNormal);
		this.currentShot.filterMode = 0;
		CameraController.Instance.cam.orthographic = false;
		SceneCapture.Instance.photoRoomParent.SetActive(false);
		if (this.saveDebugScreenshot && Game.Instance.devMode)
		{
			bool collectDebugData = Game.Instance.collectDebugData;
		}
		if (settings.enableProcessing)
		{
			this.ProcessImage(settings);
		}
		renderTexture.Release();
		Object.Destroy(renderTexture);
		return this.currentShot;
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x001CA710 File Offset: 0x001C8910
	[Button(null, 0)]
	public void UpdateNewsTickerHeadline(string newString = "")
	{
		if (newString.Length <= 0)
		{
			newString = this.defaultSettings.textString;
		}
		Game.Log("Gameplay: Updating news ticker: " + newString, 2);
		Texture2D texture2D = this.CaptureTextToImage(new TextToImageController.TextToImageSettings
		{
			textString = Strings.Get("misc", "newsstand_ticker", Strings.Casing.asIs, false, false, false, null) + " — " + newString + " — ",
			font = this.newsTickerFont,
			textSize = this.newsTickerFontSize,
			trim = true,
			trimPadding = 7,
			useAlpha = false,
			color = Color.white
		});
		this.newsTickerMaterial.SetTexture("_MarqueeImage", texture2D);
		float num = this.newsTickerDivider / (float)texture2D.width;
		this.newsTickerMaterial.SetVector("_SignArea", new Vector4(num, 1f, 0f, 0f));
		this.newsTickerMaterial.SetFloat("_Speed", num / this.newsTickerSpeedDivider);
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x001CA814 File Offset: 0x001C8A14
	[Button(null, 0)]
	public void ProcessImage(TextToImageController.TextToImageSettings settings = null)
	{
		if (settings == null)
		{
			settings = this.defaultSettings;
		}
		if (this.currentShot != null)
		{
			Vector2 vector;
			vector..ctor(99999f, 99999f);
			Vector2 zero = Vector2.zero;
			for (int i = 0; i < this.currentShot.width; i++)
			{
				for (int j = 0; j < this.currentShot.height; j++)
				{
					Color color = this.currentShot.GetPixel(i, j);
					if (color.grayscale < 0.2f)
					{
						color = Color.black;
					}
					else
					{
						if ((float)i < vector.x)
						{
							vector.x = (float)i;
						}
						if ((float)j < vector.y)
						{
							vector.y = (float)j;
						}
						if ((float)i > zero.x)
						{
							zero.x = (float)i;
						}
						if ((float)j > zero.y)
						{
							zero.y = (float)j;
						}
						if (settings.contrast != 1f)
						{
							color *= settings.contrast;
						}
					}
					Color32 color2 = color;
					int num = ((65536 + (int)color2.r) * 256 + (int)color2.b) * 256 + (int)color2.g;
					int num2 = num % 256;
					int num3 = Mathf.FloorToInt((float)(num / 256));
					int num4 = num3 % 256;
					int num5 = Mathf.FloorToInt((float)(num3 / 256)) % 256;
					float num6 = 0.2126f * (float)num5 / 255f + 0.7152f * ((float)num4 / 255f) + 0.0722f * ((float)num2 / 255f);
					Color color3;
					color3..ctor(num6, num6, num6, 1f);
					this.currentShot.SetPixel(i, j, color3);
				}
			}
			this.currentShot.Apply();
			if (settings.trim)
			{
				vector..ctor(Mathf.Max(vector.x - (float)settings.trimPadding, 0f), Mathf.Max(vector.y - (float)settings.trimPadding, 0f));
				zero..ctor(Mathf.Min(zero.x + (float)settings.trimPadding, (float)this.currentShot.width), Mathf.Min(zero.y + (float)settings.trimPadding, (float)this.currentShot.height));
				int num7 = (int)zero.x - (int)vector.x;
				int num8 = (int)zero.y - (int)vector.y;
				Texture2D texture2D = new Texture2D(num7, num8);
				texture2D.filterMode = 0;
				for (int k = 0; k < num7; k++)
				{
					for (int l = 0; l < num8; l++)
					{
						Color color4 = this.currentShot.GetPixel((int)vector.x + k, (int)vector.y + l);
						float a = 1f;
						if (settings.useAlpha)
						{
							a = color4.grayscale * 1.2f;
						}
						color4 *= settings.color;
						color4.a = a;
						texture2D.SetPixel(k, l, color4);
					}
				}
				texture2D.Apply();
				this.currentShot = texture2D;
			}
		}
	}

	// Token: 0x060021C1 RID: 8641 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void SavePNG()
	{
	}

	// Token: 0x04002C2A RID: 11306
	[Header("Components")]
	public RectTransform captureTextCanvasRect;

	// Token: 0x04002C2B RID: 11307
	public TextMeshProUGUI captureText;

	// Token: 0x04002C2C RID: 11308
	public Material newsTickerMaterial;

	// Token: 0x04002C2D RID: 11309
	public TMP_FontAsset newsTickerFont;

	// Token: 0x04002C2E RID: 11310
	public float newsTickerFontSize = 40f;

	// Token: 0x04002C2F RID: 11311
	public float newsTickerDivider = 128f;

	// Token: 0x04002C30 RID: 11312
	public float newsTickerSpeedDivider = 4f;

	// Token: 0x04002C31 RID: 11313
	[Header("Settings")]
	public bool saveDebugScreenshot;

	// Token: 0x04002C32 RID: 11314
	public Vector2 maxSize = new Vector2(2048f, 128f);

	// Token: 0x04002C33 RID: 11315
	public TextToImageController.TextToImageSettings defaultSettings = new TextToImageController.TextToImageSettings();

	// Token: 0x04002C34 RID: 11316
	[Header("Preview")]
	public string lastText;

	// Token: 0x04002C35 RID: 11317
	[ReadOnly]
	public float lastFontSize;

	// Token: 0x04002C36 RID: 11318
	[ReadOnly]
	public Vector2 lastDimenstions;

	// Token: 0x04002C37 RID: 11319
	[ShowAssetPreview(64, 64)]
	public Texture2D currentShot;

	// Token: 0x04002C38 RID: 11320
	private RenderTexture instancedRender;

	// Token: 0x04002C39 RID: 11321
	private static TextToImageController _instance;

	// Token: 0x020005FD RID: 1533
	[Serializable]
	public class TextToImageSettings
	{
		// Token: 0x04002C3A RID: 11322
		public string textString = "Example";

		// Token: 0x04002C3B RID: 11323
		public float textSize = 42f;

		// Token: 0x04002C3C RID: 11324
		public TMP_FontAsset font;

		// Token: 0x04002C3D RID: 11325
		public bool enableProcessing = true;

		// Token: 0x04002C3E RID: 11326
		public float contrast = 1.2f;

		// Token: 0x04002C3F RID: 11327
		public bool trim = true;

		// Token: 0x04002C40 RID: 11328
		public int trimPadding = 4;

		// Token: 0x04002C41 RID: 11329
		public bool useAlpha = true;

		// Token: 0x04002C42 RID: 11330
		public Color color = Color.white;
	}
}
