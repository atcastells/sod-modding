using System;

namespace Rewired
{
	// Token: 0x02000825 RID: 2085
	public interface IRacingWheelTemplate : IControllerTemplate
	{
		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060026F1 RID: 9969
		IControllerTemplateAxis wheel { get; }

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060026F2 RID: 9970
		IControllerTemplateAxis accelerator { get; }

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060026F3 RID: 9971
		IControllerTemplateAxis brake { get; }

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060026F4 RID: 9972
		IControllerTemplateAxis clutch { get; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060026F5 RID: 9973
		IControllerTemplateButton shiftDown { get; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060026F6 RID: 9974
		IControllerTemplateButton shiftUp { get; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060026F7 RID: 9975
		IControllerTemplateButton wheelButton1 { get; }

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060026F8 RID: 9976
		IControllerTemplateButton wheelButton2 { get; }

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060026F9 RID: 9977
		IControllerTemplateButton wheelButton3 { get; }

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060026FA RID: 9978
		IControllerTemplateButton wheelButton4 { get; }

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060026FB RID: 9979
		IControllerTemplateButton wheelButton5 { get; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060026FC RID: 9980
		IControllerTemplateButton wheelButton6 { get; }

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060026FD RID: 9981
		IControllerTemplateButton wheelButton7 { get; }

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060026FE RID: 9982
		IControllerTemplateButton wheelButton8 { get; }

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060026FF RID: 9983
		IControllerTemplateButton wheelButton9 { get; }

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06002700 RID: 9984
		IControllerTemplateButton wheelButton10 { get; }

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06002701 RID: 9985
		IControllerTemplateButton consoleButton1 { get; }

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06002702 RID: 9986
		IControllerTemplateButton consoleButton2 { get; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06002703 RID: 9987
		IControllerTemplateButton consoleButton3 { get; }

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06002704 RID: 9988
		IControllerTemplateButton consoleButton4 { get; }

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06002705 RID: 9989
		IControllerTemplateButton consoleButton5 { get; }

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06002706 RID: 9990
		IControllerTemplateButton consoleButton6 { get; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06002707 RID: 9991
		IControllerTemplateButton consoleButton7 { get; }

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06002708 RID: 9992
		IControllerTemplateButton consoleButton8 { get; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06002709 RID: 9993
		IControllerTemplateButton consoleButton9 { get; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x0600270A RID: 9994
		IControllerTemplateButton consoleButton10 { get; }

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x0600270B RID: 9995
		IControllerTemplateButton shifter1 { get; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x0600270C RID: 9996
		IControllerTemplateButton shifter2 { get; }

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600270D RID: 9997
		IControllerTemplateButton shifter3 { get; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600270E RID: 9998
		IControllerTemplateButton shifter4 { get; }

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x0600270F RID: 9999
		IControllerTemplateButton shifter5 { get; }

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06002710 RID: 10000
		IControllerTemplateButton shifter6 { get; }

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06002711 RID: 10001
		IControllerTemplateButton shifter7 { get; }

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06002712 RID: 10002
		IControllerTemplateButton shifter8 { get; }

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06002713 RID: 10003
		IControllerTemplateButton shifter9 { get; }

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06002714 RID: 10004
		IControllerTemplateButton shifter10 { get; }

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06002715 RID: 10005
		IControllerTemplateButton reverseGear { get; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06002716 RID: 10006
		IControllerTemplateButton select { get; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06002717 RID: 10007
		IControllerTemplateButton start { get; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06002718 RID: 10008
		IControllerTemplateButton systemButton { get; }

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06002719 RID: 10009
		IControllerTemplateButton horn { get; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x0600271A RID: 10010
		IControllerTemplateDPad dPad { get; }
	}
}
