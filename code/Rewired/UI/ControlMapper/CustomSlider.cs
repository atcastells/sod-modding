using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000873 RID: 2163
	[AddComponentMenu("")]
	public class CustomSlider : Slider, ICustomSelectable, ICancelHandler, IEventSystemHandler
	{
		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06002D19 RID: 11545 RVA: 0x0020A181 File Offset: 0x00208381
		// (set) Token: 0x06002D1A RID: 11546 RVA: 0x0020A189 File Offset: 0x00208389
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

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06002D1B RID: 11547 RVA: 0x0020A192 File Offset: 0x00208392
		// (set) Token: 0x06002D1C RID: 11548 RVA: 0x0020A19A File Offset: 0x0020839A
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

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06002D1D RID: 11549 RVA: 0x0020A1A3 File Offset: 0x002083A3
		// (set) Token: 0x06002D1E RID: 11550 RVA: 0x0020A1AB File Offset: 0x002083AB
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

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06002D1F RID: 11551 RVA: 0x0020A1B4 File Offset: 0x002083B4
		// (set) Token: 0x06002D20 RID: 11552 RVA: 0x0020A1BC File Offset: 0x002083BC
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

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06002D21 RID: 11553 RVA: 0x0020A1C5 File Offset: 0x002083C5
		// (set) Token: 0x06002D22 RID: 11554 RVA: 0x0020A1CD File Offset: 0x002083CD
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

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06002D23 RID: 11555 RVA: 0x0020A1D6 File Offset: 0x002083D6
		// (set) Token: 0x06002D24 RID: 11556 RVA: 0x0020A1DE File Offset: 0x002083DE
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

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06002D25 RID: 11557 RVA: 0x0020A1E7 File Offset: 0x002083E7
		// (set) Token: 0x06002D26 RID: 11558 RVA: 0x0020A1EF File Offset: 0x002083EF
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

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06002D27 RID: 11559 RVA: 0x00209C64 File Offset: 0x00207E64
		private bool isDisabled
		{
			get
			{
				return !this.IsInteractable();
			}
		}

		// Token: 0x14000071 RID: 113
		// (add) Token: 0x06002D28 RID: 11560 RVA: 0x0020A1F8 File Offset: 0x002083F8
		// (remove) Token: 0x06002D29 RID: 11561 RVA: 0x0020A230 File Offset: 0x00208430
		private event UnityAction _CancelEvent;

		// Token: 0x14000072 RID: 114
		// (add) Token: 0x06002D2A RID: 11562 RVA: 0x0020A265 File Offset: 0x00208465
		// (remove) Token: 0x06002D2B RID: 11563 RVA: 0x0020A26E File Offset: 0x0020846E
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

		// Token: 0x06002D2C RID: 11564 RVA: 0x0020A278 File Offset: 0x00208478
		public override Selectable FindSelectableOnLeft()
		{
			if ((base.navigation.mode & 1) != null || this._autoNavLeft)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.left);
			}
			return base.FindSelectableOnLeft();
		}

		// Token: 0x06002D2D RID: 11565 RVA: 0x0020A2B8 File Offset: 0x002084B8
		public override Selectable FindSelectableOnRight()
		{
			if ((base.navigation.mode & 1) != null || this._autoNavRight)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.right);
			}
			return base.FindSelectableOnRight();
		}

		// Token: 0x06002D2E RID: 11566 RVA: 0x0020A2F8 File Offset: 0x002084F8
		public override Selectable FindSelectableOnUp()
		{
			if ((base.navigation.mode & 2) != null || this._autoNavUp)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.up);
			}
			return base.FindSelectableOnUp();
		}

		// Token: 0x06002D2F RID: 11567 RVA: 0x0020A338 File Offset: 0x00208538
		public override Selectable FindSelectableOnDown()
		{
			if ((base.navigation.mode & 2) != null || this._autoNavDown)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.down);
			}
			return base.FindSelectableOnDown();
		}

		// Token: 0x06002D30 RID: 11568 RVA: 0x0020A377 File Offset: 0x00208577
		protected override void OnCanvasGroupChanged()
		{
			base.OnCanvasGroupChanged();
			if (EventSystem.current == null)
			{
				return;
			}
			this.EvaluateHightlightDisabled(EventSystem.current.currentSelectedGameObject == base.gameObject);
		}

		// Token: 0x06002D31 RID: 11569 RVA: 0x0020A3A8 File Offset: 0x002085A8
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

		// Token: 0x06002D32 RID: 11570 RVA: 0x0020A430 File Offset: 0x00208630
		private void StartColorTween(Color targetColor, bool instant)
		{
			if (base.targetGraphic == null)
			{
				return;
			}
			base.targetGraphic.CrossFadeColor(targetColor, instant ? 0f : base.colors.fadeDuration, true, true);
		}

		// Token: 0x06002D33 RID: 11571 RVA: 0x00209EEA File Offset: 0x002080EA
		private void DoSpriteSwap(Sprite newSprite)
		{
			if (base.image == null)
			{
				return;
			}
			base.image.overrideSprite = newSprite;
		}

		// Token: 0x06002D34 RID: 11572 RVA: 0x0020A474 File Offset: 0x00208674
		private void TriggerAnimation(string triggername)
		{
			if (base.animator == null || !base.animator.enabled || !base.animator.isActiveAndEnabled || base.animator.runtimeAnimatorController == null || string.IsNullOrEmpty(triggername))
			{
				return;
			}
			base.animator.ResetTrigger(this._disabledHighlightedTrigger);
			base.animator.SetTrigger(triggername);
		}

		// Token: 0x06002D35 RID: 11573 RVA: 0x0020A4E2 File Offset: 0x002086E2
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			this.EvaluateHightlightDisabled(true);
		}

		// Token: 0x06002D36 RID: 11574 RVA: 0x0020A4F2 File Offset: 0x002086F2
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			this.EvaluateHightlightDisabled(false);
		}

		// Token: 0x06002D37 RID: 11575 RVA: 0x0020A504 File Offset: 0x00208704
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

		// Token: 0x06002D38 RID: 11576 RVA: 0x0020A556 File Offset: 0x00208756
		public void OnCancel(BaseEventData eventData)
		{
			if (this._CancelEvent != null)
			{
				this._CancelEvent.Invoke();
			}
		}

		// Token: 0x040048FD RID: 18685
		[SerializeField]
		private Sprite _disabledHighlightedSprite;

		// Token: 0x040048FE RID: 18686
		[SerializeField]
		private Color _disabledHighlightedColor;

		// Token: 0x040048FF RID: 18687
		[SerializeField]
		private string _disabledHighlightedTrigger;

		// Token: 0x04004900 RID: 18688
		[SerializeField]
		private bool _autoNavUp = true;

		// Token: 0x04004901 RID: 18689
		[SerializeField]
		private bool _autoNavDown = true;

		// Token: 0x04004902 RID: 18690
		[SerializeField]
		private bool _autoNavLeft = true;

		// Token: 0x04004903 RID: 18691
		[SerializeField]
		private bool _autoNavRight = true;

		// Token: 0x04004904 RID: 18692
		private bool isHighlightDisabled;
	}
}
