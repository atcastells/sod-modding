using System;

namespace Rewired
{
	// Token: 0x0200082A RID: 2090
	public sealed class GamepadTemplate : ControllerTemplate, IGamepadTemplate, IControllerTemplate
	{
		// Token: 0x1700022A RID: 554
		// (get) Token: 0x060027D0 RID: 10192 RVA: 0x001FAAD8 File Offset: 0x001F8CD8
		IControllerTemplateButton IGamepadTemplate.actionBottomRow1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(4);
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x060027D1 RID: 10193 RVA: 0x001FAAD8 File Offset: 0x001F8CD8
		IControllerTemplateButton IGamepadTemplate.a
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(4);
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x060027D2 RID: 10194 RVA: 0x001FAAE1 File Offset: 0x001F8CE1
		IControllerTemplateButton IGamepadTemplate.actionBottomRow2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(5);
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x060027D3 RID: 10195 RVA: 0x001FAAE1 File Offset: 0x001F8CE1
		IControllerTemplateButton IGamepadTemplate.b
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(5);
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060027D4 RID: 10196 RVA: 0x001FAAEA File Offset: 0x001F8CEA
		IControllerTemplateButton IGamepadTemplate.actionBottomRow3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(6);
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x060027D5 RID: 10197 RVA: 0x001FAAEA File Offset: 0x001F8CEA
		IControllerTemplateButton IGamepadTemplate.c
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(6);
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x060027D6 RID: 10198 RVA: 0x001FAAF3 File Offset: 0x001F8CF3
		IControllerTemplateButton IGamepadTemplate.actionTopRow1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(7);
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x060027D7 RID: 10199 RVA: 0x001FAAF3 File Offset: 0x001F8CF3
		IControllerTemplateButton IGamepadTemplate.x
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(7);
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x060027D8 RID: 10200 RVA: 0x001FAAFC File Offset: 0x001F8CFC
		IControllerTemplateButton IGamepadTemplate.actionTopRow2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(8);
			}
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x060027D9 RID: 10201 RVA: 0x001FAAFC File Offset: 0x001F8CFC
		IControllerTemplateButton IGamepadTemplate.y
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(8);
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x060027DA RID: 10202 RVA: 0x001FAB05 File Offset: 0x001F8D05
		IControllerTemplateButton IGamepadTemplate.actionTopRow3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(9);
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x060027DB RID: 10203 RVA: 0x001FAB05 File Offset: 0x001F8D05
		IControllerTemplateButton IGamepadTemplate.z
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(9);
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x060027DC RID: 10204 RVA: 0x001FAB0F File Offset: 0x001F8D0F
		IControllerTemplateButton IGamepadTemplate.leftShoulder1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(10);
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x060027DD RID: 10205 RVA: 0x001FAB0F File Offset: 0x001F8D0F
		IControllerTemplateButton IGamepadTemplate.leftBumper
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(10);
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x060027DE RID: 10206 RVA: 0x001FAB19 File Offset: 0x001F8D19
		IControllerTemplateAxis IGamepadTemplate.leftShoulder2
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(11);
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x060027DF RID: 10207 RVA: 0x001FAB19 File Offset: 0x001F8D19
		IControllerTemplateAxis IGamepadTemplate.leftTrigger
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(11);
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x060027E0 RID: 10208 RVA: 0x001FAB23 File Offset: 0x001F8D23
		IControllerTemplateButton IGamepadTemplate.rightShoulder1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x060027E1 RID: 10209 RVA: 0x001FAB23 File Offset: 0x001F8D23
		IControllerTemplateButton IGamepadTemplate.rightBumper
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x060027E2 RID: 10210 RVA: 0x001FAB2D File Offset: 0x001F8D2D
		IControllerTemplateAxis IGamepadTemplate.rightShoulder2
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(13);
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x060027E3 RID: 10211 RVA: 0x001FAB2D File Offset: 0x001F8D2D
		IControllerTemplateAxis IGamepadTemplate.rightTrigger
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(13);
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x060027E4 RID: 10212 RVA: 0x001FAB37 File Offset: 0x001F8D37
		IControllerTemplateButton IGamepadTemplate.center1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x060027E5 RID: 10213 RVA: 0x001FAB37 File Offset: 0x001F8D37
		IControllerTemplateButton IGamepadTemplate.back
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x060027E6 RID: 10214 RVA: 0x001FAB41 File Offset: 0x001F8D41
		IControllerTemplateButton IGamepadTemplate.center2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x060027E7 RID: 10215 RVA: 0x001FAB41 File Offset: 0x001F8D41
		IControllerTemplateButton IGamepadTemplate.start
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x060027E8 RID: 10216 RVA: 0x001FAB4B File Offset: 0x001F8D4B
		IControllerTemplateButton IGamepadTemplate.center3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(16);
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x060027E9 RID: 10217 RVA: 0x001FAB4B File Offset: 0x001F8D4B
		IControllerTemplateButton IGamepadTemplate.guide
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(16);
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x060027EA RID: 10218 RVA: 0x001FAB55 File Offset: 0x001F8D55
		IControllerTemplateThumbStick IGamepadTemplate.leftStick
		{
			get
			{
				return base.GetElement<IControllerTemplateThumbStick>(23);
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x060027EB RID: 10219 RVA: 0x001FAB5F File Offset: 0x001F8D5F
		IControllerTemplateThumbStick IGamepadTemplate.rightStick
		{
			get
			{
				return base.GetElement<IControllerTemplateThumbStick>(24);
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060027EC RID: 10220 RVA: 0x001FAB69 File Offset: 0x001F8D69
		IControllerTemplateDPad IGamepadTemplate.dPad
		{
			get
			{
				return base.GetElement<IControllerTemplateDPad>(25);
			}
		}

		// Token: 0x060027ED RID: 10221 RVA: 0x001FAB73 File Offset: 0x001F8D73
		public GamepadTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x04004599 RID: 17817
		public static readonly Guid typeGuid = new Guid("83b427e4-086f-47f3-bb06-be266abd1ca5");

		// Token: 0x0400459A RID: 17818
		public const int elementId_leftStickX = 0;

		// Token: 0x0400459B RID: 17819
		public const int elementId_leftStickY = 1;

		// Token: 0x0400459C RID: 17820
		public const int elementId_rightStickX = 2;

		// Token: 0x0400459D RID: 17821
		public const int elementId_rightStickY = 3;

		// Token: 0x0400459E RID: 17822
		public const int elementId_actionBottomRow1 = 4;

		// Token: 0x0400459F RID: 17823
		public const int elementId_a = 4;

		// Token: 0x040045A0 RID: 17824
		public const int elementId_actionBottomRow2 = 5;

		// Token: 0x040045A1 RID: 17825
		public const int elementId_b = 5;

		// Token: 0x040045A2 RID: 17826
		public const int elementId_actionBottomRow3 = 6;

		// Token: 0x040045A3 RID: 17827
		public const int elementId_c = 6;

		// Token: 0x040045A4 RID: 17828
		public const int elementId_actionTopRow1 = 7;

		// Token: 0x040045A5 RID: 17829
		public const int elementId_x = 7;

		// Token: 0x040045A6 RID: 17830
		public const int elementId_actionTopRow2 = 8;

		// Token: 0x040045A7 RID: 17831
		public const int elementId_y = 8;

		// Token: 0x040045A8 RID: 17832
		public const int elementId_actionTopRow3 = 9;

		// Token: 0x040045A9 RID: 17833
		public const int elementId_z = 9;

		// Token: 0x040045AA RID: 17834
		public const int elementId_leftShoulder1 = 10;

		// Token: 0x040045AB RID: 17835
		public const int elementId_leftBumper = 10;

		// Token: 0x040045AC RID: 17836
		public const int elementId_leftShoulder2 = 11;

		// Token: 0x040045AD RID: 17837
		public const int elementId_leftTrigger = 11;

		// Token: 0x040045AE RID: 17838
		public const int elementId_rightShoulder1 = 12;

		// Token: 0x040045AF RID: 17839
		public const int elementId_rightBumper = 12;

		// Token: 0x040045B0 RID: 17840
		public const int elementId_rightShoulder2 = 13;

		// Token: 0x040045B1 RID: 17841
		public const int elementId_rightTrigger = 13;

		// Token: 0x040045B2 RID: 17842
		public const int elementId_center1 = 14;

		// Token: 0x040045B3 RID: 17843
		public const int elementId_back = 14;

		// Token: 0x040045B4 RID: 17844
		public const int elementId_center2 = 15;

		// Token: 0x040045B5 RID: 17845
		public const int elementId_start = 15;

		// Token: 0x040045B6 RID: 17846
		public const int elementId_center3 = 16;

		// Token: 0x040045B7 RID: 17847
		public const int elementId_guide = 16;

		// Token: 0x040045B8 RID: 17848
		public const int elementId_leftStickButton = 17;

		// Token: 0x040045B9 RID: 17849
		public const int elementId_rightStickButton = 18;

		// Token: 0x040045BA RID: 17850
		public const int elementId_dPadUp = 19;

		// Token: 0x040045BB RID: 17851
		public const int elementId_dPadRight = 20;

		// Token: 0x040045BC RID: 17852
		public const int elementId_dPadDown = 21;

		// Token: 0x040045BD RID: 17853
		public const int elementId_dPadLeft = 22;

		// Token: 0x040045BE RID: 17854
		public const int elementId_leftStick = 23;

		// Token: 0x040045BF RID: 17855
		public const int elementId_rightStick = 24;

		// Token: 0x040045C0 RID: 17856
		public const int elementId_dPad = 25;
	}
}
