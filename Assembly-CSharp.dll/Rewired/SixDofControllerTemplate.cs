using System;

namespace Rewired
{
	// Token: 0x0200082F RID: 2095
	public sealed class SixDofControllerTemplate : ControllerTemplate, ISixDofControllerTemplate, IControllerTemplate
	{
		// Token: 0x170002FD RID: 765
		// (get) Token: 0x060028AD RID: 10413 RVA: 0x001FB069 File Offset: 0x001F9269
		IControllerTemplateAxis ISixDofControllerTemplate.extraAxis1
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(8);
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x060028AE RID: 10414 RVA: 0x001FB072 File Offset: 0x001F9272
		IControllerTemplateAxis ISixDofControllerTemplate.extraAxis2
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(9);
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x060028AF RID: 10415 RVA: 0x001FB07C File Offset: 0x001F927C
		IControllerTemplateAxis ISixDofControllerTemplate.extraAxis3
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(10);
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x060028B0 RID: 10416 RVA: 0x001FAB19 File Offset: 0x001F8D19
		IControllerTemplateAxis ISixDofControllerTemplate.extraAxis4
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(11);
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x060028B1 RID: 10417 RVA: 0x001FAB23 File Offset: 0x001F8D23
		IControllerTemplateButton ISixDofControllerTemplate.button1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x060028B2 RID: 10418 RVA: 0x001FABBB File Offset: 0x001F8DBB
		IControllerTemplateButton ISixDofControllerTemplate.button2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(13);
			}
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x060028B3 RID: 10419 RVA: 0x001FAB37 File Offset: 0x001F8D37
		IControllerTemplateButton ISixDofControllerTemplate.button3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x060028B4 RID: 10420 RVA: 0x001FAB41 File Offset: 0x001F8D41
		IControllerTemplateButton ISixDofControllerTemplate.button4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x060028B5 RID: 10421 RVA: 0x001FAB4B File Offset: 0x001F8D4B
		IControllerTemplateButton ISixDofControllerTemplate.button5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(16);
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x060028B6 RID: 10422 RVA: 0x001FABC5 File Offset: 0x001F8DC5
		IControllerTemplateButton ISixDofControllerTemplate.button6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(17);
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060028B7 RID: 10423 RVA: 0x001FABCF File Offset: 0x001F8DCF
		IControllerTemplateButton ISixDofControllerTemplate.button7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(18);
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060028B8 RID: 10424 RVA: 0x001FABD9 File Offset: 0x001F8DD9
		IControllerTemplateButton ISixDofControllerTemplate.button8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(19);
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060028B9 RID: 10425 RVA: 0x001FABE3 File Offset: 0x001F8DE3
		IControllerTemplateButton ISixDofControllerTemplate.button9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(20);
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060028BA RID: 10426 RVA: 0x001FABED File Offset: 0x001F8DED
		IControllerTemplateButton ISixDofControllerTemplate.button10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(21);
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060028BB RID: 10427 RVA: 0x001FABF7 File Offset: 0x001F8DF7
		IControllerTemplateButton ISixDofControllerTemplate.button11
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(22);
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060028BC RID: 10428 RVA: 0x001FAC01 File Offset: 0x001F8E01
		IControllerTemplateButton ISixDofControllerTemplate.button12
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(23);
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060028BD RID: 10429 RVA: 0x001FAC0B File Offset: 0x001F8E0B
		IControllerTemplateButton ISixDofControllerTemplate.button13
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(24);
			}
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060028BE RID: 10430 RVA: 0x001FAC15 File Offset: 0x001F8E15
		IControllerTemplateButton ISixDofControllerTemplate.button14
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(25);
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060028BF RID: 10431 RVA: 0x001FAC1F File Offset: 0x001F8E1F
		IControllerTemplateButton ISixDofControllerTemplate.button15
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(26);
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060028C0 RID: 10432 RVA: 0x001FAC29 File Offset: 0x001F8E29
		IControllerTemplateButton ISixDofControllerTemplate.button16
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(27);
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060028C1 RID: 10433 RVA: 0x001FAC33 File Offset: 0x001F8E33
		IControllerTemplateButton ISixDofControllerTemplate.button17
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(28);
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060028C2 RID: 10434 RVA: 0x001FAC3D File Offset: 0x001F8E3D
		IControllerTemplateButton ISixDofControllerTemplate.button18
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(29);
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060028C3 RID: 10435 RVA: 0x001FAC47 File Offset: 0x001F8E47
		IControllerTemplateButton ISixDofControllerTemplate.button19
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(30);
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060028C4 RID: 10436 RVA: 0x001FAC51 File Offset: 0x001F8E51
		IControllerTemplateButton ISixDofControllerTemplate.button20
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(31);
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060028C5 RID: 10437 RVA: 0x001FAD46 File Offset: 0x001F8F46
		IControllerTemplateButton ISixDofControllerTemplate.button21
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(55);
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060028C6 RID: 10438 RVA: 0x001FAD50 File Offset: 0x001F8F50
		IControllerTemplateButton ISixDofControllerTemplate.button22
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(56);
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060028C7 RID: 10439 RVA: 0x001FAD5A File Offset: 0x001F8F5A
		IControllerTemplateButton ISixDofControllerTemplate.button23
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(57);
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060028C8 RID: 10440 RVA: 0x001FAD64 File Offset: 0x001F8F64
		IControllerTemplateButton ISixDofControllerTemplate.button24
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(58);
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060028C9 RID: 10441 RVA: 0x001FAD6E File Offset: 0x001F8F6E
		IControllerTemplateButton ISixDofControllerTemplate.button25
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(59);
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060028CA RID: 10442 RVA: 0x001FAD78 File Offset: 0x001F8F78
		IControllerTemplateButton ISixDofControllerTemplate.button26
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(60);
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060028CB RID: 10443 RVA: 0x001FAD82 File Offset: 0x001F8F82
		IControllerTemplateButton ISixDofControllerTemplate.button27
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(61);
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x060028CC RID: 10444 RVA: 0x001FAD8C File Offset: 0x001F8F8C
		IControllerTemplateButton ISixDofControllerTemplate.button28
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(62);
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060028CD RID: 10445 RVA: 0x001FAD96 File Offset: 0x001F8F96
		IControllerTemplateButton ISixDofControllerTemplate.button29
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(63);
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060028CE RID: 10446 RVA: 0x001FADA0 File Offset: 0x001F8FA0
		IControllerTemplateButton ISixDofControllerTemplate.button30
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(64);
			}
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060028CF RID: 10447 RVA: 0x001FADAA File Offset: 0x001F8FAA
		IControllerTemplateButton ISixDofControllerTemplate.button31
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(65);
			}
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060028D0 RID: 10448 RVA: 0x001FADB4 File Offset: 0x001F8FB4
		IControllerTemplateButton ISixDofControllerTemplate.button32
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(66);
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060028D1 RID: 10449 RVA: 0x001FB086 File Offset: 0x001F9286
		IControllerTemplateHat ISixDofControllerTemplate.hat1
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(48);
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060028D2 RID: 10450 RVA: 0x001FB090 File Offset: 0x001F9290
		IControllerTemplateHat ISixDofControllerTemplate.hat2
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(49);
			}
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x060028D3 RID: 10451 RVA: 0x001FB09A File Offset: 0x001F929A
		IControllerTemplateThrottle ISixDofControllerTemplate.throttle1
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(52);
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x060028D4 RID: 10452 RVA: 0x001FB0A4 File Offset: 0x001F92A4
		IControllerTemplateThrottle ISixDofControllerTemplate.throttle2
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(53);
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x060028D5 RID: 10453 RVA: 0x001FB0AE File Offset: 0x001F92AE
		IControllerTemplateStick6D ISixDofControllerTemplate.stick
		{
			get
			{
				return base.GetElement<IControllerTemplateStick6D>(54);
			}
		}

		// Token: 0x060028D6 RID: 10454 RVA: 0x001FAB73 File Offset: 0x001F8D73
		public SixDofControllerTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x040046EB RID: 18155
		public static readonly Guid typeGuid = new Guid("2599beb3-522b-43dd-a4ef-93fd60e5eafa");

		// Token: 0x040046EC RID: 18156
		public const int elementId_positionX = 1;

		// Token: 0x040046ED RID: 18157
		public const int elementId_positionY = 2;

		// Token: 0x040046EE RID: 18158
		public const int elementId_positionZ = 0;

		// Token: 0x040046EF RID: 18159
		public const int elementId_rotationX = 3;

		// Token: 0x040046F0 RID: 18160
		public const int elementId_rotationY = 5;

		// Token: 0x040046F1 RID: 18161
		public const int elementId_rotationZ = 4;

		// Token: 0x040046F2 RID: 18162
		public const int elementId_throttle1Axis = 6;

		// Token: 0x040046F3 RID: 18163
		public const int elementId_throttle1MinDetent = 50;

		// Token: 0x040046F4 RID: 18164
		public const int elementId_throttle2Axis = 7;

		// Token: 0x040046F5 RID: 18165
		public const int elementId_throttle2MinDetent = 51;

		// Token: 0x040046F6 RID: 18166
		public const int elementId_extraAxis1 = 8;

		// Token: 0x040046F7 RID: 18167
		public const int elementId_extraAxis2 = 9;

		// Token: 0x040046F8 RID: 18168
		public const int elementId_extraAxis3 = 10;

		// Token: 0x040046F9 RID: 18169
		public const int elementId_extraAxis4 = 11;

		// Token: 0x040046FA RID: 18170
		public const int elementId_button1 = 12;

		// Token: 0x040046FB RID: 18171
		public const int elementId_button2 = 13;

		// Token: 0x040046FC RID: 18172
		public const int elementId_button3 = 14;

		// Token: 0x040046FD RID: 18173
		public const int elementId_button4 = 15;

		// Token: 0x040046FE RID: 18174
		public const int elementId_button5 = 16;

		// Token: 0x040046FF RID: 18175
		public const int elementId_button6 = 17;

		// Token: 0x04004700 RID: 18176
		public const int elementId_button7 = 18;

		// Token: 0x04004701 RID: 18177
		public const int elementId_button8 = 19;

		// Token: 0x04004702 RID: 18178
		public const int elementId_button9 = 20;

		// Token: 0x04004703 RID: 18179
		public const int elementId_button10 = 21;

		// Token: 0x04004704 RID: 18180
		public const int elementId_button11 = 22;

		// Token: 0x04004705 RID: 18181
		public const int elementId_button12 = 23;

		// Token: 0x04004706 RID: 18182
		public const int elementId_button13 = 24;

		// Token: 0x04004707 RID: 18183
		public const int elementId_button14 = 25;

		// Token: 0x04004708 RID: 18184
		public const int elementId_button15 = 26;

		// Token: 0x04004709 RID: 18185
		public const int elementId_button16 = 27;

		// Token: 0x0400470A RID: 18186
		public const int elementId_button17 = 28;

		// Token: 0x0400470B RID: 18187
		public const int elementId_button18 = 29;

		// Token: 0x0400470C RID: 18188
		public const int elementId_button19 = 30;

		// Token: 0x0400470D RID: 18189
		public const int elementId_button20 = 31;

		// Token: 0x0400470E RID: 18190
		public const int elementId_button21 = 55;

		// Token: 0x0400470F RID: 18191
		public const int elementId_button22 = 56;

		// Token: 0x04004710 RID: 18192
		public const int elementId_button23 = 57;

		// Token: 0x04004711 RID: 18193
		public const int elementId_button24 = 58;

		// Token: 0x04004712 RID: 18194
		public const int elementId_button25 = 59;

		// Token: 0x04004713 RID: 18195
		public const int elementId_button26 = 60;

		// Token: 0x04004714 RID: 18196
		public const int elementId_button27 = 61;

		// Token: 0x04004715 RID: 18197
		public const int elementId_button28 = 62;

		// Token: 0x04004716 RID: 18198
		public const int elementId_button29 = 63;

		// Token: 0x04004717 RID: 18199
		public const int elementId_button30 = 64;

		// Token: 0x04004718 RID: 18200
		public const int elementId_button31 = 65;

		// Token: 0x04004719 RID: 18201
		public const int elementId_button32 = 66;

		// Token: 0x0400471A RID: 18202
		public const int elementId_hat1Up = 32;

		// Token: 0x0400471B RID: 18203
		public const int elementId_hat1UpRight = 33;

		// Token: 0x0400471C RID: 18204
		public const int elementId_hat1Right = 34;

		// Token: 0x0400471D RID: 18205
		public const int elementId_hat1DownRight = 35;

		// Token: 0x0400471E RID: 18206
		public const int elementId_hat1Down = 36;

		// Token: 0x0400471F RID: 18207
		public const int elementId_hat1DownLeft = 37;

		// Token: 0x04004720 RID: 18208
		public const int elementId_hat1Left = 38;

		// Token: 0x04004721 RID: 18209
		public const int elementId_hat1UpLeft = 39;

		// Token: 0x04004722 RID: 18210
		public const int elementId_hat2Up = 40;

		// Token: 0x04004723 RID: 18211
		public const int elementId_hat2UpRight = 41;

		// Token: 0x04004724 RID: 18212
		public const int elementId_hat2Right = 42;

		// Token: 0x04004725 RID: 18213
		public const int elementId_hat2DownRight = 43;

		// Token: 0x04004726 RID: 18214
		public const int elementId_hat2Down = 44;

		// Token: 0x04004727 RID: 18215
		public const int elementId_hat2DownLeft = 45;

		// Token: 0x04004728 RID: 18216
		public const int elementId_hat2Left = 46;

		// Token: 0x04004729 RID: 18217
		public const int elementId_hat2UpLeft = 47;

		// Token: 0x0400472A RID: 18218
		public const int elementId_hat1 = 48;

		// Token: 0x0400472B RID: 18219
		public const int elementId_hat2 = 49;

		// Token: 0x0400472C RID: 18220
		public const int elementId_throttle1 = 52;

		// Token: 0x0400472D RID: 18221
		public const int elementId_throttle2 = 53;

		// Token: 0x0400472E RID: 18222
		public const int elementId_stick = 54;
	}
}
