using System;

namespace Rewired
{
	// Token: 0x02000824 RID: 2084
	public interface IGamepadTemplate : IControllerTemplate
	{
		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060026D4 RID: 9940
		IControllerTemplateButton actionBottomRow1 { get; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060026D5 RID: 9941
		IControllerTemplateButton a { get; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060026D6 RID: 9942
		IControllerTemplateButton actionBottomRow2 { get; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x060026D7 RID: 9943
		IControllerTemplateButton b { get; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x060026D8 RID: 9944
		IControllerTemplateButton actionBottomRow3 { get; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060026D9 RID: 9945
		IControllerTemplateButton c { get; }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060026DA RID: 9946
		IControllerTemplateButton actionTopRow1 { get; }

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060026DB RID: 9947
		IControllerTemplateButton x { get; }

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060026DC RID: 9948
		IControllerTemplateButton actionTopRow2 { get; }

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060026DD RID: 9949
		IControllerTemplateButton y { get; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060026DE RID: 9950
		IControllerTemplateButton actionTopRow3 { get; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x060026DF RID: 9951
		IControllerTemplateButton z { get; }

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x060026E0 RID: 9952
		IControllerTemplateButton leftShoulder1 { get; }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x060026E1 RID: 9953
		IControllerTemplateButton leftBumper { get; }

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x060026E2 RID: 9954
		IControllerTemplateAxis leftShoulder2 { get; }

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x060026E3 RID: 9955
		IControllerTemplateAxis leftTrigger { get; }

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x060026E4 RID: 9956
		IControllerTemplateButton rightShoulder1 { get; }

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x060026E5 RID: 9957
		IControllerTemplateButton rightBumper { get; }

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x060026E6 RID: 9958
		IControllerTemplateAxis rightShoulder2 { get; }

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x060026E7 RID: 9959
		IControllerTemplateAxis rightTrigger { get; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x060026E8 RID: 9960
		IControllerTemplateButton center1 { get; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x060026E9 RID: 9961
		IControllerTemplateButton back { get; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x060026EA RID: 9962
		IControllerTemplateButton center2 { get; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060026EB RID: 9963
		IControllerTemplateButton start { get; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060026EC RID: 9964
		IControllerTemplateButton center3 { get; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060026ED RID: 9965
		IControllerTemplateButton guide { get; }

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060026EE RID: 9966
		IControllerTemplateThumbStick leftStick { get; }

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060026EF RID: 9967
		IControllerTemplateThumbStick rightStick { get; }

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060026F0 RID: 9968
		IControllerTemplateDPad dPad { get; }
	}
}
