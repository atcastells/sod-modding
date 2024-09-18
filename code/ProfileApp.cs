using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

// Token: 0x020002A7 RID: 679
public class ProfileApp : CruncherAppContent
{
	// Token: 0x06000F14 RID: 3860 RVA: 0x000D70B0 File Offset: 0x000D52B0
	public override void OnSetup()
	{
		if (this.controller.loggedInAs != null)
		{
			List<Evidence.DataKey> list = new List<Evidence.DataKey>();
			try
			{
				this.actorImage.texture = this.controller.loggedInAs.evidenceEntry.GetPhoto(Toolbox.Instance.GetList<Evidence.DataKey>(new Evidence.DataKey[]
				{
					Evidence.DataKey.photo
				}));
			}
			catch
			{
				Game.LogError("Unable to get cruncher login photo", 2);
			}
			list.Add(Evidence.DataKey.photo);
			list.Add(Evidence.DataKey.name);
			list.Add(Evidence.DataKey.code);
			string text = string.Empty;
			text = text + Strings.Get("descriptors", "First Name", Strings.Casing.firstLetterCaptial, false, false, false, null) + ": " + this.controller.loggedInAs.GetFirstName();
			text = string.Concat(new string[]
			{
				text,
				"\n",
				Strings.Get("descriptors", "Surname", Strings.Casing.firstLetterCaptial, false, false, false, null),
				": ",
				this.controller.loggedInAs.GetSurName()
			});
			text = string.Concat(new string[]
			{
				text,
				"\n",
				Strings.Get("descriptors", "Passcode", Strings.Casing.firstLetterCaptial, false, false, false, null),
				": ",
				this.controller.loggedInAs.passcode.GetDigit(0).ToString(),
				this.controller.loggedInAs.passcode.GetDigit(1).ToString(),
				this.controller.loggedInAs.passcode.GetDigit(2).ToString(),
				this.controller.loggedInAs.passcode.GetDigit(3).ToString()
			});
			if (this.controller.loggedInAs.home != null)
			{
				list.Add(Evidence.DataKey.address);
				text = string.Concat(new string[]
				{
					text,
					"\n\n",
					Strings.Get("descriptors", "Address", Strings.Casing.firstLetterCaptial, false, false, false, null),
					": ",
					this.controller.loggedInAs.home.name
				});
				if (this.controller.loggedInAs.home.telephones != null && this.controller.loggedInAs.home.telephones.Count > 0)
				{
					list.Add(Evidence.DataKey.telephoneNumber);
					text = string.Concat(new string[]
					{
						text,
						"\n",
						Strings.Get("descriptors", "Telephone", Strings.Casing.firstLetterCaptial, false, false, false, null),
						": ",
						this.controller.loggedInAs.home.telephones[0].numberString
					});
				}
			}
			foreach (Evidence.DataKey keyTwo in list)
			{
				if (this.controller.loggedInAs.evidenceEntry != null)
				{
					this.controller.loggedInAs.evidenceEntry.MergeDataKeys(Evidence.DataKey.name, keyTwo);
				}
			}
			this.infoText.text = text;
			this.controller.ic.interactable.SetCustomState3(true, Player.Instance, true, false, false);
		}
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x000D7410 File Offset: 0x000D5610
	private void OnDestroy()
	{
		this.controller.ic.interactable.SetCustomState3(false, Player.Instance, true, false, false);
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x000D6564 File Offset: 0x000D4764
	public void ExitButton()
	{
		this.controller.OnAppExit();
	}

	// Token: 0x04001239 RID: 4665
	public TextMeshProUGUI titleText;

	// Token: 0x0400123A RID: 4666
	public TextMeshProUGUI infoText;

	// Token: 0x0400123B RID: 4667
	public RawImage actorImage;
}
