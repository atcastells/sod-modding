using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000431 RID: 1073
public class StreetCrimeSceneController : MonoBehaviour
{
	// Token: 0x06001830 RID: 6192 RVA: 0x00169B54 File Offset: 0x00167D54
	private void Start()
	{
		int num = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.castAllExcept, new int[]
		{
			2,
			18,
			30,
			31,
			24
		});
		foreach (GameObject gameObject in this.elements)
		{
			Vector3 vector = gameObject.transform.position + new Vector3(0f, 1f, 0f);
			Vector3 vector2 = vector - this.middle.position;
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(this.middle.position, vector2), ref raycastHit, Vector3.Distance(vector, this.middle.position), num, 2) && raycastHit.collider.gameObject != gameObject)
			{
				Object.Destroy(gameObject);
			}
			if (gameObject != null)
			{
				gameObject.GetComponent<MeshRenderer>().enabled = true;
			}
		}
	}

	// Token: 0x04001E19 RID: 7705
	[Header("Components")]
	public Transform middle;

	// Token: 0x04001E1A RID: 7706
	public List<GameObject> elements = new List<GameObject>();
}
