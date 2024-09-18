using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003A1 RID: 929
public class ApartmentNumberController : MonoBehaviour
{
	// Token: 0x06001523 RID: 5411 RVA: 0x001342E8 File Offset: 0x001324E8
	public void Setup(NewNode behindNode, NewDoor door)
	{
		if (!(behindNode.gameLocation.thisAsAddress != null) || !(behindNode.gameLocation.thisAsAddress.residence != null) || behindNode.gameLocation.thisAsAddress.floor.floor < 0)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		string residenceString = behindNode.gameLocation.thisAsAddress.residence.GetResidenceString();
		for (int i = 0; i < residenceString.Length; i++)
		{
			int num = 0;
			if (int.TryParse(residenceString.get_Chars(i).ToString(), ref num))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.numberPrefabs[num], base.transform);
				this.spawned.Add(gameObject.transform);
			}
		}
		float num2 = 0.2f * ((float)(this.spawned.Count - 1) / 2f);
		for (int j = 0; j < this.spawned.Count; j++)
		{
			this.spawned[j].localPosition = new Vector3(num2, 0f, 0f);
			num2 -= 0.2f;
		}
		if (door.transform.localScale.x < 0f)
		{
			base.gameObject.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
			return;
		}
		base.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
	}

	// Token: 0x04001A17 RID: 6679
	public List<GameObject> numberPrefabs = new List<GameObject>();

	// Token: 0x04001A18 RID: 6680
	public List<Transform> spawned = new List<Transform>();
}
