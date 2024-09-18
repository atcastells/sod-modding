using System;
using Steamworks;
using TMPro;
using UnityEngine;

// Token: 0x02000490 RID: 1168
public class SteamAPIController : MonoBehaviour
{
	// Token: 0x060018FD RID: 6397 RVA: 0x001724AC File Offset: 0x001706AC
	private void Update()
	{
		if (SteamManager.Initialized)
		{
			try
			{
				Game.Log("Steam manager initialized: " + SteamFriends.GetPersonaName(), 2);
				CSteamID steamID = SteamUser.GetSteamID();
				this.steam64IDtext.text = steamID.ToString();
				base.enabled = false;
			}
			catch
			{
				Game.LogError("Unable to get steam ID", 2);
			}
		}
	}

	// Token: 0x0400219D RID: 8605
	protected Callback<SteamNetworkingIdentity> m_GameOverlayActivated;

	// Token: 0x0400219E RID: 8606
	public TextMeshProUGUI steam64IDtext;
}
