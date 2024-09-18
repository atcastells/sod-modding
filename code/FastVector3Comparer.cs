using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class FastVector3Comparer : IEqualityComparer<Vector3>
{
	// Token: 0x06000A49 RID: 2633 RVA: 0x0009889A File Offset: 0x00096A9A
	bool IEqualityComparer<Vector3>.Equals(Vector3 x, Vector3 y)
	{
		return x.GetHashCode() == y.GetHashCode();
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x000988B8 File Offset: 0x00096AB8
	int IEqualityComparer<Vector3>.GetHashCode(Vector3 obj)
	{
		HashCode hashCode = default(HashCode);
		hashCode.Add<float>(obj.x);
		hashCode.Add<float>(obj.y);
		hashCode.Add<float>(obj.z);
		return hashCode.ToHashCode();
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000A4B RID: 2635 RVA: 0x000988FB File Offset: 0x00096AFB
	public static FastVector3Comparer SharedFastVector3Comparer
	{
		get
		{
			if (FastVector3Comparer.sharedFastVector3Comparer == null)
			{
				FastVector3Comparer.sharedFastVector3Comparer = new FastVector3Comparer();
			}
			return FastVector3Comparer.sharedFastVector3Comparer;
		}
	}

	// Token: 0x04000A4E RID: 2638
	private static FastVector3Comparer sharedFastVector3Comparer;
}
