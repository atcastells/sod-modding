using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000568 RID: 1384
public class EvidenceStrengthCount : CountController
{
	// Token: 0x06001E22 RID: 7714 RVA: 0x001A6598 File Offset: 0x001A4798
	public void SetMultiplier(int newVal)
	{
		this.multiplierCount = newVal;
		if (this.multiplierCount <= 0)
		{
			this.multiplierCountImg.sprite = this.multiplierCount0Sprite;
		}
		else if (this.multiplierCount == 1)
		{
			this.multiplierCountImg.sprite = this.multiplierCount1Sprite;
		}
		else if (this.multiplierCount == 2)
		{
			this.multiplierCountImg.sprite = this.multiplierCount2Sprite;
		}
		else if (this.multiplierCount >= 3)
		{
			this.multiplierCountImg.sprite = this.multiplierCount3Sprite;
		}
		base.VisibilityCheck();
	}

	// Token: 0x06001E23 RID: 7715 RVA: 0x001A6620 File Offset: 0x001A4820
	public void SetBonus(int newVal)
	{
		this.bonusCount = newVal;
		if (this.bonusCount == 0)
		{
			this.bonusImage.gameObject.SetActive(false);
		}
		else
		{
			this.bonusImage.gameObject.SetActive(true);
		}
		if (this.bonusCount > 0)
		{
			this.bonusImage.sprite = this.plusSprite;
			return;
		}
		this.bonusImage.sprite = this.minusSprite;
	}

	// Token: 0x040027F4 RID: 10228
	public int multiplierCount;

	// Token: 0x040027F5 RID: 10229
	public Image multiplierCountImg;

	// Token: 0x040027F6 RID: 10230
	public Sprite multiplierCount0Sprite;

	// Token: 0x040027F7 RID: 10231
	public Sprite multiplierCount1Sprite;

	// Token: 0x040027F8 RID: 10232
	public Sprite multiplierCount2Sprite;

	// Token: 0x040027F9 RID: 10233
	public Sprite multiplierCount3Sprite;

	// Token: 0x040027FA RID: 10234
	public int bonusCount;

	// Token: 0x040027FB RID: 10235
	public Image bonusImage;

	// Token: 0x040027FC RID: 10236
	public Sprite plusSprite;

	// Token: 0x040027FD RID: 10237
	public Sprite minusSprite;

	// Token: 0x040027FE RID: 10238
	public Image adjEffectDisplay;

	// Token: 0x040027FF RID: 10239
	public TooltipController adjTooltip;
}
