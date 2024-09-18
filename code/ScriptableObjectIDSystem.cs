using System;
using UnityEngine;

// Token: 0x020007C9 RID: 1993
public class ScriptableObjectIDSystem : ScriptableObject
{
	// Token: 0x060025CC RID: 9676 RVA: 0x00180CB2 File Offset: 0x0017EEB2
	public bool Equals(DoorPairPreset other)
	{
		return object.Equals(other, this);
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x001E84A1 File Offset: 0x001E66A1
	public override bool Equals(object obj)
	{
		return obj != null && !(base.GetType() != obj.GetType()) && ((DoorPairPreset)obj).id == this.id;
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x001E84D4 File Offset: 0x001E66D4
	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		hashCode.Add<string>(this.id);
		hashCode.Add<Type>(base.GetType());
		return hashCode.ToHashCode();
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x001E850A File Offset: 0x001E670A
	public static bool operator ==(ScriptableObjectIDSystem c1, ScriptableObjectIDSystem c2)
	{
		if (c1 == null)
		{
			return c2 == null;
		}
		return c1.Equals(c2);
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x001E851B File Offset: 0x001E671B
	public static bool operator !=(ScriptableObjectIDSystem c1, ScriptableObjectIDSystem c2)
	{
		return !(c1 == c2);
	}

	// Token: 0x04003D2D RID: 15661
	[Header("ID System")]
	[Tooltip("Used as a replacement for names for smaller save data sizes")]
	public string id;
}
