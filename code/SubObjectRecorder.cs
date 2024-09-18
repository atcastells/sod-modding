using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class SubObjectRecorder : MonoBehaviour
{
	// Token: 0x06000BE1 RID: 3041 RVA: 0x000AB2A8 File Offset: 0x000A94A8
	[Button(null, 0)]
	public void RecordSubObjectPlacements()
	{
		new List<FurniturePreset.SubObject>();
		this.furniturePreset.subObjects.Clear();
		for (int i = 0; i < base.gameObject.transform.childCount; i++)
		{
			Transform child = base.gameObject.transform.GetChild(i);
			SubObjectPlacement component = child.GetComponent<SubObjectPlacement>();
			if (component != null)
			{
				FurniturePreset.SubObject subObject = new FurniturePreset.SubObject();
				subObject.preset = component.preset;
				subObject.localPos = component.transform.localPosition;
				subObject.localRot = component.transform.localEulerAngles;
				subObject.belongsTo = component.belongsTo;
				subObject.security = component.security;
				this.furniturePreset.subObjects.Add(subObject);
			}
			for (int j = 0; j < child.childCount; j++)
			{
				Transform child2 = child.GetChild(j);
				SubObjectPlacement component2 = child2.GetComponent<SubObjectPlacement>();
				if (component2 != null)
				{
					FurniturePreset.SubObject subObject2 = new FurniturePreset.SubObject();
					subObject2.preset = component2.preset;
					subObject2.localPos = component2.transform.localPosition;
					subObject2.localRot = component2.transform.localEulerAngles;
					subObject2.parent = child2.transform.parent.name;
					subObject2.belongsTo = component2.belongsTo;
					subObject2.security = component2.security;
					this.furniturePreset.subObjects.Add(subObject2);
				}
				for (int k = 0; k < child2.childCount; k++)
				{
					Transform child3 = child2.GetChild(k);
					SubObjectPlacement component3 = child3.GetComponent<SubObjectPlacement>();
					if (component3 != null)
					{
						FurniturePreset.SubObject subObject3 = new FurniturePreset.SubObject();
						subObject3.preset = component3.preset;
						subObject3.localPos = component3.transform.localPosition;
						subObject3.localRot = component3.transform.localEulerAngles;
						subObject3.parent = child3.transform.parent.name;
						subObject3.belongsTo = component3.belongsTo;
						subObject3.security = component3.security;
						this.furniturePreset.subObjects.Add(subObject3);
					}
				}
			}
		}
	}

	// Token: 0x04000D42 RID: 3394
	public FurniturePreset furniturePreset;
}
