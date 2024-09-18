using System;
using TMPro;
using UnityEngine;

// Token: 0x02000299 RID: 665
public class ComputerPlayerInputText : CruncherAppContent
{
	// Token: 0x06000EDE RID: 3806 RVA: 0x000D5924 File Offset: 0x000D3B24
	public override void OnSetup()
	{
		base.OnSetup();
		if (this.fullTextKey.Length > 0)
		{
			this.fullText = Strings.Get("computer", this.fullTextKey, Strings.Casing.asIs, false, false, false, null);
		}
		if (this.startingTextKey.Length > 0)
		{
			this.revealedText = Strings.Get("computer", this.startingTextKey, Strings.Casing.asIs, false, false, false, null);
			this.text.text = this.revealedText;
		}
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x000D599C File Offset: 0x000D3B9C
	private void Update()
	{
		this.keystrokes += Time.deltaTime * 100f;
		if (this.controller.playerControlled && InputController.Instance.player.GetAnyButtonDown())
		{
			this.keystrokes += (float)Toolbox.Instance.Rand(100, 120, false);
		}
		bool flag = false;
		if ((float)this.charsDisplayed < this.keystrokes)
		{
			this.charsDisplayed++;
			this.revealedText = this.fullText.Substring(0, Mathf.Min(this.charsDisplayed, this.fullText.Length));
			flag = true;
		}
		if (this.cursorTimer < 0.8f)
		{
			this.cursorTimer += Time.deltaTime;
			if (this.cursorTimer >= 0.8f)
			{
				this.displayCursor = !this.displayCursor;
				flag = true;
				this.cursorTimer = 0f;
			}
		}
		if (flag && this.text != null)
		{
			if (this.displayCursor)
			{
				this.text.text = this.revealedText + "_";
			}
			else
			{
				this.text.text = this.revealedText;
			}
		}
		if (this.revealedText.Length >= this.fullText.Length)
		{
			this.controller.OnAppExit();
		}
	}

	// Token: 0x04001207 RID: 4615
	public TextMeshProUGUI text;

	// Token: 0x04001208 RID: 4616
	public string startingTextKey;

	// Token: 0x04001209 RID: 4617
	public string fullTextKey;

	// Token: 0x0400120A RID: 4618
	private string fullText;

	// Token: 0x0400120B RID: 4619
	public float keystrokes;

	// Token: 0x0400120C RID: 4620
	public int charsDisplayed;

	// Token: 0x0400120D RID: 4621
	public bool displayCursor;

	// Token: 0x0400120E RID: 4622
	public float cursorTimer;

	// Token: 0x0400120F RID: 4623
	private string revealedText;
}
