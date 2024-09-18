using System;
using UnityEngine;

namespace FlyingWormConsole3
{
	// Token: 0x0200096A RID: 2410
	public class ConsoleProRemoteServer : MonoBehaviour
	{
		// Token: 0x060032F5 RID: 13045 RVA: 0x0022DB16 File Offset: 0x0022BD16
		public void Awake()
		{
			Debug.Log("Console Pro Remote Server is disabled in release mode, please use a Development build or define DEBUG to use it");
		}

		// Token: 0x04004F0F RID: 20239
		public bool useNATPunch;

		// Token: 0x04004F10 RID: 20240
		public int port = 51000;
	}
}
