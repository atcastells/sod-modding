using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	// Token: 0x020008BA RID: 2234
	[AddComponentMenu("")]
	public class PressAnyButtonToJoinExample_Assigner : MonoBehaviour
	{
		// Token: 0x06002FE0 RID: 12256 RVA: 0x0021277F File Offset: 0x0021097F
		private void Update()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			this.AssignJoysticksToPlayers();
		}

		// Token: 0x06002FE1 RID: 12257 RVA: 0x00212790 File Offset: 0x00210990
		private void AssignJoysticksToPlayers()
		{
			IList<Joystick> joysticks = ReInput.controllers.Joysticks;
			for (int i = 0; i < joysticks.Count; i++)
			{
				Joystick joystick = joysticks[i];
				if (!ReInput.controllers.IsControllerAssigned(joystick.type, joystick.id) && joystick.GetAnyButtonDown())
				{
					Player player = this.FindPlayerWithoutJoystick();
					if (player == null)
					{
						return;
					}
					player.controllers.AddController(joystick, false);
				}
			}
			if (this.DoAllPlayersHaveJoysticks())
			{
				ReInput.configuration.autoAssignJoysticks = true;
				base.enabled = false;
			}
		}

		// Token: 0x06002FE2 RID: 12258 RVA: 0x00212814 File Offset: 0x00210A14
		private Player FindPlayerWithoutJoystick()
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].controllers.joystickCount <= 0)
				{
					return players[i];
				}
			}
			return null;
		}

		// Token: 0x06002FE3 RID: 12259 RVA: 0x0021285A File Offset: 0x00210A5A
		private bool DoAllPlayersHaveJoysticks()
		{
			return this.FindPlayerWithoutJoystick() == null;
		}
	}
}
