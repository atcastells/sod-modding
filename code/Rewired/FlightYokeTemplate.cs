using System;

namespace Rewired
{
	// Token: 0x0200082D RID: 2093
	public sealed class FlightYokeTemplate : ControllerTemplate, IFlightYokeTemplate, IControllerTemplate
	{
		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06002875 RID: 10357 RVA: 0x001FAD6E File Offset: 0x001F8F6E
		IControllerTemplateButton IFlightYokeTemplate.leftPaddle
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(59);
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06002876 RID: 10358 RVA: 0x001FAD78 File Offset: 0x001F8F78
		IControllerTemplateButton IFlightYokeTemplate.rightPaddle
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(60);
			}
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06002877 RID: 10359 RVA: 0x001FAAF3 File Offset: 0x001F8CF3
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(7);
			}
		}

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06002878 RID: 10360 RVA: 0x001FAAFC File Offset: 0x001F8CFC
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(8);
			}
		}

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06002879 RID: 10361 RVA: 0x001FAB05 File Offset: 0x001F8D05
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(9);
			}
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x0600287A RID: 10362 RVA: 0x001FAB0F File Offset: 0x001F8D0F
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(10);
			}
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x0600287B RID: 10363 RVA: 0x001FABB1 File Offset: 0x001F8DB1
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(11);
			}
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x0600287C RID: 10364 RVA: 0x001FAB23 File Offset: 0x001F8D23
		IControllerTemplateButton IFlightYokeTemplate.leftGripButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(12);
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x0600287D RID: 10365 RVA: 0x001FABBB File Offset: 0x001F8DBB
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(13);
			}
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x0600287E RID: 10366 RVA: 0x001FAB37 File Offset: 0x001F8D37
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(14);
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x0600287F RID: 10367 RVA: 0x001FAB41 File Offset: 0x001F8D41
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(15);
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06002880 RID: 10368 RVA: 0x001FAB4B File Offset: 0x001F8D4B
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(16);
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06002881 RID: 10369 RVA: 0x001FABC5 File Offset: 0x001F8DC5
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(17);
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06002882 RID: 10370 RVA: 0x001FABCF File Offset: 0x001F8DCF
		IControllerTemplateButton IFlightYokeTemplate.rightGripButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(18);
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06002883 RID: 10371 RVA: 0x001FABD9 File Offset: 0x001F8DD9
		IControllerTemplateButton IFlightYokeTemplate.centerButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(19);
			}
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06002884 RID: 10372 RVA: 0x001FABE3 File Offset: 0x001F8DE3
		IControllerTemplateButton IFlightYokeTemplate.centerButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(20);
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06002885 RID: 10373 RVA: 0x001FABED File Offset: 0x001F8DED
		IControllerTemplateButton IFlightYokeTemplate.centerButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(21);
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06002886 RID: 10374 RVA: 0x001FABF7 File Offset: 0x001F8DF7
		IControllerTemplateButton IFlightYokeTemplate.centerButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(22);
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06002887 RID: 10375 RVA: 0x001FAC01 File Offset: 0x001F8E01
		IControllerTemplateButton IFlightYokeTemplate.centerButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(23);
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06002888 RID: 10376 RVA: 0x001FAC0B File Offset: 0x001F8E0B
		IControllerTemplateButton IFlightYokeTemplate.centerButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(24);
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06002889 RID: 10377 RVA: 0x001FAC15 File Offset: 0x001F8E15
		IControllerTemplateButton IFlightYokeTemplate.centerButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(25);
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x0600288A RID: 10378 RVA: 0x001FAC1F File Offset: 0x001F8E1F
		IControllerTemplateButton IFlightYokeTemplate.centerButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(26);
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x0600288B RID: 10379 RVA: 0x001FAD32 File Offset: 0x001F8F32
		IControllerTemplateButton IFlightYokeTemplate.wheel1Up
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(53);
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x0600288C RID: 10380 RVA: 0x001FAD3C File Offset: 0x001F8F3C
		IControllerTemplateButton IFlightYokeTemplate.wheel1Down
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(54);
			}
		}

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x0600288D RID: 10381 RVA: 0x001FAD46 File Offset: 0x001F8F46
		IControllerTemplateButton IFlightYokeTemplate.wheel1Press
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(55);
			}
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x0600288E RID: 10382 RVA: 0x001FAD50 File Offset: 0x001F8F50
		IControllerTemplateButton IFlightYokeTemplate.wheel2Up
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(56);
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x0600288F RID: 10383 RVA: 0x001FAD5A File Offset: 0x001F8F5A
		IControllerTemplateButton IFlightYokeTemplate.wheel2Down
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(57);
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06002890 RID: 10384 RVA: 0x001FAD64 File Offset: 0x001F8F64
		IControllerTemplateButton IFlightYokeTemplate.wheel2Press
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(58);
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06002891 RID: 10385 RVA: 0x001FACAB File Offset: 0x001F8EAB
		IControllerTemplateButton IFlightYokeTemplate.consoleButton1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(43);
			}
		}

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06002892 RID: 10386 RVA: 0x001FAC83 File Offset: 0x001F8E83
		IControllerTemplateButton IFlightYokeTemplate.consoleButton2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(44);
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06002893 RID: 10387 RVA: 0x001FAD00 File Offset: 0x001F8F00
		IControllerTemplateButton IFlightYokeTemplate.consoleButton3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(45);
			}
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06002894 RID: 10388 RVA: 0x001FAD0A File Offset: 0x001F8F0A
		IControllerTemplateButton IFlightYokeTemplate.consoleButton4
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(46);
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06002895 RID: 10389 RVA: 0x001FAFD9 File Offset: 0x001F91D9
		IControllerTemplateButton IFlightYokeTemplate.consoleButton5
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(47);
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06002896 RID: 10390 RVA: 0x001FAFE3 File Offset: 0x001F91E3
		IControllerTemplateButton IFlightYokeTemplate.consoleButton6
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(48);
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06002897 RID: 10391 RVA: 0x001FAFED File Offset: 0x001F91ED
		IControllerTemplateButton IFlightYokeTemplate.consoleButton7
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(49);
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06002898 RID: 10392 RVA: 0x001FAD14 File Offset: 0x001F8F14
		IControllerTemplateButton IFlightYokeTemplate.consoleButton8
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(50);
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06002899 RID: 10393 RVA: 0x001FAD1E File Offset: 0x001F8F1E
		IControllerTemplateButton IFlightYokeTemplate.consoleButton9
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(51);
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x0600289A RID: 10394 RVA: 0x001FAD28 File Offset: 0x001F8F28
		IControllerTemplateButton IFlightYokeTemplate.consoleButton10
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(52);
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x0600289B RID: 10395 RVA: 0x001FAD82 File Offset: 0x001F8F82
		IControllerTemplateButton IFlightYokeTemplate.mode1
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(61);
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x0600289C RID: 10396 RVA: 0x001FAD8C File Offset: 0x001F8F8C
		IControllerTemplateButton IFlightYokeTemplate.mode2
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(62);
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x0600289D RID: 10397 RVA: 0x001FAD96 File Offset: 0x001F8F96
		IControllerTemplateButton IFlightYokeTemplate.mode3
		{
			get
			{
				return base.GetElement<IControllerTemplateButton>(63);
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x0600289E RID: 10398 RVA: 0x001FAFF7 File Offset: 0x001F91F7
		IControllerTemplateYoke IFlightYokeTemplate.yoke
		{
			get
			{
				return base.GetElement<IControllerTemplateYoke>(69);
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x0600289F RID: 10399 RVA: 0x001FB001 File Offset: 0x001F9201
		IControllerTemplateThrottle IFlightYokeTemplate.lever1
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(70);
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x060028A0 RID: 10400 RVA: 0x001FB00B File Offset: 0x001F920B
		IControllerTemplateThrottle IFlightYokeTemplate.lever2
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(71);
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x060028A1 RID: 10401 RVA: 0x001FB015 File Offset: 0x001F9215
		IControllerTemplateThrottle IFlightYokeTemplate.lever3
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(72);
			}
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060028A2 RID: 10402 RVA: 0x001FB01F File Offset: 0x001F921F
		IControllerTemplateThrottle IFlightYokeTemplate.lever4
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(73);
			}
		}

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060028A3 RID: 10403 RVA: 0x001FB029 File Offset: 0x001F9229
		IControllerTemplateThrottle IFlightYokeTemplate.lever5
		{
			get
			{
				return base.GetElement<IControllerTemplateThrottle>(74);
			}
		}

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x060028A4 RID: 10404 RVA: 0x001FB033 File Offset: 0x001F9233
		IControllerTemplateHat IFlightYokeTemplate.leftGripHat
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(75);
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x060028A5 RID: 10405 RVA: 0x001FB03D File Offset: 0x001F923D
		IControllerTemplateHat IFlightYokeTemplate.rightGripHat
		{
			get
			{
				return base.GetElement<IControllerTemplateHat>(76);
			}
		}

		// Token: 0x060028A6 RID: 10406 RVA: 0x001FAB73 File Offset: 0x001F8D73
		public FlightYokeTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x04004699 RID: 18073
		public static readonly Guid typeGuid = new Guid("f311fa16-0ccc-41c0-ac4b-50f7100bb8ff");

		// Token: 0x0400469A RID: 18074
		public const int elementId_rotateYoke = 0;

		// Token: 0x0400469B RID: 18075
		public const int elementId_yokeZ = 1;

		// Token: 0x0400469C RID: 18076
		public const int elementId_leftPaddle = 59;

		// Token: 0x0400469D RID: 18077
		public const int elementId_rightPaddle = 60;

		// Token: 0x0400469E RID: 18078
		public const int elementId_lever1Axis = 2;

		// Token: 0x0400469F RID: 18079
		public const int elementId_lever1MinDetent = 64;

		// Token: 0x040046A0 RID: 18080
		public const int elementId_lever2Axis = 3;

		// Token: 0x040046A1 RID: 18081
		public const int elementId_lever2MinDetent = 65;

		// Token: 0x040046A2 RID: 18082
		public const int elementId_lever3Axis = 4;

		// Token: 0x040046A3 RID: 18083
		public const int elementId_lever3MinDetent = 66;

		// Token: 0x040046A4 RID: 18084
		public const int elementId_lever4Axis = 5;

		// Token: 0x040046A5 RID: 18085
		public const int elementId_lever4MinDetent = 67;

		// Token: 0x040046A6 RID: 18086
		public const int elementId_lever5Axis = 6;

		// Token: 0x040046A7 RID: 18087
		public const int elementId_lever5MinDetent = 68;

		// Token: 0x040046A8 RID: 18088
		public const int elementId_leftGripButton1 = 7;

		// Token: 0x040046A9 RID: 18089
		public const int elementId_leftGripButton2 = 8;

		// Token: 0x040046AA RID: 18090
		public const int elementId_leftGripButton3 = 9;

		// Token: 0x040046AB RID: 18091
		public const int elementId_leftGripButton4 = 10;

		// Token: 0x040046AC RID: 18092
		public const int elementId_leftGripButton5 = 11;

		// Token: 0x040046AD RID: 18093
		public const int elementId_leftGripButton6 = 12;

		// Token: 0x040046AE RID: 18094
		public const int elementId_rightGripButton1 = 13;

		// Token: 0x040046AF RID: 18095
		public const int elementId_rightGripButton2 = 14;

		// Token: 0x040046B0 RID: 18096
		public const int elementId_rightGripButton3 = 15;

		// Token: 0x040046B1 RID: 18097
		public const int elementId_rightGripButton4 = 16;

		// Token: 0x040046B2 RID: 18098
		public const int elementId_rightGripButton5 = 17;

		// Token: 0x040046B3 RID: 18099
		public const int elementId_rightGripButton6 = 18;

		// Token: 0x040046B4 RID: 18100
		public const int elementId_centerButton1 = 19;

		// Token: 0x040046B5 RID: 18101
		public const int elementId_centerButton2 = 20;

		// Token: 0x040046B6 RID: 18102
		public const int elementId_centerButton3 = 21;

		// Token: 0x040046B7 RID: 18103
		public const int elementId_centerButton4 = 22;

		// Token: 0x040046B8 RID: 18104
		public const int elementId_centerButton5 = 23;

		// Token: 0x040046B9 RID: 18105
		public const int elementId_centerButton6 = 24;

		// Token: 0x040046BA RID: 18106
		public const int elementId_centerButton7 = 25;

		// Token: 0x040046BB RID: 18107
		public const int elementId_centerButton8 = 26;

		// Token: 0x040046BC RID: 18108
		public const int elementId_wheel1Up = 53;

		// Token: 0x040046BD RID: 18109
		public const int elementId_wheel1Down = 54;

		// Token: 0x040046BE RID: 18110
		public const int elementId_wheel1Press = 55;

		// Token: 0x040046BF RID: 18111
		public const int elementId_wheel2Up = 56;

		// Token: 0x040046C0 RID: 18112
		public const int elementId_wheel2Down = 57;

		// Token: 0x040046C1 RID: 18113
		public const int elementId_wheel2Press = 58;

		// Token: 0x040046C2 RID: 18114
		public const int elementId_leftGripHatUp = 27;

		// Token: 0x040046C3 RID: 18115
		public const int elementId_leftGripHatUpRight = 28;

		// Token: 0x040046C4 RID: 18116
		public const int elementId_leftGripHatRight = 29;

		// Token: 0x040046C5 RID: 18117
		public const int elementId_leftGripHatDownRight = 30;

		// Token: 0x040046C6 RID: 18118
		public const int elementId_leftGripHatDown = 31;

		// Token: 0x040046C7 RID: 18119
		public const int elementId_leftGripHatDownLeft = 32;

		// Token: 0x040046C8 RID: 18120
		public const int elementId_leftGripHatLeft = 33;

		// Token: 0x040046C9 RID: 18121
		public const int elementId_leftGripHatUpLeft = 34;

		// Token: 0x040046CA RID: 18122
		public const int elementId_rightGripHatUp = 35;

		// Token: 0x040046CB RID: 18123
		public const int elementId_rightGripHatUpRight = 36;

		// Token: 0x040046CC RID: 18124
		public const int elementId_rightGripHatRight = 37;

		// Token: 0x040046CD RID: 18125
		public const int elementId_rightGripHatDownRight = 38;

		// Token: 0x040046CE RID: 18126
		public const int elementId_rightGripHatDown = 39;

		// Token: 0x040046CF RID: 18127
		public const int elementId_rightGripHatDownLeft = 40;

		// Token: 0x040046D0 RID: 18128
		public const int elementId_rightGripHatLeft = 41;

		// Token: 0x040046D1 RID: 18129
		public const int elementId_rightGripHatUpLeft = 42;

		// Token: 0x040046D2 RID: 18130
		public const int elementId_consoleButton1 = 43;

		// Token: 0x040046D3 RID: 18131
		public const int elementId_consoleButton2 = 44;

		// Token: 0x040046D4 RID: 18132
		public const int elementId_consoleButton3 = 45;

		// Token: 0x040046D5 RID: 18133
		public const int elementId_consoleButton4 = 46;

		// Token: 0x040046D6 RID: 18134
		public const int elementId_consoleButton5 = 47;

		// Token: 0x040046D7 RID: 18135
		public const int elementId_consoleButton6 = 48;

		// Token: 0x040046D8 RID: 18136
		public const int elementId_consoleButton7 = 49;

		// Token: 0x040046D9 RID: 18137
		public const int elementId_consoleButton8 = 50;

		// Token: 0x040046DA RID: 18138
		public const int elementId_consoleButton9 = 51;

		// Token: 0x040046DB RID: 18139
		public const int elementId_consoleButton10 = 52;

		// Token: 0x040046DC RID: 18140
		public const int elementId_mode1 = 61;

		// Token: 0x040046DD RID: 18141
		public const int elementId_mode2 = 62;

		// Token: 0x040046DE RID: 18142
		public const int elementId_mode3 = 63;

		// Token: 0x040046DF RID: 18143
		public const int elementId_yoke = 69;

		// Token: 0x040046E0 RID: 18144
		public const int elementId_lever1 = 70;

		// Token: 0x040046E1 RID: 18145
		public const int elementId_lever2 = 71;

		// Token: 0x040046E2 RID: 18146
		public const int elementId_lever3 = 72;

		// Token: 0x040046E3 RID: 18147
		public const int elementId_lever4 = 73;

		// Token: 0x040046E4 RID: 18148
		public const int elementId_lever5 = 74;

		// Token: 0x040046E5 RID: 18149
		public const int elementId_leftGripHat = 75;

		// Token: 0x040046E6 RID: 18150
		public const int elementId_rightGripHat = 76;
	}
}
