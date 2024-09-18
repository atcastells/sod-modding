using System;
using UnityEngine;
using UnityEngine.UI;

namespace BrainFailProductions.PolyFew.AsImpL
{
	// Token: 0x02000931 RID: 2353
	[RequireComponent(typeof(ObjectImporter))]
	public class ObjectImporterUI : MonoBehaviour
	{
		// Token: 0x060031F7 RID: 12791 RVA: 0x00222BDC File Offset: 0x00220DDC
		private void Awake()
		{
			if (this.progressSlider != null)
			{
				this.progressSlider.maxValue = 100f;
				this.progressSlider.gameObject.SetActive(false);
			}
			if (this.progressImage != null)
			{
				this.progressImage.gameObject.SetActive(false);
			}
			if (this.progressText != null)
			{
				this.progressText.gameObject.SetActive(false);
			}
			this.objImporter = base.GetComponent<ObjectImporter>();
		}

		// Token: 0x060031F8 RID: 12792 RVA: 0x00222C62 File Offset: 0x00220E62
		private void OnEnable()
		{
			this.objImporter.ImportingComplete += new Action(this.OnImportComplete);
			this.objImporter.ImportingStart += new Action(this.OnImportStart);
		}

		// Token: 0x060031F9 RID: 12793 RVA: 0x00222C92 File Offset: 0x00220E92
		private void OnDisable()
		{
			this.objImporter.ImportingComplete -= new Action(this.OnImportComplete);
			this.objImporter.ImportingStart -= new Action(this.OnImportStart);
		}

		// Token: 0x060031FA RID: 12794 RVA: 0x00222CC4 File Offset: 0x00220EC4
		private void Update()
		{
			bool flag = Loader.totalProgress.singleProgress.Count > 0;
			if (!flag)
			{
				return;
			}
			int numImportRequests = this.objImporter.NumImportRequests;
			int num = numImportRequests - Loader.totalProgress.singleProgress.Count;
			if (flag)
			{
				float num2 = 100f * (float)num / (float)numImportRequests;
				float num3 = 0f;
				foreach (SingleLoadingProgress singleLoadingProgress in Loader.totalProgress.singleProgress)
				{
					if (num3 < singleLoadingProgress.percentage)
					{
						num3 = singleLoadingProgress.percentage;
					}
				}
				num2 += num3 / (float)numImportRequests;
				if (this.progressSlider != null)
				{
					this.progressSlider.value = num2;
					this.progressSlider.gameObject.SetActive(flag);
				}
				if (this.progressImage != null)
				{
					this.progressImage.fillAmount = num2 / 100f;
					this.progressImage.gameObject.SetActive(flag);
				}
				if (this.progressText != null)
				{
					if (!flag)
					{
						this.progressText.gameObject.SetActive(false);
						this.progressText.text = "";
						return;
					}
					this.progressText.gameObject.SetActive(flag);
					this.progressText.text = "Loading " + Loader.totalProgress.singleProgress.Count.ToString() + " objects...";
					string text = "";
					int num4 = 0;
					foreach (SingleLoadingProgress singleLoadingProgress2 in Loader.totalProgress.singleProgress)
					{
						if (num4 > 4)
						{
							text += "...";
							break;
						}
						if (!string.IsNullOrEmpty(singleLoadingProgress2.message))
						{
							if (num4 > 0)
							{
								text += "; ";
							}
							text += singleLoadingProgress2.message;
							num4++;
						}
					}
					if (text != "")
					{
						Text text2 = this.progressText;
						text2.text = text2.text + "\n" + text;
						return;
					}
				}
			}
			else
			{
				this.OnImportComplete();
			}
		}

		// Token: 0x060031FB RID: 12795 RVA: 0x00222F24 File Offset: 0x00221124
		private void OnImportStart()
		{
			if (this.progressText != null)
			{
				this.progressText.text = "";
			}
			if (this.progressSlider != null)
			{
				this.progressSlider.value = 0f;
				this.progressSlider.gameObject.SetActive(true);
			}
			if (this.progressImage != null)
			{
				this.progressImage.fillAmount = 0f;
				this.progressImage.gameObject.SetActive(true);
			}
		}

		// Token: 0x060031FC RID: 12796 RVA: 0x00222FB0 File Offset: 0x002211B0
		private void OnImportComplete()
		{
			if (this.progressText != null)
			{
				this.progressText.text = "";
			}
			if (this.progressSlider != null)
			{
				this.progressSlider.value = 100f;
				this.progressSlider.gameObject.SetActive(false);
			}
			if (this.progressImage != null)
			{
				this.progressImage.fillAmount = 1f;
				this.progressImage.gameObject.SetActive(false);
			}
		}

		// Token: 0x04004D92 RID: 19858
		[Tooltip("Text for activity messages")]
		public Text progressText;

		// Token: 0x04004D93 RID: 19859
		[Tooltip("Slider for the overall progress")]
		public Slider progressSlider;

		// Token: 0x04004D94 RID: 19860
		[Tooltip("Panel with the Image Type set to Filled")]
		public Image progressImage;

		// Token: 0x04004D95 RID: 19861
		private ObjectImporter objImporter;
	}
}
