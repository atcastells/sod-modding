using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008BC RID: 2236
	[AddComponentMenu("")]
	public class PressStartToJoinExample_Assigner : MonoBehaviour
	{
		// Token: 0x06002FEB RID: 12267 RVA: 0x002129DC File Offset: 0x00210BDC
		public static Player GetRewiredPlayer(int gamePlayerId)
		{
			if (!ReInput.isReady)
			{
				return null;
			}
			if (PressStartToJoinExample_Assigner.instance == null)
			{
				Debug.LogError("Not initialized. Do you have a PressStartToJoinPlayerSelector in your scehe?");
				return null;
			}
			for (int i = 0; i < PressStartToJoinExample_Assigner.instance.playerMap.Count; i++)
			{
				if (PressStartToJoinExample_Assigner.instance.playerMap[i].gamePlayerId == gamePlayerId)
				{
					return ReInput.players.GetPlayer(PressStartToJoinExample_Assigner.instance.playerMap[i].rewiredPlayerId);
				}
			}
			return null;
		}

		// Token: 0x06002FEC RID: 12268 RVA: 0x00212A5E File Offset: 0x00210C5E
		private void Awake()
		{
			this.playerMap = new List<PressStartToJoinExample_Assigner.PlayerMap>();
			PressStartToJoinExample_Assigner.instance = this;
		}

		// Token: 0x06002FED RID: 12269 RVA: 0x00212A74 File Offset: 0x00210C74
		private void Update()
		{
			for (int i = 0; i < ReInput.players.playerCount; i++)
			{
				if (ReInput.players.GetPlayer(i).GetButtonDown("JoinGame"))
				{
					this.AssignNextPlayer(i);
				}
			}
		}

		// Token: 0x06002FEE RID: 12270 RVA: 0x00212AB4 File Offset: 0x00210CB4
		private void AssignNextPlayer(int rewiredPlayerId)
		{
			if (this.playerMap.Count >= this.maxPlayers)
			{
				Debug.LogError("Max player limit already reached!");
				return;
			}
			int nextGamePlayerId = this.GetNextGamePlayerId();
			this.playerMap.Add(new PressStartToJoinExample_Assigner.PlayerMap(rewiredPlayerId, nextGamePlayerId));
			Player player = ReInput.players.GetPlayer(rewiredPlayerId);
			player.controllers.maps.SetMapsEnabled(false, "Assignment");
			player.controllers.maps.SetMapsEnabled(true, "Default");
			Debug.Log("Added Rewired Player id " + rewiredPlayerId.ToString() + " to game player " + nextGamePlayerId.ToString());
		}

		// Token: 0x06002FEF RID: 12271 RVA: 0x00212B54 File Offset: 0x00210D54
		private int GetNextGamePlayerId()
		{
			int num = this.gamePlayerIdCounter;
			this.gamePlayerIdCounter = num + 1;
			return num;
		}

		// Token: 0x04004AA1 RID: 19105
		private static PressStartToJoinExample_Assigner instance;

		// Token: 0x04004AA2 RID: 19106
		public int maxPlayers = 4;

		// Token: 0x04004AA3 RID: 19107
		private List<PressStartToJoinExample_Assigner.PlayerMap> playerMap;

		// Token: 0x04004AA4 RID: 19108
		private int gamePlayerIdCounter;

		// Token: 0x020008BD RID: 2237
		private class PlayerMap
		{
			// Token: 0x06002FF1 RID: 12273 RVA: 0x00212B81 File Offset: 0x00210D81
			public PlayerMap(int rewiredPlayerId, int gamePlayerId)
			{
				this.rewiredPlayerId = rewiredPlayerId;
				this.gamePlayerId = gamePlayerId;
			}

			// Token: 0x04004AA5 RID: 19109
			public int rewiredPlayerId;

			// Token: 0x04004AA6 RID: 19110
			public int gamePlayerId;
		}
	}
}
