using System;

namespace Rewired
{
	// Token: 0x0200082B RID: 2091
	public sealed class RacingWheelTemplate : ControllerTemplate, IRacingWheelTemplate, IControllerTemplate
	{
		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060027EF RID: 10223 RVA: 0x001FAB8D File Offset: 0x001F8D8D
		IControllerTemplateAxis IRacingWheelTemplate.wheel
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(0);
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060027F0 RID: 10224 RVA: 0x001FAB96 File Offset: 0x001F8D96
		IControllerTemplateAxis IRacingWheelTemplate.accelerator
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(1);
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x060027F1 RID: 10225 RVA: 0x001FAB9F File Offset: 0x001F8D9F
		IControllerTemplateAxis IRacingWheelTemplate.brake
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(2);
			}
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x060027F2 RID: 10226 RVA: 0x001FABA8 File Offset: 0x001F8DA8
		IControllerTemplateAxis IRacingWheelTemplate.clutch
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(3);
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x060027F3 RID: 10227 RVA: 0x001FAAD8 File Offset: 0x001F8CD8
		IControllerTemplateButton IRacingWheelTemplate.shiftDown
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(4);
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x060027F4 RID: 10228 RVA: 0x001FAAE1 File Offset: 0x001F8CE1
		IControllerTemplateButton IRacingWheelTemplate.shiftUp
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(5);
			}
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x060027F5 RID: 10229 RVA: 0x001FAAEA File Offset: 0x001F8CEA
		IControllerTemplateButton IRacingWheelTemplate.wheelButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(6);
			}
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x060027F6 RID: 10230 RVA: 0x001FAAF3 File Offset: 0x001F8CF3
		IControllerTemplateButton IRacingWheelTemplate.wheelButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(7);
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x060027F7 RID: 10231 RVA: 0x001FAAFC File Offset: 0x001F8CFC
		IControllerTemplateButton IRacingWheelTemplate.wheelButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(8);
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x060027F8 RID: 10232 RVA: 0x001FAB05 File Offset: 0x001F8D05
		IControllerTemplateButton IRacingWheelTemplate.wheelButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(9);
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x060027F9 RID: 10233 RVA: 0x001FAB0F File Offset: 0x001F8D0F
		IControllerTemplateButton IRacingWheelTemplate.wheelButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(10);
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x060027FA RID: 10234 RVA: 0x001FABB1 File Offset: 0x001F8DB1
		IControllerTemplateButton IRacingWheelTemplate.wheelButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(11);
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x060027FB RID: 10235 RVA: 0x001FAB23 File Offset: 0x001F8D23
		IControllerTemplateButton IRacingWheelTemplate.wheelButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x060027FC RID: 10236 RVA: 0x001FABBB File Offset: 0x001F8DBB
		IControllerTemplateButton IRacingWheelTemplate.wheelButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(13);
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x060027FD RID: 10237 RVA: 0x001FAB37 File Offset: 0x001F8D37
		IControllerTemplateButton IRacingWheelTemplate.wheelButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x060027FE RID: 10238 RVA: 0x001FAB41 File Offset: 0x001F8D41
		IControllerTemplateButton IRacingWheelTemplate.wheelButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x060027FF RID: 10239 RVA: 0x001FAB4B File Offset: 0x001F8D4B
		IControllerTemplateButton IRacingWheelTemplate.consoleButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(16);
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06002800 RID: 10240 RVA: 0x001FABC5 File Offset: 0x001F8DC5
		IControllerTemplateButton IRacingWheelTemplate.consoleButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(17);
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06002801 RID: 10241 RVA: 0x001FABCF File Offset: 0x001F8DCF
		IControllerTemplateButton IRacingWheelTemplate.consoleButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(18);
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06002802 RID: 10242 RVA: 0x001FABD9 File Offset: 0x001F8DD9
		IControllerTemplateButton IRacingWheelTemplate.consoleButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(19);
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06002803 RID: 10243 RVA: 0x001FABE3 File Offset: 0x001F8DE3
		IControllerTemplateButton IRacingWheelTemplate.consoleButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(20);
			}
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06002804 RID: 10244 RVA: 0x001FABED File Offset: 0x001F8DED
		IControllerTemplateButton IRacingWheelTemplate.consoleButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(21);
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06002805 RID: 10245 RVA: 0x001FABF7 File Offset: 0x001F8DF7
		IControllerTemplateButton IRacingWheelTemplate.consoleButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(22);
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x06002806 RID: 10246 RVA: 0x001FAC01 File Offset: 0x001F8E01
		IControllerTemplateButton IRacingWheelTemplate.consoleButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(23);
			}
		}

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x06002807 RID: 10247 RVA: 0x001FAC0B File Offset: 0x001F8E0B
		IControllerTemplateButton IRacingWheelTemplate.consoleButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(24);
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06002808 RID: 10248 RVA: 0x001FAC15 File Offset: 0x001F8E15
		IControllerTemplateButton IRacingWheelTemplate.consoleButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(25);
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06002809 RID: 10249 RVA: 0x001FAC1F File Offset: 0x001F8E1F
		IControllerTemplateButton IRacingWheelTemplate.shifter1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(26);
			}
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x0600280A RID: 10250 RVA: 0x001FAC29 File Offset: 0x001F8E29
		IControllerTemplateButton IRacingWheelTemplate.shifter2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(27);
			}
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x0600280B RID: 10251 RVA: 0x001FAC33 File Offset: 0x001F8E33
		IControllerTemplateButton IRacingWheelTemplate.shifter3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(28);
			}
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x0600280C RID: 10252 RVA: 0x001FAC3D File Offset: 0x001F8E3D
		IControllerTemplateButton IRacingWheelTemplate.shifter4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(29);
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x0600280D RID: 10253 RVA: 0x001FAC47 File Offset: 0x001F8E47
		IControllerTemplateButton IRacingWheelTemplate.shifter5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(30);
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x0600280E RID: 10254 RVA: 0x001FAC51 File Offset: 0x001F8E51
		IControllerTemplateButton IRacingWheelTemplate.shifter6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(31);
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x0600280F RID: 10255 RVA: 0x001FAC5B File Offset: 0x001F8E5B
		IControllerTemplateButton IRacingWheelTemplate.shifter7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(32);
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06002810 RID: 10256 RVA: 0x001FAC65 File Offset: 0x001F8E65
		IControllerTemplateButton IRacingWheelTemplate.shifter8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(33);
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06002811 RID: 10257 RVA: 0x001FAC6F File Offset: 0x001F8E6F
		IControllerTemplateButton IRacingWheelTemplate.shifter9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(34);
			}
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06002812 RID: 10258 RVA: 0x001FAC79 File Offset: 0x001F8E79
		IControllerTemplateButton IRacingWheelTemplate.shifter10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(35);
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06002813 RID: 10259 RVA: 0x001FAC83 File Offset: 0x001F8E83
		IControllerTemplateButton IRacingWheelTemplate.reverseGear
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(44);
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06002814 RID: 10260 RVA: 0x001FAC8D File Offset: 0x001F8E8D
		IControllerTemplateButton IRacingWheelTemplate.select
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(36);
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06002815 RID: 10261 RVA: 0x001FAC97 File Offset: 0x001F8E97
		IControllerTemplateButton IRacingWheelTemplate.start
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(37);
			}
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06002816 RID: 10262 RVA: 0x001FACA1 File Offset: 0x001F8EA1
		IControllerTemplateButton IRacingWheelTemplate.systemButton
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(38);
			}
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06002817 RID: 10263 RVA: 0x001FACAB File Offset: 0x001F8EAB
		IControllerTemplateButton IRacingWheelTemplate.horn
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(43);
			}
		}

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06002818 RID: 10264 RVA: 0x001FACB5 File Offset: 0x001F8EB5
		IControllerTemplateDPad IRacingWheelTemplate.dPad
		{
			get
			{
				return base.GetElement<IControllerTemplateDPad>(45);
			}
		}

		// Token: 0x06002819 RID: 10265 RVA: 0x001FAB73 File Offset: 0x001F8D73
		public RacingWheelTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x040045C1 RID: 17857
		public static readonly Guid typeGuid = new Guid("104e31d8-9115-4dd5-a398-2e54d35e6c83");

		// Token: 0x040045C2 RID: 17858
		public const int elementId_wheel = 0;

		// Token: 0x040045C3 RID: 17859
		public const int elementId_accelerator = 1;

		// Token: 0x040045C4 RID: 17860
		public const int elementId_brake = 2;

		// Token: 0x040045C5 RID: 17861
		public const int elementId_clutch = 3;

		// Token: 0x040045C6 RID: 17862
		public const int elementId_shiftDown = 4;

		// Token: 0x040045C7 RID: 17863
		public const int elementId_shiftUp = 5;

		// Token: 0x040045C8 RID: 17864
		public const int elementId_wheelButton1 = 6;

		// Token: 0x040045C9 RID: 17865
		public const int elementId_wheelButton2 = 7;

		// Token: 0x040045CA RID: 17866
		public const int elementId_wheelButton3 = 8;

		// Token: 0x040045CB RID: 17867
		public const int elementId_wheelButton4 = 9;

		// Token: 0x040045CC RID: 17868
		public const int elementId_wheelButton5 = 10;

		// Token: 0x040045CD RID: 17869
		public const int elementId_wheelButton6 = 11;

		// Token: 0x040045CE RID: 17870
		public const int elementId_wheelButton7 = 12;

		// Token: 0x040045CF RID: 17871
		public const int elementId_wheelButton8 = 13;

		// Token: 0x040045D0 RID: 17872
		public const int elementId_wheelButton9 = 14;

		// Token: 0x040045D1 RID: 17873
		public const int elementId_wheelButton10 = 15;

		// Token: 0x040045D2 RID: 17874
		public const int elementId_consoleButton1 = 16;

		// Token: 0x040045D3 RID: 17875
		public const int elementId_consoleButton2 = 17;

		// Token: 0x040045D4 RID: 17876
		public const int elementId_consoleButton3 = 18;

		// Token: 0x040045D5 RID: 17877
		public const int elementId_consoleButton4 = 19;

		// Token: 0x040045D6 RID: 17878
		public const int elementId_consoleButton5 = 20;

		// Token: 0x040045D7 RID: 17879
		public const int elementId_consoleButton6 = 21;

		// Token: 0x040045D8 RID: 17880
		public const int elementId_consoleButton7 = 22;

		// Token: 0x040045D9 RID: 17881
		public const int elementId_consoleButton8 = 23;

		// Token: 0x040045DA RID: 17882
		public const int elementId_consoleButton9 = 24;

		// Token: 0x040045DB RID: 17883
		public const int elementId_consoleButton10 = 25;

		// Token: 0x040045DC RID: 17884
		public const int elementId_shifter1 = 26;

		// Token: 0x040045DD RID: 17885
		public const int elementId_shifter2 = 27;

		// Token: 0x040045DE RID: 17886
		public const int elementId_shifter3 = 28;

		// Token: 0x040045DF RID: 17887
		public const int elementId_shifter4 = 29;

		// Token: 0x040045E0 RID: 17888
		public const int elementId_shifter5 = 30;

		// Token: 0x040045E1 RID: 17889
		public const int elementId_shifter6 = 31;

		// Token: 0x040045E2 RID: 17890
		public const int elementId_shifter7 = 32;

		// Token: 0x040045E3 RID: 17891
		public const int elementId_shifter8 = 33;

		// Token: 0x040045E4 RID: 17892
		public const int elementId_shifter9 = 34;

		// Token: 0x040045E5 RID: 17893
		public const int elementId_shifter10 = 35;

		// Token: 0x040045E6 RID: 17894
		public const int elementId_reverseGear = 44;

		// Token: 0x040045E7 RID: 17895
		public const int elementId_select = 36;

		// Token: 0x040045E8 RID: 17896
		public const int elementId_start = 37;

		// Token: 0x040045E9 RID: 17897
		public const int elementId_systemButton = 38;

		// Token: 0x040045EA RID: 17898
		public const int elementId_horn = 43;

		// Token: 0x040045EB RID: 17899
		public const int elementId_dPadUp = 39;

		// Token: 0x040045EC RID: 17900
		public const int elementId_dPadRight = 40;

		// Token: 0x040045ED RID: 17901
		public const int elementId_dPadDown = 41;

		// Token: 0x040045EE RID: 17902
		public const int elementId_dPadLeft = 42;

		// Token: 0x040045EF RID: 17903
		public const int elementId_dPad = 45;
	}
}
