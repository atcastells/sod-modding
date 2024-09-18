using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000874 RID: 2164
	[AddComponentMenu("")]
	public class CustomToggle : Toggle, ICustomSelectable, ICancelHandler, IEventSystemHandler
	{
		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06002D3A RID: 11578 RVA: 0x0020A58F File Offset: 0x0020878F
		// (set) Token: 0x06002D3B RID: 11579 RVA: 0x0020A597 File Offset: 0x00208797
		public Sprite disabledHighlightedSprite
		{
			get
			{
				return this._disabledHighlightedSprite;
			}
			set
			{
				this._disabledHighlightedSprite = value;
			}
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06002D3C RID: 11580 RVA: 0x0020A5A0 File Offset: 0x002087A0
		// (set) Token: 0x06002D3D RID: 11581 RVA: 0x0020A5A8 File Offset: 0x002087A8
		public Color disabledHighlightedColor
		{
			get
			{
				return this._disabledHighlightedColor;
			}
			set
			{
				this._disabledHighlightedColor = value;
			}
		}

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06002D3E RID: 11582 RVA: 0x0020A5B1 File Offset: 0x002087B1
		// (set) Token: 0x06002D3F RID: 11583 RVA: 0x0020A5B9 File Offset: 0x002087B9
		public string disabledHighlightedTrigger
		{
			get
			{
				return this._disabledHighlightedTrigger;
			}
			set
			{
				this._disabledHighlightedTrigger = value;
			}
		}

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06002D40 RID: 11584 RVA: 0x0020A5C2 File Offset: 0x002087C2
		// (set) Token: 0x06002D41 RID: 11585 RVA: 0x0020A5CA File Offset: 0x002087CA
		public bool autoNavUp
		{
			get
			{
				return this._autoNavUp;
			}
			set
			{
				this._autoNavUp = value;
			}
		}

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06002D42 RID: 11586 RVA: 0x0020A5D3 File Offset: 0x002087D3
		// (set) Token: 0x06002D43 RID: 11587 RVA: 0x0020A5DB File Offset: 0x002087DB
		public bool autoNavDown
		{
			get
			{
				return this._autoNavDown;
			}
			set
			{
				this._autoNavDown = value;
			}
		}

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06002D44 RID: 11588 RVA: 0x0020A5E4 File Offset: 0x002087E4
		// (set) Token: 0x06002D45 RID: 11589 RVA: 0x0020A5EC File Offset: 0x002087EC
		public bool autoNavLeft
		{
			get
			{
				return this._autoNavLeft;
			}
			set
			{
				this._autoNavLeft = value;
			}
		}

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06002D46 RID: 11590 RVA: 0x0020A5F5 File Offset: 0x002087F5
		// (set) Token: 0x06002D47 RID: 11591 RVA: 0x0020A5FD File Offset: 0x002087FD
		public bool autoNavRight
		{
			get
			{
				return this._autoNavRight;
			}
			set
			{
				this._autoNavRight = value;
			}
		}

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06002D48 RID: 11592 RVA: 0x00209C64 File Offset: 0x00207E64
		private bool isDisabled
		{
			get
			{
				return !this.IsInteractable();
			}
		}

		// Token: 0x14000073 RID: 115
		// (add) Token: 0x06002D49 RID: 11593 RVA: 0x0020A608 File Offset: 0x00208808
		// (remove) Token: 0x06002D4A RID: 11594 RVA: 0x0020A640 File Offset: 0x00208840
		private event UnityAction _CancelEvent;

		// Token: 0x14000074 RID: 116
		// (add) Token: 0x06002D4B RID: 11595 RVA: 0x0020A675 File Offset: 0x00208875
		// (remove) Token: 0x06002D4C RID: 11596 RVA: 0x0020A67E File Offset: 0x0020887E
		public event UnityAction CancelEvent
		{
			add
			{
				this._CancelEvent += value;
			}
			remove
			{
				this._CancelEvent -= value;
			}
		}

		// Token: 0x06002D4D RID: 11597 RVA: 0x0020A688 File Offset: 0x00208888
		public override Selectable FindSelectableOnLeft()
		{
			if ((base.navigation.mode & 1) != null || this._autoNavLeft)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.left);
			}
			return base.FindSelectableOnLeft();
		}

		// Token: 0x06002D4E RID: 11598 RVA: 0x0020A6C8 File Offset: 0x002088C8
		public override Selectable FindSelectableOnRight()
		{
			if ((base.navigation.mode & 1) != null || this._autoNavRight)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.right);
			}
			return base.FindSelectableOnRight();
		}

		// Token: 0x06002D4F RID: 11599 RVA: 0x0020A708 File Offset: 0x00208908
		public override Selectable FindSelectableOnUp()
		{
			if ((base.navigation.mode & 2) != null || this._autoNavUp)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.up);
			}
			return base.FindSelectableOnUp();
		}

		// Token: 0x06002D50 RID: 11600 RVA: 0x0020A748 File Offset: 0x00208948
		public override Selectable FindSelectableOnDown()
		{
			if ((base.navigation.mode & 2) != null || this._autoNavDown)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.down);
			}
			return base.FindSelectableOnDown();
		}

		// Token: 0x06002D51 RID: 11601 RVA: 0x0020A787 File Offset: 0x00208987
		protected override void OnCanvasGroupChanged()
		{
			base.OnCanvasGroupChanged();
			if (EventSystem.current == null)
			{
				return;
			}
			this.EvaluateHightlightDisabled(EventSystem.current.currentSelectedGameObject == base.gameObject);
		}

		// Token: 0x06002D52 RID: 11602 RVA: 0x0020A7B8 File Offset: 0x002089B8
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (this.isHighlightDisabled)
			{
				Color disabledHighlightedColor = this._disabledHighlightedColor;
				Sprite disabledHighlightedSprite = this._disabledHighlightedSprite;
				string disabledHighlightedTrigger = this._disabledHighlightedTrigger;
				if (base.gameObject.activeInHierarchy)
				{
					switch (base.transition)
					{
					case 1:
						this.StartColorTween(disabledHighlightedColor * base.colors.colorMultiplier, instant);
						return;
					case 2:
						this.DoSpriteSwap(disabledHighlightedSprite);
						return;
					case 3:
						this.TriggerAnimation(disabledHighlightedTrigger);
						return;
					default:
						return;
					}
				}
			}
			else
			{
				base.DoStateTransition(state, instant);
			}
		}

		// Token: 0x06002D53 RID: 11603 RVA: 0x0020A840 File Offset: 0x00208A40
		private void StartColorTween(Color targetColor, bool instant)
		{
			if (base.targetGraphic == null)
			{
				return;
			}
			base.targetGraphic.CrossFadeColor(targetColor, instant ? 0f : base.colors.fadeDuration, true, true);
		}

		// Token: 0x06002D54 RID: 11604 RVA: 0x00209EEA File Offset: 0x002080EA
		private void DoSpriteSwap(Sprite newSprite)
		{
			if (base.image == null)
			{
				return;
			}
			base.image.overrideSprite = newSprite;
		}

		// Token: 0x06002D55 RID: 11605 RVA: 0x0020A884 File Offset: 0x00208A84
		private void TriggerAnimation(string triggername)
		{
			if (base.animator == null || !base.animator.enabled || !base.animator.isActiveAndEnabled || base.animator.runtimeAnimatorController == null || string.IsNullOrEmpty(triggername))
			{
				return;
			}
			base.animator.ResetTrigger(this._disabledHighlightedTrigger);
			base.animator.SetTrigger(triggername);
		}

		// Token: 0x06002D56 RID: 11606 RVA: 0x0020A8F2 File Offset: 0x00208AF2
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			this.EvaluateHightlightDisabled(true);
		}

		// Token: 0x06002D57 RID: 11607 RVA: 0x0020A902 File Offset: 0x00208B02
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			this.EvaluateHightlightDisabled(false);
		}

		// Token: 0x06002D58 RID: 11608 RVA: 0x0020A914 File Offset: 0x00208B14
		private void EvaluateHightlightDisabled(bool isSelected)
		{
			if (!isSelected)
			{
				if (this.isHighlightDisabled)
				{
					this.isHighlightDisabled = false;
					Selectable.SelectionState selectionState = this.isDisabled ? 4 : base.currentSelectionState;
					this.DoStateTransition(selectionState, false);
					return;
				}
			}
			else
			{
				if (!this.isDisabled)
				{
					return;
				}
				this.isHighlightDisabled = true;
				this.DoStateTransition(4, false);
			}
		}

		// Token: 0x06002D59 RID: 11609 RVA: 0x0020A966 File Offset: 0x00208B66
		public void OnCancel(BaseEventData eventData)
		{
			if (this._CancelEvent != null)
			{
				this._CancelEvent.Invoke();
			}
		}

		// Token: 0x04004906 RID: 18694
		[SerializeField]
		private Sprite _disabledHighlightedSprite;

		// Token: 0x04004907 RID: 18695
		[SerializeField]
		private Color _disabledHighlightedColor;

		// Token: 0x04004908 RID: 18696
		[SerializeField]
		private string _disabledHighlightedTrigger;

		// Token: 0x04004909 RID: 18697
		[SerializeField]
		private bool _autoNavUp = true;

		// Token: 0x0400490A RID: 18698
		[SerializeField]
		private bool _autoNavDown = true;

		// Token: 0x0400490B RID: 18699
		[SerializeField]
		private bool _autoNavLeft = true;

		// Token: 0x0400490C RID: 18700
		[SerializeField]
		private bool _autoNavRight = true;

		// Token: 0x0400490D RID: 18701
		private bool isHighlightDisabled;
	}
}
