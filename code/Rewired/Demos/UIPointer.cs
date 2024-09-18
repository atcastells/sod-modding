using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos
{
	// Token: 0x020008B9 RID: 2233
	[AddComponentMenu("")]
	[RequireComponent(typeof(RectTransform))]
	public sealed class UIPointer : UIBehaviour
	{
		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06002FD7 RID: 12247 RVA: 0x002125FA File Offset: 0x002107FA
		// (set) Token: 0x06002FD8 RID: 12248 RVA: 0x00212602 File Offset: 0x00210802
		public bool autoSort
		{
			get
			{
				return this._autoSort;
			}
			set
			{
				if (value == this._autoSort)
				{
					return;
				}
				this._autoSort = value;
				if (value)
				{
					base.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x06002FD9 RID: 12249 RVA: 0x00212624 File Offset: 0x00210824
		protected override void Awake()
		{
			base.Awake();
			Graphic[] componentsInChildren = base.GetComponentsInChildren<Graphic>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].raycastTarget = false;
			}
			if (this._hideHardwarePointer)
			{
				Cursor.visible = false;
			}
			if (this._autoSort)
			{
				base.transform.SetAsLastSibling();
			}
			this.GetDependencies();
		}

		// Token: 0x06002FDA RID: 12250 RVA: 0x0021267C File Offset: 0x0021087C
		private void Update()
		{
			if (this._autoSort && base.transform.GetSiblingIndex() < base.transform.parent.childCount - 1)
			{
				base.transform.SetAsLastSibling();
			}
		}

		// Token: 0x06002FDB RID: 12251 RVA: 0x002126B0 File Offset: 0x002108B0
		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.GetDependencies();
		}

		// Token: 0x06002FDC RID: 12252 RVA: 0x002126BE File Offset: 0x002108BE
		protected override void OnCanvasGroupChanged()
		{
			base.OnCanvasGroupChanged();
			this.GetDependencies();
		}

		// Token: 0x06002FDD RID: 12253 RVA: 0x002126CC File Offset: 0x002108CC
		public void OnScreenPositionChanged(Vector2 screenPosition)
		{
			if (this._canvas == null)
			{
				return;
			}
			Camera camera = null;
			RenderMode renderMode = this._canvas.renderMode;
			if (renderMode != null && renderMode - 1 <= 1)
			{
				camera = this._canvas.worldCamera;
			}
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform.parent as RectTransform, screenPosition, camera, ref vector);
			base.transform.localPosition = new Vector3(vector.x, vector.y, base.transform.localPosition.z);
		}

		// Token: 0x06002FDE RID: 12254 RVA: 0x00212751 File Offset: 0x00210951
		private void GetDependencies()
		{
			this._canvas = base.transform.root.GetComponentInChildren<Canvas>();
		}

		// Token: 0x04004A97 RID: 19095
		[Tooltip("Should the hardware pointer be hidden?")]
		[SerializeField]
		private bool _hideHardwarePointer = true;

		// Token: 0x04004A98 RID: 19096
		[SerializeField]
		[Tooltip("Sets the pointer to the last sibling in the parent hierarchy. Do not enable this on multiple UIPointers under the same parent transform or they will constantly fight each other for dominance.")]
		private bool _autoSort = true;

		// Token: 0x04004A99 RID: 19097
		private Canvas _canvas;
	}
}
