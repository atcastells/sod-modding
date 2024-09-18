using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class SlipDebug : MonoBehaviour
{
	// Token: 0x06000BDA RID: 3034 RVA: 0x000AB06C File Offset: 0x000A926C
	private void Update()
	{
		if (Input.GetKeyDown(114))
		{
			Game.Log("Slip debug!", 2);
			bool forwards = false;
			if (Toolbox.Instance.Rand(0f, 1f, false) < 0.5f)
			{
				forwards = true;
			}
			Player.Instance.Trip(0f, forwards, true);
		}
	}
}
