using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000871 RID: 2161
	[AddComponentMenu("")]
	public class CustomButton : Button, ICustomSelectable, ICancelHandler, IEventSystemHandler
	{
		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06002CEE RID: 11502 RVA: 0x00209BED File Offset: 0x00207DED
		// (set) Token: 0x06002CEF RID: 11503 RVA: 0x00209BF5 File Offset: 0x00207DF5
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

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06002CF0 RID: 11504 RVA: 0x00209BFE File Offset: 0x00207DFE
		// (set) Token: 0x06002CF1 RID: 11505 RVA: 0x00209C06 File Offset: 0x00207E06
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

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06002CF2 RID: 11506 RVA: 0x00209C0F File Offset: 0x00207E0F
		// (set) Token: 0x06002CF3 RID: 11507 RVA: 0x00209C17 File Offset: 0x00207E17
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

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06002CF4 RID: 11508 RVA: 0x00209C20 File Offset: 0x00207E20
		// (set) Token: 0x06002CF5 RID: 11509 RVA: 0x00209C28 File Offset: 0x00207E28
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

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06002CF6 RID: 11510 RVA: 0x00209C31 File Offset: 0x00207E31
		// (set) Token: 0x06002CF7 RID: 11511 RVA: 0x00209C39 File Offset: 0x00207E39
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

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06002CF8 RID: 11512 RVA: 0x00209C42 File Offset: 0x00207E42
		// (set) Token: 0x06002CF9 RID: 11513 RVA: 0x00209C4A File Offset: 0x00207E4A
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

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06002CFA RID: 11514 RVA: 0x00209C53 File Offset: 0x00207E53
		// (set) Token: 0x06002CFB RID: 11515 RVA: 0x00209C5B File Offset: 0x00207E5B
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

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06002CFC RID: 11516 RVA: 0x00209C64 File Offset: 0x00207E64
		private bool isDisabled
		{
			get
			{
				return !this.IsInteractable();
			}
		}

		// Token: 0x1400006F RID: 111
		// (add) Token: 0x06002CFD RID: 11517 RVA: 0x00209C70 File Offset: 0x00207E70
		// (remove) Token: 0x06002CFE RID: 11518 RVA: 0x00209CA8 File Offset: 0x00207EA8
		private event UnityAction _CancelEvent;

		// Token: 0x14000070 RID: 112
		// (add) Token: 0x06002CFF RID: 11519 RVA: 0x00209CDD File Offset: 0x00207EDD
		// (remove) Token: 0x06002D00 RID: 11520 RVA: 0x00209CE6 File Offset: 0x00207EE6
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

		// Token: 0x06002D01 RID: 11521 RVA: 0x00209CF0 File Offset: 0x00207EF0
		public override Selectable FindSelectableOnLeft()
		{
			if ((base.navigation.mode & 1) != null || this._autoNavLeft)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.left);
			}
			return base.FindSelectableOnLeft();
		}

		// Token: 0x06002D02 RID: 11522 RVA: 0x00209D30 File Offset: 0x00207F30
		public override Selectable FindSelectableOnRight()
		{
			if ((base.navigation.mode & 1) != null || this._autoNavRight)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.right);
			}
			return base.FindSelectableOnRight();
		}

		// Token: 0x06002D03 RID: 11523 RVA: 0x00209D70 File Offset: 0x00207F70
		public override Selectable FindSelectableOnUp()
		{
			if ((base.navigation.mode & 2) != null || this._autoNavUp)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.up);
			}
			return base.FindSelectableOnUp();
		}

		// Token: 0x06002D04 RID: 11524 RVA: 0x00209DB0 File Offset: 0x00207FB0
		public override Selectable FindSelectableOnDown()
		{
			if ((base.navigation.mode & 2) != null || this._autoNavDown)
			{
				return UISelectionUtility.FindNextSelectable(this, base.transform, Vector3.down);
			}
			return base.FindSelectableOnDown();
		}

		// Token: 0x06002D05 RID: 11525 RVA: 0x00209DEF File Offset: 0x00207FEF
		protected override void OnCanvasGroupChanged()
		{
			base.OnCanvasGroupChanged();
			if (EventSystem.current == null)
			{
				return;
			}
			this.EvaluateHightlightDisabled(EventSystem.current.currentSelectedGameObject == base.gameObject);
		}

		// Token: 0x06002D06 RID: 11526 RVA: 0x00209E20 File Offset: 0x00208020
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

		// Token: 0x06002D07 RID: 11527 RVA: 0x00209EA8 File Offset: 0x002080A8
		private void StartColorTween(Color targetColor, bool instant)
		{
			if (base.targetGraphic == null)
			{
				return;
			}
			base.targetGraphic.CrossFadeColor(targetColor, instant ? 0f : base.colors.fadeDuration, true, true);
		}

		// Token: 0x06002D08 RID: 11528 RVA: 0x00209EEA File Offset: 0x002080EA
		private void DoSpriteSwap(Sprite newSprite)
		{
			if (base.image == null)
			{
				return;
			}
			base.image.overrideSprite = newSprite;
		}

		// Token: 0x06002D09 RID: 11529 RVA: 0x00209F08 File Offset: 0x00208108
		private void TriggerAnimation(string triggername)
		{
			if (base.animator == null || !base.animator.enabled || !base.animator.isActiveAndEnabled || base.animator.runtimeAnimatorController == null || string.IsNullOrEmpty(triggername))
			{
				return;
			}
			base.animator.ResetTrigger(this._disabledHighlightedTrigger);
			base.animator.SetTrigger(triggername);
		}

		// Token: 0x06002D0A RID: 11530 RVA: 0x00209F76 File Offset: 0x00208176
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			this.EvaluateHightlightDisabled(true);
		}

		// Token: 0x06002D0B RID: 11531 RVA: 0x00209F86 File Offset: 0x00208186
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			this.EvaluateHightlightDisabled(false);
		}

		// Token: 0x06002D0C RID: 11532 RVA: 0x00209F96 File Offset: 0x00208196
		private void Press()
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			base.onClick.Invoke();
		}

		// Token: 0x06002D0D RID: 11533 RVA: 0x00209FB4 File Offset: 0x002081B4
		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (eventData.button != null)
			{
				return;
			}
			this.Press();
			if (!this.IsActive() || !this.IsInteractable())
			{
				this.isHighlightDisabled = true;
				this.DoStateTransition(4, false);
			}
		}

		// Token: 0x06002D0E RID: 11534 RVA: 0x0020A000 File Offset: 0x00208200
		public override void OnSubmit(BaseEventData eventData)
		{
			this.Press();
			if (!this.IsActive() || !this.IsInteractable())
			{
				this.isHighlightDisabled = true;
				this.DoStateTransition(4, false);
				return;
			}
			this.DoStateTransition(2, false);
			base.StartCoroutine(this.OnFinishSubmit());
		}

		// Token: 0x06002D0F RID: 11535 RVA: 0x0020A03D File Offset: 0x0020823D
		private IEnumerator OnFinishSubmit()
		{
			float fadeTime = base.colors.fadeDuration;
			float elapsedTime = 0f;
			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			this.DoStateTransition(base.currentSelectionState, false);
			yield break;
		}

		// Token: 0x06002D10 RID: 11536 RVA: 0x0020A04C File Offset: 0x0020824C
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

		// Token: 0x06002D11 RID: 11537 RVA: 0x0020A09E File Offset: 0x0020829E
		public void OnCancel(BaseEventData eventData)
		{
			if (this._CancelEvent != null)
			{
				this._CancelEvent.Invoke();
			}
		}

		// Token: 0x040048EF RID: 18671
		[SerializeField]
		private Sprite _disabledHighlightedSprite;

		// Token: 0x040048F0 RID: 18672
		[SerializeField]
		private Color _disabledHighlightedColor;

		// Token: 0x040048F1 RID: 18673
		[SerializeField]
		private string _disabledHighlightedTrigger;

		// Token: 0x040048F2 RID: 18674
		[SerializeField]
		private bool _autoNavUp = true;

		// Token: 0x040048F3 RID: 18675
		[SerializeField]
		private bool _autoNavDown = true;

		// Token: 0x040048F4 RID: 18676
		[SerializeField]
		private bool _autoNavLeft = true;

		// Token: 0x040048F5 RID: 18677
		[SerializeField]
		private bool _autoNavRight = true;

		// Token: 0x040048F6 RID: 18678
		private bool isHighlightDisabled;
	}
}
