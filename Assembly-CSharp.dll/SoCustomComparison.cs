using System;
using UnityEngine;

// Token: 0x020007CE RID: 1998
public class SoCustomComparison : ScriptableObject
{
	// Token: 0x060025D5 RID: 9685 RVA: 0x00180CB2 File Offset: 0x0017EEB2
	public bool Equals(SoCustomComparison other)
	{
		return object.Equals(other, this);
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x001E85B8 File Offset: 0x001E67B8
	public virtual string GetPresetName()
	{
		if (this.presetName != null && this.presetName.Length > 0)
		{
			return this.presetName;
		}
		try
		{
			return base.name;
		}
		catch (Exception)
		{
		}
		try
		{
			return base.GetInstanceID().ToString();
		}
		catch
		{
		}
		return string.Empty;
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x001E8628 File Offset: 0x001E6828
	public override bool Equals(object obj)
	{
		if (obj == null || this == null || base.GetType() != obj.GetType())
		{
			return false;
		}
		SoCustomComparison soCustomComparison = (SoCustomComparison)obj;
		return !(soCustomComparison == null) && soCustomComparison.GetPresetName() == this.GetPresetName() && soCustomComparison.GetType() == base.GetType();
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x001E8690 File Offset: 0x001E6890
	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		hashCode.Add<Type>(base.GetType());
		hashCode.Add<string>(this.GetPresetName());
		return hashCode.ToHashCode();
	}

	// Token: 0x060025D9 RID: 9689 RVA: 0x001E86C6 File Offset: 0x001E68C6
	public static bool operator ==(SoCustomComparison c1, SoCustomComparison c2)
	{
		if (c1 == null)
		{
			return c2 == null;
		}
		return c1.Equals(c2);
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x001E86D7 File Offset: 0x001E68D7
	public static bool operator !=(SoCustomComparison c1, SoCustomComparison c2)
	{
		return !(c1 == c2);
	}

	// Token: 0x04003D59 RID: 15705
	[Tooltip("We need a internal reference to ID the file based on name (can't access name outside main threads)")]
	public string presetName;
}
