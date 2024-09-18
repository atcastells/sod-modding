using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

// Token: 0x0200023C RID: 572
public class Benchmarking : MonoBehaviour
{
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x06000CE7 RID: 3303 RVA: 0x000B8037 File Offset: 0x000B6237
	public static Benchmarking Instance
	{
		get
		{
			return Benchmarking._instance;
		}
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x000B803E File Offset: 0x000B623E
	private void Awake()
	{
		if (Benchmarking._instance != null && Benchmarking._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Benchmarking._instance = this;
		}
		base.enabled = false;
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x000B8074 File Offset: 0x000B6274
	private void Update()
	{
		if (!SessionData.Instance.play)
		{
			return;
		}
		if (!this.benchmarkingActive)
		{
			base.enabled = false;
			return;
		}
		this.frames++;
		this.secondsPassed += Time.deltaTime;
		float num = 1f / Time.unscaledDeltaTime;
		if (this.secondsPassed > 2f)
		{
			if (num < this.fpsLow)
			{
				this.fpsLow = num;
			}
			if (num > this.fpsHigh)
			{
				this.fpsHigh = num;
			}
		}
		this.fpsText.text = string.Concat(new string[]
		{
			"Current: ",
			num.ToString(),
			" Av: ",
			((float)this.frames / this.secondsPassed).ToString(),
			" Low: ",
			this.fpsLow.ToString(),
			" High: ",
			this.fpsHigh.ToString()
		});
		if (this.secondsPassed >= 30f)
		{
			this.PauseBenchmarking();
		}
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x000B817F File Offset: 0x000B637F
	[Button(null, 0)]
	public void StartBenchmarking()
	{
		this.benchmarkingActive = true;
		base.enabled = true;
		this.fpsText.gameObject.SetActive(true);
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x000B81A0 File Offset: 0x000B63A0
	public void PauseBenchmarking()
	{
		this.benchmarkingActive = false;
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x000B81A9 File Offset: 0x000B63A9
	[Button(null, 0)]
	public void StopBenchmarking()
	{
		this.ResetBenchmarking();
		this.benchmarkingActive = false;
		base.enabled = false;
		this.fpsText.gameObject.SetActive(false);
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x000B81D0 File Offset: 0x000B63D0
	[Button(null, 0)]
	public void ResetBenchmarking()
	{
		this.frames = 0;
		this.secondsPassed = 0f;
		this.fpsHigh = -1E+11f;
		this.fpsLow = 1E+12f;
	}

	// Token: 0x04000ED8 RID: 3800
	[Header("State")]
	public bool benchmarkingActive;

	// Token: 0x04000ED9 RID: 3801
	public int frames;

	// Token: 0x04000EDA RID: 3802
	public float secondsPassed;

	// Token: 0x04000EDB RID: 3803
	public float fpsLow = 1E+10f;

	// Token: 0x04000EDC RID: 3804
	public float fpsHigh = -1E+10f;

	// Token: 0x04000EDD RID: 3805
	[Header("Components")]
	public TextMeshProUGUI fpsText;

	// Token: 0x04000EDE RID: 3806
	private static Benchmarking _instance;
}
