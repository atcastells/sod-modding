using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
public static class ConsoleProDebug
{
	// Token: 0x060001B3 RID: 435 RVA: 0x00002265 File Offset: 0x00000465
	public static void Clear()
	{
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000E2B5 File Offset: 0x0000C4B5
	public static void LogToFilter(string inLog, string inFilterName)
	{
		Debug.Log(inLog + "\nCPAPI:{\"cmd\":\"Filter\" \"name\":\"" + inFilterName + "\"}");
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000E2CD File Offset: 0x0000C4CD
	public static void Watch(string inName, string inValue)
	{
		Debug.Log(string.Concat(new string[]
		{
			inName,
			" : ",
			inValue,
			"\nCPAPI:{\"cmd\":\"Watch\" \"name\":\"",
			inName,
			"\"}"
		}));
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000E303 File Offset: 0x0000C503
	public static void Search(string inText)
	{
		Debug.Log("\nCPAPI:{\"cmd\":\"Search\" \"text\":\"" + inText + "\"}");
	}
}
