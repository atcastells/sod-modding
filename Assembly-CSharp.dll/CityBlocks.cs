using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;

// Token: 0x020001C2 RID: 450
public class CityBlocks : HighlanderSingleton<CityBlocks>
{
	// Token: 0x06000AFA RID: 2810 RVA: 0x000A4CB4 File Offset: 0x000A2EB4
	public UniTask GenerateBlocks()
	{
		CityBlocks.<GenerateBlocks>d__2 <GenerateBlocks>d__;
		<GenerateBlocks>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
		<GenerateBlocks>d__.<>4__this = this;
		<GenerateBlocks>d__.<>1__state = -1;
		<GenerateBlocks>d__.<>t__builder.Start<CityBlocks.<GenerateBlocks>d__2>(ref <GenerateBlocks>d__);
		return <GenerateBlocks>d__.<>t__builder.Task;
	}

	// Token: 0x04000B85 RID: 2949
	public int loadChunk = 10;

	// Token: 0x04000B86 RID: 2950
	public List<BlockController> blocksDirectory = new List<BlockController>();
}
