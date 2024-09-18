using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;

// Token: 0x020001CC RID: 460
public class CityDensity : HighlanderSingleton<CityDensity>
{
	// Token: 0x06000B0E RID: 2830 RVA: 0x000A5FF0 File Offset: 0x000A41F0
	public UniTask GenerateDensity()
	{
		CityDensity.<GenerateDensity>d__0 <GenerateDensity>d__;
		<GenerateDensity>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
		<GenerateDensity>d__.<>1__state = -1;
		<GenerateDensity>d__.<>t__builder.Start<CityDensity.<GenerateDensity>d__0>(ref <GenerateDensity>d__);
		return <GenerateDensity>d__.<>t__builder.Task;
	}
}
