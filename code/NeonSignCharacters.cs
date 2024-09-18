using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200079E RID: 1950
[CreateAssetMenu(fileName = "neonsign_data", menuName = "Database/Neon Sign Characters")]
public class NeonSignCharacters : SoCustomComparison
{
	// Token: 0x04003B14 RID: 15124
	public List<NeonSignCharacters.NeonCharacter> characterList = new List<NeonSignCharacters.NeonCharacter>();

	// Token: 0x0200079F RID: 1951
	[Serializable]
	public class NeonCharacter
	{
		// Token: 0x04003B15 RID: 15125
		public string character;

		// Token: 0x04003B16 RID: 15126
		public GameObject prefab;
	}
}
