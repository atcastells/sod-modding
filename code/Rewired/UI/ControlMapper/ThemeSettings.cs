using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000885 RID: 2181
	[Serializable]
	public class ThemeSettings : ScriptableObject
	{
		// Token: 0x06002E30 RID: 11824 RVA: 0x0020BD50 File Offset: 0x00209F50
		public void Apply(ThemedElement.ElementInfo[] elementInfo)
		{
			if (elementInfo == null)
			{
				return;
			}
			for (int i = 0; i < elementInfo.Length; i++)
			{
				if (elementInfo[i] != null)
				{
					this.Apply(elementInfo[i].themeClass, elementInfo[i].component);
				}
			}
		}

		// Token: 0x06002E31 RID: 11825 RVA: 0x0020BD8C File Offset: 0x00209F8C
		private void Apply(string themeClass, Component component)
		{
			if (component as Selectable != null)
			{
				this.Apply(themeClass, (Selectable)component);
				return;
			}
			if (component as Image != null)
			{
				this.Apply(themeClass, (Image)component);
				return;
			}
			if (component as TMP_Text != null)
			{
				this.Apply(themeClass, (TMP_Text)component);
				return;
			}
			if (component as UIImageHelper != null)
			{
				this.Apply(themeClass, (UIImageHelper)component);
				return;
			}
		}

		// Token: 0x06002E32 RID: 11826 RVA: 0x0020BE0C File Offset: 0x0020A00C
		private void Apply(string themeClass, Selectable item)
		{
			if (item == null)
			{
				return;
			}
			ThemeSettings.SelectableSettings_Base selectableSettings_Base;
			if (item as Button != null)
			{
				if (themeClass == "inputGridField")
				{
					selectableSettings_Base = this._inputGridFieldSettings;
				}
				else
				{
					selectableSettings_Base = this._buttonSettings;
				}
			}
			else if (item as Scrollbar != null)
			{
				selectableSettings_Base = this._scrollbarSettings;
			}
			else if (item as Slider != null)
			{
				selectableSettings_Base = this._sliderSettings;
			}
			else if (item as Toggle != null)
			{
				if (themeClass == "button")
				{
					selectableSettings_Base = this._buttonSettings;
				}
				else
				{
					selectableSettings_Base = this._selectableSettings;
				}
			}
			else
			{
				selectableSettings_Base = this._selectableSettings;
			}
			selectableSettings_Base.Apply(item);
		}

		// Token: 0x06002E33 RID: 11827 RVA: 0x0020BEBC File Offset: 0x0020A0BC
		private void Apply(string themeClass, Image item)
		{
			if (item == null)
			{
				return;
			}
			uint num = <PrivateImplementationDetails>.ComputeStringHash(themeClass);
			if (num <= 2822822017U)
			{
				if (num <= 665291243U)
				{
					if (num != 106194061U)
					{
						if (num != 283896133U)
						{
							if (num != 665291243U)
							{
								return;
							}
							if (!(themeClass == "calibrationBackground"))
							{
								return;
							}
							if (this._calibrationBackground != null)
							{
								this._calibrationBackground.CopyTo(item);
								return;
							}
						}
						else
						{
							if (!(themeClass == "popupWindow"))
							{
								return;
							}
							if (this._popupWindowBackground != null)
							{
								this._popupWindowBackground.CopyTo(item);
								return;
							}
						}
					}
					else
					{
						if (!(themeClass == "invertToggleButtonBackground"))
						{
							return;
						}
						if (this._buttonSettings != null)
						{
							this._buttonSettings.imageSettings.CopyTo(item);
						}
					}
				}
				else if (num != 2579191547U)
				{
					if (num != 2601460036U)
					{
						if (num != 2822822017U)
						{
							return;
						}
						if (!(themeClass == "invertToggle"))
						{
							return;
						}
						if (this._invertToggle != null)
						{
							this._invertToggle.CopyTo(item);
							return;
						}
					}
					else
					{
						if (!(themeClass == "area"))
						{
							return;
						}
						if (this._areaBackground != null)
						{
							this._areaBackground.CopyTo(item);
							return;
						}
					}
				}
				else
				{
					if (!(themeClass == "calibrationDeadzone"))
					{
						return;
					}
					if (this._calibrationDeadzone != null)
					{
						this._calibrationDeadzone.CopyTo(item);
						return;
					}
				}
			}
			else if (num <= 3490313510U)
			{
				if (num != 2998767316U)
				{
					if (num != 3338297968U)
					{
						if (num != 3490313510U)
						{
							return;
						}
						if (!(themeClass == "calibrationRawValueMarker"))
						{
							return;
						}
						if (this._calibrationRawValueMarker != null)
						{
							this._calibrationRawValueMarker.CopyTo(item);
							return;
						}
					}
					else
					{
						if (!(themeClass == "calibrationCalibratedZeroMarker"))
						{
							return;
						}
						if (this._calibrationCalibratedZeroMarker != null)
						{
							this._calibrationCalibratedZeroMarker.CopyTo(item);
							return;
						}
					}
				}
				else
				{
					if (!(themeClass == "mainWindow"))
					{
						return;
					}
					if (this._mainWindowBackground != null)
					{
						this._mainWindowBackground.CopyTo(item);
						return;
					}
				}
			}
			else if (num != 3776179782U)
			{
				if (num != 3836396811U)
				{
					if (num != 3911450241U)
					{
						return;
					}
					if (!(themeClass == "invertToggleBackground"))
					{
						return;
					}
					if (this._inputGridFieldSettings != null)
					{
						this._inputGridFieldSettings.imageSettings.CopyTo(item);
						return;
					}
				}
				else
				{
					if (!(themeClass == "calibrationZeroMarker"))
					{
						return;
					}
					if (this._calibrationZeroMarker != null)
					{
						this._calibrationZeroMarker.CopyTo(item);
						return;
					}
				}
			}
			else
			{
				if (!(themeClass == "calibrationValueMarker"))
				{
					return;
				}
				if (this._calibrationValueMarker != null)
				{
					this._calibrationValueMarker.CopyTo(item);
					return;
				}
			}
		}

		// Token: 0x06002E34 RID: 11828 RVA: 0x0020C14C File Offset: 0x0020A34C
		private void Apply(string themeClass, TMP_Text item)
		{
			if (item == null)
			{
				return;
			}
			ThemeSettings.TextSettings textSettings;
			if (!(themeClass == "button"))
			{
				if (!(themeClass == "inputGridField"))
				{
					textSettings = this._textSettings;
				}
				else
				{
					textSettings = this._inputGridFieldTextSettings;
				}
			}
			else
			{
				textSettings = this._buttonTextSettings;
			}
			if (textSettings.font != null)
			{
				item.font = textSettings.font;
			}
			item.color = textSettings.color;
			item.lineSpacing = textSettings.lineSpacing;
			if (textSettings.sizeMultiplier != 1f)
			{
				item.fontSize = (float)((int)(item.fontSize * textSettings.sizeMultiplier));
				item.fontSizeMax = (float)((int)(item.fontSizeMax * textSettings.sizeMultiplier));
				item.fontSizeMin = (float)((int)(item.fontSizeMin * textSettings.sizeMultiplier));
			}
			item.characterSpacing = textSettings.chracterSpacing;
			item.wordSpacing = textSettings.wordSpacing;
			if (textSettings.style != ThemeSettings.FontStyleOverride.Default)
			{
				item.fontStyle = ThemeSettings.GetFontStyle(textSettings.style);
			}
		}

		// Token: 0x06002E35 RID: 11829 RVA: 0x0020C247 File Offset: 0x0020A447
		private void Apply(string themeClass, UIImageHelper item)
		{
			if (item == null)
			{
				return;
			}
			item.SetEnabledStateColor(this._invertToggle.color);
			item.SetDisabledStateColor(this._invertToggleDisabledColor);
			item.Refresh();
		}

		// Token: 0x06002E36 RID: 11830 RVA: 0x0020C276 File Offset: 0x0020A476
		private static FontStyles GetFontStyle(ThemeSettings.FontStyleOverride style)
		{
			switch (style)
			{
			case ThemeSettings.FontStyleOverride.Default:
			case ThemeSettings.FontStyleOverride.Normal:
				return 0;
			case ThemeSettings.FontStyleOverride.Bold:
				return 1;
			case ThemeSettings.FontStyleOverride.Italic:
				return 2;
			case ThemeSettings.FontStyleOverride.BoldAndItalic:
				return 3;
			default:
				throw new NotImplementedException();
			}
		}

		// Token: 0x0400497E RID: 18814
		[SerializeField]
		private ThemeSettings.ImageSettings _mainWindowBackground;

		// Token: 0x0400497F RID: 18815
		[SerializeField]
		private ThemeSettings.ImageSettings _popupWindowBackground;

		// Token: 0x04004980 RID: 18816
		[SerializeField]
		private ThemeSettings.ImageSettings _areaBackground;

		// Token: 0x04004981 RID: 18817
		[SerializeField]
		private ThemeSettings.SelectableSettings _selectableSettings;

		// Token: 0x04004982 RID: 18818
		[SerializeField]
		private ThemeSettings.SelectableSettings _buttonSettings;

		// Token: 0x04004983 RID: 18819
		[SerializeField]
		private ThemeSettings.SelectableSettings _inputGridFieldSettings;

		// Token: 0x04004984 RID: 18820
		[SerializeField]
		private ThemeSettings.ScrollbarSettings _scrollbarSettings;

		// Token: 0x04004985 RID: 18821
		[SerializeField]
		private ThemeSettings.SliderSettings _sliderSettings;

		// Token: 0x04004986 RID: 18822
		[SerializeField]
		private ThemeSettings.ImageSettings _invertToggle;

		// Token: 0x04004987 RID: 18823
		[SerializeField]
		private Color _invertToggleDisabledColor;

		// Token: 0x04004988 RID: 18824
		[SerializeField]
		private ThemeSettings.ImageSettings _calibrationBackground;

		// Token: 0x04004989 RID: 18825
		[SerializeField]
		private ThemeSettings.ImageSettings _calibrationValueMarker;

		// Token: 0x0400498A RID: 18826
		[SerializeField]
		private ThemeSettings.ImageSettings _calibrationRawValueMarker;

		// Token: 0x0400498B RID: 18827
		[SerializeField]
		private ThemeSettings.ImageSettings _calibrationZeroMarker;

		// Token: 0x0400498C RID: 18828
		[SerializeField]
		private ThemeSettings.ImageSettings _calibrationCalibratedZeroMarker;

		// Token: 0x0400498D RID: 18829
		[SerializeField]
		private ThemeSettings.ImageSettings _calibrationDeadzone;

		// Token: 0x0400498E RID: 18830
		[SerializeField]
		private ThemeSettings.TextSettings _textSettings;

		// Token: 0x0400498F RID: 18831
		[SerializeField]
		private ThemeSettings.TextSettings _buttonTextSettings;

		// Token: 0x04004990 RID: 18832
		[SerializeField]
		private ThemeSettings.TextSettings _inputGridFieldTextSettings;

		// Token: 0x02000886 RID: 2182
		[Serializable]
		private abstract class SelectableSettings_Base
		{
			// Token: 0x17000498 RID: 1176
			// (get) Token: 0x06002E38 RID: 11832 RVA: 0x0020C2A1 File Offset: 0x0020A4A1
			public Selectable.Transition transition
			{
				get
				{
					return this._transition;
				}
			}

			// Token: 0x17000499 RID: 1177
			// (get) Token: 0x06002E39 RID: 11833 RVA: 0x0020C2A9 File Offset: 0x0020A4A9
			public ThemeSettings.CustomColorBlock selectableColors
			{
				get
				{
					return this._colors;
				}
			}

			// Token: 0x1700049A RID: 1178
			// (get) Token: 0x06002E3A RID: 11834 RVA: 0x0020C2B1 File Offset: 0x0020A4B1
			public ThemeSettings.CustomSpriteState spriteState
			{
				get
				{
					return this._spriteState;
				}
			}

			// Token: 0x1700049B RID: 1179
			// (get) Token: 0x06002E3B RID: 11835 RVA: 0x0020C2B9 File Offset: 0x0020A4B9
			public ThemeSettings.CustomAnimationTriggers animationTriggers
			{
				get
				{
					return this._animationTriggers;
				}
			}

			// Token: 0x06002E3C RID: 11836 RVA: 0x0020C2C4 File Offset: 0x0020A4C4
			public virtual void Apply(Selectable item)
			{
				Selectable.Transition transition = this._transition;
				bool flag = item.transition != transition;
				item.transition = transition;
				ICustomSelectable customSelectable = item as ICustomSelectable;
				if (transition == 1)
				{
					ThemeSettings.CustomColorBlock colors = this._colors;
					colors.fadeDuration = 0f;
					item.colors = colors;
					colors.fadeDuration = this._colors.fadeDuration;
					item.colors = colors;
					if (customSelectable != null)
					{
						customSelectable.disabledHighlightedColor = colors.disabledHighlightedColor;
					}
				}
				else if (transition == 2)
				{
					item.spriteState = this._spriteState;
					if (customSelectable != null)
					{
						customSelectable.disabledHighlightedSprite = this._spriteState.disabledHighlightedSprite;
					}
				}
				else if (transition == 3)
				{
					item.animationTriggers.disabledTrigger = this._animationTriggers.disabledTrigger;
					item.animationTriggers.highlightedTrigger = this._animationTriggers.highlightedTrigger;
					item.animationTriggers.normalTrigger = this._animationTriggers.normalTrigger;
					item.animationTriggers.pressedTrigger = this._animationTriggers.pressedTrigger;
					if (customSelectable != null)
					{
						customSelectable.disabledHighlightedTrigger = this._animationTriggers.disabledHighlightedTrigger;
					}
				}
				if (flag)
				{
					item.targetGraphic.CrossFadeColor(item.targetGraphic.color, 0f, true, true);
				}
			}

			// Token: 0x04004991 RID: 18833
			[SerializeField]
			protected Selectable.Transition _transition;

			// Token: 0x04004992 RID: 18834
			[SerializeField]
			protected ThemeSettings.CustomColorBlock _colors;

			// Token: 0x04004993 RID: 18835
			[SerializeField]
			protected ThemeSettings.CustomSpriteState _spriteState;

			// Token: 0x04004994 RID: 18836
			[SerializeField]
			protected ThemeSettings.CustomAnimationTriggers _animationTriggers;
		}

		// Token: 0x02000887 RID: 2183
		[Serializable]
		private class SelectableSettings : ThemeSettings.SelectableSettings_Base
		{
			// Token: 0x1700049C RID: 1180
			// (get) Token: 0x06002E3E RID: 11838 RVA: 0x0020C408 File Offset: 0x0020A608
			public ThemeSettings.ImageSettings imageSettings
			{
				get
				{
					return this._imageSettings;
				}
			}

			// Token: 0x06002E3F RID: 11839 RVA: 0x0020C410 File Offset: 0x0020A610
			public override void Apply(Selectable item)
			{
				if (item == null)
				{
					return;
				}
				base.Apply(item);
				if (this._imageSettings != null)
				{
					this._imageSettings.CopyTo(item.targetGraphic as Image);
				}
			}

			// Token: 0x04004995 RID: 18837
			[SerializeField]
			private ThemeSettings.ImageSettings _imageSettings;
		}

		// Token: 0x02000888 RID: 2184
		[Serializable]
		private class SliderSettings : ThemeSettings.SelectableSettings_Base
		{
			// Token: 0x1700049D RID: 1181
			// (get) Token: 0x06002E41 RID: 11841 RVA: 0x0020C449 File Offset: 0x0020A649
			public ThemeSettings.ImageSettings handleImageSettings
			{
				get
				{
					return this._handleImageSettings;
				}
			}

			// Token: 0x1700049E RID: 1182
			// (get) Token: 0x06002E42 RID: 11842 RVA: 0x0020C451 File Offset: 0x0020A651
			public ThemeSettings.ImageSettings fillImageSettings
			{
				get
				{
					return this._fillImageSettings;
				}
			}

			// Token: 0x1700049F RID: 1183
			// (get) Token: 0x06002E43 RID: 11843 RVA: 0x0020C459 File Offset: 0x0020A659
			public ThemeSettings.ImageSettings backgroundImageSettings
			{
				get
				{
					return this._backgroundImageSettings;
				}
			}

			// Token: 0x06002E44 RID: 11844 RVA: 0x0020C464 File Offset: 0x0020A664
			private void Apply(Slider item)
			{
				if (item == null)
				{
					return;
				}
				if (this._handleImageSettings != null)
				{
					this._handleImageSettings.CopyTo(item.targetGraphic as Image);
				}
				if (this._fillImageSettings != null)
				{
					RectTransform fillRect = item.fillRect;
					if (fillRect != null)
					{
						this._fillImageSettings.CopyTo(fillRect.GetComponent<Image>());
					}
				}
				if (this._backgroundImageSettings != null)
				{
					Transform transform = item.transform.Find("Background");
					if (transform != null)
					{
						this._backgroundImageSettings.CopyTo(transform.GetComponent<Image>());
					}
				}
			}

			// Token: 0x06002E45 RID: 11845 RVA: 0x0020C4F5 File Offset: 0x0020A6F5
			public override void Apply(Selectable item)
			{
				base.Apply(item);
				this.Apply(item as Slider);
			}

			// Token: 0x04004996 RID: 18838
			[SerializeField]
			private ThemeSettings.ImageSettings _handleImageSettings;

			// Token: 0x04004997 RID: 18839
			[SerializeField]
			private ThemeSettings.ImageSettings _fillImageSettings;

			// Token: 0x04004998 RID: 18840
			[SerializeField]
			private ThemeSettings.ImageSettings _backgroundImageSettings;
		}

		// Token: 0x02000889 RID: 2185
		[Serializable]
		private class ScrollbarSettings : ThemeSettings.SelectableSettings_Base
		{
			// Token: 0x170004A0 RID: 1184
			// (get) Token: 0x06002E47 RID: 11847 RVA: 0x0020C50A File Offset: 0x0020A70A
			public ThemeSettings.ImageSettings handle
			{
				get
				{
					return this._handleImageSettings;
				}
			}

			// Token: 0x170004A1 RID: 1185
			// (get) Token: 0x06002E48 RID: 11848 RVA: 0x0020C512 File Offset: 0x0020A712
			public ThemeSettings.ImageSettings background
			{
				get
				{
					return this._backgroundImageSettings;
				}
			}

			// Token: 0x06002E49 RID: 11849 RVA: 0x0020C51C File Offset: 0x0020A71C
			private void Apply(Scrollbar item)
			{
				if (item == null)
				{
					return;
				}
				if (this._handleImageSettings != null)
				{
					this._handleImageSettings.CopyTo(item.targetGraphic as Image);
				}
				if (this._backgroundImageSettings != null)
				{
					this._backgroundImageSettings.CopyTo(item.GetComponent<Image>());
				}
			}

			// Token: 0x06002E4A RID: 11850 RVA: 0x0020C56A File Offset: 0x0020A76A
			public override void Apply(Selectable item)
			{
				base.Apply(item);
				this.Apply(item as Scrollbar);
			}

			// Token: 0x04004999 RID: 18841
			[SerializeField]
			private ThemeSettings.ImageSettings _handleImageSettings;

			// Token: 0x0400499A RID: 18842
			[SerializeField]
			private ThemeSettings.ImageSettings _backgroundImageSettings;
		}

		// Token: 0x0200088A RID: 2186
		[Serializable]
		private class ImageSettings
		{
			// Token: 0x170004A2 RID: 1186
			// (get) Token: 0x06002E4C RID: 11852 RVA: 0x0020C57F File Offset: 0x0020A77F
			public Color color
			{
				get
				{
					return this._color;
				}
			}

			// Token: 0x170004A3 RID: 1187
			// (get) Token: 0x06002E4D RID: 11853 RVA: 0x0020C587 File Offset: 0x0020A787
			public Sprite sprite
			{
				get
				{
					return this._sprite;
				}
			}

			// Token: 0x170004A4 RID: 1188
			// (get) Token: 0x06002E4E RID: 11854 RVA: 0x0020C58F File Offset: 0x0020A78F
			public Material materal
			{
				get
				{
					return this._materal;
				}
			}

			// Token: 0x170004A5 RID: 1189
			// (get) Token: 0x06002E4F RID: 11855 RVA: 0x0020C597 File Offset: 0x0020A797
			public Image.Type type
			{
				get
				{
					return this._type;
				}
			}

			// Token: 0x170004A6 RID: 1190
			// (get) Token: 0x06002E50 RID: 11856 RVA: 0x0020C59F File Offset: 0x0020A79F
			public bool preserveAspect
			{
				get
				{
					return this._preserveAspect;
				}
			}

			// Token: 0x170004A7 RID: 1191
			// (get) Token: 0x06002E51 RID: 11857 RVA: 0x0020C5A7 File Offset: 0x0020A7A7
			public bool fillCenter
			{
				get
				{
					return this._fillCenter;
				}
			}

			// Token: 0x170004A8 RID: 1192
			// (get) Token: 0x06002E52 RID: 11858 RVA: 0x0020C5AF File Offset: 0x0020A7AF
			public Image.FillMethod fillMethod
			{
				get
				{
					return this._fillMethod;
				}
			}

			// Token: 0x170004A9 RID: 1193
			// (get) Token: 0x06002E53 RID: 11859 RVA: 0x0020C5B7 File Offset: 0x0020A7B7
			public float fillAmout
			{
				get
				{
					return this._fillAmout;
				}
			}

			// Token: 0x170004AA RID: 1194
			// (get) Token: 0x06002E54 RID: 11860 RVA: 0x0020C5BF File Offset: 0x0020A7BF
			public bool fillClockwise
			{
				get
				{
					return this._fillClockwise;
				}
			}

			// Token: 0x170004AB RID: 1195
			// (get) Token: 0x06002E55 RID: 11861 RVA: 0x0020C5C7 File Offset: 0x0020A7C7
			public int fillOrigin
			{
				get
				{
					return this._fillOrigin;
				}
			}

			// Token: 0x06002E56 RID: 11862 RVA: 0x0020C5D0 File Offset: 0x0020A7D0
			public virtual void CopyTo(Image image)
			{
				if (image == null)
				{
					return;
				}
				image.color = this._color;
				image.sprite = this._sprite;
				image.material = this._materal;
				image.type = this._type;
				image.preserveAspect = this._preserveAspect;
				image.fillCenter = this._fillCenter;
				image.fillMethod = this._fillMethod;
				image.fillAmount = this._fillAmout;
				image.fillClockwise = this._fillClockwise;
				image.fillOrigin = this._fillOrigin;
			}

			// Token: 0x0400499B RID: 18843
			[SerializeField]
			private Color _color = Color.white;

			// Token: 0x0400499C RID: 18844
			[SerializeField]
			private Sprite _sprite;

			// Token: 0x0400499D RID: 18845
			[SerializeField]
			private Material _materal;

			// Token: 0x0400499E RID: 18846
			[SerializeField]
			private Image.Type _type;

			// Token: 0x0400499F RID: 18847
			[SerializeField]
			private bool _preserveAspect;

			// Token: 0x040049A0 RID: 18848
			[SerializeField]
			private bool _fillCenter;

			// Token: 0x040049A1 RID: 18849
			[SerializeField]
			private Image.FillMethod _fillMethod;

			// Token: 0x040049A2 RID: 18850
			[SerializeField]
			private float _fillAmout;

			// Token: 0x040049A3 RID: 18851
			[SerializeField]
			private bool _fillClockwise;

			// Token: 0x040049A4 RID: 18852
			[SerializeField]
			private int _fillOrigin;
		}

		// Token: 0x0200088B RID: 2187
		[Serializable]
		private struct CustomColorBlock
		{
			// Token: 0x170004AC RID: 1196
			// (get) Token: 0x06002E58 RID: 11864 RVA: 0x0020C672 File Offset: 0x0020A872
			// (set) Token: 0x06002E59 RID: 11865 RVA: 0x0020C67A File Offset: 0x0020A87A
			public float colorMultiplier
			{
				get
				{
					return this.m_ColorMultiplier;
				}
				set
				{
					this.m_ColorMultiplier = value;
				}
			}

			// Token: 0x170004AD RID: 1197
			// (get) Token: 0x06002E5A RID: 11866 RVA: 0x0020C683 File Offset: 0x0020A883
			// (set) Token: 0x06002E5B RID: 11867 RVA: 0x0020C68B File Offset: 0x0020A88B
			public Color disabledColor
			{
				get
				{
					return this.m_DisabledColor;
				}
				set
				{
					this.m_DisabledColor = value;
				}
			}

			// Token: 0x170004AE RID: 1198
			// (get) Token: 0x06002E5C RID: 11868 RVA: 0x0020C694 File Offset: 0x0020A894
			// (set) Token: 0x06002E5D RID: 11869 RVA: 0x0020C69C File Offset: 0x0020A89C
			public float fadeDuration
			{
				get
				{
					return this.m_FadeDuration;
				}
				set
				{
					this.m_FadeDuration = value;
				}
			}

			// Token: 0x170004AF RID: 1199
			// (get) Token: 0x06002E5E RID: 11870 RVA: 0x0020C6A5 File Offset: 0x0020A8A5
			// (set) Token: 0x06002E5F RID: 11871 RVA: 0x0020C6AD File Offset: 0x0020A8AD
			public Color highlightedColor
			{
				get
				{
					return this.m_HighlightedColor;
				}
				set
				{
					this.m_HighlightedColor = value;
				}
			}

			// Token: 0x170004B0 RID: 1200
			// (get) Token: 0x06002E60 RID: 11872 RVA: 0x0020C6B6 File Offset: 0x0020A8B6
			// (set) Token: 0x06002E61 RID: 11873 RVA: 0x0020C6BE File Offset: 0x0020A8BE
			public Color normalColor
			{
				get
				{
					return this.m_NormalColor;
				}
				set
				{
					this.m_NormalColor = value;
				}
			}

			// Token: 0x170004B1 RID: 1201
			// (get) Token: 0x06002E62 RID: 11874 RVA: 0x0020C6C7 File Offset: 0x0020A8C7
			// (set) Token: 0x06002E63 RID: 11875 RVA: 0x0020C6CF File Offset: 0x0020A8CF
			public Color pressedColor
			{
				get
				{
					return this.m_PressedColor;
				}
				set
				{
					this.m_PressedColor = value;
				}
			}

			// Token: 0x170004B2 RID: 1202
			// (get) Token: 0x06002E64 RID: 11876 RVA: 0x0020C6D8 File Offset: 0x0020A8D8
			// (set) Token: 0x06002E65 RID: 11877 RVA: 0x0020C6E0 File Offset: 0x0020A8E0
			public Color selectedColor
			{
				get
				{
					return this.m_SelectedColor;
				}
				set
				{
					this.m_SelectedColor = value;
				}
			}

			// Token: 0x170004B3 RID: 1203
			// (get) Token: 0x06002E66 RID: 11878 RVA: 0x0020C6E9 File Offset: 0x0020A8E9
			// (set) Token: 0x06002E67 RID: 11879 RVA: 0x0020C6F1 File Offset: 0x0020A8F1
			public Color disabledHighlightedColor
			{
				get
				{
					return this.m_DisabledHighlightedColor;
				}
				set
				{
					this.m_DisabledHighlightedColor = value;
				}
			}

			// Token: 0x06002E68 RID: 11880 RVA: 0x0020C6FC File Offset: 0x0020A8FC
			public static implicit operator ColorBlock(ThemeSettings.CustomColorBlock item)
			{
				ColorBlock result = default(ColorBlock);
				result.selectedColor = item.m_SelectedColor;
				result.colorMultiplier = item.m_ColorMultiplier;
				result.disabledColor = item.m_DisabledColor;
				result.fadeDuration = item.m_FadeDuration;
				result.highlightedColor = item.m_HighlightedColor;
				result.normalColor = item.m_NormalColor;
				result.pressedColor = item.m_PressedColor;
				return result;
			}

			// Token: 0x040049A5 RID: 18853
			[SerializeField]
			private float m_ColorMultiplier;

			// Token: 0x040049A6 RID: 18854
			[SerializeField]
			private Color m_DisabledColor;

			// Token: 0x040049A7 RID: 18855
			[SerializeField]
			private float m_FadeDuration;

			// Token: 0x040049A8 RID: 18856
			[SerializeField]
			private Color m_HighlightedColor;

			// Token: 0x040049A9 RID: 18857
			[SerializeField]
			private Color m_NormalColor;

			// Token: 0x040049AA RID: 18858
			[SerializeField]
			private Color m_PressedColor;

			// Token: 0x040049AB RID: 18859
			[SerializeField]
			private Color m_SelectedColor;

			// Token: 0x040049AC RID: 18860
			[SerializeField]
			private Color m_DisabledHighlightedColor;
		}

		// Token: 0x0200088C RID: 2188
		[Serializable]
		private struct CustomSpriteState
		{
			// Token: 0x170004B4 RID: 1204
			// (get) Token: 0x06002E69 RID: 11881 RVA: 0x0020C76D File Offset: 0x0020A96D
			// (set) Token: 0x06002E6A RID: 11882 RVA: 0x0020C775 File Offset: 0x0020A975
			public Sprite disabledSprite
			{
				get
				{
					return this.m_DisabledSprite;
				}
				set
				{
					this.m_DisabledSprite = value;
				}
			}

			// Token: 0x170004B5 RID: 1205
			// (get) Token: 0x06002E6B RID: 11883 RVA: 0x0020C77E File Offset: 0x0020A97E
			// (set) Token: 0x06002E6C RID: 11884 RVA: 0x0020C786 File Offset: 0x0020A986
			public Sprite highlightedSprite
			{
				get
				{
					return this.m_HighlightedSprite;
				}
				set
				{
					this.m_HighlightedSprite = value;
				}
			}

			// Token: 0x170004B6 RID: 1206
			// (get) Token: 0x06002E6D RID: 11885 RVA: 0x0020C78F File Offset: 0x0020A98F
			// (set) Token: 0x06002E6E RID: 11886 RVA: 0x0020C797 File Offset: 0x0020A997
			public Sprite pressedSprite
			{
				get
				{
					return this.m_PressedSprite;
				}
				set
				{
					this.m_PressedSprite = value;
				}
			}

			// Token: 0x170004B7 RID: 1207
			// (get) Token: 0x06002E6F RID: 11887 RVA: 0x0020C7A0 File Offset: 0x0020A9A0
			// (set) Token: 0x06002E70 RID: 11888 RVA: 0x0020C7A8 File Offset: 0x0020A9A8
			public Sprite selectedSprite
			{
				get
				{
					return this.m_SelectedSprite;
				}
				set
				{
					this.m_SelectedSprite = value;
				}
			}

			// Token: 0x170004B8 RID: 1208
			// (get) Token: 0x06002E71 RID: 11889 RVA: 0x0020C7B1 File Offset: 0x0020A9B1
			// (set) Token: 0x06002E72 RID: 11890 RVA: 0x0020C7B9 File Offset: 0x0020A9B9
			public Sprite disabledHighlightedSprite
			{
				get
				{
					return this.m_DisabledHighlightedSprite;
				}
				set
				{
					this.m_DisabledHighlightedSprite = value;
				}
			}

			// Token: 0x06002E73 RID: 11891 RVA: 0x0020C7C4 File Offset: 0x0020A9C4
			public static implicit operator SpriteState(ThemeSettings.CustomSpriteState item)
			{
				SpriteState result = default(SpriteState);
				result.selectedSprite = item.m_SelectedSprite;
				result.disabledSprite = item.m_DisabledSprite;
				result.highlightedSprite = item.m_HighlightedSprite;
				result.pressedSprite = item.m_PressedSprite;
				return result;
			}

			// Token: 0x040049AD RID: 18861
			[SerializeField]
			private Sprite m_DisabledSprite;

			// Token: 0x040049AE RID: 18862
			[SerializeField]
			private Sprite m_HighlightedSprite;

			// Token: 0x040049AF RID: 18863
			[SerializeField]
			private Sprite m_PressedSprite;

			// Token: 0x040049B0 RID: 18864
			[SerializeField]
			private Sprite m_SelectedSprite;

			// Token: 0x040049B1 RID: 18865
			[SerializeField]
			private Sprite m_DisabledHighlightedSprite;
		}

		// Token: 0x0200088D RID: 2189
		[Serializable]
		private class CustomAnimationTriggers
		{
			// Token: 0x06002E74 RID: 11892 RVA: 0x0020C810 File Offset: 0x0020AA10
			public CustomAnimationTriggers()
			{
				this.m_DisabledTrigger = string.Empty;
				this.m_HighlightedTrigger = string.Empty;
				this.m_NormalTrigger = string.Empty;
				this.m_PressedTrigger = string.Empty;
				this.m_SelectedTrigger = string.Empty;
				this.m_DisabledHighlightedTrigger = string.Empty;
			}

			// Token: 0x170004B9 RID: 1209
			// (get) Token: 0x06002E75 RID: 11893 RVA: 0x0020C865 File Offset: 0x0020AA65
			// (set) Token: 0x06002E76 RID: 11894 RVA: 0x0020C86D File Offset: 0x0020AA6D
			public string disabledTrigger
			{
				get
				{
					return this.m_DisabledTrigger;
				}
				set
				{
					this.m_DisabledTrigger = value;
				}
			}

			// Token: 0x170004BA RID: 1210
			// (get) Token: 0x06002E77 RID: 11895 RVA: 0x0020C876 File Offset: 0x0020AA76
			// (set) Token: 0x06002E78 RID: 11896 RVA: 0x0020C87E File Offset: 0x0020AA7E
			public string highlightedTrigger
			{
				get
				{
					return this.m_HighlightedTrigger;
				}
				set
				{
					this.m_HighlightedTrigger = value;
				}
			}

			// Token: 0x170004BB RID: 1211
			// (get) Token: 0x06002E79 RID: 11897 RVA: 0x0020C887 File Offset: 0x0020AA87
			// (set) Token: 0x06002E7A RID: 11898 RVA: 0x0020C88F File Offset: 0x0020AA8F
			public string normalTrigger
			{
				get
				{
					return this.m_NormalTrigger;
				}
				set
				{
					this.m_NormalTrigger = value;
				}
			}

			// Token: 0x170004BC RID: 1212
			// (get) Token: 0x06002E7B RID: 11899 RVA: 0x0020C898 File Offset: 0x0020AA98
			// (set) Token: 0x06002E7C RID: 11900 RVA: 0x0020C8A0 File Offset: 0x0020AAA0
			public string pressedTrigger
			{
				get
				{
					return this.m_PressedTrigger;
				}
				set
				{
					this.m_PressedTrigger = value;
				}
			}

			// Token: 0x170004BD RID: 1213
			// (get) Token: 0x06002E7D RID: 11901 RVA: 0x0020C8A9 File Offset: 0x0020AAA9
			// (set) Token: 0x06002E7E RID: 11902 RVA: 0x0020C8B1 File Offset: 0x0020AAB1
			public string selectedTrigger
			{
				get
				{
					return this.m_SelectedTrigger;
				}
				set
				{
					this.m_SelectedTrigger = value;
				}
			}

			// Token: 0x170004BE RID: 1214
			// (get) Token: 0x06002E7F RID: 11903 RVA: 0x0020C8BA File Offset: 0x0020AABA
			// (set) Token: 0x06002E80 RID: 11904 RVA: 0x0020C8C2 File Offset: 0x0020AAC2
			public string disabledHighlightedTrigger
			{
				get
				{
					return this.m_DisabledHighlightedTrigger;
				}
				set
				{
					this.m_DisabledHighlightedTrigger = value;
				}
			}

			// Token: 0x06002E81 RID: 11905 RVA: 0x0020C8CC File Offset: 0x0020AACC
			public static implicit operator AnimationTriggers(ThemeSettings.CustomAnimationTriggers item)
			{
				return new AnimationTriggers
				{
					selectedTrigger = item.m_SelectedTrigger,
					disabledTrigger = item.m_DisabledTrigger,
					highlightedTrigger = item.m_HighlightedTrigger,
					normalTrigger = item.m_NormalTrigger,
					pressedTrigger = item.m_PressedTrigger
				};
			}

			// Token: 0x040049B2 RID: 18866
			[SerializeField]
			private string m_DisabledTrigger;

			// Token: 0x040049B3 RID: 18867
			[SerializeField]
			private string m_HighlightedTrigger;

			// Token: 0x040049B4 RID: 18868
			[SerializeField]
			private string m_NormalTrigger;

			// Token: 0x040049B5 RID: 18869
			[SerializeField]
			private string m_PressedTrigger;

			// Token: 0x040049B6 RID: 18870
			[SerializeField]
			private string m_SelectedTrigger;

			// Token: 0x040049B7 RID: 18871
			[SerializeField]
			private string m_DisabledHighlightedTrigger;
		}

		// Token: 0x0200088E RID: 2190
		[Serializable]
		private class TextSettings
		{
			// Token: 0x170004BF RID: 1215
			// (get) Token: 0x06002E82 RID: 11906 RVA: 0x0020C91A File Offset: 0x0020AB1A
			public Color color
			{
				get
				{
					return this._color;
				}
			}

			// Token: 0x170004C0 RID: 1216
			// (get) Token: 0x06002E83 RID: 11907 RVA: 0x0020C922 File Offset: 0x0020AB22
			public TMP_FontAsset font
			{
				get
				{
					return this._font;
				}
			}

			// Token: 0x170004C1 RID: 1217
			// (get) Token: 0x06002E84 RID: 11908 RVA: 0x0020C92A File Offset: 0x0020AB2A
			public ThemeSettings.FontStyleOverride style
			{
				get
				{
					return this._style;
				}
			}

			// Token: 0x170004C2 RID: 1218
			// (get) Token: 0x06002E85 RID: 11909 RVA: 0x0020C932 File Offset: 0x0020AB32
			public float sizeMultiplier
			{
				get
				{
					return this._sizeMultiplier;
				}
			}

			// Token: 0x170004C3 RID: 1219
			// (get) Token: 0x06002E86 RID: 11910 RVA: 0x0020C93A File Offset: 0x0020AB3A
			public float lineSpacing
			{
				get
				{
					return this._lineSpacing;
				}
			}

			// Token: 0x170004C4 RID: 1220
			// (get) Token: 0x06002E87 RID: 11911 RVA: 0x0020C942 File Offset: 0x0020AB42
			public float chracterSpacing
			{
				get
				{
					return this._characterSpacing;
				}
			}

			// Token: 0x170004C5 RID: 1221
			// (get) Token: 0x06002E88 RID: 11912 RVA: 0x0020C94A File Offset: 0x0020AB4A
			public float wordSpacing
			{
				get
				{
					return this._wordSpacing;
				}
			}

			// Token: 0x040049B8 RID: 18872
			[SerializeField]
			private Color _color = Color.white;

			// Token: 0x040049B9 RID: 18873
			[SerializeField]
			private TMP_FontAsset _font;

			// Token: 0x040049BA RID: 18874
			[SerializeField]
			private ThemeSettings.FontStyleOverride _style;

			// Token: 0x040049BB RID: 18875
			[SerializeField]
			private float _sizeMultiplier = 1f;

			// Token: 0x040049BC RID: 18876
			[SerializeField]
			private float _lineSpacing = 1f;

			// Token: 0x040049BD RID: 18877
			[SerializeField]
			private float _characterSpacing = 1f;

			// Token: 0x040049BE RID: 18878
			[SerializeField]
			private float _wordSpacing = 1f;
		}

		// Token: 0x0200088F RID: 2191
		private enum FontStyleOverride
		{
			// Token: 0x040049C0 RID: 18880
			Default,
			// Token: 0x040049C1 RID: 18881
			Normal,
			// Token: 0x040049C2 RID: 18882
			Bold,
			// Token: 0x040049C3 RID: 18883
			Italic,
			// Token: 0x040049C4 RID: 18884
			BoldAndItalic
		}
	}
}
