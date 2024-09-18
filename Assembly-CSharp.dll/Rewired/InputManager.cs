using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Rewired.Utils;
using Rewired.Utils.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Rewired
{
	// Token: 0x02000830 RID: 2096
	[EditorBrowsable(1)]
	[AddComponentMenu("Rewired/Input Manager")]
	public sealed class InputManager : InputManager_Base
	{
		// Token: 0x060028D8 RID: 10456 RVA: 0x001FB0C9 File Offset: 0x001F92C9
		protected override void OnInitialized()
		{
			this.SubscribeEvents();
		}

		// Token: 0x060028D9 RID: 10457 RVA: 0x001FB0D1 File Offset: 0x001F92D1
		protected override void OnDeinitialized()
		{
			this.UnsubscribeEvents();
		}

		// Token: 0x060028DA RID: 10458 RVA: 0x001FB0DC File Offset: 0x001F92DC
		protected override void DetectPlatform()
		{
			this.scriptingBackend = 0;
			this.scriptingAPILevel = 0;
			this.editorPlatform = 0;
			this.platform = 0;
			this.webplayerPlatform = 0;
			this.isEditor = false;
			if (SystemInfo.deviceName == null)
			{
				string empty = string.Empty;
			}
			if (SystemInfo.deviceModel == null)
			{
				string empty2 = string.Empty;
			}
			this.platform = 1;
			this.scriptingBackend = 0;
			this.scriptingAPILevel = 2;
		}

		// Token: 0x060028DB RID: 10459 RVA: 0x00002265 File Offset: 0x00000465
		protected override void CheckRecompile()
		{
		}

		// Token: 0x060028DC RID: 10460 RVA: 0x001FB142 File Offset: 0x001F9342
		protected override IExternalTools GetExternalTools()
		{
			return new ExternalTools();
		}

		// Token: 0x060028DD RID: 10461 RVA: 0x001FB149 File Offset: 0x001F9349
		private bool CheckDeviceName(string searchPattern, string deviceName, string deviceModel)
		{
			return Regex.IsMatch(deviceName, searchPattern, 1) || Regex.IsMatch(deviceModel, searchPattern, 1);
		}

		// Token: 0x060028DE RID: 10462 RVA: 0x001FB15F File Offset: 0x001F935F
		private void SubscribeEvents()
		{
			this.UnsubscribeEvents();
			SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded);
		}

		// Token: 0x060028DF RID: 10463 RVA: 0x001FB178 File Offset: 0x001F9378
		private void UnsubscribeEvents()
		{
			SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded);
		}

		// Token: 0x060028E0 RID: 10464 RVA: 0x001FB18B File Offset: 0x001F938B
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			base.OnSceneLoaded();
		}

		// Token: 0x0400472F RID: 18223
		private bool ignoreRecompile;
	}
}
