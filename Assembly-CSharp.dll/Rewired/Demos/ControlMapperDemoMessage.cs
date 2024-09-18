using System;
using System.Collections;
using Rewired.UI.ControlMapper;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos
{
	// Token: 0x020008C9 RID: 2249
	[AddComponentMenu("")]
	public class ControlMapperDemoMessage : MonoBehaviour
	{
		// Token: 0x0600302A RID: 12330 RVA: 0x00213CED File Offset: 0x00211EED
		private void Awake()
		{
			if (this.controlMapper != null)
			{
				this.controlMapper.ScreenClosedEvent += new Action(this.OnControlMapperClosed);
				this.controlMapper.ScreenOpenedEvent += new Action(this.OnControlMapperOpened);
			}
		}

		// Token: 0x0600302B RID: 12331 RVA: 0x00213D2B File Offset: 0x00211F2B
		private void Start()
		{
			this.SelectDefault();
		}

		// Token: 0x0600302C RID: 12332 RVA: 0x00213D33 File Offset: 0x00211F33
		private void OnControlMapperClosed()
		{
			base.gameObject.SetActive(true);
			base.StartCoroutine(this.SelectDefaultDeferred());
		}

		// Token: 0x0600302D RID: 12333 RVA: 0x00213D4E File Offset: 0x00211F4E
		private void OnControlMapperOpened()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600302E RID: 12334 RVA: 0x00213D5C File Offset: 0x00211F5C
		private void SelectDefault()
		{
			if (EventSystem.current == null)
			{
				return;
			}
			if (this.defaultSelectable != null)
			{
				EventSystem.current.SetSelectedGameObject(this.defaultSelectable.gameObject);
			}
		}

		// Token: 0x0600302F RID: 12335 RVA: 0x00213D8F File Offset: 0x00211F8F
		private IEnumerator SelectDefaultDeferred()
		{
			yield return null;
			this.SelectDefault();
			yield break;
		}

		// Token: 0x04004AE7 RID: 19175
		public ControlMapper controlMapper;

		// Token: 0x04004AE8 RID: 19176
		public Selectable defaultSelectable;
	}
}
