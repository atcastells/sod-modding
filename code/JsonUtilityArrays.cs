using System;
using UnityEngine;

// Token: 0x0200002E RID: 46
public static class JsonUtilityArrays
{
	// Token: 0x060001B9 RID: 441 RVA: 0x0000E354 File Offset: 0x0000C554
	public static T[] FromJson<T>(string json)
	{
		return JsonUtility.FromJson<JsonUtilityArrays.Wrapper<T>>(json).Items;
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000E361 File Offset: 0x0000C561
	public static string ToJson<T>(T[] array)
	{
		return JsonUtility.ToJson(new JsonUtilityArrays.Wrapper<T>
		{
			Items = array
		});
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000E374 File Offset: 0x0000C574
	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		return JsonUtility.ToJson(new JsonUtilityArrays.Wrapper<T>
		{
			Items = array
		}, prettyPrint);
	}

	// Token: 0x0200002F RID: 47
	[Serializable]
	private class Wrapper<T>
	{
		// Token: 0x0400010A RID: 266
		public T[] Items;
	}
}
