using System;

namespace Rewired
{
	// Token: 0x0200082C RID: 2092
	public sealed class HOTASTemplate : ControllerTemplate, IHOTASTemplate, IControllerTemplate
	{
		// Token: 0x17000271 RID: 625
		// (get) Token: 0x0600281B RID: 10267 RVA: 0x001FACD0 File Offset: 0x001F8ED0
		IControllerTemplateButton IHOTASTemplate.stickTrigger
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(3);
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x0600281C RID: 10268 RVA: 0x001FAAD8 File Offset: 0x001F8CD8
		IControllerTemplateButton IHOTASTemplate.stickTriggerStage2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(4);
			}
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x0600281D RID: 10269 RVA: 0x001FAAE1 File Offset: 0x001F8CE1
		IControllerTemplateButton IHOTASTemplate.stickPinkyButton
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(5);
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x0600281E RID: 10270 RVA: 0x001FACD9 File Offset: 0x001F8ED9
		IControllerTemplateButton IHOTASTemplate.stickPinkyTrigger
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(154);
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x0600281F RID: 10271 RVA: 0x001FAAEA File Offset: 0x001F8CEA
		IControllerTemplateButton IHOTASTemplate.stickButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(6);
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06002820 RID: 10272 RVA: 0x001FAAF3 File Offset: 0x001F8CF3
		IControllerTemplateButton IHOTASTemplate.stickButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(7);
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06002821 RID: 10273 RVA: 0x001FAAFC File Offset: 0x001F8CFC
		IControllerTemplateButton IHOTASTemplate.stickButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(8);
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06002822 RID: 10274 RVA: 0x001FAB05 File Offset: 0x001F8D05
		IControllerTemplateButton IHOTASTemplate.stickButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(9);
			}
		}

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06002823 RID: 10275 RVA: 0x001FAB0F File Offset: 0x001F8D0F
		IControllerTemplateButton IHOTASTemplate.stickButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(10);
			}
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06002824 RID: 10276 RVA: 0x001FABB1 File Offset: 0x001F8DB1
		IControllerTemplateButton IHOTASTemplate.stickButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(11);
			}
		}

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06002825 RID: 10277 RVA: 0x001FAB23 File Offset: 0x001F8D23
		IControllerTemplateButton IHOTASTemplate.stickButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06002826 RID: 10278 RVA: 0x001FABBB File Offset: 0x001F8DBB
		IControllerTemplateButton IHOTASTemplate.stickButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(13);
			}
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06002827 RID: 10279 RVA: 0x001FAB37 File Offset: 0x001F8D37
		IControllerTemplateButton IHOTASTemplate.stickButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06002828 RID: 10280 RVA: 0x001FAB41 File Offset: 0x001F8D41
		IControllerTemplateButton IHOTASTemplate.stickButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06002829 RID: 10281 RVA: 0x001FABCF File Offset: 0x001F8DCF
		IControllerTemplateButton IHOTASTemplate.stickBaseButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(18);
			}
		}

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x0600282A RID: 10282 RVA: 0x001FABD9 File Offset: 0x001F8DD9
		IControllerTemplateButton IHOTASTemplate.stickBaseButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(19);
			}
		}

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x0600282B RID: 10283 RVA: 0x001FABE3 File Offset: 0x001F8DE3
		IControllerTemplateButton IHOTASTemplate.stickBaseButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(20);
			}
		}

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x0600282C RID: 10284 RVA: 0x001FABED File Offset: 0x001F8DED
		IControllerTemplateButton IHOTASTemplate.stickBaseButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(21);
			}
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x0600282D RID: 10285 RVA: 0x001FABF7 File Offset: 0x001F8DF7
		IControllerTemplateButton IHOTASTemplate.stickBaseButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(22);
			}
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x0600282E RID: 10286 RVA: 0x001FAC01 File Offset: 0x001F8E01
		IControllerTemplateButton IHOTASTemplate.stickBaseButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(23);
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x0600282F RID: 10287 RVA: 0x001FAC0B File Offset: 0x001F8E0B
		IControllerTemplateButton IHOTASTemplate.stickBaseButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(24);
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06002830 RID: 10288 RVA: 0x001FAC15 File Offset: 0x001F8E15
		IControllerTemplateButton IHOTASTemplate.stickBaseButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(25);
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06002831 RID: 10289 RVA: 0x001FAC1F File Offset: 0x001F8E1F
		IControllerTemplateButton IHOTASTemplate.stickBaseButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(26);
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06002832 RID: 10290 RVA: 0x001FAC29 File Offset: 0x001F8E29
		IControllerTemplateButton IHOTASTemplate.stickBaseButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(27);
			}
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06002833 RID: 10291 RVA: 0x001FACE6 File Offset: 0x001F8EE6
		IControllerTemplateButton IHOTASTemplate.stickBaseButton11
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(161);
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06002834 RID: 10292 RVA: 0x001FACF3 File Offset: 0x001F8EF3
		IControllerTemplateButton IHOTASTemplate.stickBaseButton12
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(162);
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06002835 RID: 10293 RVA: 0x001FAC83 File Offset: 0x001F8E83
		IControllerTemplateButton IHOTASTemplate.mode1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(44);
			}
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06002836 RID: 10294 RVA: 0x001FAD00 File Offset: 0x001F8F00
		IControllerTemplateButton IHOTASTemplate.mode2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(45);
			}
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06002837 RID: 10295 RVA: 0x001FAD0A File Offset: 0x001F8F0A
		IControllerTemplateButton IHOTASTemplate.mode3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(46);
			}
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06002838 RID: 10296 RVA: 0x001FAD14 File Offset: 0x001F8F14
		IControllerTemplateButton IHOTASTemplate.throttleButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(50);
			}
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06002839 RID: 10297 RVA: 0x001FAD1E File Offset: 0x001F8F1E
		IControllerTemplateButton IHOTASTemplate.throttleButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(51);
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x0600283A RID: 10298 RVA: 0x001FAD28 File Offset: 0x001F8F28
		IControllerTemplateButton IHOTASTemplate.throttleButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(52);
			}
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x0600283B RID: 10299 RVA: 0x001FAD32 File Offset: 0x001F8F32
		IControllerTemplateButton IHOTASTemplate.throttleButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(53);
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x0600283C RID: 10300 RVA: 0x001FAD3C File Offset: 0x001F8F3C
		IControllerTemplateButton IHOTASTemplate.throttleButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(54);
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x0600283D RID: 10301 RVA: 0x001FAD46 File Offset: 0x001F8F46
		IControllerTemplateButton IHOTASTemplate.throttleButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(55);
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x0600283E RID: 10302 RVA: 0x001FAD50 File Offset: 0x001F8F50
		IControllerTemplateButton IHOTASTemplate.throttleButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(56);
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x0600283F RID: 10303 RVA: 0x001FAD5A File Offset: 0x001F8F5A
		IControllerTemplateButton IHOTASTemplate.throttleButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(57);
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06002840 RID: 10304 RVA: 0x001FAD64 File Offset: 0x001F8F64
		IControllerTemplateButton IHOTASTemplate.throttleButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(58);
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06002841 RID: 10305 RVA: 0x001FAD6E File Offset: 0x001F8F6E
		IControllerTemplateButton IHOTASTemplate.throttleButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(59);
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06002842 RID: 10306 RVA: 0x001FAD78 File Offset: 0x001F8F78
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(60);
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06002843 RID: 10307 RVA: 0x001FAD82 File Offset: 0x001F8F82
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(61);
			}
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06002844 RID: 10308 RVA: 0x001FAD8C File Offset: 0x001F8F8C
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(62);
			}
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06002845 RID: 10309 RVA: 0x001FAD96 File Offset: 0x001F8F96
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(63);
			}
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06002846 RID: 10310 RVA: 0x001FADA0 File Offset: 0x001F8FA0
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(64);
			}
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06002847 RID: 10311 RVA: 0x001FADAA File Offset: 0x001F8FAA
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(65);
			}
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06002848 RID: 10312 RVA: 0x001FADB4 File Offset: 0x001F8FB4
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(66);
			}
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06002849 RID: 10313 RVA: 0x001FADBE File Offset: 0x001F8FBE
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(67);
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x0600284A RID: 10314 RVA: 0x001FADC8 File Offset: 0x001F8FC8
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(68);
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x0600284B RID: 10315 RVA: 0x001FADD2 File Offset: 0x001F8FD2
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(69);
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x0600284C RID: 10316 RVA: 0x001FADDC File Offset: 0x001F8FDC
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton11
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(132);
			}
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x0600284D RID: 10317 RVA: 0x001FADE9 File Offset: 0x001F8FE9
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton12
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(133);
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x0600284E RID: 10318 RVA: 0x001FADF6 File Offset: 0x001F8FF6
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton13
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(134);
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x0600284F RID: 10319 RVA: 0x001FAE03 File Offset: 0x001F9003
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton14
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(135);
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06002850 RID: 10320 RVA: 0x001FAE10 File Offset: 0x001F9010
		IControllerTemplateButton IHOTASTemplate.throttleBaseButton15
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(136);
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06002851 RID: 10321 RVA: 0x001FAE1D File Offset: 0x001F901D
		IControllerTemplateAxis IHOTASTemplate.throttleSlider1
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(70);
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06002852 RID: 10322 RVA: 0x001FAE27 File Offset: 0x001F9027
		IControllerTemplateAxis IHOTASTemplate.throttleSlider2
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(71);
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06002853 RID: 10323 RVA: 0x001FAE31 File Offset: 0x001F9031
		IControllerTemplateAxis IHOTASTemplate.throttleSlider3
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(72);
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06002854 RID: 10324 RVA: 0x001FAE3B File Offset: 0x001F903B
		IControllerTemplateAxis IHOTASTemplate.throttleSlider4
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(73);
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06002855 RID: 10325 RVA: 0x001FAE45 File Offset: 0x001F9045
		IControllerTemplateAxis IHOTASTemplate.throttleDial1
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(74);
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06002856 RID: 10326 RVA: 0x001FAE4F File Offset: 0x001F904F
		IControllerTemplateAxis IHOTASTemplate.throttleDial2
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(142);
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06002857 RID: 10327 RVA: 0x001FAE5C File Offset: 0x001F905C
		IControllerTemplateAxis IHOTASTemplate.throttleDial3
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(143);
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06002858 RID: 10328 RVA: 0x001FAE69 File Offset: 0x001F9069
		IControllerTemplateAxis IHOTASTemplate.throttleDial4
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(144);
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06002859 RID: 10329 RVA: 0x001FAE76 File Offset: 0x001F9076
		IControllerTemplateButton IHOTASTemplate.throttleWheel1Forward
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(145);
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x0600285A RID: 10330 RVA: 0x001FAE83 File Offset: 0x001F9083
		IControllerTemplateButton IHOTASTemplate.throttleWheel1Back
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(146);
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x0600285B RID: 10331 RVA: 0x001FAE90 File Offset: 0x001F9090
		IControllerTemplateButton IHOTASTemplate.throttleWheel1Press
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(147);
			}
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x0600285C RID: 10332 RVA: 0x001FAE9D File Offset: 0x001F909D
		IControllerTemplateButton IHOTASTemplate.throttleWheel2Forward
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(148);
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x0600285D RID: 10333 RVA: 0x001FAEAA File Offset: 0x001F90AA
		IControllerTemplateButton IHOTASTemplate.throttleWheel2Back
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(149);
			}
		}

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x0600285E RID: 10334 RVA: 0x001FAEB7 File Offset: 0x001F90B7
		IControllerTemplateButton IHOTASTemplate.throttleWheel2Press
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(150);
			}
		}

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x0600285F RID: 10335 RVA: 0x001FAEC4 File Offset: 0x001F90C4
		IControllerTemplateButton IHOTASTemplate.throttleWheel3Forward
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(151);
			}
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06002860 RID: 10336 RVA: 0x001FAED1 File Offset: 0x001F90D1
		IControllerTemplateButton IHOTASTemplate.throttleWheel3Back
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(152);
			}
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06002861 RID: 10337 RVA: 0x001FAEDE File Offset: 0x001F90DE
		IControllerTemplateButton IHOTASTemplate.throttleWheel3Press
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(153);
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06002862 RID: 10338 RVA: 0x001FAEEB File Offset: 0x001F90EB
		IControllerTemplateAxis IHOTASTemplate.leftPedal
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(168);
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06002863 RID: 10339 RVA: 0x001FAEF8 File Offset: 0x001F90F8
		IControllerTemplateAxis IHOTASTemplate.rightPedal
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(169);
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06002864 RID: 10340 RVA: 0x001FAF05 File Offset: 0x001F9105
		IControllerTemplateAxis IHOTASTemplate.slidePedals
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(170);
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06002865 RID: 10341 RVA: 0x001FAF12 File Offset: 0x001F9112
		IControllerTemplateStick IHOTASTemplate.stick
		{
			get
			{
				return base.GetElement<IControllerTemplateStick>(171);
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06002866 RID: 10342 RVA: 0x001FAF1F File Offset: 0x001F911F
		IControllerTemplateThumbStick IHOTASTemplate.stickMiniStick1
		{
			get
			{
				return base.GetElement<IControllerTemplateThumbStick>(172);
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06002867 RID: 10343 RVA: 0x001FAF2C File Offset: 0x001F912C
		IControllerTemplateThumbStick IHOTASTemplate.stickMiniStick2
		{
			get
			{
				return base.GetElement<IControllerTemplateThumbStick>(173);
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06002868 RID: 10344 RVA: 0x001FAF39 File Offset: 0x001F9139
		IControllerTemplateHat IHOTASTemplate.stickHat1
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(174);
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06002869 RID: 10345 RVA: 0x001FAF46 File Offset: 0x001F9146
		IControllerTemplateHat IHOTASTemplate.stickHat2
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(175);
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x0600286A RID: 10346 RVA: 0x001FAF53 File Offset: 0x001F9153
		IControllerTemplateHat IHOTASTemplate.stickHat3
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(176);
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x0600286B RID: 10347 RVA: 0x001FAF60 File Offset: 0x001F9160
		IControllerTemplateHat IHOTASTemplate.stickHat4
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(177);
			}
		}

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x0600286C RID: 10348 RVA: 0x001FAF6D File Offset: 0x001F916D
		IControllerTemplateThrottle IHOTASTemplate.throttle1
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(178);
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x0600286D RID: 10349 RVA: 0x001FAF7A File Offset: 0x001F917A
		IControllerTemplateThrottle IHOTASTemplate.throttle2
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(179);
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x0600286E RID: 10350 RVA: 0x001FAF87 File Offset: 0x001F9187
		IControllerTemplateThumbStick IHOTASTemplate.throttleMiniStick
		{
			get
			{
				return base.GetElement<IControllerTemplateThumbStick>(180);
			}
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x0600286F RID: 10351 RVA: 0x001FAF94 File Offset: 0x001F9194
		IControllerTemplateHat IHOTASTemplate.throttleHat1
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(181);
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06002870 RID: 10352 RVA: 0x001FAFA1 File Offset: 0x001F91A1
		IControllerTemplateHat IHOTASTemplate.throttleHat2
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(182);
			}
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06002871 RID: 10353 RVA: 0x001FAFAE File Offset: 0x001F91AE
		IControllerTemplateHat IHOTASTemplate.throttleHat3
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(183);
			}
		}

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06002872 RID: 10354 RVA: 0x001FAFBB File Offset: 0x001F91BB
		IControllerTemplateHat IHOTASTemplate.throttleHat4
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(184);
			}
		}

		// Token: 0x06002873 RID: 10355 RVA: 0x001FAB73 File Offset: 0x001F8D73
		public HOTASTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x040045F0 RID: 17904
		public static readonly Guid typeGuid = new Guid("061a00cf-d8c2-4f8d-8cb5-a15a010bc53e");

		// Token: 0x040045F1 RID: 17905
		public const int elementId_stickX = 0;

		// Token: 0x040045F2 RID: 17906
		public const int elementId_stickY = 1;

		// Token: 0x040045F3 RID: 17907
		public const int elementId_stickRotate = 2;

		// Token: 0x040045F4 RID: 17908
		public const int elementId_stickMiniStick1X = 78;

		// Token: 0x040045F5 RID: 17909
		public const int elementId_stickMiniStick1Y = 79;

		// Token: 0x040045F6 RID: 17910
		public const int elementId_stickMiniStick1Press = 80;

		// Token: 0x040045F7 RID: 17911
		public const int elementId_stickMiniStick2X = 81;

		// Token: 0x040045F8 RID: 17912
		public const int elementId_stickMiniStick2Y = 82;

		// Token: 0x040045F9 RID: 17913
		public const int elementId_stickMiniStick2Press = 83;

		// Token: 0x040045FA RID: 17914
		public const int elementId_stickTrigger = 3;

		// Token: 0x040045FB RID: 17915
		public const int elementId_stickTriggerStage2 = 4;

		// Token: 0x040045FC RID: 17916
		public const int elementId_stickPinkyButton = 5;

		// Token: 0x040045FD RID: 17917
		public const int elementId_stickPinkyTrigger = 154;

		// Token: 0x040045FE RID: 17918
		public const int elementId_stickButton1 = 6;

		// Token: 0x040045FF RID: 17919
		public const int elementId_stickButton2 = 7;

		// Token: 0x04004600 RID: 17920
		public const int elementId_stickButton3 = 8;

		// Token: 0x04004601 RID: 17921
		public const int elementId_stickButton4 = 9;

		// Token: 0x04004602 RID: 17922
		public const int elementId_stickButton5 = 10;

		// Token: 0x04004603 RID: 17923
		public const int elementId_stickButton6 = 11;

		// Token: 0x04004604 RID: 17924
		public const int elementId_stickButton7 = 12;

		// Token: 0x04004605 RID: 17925
		public const int elementId_stickButton8 = 13;

		// Token: 0x04004606 RID: 17926
		public const int elementId_stickButton9 = 14;

		// Token: 0x04004607 RID: 17927
		public const int elementId_stickButton10 = 15;

		// Token: 0x04004608 RID: 17928
		public const int elementId_stickBaseButton1 = 18;

		// Token: 0x04004609 RID: 17929
		public const int elementId_stickBaseButton2 = 19;

		// Token: 0x0400460A RID: 17930
		public const int elementId_stickBaseButton3 = 20;

		// Token: 0x0400460B RID: 17931
		public const int elementId_stickBaseButton4 = 21;

		// Token: 0x0400460C RID: 17932
		public const int elementId_stickBaseButton5 = 22;

		// Token: 0x0400460D RID: 17933
		public const int elementId_stickBaseButton6 = 23;

		// Token: 0x0400460E RID: 17934
		public const int elementId_stickBaseButton7 = 24;

		// Token: 0x0400460F RID: 17935
		public const int elementId_stickBaseButton8 = 25;

		// Token: 0x04004610 RID: 17936
		public const int elementId_stickBaseButton9 = 26;

		// Token: 0x04004611 RID: 17937
		public const int elementId_stickBaseButton10 = 27;

		// Token: 0x04004612 RID: 17938
		public const int elementId_stickBaseButton11 = 161;

		// Token: 0x04004613 RID: 17939
		public const int elementId_stickBaseButton12 = 162;

		// Token: 0x04004614 RID: 17940
		public const int elementId_stickHat1Up = 28;

		// Token: 0x04004615 RID: 17941
		public const int elementId_stickHat1UpRight = 29;

		// Token: 0x04004616 RID: 17942
		public const int elementId_stickHat1Right = 30;

		// Token: 0x04004617 RID: 17943
		public const int elementId_stickHat1DownRight = 31;

		// Token: 0x04004618 RID: 17944
		public const int elementId_stickHat1Down = 32;

		// Token: 0x04004619 RID: 17945
		public const int elementId_stickHat1DownLeft = 33;

		// Token: 0x0400461A RID: 17946
		public const int elementId_stickHat1Left = 34;

		// Token: 0x0400461B RID: 17947
		public const int elementId_stickHat1Up_Left = 35;

		// Token: 0x0400461C RID: 17948
		public const int elementId_stickHat2Up = 36;

		// Token: 0x0400461D RID: 17949
		public const int elementId_stickHat2Up_right = 37;

		// Token: 0x0400461E RID: 17950
		public const int elementId_stickHat2Right = 38;

		// Token: 0x0400461F RID: 17951
		public const int elementId_stickHat2Down_Right = 39;

		// Token: 0x04004620 RID: 17952
		public const int elementId_stickHat2Down = 40;

		// Token: 0x04004621 RID: 17953
		public const int elementId_stickHat2Down_Left = 41;

		// Token: 0x04004622 RID: 17954
		public const int elementId_stickHat2Left = 42;

		// Token: 0x04004623 RID: 17955
		public const int elementId_stickHat2Up_Left = 43;

		// Token: 0x04004624 RID: 17956
		public const int elementId_stickHat3Up = 84;

		// Token: 0x04004625 RID: 17957
		public const int elementId_stickHat3Up_Right = 85;

		// Token: 0x04004626 RID: 17958
		public const int elementId_stickHat3Right = 86;

		// Token: 0x04004627 RID: 17959
		public const int elementId_stickHat3Down_Right = 87;

		// Token: 0x04004628 RID: 17960
		public const int elementId_stickHat3Down = 88;

		// Token: 0x04004629 RID: 17961
		public const int elementId_stickHat3Down_Left = 89;

		// Token: 0x0400462A RID: 17962
		public const int elementId_stickHat3Left = 90;

		// Token: 0x0400462B RID: 17963
		public const int elementId_stickHat3Up_Left = 91;

		// Token: 0x0400462C RID: 17964
		public const int elementId_stickHat4Up = 92;

		// Token: 0x0400462D RID: 17965
		public const int elementId_stickHat4Up_Right = 93;

		// Token: 0x0400462E RID: 17966
		public const int elementId_stickHat4Right = 94;

		// Token: 0x0400462F RID: 17967
		public const int elementId_stickHat4Down_Right = 95;

		// Token: 0x04004630 RID: 17968
		public const int elementId_stickHat4Down = 96;

		// Token: 0x04004631 RID: 17969
		public const int elementId_stickHat4Down_Left = 97;

		// Token: 0x04004632 RID: 17970
		public const int elementId_stickHat4Left = 98;

		// Token: 0x04004633 RID: 17971
		public const int elementId_stickHat4Up_Left = 99;

		// Token: 0x04004634 RID: 17972
		public const int elementId_mode1 = 44;

		// Token: 0x04004635 RID: 17973
		public const int elementId_mode2 = 45;

		// Token: 0x04004636 RID: 17974
		public const int elementId_mode3 = 46;

		// Token: 0x04004637 RID: 17975
		public const int elementId_throttle1Axis = 49;

		// Token: 0x04004638 RID: 17976
		public const int elementId_throttle2Axis = 155;

		// Token: 0x04004639 RID: 17977
		public const int elementId_throttle1MinDetent = 166;

		// Token: 0x0400463A RID: 17978
		public const int elementId_throttle2MinDetent = 167;

		// Token: 0x0400463B RID: 17979
		public const int elementId_throttleButton1 = 50;

		// Token: 0x0400463C RID: 17980
		public const int elementId_throttleButton2 = 51;

		// Token: 0x0400463D RID: 17981
		public const int elementId_throttleButton3 = 52;

		// Token: 0x0400463E RID: 17982
		public const int elementId_throttleButton4 = 53;

		// Token: 0x0400463F RID: 17983
		public const int elementId_throttleButton5 = 54;

		// Token: 0x04004640 RID: 17984
		public const int elementId_throttleButton6 = 55;

		// Token: 0x04004641 RID: 17985
		public const int elementId_throttleButton7 = 56;

		// Token: 0x04004642 RID: 17986
		public const int elementId_throttleButton8 = 57;

		// Token: 0x04004643 RID: 17987
		public const int elementId_throttleButton9 = 58;

		// Token: 0x04004644 RID: 17988
		public const int elementId_throttleButton10 = 59;

		// Token: 0x04004645 RID: 17989
		public const int elementId_throttleBaseButton1 = 60;

		// Token: 0x04004646 RID: 17990
		public const int elementId_throttleBaseButton2 = 61;

		// Token: 0x04004647 RID: 17991
		public const int elementId_throttleBaseButton3 = 62;

		// Token: 0x04004648 RID: 17992
		public const int elementId_throttleBaseButton4 = 63;

		// Token: 0x04004649 RID: 17993
		public const int elementId_throttleBaseButton5 = 64;

		// Token: 0x0400464A RID: 17994
		public const int elementId_throttleBaseButton6 = 65;

		// Token: 0x0400464B RID: 17995
		public const int elementId_throttleBaseButton7 = 66;

		// Token: 0x0400464C RID: 17996
		public const int elementId_throttleBaseButton8 = 67;

		// Token: 0x0400464D RID: 17997
		public const int elementId_throttleBaseButton9 = 68;

		// Token: 0x0400464E RID: 17998
		public const int elementId_throttleBaseButton10 = 69;

		// Token: 0x0400464F RID: 17999
		public const int elementId_throttleBaseButton11 = 132;

		// Token: 0x04004650 RID: 18000
		public const int elementId_throttleBaseButton12 = 133;

		// Token: 0x04004651 RID: 18001
		public const int elementId_throttleBaseButton13 = 134;

		// Token: 0x04004652 RID: 18002
		public const int elementId_throttleBaseButton14 = 135;

		// Token: 0x04004653 RID: 18003
		public const int elementId_throttleBaseButton15 = 136;

		// Token: 0x04004654 RID: 18004
		public const int elementId_throttleSlider1 = 70;

		// Token: 0x04004655 RID: 18005
		public const int elementId_throttleSlider2 = 71;

		// Token: 0x04004656 RID: 18006
		public const int elementId_throttleSlider3 = 72;

		// Token: 0x04004657 RID: 18007
		public const int elementId_throttleSlider4 = 73;

		// Token: 0x04004658 RID: 18008
		public const int elementId_throttleDial1 = 74;

		// Token: 0x04004659 RID: 18009
		public const int elementId_throttleDial2 = 142;

		// Token: 0x0400465A RID: 18010
		public const int elementId_throttleDial3 = 143;

		// Token: 0x0400465B RID: 18011
		public const int elementId_throttleDial4 = 144;

		// Token: 0x0400465C RID: 18012
		public const int elementId_throttleMiniStickX = 75;

		// Token: 0x0400465D RID: 18013
		public const int elementId_throttleMiniStickY = 76;

		// Token: 0x0400465E RID: 18014
		public const int elementId_throttleMiniStickPress = 77;

		// Token: 0x0400465F RID: 18015
		public const int elementId_throttleWheel1Forward = 145;

		// Token: 0x04004660 RID: 18016
		public const int elementId_throttleWheel1Back = 146;

		// Token: 0x04004661 RID: 18017
		public const int elementId_throttleWheel1Press = 147;

		// Token: 0x04004662 RID: 18018
		public const int elementId_throttleWheel2Forward = 148;

		// Token: 0x04004663 RID: 18019
		public const int elementId_throttleWheel2Back = 149;

		// Token: 0x04004664 RID: 18020
		public const int elementId_throttleWheel2Press = 150;

		// Token: 0x04004665 RID: 18021
		public const int elementId_throttleWheel3Forward = 151;

		// Token: 0x04004666 RID: 18022
		public const int elementId_throttleWheel3Back = 152;

		// Token: 0x04004667 RID: 18023
		public const int elementId_throttleWheel3Press = 153;

		// Token: 0x04004668 RID: 18024
		public const int elementId_throttleHat1Up = 100;

		// Token: 0x04004669 RID: 18025
		public const int elementId_throttleHat1Up_Right = 101;

		// Token: 0x0400466A RID: 18026
		public const int elementId_throttleHat1Right = 102;

		// Token: 0x0400466B RID: 18027
		public const int elementId_throttleHat1Down_Right = 103;

		// Token: 0x0400466C RID: 18028
		public const int elementId_throttleHat1Down = 104;

		// Token: 0x0400466D RID: 18029
		public const int elementId_throttleHat1Down_Left = 105;

		// Token: 0x0400466E RID: 18030
		public const int elementId_throttleHat1Left = 106;

		// Token: 0x0400466F RID: 18031
		public const int elementId_throttleHat1Up_Left = 107;

		// Token: 0x04004670 RID: 18032
		public const int elementId_throttleHat2Up = 108;

		// Token: 0x04004671 RID: 18033
		public const int elementId_throttleHat2Up_Right = 109;

		// Token: 0x04004672 RID: 18034
		public const int elementId_throttleHat2Right = 110;

		// Token: 0x04004673 RID: 18035
		public const int elementId_throttleHat2Down_Right = 111;

		// Token: 0x04004674 RID: 18036
		public const int elementId_throttleHat2Down = 112;

		// Token: 0x04004675 RID: 18037
		public const int elementId_throttleHat2Down_Left = 113;

		// Token: 0x04004676 RID: 18038
		public const int elementId_throttleHat2Left = 114;

		// Token: 0x04004677 RID: 18039
		public const int elementId_throttleHat2Up_Left = 115;

		// Token: 0x04004678 RID: 18040
		public const int elementId_throttleHat3Up = 116;

		// Token: 0x04004679 RID: 18041
		public const int elementId_throttleHat3Up_Right = 117;

		// Token: 0x0400467A RID: 18042
		public const int elementId_throttleHat3Right = 118;

		// Token: 0x0400467B RID: 18043
		public const int elementId_throttleHat3Down_Right = 119;

		// Token: 0x0400467C RID: 18044
		public const int elementId_throttleHat3Down = 120;

		// Token: 0x0400467D RID: 18045
		public const int elementId_throttleHat3Down_Left = 121;

		// Token: 0x0400467E RID: 18046
		public const int elementId_throttleHat3Left = 122;

		// Token: 0x0400467F RID: 18047
		public const int elementId_throttleHat3Up_Left = 123;

		// Token: 0x04004680 RID: 18048
		public const int elementId_throttleHat4Up = 124;

		// Token: 0x04004681 RID: 18049
		public const int elementId_throttleHat4Up_Right = 125;

		// Token: 0x04004682 RID: 18050
		public const int elementId_throttleHat4Right = 126;

		// Token: 0x04004683 RID: 18051
		public const int elementId_throttleHat4Down_Right = 127;

		// Token: 0x04004684 RID: 18052
		public const int elementId_throttleHat4Down = 128;

		// Token: 0x04004685 RID: 18053
		public const int elementId_throttleHat4Down_Left = 129;

		// Token: 0x04004686 RID: 18054
		public const int elementId_throttleHat4Left = 130;

		// Token: 0x04004687 RID: 18055
		public const int elementId_throttleHat4Up_Left = 131;

		// Token: 0x04004688 RID: 18056
		public const int elementId_leftPedal = 168;

		// Token: 0x04004689 RID: 18057
		public const int elementId_rightPedal = 169;

		// Token: 0x0400468A RID: 18058
		public const int elementId_slidePedals = 170;

		// Token: 0x0400468B RID: 18059
		public const int elementId_stick = 171;

		// Token: 0x0400468C RID: 18060
		public const int elementId_stickMiniStick1 = 172;

		// Token: 0x0400468D RID: 18061
		public const int elementId_stickMiniStick2 = 173;

		// Token: 0x0400468E RID: 18062
		public const int elementId_stickHat1 = 174;

		// Token: 0x0400468F RID: 18063
		public const int elementId_stickHat2 = 175;

		// Token: 0x04004690 RID: 18064
		public const int elementId_stickHat3 = 176;

		// Token: 0x04004691 RID: 18065
		public const int elementId_stickHat4 = 177;

		// Token: 0x04004692 RID: 18066
		public const int elementId_throttle1 = 178;

		// Token: 0x04004693 RID: 18067
		public const int elementId_throttle2 = 179;

		// Token: 0x04004694 RID: 18068
		public const int elementId_throttleMiniStick = 180;

		// Token: 0x04004695 RID: 18069
		public const int elementId_throttleHat1 = 181;

		// Token: 0x04004696 RID: 18070
		public const int elementId_throttleHat2 = 182;

		// Token: 0x04004697 RID: 18071
		public const int elementId_throttleHat3 = 183;

		// Token: 0x04004698 RID: 18072
		public const int elementId_throttleHat4 = 184;
	}
}
